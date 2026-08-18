# Arquitectura y límites — DOC-14

`ConfiguracionWorkflowModernFeatureGate` es la única decisión de alcance. Requiere gate activo, metadatos completos y ausencia de exclusión. En modo piloto requiere inclusión explícita; en modo oficial requiere la bandera oficial y listas de piloto vacías. Si falta alguno, falla a legacy.

| Capa | Elementos | Responsabilidad | Límite preservado |
| --- | --- | --- | --- |
| Presentation | `Webworkflow.aspx(.vb)`, `WorkflowModernPresentationBootstrap` | Consume el bootstrap y emite assets solo si está activo | No consulta SQL ni autoriza transiciones |
| Application | `ServicioTransicionTarea` | Revalida gate, mide duración y devuelve resultado seguro | No usa controles Web Forms |
| Domain/modelo | `AuditoriaTransicion`, interfaces Workflow | Contrato tipado y sin Session ni secretos | No conoce infraestructura |
| Infrastructure | `WorkflowLegacyAuditoriaAdapter` | Normaliza y persiste auditoría mediante la bitácora existente | Único adaptador de auditoría legacy |
| Legacy | `WorkflowLegacyExecutorAdapter` | Ejecuta la transición autorizada ya existente | No se duplica ni se revierte por DOC-14 |

La auditoría moderna contiene solo identificadores autorizados, tarea, ruta, flujo, conector, destino, canal, duración, resultado, código y correlación. No incluye login, SQL, credenciales, Session, token, documento ni payload.

Alternativas descartadas: un flag visual paralelo, autorización en JavaScript, una segunda tabla de auditoría y reversión de negocio mediante SQL o una llamada adicional al motor legacy.
