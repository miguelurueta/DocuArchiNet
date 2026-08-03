Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient

Public Class conect
    '**************************************************************
    'CONEXION BASE DE DATOS PARA RADICACION
    '**************************************************************
    Public Class Dbase_Conction_Mysql_RA
        Sub New()
            '***********************************************
            'Determina el proveedor de dbms para crear
            'la conexion
            '***********************************************
            'Dim Result As String = ""
            'If HttpContext.Current.Session("TYPE_DBMS_MODULO").ToString = "mysql" Then
            '    If conectionMysql Is Nothing Then
            '        Result = Returna_Conexion_Mysql(conectionMysql)
            '        If Result <> "YES" Then
            '            MsgBox("Imposible conectar a la base de datos Mysql")
            '        End If
            '    Else
            '        If conectionMysql.State = ConnectionState.Closed Then
            '            conectionMysql.Open()
            '        End If
            '    End If
            'End If
            'Dim corba As String = HttpContext.Current.Session(HttpContext.Current.Session("CODIGOPAGINA").ToString)
        End Sub

        Shared Function maum(ByVal df As String, _
                             ByVal d As String) As String
            maum = "YES"
        End Function

        Public Function Returna_Conexion_Mysql(ByRef CconectionMysql As MySql.Data.MySqlClient.MySqlConnection) As String
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
             & ";Pooling=" & poltrue & ";Min Pool Size=0;Max Pool Size=" & _
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
        Private conectionMysql As MySql.Data.MySqlClient.MySqlConnection
        Private MYSQL_INSERT_COMMAND As MySqlCommand
        Private MYSQL_UPDATE_COMMAND As MySqlCommand
        Private MYSQL_DELETE_COMMAND As MySqlCommand
        Private MYSQL_SELECT_COMMAND As MySqlCommand
        '----------------------------------------------------
        'FUNCIONES DE SELECCION 
        '----------------------------------------------------
        Public Function SELECTION_SELECT_FIELD(ByVal Sql_String As String, _
                                               ByRef objet As Object) As String
            Dim Result As String = ""
            SELECTION_SELECT_FIELD = "SELECTION_SELECT_FIELD NO RECONOCE EL DBMS"
            If HttpContext.Current.Session("RA_TYPE_DBMS_MODULO").ToString = "mysql" Then
                Result = MYSQL_SELECT_FIELD(Sql_String, objet)
                If Result <> "YES" Then
                    SELECTION_SELECT_FIELD = "Su sesión caduco Inconsistencia en la funcion SELECTION_SELEC_FIELD LLAMANDO A MYSQL FIELD " & Result
                    Exit Function
                Else
                    SELECTION_SELECT_FIELD = "YES"
                    Exit Function
                End If

            End If

        End Function
        Public Function SELECTION_LAST_INSERT_COMMAND(ByVal Sql_String As String, ByRef last As Object) As String
            Dim Result As String = ""
            SELECTION_LAST_INSERT_COMMAND = "SELECTION_INSERT_COMMAND NO IDENTIFICA EL DBMS"
            If HttpContext.Current.Session("RA_TYPE_DBMS_MODULO").ToString = "mysql" Then
                Result = MYSQL_LAST_INSERT_COMMNAD(Sql_String, last)
                If Result <> "YES" Then
                    SELECTION_LAST_INSERT_COMMAND = "Inconsistencia en la funcion SELECTION_INSERT_COMMAND LLAMANDO A MYSQL INSERT " & Result
                    Exit Function
                Else
                    SELECTION_LAST_INSERT_COMMAND = "YES"
                    Exit Function
                End If
            End If

        End Function
        Public Function SELECTION_INSERT_COMMAND(ByVal Sql_String As String) As String
            Dim Result As String = ""
            SELECTION_INSERT_COMMAND = "SELECTION_INSERT_COMMAND NO IDENTIFICA EL DBMS"
            If HttpContext.Current.Session("RA_TYPE_DBMS_MODULO").ToString = "mysql" Then
                Result = MYSQL_INSERT_COMMNAD(Sql_String)
                If Result <> "YES" Then
                    SELECTION_INSERT_COMMAND = "Inconsistencia en la funcion SELECTION_INSERT_COMMAND LLAMANDO A MYSQL INSERT " & Result
                    Exit Function
                Else
                    SELECTION_INSERT_COMMAND = "YES"

                    Exit Function
                End If

            End If
            'SELECTION_INSERT_COMMAND = "YES"
        End Function
        '--------------------------------------------------
        'FUNCIONES SELECT DE LOS DIFERENTES DBMS
        '--------------------------------------------------
        Private Function MYSQL_SELECT_FIELD(ByVal Sql_String As String, _
                                            ByRef Mysqldatacet As System.Data.DataSet) As String
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
       
        '--------------------------------------------------
        'FUNCIONES DE INSERT DE LOS DIFERENTES DBMS
        '--------------------------------------------------
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
        '--------------------------------------------------
        'FUNCIONES DE INSERT DE LOS DIFERENTES DBMS
        '--------------------------------------------------
        Private Function MYSQL_LAST_INSERT_COMMNAD(ByVal Sql_String As String, ByRef ob As Object) As String

            Dim Command_Base As New MySqlCommand(Sql_String)
            Dim conectmyslq As New MySql.Data.MySqlClient.MySqlConnection
            Dim Result As String = Returna_Conexion_Mysql(conectmyslq)
            If Result <> "YES" Then
                MYSQL_LAST_INSERT_COMMNAD = "Imposible conectar con la base de datos " & Result
            End If
            Me.MYSQL_INSERT_COMMAND = Command_Base
            Try
                Dim command As New MySqlCommand(Me.MYSQL_INSERT_COMMAND.CommandText, conectmyslq)
                If command.ExecuteNonQuery <> 0 Then
                    MYSQL_LAST_INSERT_COMMNAD = "YES"
                    ob = command.LastInsertedId
                    Exit Function
                Else
                    MYSQL_LAST_INSERT_COMMNAD = "NO"
                    Exit Function
                End If
                MYSQL_LAST_INSERT_COMMNAD = "YES"
            Catch ex As MySqlException
                MYSQL_LAST_INSERT_COMMNAD = ex.Message
            Finally
                conectmyslq.Close()
            End Try
        End Function
        Private Function MSQL_INSERT_COMMNAD() As String
            MSQL_INSERT_COMMNAD = "YES"
        End Function
        Private Function ORACLE_INSERT_COMMNAD() As String
            ORACLE_INSERT_COMMNAD = "YES"
        End Function
    End Class
    '**************************************************************
    'CONEXION BASE DE DATOS PARA DOCUARCHI.NET
    '**************************************************************
    Public Class Dbase_Conction_Mysql_DA

        Sub New()

            '***********************************************
            'Determina el proveedor de dbms para crear
            'la conexion
            '***********************************************
            'Dim Result As String = ""
            'If HttpContext.Current.Session("TYPE_DBMS_MODULO").ToString = "mysql" Then
            '    If conectionMysqlDa Is Nothing Then
            '        Result = Returna_Conexion_Mysql(conectionMysqlDa)
            '        If Result <> "YES" Then
            '            MsgBox("Imposible conectar a la base de datos Mysql")
            '        End If
            '    Else
            '        If conectionMysqlDa.State = ConnectionState.Closed Then
            '            conectionMysqlDa.Open()
            '        End If
            '    End If
            'End If
            'Dim corba As String = HttpContext.Current.Session(HttpContext.Current.Session("CODIGOPAGINA").ToString)
        End Sub
        
        Public Function Returna_Conexion_Mysql(ByRef CconectionMysql As MySql.Data.MySqlClient.MySqlConnection) As String
            Dim poltrue As String = "False"
            If HttpContext.Current.Session.Item("DA_ACTIVA_POOL_DBMS") = "1" Then
                poltrue = "True"
            Else
                poltrue = "False"
            End If
            Dim Contenido_Config As String = "Persist Security Info=" _
              & True & ";database=" & HttpContext.Current.Session("DA_DB_NAME_MODULO").ToString _
              & ";server=" & HttpContext.Current.Session("DA_IP_SERVER_MODULO").ToString _
             & ";user id=" & HttpContext.Current.Session("DA_USER_DBMS_MODULO").ToString _
             & ";pwd=" & HttpContext.Current.Session("DA_PASW_DBMS_MODULO").ToString _
             & ";Pooling=" & poltrue & ";Min Pool Size=0;Max Pool Size=" & _
             HttpContext.Current.Session.Item("DA_NUMERO_DBMS_CONEX")


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
        'Private conectionMysql As MySql.Data.MySqlClient.MySqlConnection
        Public conectionMysqlDa As MySql.Data.MySqlClient.MySqlConnection
        Private MYSQL_INSERT_COMMAND As MySqlCommand
        Private MYSQL_UPDATE_COMMAND As MySqlCommand
        Private MYSQL_DELETE_COMMAND As MySqlCommand
        Private MYSQL_SELECT_COMMAND As MySqlCommand
        '----------------------------------------------------
        'FUNCIONES DE SELECCION 
        '----------------------------------------------------
        Public Function SELECTION_SELECT_FIELDA(ByVal Sql_String As String, ByRef objet As Object) As String
            Dim Result As String = ""
            SELECTION_SELECT_FIELDA = "SELECTION_SELECT_FIELDA NO IDENTIFICA EL TIPO DE DBMS"
            If HttpContext.Current.Session("DA_TYPE_DBMS_MODULO").ToString = "mysql" Then
                Result = MYSQL_SELECT_FIELD(Sql_String, objet)
                If Result <> "YES" Then
                    SELECTION_SELECT_FIELDA = "Inconsistencia en la funcion SELECTION_SELEC_FIELD LLAMANDO A MYSQL FIELD " & Result
                    Exit Function
                Else
                    SELECTION_SELECT_FIELDA = "YES"
                    Exit Function
                End If
            End If

        End Function
        Public Function SELECTION_SELECT_FIELDA(ByVal Sql_String As String, ByRef objet As Object, ByVal ef As String) As String
            Dim Result As String = ""
            SELECTION_SELECT_FIELDA = "SELECTION_SELECT_FIELDA NO IDENTIFICA EL TIPO DE DBMS"
            If HttpContext.Current.Session("DA_TYPE_DBMS_MODULO").ToString = "mysql" Then
                Result = MYSQL_SELECT_FIELD(Sql_String, objet, ef)
                If Result <> "YES" Then
                    SELECTION_SELECT_FIELDA = "Inconsistencia en la funcion SELECTION_SELEC_FIELD LLAMANDO A MYSQL FIELD " & Result
                    Exit Function
                Else
                    SELECTION_SELECT_FIELDA = "YES"
                    Exit Function
                End If
            End If

        End Function
        Private Function MYSQL_SELECT_FIELD(ByVal Sql_String As String, ByRef Mysqldatacet As System.Data.DataSet, ByVal table As String) As String
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
            Dim DatMysqlAdpter As MySql.Data.MySqlClient.MySqlDataAdapter = _
                New MySql.Data.MySqlClient.MySqlDataAdapter(MYSQL_SELECT_COMMAND.CommandText, conectmyslq)
            Try
                DatMysqlAdpter.Fill(Mysqldatacet, table)
            Catch ex As MySqlException
                MYSQL_SELECT_FIELD = ex.Message
            Finally
                conectmyslq.Close()
            End Try


        End Function
        Public Function SELECTION_SELECT_FIELDA_READER(ByVal Sql_String As String, ByRef objet As Object) As String
            Dim Result As String = ""
            SELECTION_SELECT_FIELDA_READER = "SELECTION_SELECT_FIELDA NO IDENTIFICA EL TIPO DE DBMS"
            If HttpContext.Current.Session("DA_TYPE_DBMS_MODULO").ToString = "mysql" Then
                Result = C_Dareader_Mysql_Seg(Sql_String, objet)
                If Result <> "YES" Then
                    SELECTION_SELECT_FIELDA_READER = "Inconsistencia en la funcion SELECTION_SELEC_FIELD LLAMANDO A MYSQL FIELD " & Result
                    Exit Function
                Else
                    SELECTION_SELECT_FIELDA_READER = "YES"
                    Exit Function
                End If
            End If

        End Function
        Public Function SELECTION_INSERT_COMMAND(ByVal Sql_String As String) As String
            Dim Result As String = ""
            SELECTION_INSERT_COMMAND = "SELECTION_INSERT_COMMAND NO IDENTIFICA EL TIPO DE DBMS"
            If HttpContext.Current.Session("DA_TYPE_DBMS_MODULO").ToString = "mysql" Then
                Result = MYSQL_INSERT_COMMNAD(Sql_String)
                If Result <> "YES" Then
                    SELECTION_INSERT_COMMAND = "Inconsistencia en la funcion SELECTION_INSERT_COMMAND LLAMANDO A MYSQL INSERT " & Result
                    Exit Function
                Else
                    SELECTION_INSERT_COMMAND = "YES"
                    Exit Function
                End If
            End If

        End Function
        Public Function UPDATE_COMMAND(ByVal Sql_String As String) As String
            Dim Result As String = ""
            UPDATE_COMMAND = ""
            If HttpContext.Current.Session("DA_TYPE_DBMS_MODULO").ToString = "mysql" Then
                Result = MYSQL_INSERT_COMMNAD(Sql_String)
                If Result <> "YES" And Result <> "NO" Then
                    UPDATE_COMMAND = Result
                Else
                    UPDATE_COMMAND = "YES"
                End If
            End If
        End Function
        Public Function SELECTION_DELETE_COMMAND(ByVal Sql_String As String) As String
            Dim Result As String = ""
            SELECTION_DELETE_COMMAND = "SELECTION_INSERT_COMMAND NO IDENTIFICA EL TIPO DE DBMS"
            If HttpContext.Current.Session("DA_TYPE_DBMS_MODULO").ToString = "mysql" Then
                Result = MYSQL_DELETE_COMMNAD(Sql_String)
                If Result <> "YES" Then
                    SELECTION_DELETE_COMMAND = "Inconsistencia en la funcion SELECTION_DELETE_COMMAND LLAMANDO A MYSQL DELETE " & Result
                    Exit Function
                Else
                    SELECTION_DELETE_COMMAND = "YES"
                    Exit Function
                End If
            End If

        End Function
        Public Function SELECTION_LAST_INSERT_COMMAND(ByVal Sql_String As String, ByRef last As Object) As String
            Dim Result As String = ""
            SELECTION_LAST_INSERT_COMMAND = "SELECTION_INSERT_COMMAND NO IDENTIFICA EL DBMS"
            If HttpContext.Current.Session("RA_TYPE_DBMS_MODULO").ToString = "mysql" Then
                Result = MYSQL_LAST_INSERT_COMMNAD(Sql_String, last)
                If Result <> "YES" Then
                    SELECTION_LAST_INSERT_COMMAND = "Inconsistencia en la funcion SELECTION_INSERT_COMMAND LLAMANDO A MYSQL INSERT " & Result
                    Exit Function
                Else
                    SELECTION_LAST_INSERT_COMMAND = "YES"
                    Exit Function
                End If
            End If

        End Function
        Public Function C_Dareader_Mysql_Seg(ByVal Sql_String As String, _
        ByRef RefDatreader As MySqlDataReader) As String
            Try
                'Dim Ref_Car_Conec As New Conect.vb.Dbase_Conction_Mysql
                'parametro de conexion
                'Dim conect_Base As New MySqlConnection(Parametro_Conexion)
                'Me.CONEXION_MYSQL_C = conect_Base
                'parametro para el commando
                Dim Re_sult As New MySqlCommand(Sql_String)
                'Me.SELECT_COMMAND_MYSQL_C = Re_sult
                'Dim Data_Reader_Mysql As New MySqlDataReader
                'Dim Data_Conexion_Mysql As New MySqlConnection
                'Data_Conexion_Mysql = Me.MYSQL_CONEX_COMMAND_C
                'Data_Conexion_Mysql.Open()
                Dim conectmyslq As New MySql.Data.MySqlClient.MySqlConnection
                Dim Result = Returna_Conexion_Mysql(conectmyslq)
                If Result <> "YES" Then
                    C_Dareader_Mysql_Seg = "Imposible conectar con la base de datos " & Result
                    Exit Function
                End If
                Dim command As New MySqlCommand(Re_sult.CommandText, conectmyslq)
                RefDatreader = command.ExecuteReader()
                C_Dareader_Mysql_Seg = "YES"
                Exit Function
                'Me.MYSQL_CONEX_COMMAND_C.Close()
            Catch e As Exception

                'Odbc_Dareader_Mysql.Close()
                C_Dareader_Mysql_Seg = e.Message
            End Try

        End Function
        '--------------------------------------------------
        'FUNCIONES DE INSERT DE LOS DIFERENTES DBMS
        '--------------------------------------------------
        Private Function MYSQL_LAST_INSERT_COMMNAD(ByVal Sql_String As String, ByRef ob As Object) As String

            Dim Command_Base As New MySqlCommand(Sql_String)
            Dim conectmyslq As New MySql.Data.MySqlClient.MySqlConnection
            Dim Result As String = Returna_Conexion_Mysql(conectmyslq)
            If Result <> "YES" Then
                MYSQL_LAST_INSERT_COMMNAD = "Imposible conectar con la base de datos " & Result
            End If
            Me.MYSQL_INSERT_COMMAND = Command_Base
            Try
                Dim command As New MySqlCommand(Me.MYSQL_INSERT_COMMAND.CommandText, conectmyslq)
                If command.ExecuteNonQuery <> 0 Then
                    MYSQL_LAST_INSERT_COMMNAD = "YES"
                    ob = command.LastInsertedId
                    Exit Function
                Else
                    MYSQL_LAST_INSERT_COMMNAD = "NO"
                    Exit Function
                End If
                MYSQL_LAST_INSERT_COMMNAD = "YES"
            Catch ex As MySqlException
                MYSQL_LAST_INSERT_COMMNAD = ex.Message
            Finally
                conectmyslq.Close()
            End Try
        End Function
        '--------------------------------------------------
        'FUNCIONES SELECT DE LOS DIFERENTES DBMS
        '--------------------------------------------------
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
            Dim DatMysqlAdpter As MySql.Data.MySqlClient.MySqlDataAdapter = _
                New MySql.Data.MySqlClient.MySqlDataAdapter(MYSQL_SELECT_COMMAND.CommandText, conectmyslq)
            Try
                DatMysqlAdpter.Fill(Mysqldatacet)
            Catch ex As MySqlException
                MYSQL_SELECT_FIELD = ex.Message
            Finally
                conectmyslq.Close()
            End Try


        End Function
        '--------------------------------------------------
        'FUNCIONES DE INSERT DE LOS DIFERENTES DBMS
        '--------------------------------------------------
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
        '--------------------------------------------------
        'FUNCIONES DE INSERT DE LOS DIFERENTES DBMS
        '--------------------------------------------------
        Private Function MYSQL_DELETE_COMMNAD(ByVal Sql_String As String) As String

            Dim Command_Base As New MySqlCommand(Sql_String)
            Dim conectmyslq As New MySql.Data.MySqlClient.MySqlConnection
            Dim Result As String = Returna_Conexion_Mysql(conectmyslq)
            If Result <> "YES" Then
                MYSQL_DELETE_COMMNAD = "Imposible conectar con la base de datos " & Result
            End If
            Me.MYSQL_INSERT_COMMAND = Command_Base
            Try
                Dim command As New MySqlCommand(Me.MYSQL_INSERT_COMMAND.CommandText, conectmyslq)
                command.ExecuteNonQuery()
                MYSQL_DELETE_COMMNAD = "YES"
            Catch ex As MySqlException
                MYSQL_DELETE_COMMNAD = ex.Message
            Finally
                conectmyslq.Close()
            End Try
        End Function

        Private Function MSQL_INSERT_COMMNAD() As String
            MSQL_INSERT_COMMNAD = "YES"
        End Function
        Private Function ORACLE_INSERT_COMMNAD() As String
            ORACLE_INSERT_COMMNAD = "YES"
        End Function
    End Class
    '**************************************************************
    'CONEXION BASE DE DATOS PARA WORKFLOW
    '**************************************************************
    Public Class Dbase_Conction_Mysql
        Sub New()
            '***********************************************
            'Determina el proveedor de dbms para crear
            'la conexion
            '***********************************************
            'Dim Result As String = ""
            'If HttpContext.Current.Session("TYPE_DBMS_MODULO").ToString = "mysql" Then
            '    If conectionMysql Is Nothing Then
            '        Result = Returna_Conexion_Mysql(conectionMysql)
            '        If Result <> "YES" Then
            '            MsgBox("Imposible conectar a la base de datos Mysql")
            '        End If
            '    Else
            '        If conectionMysql.State = ConnectionState.Closed Then
            '            conectionMysql.Open()
            '        End If
            '    End If
            'End If
            'Dim corba As String = HttpContext.Current.Session(HttpContext.Current.Session("CODIGOPAGINA").ToString)
        End Sub
        Public Function Returna_Conexion_Mysql(ByRef CconectionMysql As MySql.Data.MySqlClient.MySqlConnection) As String
            Dim Contenido_Config As String = "Persist Security Info=" _
              & True & ";database=" & HttpContext.Current.Session("DB_NAME_MODULO").ToString _
              & ";server=" & HttpContext.Current.Session("IP_SERVER_MODULO").ToString _
             & ";user id=" & HttpContext.Current.Session("USER_DBMS_MODULO").ToString _
             & ";pwd=" & HttpContext.Current.Session("PASW_DBMS_MODULO").ToString
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
        Private conectionMysql As MySql.Data.MySqlClient.MySqlConnection
        Private MYSQL_INSERT_COMMAND As MySqlCommand
        Private MYSQL_UPDATE_COMMAND As MySqlCommand
        Private MYSQL_DELETE_COMMAND As MySqlCommand
        Private MYSQL_SELECT_COMMAND As MySqlCommand
        '----------------------------------------------------
        'FUNCIONES DE SELECCION 
        '----------------------------------------------------
        Public Function SELECTION_SELECT_FIELD(ByVal Sql_String As String, ByRef objet As Object) As String
            Dim Result As String = ""
            SELECTION_SELECT_FIELD = "SELECTION_SELECT_FIELD NO IDENTIFICA EL TIPO DE DBMS"
            If HttpContext.Current.Session("TYPE_DBMS_MODULO").ToString = "mysql" Then
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
        Public Function C_Mysql_Actualizar_Plantilla_Vss(ByVal vss_f As Byte(), ByVal Sql_Atualiza_Ruta As String) As String
            Try
                'Dim Sql_Atualiza_Ruta As String = "UPDATE RUTAS_WORKFLOW SET ARCHIVO_PLANTILLA = ?imagen  where " _
                '& "ID_RUTA =" & Id_Ruta
                Dim Command_Base As New MySqlCommand(Sql_Atualiza_Ruta)
                Dim conectmysql As New MySql.Data.MySqlClient.MySqlConnection
                Dim Result = Returna_Conexion_Mysql(conectmysql)
                If Result <> "YES" Then
                    C_Mysql_Actualizar_Plantilla_Vss = "Imposible conectar base de datos" & Result
                    Exit Function
                End If
                Me.MYSQL_INSERT_COMMAND = Command_Base
                Try
                    Dim command As New MySqlCommand(Me.MYSQL_INSERT_COMMAND.CommandText, conectmysql)
                    command.Parameters.AddWithValue("?imagen", vss_f)
                    If command.ExecuteNonQuery <> 0 Then
                        C_Mysql_Actualizar_Plantilla_Vss = "YES"
                        Return C_Mysql_Actualizar_Plantilla_Vss
                    Else
                        C_Mysql_Actualizar_Plantilla_Vss = "NO"
                        Return C_Mysql_Actualizar_Plantilla_Vss
                    End If
                    C_Mysql_Actualizar_Plantilla_Vss = "YES"
                Catch ex As MySqlException
                    C_Mysql_Actualizar_Plantilla_Vss = ex.Message
                Finally
                    conectmysql.Close()
                End Try

            Catch ex As Exception
                Return ex.Message
            End Try
        End Function
        Public Function SELECTION_INSERT_COMMAND(ByVal Sql_String As String) As String
            Dim Result As String = ""
            SELECTION_INSERT_COMMAND = "SELECTION_INSERT_COMMAND NO IDENTIFICA EL TIPO DE DBMS"
            If HttpContext.Current.Session("TYPE_DBMS_MODULO").ToString = "mysql" Then
                Result = MYSQL_INSERT_COMMNAD(Sql_String)
                If Result <> "YES" Then
                    SELECTION_INSERT_COMMAND = "Inconsistencia en la funcion SELECTION_INSERT_COMMAND LLAMANDO A MYSQL INSERT " & Result
                    Exit Function
                End If
            End If
            SELECTION_INSERT_COMMAND = "YES"
        End Function

        '--------------------------------------------------
        'FUNCIONES SELECT DE LOS DIFERENTES DBMS
        '--------------------------------------------------
        Private Function MYSQL_SELECT_FIELD(ByVal Sql_String As String, ByRef Mysqldatacet As System.Data.DataSet) As String
            Dim Result As String = ""
            MYSQL_SELECT_FIELD = "YES"
            Dim conectmysql As New MySql.Data.MySqlClient.MySqlConnection
            Result = Returna_Conexion_Mysql(conectmysql)
            If Result <> "YES" Then
                MYSQL_SELECT_FIELD = "Imposible conectar base de datos" & Result
                Exit Function
            End If
            Mysqldatacet = New DataSet
            MYSQL_SELECT_COMMAND = New MySqlCommand(Sql_String)
            Dim DatMysqlAdpter As MySql.Data.MySqlClient.MySqlDataAdapter = _
                New MySql.Data.MySqlClient.MySqlDataAdapter(MYSQL_SELECT_COMMAND.CommandText, conectmysql)
            Try
                DatMysqlAdpter.Fill(Mysqldatacet)
            Catch ex As MySqlException
                MYSQL_SELECT_FIELD = ex.Message
            Finally
                conectmysql.Close()
            End Try


        End Function
        Public Function SELECTION_LAST_INSERT_COMMAND(ByVal Sql_String As String, ByRef last As Object) As String
            Dim Result As String = ""
            SELECTION_LAST_INSERT_COMMAND = "SELECTION_INSERT_COMMAND NO IDENTIFICA EL DBMS"
            If HttpContext.Current.Session("RA_TYPE_DBMS_MODULO").ToString = "mysql" Then
                Result = MYSQL_LAST_INSERT_COMMNAD(Sql_String, last)
                If Result <> "YES" Then
                    SELECTION_LAST_INSERT_COMMAND = "Inconsistencia en la funcion SELECTION_INSERT_COMMAND LLAMANDO A MYSQL INSERT " & Result
                    Exit Function
                Else
                    SELECTION_LAST_INSERT_COMMAND = "YES"
                    Exit Function
                End If
            End If

        End Function
        '--------------------------------------------------
        'FUNCIONES DE INSERT DE LOS DIFERENTES DBMS
        '--------------------------------------------------
        Private Function MYSQL_LAST_INSERT_COMMNAD(ByVal Sql_String As String, ByRef ob As Object) As String

            Dim Command_Base As New MySqlCommand(Sql_String)
            Dim conectmyslq As New MySql.Data.MySqlClient.MySqlConnection
            Dim Result As String = Returna_Conexion_Mysql(conectmyslq)
            If Result <> "YES" Then
                MYSQL_LAST_INSERT_COMMNAD = "Imposible conectar con la base de datos " & Result
            End If
            Me.MYSQL_INSERT_COMMAND = Command_Base
            Try
                Dim command As New MySqlCommand(Me.MYSQL_INSERT_COMMAND.CommandText, conectmyslq)
                If command.ExecuteNonQuery <> 0 Then
                    MYSQL_LAST_INSERT_COMMNAD = "YES"
                    ob = command.LastInsertedId
                    Exit Function
                Else
                    MYSQL_LAST_INSERT_COMMNAD = "NO"
                    Exit Function
                End If
                MYSQL_LAST_INSERT_COMMNAD = "YES"
            Catch ex As MySqlException
                MYSQL_LAST_INSERT_COMMNAD = ex.Message
            Finally
                conectmyslq.Close()
            End Try
        End Function
        '--------------------------------------------------
        'FUNCIÓN MYSQL PARAMETER
        '--------------------------------------------------
        'FUNCIONES DE INSERT DE LOS DIFERENTES DBMS
        '--------------------------------------------------
        Private Function MYSQL_INSERT_COMMNAD(ByVal Sql_String As String) As String

            Dim Command_Base As New MySqlCommand(Sql_String)
            Dim conectmysql As New MySql.Data.MySqlClient.MySqlConnection
            Dim Result As String = Returna_Conexion_Mysql(conectmysql)
            If Result <> "YES" Then
                MYSQL_INSERT_COMMNAD = "Imposible conectar base de datos " & Result
                Exit Function
            End If
            Me.MYSQL_INSERT_COMMAND = Command_Base
            Try
                Dim command As New MySqlCommand(Me.MYSQL_INSERT_COMMAND.CommandText, conectmysql)
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
                conectmysql.Close()
            End Try
        End Function

        Private Function MSQL_INSERT_COMMNAD() As String
            MSQL_INSERT_COMMNAD = "YES"
        End Function
        Private Function ORACLE_INSERT_COMMNAD() As String
            ORACLE_INSERT_COMMNAD = "YES"
        End Function

        Friend Function SELECTION_SELECT_FIELDA(sqlConsulta As String, datset As DataSet) As String
            Throw New NotImplementedException()
        End Function
    End Class
End Class
