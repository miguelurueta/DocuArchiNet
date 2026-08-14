## Context

DOC-9: CONTRATO-TERMINAR-TAREA-WORKFLOW

## Jira Details

> # Prompt 01 — Fundación paralela y contratos
> 
> ```text
> Actúa como arquitecto .NET Framework 4.6.1 / ASP.NET Web Forms enterprise.
> 
> Repositorio:
> D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net
> 
> Rol esperado:
> Arquitecto de software senior especialista en .NET Framework 4.6.1, ASP.NET Web Forms, VB.NET, MySQL y modernización gradual de sistemas empresariales legacy.
> 
> Objetivo:
> Crear una capa paralela para modernizar el envío de tareas workflow sin modificar ni duplicar la lógica crítica existente.
> 
> Restricciones críticas:
> - No debe modificarse `workflow/Webworkflow.aspx`, su code-behind ni la interfaz existente durante esta fundación.
> - No debe exponerse `Terminar_Tarea_Workflow`, `Cambia_Estado`, `Session`, SQL, credenciales ni controles Web Forms a JavaScript, DTOs o nuevas capas Presentation/Application.
> - No debe crearse un endpoint, modal, página, microservicio, repositorio genérico ni una segunda implementación de reglas legacy.
> - No debe retirarse ni alterarse el camino legacy; se debe preservar su comportamiento hasta una fase posterior con piloto, comparación de resultados y rollback aprobado.
> 
> Arquitectura obligatoria:
> Implementar una arquitectura limpia pragmática dentro de un monolito modular y aplicar patrón Strangler Fig. La nueva capa coexistirá con el flujo anterior; no se permite una reescritura total ni microservicios para este alcance.
> 
> Capas y dependencias:
> - Presentation: ASMX moderno y JavaScript; solo invoca Application y serializa DTO JSON.
> - Application: casos de uso, servicios y validadores; no puede depender de Page, Session, GridView, UpdatePanel ni ModalPopupExtender.
> - Domain: modelos e interfaces; no depende de ASP.NET, MySQL ni interfaz.
> - Infrastructure: repositorios MySQL y adaptadores; implementa interfaces de Domain.
> - Legacy Adapter: única capa nueva autorizada para llamar Terminar_Tarea_Workflow y Cambia_Estado.
> - No crear una segunda implementación de las reglas de terminación, transacción, firma, expediente, balanceo o eventos dinámicos.
> 
> Acceso a datos reutilizable obligatorio:
> - Crear Infrastructure/Shared/Data con IModuleConnectionFactory, IDataExecutor, ITransactionFactory, contratos/modelos comunes de resultado, paginación y auditoría técnica.
> - Crear Infrastructure/Repositories/Workflow solo para persistencia específica de Workflow.
> - IModuleConnectionFactory debe recibir un ContextoModuloWorkflow validado por Presentation/Application; los repositorios no pueden leer HttpContext.Current.Session.
> - Las nuevas consultas deben ser parametrizadas y devolver modelos tipados, no DataSet ni HTML.
> - No crear GenericRepository ni una abstracción genérica que mezcle dominios.
> - La infraestructura Shared/Data debe poder ser utilizada por Radicación, Expedientes y Gestión Documental, pero cada módulo definirá sus propios repositorios de negocio.
> 
> Contexto legado:
> - Entrada actual: workflow/Webworkflow.aspx y workflow/Webworkflow.aspx.vb.
> - Decisión ruta/flujo: ClassWorkflow.Validar_enviar_actividad_por_conector_flujo_o_ruta.
> - Núcleo de transición: ClassWorkflow.Terminar_Tarea_Workflow.
> - Cambio transaccional: ClassWorkflow.Cambia_Estado.
> - Flujo: Class_flujo_trabajo_workflow.
> - Ruta: Class_grupos_workflow y Class_actividades_disponibles_envio.
> - Eventos dinámicos obligatorios: PRETERMINARACTIVIAD y TERMINARACTIVIDAD.
> 
> Implementa únicamente la base paralela:
> 1. Crear carpeta workflow/modern/ respetando namespace y convenciones VB existentes.
> 2. Crear esta estructura mínima:
>    - Domain/Models
>    - Domain/Interfaces
>    - Application/DTOs
>    - Application/Services
>    - Application/Validators
> - Infrastructure/Repositories
> - Infrastructure/Shared/Data
> - Infrastructure/Configuration
> - Infrastructure/LegacyAdapters
> 3. Crear DTOs serializables:
>    - PrevisualizacionTransicionDto
>    - DestinoTransicionDto
>    - RequisitoTransicionDto
>    - ResultadoTransicionDto
>    - ErrorTransicionDto
>    Contrato mínimo: `PrevisualizacionTransicionDto` contiene id de tarea, origen, tipo de decisión y destinos; `DestinoTransicionDto` contiene id, nombre, tipo flujo/ruta y orden; `RequisitoTransicionDto` contiene código, descripción, obligatorio y satisfecho; `ResultadoTransicionDto` contiene éxito, estado, mensaje funcional, destinos y requisitos; `ErrorTransicionDto` contiene código funcional, mensaje visible y referencia de trazabilidad. Todos deben ser tipos VB explícitos y no filtrar HTML, SQL, credenciales ni excepciones internas.
> 4. Crear ServicioTransicionTarea como fachada.
> 5. Crear ProveedorTransicionesFlujo y ProveedorTransicionesRuta.
> 6. Crear ValidadorTransicionTarea.
> 7. Crear EjecutorTransicionTarea y WorkflowLegacyExecutorAdapter. En fases posteriores debe reutilizar Terminar_Tarea_Workflow.
> 8. Crear IModuleConnectionFactory y su implementación inicial; exponer únicamente conexiones autorizadas del módulo, nunca credenciales a DTO o JavaScript.
> 9. Crear los contratos de repositorio específicos: ITareaWorkflowRepository, ITransicionFlujoRepository, ITransicionRutaRepository, IConfiguracionConectorRepository e IAuditoriaTransicionRepository.
> 10. Crear el contrato base de habilitación, sin crear endpoints ni modificar la interfaz: `IWorkflowModernFeatureGate`, `HabilitacionWorkflowModernDto` y `EvaluadorHabilitacionWorkflowModern`.
>     - Debe evaluar servidor-side `WorkflowCentroTrabajoModernActive` por usuario, grupo o configuración y devolver `activo`, `inactivo` o `excluido` con código funcional no sensible.
>     - La implementación de configuración vive en `Infrastructure/Configuration`; recibe el `ContextoModuloWorkflow` validado y no consulta `Session` desde repositorios.
>     - Debe aplicar fail-closed: si falta configuración, existe inconsistencia o no hay habilitación explícita, el resultado es `inactivo`.
>     - Este contrato se reutiliza en Prompt 02 (preview), Prompt 03 (ejecución), Prompt 04 (bootstrap visual) y Prompt 06 (gobierno de piloto). Prompt 06 configura y opera el piloto; no crea una segunda bandera.
> 11. No crear endpoints ni modificar la interfaz existente.
> 12. Los DTO no deben devolver HTML, credenciales, SQL ni detalles internos de base de datos.
> 13. Documentar responsabilidad y límites de cada clase.
> 14. Agregar pruebas unitarias donde la arquitectura actual lo permita.
> 
> Pruebas obligatorias:
> - Ejecutar la compilación de la solución o proyecto afectado con MSBuild/.NET Framework y registrar comando, resultado y limitaciones reales.
> - Agregar o ajustar pruebas unitarias focales para DTOs, validadores y servicios que no requieran motor legacy.
> - Ejecutar QA manual reproducible: abrir el flujo actual, confirmar que no se modificó la interfaz ni la transición vigente, y registrar pasos, ambiente, resultado y evidencia.
> - E2E automatizada no aplica a esta fundación porque no crea endpoint ni UI nueva. Debe quedar explícita esa justificación y la evidencia de QA manual; en la fase que conecte la interfaz se definirá E2E compatible o su justificación técnica.
> 
> Documentación técnica:
> - Este prompt es autosuficiente: no depende de README ni de documentación externa para conocer su convención documental.
> - Raíz documental obligatoria, relativa a la raíz del repositorio: `Doc/Actualizacion/workflow/Terminar/01-fundacion-contratos/`.
> - Estructura obligatoria del paquete:
>     `Doc/Actualizacion/workflow/Terminar/01-fundacion-contratos/`
>     - `00-indice.md`
>     - `01-arquitectura.md`
>     - `02-contrato.md`
>     - `03-flujo-y-seguridad.md`
>     - `04-pruebas-y-evidencia.md`
>     - `Diagramas/`
> - `00-indice.md`: ticket, fecha, estado, alcance, archivos relacionados y resumen de cambios.
> - `01-arquitectura.md`: separación Presentation/Application/Domain/Infrastructure, responsabilidades, dependencias, reutilización de `Shared/Data`, decisiones y alternativas descartadas.
> - `02-contrato.md`: DTOs, parámetros, modelos, validaciones, errores funcionales, interfaces de repositorio y el contrato de `WorkflowLegacyExecutorAdapter`.
> - `03-flujo-y-seguridad.md`: límites de capas, autorización, concurrencia, límite exclusivo de acceso al motor legacy, compatibilidad, riesgos, piloto y rollback.
> - `04-pruebas-y-evidencia.md`: comandos, compilación, pruebas focales, QA manual, resultados, limitaciones y referencias de evidencia.
> - `Diagramas/`: diagramas Mermaid o fuentes estructuradas de capas, secuencia y fronteras legacy cuando correspondan.
> - Incluir una tabla con: clase o función, ruta, capa, parámetros/DTO, responsabilidad y dependencia legacy permitida.
> - El prompt fuente `01-fundacion-contratos.md` permanece en `Doc/Actualizacion/workflow/Terminar/`; no crear documentación de implementación junto a él, en la raíz del repositorio ni en rutas alternativas sin justificarlo expresamente en el entregable.
> 
> Entregable final:
> - Entregar los archivos creados y modificados con sus rutas, la explicación de dependencias, contratos DTO, documentación técnica y tabla de responsabilidades.
> - Entregar los comandos ejecutados, resultados de compilación y pruebas, evidencia de QA manual y la justificación de E2E no aplicable en esta fase.
> - Declarar expresamente que el comportamiento legacy fue preservado, qué no se modificó y qué fase posterior queda pendiente.
> 
> Criterios de aceptación:
> - El proyecto compila.
> - No cambia ningún flujo existente.
> - Las clases nuevas no dependen de Page, GridView, UpdatePanel ni ModalPopupExtender.
> - Solo WorkflowLegacyExecutorAdapter puede depender del motor workflow legado.
> - Los repositorios no consumen Session, no devuelven DataSet y usan consultas parametrizadas.
> - Shared/Data puede reutilizarse desde otro módulo sin depender de clases Workflow.
> - `IWorkflowModernFeatureGate` falla cerrado y es la única fuente nueva de evaluación de `WorkflowCentroTrabajoModernActive` para los flujos modernos.
> - La lógica de negocio no se traslada a JavaScript.
> - Se preserva el comportamiento legacy, sin regresiones en la interfaz ni en la transición de tareas existente.
> - Entregar archivos creados, dependencias, contratos DTO, documentación técnica y pruebas ejecutadas.
> ```

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. Aplicar politica central de AppResponses<T> para evitar parsers locales y filtrado de mensajes tecnicos en UI.

## Politica AppResponses<T>

- Los tickets que consuman `AppResponses<T>` deben centralizar mensajes visibles en `src/shared/api/appResponseError.ts`.
- No se deben duplicar parsers locales para resolver `UserMessage`, `requestId`, `code` o sanitizacion de mensajes tecnicos.
- `response.message` se considera potencialmente tecnico y solo puede mostrarse si el helper confirma que no contiene senales internas.
- El diagnostico completo queda limitado a `logAppResponseErrorDiagnostic` con `window.__APP_RESPONSE_DEBUG__ = true`; la consola puede activarse con `errorsDebugOn()` y apagarse con `errorsDebugOff()`.
- Esta politica es gradual: el bloqueo estricto de nuevos consumidores aplica cuando el helper existe fisicamente.


## Risks / Trade-offs

- Tickets existentes pueden tener parsers locales; la migracion debe ser gradual y enfocada en nuevos consumidores o cambios tocados por cada ticket.

## Migration Plan

1. Sembrar reglas AppResponses<T> en nuevos artefactos `opsxj:new`.
2. Usar `src/shared/api/appResponseError.ts` cuando el ticket consuma APIs con envelope AppResponses<T>.
3. Evitar bloqueo estricto hasta que el helper exista en la rama objetivo.

## Open Questions

- TBD
