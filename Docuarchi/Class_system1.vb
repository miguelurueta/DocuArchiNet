Imports GestionDocumental_Docuarchi.net.WebServiceRadicacion
Public Structure stru_datos_configuracion_gabinete
    Dim disco As Integer
    Dim proxid As Integer
    Dim tamdisc As Object
    Dim numcarp As Integer
    Dim numpag_carp As Integer
End Structure
Public Class Class_system1_detalle_gabinete
    Property IDENTI As String
    Property DISCO As Integer
    Property PROXID As Integer
    Property NOMBRE As String
    Property FECHA As String
    Property PAG As Integer
    Property TAMDISC As Integer
    Property RUTBUSCA As String
    Property RUTALMA As String
    Property TIPOGABINETE As String
    Property NUMPAG_CARP As Integer
    Property INVENTARIO_DOCUMENTAL As Integer
    Property APLICA_TRD As Integer
    Property ASIGNA_UNIDAD As Integer
    Property id_gabinete As Integer
    Property UTIL_MIGRA_DOCUMENTO As Integer
    Property Error_result As String
End Class

Public Class Class_system1
    Function Solicita_estructura_configuracion_gabinete(ByVal nombre_gabinete As String,
                                                        ByRef Class_system1_detalle_gabinete As Class_system1_detalle_gabinete) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita_estructura_configuracion_gabinete
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'nombre_gabinete           : Respresenta la identificación del gabinete
        '                        
        '
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_system1_detalle_gabinete : Retorna la clase de configuracion
        '                                    de gabinete
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-27
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim ParametroSQL As String = "SELECT IDENTI,DISCO,PROXID,NOMBRE,FECHA,PAG,TAMDISC,RUTBUSCA,RUTALMA," &
                "TIPOGABINETE,NUMPAG_CARP,INVENTARIO_DOCUMENTAL,APLICA_TRD,ASIGNA_UNIDAD,id_gabinete,UTIL_MIGRA_DOCUMENTO " &
            " FROM system1 where NOMBRE='" & nombre_gabinete & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system1")
            Dim Result = ref2.SELECTION_SELECT_FIELD(ParametroSQL, Datset)
            If Result <> "YES" Then
                Solicita_estructura_configuracion_gabinete = "Error de conexion Funcion  Solicita_estructura_configuracion_gabinete dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_configuracion_gabinete = "Impsible encontrar los datos de configuración del gabinete (" & nombre_gabinete & ")"
                Exit Function
            Else
                'Class_system1_detalle_gabinete.IDENTI = Datset.Tables(0).Rows(0).Item("IDENTI")
                'Class_system1_detalle_gabinete.DISCO = Datset.Tables(0).Rows(0).Item("DISCO")
                'Class_system1_detalle_gabinete.PROXID = Datset.Tables(0).Rows(0).Item("PROXID")
                'Class_system1_detalle_gabinete.NOMBRE = Datset.Tables(0).Rows(0).Item("NOMBRE")
                'Class_system1_detalle_gabinete.FECHA = Datset.Tables(0).Rows(0).Item("FECHA")
                'Class_system1_detalle_gabinete.PAG = Datset.Tables(0).Rows(0).Item("PAG")
                'Class_system1_detalle_gabinete.TAMDISC = Datset.Tables(0).Rows(0).Item("TAMDISC")
                'Class_system1_detalle_gabinete.RUTBUSCA = Datset.Tables(0).Rows(0).Item("RUTBUSCA")
                'Class_system1_detalle_gabinete.RUTALMA = Datset.Tables(0).Rows(0).Item("RUTALMA")
                'Class_system1_detalle_gabinete.TIPOGABINETE = Datset.Tables(0).Rows(0).Item("TIPOGABINETE")
                'Class_system1_detalle_gabinete.NUMPAG_CARP = Datset.Tables(0).Rows(0).Item("NUMPAG_CARP")
                'Class_system1_detalle_gabinete.INVENTARIO_DOCUMENTAL = Datset.Tables(0).Rows(0).Item("INVENTARIO_DOCUMENTAL")
                'Class_system1_detalle_gabinete.APLICA_TRD = Datset.Tables(0).Rows(0).Item("APLICA_TRD")
                'Class_system1_detalle_gabinete.ASIGNA_UNIDAD = Datset.Tables(0).Rows(0).Item("ASIGNA_UNIDAD")
                Class_system1_detalle_gabinete.id_gabinete = Datset.Tables(0).Rows(0).Item("id_gabinete")
                Class_system1_detalle_gabinete.UTIL_MIGRA_DOCUMENTO = Datset.Tables(0).Rows(0).Item("UTIL_MIGRA_DOCUMENTO")
                Solicita_estructura_configuracion_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_configuracion_gabinete = "Inconsitencia general funcion Solicita_estructura_configuracion_gabinete " & ex.Message
        End Try
    End Function



    Function Solicita_datos_configuracion_gabinete(ByVal id_gabinete As Integer,
                                                   ByRef Stru_datos_configuracion_gabinete As stru_datos_configuracion_gabinete) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita estructura de configuración de gabinete
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Respresenta la identificación del gabinete
        '                        
        '
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Stru_datos_configuracion_gabinete : Retorna la estructura de configuracion
        '                                    de gabinete
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-03
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim ParametroSQL As String = " SELECT disco,proxid,tamdisc,numcarp,NUMPAG_CARP FROM system1 where id_gabinete=" & id_gabinete
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system1")
            Dim Result = ref2.SELECTION_SELECT_FIELD(ParametroSQL, Datset)
            If Result <> "YES" Then
                Solicita_datos_configuracion_gabinete = "Error de conexion Funcion  Solicita_datos_configuracion_gabinete dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_configuracion_gabinete = "Impsible encontrar los datos de configuración del gabinete (" & id_gabinete & ")"
                Exit Function
            Else
                Stru_datos_configuracion_gabinete.disco = Datset.Tables(0).Rows(0).Item(0)
                Stru_datos_configuracion_gabinete.proxid = Datset.Tables(0).Rows(0).Item(1)
                Stru_datos_configuracion_gabinete.tamdisc = Datset.Tables(0).Rows(0).Item(2)
                Stru_datos_configuracion_gabinete.numcarp = Datset.Tables(0).Rows(0).Item(3)
                Stru_datos_configuracion_gabinete.numpag_carp = Datset.Tables(0).Rows(0).Item(4)
                Solicita_datos_configuracion_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_configuracion_gabinete = "Inconsistencia general funcion Solicita_datos_configuracion_gabinete " & ex.Message
        End Try
    End Function
    Function Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(ByVal nombre_gabinete As String,
                                                                               ByRef inventario_documental As Integer,
                                                                               ByRef aplica_trd As Integer,
                                                                               ByRef asigna_unidad As Integer) As String
        Try
            Dim Parametro_Consulta = "select INVENTARIO_DOCUMENTAL,APLICA_TRD,ASIGNA_UNIDAD " &
            " from system1 where NOMBRE='" & nombre_gabinete & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system1")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete = "Funcion  Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete = "Impsible encontrar los datos de configuración del gabinete " & nombre_gabinete
                Exit Try
            Else
                inventario_documental = Datset.Tables(0).Rows(0).Item(0)
                aplica_trd = Datset.Tables(0).Rows(0).Item(1)
                asigna_unidad = Datset.Tables(0).Rows(0).Item(2)
                Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete = "Inconsistencia general función Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete " & ex.Message
        End Try
    End Function
    Function VerificaOpcionAplicarInventarioDocumental(ByRef OptInventarioDocumental As Integer,
                                                       ByVal NombreGabinete As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Verifica la opción de aplicar tablas de retención para un gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreGabinete      : Representa el nombre del gabinete
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'OptInventarioDocumental  : Retorna la opcion de inventario documental
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2015-02-10
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            OptInventarioDocumental = 0
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "Select INVENTARIO_DOCUMENTAL from system1 where NOMBRE='" & NombreGabinete & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("DATOS_GABINETE")
            Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                VerificaOpcionAplicarInventarioDocumental = "Funcion  VerificaOpcionAplicarTablaRetencion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                VerificaOpcionAplicarInventarioDocumental = "No se encontró la opción de aplicación de tablas de retención asociada al gabinete (" & NombreGabinete & ")"
                Exit Function
            Else
                OptInventarioDocumental = Datset.Tables(0).Rows(0).Item(0)
                VerificaOpcionAplicarInventarioDocumental = "YES"
                Exit Function
            End If

        Catch ex As Exception
            VerificaOpcionAplicarInventarioDocumental = "Inconsistencia general funcion VerificaOpcionAplicarTablaRetencion " & ex.Message
        End Try
    End Function
    Function VerificaOpcionAplicarTablaRetencion(ByRef opt_tabla_retencion As Integer,
                                                    ByVal nombre_gabinete As String) As String
        '*********************************************************
        'Funcion : retorna la opcion aplica tabla de retencion
        'ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-12-26
        '*********************************************************
        Try
            opt_tabla_retencion = 0
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "Select APLICA_TRD from system1 where NOMBRE='" & nombre_gabinete & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system1")
            Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                VerificaOpcionAplicarTablaRetencion = "Funcion  Verifica_opcion_aplicar_tabla_retencion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                VerificaOpcionAplicarTablaRetencion = "Imposible encontrar gabinete para opcion trd"
                Exit Function
            Else
                opt_tabla_retencion = Datset.Tables(0).Rows(0).Item(0)
                VerificaOpcionAplicarTablaRetencion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            VerificaOpcionAplicarTablaRetencion = "Inconsistencia general funcion Verifica_opcion_aplicar_tabla_retencion " & ex.Message
        End Try
    End Function
    Function Verfica_opcion_seleccion_unidad(ByRef opt_seleccion_unidad As Integer,
                                             ByVal nombre_gabinete As String) As String
        '*********************************************************
        'Funcion : Retorna la opcion aplica seleccion unidad
        'ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-12-29
        '*********************************************************
        Try
            opt_seleccion_unidad = 0
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "Select ASIGNA_UNIDAD from system1 where NOMBRE='" & nombre_gabinete & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system1")
            Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verfica_opcion_seleccion_unidad = "Funcion  Verfica_opcion_seleccion_unidad dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Verfica_opcion_seleccion_unidad = "Imposible encontrar gabinete para opcion trd unidad de conservación"
                Exit Function
            Else
                opt_seleccion_unidad = Datset.Tables(0).Rows(0).Item(0)
                Verfica_opcion_seleccion_unidad = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verfica_opcion_seleccion_unidad = "Inconsistencia general funcion Verfica_opcion_seleccion_unidad " & ex.Message
        End Try
    End Function
    Function SolicitaIdGabineteDocuarchi(ByVal NombreGabinete As String,
                                         ByRef IdGabineteDocuarchi As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la identificiación del gabinete con el nombre del gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'nombreGabinete      : Representa el nombre del gabinete
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdGabineteDocuarchi  : Retorna la idnetificación del gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  id_gabinete  from   system1 " &
                  " where NOMBRE='" & NombreGabinete & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(NombreGabinete)
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaIdGabineteDocuarchi = " La función SolictaIdGabineteDocuarchi : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdGabineteDocuarchi = "Imposible encontrar la identificación del gabinete con el nombre del gabinete (" & NombreGabinete & ")"
                Exit Function
            Else
                IdGabineteDocuarchi = Datset.Tables(0).Rows(0).Item(0)
                SolicitaIdGabineteDocuarchi = "YES"
            End If
        Catch ex As Exception
            SolicitaIdGabineteDocuarchi = "Inconsistencia general función SolictaIdGabineteDocuarchi " & ex.Message
        End Try
    End Function
    Function SolicitaNombreGabinetePorId(ByVal IdGabinete As Integer,
                                         ByRef Gabinete As String) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita el nombre del gabinete con la identiifcación del gabinete
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_gabinete                : Representa la identificación del gabinete
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        'gabinete                   : Representa el nombre del gabinete
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2024-05-21
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  NOMBRE  from   system1 " &
                 " where id_gabinete=" & IdGabinete
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("GABINETE")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Return " La función SolicitaNombreGabinetePorId dice : " & Result
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Return "Imposible encontrar el nombre del gabinete con la identificación (" & IdGabinete & ")"
            Else
                Gabinete = Datset.Tables(0).Rows(0).Item(0)
                Return "YES"
            End If
        Catch ex As Exception
            Return "Inconsistencia general funcion SolicitaNombreGabinetePorId " & ex.Message
        End Try
    End Function
    Function Solicita_disco_carpeta_gabinete(ByVal gabinete As String,
                                             ByRef disco As Integer,
                                             ByRef carpeta As Integer) As String
        '-----------------------------------------------
        'Función : Retorna el disco y la carpeta actual
        'del gabinete informado
        'Ing . Miguel Angel Urueta Miranda
        'Fecha : 2022-03-15
        '-----------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  DISCO,NUMCARP  from   system1 " &
                  " where NOMBRE='" & gabinete & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(gabinete)
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_disco_carpeta_gabinete = " La función Solicita_disco_carpeta_gabinete dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_disco_carpeta_gabinete = "Imposible encontrar el id del gabinete (" & gabinete & ")"
                Exit Function
            Else
                disco = Datset.Tables(0).Rows(0).Item(0)
                carpeta = Datset.Tables(0).Rows(0).Item(1)
                Solicita_disco_carpeta_gabinete = "YES"
            End If
        Catch ex As Exception
            Solicita_disco_carpeta_gabinete = "Inconsistencia general funcion Solicita_disco_carpeta_gabinete " & ex.Message
        End Try
    End Function
    Function Retorna_gabinetes_disponibles(ByRef Matri_Datos() As String) As String
        '--------------------------------------------------------------
        'Funcion : Solicita los datos de los gabinetes disponibles
        'para el sistema workflow
        'Fecha : 2017-07-19
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  NOMBRE  from  system1 "
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("system1")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_gabinetes_disponibles = " La funcion Retorna_gabinetes_disponibles dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_gabinetes_disponibles = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos(i)
                    Matri_Datos(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Retorna_gabinetes_disponibles = "YES"
            End If

        Catch ex As Exception
            Retorna_gabinetes_disponibles = "Inconsistencia función  Retorna_gabinetes_disponibles " & ex.Message
        End Try
    End Function
    Function Service_lista_gabinetes_net(ByRef control_drow_lista As List(Of control_drow_lista)) As String
        Try
            Dim Parametro_Consulta As String = "select  id_gabinete,NOMBRE  from  system1 "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system1")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Service_lista_gabinetes_net = " Función Service_lista_gabinetes_net dice " & Result
                Exit Function
            End If
            Dim item As control_drow_lista
            If Datset.Tables(0).Rows.Count > 0 Then
                item = New control_drow_lista
                item.value = "0"
                item.text = ""
                control_drow_lista.Add(item)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New control_drow_lista
                    item.value = Datset.Tables(0).Rows(i).Item(0)
                    item.text = Datset.Tables(0).Rows(i).Item(1)
                    control_drow_lista.Add(item)
                Next
                Service_lista_gabinetes_net = "YES"
                Exit Function
            Else
                Service_lista_gabinetes_net = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Service_lista_gabinetes_net = "Inconsistencia general función Service_lista_gabinetes_net " & ex.Message
        End Try

    End Function
    Function Solicita_gabinetes_migracion(ByRef control_drow_lista As List(Of control_drow_lista)) As String
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
            Dim Parametro_Consulta As String = "select  id_gabinete,NOMBRE  from  system1 where UTIL_MIGRA_DOCUMENTO=1 order by NOMBRE"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system1")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_gabinetes_migracion = " Función Solicita_gabinetes_migracion dice " & Result
                Exit Function
            End If
            Dim item As control_drow_lista
            If Datset.Tables(0).Rows.Count > 0 Then
                item = New control_drow_lista
                item.value = "0"
                item.text = "SELECCIONE GABINETE"
                control_drow_lista.Add(item)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New control_drow_lista
                    item.value = Datset.Tables(0).Rows(i).Item(0)
                    item.text = Datset.Tables(0).Rows(i).Item(1)
                    control_drow_lista.Add(item)
                Next
                Solicita_gabinetes_migracion = "YES"
                Exit Function
            Else
                Solicita_gabinetes_migracion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_gabinetes_migracion = "Inconsistencia general función Solicita_gabinetes_migracion " & ex.Message
        End Try
    End Function
End Class
