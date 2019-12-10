using Datums;
using StringFormat;
using GeographicCalculatorWPFCore.Strings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeographicCalculatorWPFCore.Pages
{
    /// <summary>
    /// Logica di interazione per SetCrs.xaml
    /// </summary>
    public partial class SetCrs : Page
    {
        private Projections TempCrsIn;
        private readonly CoordSettings CoordCfg;
        private readonly IStrings UIStrings; // Localization strings for the app
        private readonly DatumEnumStrings enumStrings; // Localization strings for the linked Datum enums
        private readonly CultureInfo InvCult = CultureInfo.InvariantCulture;

        public SetCrs()
        {
            InitializeComponent();
            UIStrings = new StringProvider(Thread.CurrentThread.CurrentUICulture);
            enumStrings = new DatumEnumStrings();
            CoordCfg = new CoordSettings();
            UpdateInputCrsPanel();
        }

        #region User interaction events

        private void BtnCancelCrs_Click(object sender, RoutedEventArgs e)
        {
            // TODO Discard the changes and return to the main page
        }

        private void BtnApplyCrs_Click(object sender, RoutedEventArgs e)
        {
            // Collect the info and update the input CRS
            RetrieveInputCrsProjInfos();
            RetrieveInputCrsTransParams();
            // TODO Send the new CRS to the main window and load the main page
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

        private void CmbPrjMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbPrjMethod.SelectedIndex >= 0)
            {
                RetrieveInputCrsProjInfos();
                UpdateInputProjectionParams(TempCrsIn);
            }
            else
            { CmbPrjMethod.SelectedIndex = (int)TempCrsIn.Type; }
        }

        private void CmbEllEpsgId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Ellipsoid> availEll = new EpsgEllipsoids().KnownEllipsoids;
            Ellipsoid TempEll = availEll[CmbEllEpsgId.SelectedIndex];
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
                CmbEllEpsgId.SelectedIndex = selEll;
                UpdateInputCrsEllipsoidInfos();
            }
        }

        private void CmbTransMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Transformations.Methods tmpMethod = (Transformations.Methods)CmbTransMethod.SelectedIndex;
            List<ParamNameValue> param = TempCrsIn.BaseEllipsoid.ToWgs84.GetParams();

            TxbTransDxTitle.Visibility = Visibility.Collapsed;
            TxtTransDx.Visibility = Visibility.Collapsed;
            TxbTransDyTitle.Visibility = Visibility.Collapsed;
            TxtTransDy.Visibility = Visibility.Collapsed;
            TxbTransDzTitle.Visibility = Visibility.Collapsed;
            TxtTransDz.Visibility = Visibility.Collapsed;
            TxbTransRxTitle.Visibility = Visibility.Collapsed;
            TxtTransRx.Visibility = Visibility.Collapsed;
            TxbTransRyTitle.Visibility = Visibility.Collapsed;
            TxtTransRy.Visibility = Visibility.Collapsed;
            TxbTransRzTitle.Visibility = Visibility.Collapsed;
            TxtTransRz.Visibility = Visibility.Collapsed;
            TxbTransScaleTitle.Visibility = Visibility.Collapsed;
            TxtTransScale.Visibility = Visibility.Collapsed;
            TxbTransRotConvTitle.Visibility = Visibility.Collapsed;
            ChkTransRotConv1.Visibility = Visibility.Collapsed;
            ChkTransRotConv2.Visibility = Visibility.Collapsed;
            TxbTransPxTitle.Visibility = Visibility.Collapsed;
            TxtTransPx.Visibility = Visibility.Collapsed;
            TxbTransPyTitle.Visibility = Visibility.Collapsed;
            TxtTransPy.Visibility = Visibility.Collapsed;
            TxbTransPzTitle.Visibility = Visibility.Collapsed;
            TxtTransPz.Visibility = Visibility.Collapsed;

            switch (tmpMethod)
            {
                case Transformations.Methods.None:
                    RetrieveInputCrsTransParams();
                    break;

                case Transformations.Methods.Geocentric3Parameter:
                case Transformations.Methods.AbridgedMolodensky:
                    TxbTransDxTitle.Visibility = Visibility.Visible;
                    TxtTransDx.Visibility = Visibility.Visible;
                    TxbTransDyTitle.Visibility = Visibility.Visible;
                    TxtTransDy.Visibility = Visibility.Visible;
                    TxbTransDzTitle.Visibility = Visibility.Visible;
                    TxtTransDz.Visibility = Visibility.Visible;
                    if (param.Count > 3)
                    {
                        TxtTransDx.Text = Formattings.FormatNumber(param[0].Value, CoordCfg.MetricDecimals);
                        TxtTransDx.Text = Formattings.FormatNumber(param[1].Value, CoordCfg.MetricDecimals);
                        TxtTransDx.Text = Formattings.FormatNumber(param[2].Value, CoordCfg.MetricDecimals);
                    }
                    else
                    {
                        TxtTransDx.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                        TxtTransDx.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                        TxtTransDx.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                    }
                    RetrieveInputCrsTransParams();
                    break;

                case Transformations.Methods.Helmert7Parameter:
                    TxbTransDxTitle.Visibility = Visibility.Visible;
                    TxtTransDx.Visibility = Visibility.Visible;
                    TxbTransDyTitle.Visibility = Visibility.Visible;
                    TxtTransDy.Visibility = Visibility.Visible;
                    TxbTransDzTitle.Visibility = Visibility.Visible;
                    TxtTransDz.Visibility = Visibility.Visible;
                    TxbTransRxTitle.Visibility = Visibility.Visible;
                    TxtTransRx.Visibility = Visibility.Visible;
                    TxbTransRyTitle.Visibility = Visibility.Visible;
                    TxtTransRy.Visibility = Visibility.Visible;
                    TxbTransRzTitle.Visibility = Visibility.Visible;
                    TxtTransRz.Visibility = Visibility.Visible;
                    TxbTransScaleTitle.Visibility = Visibility.Visible;
                    TxtTransScale.Visibility = Visibility.Visible;
                    TxbTransRotConvTitle.Visibility = Visibility.Visible;
                    ChkTransRotConv1.Visibility = Visibility.Visible;
                    ChkTransRotConv2.Visibility = Visibility.Visible;
                    if (param.Count > 3)
                    {
                        TxtTransDx.Text = Formattings.FormatNumber(param[0].Value, CoordCfg.MetricDecimals);
                        TxtTransDy.Text = Formattings.FormatNumber(param[1].Value, CoordCfg.MetricDecimals);
                        TxtTransDz.Text = Formattings.FormatNumber(param[2].Value, CoordCfg.MetricDecimals);
                        if (param.Count > 7)
                        {
                            TxtTransRx.Text = Formattings.FormatNumber(param[3].Value, 9);
                            TxtTransRy.Text = Formattings.FormatNumber(param[4].Value, 9);
                            TxtTransRz.Text = Formattings.FormatNumber(param[5].Value, 9);
                            ChkTransRotConv1.IsChecked = (param[6].Value == 0);
                            TxtTransScale.Text = Formattings.FormatNumber(param[7].Value, 9);
                        }
                        else
                        {
                            TxtTransRx.Text = Formattings.FormatNumber(0, 9);
                            TxtTransRy.Text = Formattings.FormatNumber(0, 9);
                            TxtTransRz.Text = Formattings.FormatNumber(0, 9);
                            ChkTransRotConv1.IsChecked = true;
                            TxtTransScale.Text = Formattings.FormatNumber(0, 9);
                        }
                    }
                    else
                    {
                        TxtTransDx.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                        TxtTransDx.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                        TxtTransDx.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                        TxtTransRx.Text = Formattings.FormatNumber(0, 9);
                        TxtTransRy.Text = Formattings.FormatNumber(0, 9);
                        TxtTransRz.Text = Formattings.FormatNumber(0, 9);
                        ChkTransRotConv1.IsChecked = true;
                        TxtTransScale.Text = Formattings.FormatNumber(0, 9);
                    }
                    RetrieveInputCrsTransParams();
                    break;

                case Transformations.Methods.MolodenskyBadekas10Parameter:
                    TxbTransDxTitle.Visibility = Visibility.Visible;
                    TxtTransDx.Visibility = Visibility.Visible;
                    TxbTransDyTitle.Visibility = Visibility.Visible;
                    TxtTransDy.Visibility = Visibility.Visible;
                    TxbTransDzTitle.Visibility = Visibility.Visible;
                    TxtTransDz.Visibility = Visibility.Visible;
                    TxbTransRxTitle.Visibility = Visibility.Visible;
                    TxtTransRx.Visibility = Visibility.Visible;
                    TxbTransRyTitle.Visibility = Visibility.Visible;
                    TxtTransRy.Visibility = Visibility.Visible;
                    TxbTransRzTitle.Visibility = Visibility.Visible;
                    TxtTransRz.Visibility = Visibility.Visible;
                    TxbTransScaleTitle.Visibility = Visibility.Visible;
                    TxtTransScale.Visibility = Visibility.Visible;
                    TxbTransRotConvTitle.Visibility = Visibility.Visible;
                    ChkTransRotConv1.Visibility = Visibility.Visible;
                    ChkTransRotConv2.Visibility = Visibility.Visible;
                    TxbTransPxTitle.Visibility = Visibility.Visible;
                    TxtTransPx.Visibility = Visibility.Visible;
                    TxbTransPyTitle.Visibility = Visibility.Visible;
                    TxtTransPy.Visibility = Visibility.Visible;
                    TxbTransPzTitle.Visibility = Visibility.Visible;
                    TxtTransPz.Visibility = Visibility.Visible;
                    if (param.Count >= 3)
                    {
                        TxtTransDx.Text = Formattings.FormatNumber(param[0].Value, CoordCfg.MetricDecimals);
                        TxtTransDy.Text = Formattings.FormatNumber(param[1].Value, CoordCfg.MetricDecimals);
                        TxtTransDz.Text = Formattings.FormatNumber(param[2].Value, CoordCfg.MetricDecimals);
                        if (param.Count >= 7)
                        {
                            TxtTransRx.Text = Formattings.FormatNumber(param[3].Value, 9);
                            TxtTransRy.Text = Formattings.FormatNumber(param[4].Value, 9);
                            TxtTransRz.Text = Formattings.FormatNumber(param[5].Value, 9);
                            ChkTransRotConv1.IsChecked = (param[6].Value == 0);
                            TxtTransScale.Text = Formattings.FormatNumber(param[7].Value, 9);
                            if (param.Count >= 10)
                            {
                                TxtTransPx.Text = Formattings.FormatNumber(param[8].Value, CoordCfg.MetricDecimals);
                                TxtTransPy.Text = Formattings.FormatNumber(param[9].Value, CoordCfg.MetricDecimals);
                                TxtTransPz.Text = Formattings.FormatNumber(param[10].Value, CoordCfg.MetricDecimals);
                            }
                            else
                            {
                                TxtTransPx.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                                TxtTransPy.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                                TxtTransPz.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                            }
                        }
                        else
                        {
                            TxtTransRx.Text = Formattings.FormatNumber(0, 9);
                            TxtTransRy.Text = Formattings.FormatNumber(0, 9);
                            TxtTransRz.Text = Formattings.FormatNumber(0, 9);
                            ChkTransRotConv1.IsChecked = true;
                            TxtTransScale.Text = Formattings.FormatNumber(0, 9);
                            TxtTransPx.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                            TxtTransPy.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                            TxtTransPz.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                        }
                    }
                    else
                    {
                        TxtTransDx.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                        TxtTransDx.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                        TxtTransDx.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                        TxtTransRx.Text = Formattings.FormatNumber(0, 9);
                        TxtTransRy.Text = Formattings.FormatNumber(0, 9);
                        TxtTransRz.Text = Formattings.FormatNumber(0, 9);
                        ChkTransRotConv1.IsChecked = true;
                        TxtTransScale.Text = Formattings.FormatNumber(0, 9);
                        TxtTransPx.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                        TxtTransPy.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                        TxtTransPz.Text = Formattings.FormatNumber(0, CoordCfg.MetricDecimals);
                    }
                    RetrieveInputCrsTransParams();
                    break;
            }
        }

        private void ChkTransRotConv_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() == typeof(CheckBox))
            {
                CheckBox chkSender = (CheckBox)sender;
                if (chkSender.Name == "ChkTransRotConv1")
                {
                    if (chkSender.IsChecked == true)
                    {
                        ChkTransRotConv2.IsChecked = false;
                        TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention = Transformations.RotationConventions.PositionVector;
                    }
                    else
                    {
                        ChkTransRotConv2.IsChecked = true;
                        TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention = Transformations.RotationConventions.CoordinateFrame;
                    }
                }
                else
                {
                    if (chkSender.IsChecked == true)
                    {
                        ChkTransRotConv1.IsChecked = false;
                        TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention = Transformations.RotationConventions.CoordinateFrame;
                    }
                    else
                    {
                        ChkTransRotConv1.IsChecked = true;
                        TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention = Transformations.RotationConventions.PositionVector;
                    }
                }
            }
            else
            {
                if (TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention == Transformations.RotationConventions.PositionVector)
                {
                    ChkTransRotConv1.IsChecked = true;
                    ChkTransRotConv2.IsChecked = false;
                }
                else
                {
                    ChkTransRotConv1.IsChecked = false;
                    ChkTransRotConv2.IsChecked = true;
                }
            }
        }

        #endregion User interaction events

        #region Fields Updates

        /// <summary>
        /// Populate the fields in the Input CRS panel
        /// </summary>
        private void UpdateInputCrsPanel()
        {
            TxbFullNameTitle.Text = UIStrings.SetCrsPrjFullName;
            TxtFullName.Text = TempCrsIn.FullName;
            TxbShortNameTitle.Text = UIStrings.SetCrsPrjShortName;
            TxtShortName.Text = TempCrsIn.ShortName;
            TxbPrjMethodTitle.Text = UIStrings.SetCrsPrjMethod;
            PopulateInputCrsProjMethods();
            CmbPrjMethod.SelectedIndex = (int)TempCrsIn.Type;
            PopulateInputCrsEllipsoids();
            PopulateInputCrsTransformations();
            BtnSetCrsInPrj_Click(BtnSetCrsInPrj1, new RoutedEventArgs());
        }

        private void PopulateInputCrsProjMethods()
        {
            CmbPrjMethod.Items.Clear();
            for (int p = 0; p < 50; p++)
            {
                if (enumStrings.LocalizedProjectionType((Projections.Method)p) != UIStrings.NotDefined)
                { CmbPrjMethod.Items.Add(enumStrings.LocalizedProjectionType((Projections.Method)p)); }
                else
                { break; }
            }
        }

        /// <summary>
        /// Change the visibility of the fields for the projection parameters
        /// </summary>
        /// <param name="proj">Selected Projection</param>
        private void UpdateInputProjectionParams(Projections proj)
        {
            if (proj != null)
            {
                TxbPrjParam1Title.Visibility = Visibility.Collapsed;
                TxbPrjParam2Title.Visibility = Visibility.Collapsed;
                TxbPrjParam3Title.Visibility = Visibility.Collapsed;
                TxbPrjParam4Title.Visibility = Visibility.Collapsed;
                TxbPrjParam5Title.Visibility = Visibility.Collapsed;
                TxbPrjParam6Title.Visibility = Visibility.Collapsed;
                TxbPrjParam7Title.Visibility = Visibility.Collapsed;
                TxbPrjParam8Title.Visibility = Visibility.Collapsed;
                TxtPrjParam1.Visibility = Visibility.Collapsed;
                TxtPrjParam2.Visibility = Visibility.Collapsed;
                TxtPrjParam3.Visibility = Visibility.Collapsed;
                TxtPrjParam4.Visibility = Visibility.Collapsed;
                TxtPrjParam5.Visibility = Visibility.Collapsed;
                TxtPrjParam6.Visibility = Visibility.Collapsed;
                TxtPrjParam7.Visibility = Visibility.Collapsed;
                ChkPrjParam8A.Visibility = Visibility.Collapsed;
                ChkPrjParam8B.Visibility = Visibility.Collapsed;

                List<ParamNameValue> ParamList = proj.GetParams();
                if (ParamList.Count >= 1) // Set the first param //
                { SetProjParamFormat(TxbPrjParam1Title, TxtPrjParam1, ParamList[0]); }
                if (ParamList.Count >= 2) // Set the second param //
                { SetProjParamFormat(TxbPrjParam2Title, TxtPrjParam2, ParamList[1]); }
                if (ParamList.Count >= 3) // Set the third param //
                { SetProjParamFormat(TxbPrjParam3Title, TxtPrjParam3, ParamList[2]); }
                if (ParamList.Count >= 4) // Set the fourth param //
                { SetProjParamFormat(TxbPrjParam4Title, TxtPrjParam4, ParamList[3]); }
                if (ParamList.Count >= 5) // Set the fifth param //
                { SetProjParamFormat(TxbPrjParam5Title, TxtPrjParam5, ParamList[4]); }
                if (ParamList.Count >= 6) // Set the sixth param //
                { SetProjParamFormat(TxbPrjParam6Title, TxtPrjParam6, ParamList[5]); }
                if (ParamList.Count >= 7) // Set the seventh param //
                { SetProjParamFormat(TxbPrjParam7Title, TxtPrjParam7, ParamList[6]); }
            }
        }

        /// <summary>
        /// Set the selected param fields
        /// </summary>
        /// <param name="txbTitle">TextBlock for the param name</param>
        /// <param name="txtValue">Textbox for the param value</param>
        /// <param name="param">Parameter to apply</param>
        private void SetProjParamFormat(TextBlock txbTitle, TextBox txtValue, ParamNameValue param)
        {
            // Set the first param //
            txbTitle.Visibility = Visibility.Visible;
            txbTitle.Text = param.Name;
            txtValue.Visibility = Visibility.Visible;
            switch (param.Type)
            {
                case ParamType.EastNorth:
                    txtValue.Text = Formattings.FormatNumber(param.Value, CoordCfg.MetricDecimals);
                    break;

                case ParamType.LatLong:
                    txtValue.Text = Formattings.FormatDMS(param.Value, CoordCfg.LatLonFormat, CoordCfg.LatLonSign, CoordCfg.LatLonDecimals, param.IsNorthAxis);
                    break;

                case ParamType.TrueFalse:
                    txbTitle.Visibility = Visibility.Collapsed;
                    txtValue.Visibility = Visibility.Collapsed;
                    SetInputProjParamTrueFalse(param);
                    break;

                default:
                    txtValue.Text = Formattings.FormatNumber(param.Value, 6);
                    break;
            }
        }

        /// <summary>
        /// Set the special param field for the boolean value
        /// </summary>
        /// <param name="param">Parameter to apply</param>
        private void SetInputProjParamTrueFalse(ParamNameValue param)
        {
            TxbPrjParam8Title.Visibility = Visibility.Visible;
            ChkPrjParam8A.Visibility = Visibility.Visible;
            ChkPrjParam8B.Visibility = Visibility.Visible;
            TxbPrjParam8Title.Text = param.Name;

            if (param.Value >= 0)
            {
                ChkPrjParam8A.IsChecked = true;
                ChkPrjParam8B.IsChecked = false;
            }
            else
            {
                ChkPrjParam8A.IsChecked = false;
                ChkPrjParam8B.IsChecked = true;
            }
        }

        private void RetrieveInputCrsProjInfos()
        {
            List<double> DummyParams = new List<double> { };
            List<ParamNameValue> oldParams = TempCrsIn.GetParams();
            string fn, sn;
            int pc = Projections.GetParamCount(TempCrsIn.Type);
            fn = TxtFullName.Text;
            sn = TxtShortName.Text;

            if (pc >= 1)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtPrjParam1, oldParams[0])); }
            if (pc >= 2)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtPrjParam2, oldParams[1])); }
            if (pc >= 3)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtPrjParam3, oldParams[2])); }
            if (pc >= 4)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtPrjParam4, oldParams[3])); }
            if (pc >= 5)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtPrjParam5, oldParams[4])); }
            if (pc >= 6)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtPrjParam6, oldParams[5])); }
            if (pc >= 7)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtPrjParam7, oldParams[6])); }
            if (pc >= 8)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtPrjParam7, oldParams[7])); }

            TempCrsIn = Projections.FromParams(TempCrsIn.Type, TempCrsIn.BaseEllipsoid, fn, sn, DummyParams);
        }

        private double RetrieveInputCrsProjParams(TextBox txtValue, ParamNameValue param)
        {
            double paramvalue;
            switch (param.Type)
            {
                case ParamType.LatLong:
                    paramvalue = Formattings.DmsParse(txtValue.Text, CoordCfg.LatLonFormat, CoordCfg.LatLonSign);
                    break;

                case ParamType.TrueFalse:
                    if (ChkPrjParam8A.IsChecked == true)
                    { paramvalue = 1; }
                    else
                    { paramvalue = 0; }
                    break;

                default:
                    paramvalue = double.Parse(txtValue.Text, InvCult);
                    break;
            }
            if (!MathExt.Values.IsFinite(paramvalue))
            { paramvalue = param.Value; }
            return paramvalue;
        }

        /// <summary>
        /// Populate the ComboBox of the available ellipsoids.
        /// </summary>
        private void PopulateInputCrsEllipsoids()
        {
            List<Ellipsoid> availEll = new EpsgEllipsoids().KnownEllipsoids;
            CmbEllEpsgId.Items.Clear();
            foreach (Ellipsoid e in availEll)
            {
                CmbEllEpsgId.Items.Add(e.EpsgId + ": " + e.ShortName);
            }
            int selEll = 0;
            for (int id = 0; id < availEll.Count; id++)
            {
                if (TempCrsIn.BaseEllipsoid.EpsgId == availEll[id].EpsgId)
                {
                    selEll = id;
                    break;
                }
            }
            CmbEllEpsgId.SelectedIndex = selEll;
        }

        private void UpdateInputCrsEllipsoidInfos()
        {
            TxbEllFullName.Text = TempCrsIn.BaseEllipsoid.FullName;
            TxbEllMayor.Text = Formattings.FormatNumber(TempCrsIn.BaseEllipsoid.SemiMayorAxis, 3);
            TxbEllFlatt.Text = Formattings.FormatNumber(TempCrsIn.BaseEllipsoid.InverseFlattening, 6);
        }

        /// <summary>
        /// Populate the ComboBox of the available transformations.
        /// </summary>
        private void PopulateInputCrsTransformations()
        {
            CmbTransMethod.Items.Clear();
            CmbTransMethod.Items.Add(UIStrings.SftNone);
            CmbTransMethod.Items.Add(UIStrings.SftGeocentric3Parameter);
            CmbTransMethod.Items.Add(UIStrings.SftHelmert7Parameter);
            CmbTransMethod.Items.Add(UIStrings.SftMolodenskyBadekas10Parameter);
            CmbTransMethod.Items.Add(UIStrings.SftAbridgedMolodensky);
            TxbTransFullNameTitle.Text = UIStrings.SetCrsPrjFullName;
            TxbTransShortNameTitle.Text = UIStrings.SetCrsPrjShortName;
            TxbTransDxTitle.Text = UIStrings.SetCrsShiftDx;
            TxbTransDyTitle.Text = UIStrings.SetCrsShiftDy;
            TxbTransDzTitle.Text = UIStrings.SetCrsShiftDz;
            TxbTransRxTitle.Text = UIStrings.SetCrsShiftRx;
            TxbTransRyTitle.Text = UIStrings.SetCrsShiftRy;
            TxbTransRzTitle.Text = UIStrings.SetCrsShiftRz;
            TxbTransScaleTitle.Text = UIStrings.SetCrsShiftScale;
            TxbTransPxTitle.Text = UIStrings.SetCrsShiftPx;
            TxbTransPyTitle.Text = UIStrings.SetCrsShiftPy;
            TxbTransPzTitle.Text = UIStrings.SetCrsShiftPz;
            ChkTransRotConv1.Content = UIStrings.SftPositionVector;
            ChkTransRotConv2.Content = UIStrings.SftCoordinateFrame;
            CmbTransMethod.SelectedIndex = (int)TempCrsIn.BaseEllipsoid.ToWgs84.Type;
            ChkTransRotConv1.IsChecked = (TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention == Transformations.RotationConventions.PositionVector);
        }

        private void RetrieveInputCrsTransParams()
        {
            double dx, dy, dz, rx, ry, rz, sf, px, py, pz;
            string fn, sn;
            Transformations trs;
            Transformations.RotationConventions rc;

            fn = TxtTransFullName.Text;
            sn = TxtTransShortName.Text;
            trs = TempCrsIn.BaseEllipsoid.ToWgs84;
            rc = TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention;

            dx = double.Parse(TxtTransDx.Text, InvCult);
            dy = double.Parse(TxtTransDy.Text, InvCult);
            dz = double.Parse(TxtTransDz.Text, InvCult);
            rx = double.Parse(TxtTransRx.Text, InvCult);
            ry = double.Parse(TxtTransRy.Text, InvCult);
            rz = double.Parse(TxtTransRz.Text, InvCult);
            sf = double.Parse(TxtTransScale.Text, InvCult);
            px = double.Parse(TxtTransPx.Text, InvCult);
            py = double.Parse(TxtTransPy.Text, InvCult);
            pz = double.Parse(TxtTransPz.Text, InvCult);

            switch (CmbTransMethod.SelectedIndex)
            {
                case 0:
                    TempCrsIn.BaseEllipsoid.ToWgs84 = new None();
                    break;

                case 1:
                    TempCrsIn.BaseEllipsoid.ToWgs84 = new Geocentric3p(trs.SourceEllipsoid, fn, sn, dx, dy, dz);
                    break;

                case 2:
                    TempCrsIn.BaseEllipsoid.ToWgs84 = new Helmert7p(trs.SourceEllipsoid, fn, sn, dx, dy, dz, rx, ry, rz, sf, rc);
                    break;

                case 3:
                    TempCrsIn.BaseEllipsoid.ToWgs84 = new MolodenskyBadekas10p(trs.SourceEllipsoid, fn, sn, dx, dy, dz, rx, ry, rz, sf, px, py, pz, rc);
                    break;

                case 4:
                    TempCrsIn.BaseEllipsoid.ToWgs84 = new AbridgedMolodensky(trs.SourceEllipsoid, fn, sn, dx, dy, dz);
                    break;

                default:
                    TempCrsIn.BaseEllipsoid.ToWgs84 = new Geocentric3p(trs.SourceEllipsoid, fn, sn, 0, 0, 0);
                    break;
            }
        }

        #endregion Fields Updates
    }
}