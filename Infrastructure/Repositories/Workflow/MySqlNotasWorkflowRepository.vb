Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization

'Persistencia moderna de lectura de Notas. Las mutaciones quedan fail-closed hasta una fase aprobada.
Public Class MySqlNotasWorkflowRepository
    Inherits MySqlWorkflowPreviewRepositoryBase
    Implements INotasWorkflowRepository

    Private Const TamanoPaginaMaximo As Integer = 50

    Public Sub New()
        Me.New(New ModuleConnectionFactory("MyDbContext"), New AdoNetDataExecutor())
    End Sub

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory,
                   ByVal dataExecutor As IDataExecutor)
        MyBase.New(connectionFactory, dataExecutor)
    End Sub

    Public Function Listar(ByVal contexto As ContextoModuloWorkflow,
                           ByVal tarea As TareaWorkflow,
                           ByVal solicitud As SolicitudListarNotasWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Listar
        If Not EsSolicitudListarValida(tarea, solicitud) Then Return NoDisponible()

        Const sql As String = "SELECT at.ID_ANOTACION AS ID_NOTA, " &
                              "at.INICIO_TAREAS_WORKFLOW_ID_TAREA AS ID_TAREA, " &
                              "COALESCE(at.ID_USUARIO, 0) AS ID_AUTOR, " &
                              "COALESCE(at.ID_ACTIVIDAD, 0) AS ID_ACTIVIDAD_ORIGEN, " &
                              "at.FECHA_ANOTACION AS FECHA_CREACION " &
                              "FROM ANOTACION_TAREA AS at " &
                              "WHERE at.INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea " &
                              "AND at.ESTADO_TAREA = 1 " &
                              "AND (@tieneCursor = 0 OR at.FECHA_ANOTACION < @fechaCursor " &
                              "OR (at.FECHA_ANOTACION = @fechaCursor AND at.ID_ANOTACION < @idNotaCursor)) " &
                              "ORDER BY at.FECHA_ANOTACION DESC, at.ID_ANOTACION DESC " &
                              "LIMIT @limite"

        Dim tamanoPagina As Integer = Math.Max(1, Math.Min(TamanoPaginaMaximo, solicitud.TamanoPagina))
        Dim filas As IList(Of NotaWorkflow) = EjecutarLectura(Of IList(Of NotaWorkflow))(contexto, sql,
            New List(Of IDataParameter) From {
                Parametro("@idTarea", tarea.IdTarea),
                Parametro("@tieneCursor", If(solicitud.FechaCursorUtc.HasValue, 1, 0)),
                Parametro("@fechaCursor", If(solicitud.FechaCursorUtc.HasValue, solicitud.FechaCursorUtc.Value, DateTime.MinValue)),
                Parametro("@idNotaCursor", solicitud.IdNotaCursor),
                Parametro("@limite", tamanoPagina + 1)},
            Function(reader As IDataReader) As IList(Of NotaWorkflow)
                Dim resultado As New List(Of NotaWorkflow)()
                While reader.Read()
                    resultado.Add(New NotaWorkflow With {
                        .IdNota = EnteroLargo(reader, "ID_NOTA"),
                        .IdTarea = EnteroLargo(reader, "ID_TAREA"),
                        .IdAutorWorkflow = Entero(reader, "ID_AUTOR"),
                        .IdActividadOrigen = Entero(reader, "ID_ACTIVIDAD_ORIGEN"),
                        .FechaCreacionUtc = FechaUtc(reader, "FECHA_CREACION")
                    })
                End While
                Return resultado
            End Function)

        Dim respuesta As New ResultadoNotasWorkflow With {.TieneMas = filas.Count > tamanoPagina}
        For indice As Integer = 0 To Math.Min(tamanoPagina, filas.Count) - 1
            respuesta.Notas.Add(filas(indice))
        Next
        Return respuesta
    End Function

    Public Function Contar(ByVal contexto As ContextoModuloWorkflow,
                           ByVal tarea As TareaWorkflow,
                           ByVal solicitud As SolicitudContarNotasWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Contar
        If tarea Is Nothing OrElse solicitud Is Nothing OrElse solicitud.IdTarea <> tarea.IdTarea Then Return NoDisponible()

        Const sql As String = "SELECT COUNT(*) AS TOTAL " &
                              "FROM ANOTACION_TAREA AS at " &
                              "WHERE at.INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea " &
                              "AND at.ESTADO_TAREA = 1"
        Dim contador As Integer = EjecutarLectura(Of Integer)(contexto, sql,
            New List(Of IDataParameter) From {Parametro("@idTarea", tarea.IdTarea)},
            Function(reader As IDataReader) As Integer
                If Not reader.Read() OrElse reader.IsDBNull(reader.GetOrdinal("TOTAL")) Then Return 0
                Return Convert.ToInt32(reader.GetValue(reader.GetOrdinal("TOTAL")), CultureInfo.InvariantCulture)
            End Function)
        Return New ResultadoNotasWorkflow With {.Contador = Math.Max(0, contador)}
    End Function

    Public Function Crear(ByVal contexto As ContextoModuloWorkflow,
                          ByVal tarea As TareaWorkflow,
                          ByVal solicitud As SolicitudCrearNotaWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Crear
        Return NoDisponible()
    End Function

    Public Function Consultar(ByVal contexto As ContextoModuloWorkflow,
                              ByVal tarea As TareaWorkflow,
                              ByVal solicitud As SolicitudConsultarNotaWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Consultar
        If tarea Is Nothing OrElse solicitud Is Nothing OrElse solicitud.IdTarea <> tarea.IdTarea OrElse solicitud.IdNota <= 0 Then
            Return NoDisponible()
        End If

        Const sql As String = "SELECT at.ID_ANOTACION AS ID_NOTA, " &
                              "at.INICIO_TAREAS_WORKFLOW_ID_TAREA AS ID_TAREA, " &
                              "COALESCE(at.ID_USUARIO, 0) AS ID_AUTOR, " &
                              "COALESCE(at.ID_ACTIVIDAD, 0) AS ID_ACTIVIDAD_ORIGEN, " &
                              "at.DATO_ANOTACION AS CONTENIDO, " &
                              "at.FECHA_ANOTACION AS FECHA_CREACION " &
                              "FROM ANOTACION_TAREA AS at " &
                              "WHERE at.ID_ANOTACION = @idNota " &
                              "AND at.INICIO_TAREAS_WORKFLOW_ID_TAREA = @idTarea " &
                              "AND at.ESTADO_TAREA = 1 " &
                              "LIMIT 1"
        Dim nota As NotaWorkflow = EjecutarLectura(Of NotaWorkflow)(contexto, sql,
            New List(Of IDataParameter) From {Parametro("@idNota", solicitud.IdNota), Parametro("@idTarea", tarea.IdTarea)},
            Function(reader As IDataReader) As NotaWorkflow
                If Not reader.Read() Then Return Nothing
                Return New NotaWorkflow With {
                    .IdNota = EnteroLargo(reader, "ID_NOTA"),
                    .IdTarea = EnteroLargo(reader, "ID_TAREA"),
                    .IdAutorWorkflow = Entero(reader, "ID_AUTOR"),
                    .IdActividadOrigen = Entero(reader, "ID_ACTIVIDAD_ORIGEN"),
                    .Contenido = Texto(reader, "CONTENIDO"),
                    .FechaCreacionUtc = FechaUtc(reader, "FECHA_CREACION")
                }
            End Function)
        If nota Is Nothing Then
            Return New ResultadoNotasWorkflow With {.Codigo = CodigosResultadoNotasWorkflow.NoteNotFound,
                                                    .MensajeFuncional = "La nota solicitada no está disponible."}
        End If
        Return New ResultadoNotasWorkflow With {.Nota = nota}
    End Function

    Public Function Actualizar(ByVal contexto As ContextoModuloWorkflow,
                               ByVal tarea As TareaWorkflow,
                               ByVal solicitud As SolicitudActualizarNotaWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Actualizar
        Return NoDisponible()
    End Function

    Public Function Eliminar(ByVal contexto As ContextoModuloWorkflow,
                             ByVal tarea As TareaWorkflow,
                             ByVal solicitud As SolicitudEliminarNotaWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Eliminar
        Return NoDisponible()
    End Function

    Private Shared Function EsSolicitudListarValida(ByVal tarea As TareaWorkflow,
                                                     ByVal solicitud As SolicitudListarNotasWorkflow) As Boolean
        Return tarea IsNot Nothing AndAlso solicitud IsNot Nothing AndAlso solicitud.IdTarea = tarea.IdTarea AndAlso
               solicitud.TamanoPagina >= 1 AndAlso solicitud.TamanoPagina <= TamanoPaginaMaximo AndAlso
               (Not solicitud.FechaCursorUtc.HasValue OrElse solicitud.IdNotaCursor > 0)
    End Function

    Private Shared Function FechaUtc(ByVal reader As IDataReader, ByVal fieldName As String) As DateTime
        Dim ordinal As Integer = reader.GetOrdinal(fieldName)
        If reader.IsDBNull(ordinal) Then Return DateTime.MinValue
        Return DateTime.SpecifyKind(Convert.ToDateTime(reader.GetValue(ordinal), CultureInfo.InvariantCulture), DateTimeKind.Utc)
    End Function

    Private Shared Function NoDisponible() As ResultadoNotasWorkflow
        Return New ResultadoNotasWorkflow With {
            .Codigo = CodigosResultadoNotasWorkflow.Unavailable,
            .MensajeFuncional = "La persistencia moderna de notas no está disponible."
        }
    End Function
End Class
