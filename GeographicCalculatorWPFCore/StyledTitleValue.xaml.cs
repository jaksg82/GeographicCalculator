using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeographicCalculatorWPFCore
{
    /// <summary>
    /// Logica di interazione per StyledTitleValue.xaml
    /// </summary>
    public partial class StyledTitleValue : UserControl
    {
        public string TitleString { get; set; }
        public string ValueString { get; set; }
        public DynamicResourceExtension TitleStyle { get; set; }
        public DynamicResourceExtension ValueStyle { get; set; }

        public StyledTitleValue()
        {
            InitializeComponent();
            TitleString = "TitleString";
            ValueString = "ValueString";
            TitleStyle = (DynamicResourceExtension)Resources["InOutTitle"];
            ValueStyle = (DynamicResourceExtension)Resources["InOutValue"];
        }

        public StyledTitleValue(string title, string value)
        {
            InitializeComponent();
            TitleString = title;
            ValueString = value;
            TitleStyle = (DynamicResourceExtension)Resources["InOutTitle"];
            ValueStyle = (DynamicResourceExtension)Resources["InOutValue"];
        }

        public StyledTitleValue(string title, string value, DynamicResourceExtension styleTitle, DynamicResourceExtension styleValue)
        {
            InitializeComponent();
            TitleString = title;
            ValueString = value;
            TitleStyle = styleTitle;
            ValueStyle = styleValue;
        }
    }
}