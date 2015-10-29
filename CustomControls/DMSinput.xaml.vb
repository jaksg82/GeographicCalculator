' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

Public NotInheritable Class DMSinput
    Inherits UserControl

    Public Enum AngleFormat
        DMS = 0
        DM = 1
        D = 2
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
    Dim Fmt0 As AngleFormat
    Dim MaxDeg, DecCount As Integer
    Dim SuspendedUpdates As Boolean

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Fmt0 = AngleFormat.DMS

    End Sub

    Public Sub New(Optional Format As AngleFormat = AngleFormat.DMS, Optional MaxDegrees As Integer = 180, Optional Decimals As Integer = 4)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Fmt0 = Format
        DecCount = If(Decimals > 9, 9, Decimals)
        MaxDeg = MaxDegrees
        SuspendedUpdates = False

    End Sub

    Private Sub DMSinput_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        'Make sure there is no items in Deg, Min and Sec
        Deg.Items.Clear()
        Min.Items.Clear()
        Sec.Items.Clear()

        'Fill the Deg combobox
        If MaxDeg > 99 Then
            'Three numbers needed
            For d = MaxDeg + 1 To 0 Step -1
                Deg.Items.Add(d.ToString("-000"))
            Next
            For d = 0 To MaxDeg - 1
                Deg.Items.Add(d.ToString("+000"))
            Next
        Else
            'Two numbers needed
            For d = MaxDeg + 1 To 0 Step -1
                Deg.Items.Add(d.ToString("-00"))
            Next
            For d = 0 To MaxDeg - 1
                Deg.Items.Add(d.ToString("+00"))
            Next
        End If
        'Fill the Min and Sec comboboxes
        For m = 0 To 59
            Min.Items.Add(m.ToString("00"))
            Sec.Items.Add(m.ToString("00"))
        Next
        'Update the UI
        SuspendedUpdates = True
        UpdateUI()

    End Sub

    Private Sub ValueChangedByUser() Handles Dec1.SelectionChanged, Dec2.SelectionChanged, Dec3.SelectionChanged, _
        Dec4.SelectionChanged, Dec5.SelectionChanged, Dec6.SelectionChanged, Dec7.SelectionChanged, _
        Dec8.SelectionChanged, Dec9.SelectionChanged, Deg.SelectionChanged, Min.SelectionChanged, Sec.SelectionChanged

        Dim AngleString As String = ""
        Dim Num1 As Double

        'Check if the changes are made by the user interaction
        If SuspendedUpdates = False Then
            'Get the degree part of the string
            Select Case Fmt0
                Case AngleFormat.DMS
                    AngleString = Deg.SelectedValue.ToString & ":" & Min.SelectedValue.ToString & ":" & Sec.SelectedValue.ToString
                Case AngleFormat.DM
                    AngleString = Deg.SelectedValue.ToString & ":" & Min.SelectedValue.ToString
                Case AngleFormat.D
                    AngleString = Deg.SelectedValue.ToString
            End Select

            'Get the decimal part of the string
            Select Case DecCount
                Case 0
                    'do nothing
                Case 1
                    AngleString = AngleString & "." & Dec1.SelectedIndex.ToString
                Case 2
                    AngleString = AngleString & "." & Dec1.SelectedIndex.ToString & Dec2.SelectedIndex.ToString
                Case 3
                    AngleString = AngleString & "." & Dec1.SelectedIndex.ToString & Dec2.SelectedIndex.ToString & Dec3.SelectedIndex.ToString
                Case 4
                    AngleString = AngleString & "." & Dec1.SelectedIndex.ToString & Dec2.SelectedIndex.ToString & Dec3.SelectedIndex.ToString
                    AngleString = AngleString & Dec4.SelectedIndex.ToString
                Case 5
                    AngleString = AngleString & "." & Dec1.SelectedIndex.ToString & Dec2.SelectedIndex.ToString & Dec3.SelectedIndex.ToString
                    AngleString = AngleString & Dec4.SelectedIndex.ToString & Dec5.SelectedIndex.ToString
                Case 6
                    AngleString = AngleString & "." & Dec1.SelectedIndex.ToString & Dec2.SelectedIndex.ToString & Dec3.SelectedIndex.ToString
                    AngleString = AngleString & Dec4.SelectedIndex.ToString & Dec5.SelectedIndex.ToString & Dec6.SelectedIndex.ToString
                Case 7
                    AngleString = AngleString & "." & Dec1.SelectedIndex.ToString & Dec2.SelectedIndex.ToString & Dec3.SelectedIndex.ToString
                    AngleString = AngleString & Dec4.SelectedIndex.ToString & Dec5.SelectedIndex.ToString & Dec6.SelectedIndex.ToString
                    AngleString = AngleString & Dec7.SelectedIndex.ToString
                Case 8
                    AngleString = AngleString & "." & Dec1.SelectedIndex.ToString & Dec2.SelectedIndex.ToString & Dec3.SelectedIndex.ToString
                    AngleString = AngleString & Dec4.SelectedIndex.ToString & Dec5.SelectedIndex.ToString & Dec6.SelectedIndex.ToString
                    AngleString = AngleString & Dec7.SelectedIndex.ToString & Dec8.SelectedIndex.ToString
                Case 9
                    AngleString = AngleString & "." & Dec1.SelectedIndex.ToString & Dec2.SelectedIndex.ToString & Dec3.SelectedIndex.ToString
                    AngleString = AngleString & Dec4.SelectedIndex.ToString & Dec5.SelectedIndex.ToString & Dec6.SelectedIndex.ToString
                    AngleString = AngleString & Dec7.SelectedIndex.ToString & Dec8.SelectedIndex.ToString & Dec9.SelectedIndex.ToString
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
            Num0 = If(JakMathLib.IsFinite(Num1), Num1, Num0)
        End If

    End Sub

    Private Sub UpdateUI()
        Dim TempString, SplitString(), DecString As String
        'Make visible only the needed comboboxes
        Select Case DecCount
            Case 0
                Dec1.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec2.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec3.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec4.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec5.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec6.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec7.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec8.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec9.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            Case 1
                Dec1.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec2.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec3.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec4.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec5.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec6.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec7.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec8.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec9.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            Case 2
                Dec1.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec2.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec3.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec4.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec5.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec6.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec7.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec8.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec9.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            Case 3
                Dec1.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec2.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec3.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec4.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec5.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec6.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec7.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec8.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec9.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            Case 4
                Dec1.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec2.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec3.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec4.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec5.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec6.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec7.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec8.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec9.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            Case 5
                Dec1.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec2.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec3.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec4.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec5.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec6.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec7.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec8.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec9.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            Case 6
                Dec1.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec2.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec3.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec4.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec5.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec6.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec7.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec8.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec9.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            Case 7
                Dec1.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec2.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec3.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec4.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec5.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec6.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec7.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec8.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec9.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            Case 8
                Dec1.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec2.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec3.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec4.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec5.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec6.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec7.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec8.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec9.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            Case 9
                Dec1.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec2.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec3.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec4.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec5.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec6.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec7.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec8.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec9.Visibility = Windows.UI.Xaml.Visibility.Visible
            Case Else
                Dec1.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec2.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec3.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec4.Visibility = Windows.UI.Xaml.Visibility.Visible
                Dec5.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec6.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec7.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec8.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Dec9.Visibility = Windows.UI.Xaml.Visibility.Collapsed
        End Select
        Select Case Fmt0
            Case AngleFormat.DMS
                Deg.Visibility = Windows.UI.Xaml.Visibility.Visible
                Min.Visibility = Windows.UI.Xaml.Visibility.Visible
                Sec.Visibility = Windows.UI.Xaml.Visibility.Visible
                TempString = JakStringFormats.FormatDMS(Num0, JakStringFormats.DmsFormat.SimpleDMS, JakStringFormats.DmsSign.PlusMinus, DecCount, False)
            Case AngleFormat.DM
                Deg.Visibility = Windows.UI.Xaml.Visibility.Visible
                Min.Visibility = Windows.UI.Xaml.Visibility.Visible
                Sec.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                TempString = JakStringFormats.FormatDMS(Num0, JakStringFormats.DmsFormat.SimpleDM, JakStringFormats.DmsSign.PlusMinus, DecCount, False)
            Case AngleFormat.D
                Deg.Visibility = Windows.UI.Xaml.Visibility.Visible
                Min.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                Sec.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                TempString = JakStringFormats.FormatDMS(Num0, JakStringFormats.DmsFormat.SimpleD, JakStringFormats.DmsSign.PlusMinus, DecCount, False)
            Case Else
                Deg.Visibility = Windows.UI.Xaml.Visibility.Visible
                Min.Visibility = Windows.UI.Xaml.Visibility.Visible
                Sec.Visibility = Windows.UI.Xaml.Visibility.Visible
                TempString = JakStringFormats.FormatDMS(Num0, JakStringFormats.DmsFormat.SimpleDMS, JakStringFormats.DmsSign.PlusMinus, DecCount, False)
        End Select

        'Split string in Degrees Minutes and Seconds
        SplitString = TempString.Split(CChar("."))
        DecString = SplitString(1)
        Select Case Fmt0
            Case AngleFormat.DMS
                SplitString = TempString.Split(CChar(":"))
                Deg.SelectedIndex = Deg.Items.IndexOf(SplitString(0))
                Min.SelectedIndex = Min.Items.IndexOf(SplitString(1))
                Sec.SelectedIndex = Sec.Items.IndexOf(SplitString(2).Substring(0, 2))
            Case AngleFormat.DM
                SplitString = TempString.Split(CChar(":"))
                Deg.SelectedIndex = Deg.Items.IndexOf(SplitString(0))
                Min.SelectedIndex = Min.Items.IndexOf(SplitString(1).Substring(0, 2))
            Case AngleFormat.D
                SplitString = TempString.Split(CChar("."))
                Deg.SelectedIndex = Deg.Items.IndexOf(SplitString(0))
        End Select

        Select Case DecCount
            Case 0
                Dec1.SelectedIndex = 0
                Dec2.SelectedIndex = 0
                Dec3.SelectedIndex = 0
                Dec4.SelectedIndex = 0
                Dec5.SelectedIndex = 0
                Dec6.SelectedIndex = 0
                Dec7.SelectedIndex = 0
                Dec8.SelectedIndex = 0
                Dec9.SelectedIndex = 0
            Case 1
                Dec1.SelectedIndex = CInt(DecString.Substring(0, 1))
                Dec2.SelectedIndex = 0
                Dec3.SelectedIndex = 0
                Dec4.SelectedIndex = 0
                Dec5.SelectedIndex = 0
                Dec6.SelectedIndex = 0
                Dec7.SelectedIndex = 0
                Dec8.SelectedIndex = 0
                Dec9.SelectedIndex = 0
            Case 2
                Dec1.SelectedIndex = CInt(DecString.Substring(0, 1))
                Dec2.SelectedIndex = CInt(DecString.Substring(1, 1))
                Dec3.SelectedIndex = 0
                Dec4.SelectedIndex = 0
                Dec5.SelectedIndex = 0
                Dec6.SelectedIndex = 0
                Dec7.SelectedIndex = 0
                Dec8.SelectedIndex = 0
                Dec9.SelectedIndex = 0
            Case 3
                Dec1.SelectedIndex = CInt(DecString.Substring(0, 1))
                Dec2.SelectedIndex = CInt(DecString.Substring(1, 1))
                Dec3.SelectedIndex = CInt(DecString.Substring(2, 1))
                Dec4.SelectedIndex = 0
                Dec5.SelectedIndex = 0
                Dec6.SelectedIndex = 0
                Dec7.SelectedIndex = 0
                Dec8.SelectedIndex = 0
                Dec9.SelectedIndex = 0
            Case 4
                Dec1.SelectedIndex = CInt(DecString.Substring(0, 1))
                Dec2.SelectedIndex = CInt(DecString.Substring(1, 1))
                Dec3.SelectedIndex = CInt(DecString.Substring(2, 1))
                Dec4.SelectedIndex = CInt(DecString.Substring(3, 1))
                Dec5.SelectedIndex = 0
                Dec6.SelectedIndex = 0
                Dec7.SelectedIndex = 0
                Dec8.SelectedIndex = 0
                Dec9.SelectedIndex = 0
            Case 5
                Dec1.SelectedIndex = CInt(DecString.Substring(0, 1))
                Dec2.SelectedIndex = CInt(DecString.Substring(1, 1))
                Dec3.SelectedIndex = CInt(DecString.Substring(2, 1))
                Dec4.SelectedIndex = CInt(DecString.Substring(3, 1))
                Dec5.SelectedIndex = CInt(DecString.Substring(4, 1))
                Dec6.SelectedIndex = 0
                Dec7.SelectedIndex = 0
                Dec8.SelectedIndex = 0
                Dec9.SelectedIndex = 0
            Case 6
                Dec1.SelectedIndex = CInt(DecString.Substring(0, 1))
                Dec2.SelectedIndex = CInt(DecString.Substring(1, 1))
                Dec3.SelectedIndex = CInt(DecString.Substring(2, 1))
                Dec4.SelectedIndex = CInt(DecString.Substring(3, 1))
                Dec5.SelectedIndex = CInt(DecString.Substring(4, 1))
                Dec6.SelectedIndex = CInt(DecString.Substring(5, 1))
                Dec7.SelectedIndex = 0
                Dec8.SelectedIndex = 0
                Dec9.SelectedIndex = 0
            Case 7
                Dec1.SelectedIndex = CInt(DecString.Substring(0, 1))
                Dec2.SelectedIndex = CInt(DecString.Substring(1, 1))
                Dec3.SelectedIndex = CInt(DecString.Substring(2, 1))
                Dec4.SelectedIndex = CInt(DecString.Substring(3, 1))
                Dec5.SelectedIndex = CInt(DecString.Substring(4, 1))
                Dec6.SelectedIndex = CInt(DecString.Substring(5, 1))
                Dec7.SelectedIndex = CInt(DecString.Substring(6, 1))
                Dec8.SelectedIndex = 0
                Dec9.SelectedIndex = 0
            Case 8
                Dec1.SelectedIndex = CInt(DecString.Substring(0, 1))
                Dec2.SelectedIndex = CInt(DecString.Substring(1, 1))
                Dec3.SelectedIndex = CInt(DecString.Substring(2, 1))
                Dec4.SelectedIndex = CInt(DecString.Substring(3, 1))
                Dec5.SelectedIndex = CInt(DecString.Substring(4, 1))
                Dec6.SelectedIndex = CInt(DecString.Substring(5, 1))
                Dec7.SelectedIndex = CInt(DecString.Substring(6, 1))
                Dec8.SelectedIndex = CInt(DecString.Substring(7, 1))
                Dec9.SelectedIndex = 0
            Case 9
                Dec1.SelectedIndex = CInt(DecString.Substring(0, 1))
                Dec2.SelectedIndex = CInt(DecString.Substring(1, 1))
                Dec3.SelectedIndex = CInt(DecString.Substring(2, 1))
                Dec4.SelectedIndex = CInt(DecString.Substring(3, 1))
                Dec5.SelectedIndex = CInt(DecString.Substring(4, 1))
                Dec6.SelectedIndex = CInt(DecString.Substring(5, 1))
                Dec7.SelectedIndex = CInt(DecString.Substring(6, 1))
                Dec8.SelectedIndex = CInt(DecString.Substring(7, 1))
                Dec9.SelectedIndex = CInt(DecString.Substring(8, 1))
        End Select
        'Re Enable the events
        SuspendedUpdates = False

    End Sub
End Class
