﻿

#ExternalChecksum("C:\Users\Simone\Dropbox\Progetti Windows8\GeographicCalculator\GeographicCalculator_WP\SrcSetPage_WP.xaml", "{406ea660-64cf-4c82-b6f0-42d48172a799}", "2C051D5B639B7875B6032D3D850C6921")
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

Namespace Global.GeographicCalculator_WP

    Partial Class SrcSetPageVB
        Implements Global.Windows.UI.Xaml.Markup.IComponentConnector

        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", "4.0.0.0")>  _
        <Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
        Public Sub Connect(ByVal connectionId As Integer, ByVal target As Global.System.Object) Implements Global.Windows.UI.Xaml.Markup.IComponentConnector.Connect
            If(connectionId = 1) Then
                #ExternalSource("..\..\SrcSetPage_WP.xaml",40)
                AddHandler CType(target,Global.Windows.UI.Xaml.Controls.Primitives.ButtonBase).Click, AddressOf Me.AcceptButton_Click
                #End ExternalSource
            Else If(connectionId = 2) Then
                #ExternalSource("..\..\SrcSetPage_WP.xaml",41)
                AddHandler CType(target,Global.Windows.UI.Xaml.Controls.Primitives.ButtonBase).Click, AddressOf Me.CancelButton_Click
                #End ExternalSource
            End If
            Me._contentLoaded = true
        End Sub
    End Class

End Namespace


