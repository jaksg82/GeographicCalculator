﻿

#ExternalChecksum("C:\Users\Simone\Dropbox\Progetti Windows8\GeographicCalculator\GeographicCalculator\DMSinput.xaml", "{406ea660-64cf-4c82-b6f0-42d48172a799}", "FAFE91C1E6EF17ECE8D639562C37EE02")
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On


Namespace Global.GeographicCalculator

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>  _
    Partial Class DMSinput
        Inherits Global.Windows.UI.Xaml.Controls.UserControl

        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", "4.0.0.0")>  _
        private WithEvents Dir As Global.Windows.UI.Xaml.Controls.ComboBox
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", "4.0.0.0")>  _
        private WithEvents Deg As Global.Windows.UI.Xaml.Controls.ComboBox
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", "4.0.0.0")>  _
        private WithEvents Min As Global.Windows.UI.Xaml.Controls.ComboBox
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", "4.0.0.0")>  _
        private WithEvents Sec As Global.GeographicCalculator.NumericBox

        Private _contentLoaded As Boolean

        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", "4.0.0.0")>  _
        <Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
        Public Sub InitializeComponent()
            If _contentLoaded Then
                Return
            End If
            _contentLoaded = true

            Dim uri As New Global.System.Uri("ms-appx:///DMSinput.xaml")
            Global.Windows.UI.Xaml.Application.LoadComponent(Me, uri, Global.Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application)

            Dir = CType(Me.FindName("Dir"), Global.Windows.UI.Xaml.Controls.ComboBox)
            Deg = CType(Me.FindName("Deg"), Global.Windows.UI.Xaml.Controls.ComboBox)
            Min = CType(Me.FindName("Min"), Global.Windows.UI.Xaml.Controls.ComboBox)
            Sec = CType(Me.FindName("Sec"), Global.GeographicCalculator.NumericBox)
        End Sub
    End Class

End Namespace

