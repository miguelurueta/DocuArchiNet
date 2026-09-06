# Estados, errores y antirregresión

## Estados e invariantes

Las intenciones futuras expresan fases tipadas desde `Creada` y `Validada`, pasando por obtención, preparación, almacenamiento e índices, hasta resultados como `Completada`, `Parcial`, `Detenida`, `FallidaAntesDePersistir` o `Reconciliada`. DOC-50 no persiste ni transiciona esos estados. Las lecturas son idempotentes; creación exige una clave de idempotencia y ejecución un token de versión. El timeout se declara por capacidad/proveedor y no se fija globalmente.

## Errores seguros

| Código | Condición |
| --- | --- |
| `INVALID_CONTEXT` | Contexto nulo o identidad requerida ausente |
| `FORBIDDEN` | Usuario o permiso no vigente |
| `TASK_NOT_OPERABLE` | Tarea no disponible para la operación |
| `ROUTE_MISMATCH` | Ruta autorizada distinta |
| `PROCEDURE_MISMATCH` | Trámite autorizado distinto |
| `PROVIDER_NOT_SUPPORTED` | Proveedor no habilitado o no registrado |
| `EXTERNAL_ITEM_NOT_FOUND` | Elemento externo inexistente |
| `CONCURRENCY_CONFLICT` | Token de versión incompatible |
| `TIMEOUT` | Tiempo de proveedor agotado |
| `INTERNAL_ERROR` | Falla no publicable, con referencia saneada |

El validador falla en el primer error y no invoca al proveedor. Una identidad desconocida nunca selecciona SII por defecto. El contexto inmutable conserva la instantánea aunque cambie la sesión o exista otra pestaña activa.

## Compatibilidad, regresión y reversión

- No se modifican ASMX, firmas públicas ni `workflow/ClassAlmacenamiento.vb`.
- No se invoca `AlmacenaDocumentoTareaWorkflow(...)`.
- No se aceptan valores del navegador o sesión mutable como autoridad.
- Los contratos comunes no contienen vocabulario ni caché específica de SII.
- La prueba estructural impide contratos duplicados en fronteras legacy y entradas de proyecto duplicadas.
- El rollback es aditivo: retirar seis archivos VB, sus seis `Compile Include`, tres suites, ocho fixtures y este paquete. No hay datos que migrar ni endpoints que restaurar.
