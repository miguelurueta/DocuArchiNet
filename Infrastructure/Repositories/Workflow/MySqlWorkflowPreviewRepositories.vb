Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient

'Repositorios de lectura para el preview. Ninguno conoce HttpContext, controles Web Forms ni operaciones de escritura.
Public MustInherit Class MySqlWorkflowPreviewRepositoryBase
    Private ReadOnly _connectionFactory As IModuleConnectionFactory
    Private ReadOnly _dataExecutor As IDataExecutor

    Protected Sub New(ByVal connectionFactory As IModuleConnectionFactory, ByVal dataExecutor As IDataExecutor)
        If connectionFactory Is Nothing Then Throw New ArgumentNullException(NameOf(connectionFactory))
        If dataExecutor Is Nothing Then Throw New ArgumentNullException(NameOf(dataExecutor))

        _connectionFactory = connectionFactory
        _dataExecutor = dataExecutor
    End Sub

    Protected Function EjecutarLectura(Of T)(ByVal contexto As ContextoModuloWorkflow,
                                             ByVal sql As String,
                                             ByVal parameters As IEnumerable(Of IDataParameter),
                                             ByVal projector As Func(Of IDataReader, T)) As T
        Return EjecutarLecturaDesde(_connectionFactory, contexto, sql, parameters, projector)
    End Function

    Protected Function EjecutarLecturaDesde(Of T)(ByVal connectionFactory As IModuleConnectionFactory,
                                                  ByVal contexto As ContextoModuloWorkflow,
                                                  ByVal sql As String,
                                                  ByVal parameters As IEnumerable(Of IDataParameter),
                                                  ByVal projector As Func(Of IDataReader, T)) As T
        If connectionFactory Is Nothing Then Throw New InvalidOperationException("MODULE_CONNECTION_UNAVAILABLE")
        Using connection As IDbConnection = connectionFactory.CreateOpenConnection(contexto)
            Return _dataExecutor.ExecuteReader(connection, Nothing, sql, parameters, projector)
        End Using
    End Function

    Protected Function ObtenerMetadatosRuta(ByVal contexto As ContextoModuloWorkflow, ByVal idRuta As Integer) As MetadatosRutaWorkflow
        Const sql As String = "SELECT rw.Nombre_Ruta, " &
                              "MAX(CASE WHEN clr.Campo_Radicado = 1 THEN clr.Nombre_Campo END) AS Campo_Radicado, " &
                              "MAX(CASE WHEN clr.Campo_Tramite = 1 THEN clr.Nombre_Campo END) AS Campo_Tramite " &
                              "FROM rutas_workflow AS rw " &
                              "LEFT JOIN configuracion_listado_ruta AS clr ON clr.Rutas_Workflow_id_Ruta = rw.ID_RUTA " &
                              "WHERE rw.ID_RUTA = @idRuta " &
                              "GROUP BY rw.ID_RUTA, rw.Nombre_Ruta"

        Return EjecutarLectura(Of MetadatosRutaWorkflow)(contexto, sql,
            New List(Of IDataParameter) From {Parametro("@idRuta", idRuta)},
            Function(reader As IDataReader) As MetadatosRutaWorkflow
                If Not reader.Read() Then Return Nothing
                Return New MetadatosRutaWorkflow With {
                    .NombreRuta = Texto(reader, "Nombre_Ruta"),
                    .CampoRadicado = Texto(reader, "Campo_Radicado"),
                    .CampoTramite = Texto(reader, "Campo_Tramite")
                }
            End Function)
    End Function

    Protected Function ObtenerNombreGrupo(ByVal contexto As ContextoModuloWorkflow) As String
        Const sql As String = "SELECT Nombre_Grupo FROM grupos_workflow WHERE ID_GRUPO = @idGrupo LIMIT 1"
        Return EjecutarLectura(Of String)(contexto, sql,
            New List(Of IDataParameter) From {Parametro("@idGrupo", contexto.IdGrupoWorkflow)},
            Function(reader As IDataReader) As String
                If Not reader.Read() Then Return String.Empty
                Return Texto(reader, "Nombre_Grupo")
            End Function)
    End Function

    Protected Shared Function Bloqueo(ByVal codigo As String, ByVal mensaje As String) As ResultadoDestinosTransicion
        Return New ResultadoDestinosTransicion With {
            .CodigoBloqueo = codigo,
            .MensajeFuncional = mensaje
        }
    End Function

    Protected Shared Function Parametro(ByVal nombre As String, ByVal valor As Object) As IDataParameter
        Dim parameter As New MySqlParameter()
        parameter.ParameterName = nombre
        parameter.Value = If(valor, DBNull.Value)
        Return parameter
    End Function

    Protected Shared Function EsIdentificadorSeguro(ByVal value As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(value) AndAlso Regex.IsMatch(value, "^[A-Za-z0-9_]+$")
    End Function

    Protected Shared Function Texto(ByVal reader As IDataReader, ByVal fieldName As String) As String
        Dim ordinal As Integer = reader.GetOrdinal(fieldName)
        If reader.IsDBNull(ordinal) Then Return String.Empty
        Return Convert.ToString(reader.GetValue(ordinal), CultureInfo.InvariantCulture)
    End Function

    Protected Shared Function Entero(ByVal reader As IDataReader, ByVal fieldName As String) As Integer
        Dim ordinal As Integer = reader.GetOrdinal(fieldName)
        If reader.IsDBNull(ordinal) Then Return 0
        Return Convert.ToInt32(reader.GetValue(ordinal), CultureInfo.InvariantCulture)
    End Function

    Protected Shared Function EnteroLargo(ByVal reader As IDataReader, ByVal fieldName As String) As Long
        Dim ordinal As Integer = reader.GetOrdinal(fieldName)
        If reader.IsDBNull(ordinal) Then Return 0L
        Return Convert.ToInt64(reader.GetValue(ordinal), CultureInfo.InvariantCulture)
    End Function

    Protected Class MetadatosRutaWorkflow
        Public Property NombreRuta As String
        Public Property CampoRadicado As String
        Public Property CampoTramite As String
    End Class
End Class

Public Class MySqlTareaWorkflowRepository
    Inherits MySqlWorkflowPreviewRepositoryBase
    Implements ITareaWorkflowRepository

    Public Sub New()
        Me.New(New ModuleConnectionFactory("MyDbContext"), New AdoNetDataExecutor())
    End Sub

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory, ByVal dataExecutor As IDataExecutor)
        MyBase.New(connectionFactory, dataExecutor)
    End Sub

    Public Function ObtenerTarea(ByVal contexto As ContextoModuloWorkflow, ByVal idTarea As Long) As TareaWorkflow Implements ITareaWorkflowRepository.ObtenerTarea
        Const sqlEstado As String = "SELECT id_Estado, Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta, " &
                                    "Inicio_Tareas_Workflow_id_Tarea, Id_Actividad, ID_FLUJO_TRABAJO, " &
                                    "ID_ACTIVIDAD_FLUJO_TRABAJO " &
                                    "FROM estados_tarea_workflow " &
                                    "WHERE Inicio_Tareas_Workflow_id_Tarea = @idTarea " &
                                    "AND ID_USUARIO = @idUsuario " &
                                    "AND FECHA_SELECCION IS NOT NULL " &
                                    "AND FECHA_FIN IS NULL " &
                                    "AND ESTADO_TAREA = 0 " &
                                    "ORDER BY id_Estado DESC LIMIT 1"

        Dim tarea As TareaWorkflow = EjecutarLectura(Of TareaWorkflow)(contexto, sqlEstado,
            New List(Of IDataParameter) From {Parametro("@idTarea", idTarea), Parametro("@idUsuario", contexto.IdUsuarioWorkflow)},
            Function(reader As IDataReader) As TareaWorkflow
                If Not reader.Read() Then Return Nothing
                Return New TareaWorkflow With {
                    .IdEstado = EnteroLargo(reader, "id_Estado"),
                    .IdTarea = EnteroLargo(reader, "Inicio_Tareas_Workflow_id_Tarea"),
                    .IdRuta = Entero(reader, "Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta"),
                    .IdActividadOrigen = Entero(reader, "Id_Actividad"),
                    .IdFlujoTrabajo = Entero(reader, "ID_FLUJO_TRABAJO"),
                    .IdActividadFlujoTrabajo = Entero(reader, "ID_ACTIVIDAD_FLUJO_TRABAJO"),
                    .IdGrupoWorkflow = contexto.IdGrupoWorkflow,
                    .EstaActiva = True
                }
            End Function)
        If tarea Is Nothing OrElse tarea.IdRuta <= 0 Then Return Nothing

        Dim metadatos As MetadatosRutaWorkflow = ObtenerMetadatosRuta(contexto, tarea.IdRuta)
        If metadatos Is Nothing OrElse Not EsIdentificadorSeguro(metadatos.NombreRuta) OrElse Not EsIdentificadorSeguro(metadatos.CampoRadicado) Then
            Return Nothing
        End If

        Dim tabla As String = "dat_adic_tar" & metadatos.NombreRuta
        If Not EsIdentificadorSeguro(tabla) Then Return Nothing

        Dim sqlDatos As String = "SELECT `" & metadatos.CampoRadicado & "` AS RADICADO, FLUJO_TRABAJO_WF " &
                                 "FROM `" & tabla & "` WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea LIMIT 1"
        Dim datos As TareaWorkflow = EjecutarLectura(Of TareaWorkflow)(contexto, sqlDatos,
            New List(Of IDataParameter) From {Parametro("@idTarea", tarea.IdTarea)},
            Function(reader As IDataReader) As TareaWorkflow
                If Not reader.Read() Then Return Nothing
                tarea.Radicado = Texto(reader, "RADICADO")
                Dim flujoDatos As Integer = Entero(reader, "FLUJO_TRABAJO_WF")
                If flujoDatos > 0 Then tarea.IdFlujoTrabajo = flujoDatos
                tarea.TipoDecision = If(tarea.IdFlujoTrabajo > 0, "FLUJO", "RUTA")
                tarea.GrupoActual = ObtenerNombreGrupo(contexto)
                tarea.RequiereNotificacion = False
                tarea.TokenVersion = tarea.IdEstado.ToString(CultureInfo.InvariantCulture)
                Return tarea
            End Function)
        Return datos
    End Function
End Class

Public Class MySqlTransicionFlujoRepository
    Inherits MySqlWorkflowPreviewRepositoryBase
    Implements ITransicionFlujoRepository

    Public Sub New()
        Me.New(New ModuleConnectionFactory("MyDbContext"), New AdoNetDataExecutor())
    End Sub

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory, ByVal dataExecutor As IDataExecutor)
        MyBase.New(connectionFactory, dataExecutor)
    End Sub

    Public Function ObtenerDestinos(ByVal contexto As ContextoModuloWorkflow, ByVal tarea As TareaWorkflow) As ResultadoDestinosTransicion Implements ITransicionFlujoRepository.ObtenerDestinos
        If tarea Is Nothing OrElse tarea.IdFlujoTrabajo <= 0 OrElse tarea.IdActividadFlujoTrabajo <= 0 Then
            Return Bloqueo(CodigosBloqueoPrevisualizacion.TransicionInconsistente, "No fue posible resolver el flujo de la tarea.")
        End If

        Const sqlDestinos As String = "SELECT conector.ID_REGISTRO_ACTIVIDAD_ENVIO AS ID_CONECTOR, " &
                                      "destino.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO AS ID_ACTIVIDAD_DESTINO, " &
                                      "COALESCE(conector.ID_USUARIO_WORKFLOW_DESTINO, 0) AS ID_USUARIO_DESTINO, " &
                                      "COALESCE(grupo.ID_GRUPO, 0) AS ID_GRUPO_DESTINO, " &
                                      "actividad.Nombre_Actividad AS NOMBRE_ACTIVIDAD, " &
                                      "CONCAT_WS(' - ', usuario.Nombre_Usuario, usuario.Cargo_Usuario) AS DESTINATARIO, " &
                                      "grupo.Nombre_Grupo AS NOMBRE_GRUPO " &
                                      "FROM wf_registro_conectores_actividades_envio_flujo_trabajo AS conector " &
                                      "INNER JOIN wf_registro_actividaes_flujos_trabajo AS destino " &
                                      "ON destino.ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO = conector.IDENT_ACTIVIDAD_FLUJO_TRAB_DESTINO " &
                                      "AND destino.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = conector.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO " &
                                      "INNER JOIN listado_actividades_workflow AS actividad " &
                                      "ON actividad.ID_ACTIVIDAD = destino.listado_actividades_workflow_id_Actividad " &
                                      "LEFT JOIN usuario_workflow AS usuario ON usuario.idU_suario = conector.ID_USUARIO_WORKFLOW_DESTINO " &
                                      "LEFT JOIN grupos_workflow AS grupo ON grupo.ID_ACTIVIDAD = actividad.ID_ACTIVIDAD " &
                                      "WHERE conector.wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = @idFlujo " &
                                      "AND conector.IDENT_ACTIVIDAD_FLUJO_TRAB_FUENTE = @idActividad " &
                                      "AND (conector.ID_USUARIO_WORKFLOW_FUENTE IS NULL OR conector.ID_USUARIO_WORKFLOW_FUENTE = 0 " &
                                      "OR conector.ID_USUARIO_WORKFLOW_FUENTE = @idUsuario) " &
                                      "ORDER BY actividad.Nombre_Actividad, conector.ID_REGISTRO_ACTIVIDAD_ENVIO"

        Dim destinos As IList(Of DestinoTransicion) = EjecutarLectura(Of IList(Of DestinoTransicion))(contexto, sqlDestinos,
            New List(Of IDataParameter) From {Parametro("@idFlujo", tarea.IdFlujoTrabajo), Parametro("@idActividad", tarea.IdActividadFlujoTrabajo), Parametro("@idUsuario", contexto.IdUsuarioWorkflow)},
            Function(reader As IDataReader) As IList(Of DestinoTransicion)
                Dim resultado As New List(Of DestinoTransicion)()
                Dim orden As Integer = 1
                While reader.Read()
                    resultado.Add(New DestinoTransicion With {
                        .IdConector = Entero(reader, "ID_CONECTOR"),
                        .IdActividadDestino = Entero(reader, "ID_ACTIVIDAD_DESTINO"),
                        .IdUsuarioWorkflowDestino = Entero(reader, "ID_USUARIO_DESTINO"),
                        .IdGrupoWorkflowDestino = Entero(reader, "ID_GRUPO_DESTINO"),
                        .Nombre = Texto(reader, "NOMBRE_ACTIVIDAD"),
                        .NombreDestinatario = Texto(reader, "DESTINATARIO"),
                        .NombreGrupo = Texto(reader, "NOMBRE_GRUPO"),
                        .TipoTransicion = "FLUJO",
                        .Orden = orden
                    })
                    orden += 1
                End While
                Return resultado
            End Function)
        Return New ResultadoDestinosTransicion With {.Destinos = destinos}
    End Function
End Class

Public Class MySqlTransicionRutaRepository
    Inherits MySqlWorkflowPreviewRepositoryBase
    Implements ITransicionRutaRepository

    Private ReadOnly _docuarchiConnectionFactory As IModuleConnectionFactory

    Public Sub New()
        Me.New(New ModuleConnectionFactory("MyDbContext"), New ModuleConnectionFactory("MyDbContext"), New AdoNetDataExecutor())
    End Sub

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory, ByVal dataExecutor As IDataExecutor)
        Me.New(connectionFactory, connectionFactory, dataExecutor)
    End Sub

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory,
                   ByVal docuarchiConnectionFactory As IModuleConnectionFactory,
                   ByVal dataExecutor As IDataExecutor)
        MyBase.New(connectionFactory, dataExecutor)
        _docuarchiConnectionFactory = docuarchiConnectionFactory
    End Sub

    Public Function ObtenerDestinos(ByVal contexto As ContextoModuloWorkflow, ByVal tarea As TareaWorkflow) As ResultadoDestinosTransicion Implements ITransicionRutaRepository.ObtenerDestinos
        If tarea Is Nothing OrElse tarea.IdRuta <= 0 Then
            Return Bloqueo(CodigosBloqueoPrevisualizacion.TransicionInconsistente, "No fue posible resolver la ruta de la tarea.")
        End If

        Dim metadatos As MetadatosRutaWorkflow = ObtenerMetadatosRuta(contexto, tarea.IdRuta)
        If metadatos Is Nothing OrElse Not EsIdentificadorSeguro(metadatos.NombreRuta) OrElse Not EsIdentificadorSeguro(metadatos.CampoTramite) Then
            Return Bloqueo(CodigosBloqueoPrevisualizacion.TransicionInconsistente, "No fue posible validar la ruta de la tarea.")
        End If

        Dim tabla As String = "dat_adic_tar" & metadatos.NombreRuta
        If Not EsIdentificadorSeguro(tabla) Then
            Return Bloqueo(CodigosBloqueoPrevisualizacion.TransicionInconsistente, "No fue posible validar la ruta de la tarea.")
        End If

        Dim sqlTramite As String = "SELECT `" & metadatos.CampoTramite & "` AS TRAMITE, FLUJO_TRABAJO_WF " &
                                   "FROM `" & tabla & "` WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea LIMIT 1"
        Dim tramite As String = EjecutarLectura(Of String)(contexto, sqlTramite,
            New List(Of IDataParameter) From {Parametro("@idTarea", tarea.IdTarea)},
            Function(reader As IDataReader) As String
                If Not reader.Read() OrElse Entero(reader, "FLUJO_TRABAJO_WF") <> 0 Then Return Nothing
                Return Texto(reader, "TRAMITE")
            End Function)
        If String.IsNullOrWhiteSpace(tramite) Then
            Return Bloqueo(CodigosBloqueoPrevisualizacion.RutaCerrada, "La ruta de la tarea no esta disponible.")
        End If

        If _docuarchiConnectionFactory Is Nothing Then
            Return Bloqueo(CodigosBloqueoPrevisualizacion.TransicionInconsistente, "No fue posible validar la ruta de la tarea.")
        End If

        Const sqlEstadoRuta As String = "SELECT estado_ruta_open_close FROM tipo_doc_entrante " &
                                        "WHERE Descripcion_Doc = @tramite LIMIT 1"
        Dim rutaCerrada As Boolean = EjecutarLecturaDesde(Of Boolean)(_docuarchiConnectionFactory, contexto, sqlEstadoRuta,
            New List(Of IDataParameter) From {Parametro("@tramite", tramite)},
            Function(reader As IDataReader) As Boolean
                Return reader.Read() AndAlso Entero(reader, "estado_ruta_open_close") <> 0
            End Function)
        If rutaCerrada Then Return Bloqueo(CodigosBloqueoPrevisualizacion.RutaCerrada, "La ruta de la tarea no esta disponible.")

        Const sqlDestinos As String = "SELECT destino.ID_ACTIVIDAD AS ID_ACTIVIDAD_DESTINO, " &
                                      "disponible.ID_ACTIVIDADES_DISPONIBLES_ENVIO AS ID_CONECTOR, " &
                                      "COALESCE(grupoDestino.ID_GRUPO, 0) AS ID_GRUPO_DESTINO, " &
                                      "destino.NOMBRE_ACTIVIDAD AS NOMBRE_ACTIVIDAD, " &
                                      "grupoDestino.NOMBRE_GRUPO AS NOMBRE_GRUPO " &
                                      "FROM grupos_workflow AS grupoOrigen " &
                                      "INNER JOIN actividades_disponibles_envio AS disponible " &
                                      "ON disponible.Listado_Actividades_Workflow_Id_Actividad = grupoOrigen.ID_ACTIVIDAD " &
                                      "INNER JOIN listado_actividades_workflow AS destino " &
                                      "ON destino.ID_ACTIVIDAD = disponible.ID_ACTIVIDAD_SIGUIENTE " &
                                      "LEFT JOIN grupos_workflow AS grupoDestino ON grupoDestino.ID_ACTIVIDAD = destino.ID_ACTIVIDAD " &
                                      "WHERE grupoOrigen.ID_GRUPO = @idGrupo " &
                                      "AND grupoOrigen.ID_ACTIVIDAD = @idActividadOrigen " &
                                      "AND disponible.id_Ruta = @idRuta " &
                                      "AND destino.RUTAS_WORKFLOW_ID_RUTA = @idRuta " &
                                      "ORDER BY destino.NOMBRE_ACTIVIDAD, disponible.ID_ACTIVIDADES_DISPONIBLES_ENVIO"
        Dim destinos As IList(Of DestinoTransicion) = EjecutarLectura(Of IList(Of DestinoTransicion))(contexto, sqlDestinos,
            New List(Of IDataParameter) From {Parametro("@idGrupo", contexto.IdGrupoWorkflow), Parametro("@idActividadOrigen", tarea.IdActividadOrigen), Parametro("@idRuta", tarea.IdRuta)},
            Function(reader As IDataReader) As IList(Of DestinoTransicion)
                Dim resultado As New List(Of DestinoTransicion)()
                Dim orden As Integer = 1
                While reader.Read()
                    resultado.Add(New DestinoTransicion With {
                        .IdConector = Entero(reader, "ID_CONECTOR"),
                        .IdActividadDestino = Entero(reader, "ID_ACTIVIDAD_DESTINO"),
                        .IdGrupoWorkflowDestino = Entero(reader, "ID_GRUPO_DESTINO"),
                        .Nombre = Texto(reader, "NOMBRE_ACTIVIDAD"),
                        .NombreGrupo = Texto(reader, "NOMBRE_GRUPO"),
                        .TipoTransicion = "RUTA",
                        .Orden = orden
                    })
                    orden += 1
                End While
                Return resultado
            End Function)
        Return New ResultadoDestinosTransicion With {.Destinos = destinos}
    End Function
End Class
