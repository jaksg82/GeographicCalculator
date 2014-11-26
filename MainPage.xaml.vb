''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page

    ''' <summary>
    ''' NavigationHelper is used on each page to aid in navigation and 
    ''' process lifetime management
    ''' </summary>
    Public ReadOnly Property NavigationHelper As Common.NavigationHelper
        Get
            Return Me._navigationHelper
        End Get
    End Property
    Private _navigationHelper As Common.NavigationHelper

    ''' <summary>
    ''' This can be changed to a strongly typed view model.
    ''' </summary>
    Public ReadOnly Property DefaultViewModel As Common.ObservableDictionary
        Get
            Return Me._defaultViewModel
        End Get
    End Property
    Private _defaultViewModel As New Common.ObservableDictionary()

    Public Sub New()

        InitializeComponent()
        Me._navigationHelper = New Common.NavigationHelper(Me)
        AddHandler Me._navigationHelper.LoadState, AddressOf NavigationHelper_LoadState
        AddHandler Me._navigationHelper.SaveState, AddressOf NavigationHelper_SaveState
    End Sub

    ''' <summary>
    ''' Populates the page with content passed during navigation.  Any saved state is also
    ''' provided when recreating a page from a prior session.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event; typically <see cref="NavigationHelper"/>
    ''' </param>
    ''' <param name="e">Event data that provides both the navigation parameter passed to
    ''' <see cref="Frame.Navigate"/> when this page was initially requested and
    ''' a dictionary of state preserved by this page during an earlier
    ''' session.  The state will be null the first time a page is visited.</param>
    Private Sub NavigationHelper_LoadState(sender As Object, e As Common.LoadStateEventArgs)
        ' Restore values stored in app data.
        Dim roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings
        UserPoint.X = If(roamingSettings.Values.ContainsKey("UserPointX"), CType(roamingSettings.Values("UserPointX"), Double), Double.NaN)
        UserPoint.Y = If(roamingSettings.Values.ContainsKey("UserPointY"), CType(roamingSettings.Values("UserPointY"), Double), Double.NaN)
        UserPoint.Z = If(roamingSettings.Values.ContainsKey("UserPointZ"), CType(roamingSettings.Values("UserPointZ"), Double), Double.NaN)
        UserPointIsGeo = If(roamingSettings.Values.ContainsKey("UserPointIsGeo"), CType(roamingSettings.Values("UserPointIsGeo"), Boolean), True)
        UserInputDecimals = If(roamingSettings.Values.ContainsKey("UserDecimals"), CType(roamingSettings.Values("UserDecimals"), Integer), 3)
        UserFormat = If(roamingSettings.Values.ContainsKey("UserFormat"), CType(roamingSettings.Values("UserFormat"), JakStringFormats.DmsFormat), JakStringFormats.DmsFormat.SimpleDMS)
        CrsString1 = If(roamingSettings.Values.ContainsKey("Crs1"), CType(roamingSettings.Values("Crs1"), String), "")
        CrsString2 = If(roamingSettings.Values.ContainsKey("Crs2"), CType(roamingSettings.Values("Crs2"), String), "")

    End Sub

    ''' <summary>
    ''' Preserves state associated with this page in case the application is suspended or the
    ''' page is discarded from the navigation cache.  Values must conform to the serialization
    ''' requirements of <see cref="Common.SuspensionManager.SessionState"/>.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event; typically <see cref="NavigationHelper"/>
    ''' </param>
    ''' <param name="e">Event data that provides an empty dictionary to be populated with 
    ''' serializable state.</param>
    Private Sub NavigationHelper_SaveState(sender As Object, e As Common.SaveStateEventArgs)

    End Sub

#Region "NavigationHelper registration"

    ''' The methods provided in this section are simply used to allow
    ''' NavigationHelper to respond to the page's navigation methods.
    ''' 
    ''' Page specific logic should be placed in event handlers for the  
    ''' <see cref="Common.NavigationHelper.LoadState"/>
    ''' and <see cref="Common.NavigationHelper.SaveState"/>.
    ''' The navigation parameter is available in the LoadState method 
    ''' in addition to page state preserved during an earlier session.

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        _navigationHelper.OnNavigatedTo(e)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        _navigationHelper.OnNavigatedFrom(e)
    End Sub

#End Region

    Public UserPoint As New JakMathLib.Point3D
    Public GeoPoint1, GeoPoint2, PrjPoint1, PrjPoint2, CentricPoint1, CentricPoint2, CentricPointWgs As New JakMathLib.Point3D
    Public Crs1, Crs2 As New JakDatum.GeoDatum
    Public UserPointIsGeo As Boolean
    Public UserFormat As JakStringFormats.DmsFormat
    Public UserInputDecimals As Integer
    Public CrsString1, CrsString2 As String

    ''' <summary>
    ''' Initialize the page
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub MainPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings
        UserPoint.X = If(JakMathLib.IsFinite(UserPoint.X), UserPoint.X, 0.0)
        UserPoint.Y = If(JakMathLib.IsFinite(UserPoint.Y), UserPoint.Y, 0.0)
        UserPoint.Z = If(JakMathLib.IsFinite(UserPoint.Z), UserPoint.Z, 0.0)
        If CrsString1 = "" Then
            CrsString1 = Crs1.ToString
            roamingSettings.Values("Crs1") = CrsString1
        Else
            Crs1 = New JakDatum.GeoDatum(CrsString1)
        End If
        If CrsString2 = "" Then
            CrsString2 = Crs2.ToString
            roamingSettings.Values("Crs2") = CrsString2
        Else
            Crs2 = New JakDatum.GeoDatum(CrsString2)
        End If
        UpdatePoints()
        UpdateUI()
    End Sub

    ''' <summary>
    ''' Read the coordinates given by the user
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UserPointUpdate() Handles InputX.LostFocus, InputY.LostFocus, InputZ.LostFocus
        Dim ParsedPoint As New JakMathLib.Point3D
        If UserPointIsGeo Then
            Try
                ParsedPoint.X = JakStringFormats.DmsParse(InputX.Text, UserFormat, JakStringFormats.DmsSign.Generic)
                ParsedPoint.Y = JakStringFormats.DmsParse(InputY.Text, UserFormat, JakStringFormats.DmsSign.Generic)
                ParsedPoint.Z = JakStringFormats.MetricParse(InputZ.Text, JakStringFormats.MetricSign.Number)
                If Double.IsNaN(ParsedPoint.X) Then ParsedPoint.X = UserPoint.X
                If Double.IsNaN(ParsedPoint.Y) Then ParsedPoint.Y = UserPoint.Y
                If Double.IsNaN(ParsedPoint.Z) Then ParsedPoint.Z = UserPoint.Z
            Catch ex As Exception
                ParsedPoint = UserPoint
            End Try
        Else
            Try
                ParsedPoint.X = JakStringFormats.MetricParse(InputX.Text, JakStringFormats.MetricSign.Number)
                ParsedPoint.Y = JakStringFormats.MetricParse(InputY.Text, JakStringFormats.MetricSign.Number)
                ParsedPoint.Z = JakStringFormats.MetricParse(InputZ.Text, JakStringFormats.MetricSign.Number)
                If Double.IsNaN(ParsedPoint.X) Then ParsedPoint.X = UserPoint.X
                If Double.IsNaN(ParsedPoint.Y) Then ParsedPoint.Y = UserPoint.Y
                If Double.IsNaN(ParsedPoint.Z) Then ParsedPoint.Z = UserPoint.Z
            Catch ex As Exception
                ParsedPoint = UserPoint
            End Try
        End If
        UserPoint = ParsedPoint
        UpdatePoints()
        UpdateUI()
    End Sub

    ''' <summary>
    ''' Write the calculated coordinates inside the respective fields
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateUI() As Boolean
        InfoCrs1.Text = Crs1.ToString
        InfoCrs2.Text = Crs2.ToString
        If UserPointIsGeo Then
            InputX.Text = JakStringFormats.FormatDMS(UserPoint.X, UserFormat, JakStringFormats.DmsSign.Generic, UserInputDecimals, False)
            InputY.Text = JakStringFormats.FormatDMS(UserPoint.Y, UserFormat, JakStringFormats.DmsSign.Generic, UserInputDecimals, False)
            InputZ.Text = JakStringFormats.FormatNumber(UserPoint.Z, 2)
        Else
            InputX.Text = JakStringFormats.FormatNumber(UserPoint.X, 2)
            InputY.Text = JakStringFormats.FormatNumber(UserPoint.Y, 2)
            InputZ.Text = JakStringFormats.FormatNumber(UserPoint.Z, 2)
        End If
        'GridView
        WSrcPrjX.Text = JakStringFormats.FormatMetric(PrjPoint1.X, JakStringFormats.MetricSign.UnitSuffix, 2, False)
        WSrcPrjY.Text = JakStringFormats.FormatMetric(PrjPoint1.Y, JakStringFormats.MetricSign.UnitSuffix, 2, True)
        WSrcPrjZ.Text = JakStringFormats.FormatMetric(PrjPoint1.Z, JakStringFormats.MetricSign.Unit, 2, False)
        WSrcGeoX.Text = JakStringFormats.FormatDMS(GeoPoint1.X, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 3, False)
        WSrcGeoY.Text = JakStringFormats.FormatDMS(GeoPoint1.Y, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 3, True)
        WSrcGeoZ.Text = JakStringFormats.FormatMetric(GeoPoint1.Z, JakStringFormats.MetricSign.Unit, 2, False)
        WSrcCntX.Text = JakStringFormats.FormatMetric(CentricPoint1.X, JakStringFormats.MetricSign.Unit, 2, False)
        WSrcCntY.Text = JakStringFormats.FormatMetric(CentricPoint1.Y, JakStringFormats.MetricSign.Unit, 2, False)
        WSrcCntZ.Text = JakStringFormats.FormatMetric(CentricPoint1.Z, JakStringFormats.MetricSign.Unit, 2, False)
        WWgsCntX.Text = JakStringFormats.FormatMetric(CentricPointWgs.X, JakStringFormats.MetricSign.Unit, 2, False)
        WWgsCntY.Text = JakStringFormats.FormatMetric(CentricPointWgs.Y, JakStringFormats.MetricSign.Unit, 2, False)
        WWgsCntZ.Text = JakStringFormats.FormatMetric(CentricPointWgs.Z, JakStringFormats.MetricSign.Unit, 2, False)
        WTgtCntX.Text = JakStringFormats.FormatMetric(CentricPoint2.X, JakStringFormats.MetricSign.Unit, 2, False)
        WTgtCntY.Text = JakStringFormats.FormatMetric(CentricPoint2.Y, JakStringFormats.MetricSign.Unit, 2, False)
        WTgtCntZ.Text = JakStringFormats.FormatMetric(CentricPoint2.Z, JakStringFormats.MetricSign.Unit, 2, False)
        WTgtGeoX.Text = JakStringFormats.FormatDMS(GeoPoint2.X, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 3, False)
        WTgtGeoY.Text = JakStringFormats.FormatDMS(GeoPoint2.Y, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 3, True)
        WTgtGeoZ.Text = JakStringFormats.FormatMetric(GeoPoint2.Z, JakStringFormats.MetricSign.Unit, 2, False)
        WTgtPrjX.Text = JakStringFormats.FormatMetric(PrjPoint2.X, JakStringFormats.MetricSign.UnitSuffix, 2, False)
        WTgtPrjY.Text = JakStringFormats.FormatMetric(PrjPoint2.Y, JakStringFormats.MetricSign.UnitSuffix, 2, True)
        WTgtPrjZ.Text = JakStringFormats.FormatMetric(PrjPoint2.Z, JakStringFormats.MetricSign.Unit, 2, False)
        'ListView
        LSrcPrjX.Text = JakStringFormats.FormatMetric(PrjPoint1.X, JakStringFormats.MetricSign.UnitSuffix, 2, False)
        LSrcPrjY.Text = JakStringFormats.FormatMetric(PrjPoint1.Y, JakStringFormats.MetricSign.UnitSuffix, 2, True)
        LSrcPrjZ.Text = JakStringFormats.FormatMetric(PrjPoint1.Z, JakStringFormats.MetricSign.Unit, 2, False)
        LSrcGeoX.Text = JakStringFormats.FormatDMS(GeoPoint1.X, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 3, False)
        LSrcGeoY.Text = JakStringFormats.FormatDMS(GeoPoint1.Y, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 3, True)
        LSrcGeoZ.Text = JakStringFormats.FormatMetric(GeoPoint1.Z, JakStringFormats.MetricSign.Unit, 2, False)
        LSrcCntX.Text = JakStringFormats.FormatMetric(CentricPoint1.X, JakStringFormats.MetricSign.Unit, 2, False)
        LSrcCntY.Text = JakStringFormats.FormatMetric(CentricPoint1.Y, JakStringFormats.MetricSign.Unit, 2, False)
        LSrcCntZ.Text = JakStringFormats.FormatMetric(CentricPoint1.Z, JakStringFormats.MetricSign.Unit, 2, False)
        LWgsCntX.Text = JakStringFormats.FormatMetric(CentricPointWgs.X, JakStringFormats.MetricSign.Unit, 2, False)
        LWgsCntY.Text = JakStringFormats.FormatMetric(CentricPointWgs.Y, JakStringFormats.MetricSign.Unit, 2, False)
        LWgsCntZ.Text = JakStringFormats.FormatMetric(CentricPointWgs.Z, JakStringFormats.MetricSign.Unit, 2, False)
        LTgtCntX.Text = JakStringFormats.FormatMetric(CentricPoint2.X, JakStringFormats.MetricSign.Unit, 2, False)
        LTgtCntY.Text = JakStringFormats.FormatMetric(CentricPoint2.Y, JakStringFormats.MetricSign.Unit, 2, False)
        LTgtCntZ.Text = JakStringFormats.FormatMetric(CentricPoint2.Z, JakStringFormats.MetricSign.Unit, 2, False)
        LTgtGeoX.Text = JakStringFormats.FormatDMS(GeoPoint2.X, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 3, False)
        LTgtGeoY.Text = JakStringFormats.FormatDMS(GeoPoint2.Y, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 3, True)
        LTgtGeoZ.Text = JakStringFormats.FormatMetric(GeoPoint2.Z, JakStringFormats.MetricSign.Unit, 2, False)
        LTgtPrjX.Text = JakStringFormats.FormatMetric(PrjPoint2.X, JakStringFormats.MetricSign.UnitSuffix, 2, False)
        LTgtPrjY.Text = JakStringFormats.FormatMetric(PrjPoint2.Y, JakStringFormats.MetricSign.UnitSuffix, 2, True)
        LTgtPrjZ.Text = JakStringFormats.FormatMetric(PrjPoint2.Z, JakStringFormats.MetricSign.Unit, 2, False)
        Return True
    End Function

    ''' <summary>
    ''' Calculate the converted coordinates
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdatePoints() As Boolean
        'Define the start coordinate
        If Double.IsNaN(UserPoint.X) Then
            UserPoint.X = 0
            UserPoint.Y = 0
            UserPoint.Z = 0
            UserPointIsGeo = True
        End If

        Try
            'Convert the UserPoint
            If UserPointIsGeo Then
                GeoPoint1 = UserPoint
                PrjPoint1 = JakDatum.LL2EN(Crs1, GeoPoint1)
                CentricPoint1 = JakDatum.LL2XYZ(Crs1, GeoPoint1)
                CentricPointWgs = JakDatum.XYZ2XYZ(Crs1, CentricPoint1, True)
                CentricPoint2 = JakDatum.XYZ2XYZ(Crs2, CentricPointWgs, False)
                GeoPoint2 = JakDatum.XYZ2LL(Crs2, CentricPoint2)
                PrjPoint2 = JakDatum.LL2EN(Crs2, GeoPoint2)
            Else
                PrjPoint1 = UserPoint
                GeoPoint1 = JakDatum.EN2LL(Crs1, PrjPoint1)
                CentricPoint1 = JakDatum.LL2XYZ(Crs1, GeoPoint1)
                CentricPointWgs = JakDatum.XYZ2XYZ(Crs1, CentricPoint1, True)
                CentricPoint2 = JakDatum.XYZ2XYZ(Crs2, CentricPointWgs, False)
                GeoPoint2 = JakDatum.XYZ2LL(Crs2, CentricPoint2)
                PrjPoint2 = JakDatum.LL2EN(Crs2, GeoPoint2)
            End If
            'Save the info in the app roaming data
            Dim roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings
            roamingSettings.Values("UserPointX") = UserPoint.X
            roamingSettings.Values("UserPointY") = UserPoint.Y
            roamingSettings.Values("UserPointZ") = UserPoint.Z
            roamingSettings.Values("UserPointIsGeo") = UserPointIsGeo
            roamingSettings.Values("UserDecimals") = UserInputDecimals
            roamingSettings.Values("UserFormat") = UserFormat
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Handle the resize of the app frame
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub LayoutGrid_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles LayoutGrid.SizeChanged
        If LayoutGrid.ActualWidth < 750 Then
            If LayoutGrid.ActualWidth < 500 Then
                VisualStateManager.GoToState(Me, "VeryNarrow", False)
            Else
                VisualStateManager.GoToState(Me, "LandscapeNarrow", False)
            End If

        Else
            VisualStateManager.GoToState(Me, "LandscapeDefault", False)
        End If
    End Sub

    ''' <summary>
    ''' Handle the change in the user input format
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub InputFormat_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles InputFormat.SelectionChanged
        Select Case InputFormat.SelectedIndex
            Case 0
                If UserPointIsGeo Then
                    UserPoint = PrjPoint1
                End If
                UserInputDecimals = 2
                UserPointIsGeo = False
            Case 1
                If UserPointIsGeo = False Then
                    UserPoint = GeoPoint1
                End If
                UserPointIsGeo = True
                UserFormat = JakStringFormats.DmsFormat.SimpleDMS
                UserInputDecimals = 2
            Case 2
                If UserPointIsGeo = False Then
                    UserPoint = GeoPoint1
                End If
                UserPointIsGeo = True
                UserFormat = JakStringFormats.DmsFormat.SimpleDM
                UserInputDecimals = 4
            Case 3
                If UserPointIsGeo = False Then
                    UserPoint = GeoPoint1
                End If
                UserPointIsGeo = True
                UserFormat = JakStringFormats.DmsFormat.SimpleD
                UserInputDecimals = 6
            Case 4
                If UserPointIsGeo = False Then
                    UserPoint = GeoPoint1
                End If
                UserPointIsGeo = True
                UserFormat = JakStringFormats.DmsFormat.NMEA
                UserInputDecimals = 4
            Case Else
                'do nothing
        End Select
        UpdateUI()

    End Sub

    Private Sub SetSrcCrs_Click(sender As Object, e As RoutedEventArgs) Handles SetSrcCrs.Click
        Me.Frame.Navigate(GetType(SrcSetPageVB), "Crs1")
    End Sub

    Private Sub SetTgtCrs_Click(sender As Object, e As RoutedEventArgs) Handles SetTgtCrs.Click
        Me.Frame.Navigate(GetType(SrcSetPageVB), "Crs2")
    End Sub
End Class
