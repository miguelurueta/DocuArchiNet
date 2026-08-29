## Why

BACKEND-CONTRATOS-NOTAS. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-40.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> # Prompt 01 — Fundación backend y contratos de Notas
> 
> ## Prompt para ejecutar
> 
> ```text
> Aplica primero el contexto común de Prompt/00-guia-de-uso-y-contexto-comun.md.
> 
> Objetivo: crear la base aislada y reutilizable del backend moderno de Notas de Workflow, sin migrar aún consumidores ni habilitar escrituras productivas.
> 
> Rol esperado: arquitecto y desarrollador senior de ASP.NET Web Forms/VB.NET y MySQL, responsable de diseñar contratos de aplicación tipados y compatibles con el monolito modular existente.
> 
> Contexto obligatorio: revisa `WorkflowPreviewSessionContextGate`, `WebServiceWorkflowModern`, los proyectos de DTOs, `Modelo/Workflow`, `Services/Workflow`, `Infrastructure/Repositories/Workflow`, `AdoNetDataInfrastructure` y los documentos de Exploración de Notas antes de crear una ruta nueva. Ubica código nuevo en las carpetas equivalentes existentes de DTOs, modelos, servicios, interfaces y repositorios Workflow; los endpoints ASMX quedan en `webservice/` y no se usa una página WebForms como capa de dominio.
> 
> Restricciones críticas:
> - No modificar páginas, consumidores ni `WorkflowCentroTrabajoModernActive` en esta fase.
> - No copiar, envolver ni extender `Class_anotacion_tarea` como implementación moderna; no aceptar identidad, permiso ni tarea desde el cliente.
> - No hacer escrituras reales, migraciones de esquema, E2E autenticada ni consultas a base real sin la autorización aplicable.
> - Preserva contratos legacy y cambios ajenos; todo contrato moderno usa `idTarea` explícito y SQL parametrizado.
> 
> Reglas de anti-regresión: conserva el comportamiento legacy mientras no exista consumidor migrado, no modifica contratos públicos ajenos y no agrega atajos, wrappers ni dependencias que dupliquen una capacidad moderna existente.
> 
> Pruebas obligatorias: agrega pruebas focales de contratos, gate y resultados funcionales. Ejecuta MSBuild o `dotnet` si se modifica el proyecto VB, registra cada comando, resultado y evidencia saneada; si una prueba no procede, deja una justificación reproducible.
> 
> E2E no aplica: esta fase solo establece contratos y gate de fundación, y no expone un endpoint ni un recorrido de usuario. La E2E se integra con el mismo cambio que exponga el primer comportamiento verificable; no se crea una tarea E2E independiente.
> 
> Documentación técnica: actualiza la propuesta OpenSpec de Notas y los documentos de Exploración/requerimientos bajo `Doc/Actualizacion/workflow/Notas/` con contratos, decisiones pendientes, rutas afectadas y riesgos. No crees documentos en la raíz.
> 
> Entregable final: entrega DTOs, modelos, interfaces y contratos mínimos; lista de archivos modificados, pruebas y comandos ejecutados, resultados, decisiones de negocio pendientes y precondiciones verificables de la fase 02.
> 
> Alcance funcional: RF-07, RF-08, RF-12, RF-14 y RF-15; RS-01 a RS-09; RN-11 y RNF-10 del modelo de requerimientos. Deja preparada la estructura para RF-01 a RF-20, pero no dupliques lógica legacy.
> 
> 1. Crea o continúa una propuesta OpenSpec dedicada a modernización de notas. No reutilices doc-32-backend-actividad-anterior ni otros cambios activos.
> 2. Inspecciona los patrones existentes: WorkflowPreviewSessionContextGate, WebServiceWorkflowModern, DTOs y repositorios Workflow, además de AdoNetDataInfrastructure. Explica brevemente qué patrón reutilizas antes de modificarlo.
> 3. Diseña y crea los modelos, DTOs e interfaces de Notas en los namespaces/ubicaciones coherentes con el patrón moderno existente. Los contratos mínimos son listar, contar, crear, consultar, actualizar y eliminar; todos reciben idTarea explícito y las operaciones sobre una nota reciben también idNota.
> 4. Implementa un gate de contexto específico para notas. Debe resolver identidad, grupo y permiso de anotaciones en servidor, fallar cerrado y no aceptar autor, usuario, grupo ni permiso desde el cliente. No uses Session("ID_TAREA_SELECCIONDA") para ningún flujo moderno.
> 5. Define un puerto de acceso a tarea que valide, en cada solicitud, pertenencia al actor, selección/estado aplicable y datos necesarios del snapshot de tarea. Reutiliza o adapta con cuidado el patrón de ITareaWorkflowRepository.ObtenerTarea; no copies SQL ni lógica de UI.
> 6. Define resultados funcionales tipados y estables como Forbidden, TaskNotActive, NoteNotFound, NotOwner, VersionConflict, InvalidContent y Unavailable. No devuelvas excepciones, SQL ni detalles de infraestructura al navegador.
> 7. Establece la separación de capas: solo el transporte conoce ASMX y sesión; servicio, modelos y repositorios no conocen Page, GridView, UpdatePanel ni HttpContext.
> 8. Agrega pruebas unitarias de gate, mapeo de contratos y rechazo por sesión, permiso o tarea incompletos. No uses bases reales, E2E autenticado ni actives el gate.
> 
> Fuera de alcance: UI, JavaScript, migración del consumidor Centro de Trabajo Workflow, operaciones de escritura reales, cambios de esquema, consulta histórica y retiro de legacy.
> 
> Criterios de aceptación:
> - Cada contrato moderno exige idTarea y el compilador/pruebas demuestran que no hay fuente alternativa de tarea desde sesión.
> - El gate decide permiso en servidor y falla cerrado.
> - La capa moderna no depende de Class_anotacion_tarea ni de controles WebForms.
> - Existen DTOs y códigos funcionales que permiten a las fases posteriores no filtrar mensajes técnicos.
> - La propuesta y los cambios documentan las decisiones que quedan pendientes: borrado, histórico, supervisión, retención e idempotencia.
> 
> Al finalizar, no actives consumidores ni modifiques WorkflowCentroTrabajoModernActive. Reporta archivos, pruebas, riesgos y la ruta propuesta para la fase 02.
> ```

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: BACKEND, CONTRATOS, NOTAS

## Capabilities

### New Capabilities
- `backend-contratos-notas`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

