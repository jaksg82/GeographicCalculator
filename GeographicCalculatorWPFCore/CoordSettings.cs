using System;
using System.Collections.Generic;
using System.Text;
using StringFormat;

namespace GeographicCalculatorWPFCore
{
    public class CoordSettings
    {
        public Formattings.DmsFormat LatLonFormat { get; set; }
        public Formattings.DmsSign LatLonSign { get; set; }
        public Formattings.MetricSign MetricFormat { get; set; }
        public int LatLonDecimals { get; set; }
        public int MetricDecimals { get; set; }

        public CoordSettings()
        {
            LatLonFormat = Formattings.DmsFormat.SimpleDMS;
            LatLonSign = Formattings.DmsSign.Suffix;
            LatLonDecimals = 4;
            MetricFormat = Formattings.MetricSign.Suffix;
            MetricDecimals = 3;
        }

        public CoordSettings(Formattings.DmsFormat llformat, Formattings.DmsSign llsign, Formattings.MetricSign msign)
        {
            LatLonFormat = llformat;
            LatLonSign = llsign;
            LatLonDecimals = 4;
            MetricFormat = msign;
            MetricDecimals = 3;
        }

        public CoordSettings(Formattings.DmsFormat llformat, Formattings.DmsSign llsign, int lldec, Formattings.MetricSign msign, int mdec)
        {
            LatLonFormat = llformat;
            LatLonSign = llsign;
            LatLonDecimals = lldec;
            MetricFormat = msign;
            MetricDecimals = mdec;
        }
    }
}