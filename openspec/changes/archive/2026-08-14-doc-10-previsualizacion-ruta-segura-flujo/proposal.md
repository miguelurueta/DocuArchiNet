## Why

PREVISUALIZACION-RUTA-SEGURA-FLUJO. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-10.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> # Prompt 02 — Previsualización segura de ruta y flujo
> 
> ```text
> Rol esperado:
> Arquitecto de software senior especialista en ASP.NET Web Forms .NET Framework 4.6.1, VB.NET, ASMX, seguridad de workflows y modernización gradual de sistemas legacy.
> 
> Objetivo:
> Implementar una API paralela de solo lectura para previsualizar destinos de una tarea workflow, sin modificar el comportamiento ni el contrato operativo del flujo actual.
> 
> Restricciones críticas:
> - No debe modificarse `workflow/Webworkflow.aspx`, su code-behind ni la implementación legacy existente.
> - No debe ejecutarse ningún cambio de estado, evento dinámico, envío de correo, firma, transacción ni transición real.
> - No debe invocarse `Terminar_Tarea_Workflow`, `Cambia_Estado`, `PRETERMINARACTIVIAD` ni `TERMINARACTIVIDAD` desde este endpoint.
> - No debe recibirse ni confiarse en IDs de usuario, grupo, ruta, actividad o permisos enviados por el navegador; el contexto se resuelve en servidor.
> - No debe exponerse SQL, credenciales, Session, HTML, DataSet, excepciones internas ni controles Web Forms en el JSON.
> - No se deben crear repositorios genéricos, duplicar reglas legacy ni retirar el camino anterior.
> 
> Tecnología obligatoria:
> - ASP.NET Web Forms .NET Framework 4.6.1.
> - Crear un ASMX paralelo, siguiendo el patrón existente de webservice/WebServiceWorkflow.asmx.
> - Usar ScriptService y WebMethod(EnableSession:=True).
> - JavaScript consume JSON; no devolver HTML de GridView.
> - El ASMX pertenece a Presentation: delega el caso de uso en Application y no contiene SQL, reglas de ruta/flujos ni acceso a controles Web Forms.
> - Los accesos a datos deben pasar por interfaces Domain e implementaciones Infrastructure/Repositories.
> - Los repositorios deben usar la infraestructura reutilizable `Infrastructure/Shared/Data`; no pueden leer `Session`, devolver `DataSet` ni construir HTML.
> 
> Crear:
> - webservice/WebServiceWorkflowModern.asmx
> - webservice/WebServiceWorkflowModern.asmx.vb
> 
> Endpoint:
> PreviewEnviarTarea(idTarea As Long) As PrevisualizacionTransicionDto
> 
> Reglas:
> 1. Validar sesión y usuario workflow autenticado.
> 2. Evaluar `IWorkflowModernFeatureGate` antes de resolver datos. Si el resultado no es `activo`, devolver bloqueo funcional `WORKFLOW_MODERN_INACTIVE`, sin destinos, sin invocar flujo legacy y sin hacer fallback automático.
> 3. Validar que la tarea exista, esté activa y sea accesible al usuario/grupo actual.
> 4. Resolver radicado, actividad actual, grupo y ruta.
> 5. Resolver si corresponde a flujo documental activo o a ruta.
> 6. Si es flujo, listar solo conectores permitidos desde nodo, actividad y usuario/grupo de origen reales.
> 7. Si es ruta, listar solo actividades permitidas para grupo, ruta y actividad actuales.
> 8. Nunca inferir permisos con IDs recibidos del navegador.
> 9. Devolver tipoTransicion, contexto, destinos, destinatario/grupo, requisitos, notificación y token de versión/concurrencia.
> 10. Si no hay destinos o existe inconsistencia, devolver bloqueo funcional legible.
> 11. No ejecutar PRETERMINARACTIVIAD, no cambiar estado y no enviar correo.
> 12. No modificar Webworkflow.aspx ni retirar la implementación anterior.
> 13. No invocar Terminar_Tarea_Workflow ni Cambia_Estado: este endpoint es estrictamente de lectura.
> 
> Pruebas obligatorias:
> - Tarea por flujo con uno y varios conectores.
> - Tarea por ruta.
> - Tarea inexistente, cerrada o no autorizada.
> - Flujo cerrado o nodo no válido.
> - Conector que no pertenece al origen actual.
> - Ejecutar compilación del proyecto o solución afectada con MSBuild/.NET Framework y registrar comando, resultado y limitaciones reales.
> - Ejecutar pruebas unitarias focales donde la arquitectura lo permita para validadores, servicios y mapeo de DTOs.
> - Ejecutar QA manual reproducible contra un usuario autorizado y uno no autorizado, registrando ambiente, pasos, resultado y evidencia.
> - E2E automatizada no aplica en esta fase si no existe infraestructura compatible; documentar la justificación y conservar evidencia de QA manual.
> 
> Documentación técnica:
> - Este prompt es autosuficiente: no depende de README ni de documentación externa para conocer su convención documental.
> - Raíz documental obligatoria, relativa a la raíz del repositorio: `Doc/Actualizacion/workflow/Terminar/02-preview-ruta-flujo/`.
> - Estructura obligatoria del paquete:
>   ```text
>   Doc/Actualizacion/workflow/Terminar/02-preview-ruta-flujo/
>     00-indice.md
>     01-arquitectura.md
>     02-contrato.md
>     03-flujo-y-seguridad.md
>     04-pruebas-y-evidencia.md
>     Diagramas/
>   ```
> - `00-indice.md`: ticket, fecha, estado, alcance, archivos relacionados y resumen de cambios.
> - `01-arquitectura.md`: capas, responsabilidades, dependencias, decisiones y alternativas descartadas.
> - `02-contrato.md`: entrada `PreviewEnviarTarea`, DTOs, JSON de ejemplo, validaciones, bloqueos funcionales, errores y compatibilidad.
> - `03-flujo-y-seguridad.md`: secuencia tarea → contexto → decisión flujo/ruta → destinos → respuesta; autorización, concurrencia, límite legacy, riesgos y rollback.
> - `04-pruebas-y-evidencia.md`: comandos, compilación, pruebas focales, QA manual, resultados, limitaciones y referencias de evidencia.
> - `Diagramas/`: diagramas Mermaid o fuentes estructuradas de componentes, secuencia y estados cuando correspondan.
> - El prompt fuente `02-preview-ruta-flujo.md` permanece en `Doc/Actualizacion/workflow/Terminar/`; no crear documentación de implementación junto a él, en la raíz del repositorio ni en rutas alternativas sin justificarlo expresamente en el entregable.
> - Cada archivo debe referenciar las rutas reales de ASMX, code-behind, Application, Domain e Infrastructure afectadas.
> - Documentar el contrato `PreviewEnviarTarea`: entrada, sesión requerida, autorizaciones, DTO de salida, bloqueos funcionales, errores controlados y token de concurrencia.
> - Documentar la secuencia de resolución servidor: tarea → radicado/actividad/grupo/ruta → decisión flujo/ruta → destinos permitidos → respuesta JSON.
> - Incluir tabla de funciones/clases con ruta, capa, parámetros, responsabilidad, datos consultados y dependencia legacy permitida.
> - Registrar decisiones de seguridad, compatibilidad preservada, riesgos, comandos, resultados y evidencia de pruebas.
> 
> Criterios de aceptación:
> - `PreviewEnviarTarea(idTarea)` devuelve exclusivamente una `PrevisualizacionTransicionDto` serializable y no devuelve HTML, `DataSet`, SQL ni detalle interno.
> - El servidor valida sesión, usuario workflow, existencia, estado y autorización de la tarea antes de resolver destinos.
> - El servidor valida `IWorkflowModernFeatureGate` para el usuario/grupo/configuración autorizados; una llamada ASMX directa fuera del piloto devuelve `WORKFLOW_MODERN_INACTIVE` y no expone destinos.
> - La respuesta distingue flujo de ruta, lista únicamente destinos autorizados y devuelve bloqueo funcional legible cuando no hay destino válido.
> - El endpoint no produce efectos secundarios: no cambia estado, no ejecuta eventos dinámicos, no firma, no envía correo y no invoca el motor de terminación.
> - Se preserva el comportamiento legacy y no se rompe la interfaz, transición ni ruta de envío vigente.
> - La compilación, pruebas focales y QA manual quedan registradas con resultados verificables.
> 
> Entregable final:
> - Entregar archivos creados o modificados con sus rutas, dependencias, contrato JSON de ejemplo y tabla de responsabilidades.
> - Entregar resultados de compilación, pruebas unitarias/focales, QA manual, comandos ejecutados, evidencia y limitaciones.
> - Declarar explícitamente qué comportamiento legacy se preservó, qué no se modificó y la justificación E2E de esta fase.
> 
> ```

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: PREVISUALIZACION, RUTA, SEGURA, WORKFLOW

## Capabilities

### New Capabilities
- `previsualizacion-ruta-segura-flujo`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

