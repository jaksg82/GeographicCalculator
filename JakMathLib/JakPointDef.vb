Public Class Point3D
    Dim dblX, dblY, dblZ As Double

    Property X As Double
        Set(value As Double)
            dblX = value
        End Set
        Get
            Return dblX
        End Get
    End Property

    Property Y As Double
        Set(value As Double)
            dblY = value
        End Set
        Get
            Return dblY
        End Get
    End Property

    Property Z As Double
        Set(value As Double)
            dblZ = value
        End Set
        Get
            Return dblZ
        End Get
    End Property

    Public Sub New()
        dblX = Double.NaN
        dblY = Double.NaN
        dblZ = Double.NaN
    End Sub


End Class
