'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'
' Filename:     GeoFunction.vb
' Project:      jaktools
' Version:      0.3
' Author:       Simone Giacomoni (jaksg82@yahoo.it)
'               http://www.vivoscuola.it/us/simone.giacomoni/devtools/index.html
'
' Description:  the function to convert the coordinates between various crs
'
' Public Functions:  EN2LL(ByVal Ellipsoid As Ellipsoid, ByVal Projection As ProjectionParam, ByVal Point As Point3D) As Point3D
'                    LL2EN(ByVal Ellipsoid As Ellipsoid, ByVal Projection As ProjectionParam, ByVal Point As Point3D) As Point3D
'                    LL2XYZ(ByVal Ellipsoid As Ellipsoid, ByVal Point As Point3D) As Point3D
'                    XYZ2LL(ByVal Ellipsoid As Ellipsoid, ByVal Point As Point3D) As Point3D
'                    XYZ2XYZ(ByVal Conversion As ConversionParameters, ByVal Point As Point3D, ByVal ToWgs84 As Boolean) As Point3D
'                    EllDistance(ByVal Ellipsoid As Ellipsoid, ByVal Point1 As Point3D, ByVal Point2 As Point3D, Optional ByRef Alpha1 As Double = 0.0, Optional ByRef Alpha2 As Double = 0.0) As Double
'
' Private Functions: UTM(ByVal Ellipsoid As Ellipsoid, ByVal Projection As ProjectionParam, ByVal Point As Point3D, ByVal IsInverse As Boolean) As Point3D
'                    TMerc(ByVal Ellipsoid As Ellipsoid, ByVal Projection As ProjectionParam, ByVal Point As Point3D, ByVal IsInverse As Boolean) As Point3D
'                    Merc(ByVal Ellipsoid As Ellipsoid, ByVal Projection As ProjectionParam, ByVal Point As Point3D, ByVal IsInverse As Boolean) As Point3D
'                    LCC1(ByVal Ellipsoid As Ellipsoid, ByVal Projection As ProjectionParam, ByVal Point As Point3D, ByVal IsInverse As Boolean) As Point3D
'                    LCC2(ByVal Ellipsoid As Ellipsoid, ByVal Projection As ProjectionParam, ByVal Point As Point3D, ByVal IsInverse As Boolean) As Point3D
'                    
' Copyright @2014, Simone Giacomoni
' 
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Imports System.Math
Imports JakMathLib

Public Module GeoFunction

    ''' <summary>
    ''' Convert projected coordinates to the corresponding geographic coordinates
    ''' </summary>
    ''' <param name="CurrentCrs">Reference CRS</param>
    ''' <param name="Point">Projected point to convert</param>
    ''' <returns>Corresponding geographic point</returns>
    ''' <remarks></remarks>
    Public Function EN2LL(ByVal CurrentCrs As GeoDatum, ByVal Point As Point3D) As Point3D
        Dim tmpstring() As String = CurrentCrs.EnumerateProjectionMethods()
        'Select the correct conversion method
        Select Case CurrentCrs.ProjectionType
            Case tmpstring(0)
                EN2LL = UTM(CurrentCrs, Point, True)
            Case tmpstring(1)
                EN2LL = TMerc(CurrentCrs, Point, True)
            Case tmpstring(2)
                EN2LL = Merc(CurrentCrs, Point, True)
            Case tmpstring(3)
                EN2LL = LCC1(CurrentCrs, Point, True)
            Case tmpstring(4)
                EN2LL = LCC2(CurrentCrs, Point, True)
            Case Else
                EN2LL = Point
        End Select

    End Function

    ''' <summary>
    ''' Convert geographic coordinates to the corresponding projected coordinates
    ''' </summary>
    ''' <param name="CurrentCrs">Reference CRS</param>
    ''' <param name="Point">Geographic point to convert</param>
    ''' <returns>Corresponding projected point</returns>
    ''' <remarks></remarks>
    Public Function LL2EN(ByVal CurrentCrs As GeoDatum, ByVal Point As Point3D) As Point3D
        Dim tmpstring() As String = CurrentCrs.EnumerateProjectionMethods()
        'Select the correct conversion method
        Select Case CurrentCrs.ProjectionCode
            Case GeoDatum.ProjectionMethod.UTM
                LL2EN = UTM(CurrentCrs, Point, False)
            Case GeoDatum.ProjectionMethod.Tmerc
                LL2EN = TMerc(CurrentCrs, Point, False)
            Case GeoDatum.ProjectionMethod.Merc
                LL2EN = Merc(CurrentCrs, Point, False)
            Case GeoDatum.ProjectionMethod.Lcc1
                LL2EN = LCC1(CurrentCrs, Point, False)
            Case GeoDatum.ProjectionMethod.Lcc2
                LL2EN = LCC2(CurrentCrs, Point, False)
            Case Else
                LL2EN = Point
        End Select

    End Function

    ''' <summary>
    ''' Convert geographic coordinates to the corresponding geocentric coordinates
    ''' </summary>
    ''' <param name="CurrentCrs">Reference CRS</param>
    ''' <param name="Point">Geographic point to convert</param>
    ''' <returns>Corresponding geocentric point</returns>
    ''' <remarks></remarks>
    Public Function LL2XYZ(ByVal CurrentCrs As GeoDatum, ByVal Point As Point3D) As Point3D
        'Complete the definition of the ellipsoid
        Dim f As Double = CurrentCrs.EllipsoidInverseFlattening ^ -1
        If CurrentCrs.EllipsoidInverseFlattening = 0 Then f = 1
        Dim a As Double = CurrentCrs.EllipsoidSemiMayorAxis
        Dim e As Double = Sqrt((2 * f) - (f ^ 2))
        'Calculate the prime vertical radious of curvature at given latitude
        Dim v As Double = a / Sqrt(1 - e ^ 2 * (Sin(Point.Y) ^ 2))
        'Calculate the results
        Dim TmpResult As New Point3D
        Try
            TmpResult.X = (v + Point.Z) * Cos(Point.Y) * Cos(Point.X)
            TmpResult.Y = (v + Point.Z) * Cos(Point.Y) * Sin(Point.X)
            TmpResult.Z = ((1 - e ^ 2) * v + Point.Z) * Sin(Point.Y)
            Return TmpResult
        Catch ex As Exception
            Return New Point3D
        End Try

    End Function

    ''' <summary>
    ''' Convert geocentric coordinates to the corresponding geographic coordinates
    ''' </summary>
    ''' <param name="CurrentCrs">Reference CRS</param>
    ''' <param name="Point">Geocentric point to convert</param>
    ''' <returns>Corresponding geographic point</returns>
    ''' <remarks></remarks>
    Public Function XYZ2LL(ByVal CurrentCrs As GeoDatum, ByVal Point As Point3D) As Point3D
        'Complete the definition of the ellipsoid
        Dim f As Double = CurrentCrs.EllipsoidInverseFlattening ^ -1
        Dim a As Double = CurrentCrs.EllipsoidSemiMayorAxis
        Dim b As Double = (1 - f) * a
        Dim e As Double = Sqrt((2 * f) - (f ^ 2))
        Dim eps, p, q, v As Double
        Dim TmpResult As New Point3D

        Try
            'Calculate the latitude
            eps = e ^ 2 / (1 - e ^ 2)
            p = (Point.X ^ 2 + Point.Y ^ 2) ^ 0.5
            q = Atan((Point.Z * a) / (p * b))
            TmpResult.Y = Atan((Point.Z + eps * b * Sin(q) ^ 3) / (p - e ^ 2 * a * Cos(q) ^ 3))
            'Calculate the elevetion
            v = a / (1 - e ^ 2 * (Sin(TmpResult.Y) ^ 2)) ^ 0.5
            TmpResult.Z = (p / Cos(TmpResult.Y)) - v
            'Calculate the longitude
            TmpResult.X = Atan(Point.Y / Point.X)
            Return TmpResult

        Catch ex As Exception
            Return New Point3D

        End Try

    End Function

    ''' <summary>
    ''' Apply the shifting parameters between two geocentric points on different datums
    ''' </summary>
    ''' <param name="CurrentCrs">Reference CRS</param>
    ''' <param name="Point">Geocentric point to convert</param>
    ''' <param name="ToWgs84">To WGS84 datum?</param>
    ''' <returns>Converted geocentric point</returns>
    ''' <remarks></remarks>
    Public Function XYZ2XYZ(ByVal CurrentCrs As GeoDatum, ByVal Point As Point3D, ByVal ToWgs84 As Boolean) As Point3D
        Dim dX, dY, dZ, Rx, Ry, Rz, M As Double
        Dim Xs, Ys, Zs, Xr, Yr, Zr, Xp, Yp, Zp As Double
        Dim TmpResult As New Point3D

        Xs = Point.X
        Ys = Point.Y
        Zs = Point.Z

        Try
            If ToWgs84 Then
                M = 1 + (CurrentCrs.TransformationScaleFactorPPM * 10 ^ -6)
                dX = CurrentCrs.TransformationDeltas.X
                dY = CurrentCrs.TransformationDeltas.Y
                dZ = CurrentCrs.TransformationDeltas.Z
                Rx = CurrentCrs.TransformationRotations.X
                Ry = CurrentCrs.TransformationRotations.Y
                Rz = CurrentCrs.TransformationRotations.Z
                Xp = CurrentCrs.TransformationRotationPoint.X
                Yp = CurrentCrs.TransformationRotationPoint.Y
                Zp = CurrentCrs.TransformationRotationPoint.Z
            Else
                M = 1 + (-CurrentCrs.TransformationScaleFactorPPM * 10 ^ -6)
                dX = -CurrentCrs.TransformationDeltas.X
                dY = -CurrentCrs.TransformationDeltas.Y
                dZ = -CurrentCrs.TransformationDeltas.Z
                Rx = -CurrentCrs.TransformationRotations.X
                Ry = -CurrentCrs.TransformationRotations.Y
                Rz = -CurrentCrs.TransformationRotations.Z
                Xp = CurrentCrs.TransformationRotationPoint.X
                Yp = CurrentCrs.TransformationRotationPoint.Y
                Zp = CurrentCrs.TransformationRotationPoint.Z
            End If

            'Select Case UCase(Left(Conversion.Method, 1))
            Select Case CurrentCrs.TransformationType.Substring(0, 1).ToUpper
                Case "G"
                    TmpResult.X = Xs + dX
                    TmpResult.Y = Ys + dY
                    TmpResult.Z = Zs + dZ

                Case "P"
                    TmpResult.X = M * ((1 * Xs) + (-Rz * Ys) + (Ry * Zs)) + dX
                    TmpResult.Y = M * ((Rz * Xs) + (1 * Ys) + (-Rx * Zs)) + dY
                    TmpResult.Z = M * ((-Ry * Xs) + (Rx * Ys) + (1 * Zs)) + dZ

                Case "C"
                    TmpResult.X = M * ((1 * Xs) + (Rz * Ys) + (-Ry * Zs)) + dX
                    TmpResult.Y = M * ((-Rz * Xs) + (1 * Ys) + (Rx * Zs)) + dY
                    TmpResult.Z = M * ((Ry * Xs) + (-Rx * Ys) + (1 * Zs)) + dZ

                Case "M"
                    Xr = Xs - Xp
                    Yr = Ys - Yp
                    Zr = Zs - Zp
                    TmpResult.X = M * ((1 * Xr) + (Rz * Yr) + (-Ry * Zr)) + dX + Xp
                    TmpResult.Y = M * ((-Rz * Xr) + (1 * Yr) + (Rx * Zr)) + dY + Yp
                    TmpResult.Z = M * ((Ry * Xr) + (-Rx * Yr) + (1 * Zr)) + dZ + Zp

            End Select
            Return TmpResult

        Catch ex As Exception
            Return New Point3D

        End Try

    End Function

    ''' <summary>
    ''' Calculate the ellipsoidical distance between two geographic point using the Vincenty's formula
    ''' </summary>
    ''' <param name="CurrentCrs">Reference CRS</param>
    ''' <param name="Point1">Start point</param>
    ''' <param name="Point2">End point</param>
    ''' <param name="Alpha1">Bearing from first to second point</param>
    ''' <param name="Alpha2">Bearing from second to first point</param>
    ''' <returns>Ellipsoidical distance</returns>
    ''' <remarks></remarks>
    Public Function EllDistance(ByVal CurrentCrs As GeoDatum, ByVal Point1 As Point3D, ByVal Point2 As Point3D, _
                                Optional ByRef Alpha1 As Double = 0.0, Optional ByRef Alpha2 As Double = 0.0) As Double
        Dim a, b, f, lat1, lat2, U1, U2, L, L1, L2, Alpha, S, sinSigma, cosSigma, Sigma, lambda As Double
        Dim sinAlpha, cos2Alpha, cos2sigma, C, lambdaPrev, Usquare, Aa, Bb, k1, DeltaSigma As Double
        Dim maxIter As Integer = 100000
        Dim Iter As Integer = 0

        If Point1.X = Point2.X And Point1.Y = Point2.Y Then
            Return 0
        Else
            a = CurrentCrs.EllipsoidSemiMayorAxis
            f = 1 / CurrentCrs.EllipsoidInverseFlattening
            b = (1 - f) * a
            lat1 = Point1.Y
            lat2 = Point2.Y
            U1 = Atan((1 - f) * Tan(lat1))
            U2 = Atan((1 - f) * Tan(lat2))
            L1 = Point1.X
            L2 = Point2.X
            L = L2 - L1
            lambda = L
            If lat1 = 0 And lat2 = 0 Then
                'Geodesic runs along equator
                S = a * lambda
                If L1 > L2 Then
                    Alpha1 = DegRad(270)
                    Alpha2 = Alpha1
                Else
                    Alpha1 = DegRad(90)
                    Alpha2 = Alpha1
                End If
            Else
                Do
                    sinSigma = Sqrt((Cos(U2) * Sin(lambda)) ^ 2 + (Cos(U1) * Sin(U2) - Sin(U1) * Cos(U2) * Cos(lambda)) ^ 2)
                    cosSigma = Sin(U1) * Sin(U2) + Cos(U1) * Cos(U2) * Cos(lambda)
                    Sigma = Atan2(sinSigma, cosSigma)
                    sinAlpha = (Cos(U1) * Cos(U2) * Sin(lambda)) / Sin(Sigma)
                    cos2Alpha = 1 - sinAlpha ^ 2
                    cos2sigma = Cos(Sigma) - ((2 * Sin(U1) * Sin(U2)) / cos2Alpha)
                    C = (f / 16) * cos2Alpha * (4 + f * (4 - 3 * cos2Alpha))
                    lambdaPrev = lambda
                    lambda = L + (1 - C) * f * sinAlpha * (Sigma + C * Sin(Sigma) * (cos2sigma + C * Cos(Sigma) * (-1 + 2 * cos2sigma ^ 2)))
                    Iter = Iter + 1
                Loop Until Abs(lambda - lambdaPrev) < 0.0000000001 Or Iter = maxIter
                Alpha = Asin(sinAlpha)
                Usquare = Cos(Alpha) ^ 2 * ((a ^ 2 - b ^ 2) / b ^ 2)
                k1 = (Sqrt(1 + Usquare) - 1) / (Sqrt(1 + Usquare) + 1)
                Aa = (1 + 0.25 * k1 ^ 2) / (1 - k1)
                Bb = k1 * (1 - 3 / 8 * k1 ^ 2)
                DeltaSigma = Bb * sinSigma * (cos2sigma + 0.25 * Bb * (cosSigma * (-1 + 2 * cos2sigma ^ 2) - (1 / 6) * Bb * cos2sigma * (-3 + 4 * sinSigma ^ 2) * (-3 + 4 * cosSigma ^ 2)))
                S = b * Aa * (Sigma - DeltaSigma)
                'Alpha1 = Atan((Cos(U2) * Sin(lambda)) / (Cos(U1) * Sin(U2) - Sin(U1) * Cos(U2) * Cos(lambda)))
                'Alpha2 = Atan((Cos(U1) * Sin(lambda)) / (-Sin(U1) * Cos(U2) + Cos(U1) * Sin(U2) * Cos(lambda)))
                Dim Alp1 As Double = Atan2(Cos(U2) * Sin(lambda), (Cos(U1) * Sin(U2) - Sin(U1) * Cos(U2) * Cos(lambda)))
                Dim Alp2 As Double = Atan2(Cos(U1) * Sin(lambda), (-Sin(U1) * Cos(U2) + Cos(U1) * Sin(U2) * Cos(lambda)))
                Alpha1 = Alp1
                Alpha2 = Alp2
                If Double.IsNaN(S) Then S = 0
            End If
            Return S
        End If
    End Function

    ''' <summary>
    ''' Handling the Universal Transverse of Mercator projection method
    ''' </summary>
    ''' <param name="CurrentCrs">Reference CRS</param>
    ''' <param name="Point">Point to convert</param>
    ''' <param name="IsInverse">True to convert from projected to geographic coordinates, False for the inverse</param>
    ''' <returns>Converted point</returns>
    ''' <remarks></remarks>
    Private Function UTM(ByVal CurrentCrs As GeoDatum, ByVal Point As Point3D, ByVal IsInverse As Boolean) As Point3D
        Dim f As Double
        If CurrentCrs.EllipsoidInverseFlattening = 0 Then
            f = 1
        Else
            f = 1 / CurrentCrs.EllipsoidInverseFlattening
        End If
        Dim a As Double = CurrentCrs.EllipsoidSemiMayorAxis
        Dim b As Double = (1 - f) * a
        Dim e As Double = Sqrt(2 * f - f ^ 2)
        Dim secEcc As Double = Sqrt(e ^ 2 / (1 - e ^ 2))
        Dim lat0, lon0, E0, N0, latf, Ef, Nf, lonf, Mo, k0 As Double
        'Projection costant
        Dim h1, h2, h3, h4, h1i, h2i, h3i, h4i As Double
        Dim BI, n, Qo, Q, Qi, Qii, Beta, BetaO, Betai As Double
        Dim Eta0, Eta1, Eta2, Eta3, Eta4, Eta As Double
        Dim Eta0i, Eta1i, Eta2i, Eta3i, Eta4i, Etai As Double
        Dim xiO, xiO0, xiO1, xiO2, xiO3, xiO4 As Double
        Dim xi0, xi1, xi2, xi3, xi4, xi As Double
        Dim xi0i, xi1i, xi2i, xi3i, xi4i, xii As Double

        Dim TmpResult As New Point3D

        Try
            'Calculate the ellipsoidal constants
            k0 = 0.9996
            lon0 = Point.X
            lat0 = Point.Y
            E0 = Point.X
            N0 = Point.Y
            lonf = DegRad((CurrentCrs.ProjectionUtmZone - 1) * 6 - 180 + 3)
            Ef = 500000
            If CurrentCrs.ProjectionIsNorthHemisphere Then
                'North
                latf = 0
                Nf = 0
            Else
                latf = 0
                Nf = 10000000
            End If

            n = f / (2 - f)
            BI = (a / (1 + n)) * (1 + n ^ 2 / 4 + n ^ 4 / 64)
            h1 = n / 2 - (2 / 3) * n ^ 2 + (5 / 16) * n ^ 3 + (41 / 180) * n ^ 4
            h2 = (13 / 48) * n ^ 2 - (3 / 5) * n ^ 3 + (557 / 1440) * n ^ 4
            h3 = (61 / 240) * n ^ 3 - (103 / 140) * n ^ 4
            h4 = (49561 / 161280) * n ^ 4
            h1i = n / 2 - (2 / 3) * n ^ 2 + (37 / 96) * n ^ 3 - (1 / 360) * n ^ 4
            h2i = (1 / 48) * n ^ 2 + (1 / 15) * n ^ 3 - (437 / 1440) * n ^ 4
            h3i = (17 / 480) * n ^ 3 - (37 / 840) * n ^ 4
            h4i = (4397 / 161280) * n ^ 4

            If latf = 0 Then
                Mo = 0
            ElseIf latf = (PI / 2) Then
                Mo = BI * (PI / 2)
            ElseIf latf = -(PI / 2) Then
                Mo = BI * -(PI / 2)
            Else
                Qo = ASinH(Tan(latf)) - (e * ATanH(e * Sin(latf)))
                BetaO = Atan(Sinh(Qo))
                xiO0 = Asin(Sin(BetaO))
                xiO1 = h1 * Sin(2 * xiO0)
                xiO2 = h2 * Sin(4 * xiO0)
                xiO3 = h3 * Sin(6 * xiO0)
                xiO4 = h4 * Sin(8 * xiO0)
                xiO = xiO0 + xiO1 + xiO2 + xiO3 + xiO4
                Mo = BI * xiO
            End If

            If IsInverse Then
                'From EN to LL
                Etai = (E0 - Ef) / (BI * k0)
                xii = ((N0 - Nf) + k0 * Mo) / (BI * k0)
                xi1i = h1i * Sin(2 * xii) * Cosh(2 * Etai)
                xi2i = h2i * Sin(4 * xii) * Cosh(4 * Etai)
                xi3i = h3i * Sin(6 * xii) * Cosh(6 * Etai)
                xi4i = h4i * Sin(8 * xii) * Cosh(8 * Etai)
                xi0i = xii - (xi1i + xi2i + xi3i + xi4i)
                Eta1i = h1i * Cos(2 * xii) * Sinh(2 * Etai)
                Eta2i = h2i * Cos(4 * xii) * Sinh(4 * Etai)
                Eta3i = h3i * Cos(6 * xii) * Sinh(6 * Etai)
                Eta4i = h4i * Cos(8 * xii) * Sinh(8 * Etai)
                Eta0i = Etai - (Eta1i + Eta2i + Eta3i + Eta4i)
                Betai = Asin(Sin(xi0i) / Cosh(Eta0i))
                Qi = ASinH(Tan(Betai))
                Qii = Qi + (e * ATanH(e * Tanh(Qi)))
                For i = 0 To 100
                    Qii = Qi + (e * ATanH(e * Tanh(Qii)))
                Next
                TmpResult.X = lonf + Asin(Tanh(Eta0i) / Cos(Betai))
                TmpResult.Y = Atan(Sinh(Qii))
                TmpResult.Z = Point.Z

            Else
                'From LL to EN
                Q = ASinH(Tan(lat0)) - (e * ATanH(e * Sin(lat0)))
                Beta = Atan(Sinh(Q))
                Eta0 = ATanH(Cos(Beta) * Sin(lon0 - lonf))
                xi0 = Asin(Sin(Beta) * Cosh(Eta0))
                xi1 = h1 * Sin(2 * xi0) * Cosh(2 * Eta0)
                xi2 = h2 * Sin(4 * xi0) * Cosh(4 * Eta0)
                xi3 = h3 * Sin(6 * xi0) * Cosh(6 * Eta0)
                xi4 = h4 * Sin(8 * xi0) * Cosh(8 * Eta0)
                xi = xi0 + xi1 + xi2 + xi3 + xi4
                Eta1 = h1 * Cos(2 * xi0) * Sinh(2 * Eta0)
                Eta2 = h2 * Cos(4 * xi0) * Sinh(4 * Eta0)
                Eta3 = h3 * Cos(6 * xi0) * Sinh(6 * Eta0)
                Eta4 = h4 * Cos(8 * xi0) * Sinh(8 * Eta0)
                Eta = Eta0 + Eta1 + Eta2 + Eta3 + Eta4
                TmpResult.X = Ef + k0 * BI * Eta
                TmpResult.Y = Nf + k0 * (BI * xi - Mo)
                TmpResult.Z = Point.Z
            End If
            'return the results
            Return TmpResult

        Catch ex As Exception
            Return New Point3D

        End Try

    End Function

    ''' <summary>
    ''' Handling the Transverse of Mercator projection method
    ''' </summary>
    ''' <param name="CurrentCrs">Reference CRS</param>
    ''' <param name="Point">Point to convert</param>
    ''' <param name="IsInverse">True to convert from projected to geographic coordinates, False for the inverse</param>
    ''' <returns>Converted point</returns>
    ''' <remarks></remarks>
    Private Function TMerc(ByVal CurrentCrs As GeoDatum, ByVal Point As Point3D, ByVal IsInverse As Boolean) As Point3D
        Dim f As Double
        If CurrentCrs.EllipsoidInverseFlattening = 0 Then
            f = 1
        Else
            f = 1 / CurrentCrs.EllipsoidInverseFlattening
        End If
        Dim a As Double = CurrentCrs.EllipsoidSemiMayorAxis
        Dim b As Double = (1 - f) * a
        Dim e As Double = Sqrt(2 * f - f ^ 2)
        Dim lat0, lon0, E0, N0, latf, Ef, Nf, lonf, Mo, k0 As Double
        'Projection costant
        Dim h1, h2, h3, h4, h1i, h2i, h3i, h4i As Double
        Dim BI, n, Qo, Q, Qi, Qii, Beta, BetaO, Betai As Double
        Dim Eta0, Eta1, Eta2, Eta3, Eta4, Eta As Double
        Dim Eta0i, Eta1i, Eta2i, Eta3i, Eta4i, Etai As Double
        Dim xiO, xiO0, xiO1, xiO2, xiO3, xiO4 As Double
        Dim xi0, xi1, xi2, xi3, xi4, xi As Double
        Dim xi0i, xi1i, xi2i, xi3i, xi4i, xii As Double

        Dim TmpResult As New Point3D

        Try
            lon0 = Point.X
            lat0 = Point.Y
            E0 = Point.X
            N0 = Point.Y
            lonf = CurrentCrs.ProjectionOriginLongitude
            latf = CurrentCrs.ProjectionOriginLatitude
            Ef = CurrentCrs.ProjectionFalseEasting
            Nf = CurrentCrs.ProjectionFalseNorthing
            k0 = CurrentCrs.ProjectionScaleAtOrigin

            n = f / (2 - f)
            BI = (a / (1 + n)) * (1 + n ^ 2 / 4 + n ^ 4 / 64)
            h1 = n / 2 - (2 / 3) * n ^ 2 + (5 / 16) * n ^ 3 + (41 / 180) * n ^ 4
            h2 = (13 / 48) * n ^ 2 - (3 / 5) * n ^ 3 + (557 / 1440) * n ^ 4
            h3 = (61 / 240) * n ^ 3 - (103 / 140) * n ^ 4
            h4 = (49561 / 161280) * n ^ 4
            h1i = n / 2 - (2 / 3) * n ^ 2 + (37 / 96) * n ^ 3 - (1 / 360) * n ^ 4
            h2i = (1 / 48) * n ^ 2 + (1 / 15) * n ^ 3 - (437 / 1440) * n ^ 4
            h3i = (17 / 480) * n ^ 3 - (37 / 840) * n ^ 4
            h4i = (4397 / 161280) * n ^ 4

            If latf = 0 Then
                Mo = 0
            ElseIf latf = (PI / 2) Then
                Mo = BI * (PI / 2)
            ElseIf latf = -(PI / 2) Then
                Mo = BI * -(PI / 2)
            Else
                Qo = ASinH(Tan(latf)) - (e * ATanH(e * Sin(latf)))
                BetaO = Atan(Sinh(Qo))
                xiO0 = Asin(Sin(BetaO))
                xiO1 = h1 * Sin(2 * xiO0)
                xiO2 = h2 * Sin(4 * xiO0)
                xiO3 = h3 * Sin(6 * xiO0)
                xiO4 = h4 * Sin(8 * xiO0)
                xiO = xiO0 + xiO1 + xiO2 + xiO3 + xiO4
                Mo = BI * xiO
            End If

            If IsInverse Then
                'From EN to LL
                Etai = (E0 - Ef) / (BI * k0)
                xii = ((N0 - Nf) + k0 * Mo) / (BI * k0)
                xi1i = h1i * Sin(2 * xii) * Cosh(2 * Etai)
                xi2i = h2i * Sin(4 * xii) * Cosh(4 * Etai)
                xi3i = h3i * Sin(6 * xii) * Cosh(6 * Etai)
                xi4i = h4i * Sin(8 * xii) * Cosh(8 * Etai)
                xi0i = xii - (xi1i + xi2i + xi3i + xi4i)
                Eta1i = h1i * Cos(2 * xii) * Sinh(2 * Etai)
                Eta2i = h2i * Cos(4 * xii) * Sinh(4 * Etai)
                Eta3i = h3i * Cos(6 * xii) * Sinh(6 * Etai)
                Eta4i = h4i * Cos(8 * xii) * Sinh(8 * Etai)
                Eta0i = Etai - (Eta1i + Eta2i + Eta3i + Eta4i)
                Betai = Asin(Sin(xi0i) / Cosh(Eta0i))
                Qi = ASinH(Tan(Betai))
                Qii = Qi + (e * ATanH(e * Tanh(Qi)))
                For i = 0 To 100
                    Qii = Qi + (e * ATanH(e * Tanh(Qii)))
                Next
                TmpResult.X = lonf + Asin(Tanh(Eta0i) / Cos(Betai))
                TmpResult.Y = Atan(Sinh(Qii))
                TmpResult.Z = Point.Z

            Else
                'From LL to EN
                Q = ASinH(Tan(lat0)) - (e * ATanH(e * Sin(lat0)))
                Beta = Atan(Sinh(Q))
                Eta0 = ATanH(Cos(Beta) * Sin(lon0 - lonf))
                xi0 = Asin(Sin(Beta) * Cosh(Eta0))
                xi1 = h1 * Sin(2 * xi0) * Cosh(2 * Eta0)
                xi2 = h2 * Sin(4 * xi0) * Cosh(4 * Eta0)
                xi3 = h3 * Sin(6 * xi0) * Cosh(6 * Eta0)
                xi4 = h4 * Sin(8 * xi0) * Cosh(8 * Eta0)
                xi = xi0 + xi1 + xi2 + xi3 + xi4
                Eta1 = h1 * Cos(2 * xi0) * Sinh(2 * Eta0)
                Eta2 = h2 * Cos(4 * xi0) * Sinh(4 * Eta0)
                Eta3 = h3 * Cos(6 * xi0) * Sinh(6 * Eta0)
                Eta4 = h4 * Cos(8 * xi0) * Sinh(8 * Eta0)
                Eta = Eta0 + Eta1 + Eta2 + Eta3 + Eta4
                TmpResult.X = Ef + k0 * BI * Eta
                TmpResult.Y = Nf + k0 * (BI * xi - Mo)
                TmpResult.Z = Point.Z
            End If
            Return TmpResult

        Catch ex As Exception
            Return New Point3D

        End Try

    End Function

    ''' <summary>
    ''' Handling the Mercator (EPSG Variant A) projection method
    ''' </summary>
    ''' <param name="CurrentCrs">Reference CRS</param>
    ''' <param name="Point">Point to convert</param>
    ''' <param name="IsInverse">True to convert from projected to geographic coordinates, False for the inverse</param>
    ''' <returns>Converted point</returns>
    ''' <remarks></remarks>
    Private Function Merc(ByVal CurrentCrs As GeoDatum, ByVal Point As Point3D, ByVal IsInverse As Boolean) As Point3D
        Dim f, a, b, e As Double
        Dim lat0, lon0, E0, N0, latf, Ef, Nf, lonf, k0 As Double
        Dim X, t, A1, A2, A3, A4 As Double
        Dim TmpResult As New Point3D

        Try
            'Ellipsoid costants
            f = 1 / CurrentCrs.EllipsoidInverseFlattening
            a = CurrentCrs.EllipsoidSemiMayorAxis
            b = (1 - f) * a
            e = Sqrt(2 * f - f ^ 2)
            'Projection info
            lon0 = Point.X
            lat0 = Point.Y
            E0 = Point.X
            N0 = Point.Y
            lonf = CurrentCrs.ProjectionOriginLongitude
            latf = 0
            Ef = CurrentCrs.ProjectionFalseEasting
            Nf = CurrentCrs.ProjectionFalseNorthing
            k0 = CurrentCrs.ProjectionScaleAtOrigin
            'Projection costants
            A1 = (e ^ 2 / 2) + (5 * e ^ 4 / 24) + (e ^ 6 / 12) + (13 * e ^ 8 / 360)
            A2 = (7 * e ^ 4 / 48) + (29 * e ^ 6 / 240) + (811 * e ^ 8 / 11520)
            A3 = (7 * e ^ 6 / 120) + (81 * e ^ 8 / 1120)
            A4 = (4279 * e ^ 8 / 161280)
            If IsInverse Then
                'From EN to LL
                t = Math.E ^ ((Nf - N0) / (a * k0))
                X = PI / 2 - 2 * Atan(t)
                TmpResult.Y = X + A1 * Sin(2 * X) + A2 * Sin(4 * X) + A3 * Sin(6 * X) + A4 * Sin(8 * X)
                TmpResult.X = ((E0 - Ef) / a * k0) + lonf
                TmpResult.Z = Point.Z
            Else
                'From LL to EN
                TmpResult.X = Ef + a * k0 * (lon0 - lonf)
                TmpResult.Y = Nf + a * k0 * Log(Tan(PI / 4 + lat0 / 2) * ((1 - e * Sin(lat0)) / (1 + e * Sin(lat0))) * (e / 2))
                TmpResult.Z = Point.Z
            End If
            Return TmpResult
        Catch ex As Exception
            Return New Point3D

        End Try

    End Function

    ''' <summary>
    ''' Handling the Lambert Conformal Conic (1 standard parallel) projection method
    ''' </summary>
    ''' <param name="CurrentCrs">Reference CRS</param>
    ''' <param name="Point">Point to convert</param>
    ''' <param name="IsInverse">True to convert from projected to geographic coordinates, False for the inverse</param>
    ''' <returns>Converted point</returns>
    ''' <remarks></remarks>
    Private Function LCC1(ByVal CurrentCrs As GeoDatum, ByVal Point As Point3D, ByVal IsInverse As Boolean) As Point3D
        Dim f As Double
        If CurrentCrs.EllipsoidInverseFlattening = 0 Then
            f = 1
        Else
            f = 1 / CurrentCrs.EllipsoidInverseFlattening
        End If
        Dim a As Double = CurrentCrs.EllipsoidSemiMayorAxis
        Dim b As Double = (1 - f) * a
        Dim e As Double = Sqrt(2 * f - f ^ 2)
        Dim secEcc As Double = Sqrt(e ^ 2 / (1 - e ^ 2))
        Dim lat0, latf, Ef, Nf, lon0, lonf, k0 As Double
        Dim theta, thetai, Mo, Tf, T0, Ff, Rf, R0, Ti, Ri, n As Double
        Dim TmpResult As New Point3D

        Try
            'Projection costants
            lon0 = Point.X
            lat0 = Point.Y
            latf = CurrentCrs.ProjectionOriginLatitude
            lonf = CurrentCrs.ProjectionOriginLongitude
            Ef = CurrentCrs.ProjectionFalseEasting
            Nf = CurrentCrs.ProjectionFalseNorthing
            k0 = CurrentCrs.ProjectionScaleAtOrigin

            If IsInverse Then
                'From EN to LL
                Mo = Cos(latf) / Sqrt(1 - e ^ 2 * Sin(latf) ^ 2)
                Tf = Tan(PI / 4 - latf / 2) / ((1 - e * Sin(latf)) / (1 + e * Sin(latf))) ^ (e / 2)
                n = Sin(latf)
                Ff = Mo / (n * Tf ^ n)
                Rf = a * f * Tf ^ n
                If n > 0 Then
                    Ri = Abs(((lon0 - Ef) ^ 2 + (Rf - (lat0 - Nf)) ^ 2) ^ 0.5)
                Else
                    Ri = -Abs(((lon0 - Ef) ^ 2 + (Rf - (lat0 - Nf)) ^ 2) ^ 0.5)
                End If
                Ti = (Ri / (a * k0 * Ff)) ^ (1 / n)
                Dim tmplat As Double = PI / 2 - 2 * Atan(Ti)
                For i = 0 To 100
                    tmplat = PI / 2 - 2 * Atan(Ti * ((1 - e * Sin(tmplat)) / (1 + e * Sin(tmplat))) ^ (e / 2))
                Next
                thetai = Atan((lon0 - Ef) / (Rf - (lat0 - Nf)))
                TmpResult.X = thetai / n + lonf
                TmpResult.Y = tmplat
                TmpResult.Z = Point.Z
            Else
                'From LL to EN
                Mo = Cos(latf) / Sqrt(1 - e ^ 2 * Sin(latf) ^ 2)
                T0 = Tan(PI / 4 - lat0 / 2) / ((1 - e * Sin(lat0)) / (1 + e * Sin(lat0))) ^ (e / 2)
                Tf = Tan(PI / 4 - latf / 2) / ((1 - e * Sin(latf)) / (1 + e * Sin(latf))) ^ (e / 2)
                n = Sin(latf)
                Ff = Mo / (n * Tf ^ n)
                Rf = a * Ff * Tf ^ n * k0
                R0 = a * Ff * T0 ^ n * k0
                theta = n * (lon0 - lonf)
                TmpResult.X = Ef + R0 * Sin(theta)
                TmpResult.Y = Nf + Rf - R0 * Cos(theta)
                TmpResult.Z = Point.Z
            End If
            Return TmpResult

        Catch ex As Exception
            Return New Point3D

        End Try

    End Function

    ''' <summary>
    ''' Handling the Lambert Conformal Conic (2 standard parallels) projection method
    ''' </summary>
    ''' <param name="CurrentCrs">Reference CRS</param>
    ''' <param name="Point">Point to convert</param>
    ''' <param name="IsInverse">True to convert from projected to geographic coordinates, False for the inverse</param>
    ''' <returns>Converted point</returns>
    ''' <remarks></remarks>
    Private Function LCC2(ByVal CurrentCrs As GeoDatum, ByVal Point As Point3D, ByVal IsInverse As Boolean) As Point3D
        Dim flat As Double
        If CurrentCrs.EllipsoidInverseFlattening = 0 Then
            flat = 1
        Else
            flat = 1 / CurrentCrs.EllipsoidInverseFlattening
        End If
        Dim a As Double = CurrentCrs.EllipsoidSemiMayorAxis
        Dim b As Double = (1 - flat) * a
        Dim e As Double = Sqrt(2 * flat - flat ^ 2)
        Dim secEcc As Double = Sqrt(e ^ 2 / (1 - e ^ 2))
        Dim lat0, lat1, lat2, latf, Ef, Nf, lon0, lonf As Double
        Dim theta, Rf, m1, m2, t0, t1, t2, tf, n, F, r, Ri, tmplat, thetainv As Double
        Dim TmpResult As New Point3D

        Try
            'Projection costants
            lon0 = Point.X
            lat0 = Point.Y
            lat1 = CurrentCrs.LambertFirstParallel
            lat2 = CurrentCrs.LambertSecondParallel
            latf = CurrentCrs.ProjectionOriginLatitude
            lonf = CurrentCrs.ProjectionOriginLongitude
            Ef = CurrentCrs.ProjectionFalseEasting
            Nf = CurrentCrs.ProjectionFalseNorthing

            If IsInverse Then
                'From EN to LL
                m1 = Cos(lat1) / Sqrt(1 - e ^ 2 * Sin(lat1) ^ 2)
                m2 = Cos(lat2) / Sqrt(1 - e ^ 2 * Sin(lat2) ^ 2)
                t1 = Tan(PI / 4 - lat1 / 2) / ((1 - e * Sin(lat1)) / (1 + e * Sin(lat1))) ^ (e / 2)
                t2 = Tan(PI / 4 - lat2 / 2) / ((1 - e * Sin(lat2)) / (1 + e * Sin(lat2))) ^ (e / 2)
                tf = Tan(PI / 4 - latf / 2) / ((1 - e * Sin(latf)) / (1 + e * Sin(latf))) ^ (e / 2)
                n = (Log(m1) - Log(m2)) / (Log(t1) - Log(t2))
                F = m1 / (n * t1 ^ n)
                Rf = a * F * tf ^ n
                If n > 0 Then
                    Ri = Abs(((lon0 - Ef) ^ 2 + (Rf - (lat0 - Nf)) ^ 2) ^ 0.5)
                Else
                    Ri = -Abs(((lon0 - Ef) ^ 2 + (Rf - (lat0 - Nf)) ^ 2) ^ 0.5)
                End If
                t0 = (Ri / (a * F)) ^ (1 / n)
                tmplat = PI / 2 - 2 * Atan(t0)
                For i = 0 To 100
                    tmplat = PI / 2 - 2 * Atan(t0 * ((1 - e * Sin(tmplat)) / (1 + e * Sin(tmplat))) ^ (e / 2))
                Next
                thetainv = Atan((lon0 - Ef) / (Rf - (lat0 - Nf)))
                'Return the results
                TmpResult.X = thetainv / n + lonf
                TmpResult.Y = tmplat
                TmpResult.Z = Point.Z
            Else
                'From LL to EN
                m1 = Cos(lat1) / Sqrt(1 - e ^ 2 * Sin(lat1) ^ 2)
                m2 = Cos(lat2) / Sqrt(1 - e ^ 2 * Sin(lat2) ^ 2)
                t0 = Tan(PI / 4 - lat0 / 2) / ((1 - e * Sin(lat0)) / (1 + e * Sin(lat0))) ^ (e / 2)
                t1 = Tan(PI / 4 - lat1 / 2) / ((1 - e * Sin(lat1)) / (1 + e * Sin(lat1))) ^ (e / 2)
                t2 = Tan(PI / 4 - lat2 / 2) / ((1 - e * Sin(lat2)) / (1 + e * Sin(lat2))) ^ (e / 2)
                tf = Tan(PI / 4 - latf / 2) / ((1 - e * Sin(latf)) / (1 + e * Sin(latf))) ^ (e / 2)
                n = (Log(m1) - Log(m2)) / (Log(t1) - Log(t2))
                F = m1 / (n * t1 ^ n)
                r = a * F * t0 ^ n
                theta = n * (lon0 - lonf)
                Rf = a * F * tf ^ n
                'Return the results
                TmpResult.X = Ef + r * Sin(theta)
                TmpResult.Y = Nf + Rf - r * Cos(theta)
                TmpResult.Z = Point.Z
            End If
            Return TmpResult

        Catch ex As Exception
            Return New Point3D

        End Try

    End Function


End Module
