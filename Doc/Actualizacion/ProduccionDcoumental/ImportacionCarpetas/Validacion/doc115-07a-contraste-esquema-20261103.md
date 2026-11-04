# DOC-115 — contraste local de fundaciones 07-A

Fecha: 2026-11-03. Alcance: archivos versionados, sin conexión MySQL. Decisiones D-01/D-02/D-06 y requisitos RQ-01/RQ-02/RQ-06. Fuente: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-carga-carpetas-mysql51.sql; consumidor: Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb.

| Entidad | Garantía del DDL | Responsabilidad del repositorio |
| --- | --- | --- |
| ra_integracion_plan | PK plan_id y FK version_id | Contexto/huella/TTL/revisión y bloqueo FOR UPDATE en confirmación |
| ra_integracion_lote | UNIQUE plan_id y (ambito_ref,solicitante_ref,clave_confirmacion) | Repeticion compara plan/solicitud_hash; congela snapshot y totales |
| ra_integracion_raiz | UNIQUE (lote_id,cliente_raiz_id), FK lote_id | Conserva raíz original/estado/revisión; pertenencia bajo transacción |
| ra_integracion_archivo | UNIQUE (raiz_id,cliente_archivo_id) y (raiz_id,ruta_hash), FK raiz_id | Separa recepción/efecto, comprueba cuota/hash y archivo del lote |
| ra_integracion_intento | FK lote/raíz/archivo e índices por fase | Fase/recurso/token, intento activo lógico y revisión/exclusión |
| ra_integracion_evento | FK lote/intento e índice temporal | Auditoría operativa en transacción, sin efecto documental |

Se constataron 6 CREATE TABLE y 10 constraints FOREIGN KEY; InnoDB y ASCII en cada tabla, hashes BINARY(32) y documentos LONGTEXT. El script referencia ra_integracion_perfil_version y no recrea tablas de perfil. La extensión IMPORTAR_CARPETAS queda en el script histórico; no se ejecuta ni concede permisos en este corte.

Operar abre conexión propia, comprueba metadatos y ejecuta transacción ReadCommitted; 1062/1205/1213 se traducen a CONFLICTO_REVISION. Un SELECT FOR UPDATE del plan serializa confirmadores de ese plan; claves compartidas por planes diferentes dependen además del índice único. Este contraste no demuestra comportamiento MySQL real. No se detecta necesidad de cambio DDL a partir de esta revisión estática: si la integración demuestra otra necesidad, se entrega incremento separado coordinado.

07-A transacciones reales: NO_EJECUTADO, falta autorización específica de ambiente/cuentas/recursos; responsable equipo del ambiente. DDL aplicado en este corte: ninguno. Esquema instalado actual: no consultado. SII y tablas documentales: sin cambios.
