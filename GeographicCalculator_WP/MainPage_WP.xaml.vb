Imports GeographicCalculator_WP.Common

' The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

''' <summary>
''' A basic page that provides characteristics common to most applications.
''' </summary>
Public NotInheritable Class MainPage_WP
    Inherits Page

    Private WithEvents _navigationHelper As New NavigationHelper(Me)
    Private ReadOnly _defaultViewModel As New ObservableDictionary()

    ''' <summary>
    ''' Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
    ''' </summary>
    Public ReadOnly Property NavigationHelper As NavigationHelper
        Get
            Return _navigationHelper
        End Get
    End Property

    ''' <summary>
    ''' Gets the view model for this <see cref="Page"/>.
    ''' This can be changed to a strongly typed view model.
    ''' </summary>
    Public ReadOnly Property DefaultViewModel As ObservableDictionary
        Get
            Return _defaultViewModel
        End Get
    End Property

    ''' <summary>
    ''' Populates the page with content passed during navigation. Any saved state is also
    ''' provided when recreating a page from a prior session.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event; typically <see cref="NavigationHelper"/>.
    ''' </param>
    ''' <param name="e">Event data that provides both the navigation parameter passed to
    ''' <see cref="Frame.Navigate"/> when this page was initially requested and
    ''' a dictionary of state preserved by this page during an earlier.
    ''' session. The state will be null the first time a page is visited.</param>
    Private Sub NavigationHelper_LoadState(sender As Object, e As LoadStateEventArgs) Handles _navigationHelper.LoadState
        ' TODO: Load the saved state of the page here.
        ' Restore values stored in app data.
        Dim roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings
        UserPoint.X = If(roamingSettings.Values.ContainsKey("UserPointX"), CType(roamingSettings.Values("UserPointX"), Double), Double.NaN)
        UserPoint.Y = If(roamingSettings.Values.ContainsKey("UserPointY"), CType(roamingSettings.Values("UserPointY"), Double), Double.NaN)
        UserPoint.Z = If(roamingSettings.Values.ContainsKey("UserPointZ"), CType(roamingSettings.Values("UserPointZ"), Double), Double.NaN)
        UserPointIsGeo = If(roamingSettings.Values.ContainsKey("UserPointIsGeo"), CType(roamingSettings.Values("UserPointIsGeo"), Boolean), True)
        UserInputDecimals = If(roamingSettings.Values.ContainsKey("UserDecimals"), CType(roamingSettings.Values("UserDecimals"), Integer), 3)
        UserFormat = If(roamingSettings.Values.ContainsKey("UserFormat"), CType(roamingSettings.Values("UserFormat"), JakStringFormats.DmsFormat), JakStringFormats.DmsFormat.SpacedDMS)
        CrsString1 = If(roamingSettings.Values.ContainsKey("Crs1"), CType(roamingSettings.Values("Crs1"), String), "")
        CrsString2 = If(roamingSettings.Values.ContainsKey("Crs2"), CType(roamingSettings.Values("Crs2"), String), "")

    End Sub

    ''' <summary>
    ''' Preserves state associated with this page in case the application is suspended or the
    ''' page is discarded from the navigation cache.  Values must conform to the serialization
    ''' requirements of <see cref="SuspensionManager.SessionState"/>.
    ''' </summary>
    ''' <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
    ''' <param name="e">Event data that provides an empty dictionary to be populated with
    ''' serializable state.</param>
    Private Sub NavigationHelper_SaveState(sender As Object, e As SaveStateEventArgs) Handles _navigationHelper.SaveState
        ' TODO: Save the unique state of the page here.
    End Sub

#Region "NavigationHelper registration"

    ''' <summary>
    ''' The methods provided in this section are simply used to allow
    ''' NavigationHelper to respond to the page's navigation methods.
    ''' <para>
    ''' Page specific logic should be placed in event handlers for the
    ''' <see cref="NavigationHelper.LoadState"/>
    ''' and <see cref="NavigationHelper.SaveState"/>.
    ''' The navigation parameter is available in the LoadState method
    ''' in addition to page state preserved during an earlier session.
    ''' </para>
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.</param>
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

    Private Sub MainPage_WP_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings
        UserPoint.X = If(JakMathLib.IsFinite(UserPoint.X), UserPoint.X, JakMathLib.DegRad(11.0))
        UserPoint.Y = If(JakMathLib.IsFinite(UserPoint.Y), UserPoint.Y, JakMathLib.DegRad(46.0))
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
                UserFormat = JakStringFormats.DmsFormat.SpacedDMS
                UserInputDecimals = 2
            Case 2
                If UserPointIsGeo = False Then
                    UserPoint = GeoPoint1
                End If
                UserPointIsGeo = True
                UserFormat = JakStringFormats.DmsFormat.SpacedDM
                UserInputDecimals = 4
            Case 3
                If UserPointIsGeo = False Then
                    UserPoint = GeoPoint1
                End If
                UserPointIsGeo = True
                UserFormat = JakStringFormats.DmsFormat.SimpleD
                UserInputDecimals = 6
            Case Else
                'do nothing
        End Select
        UpdateUI()
    End Sub

    Private Sub SetSrcCrs_Click(sender As Object, e As RoutedEventArgs) Handles BtCrs1.Click
        Me.Frame.Navigate(GetType(SrcSetPageVB), "Crs1")
    End Sub

    Private Sub SetTgtCrs_Click(sender As Object, e As RoutedEventArgs) Handles BtCrs2.Click
        Me.Frame.Navigate(GetType(SrcSetPageVB), "Crs2")
    End Sub

    Private Async Sub GetGpsLocation(sender As Object, e As RoutedEventArgs) Handles BtGps.Click
        Dim DevPos As New Windows.Devices.Geolocation.Geolocator
        Dim ActualPos As Windows.Devices.Geolocation.Geoposition
        WaitingPosition.Visibility = Windows.UI.Xaml.Visibility.Visible
        ActualPos = Await DevPos.GetGeopositionAsync()
        'update the input point
        UserPointIsGeo = True
        UserPoint.X = JakMathLib.DegRad(ActualPos.Coordinate.Point.Position.Longitude)
        UserPoint.Y = JakMathLib.DegRad(ActualPos.Coordinate.Point.Position.Latitude)
        UserPoint.Z = ActualPos.Coordinate.Point.Position.Altitude
        UpdatePoints()
        UpdateUI()
        WaitingPosition.Visibility = Windows.UI.Xaml.Visibility.Collapsed

    End Sub
    ''' <summary>
    ''' Write the calculated coordinates inside the respective fields
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateUI() As Boolean
        If UserPointIsGeo Then
            InputX.Text = JakStringFormats.FormatDMS(UserPoint.X, UserFormat, JakStringFormats.DmsSign.Generic, UserInputDecimals, False)
            InputY.Text = JakStringFormats.FormatDMS(UserPoint.Y, UserFormat, JakStringFormats.DmsSign.Generic, UserInputDecimals, False)
            InputZ.Text = JakStringFormats.FormatNumber(UserPoint.Z, 2)
        Else
            InputX.Text = JakStringFormats.FormatNumber(UserPoint.X, 2)
            InputY.Text = JakStringFormats.FormatNumber(UserPoint.Y, 2)
            InputZ.Text = JakStringFormats.FormatNumber(UserPoint.Z, 2)
        End If
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

End Class
