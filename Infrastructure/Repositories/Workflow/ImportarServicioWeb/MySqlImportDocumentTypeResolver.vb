Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

' La lista de chequeo pertenece a Radicación. La consulta es únicamente SELECT y parametrizada.
Public NotInheritable Class MySqlImportDocumentTypeResolver
    Implements IImportDocumentTypeResolver

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        If connections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections = connections
        _executor = executor
    End Sub

    Public Function Resolver(ByVal contexto As ContextoImportacionServicio,
                             ByVal idTipoDocumentalTrd As Integer,
                             ByVal nombreTipoDocumental As String) As ResolucionTipoDocumentalImportacion Implements IImportDocumentTypeResolver.Resolver
        If contexto Is Nothing OrElse contexto.IdTramite <= 0 OrElse idTipoDocumentalTrd <= 0 OrElse String.IsNullOrWhiteSpace(nombreTipoDocumental) Then
            Return Fallo("INVALID_DOCUMENT_TYPE")
        End If

        Const sql As String = "SELECT rdt.ID_TIPO_DOCUMENTAL_CHEQUEO,tds.Id_Tipo_Doc_Series,tds.Descripcion_Documento " &
            "FROM ra_dig_tipos_docum_lista_chequeo rdt " &
            "INNER JOIN tipo_doc_series tds ON tds.Id_Tipo_Doc_Series=rdt.tipo_doc_series_Id_Tipo_Doc_Series " &
            "WHERE rdt.tipo_doc_entrante_id_Tipo_Doc_Entrante=@procedureId " &
            "AND rdt.tipo_doc_series_Id_Tipo_Doc_Series=@documentTypeId " &
            "ORDER BY rdt.ID_TIPO_DOCUMENTAL_CHEQUEO LIMIT 2"
        Dim matches As IList(Of ResolucionTipoDocumentalImportacion)
        Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
            matches = _executor.ExecuteReader(connection, Nothing, sql,
                New List(Of IDataParameter) From {P("@procedureId", contexto.IdTramite), P("@documentTypeId", idTipoDocumentalTrd)},
                AddressOf MapMatches)
        End Using
        If matches.Count = 0 Then Return Fallo("DOCUMENT_TYPE_NOT_ALLOWED_FOR_PROCEDURE")
        If matches.Count > 1 Then Return Fallo("DOCUMENT_TYPE_MAPPING_AMBIGUOUS")
        Dim match = matches(0)
        If Not String.Equals(match.NombreTipoDocumental.Trim(), nombreTipoDocumental.Trim(), StringComparison.OrdinalIgnoreCase) Then
            Return Fallo("DOCUMENT_TYPE_NAME_MISMATCH")
        End If
        match.Valida = True
        Return match
    End Function

    Private Shared Function MapMatches(ByVal reader As IDataReader) As IList(Of ResolucionTipoDocumentalImportacion)
        Dim result As New List(Of ResolucionTipoDocumentalImportacion)()
        While reader.Read()
            result.Add(New ResolucionTipoDocumentalImportacion With {
                .IdTipoListaChequeo = Convert.ToInt32(reader("ID_TIPO_DOCUMENTAL_CHEQUEO")),
                .IdTipoDocumentalTrd = Convert.ToInt32(reader("Id_Tipo_Doc_Series")),
                .NombreTipoDocumental = Convert.ToString(reader("Descripcion_Documento"))})
        End While
        Return result
    End Function

    Private Shared Function Fallo(ByVal codigo As String) As ResolucionTipoDocumentalImportacion
        Return New ResolucionTipoDocumentalImportacion With {.Valida = False, .Codigo = codigo}
    End Function

    Private Shared Function P(ByVal name As String, ByVal value As Object) As IDataParameter
        Return New MySqlParameter(name, If(value, DBNull.Value))
    End Function

    Private Shared Function ModuleContext(ByVal c As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo = "IMPORTAR_SERVICIO_WEB", .IdUsuario = c.IdUsuario, .IdGrupo = c.IdGrupo, .LoginUsuario = c.LoginUsuario}
    End Function
End Class
