' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

Public NotInheritable Class DMSinput
    Inherits UserControl

    Public Enum AngleAxis
        Generic = 0
        Longitude = 1
        Latitude = 2
    End Enum

    Public Enum AngleFormat
        DMS = 0
        DM = 1
    End Enum

    Public Property Radians As Double
        Get
            Return Num0
        End Get
        Set(Radians As Double)
            If JakMathLib.IsFinite(Radians) Then
                Num0 = Radians
            Else
                Num0 = 0
            End If
        End Set
    End Property

    Public Property Degrees As Double
        Get
            Return JakMathLib.RadDeg(Num0)
        End Get
        Set(Degrees As Double)
            If JakMathLib.IsFinite(Degrees) Then
                Num0 = JakMathLib.DegRad(Degrees)
            Else
                Num0 = 0
            End If
        End Set
    End Property

    Dim Num0 As Double
    Dim Dir0 As AngleAxis
    Dim Fmt0 As AngleFormat

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dir0 = AngleAxis.Generic
        Fmt0 = AngleFormat.DMS

    End Sub

    Public Sub New(Format As AngleFormat, Axis As AngleAxis)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dir0 = Axis
        Fmt0 = Format

    End Sub

    Private Sub DMSinput_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim MaxDeg, iDeg, iMin As Integer
        Dim iSec As Double
        Dim TempString, SplitString() As String
        Dim TmpSign As JakStringFormats.DmsSign

        Select Case Dir0
            Case AngleAxis.Latitude
                Dir.Items.Add("N")
                Dir.Items.Add("S")
                MaxDeg = 90
                TmpSign = JakStringFormats.DmsSign.Prefix
            Case AngleAxis.Longitude
                Dir.Items.Add("E")
                Dir.Items.Add("W")
                MaxDeg = 180
                TmpSign = JakStringFormats.DmsSign.Prefix
            Case Else
                Dir.Items.Add("+")
                Dir.Items.Add("-")
                MaxDeg = 360
                TmpSign = JakStringFormats.DmsSign.PlusMinus
        End Select
        'Fill the Deg and Min comboboxes
        For d = 0 To MaxDeg
            Deg.Items.Add(d.ToString("000"))
        Next
        For m = 0 To 59
            Min.Items.Add(m.ToString("00"))
        Next

        If Fmt0 = AngleFormat.DMS Then
            TempString = JakStringFormats.FormatDMS(Num0, JakStringFormats.DmsFormat.SimpleDMS, TmpSign, 4, If(Dir0 = AngleAxis.Latitude, True, False))
        Else
            Min.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            TempString = JakStringFormats.FormatDMS(Num0, JakStringFormats.DmsFormat.SimpleDM, TmpSign, 4, If(Dir0 = AngleAxis.Latitude, True, False))
        End If
        'Select the correct axis
        Select Case TempString.Substring(0, 1)
            Case "N", "E", "+"
                Dir.SelectedIndex = 0
            Case Else
                Dir.SelectedIndex = 1
        End Select
        'Strip the axis from the string
        TempString = TempString.Substring(1)
        'Split string in Degrees Minutes and Seconds
        If Fmt0 = AngleFormat.DMS Then
            SplitString = TempString.Split(CChar(":"))
            iDeg = CInt(SplitString(0))
            iMin = CInt(SplitString(1))
            iSec = CDbl(SplitString(2))
            'Put the result in the fields
            Deg.SelectedIndex = iDeg
            Min.SelectedIndex = iMin
            Sec.value = iSec
        Else
            SplitString = TempString.Split(CChar(":"))
            iDeg = CInt(SplitString(0))
            iSec = CDbl(SplitString(1))
            'Put the result in the fields
            Deg.SelectedIndex = iDeg
            Sec.value = iSec
        End If

    End Sub
End Class
