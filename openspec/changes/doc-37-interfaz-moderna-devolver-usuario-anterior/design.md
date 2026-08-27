<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Context

DOC-36 ya resuelve el usuario histórico, actividad, token opaco, autorización y concurrencia en el servidor. La página Web Forms todavía ofrece Usuario anterior mediante `D-TWU-ANT`, que llama a JavaScript heredado, dispara `Button_tool_devolver_a_usuario` y usa un handler de página. DOC-37 elimina únicamente esa ruta y conecta el menú al contrato de servidor existente.

## Goals / Non-Goals

**Goals:** presentar una confirmación moderna única, accesible y aislada para devolver al usuario histórico; conservar el token opaco; mantener la bandeja coherente después del resultado; dejar una arquitectura E2E DOC-37 reutilizable sobre los controles y recursos de DOC-36.

**Non-Goals:** cambiar DOC-36, crear selector o búsqueda de destinos, modificar feature gates, cambiar otras operaciones Workflow, aprovisionar o configurar un ambiente, o ejecutar E2E autenticada sin ambiente y cuentas de prueba autorizados.

## Decisions

### D-01 — Registro independiente del feature gate

`ConfigureWorkflowTransitionModernPresentation` registrará la nueva presentación junto con las operaciones que ya se registran antes de evaluar `WorkflowTransitionModernActive`. El bootstrap se limita a los IDs de tarea ya presentes y no introduce autorización de cliente.

### D-02 — Sustitución puntual de la ruta heredada

El menú usará `workflow-return-user-previous-trigger`, de tipo `button`, sin `onclick`. Se retiran `D-TWU-ANT`, el `asp:Button` oculto, el handler de code-behind, la declaración del diseñador y las ramas JavaScript que nombran ese botón. El trigger y modal de devolución de actividad no se modifican funcionalmente.

### D-03 — Adaptador y contratos exclusivos

`workflow-return-user-previous-ui.js` hace `POST` a `PreviewDevolverUsuarioAnterior` con `{ idTarea }`, interpreta el envoltorio ASMX y guarda solo `{ idTarea, tokenVersion, actividadAnterior, usuarioAnterior }`. `workflow-return-user-previous-confirmation.js` confirma y hace `POST` a `EjecutarDevolverUsuarioAnterior` con `{ idTarea, tokenVersion }`. El código no consume endpoints, payloads, selectores ni eventos de actividad anterior, envío a usuario o envío a grupo.

### D-04 — Interacción accesible y recuperación

El modal propio muestra un único destino histórico o el bloqueo funcional del preview. Reutiliza clases de `workflow-transition-modern.css`, foco inicial, trampa de foco, Escape, backdrop y restauración del foco al trigger. La confirmación reutiliza `ConfirmationDialog`, que deshabilita confirmar, cancelar y cerrar mientras ejecuta. Cada request usa `AbortController`; un timeout controlado de quince segundos aborta la request, conserva la bandeja y permite reintentar desde un estado seguro.

### D-05 — Resultado localizado

En éxito se cierra el modal propio y `WorkflowTransitionPagePresentation.applySuccess` elimina solo la fila de `idTarea`, actualiza contador, visor, listado y scroll, y muestra mensaje correlacionado. Los bloqueos, conflictos y fallas técnicas no invocan esa actualización.

### D-06 — Harness E2E DOC-37 reutilizable y aislado

`tools/e2e/` registra `doc37` como una corrida distinta de DOC-36, pero reutiliza su patrón de perfil no sensible, sesión efímera, ODBC de solo lectura y ciclo de reservas. Un adaptador DOC-37 describe dos tareas descartables distintas: `uiExecution` y `uiLock`. El runner ofrece solo `preview`, `execution` y `ui-lock`, pero exige exactamente una etapa por invocación porque Workflow conserva una única tarea seleccionada. En una sesión nueva, la prueba establece esa precondición exclusivamente al pulsar el comando oficial de la bandeja para la tarea autorizada y espera que Workflow confirme la selección; nunca escribe campos ocultos, sesión ni llama servicios internos para simularla. Las huellas de estado y auditoría se toman después de esa precondición, por lo que prueban que preview no introduce cambios propios. Si la tarea no está disponible, la prueba falla cerrada antes de preview o ejecución. Cada etapa exige las autorizaciones explícitas correspondientes. El perfil no almacena actividad, usuario, destino ni token: la prueba toma la actividad anterior del preview vigente en memoria y la usa exclusivamente para controlar el resultado de esa misma ejecución. La especificación Playwright verifica el trigger y modal exclusivos, los endpoints DOC-36 y el bloqueo de confirmación. No persiste credenciales, cookies, tokens, respuestas, usuario o destino.

## Risks / Trade-offs

- El menú puede presentarse sin destino elegible; se acepta porque el preview del servidor es la única fuente de elegibilidad.
- El HTML Web Forms necesita retirar también diseñador y code-behind para que no sobreviva un fallback de postback.
- El timeout del navegador no cancela una mutación ya recibida por el servidor; por eso el diálogo bloquea abandono mientras espera y la recuperación solicita un preview nuevo antes de otro intento.
- La infraestructura no equivale a autorización operacional: sin perfil, ambiente y cuentas de prueba expresamente aprobados, se ejecutan solamente pruebas locales de política y del orquestador.

## Migration Plan

1. Registrar scripts y bootstrap propios, sustituir el trigger y retirar la ruta heredada.
2. Implementar preview, modal y confirmación aislados usando el contrato DOC-36.
3. Agregar pruebas CJS estáticas y de contrato, ejecutar compilación y pruebas focales.
4. Registrar el perfil/runner DOC-37, el adaptador de recursos y la prueba UI reutilizando los controles DOC-36.
5. Ejecutar las pruebas locales de política y orquestación, y después las etapas autenticadas de una en una cuando ambiente, cuentas y tareas estén autorizados.
