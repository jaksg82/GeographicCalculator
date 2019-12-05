using Datums;
using MathExt;
using GeographicCalculatorWPFCore.Strings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace GeographicCalculatorWPFCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IStrings UIStrings; // Localization strings for the app
        private readonly DatumEnumStrings enumStrings; // Localization strings for the linked Datum enums
        private Projections InputCrs, OutputCrs, TempCrsIn, TempCrsOut;
        private readonly CultureInfo InvCult = CultureInfo.InvariantCulture;
        private Point3D UserPointGeo, UserPointPrj, UserPointWgs, OutPointGeo, OutPointPrj;
        private StringFormat.Formattings.DmsFormat GeoFormat;
        private StringFormat.Formattings.DmsSign GeoSign;
        private StringFormat.Formattings.MetricSign PrjSign;
        private int GeoDecimals, PrjDecimals;

        public MainWindow()
        {
            InitializeComponent();
            UIStrings = new StringProvider(Thread.CurrentThread.CurrentUICulture);
            enumStrings = new DatumEnumStrings();
            Ellipsoid newWgs84 = new Ellipsoid { ToWgs84 = new None() };
            InputCrs = new TransverseMercatorUniversal(newWgs84, "UTM 32 North on WGS 84", "UTM32N-WGS84", 32, true);
            OutputCrs = new TransverseMercatorUniversal(newWgs84, "UTM 33 North on WGS 84", "UTM33N-WGS84", 33, true);
            TempCrsIn = InputCrs;
            TempCrsOut = OutputCrs;
            UserPointGeo = new Point3D(Angles.DegRad(9), Angles.DegRad(45), 0);
            UserPointPrj = new Point3D(0, 0, 0);
            UserPointWgs = new Point3D(0, 0, 0);
            OutPointGeo = new Point3D(0, 0, 0);
            OutPointPrj = new Point3D(0, 0, 0);
            GeoFormat = StringFormat.Formattings.DmsFormat.VerboseDMS;
            GeoSign = StringFormat.Formattings.DmsSign.Suffix;
            PrjSign = StringFormat.Formattings.MetricSign.Suffix;
            GeoDecimals = 3;
            PrjDecimals = 3;
            // Apply the language
            ChangeLanguage();
            UpdateSettingsPanel();
            PanelSettings.Visibility = Visibility.Collapsed;
            UpdateInputCrsPanel();
            PanelCrsInput.Visibility = Visibility.Collapsed;
            // Finally compute all the coordinates
            UpdatePoints(true);
        }

        #region Coordinates update

        /// <summary>
        /// Get the new values from the geo fields
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Arguments</param>
        private void TxtInGeoValue_LostFocus(object sender, RoutedEventArgs e)
        {
            Point3D ParsedPoint = new Point3D();
            ParsedPoint.X = StringFormat.Formattings.DmsParse(TxtInGeoLonValue.Text, GeoFormat, GeoSign);
            ParsedPoint.Y = StringFormat.Formattings.DmsParse(TxtInGeoLatValue.Text, GeoFormat, GeoSign);
            ParsedPoint.Z = double.Parse(TxtInGeoElevValue.Text, InvCult);
            if (double.IsNaN(ParsedPoint.X)) { ParsedPoint.X = UserPointGeo.X; }
            if (double.IsNaN(ParsedPoint.Y)) { ParsedPoint.Y = UserPointGeo.Y; }
            if (double.IsNaN(ParsedPoint.Z)) { ParsedPoint.Z = UserPointGeo.Z; }
            UserPointGeo = ParsedPoint;
            UpdatePoints(true);
        }

        /// <summary>
        /// Get the new values from the prj fields
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Arguments</param>
        private void TxtInPrjValue_LostFocus(object sender, RoutedEventArgs e)
        {
            Point3D ParsedPoint = new Point3D();
            ParsedPoint.X = StringFormat.Formattings.MetricParse(TxtInPrjEastValue.Text, PrjSign);
            ParsedPoint.Y = StringFormat.Formattings.MetricParse(TxtInPrjNorthValue.Text, PrjSign);
            ParsedPoint.Z = double.Parse(TxtInPrjElevValue.Text, InvCult);
            if (double.IsNaN(ParsedPoint.X)) { ParsedPoint.X = UserPointGeo.X; }
            if (double.IsNaN(ParsedPoint.Y)) { ParsedPoint.Y = UserPointGeo.Y; }
            if (double.IsNaN(ParsedPoint.Z)) { ParsedPoint.Z = UserPointGeo.Z; }
            UserPointPrj = ParsedPoint;
            UpdatePoints(false);
        }

        /// <summary>
        /// Recompute the prj & geo points
        /// </summary>
        /// <param name="IsGeoChanged"></param>
        private void UpdatePoints(bool IsGeoChanged)
        {
            if (IsGeoChanged) // Start from geo point
            {
                UserPointPrj = InputCrs.FromGeographic(UserPointGeo);
            }
            else // Start from prj point
            {
                UserPointGeo = InputCrs.ToGeographic(UserPointPrj);
            }
            UserPointWgs = InputCrs.BaseEllipsoid.ToWgs84.ToWGS84(UserPointGeo);
            OutPointGeo = OutputCrs.BaseEllipsoid.ToWgs84.FromWGS84(UserPointWgs);
            OutPointPrj = OutputCrs.FromGeographic(OutPointGeo);
            UpdatePointCoordinates();
        }

        #endregion Coordinates update

        #region Panel Settings

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            if (PanelCrsInput.Visibility == Visibility.Visible)
            {
                PanelCrsInput.Visibility = Visibility.Collapsed;
                TempCrsIn = InputCrs;
            }
            if (PanelSettings.Visibility == Visibility.Visible)
            { PanelSettings.Visibility = Visibility.Collapsed; }
            else
            { PanelSettings.Visibility = Visibility.Visible; }
        }

        private void CmbSetGeoFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSetGeoFormat.SelectedIndex >= 0)
            { GeoFormat = (StringFormat.Formattings.DmsFormat)CmbSetGeoFormat.SelectedIndex; }
            else
            { CmbSetGeoFormat.SelectedIndex = (int)GeoFormat; }
            UpdatePointCoordinates();
        }

        private void CmbSetGeoSign_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSetGeoSign.SelectedIndex >= 0)
            { GeoSign = (StringFormat.Formattings.DmsSign)CmbSetGeoSign.SelectedIndex; }
            else
            { CmbSetGeoSign.SelectedIndex = (int)GeoSign; }
            UpdatePointCoordinates();
        }

        private void CmbSetGeoDecimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSetGeoDecimals.SelectedIndex >= 0)
            { GeoDecimals = CmbSetGeoDecimals.SelectedIndex; }
            else
            { CmbSetGeoDecimals.SelectedIndex = GeoDecimals; }
            UpdatePointCoordinates();
        }

        private void CmbSetPrjFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSetPrjFormat.SelectedIndex >= 0)
            { PrjSign = (StringFormat.Formattings.MetricSign)CmbSetPrjFormat.SelectedIndex; }
            else
            { CmbSetPrjFormat.SelectedIndex = (int)PrjSign; }
            UpdatePointCoordinates();
        }

        private void CmbSetPrjDecimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSetPrjDecimals.SelectedIndex >= 0)
            { PrjDecimals = CmbSetPrjDecimals.SelectedIndex; }
            else
            { CmbSetPrjDecimals.SelectedIndex = PrjDecimals; }
            UpdatePointCoordinates();
        }

        #endregion Panel Settings

        #region Panel Crs Input

        private void BtnInDatum_Click(object sender, RoutedEventArgs e)
        {
            if (PanelSettings.Visibility == Visibility.Visible)
            { PanelSettings.Visibility = Visibility.Collapsed; }
            if (PanelCrsInput.Visibility == Visibility.Visible)
            {
                PanelCrsInput.Visibility = Visibility.Collapsed;
                TempCrsIn = InputCrs;
            }
            else
            { PanelCrsInput.Visibility = Visibility.Visible; }
        }

        private void BtnApplyCrsInput_Click(object sender, RoutedEventArgs e)
        {
            // Collect the info and update the input CRS
            RetrieveInputCrsProjInfos();
            RetrieveInputCrsTransParams();
            InputCrs = TempCrsIn;
            PanelCrsInput.Visibility = Visibility.Collapsed;
        }

        private void BtnSetCrsInPrj_Click(object sender, RoutedEventArgs e)
        {
            string btnClose = "\U0001F53A;";
            string btnOpen = "\U0001F53B;";
            if (sender.GetType() == typeof(Button))
            {
                Button btnSender = (Button)sender;
                GrdSetCrsInPrj.Visibility = Visibility.Collapsed;
                BtnSetCrsInPrj2.Content = btnClose;
                GrdSetCrsInEll.Visibility = Visibility.Collapsed;
                BtnSetCrsInEll2.Content = btnClose;
                GrdSetCrsInTrans.Visibility = Visibility.Collapsed;
                BtnSetCrsInTrans2.Content = btnClose;

                switch (btnSender.Name)
                {
                    case "BtnSetCrsInPrj1":
                    case "BtnSetCrsInPrj2":
                        if (GrdSetCrsInPrj.Visibility == Visibility.Collapsed)
                        {
                            GrdSetCrsInPrj.Visibility = Visibility.Visible;
                            BtnSetCrsInPrj2.Content = btnOpen;
                            GrdSetCrsInEll.Visibility = Visibility.Collapsed;
                            BtnSetCrsInEll2.Content = btnClose;
                            GrdSetCrsInTrans.Visibility = Visibility.Collapsed;
                            BtnSetCrsInTrans2.Content = btnClose;
                        }
                        break;

                    case "BtnSetCrsInEll1":
                    case "BtnSetCrsInEll2":
                        if (GrdSetCrsInEll.Visibility == Visibility.Collapsed)
                        {
                            GrdSetCrsInPrj.Visibility = Visibility.Collapsed;
                            BtnSetCrsInPrj2.Content = btnClose;
                            GrdSetCrsInEll.Visibility = Visibility.Visible;
                            BtnSetCrsInEll2.Content = btnOpen;
                            GrdSetCrsInTrans.Visibility = Visibility.Collapsed;
                            BtnSetCrsInTrans2.Content = btnClose;
                        }
                        break;

                    case "BtnSetCrsInTrans1":
                    case "BtnSetCrsInTrans2":
                        if (GrdSetCrsInEll.Visibility == Visibility.Collapsed)
                        {
                            GrdSetCrsInPrj.Visibility = Visibility.Collapsed;
                            BtnSetCrsInPrj2.Content = btnClose;
                            GrdSetCrsInEll.Visibility = Visibility.Collapsed;
                            BtnSetCrsInEll2.Content = btnClose;
                            GrdSetCrsInTrans.Visibility = Visibility.Visible;
                            BtnSetCrsInTrans2.Content = btnOpen;
                        }
                        break;
                }
            }
        }

        private void CmbSetCrsInPrjMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Projections.Method TempMethod;
            //List<double> DummyParams = new List<double> { };

            if (CmbSetCrsInPrjMethod.SelectedIndex >= 0)
            {
                //TempMethod = (Projections.Method)CmbSetCrsInPrjMethod.SelectedIndex;
                //List<ParamNameValue> oldParams = TempCrsIn.GetParams();
                //for (int p = 0; p < Projections.GetParamCount(TempMethod); p++)
                //{
                //    if (oldParams.Count > p)
                //    { DummyParams.Add(oldParams[p].Value); }
                //    else
                //    { DummyParams.Add(p); }
                //}
                //TempCrsIn = Projections.FromParams(TempMethod, TempCrsIn.BaseEllipsoid, TempCrsIn.FullName, TempCrsIn.ShortName, DummyParams);
                RetrieveInputCrsProjInfos();
                UpdateInputProjectionParams(TempCrsIn);
            }
            else
            { CmbSetCrsInPrjMethod.SelectedIndex = (int)TempCrsIn.Type; }
        }

        private void CmbSetCrsInEllEpsgId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Ellipsoid> availEll = new EpsgEllipsoids().KnownEllipsoids;
            Ellipsoid TempEll = availEll[CmbSetCrsInEllEpsgId.SelectedIndex];
            if (TempEll != null)
            {
                TempEll.ToWgs84 = TempCrsIn.BaseEllipsoid.ToWgs84;
                TempCrsIn.BaseEllipsoid = TempEll;
                UpdateInputCrsEllipsoidInfos();
            }
            else
            {
                int selEll = 0;
                for (int id = 0; id < availEll.Count; id++)
                {
                    if (TempCrsIn.BaseEllipsoid.EpsgId == availEll[id].EpsgId)
                    {
                        selEll = id;
                        break;
                    }
                }
                CmbSetCrsInEllEpsgId.SelectedIndex = selEll;
                UpdateInputCrsEllipsoidInfos();
            }
        }

        private void CmbSetCrsInTransMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Transformations.Methods tmpMethod = (Transformations.Methods)CmbSetCrsInTransMethod.SelectedIndex;
            List<ParamNameValue> param = TempCrsIn.BaseEllipsoid.ToWgs84.GetParams();

            TxbSetCrsInTransDxTitle.Visibility = Visibility.Collapsed;
            TxtSetCrsInTransDx.Visibility = Visibility.Collapsed;
            TxbSetCrsInTransDyTitle.Visibility = Visibility.Collapsed;
            TxtSetCrsInTransDy.Visibility = Visibility.Collapsed;
            TxbSetCrsInTransDzTitle.Visibility = Visibility.Collapsed;
            TxtSetCrsInTransDz.Visibility = Visibility.Collapsed;
            TxbSetCrsInTransRxTitle.Visibility = Visibility.Collapsed;
            TxtSetCrsInTransRx.Visibility = Visibility.Collapsed;
            TxbSetCrsInTransRyTitle.Visibility = Visibility.Collapsed;
            TxtSetCrsInTransRy.Visibility = Visibility.Collapsed;
            TxbSetCrsInTransRzTitle.Visibility = Visibility.Collapsed;
            TxtSetCrsInTransRz.Visibility = Visibility.Collapsed;
            TxbSetCrsInTransScaleTitle.Visibility = Visibility.Collapsed;
            TxtSetCrsInTransScale.Visibility = Visibility.Collapsed;
            TxbSetCrsInTransRotConvTitle.Visibility = Visibility.Collapsed;
            ChkSetCrsInTransRotConv1.Visibility = Visibility.Collapsed;
            ChkSetCrsInTransRotConv2.Visibility = Visibility.Collapsed;
            TxbSetCrsInTransPxTitle.Visibility = Visibility.Collapsed;
            TxtSetCrsInTransPx.Visibility = Visibility.Collapsed;
            TxbSetCrsInTransPyTitle.Visibility = Visibility.Collapsed;
            TxtSetCrsInTransPy.Visibility = Visibility.Collapsed;
            TxbSetCrsInTransPzTitle.Visibility = Visibility.Collapsed;
            TxtSetCrsInTransPz.Visibility = Visibility.Collapsed;

            switch (tmpMethod)
            {
                case Transformations.Methods.None:
                    RetrieveInputCrsTransParams();
                    break;

                case Transformations.Methods.Geocentric3Parameter:
                case Transformations.Methods.AbridgedMolodensky:
                    TxbSetCrsInTransDxTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransDx.Visibility = Visibility.Visible;
                    TxbSetCrsInTransDyTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransDy.Visibility = Visibility.Visible;
                    TxbSetCrsInTransDzTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransDz.Visibility = Visibility.Visible;
                    if (param.Count > 3)
                    {
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(param[0].Value, PrjDecimals);
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(param[1].Value, PrjDecimals);
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(param[2].Value, PrjDecimals);
                    }
                    else
                    {
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                    }
                    RetrieveInputCrsTransParams();
                    break;

                case Transformations.Methods.Helmert7Parameter:
                    TxbSetCrsInTransDxTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransDx.Visibility = Visibility.Visible;
                    TxbSetCrsInTransDyTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransDy.Visibility = Visibility.Visible;
                    TxbSetCrsInTransDzTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransDz.Visibility = Visibility.Visible;
                    TxbSetCrsInTransRxTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransRx.Visibility = Visibility.Visible;
                    TxbSetCrsInTransRyTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransRy.Visibility = Visibility.Visible;
                    TxbSetCrsInTransRzTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransRz.Visibility = Visibility.Visible;
                    TxbSetCrsInTransScaleTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransScale.Visibility = Visibility.Visible;
                    TxbSetCrsInTransRotConvTitle.Visibility = Visibility.Visible;
                    ChkSetCrsInTransRotConv1.Visibility = Visibility.Visible;
                    ChkSetCrsInTransRotConv2.Visibility = Visibility.Visible;
                    if (param.Count > 3)
                    {
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(param[0].Value, PrjDecimals);
                        TxtSetCrsInTransDy.Text = StringFormat.Formattings.FormatNumber(param[1].Value, PrjDecimals);
                        TxtSetCrsInTransDz.Text = StringFormat.Formattings.FormatNumber(param[2].Value, PrjDecimals);
                        if (param.Count > 7)
                        {
                            TxtSetCrsInTransRx.Text = StringFormat.Formattings.FormatNumber(param[3].Value, 9);
                            TxtSetCrsInTransRy.Text = StringFormat.Formattings.FormatNumber(param[4].Value, 9);
                            TxtSetCrsInTransRz.Text = StringFormat.Formattings.FormatNumber(param[5].Value, 9);
                            ChkSetCrsInTransRotConv1.IsChecked = (param[6].Value == 0);
                            TxtSetCrsInTransScale.Text = StringFormat.Formattings.FormatNumber(param[7].Value, 9);
                        }
                        else
                        {
                            TxtSetCrsInTransRx.Text = StringFormat.Formattings.FormatNumber(0, 9);
                            TxtSetCrsInTransRy.Text = StringFormat.Formattings.FormatNumber(0, 9);
                            TxtSetCrsInTransRz.Text = StringFormat.Formattings.FormatNumber(0, 9);
                            ChkSetCrsInTransRotConv1.IsChecked = true;
                            TxtSetCrsInTransScale.Text = StringFormat.Formattings.FormatNumber(0, 9);
                        }
                    }
                    else
                    {
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                        TxtSetCrsInTransRx.Text = StringFormat.Formattings.FormatNumber(0, 9);
                        TxtSetCrsInTransRy.Text = StringFormat.Formattings.FormatNumber(0, 9);
                        TxtSetCrsInTransRz.Text = StringFormat.Formattings.FormatNumber(0, 9);
                        ChkSetCrsInTransRotConv1.IsChecked = true;
                        TxtSetCrsInTransScale.Text = StringFormat.Formattings.FormatNumber(0, 9);
                    }
                    RetrieveInputCrsTransParams();
                    break;

                case Transformations.Methods.MolodenskyBadekas10Parameter:
                    TxbSetCrsInTransDxTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransDx.Visibility = Visibility.Visible;
                    TxbSetCrsInTransDyTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransDy.Visibility = Visibility.Visible;
                    TxbSetCrsInTransDzTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransDz.Visibility = Visibility.Visible;
                    TxbSetCrsInTransRxTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransRx.Visibility = Visibility.Visible;
                    TxbSetCrsInTransRyTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransRy.Visibility = Visibility.Visible;
                    TxbSetCrsInTransRzTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransRz.Visibility = Visibility.Visible;
                    TxbSetCrsInTransScaleTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransScale.Visibility = Visibility.Visible;
                    TxbSetCrsInTransRotConvTitle.Visibility = Visibility.Visible;
                    ChkSetCrsInTransRotConv1.Visibility = Visibility.Visible;
                    ChkSetCrsInTransRotConv2.Visibility = Visibility.Visible;
                    TxbSetCrsInTransPxTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransPx.Visibility = Visibility.Visible;
                    TxbSetCrsInTransPyTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransPy.Visibility = Visibility.Visible;
                    TxbSetCrsInTransPzTitle.Visibility = Visibility.Visible;
                    TxtSetCrsInTransPz.Visibility = Visibility.Visible;
                    if (param.Count >= 3)
                    {
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(param[0].Value, PrjDecimals);
                        TxtSetCrsInTransDy.Text = StringFormat.Formattings.FormatNumber(param[1].Value, PrjDecimals);
                        TxtSetCrsInTransDz.Text = StringFormat.Formattings.FormatNumber(param[2].Value, PrjDecimals);
                        if (param.Count >= 7)
                        {
                            TxtSetCrsInTransRx.Text = StringFormat.Formattings.FormatNumber(param[3].Value, 9);
                            TxtSetCrsInTransRy.Text = StringFormat.Formattings.FormatNumber(param[4].Value, 9);
                            TxtSetCrsInTransRz.Text = StringFormat.Formattings.FormatNumber(param[5].Value, 9);
                            ChkSetCrsInTransRotConv1.IsChecked = (param[6].Value == 0);
                            TxtSetCrsInTransScale.Text = StringFormat.Formattings.FormatNumber(param[7].Value, 9);
                            if (param.Count >= 10)
                            {
                                TxtSetCrsInTransPx.Text = StringFormat.Formattings.FormatNumber(param[8].Value, PrjDecimals);
                                TxtSetCrsInTransPy.Text = StringFormat.Formattings.FormatNumber(param[9].Value, PrjDecimals);
                                TxtSetCrsInTransPz.Text = StringFormat.Formattings.FormatNumber(param[10].Value, PrjDecimals);
                            }
                            else
                            {
                                TxtSetCrsInTransPx.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                                TxtSetCrsInTransPy.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                                TxtSetCrsInTransPz.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                            }
                        }
                        else
                        {
                            TxtSetCrsInTransRx.Text = StringFormat.Formattings.FormatNumber(0, 9);
                            TxtSetCrsInTransRy.Text = StringFormat.Formattings.FormatNumber(0, 9);
                            TxtSetCrsInTransRz.Text = StringFormat.Formattings.FormatNumber(0, 9);
                            ChkSetCrsInTransRotConv1.IsChecked = true;
                            TxtSetCrsInTransScale.Text = StringFormat.Formattings.FormatNumber(0, 9);
                            TxtSetCrsInTransPx.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                            TxtSetCrsInTransPy.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                            TxtSetCrsInTransPz.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                        }
                    }
                    else
                    {
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                        TxtSetCrsInTransDx.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                        TxtSetCrsInTransRx.Text = StringFormat.Formattings.FormatNumber(0, 9);
                        TxtSetCrsInTransRy.Text = StringFormat.Formattings.FormatNumber(0, 9);
                        TxtSetCrsInTransRz.Text = StringFormat.Formattings.FormatNumber(0, 9);
                        ChkSetCrsInTransRotConv1.IsChecked = true;
                        TxtSetCrsInTransScale.Text = StringFormat.Formattings.FormatNumber(0, 9);
                        TxtSetCrsInTransPx.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                        TxtSetCrsInTransPy.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                        TxtSetCrsInTransPz.Text = StringFormat.Formattings.FormatNumber(0, PrjDecimals);
                    }
                    RetrieveInputCrsTransParams();
                    break;
            }
        }

        private void ChkSetCrsInTransRotConv_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() == typeof(CheckBox))
            {
                CheckBox chkSender = (CheckBox)sender;
                if (chkSender.Name == "ChkSetCrsInTransRotConv1")
                {
                    if (chkSender.IsChecked == true)
                    {
                        ChkSetCrsInTransRotConv2.IsChecked = false;
                        TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention = Transformations.RotationConventions.PositionVector;
                    }
                    else
                    {
                        ChkSetCrsInTransRotConv2.IsChecked = true;
                        TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention = Transformations.RotationConventions.CoordinateFrame;
                    }
                }
                else
                {
                    if (chkSender.IsChecked == true)
                    {
                        ChkSetCrsInTransRotConv1.IsChecked = false;
                        TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention = Transformations.RotationConventions.CoordinateFrame;
                    }
                    else
                    {
                        ChkSetCrsInTransRotConv1.IsChecked = true;
                        TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention = Transformations.RotationConventions.PositionVector;
                    }
                }
            }
            else
            {
                if (TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention == Transformations.RotationConventions.PositionVector)
                {
                    ChkSetCrsInTransRotConv1.IsChecked = true;
                    ChkSetCrsInTransRotConv2.IsChecked = false;
                }
                else
                {
                    ChkSetCrsInTransRotConv1.IsChecked = false;
                    ChkSetCrsInTransRotConv2.IsChecked = true;
                }
            }
        }

        #endregion Panel Crs Input
    }
}