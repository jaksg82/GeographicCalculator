' The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

''' <summary>
''' A basic page that provides characteristics common to most applications.
''' </summary>
Public NotInheritable Class SrcSetPageVB
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
        Dim roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings
        CrsID = CType(e.NavigationParameter, String)
        CrsString = If(roamingSettings.Values.ContainsKey(CrsID), CType(roamingSettings.Values(CrsID), String), "")
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

    Dim TempCrs, SavedCrs, OriginalCrs As New JakDatum.GeoDatum
    Dim IsCrsUpdated, EventDisabled As Boolean
    Dim EpsgId() As Integer
    Dim EpsgFname(), EpsgSname(), CrsID, CrsString As String
    Dim EpsgSma(), EpsgInvFlat() As Double

    Private Sub backButton_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(MainPage_WP))
    End Sub

    Private Sub SrcSetPageVB_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        TempCrs = New JakDatum.GeoDatum(CrsString)
        SavedCrs = TempCrs
        OriginalCrs = TempCrs
        IsCrsUpdated = False
        EventDisabled = False
        'Populate the UtmZoneIn combobox
        For u = 1 To 60
            ListSrcUtmZoneIn.Items.Add(u.ToString("00"))
        Next
        'Populate the ellipsoid combobox
        TempCrs.EnumerateEpsgEllipsoids(EpsgId, EpsgFname, EpsgSname, EpsgSma, EpsgInvFlat)
        For l = 0 To EpsgFname.Count - 1
            ListSrcEllFname.Items.Add(EpsgFname(l))
        Next
        'Populate the transformation combobox
        Dim DtmNames() As String = TempCrs.EnumerateShiftingMethods()
        For t = 0 To DtmNames.Count - 1
            ListSrcDtmMethod.Items.Add(DtmNames(t))
        Next
        pageSubTitle.Text = If(CrsID = "Crs1", "Source C.R.S.", "Target C.R.S.")
        UpdateUI()
        IsCrsUpdated = True
        AcceptButton.IsEnabled = False
    End Sub

    Private Sub AcceptButton_Click()
        'Convert the user inputs in a new CRS
        Dim SelID As Integer
        Dim Lon0, Lat0, Lat1, Lat2, DX, DY, DZ, RX, RY, RZ, ScalePPM, PX, PY, PZ, FX, FY, CmScale As Double

        'Take the values from the fields in the listview
        '-> 1 : Datum shift parameters
        Try
            SelID = ListSrcDtmMethod.SelectedIndex
            If SelID > 0 Then
                'Convert the seconds in radians
                RX = JakMathLib.DegRad(CDbl(ListSrcDtmRX.Text) / 3600)
                RY = JakMathLib.DegRad(CDbl(ListSrcDtmRY.Text) / 3600)
                RZ = JakMathLib.DegRad(CDbl(ListSrcDtmRZ.Text) / 3600)
            End If
            If Double.TryParse(ListSrcDtmDX.Text, DX) = False Then DX = OriginalCrs.TransformationDeltas.X
            If Double.TryParse(ListSrcDtmDY.Text, DY) = False Then DY = OriginalCrs.TransformationDeltas.Y
            If Double.TryParse(ListSrcDtmDZ.Text, DZ) = False Then DZ = OriginalCrs.TransformationDeltas.Z
            If Double.TryParse(ListSrcDtmScale.Text, ScalePPM) = False Then ScalePPM = OriginalCrs.TransformationScaleFactorPPM
            If Double.TryParse(ListSrcDtmPX.Text, PX) = False Then PX = OriginalCrs.TransformationRotationPoint.X
            If Double.TryParse(ListSrcDtmPY.Text, PY) = False Then PY = OriginalCrs.TransformationRotationPoint.Y
            If Double.TryParse(ListSrcDtmPZ.Text, PZ) = False Then PZ = OriginalCrs.TransformationRotationPoint.Z


            Select Case SelID
                Case 0 '3 parameters
                    TempCrs.SetDatumConversion(ListSrcDtmFnameIn.Text, ListSrcDtmSnameIn.Text, DX, DY, DZ)
                Case 1 '7 parameters PV
                    TempCrs.SetDatumConversion(ListSrcDtmFnameIn.Text, ListSrcDtmSnameIn.Text, DX, DY, DZ, RX, RY, RZ, ScalePPM, True)
                Case 2 '7 parameters CF
                    TempCrs.SetDatumConversion(ListSrcDtmFnameIn.Text, ListSrcDtmSnameIn.Text, DX, DY, DZ, RX, RY, RZ, ScalePPM, False)
                Case 3 '10 parameters
                    TempCrs.SetDatumConversion(ListSrcDtmFnameIn.Text, ListSrcDtmSnameIn.Text, DX, DY, DZ, RX, RY, RZ, ScalePPM, PX, PY, PZ)
            End Select

        Catch ex As Exception
            Select Case OriginalCrs.TransformationCode
                Case JakDatum.GeoDatum.TransformationMethod.Geocentric
                    TempCrs.SetDatumConversion(OriginalCrs.TransformationFullName, OriginalCrs.TransformationShortName, _
                                               OriginalCrs.TransformationDeltas.X, OriginalCrs.TransformationDeltas.Y, _
                                               OriginalCrs.TransformationDeltas.Z)
                Case JakDatum.GeoDatum.TransformationMethod.PositionVector
                    TempCrs.SetDatumConversion(OriginalCrs.TransformationFullName, OriginalCrs.TransformationShortName, _
                                               OriginalCrs.TransformationDeltas.X, OriginalCrs.TransformationDeltas.Y, _
                                               OriginalCrs.TransformationDeltas.Z, OriginalCrs.TransformationRotations.X, _
                                               OriginalCrs.TransformationRotations.Y, OriginalCrs.TransformationRotations.Z, _
                                               OriginalCrs.TransformationScaleFactorPPM, True)
                Case JakDatum.GeoDatum.TransformationMethod.CoordinateFrame
                    TempCrs.SetDatumConversion(OriginalCrs.TransformationFullName, OriginalCrs.TransformationShortName, _
                                               OriginalCrs.TransformationDeltas.X, OriginalCrs.TransformationDeltas.Y, _
                                               OriginalCrs.TransformationDeltas.Z, OriginalCrs.TransformationRotations.X, _
                                               OriginalCrs.TransformationRotations.Y, OriginalCrs.TransformationRotations.Z, _
                                               OriginalCrs.TransformationScaleFactorPPM, False)
                Case JakDatum.GeoDatum.TransformationMethod.MolodenskyBadekas
                    TempCrs.SetDatumConversion(OriginalCrs.TransformationFullName, OriginalCrs.TransformationShortName, _
                                               OriginalCrs.TransformationDeltas.X, OriginalCrs.TransformationDeltas.Y, _
                                               OriginalCrs.TransformationDeltas.Z, OriginalCrs.TransformationRotations.X, _
                                               OriginalCrs.TransformationRotations.Y, OriginalCrs.TransformationRotations.Z, _
                                               OriginalCrs.TransformationScaleFactorPPM, OriginalCrs.TransformationRotationPoint.X, _
                                               OriginalCrs.TransformationRotationPoint.Y, OriginalCrs.TransformationRotationPoint.Z)
            End Select
        End Try

        '-> 2 : Ellipsoid parameters
        Try
            SelID = ListSrcEllFname.SelectedIndex
            TempCrs.SetEllipsoidByEpsgID(EpsgId(SelID))
        Catch ex As Exception
            TempCrs.SetEllipsoidByEpsgID(OriginalCrs.EllipsoidID)
        End Try

        '-> 3 : Projection parameters
        Try
            SelID = ListSrcPrjMethod.SelectedIndex
            If Double.TryParse(ListSrcFalseXIn.Text, FX) = False Then FX = OriginalCrs.ProjectionFalseEasting
            If Double.TryParse(ListSrcFalseYIn.Text, FY) = False Then FY = OriginalCrs.ProjectionFalseNorthing
            If Double.TryParse(ListSrcScaleIn.Text, CmScale) = False Then CmScale = OriginalCrs.ProjectionScaleAtOrigin

            Select Case SelID
                Case 0 'UTM
                    TempCrs.SetProjectionUTM(ListSrcUtmZoneIn.SelectedIndex + 1, ListSrcUtmHemiIn.IsOn)
                Case 1 'Tmerc
                    Lon0 = JakStringFormats.DmsParse(ListSrcLon0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                    Lat0 = JakStringFormats.DmsParse(ListSrcLat0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                    TempCrs.SetProjectionTransverseMercator(ListSrcPrjFnameIn.Text, ListSrcPrjSnameIn.Text, Lon0, Lat0, CmScale, FX, FY)
                Case 2 'Merc
                    Lon0 = JakStringFormats.DmsParse(ListSrcLon0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                    TempCrs.SetProjectionMercator(ListSrcPrjFnameIn.Text, ListSrcPrjSnameIn.Text, Lon0, CmScale, FX, FY)
                Case 3 'LCC1
                    Lon0 = JakStringFormats.DmsParse(ListSrcLon0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                    Lat0 = JakStringFormats.DmsParse(ListSrcLat0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                    TempCrs.SetProjectionLambertConical1(ListSrcPrjFnameIn.Text, ListSrcPrjSnameIn.Text, Lon0, Lat0, CmScale, FX, FY)
                Case 4 'LCC2
                    Lon0 = JakStringFormats.DmsParse(ListSrcLon0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                    Lat0 = JakStringFormats.DmsParse(ListSrcLat0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                    Lat1 = JakStringFormats.DmsParse(ListSrcLat1In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                    Lat2 = JakStringFormats.DmsParse(ListSrcLat2In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                    TempCrs.SetProjectionLambertConical2(ListSrcPrjFnameIn.Text, ListSrcPrjSnameIn.Text, Lon0, Lat0, Lat1, Lat2, FX, FY)
            End Select

        Catch ex As Exception
            Select Case OriginalCrs.ProjectionCode
                Case JakDatum.GeoDatum.ProjectionMethod.UTM
                    TempCrs.SetProjectionUTM(OriginalCrs.ProjectionUtmZone, OriginalCrs.ProjectionIsNorthHemisphere)
                Case JakDatum.GeoDatum.ProjectionMethod.Tmerc
                    TempCrs.SetProjectionTransverseMercator(OriginalCrs.ProjectionFullName, OriginalCrs.ProjectionShortName, _
                                                            OriginalCrs.ProjectionOriginLongitude, OriginalCrs.ProjectionOriginLatitude, _
                                                            OriginalCrs.ProjectionScaleAtOrigin, OriginalCrs.ProjectionFalseEasting, _
                                                            OriginalCrs.ProjectionFalseNorthing)
                Case JakDatum.GeoDatum.ProjectionMethod.Merc
                    TempCrs.SetProjectionMercator(OriginalCrs.ProjectionFullName, OriginalCrs.ProjectionShortName, _
                                                  OriginalCrs.ProjectionOriginLongitude, OriginalCrs.ProjectionScaleAtOrigin, _
                                                  OriginalCrs.ProjectionFalseEasting, OriginalCrs.ProjectionFalseNorthing)
                Case JakDatum.GeoDatum.ProjectionMethod.Lcc1
                    TempCrs.SetProjectionLambertConical1(OriginalCrs.ProjectionFullName, OriginalCrs.ProjectionShortName, _
                                                         OriginalCrs.ProjectionOriginLongitude, OriginalCrs.ProjectionOriginLatitude, _
                                                         OriginalCrs.ProjectionScaleAtOrigin, OriginalCrs.ProjectionFalseEasting, _
                                                         OriginalCrs.ProjectionFalseNorthing)
                Case JakDatum.GeoDatum.ProjectionMethod.Lcc2
                    TempCrs.SetProjectionLambertConical2(OriginalCrs.ProjectionFullName, OriginalCrs.ProjectionShortName, _
                                                         OriginalCrs.ProjectionOriginLongitude, OriginalCrs.ProjectionOriginLatitude, _
                                                         OriginalCrs.LambertFirstParallel, OriginalCrs.LambertSecondParallel, _
                                                         OriginalCrs.ProjectionFalseEasting, OriginalCrs.ProjectionFalseNorthing)
            End Select
        End Try

        IsCrsUpdated = True
        AcceptButton.IsEnabled = False
        SavedCrs = TempCrs
        Dim roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings
        roamingSettings.Values(CrsID) = SavedCrs.ToString
        CrsString = SavedCrs.ToString

        UpdateUI()

    End Sub

    Private Sub CancelButton_Click()
        'Restore the original CRS
        TempCrs = OriginalCrs
        UpdateUI()
    End Sub

    Private Sub UpdateUI()
        'Disable the events
        EventDisabled = True
        If CrsID = "Crs1" Then
            'Source
            ListPrjTitle.Text = "Source Projection"
            ListEllTitle.Text = "Source Ellipsoid"
        Else
            'Target 
            ListPrjTitle.Text = "Target Projection"
            ListEllTitle.Text = "Target Ellipsoid"
        End If

        ' >>> PROJECTION PANEL - ListView <<<
        'Fill the textboxes and the textblocks with the values
        ListSrcPrjFnameOut.Text = TempCrs.ProjectionFullName
        ListSrcPrjFnameIn.Text = TempCrs.ProjectionFullName
        ListSrcPrjSnameOut.Text = TempCrs.ProjectionShortName
        ListSrcPrjSnameIn.Text = TempCrs.ProjectionShortName
        ListSrcUtmZoneOut.Text = TempCrs.ProjectionUtmZone.ToString()
        ListSrcUtmZoneIn.SelectedIndex = TempCrs.ProjectionUtmZone - 1
        ListSrcUtmHemiIn.IsOn = TempCrs.ProjectionIsNorthHemisphere
        ListSrcLon0Out.Text = JakStringFormats.FormatDMS(TempCrs.ProjectionOriginLongitude, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, False)
        ListSrcLon0In.Text = JakStringFormats.FormatDMS(TempCrs.ProjectionOriginLongitude, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, False)
        ListSrcLat0Out.Text = JakStringFormats.FormatDMS(TempCrs.ProjectionOriginLatitude, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, True)
        ListSrcLat0In.Text = JakStringFormats.FormatDMS(TempCrs.ProjectionOriginLatitude, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, True)
        ListSrcLat1Out.Text = JakStringFormats.FormatDMS(TempCrs.LambertFirstParallel, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, True)
        ListSrcLat1In.Text = JakStringFormats.FormatDMS(TempCrs.LambertFirstParallel, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, True)
        ListSrcLat2Out.Text = JakStringFormats.FormatDMS(TempCrs.LambertSecondParallel, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, True)
        ListSrcLat2In.Text = JakStringFormats.FormatDMS(TempCrs.LambertSecondParallel, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, True)
        ListSrcFalseXOut.Text = JakStringFormats.FormatNumber(TempCrs.ProjectionFalseEasting, 3)
        ListSrcFalseXIn.Text = TempCrs.ProjectionFalseEasting.ToString("0.000")
        ListSrcFalseYOut.Text = JakStringFormats.FormatNumber(TempCrs.ProjectionFalseNorthing, 3)
        ListSrcFalseYIn.Text = TempCrs.ProjectionFalseNorthing.ToString("0.000")
        ListSrcScaleOut.Text = JakStringFormats.FormatNumber(TempCrs.ProjectionScaleAtOrigin, 3)
        ListSrcScaleIn.Text = TempCrs.ProjectionScaleAtOrigin.ToString("0.000000")
        ListSrcPrjMethod.SelectedIndex = TempCrs.ProjectionCode

        ' >>> ELLIPSOID PANEL - ListView <<<
        ListSrcEllFname.SelectedIndex = ListSrcEllFname.Items.IndexOf(TempCrs.EllipsoidFullName)

        ' >>> TRANSFORMATION PANEL - ListView <<<
        ListSrcDtmFnameIn.Text = TempCrs.TransformationFullName
        ListSrcDtmSnameIn.Text = TempCrs.TransformationShortName
        ListSrcDtmDX.Text = TempCrs.TransformationDeltas.X.ToString("0.000")
        ListSrcDtmDY.Text = TempCrs.TransformationDeltas.Y.ToString("0.000")
        ListSrcDtmDZ.Text = TempCrs.TransformationDeltas.Z.ToString("0.000")
        ListSrcDtmRX.Text = (JakMathLib.RadDeg(TempCrs.TransformationRotations.X) * 3600).ToString("0.000000") 'Convert the radians values the seconds of degree
        ListSrcDtmRY.Text = (JakMathLib.RadDeg(TempCrs.TransformationRotations.Y) * 3600).ToString("0.000000") 'Convert the radians values the seconds of degree
        ListSrcDtmRZ.Text = (JakMathLib.RadDeg(TempCrs.TransformationRotations.Z) * 3600).ToString("0.000000") 'Convert the radians values the seconds of degree
        ListSrcDtmScale.Text = TempCrs.TransformationScaleFactorPPM.ToString("0.000000000")
        ListSrcDtmPX.Text = TempCrs.TransformationRotationPoint.X.ToString("0.000")
        ListSrcDtmPY.Text = TempCrs.TransformationRotationPoint.Y.ToString("0.000")
        ListSrcDtmPZ.Text = TempCrs.TransformationRotationPoint.Z.ToString("0.000")
        ListSrcDtmMethod.SelectedIndex = TempCrs.TransformationCode
        IsCrsUpdated = True
        AcceptButton.IsEnabled = False
        'ReEnable the events
        EventDisabled = False

    End Sub

    Private Sub SrcPrjMethod_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ListSrcPrjMethod.SelectionChanged
        'Select witch fields are visible
        Dim SelID As Integer
        SelID = CType(sender, ComboBox).SelectedIndex
        Select Case SelID
            Case 1 'TMERC
                ListSrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

            Case 2 'MERC
                ListSrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

            Case 3 'LCC1
                ListSrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

            Case 4 'LCC2
                ListSrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

            Case Else 'UTM
                ListSrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Visible

                ListSrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Visible

                ListSrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Visible

                ListSrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Visible

                ListSrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Visible

                ListSrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Visible

        End Select
        SomethingChanged()
    End Sub

    Private Sub SrcEllFname_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ListSrcEllFname.SelectionChanged
        Dim SelID As Integer
        SelID = CType(sender, ComboBox).SelectedIndex

        ListSrcEllSname.Text = EpsgSname(SelID)
        ListSrcEllSma.Text = JakStringFormats.FormatNumber(EpsgSma(SelID), 3)
        ListSrcEllInvFlat.Text = JakStringFormats.FormatNumber(EpsgInvFlat(SelID), 9)
        ListSrcEllID.Text = EpsgId(SelID).ToString
        'Update the CRS
        TempCrs.SetEllipsoidByEpsgID(EpsgId(SelID))
        SomethingChanged()
    End Sub

    Private Sub SrcDtmMethod_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ListSrcDtmMethod.SelectionChanged
        Dim SelID As Integer
        SelID = CType(sender, ComboBox).SelectedIndex
        Select Case SelID
            Case 1, 2 '7 parameters
                ListSrcDtmRXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRX.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRY.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRZTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRZ.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmScale.Visibility = Windows.UI.Xaml.Visibility.Visible

                ListSrcDtmPXTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPX.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPYTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPY.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPZTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPZ.Visibility = Windows.UI.Xaml.Visibility.Collapsed

            Case 3 '10 parameters
                ListSrcDtmRXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRX.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRY.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRZTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRZ.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmScale.Visibility = Windows.UI.Xaml.Visibility.Visible

                ListSrcDtmPXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmPX.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmPYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmPY.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmPZTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmPZ.Visibility = Windows.UI.Xaml.Visibility.Visible

            Case Else '3 parameters
                ListSrcDtmRXTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmRX.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmRYTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmRY.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmRZTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmRZ.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmScale.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                ListSrcDtmPXTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPX.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPYTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPY.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPZTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPZ.Visibility = Windows.UI.Xaml.Visibility.Collapsed

        End Select
        SomethingChanged()
    End Sub

    Private Sub SrcSetPageVB_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Me.SizeChanged
        If Me.ActualWidth < 750 Then
            If Me.ActualWidth < 600 Then
                VisualStateManager.GoToState(Me, "LandscapeNarrow", False)
            Else
                VisualStateManager.GoToState(Me, "Landscape700", False)
            End If
        Else
            VisualStateManager.GoToState(Me, "LandscapeFull", False)
        End If

    End Sub

    Private Sub SrcUtm_Changed() Handles ListSrcUtmZoneIn.SelectionChanged, ListSrcUtmHemiIn.Toggled
        If EventDisabled Then Exit Sub
        TempCrs.SetProjectionUTM(ListSrcUtmZoneIn.SelectedIndex + 1, ListSrcUtmHemiIn.IsOn)
        UpdateUI()
        SomethingChanged()
    End Sub

    Private Sub SomethingChanged() Handles _
        ListSrcPrjFnameIn.TextChanged, ListSrcPrjSnameIn.TextChanged, _
        ListSrcLon0In.TextChanged, ListSrcLat0In.TextChanged, ListSrcLat1In.TextChanged, ListSrcLat2In.TextChanged, _
        ListSrcFalseXIn.TextChanged, ListSrcFalseYIn.TextChanged, ListSrcScaleIn.TextChanged, _
        ListSrcDtmDX.TextChanged, ListSrcDtmDY.TextChanged, ListSrcDtmDZ.TextChanged, _
        ListSrcDtmRX.TextChanged, ListSrcDtmRY.TextChanged, ListSrcDtmRZ.TextChanged, ListSrcDtmScale.TextChanged, _
        ListSrcDtmPX.TextChanged, ListSrcDtmPY.TextChanged, ListSrcDtmPZ.TextChanged

        If EventDisabled Then Exit Sub
        IsCrsUpdated = False
        AcceptButton.IsEnabled = True
    End Sub
End Class
