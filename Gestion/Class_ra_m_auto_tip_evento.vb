
Public Class Class_ra_m_auto_tip_evento
    Private m_ID_M_AUTO_TIP_EVENT As Integer
    Public Property ID_M_AUTO_TIP_EVENT() As Integer
        Get
            Return m_ID_M_AUTO_TIP_EVENT
        End Get
        Set(value As Integer)
            m_ID_M_AUTO_TIP_EVENT = value
        End Set
    End Property
    Private m_TIP_AUTO_TIP_EVENTO As Integer
    Public Property TIP_AUTO_TIP_EVENTO() As Integer
        Get
            Return m_TIP_AUTO_TIP_EVENTO
        End Get
        Set(value As Integer)
            m_TIP_AUTO_TIP_EVENTO = value
        End Set
    End Property
    Private m_NOMBRE_TIPO_EVENTO As String
    Public Property NOMBRE_TIPO_EVENTO() As String
        Get
            Return m_NOMBRE_TIPO_EVENTO
        End Get
        Set(value As String)
            m_NOMBRE_TIPO_EVENTO = value
        End Set
    End Property
    Private m_COMEXION_DB As String
    Public Property COMEXION_DB() As String
        Get
            Return m_COMEXION_DB
        End Get
        Set(value As String)
            m_COMEXION_DB = value
        End Set
    End Property

    Function Solicita_estructura_tipo_evento(ByVal id_auto_eveto As Integer,
                                             ByRef Class_ra_m_auto_tip_evento_ As Class_ra_m_auto_tip_evento) As String
        '-----------------------------------------------
        'Funcion : Solicita del tipo evento
        'con la identificación del tipo de evento
        'Fecha : 2022-02-14
        'Ing . Miguel Angel Urueta Miranda
        '-----------------------------------------------
        Try
            Dim Parametro_Consulta As String = "SELECT ID_M_AUTO_TIP_EVENT,TIP_AUTO_TIP_EVENTO,NOMBRE_TIPO_EVENTO,COMEXION_DB " &
            " FROM ra_m_auto_tip_evento  " &
            " where ID_M_AUTO_TIP_EVENT=" & id_auto_eveto
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_m_auto_tip_evento")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_tipo_evento = "Función Solicita_estructura_tipo_evento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Class_ra_m_auto_tip_evento_ = New Class_ra_m_auto_tip_evento
                Class_ra_m_auto_tip_evento_.ID_M_AUTO_TIP_EVENT = Datset.Tables(0).Rows(0).Item(0)
                Class_ra_m_auto_tip_evento_.TIP_AUTO_TIP_EVENTO = Datset.Tables(0).Rows(0).Item(1)
                Class_ra_m_auto_tip_evento_.NOMBRE_TIPO_EVENTO = Datset.Tables(0).Rows(0).Item(2)
                Class_ra_m_auto_tip_evento_.COMEXION_DB = Datset.Tables(0).Rows(0).Item(3)
                Solicita_estructura_tipo_evento = "YES"
                Exit Function
            Else
                Solicita_estructura_tipo_evento = "Imposible encontrar la conexión del evento de auto poblado del meta dato (" & id_auto_eveto & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_tipo_evento = "Inconsistencia general fucion Solicita_estructura_tipo_evento " & ex.Message
        End Try
    End Function
End Class
