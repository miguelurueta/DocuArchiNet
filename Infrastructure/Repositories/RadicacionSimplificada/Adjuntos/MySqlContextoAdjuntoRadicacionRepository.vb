Imports System
Imports System.Collections.Generic
Imports System.Data
Imports MySql.Data.MySqlClient

Public NotInheritable Class MySqlContextoAdjuntoRadicacionRepository
    Implements IContextoAdjuntoRadicacionRepository

    Private ReadOnly _connections As IModuleConnectionFactory
    Private ReadOnly _executor As IDataExecutor

    Public Sub New(ByVal connections As IModuleConnectionFactory, ByVal executor As IDataExecutor)
        If connections Is Nothing Then Throw New ArgumentNullException(NameOf(connections))
        If executor Is Nothing Then Throw New ArgumentNullException(NameOf(executor))
        _connections = connections
        _executor = executor
    End Sub

    Public Function ObtenerAutorizado(ByVal contextoModulo As ContextoModulo,
                                      ByVal idRegistroEstado As Long,
                                      ByRef contextoAdjunto As ContextoAdjuntoRadicacion) As String Implements IContextoAdjuntoRadicacionRepository.ObtenerAutorizado
        contextoAdjunto = Nothing
        If contextoModulo Is Nothing OrElse Not contextoModulo.EsValido() Then
            Return "La sesión no contiene un contexto válido para adjuntar el documento."
        End If
        If idRegistroEstado <= 0 Then
            Return "El registro seleccionado no es válido para adjuntar el documento."
        End If

        Const sql As String =
            "SELECT id_estado_radicado, consecutivo_radicado, id_tarea_workflow, " &
            "tipo_doc_entrante_id_Tipo_Doc_Entrante, system_plantilla_radicado_id_Plantilla " &
            "FROM ra_rad_estados_modulo_radicacion " &
            "WHERE id_estado_radicado = @idRegistroEstado " &
            "AND id_usuario_radicado = @idUsuarioRadicacion LIMIT 1"

        Try
            Using connection As IDbConnection = _connections.CreateOpenConnection(contextoModulo)
                contextoAdjunto = _executor.ExecuteReader(Of ContextoAdjuntoRadicacion)(
                    connection,
                    Nothing,
                    sql,
                    New List(Of IDataParameter) From {
                        Parametro("@idRegistroEstado", idRegistroEstado),
                        Parametro("@idUsuarioRadicacion", contextoModulo.IdUsuario)
                    },
                    Function(reader As IDataReader) As ContextoAdjuntoRadicacion
                        If Not reader.Read() Then Return Nothing
                        Return New ContextoAdjuntoRadicacion(
                            Convert.ToInt64(reader("id_estado_radicado")),
                            Convert.ToString(reader("consecutivo_radicado")),
                            Convert.ToInt64(reader("id_tarea_workflow")),
                            Convert.ToInt32(reader("tipo_doc_entrante_id_Tipo_Doc_Entrante")),
                            Convert.ToInt32(reader("system_plantilla_radicado_id_Plantilla")),
                            String.Empty)
                    End Function)
            End Using
        Catch
            Return "No fue posible validar el registro seleccionado para adjuntar el documento."
        End Try

        If contextoAdjunto Is Nothing Then
            Return "El registro seleccionado no existe o no está autorizado para el usuario actual."
        End If
        If Not contextoAdjunto.EsValido() Then
            contextoAdjunto = Nothing
            Return "El registro seleccionado no contiene una tarea, trámite, plantilla y radicado válidos para adjuntar el documento."
        End If
        Return "YES"
    End Function

    Private Shared Function Parametro(ByVal nombre As String, ByVal valor As Object) As IDataParameter
        Return New MySqlParameter(nombre, valor)
    End Function

End Class
