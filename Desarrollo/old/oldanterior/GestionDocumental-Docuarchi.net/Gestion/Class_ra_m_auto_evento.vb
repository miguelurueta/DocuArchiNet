Public Class Class_ra_m_auto_evento
    Private m_ID_M_AUTO_EVENTO As Integer
    Public Property ID_M_AUTO_EVENTO() As Integer
        Get
            Return m_ID_M_AUTO_EVENTO
        End Get
        Set(value As Integer)
            m_ID_M_AUTO_EVENTO = value
        End Set
    End Property
    Private m_id_detalle_sistema_meta_datos As Integer
    Public Property id_detalle_sistema_meta_datos() As Integer
        Get
            Return m_id_detalle_sistema_meta_datos
        End Get
        Set(value As Integer)
            m_id_detalle_sistema_meta_datos = value
        End Set
    End Property
    Private m_ID_M_AUTO_TIP_EVENT As Integer
    Public Property ID_M_AUTO_TIP_EVENT() As Integer
        Get
            Return m_ID_M_AUTO_TIP_EVENT
        End Get
        Set(value As Integer)
            m_ID_M_AUTO_TIP_EVENT = value
        End Set
    End Property
    Private m_NOMBRE_TABLA_EVENTO As String
    Public Property NOMBRE_TABLA_EVENTO() As String
        Get
            Return m_NOMBRE_TABLA_EVENTO
        End Get
        Set(value As String)
            m_NOMBRE_TABLA_EVENTO = value
        End Set
    End Property
    Private m_NOMBRE_CAMPO_PARAMETRO As String
    Public Property NOMBRE_CAMPO_PARAMETRO() As String
        Get
            Return m_NOMBRE_CAMPO_PARAMETRO
        End Get
        Set(value As String)
            m_NOMBRE_CAMPO_PARAMETRO = value
        End Set
    End Property
    Private m_NOMBRE_CAMPO_RETORNO As String
    Public Property NOMBRE_CAMPO_RETORNO() As String
        Get
            Return m_NOMBRE_CAMPO_RETORNO
        End Get
        Set(value As String)
            m_NOMBRE_CAMPO_RETORNO = value
        End Set
    End Property
    Private m_TIPO_CAMPO_PARAMENTO As String
    Public Property TIPO_CAMPO_PARAMENTO() As String
        Get
            Return m_TIPO_CAMPO_PARAMENTO
        End Get
        Set(value As String)
            m_TIPO_CAMPO_PARAMENTO = value
        End Set
    End Property
    Private m_LONGITUD_CAMPO_PARAMENTO As String
    Public Property LONGITUD_CAMPO_PARAMENTO() As String
        Get
            Return m_LONGITUD_CAMPO_PARAMENTO
        End Get
        Set(value As String)
            m_LONGITUD_CAMPO_PARAMENTO = value
        End Set
    End Property
    Private m_TIPO_CAMPO_RETORNO As String
    Public Property TIPO_CAMPO_RETORNO() As String
        Get
            Return m_TIPO_CAMPO_RETORNO
        End Get
        Set(value As String)
            m_TIPO_CAMPO_RETORNO = value
        End Set
    End Property
    Private m_LONGITUD_CAMPO_RETORNO As String
    Public Property LONGITUD_CAMPO_RETORNO() As String
        Get
            Return m_LONGITUD_CAMPO_RETORNO
        End Get
        Set(value As String)
            m_LONGITUD_CAMPO_RETORNO = value
        End Set
    End Property
    Function Solicita_detalle_evento_auto_poblado(ByVal id_detalle_meta_dato_auto_poblado As Integer,
                                         ByRef class_ra_m_auto_evento As Class_ra_m_auto_evento) As String
        '---------------------------------------------------------
        'Funcion : Solicita detalles evento de auto poblado de un
        'meta dato, con el parametro del item del meta dato
        'Fecha : 2022-02-13
        'ing . Miguel Angel Urueta Miranda
        '----------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "SELECT ID_M_AUTO_EVENTO," &
            "ra_m_detalle_sis_meta_datos_id_detalle_sistema_meta_datos," &
            "RA_M_AUTO_TIP_EVENTO_ID_M_AUTO_TIP_EVENT,NOMBRE_TABLA_EVENTO,NOMBRE_CAMPO_PARAMETRO,NOMBRE_CAMPO_RETORNO," &
            "TIPO_CAMPO_PARAMENTO,LONGITUD_CAMPO_PARAMENTO,TIPO_CAMPO_RETORNO,LONGITUD_CAMPO_RETORNO" &
            " FROM ra_m_auto_evento  " &
            " where ra_m_detalle_sis_meta_datos_id_detalle_sistema_meta_datos=" & id_detalle_meta_dato_auto_poblado
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_m_detalle_sis_meta_datos")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_detalle_evento_auto_poblado = "Función Solicita_detalle_evento_auto_poblado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                class_ra_m_auto_evento = New Class_ra_m_auto_evento
                class_ra_m_auto_evento.ID_M_AUTO_EVENTO = Datset.Tables(0).Rows(0).Item(0)
                class_ra_m_auto_evento.id_detalle_sistema_meta_datos = Datset.Tables(0).Rows(0).Item(1)
                class_ra_m_auto_evento.ID_M_AUTO_TIP_EVENT = Datset.Tables(0).Rows(0).Item(2)
                class_ra_m_auto_evento.NOMBRE_TABLA_EVENTO = Datset.Tables(0).Rows(0).Item(3)
                class_ra_m_auto_evento.NOMBRE_CAMPO_PARAMETRO = Datset.Tables(0).Rows(0).Item(4)
                class_ra_m_auto_evento.NOMBRE_CAMPO_RETORNO = Datset.Tables(0).Rows(0).Item(5)
                class_ra_m_auto_evento.TIPO_CAMPO_PARAMENTO = Datset.Tables(0).Rows(0).Item(6)
                class_ra_m_auto_evento.LONGITUD_CAMPO_PARAMENTO = Datset.Tables(0).Rows(0).Item(7)
                class_ra_m_auto_evento.TIPO_CAMPO_RETORNO = Datset.Tables(0).Rows(0).Item(8)
                class_ra_m_auto_evento.LONGITUD_CAMPO_RETORNO = Datset.Tables(0).Rows(0).Item(9)
                Solicita_detalle_evento_auto_poblado = "YES"
                Exit Function
            Else
                Solicita_detalle_evento_auto_poblado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_detalle_evento_auto_poblado = "Inconsistencia general funcion Solicita_detalle_evento_auto_poblado " & ex.Message
        End Try
    End Function
    Function Solicita_valor_meta_dato_auto_poblado(ByVal Class_ra_m_auto_tip_evento As Class_ra_m_auto_tip_evento,
                                                   ByVal Class_ra_m_auto_evento_ As Class_ra_m_auto_evento,
                                                   ByVal valor_parametro As Object,
                                                   ByRef valor_auto_poblado As String) As String
        Try
            Dim Sql_consulta As String = "Select " & Class_ra_m_auto_evento_.NOMBRE_CAMPO_RETORNO & " From " & Class_ra_m_auto_evento_.NOMBRE_TABLA_EVENTO & " where " & Class_ra_m_auto_evento_.NOMBRE_CAMPO_PARAMETRO & "=" &
                "'" & valor_parametro & "'"
            Dim Ref_Car_Conec_da As New conect.Dbase_Conction_Mysql_DA
            Dim Ref_Car_Conec_ra As New conect.Dbase_Conction_Mysql_RA
            Dim Ref_Car_Conec_wf As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ra_m_auto_tip_evento")
            Dim Result As String = ""
            Dim Ref_Car_Conec As Object = Nothing
            If Class_ra_m_auto_tip_evento.TIP_AUTO_TIP_EVENTO = 1 Then
                Result = Ref_Car_Conec_ra.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            End If
            If Class_ra_m_auto_tip_evento.TIP_AUTO_TIP_EVENTO = 2 Then
                Result = Ref_Car_Conec_wf.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            End If
            If Class_ra_m_auto_tip_evento.TIP_AUTO_TIP_EVENTO = 3 Then
                Result = Ref_Car_Conec_ra.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            End If
            If Class_ra_m_auto_tip_evento.TIP_AUTO_TIP_EVENTO = 4 Then
                Result = Ref_Car_Conec_ra.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            End If
            If Result <> "YES" Then
                Solicita_valor_meta_dato_auto_poblado = "Functión Solicita_valor_meta_dato_auto_poblado dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    valor_auto_poblado = ""
                Else
                    valor_auto_poblado = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_valor_meta_dato_auto_poblado = "YES"
                Exit Function
            Else
                valor_auto_poblado = ""
                Solicita_valor_meta_dato_auto_poblado = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_valor_meta_dato_auto_poblado = "Inconsistencia general funcion Solicita_valor_meta_dato_auto_poblado " & ex.Message
        End Try
    End Function
End Class
