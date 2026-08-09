Public Class class_wf_tipo_gestion_stru
    Property error_result As String
    Property id_tipo_gestion As Integer
    Property nombre_gestion As String
    Property estdo_lista As Integer
    Property estado_envio_correo As Integer
End Class
Public Class Class_wf_tipo_gestion
    Function Solicita_lista_tipo_gestion(ByRef control_drow_lista As List(Of control_drow_lista)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita los gabinetes permitidos para migración
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id                               : Opcional
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'control_drow_lista        : Retorna la lista de gabinetes 
        '                     value: identificación del gabinete
        '                      text: Nombre del gabinete  
        'error_sistema             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-05-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  id_tipo_gestion,nombre_gestion  from  wf_gestion_tipos where estdo_lista=1 order by nombre_gestion"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_gestion_tipos")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_tipo_gestion = " Función Solicita_lista_tipo_gestion dice " & Result
                Exit Function
            End If
            Dim item As control_drow_lista
            If Datset.Tables(0).Rows.Count > 0 Then
                item = New control_drow_lista
                item.value = "0"
                item.text = "Seleccione tipo gestión"
                control_drow_lista.Add(item)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New control_drow_lista
                    item.value = Datset.Tables(0).Rows(i).Item(0)
                    item.text = Datset.Tables(0).Rows(i).Item(1)
                    control_drow_lista.Add(item)
                Next
                Solicita_lista_tipo_gestion = "YES"
                Exit Function
            Else
                Solicita_lista_tipo_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_tipo_gestion = "Inconsistencia general función Solicita_lista_tipo_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_estado_envio_correo_gestion_usuario(ByVal Id_tipo_gestion As Integer,
                                                          ByRef Estado_envio_correo As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el estado de envio de correo del tipo de gestión de
        '          usuario
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Id_tipo_gestion           : Representa la identificación del tipo de gestión
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'Estado_envio_correo        : Retorna el estado de envio de correo electronico
        '                     
        '                      
        '
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-09-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim SQL_consulta As String = "select  estado_envio_correo  from  wf_gestion_tipos where Id_tipo_gestion=" & Id_tipo_gestion
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_gestion_tipos")
            Dim Result = ref.SELECTION_SELECT_FIELD(SQL_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_envio_correo_gestion_usuario = "Función Solicita_estado_envio_correo_gestion_usuario dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Estado_envio_correo = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_envio_correo_gestion_usuario = "YES"
                Exit Function
            Else
                Solicita_estado_envio_correo_gestion_usuario = "Imposible encontrar el estado de envio de correo del identificador (" & Id_tipo_gestion & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_envio_correo_gestion_usuario = "Inconsistencia general funcion Solicita_estado_envio_correo_gestion_usuario " & ex.Message
        End Try
    End Function
End Class
