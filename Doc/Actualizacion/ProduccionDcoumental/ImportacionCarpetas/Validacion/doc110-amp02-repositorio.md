# DOC-110 AMP-02 — Repositorio de fuentes

Fecha: 2026-10-23. Avance de 15.5/15.6, pendiente de aceptación transaccional real.

Implementados FuenteImportacionModels, ReglasFuenteImportacion y MySqlFuenteImportacionRepository. Incluidos en el proyecto y harness .NET existentes.

| Operación | Comportamiento implementado |
|---|---|
| Listar/obtener | Ámbito del contexto, paginación de 25, lectura de cabecera y columnas en transacción RepeatableRead. |
| Alta | ID incremental de MySQL, fuente inactiva, revisión 1, columnas e historial ALTA en la misma transacción. |
| Editar | Bloqueo de cabecera FOR UPDATE, revisión esperada, identidad conexión/tabla inmutable, reemplazo transaccional de columnas, aumento de revisión e historial. |
| Activar/revocar | Revisión esperada, cambio de disponibilidad e historial. La validación de metadatos previa a activar corresponde al servicio aún pendiente. |
| Historial | Autorización por fuente/ámbito antes de leer, paginación y snapshot tipado con JSON canónico. |
| Error | La transacción no se confirma si falla historial. Conflictos de clave/bloqueo se traducen; no se reintenta automáticamente una escritura. |

Reglas locales: permiso de fuentes independiente del de proveedores, identidad/ámbito canónicos, alias RA/DA, nombres seguros, hasta 128 columnas sin duplicados, columna empresarial exacta incluida en modalidad COLUMNA y vacía en CONEXION, motivo no vacío, revisión válida sin overflow. No se exponen nuevas operaciones HTTP en este avance; nadie obtiene automáticamente PuedeAdministrarFuentes.

## Verificación ejecutada

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: PASS. El harness ahora incorpora FuenteImportacionBehaviorTests además de las pruebas anteriores.
- Las nuevas pruebas verifican permiso insuficiente para las cuatro operaciones, rechazo antes de conexión, ID/página/revisión inválidos, modalidad empresarial incompatible, columnas duplicadas y serialización del snapshot.
- Build completo aislado: PASS, salida `.tmp/doc110-amp02-build/`, sin copia a bin ni despliegue.

Estas pruebas no abren conexiones para mutaciones y no acreditan rollback real, concurrencia entre conexiones ni revocación en el consumidor. El último SELECT de SQL 07 detectó ausencia de ra_integracion_fuente_historial; no se repitió DDL ni se fabricó evidencia de persistencia. 15.5/15.6 permanecen abiertas hasta su verificación primaria. Servicios, comprobación del nuevo permiso contra RA, descubrimiento, interfaz y sustitución de Web.config siguen pendientes. SII y tablas DA intactos.
