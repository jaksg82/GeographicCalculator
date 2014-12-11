' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

Public NotInheritable Class NumericBox
    Inherits UserControl
    Dim Num0, Num1 As Double

    Public Event ValueChanged()

    Public Property Value As Double
        Get
            Return Num0
        End Get
        Set(Value As Double)
            If JakMathLib.IsFinite(Value) Then
                Num0 = Value
            Else
                Num0 = 0.0
            End If
            NumInput_LostFocus()
        End Set
    End Property

    Private Sub NumInput_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles NumInput.KeyDown
        Dim InKey As Integer = e.Key
        e.Handled = (InKey >= 34 And InKey <= 43) Or (InKey >= 74 And InKey <= 83) Or InKey = 2
    End Sub

    Private Sub NumInput_Loaded(sender As Object, e As RoutedEventArgs) Handles NumInput.Loaded
        If Num0 = Nothing Then
            Num0 = 0.0
        End If
        Num1 = Num0
    End Sub

    Private Sub NumInput_LostFocus() Handles NumInput.LostFocus
        Try
            If Double.TryParse(NumInput.Text, Num1) Then
                'number parsed
                Num0 = Num1
                RaiseEvent ValueChanged()
            End If
        Catch ex As Exception
            'do nothing
        End Try
        NumInput.Text = Num0.ToString
    End Sub

End Class
