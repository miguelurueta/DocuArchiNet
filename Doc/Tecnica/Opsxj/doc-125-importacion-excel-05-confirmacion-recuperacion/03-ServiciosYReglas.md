# DOC-125 — Confirmación transaccional y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: implementación y aceptación real5/5 verificadas.

## Servicios y reglas

ServicioConfirmacionExcel valida autoridad, identidad, documento v1 y archivo completo; compone confirmación con los repositorios existentes. MySqlCargaExcelRepository conserva su sesión ID y agrega el lock físico de destino en otra conexión del mismo recurso DA, compatible con el único GET_LOCK por sesión en MySQL5.1. No se agrega una cadena de conexión.

MySqlConfirmacionExcelWriter agrupa parámetros/bytes, conserva tipos y ejecuta AGREGAR o DELETE/INSERT autorizado. La ejecución y plantilla/campos se bloquean y releen en la transacción que publica CONFIRMADA/conteo/UTC. MySqlEstructuraExcel inspecciona motor, unicidad, triggers/FK, proyección y procedencia del CREATE. Una estructura propia vacía puede recuperarse; una ajena/alterada no se modifica.

La procedencia compacta docuarchi/x1/ vincula UUID/EsquemaHash completos por SHA256/base64url en56ASCII, compatible con el límite60 de MySQL5.1; una marca literal anterior se reconoce sólo con coincidencia íntegra. La inspección global de FK se acredita una vez por operación/conexión/base/destino, con presupuesto20s; se conservan15s en el restoSQL y30s en HTTP. No se comparten resultados entre sesiones; permisos/TRIGGER/motores/esquema/autoridad/locks se revalidan. Se excluye DDL administrativo concurrente.

FALLIDA sólo se registra tras rollback acreditado y CAS de identidad/snapshots. Un commit o limpieza inciertos conserva el ID sin degradar una CONFIRMADA. Cambios de autoridad antes del commit revierten sin publicar incidencia no autorizada. La inspección de metadatos cierra la escritura si no acredita visibilidad; no concede permisos. Los tres archivos VB nuevos están registrados en vbproj.
