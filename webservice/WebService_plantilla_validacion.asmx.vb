Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports GestionDocumental_Docuarchi.net.conect
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports Newtonsoft.Json
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Web.Script.Serialization
Imports System.Threading

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebService_plantilla_validacion
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_update_tercero_plantilla_validacion(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que actualiza un  tercero
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'parameter              : Representa la estructura de los de los datos
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-01-03
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try
            Dim Result As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Dim deserialize_parameter = Nothing
            Dim id_escript As Integer = Val(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.error_gestion = "Imposible deserializar los datos del formulario"
                resultList.Add(parameter_gestion)
                Return resultList
                Exit Function
            End If
            If HttpContext.Current.Session.Item("RA_VALIDACION_EDITAR") = "0" Then
                deserialize_parameter(0).error_gestion = "El usuario no tiene permisos para editar "
                Return deserialize_parameter
                Exit Function
            End If
            Result = Class_plantilla_validacion.Update_tercero_plantilla_validacion(id_escript,
                                                                                    deserialize_parameter)
            If Result <> "YES" Then
                deserialize_parameter(0).error_gestion = Result
                Return deserialize_parameter
                Exit Function
            Else
                deserialize_parameter(0).error_gestion = Result
                Return deserialize_parameter
                Exit Function
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_delete_tercero_plantilla_validacion(ByVal parameter As Object) As IEnumerable(Of class_service_operatio_control)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que registra un nuevo tercero
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'parameter                  : Representa la identiifcación del tercero
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-01-04
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_service_operatio_control)()
        Dim parameter_gestion As class_service_operatio_control = New class_service_operatio_control()
        Try
            Dim Result As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Dim id_escript As Integer = Val(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
            If HttpContext.Current.Session.Item("RA_VALIDACION_ELIMINAR") = "0" Then
                parameter_gestion.error_gestion = "El usuario no tiene permisos para eliminar regitros"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Result = Class_plantilla_validacion.Delete_tercero_pantilla_validacion(parameter,
                                                                                   id_escript)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.error_gestion = Result
                parameter_gestion.dms_id_registro = parameter
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_registra_tercero_plantilla_validacion(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que registra un nuevo tercero
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'parameter                        : Representa la estructura de los de los datos
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2023-12-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try
            Dim Result As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Dim deserialize_parameter = Nothing
            Dim id_escript As Integer = Val(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.error_gestion = "Imposible deserializar los datos del formulario"
                resultList.Add(parameter_gestion)
                Return resultList
                Exit Function
            End If
            If HttpContext.Current.Session.Item("RA_VALIDACION_AGREGAR") = "0" Then
                deserialize_parameter(0).error_gestion = "El usuario no tiene permisos para agregar registros"
                Return deserialize_parameter
                Exit Function
            End If
            Result = Class_plantilla_validacion.Registra_tercero_plantilla_validacion(id_escript,
                                                                                      deserialize_parameter)
            If Result <> "YES" Then
                deserialize_parameter(0).error_gestion = Result
                Return deserialize_parameter
                Exit Function
            Else
                deserialize_parameter(0).error_gestion = Result
                Return deserialize_parameter
                Exit Function
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_estructura_formulario_registro_validacion_externo(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)
        Dim resultList = New List(Of Class_config_general_service)()
        Dim resultList_error = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura del los externos o terceros peticionarios
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter      : Representa el identificador de la plantilla
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_config_general_service : Retorna la estructura de los campos de los
        '                               terceros de radicación
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-10-30
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Class_campos_plantilla_validacion As New Class_campos_plantilla_validacion
            Dim Result As String = ""
            Dim id_script_plantilla As Integer = Val(Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION"))
            Dim Nombre_plantilla_validacion As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Retorna_Nombre_Plantilla_Validacion(Val(id_script_plantilla),
                                                                                    Nombre_plantilla_validacion)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList_error.Add(parameter_gestion)
                Return resultList_error
            End If
            Dim id_plantilla As Integer = 0
            Result = Class_plantilla_validacion.Retorna_id_Plantilla_Validacion_id_script(id_script_plantilla,
                                                                                          id_plantilla)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList_error.Add(parameter_gestion)
                Return resultList_error
            End If
            Result = Class_plantilla_validacion.Solicita_estructura_plantilla_validacion_externos(Val(id_script_plantilla),
                                                                                                  Nombre_plantilla_validacion,
                                                                                                  "registro_validacion_externo",
                                                                                                  resultList)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList_error.Add(parameter_gestion)
                Return resultList_error
            End If
            Dim campo_primary As String = ""
            Result = Class_campos_plantilla_validacion.Retorna_Campo_Primary_key_plantilla_validacion(id_plantilla,
                                                                                                      campo_primary)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList_error.Add(parameter_gestion)
                Return resultList_error
            End If
            Dim Class_config_general_service As New Class_config_general_service
            '-------Asigna datos a la interface
            If parameter <> 0 Then
                Result = Class_plantilla_validacion.Asigna_datos_campos_plantilla_validacion_externos(parameter,
                                                                                                      Nombre_plantilla_validacion,
                                                                                                      campo_primary,
                                                                                                      resultList)
                If Result <> "YES" Then
                    parameter_gestion.error_gestion = Result
                    resultList_error.Add(parameter_gestion)
                    Return resultList_error
                End If
                For i As Integer = 0 To resultList.Count - 1
                    If resultList.Item(i).campo_tip = 0 Then
                        If Not resultList.Item(i).config_service_drowlis_destino Is Nothing Then
                            '-----------------------------asigna valor clave campo destino------------------------------------
                            For z As Integer = 0 To resultList.Count - 1
                                If resultList.Item(i).name_campo = resultList.Item(z).drow_name_controls_destino Then
                                    resultList.Item(i).config_service_drowlis_destino.Item(0).value_condicion = resultList.Item(z).config_service_drowlis_destino.Item(0).value_condicion
                                End If
                            Next
                            Dim Class_service_ilist_drowlist = New List(Of Class_config_general_service.Class_service_ilist_drowlist)()
                            resultList.Item(i).config_service_drowlis_destino(0).value_default = resultList.Item(i).texto_campo
                            Result = Class_config_general_service.Solicita_datos_drowlist_form_control(resultList.Item(i),
                                                                                                       Class_service_ilist_drowlist)
                            If Result <> "YES" Then
                                parameter_gestion.error_gestion = Result
                                resultList_error.Add(parameter_gestion)
                                Return resultList_error
                                Exit For
                            Else
                                resultList.Item(i).ilist_row_drowlist = Class_service_ilist_drowlist
                            End If
                        End If
                    End If
                Next
            Else
                '-------Lista los registros de cada campo tipo drowplist  sin asignacion de datos al formulario
                For i As Integer = 0 To resultList.Count - 1
                    If resultList.Item(i).campo_tip = 0 Then
                        If Not resultList.Item(i).config_service_drowlis_destino Is Nothing Then
                            If resultList.Item(i).config_service_drowlis_destino(0).campo_estado_auto_lista <> 0 Then
                                Dim Class_service_ilist_drowlist = New List(Of Class_config_general_service.Class_service_ilist_drowlist)()
                                resultList.Item(i).config_service_drowlis_destino(0).value_default = resultList.Item(i).texto_campo
                                Result = Class_config_general_service.Solicita_datos_drowlist_form_control(resultList.Item(i),
                                                                                                           Class_service_ilist_drowlist)
                                If Result <> "YES" Then
                                    parameter_gestion.error_gestion = Result
                                    resultList_error.Add(parameter_gestion)
                                    Return resultList_error
                                    Exit For
                                Else
                                    resultList.Item(i).ilist_row_drowlist = Class_service_ilist_drowlist
                                End If
                            End If
                        End If
                    End If
                Next
            End If
            resultList.Item(0).name_campo_id = campo_primary
            resultList.Item(0).tbl_control = Nombre_plantilla_validacion
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList_error.Add(parameter_gestion)
            Return resultList_error
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_solicita_estructura_formulario_registro_validacion_externo_id(ByVal parameter As Object) As IEnumerable(Of Class_config_general_service)

        '---------------------------------------------------------------------------
        'Funcion : Expone la estructura del los externos o terceros peticionarios
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'parameter      : Representa la estructura con los parmetros de la interfaz
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_config_general_service : Retorna la estructura de los campos de los
        '                               terceros o solicitantes
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-21
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of Class_config_general_service)()
        Dim resultList_error = New List(Of Class_config_general_service)()
        Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
        Try
            Dim Result As String = ""
            Dim deserialize_parameter As New List(Of class_config_general_parmeter_interface_show)
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of class_config_general_parmeter_interface_show))(parameter)
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Result = Class_plantilla_validacion.Solicita_estructura_formulario_registro_validacion_externo(deserialize_parameter(0).id_script,
                                                                                                           deserialize_parameter(0).id_registro,
                                                                                                           deserialize_parameter(0).name_space_campo,
                                                                                                           resultList)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList_error.Add(parameter_gestion)
                Return resultList_error
            Else
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList_error.Add(parameter_gestion)
            Return resultList_error
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_registra_tercero_plantilla_validacion_simplificada(ByVal parameter As Object, ByVal id_script As Object) As IEnumerable(Of class_config_gneral_service_row_tom)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que registra un nuevo tercero
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'parameter                        : Representa la estructura de los de los datos
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2023-12-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_config_gneral_service_row_tom)()
        Dim parameter_gestion As class_config_gneral_service_row_tom = New class_config_gneral_service_row_tom()
        Try
            Dim Result As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Dim deserialize_parameter As New List(Of Class_config_general_service)
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.error_gestion = "Imposible deserializar los datos del formulario"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            If HttpContext.Current.Session.Item("RA_VALIDACION_AGREGAR") = "0" Then
                parameter_gestion.error_gestion = "El usuario no tiene permisos para agregar registros"
                resultList.Add(parameter_gestion)
                Return resultList

            End If
            Dim id_escript As Integer = id_script
            parameter_gestion.row_tom = New List(Of class_config_gneral_service_row_option_tom_select)
            Result = Class_plantilla_validacion.Registra_tercero_plantilla_validacion_simpificada(id_escript,
                                                                                                  deserialize_parameter,
                                                                                                  parameter_gestion.row_tom)
            parameter_gestion.error_gestion = Result
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_update_tercero_plantilla_validacion_simplificada(ByVal parameter As Object, ByVal id_script As Object) As IEnumerable(Of class_config_gneral_service_row_tom)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que actualiza el registro de un solicitante
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  RA_VALIDACION_EDITAR
        '-------------------------------------------------------------------------------
        'parameter              : Representa la estructura de los de los datos
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-10-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_config_gneral_service_row_tom)()
        Dim parameter_gestion As class_config_gneral_service_row_tom = New class_config_gneral_service_row_tom()
        Try
            Dim Result As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            Dim deserialize_parameter As New List(Of Class_config_general_service)
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of Class_config_general_service))(parameter)
            If deserialize_parameter Is Nothing Then
                parameter_gestion.error_gestion = "Imposible deserializar los datos del formulario"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            If HttpContext.Current.Session.Item("RA_VALIDACION_EDITAR") = "0" Then
                parameter_gestion.error_gestion = "El usuario no tiene permisos para editar el solicitante"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Dim id_escript As Integer = id_script
            parameter_gestion.row_tom = New List(Of class_config_gneral_service_row_option_tom_select)
            Result = Class_plantilla_validacion.Update_tercero_plantilla_validacion_simplificada(id_escript,
                                                                                                 deserialize_parameter,
                                                                                                 parameter_gestion.row_tom)
            parameter_gestion.error_gestion = Result
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_delete_tercero_plantilla_validacion_simplificada(ByVal parameter As Object, ByVal id_script As Object) As IEnumerable(Of class_config_gneral_service_row_tom)
        '-------------------------------------------------------------------------------
        'Funcion : Servicio que expone al eliminación del tercero
        '          
        '-------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------
        'parameter                  : Representa la identiifcación del tercero
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------

        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-10-23
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Dim resultList = New List(Of class_config_gneral_service_row_tom)()
        Dim parameter_gestion As class_config_gneral_service_row_tom = New class_config_gneral_service_row_tom()
        Try
            Dim Result As String = ""
            Dim Class_plantilla_validacion As New Class_plantilla_validacion
            If HttpContext.Current.Session.Item("RA_VALIDACION_ELIMINAR") = "0" Then
                parameter_gestion.error_gestion = "El usuario no tiene permisos para eliminar regitros"
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Result = Class_plantilla_validacion.Delete_tercero_pantilla_validacion(parameter,
                                                                                   id_script)
            If Result <> "YES" Then
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.row_tom = New List(Of class_config_gneral_service_row_option_tom_select)
                Dim item As New class_config_gneral_service_row_option_tom_select
                item.id_value = parameter
                item.tex_value = ""
                parameter_gestion.row_tom.Add(item)
                parameter_gestion.error_gestion = Result
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.error_gestion = ex.Message
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
End Class