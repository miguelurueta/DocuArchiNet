Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports GestionDocumental_Docuarchi.net.conect
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.Web.Http
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceGestorDocumental
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_actualiza_tipologia_migracion(ByVal parameter As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone el modulo de actualización tipo documental
        '          desde gsbinete o modulo de migracion
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_ra_tipo_documental_serie      : Representa la estructura con los datos
        'de gestión del del tipo documental
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-21
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_ra_tipo_documental_serie)
        Try
            Dim Result As String = ""
            resultList = JsonConvert.DeserializeObject(Of List(Of class_ra_tipo_documental_serie))(parameter)
            Dim Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            Dim valor_cambio As String = ""
            If Session.Item("UTIL_MIGRA_UPDATE_TIPOLOGIA") = 0 Then
                resultList.Item(0).error_gestion = "El usuario no tiene permisos para cambiar tipologia"
                Return resultList
            End If
            Result = Class_ra_tipo_doc_series.Actualiza_tipo_documental_migracion(resultList.Item(0),
                                                                                  valor_cambio)
            If Result <> "YES" Then
                resultList.Item(0).error_gestion = Result
                Return resultList
            Else
                resultList.Item(0).error_gestion = Result
                Return resultList
            End If
        Catch ex As Exception
            resultList.Item(0).error_gestion = "Inconsistencia general funcion Service_Auto_registra_expediente_tramite " & ex.Message
            Return resultList
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_gestion_autoregistro_gabinete(ByVal id_auto_registro As Object)
        '--------------------------------------------------------------------------------
        'Funcion : Servicio que espone las estructuras para clasidicación de la tipologia
        '          documental de auto registro
        '  
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_auto_registro       : Representa la identificación de auto registro
        '                        
        '
        '
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'class_cambio_tipologia_gabinete : Retorna la estructura de gestion documental 
        '                                  para auto registro
        '                   
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-08-19
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim resultList = New List(Of class_cambio_tipologia_gabinete)()
        Dim item_ilist As class_cambio_tipologia_gabinete = New class_cambio_tipologia_gabinete
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            item_ilist.Error_result = ClassDaGabinete.Solicita_gestion_autoregistro_gabinete(id_auto_registro,
                                                                                             item_ilist)

            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_lista_sub_series_documentales_id_serie(ByVal id_serie As Object)
        '--------------------------------------------------------------------------------
        'Funcion : Servicio que expone la estructura de series documentales
        '
        '          
        '  
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_serie              : Representa la identificación de la serie documental
        '                        
        '
        '
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'class_cambio_tipologia_gabinete : Retorna la estructura de las series
        '                              
        '                   
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-08-18
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim resultList = New List(Of class_cambio_tipologia_gabinete)()
        Dim item_ilist As class_cambio_tipologia_gabinete = New class_cambio_tipologia_gabinete
        Try
            Dim Result As String = ""
            Dim Class_subseries_documentales As New Class_subseries_documentales
            item_ilist.iLIStSubSerie = New List(Of control_drow_lista)
            item_ilist.Error_result = Class_subseries_documentales.Solicita_lista_series_sub_documentales_id_serie(id_serie,
                                                                                                                   item_ilist.iLIStSubSerie)

            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            End If
            Dim Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            item_ilist.iLIStTipo = New List(Of control_drow_lista)
            item_ilist.Error_result = Class_ra_tipo_doc_series.Solicita_lista_tipos_documentales_relacionados_id_sub_serie(Val(item_ilist.iLIStSubSerie.Item(0).value),
                                                                                                                           item_ilist.iLIStTipo)
            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Solicita_lista_tipos_documentales_relacionados_sub_serie(ByVal id_sub_serie As Object)
        '--------------------------------------------------------------------------------
        'Funcion : Servicio que expone la estructura de tipos documentales relcionados a
        'sub serie documental
        '          
        '  
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'id_sub_serie              : Representa la identificación de la serie documental
        '                        
        '
        '
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'class_cambio_tipologia_gabinete : Retorna la estructura de la lista de tipologias 
        '                              
        '                   
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-08-18
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim resultList = New List(Of class_cambio_tipologia_gabinete)()
        Dim item_ilist As class_cambio_tipologia_gabinete = New class_cambio_tipologia_gabinete
        Try
            Dim Result As String = ""
            Dim Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            item_ilist.iLIStTipo = New List(Of control_drow_lista)
            item_ilist.Error_result = Class_ra_tipo_doc_series.Solicita_lista_tipos_documentales_relacionados_id_sub_serie(id_sub_serie,
                                                                                                                           item_ilist.iLIStTipo)

            If item_ilist.Error_result <> "YES" Then
                resultList.Add(item_ilist)
                Return resultList
            Else
                resultList.Add(item_ilist)
                Return resultList
            End If
        Catch ex As Exception
            item_ilist.Error_result = ex.Message
            resultList.Add(item_ilist)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_actualiza_log_sesion_usuario_gestion_documental(name) As String
        Try
            Dim Ref_Class_log_usuario_gestion As New Class_log_usuario_gestion
            Dim Result As String = ""
            Result = Ref_Class_log_usuario_gestion.Actualiza_log_sesion_usuario_gestion_documental(HttpContext.Current.Session.Item("id_registro_sesion_log_gd"))
            Return "YES"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Private MYSQL_SELECT_COMMAND As MySqlCommand
    Private MYSQL_INSERT_COMMAND As MySqlCommand
    Private Function MYSQL_INSERT_COMMNAD(ByVal Sql_String As String) As String

        Dim Command_Base As New MySqlCommand(Sql_String)
        Dim conectmyslq As New MySql.Data.MySqlClient.MySqlConnection
        Dim Result As String = Returna_Conexion_Mysql(conectmyslq)
        If Result <> "YES" Then
            MYSQL_INSERT_COMMNAD = "Imposible conectar con la base de datos " & Result
        End If
        Me.MYSQL_INSERT_COMMAND = Command_Base
        Try
            Dim command As New MySqlCommand(Me.MYSQL_INSERT_COMMAND.CommandText, conectmyslq)
            If command.ExecuteNonQuery <> 0 Then
                MYSQL_INSERT_COMMNAD = "YES"
                Return MYSQL_INSERT_COMMNAD
            Else
                MYSQL_INSERT_COMMNAD = "NO"

                Return MYSQL_INSERT_COMMNAD
            End If
            MYSQL_INSERT_COMMNAD = "YES"
        Catch ex As MySqlException
            MYSQL_INSERT_COMMNAD = ex.Message
        Finally
            conectmyslq.Close()
        End Try
    End Function
    Private Function Returna_Conexion_Mysql(ByRef CconectionMysql As MySql.Data.MySqlClient.MySqlConnection) As String
        Dim poltrue As String = "False"
        If HttpContext.Current.Session.Item("RA_ACTIVA_POOL_DBMS") = "1" Then
            poltrue = "True"
        Else
            poltrue = "False"
        End If
        Dim Contenido_Config As String = "Persist Security Info=" _
          & True & ";database=" & HttpContext.Current.Session("RA_DB_NAME_MODULO").ToString _
          & ";server=" & HttpContext.Current.Session("RA_IP_SERVER_MODULO").ToString _
         & ";user id=" & HttpContext.Current.Session("RA_USER_DBMS_MODULO").ToString _
         & ";pwd=" & HttpContext.Current.Session("RA_PASW_DBMS_MODULO").ToString _
         & ";Pooling=" & poltrue & ";Min Pool Size=0;Max Pool Size=" &
         HttpContext.Current.Session.Item("RA_NUMERO_DBMS_CONEX")


        Try
            CconectionMysql = New MySql.Data.MySqlClient.MySqlConnection(Contenido_Config)
            If Not CconectionMysql Is Nothing Then
                CconectionMysql.Open()
            Else
                Returna_Conexion_Mysql = "Imposible conectar en la base de datos"
                Exit Function
            End If
            Returna_Conexion_Mysql = "YES"
        Catch ex As MySqlException
            Returna_Conexion_Mysql = ex.Message
        Finally
            'CconectionMysql = Nothing
        End Try
    End Function
    Public Function SELECTION_SELECT_FIELD(ByVal Sql_String As String, ByRef objet As Object) As String
        Dim Result As String = ""
        SELECTION_SELECT_FIELD = "SELECTION_SELECT_FIELD NO RECONOCE EL DBMS"
        If HttpContext.Current.Session("RA_TYPE_DBMS_MODULO").ToString = "mysql" Then
            Result = MYSQL_SELECT_FIELD(Sql_String, objet)
            If Result <> "YES" Then
                SELECTION_SELECT_FIELD = "Inconsistencia en la funcion SELECTION_SELEC_FIELD LLAMANDO A MYSQL FIELD " & Result
                Exit Function
            Else
                SELECTION_SELECT_FIELD = "YES"
                Exit Function
            End If

        End If

    End Function
    Private Function MYSQL_SELECT_FIELD(ByVal Sql_String As String, ByRef Mysqldatacet As System.Data.DataSet) As String
        Dim Result As String = ""
        MYSQL_SELECT_FIELD = "YES"
        Mysqldatacet = New DataSet
        Dim conectmyslq As New MySql.Data.MySqlClient.MySqlConnection
        Result = Returna_Conexion_Mysql(conectmyslq)
        If Result <> "YES" Then
            MYSQL_SELECT_FIELD = "Imposible conectar con la base de datos " & Result
            Exit Function
        End If
        MYSQL_SELECT_COMMAND = New MySqlCommand(Sql_String)
        Dim DatMysqlAdpter As MySql.Data.MySqlClient.MySqlDataAdapter =
            New MySql.Data.MySqlClient.MySqlDataAdapter(MYSQL_SELECT_COMMAND.CommandText, conectmyslq)
        Try
            DatMysqlAdpter.Fill(Mysqldatacet)
        Catch ex As MySqlException
            MYSQL_SELECT_FIELD = ex.Message
        Finally
            conectmyslq.Close()
        End Try


    End Function
End Class