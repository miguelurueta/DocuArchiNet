# Prompt 03 — Ejecución segura de la transición

```text
Rol esperado:
Arquitecto de software senior especialista en ASP.NET Web Forms .NET Framework 4.6.1, VB.NET, ASMX, seguridad transaccional y modernización gradual de workflows legacy.

Contexto:
- Repositorio: `D:\imagenesda\DocuachiNet\DocuArchiNet`.
- Interfaz y code-behind legacy: `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb`.
- Núcleo que se debe preservar: `ClassWorkflow.Terminar_Tarea_Workflow`, `ClassWorkflow.Cambia_Estado`, `PRETERMINARACTIVIAD` y `TERMINARACTIVIDAD`.
- La fundación y los contratos paralelos ya definidos viven en `Modelo/Workflow/Terminar/`, `DTOs/Workflow/Terminar/`, `Services/Workflow/Terminar/` e `Infrastructure/`; este prompt conecta la ejecución mediante esos límites tipados.

Objetivo:
Implementar el endpoint paralelo de ejecución de transición para la versión moderna, reutilizando el núcleo legacy sin duplicarlo y sin alterar el flujo vigente hasta completar piloto y rollback.

Restricciones críticas:
- No debe modificarse ni retirarse `workflow/Webworkflow.aspx`, su code-behind, la interfaz actual ni el camino legacy de envío.
- No debe invocarse `Terminar_Tarea_Workflow` ni `Cambia_Estado` desde JavaScript, ASMX, Application, repositorios o una segunda implementación; solo `WorkflowLegacyExecutorAdapter` puede hacerlo.
- No debe crearse una transacción paralela, repositorio genérico, endpoint sin validación de servidor, microservicio ni lógica de negocio en JavaScript.
- No debe confiarse en hidden fields, token, conector, usuario, grupo, ruta, actividad, permisos ni otros valores enviados por el navegador sin revalidarlos en servidor.
- No debe devolverse HTML, `DataSet`, SQL, credenciales, Session, controles Web Forms, excepciones internas ni trazas técnicas en JSON.
- No debe ocultarse ni retirarse una tarea del cliente hasta que el servidor confirme éxito funcional real.

Endpoint:
EjecutarEnvioTarea(idTarea As Long, idConector As Integer, tokenVersion As String) As ResultadoTransicionDto

Ubicación obligatoria del endpoint:
- Agregar `EjecutarEnvioTarea` al mismo ASMX paralelo creado en Prompt 02: `webservice/WebServiceWorkflowModern.asmx` y `webservice/WebServiceWorkflowModern.asmx.vb`.
- No crear un segundo ASMX moderno, una página adicional ni otro punto de entrada para preview o ejecución.

Contrato técnico:
- Entrada: `idTarea` identifica la tarea existente; `idConector` representa el destino solicitado; `tokenVersion` representa la versión mostrada en el preview. Ninguno sustituye la autorización ni el contexto resuelto en servidor.
- Respuesta `ResultadoTransicionDto`: `Exito`, `EstadoFinal`, `MensajeFuncional`, `CodigoBloqueo`, `Advertencias`, `ActividadDestino`, `Destino`, `TokenVersion`, `ReferenciaAuditoria` y `EsReintentable`.
- Error o bloqueo: devolver un código funcional estable y un mensaje visible; no serializar excepciones, SQL, rutas internas, credenciales ni detalle del motor legacy.
- Idempotencia: la misma operación no puede ejecutar dos transiciones efectivas para una misma tarea y versión; un reintento debe devolver el resultado conocido o un bloqueo controlado.

Regla arquitectónica:
El navegador solicita; el servidor decide, valida y ejecuta.

Estructura obligatoria:
- El ASMX moderno llama a ServicioTransicionTarea de Application.
- ServicioTransicionTarea usa interfaces Domain, validadores y EjecutorTransicionTarea.
- Los repositorios de ruta, flujo y tarea viven en Infrastructure/Repositories.
- Los repositorios usan `IModuleConnectionFactory` e infraestructura `Shared/Data`; no acceden directamente a Session ni exponen credenciales.
- WorkflowLegacyExecutorAdapter, en `Infrastructure/Workflow/Terminar/`, es el único punto que puede invocar Terminar_Tarea_Workflow y Cambia_Estado.
- No introducir una segunda transacción para el cambio de estado ni duplicar reglas del núcleo legado.

Antes de llamar Terminar_Tarea_Workflow, el servidor debe revalidar:
1. `IWorkflowModernFeatureGate`: si no está activo para el usuario/grupo/configuración, devolver `WORKFLOW_MODERN_INACTIVE` sin invocar el motor legacy ni hacer fallback automático.
2. Sesión y permisos.
3. Pertenencia de la tarea al usuario/grupo actual.
4. Que la tarea siga activa y coincida con tokenVersion.
5. Que idConector corresponda a la ruta o flujo actual de la tarea.
6. Para flujo: flujo, nodo origen, actividad origen y usuario/grupo origen.
7. Para ruta: grupo, ruta y actividad origen.
8. Respuesta o confirmación requerida.
9. Solicitudes de aprobación pendientes.
10. Requisitos de firma, expediente, copia documental y balanceo.

Ejecución:
- Reutilizar ClassWorkflow.Terminar_Tarea_Workflow y ClassWorkflow.Cambia_Estado.
- Hacerlo exclusivamente a través de WorkflowLegacyExecutorAdapter.
- Mantener PRETERMINARACTIVIAD en servidor y bloquear si falla.
- Mantener TERMINARACTIVIDAD, correo y trazabilidad actuales.
- No retirar la tarea de la interfaz hasta recibir éxito real.
- Evitar doble envío con control de concurrencia e idempotencia.
- Devolver JSON estructurado: éxito, bloqueo, advertencia, actividad destino y estado final.

Seguridad obligatoria:
- No confiar en hidden fields ni valores JavaScript.
- No ejecutar una transición solo porque exista un conector.
- Registrar auditoría: usuario, tarea, origen, destino, mecanismo, fecha y resultado.
- Si el estado de flujo es inconsistente, bloquear con mensaje controlado.

Pruebas obligatorias:
- Éxito por ruta y por flujo.
- Doble clic y dos usuarios sobre la misma tarea.
- Conector alterado.
- PRETERMINARACTIVIAD exitoso y fallido.
- Firma, expediente, autorización y respuesta faltante.
- Ejecutar compilación del proyecto o solución afectada con MSBuild/.NET Framework y registrar comando, resultado y limitaciones reales.
- Agregar o ajustar pruebas unitarias focales para validadores, idempotencia, DTOs, servicios y mapeos que no requieran ejecutar el motor legacy.
- Ejecutar QA manual reproducible para ruta, flujo, bloqueo, concurrencia y resultado visible, registrando ambiente, pasos, resultado y evidencia.
- E2E automatizada no aplica si el repositorio no cuenta con infraestructura compatible para Web Forms; registrar la justificación y la evidencia de QA manual. Si existe infraestructura disponible, ejecutar el recorrido end-to-end y adjuntar el resultado.

Documentación técnica:
- Este prompt es autosuficiente: no depende de README ni de documentación externa para conocer su convención documental.
- Raíz documental obligatoria, relativa a la raíz del repositorio: `Doc/Actualizacion/workflow/Terminar/03-ejecucion-segura/`.
- Estructura obligatoria del paquete:
    `Doc/Actualizacion/workflow/Terminar/03-ejecucion-segura/`
    - `00-indice.md`
    - `01-arquitectura.md`
    - `02-contrato.md`
    - `03-flujo-y-seguridad.md`
    - `04-pruebas-y-evidencia.md`
    - `Diagramas/`
- `00-indice.md`: ticket, fecha, estado, alcance, archivos relacionados y resumen de cambios.
- `01-arquitectura.md`: capas, responsabilidades, dependencias, decisiones, alternativas descartadas y el límite único de `WorkflowLegacyExecutorAdapter`.
- `02-contrato.md`: entrada, `ResultadoTransicionDto`, DTOs relacionados, JSON de ejemplo, validaciones, bloqueos, idempotencia, errores funcionales y compatibilidad.
- `03-flujo-y-seguridad.md`: secuencia revalidación → PRETERMINARACTIVIAD → motor legacy → TERMINARACTIVIDAD → auditoría; autorización, concurrencia, límites, riesgos, piloto y rollback.
- `04-pruebas-y-evidencia.md`: comandos, compilación, pruebas focales, QA manual, E2E o justificación, resultados, limitaciones y referencias de evidencia.
- `Diagramas/`: diagramas Mermaid o fuentes estructuradas de componentes, secuencia, concurrencia y estados cuando correspondan.
- Incluir una tabla con: clase o función, ruta, capa, parámetros/DTO, responsabilidad y dependencia legacy permitida.
- El prompt fuente `03-ejecucion-segura.md` permanece en `Doc/Actualizacion/workflow/Terminar/`; no crear documentación de implementación junto a él, en la raíz del repositorio ni en rutas alternativas sin justificarlo expresamente en el entregable.

Criterios de aceptación:
- `EjecutarEnvioTarea` revalida en servidor sesión, permisos, pertenencia, estado, token de versión, conector y requisitos de negocio antes de invocar el motor legacy.
- `EjecutarEnvioTarea` vive en `WebServiceWorkflowModern` y rechaza llamadas directas fuera del piloto mediante `IWorkflowModernFeatureGate`, sin ejecutar transición ni fallback automático.
- Solo `WorkflowLegacyExecutorAdapter` invoca `Terminar_Tarea_Workflow` y `Cambia_Estado`; no existe una segunda transacción ni duplicación de reglas.
- Los eventos `PRETERMINARACTIVIAD` y `TERMINARACTIVIDAD`, correo y trazabilidad se conservan en servidor con el comportamiento legacy aplicable.
- La operación evita doble envío y entrega resultado idempotente o bloqueo funcional controlado ante concurrencia.
- El JSON no filtra datos internos y registra auditoría con usuario, tarea, origen, destino, mecanismo, fecha y resultado.
- La interfaz y el flujo legacy se preservan sin regresiones; compilación, pruebas focales y QA manual quedan registrados con evidencia.

Entregable final:
- Entregar los archivos creados o modificados con rutas, capas, dependencias y tabla de responsabilidades.
- Entregar contrato JSON de ejemplo, reglas de validación, límites legacy, documentación del paquete obligatorio y diagramas aplicables.
- Entregar comandos ejecutados, resultados de compilación/pruebas, evidencia de QA manual, E2E o justificación, riesgos y limitaciones.
- Declarar explícitamente qué comportamiento legacy se preservó, qué no se modificó y cuál es el criterio de piloto/rollback para habilitar la versión moderna.
```
