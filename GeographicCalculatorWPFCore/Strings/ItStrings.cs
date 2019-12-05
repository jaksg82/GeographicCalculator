using System;

namespace GeographicCalculatorWPFCore.Strings
{
    public class ItStrings : IStrings
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

        public ItStrings()
        {
            // Generic Strings //
            AppTitle = "Calcolatrice Geografica";
            NotDefined = "Non Definito";
            BtnApply = "&#x1F5F8; Applica";
            BtnCancel = "&#x1F5F4; Annulla";
            // UI Main Single Point //
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
            // Panel Settings //
            TxbSetGeoFormat = "Formato Lat/Long:";
            TxbSetGeoSign = "Segno Lat/Long:";
            TxbSetGeoDecimals = "Decimali Lat/Long:";
            TxbSetPrjFormat = "Formato Est/Nord:";
            TxbSetPrjDecimals = "Decimali Est/Nord:";
            SetCrsPrj = "Proiezione";
            SetCrsEll = "Ellissoide";
            SetCrsTrans = "Trasformazione";
            SetCrsPrjFullName = "Nome:";
            SetCrsPrjShortName = "Abbreviazione:";
            SetCrsPrjMethod = "Metodo:";
            SetCrsEllEpsgId = "EPSG ID:";
            SetCrsEllFullName = "Nome:";
            SetCrsEllSmAxis = "Semi asse Maggiore:";
            SetCrsEllInvFlatt = "Appiattimento inverso:";
            SetCrsShiftMethod = "Metodo:";
            SetCrsShiftDx = "Delta X:";
            SetCrsShiftDy = "Delta Y:";
            SetCrsShiftDz = "Delta Z:";
            SetCrsShiftRx = "Rotazione X:";
            SetCrsShiftRy = "Rotazione Y:";
            SetCrsShiftRz = "Rotazione Z:";
            SetCrsShiftScale = "Fattore di Scala:";
            SetCrsShiftPx = "Punto di rotazione X:";
            SetCrsShiftPy = "Punto di rotazione Y:";
            SetCrsShiftPz = "Punto di rotazione Z:";

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
            PrjObliqueMercatorHotineA = "Obliqua di Mercatore Hotine (Variante A)";
            PrjObliqueMercatorHotineB = "Obliqua di Mercatore Hotine (Variante B)";
            PrjObliqueMercatorLaborde = "Obliqua di Mercatore Laborde";
            PrjStereographicOblique = "Stereografica Obliqua";
            PrjStereographicPolarA = "Stereografica Polare (Variante A)";
            PrjStereographicPolarB = "Stereografica Polare (Variante B)";
            PrjStereographicPolarC = "Stereografica Polare (Variante C)";
            PrjNewZealandGrid = "Griglia della Nuova Zelanda";
            PrjTunisiaMining = "Miniere della Tunisia";
            PrjAmericanPolyconic = "Policonica Americana";
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
            SftNone = "Nessuno";
            SftGeocentric3Parameter = "Geocentrico (3 parametri)";
            SftHelmert7Parameter = "Helmert (7 parametri)";
            SftMolodenskyBadekas10Parameter = "Molodensky-Badekas (10 parametri)";
            SftAbridgedMolodensky = "Molodensky sintetica";
            SftPositionVector = "Vettore di posizione";
            SftCoordinateFrame = "Cornice di coordinate";

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
            DmsSignPrefix = "Prefisso (E/W & N/S)";
            DmsSignSuffix = "Suffisso (E/W & N/S)";
            DmsSignGeneric = "Angolo generico";
            // MetricSign
            MetricSignNumber = "Numero semplice";
            MetricSignUnit = "Unità (m)";
            MetricSignPrefix = "Prefisso (E/W & N/S)";
            MetricSignSuffix = "Suffisso (E/W & N/S)";
            MetricSignUnitPrefix = "Prefisso (E/W & N/S) & Unità (m)";
            MetricSignUnitSuffix = "Unità (m) & Suffisso (E/W & N/S)";
        }

        #endregion Constructors
    }
}