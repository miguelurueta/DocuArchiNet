# E2E autorizada — concurrencia RUP confirmada

- Fecha: 2026-09-11, zona America/Bogota.
- Ambiente: certificación local autorizada, con TLS autofirmado temporal.
- Escenario: `import-sii-concurrency`.
- Recurso: tarea descartable RUP expresamente autorizada; los identificadores sensibles o de cuenta no se conservan en esta evidencia.
- Concurrencia: nivel fijo y máximo autorizado de 2 solicitudes simultáneas.
- Resultado: aprobado; una ejecución fue aceptada y la otra fue bloqueada mediante `VERSION_CONFLICT`.

## Resultado documental

- Solicitudes concurrentes reales: 2.
- Documento confirmado después de la carrera: 1.
- Código de confirmación: `CONFIRMED`.
- Bloqueos optimistas esperados: 1.
- Errores inesperados: 0.
- El recurso recorrió `E2E_RESOURCE_READY`, `E2E_RESOURCE_RESERVED` y `E2E_RESOURCE_CONSUMED`.

## Tiempos observados

| Medición | Resultado |
|---|---:|
| observaciones | 6 |
| mínimo | 8 ms |
| promedio | 829 ms |
| p50 | 26 ms |
| p95 | 3417 ms |
| p99 | 3417 ms |
| máximo | 3417 ms |
| ventana de carrera | 3417 ms |
| throughput de la carrera | 0.59 solicitudes/s |

Las latencias saneadas fueron `1434`, `68`, `26`, `21`, `3417` y `8` ms. La ventana concurrente corresponde a la mayor de las dos invocaciones de ejecución (`21` y `3417` ms).

## Controles de cierre

- Controles `SELECT`: 3 comprobados, con cambio esperado por la operación mutante.
- Gate al finalizar: `false`.
- Usuarios y grupos del gate: vacíos.
- Páginas Workflow legacy: sin cambios.
- No se conservaron credenciales, cookies, tokens, cadenas de conexión ni cuerpos externos.

Fuente saneada: `tools/e2e/artifacts/workflow-e2e-platform-import-sii-concurrency.json`.

La recuperación de la intención concurrente se registra por separado para completar la tarea 4.6.

## Estados y recuperación posterior

El escenario concurrente ejecutó `GetImportIntent` inmediatamente después de las dos llamadas a `ExecuteImportIntent`. El adaptador solo publicó `storedDocument=CONFIRMED` después de que `assertSingleStoredDocument` comprobara exactamente un item `Disponible`, `DocumentId` positivo y `PersistenceKnown=true`.

Una consulta posterior exclusivamente `SELECT`, parametrizada por la tarea, confirmó:

- estado de la intención: `Completada`;
- tipología: ID `175`, nombre `Constancia`;
- código de error del item: vacío;
- persistencia conocida: `true`;
- documento presente: `true`.

El resultado de la carrera demuestra el rechazo de la segunda solicitud con un token ya desplazado mediante `VERSION_CONFLICT`; no se efectuó un segundo almacenamiento. La intención completada no se reintentó, porque ya posee documento confirmado.

Una recuperación aislada posterior desde una sesión nueva fue rechazada antes del repositorio con `IMPORT_E2E_GET_FAILED_FORBIDDEN`. Sus tres controles permanecieron sin cambios y no consumió recursos. Este rechazo se conserva como evidencia de fallo cerrado del contexto; no invalida la recuperación ejecutada dentro de la sesión concurrente ni la confirmación `SELECT` posterior.
