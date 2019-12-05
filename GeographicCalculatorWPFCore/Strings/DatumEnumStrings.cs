using Datums;
using GeographicCalculatorWPFCore.Strings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GeographicCalculatorWPFCore.Strings
{
    public class DatumEnumStrings
    {
        private readonly IStrings UIStrings;

        public DatumEnumStrings()
        {
            UIStrings = new StringProvider(Thread.CurrentThread.CurrentUICulture);
        }

        /// <summary>
        /// Get the localized string for the projection method
        /// </summary>
        /// <param name="prj">Given method</param>
        /// <returns>Localized string</returns>
        public string LocalizedProjectionType(Projections.Method prjType)
        {
            switch (prjType)
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

        /// <summary>
        /// Get the localized string for the transformation method
        /// </summary>
        /// <param name="sft">Given method</param>
        /// <returns>Localized string</returns>
        public string LocalizedTransformationMethod(Transformations sft)
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
        /// Get the localized string for the Lat/Lon formats
        /// </summary>
        /// <param name="format">Dms Format type</param>
        /// <returns></returns>
        public string LocalizedDmsFormat(StringFormat.Formattings.DmsFormat format)
        {
            switch (format)
            {
                case StringFormat.Formattings.DmsFormat.EsriD: return UIStrings.FormatEsriD;
                case StringFormat.Formattings.DmsFormat.EsriDM: return UIStrings.FormatEsriDM;
                case StringFormat.Formattings.DmsFormat.EsriDMS: return UIStrings.FormatEsriDMS;
                case StringFormat.Formattings.DmsFormat.EsriPackedDMS: return UIStrings.FormatEsriPackedDMS;
                case StringFormat.Formattings.DmsFormat.NMEA: return UIStrings.FormatNMEA;
                case StringFormat.Formattings.DmsFormat.SimpleD: return UIStrings.FormatSimpleD;
                case StringFormat.Formattings.DmsFormat.SimpleDM: return UIStrings.FormatSimpleDM;
                case StringFormat.Formattings.DmsFormat.SimpleDMS: return UIStrings.FormatSimpleDMS;
                case StringFormat.Formattings.DmsFormat.SimpleR: return UIStrings.FormatSimpleR;
                case StringFormat.Formattings.DmsFormat.SpacedDM: return UIStrings.FormatSpacedDM;
                case StringFormat.Formattings.DmsFormat.SpacedDMS: return UIStrings.FormatSpacedDMS;
                case StringFormat.Formattings.DmsFormat.UkooaDMS: return UIStrings.FormatUkooaDMS;
                case StringFormat.Formattings.DmsFormat.VerboseD: return UIStrings.FormatVerboseD;
                case StringFormat.Formattings.DmsFormat.VerboseDM: return UIStrings.FormatVerboseDM;
                case StringFormat.Formattings.DmsFormat.VerboseDMS: return UIStrings.FormatVerboseDMS;

                default: return UIStrings.NotDefined;
            }
        }

        /// <summary>
        /// Get the localized string for the Lat/Lon signs
        /// </summary>
        /// <param name="sign">Dms Sign type</param>
        /// <returns></returns>
        public string LocalizedDmsSign(StringFormat.Formattings.DmsSign sign)
        {
            switch (sign)
            {
                case StringFormat.Formattings.DmsSign.Generic: return UIStrings.DmsSignGeneric;
                case StringFormat.Formattings.DmsSign.PlusMinus: return UIStrings.DmsSignPlusMinus;
                case StringFormat.Formattings.DmsSign.Prefix: return UIStrings.DmsSignPrefix;
                case StringFormat.Formattings.DmsSign.Suffix: return UIStrings.DmsSignSuffix;

                default: return UIStrings.NotDefined;
            }
        }

        /// <summary>
        /// Get the localized string for the Metric coord signs
        /// </summary>
        /// <param name="sign">Metric Sign type</param>
        /// <returns></returns>
        public string LocalizedMetricSign(StringFormat.Formattings.MetricSign sign)
        {
            switch (sign)
            {
                case StringFormat.Formattings.MetricSign.Number: return UIStrings.MetricSignNumber;
                case StringFormat.Formattings.MetricSign.Prefix: return UIStrings.MetricSignPrefix;
                case StringFormat.Formattings.MetricSign.Suffix: return UIStrings.MetricSignSuffix;
                case StringFormat.Formattings.MetricSign.Unit: return UIStrings.MetricSignUnit;
                case StringFormat.Formattings.MetricSign.UnitPrefix: return UIStrings.MetricSignUnitPrefix;
                case StringFormat.Formattings.MetricSign.UnitSuffix: return UIStrings.MetricSignUnitSuffix;

                default: return UIStrings.NotDefined;
            }
        }
    }
}