Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient

' Lectura moderna y parametrizada de la configuración legacy; no invoca sus funciones ni usa sesión.
Public NotInheritable Class MySqlImportExpedientConfigurationRepository
    Implements IImportExpedientConfigurationRepository

    Private ReadOnly _docuarchiConnections As IModuleConnectionFactory
    Private ReadOnly _radicacionConnections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        Me.New(connections, connections, executor)
    End Sub

    Public Sub New(ByVal docuarchiConnections As IModuleConnectionFactory,
                   ByVal radicacionConnections As IModuleConnectionFactory,
                   ByVal executor As IDataExecutor)
        If docuarchiConnections Is Nothing OrElse radicacionConnections Is Nothing OrElse executor Is Nothing Then Throw New ArgumentNullException("dependency")
        _docuarchiConnections = docuarchiConnections
        _radicacionConnections = radicacionConnections
        _executor = executor
    End Sub

    Public Function Obtener(ByVal contexto As ContextoImportacionServicio) As ConfiguracionExpedienteImportacion Implements IImportExpedientConfigurationRepository.Obtener
        If contexto Is Nothing OrElse contexto.IdTramite <= 0 Then Return Nothing
        Dim header As ConfigurationHeader = Nothing
        Try
            Using connection = _docuarchiConnections.CreateOpenConnection(ModuleContext(contexto))
                Const headerSql As String =
                    "SELECT nombre_gabinete_workflow,ra_auto_registro_expediente_id_auto_registro," &
                    "util_Estado_Crea_ExpedienteSII,util_Estado_Multiple_expedienteSII " &
                    "FROM tipo_doc_entrante WHERE id_Tipo_Doc_Entrante=@procedureId LIMIT 2"
                Dim headers = _executor.ExecuteReader(connection, Nothing, headerSql,
                    New List(Of IDataParameter) From {P("@procedureId", contexto.IdTramite)}, AddressOf MapHeaders)
                If headers.Count <> 1 Then Return Nothing
                header = headers(0)
                If String.IsNullOrWhiteSpace(header.CabinetName) OrElse header.AutoRegistrationId <= 0 Then Return Nothing
            End Using
        Catch
            Throw New InvalidOperationException("EXPEDIENT_CONFIGURATION_HEADER_QUERY_FAILED")
        End Try

        Try
            Using connection = _radicacionConnections.CreateOpenConnection(ModuleContext(contexto))
            Const fieldsSql As String =
                "SELECT campo_expediente,estado_obligatorio FROM ra_auto_campo_unico_expediente " &
                "WHERE ra_auto_registro_expediente_id_auto_registro=@autoRegistrationId AND estado_unico=1 " &
                "ORDER BY campo_expediente"
            Dim fields As IList(Of CampoIdentidadExpedienteImportacion)
            Try
                fields = _executor.ExecuteReader(connection, Nothing, fieldsSql,
                    New List(Of IDataParameter) From {P("@autoRegistrationId", header.AutoRegistrationId)}, AddressOf MapIdentityFields)
            Catch
                Throw New InvalidOperationException("EXPEDIENT_CONFIGURATION_IDENTITY_FIELDS_QUERY_FAILED")
            End Try
            If fields.Count = 0 Then Return Nothing
            For Each field In fields
                If Not SafeIdentifier(field.NombreCampo) Then Return Nothing
            Next

            Const secondarySql As String =
                "SELECT DISTINCT tipo_doc_series_Id_Tipo_Doc_Series FROM ra_dig_tipos_docum_lista_chequeo " &
                "WHERE tipo_doc_entrante_id_Tipo_Doc_Entrante=@procedureId AND UtilExpedienteRelacionado=1 " &
                "ORDER BY tipo_doc_series_Id_Tipo_Doc_Series"
            Dim secondaryTypes As IList(Of Integer)
            Try
                secondaryTypes = _executor.ExecuteReader(connection, Nothing, secondarySql,
                    New List(Of IDataParameter) From {P("@procedureId", contexto.IdTramite)}, AddressOf MapSecondaryTypes)
            Catch
                Throw New InvalidOperationException("EXPEDIENT_CONFIGURATION_SECONDARY_TYPES_QUERY_FAILED")
            End Try

            Return New ConfiguracionExpedienteImportacion With {
                .NombreGabinete = header.CabinetName,
                .IdAutoRegistro = header.AutoRegistrationId,
                .CreacionAutomaticaHabilitada = header.CreateEnabled,
                .ExpedienteObligatorio = header.CreateEnabled,
                .Modo = If(header.CreateEnabled, ModoExpedienteImportacion.GestionarExpediente, ModoExpedienteImportacion.SinExpediente),
                .MultiplesExpedientes = header.MultipleEnabled,
                .CamposIdentidad = fields,
                .TipologiasSecundarias = secondaryTypes
            }
            End Using
        Catch ex As InvalidOperationException
            Throw
        Catch
            Throw New InvalidOperationException("EXPEDIENT_CONFIGURATION_RADICACION_CONNECTION_FAILED")
        End Try
    End Function

    Private Shared Function MapHeaders(ByVal reader As IDataReader) As IList(Of ConfigurationHeader)
        Dim values As New List(Of ConfigurationHeader)()
        While reader.Read()
            values.Add(New ConfigurationHeader With {
                .CabinetName = Convert.ToString(reader("nombre_gabinete_workflow")),
                .AutoRegistrationId = SafeInteger(reader("ra_auto_registro_expediente_id_auto_registro")),
                .CreateEnabled = SafeInteger(reader("util_Estado_Crea_ExpedienteSII")) = 1,
                .MultipleEnabled = SafeInteger(reader("util_Estado_Multiple_expedienteSII")) = 1
            })
        End While
        Return values
    End Function

    Private Shared Function MapIdentityFields(ByVal reader As IDataReader) As IList(Of CampoIdentidadExpedienteImportacion)
        Dim values As New List(Of CampoIdentidadExpedienteImportacion)()
        While reader.Read()
            values.Add(New CampoIdentidadExpedienteImportacion With {
                .NombreCampo = Convert.ToString(reader("campo_expediente")),
                .Obligatorio = SafeInteger(reader("estado_obligatorio")) = 1
            })
        End While
        Return values
    End Function

    Private Shared Function MapSecondaryTypes(ByVal reader As IDataReader) As IList(Of Integer)
        Dim values As New List(Of Integer)()
        While reader.Read()
            Dim value = SafeInteger(reader("tipo_doc_series_Id_Tipo_Doc_Series"))
            If value > 0 Then values.Add(value)
        End While
        Return values
    End Function

    Private Shared Function SafeInteger(ByVal value As Object) As Integer
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return 0
        Return Convert.ToInt32(value)
    End Function

    Private Shared Function SafeIdentifier(ByVal value As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(value) AndAlso Regex.IsMatch(value, "^[A-Za-z_][A-Za-z0-9_]*$")
    End Function

    Private Shared Function P(ByVal name As String, ByVal value As Object) As IDataParameter
        Return New MySqlParameter(name, If(value, DBNull.Value))
    End Function

    Private Shared Function ModuleContext(ByVal c As ContextoImportacionServicio) As ContextoModulo
        Return New ContextoModulo With {
            .CodigoModulo = "IMPORTAR_SERVICIO_WEB",
            .IdUsuario = c.IdUsuario,
            .IdGrupo = c.IdGrupo,
            .LoginUsuario = c.LoginUsuario
        }
    End Function

    Private NotInheritable Class ConfigurationHeader
        Public Property CabinetName As String
        Public Property AutoRegistrationId As Integer
        Public Property CreateEnabled As Boolean
        Public Property MultipleEnabled As Boolean
    End Class
End Class
