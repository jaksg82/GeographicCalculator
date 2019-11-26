using System;

namespace GeographicCalculatorWPFCore.Strings
{
    public class EnStrings : IStrings
    {
        #region Public properties

        // Generic Strings
        public string AppTitle { get; }

        public string NotDefined { get; }

        // UI Main Single Point
        public string TxbGeoTitle { get; }

        public string TxbGeoLatTitle { get; }
        public string TxbGeoLonTitle { get; }
        public string TxbGeoElevTitle { get; }
        public string TxbPrjTitle { get; }
        public string TxbPrjEastTitle { get; }
        public string TxbPrjNorthTitle { get; }
        public string TxbPrjElevTitle { get; }
        public string TxbCrsInTitle { get; }
        public string TxbCrsOutTitle { get; }
        public string CrsPrjName { get; }
        public string CrsPrjType { get; }
        public string CrsEllName { get; }
        public string CrsShiftName { get; }

        // Panel Settings
        public string TxbSetGeoFormat { get; }

        public string TxbSetGeoSign { get; }
        public string TxbSetGeoDecimals { get; }
        public string TxbSetPrjFormat { get; }
        public string TxbSetPrjDecimals { get; }

        // Projection Methods
        public string PrjLambertConicConformal2SP { get; }

        public string PrjLambertConicConformal1SP { get; }
        public string PrjLambertConicConformalWest { get; }
        public string PrjLambertConicConformalBelgium { get; }
        public string PrjLambertConicNearConformal { get; }
        public string PrjKrovak { get; }
        public string PrjKrovakNorth { get; }
        public string PrjKrovakModified { get; }
        public string PrjKrovakModifiedNorth { get; }
        public string PrjMercatorVariantA { get; }
        public string PrjMercatorVariantB { get; }
        public string PrjMercatorVariantC { get; }
        public string PrjMercatorSpherical { get; }
        public string PrjMercatorPseudo { get; }
        public string PrjCassiniSoldner { get; }
        public string PrjCassiniSoldnerHyperbolic { get; }
        public string PrjTransverseMercator { get; }
        public string PrjTransverseMercatorUniversal { get; }
        public string PrjTransverseMercatorZoned { get; }
        public string PrjTransverseMercatorSouth { get; }
        public string PrjObliqueMercatorHotineA { get; }
        public string PrjObliqueMercatorHotineB { get; }
        public string PrjObliqueMercatorLaborde { get; }
        public string PrjStereographicOblique { get; }
        public string PrjStereographicPolarA { get; }
        public string PrjStereographicPolarB { get; }
        public string PrjStereographicPolarC { get; }
        public string PrjNewZealandGrid { get; }
        public string PrjTunisiaMining { get; }
        public string PrjAmericanPolyconic { get; }
        public string PrjLambertAzimutalEqualArea { get; }
        public string PrjLambertAzimutalEqualAreaPolar { get; }
        public string PrjLambertAzimutalEqualAreaSpherical { get; }
        public string PrjLambertCylindricalEqualArea { get; }
        public string PrjLambertCylindricalEqualAreaSpherical { get; }
        public string PrjAlbersEqualArea { get; }
        public string PrjEquidistantCylindrical { get; }
        public string PrjEquidistantCylindricalSpherical { get; }
        public string PrjPseudoPlateCarree { get; }
        public string PrjBonne { get; }
        public string PrjBonneSouth { get; }
        public string PrjAzimutalEquidistantModified { get; }
        public string PrjAzimutalEquidistantGuam { get; }

        // Transformation Methods
        public string SftNone { get; }

        public string SftGeocentric3Parameter { get; }
        public string SftHelmert7Parameter { get; }
        public string SftMolodenskyBadekas10Parameter { get; }
        public string SftAbridgedMolodensky { get; }

        #endregion Public properties

        #region Constructors

        public EnStrings()
        {
            // Generic Strings
            AppTitle = "Geographic Calculator";
            NotDefined = "Not Defined";
            // UI Main Single Point
            TxbGeoTitle = "Geographic Coordinates";
            TxbGeoLatTitle = "Latitude:";
            TxbGeoLonTitle = "Longitude:";
            TxbGeoElevTitle = "Elevation:";
            TxbPrjTitle = "Cartesian Coordinates";
            TxbPrjEastTitle = "East:";
            TxbPrjNorthTitle = "North:";
            TxbPrjElevTitle = "Elevation:";
            TxbCrsInTitle = "Selected Input CRS";
            TxbCrsOutTitle = "Selected Output CRS";
            CrsPrjName = "Projection Name:";
            CrsPrjType = "Projection Type:";
            CrsEllName = "Ellipsoid Name:";
            CrsShiftName = "Shift Method:";
            // Panel Settings
            TxbSetGeoFormat = "Lat/Lon Format:";
            TxbSetGeoSign = "Lat/Lon Sign:";
            TxbSetGeoDecimals = "Lat/Lon Decimals:";
            TxbSetPrjFormat = "East/North Format:";
            TxbSetPrjDecimals = "East/North Decimals:";

            // Projection Methods
            PrjLambertConicConformal2SP = "Lambert Conic Conformal (2 Parallels)";
            PrjLambertConicConformal1SP = "Lambert Conic Conformal (1 Parallel)";
            PrjLambertConicConformalWest = "Lambert Conic Conformal (West Oriented)";
            PrjLambertConicConformalBelgium = "Lambert Conic Conformal (Belgium)";
            PrjLambertConicNearConformal = "Lambert Conic Near Conformal";
            PrjKrovak = "Krovak";
            PrjKrovakNorth = "Krovak (North oriented)";
            PrjKrovakModified = "Krovak Modified";
            PrjKrovakModifiedNorth = "Krovak Modified (North oriented)";
            PrjMercatorVariantA = "Mercator (Variant A)";
            PrjMercatorVariantB = "Mercator (Variant B)";
            PrjMercatorVariantC = "Mercator (Variant C)";
            PrjMercatorSpherical = "Mercator Spherical";
            PrjMercatorPseudo = "Pseudo-Mercator";
            PrjCassiniSoldner = "Cassini-Soldner";
            PrjCassiniSoldnerHyperbolic = "Cassini-Soldner Hyperbolic";
            PrjTransverseMercator = "Transverse Mercator";
            PrjTransverseMercatorUniversal = "Universal Transverse Mercator (U.T.M.)";
            PrjTransverseMercatorZoned = "Zoned Transverse Mercator";
            PrjTransverseMercatorSouth = "Transverse Mercator (South oriented)";
            PrjObliqueMercatorHotineA = "PrjObliqueMercatorHotineA";
            PrjObliqueMercatorHotineB = "PrjObliqueMercatorHotineB";
            PrjObliqueMercatorLaborde = "PrjObliqueMercatorLaborde";
            PrjStereographicOblique = "PrjStereographicOblique";
            PrjStereographicPolarA = "PrjStereographicPolarA";
            PrjStereographicPolarB = "PrjStereographicPolarB";
            PrjStereographicPolarC = "PrjStereographicPolarC";
            PrjNewZealandGrid = "PrjNewZealandGrid";
            PrjTunisiaMining = "PrjTunisiaMining";
            PrjAmericanPolyconic = "PrjAmericanPolyconic";
            PrjLambertAzimutalEqualArea = "PrjLambertAzimutalEqualArea";
            PrjLambertAzimutalEqualAreaPolar = "PrjLambertAzimutalEqualAreaPolar";
            PrjLambertAzimutalEqualAreaSpherical = "PrjLambertAzimutalEqualAreaSpherical";
            PrjLambertCylindricalEqualArea = "PrjLambertCylindricalEqualArea";
            PrjLambertCylindricalEqualAreaSpherical = "PrjLambertCylindricalEqualAreaSpherical";
            PrjAlbersEqualArea = "PrjAlbersEqualArea";
            PrjEquidistantCylindrical = "PrjEquidistantCylindrical";
            PrjEquidistantCylindricalSpherical = "PrjEquidistantCylindricalSpherical";
            PrjPseudoPlateCarree = "PrjPseudoPlateCarree";
            PrjBonne = "PrjBonne";
            PrjBonneSouth = "PrjBonneSouth";
            PrjAzimutalEquidistantModified = "PrjAzimutalEquidistantModified";
            PrjAzimutalEquidistantGuam = "PrjAzimutalEquidistantGuam";
            // Transformation Methods
            SftNone = "SftNone";
            SftGeocentric3Parameter = "SftGeocentric3Parameter";
            SftHelmert7Parameter = "StHelmert7Parameter";
            SftMolodenskyBadekas10Parameter = "SftMolodenskyBadekas10Parameter";
            SftAbridgedMolodensky = "SftAbridgedMolodensky";
        }

        #endregion Constructors
    }
}