Imports System.Data
Imports MySql.Data.MySqlClient

'Base de persistencia de Notas. DOC-40 no habilita lecturas ni escrituras; las seis operaciones responden fail-closed hasta que una fase posterior apruebe sus reglas y SQL parametrizado.
Public Class MySqlNotasWorkflowRepository
    Implements INotasWorkflowRepository

    Private ReadOnly _connectionFactory As IModuleConnectionFactory
    Private ReadOnly _dataExecutor As IDataExecutor

    Public Sub New()
        Me.New(New ModuleConnectionFactory("MyDbContext"), New AdoNetDataExecutor())
    End Sub

    Public Sub New(ByVal connectionFactory As IModuleConnectionFactory,
                   ByVal dataExecutor As IDataExecutor)
        _connectionFactory = connectionFactory
        _dataExecutor = dataExecutor
    End Sub

    Public Function Listar(ByVal contexto As ContextoModuloWorkflow,
                           ByVal tarea As TareaWorkflow,
                           ByVal solicitud As SolicitudListarNotasWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Listar
        Return NoDisponible()
    End Function

    Public Function Contar(ByVal contexto As ContextoModuloWorkflow,
                           ByVal tarea As TareaWorkflow,
                           ByVal solicitud As SolicitudContarNotasWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Contar
        Return NoDisponible()
    End Function

    Public Function Crear(ByVal contexto As ContextoModuloWorkflow,
                          ByVal tarea As TareaWorkflow,
                          ByVal solicitud As SolicitudCrearNotaWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Crear
        Return NoDisponible()
    End Function

    Public Function Consultar(ByVal contexto As ContextoModuloWorkflow,
                              ByVal tarea As TareaWorkflow,
                              ByVal solicitud As SolicitudConsultarNotaWorkflow) As ResultadoNotasWorkflow Implements INotasWorkflowRepository.Consultar
        Return NoDisponible()
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

    Private Shared Function NoDisponible() As ResultadoNotasWorkflow
        Return New ResultadoNotasWorkflow With {
            .Codigo = CodigosResultadoNotasWorkflow.Unavailable,
            .MensajeFuncional = "La persistencia moderna de notas aún no está disponible."
        }
    End Function

    'Punto único para la fase que autorice SQL de Notas: los valores de negocio solo se suministrarán mediante parámetros.
    Private Shared Function Parametro(ByVal nombre As String, ByVal valor As Object) As IDataParameter
        Return New MySqlParameter(nombre, If(valor, DBNull.Value))
    End Function
End Class
