'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'
' Filename:     JakMathLib.vb
' Project:      
' Version:      
' Author:       Simone Giacomoni (jaksg82@yahoo.it)
'               http://www.vivoscuola.it/us/simone.giacomoni/devtools/index.html
'
' Copyright @2014, Simone Giacomoni
' 
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Imports System.Math

Public Module JakMathLib
    ''' <summary>
    ''' Calculate the heading of the line to join the two given point
    ''' </summary>
    ''' <param name="Point1X">Start point X</param>
    ''' <param name="Point1Y">Start point Y</param>
    ''' <param name="Point2X">End point X</param>
    ''' <param name="Point2Y">End point Y</param>
    ''' <returns>Heading value in radians</returns>
    ''' <remarks>VB Math reference system</remarks>
    Public Function CalcHeading(ByVal Point1X As Double, ByVal Point1Y As Double, _
                                ByVal Point2X As Double, ByVal Point2Y As Double) As Double
        Dim DeltaX, DeltaY As Double
        DeltaX = Point2X - Point1X
        DeltaY = Point2Y - Point1Y
        CalcHeading = Math.Atan2(DeltaY, DeltaX)
        If CalcHeading < 0 Then CalcHeading = (2 * Math.PI) + CalcHeading
    End Function

    ''' <summary>
    ''' Calculate the coordinate of a point from the given point.
    ''' </summary>
    ''' <param name="StartPointX">Start point X</param>
    ''' <param name="StartPointY">Start point Y</param>
    ''' <param name="Range">Distance between start point and end point</param>
    ''' <param name="BearingR">Bearing in radians between start point and end point</param>
    ''' <param name="EndPointX">Computed End point X</param>
    ''' <param name="EndPointY">Computed End point y</param>
    ''' <returns>Successful computation</returns>
    ''' <remarks>Bearing as cartographic reference system</remarks>
    Public Function RangeBearing(ByVal StartPointX As Double, ByVal StartPointY As Double, ByVal Range As Double, _
                                 ByVal BearingR As Double, ByRef EndPointX As Double, ByRef EndPointY As Double) As Boolean
        Select Case BearingR
            Case Is = 0
                EndPointX = StartPointX + Range
                EndPointY = StartPointY
            Case Is = Math.PI / 2
                EndPointX = StartPointX
                EndPointY = StartPointY + Range
            Case Is = Math.PI
                EndPointX = StartPointX - Range
                EndPointY = StartPointY
            Case Is = 3 * Math.PI / 2
                EndPointX = StartPointX
                EndPointY = StartPointY - Range
            Case Is = 2 * Math.PI
                EndPointX = StartPointX + Range
                EndPointY = StartPointY
            Case Is < Math.PI / 2
                EndPointX = StartPointX + If(Range > 0, Math.Abs(Range * Math.Cos(BearingR)), -Math.Abs(Range * Math.Cos(BearingR)))
                EndPointY = StartPointY + If(Range > 0, Math.Abs(Range * Math.Sin(BearingR)), -Math.Abs(Range * Math.Sin(BearingR)))
            Case Is > 3 * Math.PI / 2
                EndPointX = StartPointX + If(Range > 0, Math.Abs(Range * Math.Cos(BearingR)), -Math.Abs(Range * Math.Cos(BearingR)))
                EndPointY = StartPointY - If(Range > 0, Math.Abs(Range * Math.Sin(BearingR)), -Math.Abs(Range * Math.Sin(BearingR)))
            Case Else
                If BearingR > Math.PI / 2 And BearingR < Math.PI Then
                    EndPointX = StartPointX - If(Range > 0, Math.Abs(Range * Math.Cos(BearingR)), -Math.Abs(Range * Math.Cos(BearingR)))
                    EndPointY = StartPointY + If(Range > 0, Math.Abs(Range * Math.Sin(BearingR)), -Math.Abs(Range * Math.Sin(BearingR)))
                Else
                    If BearingR > Math.PI And BearingR < 3 * Math.PI / 2 Then
                        EndPointX = StartPointX - If(Range > 0, Math.Abs(Range * Math.Cos(BearingR)), -Math.Abs(Range * Math.Cos(BearingR)))
                        EndPointY = StartPointY - If(Range > 0, Math.Abs(Range * Math.Sin(BearingR)), -Math.Abs(Range * Math.Sin(BearingR)))
                    Else
                        EndPointX = StartPointX
                        EndPointY = StartPointY
                    End If
                End If
        End Select
        Return True
    End Function

    'Convert the degree value in to the equivalent radian value
    Public Function DegRad(ByVal AngleD As Double) As Double
        DegRad = AngleD / (180 / Math.PI)
    End Function

    'Convert the radian value in to the equivalent degree value
    Public Function RadDeg(ByVal AngleR As Double) As Double
        RadDeg = AngleR * (180 / Math.PI)
    End Function

    'Function to shrink the angle value between 0 and 2pi (0° and 360°)
    Public Function AngleFit2pi(ByVal AngleR As Double, Optional ByVal IsRadians As Boolean = True) As Double
        Dim TmpAngle As Double = If(IsRadians, AngleR, DegRad(AngleR))
        Do
            If TmpAngle > (Math.PI * 2) Then TmpAngle = TmpAngle - (Math.PI * 2)
            If TmpAngle < 0 Then TmpAngle = (Math.PI * 2) + TmpAngle
        Loop Until (TmpAngle <= (Math.PI * 2) And TmpAngle >= 0)
        Return If(IsRadians, TmpAngle, RadDeg(TmpAngle))
    End Function

    'Place angle between -180° and +180° degrees, or -pi and +pi
    Public Function AngleFit1Pi(ByVal AngleR As Double, Optional ByVal IsRadians As Boolean = True) As Double
        Dim TmpAngle As Double = If(IsRadians, AngleR, DegRad(AngleR))
        Do
            If TmpAngle > Math.PI Then TmpAngle = TmpAngle - (Math.PI * 2)
            If TmpAngle < -Math.PI Then TmpAngle = (Math.PI * 2) + TmpAngle
        Loop Until (TmpAngle <= Math.PI And TmpAngle >= -Math.PI)
        Return If(IsRadians, TmpAngle, RadDeg(TmpAngle))
    End Function

    'Convert an angle from the Math to the Coord convention.
    Public Function AngleToBearing(ByVal AngleR As Double) As Double
        Return AngleFit2pi(((2 * Math.PI) - AngleR) + (Math.PI / 2))
    End Function
    Public Function BearingToAngle(ByVal AngleR As Double) As Double
        Return AngleFit2pi((2 * Math.PI) - (AngleR - (Math.PI / 2)))
    End Function

    'Round angle at 1/2^57 value
    Public Function AngleRound(ByVal Angle As Double, Optional ByVal IsRadians As Boolean = False) As Double
        Dim z As Double = If(IsRadians, 1 / 1000000000, DegRad(1 / 1000000000)) ' Original value 1/16
        Dim y As Double = Abs(Angle)
        If y < z Then y = z - (z - y)
        If Angle < 0 Then
            Return -y
        Else
            Return y
        End If
    End Function

    'Return the opposite value of the given boolean variable
    Public Function ReverseBoolean(ByRef Value As Boolean) As Boolean
        If Value Then
            Return False
        Else
            Return True
        End If
    End Function

    'Swap two value DOUBLE
    Public Function Swap(ByRef Value1 As Double, ByRef Value2 As Double) As Boolean
        Dim Value3 As Double = Value1
        Value1 = Value2
        Value2 = Value3
        Return True
    End Function

    'Swap two value INTEGER
    Public Function Swap(ByRef Value1 As Integer, ByRef Value2 As Integer) As Boolean
        Dim Value3 As Integer = Value1
        Value1 = Value2
        Value2 = Value3
        Return True
    End Function

    'Swap two value STRING
    Public Function Swap(ByRef Value1 As String, ByRef Value2 As String) As Boolean
        Dim Value3 As String = Value1
        Value1 = Value2
        Value2 = Value3
        Return True
    End Function

    'Swap two value BYTE    
    Public Function Swap(ByRef Value1 As Byte, ByRef Value2 As Byte) As Boolean
        Dim Value3 As Byte = Value1
        Value1 = Value2
        Value2 = Value3
        Return True
    End Function

    'Calculate the distance between two points 2D
    Public Function Distance(ByVal Point1X As Double, ByVal Point1Y As Double, _
                             ByVal Point2X As Double, ByVal Point2Y As Double) As Double
        Distance = Math.Sqrt((Point1X - Point2X) ^ 2 + (Point1Y - Point2Y) ^ 2)
    End Function

    'Calculate the distance between two points 3D
    Public Function Distance(ByVal Point1X As Double, ByVal Point1Y As Double, ByVal Point1Z As Double, _
                             ByVal Point2X As Double, ByVal Point2Y As Double, ByVal Point2Z As Double) As Double
        Distance = Math.Sqrt((Point1X - Point2X) ^ 2 + (Point1Y - Point2Y) ^ 2 + (Point1Z - Point2Z) ^ 2)
    End Function

    'Secant
    Public Function Sec(ByVal Value As Double) As Double
        Sec = 1 / Cos(Value)
    End Function

    'CoSecant
    Public Function Csc(ByVal Value As Double) As Double
        Csc = 1 / Sin(Value)
    End Function

    'CoTangent
    Public Function Ctan(ByVal Value As Double) As Double
        Ctan = 1 / Tan(Value)
    End Function

    'Inverse Sin
    Public Function ASin(ByVal Value As Double) As Double
        ASin = Atan(Value / Sqrt(-Value * Value + 1))
    End Function

    'Inverse Cosine
    Public Function ACos(ByVal Value As Double) As Double
        ACos = Atan(-Value / Sqrt(-Value * Value + 1)) + 2 * Atan(1)
    End Function

    'Inverse Secant
    Public Function ASec(ByVal Value As Double) As Double
        ASec = 2 * Atan(1) - Atan(Sign(Value) / Sqrt(Value * Value - 1))
    End Function

    'Inverse CoSecant
    Public Function ACsc(ByVal Value As Double) As Double
        ACsc = Atan(Sign(Value) / Sqrt(Value * Value - 1))
    End Function

    'Inverse CoTangent
    Public Function ACot(ByVal Value As Double) As Double
        ACot = 2 * Atan(1) - Atan(Value)
    End Function

    'Hyperbolic Sin
    Public Function SinH(ByVal Value As Double) As Double
        SinH = (Exp(Value) - Exp(-Value)) / 2
    End Function

    'Hyperbolic Cosine
    Public Function CosH(ByVal Value As Double) As Double
        CosH = (Exp(Value) + Exp(-Value)) / 2
    End Function

    'Hyperbolic Tangent
    Public Function TanH(ByVal Value As Double) As Double
        TanH = (Exp(Value) - Exp(-Value)) / (Exp(Value) + Exp(-Value))
    End Function

    'Hyperbolic Secant
    Public Function SecH(ByVal Value As Double) As Double
        SecH = 2 / (Exp(Value) + Exp(-Value))
    End Function

    'Hyperbolic CoSecant
    Public Function CscH(ByVal Value As Double) As Double
        CscH = 2 / (Exp(Value) - Exp(-Value))
    End Function

    'Hyperbolic CoTangent
    Public Function CotH(ByVal Value As Double) As Double
        CotH = (Exp(Value) + Exp(-Value)) / (Exp(Value) - Exp(-Value))
    End Function

    'Inverse Hyperbolic Sin
    Public Function ASinH(ByVal Value As Double) As Double
        ASinH = Log(Value + Sqrt(Value * Value + 1))
    End Function

    'Inverse Hyperbolic Cosine
    Public Function ACosH(ByVal Value As Double) As Double
        ACosH = Log(Value + Sqrt(Value * Value - 1))
    End Function

    'Inverse Hyperbolic Tangent
    Public Function ATanH(ByVal Value As Double) As Double
        ATanH = Log((1 + Value) / (1 - Value)) / 2
    End Function

    'Inverse Hyperbolic Secant
    Public Function ASecH(ByVal Value As Double) As Double
        ASecH = Log((Sqrt(-Value * Value + 1) + 1) / Value)
    End Function

    'Inverse Hyperbolic CoSecant
    Public Function ACscH(ByVal Value As Double) As Double
        ACscH = Log((Sign(Value) * Sqrt(Value * Value + 1) + 1) / Value)
    End Function

    'Inverse Hyperbolic CoTangent
    Public Function ACotH(ByVal Value As Double) As Double
        ACotH = Log((Value + 1) / (Value - 1)) / 2
    End Function

    'Hypotenuse
    Public Function Hypot(ByVal X As Double, ByVal Y As Double) As Double
        Dim a, b As Double
        X = Abs(X)
        Y = Abs(Y)
        a = Max(X, Y)
        b = Min(X, Y) / (If(a = 0, 1, a))
        Hypot = a * Sqrt(1 + b * b)
    End Function

    'Calculate the Nth root of a number
    Public Function NthRoot(ByVal Value As Double, ByVal Root As Double) As Double
        NthRoot = Pow(Value, (1 / Root))
    End Function

    Public Function IsFinite(ByVal Value As Double) As Boolean
        If Double.IsInfinity(Value) Then
            Return False
        ElseIf Double.IsNaN(Value) Then
            Return False
        ElseIf Double.MinValue < Value And Value < Double.MaxValue Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function MultipleOf(Value As Double, Base As Double) As Boolean
        Dim Res1, Res2 As Double
        Res1 = Value / Base
        Res2 = Truncate(Res1)
        If Res1 = Res2 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function MultipleOf(Value As Integer, Base As Integer) As Boolean
        Dim Res1, Res2 As Double
        Res1 = Value / Base
        Res2 = Truncate(Res1)
        If Res1 = Res2 Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function MultipleOf(Value As Single, Base As Single) As Boolean
        Dim Res1, Res2 As Double
        Res1 = Value / Base
        Res2 = Truncate(Res1)
        If Res1 = Res2 Then
            Return True
        Else
            Return False
        End If
    End Function

End Module
