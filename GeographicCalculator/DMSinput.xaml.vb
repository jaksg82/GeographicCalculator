' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

Public NotInheritable Class DMSinput
    Inherits UserControl

    Public Enum AngleFormat
        DMS = 0
        DM = 1
        D = 2
    End Enum

    Public Enum DegreeSign
        PlusMinus = 0
        EastWest = 1
        NorthSouth = 2
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

    Public Property MinValue As Double
        Get
            Return MinRad
        End Get
        Set(value As Double)
            MinRad = If(value > MaxRad, MaxRad - JakMathLib.DegRad(1), value)
            OnPropertyChanged("MinValue")
        End Set
    End Property

    Public Property MaxValue As Double
        Get
            Return MaxRad
        End Get
        Set(value As Double)
            MaxRad = If(value < MinRad, MinRad + JakMathLib.DegRad(1), value)
            OnPropertyChanged("MaxValue")
        End Set
    End Property

    Public Property Decimals As Integer
        Get
            Return DecCount
        End Get
        Set(value As Integer)
            DecCount = If(value > 9, 9, If(value < 0, 0, value))
            OnPropertyChanged("Decimals")
        End Set
    End Property

    Dim Num0, MinRad, MaxRad As Double
    Dim Fmt0 As AngleFormat
    Dim DecCount As Integer
    Dim SuspendedUpdates, IsDegreePositive As Boolean
    Dim DegSign As DegreeSign

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Fmt0 = AngleFormat.DMS
        DegSign = DegreeSign.PlusMinus
        DecCount = 4
        SuspendedUpdates = False
        Num0 = 0.0
        Deg0.Text = "+"
        IsDegreePositive = True
        MinRad = -Math.PI
        MaxRad = Math.PI

    End Sub

    Public Sub New(Optional Format As AngleFormat = AngleFormat.DMS, Optional DegreeSign As DegreeSign = DegreeSign.PlusMinus)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Fmt0 = Format
        DegSign = DegreeSign
        SuspendedUpdates = True
        Num0 = 0.0
        IsDegreePositive = True
        Select Case DegSign
            Case DMSinput.DegreeSign.EastWest
                Deg0.Text = "E"
            Case DMSinput.DegreeSign.NorthSouth
                Deg0.Text = "N"
            Case Else
                Deg0.Text = "+"
        End Select
        UpdateUI()

    End Sub

    Private Sub DMSinput_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Update the UI
        SuspendedUpdates = True
        UpdateUI()
    End Sub

    Protected Sub OnPropertyChanged(ByVal name As String)
        'Update the UI
        SuspendedUpdates = True
        UpdateUI()
    End Sub

    Public Function SetAspect(Format As AngleFormat, DegreeSign As DegreeSign) As Boolean
        Fmt0 = Format
        DegSign = DegreeSign
        Select Case DegSign
            Case DMSinput.DegreeSign.EastWest
                Deg0.Text = If(IsDegreePositive, "E", "W")
            Case DMSinput.DegreeSign.NorthSouth
                Deg0.Text = If(IsDegreePositive, "N", "S")
            Case Else
                Deg0.Text = If(IsDegreePositive, "+", "-")
        End Select
        'Update the UI
        SuspendedUpdates = True
        UpdateUI()
        Return True

    End Function

    Private Sub Deg0_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles Deg0.Tapped
        Dim Num1 As Double
        Num1 = -Num0
        If IsInsideRange(Num1) Then
            IsDegreePositive = Not IsDegreePositive
            Num0 = Num1
        End If
        Select Case DegSign
            Case DMSinput.DegreeSign.EastWest
                Deg0.Text = If(IsDegreePositive, "E", "W")
            Case DMSinput.DegreeSign.NorthSouth
                Deg0.Text = If(IsDegreePositive, "N", "S")
            Case Else
                Deg0.Text = If(IsDegreePositive, "+", "-")
        End Select
        'Update the UI
        SuspendedUpdates = True
        UpdateUI()

    End Sub

    Private Sub ValueChangedByUser() Handles Dec1.SelectedValueChanged, Dec2.SelectedValueChanged, Dec3.SelectedValueChanged, _
        Dec4.SelectedValueChanged, Dec5.SelectedValueChanged, Dec6.SelectedValueChanged, Dec7.SelectedValueChanged, _
        Dec8.SelectedValueChanged, Dec9.SelectedValueChanged, Deg1.SelectedValueChanged, Deg2.SelectedValueChanged, _
        Deg3.SelectedValueChanged, Min1.SelectedValueChanged, Min2.SelectedValueChanged, Sec1.SelectedValueChanged, _
        Sec2.SelectedValueChanged

        Dim AngleString As String = ""
        Dim Num1 As Double

        'Check if the changes are made by the user interaction
        If SuspendedUpdates = False Then
            'Get the sign part of the string
            If Deg0.Text = "E" Or Deg0.Text = "N" Or Deg0.Text = "+" Then
                AngleString = "+"
            Else
                AngleString = "-"
            End If
            'Get the Degree part of the string
            AngleString = AngleString & Deg1.SelectedValue.ToString & Deg2.SelectedValue.ToString & Deg3.SelectedValue.ToString
            'Get the Minutes
            If Fmt0 <> AngleFormat.D Then
                AngleString = AngleString & ":" & Min1.SelectedValue.ToString & Min2.SelectedValue.ToString
                If Fmt0 = AngleFormat.DMS Then
                    AngleString = AngleString & ":" & Sec1.SelectedValue.ToString & Sec2.SelectedValue.ToString
                End If
            End If

            'Get the decimal part of the string
            Select Case DecCount
                Case 0
                    'do nothing
                Case 1
                    AngleString = AngleString & "." & Dec1.SelectedValue.ToString
                Case 2
                    AngleString = AngleString & "." & Dec1.SelectedValue.ToString & Dec2.SelectedValue.ToString
                Case 3
                    AngleString = AngleString & "." & Dec1.SelectedValue.ToString & Dec2.SelectedValue.ToString & Dec3.SelectedValue.ToString
                Case 4
                    AngleString = AngleString & "." & Dec1.SelectedValue.ToString & Dec2.SelectedValue.ToString & Dec3.SelectedValue.ToString
                    AngleString = AngleString & Dec4.SelectedValue.ToString
                Case 5
                    AngleString = AngleString & "." & Dec1.SelectedValue.ToString & Dec2.SelectedValue.ToString & Dec3.SelectedValue.ToString
                    AngleString = AngleString & Dec4.SelectedValue.ToString & Dec5.SelectedValue.ToString
                Case 6
                    AngleString = AngleString & "." & Dec1.SelectedValue.ToString & Dec2.SelectedValue.ToString & Dec3.SelectedValue.ToString
                    AngleString = AngleString & Dec4.SelectedValue.ToString & Dec5.SelectedValue.ToString & Dec6.SelectedValue.ToString
                Case 7
                    AngleString = AngleString & "." & Dec1.SelectedValue.ToString & Dec2.SelectedValue.ToString & Dec3.SelectedValue.ToString
                    AngleString = AngleString & Dec4.SelectedValue.ToString & Dec5.SelectedValue.ToString & Dec6.SelectedValue.ToString
                    AngleString = AngleString & Dec7.SelectedValue.ToString
                Case 8
                    AngleString = AngleString & "." & Dec1.SelectedValue.ToString & Dec2.SelectedValue.ToString & Dec3.SelectedValue.ToString
                    AngleString = AngleString & Dec4.SelectedValue.ToString & Dec5.SelectedValue.ToString & Dec6.SelectedValue.ToString
                    AngleString = AngleString & Dec7.SelectedValue.ToString & Dec8.SelectedValue.ToString
                Case 9
                    AngleString = AngleString & "." & Dec1.SelectedValue.ToString & Dec2.SelectedValue.ToString & Dec3.SelectedValue.ToString
                    AngleString = AngleString & Dec4.SelectedValue.ToString & Dec5.SelectedValue.ToString & Dec6.SelectedValue.ToString
                    AngleString = AngleString & Dec7.SelectedValue.ToString & Dec8.SelectedValue.ToString & Dec9.SelectedValue.ToString
            End Select
            'Parse the result string
            Select Case Fmt0
                Case AngleFormat.DMS
                    Num1 = JakStringFormats.DmsParse(AngleString, JakStringFormats.DmsFormat.SimpleDMS, JakStringFormats.DmsSign.PlusMinus)
                Case AngleFormat.DM
                    Num1 = JakStringFormats.DmsParse(AngleString, JakStringFormats.DmsFormat.SimpleDM, JakStringFormats.DmsSign.PlusMinus)
                Case AngleFormat.D
                    Num1 = JakStringFormats.DmsParse(AngleString, JakStringFormats.DmsFormat.SimpleD, JakStringFormats.DmsSign.PlusMinus)
            End Select
            'Check the result
            Num0 = If(JakMathLib.IsFinite(Num1), If(IsInsideRange(Num1), Num1, Num0), Num0)
        End If
        'Update the UI
        SuspendedUpdates = True
        UpdateUI()

    End Sub

    Private Sub UpdateUI()
        Dim TempString, SplitString(), DecString, DmsString() As String
        Dim TmpWidth As Double
        'Make visible only the needed comboboxes

        Dec1.Visibility = If(DecCount < 1, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)
        Dec2.Visibility = If(DecCount < 2, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)
        Dec3.Visibility = If(DecCount < 3, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)
        Dec4.Visibility = If(DecCount < 4, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)
        Dec5.Visibility = If(DecCount < 5, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)
        Dec6.Visibility = If(DecCount < 6, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)
        Dec7.Visibility = If(DecCount < 7, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)
        Dec8.Visibility = If(DecCount < 8, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)
        Dec9.Visibility = If(DecCount < 9, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)

        Select Case Fmt0
            Case AngleFormat.DM
                DegField.Visibility = Windows.UI.Xaml.Visibility.Visible
                MinField.Visibility = Windows.UI.Xaml.Visibility.Visible
                SecField.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                TmpWidth = DegField.Width + MinField.Width
                TempString = JakStringFormats.FormatDMS(Num0, JakStringFormats.DmsFormat.SimpleDM, JakStringFormats.DmsSign.PlusMinus, DecCount, False)
            Case AngleFormat.D
                DegField.Visibility = Windows.UI.Xaml.Visibility.Visible
                MinField.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SecField.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                TmpWidth = DegField.Width
                TempString = JakStringFormats.FormatDMS(Num0, JakStringFormats.DmsFormat.SimpleD, JakStringFormats.DmsSign.PlusMinus, DecCount, False)
            Case Else
                DegField.Visibility = Windows.UI.Xaml.Visibility.Visible
                MinField.Visibility = Windows.UI.Xaml.Visibility.Visible
                SecField.Visibility = Windows.UI.Xaml.Visibility.Visible
                TmpWidth = DegField.Width + MinField.Width + SecField.Width
                TempString = JakStringFormats.FormatDMS(Num0, JakStringFormats.DmsFormat.SimpleDMS, JakStringFormats.DmsSign.PlusMinus, DecCount, False)
        End Select
        'Update the width of the control
        Me.Width = TmpWidth + (Dec1.Width * DecCount)

        'Split string in Degrees Minutes and Seconds
        SplitString = TempString.Split(CChar("."))
        DecString = SplitString(1)
        DmsString = SplitString(0).Split(CChar(":"))
        'Select the default value
        'Deg0.Text = DmsString(0).Substring(0, 1)
        If DmsString(0).Length = 3 Then
            'Only two numeric values
            Deg1.SelectedValue = 0
            Deg2.SelectedValue = CInt(DmsString(0).Substring(1, 1))
            Deg3.SelectedValue = CInt(DmsString(0).Substring(2, 1))
        Else
            'Three numeric values
            Deg1.SelectedValue = CInt(DmsString(0).Substring(1, 1))
            Deg2.SelectedValue = CInt(DmsString(0).Substring(2, 1))
            Deg3.SelectedValue = CInt(DmsString(0).Substring(3, 1))
        End If
        If Fmt0 <> AngleFormat.D Then
            Min1.SelectedValue = CInt(DmsString(1).Substring(0, 1))
            Min2.SelectedValue = CInt(DmsString(1).Substring(1, 1))
            If Fmt0 = AngleFormat.DMS Then
                Sec1.SelectedValue = CInt(DmsString(2).Substring(0, 1))
                Sec2.SelectedValue = CInt(DmsString(2).Substring(1, 1))
            End If
        End If

        Dec1.SelectedValue = If(DecCount < 1, 0, CInt(DecString.Substring(0, 1)))
        Dec2.SelectedValue = If(DecCount < 2, 0, CInt(DecString.Substring(1, 1)))
        Dec3.SelectedValue = If(DecCount < 3, 0, CInt(DecString.Substring(2, 1)))
        Dec4.SelectedValue = If(DecCount < 4, 0, CInt(DecString.Substring(3, 1)))
        Dec5.SelectedValue = If(DecCount < 5, 0, CInt(DecString.Substring(4, 1)))
        Dec6.SelectedValue = If(DecCount < 6, 0, CInt(DecString.Substring(5, 1)))
        Dec7.SelectedValue = If(DecCount < 7, 0, CInt(DecString.Substring(6, 1)))
        Dec8.SelectedValue = If(DecCount < 8, 0, CInt(DecString.Substring(7, 1)))
        Dec9.SelectedValue = If(DecCount < 9, 0, CInt(DecString.Substring(8, 1)))

        'Re Enable the events
        SuspendedUpdates = False

    End Sub

    Private Function IsInsideRange(value As Double) As Boolean
        If value >= MinRad Then
            If value <= MaxRad Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

End Class
