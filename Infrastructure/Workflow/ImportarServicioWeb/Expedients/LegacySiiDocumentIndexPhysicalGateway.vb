Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Xml
Imports MySql.Data.MySqlClient

' Actualiza con la función legacy conservada y verifica SQL/XML mediante lecturas independientes.
Public NotInheritable Class LegacySiiDocumentIndexPhysicalGateway
    Implements ISiiDocumentIndexPhysicalGateway

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        If connections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _connections = connections : _executor = executor
    End Sub

    Public Function ActualizarCampos(ByVal contexto As ContextoImportacionServicio,
                                     ByVal documento As DocumentoRelacionadoImportacion,
                                     ByVal campos As IDictionary(Of String, String)) As Boolean Implements ISiiDocumentIndexPhysicalGateway.ActualizarCampos
        If Not Valid(contexto, documento) OrElse campos Is Nothing Then Return False
        Dim cabinet = SafeCabinet(documento.NombreGabinete)
        Try
          Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
            Dim effective As IDictionary(Of String, String)
            effective = ResolveEffectiveFields(connection, cabinet, campos)
            If effective.Count = 0 Then Return False
            Dim assignments As New List(Of String)()
            Dim parameters As New List(Of IDataParameter)()
            Dim ordinal As Integer = 0
            For Each pair In effective
                Dim parameterName = "@field" & ordinal.ToString()
                assignments.Add("`" & pair.Key & "`=" & parameterName)
                parameters.Add(New MySqlParameter(parameterName, pair.Value))
                ordinal += 1
            Next
            parameters.Add(New MySqlParameter("@imageId", documento.IdImagen))
            parameters.Add(New MySqlParameter("@expedientId", documento.IdExpedienteEsperado.Value))
            Dim sql = "UPDATE `" & cabinet & "` SET " & String.Join(",", assignments.ToArray()) &
                " WHERE ID=@imageId AND ID_EXPEDIENTE=@expedientId"
            Try
                _executor.ExecuteNonQuery(connection, Nothing, sql, parameters)
            Catch ex As MySqlException
                Throw New InvalidOperationException(SafeUpdateFailureCode(ex.Number))
            Catch
                Throw New InvalidOperationException("DOCUMENT_INDEX_DYNAMIC_UPDATE_FAILED")
            End Try
            campos.Clear()
            For Each pair In effective : campos(pair.Key) = pair.Value : Next
            Return True
          End Using
        Catch ex As InvalidOperationException
            Throw
        Catch
            Throw New InvalidOperationException("DOCUMENT_INDEX_CONNECTION_FAILED")
        End Try
    End Function

    Public Function LeerCampos(ByVal contexto As ContextoImportacionServicio,
                               ByVal documento As DocumentoRelacionadoImportacion,
                               ByVal campos As IDictionary(Of String, String)) As IDictionary(Of String, String) Implements ISiiDocumentIndexPhysicalGateway.LeerCampos
        If Not Valid(contexto, documento) OrElse campos Is Nothing OrElse campos.Count = 0 Then Return Nothing
        Dim cabinet = SafeCabinet(documento.NombreGabinete)
        Try
          Using connection = _connections.CreateOpenConnection(ModuleContext(contexto))
            Dim names As New List(Of String)()
            For Each pair In campos
                If Not SafeIdentifier(pair.Key) Then Return Nothing
                names.Add("`" & pair.Key & "`")
            Next
            Dim sql = "SELECT " & String.Join(",", names.ToArray()) & " FROM `" & cabinet & "` WHERE ID=@imageId AND ID_EXPEDIENTE=@expedientId LIMIT 2"
            Try
                Return _executor.ExecuteReader(connection, Nothing, sql, New List(Of IDataParameter) From {New MySqlParameter("@imageId", documento.IdImagen), New MySqlParameter("@expedientId", documento.IdExpedienteEsperado.Value)}, AddressOf MapFields)
            Catch
                Throw New InvalidOperationException("DOCUMENT_INDEX_DYNAMIC_READ_FAILED")
            End Try
          End Using
        Catch ex As InvalidOperationException
            Throw
        Catch
            Throw New InvalidOperationException("DOCUMENT_INDEX_CONNECTION_FAILED")
        End Try
    End Function

    Public Function ExisteIndiceSql(ByVal contexto As ContextoImportacionServicio,
                                    ByVal documento As DocumentoRelacionadoImportacion) As Boolean Implements ISiiDocumentIndexPhysicalGateway.ExisteIndiceSql
        Return FindProductionRecord(documento) > 0
    End Function

    Public Function ExisteIndiceXml(ByVal contexto As ContextoImportacionServicio,
                                    ByVal documento As DocumentoRelacionadoImportacion) As Boolean Implements ISiiDocumentIndexPhysicalGateway.ExisteIndiceXml
        If Not Valid(contexto, documento) Then Return False
        Dim productionId = FindProductionRecord(documento)
        If productionId <= 0 Then Return False
        Dim route As stru_ruta_expediente = Nothing
        If Not String.Equals(New Class_ra_ruta_expediente().Solicita_datos_estructura_ruta_expediente(route), "YES", StringComparison.OrdinalIgnoreCase) Then Return False
        Dim xmlPath As String = System.IO.Path.Combine(Convert.ToString(route.RUTA).Replace("/", "\"), Convert.ToString(route.DISCO).PadLeft(9, "0"c), documento.IdExpedienteEsperado.Value.ToString().PadLeft(9, "0"c) & ".xml")
        If Not File.Exists(xmlPath) Then Return False
        Dim xml As New XmlDocument() : xml.Load(xmlPath)
        For Each node As XmlNode In xml.SelectNodes("//*[local-name()='DocumentoIndizado']/*[local-name()='Id']")
            Dim value As Long
            If Long.TryParse(node.InnerText, value) AndAlso value = productionId Then Return True
        Next
        Return False
    End Function

    Private Shared Function FindProductionRecord(ByVal documento As DocumentoRelacionadoImportacion) As Long
        If documento Is Nothing OrElse Not documento.IdExpedienteEsperado.HasValue OrElse documento.IdExpedienteEsperado.Value > Integer.MaxValue Then Return 0
        Dim records() As stru_produccion_indice = Nothing
        Dim response = New ClassGaProducionDocumental().Solicita_estructura_registro_relacion_expediente_indice(CInt(documento.IdExpedienteEsperado.Value), records)
        If Not String.Equals(response, "YES", StringComparison.OrdinalIgnoreCase) OrElse records Is Nothing Then Return 0
        Dim found As Long = 0
        For Each record In records
            If record.ID_DOCUMENTO_DOCUARCHI_ALMACEN = documento.IdImagen AndAlso String.Equals(record.NOMBRE_GABINETE, documento.NombreGabinete, StringComparison.OrdinalIgnoreCase) Then
                If found <> 0 Then Return 0
                found = record.ID_REGISTRO_PRODUCION_DOCUMENTAL
            End If
        Next
        Return found
    End Function

    Private Function ResolveEffectiveFields(ByVal connection As IDbConnection,
                                            ByVal cabinet As String,
                                            ByVal candidates As IDictionary(Of String, String)) As IDictionary(Of String, String)
        Const sql As String = "SELECT d.CAMPO,c.DATA_TYPE,c.CHARACTER_MAXIMUM_LENGTH " &
            "FROM DETALLE_GABIENETE d INNER JOIN INFORMATION_SCHEMA.COLUMNS c " &
            "ON c.TABLE_SCHEMA=DATABASE() AND c.TABLE_NAME=d.GABINETE AND c.COLUMN_NAME=d.CAMPO " &
            "WHERE d.GABINETE=@cabinet ORDER BY d.IDENTI"
        Dim configured As IDictionary(Of String, DynamicFieldMetadata)
        Try
            configured = _executor.ExecuteReader(connection, Nothing, sql,
                New List(Of IDataParameter) From {New MySqlParameter("@cabinet", cabinet)}, AddressOf MapConfiguredFields)
        Catch
            Throw New InvalidOperationException("DOCUMENT_INDEX_STRUCTURE_QUERY_FAILED")
        End Try
        Dim effective As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        For Each pair In candidates
            Dim metadata As DynamicFieldMetadata = Nothing
            If SafeIdentifier(pair.Key) AndAlso configured.TryGetValue(pair.Key, metadata) Then
                Dim fitted = FitValue(If(pair.Value, String.Empty), metadata)
                If Not ValidForType(fitted, metadata.DataType) Then
                    Throw New InvalidOperationException("DOCUMENT_INDEX_VALUE_TYPE_INVALID_" & SafeCodeField(pair.Key))
                End If
                effective(pair.Key) = fitted
            End If
        Next
        Return effective
    End Function

    Private Shared Function MapConfiguredFields(ByVal reader As IDataReader) As IDictionary(Of String, DynamicFieldMetadata)
        Dim result As New Dictionary(Of String, DynamicFieldMetadata)(StringComparer.OrdinalIgnoreCase)
        While reader.Read()
            Dim name = Convert.ToString(reader("CAMPO")).Trim()
            If SafeIdentifier(name) Then
                Dim maximumLength As Long = 0
                If Not Convert.IsDBNull(reader("CHARACTER_MAXIMUM_LENGTH")) Then maximumLength = Convert.ToInt64(reader("CHARACTER_MAXIMUM_LENGTH"))
                result(name) = New DynamicFieldMetadata With {
                    .Name = name,
                    .DataType = Convert.ToString(reader("DATA_TYPE")).Trim().ToLowerInvariant(),
                    .MaximumLength = maximumLength
                }
            End If
        End While
        Return result
    End Function

    Private Shared Function FitValue(ByVal value As String, ByVal metadata As DynamicFieldMetadata) As String
        Dim clean = If(value, String.Empty)
        If metadata Is Nothing OrElse metadata.MaximumLength <= 0 OrElse Not IsTextType(metadata.DataType) Then Return clean
        If clean.Length <= metadata.MaximumLength Then Return clean
        Return clean.Substring(0, CInt(Math.Min(metadata.MaximumLength, Integer.MaxValue)))
    End Function

    Private Shared Function IsTextType(ByVal dataType As String) As Boolean
        Select Case If(dataType, String.Empty).ToLowerInvariant()
            Case "char", "varchar", "tinytext", "text", "mediumtext", "longtext"
                Return True
            Case Else
                Return False
        End Select
    End Function

    Private Shared Function ValidForType(ByVal value As String, ByVal dataType As String) As Boolean
        Dim kind = If(dataType, String.Empty).ToLowerInvariant()
        If IsTextType(kind) OrElse kind = "binary" OrElse kind = "varbinary" OrElse kind.EndsWith("blob", StringComparison.Ordinal) Then Return True
        If kind = "tinyint" OrElse kind = "smallint" OrElse kind = "mediumint" OrElse kind = "int" OrElse
           kind = "integer" OrElse kind = "bigint" OrElse kind = "decimal" OrElse kind = "numeric" OrElse
           kind = "float" OrElse kind = "double" OrElse kind = "real" OrElse kind = "bit" Then
            Dim number As Decimal
            Return Decimal.TryParse(value, NumberStyles.Number Or NumberStyles.AllowExponent, CultureInfo.InvariantCulture, number)
        End If
        If kind = "date" OrElse kind = "datetime" OrElse kind = "timestamp" OrElse kind = "time" OrElse kind = "year" Then
            Dim temporal As DateTime
            Return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, temporal)
        End If
        Return True
    End Function

    Private Shared Function SafeCodeField(ByVal fieldName As String) As String
        Dim value = If(fieldName, String.Empty).Trim().ToUpperInvariant()
        If value.Length > 36 Then value = value.Substring(0, 36)
        Return If(value.Length = 0, "UNKNOWN", value)
    End Function

    Private Shared Function SafeUpdateFailureCode(ByVal errorNumber As Integer) As String
        Select Case errorNumber
            Case 1044, 1045, 1142, 1227
                Return "DOCUMENT_INDEX_UPDATE_PERMISSION_DENIED"
            Case 1054
                Return "DOCUMENT_INDEX_UPDATE_COLUMN_INVALID"
            Case 1146
                Return "DOCUMENT_INDEX_UPDATE_TABLE_UNAVAILABLE"
            Case 1264, 1265, 1366, 1292
                Return "DOCUMENT_INDEX_UPDATE_TYPE_INVALID"
            Case 1406
                Return "DOCUMENT_INDEX_UPDATE_TOO_LONG"
            Case 1062, 1451, 1452
                Return "DOCUMENT_INDEX_UPDATE_CONSTRAINT_FAILED"
            Case 1205, 1213
                Return "DOCUMENT_INDEX_UPDATE_CONCURRENCY_FAILED"
            Case Else
                Return "DOCUMENT_INDEX_DYNAMIC_UPDATE_FAILED"
        End Select
    End Function

    Private Shared Function MapFields(ByVal reader As IDataReader) As IDictionary(Of String, String)
        If Not reader.Read() Then Return Nothing
        Dim result As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        For index As Integer = 0 To reader.FieldCount - 1
            result(reader.GetName(index)) = Convert.ToString(reader.GetValue(index))
        Next
        If reader.Read() Then Return Nothing
        Return result
    End Function

    Private Shared Function SafeCabinet(ByVal value As String) As String
        Dim cabinet = If(value, String.Empty).Trim().ToUpperInvariant()
        If Not SafeIdentifier(cabinet) Then Throw New InvalidOperationException("DOCUMENT_INDEX_CABINET_NOT_SUPPORTED")
        Return cabinet
    End Function
    Private Shared Function SafeIdentifier(ByVal value As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(value) AndAlso Regex.IsMatch(value, "^[A-Za-z_][A-Za-z0-9_]*$")
    End Function
    Private Shared Function Valid(ByVal context As ContextoImportacionServicio, ByVal document As DocumentoRelacionadoImportacion) As Boolean
        Return context IsNot Nothing AndAlso document IsNot Nothing AndAlso context.IdTarea = document.IdTarea AndAlso document.IdImagen > 0 AndAlso document.IdExpedienteEsperado.HasValue AndAlso document.IdExpedienteEsperado.Value > 0 AndAlso document.IdExpedienteEsperado.Value <= Integer.MaxValue
    End Function
    Private Shared Function ModuleContext(ByVal c As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {.CodigoModulo = "IMPORTAR_SERVICIO_WEB", .IdUsuario = c.IdUsuario, .IdGrupo = c.IdGrupo, .LoginUsuario = c.LoginUsuario}
    End Function

    Private NotInheritable Class DynamicFieldMetadata
        Public Property Name As String
        Public Property DataType As String
        Public Property MaximumLength As Long
    End Class
End Class
