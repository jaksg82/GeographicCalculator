using System;

namespace GeographicCalculatorWPFCore.Strings
{
    public class EnStrings : IStrings
    {
        #region Public properties

        // Generic Strings //
        public string AppTitle { get; }

        public string NotDefined { get; }
        public string BtnApply { get; }
        public string BtnCancel { get; }

        // UI Main Single Point //
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

        // Panel Settings //
        public string TxbSetGeoFormat { get; }

        public string TxbSetGeoSign { get; }
        public string TxbSetGeoDecimals { get; }
        public string TxbSetPrjFormat { get; }
        public string TxbSetPrjDecimals { get; }
        public string SetCrsPrj { get; }
        public string SetCrsEll { get; }
        public string SetCrsTrans { get; }
        public string SetCrsPrjFullName { get; }
        public string SetCrsPrjShortName { get; }
        public string SetCrsPrjMethod { get; }
        public string SetCrsEllEpsgId { get; }
        public string SetCrsEllFullName { get; }
        public string SetCrsEllSmAxis { get; }
        public string SetCrsEllInvFlatt { get; }
        public string SetCrsShiftMethod { get; }
        public string SetCrsShiftDx { get; }
        public string SetCrsShiftDy { get; }
        public string SetCrsShiftDz { get; }
        public string SetCrsShiftRx { get; }
        public string SetCrsShiftRy { get; }
        public string SetCrsShiftRz { get; }
        public string SetCrsShiftScale { get; }
        public string SetCrsShiftPx { get; }
        public string SetCrsShiftPy { get; }
        public string SetCrsShiftPz { get; }

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
        public string SftPositionVector { get; }
        public string SftCoordinateFrame { get; }

        // DmsFormat
        public string FormatSimpleDMS { get; }

        public string FormatSimpleDM { get; }
        public string FormatSimpleD { get; }
        public string FormatVerboseDMS { get; }
        public string FormatVerboseDM { get; }
        public string FormatVerboseD { get; }
        public string FormatSimpleR { get; }
        public string FormatEsriDMS { get; }
        public string FormatEsriDM { get; }
        public string FormatEsriD { get; }
        public string FormatEsriPackedDMS { get; }
        public string FormatUkooaDMS { get; }
        public string FormatNMEA { get; }
        public string FormatSpacedDMS { get; }
        public string FormatSpacedDM { get; }

        // DmsSign
        public string DmsSignPlusMinus { get; }

        public string DmsSignPrefix { get; }
        public string DmsSignSuffix { get; }
        public string DmsSignGeneric { get; }

        // MetricSign
        public string MetricSignNumber { get; }

        public string MetricSignUnit { get; }
        public string MetricSignPrefix { get; }
        public string MetricSignSuffix { get; }
        public string MetricSignUnitPrefix { get; }
        public string MetricSignUnitSuffix { get; }

        #endregion Public properties

        #region Constructors

        public EnStrings()
        {
            // Generic Strings //
            AppTitle = "Geographic Calculator";
            NotDefined = "Not Defined";
            BtnApply = "&#x1F5F8; Apply";
            BtnCancel = "&#x1F5F4; Cancel";
            // UI Main Single Point //
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
            // Panel Settings //
            TxbSetGeoFormat = "Lat/Lon Format:";
            TxbSetGeoSign = "Lat/Lon Sign:";
            TxbSetGeoDecimals = "Lat/Lon Decimals:";
            TxbSetPrjFormat = "East/North Format:";
            TxbSetPrjDecimals = "East/North Decimals:";
            SetCrsPrj = "Projection";
            SetCrsEll = "Ellipsoid";
            SetCrsTrans = "Transformation";
            SetCrsPrjFullName = "Name:";
            SetCrsPrjShortName = "Short Name:";
            SetCrsPrjMethod = "Method:";
            SetCrsEllEpsgId = "EPSG ID:";
            SetCrsEllFullName = "Name:";
            SetCrsEllSmAxis = "SemiMayor Axis:";
            SetCrsEllInvFlatt = "Inverse Flattening:";
            SetCrsShiftMethod = "Method:";
            SetCrsShiftDx = "Delta X:";
            SetCrsShiftDy = "Delta Y:";
            SetCrsShiftDz = "Delta Z:";
            SetCrsShiftRx = "Rotation X:";
            SetCrsShiftRy = "Rotation Y:";
            SetCrsShiftRz = "Rotation Z:";
            SetCrsShiftScale = "Scale Factor:";
            SetCrsShiftPx = "Rotation Point X:";
            SetCrsShiftPy = "Rotation Point Y:";
            SetCrsShiftPz = "Rotation Point Z:";

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
            PrjObliqueMercatorHotineA = "Oblique Mercator Hotine A";
            PrjObliqueMercatorHotineB = "Oblique Mercator Hotine B";
            PrjObliqueMercatorLaborde = "Oblique Mercator Laborde";
            PrjStereographicOblique = "Stereographic Oblique";
            PrjStereographicPolarA = "Stereographic Polar A";
            PrjStereographicPolarB = "Stereographic Polar B";
            PrjStereographicPolarC = "Stereographic Polar C";
            PrjNewZealandGrid = "New Zealand Grid";
            PrjTunisiaMining = "Tunisia Mining";
            PrjAmericanPolyconic = "American Polyconic";
            PrjLambertAzimutalEqualArea = "Lambert Azimutal Equal Area";
            PrjLambertAzimutalEqualAreaPolar = "Lambert Azimutal Equal Area Polar";
            PrjLambertAzimutalEqualAreaSpherical = "Lambert Azimutal Equal Area Spherical";
            PrjLambertCylindricalEqualArea = "Lambert Cylindrical Equal Area";
            PrjLambertCylindricalEqualAreaSpherical = "Lambert Cylindrical Equal Area Spherical";
            PrjAlbersEqualArea = "Albers Equal Area";
            PrjEquidistantCylindrical = "Equidistant Cylindrical";
            PrjEquidistantCylindricalSpherical = "Equidistant Cylindrical Spherical";
            PrjPseudoPlateCarree = "Pseudo Plate Carree";
            PrjBonne = "Bonne";
            PrjBonneSouth = "Bonne South oriented";
            PrjAzimutalEquidistantModified = "Azimutal Equidistant Modified";
            PrjAzimutalEquidistantGuam = "Azimutal Equidistant Guam";
            // Transformation Methods
            SftNone = "None";
            SftGeocentric3Parameter = "Geocentric (3 parameters)";
            SftHelmert7Parameter = "Helmert (7 parameters)";
            SftMolodenskyBadekas10Parameter = "Molodensky-Badekas (10 parameters)";
            SftAbridgedMolodensky = "Abridged Molodensky";
            SftPositionVector = "Position Vector";
            SftCoordinateFrame = "Coordinate Frame";

            // DmsFormat
            FormatSimpleDMS = "DDD:MM:SS.000";
            FormatSimpleDM = "DDD:MM.000";
            FormatSimpleD = "DDD.000";
            FormatVerboseDMS = "DDD°MM'SS.000\"";
            FormatVerboseDM = "DDD°MM.000'";
            FormatVerboseD = "DDD.000°";
            FormatSimpleR = "R.000000r";
            FormatEsriDMS = "DDD° MM' SS.000\"";
            FormatEsriDM = "DDD° MM.000'";
            FormatEsriD = "DDD.000°";
            FormatEsriPackedDMS = "DDD.MMSS000000";
            FormatUkooaDMS = "DDMMSS.00";
            FormatNMEA = "DDMM.000";
            FormatSpacedDMS = "DD MM SS.000";
            FormatSpacedDM = "DD MM.000";
            // DmsSign
            DmsSignPlusMinus = "+/-";
            DmsSignPrefix = "Prefix (E/W & N/S)";
            DmsSignSuffix = "Suffix (E/W & N/S)";
            DmsSignGeneric = "Generic angle";
            // MetricSign
            MetricSignNumber = "Simple Number";
            MetricSignUnit = "Unit (m)";
            MetricSignPrefix = "Prefix (E/W & N/S)";
            MetricSignSuffix = "Suffix (E/W & N/S)";
            MetricSignUnitPrefix = "Prefix (E/W & N/S) & Unit (m)";
            MetricSignUnitSuffix = "Unit (m) & Suffix (E/W & N/S)";
        }

        #endregion Constructors
    }
}