<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento aprobado — DOC-30 Verificación transversal de Enviar a usuario

## Fuente y alcance

- Ticket: `DOC-30` — Verificación transversal de Enviar a usuario.
- Cambio OpenSpec: `doc-30-verificacion-transversal-enviar-usuario`.
- Fuente funcional: `specs/verificacion-transversal-enviar-usuario/jira-context.md`.
- Perfil tecnológico: ASP.NET Web Forms, VB.NET, ASMX y JavaScript legado.

DOC-30 es una compuerta de calidad sobre la implementación integrada de DOC-28/DOC-29. Solo produce evidencia y documentación; no cambia código de producción, configuración, estado de tareas, auditoría, datos ni contratos. No ejecuta E2E autenticado, carga, activación de gates, despliegue o publicación.

## Contexto inspeccionado

- `webservice/WebServiceWorkflowModern.asmx.vb`: endpoints directos `PreviewEnviarUsuario` y `EjecutarEnvioUsuario`.
- `Services/Workflow/Terminar/ServicioEnvioUsuarioTarea.vb` y `ValidadorEnvioUsuarioTarea.vb`: reglas de ejecución, requisitos, token y concurrencia.
- `Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioUsuario*Adapter.vb` y `Infrastructure/Repositories/Workflow/MySqlEnvioUsuarioRepository.vb`: frontera legacy, autorización, auditoría y consulta de destinos.
- `workflow/Webworkflow.aspx`, su code-behind y `js/workflow/workflow-user-send-*.js`: ruta moderna, lista paginada, confirmación y presentación correlacionada.
- `tests/workflow-user-send*.cjs`, `tests/confirmation-dialog.test.cjs`, pruebas de transición/grupo/gate y documentación técnica DOC-28/DOC-29: evidencia de regresión y límites.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | La verificación es reproducible y no mutante: solo compilación, pruebas locales, inspección estática y QA visual no autenticada. | `tests/`, solución MSBuild y documentación QA existente. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | El contrato directo conserva preview de lectura, revalidación servidor bajo lock, bloqueo de respuesta y auditoría sanitizada. | `WebServiceWorkflowModern.asmx.vb`, servicios y adaptadores de Envío a usuario. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | La UI moderna conserva búsqueda paginada, accesibilidad y aislamiento total de Grupo/Continuar flujo. | `Webworkflow.aspx`, `workflow-user-send-*.js` y pruebas CJS de UI. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | DOC-30 concluye con un dictamen técnico para operación posterior, sin desplegar ni editar configuración. | Paquete documental DOC-30 y matriz de resultados. | D-04 | RQ-04 | Origen: D-04, RQ-04 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Existe evidencia local reproducible y no mutante. | Al ejecutar las verificaciones permitidas se registran comandos, resultados, cobertura y límites. | No autoriza operación ni despliegue. |
| RQ-02 | Preview y ejecución conservan sus barreras de autorización y concurrencia. | La inspección y pruebas confirman lectura, token, lock, destino válido, bloqueo de respuesta y auditoría segura. | Evita doble transición, reasignación y exposición interna. |
| RQ-03 | La ruta de usuario conserva interfaz moderna aislada y accesible. | Las pruebas y QA cubren búsqueda, cursor, respuestas obsoletas, teclado, foco, Escape, responsive y bloqueo. | Grupo y Continuar flujo mantienen contratos con `IdConector`. |
| RQ-04 | El dictamen técnico es único y trazable. | La matriz concluye apto, bloqueado o requiere corrección y documenta riesgos. | La autorización por ambiente permanece fuera del ticket. |

## Reglas de trazabilidad

1. D-01 a D-04 se desarrollan en `design.md`, se reflejan en `spec.md` y se vinculan desde cada tarea mediante el formato `Origen: D-XX, RQ-XX`.
2. Las tareas de verificación, documentación y cierre conservan su decisión y requisito de origen.
3. Las comprobaciones respetan el perfil Web Forms/VB.NET/ASMX/JavaScript y no introducen políticas ajenas al código inspeccionado.

## Resultado del refinamiento

- Estado: aprobado para ejecutar la verificación técnica y documental.
- Resultado esperado: un dictamen técnico, no una liberación ni una modificación de ambiente.
