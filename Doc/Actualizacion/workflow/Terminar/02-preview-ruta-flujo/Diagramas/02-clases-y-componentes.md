# Clases y componentes

```mermaid
classDiagram
    direction LR

    class WebServiceWorkflowModern {
        +PreviewEnviarTarea(idTarea: Long) PrevisualizacionTransicionDto
        -CrearServicioSinConexion() ServicioTransicionTarea
        -CrearRespuestaSegura(idTarea: Long) PrevisualizacionTransicionDto
    }
    class WorkflowPreviewSessionContextGate {
        +AsegurarContexto() ResultadoContextoSesionWorkflow
    }
    class ResultadoContextoSesionWorkflow {
        +Contexto: ContextoModuloWorkflow
        +CadenaConexionWorkflow: String
        +CadenaConexionDocuarchi: String
    }
    class ServicioTransicionTarea {
        +EvaluarHabilitacion(contexto) HabilitacionWorkflowModernDto
        +Previsualizar(contexto, idTarea) PrevisualizacionTransicionDto
    }
    class ValidadorTransicionTarea {
        +ValidarContexto(contexto) ErrorTransicionDto
    }
    class IWorkflowModernFeatureGate {
        <<interface>>
        +Evaluar(contexto) HabilitacionWorkflowModern
    }
    class ConfiguracionWorkflowModernFeatureGate
    class ITareaWorkflowRepository {
        <<interface>>
        +ObtenerTarea(contexto, idTarea) TareaWorkflow
    }
    class ITransicionFlujoRepository {
        <<interface>>
        +ObtenerDestinos(contexto, tarea) ResultadoDestinosTransicion
    }
    class ITransicionRutaRepository {
        <<interface>>
        +ObtenerDestinos(contexto, tarea) ResultadoDestinosTransicion
    }
    class MySqlTareaWorkflowRepository
    class MySqlTransicionFlujoRepository
    class MySqlTransicionRutaRepository
    class IModuleConnectionFactory {
        <<interface>>
        +CreateOpenConnection(contexto) IDbConnection
    }
    class WorkflowModuleConnectionFactory
    class DocuarchiModuleConnectionFactory
    class PrevisualizacionTransicionDto
    class ContextoModuloWorkflow
    class TareaWorkflow

    WebServiceWorkflowModern --> ServicioTransicionTarea
    WebServiceWorkflowModern --> WorkflowPreviewSessionContextGate
    WorkflowPreviewSessionContextGate --> ResultadoContextoSesionWorkflow
    WebServiceWorkflowModern --> ContextoModuloWorkflow
    WebServiceWorkflowModern --> WorkflowModuleConnectionFactory
    WebServiceWorkflowModern --> DocuarchiModuleConnectionFactory
    ServicioTransicionTarea --> ValidadorTransicionTarea
    ServicioTransicionTarea --> IWorkflowModernFeatureGate
    ServicioTransicionTarea --> ITareaWorkflowRepository
    ServicioTransicionTarea --> ITransicionFlujoRepository
    ServicioTransicionTarea --> ITransicionRutaRepository
    ServicioTransicionTarea --> PrevisualizacionTransicionDto
    ConfiguracionWorkflowModernFeatureGate ..|> IWorkflowModernFeatureGate
    MySqlTareaWorkflowRepository ..|> ITareaWorkflowRepository
    MySqlTransicionFlujoRepository ..|> ITransicionFlujoRepository
    MySqlTransicionRutaRepository ..|> ITransicionRutaRepository
    WorkflowModuleConnectionFactory ..|> IModuleConnectionFactory
    DocuarchiModuleConnectionFactory ..|> IModuleConnectionFactory
    MySqlTransicionRutaRepository --> WorkflowModuleConnectionFactory
    MySqlTransicionRutaRepository --> DocuarchiModuleConnectionFactory
    ITareaWorkflowRepository --> TareaWorkflow
```

Los puertos apuntan desde Application a Infrastructure. En ruta, Workflow aporta tarea/configuración/destinos y Docuarchi aporta exclusivamente el estado documental. Ninguna relación llega al ejecutor legacy.
