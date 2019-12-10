using GeographicCalculatorWPFCore.Strings;
using MathExt;
using StringFormat;
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
    /// Logica di interazione per PageCoodFormats.xaml
    /// </summary>
    public partial class CoodFormats : Page
    {
        #region Private Variables

        private readonly IStrings UIStrings; // Localization strings for the app
        private readonly DatumEnumStrings enumStrings; // Localization strings for the linked Datum enums
        private readonly CoordSettings CoordCfg;
        private readonly Point3D GeoPoint, PrjPoint;

        #endregion Private Variables

        #region Constructors

        public CoodFormats()
        {
            InitializeComponent();
            UIStrings = new StringProvider(Thread.CurrentThread.CurrentUICulture);
            enumStrings = new DatumEnumStrings();
            CoordCfg = new CoordSettings();
            GeoPoint = new Point3D(Angles.DegRad(45), 0, 0);
            PrjPoint = new Point3D(500000.12345678, 0, 0);
            // Apply the language
            ApplyLocalStrings();
            ApplyCoordSettings();
        }

        public CoodFormats(CoordSettings InSettings)
        {
            InitializeComponent();
            UIStrings = new StringProvider(Thread.CurrentThread.CurrentUICulture);
            enumStrings = new DatumEnumStrings();
            CoordCfg = InSettings;
            GeoPoint = new Point3D(Angles.DegRad(45), 0, 0);
            PrjPoint = new Point3D(500000.12345678, 0, 0);
            // Apply the language
            ApplyLocalStrings();
            ApplyCoordSettings();
        }

        public CoodFormats(CoordSettings InSettings, Point3D llpoint, Point3D mpoint)
        {
            InitializeComponent();
            UIStrings = new StringProvider(Thread.CurrentThread.CurrentUICulture);
            enumStrings = new DatumEnumStrings();
            CoordCfg = InSettings;
            GeoPoint = llpoint;
            PrjPoint = mpoint;
            // Apply the language
            ApplyLocalStrings();
            ApplyCoordSettings();
        }

        private void ApplyCoordSettings()
        {
            CmbSetGeoFormat.SelectedIndex = (int)CoordCfg.LatLonFormat;
            CmbSetGeoSign.SelectedIndex = (int)CoordCfg.LatLonSign;
            CmbSetGeoDecimals.SelectedIndex = CoordCfg.LatLonDecimals;
            CmbSetPrjFormat.SelectedIndex = (int)CoordCfg.MetricFormat;
            CmbSetPrjDecimals.SelectedIndex = CoordCfg.MetricDecimals;
        }

        #endregion Constructors

        #region Multilanguage Support

        /// <summary>
        /// Populate the fields in the setting panel
        /// </summary>
        private void ApplyLocalStrings()
        {
            if (UIStrings != null)
            {
                // LatLon Format Enum
                CmbSetGeoFormat.Items.Clear();
                foreach (Formattings.DmsFormat name in Enum.GetValues(typeof(Formattings.DmsFormat)))
                { CmbSetGeoFormat.Items.Add(enumStrings.LocalizedDmsFormat(name)); }
                // LatLon Sign Enum
                CmbSetGeoSign.Items.Clear();
                foreach (Formattings.DmsSign name in Enum.GetValues(typeof(Formattings.DmsSign)))
                { CmbSetGeoSign.Items.Add(enumStrings.LocalizedDmsSign(name)); }
                // Metric Format Enum
                CmbSetPrjFormat.Items.Clear();
                foreach (Formattings.MetricSign name in Enum.GetValues(typeof(Formattings.MetricSign)))
                { CmbSetPrjFormat.Items.Add(enumStrings.LocalizedMetricSign(name)); }
            }
        }

        #endregion Multilanguage Support

        #region User Interactions

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO Send the active CoordSettings to the main window
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO Return to the main window without updates
        }

        private void CmbSetGeoFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSetGeoFormat.SelectedIndex >= 0)
            { CoordCfg.LatLonFormat = (Formattings.DmsFormat)CmbSetGeoFormat.SelectedIndex; }
            else
            { CmbSetGeoFormat.SelectedIndex = (int)CoordCfg.LatLonFormat; }
            UpdateExamples();
        }

        private void CmbSetGeoSign_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSetGeoSign.SelectedIndex >= 0)
            { CoordCfg.LatLonSign = (Formattings.DmsSign)CmbSetGeoSign.SelectedIndex; }
            else
            { CmbSetGeoSign.SelectedIndex = (int)CoordCfg.LatLonSign; }
            UpdateExamples();
        }

        private void CmbSetGeoDecimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSetGeoDecimals.SelectedIndex >= 0)
            { CoordCfg.LatLonDecimals = CmbSetGeoDecimals.SelectedIndex; }
            else
            { CmbSetGeoDecimals.SelectedIndex = CoordCfg.LatLonDecimals; }
            UpdateExamples();
        }

        private void CmbSetPrjFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSetPrjFormat.SelectedIndex >= 0)
            { CoordCfg.MetricFormat = (Formattings.MetricSign)CmbSetPrjFormat.SelectedIndex; }
            else
            { CmbSetPrjFormat.SelectedIndex = (int)CoordCfg.MetricFormat; }
            UpdateExamples();
        }

        private void CmbSetPrjDecimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSetPrjDecimals.SelectedIndex >= 0)
            { CoordCfg.MetricDecimals = CmbSetPrjDecimals.SelectedIndex; }
            else
            { CmbSetPrjDecimals.SelectedIndex = CoordCfg.MetricDecimals; }
            UpdateExamples();
        }

        private void UpdateExamples()
        {
            TxbSetGeoResultValue.Text = Formattings.FormatDMS(GeoPoint.X, CoordCfg.LatLonFormat, CoordCfg.LatLonSign, CoordCfg.LatLonDecimals, false);
            TxbSetPrjResultValue.Text = Formattings.FormatMetric(PrjPoint.X, CoordCfg.MetricFormat, CoordCfg.MetricDecimals, false);
        }

        #endregion User Interactions
    }
}