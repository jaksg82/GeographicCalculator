using System;
using System.Globalization;

namespace GeographicCalculatorWPFCore.Strings
{
    public class StringProvider : IStrings
    {
        #region Public properties

        // Generic Strings
        public string AppTitle { get; set; }

        public string NotDefined { get; set; }

        // UI Main Single Point
        public string TxbGeoTitle { get; set; }

        public string TxbGeoLatTitle { get; set; }
        public string TxbGeoLonTitle { get; set; }
        public string TxbGeoElevTitle { get; set; }
        public string TxbPrjTitle { get; set; }
        public string TxbPrjEastTitle { get; set; }
        public string TxbPrjNorthTitle { get; set; }
        public string TxbPrjElevTitle { get; set; }
        public string TxbCrsInTitle { get; set; }
        public string TxbCrsOutTitle { get; set; }
        public string CrsPrjName { get; set; }
        public string CrsPrjType { get; set; }
        public string CrsEllName { get; set; }
        public string CrsShiftName { get; set; }

        // Panel Settings
        public string TxbSetGeoFormat { get; set; }

        public string TxbSetGeoSign { get; set; }
        public string TxbSetGeoDecimals { get; set; }
        public string TxbSetPrjFormat { get; set; }
        public string TxbSetPrjDecimals { get; set; }

        // Projection Methods
        public string PrjLambertConicConformal2SP { get; set; }

        public string PrjLambertConicConformal1SP { get; set; }
        public string PrjLambertConicConformalWest { get; set; }
        public string PrjLambertConicConformalBelgium { get; set; }
        public string PrjLambertConicNearConformal { get; set; }
        public string PrjKrovak { get; set; }
        public string PrjKrovakNorth { get; set; }
        public string PrjKrovakModified { get; set; }
        public string PrjKrovakModifiedNorth { get; set; }
        public string PrjMercatorVariantA { get; set; }
        public string PrjMercatorVariantB { get; set; }
        public string PrjMercatorVariantC { get; set; }
        public string PrjMercatorSpherical { get; set; }
        public string PrjMercatorPseudo { get; set; }
        public string PrjCassiniSoldner { get; set; }
        public string PrjCassiniSoldnerHyperbolic { get; set; }
        public string PrjTransverseMercator { get; set; }
        public string PrjTransverseMercatorUniversal { get; set; }
        public string PrjTransverseMercatorZoned { get; set; }
        public string PrjTransverseMercatorSouth { get; set; }
        public string PrjObliqueMercatorHotineA { get; set; }
        public string PrjObliqueMercatorHotineB { get; set; }
        public string PrjObliqueMercatorLaborde { get; set; }
        public string PrjStereographicOblique { get; set; }
        public string PrjStereographicPolarA { get; set; }
        public string PrjStereographicPolarB { get; set; }
        public string PrjStereographicPolarC { get; set; }
        public string PrjNewZealandGrid { get; set; }
        public string PrjTunisiaMining { get; set; }
        public string PrjAmericanPolyconic { get; set; }
        public string PrjLambertAzimutalEqualArea { get; set; }
        public string PrjLambertAzimutalEqualAreaPolar { get; set; }
        public string PrjLambertAzimutalEqualAreaSpherical { get; set; }
        public string PrjLambertCylindricalEqualArea { get; set; }
        public string PrjLambertCylindricalEqualAreaSpherical { get; set; }
        public string PrjAlbersEqualArea { get; set; }
        public string PrjEquidistantCylindrical { get; set; }
        public string PrjEquidistantCylindricalSpherical { get; set; }
        public string PrjPseudoPlateCarree { get; set; }
        public string PrjBonne { get; set; }
        public string PrjBonneSouth { get; set; }
        public string PrjAzimutalEquidistantModified { get; set; }
        public string PrjAzimutalEquidistantGuam { get; set; }

        // Transformation Methods
        public string SftNone { get; set; }

        public string SftGeocentric3Parameter { get; set; }
        public string SftHelmert7Parameter { get; set; }
        public string SftMolodenskyBadekas10Parameter { get; set; }
        public string SftAbridgedMolodensky { get; set; }

        #endregion Public properties

        #region Constructors

        public StringProvider()
        {
            ApplyCulture("en");
        }

        public StringProvider(CultureInfo culture)
        {
            if (culture != null) { ApplyCulture(culture.TwoLetterISOLanguageName); }
            else { ApplyCulture("en"); }
        }

        #endregion Constructors

        #region private Methods

        private void ApplyCulture(string cultureID)
        {
            IStrings locStrings = new EnStrings();

            if (cultureID == null)
            { locStrings = new EnStrings(); }
            else
            {
                locStrings = cultureID switch
                {
                    "en" => new EnStrings(),
                    "it" => new ItStrings(),
                    _ => new EnStrings(),
                };
            }

            // Generic Strings
            AppTitle = locStrings.AppTitle;
            NotDefined = locStrings.NotDefined;
            // UI Main Single Point
            TxbGeoTitle = locStrings.TxbGeoTitle;
            TxbGeoLatTitle = locStrings.TxbGeoLatTitle;
            TxbGeoLonTitle = locStrings.TxbGeoLonTitle;
            TxbGeoElevTitle = locStrings.TxbGeoElevTitle;
            TxbPrjTitle = locStrings.TxbPrjTitle;
            TxbPrjEastTitle = locStrings.TxbPrjEastTitle;
            TxbPrjNorthTitle = locStrings.TxbPrjNorthTitle;
            TxbPrjElevTitle = locStrings.TxbPrjElevTitle;
            TxbCrsInTitle = locStrings.TxbCrsInTitle;
            TxbCrsOutTitle = locStrings.TxbCrsOutTitle;
            CrsPrjName = locStrings.CrsPrjName;
            CrsPrjType = locStrings.CrsPrjType;
            CrsEllName = locStrings.CrsEllName;
            CrsShiftName = locStrings.CrsShiftName;

            // Panel Settings
            TxbSetGeoFormat = locStrings.TxbSetGeoFormat;
            TxbSetGeoSign = locStrings.TxbSetGeoSign;
            TxbSetGeoDecimals = locStrings.TxbSetGeoDecimals;
            TxbSetPrjFormat = locStrings.TxbSetPrjFormat;
            TxbSetPrjDecimals = locStrings.TxbSetPrjDecimals;

            // Projection Methods
            PrjLambertConicConformal2SP = locStrings.PrjLambertConicConformal2SP;
            PrjLambertConicConformal1SP = locStrings.PrjLambertConicConformal1SP;
            PrjLambertConicConformalWest = locStrings.PrjLambertConicConformalWest;
            PrjLambertConicConformalBelgium = locStrings.PrjLambertConicConformalBelgium;
            PrjLambertConicNearConformal = locStrings.PrjLambertConicNearConformal;
            PrjKrovak = locStrings.PrjKrovak;
            PrjKrovakNorth = locStrings.PrjKrovakNorth;
            PrjKrovakModified = locStrings.PrjKrovakModified;
            PrjKrovakModifiedNorth = locStrings.PrjKrovakModifiedNorth;
            PrjMercatorVariantA = locStrings.PrjMercatorVariantA;
            PrjMercatorVariantB = locStrings.PrjMercatorVariantB;
            PrjMercatorVariantC = locStrings.PrjMercatorVariantC;
            PrjMercatorSpherical = locStrings.PrjMercatorSpherical;
            PrjMercatorPseudo = locStrings.PrjMercatorPseudo;
            PrjCassiniSoldner = locStrings.PrjCassiniSoldner;
            PrjCassiniSoldnerHyperbolic = locStrings.PrjCassiniSoldnerHyperbolic;
            PrjTransverseMercator = locStrings.PrjTransverseMercator;
            PrjTransverseMercatorUniversal = locStrings.PrjTransverseMercatorUniversal;
            PrjTransverseMercatorZoned = locStrings.PrjTransverseMercatorZoned;
            PrjTransverseMercatorSouth = locStrings.PrjTransverseMercatorSouth;
            PrjObliqueMercatorHotineA = locStrings.PrjObliqueMercatorHotineA;
            PrjObliqueMercatorHotineB = locStrings.PrjObliqueMercatorHotineB;
            PrjObliqueMercatorLaborde = locStrings.PrjObliqueMercatorLaborde;
            PrjStereographicOblique = locStrings.PrjStereographicOblique;
            PrjStereographicPolarA = locStrings.PrjStereographicPolarA;
            PrjStereographicPolarB = locStrings.PrjStereographicPolarB;
            PrjStereographicPolarC = locStrings.PrjStereographicPolarC;
            PrjNewZealandGrid = locStrings.PrjNewZealandGrid;
            PrjTunisiaMining = locStrings.PrjTunisiaMining;
            PrjAmericanPolyconic = locStrings.PrjAmericanPolyconic;
            PrjLambertAzimutalEqualArea = locStrings.PrjLambertAzimutalEqualArea;
            PrjLambertAzimutalEqualAreaPolar = locStrings.PrjLambertAzimutalEqualAreaPolar;
            PrjLambertAzimutalEqualAreaSpherical = locStrings.PrjLambertAzimutalEqualAreaSpherical;
            PrjLambertCylindricalEqualArea = locStrings.PrjLambertCylindricalEqualArea;
            PrjLambertCylindricalEqualAreaSpherical = locStrings.PrjLambertCylindricalEqualAreaSpherical;
            PrjAlbersEqualArea = locStrings.PrjAlbersEqualArea;
            PrjEquidistantCylindrical = locStrings.PrjEquidistantCylindrical;
            PrjEquidistantCylindricalSpherical = locStrings.PrjEquidistantCylindricalSpherical;
            PrjPseudoPlateCarree = locStrings.PrjPseudoPlateCarree;
            PrjBonne = locStrings.PrjBonne;
            PrjBonneSouth = locStrings.PrjBonneSouth;
            PrjAzimutalEquidistantModified = locStrings.PrjAzimutalEquidistantModified;
            PrjAzimutalEquidistantGuam = locStrings.PrjAzimutalEquidistantGuam;
            // Transformation Methods
            SftNone = locStrings.SftNone;
            SftGeocentric3Parameter = locStrings.SftGeocentric3Parameter;
            SftHelmert7Parameter = locStrings.SftHelmert7Parameter;
            SftMolodenskyBadekas10Parameter = locStrings.SftMolodenskyBadekas10Parameter;
            SftAbridgedMolodensky = locStrings.SftAbridgedMolodensky;
        }

        #endregion private Methods
    }
}