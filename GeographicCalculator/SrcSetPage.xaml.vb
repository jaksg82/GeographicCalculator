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

    Private Sub backButton_Click(sender As Object, e As RoutedEventArgs) Handles backButton.Click
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Sub SrcSetPageVB_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        TempCrs = New JakDatum.GeoDatum(CrsString)
        SavedCrs = TempCrs
        OriginalCrs = TempCrs
        IsCrsUpdated = False
        EventDisabled = False
        'Populate the UtmZoneIn combobox
        For u = 1 To 60
            SrcUtmZoneIn.Items.Add(u.ToString("00"))
            ListSrcUtmZoneIn.Items.Add(u.ToString("00"))
        Next
        'Populate the ellipsoid combobox
        TempCrs.EnumerateEpsgEllipsoids(EpsgId, EpsgFname, EpsgSname, EpsgSma, EpsgInvFlat)
        For l = 0 To EpsgFname.Count - 1
            SrcEllFname.Items.Add(EpsgFname(l))
            ListSrcEllFname.Items.Add(EpsgFname(l))
        Next
        'Populate the transformation combobox
        Dim DtmNames() As String = TempCrs.EnumerateShiftingMethods()
        For t = 0 To DtmNames.Count - 1
            SrcDtmMethod.Items.Add(DtmNames(t))
            ListSrcDtmMethod.Items.Add(DtmNames(t))
        Next
        CrsLabel.Text = If(CrsID = "Crs1", "Source", "Target")
        UpdateUI()
        IsCrsUpdated = True
        AcceptButton.IsEnabled = False
    End Sub

    Private Sub AcceptButton_Click()
        'Convert the user inputs in a new CRS
        Dim SelID As Integer
        Dim Lon0, Lat0, Lat1, Lat2, RX, RY, RZ As Double

        If ListPanels.Visibility = Windows.UI.Xaml.Visibility.Collapsed Then
            'Take the values from the fields in the gridview
            '-> 1 : Datum shift parameters
            Try
                SelID = SrcDtmMethod.SelectedIndex
                If SelID > 0 Then
                    'Convert the seconds in radians
                    RX = JakMathLib.DegRad(SrcDtmRX.Value / 3600)
                    RY = JakMathLib.DegRad(SrcDtmRY.Value / 3600)
                    RZ = JakMathLib.DegRad(SrcDtmRZ.Value / 3600)
                End If
                Select Case SelID
                    Case 0 '3 parameters
                        TempCrs.SetDatumConversion(SrcDtmFnameIn.Text, SrcDtmSnameIn.Text, SrcDtmDX.Value, SrcDtmDY.Value, SrcDtmDZ.Value)
                    Case 1 '7 parameters PV
                        TempCrs.SetDatumConversion(SrcDtmFnameIn.Text, SrcDtmSnameIn.Text, SrcDtmDX.Value, SrcDtmDY.Value, SrcDtmDZ.Value, _
                                                   RX, RY, RZ, SrcDtmScale.Value, True)
                    Case 2 '7 parameters CF
                        TempCrs.SetDatumConversion(SrcDtmFnameIn.Text, SrcDtmSnameIn.Text, SrcDtmDX.Value, SrcDtmDY.Value, SrcDtmDZ.Value, _
                                                   RX, RY, RZ, SrcDtmScale.Value, False)
                    Case 3 '10 parameters
                        TempCrs.SetDatumConversion(SrcDtmFnameIn.Text, SrcDtmSnameIn.Text, SrcDtmDX.Value, SrcDtmDY.Value, SrcDtmDZ.Value, _
                                                   RX, RY, RZ, SrcDtmScale.Value, SrcDtmPX.Value, SrcDtmPY.Value, SrcDtmPZ.Value)
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
                SelID = SrcEllFname.SelectedIndex
                TempCrs.SetEllipsoidByEpsgID(EpsgId(SelID))
            Catch ex As Exception
                TempCrs.SetEllipsoidByEpsgID(OriginalCrs.EllipsoidID)
            End Try

            '-> 3 : Projection parameters
            Try
                SelID = SrcPrjMethod.SelectedIndex
                Select Case SelID
                    Case 0 'UTM
                        TempCrs.SetProjectionUTM(SrcUtmZoneIn.SelectedIndex + 1, SrcUtmHemiIn.IsOn)
                    Case 1 'Tmerc
                        Lon0 = JakStringFormats.DmsParse(SrcLon0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        Lat0 = JakStringFormats.DmsParse(SrcLat0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        TempCrs.SetProjectionTransverseMercator(SrcPrjFnameIn.Text, SrcPrjSnameIn.Text, Lon0, Lat0, _
                                                                SrcScaleIn.Value, SrcFalseXIn.Value, SrcFalseYIn.Value)
                    Case 2 'Merc
                        Lon0 = JakStringFormats.DmsParse(SrcLon0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        TempCrs.SetProjectionMercator(SrcPrjFnameIn.Text, SrcPrjSnameIn.Text, Lon0, SrcScaleIn.Value, _
                                                      SrcFalseXIn.Value, SrcFalseYIn.Value)
                    Case 3 'LCC1
                        Lon0 = JakStringFormats.DmsParse(SrcLon0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        Lat0 = JakStringFormats.DmsParse(SrcLat0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        TempCrs.SetProjectionLambertConical1(SrcPrjFnameIn.Text, SrcPrjSnameIn.Text, Lon0, Lat0, _
                                                             SrcScaleIn.Value, SrcFalseXIn.Value, SrcFalseYIn.Value)
                    Case 4 'LCC2
                        Lon0 = JakStringFormats.DmsParse(SrcLon0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        Lat0 = JakStringFormats.DmsParse(SrcLat0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        Lat1 = JakStringFormats.DmsParse(SrcLat1In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        Lat2 = JakStringFormats.DmsParse(SrcLat2In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        TempCrs.SetProjectionLambertConical2(SrcPrjFnameIn.Text, SrcPrjSnameIn.Text, Lon0, Lat0, Lat1, Lat2, _
                                                             SrcFalseXIn.Value, SrcFalseYIn.Value)
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

        Else
            'Take the values from the fields in the listview
            '-> 1 : Datum shift parameters
            Try
                SelID = ListSrcDtmMethod.SelectedIndex
                If SelID > 0 Then
                    'Convert the seconds in radians
                    RX = JakMathLib.DegRad(ListSrcDtmRX.Value / 3600)
                    RY = JakMathLib.DegRad(ListSrcDtmRY.Value / 3600)
                    RZ = JakMathLib.DegRad(ListSrcDtmRZ.Value / 3600)
                End If
                Select Case SelID
                    Case 0 '3 parameters
                        TempCrs.SetDatumConversion(ListSrcDtmFnameIn.Text, ListSrcDtmSnameIn.Text, ListSrcDtmDX.Value, ListSrcDtmDY.Value, ListSrcDtmDZ.Value)
                    Case 1 '7 parameters PV
                        TempCrs.SetDatumConversion(ListSrcDtmFnameIn.Text, ListSrcDtmSnameIn.Text, ListSrcDtmDX.Value, ListSrcDtmDY.Value, ListSrcDtmDZ.Value, _
                                                   RX, RY, RZ, ListSrcDtmScale.Value, True)
                    Case 2 '7 parameters CF
                        TempCrs.SetDatumConversion(ListSrcDtmFnameIn.Text, ListSrcDtmSnameIn.Text, ListSrcDtmDX.Value, ListSrcDtmDY.Value, ListSrcDtmDZ.Value, _
                                                   RX, RY, RZ, ListSrcDtmScale.Value, False)
                    Case 3 '10 parameters
                        TempCrs.SetDatumConversion(ListSrcDtmFnameIn.Text, ListSrcDtmSnameIn.Text, ListSrcDtmDX.Value, ListSrcDtmDY.Value, ListSrcDtmDZ.Value, _
                                                   RX, RY, RZ, ListSrcDtmScale.Value, ListSrcDtmPX.Value, ListSrcDtmPY.Value, ListSrcDtmPZ.Value)
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
                Select Case SelID
                    Case 0 'UTM
                        TempCrs.SetProjectionUTM(ListSrcUtmZoneIn.SelectedIndex + 1, ListSrcUtmHemiIn.IsOn)
                    Case 1 'Tmerc
                        Lon0 = JakStringFormats.DmsParse(ListSrcLon0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        Lat0 = JakStringFormats.DmsParse(ListSrcLat0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        TempCrs.SetProjectionTransverseMercator(ListSrcPrjFnameIn.Text, ListSrcPrjSnameIn.Text, Lon0, Lat0, _
                                                                ListSrcScaleIn.Value, ListSrcFalseXIn.Value, ListSrcFalseYIn.Value)
                    Case 2 'Merc
                        Lon0 = JakStringFormats.DmsParse(ListSrcLon0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        TempCrs.SetProjectionMercator(ListSrcPrjFnameIn.Text, ListSrcPrjSnameIn.Text, Lon0, ListSrcScaleIn.Value, _
                                                      ListSrcFalseXIn.Value, ListSrcFalseYIn.Value)
                    Case 3 'LCC1
                        Lon0 = JakStringFormats.DmsParse(ListSrcLon0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        Lat0 = JakStringFormats.DmsParse(ListSrcLat0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        TempCrs.SetProjectionLambertConical1(ListSrcPrjFnameIn.Text, ListSrcPrjSnameIn.Text, Lon0, Lat0, _
                                                             ListSrcScaleIn.Value, ListSrcFalseXIn.Value, ListSrcFalseYIn.Value)
                    Case 4 'LCC2
                        Lon0 = JakStringFormats.DmsParse(ListSrcLon0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        Lat0 = JakStringFormats.DmsParse(ListSrcLat0In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        Lat1 = JakStringFormats.DmsParse(ListSrcLat1In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        Lat2 = JakStringFormats.DmsParse(ListSrcLat2In.Text, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix)
                        TempCrs.SetProjectionLambertConical2(ListSrcPrjFnameIn.Text, ListSrcPrjSnameIn.Text, Lon0, Lat0, Lat1, Lat2, _
                                                             ListSrcFalseXIn.Value, ListSrcFalseYIn.Value)
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

        End If
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
            GrdPrjTitle.Text = "Source Projection"
            GrdEllTitle.Text = "Source Ellipsoid"
            ListPrjTitle.Text = "Source Projection"
            ListEllTitle.Text = "Source Ellipsoid"
        Else
            'Target 
            GrdPrjTitle.Text = "Target Projection"
            GrdEllTitle.Text = "Target Ellipsoid"
            ListPrjTitle.Text = "Target Projection"
            ListEllTitle.Text = "Target Ellipsoid"
        End If
        ' >>> PROJECTION PANEL - GridView <<<
        'Fill the textboxes and the textblocks with the values
        SrcPrjFnameOut.Text = TempCrs.ProjectionFullName
        SrcPrjFnameIn.Text = TempCrs.ProjectionFullName
        SrcPrjSnameOut.Text = TempCrs.ProjectionShortName
        SrcPrjSnameIn.Text = TempCrs.ProjectionShortName
        SrcUtmZoneOut.Text = TempCrs.ProjectionUtmZone.ToString()
        SrcUtmZoneIn.SelectedIndex = TempCrs.ProjectionUtmZone - 1
        SrcUtmHemiIn.IsOn = TempCrs.ProjectionIsNorthHemisphere
        SrcLon0Out.Text = JakStringFormats.FormatDMS(TempCrs.ProjectionOriginLongitude, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, False)
        SrcLon0In.Text = JakStringFormats.FormatDMS(TempCrs.ProjectionOriginLongitude, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, False)
        SrcLat0Out.Text = JakStringFormats.FormatDMS(TempCrs.ProjectionOriginLatitude, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, True)
        SrcLat0In.Text = JakStringFormats.FormatDMS(TempCrs.ProjectionOriginLatitude, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, True)
        SrcLat1Out.Text = JakStringFormats.FormatDMS(TempCrs.LambertFirstParallel, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, True)
        SrcLat1In.Text = JakStringFormats.FormatDMS(TempCrs.LambertFirstParallel, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, True)
        SrcLat2Out.Text = JakStringFormats.FormatDMS(TempCrs.LambertSecondParallel, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, True)
        SrcLat2In.Text = JakStringFormats.FormatDMS(TempCrs.LambertSecondParallel, JakStringFormats.DmsFormat.VerboseDMS, JakStringFormats.DmsSign.Suffix, 4, True)
        SrcFalseXOut.Text = JakStringFormats.FormatNumber(TempCrs.ProjectionFalseEasting, 3)
        SrcFalseXIn.Value = TempCrs.ProjectionFalseEasting
        SrcFalseYOut.Text = JakStringFormats.FormatNumber(TempCrs.ProjectionFalseNorthing, 3)
        SrcFalseYIn.Value = TempCrs.ProjectionFalseNorthing
        SrcScaleOut.Text = JakStringFormats.FormatNumber(TempCrs.ProjectionScaleAtOrigin, 3)
        SrcScaleIn.Value = TempCrs.ProjectionScaleAtOrigin
        SrcPrjMethod.SelectedIndex = TempCrs.ProjectionCode

        ' >>> ELLIPSOID PANEL - GridView <<<
        SrcEllFname.SelectedIndex = SrcEllFname.Items.IndexOf(TempCrs.EllipsoidFullName)

        ' >>> TRANSFORMATION PANEL - GridView <<<
        SrcDtmFnameIn.Text = TempCrs.TransformationFullName
        SrcDtmSnameIn.Text = TempCrs.TransformationShortName
        SrcDtmDX.Value = TempCrs.TransformationDeltas.X
        SrcDtmDY.Value = TempCrs.TransformationDeltas.Y
        SrcDtmDZ.Value = TempCrs.TransformationDeltas.Z
        SrcDtmRX.Value = JakMathLib.RadDeg(TempCrs.TransformationRotations.X) * 3600 'Convert the radians values the seconds of degree
        SrcDtmRY.Value = JakMathLib.RadDeg(TempCrs.TransformationRotations.Y) * 3600 'Convert the radians values the seconds of degree
        SrcDtmRZ.Value = JakMathLib.RadDeg(TempCrs.TransformationRotations.Z) * 3600 'Convert the radians values the seconds of degree
        SrcDtmScale.Value = TempCrs.TransformationScaleFactorPPM
        SrcDtmPX.Value = TempCrs.TransformationRotationPoint.X
        SrcDtmPY.Value = TempCrs.TransformationRotationPoint.Y
        SrcDtmPZ.Value = TempCrs.TransformationRotationPoint.Z
        SrcDtmMethod.SelectedIndex = TempCrs.TransformationCode

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
        ListSrcFalseXIn.Value = TempCrs.ProjectionFalseEasting
        ListSrcFalseYOut.Text = JakStringFormats.FormatNumber(TempCrs.ProjectionFalseNorthing, 3)
        ListSrcFalseYIn.Value = TempCrs.ProjectionFalseNorthing
        ListSrcScaleOut.Text = JakStringFormats.FormatNumber(TempCrs.ProjectionScaleAtOrigin, 3)
        ListSrcScaleIn.Value = TempCrs.ProjectionScaleAtOrigin
        ListSrcPrjMethod.SelectedIndex = TempCrs.ProjectionCode

        ' >>> ELLIPSOID PANEL - ListView <<<
        ListSrcEllFname.SelectedIndex = SrcEllFname.Items.IndexOf(TempCrs.EllipsoidFullName)

        ' >>> TRANSFORMATION PANEL - ListView <<<
        ListSrcDtmFnameIn.Text = TempCrs.TransformationFullName
        ListSrcDtmSnameIn.Text = TempCrs.TransformationShortName
        ListSrcDtmDX.Value = TempCrs.TransformationDeltas.X
        ListSrcDtmDY.Value = TempCrs.TransformationDeltas.Y
        ListSrcDtmDZ.Value = TempCrs.TransformationDeltas.Z
        ListSrcDtmRX.Value = JakMathLib.RadDeg(TempCrs.TransformationRotations.X) * 3600 'Convert the radians values the seconds of degree
        ListSrcDtmRY.Value = JakMathLib.RadDeg(TempCrs.TransformationRotations.Y) * 3600 'Convert the radians values the seconds of degree
        ListSrcDtmRZ.Value = JakMathLib.RadDeg(TempCrs.TransformationRotations.Z) * 3600 'Convert the radians values the seconds of degree
        ListSrcDtmScale.Value = TempCrs.TransformationScaleFactorPPM
        ListSrcDtmPX.Value = TempCrs.TransformationRotationPoint.X
        ListSrcDtmPY.Value = TempCrs.TransformationRotationPoint.Y
        ListSrcDtmPZ.Value = TempCrs.TransformationRotationPoint.Z
        ListSrcDtmMethod.SelectedIndex = TempCrs.TransformationCode
        IsCrsUpdated = True
        AcceptButton.IsEnabled = False
        'ReEnable the events
        EventDisabled = False

    End Sub

    Private Sub SrcPrjMethod_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles SrcPrjMethod.SelectionChanged, ListSrcPrjMethod.SelectionChanged
        'Select witch fields are visible
        Dim SelID As Integer
        SelID = CType(sender, ComboBox).SelectedIndex
        Select Case SelID
            Case 1 'TMERC
                SrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

            Case 2 'MERC
                SrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

            Case 3 'LCC1
                SrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

            Case 4 'LCC2
                SrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

            Case Else 'UTM
                SrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjFnameIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjFnameOut.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcPrjSnameIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcPrjSnameOut.Visibility = Windows.UI.Xaml.Visibility.Visible

                SrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmZoneTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcUtmZoneIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcUtmZoneOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcUtmHemiTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcUtmHemiIn.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcUtmHemiOut.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLon0In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLon0Out.Visibility = Windows.UI.Xaml.Visibility.Visible

                SrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0Title.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcLat0In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat0Out.Visibility = Windows.UI.Xaml.Visibility.Visible

                SrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat1Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Title.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2In.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcLat2Out.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseXIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcFalseXOut.Visibility = Windows.UI.Xaml.Visibility.Visible

                SrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcFalseYIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcFalseYOut.Visibility = Windows.UI.Xaml.Visibility.Visible

                SrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcScaleIn.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcScaleOut.Visibility = Windows.UI.Xaml.Visibility.Visible

        End Select
        SomethingChanged()
    End Sub

    Private Sub SrcEllFname_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles SrcEllFname.SelectionChanged, ListSrcEllFname.SelectionChanged
        Dim SelID As Integer
        SelID = CType(sender, ComboBox).SelectedIndex

        SrcEllSname.Text = EpsgSname(SelID)
        SrcEllSma.Text = JakStringFormats.FormatNumber(EpsgSma(SelID), 3)
        SrcEllInvFlat.Text = JakStringFormats.FormatNumber(EpsgInvFlat(SelID), 9)
        SrcEllID.Text = EpsgId(SelID).ToString
        ListSrcEllSname.Text = EpsgSname(SelID)
        ListSrcEllSma.Text = JakStringFormats.FormatNumber(EpsgSma(SelID), 3)
        ListSrcEllInvFlat.Text = JakStringFormats.FormatNumber(EpsgInvFlat(SelID), 9)
        ListSrcEllID.Text = EpsgId(SelID).ToString
        'Update the CRS
        TempCrs.SetEllipsoidByEpsgID(EpsgId(SelID))
        SomethingChanged()
    End Sub

    Private Sub SrcDtmMethod_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles SrcDtmMethod.SelectionChanged, ListSrcDtmMethod.SelectionChanged
        Dim SelID As Integer
        SelID = CType(sender, ComboBox).SelectedIndex
        Select Case SelID
            Case 1, 2 '7 parameters
                SrcDtmRXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmRX.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmRYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmRY.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmRZTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmRZ.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmScale.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRX.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRY.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRZTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRZ.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmScale.Visibility = Windows.UI.Xaml.Visibility.Visible

                SrcDtmPXTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmPX.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmPYTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmPY.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmPZTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmPZ.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPXTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPX.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPYTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPY.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPZTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmPZ.Visibility = Windows.UI.Xaml.Visibility.Collapsed

            Case 3 '10 parameters
                SrcDtmRXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmRX.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmRYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmRY.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmRZTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmRZ.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmScale.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRX.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRY.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRZTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmRZ.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmScale.Visibility = Windows.UI.Xaml.Visibility.Visible

                SrcDtmPXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmPX.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmPYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmPY.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmPZTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                SrcDtmPZ.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmPXTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmPX.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmPYTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmPY.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmPZTitle.Visibility = Windows.UI.Xaml.Visibility.Visible
                ListSrcDtmPZ.Visibility = Windows.UI.Xaml.Visibility.Visible

            Case Else '3 parameters
                SrcDtmRXTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmRX.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmRYTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmRY.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmRZTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmRZ.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmScale.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmRXTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmRX.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmRYTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmRY.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmRZTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmRZ.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmScaleTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                ListSrcDtmScale.Visibility = Windows.UI.Xaml.Visibility.Collapsed

                SrcDtmPXTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmPX.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmPYTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmPY.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmPZTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                SrcDtmPZ.Visibility = Windows.UI.Xaml.Visibility.Collapsed
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

    Private Sub SrcUtm_Changed() Handles ListSrcUtmZoneIn.SelectionChanged, SrcUtmZoneIn.SelectionChanged, SrcUtmHemiIn.Toggled, ListSrcUtmHemiIn.Toggled
        If EventDisabled Then Exit Sub
        If ListPanels.Visibility = Windows.UI.Xaml.Visibility.Collapsed Then
            TempCrs.SetProjectionUTM(SrcUtmZoneIn.SelectedIndex + 1, SrcUtmHemiIn.IsOn)
        Else
            TempCrs.SetProjectionUTM(ListSrcUtmZoneIn.SelectedIndex + 1, ListSrcUtmHemiIn.IsOn)
        End If
        UpdateUI()
        SomethingChanged()
    End Sub

    Private Sub SomethingChanged() Handles _
        SrcPrjFnameIn.TextChanged, SrcPrjSnameIn.TextChanged, _
        SrcLon0In.TextChanged, SrcLat0In.TextChanged, SrcLat1In.TextChanged, SrcLat2In.TextChanged, _
        SrcFalseXIn.ValueChanged, SrcFalseYIn.ValueChanged, SrcScaleIn.ValueChanged, _
        SrcDtmDX.ValueChanged, SrcDtmDY.ValueChanged, SrcDtmDZ.ValueChanged, _
        SrcDtmRX.ValueChanged, SrcDtmRY.ValueChanged, SrcDtmRZ.ValueChanged, SrcDtmScale.ValueChanged, _
        SrcDtmPX.ValueChanged, SrcDtmPY.ValueChanged, SrcDtmPZ.ValueChanged, _
        ListSrcPrjFnameIn.TextChanged, ListSrcPrjSnameIn.TextChanged, _
        ListSrcLon0In.TextChanged, ListSrcLat0In.TextChanged, ListSrcLat1In.TextChanged, ListSrcLat2In.TextChanged, _
        ListSrcFalseXIn.ValueChanged, ListSrcFalseYIn.ValueChanged, ListSrcScaleIn.ValueChanged, _
        ListSrcDtmDX.ValueChanged, ListSrcDtmDY.ValueChanged, ListSrcDtmDZ.ValueChanged, _
        ListSrcDtmRX.ValueChanged, ListSrcDtmRY.ValueChanged, ListSrcDtmRZ.ValueChanged, ListSrcDtmScale.ValueChanged, _
        ListSrcDtmPX.ValueChanged, ListSrcDtmPY.ValueChanged, ListSrcDtmPZ.ValueChanged

        If EventDisabled Then Exit Sub
        IsCrsUpdated = False
        AcceptButton.IsEnabled = True
    End Sub
End Class
