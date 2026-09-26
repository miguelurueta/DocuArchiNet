# Estados, idempotencia y reconciliación

## Máquina de estados ENLASE

```text
Creada -> Validada -> RecursoObtenido -> ExpedientePreparado
       -> IndicesActualizados -> DocumentoAlmacenado -> Reconciliada -> Completada

Cualquier frontera mutadora puede terminar en:
- FallidaAntesDePersistir: efecto descartado/conocido; retry solo si la fábrica lo permite.
- Parcial: mezcla de resultados por elemento.
- ResultadoIncierto: el efecto pudo ocurrir; retry automático prohibido.
- Detenida: no inicia efectos pendientes.
```

Aunque los nombres `ExpedientePreparado` e `IndicesActualizados` son compartidos por el pipeline histórico, ENLASE usa `ModoExpedienteImportacion.SinExpediente`; `EstadoRelacion` y `EstadoCache` permanecen `NoAplica`. No se crea ni vincula un expediente.

## Idempotencia y concurrencia

- Una intención cubre toda la colección seleccionada.
- `ServicioIntencionImportacion.Canonical` incluye contexto inmutable, `ProviderId`, capacidad, `ProviderReference`, identidad externa y tipología ordenada.
- Una URL SII nunca participa como identidad; `ExternalKey` es el `idanexo` opaco.
- `IImportIntentConcurrencyGuard` serializa la creación/ejecución y el repositorio usa restricciones de unicidad/versionado.
- Una intención ya completada se devuelve sin repetir pasos.
- Un resultado incierto tiene `Reintentable=False`; requiere `ReconcileImportIntent`.

## Matriz de reconciliación

| Evidencia | Estado por elemento | ID público | Reintento |
|---|---|---|---|
| Un registro, una relación de la tarea, un recurso físico | `Disponible` | Sí | Omitido idempotente |
| Registro lógico y recurso físico ausente | `Recuperable` / `DOCUMENT_PHYSICAL_RESOURCE_MISSING` | No | Nueva ejecución controlada |
| Más de una relación | `Inconsistente` / `DOCUMENT_RELATION_DUPLICATED` | No | No automático |
| Relación con otra tarea | `Inconsistente` / `DOCUMENT_TASK_MISMATCH` | No | No automático |
| Respuesta perdida o evidencia no consultable | `ResultadoIncierto` / `IMPORT_RESULT_UNCERTAIN` | No | Reconciliación explícita |
| Mezcla de estados | agregado `Parcial` | Solo confirmados | Por elemento según estado |

`MySqlImportReconciliationRepository` filtra por usuario, tarea, proveedor, capacidad e intención; `ObtenerItem` añade `ExternalKey`. La relación se consulta en `logdocuarchi` por documento y tarea. El gabinete debe cumplir el identificador seguro antes de consultar `SELECT ID ... LIMIT 2`; solo un resultado confirma existencia física.

## Mapping legacy

| Resultado legacy | Traducción moderna |
|---|---|
| `YES` + ID | éxito provisional, `PersistenciaConocida=False`, evidencia física pendiente |
| Texto clasificable previo al efecto | código `ENLASE_STORAGE_*`, mensaje saneado |
| ID asignado + texto/error | `ENLASE_STORAGE_UNCERTAIN` conservando el ID solo internamente para reconciliar |
| Excepción sin evidencia | `ENLASE_STORAGE_UNCERTAIN`, sin retry automático |

El mapper nunca publica respuesta textual cruda, excepción, ruta física, URL temporal, credencial o conexión.