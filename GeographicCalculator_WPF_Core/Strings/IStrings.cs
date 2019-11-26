﻿namespace GeographicCalculatorWPFCore.Strings
{
    public interface IStrings
    {
        // Generic Strings
        string AppTitle { get; }

        string NotDefined { get; }

        // UI Main Single Point
        string TxbGeoTitle { get; }

        string TxbGeoLatTitle { get; }
        string TxbGeoLonTitle { get; }
        string TxbGeoElevTitle { get; }
        string TxbPrjTitle { get; }
        string TxbPrjEastTitle { get; }
        string TxbPrjNorthTitle { get; }
        string TxbPrjElevTitle { get; }
        string TxbCrsInTitle { get; }
        string TxbCrsOutTitle { get; }
        string CrsPrjName { get; }
        string CrsPrjType { get; }
        string CrsEllName { get; }
        string CrsShiftName { get; }

        // Panel Settings
        string TxbSetGeoFormat { get; }

        string TxbSetGeoSign { get; }
        string TxbSetGeoDecimals { get; }
        string TxbSetPrjFormat { get; }
        string TxbSetPrjDecimals { get; }

        // Projection Methods
        string PrjLambertConicConformal2SP { get; }

        string PrjLambertConicConformal1SP { get; }
        string PrjLambertConicConformalWest { get; }
        string PrjLambertConicConformalBelgium { get; }
        string PrjLambertConicNearConformal { get; }
        string PrjKrovak { get; }
        string PrjKrovakNorth { get; }
        string PrjKrovakModified { get; }
        string PrjKrovakModifiedNorth { get; }
        string PrjMercatorVariantA { get; }
        string PrjMercatorVariantB { get; }
        string PrjMercatorVariantC { get; }
        string PrjMercatorSpherical { get; }
        string PrjMercatorPseudo { get; }
        string PrjCassiniSoldner { get; }
        string PrjCassiniSoldnerHyperbolic { get; }
        string PrjTransverseMercator { get; }
        string PrjTransverseMercatorUniversal { get; }
        string PrjTransverseMercatorZoned { get; }
        string PrjTransverseMercatorSouth { get; }
        string PrjObliqueMercatorHotineA { get; }
        string PrjObliqueMercatorHotineB { get; }
        string PrjObliqueMercatorLaborde { get; }
        string PrjStereographicOblique { get; }
        string PrjStereographicPolarA { get; }
        string PrjStereographicPolarB { get; }
        string PrjStereographicPolarC { get; }
        string PrjNewZealandGrid { get; }
        string PrjTunisiaMining { get; }
        string PrjAmericanPolyconic { get; }
        string PrjLambertAzimutalEqualArea { get; }
        string PrjLambertAzimutalEqualAreaPolar { get; }
        string PrjLambertAzimutalEqualAreaSpherical { get; }
        string PrjLambertCylindricalEqualArea { get; }
        string PrjLambertCylindricalEqualAreaSpherical { get; }
        string PrjAlbersEqualArea { get; }
        string PrjEquidistantCylindrical { get; }
        string PrjEquidistantCylindricalSpherical { get; }
        string PrjPseudoPlateCarree { get; }
        string PrjBonne { get; }
        string PrjBonneSouth { get; }
        string PrjAzimutalEquidistantModified { get; }
        string PrjAzimutalEquidistantGuam { get; }

        // Transformation Methods
        string SftNone { get; }

        string SftGeocentric3Parameter { get; }
        string SftHelmert7Parameter { get; }
        string SftMolodenskyBadekas10Parameter { get; }
        string SftAbridgedMolodensky { get; }
    }
}