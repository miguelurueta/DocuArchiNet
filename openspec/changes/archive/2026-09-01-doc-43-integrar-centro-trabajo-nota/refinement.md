<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-43-integrar-centro-trabajo-nota

## Fuente y alcance

- Ticket: `DOC-43` — INTEGRAR-CENTRO-TRABAJO-NOTA
- Cambio OpenSpec: `doc-43-integrar-centro-trabajo-nota`
- Fuente Jira: `specs/*/jira-context.md`
- Perfil tecnologico: legacy-webforms-vb-js; consumidor `workflow/Webworkflow.aspx(.vb)` y `js/workflow/Webworkflow.js`.

Este artefacto es la compuerta entre el ticket y la implementacion. No se aprueba por generacion automatica: una persona responsable debe confirmar alcance, decisiones, compatibilidad y evidencia de codigo.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx(.vb)`, `js/workflow/Webworkflow.js`, contratos modernos de Notas y contrato CSS del Centro de Trabajo.
- Se preservan GridView, postbacks, eventos y flujo legacy mientras el gate permanece deshabilitado; la ruta moderna usa `idTarea` explícito y el cliente único de Notas.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Integrar la UI moderna detrás de `WorkflowCentroTrabajoModernActive`, apagado por defecto y con fallback legacy intacto. | `workflow/Webworkflow.aspx(.vb)` y configuración del gate. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Usar un único adaptador JavaScript para listar, contar, crear, editar y eliminar Notas mediante JSON real y `idTarea` explícito. | `js/workflow/Webworkflow.js` y contratos ASMX modernos. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Renderizar contenido con `textContent`/equivalente seguro y mantener estados accesibles de carga, vacío, error, éxito y conflicto. | Adaptador y estilos bajo `.workflow-centro-trabajo-moderno`. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Validar responsive y regresión sin E2E autenticada hasta contar con autorización; rollback exacto es apagar gate y conservar legacy. | Matriz QA y `tools/e2e` existente. | D-04 | RQ-04 | Origen: D-04, RQ-04 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | La página muestra la experiencia moderna solo cuando el gate autoriza el contexto. | WHEN el gate está activo, THEN carga la UI moderna; WHEN está inactivo, THEN funciona el recorrido legacy sin doble operación. | Gate apagado por defecto; rollback desactivándolo. |
| RQ-02 | Todas las operaciones de Notas usan el adaptador único y `idTarea` explícito. | WHEN se lista o muta una Nota, THEN se invoca el contrato moderno sin `Session("ID_TAREA_SELECCIONDA")`, JSON concatenado ni doble escritura. | Mantener contratos backend sin cambios; revertir solo scripts/UI. |
| RQ-03 | La UI es segura y accesible en todos los estados. | WHEN hay contenido no confiable o conflicto, THEN se escapa y se informa sin bloquear controles; teclado, foco y Escape funcionan. | Evita XSS y bloqueos de interacción; rollback de assets modernos. |
| RQ-04 | La validación cubre responsive, fallback y regresión. | WHEN se ejecuta QA, THEN se registran 375/768/1024/1440 px, orientaciones móviles y evidencia saneada; E2E solo con autorización explícita. | No activar gate ni alterar usuarios/grupos; bloqueo explícito si falta ambiente. |

## Reglas de trazabilidad obligatorias

1. Cada decision `D-XX` debe estar desarrollada en `design.md`, reflejada en al menos un requirement/scenario de `spec.md` y vinculada a una tarea mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea con checkbox debe conservar su origen. Las tareas de validacion, rollout y documentacion tambien deben indicar la decision o requisito que verifican.
3. Las reglas de frontend, WebForms, Node u otro framework solo se agregan cuando el perfil tecnologico y el codigo afectado las justifican.
4. El estado solo puede cambiar a `approved` cuando no haya marcadores pendientes, las decisiones sean especificas y la matriz sea completa.

## Resultado del refinamiento

- Estado: approved. Decisiones y requisitos revisados contra el contexto Jira y el código legacy inspeccionado.
- Comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- <ISSUE-KEY> --sync`.
