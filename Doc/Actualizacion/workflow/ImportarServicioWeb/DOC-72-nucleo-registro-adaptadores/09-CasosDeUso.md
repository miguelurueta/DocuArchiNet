# Casos de uso implementados

## UC-01 — Conservar recorrido legacy con gate apagado

- Actor: usuario Workflow.
- Precondiciones: `WorkflowCentroTrabajoModernActive=false`.
- Flujo principal: `Page_Load` entra a configuración, retorna antes de registrar DOC-72; `btnloadservice` conserva el listener de `js/workflow/Webworkflow.js`.
- Alternos/errores: no aplica; no se crea cliente moderno.
- Resultado: comportamiento legacy disponible, modal DOC-72 inerte.
- Participantes: `Webworkflow.ConfigureWorkflowTransitionModernPresentation`.
- Endpoint: ninguno.

## UC-02 — Inicializar recorrido moderno autorizado

- Actor: usuario Workflow autorizado por configuración compartida.
- Precondiciones: gate activo; página y controles presentes.
- Flujo: code-behind registra assets, bootstrap configura tarea/proveedor, `ImportarServicioWebUi.initialize` crea API/registro/core, enlaza disparador y oculta puente legacy.
- Alternos: nodo ausente, ya enlazado o módulo faltante devuelve `null` sin mutación.
- Resultado: una entrada visible y modal listo.
- Endpoint: ninguno durante inicialización.

## UC-03 — Abrir y consultar documentos

- Actor: usuario Workflow.
- Precondiciones: UC-02, tarea positiva y proveedor registrado.
- Flujo: `open` muestra modal → `core.open` resuelve registro → `core.query` → adaptador valida tarea → API llama `ResolveCapabilities` → API llama `QueryItems` → core entra en `resultados` o `vacio` → UI renderiza.
- Alternos: error funcional de capacidades, transporte inválido, respuesta vacía o excepción del adaptador conducen a `error` o `vacio`.
- Resultado: lista de `DisplayName` o mensaje de estado.
- Endpoints: POST `ResolveCapabilities`, POST `QueryItems`.
- Métodos: `JS.Ui.open`, `JS.Core.open`, `JS.Registry.resolve`, `JS.Core.query`, `VB.Endpoint.ResolveCapabilities`, `VB.Endpoint.QueryItems`.

## UC-04 — Rechazar proveedor inseguro

- Actor: bootstrap/configuración.
- Precondiciones: proveedor vacío, marcado no migrado o no registrado.
- Flujo: `registry.resolve` devuelve código; core transiciona a error; UI anuncia mensaje.
- Resultado: cero solicitudes HTTP y sin fallback a SII.
- Endpoint: ninguno.

## UC-05 — Cerrar y restaurar foco

- Actor: usuario de teclado/puntero.
- Precondiciones: modal abierto.
- Flujo: botón, backdrop o Escape llama `close`; core vuelve a `cerrado`, modal se oculta, body recupera scroll y foco vuelve al disparador.
- Alterno: sin disparador enfocable, cierre continúa sin restauración.
- Resultado: contexto/ejecución local limpiados.
- Endpoint: ninguno.

## UC-06 — Ejecutar una intención desde consumidor programático

- Actor: consumidor JavaScript del core; no existe control visible DOC-72 que lo dispare.
- Precondiciones: core en `resultados`, adaptador con `executeImportIntent`.
- Flujo: `execute(request)` → `preparando` → `ejecutando` → una llamada adaptador/API → `completado` o `error`.
- Alterno: llamadas concurrentes reciben la misma promesa; adaptador ausente produce `IMPORT_EXECUTION_UNAVAILABLE`.
- Resultado: snapshot final. No ejecuta reconciliación separada.
- Endpoint: POST `ExecuteImportIntent`.
- Métodos: `JS.Core.execute`, `VB.Endpoint.ExecuteImportIntent`.

## UC-07 — Rechazo seguro en la frontera ASMX

- Actor: navegador o cliente HTTP `<<external>>`.
- Precondiciones: solicitud a cualquiera de las ocho operaciones.
- Flujo: endpoint comprueba gate → solicitud base → contexto confiable → proveedor/composición según operación.
- Alternos: `FEATURE_DISABLED`, `INVALID_REQUEST`, códigos de sesión/tarea/proveedor y `IMPORT_UNAVAILABLE` según catch implementado.
- Resultado: DTO de respuesta con `Error`, sin lanzar detalle interno al cliente.
- Endpoints: los ocho inventariados.

## Capacidades no implementadas en la UI DOC-72

Preview, selección múltiple visible, tipología, preflight, creación de intención, botón de ejecución, reconciliación, inserción en lista documental y resultados por elemento no tienen controles ni flujo completo en este ticket. El cliente API expone nombres para esas operaciones, pero disponibilidad de método no equivale a caso de uso UI terminado.
