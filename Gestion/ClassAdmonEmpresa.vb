Imports System.IO
Imports MySql.Data
Imports System.Xml
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.IO.IsolatedStorage
Public Class ClassAdmonEmpresa
    Function Listar_Empresa_de_Gestion_Activa(ByRef Combo As DropDownList, _
                                              ByRef update As UpdatePanel) As String
        '******************************************************
        'Funcion : Lista la empresas de gestion en un combobox
        'Fecha : 2013-10-04
        'Igeniero: Miguel Angel Urueta Miranda
        '******************************************************
        Try
            Combo.Items.Clear()
            Dim Parametro_Consulta As String = "select  RAZON_SOCIAL_EMPRESA " & _
                  " from EMPRESA_GESTION_DOCUMENTAL "

            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("EMPRESA_GESTION_DOCUMENTAL")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Listar_Empresa_de_Gestion_Activa = " Imposible conectar la base de datos del gestor documental " & Result
                Combo.Items.Clear()
                update.Update()
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    Combo.Items.Add(Dat_reader.Tables(0).Rows(i).Item(0).ToString)
                Next
                update.Update()
                Listar_Empresa_de_Gestion_Activa = "YES"
            Else
                Combo.Items.Clear()
                update.Update()
                Listar_Empresa_de_Gestion_Activa = "YES"
            End If

        Catch ex As Exception
            Listar_Empresa_de_Gestion_Activa = "Inconsistencia General Funcion Listar_Empresa_de_Gestion_Activa " & ex.Message
        End Try
    End Function
    
    Function Retorna_nombre_empresa_usuario_gestion(ByRef nombre_empresa As String, _
    ByVal id_usuario_gestion As Integer) As String
        '*********************************************************
        'Funcion : Retorna nombre empresa usuario de gestion con
        'el parametro id usuario gestion
        'Fecha : 2015-01-08
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT egd.RAZON_SOCIAL_EMPRESA FROM remit_dest_interno as rdt " & _
            " inner join empresa_gestion_documental as egd on (egd.ID_EMPRESA=rdt.Empresa_Gestion_Documental_id_empresa) " & _
                                 " where id_Remit_Dest_int='" & id_usuario_gestion & "'"
             Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("EMPRESA_GESTION_DOCUMENTAL")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Retorna_nombre_empresa_usuario_gestion = " Función Retorna_nombre_empresa_usuario_gestion dice " & Result
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                nombre_empresa = Dat_reader.Tables(0).Rows(0).Item(0)
                Retorna_nombre_empresa_usuario_gestion = "YES"
            Else          
                Retorna_nombre_empresa_usuario_gestion = "YES"
            End If
        Catch ex As Exception
            Retorna_nombre_empresa_usuario_gestion = "Inconsistencia función Retorna_nombre_empresa_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Retorna_id_empresa_usuario_gestion(ByRef id_empresa As Integer, _
   ByVal id_usuario_gestion As Integer) As String
        '*********************************************************
        'Funcion : Retorna id empresa usuario de gestion con
        'el parametro id usuario gestion
        'Fecha : 2016-07-17
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT egd.ID_EMPRESA FROM remit_dest_interno as rdt " & _
            " inner join empresa_gestion_documental as egd on (egd.ID_EMPRESA=rdt.Empresa_Gestion_Documental_id_empresa) " & _
                                 " where id_Remit_Dest_int='" & id_usuario_gestion & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("EMPRESA_GESTION_DOCUMENTAL")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Retorna_id_empresa_usuario_gestion = " Función Retorna_nombre_empresa_usuario_gestion dice " & Result
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                id_empresa = Dat_reader.Tables(0).Rows(0).Item(0)
                Retorna_id_empresa_usuario_gestion = "YES"
                Exit Function
            Else
                id_empresa = 0
                Retorna_id_empresa_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_empresa_usuario_gestion = "Inconsistencia función Retorna_id_empresa_usuario_gestion " & ex.Message
        End Try
    End Function

    Function Retorna_Id_Emprea(ByVal Nombre_Empresa As String, ByRef id_empresa As Integer) As String
        '*********************************************************
        'Funcion : Retorna_Id_Emprea, retorna el id de la mepresa
        'o entidad con el parametro nombre_emresa o razon social
        'Fecha : 2013-10-04
        'ingeniero : Miguel Angel Urueta Miranda
        '*********************************************************
        Try
            Dim Parametro_Consulta As String = "select  ID_EMPRESA " & _
                      " from EMPRESA_GESTION_DOCUMENTAL where RAZON_SOCIAL_EMPRESA='" & Nombre_Empresa & "'"
             Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("EMPRESA_GESTION_DOCUMENTAL")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Retorna_Id_Emprea = " Función Retorna_Id_Emprea dice " & Result
                Exit Function
            End If
            Dim Paswuser As String = ""
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                id_empresa = Dat_reader.Tables(0).Rows(0).Item(0)
                Retorna_Id_Emprea = "YES"
            Else
                Retorna_Id_Emprea = "YES"
            End If
            Retorna_Id_Emprea = "YES"
        Catch ex As Exception
            Retorna_Id_Emprea = "Inconsistencia general Funcion Retorna_Id_Emprea " & ex.Message
        End Try
    End Function
End Class
