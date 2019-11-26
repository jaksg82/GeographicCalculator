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

        private void UpdateSettingsPanel()
        {
            // Geo Format
            CmbSetGeoFormat.Items.Clear();
            foreach (string name in Enum.GetNames(typeof(StringFormat.Formattings.DmsFormat)))
            { CmbSetGeoFormat.Items.Add(name); }
            CmbSetGeoFormat.SelectedIndex = (int)GeoFormat;
            // Geo Sign
            CmbSetGeoSign.Items.Clear();
            foreach (string name in Enum.GetNames(typeof(StringFormat.Formattings.DmsSign)))
            { CmbSetGeoSign.Items.Add(name); }
            CmbSetGeoSign.SelectedIndex = (int)GeoSign;
            // Geo Decimals
            CmbSetGeoDecimals.SelectedIndex = GeoDecimals;
            // Prj Format
            CmbSetPrjFormat.Items.Clear();
            foreach (string name in Enum.GetNames(typeof(StringFormat.Formattings.MetricSign)))
            { CmbSetPrjFormat.Items.Add(name); }
            CmbSetPrjFormat.SelectedIndex = (int)PrjSign;
            // Prj Decimals
            CmbSetPrjDecimals.SelectedIndex = PrjDecimals;
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
                outString += LocalizedProjectionType(prj) + "\n";
                outString += UIStrings.CrsEllName + "\n";
                outString += prj.BaseEllipsoid.FullName + "\n";
                outString += UIStrings.CrsShiftName + "\n";
                outString += LocalizedTransformationMethod(prj.BaseEllipsoid.ToWgs84);
            }

            return outString;
        }

        /// <summary>
        /// Get the localized string for the projection method
        /// </summary>
        /// <param name="prj">Given method</param>
        /// <returns>Localized string</returns>
        private string LocalizedProjectionType(Projections prj)
        {
            if (prj != null)
            {
                switch (prj.Type)
                {
                    case Projections.Method.LambertConicConformal2SP: return UIStrings.PrjLambertConicConformal2SP;
                    case Projections.Method.LambertConicConformal1SP: return UIStrings.PrjLambertConicConformal1SP;
                    case Projections.Method.LambertConicConformalWest: return UIStrings.PrjLambertConicConformalWest;
                    case Projections.Method.LambertConicConformalBelgium: return UIStrings.PrjLambertConicConformalBelgium;
                    case Projections.Method.LambertConicNearConformal: return UIStrings.PrjLambertConicNearConformal;
                    case Projections.Method.Krovak: return UIStrings.PrjKrovak;
                    case Projections.Method.KrovakNorth: return UIStrings.PrjKrovakNorth;
                    case Projections.Method.KrovakModified: return UIStrings.PrjKrovakModified;
                    case Projections.Method.KrovakModifiedNorth: return UIStrings.PrjKrovakModifiedNorth;
                    case Projections.Method.MercatorVariantA: return UIStrings.PrjMercatorVariantA;
                    case Projections.Method.MercatorVariantB: return UIStrings.PrjMercatorVariantB;
                    case Projections.Method.MercatorVariantC: return UIStrings.PrjMercatorVariantC;
                    case Projections.Method.MercatorSpherical: return UIStrings.PrjMercatorSpherical;
                    case Projections.Method.MercatorPseudo: return UIStrings.PrjMercatorPseudo;
                    case Projections.Method.CassiniSoldner: return UIStrings.PrjCassiniSoldner;
                    case Projections.Method.CassiniSoldnerHyperbolic: return UIStrings.PrjCassiniSoldnerHyperbolic;
                    case Projections.Method.TransverseMercator: return UIStrings.PrjTransverseMercator;
                    case Projections.Method.TransverseMercatorUniversal: return UIStrings.PrjTransverseMercatorUniversal;
                    case Projections.Method.TransverseMercatorZoned: return UIStrings.PrjTransverseMercatorZoned;
                    case Projections.Method.TransverseMercatorSouth: return UIStrings.PrjTransverseMercatorSouth;
                    case Projections.Method.ObliqueMercatorHotineA: return UIStrings.PrjObliqueMercatorHotineA;
                    case Projections.Method.ObliqueMercatorHotineB: return UIStrings.PrjObliqueMercatorHotineB;
                    case Projections.Method.ObliqueMercatorLaborde: return UIStrings.PrjObliqueMercatorLaborde;
                    case Projections.Method.StereographicOblique: return UIStrings.PrjStereographicOblique;
                    case Projections.Method.StereographicPolarA: return UIStrings.PrjStereographicPolarA;
                    case Projections.Method.StereographicPolarB: return UIStrings.PrjStereographicPolarB;
                    case Projections.Method.StereographicPolarC: return UIStrings.PrjStereographicPolarC;
                    //case Projections.Method.NewZealandGrid: return UIStrings.PrjNewZealandGrid;
                    //case Projections.Method.TunisiaMining: return UIStrings.PrjTunisiaMining;
                    case Projections.Method.AmericanPolyconic: return UIStrings.PrjAmericanPolyconic;
                    case Projections.Method.LambertAzimutalEqualArea: return UIStrings.PrjLambertAzimutalEqualArea;
                    case Projections.Method.LambertAzimutalEqualAreaPolar: return UIStrings.PrjLambertAzimutalEqualAreaPolar;
                    //case Projections.Method.LambertAzimutalEqualAreaSpherical: return UIStrings.PrjLambertAzimutalEqualAreaSpherical;
                    //case Projections.Method.LambertCylindricalEqualArea: return UIStrings.PrjLambertCylindricalEqualArea;
                    //case Projections.Method.LambertCylindricalEqualAreaSpherical: return UIStrings.PrjLambertCylindricalEqualAreaSpherical;
                    case Projections.Method.AlbersEqualArea: return UIStrings.PrjAlbersEqualArea;
                    case Projections.Method.EquidistantCylindrical: return UIStrings.PrjEquidistantCylindrical;
                    case Projections.Method.EquidistantCylindricalSpherical: return UIStrings.PrjEquidistantCylindricalSpherical;
                    case Projections.Method.PseudoPlateCarree: return UIStrings.PrjPseudoPlateCarree;
                    //case Projections.Method.Bonne: return UIStrings.PrjBonne;
                    //case Projections.Method.BonneSouth: return UIStrings.PrjBonneSouth;
                    //case Projections.Method.AzimutalEquidistantModified: return UIStrings.PrjAzimutalEquidistantModified;
                    //case Projections.Method.AzimutalEquidistantGuam: return UIStrings.PrjAzimutalEquidistantGuam;

                    default: return UIStrings.NotDefined;
                }
            }
            else
            { return UIStrings.NotDefined; }
        }

        /// <summary>
        /// Get the localized string for the transformation method
        /// </summary>
        /// <param name="sft">Given method</param>
        /// <returns>Localized string</returns>
        private string LocalizedTransformationMethod(Transformations sft)
        {
            if (sft != null)
            {
                switch (sft.Type)
                {
                    case Transformations.Methods.None: return UIStrings.SftNone;
                    case Transformations.Methods.Geocentric3Parameter: return UIStrings.SftGeocentric3Parameter;
                    case Transformations.Methods.Helmert7Parameter: return UIStrings.SftHelmert7Parameter;
                    case Transformations.Methods.MolodenskyBadekas10Parameter: return UIStrings.SftMolodenskyBadekas10Parameter;
                    case Transformations.Methods.AbridgedMolodensky: return UIStrings.SftAbridgedMolodensky;

                    default: return UIStrings.NotDefined;
                }
            }
            return UIStrings.NotDefined;
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
    }
}