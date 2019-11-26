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
        private readonly IStrings UIStrings;
        private Projections InputCrs;
        private Projections OutputCrs;
        private CultureInfo InvCult = CultureInfo.InvariantCulture;
        private Point3D UserPointGeo, UserPointPrj, UserPointWgs, OutPointGeo, OutPointPrj;
        private StringFormat.Formattings.DmsFormat GeoFormat;
        private StringFormat.Formattings.DmsSign GeoSign;
        private StringFormat.Formattings.MetricSign PrjSign;
        private int GeoDecimals, PrjDecimals;

        public MainWindow()
        {
            InitializeComponent();
            UIStrings = new StringProvider(Thread.CurrentThread.CurrentUICulture);
            Ellipsoid newWgs84 = new Ellipsoid { ToWgs84 = new Geocentric3p() };
            InputCrs = new TransverseMercatorUniversal(newWgs84, "UTM 32 North on WGS 84", "UTM32N-WGS84", 32, true);
            OutputCrs = new TransverseMercatorUniversal(newWgs84, "UTM 33 North on WGS 84", "UTM33N-WGS84", 33, true);
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
            // Finally compute all the coordinates
            UpdatePoints(true);
        }

        #region Panel Settings

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
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
    }
}