Imports JakMathLib
Imports JakStringFormats
Imports System.Math

Public Class GeoDatum
    Public Enum TransformationMethod
        Geocentric = 0
        PositionVector = 1
        CoordinateFrame = 2
        MolodenskyBadekas = 3
    End Enum

    Public Enum ProjectionMethod
        UTM = 0
        Tmerc = 1
        Merc = 2
        Lcc1 = 3
        Lcc2 = 4
    End Enum

    Public Enum CrsFormat
        Xml = 1
        Esri = 2
    End Enum

    Private Structure iEll
        Dim sma, invflat As Double
        Dim sname, fname As String
        Dim id As Integer
    End Structure

    Dim iEllFName, iEllSName, iPrjFName, iPrjSName, iShiftFName, iShiftSName As String
    Dim iEllSMAxis, iEllInvFlat, iPrjScale, iShiftScale As Double
    Dim iEllID, iPrjUTMZone As Integer
    Dim iPrjHemi As Boolean
    Dim iPrjOrigin, iPrjFalse, iPrjParallel, iShiftDelta, iShiftRot, iShiftPoint As New Point3D
    Dim iShiftMethod As New TransformationMethod
    Dim iPrjMethod As New ProjectionMethod
    Dim iEpsg() As iEll

    'Define the properties
    ReadOnly Property EllipsoidFullName As String
        Get
            Return iEllFName
        End Get
    End Property

    ReadOnly Property EllipsoidShortName As String
        Get
            Return iEllSName
        End Get
    End Property

    ReadOnly Property EllipsoidID As Integer
        Get
            Return iEllID
        End Get
    End Property

    ReadOnly Property EllipsoidSemiMayorAxis As Double
        Get
            Return iEllSMAxis
        End Get
    End Property

    ReadOnly Property EllipsoidInverseFlattening As Double
        Get
            Return iEllInvFlat
        End Get
    End Property

    ReadOnly Property ProjectionType As String
        Get
            Dim tmpstring() As String = EnumerateProjectionMethods()
            Select Case iPrjMethod
                Case ProjectionMethod.UTM
                    Return tmpstring(0)
                Case ProjectionMethod.Tmerc
                    Return tmpstring(1)
                Case ProjectionMethod.Merc
                    Return tmpstring(2)
                Case ProjectionMethod.Lcc1
                    Return tmpstring(3)
                Case ProjectionMethod.Lcc2
                    Return tmpstring(4)
                Case Else
                    Return iPrjMethod.ToString
            End Select
        End Get
    End Property

    ReadOnly Property ProjectionCode As ProjectionMethod
        Get
            Return iPrjMethod
        End Get
    End Property

    ReadOnly Property ProjectionOriginLatitude As Double
        Get
            Return iPrjOrigin.Y
        End Get
    End Property

    ReadOnly Property ProjectionOriginLongitude As Double
        Get
            Return iPrjOrigin.X
        End Get
    End Property

    ReadOnly Property LambertFirstParallel As Double
        Get
            Return iPrjParallel.X
        End Get
    End Property

    ReadOnly Property LambertSecondParallel As Double
        Get
            Return iPrjParallel.Y
        End Get
    End Property

    ReadOnly Property ProjectionScaleAtOrigin As Double
        Get
            Return iPrjScale
        End Get
    End Property

    ReadOnly Property ProjectionFalseEasting As Double
        Get
            Return iPrjFalse.X
        End Get
    End Property

    ReadOnly Property ProjectionFalseNorthing As Double
        Get
            Return iPrjFalse.Y
        End Get
    End Property

    ReadOnly Property ProjectionUtmZone As Integer
        Get
            Return iPrjUTMZone
        End Get
    End Property

    ReadOnly Property ProjectionIsNorthHemisphere As Boolean
        Get
            Return iPrjHemi
        End Get
    End Property

    ReadOnly Property ProjectionFullName As String
        Get
            Return iPrjFName
        End Get
    End Property

    ReadOnly Property ProjectionShortName As String
        Get
            Return iPrjSName
        End Get
    End Property

    ReadOnly Property TransformationDeltas As Point3D
        Get
            Return iShiftDelta
        End Get
    End Property

    ReadOnly Property TransformationRotations As Point3D
        Get
            Return iShiftRot
        End Get
    End Property

    ReadOnly Property TransformationRotationPoint As Point3D
        Get
            Return iShiftPoint
        End Get
    End Property

    ReadOnly Property TransformationScaleFactorPPM As Double
        Get
            Return iShiftScale
        End Get
    End Property

    ReadOnly Property TransformationType As String
        Get
            Select Case iShiftMethod
                Case TransformationMethod.Geocentric
                    Return "Geocentric Translation (3 parameters)"
                Case TransformationMethod.PositionVector
                    Return "Position Vector Transformation (7 parameters UK)"
                Case TransformationMethod.CoordinateFrame
                    Return "Coordinate Frame Rotation (7 parameters US)"
                Case TransformationMethod.MolodenskyBadekas
                    Return "Molodensky-Badekas Transformation (10 parameters)"
                Case Else
                    Return iShiftMethod.ToString
            End Select
        End Get
    End Property

    ReadOnly Property TransformationCode As TransformationMethod
        Get
            Return iShiftMethod
        End Get
    End Property

    ReadOnly Property TransformationFullName As String
        Get
            Return iShiftFName
        End Get
    End Property

    ReadOnly Property TransformationShortName As String
        Get
            Return iShiftSName
        End Get
    End Property

    ''' <summary>
    ''' Return an array of the available projection methods names
    ''' </summary>
    ''' <returns>Array of names</returns>
    ''' <remarks></remarks>
    Public Function EnumerateProjectionMethods() As String()
        Dim outnames(4) As String
        outnames(0) = "Universal Transverse of Mercator"
        outnames(1) = "Transverse of Mercator"
        outnames(2) = "Direct of Mercator"
        outnames(3) = "Lambert Conformal Conical (1 standard parallel)"
        outnames(4) = "Lambert Conformal Conical (2 standard parallels)"
        Return outnames
    End Function

    ''' <summary>
    ''' Return an array of the available datum shift methods names
    ''' </summary>
    ''' <returns>Array of names</returns>
    ''' <remarks></remarks>
    Public Function EnumerateShiftingMethods() As String()
        Dim outnames(3) As String
        outnames(0) = "Geocentric Translation (3 parameters)"
        outnames(1) = "Position Vector Transformation (7 parameters UK)"
        outnames(2) = "Coordinate Frame Rotation (7 parameters US)"
        outnames(3) = "Molodensky-Badekas Transformation (10 parameters)"
        Return outnames
    End Function

    ''' <summary>
    ''' Return a series of arrays for the
    ''' </summary>
    ''' <param name="EpsgIDs">Array of the IDs</param>
    ''' <param name="EpsgNames">Array of the Official Names</param>
    ''' <param name="ShortNames">Array of the Shortened Names (for compatibility reasons)</param>
    ''' <param name="EpsgSemiMayorAxis">Array of the Semi mayor axis dimensions</param>
    ''' <param name="EpsgInverseFlattenings">Array of the inverse flattening values</param>
    ''' <returns>Always TRUE</returns>
    ''' <remarks></remarks>
    Public Function EnumerateEpsgEllipsoids(ByRef EpsgIDs() As Integer, ByRef EpsgNames() As String, _
                                            ByRef ShortNames() As String, ByRef EpsgSemiMayorAxis() As Double, _
                                            ByRef EpsgInverseFlattenings() As Double) As Boolean
        ReDim EpsgIDs(iEpsg.Count - 1)
        ReDim EpsgNames(iEpsg.Count - 1)
        ReDim ShortNames(iEpsg.Count - 1)
        ReDim EpsgSemiMayorAxis(iEpsg.Count - 1)
        ReDim EpsgInverseFlattenings(iEpsg.Count - 1)
        For el = 0 To iEpsg.Count - 1
            EpsgIDs(el) = iEpsg(el).id
            EpsgNames(el) = iEpsg(el).fname
            ShortNames(el) = iEpsg(el).sname
            EpsgSemiMayorAxis(el) = iEpsg(el).sma
            EpsgInverseFlattenings(el) = iEpsg(el).invflat
        Next
        Return True
    End Function

    ''' <summary>
    ''' Create a string that is parsable by the sub New(String)
    ''' </summary>
    ''' <returns>A formatted string</returns>
    ''' <remarks></remarks>
    Public Overrides Function ToString() As String
        Dim OutString As String
        OutString = "CRS|" & iEllID.ToString & "|" & iPrjFName & "|" & iPrjSName
        Select Case iPrjMethod
            Case ProjectionMethod.UTM
                OutString = OutString & "|UTM|" & iPrjUTMZone.ToString & "|" & If(iPrjHemi, "N", "S")
            Case ProjectionMethod.Tmerc
                OutString = OutString & "|TMERC|" & iPrjOrigin.X.ToString & "|" & iPrjOrigin.Y.ToString & "|" & iPrjScale.ToString
                OutString = OutString & "|" & iPrjFalse.X.ToString & "|" & iPrjFalse.Y.ToString
            Case ProjectionMethod.Merc
                OutString = OutString & "|MERC|" & iPrjOrigin.X.ToString & "|" & iPrjScale.ToString
                OutString = OutString & "|" & iPrjFalse.X.ToString & "|" & iPrjFalse.Y.ToString
            Case ProjectionMethod.Lcc1
                OutString = OutString & "|LCC1|" & iPrjOrigin.X.ToString & "|" & iPrjOrigin.Y.ToString & "|" & iPrjScale.ToString
                OutString = OutString & "|" & iPrjFalse.X.ToString & "|" & iPrjFalse.Y.ToString
            Case ProjectionMethod.Lcc2
                OutString = OutString & "|LCC2|" & iPrjOrigin.X.ToString & "|" & iPrjOrigin.Y.ToString
                OutString = OutString & "|" & iPrjParallel.X.ToString & "|" & iPrjParallel.Y.ToString
                OutString = OutString & "|" & iPrjFalse.X.ToString & "|" & iPrjFalse.Y.ToString
        End Select
        OutString = OutString & "|" & iShiftFName & "|" & iShiftSName
        Select Case iShiftMethod
            Case TransformationMethod.Geocentric
                OutString = OutString & "|3|" & iShiftDelta.X.ToString & "|" & iShiftDelta.Y.ToString & "|" & iShiftDelta.Z.ToString
            Case TransformationMethod.PositionVector, TransformationMethod.CoordinateFrame
                OutString = OutString & "|7|" & iShiftDelta.X.ToString & "|" & iShiftDelta.Y.ToString & "|" & iShiftDelta.Z.ToString
                OutString = OutString & "|" & iShiftRot.X.ToString & "|" & iShiftRot.Y.ToString & "|" & iShiftRot.Z.ToString
                OutString = OutString & "|" & iShiftScale.ToString & "|" & If(iShiftMethod = TransformationMethod.PositionVector, "PV", "CF")
            Case TransformationMethod.MolodenskyBadekas
                OutString = OutString & "|10|" & iShiftDelta.X.ToString & "|" & iShiftDelta.Y.ToString & "|" & iShiftDelta.Z.ToString
                OutString = OutString & "|" & iShiftRot.X.ToString & "|" & iShiftRot.Y.ToString & "|" & iShiftRot.Z.ToString
                OutString = OutString & "|" & iShiftScale.ToString
                OutString = OutString & "|" & iShiftPoint.X.ToString & "|" & iShiftPoint.Y.ToString & "|" & iShiftPoint.Z.ToString
        End Select
        Return OutString
    End Function

    ''' <summary>
    ''' Create a new default datum
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        'Populate the EPSG list
        PopulateEpsgList()
        'Fill the default ellipsoid parameters
        iEllID = iEpsg(39).id
        iEllInvFlat = iEpsg(39).invflat
        iEllFName = iEpsg(39).fname
        iEllSName = iEpsg(39).sname
        iEllSMAxis = iEpsg(39).sma
        'Fill the default projection parameters
        iPrjMethod = ProjectionMethod.UTM
        iPrjUTMZone = 32
        iPrjHemi = True
        iPrjOrigin.X = DegRad(9)
        iPrjOrigin.Y = 0
        iPrjParallel.X = 0
        iPrjParallel.Y = 0
        iPrjScale = 0.9996
        iPrjFalse.X = 500000
        iPrjFalse.Y = 0
        iPrjFName = "U.T.M. Zone 32 North"
        iPrjSName = "UTM_32_N"
        'Fill the default shifting parameters
        iShiftMethod = TransformationMethod.Geocentric
        iShiftFName = "W.G.S. 1984"
        iShiftSName = "WGS_1984"
        iShiftDelta.X = 0
        iShiftDelta.Y = 0
        iShiftDelta.Z = 0
        iShiftRot.X = 0
        iShiftRot.Y = 0
        iShiftRot.Z = 0
        iShiftPoint.X = 0
        iShiftPoint.Y = 0
        iShiftPoint.Z = 0
        iShiftScale = 0
    End Sub

    ''' <summary>
    ''' Create a new Datum from the formatted string
    ''' </summary>
    ''' <param name="CrsString">A string created from ToString() function</param>
    ''' <remarks></remarks>
    Public Sub New(CrsString As String)
        'Populate the EPSG list
        PopulateEpsgList()
        'Parse the input string
        Dim SplittedString() As String
        Dim ShiftBase As Integer
        SplittedString = CrsString.Split(CChar("|"))
        If SplittedString.Count > 12 Then
            If SplittedString(0) = "CRS" Then
                Me.SetEllipsoidByEpsgID(CInt(SplittedString(1)))
                Select Case SplittedString(4)
                    Case "UTM"
                        Me.SetProjectionUTM(CInt(SplittedString(5)), If(SplittedString(6) = "N", True, False))
                        ShiftBase = 7
                    Case "TMERC"
                        Me.SetProjectionTransverseMercator(SplittedString(2), SplittedString(3), CDbl(SplittedString(5)), CDbl(SplittedString(6)), _
                                                           CDbl(SplittedString(7)), CDbl(SplittedString(8)), CDbl(SplittedString(9)))
                        ShiftBase = 10
                    Case "MERC"
                        Me.SetProjectionMercator(SplittedString(2), SplittedString(3), CDbl(SplittedString(5)), CDbl(SplittedString(6)), _
                                                           CDbl(SplittedString(7)), CDbl(SplittedString(8)))
                        ShiftBase = 9
                    Case "LCC1"
                        Me.SetProjectionLambertConical1(SplittedString(2), SplittedString(3), CDbl(SplittedString(5)), CDbl(SplittedString(6)), _
                                                           CDbl(SplittedString(7)), CDbl(SplittedString(8)), CDbl(SplittedString(9)))
                        ShiftBase = 10
                    Case "LCC2"
                        Me.SetProjectionLambertConical2(SplittedString(2), SplittedString(3), CDbl(SplittedString(5)), CDbl(SplittedString(6)), _
                                   CDbl(SplittedString(7)), CDbl(SplittedString(8)), CDbl(SplittedString(9)), CDbl(SplittedString(10)))
                        ShiftBase = 11
                End Select

                Select Case SplittedString(ShiftBase + 2)
                    Case "3"
                        If SplittedString.Count > ShiftBase + 5 Then
                            Me.SetDatumConversion(SplittedString(ShiftBase), SplittedString(ShiftBase + 1), CDbl(SplittedString(ShiftBase + 3)), _
                                                  CDbl(SplittedString(ShiftBase + 4)), CDbl(SplittedString(ShiftBase + 5)))
                        Else
                            Me.SetDatumConversion("W.G.S. 1984", "WGS_1984", 0.0, 0.0, 0.0)
                        End If
                    Case "7"
                        If SplittedString.Count > ShiftBase + 10 Then
                            Me.SetDatumConversion(SplittedString(ShiftBase), SplittedString(ShiftBase + 1), CDbl(SplittedString(ShiftBase + 3)), _
                                                  CDbl(SplittedString(ShiftBase + 4)), CDbl(SplittedString(ShiftBase + 5)), CDbl(SplittedString(ShiftBase + 6)), _
                                                  CDbl(SplittedString(ShiftBase + 7)), CDbl(SplittedString(ShiftBase + 8)), CDbl(SplittedString(ShiftBase + 9)), _
                                                  If(SplittedString(ShiftBase + 10) = "PV", True, False))
                        Else
                            Me.SetDatumConversion("W.G.S. 1984", "WGS_1984", 0.0, 0.0, 0.0)
                        End If

                    Case "10"
                        If SplittedString.Count > ShiftBase + 12 Then
                            Me.SetDatumConversion(SplittedString(ShiftBase), SplittedString(ShiftBase + 1), CDbl(SplittedString(ShiftBase + 3)), _
                                                    CDbl(SplittedString(ShiftBase + 4)), CDbl(SplittedString(ShiftBase + 5)), CDbl(SplittedString(ShiftBase + 6)), _
                                                    CDbl(SplittedString(ShiftBase + 7)), CDbl(SplittedString(ShiftBase + 8)), CDbl(SplittedString(ShiftBase + 9)), _
                                                    CDbl(SplittedString(ShiftBase + 10)), CDbl(SplittedString(ShiftBase + 11)), CDbl(SplittedString(ShiftBase + 12)))
                        Else
                            Me.SetDatumConversion("W.G.S. 1984", "WGS_1984", 0.0, 0.0, 0.0)
                        End If
                End Select
            End If
        End If
    End Sub

    ''' <summary>
    ''' Populate the EPSG known ellipsoid array
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateEpsgList()
        ReDim iEpsg(39)
        iEpsg(0).id = 7001
        iEpsg(0).fname = "Airy 1830"
        iEpsg(0).sname = "AIRY_1830"
        iEpsg(0).sma = 6377563.396
        iEpsg(0).invflat = 299.3249646
        iEpsg(1).id = 7002
        iEpsg(1).fname = "Airy Modified 1849"
        iEpsg(1).sname = "AIRY_1849"
        iEpsg(1).sma = 6377340.189
        iEpsg(1).invflat = 299.3249646
        iEpsg(2).id = 7003
        iEpsg(2).fname = "Australian National Spheroid"
        iEpsg(2).sname = "AUSTRALIAN"
        iEpsg(2).sma = 6378160
        iEpsg(2).invflat = 298.25
        iEpsg(3).id = 7041
        iEpsg(3).fname = "Average Terrestrial System 1977"
        iEpsg(3).sname = "ATS_1977"
        iEpsg(3).sma = 6378135
        iEpsg(3).invflat = 298.257
        iEpsg(4).id = 7004
        iEpsg(4).fname = "Bessel 1841"
        iEpsg(4).sname = "BESSEL_1841"
        iEpsg(4).sma = 6377397.155
        iEpsg(4).invflat = 299.1528128
        iEpsg(5).id = 7005
        iEpsg(5).fname = "Bessel Modified"
        iEpsg(5).sname = "BESSEL_MOD"
        iEpsg(5).sma = 6377492.018
        iEpsg(5).invflat = 299.1528128
        iEpsg(6).id = 1024
        iEpsg(6).fname = "CGCS2000"
        iEpsg(6).sname = "CGCS_2000"
        iEpsg(6).sma = 6378137
        iEpsg(6).invflat = 298.2572221
        iEpsg(7).id = 7008
        iEpsg(7).fname = "Clarke 1866"
        iEpsg(7).sname = "CLARKE_1866"
        iEpsg(7).sma = 6378206.4
        iEpsg(7).invflat = 294.9786982
        iEpsg(8).id = 7013
        iEpsg(8).fname = "Clarke 1880 (Arc)"
        iEpsg(8).sname = "CLARKE80_ARC"
        iEpsg(8).sma = 6378249.145
        iEpsg(8).invflat = 293.4663077
        iEpsg(9).id = 7010
        iEpsg(9).fname = "Clarke 1880 (Benoit)"
        iEpsg(9).sname = "CLARKE80_BEN"
        iEpsg(9).sma = 6378300.789
        iEpsg(9).invflat = 293.4663155
        iEpsg(10).id = 7011
        iEpsg(10).fname = "Clarke 1880 (IGN)"
        iEpsg(10).sname = "CLARKE80_IGN"
        iEpsg(10).sma = 6378249.2
        iEpsg(10).invflat = 293.4660213
        iEpsg(11).id = 7012
        iEpsg(11).fname = "Clarke 1880 (RGS)"
        iEpsg(11).sname = "CLARKE80_RGS"
        iEpsg(11).sma = 6378249.145
        iEpsg(11).invflat = 293.465
        iEpsg(12).id = 7014
        iEpsg(12).fname = "Clarke 1880 (SGA 1922)"
        iEpsg(12).sname = "CLARKE80_SGA"
        iEpsg(12).sma = 6378249.2
        iEpsg(12).invflat = 293.46598
        iEpsg(13).id = 7051
        iEpsg(13).fname = "Danish 1876"
        iEpsg(13).sname = "DANISH_1876"
        iEpsg(13).sma = 6377019.27
        iEpsg(13).invflat = 300
        iEpsg(14).id = 7015
        iEpsg(14).fname = "Everest 1830 (1937 Adjustment)"
        iEpsg(14).sname = "EVEREST_1937"
        iEpsg(14).sma = 6377276.345
        iEpsg(14).invflat = 300.8017
        iEpsg(15).id = 7044
        iEpsg(15).fname = "Everest 1830 (1962 Definition)"
        iEpsg(15).sname = "EVEREST_1962"
        iEpsg(15).sma = 6377301.243
        iEpsg(15).invflat = 300.8017255
        iEpsg(16).id = 7016
        iEpsg(16).fname = "Everest 1830 (1967 Definition)"
        iEpsg(16).sname = "EVEREST_1967"
        iEpsg(16).sma = 6377298.556
        iEpsg(16).invflat = 300.8017
        iEpsg(17).id = 7045
        iEpsg(17).fname = "Everest 1830 (1975 Definition)"
        iEpsg(17).sname = "EVEREST_1975"
        iEpsg(17).sma = 6377299.151
        iEpsg(17).invflat = 300.8017255
        iEpsg(18).id = 7056
        iEpsg(18).fname = "Everest 1830 (RSO 1969)"
        iEpsg(18).sname = "EVEREST_1969"
        iEpsg(18).sma = 6377295.664
        iEpsg(18).invflat = 300.8017
        iEpsg(19).id = 7018
        iEpsg(19).fname = "Everest 1830 Modified"
        iEpsg(19).sname = "EVEREST_1830"
        iEpsg(19).sma = 6377304.063
        iEpsg(19).invflat = 300.8017
        iEpsg(20).id = 7031
        iEpsg(20).fname = "GEM 10C"
        iEpsg(20).sname = "GEM_10C"
        iEpsg(20).sma = 6378137
        iEpsg(20).invflat = 298.2572236
        iEpsg(21).id = 7036
        iEpsg(21).fname = "GRS 1967"
        iEpsg(21).sname = "GRS_1967"
        iEpsg(21).sma = 6378160
        iEpsg(21).invflat = 298.2471674
        iEpsg(22).id = 7050
        iEpsg(22).fname = "GRS 1967 Modified"
        iEpsg(22).sname = "GRS_1967_MOD"
        iEpsg(22).sma = 6378160
        iEpsg(22).invflat = 298.25
        iEpsg(23).id = 7019
        iEpsg(23).fname = "GRS 1980"
        iEpsg(23).sname = "GRS_1980"
        iEpsg(23).sma = 6378137
        iEpsg(23).invflat = 298.2572221
        iEpsg(24).id = 7020
        iEpsg(24).fname = "Helmert 1906"
        iEpsg(24).sname = "HELMERT_1906"
        iEpsg(24).sma = 6378200
        iEpsg(24).invflat = 298.3
        iEpsg(25).id = 7053
        iEpsg(25).fname = "Hough 1960"
        iEpsg(25).sname = "HOUGH_1960"
        iEpsg(25).sma = 6378270
        iEpsg(25).invflat = 297
        iEpsg(26).id = 7058
        iEpsg(26).fname = "Hughes 1980"
        iEpsg(26).sname = "HUGHES_1980"
        iEpsg(26).sma = 6378273
        iEpsg(26).invflat = 298.2794111
        iEpsg(27).id = 7049
        iEpsg(27).fname = "IAG 1975"
        iEpsg(27).sname = "IAG_1975"
        iEpsg(27).sma = 6378140
        iEpsg(27).invflat = 298.257
        iEpsg(28).id = 7021
        iEpsg(28).fname = "Indonesian National Spheroid"
        iEpsg(28).sname = "INDONESIAN"
        iEpsg(28).sma = 6378160
        iEpsg(28).invflat = 298.247
        iEpsg(29).id = 7022
        iEpsg(29).fname = "International 1924"
        iEpsg(29).sname = "INTL_1924"
        iEpsg(29).sma = 6378388
        iEpsg(29).invflat = 297
        iEpsg(30).id = 7024
        iEpsg(30).fname = "Krassowsky 1940"
        iEpsg(30).sname = "KRASSOWSKY"
        iEpsg(30).sma = 6378245
        iEpsg(30).invflat = 298.3
        iEpsg(31).id = 7025
        iEpsg(31).fname = "NWL 9D"
        iEpsg(31).sname = "NWL_9D"
        iEpsg(31).sma = 6378145
        iEpsg(31).invflat = 298.25
        iEpsg(32).id = 7032
        iEpsg(32).fname = "OSU86F"
        iEpsg(32).sname = "OSU86F"
        iEpsg(32).sma = 6378136.2
        iEpsg(32).invflat = 298.2572236
        iEpsg(33).id = 7033
        iEpsg(33).fname = "OSU91A"
        iEpsg(33).sname = "OSU91A"
        iEpsg(33).sma = 6378136.3
        iEpsg(33).invflat = 298.2572236
        iEpsg(34).id = 7027
        iEpsg(34).fname = "Plessis 1817"
        iEpsg(34).sname = "PLESSIS_1817"
        iEpsg(34).sma = 6376523
        iEpsg(34).invflat = 308.64
        iEpsg(35).id = 7054
        iEpsg(35).fname = "PZ-90"
        iEpsg(35).sname = "PZ_90"
        iEpsg(35).sma = 6378136
        iEpsg(35).invflat = 298.2578393
        iEpsg(36).id = 7028
        iEpsg(36).fname = "Struve 1860"
        iEpsg(36).sname = "STRUVE_1860"
        iEpsg(36).sma = 6378298.3
        iEpsg(36).invflat = 294.73
        iEpsg(37).id = 7029
        iEpsg(37).fname = "War Office"
        iEpsg(37).sname = "WAR_OFFICE"
        iEpsg(37).sma = 6378300
        iEpsg(37).invflat = 296
        iEpsg(38).id = 7043
        iEpsg(38).fname = "World Geodetic System 1972"
        iEpsg(38).sname = "WGS_1972"
        iEpsg(38).sma = 6378135
        iEpsg(38).invflat = 298.26
        iEpsg(39).id = 7030
        iEpsg(39).fname = "World Geodetic System 1984"
        iEpsg(39).sname = "WGS_1984"
        iEpsg(39).sma = 6378137
        iEpsg(39).invflat = 298.2572236
    End Sub

    ''' <summary>
    ''' Set the datum ellipsoid from the known EPSG ellipsoids
    ''' </summary>
    ''' <param name="EpsgID"></param>
    ''' <remarks></remarks>
    Public Sub SetEllipsoidByEpsgID(EpsgID As Integer)
        Dim EpsgEllCount As Integer = iEpsg.Count - 1
        Dim EllFound As Boolean = False
        'Look for a matching ID
        For el = 0 To EpsgEllCount
            If EpsgID = iEpsg(el).id Then
                iEllID = iEpsg(el).id
                iEllInvFlat = iEpsg(el).invflat
                iEllFName = iEpsg(el).fname
                iEllSName = iEpsg(el).sname
                iEllSMAxis = iEpsg(el).sma
                EllFound = True
                Exit For
            End If
        Next
        If EllFound = False Then
            iEllID = iEpsg(EpsgEllCount).id
            iEllInvFlat = iEpsg(EpsgEllCount).invflat
            iEllFName = iEpsg(EpsgEllCount).fname
            iEllSName = iEpsg(EpsgEllCount).sname
            iEllSMAxis = iEpsg(EpsgEllCount).sma
        End If
    End Sub

    ''' <summary>
    ''' Set the datum ellipsoid from the given parameters
    ''' </summary>
    ''' <param name="ID">Ellipsoid ID</param>
    ''' <param name="FullName">Ellipsoid Name</param>
    ''' <param name="ShortName">Shortened Ellipsoid Name (for compatibility reasons)</param>
    ''' <param name="SemiMayorAxis">Ellipsoid Semi Mayor Axis</param>
    ''' <param name="InverseFlattening">Ellipsoid Inverse Flattening</param>
    ''' <remarks></remarks>
    Public Sub SetEllipsoidByValues(FullName As String, ShortName As String, ID As Integer, _
                                    SemiMayorAxis As Double, InverseFlattening As Double)
        iEllID = ID
        iEllInvFlat = InverseFlattening
        iEllFName = FullName
        iEllSName = ShortName
        iEllSMAxis = SemiMayorAxis
        'verify the ShortName
        If CountCharacter(iEllSName, CChar(" ")) > 0 Then
            iEllSName = DeleteExtraSpaceChar(iEllSName)
            iEllSName = iEllSName.Replace(" ", "_")
            iEllSName = iEllSName.Replace("(", "")
            iEllSName = iEllSName.Replace(")", "")
        End If
        If iEllSName.Count > 12 Then
            iEllSName = iEllSName.Replace("_", "")
        End If
        If iEllSName.Count > 12 Then
            iEllSName = iEllSName.Substring(0, 12)
        End If
    End Sub

    ''' <summary>
    ''' Set the projection as U.T.M. with the given parameters
    ''' </summary>
    ''' <param name="Zone">U.T.M. Zone (from 1 to 60)</param>
    ''' <param name="IsNorthHemisphere">True if North Hemisphere, False otherwise</param>
    ''' <remarks></remarks>
    Public Sub SetProjectionUTM(Zone As Integer, IsNorthHemisphere As Boolean)
        iPrjMethod = ProjectionMethod.UTM
        If Zone < 61 And Zone > 0 Then
            iPrjUTMZone = Zone
        Else
            iPrjUTMZone = 31
        End If
        iPrjHemi = IsNorthHemisphere
        iPrjOrigin.X = DegRad(((iPrjUTMZone - 1) * 6) - 180 + 3)
        iPrjOrigin.Y = 0.0
        iPrjFalse.X = 500000.0
        iPrjFalse.Y = If(iPrjHemi, 0.0, 10000000.0)
        iPrjParallel.X = 0.0
        iPrjParallel.Y = 0.0
        iPrjScale = 0.9996
        iPrjFName = "U.T.M. Zone " & iPrjUTMZone & If(iPrjHemi, " North", " South")
        iPrjSName = "UTM_" & iPrjUTMZone.ToString("00") & If(iPrjHemi, "_N", "_S")
    End Sub

    Public Sub SetProjectionUTM(Longitude As Double, Latitude As Double)
        iPrjMethod = ProjectionMethod.UTM
        'Calculate the zone by the given longitude
        Dim LongDeg As Double = RadDeg(Longitude)
        Dim TmpCM As Double
        For z As Double = 1.0 To 60.0
            TmpCM = z * 6 - 183
            If Abs(TmpCM - LongDeg) < 3.0 Then
                'Given point inside the zone
                iPrjOrigin.X = DegRad(TmpCM)
                iPrjUTMZone = CInt(z)
                Exit For
            End If
        Next
        iPrjHemi = If(Latitude >= 0.0, True, False)
        iPrjOrigin.Y = 0.0
        iPrjFalse.X = 500000.0
        iPrjFalse.Y = If(iPrjHemi, 0.0, 10000000.0)
        iPrjParallel.X = 0.0
        iPrjParallel.Y = 0.0
        iPrjScale = 0.9996
        iPrjFName = "U.T.M. Zone " & iPrjUTMZone & If(iPrjHemi, " North", " South")
        iPrjSName = "UTM_" & iPrjUTMZone.ToString("00") & If(iPrjHemi, "_N", "_S")

    End Sub
    ''' <summary>
    ''' Set the projection as Transverse of Mercator with the given parameters
    ''' </summary>
    ''' <param name="OriginLongitude">Longitude at Origin</param>
    ''' <param name="OriginLatitude">Latitude at Origin</param>
    ''' <param name="OriginScale">Scale at Origin</param>
    ''' <param name="FalseEasting">Easting at Origin</param>
    ''' <param name="FalseNorthing">Northing at Origin</param>
    ''' <param name="FullName">Projection Name</param>
    ''' <param name="ShortName">Shortened Projection Name (for compatibility reasons)</param>
    ''' <remarks></remarks>
    Public Sub SetProjectionTransverseMercator(FullName As String, ShortName As String, _
                                               OriginLongitude As Double, OriginLatitude As Double, _
                                               OriginScale As Double, FalseEasting As Double, _
                                               FalseNorthing As Double)
        iPrjMethod = ProjectionMethod.Tmerc
        iPrjOrigin.X = OriginLongitude
        iPrjOrigin.Y = OriginLatitude
        iPrjFalse.X = FalseEasting
        iPrjFalse.Y = FalseNorthing
        iPrjScale = OriginScale
        iPrjFName = FullName
        'Verify the ShortName
        If CountCharacter(ShortName, CChar(" ")) > 0 Then
            iPrjSName = DeleteExtraSpaceChar(ShortName)
            iPrjSName = iPrjSName.Replace(" ", "_")
            iPrjSName = iPrjSName.Replace("(", "")
            iPrjSName = iPrjSName.Replace(")", "")
        End If
        If iPrjSName.Count > 12 Then
            iPrjSName = iPrjSName.Replace("_", "")
        End If
        If iPrjSName.Count > 12 Then
            iPrjSName = iPrjSName.Substring(0, 12)
        End If
        'Estimate the nearest UTM zone
        iPrjUTMZone = CInt(Truncate((RadDeg(OriginLongitude) + 180) / 6) + 1)
        iPrjHemi = OriginLatitude > 0
        'Fill the unused variables
        iPrjParallel.X = 0.0
        iPrjParallel.Y = 0.0
    End Sub

    ''' <summary>
    ''' Set the projection as Direct of Mercator with the given parameters
    ''' </summary>
    ''' <param name="OriginLongitude">Longitude at Origin</param>
    ''' <param name="OriginScale">Scale at Origin</param>
    ''' <param name="FalseEasting">Easting at Origin</param>
    ''' <param name="FalseNorthing">Northing at Origin</param>
    ''' <param name="FullName">Projection Name</param>
    ''' <param name="ShortName">Shortened Projection Name (for compatibility reasons)</param>
    ''' <remarks></remarks>
    Public Sub SetProjectionMercator(FullName As String, ShortName As String, OriginLongitude As Double, _
                                     OriginScale As Double, FalseEasting As Double, FalseNorthing As Double)
        iPrjMethod = ProjectionMethod.Merc
        iPrjOrigin.X = OriginLongitude
        iPrjOrigin.Y = 0.0
        iPrjFalse.X = FalseEasting
        iPrjFalse.Y = FalseNorthing
        iPrjScale = OriginScale
        iPrjFName = FullName
        'Verify the ShortName
        If CountCharacter(ShortName, CChar(" ")) > 0 Then
            iPrjSName = DeleteExtraSpaceChar(ShortName)
            iPrjSName = iPrjSName.Replace(" ", "_")
            iPrjSName = iPrjSName.Replace("(", "")
            iPrjSName = iPrjSName.Replace(")", "")
        End If
        If iPrjSName.Count > 12 Then
            iPrjSName = iPrjSName.Replace("_", "")
        End If
        If iPrjSName.Count > 12 Then
            iPrjSName = iPrjSName.Substring(0, 12)
        End If
        'Estimate the nearest UTM zone
        iPrjUTMZone = CInt(Truncate((RadDeg(OriginLongitude) + 180) / 6) + 1)
        iPrjHemi = True
        'Fill the unused variables
        iPrjParallel.X = 0.0
        iPrjParallel.Y = 0.0
    End Sub

    ''' <summary>
    ''' Set the projection as Lambert Conformal Conical (1 parallel) with the given parameters
    ''' </summary>
    ''' <param name="OriginLongitude">Longitude at Origin</param>
    ''' <param name="OriginLatitude">Latitude at Origin</param>
    ''' <param name="OriginScale">Scale at Origin</param>
    ''' <param name="FalseEasting">Easting at Origin</param>
    ''' <param name="FalseNorthing">Northing at Origin</param>
    ''' <param name="FullName">Projection Name</param>
    ''' <param name="ShortName">Shortened Projection Name (for compatibility reasons)</param>
    ''' <remarks></remarks>
    Public Sub SetProjectionLambertConical1(FullName As String, ShortName As String, OriginLongitude As Double, _
                                            OriginLatitude As Double, OriginScale As Double, _
                                            FalseEasting As Double, FalseNorthing As Double)
        iPrjMethod = ProjectionMethod.Lcc1
        iPrjOrigin.X = OriginLongitude
        iPrjOrigin.Y = OriginLatitude
        iPrjParallel.X = OriginLatitude
        iPrjParallel.Y = OriginLatitude
        iPrjFalse.X = FalseEasting
        iPrjFalse.Y = FalseNorthing
        iPrjScale = OriginScale
        iPrjFName = FullName
        'Verify the ShortName
        If CountCharacter(ShortName, CChar(" ")) > 0 Then
            iPrjSName = DeleteExtraSpaceChar(ShortName)
            iPrjSName = iPrjSName.Replace(" ", "_")
            iPrjSName = iPrjSName.Replace("(", "")
            iPrjSName = iPrjSName.Replace(")", "")
        End If
        If iPrjSName.Count > 12 Then
            iPrjSName = iPrjSName.Replace("_", "")
        End If
        If iPrjSName.Count > 12 Then
            iPrjSName = iPrjSName.Substring(0, 12)
        End If
        'Estimate the nearest UTM zone
        iPrjUTMZone = CInt(Truncate((RadDeg(OriginLongitude) + 180) / 6) + 1)
        iPrjHemi = OriginLatitude > 0
    End Sub

    ''' <summary>
    ''' Set the projection as Lambert Conformal Conical (2 parallels) with the given parameters
    ''' </summary>
    ''' <param name="OriginLongitude">Longitude at Origin</param>
    ''' <param name="OriginLatitude">Latitude at Origin</param>
    ''' <param name="FirstParallel">First Tanget Parallel</param>
    ''' <param name="SecondParallel">Second Tangent Parallel</param>
    ''' <param name="FalseEasting">Easting at Origin</param>
    ''' <param name="FalseNorthing">Northing at Origin</param>
    ''' <param name="FullName">Projection Name</param>
    ''' <param name="ShortName">Shortened Projection Name (for compatibility reasons)</param>
    ''' <remarks></remarks>
    Public Sub SetProjectionLambertConical2(FullName As String, ShortName As String, _
                                            OriginLongitude As Double, OriginLatitude As Double, _
                                            FirstParallel As Double, SecondParallel As Double, _
                                            FalseEasting As Double, FalseNorthing As Double)
        iPrjMethod = ProjectionMethod.Lcc1
        iPrjOrigin.X = OriginLongitude
        iPrjOrigin.Y = OriginLatitude
        iPrjParallel.X = FirstParallel
        iPrjParallel.Y = SecondParallel
        iPrjFalse.X = FalseEasting
        iPrjFalse.Y = FalseNorthing
        iPrjFName = FullName
        'Verify the ShortName
        If CountCharacter(ShortName, CChar(" ")) > 0 Then
            iPrjSName = DeleteExtraSpaceChar(ShortName)
            iPrjSName = iPrjSName.Replace(" ", "_")
            iPrjSName = iPrjSName.Replace("(", "")
            iPrjSName = iPrjSName.Replace(")", "")
        End If
        If iPrjSName.Count > 12 Then
            iPrjSName = iPrjSName.Replace("_", "")
        End If
        If iPrjSName.Count > 12 Then
            iPrjSName = iPrjSName.Substring(0, 12)
        End If
        'Estimate the nearest UTM zone
        iPrjUTMZone = CInt(Truncate((RadDeg(OriginLongitude) + 180) / 6) + 1)
        iPrjHemi = OriginLatitude > 0
        iPrjScale = 1.0
    End Sub

    ''' <summary>
    ''' Set Geocentric Translation (3 parameters) conversion
    ''' </summary>
    ''' <param name="FullName">Conversion Name</param>
    ''' <param name="ShortName">Shortened Conversion Name (for compatibility reasons)</param>
    ''' <param name="DeltaX">Delta X</param>
    ''' <param name="DeltaY">Delta Y</param>
    ''' <param name="DeltaZ">Delta Z</param>
    ''' <remarks>All parameters are from local datum to WGS1984 datum.</remarks>
    Public Sub SetDatumConversion(FullName As String, ShortName As String, _
                                  DeltaX As Double, DeltaY As Double, DeltaZ As Double)
        iShiftMethod = TransformationMethod.Geocentric
        iShiftDelta.X = DeltaX
        iShiftDelta.Y = DeltaY
        iShiftDelta.Z = DeltaZ
        iShiftFName = FullName
        iShiftSName = ShortName
        'Verify the ShortName
        If CountCharacter(iShiftSName, CChar(" ")) > 0 Then
            iShiftSName = DeleteExtraSpaceChar(iShiftSName)
            iShiftSName = iShiftSName.Replace(" ", "_")
            iShiftSName = iShiftSName.Replace("(", "")
            iShiftSName = iShiftSName.Replace(")", "")
        End If
        If iShiftSName.Count > 12 Then
            iShiftSName = iShiftSName.Replace("_", "")
        End If
        If iShiftSName.Count > 12 Then
            iShiftSName = iShiftSName.Substring(0, 12)
        End If
        'Zeroed unused values
        iShiftRot.X = 0.0
        iShiftRot.Y = 0.0
        iShiftRot.Z = 0.0
        iShiftPoint.X = 0.0
        iShiftPoint.Y = 0.0
        iShiftPoint.Z = 0.0
        iShiftScale = 0.0
    End Sub

    ''' <summary>
    ''' Set Position Vector or Coordinate Frame (7 parameters) conversion
    ''' </summary>
    ''' <param name="FullName">Conversion Name</param>
    ''' <param name="ShortName">Shortened Conversion Name (for compatibility reasons)</param>
    ''' <param name="DeltaX">Delta X</param>
    ''' <param name="DeltaY">Delta Y</param>
    ''' <param name="DeltaZ">Delta Z</param>
    ''' <param name="RotationX">Rotation X</param>
    ''' <param name="RotationY">Rotation Y</param>
    ''' <param name="RotationZ">Rotation Z</param>
    ''' <param name="ScaleFactor">Scale factor (ppm)</param>
    ''' <param name="IsPositionVectorConvention">True for Position Vector, False for Coordinate Frame</param>
    ''' <remarks>All parameters are from local datum to WGS1984 datum.</remarks>
    Public Sub SetDatumConversion(FullName As String, ShortName As String, _
                              DeltaX As Double, DeltaY As Double, DeltaZ As Double, _
                              RotationX As Double, RotationY As Double, RotationZ As Double, _
                              ScaleFactor As Double, IsPositionVectorConvention As Boolean)
        iShiftMethod = If(IsPositionVectorConvention, TransformationMethod.PositionVector, TransformationMethod.CoordinateFrame)
        iShiftDelta.X = DeltaX
        iShiftDelta.Y = DeltaY
        iShiftDelta.Z = DeltaZ
        iShiftRot.X = RotationX
        iShiftRot.Y = RotationY
        iShiftRot.Z = RotationZ
        iShiftScale = ScaleFactor
        iShiftFName = FullName
        iShiftSName = ShortName
        'Verify the ShortName
        If CountCharacter(iShiftSName, CChar(" ")) > 0 Then
            iShiftSName = DeleteExtraSpaceChar(iShiftSName)
            iShiftSName = iShiftSName.Replace(" ", "_")
            iShiftSName = iShiftSName.Replace("(", "")
            iShiftSName = iShiftSName.Replace(")", "")
        End If
        If iShiftSName.Count > 12 Then
            iShiftSName = iShiftSName.Replace("_", "")
        End If
        If iShiftSName.Count > 12 Then
            iShiftSName = iShiftSName.Substring(0, 12)
        End If
        'Zeroed unused values
        iShiftPoint.X = 0.0
        iShiftPoint.Y = 0.0
        iShiftPoint.Z = 0.0
    End Sub

    ''' <summary>
    ''' Set Molodensky-Badekas (10 parameters) conversion
    ''' </summary>
    ''' <param name="FullName">Conversion Name</param>
    ''' <param name="ShortName">Shortened Conversion Name (for compatibility reasons)</param>
    ''' <param name="DeltaX">Delta X</param>
    ''' <param name="DeltaY">Delta Y</param>
    ''' <param name="DeltaZ">Delta Z</param>
    ''' <param name="RotationX">Rotation X</param>
    ''' <param name="RotationY">Rotation Y</param>
    ''' <param name="RotationZ">Rotation Z</param>
    ''' <param name="ScaleFactor">Scale factor (ppm)</param>
    ''' <param name="PivotX">Coordinate X of pivot point</param>
    ''' <param name="PivotY">Coordinate Y of pivot point</param>
    ''' <param name="PivotZ">Coordinate Z of pivot point</param>
    ''' <remarks>All parameters are from local datum to WGS1984 datum.</remarks>
    Public Sub SetDatumConversion(FullName As String, ShortName As String, _
                          DeltaX As Double, DeltaY As Double, DeltaZ As Double, _
                          RotationX As Double, RotationY As Double, RotationZ As Double, _
                          ScaleFactor As Double, PivotX As Double, PivotY As Double, _
                          PivotZ As Double)
        iShiftMethod = TransformationMethod.MolodenskyBadekas
        iShiftDelta.X = DeltaX
        iShiftDelta.Y = DeltaY
        iShiftDelta.Z = DeltaZ
        iShiftRot.X = RotationX
        iShiftRot.Y = RotationY
        iShiftRot.Z = RotationZ
        iShiftPoint.X = PivotX
        iShiftPoint.Y = PivotY
        iShiftPoint.Z = PivotZ
        iShiftScale = ScaleFactor
        iShiftFName = FullName
        iShiftSName = ShortName
        'Verify the ShortName
        If CountCharacter(iShiftSName, CChar(" ")) > 0 Then
            iShiftSName = DeleteExtraSpaceChar(iShiftSName)
            iShiftSName = iShiftSName.Replace(" ", "_")
            iShiftSName = iShiftSName.Replace("(", "")
            iShiftSName = iShiftSName.Replace(")", "")
        End If
        If iShiftSName.Count > 12 Then
            iShiftSName = iShiftSName.Replace("_", "")
        End If
        If iShiftSName.Count > 12 Then
            iShiftSName = iShiftSName.Substring(0, 12)
        End If
    End Sub

    Public Function SaveToFile(FileName As IO.Stream, FileFormat As CrsFormat) As Boolean
        Dim DblQt, SngQt As Char
        DblQt = Convert.ToChar(34)
        SngQt = Convert.ToChar(39)

        Select Case FileFormat
            'NEW PREDEFINED FORMAT
            Case CrsFormat.Xml
                Try
                    Dim CrsX As New XDocument
                    Dim NdEll As New XElement("Ellipsoid")
                    Dim NdPrj As New XElement("Projection")
                    Dim NdCnv As New XElement("Conversion")
                    Dim NdMain As New XElement("CRS")
                    NdMain.SetAttributeValue("Version", "0.3")
                    'Add the ellipsoid info to the node
                    NdEll.Add(New XElement("Code", iEllID.ToString))
                    NdEll.Add(New XElement("FullName", iEllFName))
                    NdEll.Add(New XElement("ShortName", iEllSName))
                    NdEll.Add(New XElement("SemiMajorAxis", iEllSMAxis))
                    NdEll.Add(New XElement("InverseFlattening", iEllInvFlat))
                    'Add the node to the main node
                    NdMain.Add(NdEll)
                    'Define the projection elements
                    NdPrj.Add(New XElement("FullName", iPrjFName))
                    NdPrj.Add(New XElement("ShortName", iPrjSName))
                    Select Case iPrjMethod
                        Case ProjectionMethod.UTM
                            NdPrj.SetAttributeValue("Method", "UTM")
                            NdPrj.Add(New XElement("Zone", iPrjUTMZone.ToString))
                            NdPrj.Add(New XElement("Hemisphere", If(iPrjHemi, "North", "South")))
                        Case ProjectionMethod.Tmerc
                            NdPrj.SetAttributeValue("Method", "TMerc")
                            NdPrj.Add(New XElement("Origin", RadDeg(iPrjOrigin.X).ToString & "," & RadDeg(iPrjOrigin.Y).ToString))
                            NdPrj.Add(New XElement("ScaleFactor", iPrjScale.ToString))
                            NdPrj.Add(New XElement("FalseCoords", iPrjFalse.X.ToString & "," & iPrjFalse.Y.ToString))
                        Case ProjectionMethod.Merc
                            NdPrj.SetAttributeValue("Method", "Merc")
                            NdPrj.Add(New XElement("Origin", RadDeg(iPrjOrigin.X).ToString & "," & RadDeg(iPrjOrigin.Y).ToString))
                            NdPrj.Add(New XElement("ScaleFactor", iPrjScale.ToString))
                            NdPrj.Add(New XElement("FalseCoords", iPrjFalse.X.ToString & "," & iPrjFalse.Y.ToString))
                        Case ProjectionMethod.Lcc1
                            NdPrj.SetAttributeValue("Method", "LCC1")
                            NdPrj.Add(New XElement("Origin", RadDeg(iPrjOrigin.X).ToString & "," & RadDeg(iPrjOrigin.Y).ToString))
                            NdPrj.Add(New XElement("ScaleFactor", iPrjScale.ToString))
                            NdPrj.Add(New XElement("FalseCoords", iPrjFalse.X.ToString & "," & iPrjFalse.Y.ToString))
                        Case ProjectionMethod.Lcc2
                            NdPrj.SetAttributeValue("Method", "LCC2")
                            NdPrj.Add(New XElement("Origin", RadDeg(iPrjOrigin.X).ToString & "," & RadDeg(iPrjOrigin.Y).ToString))
                            NdPrj.Add(New XElement("Parallels", RadDeg(iPrjParallel.X).ToString & "," & RadDeg(iPrjParallel.Y).ToString))
                            NdPrj.Add(New XElement("FalseCoords", iPrjFalse.X.ToString & "," & iPrjFalse.Y.ToString))
                    End Select
                    'Add the node to the main node
                    NdMain.Add(NdPrj)
                    'Define the Datum elements
                    NdCnv.Add(New XElement("FullName", iShiftFName))
                    NdCnv.Add(New XElement("ShortName", iShiftSName))
                    NdCnv.SetAttributeValue("Method", iShiftMethod.ToString)
                    Dim tmpdelta, tmprot, tmppnt As String
                    tmpdelta = iShiftDelta.X.ToString & "," & iShiftDelta.Y.ToString & "," & iShiftDelta.Z.ToString
                    tmppnt = iShiftPoint.X.ToString & "," & iShiftPoint.Y.ToString & "," & iShiftPoint.Z.ToString
                    tmprot = (RadDeg(iShiftRot.X) * 3600).ToString & "," & (RadDeg(iShiftRot.Y) * 3600).ToString & ","
                    tmprot = tmprot & (RadDeg(iShiftRot.Z) * 3600).ToString
                    Select Case iShiftMethod
                        Case TransformationMethod.Geocentric
                            NdCnv.Add(New XElement("Shift", tmpdelta))
                        Case TransformationMethod.PositionVector, TransformationMethod.CoordinateFrame
                            NdCnv.Add(New XElement("Shift", tmpdelta))
                            NdCnv.Add(New XElement("RotationSec", tmprot))
                            NdCnv.Add(New XElement("ScaleFactor", iShiftScale.ToString))
                        Case TransformationMethod.MolodenskyBadekas
                            NdCnv.Add(New XElement("Shift", tmpdelta))
                            NdCnv.Add(New XElement("RotationSec", tmprot))
                            NdCnv.Add(New XElement("ScaleFactor", iShiftScale.ToString))
                            NdCnv.Add(New XElement("ReferencePoint", tmppnt))
                    End Select
                    'Add the node to the main node
                    NdMain.Add(NdCnv)
                    'Set and save the complete crs file
                    CrsX.Declaration = New XDeclaration("1.0", "UTF-8", "yes")
                    CrsX.Add(NdMain)
                    CrsX.Save(FileName, SaveOptions.None)
                    Return True
                Catch ex As Exception
                    Return False
                End Try

                'Well Known Text pseudo ESRI prj
            Case CrsFormat.Esri
                Try
                    Dim ExportString As String
                    ExportString = "PROJCS[" & DblQt & iPrjFName & DblQt & ","
                    ExportString = ExportString & "GEOGCS[" & DblQt & iShiftFName & DblQt & ","
                    ExportString = ExportString & "DATUM[" & DblQt & iShiftFName & DblQt & ","
                    ExportString = ExportString & "SPHEROID[" & DblQt & iEllFName & DblQt & ","
                    ExportString = ExportString & iEllSMAxis.ToString & "," & iEllInvFlat.ToString
                    If iEllID = 9999 Then
                        ExportString = ExportString & "]"
                    Else
                        ExportString = ExportString & ",AUTHORITY[" & DblQt & "EPSG" & DblQt & ","
                        ExportString = ExportString & DblQt & iEllID.ToString & DblQt & "]]"
                    End If


                    If iShiftMethod <> TransformationMethod.MolodenskyBadekas Then
                        ExportString = ExportString & ",TOWGS84[" & iShiftDelta.X.ToString & ","
                        ExportString = ExportString & iShiftDelta.Y.ToString & ","
                        ExportString = ExportString & iShiftDelta.Z.ToString & ","
                        ExportString = ExportString & (RadDeg(iShiftRot.X) * 3600).ToString & ","
                        ExportString = ExportString & (RadDeg(iShiftRot.Y) * 3600).ToString & ","
                        ExportString = ExportString & (RadDeg(iShiftRot.Z) * 3600).ToString & ","
                        ExportString = ExportString & iShiftScale.ToString & "]],"
                    Else
                        ExportString = ExportString & "],"
                    End If
                    ExportString = ExportString & "PRIMEM[" & DblQt & "Greenwich" & DblQt & ",0.0],"
                    ExportString = ExportString & "UNIT[" & DblQt & "Degree" & DblQt & ",0.0174532925199433]],"
                    'Define the projection method and the relative parameters
                    Select Case iPrjMethod
                        Case ProjectionMethod.UTM, ProjectionMethod.Tmerc
                            ExportString = ExportString & "PROJECTION[" & DblQt & "Transverse_Mercator" & DblQt & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "false_easting" & DblQt
                            ExportString = ExportString & "," & iPrjFalse.X.ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "false_northing" & DblQt
                            ExportString = ExportString & "," & iPrjFalse.Y.ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "central_meridian" & DblQt
                            ExportString = ExportString & "," & RadDeg(iPrjOrigin.X).ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "scale_factor" & DblQt
                            ExportString = ExportString & "," & iPrjScale.ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "latitude_of_origin" & DblQt
                            ExportString = ExportString & "," & RadDeg(iPrjOrigin.Y).ToString & "],"
                        Case ProjectionMethod.Merc
                            ExportString = ExportString & "PROJECTION[" & DblQt & "Mercator" & DblQt & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "false_easting" & DblQt
                            ExportString = ExportString & "," & iPrjFalse.X.ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "false_northing" & DblQt
                            ExportString = ExportString & "," & iPrjFalse.Y.ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "central_meridian" & DblQt
                            ExportString = ExportString & "," & RadDeg(iPrjOrigin.X).ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "scale_factor" & DblQt
                            ExportString = ExportString & "," & iPrjScale.ToString & "],"
                        Case ProjectionMethod.Lcc1
                            ExportString = ExportString & "PROJECTION[" & DblQt & "Lambert_Conformal_Conic" & DblQt & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "false_easting" & DblQt
                            ExportString = ExportString & "," & iPrjFalse.X.ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "false_northing" & DblQt
                            ExportString = ExportString & "," & iPrjFalse.Y.ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "central_meridian" & DblQt
                            ExportString = ExportString & "," & RadDeg(iPrjOrigin.X).ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "standard_parallel_1" & DblQt
                            ExportString = ExportString & "," & RadDeg(iPrjParallel.X).ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "scale_factor" & DblQt
                            ExportString = ExportString & "," & iPrjScale.ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "latitude_of_origin" & DblQt
                            ExportString = ExportString & "," & RadDeg(iPrjOrigin.Y).ToString & "],"
                        Case ProjectionMethod.Lcc2
                            ExportString = ExportString & "PROJECTION[" & DblQt & "Lambert_Conformal_Conic" & DblQt & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "false_easting" & DblQt
                            ExportString = ExportString & "," & iPrjFalse.X.ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "false_northing" & DblQt
                            ExportString = ExportString & "," & iPrjFalse.Y.ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "central_meridian" & DblQt
                            ExportString = ExportString & "," & RadDeg(iPrjOrigin.X).ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "standard_parallel_1" & DblQt
                            ExportString = ExportString & "," & RadDeg(iPrjParallel.X).ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "standard_parallel_2" & DblQt
                            ExportString = ExportString & "," & RadDeg(iPrjParallel.Y).ToString & "],"
                            ExportString = ExportString & "PARAMETER[" & DblQt & "latitude_of_origin" & DblQt
                            ExportString = ExportString & "," & RadDeg(iPrjOrigin.Y).ToString & "],"
                    End Select
                    ExportString = ExportString & "UNIT[" & DblQt & "Meter" & DblQt & ",1.0]]"
                    'Create the export file
                    Using PrjFile As New IO.StreamWriter(FileName)
                        'Write the WKT string in the file
                        PrjFile.Write(ExportString)
                    End Using
                    Return True
                Catch ex As Exception
                    Return False
                End Try
            Case Else
                Return False
        End Select
    End Function

End Class
