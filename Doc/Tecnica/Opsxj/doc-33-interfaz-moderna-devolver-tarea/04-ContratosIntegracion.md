# INTERFAZ-MODERNA-DEVOLVER-TAREA

- Ticket: DOC-33
- Cambio OpenSpec: doc-33-interfaz-moderna-devolver-tarea
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

Los endpoints ASMX permanecen autenticados y JSON:

| Operación | Payload mínimo | Uso UI |
| --- | --- | --- |
| `PreviewDevolverActividad` | `idTarea`, `termino`, `cursor`, `tamanoPagina` | Lista destinos autorizados y entrega token/cursor. |
| `EjecutarDevolverActividad` | `idTarea`, `idConector`, `tokenVersion` | Revalida y ejecuta el destino seleccionado. |

La respuesta ASMX se desempaqueta desde `d`. La UI usa campos publicados del preview: conector, actividad, destinatario/grupo resumido, contexto, token y paginación. El contrato de ejecución no contiene actividad final, usuario, grupo, Ruta, Flujo, `Page` ni información de infraestructura.

No hay handlers nuevos de servidor, cambios de esquema o conexiones adicionales. La compatibilidad se conserva porque DOC-33 registra sus assets aparte y no altera los módulos de Enviar a usuario, Enviar a grupo ni Continuar flujo.

## Corrida E2E de interfaz

`tools/e2e/tests/doc33-return-activity-ui.spec.cjs` usa la sesión compartida, el perfil no sensible `doc33` y ODBC de solo lectura. El perfil separa `uiExecutionTaskId` de `uiLockTaskId`; el orquestador reserva cada recurso localmente y solicita autorizaciones independientes `execution` y `ui_lock`.

La prueba de bloqueo retiene la respuesta de `EjecutarDevolverActividad` después del POST. Durante la espera, el contrato cliente exige que `ConfirmationDialog` rechace cancelación, X, fondo, Escape y API, y que `WorkflowReturnActivityUi` rechace el cierre de su modal mediante `executionPending`. No se transmite ni registra información adicional; el POST conserva la terna `idTarea`, `idConector` y `tokenVersion` del preview vigente.
