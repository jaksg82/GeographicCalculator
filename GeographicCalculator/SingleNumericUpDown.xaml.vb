Public NotInheritable Class SingleNumericUpDown
    Inherits UserControl
    Implements INotifyPropertyChanged
    'Dim IntValue As Integer

    Public ReadOnly ValueProperty As DependencyProperty = DependencyProperty.Register("SelectedValue", GetType(Integer), GetType(SingleNumericUpDown), Nothing)
    'Public ValueProperty As DependencyProperty = DependencyProperty.Register("SelectedValue", GetType(Integer), GetType(SingleNumericUpDown), Nothing)
    Public Property SelectedValue As Integer
        Set(value As Integer)
            SetValue(ValueProperty, value)
            NotifyPropertyChanged()
        End Set
        Get
            SelectedValue = CType(GetValue(ValueProperty), Integer)
        End Get
    End Property

    'Public ReadOnly ValuePropertyString As DependencyProperty = DependencyProperty.Register("SelectedValueString", GetType(String), GetType(SingleNumericUpDown), Nothing)
    'Public ReadOnly Property SelectedValueString As String
    'Get
    'SelectedValueString = CType(GetValue(ValueProperty), String)
    'End Get
    'End Property

    Public Event SelectedValueChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Private Sub NotifyPropertyChanged(<CallerMemberName()> Optional ByVal propertyName As String = Nothing)
        RaiseEvent SelectedValueChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Sub New()
        Me.DataContext = Me

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        SelectedValue = 0
        TbValue.Text = SelectedValue.ToString
    End Sub

    Private Sub BtUp_Click(sender As Object, e As RoutedEventArgs) Handles BtUp.Tapped
        If SelectedValue = 9 Then
            SelectedValue = 0
        Else
            SelectedValue = SelectedValue + 1
        End If
        TbValue.Text = SelectedValue.ToString
    End Sub

    Private Sub BtDn_Click(sender As Object, e As RoutedEventArgs) Handles BtDn.Tapped
        If SelectedValue = 0 Then
            SelectedValue = 9
        Else
            SelectedValue = SelectedValue - 1
        End If
        TbValue.Text = SelectedValue.ToString
    End Sub

    Private Sub SingleNumericUpDown_SelectedValueChanged(sender As Object, e As PropertyChangedEventArgs) 'Handles Me.SelectedValueChanged
        TbValue.Text = SelectedValue.ToString
    End Sub
End Class
