# Operación SQL DOC-92

Este paquete extiende únicamente `ra_auth_second_factor_challenge`. No fue ejecutado durante la implementación.

Orden autorizado: `00-preflight.sql`, respaldo, `01-apply.sql` y `02-postflight.sql`. Los tres deben ejecutarse en la misma base central configurada para DocuArchi. Ante cualquier error, detener el despliegue; no adaptar tipos ni nombres directamente en el ambiente.

`03-rollback.sql` elimina exclusivamente las nueve columnas y tres índices DOC-92. Es destructivo para información v1 y requiere respaldo, ventana aprobada y confirmación de que la aplicación no usa el contrato nuevo.

`04-cleanup-terminal.sql` es manual. Por defecto conserva 30 días y solo elimina estados terminales con `TerminalAtUtc` informado. No se instala evento, job ni scheduler.

Ningún script contiene credenciales o cambia usuarios, módulos, tareas, auditoría, SMTP ni filas legacy durante el apply. Una ejecución real o una inspección adicional de `information_schema` requiere autorización explícita vigente para el ambiente.
