using System;

namespace GeographicCalculatorWPFCore.Strings
{
    public class ItStrings : IStrings
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

        public ItStrings()
        {
            // Generic Strings
            AppTitle = "Calcolatrice Geografica";
            NotDefined = "Non Definito";
            // UI Main Single Point
            TxbGeoTitle = "Coordinate Geografiche";
            TxbGeoLatTitle = "Latitudine:";
            TxbGeoLonTitle = "Longitudine:";
            TxbGeoElevTitle = "Elevazione:";
            TxbPrjTitle = "Coordinate Cartesiane";
            TxbPrjEastTitle = "Est:";
            TxbPrjNorthTitle = "Nord:";
            TxbPrjElevTitle = "Elevazione:";
            TxbCrsInTitle = "CRS Origine Selezionato";
            TxbCrsOutTitle = "CRS Destinazione Selezionato";
            CrsPrjName = "Nome della proiezione:";
            CrsPrjType = "Tipo di proiezione:";
            CrsEllName = "Nome dell’elissoide:";
            CrsShiftName = "Metodo di trasformazione:";
            // Panel Settings
            TxbSetGeoFormat = "Formato Lat/Long:";
            TxbSetGeoSign = "Segno Lat/Long:";
            TxbSetGeoDecimals = "Decimali Lat/Long:";
            TxbSetPrjFormat = "Formato Est/Nord:";
            TxbSetPrjDecimals = "Decimali Est/Nord:";

            // Projection Methods
            PrjLambertConicConformal2SP = "Conica Conforme di Lambert (2 paralleli)";
            PrjLambertConicConformal1SP = "Conica Conforme di Lambert (1 parallelo)";
            PrjLambertConicConformalWest = "Conica Conforme di Lambert (Orientata ad ovest)";
            PrjLambertConicConformalBelgium = "Conica Conforme di Lambert (Belgio)";
            PrjLambertConicNearConformal = "Conica Quasi Conforme di Lambert";
            PrjKrovak = "Krovak";
            PrjKrovakNorth = "Krovak (Orientata a nord)";
            PrjKrovakModified = "Krovak Modificata";
            PrjKrovakModifiedNorth = "Krovak Modificata (Orientata a nord)";
            PrjMercatorVariantA = "Mercatore (Variante A)";
            PrjMercatorVariantB = "Mercatore (Variante B)";
            PrjMercatorVariantC = "Mercatore (Variante C)";
            PrjMercatorSpherical = "Mercatore Sferica";
            PrjMercatorPseudo = "Pseudo-Mercatore";
            PrjCassiniSoldner = "Cassini-Soldner";
            PrjCassiniSoldnerHyperbolic = "Cassini-Soldner Iperbolica";
            PrjTransverseMercator = "Trasversa di Mercatore";
            PrjTransverseMercatorUniversal = "Trasversa di Mercatore Universale (U.T.M.)";
            PrjTransverseMercatorZoned = "Trasversa di Mercatore a Zone";
            PrjTransverseMercatorSouth = "Trasversa di Mercatore (Orientata a sud)";
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