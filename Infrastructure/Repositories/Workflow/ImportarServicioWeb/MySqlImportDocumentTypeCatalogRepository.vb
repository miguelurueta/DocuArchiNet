Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

Public NotInheritable Class MySqlImportDocumentTypeCatalogRepository
    Implements IImportDocumentTypeCatalogRepository

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        If connections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections = connections : _executor = executor
    End Sub

    Public Function Obtener(ByVal contexto As ContextoImportacionServicio) As IList(Of TipoDocumentalCatalogoImportacion) Implements IImportDocumentTypeCatalogRepository.Obtener
        If contexto Is Nothing OrElse contexto.IdTramite <= 0 Then Throw New InvalidOperationException("DOCUMENT_TYPE_CONTEXT_INVALID")
        Const sql As String = "SELECT rdt.tipo_doc_series_Id_Tipo_Doc_Series AS document_type_id," &
            "tds.Descripcion_Documento AS document_type_name,MAX(COALESCE(rdt.OBLIGATORIO,0)) AS required_value," &
            "MIN(COALESCE(rdt.ORDEN_LISTA,0)) AS sort_order,COUNT(DISTINCT tds.Descripcion_Documento) AS name_count " &
            "FROM ra_dig_tipos_docum_lista_chequeo rdt " &
            "INNER JOIN tipo_doc_series tds ON tds.Id_Tipo_Doc_Series=rdt.tipo_doc_series_Id_Tipo_Doc_Series " &
            "WHERE rdt.tipo_doc_entrante_id_Tipo_Doc_Entrante=@procedureId " &
            "GROUP BY rdt.tipo_doc_series_Id_Tipo_Doc_Series,tds.Descripcion_Documento " &
            "ORDER BY sort_order,document_type_name,document_type_id"
        Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
            Return _executor.ExecuteReader(connection, Nothing, sql,
                New List(Of IDataParameter) From {New MySqlParameter("@procedureId", contexto.IdTramite)}, AddressOf Map)
        End Using
    End Function

    Private Shared Function Map(ByVal reader As IDataReader) As IList(Of TipoDocumentalCatalogoImportacion)
        Dim result As New List(Of TipoDocumentalCatalogoImportacion)(), ids As New HashSet(Of Integer)()
        While reader.Read()
            Dim id = Convert.ToInt32(reader("document_type_id"))
            If id <= 0 OrElse Not ids.Add(id) OrElse Convert.ToInt32(reader("name_count")) <> 1 Then Throw New InvalidOperationException("DOCUMENT_TYPE_CATALOG_AMBIGUOUS")
            Dim name = Convert.ToString(reader("document_type_name")).Trim()
            If name.Length = 0 Then Throw New InvalidOperationException("DOCUMENT_TYPE_CATALOG_INVALID")
            result.Add(New TipoDocumentalCatalogoImportacion With {.IdTipoDocumentalTrd=id,.Nombre=name,
                .Obligatorio=Convert.ToInt32(reader("required_value"))=1,.Orden=Convert.ToInt32(reader("sort_order"))})
        End While
        Return result
    End Function

    Private Shared Function ModuleContext(ByVal c As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo="IMPORTAR_SERVICIO_WEB",.IdUsuario=c.IdUsuario,.IdGrupo=c.IdGrupo,.LoginUsuario=c.LoginUsuario}
    End Function
End Class
