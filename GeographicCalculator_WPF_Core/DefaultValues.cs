using GeographicCalculatorWPFCore.Strings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GeographicCalculatorWPFCore.Statics
{
    public static class DefaultValues
    {
        public static double BaseFontSize { get; set; }
        public static double BaseButtonSize { get; set; }
        public static double BaseIconSize { get; set; }
        public static double TitleFontSize { get; set; }
        public static double HalfFontSize { get; set; }
        public static double AppMinHeight { get; set; }
        public static double AppMinWidth { get; set; }

        static DefaultValues()
        {
            BaseFontSize = 12.0;
            BaseButtonSize = 48.0;
            BaseIconSize = 24.0;
            TitleFontSize = 16.0;
            HalfFontSize = BaseFontSize / 2;
            AppMinHeight = BaseButtonSize * 6;
            AppMinWidth = BaseButtonSize * 7;
        }
    }
}