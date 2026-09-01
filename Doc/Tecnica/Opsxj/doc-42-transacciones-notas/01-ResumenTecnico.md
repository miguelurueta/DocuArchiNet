# TRANSACCIONES-NOTAS

- Ticket: DOC-42
- Cambio OpenSpec: doc-42-transacciones-notas
- Clasificacion: cross_cutting (Transversal)
## Objetivo

El cambio implementa escrituras modernas de notas con autorización atómica, idempotencia y auditoría, preservando los componentes legacy.

## Alcance y compatibilidad

- [x] No se modifican páginas WebForms ni consumidores; se añaden ASMX moderno, servicio, repositorio y adaptador E2E.
- [x] Las lecturas/gates legacy se preservan; la migración incluye reversa documentada y no se aplica automáticamente.

## Rollback de despliegue y migración

- La frontera moderna no está conectada a consumidores ni UI WebForms y no modifica gates, usuarios ni grupos; retirar su consumidor futuro conserva la ruta legacy sin doble escritura.
- La escritura queda cerrada con `Unavailable` mientras el preflight no confirme el esquema InnoDB, columnas, índices y el libro de versiones exigidos.
- El script DOC-42 contiene únicamente DDL revisable por esquema y un orden de reversa para los índices, idempotencia y `workflow_notas_version`; la conversión de motor sólo se revierte si el inventario previo documentó MyISAM. Una precondición de idempotencia se aplicó manualmente y con autorización en el ambiente de prueba durante la E2E; el nuevo libro de versiones exige su propia inspección y autorización. No existe aplicación automática de migraciones desde el endpoint ni desde las pruebas locales.
