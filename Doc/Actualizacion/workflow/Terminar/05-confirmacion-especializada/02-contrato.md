# Contrato técnico

## API genérica

```javascript
ConfirmationDialog.open(config);
ConfirmationDialog.close();
```

`config` recibe `title`, `primaryLabel`, `cancelLabel`, `summaryFields`, `requirements`, `warnings`, `confirmationNotice`, `executionContext`, `execute`, `normalizeResult`, `onSuccess`, `onBlocked`, `onTechnicalError`, `onCancel` y `labels`.

- `executionContext` es opaco para el componente.
- `execute(executionContext)` devuelve una promesa del consumidor.
- `normalizeResult` devuelve `{ status, message, warnings, canRetry, reference }`, donde `status` es `success`, `blocked` o `technical-error`.
- El estado interno `ignored` descarta de forma silenciosa una respuesta que ya no corresponde a la apertura vigente; no es un estado visible.
- Antes de enviar, Escape se captura en `window` para cancelar la confirmación antes de que un modal legacy pueda consumir la tecla. Durante `enviando`, `close()` devuelve `false`; `X`, Cancelar, fondo, Escape y una nueva apertura no cierran el diálogo. `labels.sendingLocked` define el aviso accesible de espera.
- El componente registra `beforeunload` solo mientras envía; el navegador puede mostrar una confirmación nativa al cerrar o recargar, pero el cliente no promete cancelar una operación que ya alcanzó el servidor.
- Un rechazo de red o excepción cliente usa siempre `labels.technicalError`; no se muestra el texto de `Error.message` ni detalles del navegador.

## Selección y ejecución

| Función o selector | Ruta | DTO/parámetros | Responsabilidad | Estado UI | Dependencia legacy permitida |
| --- | --- | --- | --- | --- | --- |
| `workflow:destination-selected` | `workflow-transition-ui.js` | tarea, conector, token, tipo, radicado, grupo y destino | Publicar selección normalizada | `confirmando` | Ninguna |
| `ConfirmationDialog.open` | `ConfirmationDialog.js` | Configuración genérica | Mostrar confirmación; rechazar reemplazo durante `enviando` | Todos | Ninguna |
| `executeSend` | `workflow-transition-confirmation-integration.js` | `{ idTarea, idConector, tokenVersion }` | POST asíncrono de mismo origen | `enviando` | ASMX existente |
| `normalizeResult` | `workflow-transition-confirmation-integration.js` | `ResultadoTransicionDto` | Clasificar éxito, bloqueo o error técnico | `exito`, `bloqueo-funcional`, `error-tecnico-controlado` | Ninguna |
| `WorkflowTransitionPagePresentation.applySuccess` | `workflow-transition-page-presentation.js` | detalle correlacionado | Retirar la fila si existe y restaurar lista, contexto, visor, acciones y contador; anunciar éxito por seis segundos | Después de `exito` | Atributos `data-workflow-*` |

## Solicitud ASMX

```http
POST ../webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioTarea
Content-Type: application/json; charset=utf-8
credentials: same-origin

{"idTarea":41,"idConector":8,"tokenVersion":"v-41"}
```

El adaptador no agrega usuario, grupo, ruta, permisos, requisitos ni otra información derivada.

## Resultado del servidor

`ResultadoTransicionDto` suministra `Exito`, `EstadoFinal`, `MensajeFuncional`, `CodigoBloqueo`, `Advertencias`, `ActividadDestino`, `Destino`, `TokenVersion`, `ReferenciaAuditoria` y `EsReintentable`.

La respuesta solo afecta la interfaz si corresponde a la apertura actual y el `TokenVersion` coincide. `Exito = true` habilita el callback visual; bloqueo o error técnico conservan fila, visor y contador. La cancelación solo está disponible antes del envío o después de recuperar acciones.
