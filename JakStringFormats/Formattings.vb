'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
'
' Filename:     Formattings.vb
' Project:      jaktools
' Version:      0.3
' Author:       Simone Giacomoni (jaksg82@yahoo.it)
'               http://www.vivoscuola.it/us/simone.giacomoni/devtools/index.html
'
' Copyright @2014, Simone Giacomoni
' 
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

Public Module Formattings

    Public Enum DmsFormat As Integer
        SimpleDMS = 0
        SimpleDM = 1
        SimpleD = 2
        VerboseDMS = 3
        VerboseDM = 4
        VerboseD = 5
        SimpleR = 6
        EsriDMS = 7
        EsriDM = 8
        EsriD = 9
        EsriPackedDMS = 10
        UkooaDMS = 11
        NMEA = 12
        SpacedDMS = 13
        SpacedDM = 14
    End Enum

    Public Enum DmsSign As Integer
        PlusMinus = 0
        Prefix = 1
        Suffix = 2
        Generic = 3
    End Enum

    ''' <summary>
    ''' Create an array of angle formatting strings
    ''' </summary>
    ''' <param name="FormatArray">Array of strings</param>
    ''' <returns>Number of formats available</returns>
    ''' <remarks></remarks>
    Public Function ListDmsFormats(ByRef FormatArray() As String) As Integer
        Dim MaxIndex As Integer = 14
        ReDim FormatArray(MaxIndex)
        FormatArray(0) = "DDD:MM:SS.000"
        FormatArray(1) = "DDD:MM.000"
        FormatArray(2) = "DDD.000"
        FormatArray(3) = "DDD°MM" & Convert.ToChar(39) & "SS.000" & Convert.ToChar(34)
        FormatArray(4) = "DDD°MM.000" & Convert.ToChar(39)
        FormatArray(5) = "DDD.000°"
        FormatArray(6) = "R.000000r"
        FormatArray(7) = "DDD° MM" & Convert.ToChar(39) & " SS.000" & Convert.ToChar(34)
        FormatArray(8) = "DDD° MM.000" & Convert.ToChar(39)
        FormatArray(9) = "DDD.000°"
        FormatArray(10) = "DDD.MMSS000000"
        FormatArray(11) = "DDMMSS.00"
        FormatArray(12) = "DDMM.000"
        FormatArray(13) = "DD MM SS.000"
        FormatArray(14) = "DD MM.000"
        Return MaxIndex
    End Function

    ''' <summary>
    ''' Create an array of angle formatting strings
    ''' </summary>
    ''' <param name="FormatArray">Array of strings</param>
    ''' <returns>Number of formats available</returns>
    ''' <remarks></remarks>
    Public Function ListDmsSigns(ByRef FormatArray() As String) As Integer
        Dim MaxIndex As Integer = 3
        ReDim FormatArray(MaxIndex)
        FormatArray(0) = "+/-"
        FormatArray(1) = "Prefix (E/W & N/S)"
        FormatArray(2) = "Suffix (E/W & N/S)"
        FormatArray(3) = "Generic Angle"
        Return MaxIndex
    End Function

    Public Enum MetricSign As Integer
        Number = 0
        Unit = 1
        Prefix = 2
        Suffix = 3
        UnitPrefix = 4
        UnitSuffix = 5
    End Enum

    ''' <summary>
    ''' Create an array of metric coordinates formatting strings
    ''' </summary>
    ''' <param name="SignArray">Array of strings</param>
    ''' <returns>Number of formats available</returns>
    ''' <remarks></remarks>
    Public Function ListMetricSigns(ByRef SignArray() As String) As Integer
        Dim MaxIndex As Integer = 5
        ReDim SignArray(MaxIndex)
        SignArray(0) = "Simple Number"
        SignArray(1) = "Unit (m)"
        SignArray(2) = "Prefix (E/W & N/S)"
        SignArray(3) = "Suffix (E/W & N/S)"
        SignArray(4) = "Prefix (E/W & N/S) & Unit (m)"
        SignArray(5) = "Unit (m) & Suffix (E/W & N/S)"
        Return MaxIndex
    End Function

    ''' <summary>
    ''' Convert a number in to string representation with given decimals
    ''' </summary>
    ''' <param name="Value">The value to convert</param>
    ''' <param name="Decimals">Quantity of numbers after the decimal point</param>
    ''' <param name="LeaderPaddings">Quantity of numbers before the decimal point</param>
    ''' <returns>String representation of the given value</returns>
    ''' <remarks></remarks>
    Public Function FormatNumber(Value As Double, Decimals As Integer, Optional LeaderPaddings As Integer = 0) As String
        Dim TempString As String
        Dim IntegerPart, DecimalPart, DecimalMultiplier As Integer
        Try
            IntegerPart = CInt(Math.Truncate(Value))
            DecimalMultiplier = CInt(Math.Pow(10.0, Decimals))
            DecimalPart = Math.Abs(CInt(Math.Round((Value - Math.Truncate(Value)) * DecimalMultiplier)))
            TempString = IntegerPart.ToString
            If LeaderPaddings > TempString.Length Then
                TempString = TempString.PadLeft(LeaderPaddings, CChar("0"))
            End If
            TempString = TempString & "." & DecimalPart.ToString.PadLeft(Decimals, CChar("0"))
        Catch ex As Exception
            TempString = Value.ToString()
        End Try
        Return TempString
    End Function

    ''' <summary>
    ''' Retrieve the angular value from a formatted string
    ''' </summary>
    ''' <param name="AngleString">Formatted string to parse</param>
    ''' <param name="AngleFormat">Expected angle format</param>
    ''' <param name="SignFormat">Expected sign format</param>
    ''' <param name="AsRadians">True to have the result in radians, False for degrees</param>
    ''' <returns>Parsed angle</returns>
    ''' <remarks></remarks>
    Public Function DmsParse(ByVal AngleString As String, ByVal AngleFormat As DmsFormat, ByVal SignFormat As DmsSign, _
                             Optional ByVal AsRadians As Boolean = True) As Double
        Dim SignMult As Integer
        Dim Degs, Mins, Secs, DecSecs, Signs, TmpString, TmpSubString() As String
        Dim ParsedAngleD, ParsedAngleR As Double
        'Define the number format
        Dim InternalCulture As New Globalization.CultureInfo("en-US")
        InternalCulture.NumberFormat.NumberDecimalSeparator = "."
        InternalCulture.NumberFormat.NumberGroupSeparator = "'"
        'Clean the Angle string from extra space characters
        AngleString = AngleString.Trim
        'Retrieve the sign of the angle
        Select Case SignFormat
            Case DmsSign.Suffix 'Suffix
                Signs = AngleString.Substring(AngleString.Length - 1)
            Case Else 'Prefix and Sign
                Signs = AngleString.Substring(0, 1)
        End Select
        'Handle the sign of the angle
        Select Case Signs.ToUpper
            Case "E", "N"
                SignMult = 1
            Case "W", "S", "-"
                SignMult = -1
            Case Else
                SignMult = 1
        End Select
        'Clean the Angle string from extra characters
        Do
            Select Case AngleString.Substring(0, 1)
                Case "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                    Exit Do
                Case Else
                    AngleString = AngleString.Substring(1)
            End Select
        Loop
        Do
            Select Case AngleString.Substring(AngleString.Length - 1)
                Case "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", Convert.ToChar(34).ToString, Convert.ToChar(39).ToString, Convert.ToChar(176).ToString
                    Exit Do
                Case Else
                    AngleString.Substring(0, AngleString.Length - 1)
            End Select
        Loop
        'Parse the Angle value
        Select Case AngleFormat
            Case DmsFormat.SimpleDMS 'ddd:mm:ss.000
                TmpSubString = AngleString.Split(CChar(":"))
                If TmpSubString.Count = 3 Then
                    Try
                        ParsedAngleD = (Integer.Parse(TmpSubString(0)) + (Integer.Parse(TmpSubString(1)) / 60) + _
                                    (Double.Parse(TmpSubString(2), InternalCulture.NumberFormat) / 3600))
                    Catch ex As Exception
                        Return Double.NaN
                    End Try
                Else
                    Return Double.NaN
                End If

            Case DmsFormat.SimpleDM 'ddd:mm.000
                TmpSubString = AngleString.Split(CChar(":"))
                If TmpSubString.Count = 2 Then
                    Try
                        ParsedAngleD = (Integer.Parse(TmpSubString(0)) + (Double.Parse(TmpSubString(1), InternalCulture.NumberFormat) / 60))
                    Catch ex As Exception
                        Return Double.NaN
                    End Try
                Else
                    Return Double.NaN
                End If

            Case DmsFormat.SimpleD 'ddd.000
                ParsedAngleD = (Double.Parse(AngleString, InternalCulture.NumberFormat))

            Case DmsFormat.VerboseDMS, DmsFormat.EsriDMS  'dd°mm'ss"
                TmpSubString = AngleString.Split(CChar("°"))
                If TmpSubString.Count = 2 Then
                    Degs = TmpSubString(0)
                    TmpString = TmpSubString(1)
                    TmpSubString = TmpString.Split(CChar("'"))
                    If TmpSubString.Count = 2 Then
                        Mins = TmpSubString(0)
                        Secs = TmpSubString(1)
                        If Secs.EndsWith(Convert.ToChar(39)) Then
                            Secs = Secs.Substring(0, Secs.Length - 1)
                        End If
                        Try
                            ParsedAngleD = (Integer.Parse(Degs) + (Integer.Parse(Mins) / 60) + _
                                       (Double.Parse(Secs, InternalCulture.NumberFormat) / 3600))
                        Catch ex As Exception
                            Return Double.NaN
                        End Try
                    Else
                        Return Double.NaN
                    End If
                Else
                    Return Double.NaN
                End If

            Case DmsFormat.VerboseDM, DmsFormat.EsriDM  'dd°mm'
                TmpSubString = AngleString.Split(CChar("°"))
                If TmpSubString.Count = 2 Then
                    Try
                        ParsedAngleD = (Integer.Parse(TmpSubString(0)) + (Double.Parse(TmpSubString(1), InternalCulture.NumberFormat) / 60))
                    Catch ex As Exception
                        Return Double.NaN
                    End Try
                Else
                    Return Double.NaN
                End If

            Case DmsFormat.VerboseD, DmsFormat.EsriD  'dd°
                If AngleString.EndsWith(Convert.ToChar(39)) Then
                    Degs = AngleString.Substring(0, AngleString.Length - 1)
                Else
                    Degs = AngleString
                End If
                Try
                    ParsedAngleD = Double.Parse(Degs, InternalCulture.NumberFormat)
                Catch ex As Exception
                    Return Double.NaN
                End Try

            Case DmsFormat.SimpleR  'RRR.000000r
                If AngleString.EndsWith("r") Then
                    Degs = AngleString.Substring(0, AngleString.Length - 1)
                Else
                    Degs = AngleString
                End If
                Try
                    ParsedAngleD = (Double.Parse(Degs, InternalCulture.NumberFormat) * (180 / Math.PI))
                Catch ex As Exception
                    Return Double.NaN
                End Try

            Case DmsFormat.EsriPackedDMS  'DDD.MMSSsss
                TmpSubString = AngleString.Split(CChar("."))
                If TmpSubString.Count = 2 Then
                    Degs = TmpSubString(0)
                    Mins = TmpSubString(1).Substring(0, 2)
                    Secs = TmpSubString(1).Substring(2, 2)
                    DecSecs = TmpSubString(1).Substring(4)
                    Try
                        ParsedAngleD = (Integer.Parse(Degs) + (Integer.Parse(Mins) / 60) + _
                                    (Double.Parse(Secs & "." & DecSecs, InternalCulture.NumberFormat) / 3600))
                    Catch ex As Exception
                        Return Double.NaN
                    End Try
                Else
                    Return Double.NaN
                End If

            Case DmsFormat.UkooaDMS  'DDMMSS.ss
                TmpSubString = AngleString.Split(CChar("."))
                If TmpSubString.Count = 2 Then
                    Degs = TmpSubString(0).Substring(0, TmpSubString.Count - 4)
                    Mins = TmpSubString(0).Substring(TmpSubString.Count - 4, 2)
                    Secs = TmpSubString(0).Substring(TmpSubString.Count - 2, 2)
                    DecSecs = TmpSubString(1)
                    Try
                        ParsedAngleD = (Integer.Parse(Degs) + (Integer.Parse(Mins) / 60) + _
                                    (Double.Parse(Secs & "." & DecSecs, InternalCulture.NumberFormat) / 3600))
                    Catch ex As Exception
                        Return Double.NaN
                    End Try
                Else
                    Return Double.NaN
                End If

            Case DmsFormat.NMEA 'DDMM.mmm
                TmpSubString = AngleString.Split(CChar("."))
                If TmpSubString.Count = 2 Then
                    Degs = TmpSubString(0).Substring(0, TmpSubString.Count - 2)
                    Mins = TmpSubString(0).Substring(TmpSubString.Count - 2, 2)
                    Mins = Mins & "." & TmpSubString(1)
                    Try
                        ParsedAngleD = (Integer.Parse(Degs) + (Double.Parse(Mins, InternalCulture.NumberFormat) / 60))
                    Catch ex As Exception
                        Return Double.NaN
                    End Try
                Else
                    Return Double.NaN
                End If

            Case DmsFormat.SpacedDMS 'ddd mm ss.000
                TmpSubString = AngleString.Split(CChar(" "))
                If TmpSubString.Count = 3 Then
                    Try
                        ParsedAngleD = (Integer.Parse(TmpSubString(0)) + (Integer.Parse(TmpSubString(1)) / 60) + _
                                    (Double.Parse(TmpSubString(2), InternalCulture.NumberFormat) / 3600))
                    Catch ex As Exception
                        Return Double.NaN
                    End Try
                Else
                    Return Double.NaN
                End If

            Case DmsFormat.SpacedDM  'ddd:mm.000
                TmpSubString = AngleString.Split(CChar(" "))
                If TmpSubString.Count = 2 Then
                    Try
                        ParsedAngleD = (Integer.Parse(TmpSubString(0)) + (Double.Parse(TmpSubString(1), InternalCulture.NumberFormat) / 60))
                    Catch ex As Exception
                        Return Double.NaN
                    End Try
                Else
                    Return Double.NaN
                End If

            Case Else
                Return Double.NaN

        End Select
        ParsedAngleD = ParsedAngleD * SignMult
        ParsedAngleR = ParsedAngleD / (180 / Math.PI)
        Return If(AsRadians, ParsedAngleR, ParsedAngleD)

    End Function

    ''' <summary>
    ''' Convert the degree value in a formatted geographic coordinate string
    ''' </summary>
    ''' <param name="AngleR">Latitude or Longitude value</param>
    ''' <param name="AngleFormat">Format for the numbers</param>
    ''' <param name="SignFormat">Format for the sign</param>
    ''' <param name="Decimals">Decimal places</param>
    ''' <param name="IsLat">True for Latitude, False for Longitude values</param>
    ''' <returns>Formatted String</returns>
    ''' <remarks></remarks>
    Public Function FormatDMS(ByVal AngleR As Double, ByVal AngleFormat As DmsFormat, ByVal SignFormat As DmsSign, _
                              ByVal Decimals As Integer, ByVal IsLat As Boolean) As String
        Dim Degs, Mins, Secs, AngleD As Double
        Dim Lbl, Result As String
        Dim DblQt, SngQt As Char
        DblQt = Convert.ToChar(34)
        SngQt = Convert.ToChar(39)

        AngleD = AngleR * (180 / Math.PI)

        'Evaluate the quadrant of the angle
        If IsLat Then
            If AngleR < 0 Then
                Lbl = If(SignFormat = DmsSign.PlusMinus, "-", "S")
            Else
                Lbl = If(SignFormat = DmsSign.PlusMinus, "+", "N")
            End If
        Else
            If AngleR < 0 Then
                Lbl = If(SignFormat = DmsSign.PlusMinus, "-", "W")
            Else
                Lbl = If(SignFormat = DmsSign.PlusMinus, "+", "E")
            End If
        End If

        'Take the absolute value of angle
        AngleD = Math.Abs(AngleD)
        'Compose the formatted string
        Select Case AngleFormat
            Case DmsFormat.SimpleDMS  'dd:mm:ss
                Degs = Math.Truncate(AngleD)
                Mins = Math.Truncate((AngleD - Degs) * 60)
                Secs = (((AngleD - Degs) * 60) - Mins) * 60
                If Math.Round(Secs, Decimals) = 60.0 Then
                    Secs = 0
                    Mins = Mins + 1
                    If Mins = 60 Then
                        Mins = 0
                        Degs = Degs + 1
                    End If
                End If
                Result = Degs.ToString("00") & ":" & Mins.ToString("00") & ":" & FormatNumber(Secs, Decimals, 2)

            Case DmsFormat.SimpleDM  'dd:mm
                Degs = Math.Truncate(AngleD)
                Mins = (AngleD - Degs) * 60
                If Math.Round(Mins, Decimals) = 60.0 Then
                    Mins = 0
                    Degs = Degs + 1
                End If
                Result = Degs.ToString("00") & ":" & FormatNumber(Mins, Decimals, 2)

            Case DmsFormat.SimpleD  'dd
                Result = FormatNumber(AngleD, Decimals)

            Case DmsFormat.VerboseDMS  'dd°mm'ss"
                Degs = Math.Truncate(AngleD)
                Mins = Math.Truncate((AngleD - Degs) * 60)
                Secs = (((AngleD - Degs) * 60) - Mins) * 60
                If Math.Round(Secs, Decimals) = 60.0 Then
                    Secs = 0
                    Mins = Mins + 1
                    If Mins = 60 Then
                        Mins = 0
                        Degs = Degs + 1
                    End If
                End If
                Result = Degs.ToString("00") & "°" & Mins.ToString("00") & SngQt & FormatNumber(Secs, Decimals, 2) & DblQt

            Case DmsFormat.VerboseDM  'dd°mm'
                Degs = Math.Truncate(AngleD)
                Mins = (AngleD - Degs) * 60
                If Math.Round(Mins, Decimals) = 60.0 Then
                    Mins = 0
                    Degs = Degs + 1
                End If
                Result = Degs.ToString("00") & "°" & FormatNumber(Mins, Decimals, 2) & SngQt

            Case DmsFormat.VerboseD  'dd°
                Result = FormatNumber(AngleD, Decimals) & "°"

            Case DmsFormat.SimpleR  'rad
                Result = FormatNumber(AngleR, Decimals) & "r"

            Case DmsFormat.EsriDMS  'dd° mm' ss"
                Degs = Math.Truncate(AngleD)
                Mins = Math.Truncate((AngleD - Degs) * 60)
                Secs = (((AngleD - Degs) * 60) - Mins) * 60
                If Math.Round(Secs, Decimals) = 60.0 Then
                    Secs = 0
                    Mins = Mins + 1
                    If Mins = 60 Then
                        Mins = 0
                        Degs = Degs + 1
                    End If
                End If
                Result = Degs.ToString("00") & "° " & Mins.ToString("00") & SngQt & FormatNumber(Secs, Decimals, 2) & DblQt

            Case DmsFormat.EsriDM  'dd° mm'
                Degs = Math.Truncate(AngleD)
                Mins = (AngleD - Degs) * 60
                If Math.Round(Mins, Decimals) = 60.0 Then
                    Mins = 0
                    Degs = Degs + 1
                End If
                Result = Degs.ToString("00") & "° " & FormatNumber(Mins, Decimals, 2) & SngQt

            Case DmsFormat.EsriD  'dd°
                Result = FormatNumber(AngleD, Decimals) & "°"

            Case DmsFormat.EsriPackedDMS  'DDD.MMSSsss
                Degs = Math.Truncate(AngleD)
                Mins = Math.Truncate((AngleD - Degs) * 60)
                Secs = ((((AngleD - Degs) * 60) - Mins) * 60)
                If Math.Round(Secs, Decimals) = 60.0 Then
                    Secs = 0
                    Mins = Mins + 1
                    If Mins = 60 Then
                        Mins = 0
                        Degs = Degs + 1
                    End If
                End If
                Result = Degs.ToString("000") & "." & Mins.ToString("00") & FormatNumber(Secs, Decimals, 2)

            Case DmsFormat.UkooaDMS  'DDMMSS.ss
                Degs = Math.Truncate(AngleD)
                Mins = Math.Truncate((AngleD - Degs) * 60)
                Secs = ((((AngleD - Degs) * 60) - Mins) * 60)
                If Math.Round(Secs, Decimals) = 60.0 Then
                    Secs = 0
                    Mins = Mins + 1
                    If Mins = 60 Then
                        Mins = 0
                        Degs = Degs + 1
                    End If
                End If
                Result = Degs.ToString("0") & Mins.ToString("00") & FormatNumber(Secs, Decimals, 2)

            Case DmsFormat.NMEA
                Degs = Math.Truncate(AngleD)
                Mins = (AngleD - Degs) * 60
                If Math.Round(Mins, Decimals) = 60.0 Then
                    Mins = 0
                    Degs = Degs + 1
                End If
                Result = Degs.ToString("00") & FormatNumber(Mins, Decimals, 2)

            Case DmsFormat.SpacedDMS  'dd:mm:ss
                Degs = Math.Truncate(AngleD)
                Mins = Math.Truncate((AngleD - Degs) * 60)
                Secs = (((AngleD - Degs) * 60) - Mins) * 60
                If Math.Round(Secs, Decimals) = 60.0 Then
                    Secs = 0
                    Mins = Mins + 1
                    If Mins = 60 Then
                        Mins = 0
                        Degs = Degs + 1
                    End If
                End If
                Result = Degs.ToString("00") & " " & Mins.ToString("00") & " " & FormatNumber(Secs, Decimals, 2)

            Case DmsFormat.SpacedDM  'dd:mm
                Degs = Math.Truncate(AngleD)
                Mins = (AngleD - Degs) * 60
                If Math.Round(Mins, Decimals) = 60.0 Then
                    Mins = 0
                    Degs = Degs + 1
                End If
                Result = Degs.ToString("00") & " " & FormatNumber(Mins, Decimals, 2)

            Case Else 'rad
                Result = FormatNumber(AngleR, Decimals) & "r"

        End Select
        'Add the correct sign
        Select Case SignFormat
            Case DmsSign.Suffix
                Return Result & Lbl
            Case DmsSign.Generic
                Return If(AngleR < 0, "-" & Result, Result)
            Case Else
                Return Lbl & Result
        End Select

    End Function

    ''' <summary>
    ''' Retrieve the metric value from a formatted string
    ''' </summary>
    ''' <param name="MetricString">Text to be parsed</param>
    ''' <param name="MetricFormat">Expected format</param>
    ''' <returns>Parsed value</returns>
    ''' <remarks></remarks>
    Public Function MetricParse(ByVal MetricString As String, ByVal MetricFormat As MetricSign) As Double
        Dim SignMult As Integer
        Dim Signs As String
        Dim ParsedMetric As Double

        'Clean the metric string from extra space characters
        MetricString = MetricString.Trim
        'Retrieve the sign of the coord
        Select Case MetricFormat
            Case MetricSign.Suffix, MetricSign.UnitSuffix 'Suffix
                Signs = MetricString.Substring(MetricString.Length - 1, 1)
            Case Else 'Prefix and Sign
                Signs = MetricString.Substring(0, 1)
        End Select

        'Handle the sign of the coord
        Select Case Signs.ToUpper
            Case "E", "N"
                SignMult = 1
            Case "W", "S", "-"
                SignMult = -1
            Case Else
                SignMult = 1
        End Select

        'Clean the coord string from extra characters
        Do
            Select Case MetricString.Substring(0, 1)
                Case "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                    'String ok on the left side
                    Exit Do
                Case Else
                    MetricString = MetricString.Substring(1)
            End Select
        Loop
        Do
            Select Case MetricString.Substring(MetricString.Length - 1, 1)
                Case "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                    'String ok on the right side
                    Exit Do
                Case Else
                    MetricString = MetricString.Substring(0, MetricString.Length - 1)
            End Select
        Loop
        'Convert the coord in a value
        Try
            ParsedMetric = CDbl(MetricString)
        Catch ex As Exception
            ParsedMetric = 0
        End Try

        'Prepare the final result
        ParsedMetric = ParsedMetric * SignMult
        Return ParsedMetric

    End Function

    ''' <summary>
    ''' Convert the metric value in a formatted coordinate string
    ''' </summary>
    ''' <param name="MetricCoord">East or North coordinate</param>
    ''' <param name="MetricFormat">Format for the numbers</param>
    ''' <param name="Decimals">Decimal places</param>
    ''' <param name="IsNorth">True for North, False for East values</param>
    ''' <returns>Formatted string</returns>
    ''' <remarks></remarks>
    Public Function FormatMetric(ByVal MetricCoord As Double, ByVal MetricFormat As MetricSign, ByVal Decimals As Integer, ByVal IsNorth As Boolean) As String
        Dim Lbl, Result As String

        'Evaluate the quadrant of the angle
        If MetricCoord < 0 Then
            Lbl = If(IsNorth, "S", "W")
        Else
            Lbl = If(IsNorth, "N", "E")
        End If

        'Prepare the coord string
        Select Case MetricFormat
            Case MetricSign.Number
                Result = FormatNumber(MetricCoord, Decimals)
            Case MetricSign.Unit
                Result = FormatNumber(MetricCoord, Decimals) & "m"
            Case MetricSign.Prefix
                Result = Lbl & FormatNumber(Math.Abs(MetricCoord), Decimals)
            Case MetricSign.Suffix
                Result = FormatNumber(Math.Abs(MetricCoord), Decimals) & Lbl
            Case MetricSign.UnitPrefix
                Result = Lbl & FormatNumber(Math.Abs(MetricCoord), Decimals) & "m"
            Case MetricSign.UnitSuffix
                Result = FormatNumber(Math.Abs(MetricCoord), Decimals) & "m" & Lbl
            Case Else
                Result = FormatNumber(MetricCoord, Decimals)
        End Select
        Return Result
    End Function

    ''' <summary>
    ''' Convert the date value in to Posix value
    ''' </summary>
    ''' <param name="InDateTime">Input Date value</param>
    ''' <returns>Converted Posix value</returns>
    ''' <remarks></remarks>
    Public Function FormatPosixTime(InDateTime As DateTime) As Double
        Dim UnixTimeSpan As TimeSpan = (InDateTime.Subtract(New DateTime(1970, 1, 1, 0, 0, 0)))
        Return UnixTimeSpan.TotalSeconds
    End Function

    ''' <summary>
    ''' Convert Posix value in to a Date value
    ''' </summary>
    ''' <param name="InPosixTime">Input Posix value</param>
    ''' <returns>Converted Date value</returns>
    ''' <remarks></remarks>
    Public Function ParsePosixTime(InPosixTime As Double) As DateTime
        Return (New DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(InPosixTime)
    End Function

    ''' <summary>
    ''' Count the occurrences of a given char inside a string
    ''' </summary>
    ''' <param name="value">Input String</param>
    ''' <param name="ch">Char to count</param>
    ''' <returns>Number of char found</returns>
    ''' <remarks></remarks>
    Public Function CountCharacter(ByVal value As String, ByVal ch As Char) As Integer
        Dim cnt As Integer = 0
        For Each c As Char In value
            If c = ch Then cnt += 1
        Next
        Return cnt
    End Function

    ''' <summary>
    ''' Delete duplicated spaces inside the given string
    ''' </summary>
    ''' <param name="OriginalString">Input String</param>
    ''' <returns>Cleaned String</returns>
    ''' <remarks></remarks>
    Public Function DeleteExtraSpaceChar(OriginalString As String) As String
        Dim ResString, ActChar As String
        ResString = ""
        For c = 0 To OriginalString.Count - 1
            ActChar = OriginalString.Substring(c, 1)
            If ActChar = " " Then
                'Actual character is a space so check if the last saved character
                If ResString.Count > 0 Then
                    If ResString.Substring(ResString.Count - 1) = " " Then
                        'Skip space
                    Else
                        'Add the space
                        ResString = ResString & ActChar
                    End If
                End If
            Else
                'Actual character is not a space so add to the result string
                ResString = ResString & ActChar
            End If
        Next
        'Delete the last character if is a space
        If ResString.Substring(ResString.Count - 1, 1) = " " Then
            ResString = ResString.Substring(0, ResString.Count - 1)
        End If
        Return ResString
    End Function

End Module
