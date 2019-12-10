using Datums;
using StringFormat;
using GeographicCalculatorWPFCore.Strings;
using MathExt;
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
    /// Logica di interazione per PageSinglePoint.xaml
    /// </summary>
    public partial class SinglePoint : Page
    {
        #region Private Variables

        private readonly IStrings UIStrings; // Localization strings for the app
        private readonly DatumEnumStrings enumStrings; // Localization strings for the linked Datum enums
        private readonly CultureInfo InvCult = CultureInfo.InvariantCulture;
        private Point3D UserPointGeo, UserPointPrj, UserPointWgs, OutPointGeo, OutPointPrj;
        private readonly CoordSettings CoordCfg;
        private readonly Projections CrsInput;
        private readonly Projections CrsOutput;

        #endregion Private Variables

        #region Constructors

        public SinglePoint()
        {
            InitializeComponent();
            UIStrings = new StringProvider(Thread.CurrentThread.CurrentUICulture);
            enumStrings = new DatumEnumStrings();
            Ellipsoid newWgs84 = new Ellipsoid { ToWgs84 = new None() };
            CrsInput = new TransverseMercatorUniversal(newWgs84, "UTM 32 North on WGS 84", "UTM32N-WGS84", 32, true);
            CrsOutput = new TransverseMercatorUniversal(newWgs84, "UTM 33 North on WGS 84", "UTM33N-WGS84", 33, true);
            UserPointGeo = new Point3D(Angles.DegRad(9), Angles.DegRad(45), 0);
            UserPointPrj = new Point3D(0, 0, 0);
            UserPointWgs = new Point3D(0, 0, 0);
            OutPointGeo = new Point3D(0, 0, 0);
            OutPointPrj = new Point3D(0, 0, 0);
            CoordCfg = new CoordSettings();
            // Apply the language
            ApplyLocalStrings();
            // Finally compute all the coordinates
            UpdatePoints(true);
        }

        public SinglePoint(Projections CrsIn, Projections CrsOut, CoordSettings Settings)
        {
            InitializeComponent();
            UIStrings = new StringProvider(Thread.CurrentThread.CurrentUICulture);
            enumStrings = new DatumEnumStrings();
            CrsInput = CrsIn;
            CrsOutput = CrsOut;
            UserPointGeo = new Point3D(Angles.DegRad(9), Angles.DegRad(45), 0);
            UserPointPrj = new Point3D(0, 0, 0);
            UserPointWgs = new Point3D(0, 0, 0);
            OutPointGeo = new Point3D(0, 0, 0);
            OutPointPrj = new Point3D(0, 0, 0);
            CoordCfg = Settings;
            // Apply the language
            ApplyLocalStrings();
            // Finally compute all the coordinates
            UpdatePoints(true);
        }

        public SinglePoint(Projections CrsIn, Projections CrsOut, CoordSettings Settings, Point3D LLPoint)
        {
            InitializeComponent();
            UIStrings = new StringProvider(Thread.CurrentThread.CurrentUICulture);
            enumStrings = new DatumEnumStrings();
            CrsInput = CrsIn;
            CrsOutput = CrsOut;
            UserPointGeo = LLPoint;
            UserPointPrj = new Point3D(0, 0, 0);
            UserPointWgs = new Point3D(0, 0, 0);
            OutPointGeo = new Point3D(0, 0, 0);
            OutPointPrj = new Point3D(0, 0, 0);
            CoordCfg = Settings;
            // Apply the language
            ApplyLocalStrings();
            // Finally compute all the coordinates
            UpdatePoints(true);
        }

        #endregion Constructors

        #region Multilanguage Support

        private void ApplyLocalStrings()
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
                TxbInCrs.Text = FormatProjection(CrsInput);
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
                TxbOutCrs.Text = FormatProjection(CrsOutput);
            }
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

        #endregion Multilanguage Support

        #region Coordinates update

        /// <summary>
        /// Get the new values from the geo fields
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">Event Arguments</param>
        private void TxtInGeoValue_LostFocus(object sender, RoutedEventArgs e)
        {
            Point3D ParsedPoint = new Point3D
            {
                X = Formattings.DmsParse(TxtInGeoLonValue.Text, CoordCfg.LatLonFormat, CoordCfg.LatLonSign),
                Y = Formattings.DmsParse(TxtInGeoLatValue.Text, CoordCfg.LatLonFormat, CoordCfg.LatLonSign),
                Z = double.Parse(TxtInGeoElevValue.Text, InvCult)
            };
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
            Point3D ParsedPoint = new Point3D
            {
                X = Formattings.MetricParse(TxtInPrjEastValue.Text, CoordCfg.MetricFormat),
                Y = Formattings.MetricParse(TxtInPrjNorthValue.Text, CoordCfg.MetricFormat),
                Z = double.Parse(TxtInPrjElevValue.Text, InvCult)
            };
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
                UserPointPrj = CrsInput.FromGeographic(UserPointGeo);
            }
            else // Start from prj point
            {
                UserPointGeo = CrsInput.ToGeographic(UserPointPrj);
            }
            UserPointWgs = CrsInput.BaseEllipsoid.ToWgs84.ToWGS84(UserPointGeo);
            OutPointGeo = CrsOutput.BaseEllipsoid.ToWgs84.FromWGS84(UserPointWgs);
            OutPointPrj = CrsOutput.FromGeographic(OutPointGeo);
            UpdatePointCoordinates();
            // TODO Store the points inside the main window for resume.
        }

        /// <summary>
        /// Single Point - Write the computed coordinates inside the fields
        /// </summary>
        private void UpdatePointCoordinates()
        {
            TxtInGeoLonValue.Text = Formattings.FormatDMS(UserPointGeo.X, CoordCfg.LatLonFormat, CoordCfg.LatLonSign, CoordCfg.LatLonDecimals, false);
            TxtInGeoLatValue.Text = Formattings.FormatDMS(UserPointGeo.Y, CoordCfg.LatLonFormat, CoordCfg.LatLonSign, CoordCfg.LatLonDecimals, true);
            TxtInGeoElevValue.Text = Formattings.FormatNumber(UserPointGeo.Z, CoordCfg.MetricDecimals);
            TxtInPrjEastValue.Text = Formattings.FormatMetric(UserPointPrj.X, CoordCfg.MetricFormat, CoordCfg.MetricDecimals, false);
            TxtInPrjNorthValue.Text = Formattings.FormatMetric(UserPointPrj.Y, CoordCfg.MetricFormat, CoordCfg.MetricDecimals, true);
            TxtInPrjElevValue.Text = Formattings.FormatNumber(UserPointGeo.Z, CoordCfg.MetricDecimals);
            TxtOutGeoLonValue.Text = Formattings.FormatDMS(OutPointGeo.X, CoordCfg.LatLonFormat, CoordCfg.LatLonSign, CoordCfg.LatLonDecimals, false);
            TxtOutGeoLatValue.Text = Formattings.FormatDMS(OutPointGeo.Y, CoordCfg.LatLonFormat, CoordCfg.LatLonSign, CoordCfg.LatLonDecimals, true);
            TxtOutGeoElevValue.Text = Formattings.FormatNumber(OutPointGeo.Z, CoordCfg.MetricDecimals);
            TxtOutPrjEastValue.Text = Formattings.FormatMetric(OutPointPrj.X, CoordCfg.MetricFormat, CoordCfg.MetricDecimals, false);
            TxtOutPrjNorthValue.Text = Formattings.FormatMetric(OutPointPrj.Y, CoordCfg.MetricFormat, CoordCfg.MetricDecimals, true);
            TxtOutPrjElevValue.Text = Formattings.FormatNumber(OutPointGeo.Z, CoordCfg.MetricDecimals);
        }

        #endregion Coordinates update
    }
}