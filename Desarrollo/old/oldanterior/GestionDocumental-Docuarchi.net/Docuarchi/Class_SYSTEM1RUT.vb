Public Class Class_SYSTEM1RUT
    Function Consulta_Ruta_Almacenamiento(ByRef _Ruta_Almacenamiento As String,
                                          ByVal _Nombre_Gabienete As String) As String
        '**********************************************
        'Function : consulta la ruta de almacenamiento
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2010-09-01
        'Modificada 2013-09-08 ing Miguel Angel Urueta
        'para cumlplir con los parametros de conexion
        'del sistema web
        '**********************************************

        Try
            Dim Sql_consulta = "SELECT ruta_gabi FROM " &
                 "SYSTEM1RUT " &
                 "WHERE gabinete='" & _Nombre_Gabienete & "' and tipo_rut=1 and Est_rut=1"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("SYSTEM1RUT")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If result <> "YES" Then
                Consulta_Ruta_Almacenamiento = "Error Consultando en tabla 35 " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Consulta_Ruta_Almacenamiento = "Error 1- Funcion Consulta_Ruta_Almacenamiento: No se encontraron datos del gabinete " _
               & _Nombre_Gabienete & " en la tabla system1"
                Exit Function
            Else

                Dim Tempvalor As Object = Datset.Tables(0).Rows(0).Item(0)
                If IsDBNull(Tempvalor) Then
                    Consulta_Ruta_Almacenamiento = "Error 1- consultando ruta busqueda campo null en la tabla SYSTEM1RUT"
                    Exit Function
                Else
                    _Ruta_Almacenamiento = Tempvalor.ToString
                    _Ruta_Almacenamiento = _Ruta_Almacenamiento.Replace("/", "\")
                    Consulta_Ruta_Almacenamiento = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Consulta_Ruta_Almacenamiento = "Error 1- General consultando tabla SYSTEM1RUT " & ex.Message
        End Try

    End Function
    Function Consulta_Ruta_Busqueda_Webservice(ByRef _Ruta_Almacenamiento As String,
                                               ByVal _Nombre_Gabienete As String) As String
        '****************************************************
        'Funcion : Consulta la ruta de busqueda del gabienete
        'cuando esta activo el servicio web service
        'Ing Miguel Angel Urueta Miranda
        'Fecha : 2013-03-20
        'Modficado 2013-05-20 Ing Miguel Angel Urueta Miranda
        'se cambia el modo de conexion de base de datos
        '****************************************************
        Try

            Dim Sql_consulta = "SELECT ruta_gabi FROM " &
                 "SYSTEM1rut " &
                 "WHERE GABINETE='" & _Nombre_Gabienete & "' AND TIPO_RUT=1"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DATOS_TAREA")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_Ruta_Busqueda_Webservice = "Funcion  Consulta_Ruta_Busqueda_Webservice WF-01 Mensaje DBMS" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Consulta_Ruta_Busqueda_Webservice = "Funcion Consulta_Ruta_Busqueda_Webservice WF-02 Imposible ruta webserice   del gabiente " & _Nombre_Gabienete
                Exit Function
            End If
            Dim Tempvalor As Object = Datset.Tables(0).Rows(0).Item(0)
            If IsDBNull(Tempvalor) Then
                Consulta_Ruta_Busqueda_Webservice = "Funcion Consulta_Ruta_Busqueda_Webservice WF-03 Imposible ruta webserice es null " & _Nombre_Gabienete
                Exit Function
            Else
                _Ruta_Almacenamiento = Tempvalor
                _Ruta_Almacenamiento = _Ruta_Almacenamiento.Replace("/", "\")
                Consulta_Ruta_Busqueda_Webservice = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_Ruta_Busqueda_Webservice = "Inconsistencia general funcion Consulta_Ruta_Busqueda_Webservice " & ex.Message
        End Try

    End Function
End Class
