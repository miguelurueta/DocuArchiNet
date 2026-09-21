<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Context

DOC-72 crea el núcleo frontend genérico para importar documentos desde servicios en `workflow/Webworkflow.aspx`. El backend moderno ya publica el contrato compartido y conserva la ejecución mutadora dentro de `ImportServiceOrchestrator`. La entrega coexiste con `btnloadservice`, depende del gate `WorkflowCentroTrabajoModernActive` y permanece desacoplada de conceptos SII.

## Decisions

### D-01 — Módulos aislados y cliente API único

Se crearán `importar-servicio-web-api.js`, `importar-servicio-web-core.js`, `importar-servicio-web-provider-registry.js` e `importar-servicio-web-ui.js`. API encapsula las operaciones ASMX con transporte inyectable; UI y core solo consumen su interfaz. Satisface RQ-01.

### D-02 — Registro por identidad y capacidades

El registro normaliza la identidad sin inferir proveedores. Cada adaptador declara selección múltiple, preview, descarga, tipología, requisitos adicionales y acciones. Los resultados para proveedor no configurado, no migrado o no soportado son explícitos; no existe fallback a SII. Satisface RQ-02.

### D-03 — Máquina de estados y mutación única

Core posee los estados `cerrado`, `resolviendo-proveedor`, `consultando`, `vacio`, `resultados`, `preparando`, `ejecutando`, `reconciliando`, `completado` y `error`. Coordina presentación y solicitudes, pero el backend conserva el orden mutador. Las invocaciones concurrentes de `execute(request)` reutilizan la misma promesa en curso y generan una sola llamada a `ExecuteImportIntent`; `ejecutando` es espera global indeterminada. La entrega no conecta selección ni confirmación visibles con esta capacidad programática. Satisface RQ-03 dentro del alcance de núcleo de DOC-72.

### D-04 — Integración aditiva bajo gate

`Webworkflow.aspx` alojará el modal y metadatos mínimos. `Webworkflow.aspx.vb` registrará CSS, scripts y bootstrap para la ruta moderna autorizada. `ctw-document-action-service` abre el modal; `btnloadservice` conserva legacy con gate apagado. Los módulos se agregan como `<Content>` al `.vbproj`. Satisface RQ-04.

### D-05 — Accesibilidad y estilos propios

El modal usa diálogo nombrado, foco inicial, ciclo/liberación de foco, Escape cuando sea seguro, restauración al disparador y regiones `aria-live`. El CSS queda aislado en `Styles/importar-servicio-web-modern.css`; no depende de `JSProgresBar`. Satisface RQ-05.

### D-06 — Verificación local y gobierno E2E

Las suites focales prueban core, registro/UI y accesibilidad con DOM y adaptadores falsos. También se ejecutan regresiones del gate. No se activa el gate ni se ejecuta E2E autenticado sin autorización; una futura corrida seguirá el runbook y restaurará configuración. Satisface RQ-06.

## Risks / Trade-offs

- UpdatePanel puede reemplazar nodos: el bootstrap será idempotente y reconsultará el DOM.
- Presentación visible no equivale a autorización; apertura y endpoints respetarán el gate real.
- El cliente normalizará el envoltorio ASMX sin inventar campos.
- La accesibilidad amplía la superficie de UI y exige pruebas focales de teclado y foco.

## Migration Plan

1. Crear módulos, estilos y pruebas con adaptador falso.
2. Registrar assets y marcado aditivo.
3. Conectar el contrato moderno solo bajo gate autorizado.
4. Validar suites y regresión con gate apagado.
5. Mantener rollback por configuración; DOC-72 no retira el puente legacy.

## Open Questions

- Ambiente y cuentas para E2E se definirán mediante autorización operativa separada.
