using Datums;
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
        #region Main page string update

        /// <summary>
        /// Apply the localized strings for the app UI
        /// </summary>
        private void ChangeLanguage()
        {
            if (UIStrings != null)
            {
                Title = UIStrings.AppTitle;
                // Input Column
                TxbInGeoTitle.Text = UIStrings.TxbGeoTitle;
                TxbInGeoLatTitle.Text = UIStrings.TxbGeoLatTitle;
                TxbInGeoLonTitle.Text = UIStrings.TxbGeoLonTitle;
                TxbInGeoElevTitle.Text = UIStrings.TxbGeoElevTitle;
                TxbInPrjTitle.Text = UIStrings.TxbPrjTitle;
                TxbInPrjEastTitle.Text = UIStrings.TxbPrjEastTitle;
                TxbInPrjNorthTitle.Text = UIStrings.TxbPrjNorthTitle;
                TxbInPrjElevTitle.Text = UIStrings.TxbPrjElevTitle;
                TxbInCrsTitle.Text = UIStrings.TxbCrsInTitle;
                TxbInCrs.Text = FormatProjection(InputCrs);
                // Output Column
                TxbOutGeoTitle.Text = UIStrings.TxbGeoTitle;
                TxbOutGeoLatTitle.Text = UIStrings.TxbGeoLatTitle;
                TxbOutGeoLonTitle.Text = UIStrings.TxbGeoLonTitle;
                TxbOutGeoElevTitle.Text = UIStrings.TxbGeoElevTitle;
                TxbOutPrjTitle.Text = UIStrings.TxbPrjTitle;
                TxbOutPrjEastTitle.Text = UIStrings.TxbPrjEastTitle;
                TxbOutPrjNorthTitle.Text = UIStrings.TxbPrjNorthTitle;
                TxbOutPrjElevTitle.Text = UIStrings.TxbPrjElevTitle;
                TxbOutCrsTitle.Text = UIStrings.TxbCrsOutTitle;
                TxbOutCrs.Text = FormatProjection(OutputCrs);
            }
        }

        /// <summary>
        /// Single Point - Write the computed coordinates inside the fields
        /// </summary>
        private void UpdatePointCoordinates()
        {
            TxtInGeoLonValue.Text = StringFormat.Formattings.FormatDMS(UserPointGeo.X, GeoFormat, GeoSign, GeoDecimals, false);
            TxtInGeoLatValue.Text = StringFormat.Formattings.FormatDMS(UserPointGeo.Y, GeoFormat, GeoSign, GeoDecimals, true);
            TxtInGeoElevValue.Text = StringFormat.Formattings.FormatNumber(UserPointGeo.Z, PrjDecimals);
            TxtInPrjEastValue.Text = StringFormat.Formattings.FormatMetric(UserPointPrj.X, PrjSign, PrjDecimals, false);
            TxtInPrjNorthValue.Text = StringFormat.Formattings.FormatMetric(UserPointPrj.Y, PrjSign, PrjDecimals, true);
            TxtInPrjElevValue.Text = StringFormat.Formattings.FormatNumber(UserPointGeo.Z, PrjDecimals);
            TxtOutGeoLonValue.Text = StringFormat.Formattings.FormatDMS(OutPointGeo.X, GeoFormat, GeoSign, GeoDecimals, false);
            TxtOutGeoLatValue.Text = StringFormat.Formattings.FormatDMS(OutPointGeo.Y, GeoFormat, GeoSign, GeoDecimals, true);
            TxtOutGeoElevValue.Text = StringFormat.Formattings.FormatNumber(OutPointGeo.Z, PrjDecimals);
            TxtOutPrjEastValue.Text = StringFormat.Formattings.FormatMetric(OutPointPrj.X, PrjSign, PrjDecimals, false);
            TxtOutPrjNorthValue.Text = StringFormat.Formattings.FormatMetric(OutPointPrj.Y, PrjSign, PrjDecimals, true);
            TxtOutPrjElevValue.Text = StringFormat.Formattings.FormatNumber(OutPointGeo.Z, PrjDecimals);
        }

        /// <summary>
        /// Get a localized and formatted string with datum informations
        /// </summary>
        /// <param name="prj">Datum</param>
        /// <returns>Localized and formatted string</returns>
        private string FormatProjection(Projections prj)
        {
            string outString = UIStrings.NotDefined;

            if (prj != null)
            {
                outString = UIStrings.CrsPrjName + "\n";
                outString += prj.FullName + "\n";
                outString += UIStrings.CrsPrjType + "\n";
                outString += enumStrings.LocalizedProjectionType(prj.Type) + "\n";
                outString += UIStrings.CrsEllName + "\n";
                outString += prj.BaseEllipsoid.FullName + "\n";
                outString += UIStrings.CrsShiftName + "\n";
                outString += enumStrings.LocalizedTransformationMethod(prj.BaseEllipsoid.ToWgs84);
            }

            return outString;
        }

        #endregion Main page string update

        #region Setting panel updates

        /// <summary>
        /// Populate the fields in the setting panel
        /// </summary>
        private void UpdateSettingsPanel()
        {
            // Geo Format
            CmbSetGeoFormat.Items.Clear();
            foreach (StringFormat.Formattings.DmsFormat name in Enum.GetValues(typeof(StringFormat.Formattings.DmsFormat)))
            { CmbSetGeoFormat.Items.Add(enumStrings.LocalizedDmsFormat(name)); }
            CmbSetGeoFormat.SelectedIndex = (int)GeoFormat;
            // Geo Sign
            CmbSetGeoSign.Items.Clear();
            foreach (StringFormat.Formattings.DmsSign name in Enum.GetValues(typeof(StringFormat.Formattings.DmsSign)))
            { CmbSetGeoSign.Items.Add(enumStrings.LocalizedDmsSign(name)); }
            CmbSetGeoSign.SelectedIndex = (int)GeoSign;
            // Geo Decimals
            CmbSetGeoDecimals.SelectedIndex = GeoDecimals;
            // Prj Format
            CmbSetPrjFormat.Items.Clear();
            foreach (StringFormat.Formattings.MetricSign name in Enum.GetValues(typeof(StringFormat.Formattings.MetricSign)))
            { CmbSetPrjFormat.Items.Add(enumStrings.LocalizedMetricSign(name)); }
            CmbSetPrjFormat.SelectedIndex = (int)PrjSign;
            // Prj Decimals
            CmbSetPrjDecimals.SelectedIndex = PrjDecimals;
        }

        /// <summary>
        /// Populate the fields in the Input CRS panel
        /// </summary>
        private void UpdateInputCrsPanel()
        {
            TxbSetCrsInFullNameTitle.Text = UIStrings.SetCrsPrjFullName;
            TxtSetCrsInFullName.Text = InputCrs.FullName;
            TxbSetCrsInShortNameTitle.Text = UIStrings.SetCrsPrjShortName;
            TxtSetCrsInShortName.Text = InputCrs.ShortName;
            TxbSetCrsInPrjMethodTitle.Text = UIStrings.SetCrsPrjMethod;
            PopulateInputCrsProjMethods();
            CmbSetCrsInPrjMethod.SelectedIndex = (int)InputCrs.Type;
            PopulateInputCrsEllipsoids();
            PopulateInputCrsTransformations();
            BtnSetCrsInPrj_Click(BtnSetCrsInPrj1, new RoutedEventArgs());
        }

        private void PopulateInputCrsProjMethods()
        {
            CmbSetCrsInPrjMethod.Items.Clear();
            for (int p = 0; p < 50; p++)
            {
                if (enumStrings.LocalizedProjectionType((Projections.Method)p) != UIStrings.NotDefined)
                { CmbSetCrsInPrjMethod.Items.Add(enumStrings.LocalizedProjectionType((Projections.Method)p)); }
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
                TxbSetCrsInParam1Title.Visibility = Visibility.Collapsed;
                TxbSetCrsInParam2Title.Visibility = Visibility.Collapsed;
                TxbSetCrsInParam3Title.Visibility = Visibility.Collapsed;
                TxbSetCrsInParam4Title.Visibility = Visibility.Collapsed;
                TxbSetCrsInParam5Title.Visibility = Visibility.Collapsed;
                TxbSetCrsInParam6Title.Visibility = Visibility.Collapsed;
                TxbSetCrsInParam7Title.Visibility = Visibility.Collapsed;
                TxbSetCrsInParam8Title.Visibility = Visibility.Collapsed;
                TxtSetCrsInParam1.Visibility = Visibility.Collapsed;
                TxtSetCrsInParam2.Visibility = Visibility.Collapsed;
                TxtSetCrsInParam3.Visibility = Visibility.Collapsed;
                TxtSetCrsInParam4.Visibility = Visibility.Collapsed;
                TxtSetCrsInParam5.Visibility = Visibility.Collapsed;
                TxtSetCrsInParam6.Visibility = Visibility.Collapsed;
                TxtSetCrsInParam7.Visibility = Visibility.Collapsed;
                ChkSetCrsInParam8A.Visibility = Visibility.Collapsed;
                ChkSetCrsInParam8B.Visibility = Visibility.Collapsed;

                List<ParamNameValue> ParamList = proj.GetParams();
                if (ParamList.Count >= 1) // Set the first param //
                { SetProjParamFormat(TxbSetCrsInParam1Title, TxtSetCrsInParam1, ParamList[0]); }
                if (ParamList.Count >= 2) // Set the second param //
                { SetProjParamFormat(TxbSetCrsInParam2Title, TxtSetCrsInParam2, ParamList[1]); }
                if (ParamList.Count >= 3) // Set the third param //
                { SetProjParamFormat(TxbSetCrsInParam3Title, TxtSetCrsInParam3, ParamList[2]); }
                if (ParamList.Count >= 4) // Set the fourth param //
                { SetProjParamFormat(TxbSetCrsInParam4Title, TxtSetCrsInParam4, ParamList[3]); }
                if (ParamList.Count >= 5) // Set the fifth param //
                { SetProjParamFormat(TxbSetCrsInParam5Title, TxtSetCrsInParam5, ParamList[4]); }
                if (ParamList.Count >= 6) // Set the sixth param //
                { SetProjParamFormat(TxbSetCrsInParam6Title, TxtSetCrsInParam6, ParamList[5]); }
                if (ParamList.Count >= 7) // Set the seventh param //
                { SetProjParamFormat(TxbSetCrsInParam7Title, TxtSetCrsInParam7, ParamList[6]); }
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
                    txtValue.Text = StringFormat.Formattings.FormatNumber(param.Value, PrjDecimals);
                    break;

                case ParamType.LatLong:
                    txtValue.Text = StringFormat.Formattings.FormatDMS(param.Value, GeoFormat, GeoSign, GeoDecimals, param.IsNorthAxis);
                    break;

                case ParamType.TrueFalse:
                    txbTitle.Visibility = Visibility.Collapsed;
                    txtValue.Visibility = Visibility.Collapsed;
                    SetInputProjParamTrueFalse(param);
                    break;

                default:
                    txtValue.Text = StringFormat.Formattings.FormatNumber(param.Value, 6);
                    break;
            }
        }

        /// <summary>
        /// Set the special param field for the boolean value
        /// </summary>
        /// <param name="param">Parameter to apply</param>
        private void SetInputProjParamTrueFalse(ParamNameValue param)
        {
            TxbSetCrsInParam8Title.Visibility = Visibility.Visible;
            ChkSetCrsInParam8A.Visibility = Visibility.Visible;
            ChkSetCrsInParam8B.Visibility = Visibility.Visible;
            TxbSetCrsInParam8Title.Text = param.Name;

            if (param.Value >= 0)
            {
                ChkSetCrsInParam8A.IsChecked = true;
                ChkSetCrsInParam8B.IsChecked = false;
            }
            else
            {
                ChkSetCrsInParam8A.IsChecked = false;
                ChkSetCrsInParam8B.IsChecked = true;
            }
        }

        private void RetrieveInputCrsProjInfos()
        {
            List<double> DummyParams = new List<double> { };
            List<ParamNameValue> oldParams = TempCrsIn.GetParams();
            string fn, sn;
            int pc = Projections.GetParamCount(TempCrsIn.Type);
            fn = TxtSetCrsInFullName.Text;
            sn = TxtSetCrsInShortName.Text;

            if (pc >= 1)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtSetCrsInParam1, oldParams[0])); }
            if (pc >= 2)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtSetCrsInParam2, oldParams[1])); }
            if (pc >= 3)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtSetCrsInParam3, oldParams[2])); }
            if (pc >= 4)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtSetCrsInParam4, oldParams[3])); }
            if (pc >= 5)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtSetCrsInParam5, oldParams[4])); }
            if (pc >= 6)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtSetCrsInParam6, oldParams[5])); }
            if (pc >= 7)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtSetCrsInParam7, oldParams[6])); }
            if (pc >= 8)
            { DummyParams.Add(RetrieveInputCrsProjParams(TxtSetCrsInParam7, oldParams[7])); }

            TempCrsIn = Projections.FromParams(TempCrsIn.Type, TempCrsIn.BaseEllipsoid, fn, sn, DummyParams);
        }

        private double RetrieveInputCrsProjParams(TextBox txtValue, ParamNameValue param)
        {
            double paramvalue;
            switch (param.Type)
            {
                case ParamType.LatLong:
                    paramvalue = StringFormat.Formattings.DmsParse(txtValue.Text, GeoFormat, GeoSign);
                    break;

                case ParamType.TrueFalse:
                    if (ChkSetCrsInParam8A.IsChecked == true)
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
            CmbSetCrsInEllEpsgId.Items.Clear();
            foreach (Ellipsoid e in availEll)
            {
                CmbSetCrsInEllEpsgId.Items.Add(e.EpsgId + ": " + e.ShortName);
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
            CmbSetCrsInEllEpsgId.SelectedIndex = selEll;
        }

        private void UpdateInputCrsEllipsoidInfos()
        {
            TxbSetCrsInEllFullName.Text = TempCrsIn.BaseEllipsoid.FullName;
            TxbSetCrsInEllMayor.Text = StringFormat.Formattings.FormatNumber(TempCrsIn.BaseEllipsoid.SemiMayorAxis, 3);
            TxbSetCrsInEllFlatt.Text = StringFormat.Formattings.FormatNumber(TempCrsIn.BaseEllipsoid.InverseFlattening, 6);
        }

        /// <summary>
        /// Populate the ComboBox of the available transformations.
        /// </summary>
        private void PopulateInputCrsTransformations()
        {
            CmbSetCrsInTransMethod.Items.Clear();
            CmbSetCrsInTransMethod.Items.Add(UIStrings.SftNone);
            CmbSetCrsInTransMethod.Items.Add(UIStrings.SftGeocentric3Parameter);
            CmbSetCrsInTransMethod.Items.Add(UIStrings.SftHelmert7Parameter);
            CmbSetCrsInTransMethod.Items.Add(UIStrings.SftMolodenskyBadekas10Parameter);
            CmbSetCrsInTransMethod.Items.Add(UIStrings.SftAbridgedMolodensky);
            TxbSetCrsInTransFullNameTitle.Text = UIStrings.SetCrsPrjFullName;
            TxbSetCrsInTransShortNameTitle.Text = UIStrings.SetCrsPrjShortName;
            TxbSetCrsInTransDxTitle.Text = UIStrings.SetCrsShiftDx;
            TxbSetCrsInTransDyTitle.Text = UIStrings.SetCrsShiftDy;
            TxbSetCrsInTransDzTitle.Text = UIStrings.SetCrsShiftDz;
            TxbSetCrsInTransRxTitle.Text = UIStrings.SetCrsShiftRx;
            TxbSetCrsInTransRyTitle.Text = UIStrings.SetCrsShiftRy;
            TxbSetCrsInTransRzTitle.Text = UIStrings.SetCrsShiftRz;
            TxbSetCrsInTransScaleTitle.Text = UIStrings.SetCrsShiftScale;
            TxbSetCrsInTransPxTitle.Text = UIStrings.SetCrsShiftPx;
            TxbSetCrsInTransPyTitle.Text = UIStrings.SetCrsShiftPy;
            TxbSetCrsInTransPzTitle.Text = UIStrings.SetCrsShiftPz;
            ChkSetCrsInTransRotConv1.Content = UIStrings.SftPositionVector;
            ChkSetCrsInTransRotConv2.Content = UIStrings.SftCoordinateFrame;
            CmbSetCrsInTransMethod.SelectedIndex = (int)TempCrsIn.BaseEllipsoid.ToWgs84.Type;
            ChkSetCrsInTransRotConv1.IsChecked = (TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention == Transformations.RotationConventions.PositionVector);
        }

        private void RetrieveInputCrsTransParams()
        {
            double dx, dy, dz, rx, ry, rz, sf, px, py, pz;
            string fn, sn;
            Transformations trs;
            Transformations.RotationConventions rc;

            fn = TxtSetCrsInTransFullName.Text;
            sn = TxtSetCrsInTransShortName.Text;
            trs = TempCrsIn.BaseEllipsoid.ToWgs84;
            rc = TempCrsIn.BaseEllipsoid.ToWgs84.RotationConvention;

            dx = double.Parse(TxtSetCrsInTransDx.Text, InvCult);
            dy = double.Parse(TxtSetCrsInTransDy.Text, InvCult);
            dz = double.Parse(TxtSetCrsInTransDz.Text, InvCult);
            rx = double.Parse(TxtSetCrsInTransRx.Text, InvCult);
            ry = double.Parse(TxtSetCrsInTransRy.Text, InvCult);
            rz = double.Parse(TxtSetCrsInTransRz.Text, InvCult);
            sf = double.Parse(TxtSetCrsInTransScale.Text, InvCult);
            px = double.Parse(TxtSetCrsInTransPx.Text, InvCult);
            py = double.Parse(TxtSetCrsInTransPy.Text, InvCult);
            pz = double.Parse(TxtSetCrsInTransPz.Text, InvCult);

            switch (CmbSetCrsInTransMethod.SelectedIndex)
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

        #endregion Setting panel updates
    }
}