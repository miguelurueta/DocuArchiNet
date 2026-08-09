Imports MySql.Data.MySqlClient

Public Structure estru_seleccion_unidad
    Dim id_registro_produccion As Integer
    Dim idex As Integer
    Dim fecha_elaboracion As String
    Dim id_expediente As Integer
    Dim id_unidad_unidad_conservacion As Integer
    Dim id_clase_documento As Integer
    Dim estado_archivo_documento As Integer
End Structure
Public Class ClassGaGestionInventario
    Function Verifica_propiedad_usuario_documento(ByVal id_registro_produccion As Integer, _
        ByVal id_usuario_gstion As Integer) As String
        '********************************************************
        'Funcion : Verfica si el usuario es propiestario del
        'documento
        'Fecha 2015-03-03
        'Ing Migeuel Angel Urueta Miranda
        '********************************************************
        Try
            Dim Parametro_Consulta As String = "select * from  registro_producion_documental " & _
            " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_produccion & " and ID_USUARIO_GESTION=" & id_usuario_gstion
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("registro_producion_documental")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_propiedad_usuario_documento = "Funcion  Verifica_propiedad_usuario_documento dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Verifica_propiedad_usuario_documento = "Usted no es propietario del documento " &
                " no puede interactuar con el documento "
                Exit Function
            Else
                Verifica_propiedad_usuario_documento = "YES"
                Exit Function
            End If
          
        Catch ex As Exception
            Verifica_propiedad_usuario_documento = "Inconsistencia función Verifica_propiedad_usuario_documento " & ex.Message
        End Try
    End Function
End Class
