<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
# Tasks — DOC-15: base Enviar a grupo

## 1. Refinamiento trazable

- [x] 1.1 [S] Inspeccionar el camino legacy de grupo, el ASMX moderno, el guard, el gate y la frontera `Terminar_Tarea_Workflow`. Área/archivos: `workflow/Webworkflow.aspx.vb`, `workflow/Class_Listado_Actividades_workflow.vb`, `webservice/WebServiceWorkflowModern.asmx.vb`, `Infrastructure/Workflow/Terminar/`. Origen: D-01, RQ-01. Verificación: decisiones y evidencia de código registradas en `refinement.md`.
- [x] 1.2 [S] Reemplazar los artefactos genéricos por diseño, requisitos y tareas verificables de envío directo a actividad. Área/archivos: `openspec/changes/doc-15-base-enviar-grupo/`. Origen: D-09, RQ-07. Verificación: `refinement.md` aprobado sin marcadores pendientes.

## 2. Contratos y modelo de grupo

- [x] 2.1 [S] Incorporar DTOs de solicitud, preview y resultado de grupo con `IdTarea`, `IdActividadDestino` y `TokenVersion`, sin `IdConector`. Área/archivos: `Modelo/Workflow/Terminar/WorkflowModernModels.vb` y DTOs relacionados. Origen: D-01, RQ-01. Verificación: pruebas de contrato rechazan conector en la operación de grupo y preservan el contrato de transición existente.
- [x] 2.2 [M] Definir puertos y códigos funcionales para destinos, requisitos y ejecución directa de grupo. Área/archivos: `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb`, modelos de destino y códigos de bloqueo. Origen: D-01, RQ-03. Verificación: prueba unitaria compila contratos sin dependencia de `Page`, `Session` ni Infrastructure.

## 3. Preview seguro de actividades

- [x] 3.1 [M] Implementar el repositorio de lectura de actividades permitidas para envío a grupo por ruta. Área/archivos: `Infrastructure/Repositories/Workflow/`. Origen: D-03, RQ-02. Verificación: prueba de repositorio confirma consultas `SELECT`, pertenencia a ruta y ausencia de destinos no autorizados.
- [x] 3.2 [M] Implementar el servicio de preview de grupo con tarea activa, permiso, estado de ruta/flujo y token de versión. Área/archivos: `Services/Workflow/Terminar/ServicioEnvioGrupoTarea.vb` y validadores de grupo. Origen: D-03, RQ-02. Verificación: pruebas cubren permiso denegado, ruta/flujo cerrado, tarea no disponible y lista vacía sin escritura.
- [x] 3.3 [M] Exponer `PreviewEnviarGrupo` en el ASMX existente y componer sus dependencias de solo lectura desde el contexto autenticado. Área/archivos: `webservice/WebServiceWorkflowModern.asmx.vb`. Origen: D-02, RQ-01. Verificación: prueba del endpoint bloquea gate/sesión inválidos y no instancia adaptador, auditoría ni guard.

## 4. Ejecución directa revalidada

- [x] 4.1 [S] Validar la solicitud de grupo para tarea, actividad destino y token obligatorios, con códigos públicos estables. Área/archivos: `Services/Workflow/Terminar/ValidadorEnvioGrupoTarea.vb`. Origen: D-01, RQ-01. Verificación: pruebas unitarias rechazan identificadores no positivos y token vacío sin invocar repositorios.
- [x] 4.2 [M] Resolver dentro del lock el destino de actividad y revalidar permiso, tarea, token, ruta, flujo/actividad y pertenencia actual a ruta. Área/archivos: `Infrastructure/Repositories/Workflow/` y `Services/Workflow/Terminar/ServicioEnvioGrupoTarea.vb`. Origen: D-04, RQ-03. Verificación: pruebas de token vencido y destino retirado no alcanzan el adaptador.
- [x] 4.3 [S] Implementar el requisito de aprobaciones pendientes específico de grupo sin añadir validación de respuesta radicada. Área/archivos: `Infrastructure/Workflow/Terminar/` y puertos de requisitos. Origen: D-05, RQ-04. Verificación: prueba bloquea aprobación pendiente y prueba de regresión confirma que no invoca regla de respuesta.
- [x] 4.4 [M] Implementar el adaptador `ENVIO_GRUPO_DIRECTO` como único llamador nuevo de `Terminar_Tarea_Workflow`. Área/archivos: `Infrastructure/Workflow/Terminar/`. Origen: D-06, RQ-03. Verificación: prueba estática confirma `Page=Nothing`, conector/flujo en cero y ausencia de llamadas al motor desde ASMX, Application o JavaScript.
- [x] 4.5 [M] Orquestar gate, validación, guard, relectura, destino, requisitos, adaptador, auditoría y resultado en ese orden. Área/archivos: `Services/Workflow/Terminar/ServicioEnvioGrupoTarea.vb`. Origen: D-08, RQ-06. Verificación: prueba de concurrencia demuestra que a lo sumo una ejecución alcanza el adaptador y que una advertencia no revierte éxito.
- [x] 4.6 [M] Exponer `EjecutarEnvioGrupo` en el ASMX existente con composición de dependencias y respuesta segura. Área/archivos: `webservice/WebServiceWorkflowModern.asmx.vb`. Origen: D-02, RQ-01. Verificación: pruebas de llamada directa bloqueada y excepción controlada no exponen detalles internos.

## 5. Presentación y fallback

- [x] 5.1 [M] Condicionar los assets y bootstrap de grupo al gate existente sin crear claves ni evaluaciones paralelas. Área/archivos: `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb`, `workflow/WorkflowModernPresentationBootstrap.vb`. Origen: D-02, RQ-05. Verificación: prueba con gate inactivo conserva el postback legacy y no registra assets modernos de grupo.
- [x] 5.2 [M] Integrar selección, confirmación, teclado, Escape, foco y llamadas ASMX para el identificador de actividad destino. Área/archivos: JavaScript/CSS de Workflow y `workflow/Webworkflow.aspx`. Origen: D-07, RQ-05. Verificación: prueba JavaScript verifica payload de grupo y ausencia de `IdConector`, SQL, handlers o motor legacy.
- [x] 5.3 [S] Actualizar la vista tras éxito de grupo sin alterar el camino de Continuar flujo. Área/archivos: JavaScript de Workflow y pruebas `tests/workflow-transition-*.test.cjs`. Origen: D-07, RQ-05. Verificación: prueba de no regresión conserva endpoints, payload `IdConector` y adaptador actual de Continuar flujo.

## 6. Pruebas y evidencia

- [x] 6.1 [M] Agregar pruebas de contratos y preview para permiso, lectura exclusiva, ruta/flujo cerrado y destino fuera de ruta. Área/archivos: pruebas VB/JavaScript del área Workflow. Origen: D-03, RQ-02. Verificación: suite focal termina correctamente y declara archivos cubiertos.
- [x] 6.2 [M] Agregar pruebas de ejecución para aprobación pendiente, token vencido, destino retirado, concurrencia, auditoría y advertencias posteriores. Área/archivos: pruebas VB del servicio, repositorio y adaptador de grupo. Origen: D-04, RQ-03. Verificación: ninguna prueba produce transición duplicada, reasignación de respuesta ni filtración de detalle técnico.
- [x] 6.3 [M] Agregar pruebas de UI, accesibilidad, fallback y no regresión de Continuar flujo. Área/archivos: `tests/workflow-transition-*.test.cjs` y pruebas de interfaz de grupo. Origen: D-07, RQ-05. Verificación: gate inactivo, teclado, Escape, foco y payload de conector existente permanecen conformes.
- [x] 6.4 [S] Ejecutar MSBuild y las pruebas focales disponibles, registrando comandos, códigos de salida, cobertura y limitaciones reproducibles. Área/archivos: proyecto y paquete documental DOC-15. Origen: D-09, RQ-07. Verificación: evidencia de compilación y pruebas enlazada desde la documentación.
- [x] 6.5 [S] Ejecutar solo QA manual autorizado y documentar la ausencia de E2E/carga cuando no haya aprobación de ambiente. Área/archivos: `Doc/Actualizacion/workflow/TerminarGrupo/01-implementacion-envio-grupo/04-pruebas-y-evidencia.md`. Origen: D-09, RQ-07. Verificación: matriz QA cubre cancelación, éxito, bloqueo, error, responsive y accesibilidad sin modificar configuración.

## 7. Documentación, rollout y control

- [x] 7.1 [M] Crear o actualizar el paquete documental único que consolida contratos, preview, ejecución, adaptador, UI, gate, pruebas y liberación de grupo. Área/archivos: `Doc/Actualizacion/workflow/TerminarGrupo/01-implementacion-envio-grupo/`. Origen: D-09, RQ-07. Verificación: el paquete contiene índice, arquitectura, contrato, seguridad y evidencia aplicables, sin carpetas documentales por etapa.
- [x] 7.2 [S] Documentar el uso del gate único, la activación autorizada por ambiente y el rollback sin migración. Área/archivos: `Doc/Actualizacion/workflow/TerminarGrupo/01-implementacion-envio-grupo/`. Origen: D-02, RQ-07. Verificación: el runbook distingue aprobación técnica de autorización operativa y no crea una bandera de grupo.
- [x] 7.3 [S] Ejecutar auditoría de refinement y validación OpenSpec antes de implementar o cerrar el cambio. Área/archivos: `openspec/changes/doc-15-base-enviar-grupo/`. Origen: D-09, RQ-07. Verificación: `opsxj:refine --sync` y `openspec validate --strict` finalizan sin hallazgos de trazabilidad.
