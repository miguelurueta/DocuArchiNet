# DOC-116 EXP-03 — autoridad vigente y restauración

PASS revisado el 2026-11-05. Plan 34, lote 18, nivel 215, contexto 141/2 no propietario y concesión 629. Perfil 24/versión 38 y dos originales PDF de 677 bytes en staging. Se conservan ambos recursos y todos los anteriores.

El permiso agregar_expediente se comprobó activo, se retiró por el puente administrativo existente (1 -> 0) y una única solicitud de creación fue denegada con 403/ACCESO_DENEGADO. Las instantáneas operativas y los conteos de expedientes/relaciones permanecieron sin cambios. No se creó ningún expediente ni documento definitivo.

Finally restauró exactamente 0 -> 1. La ejecución verificó lectura independiente y columnas ajenas intactas. El diario durable final tiene stage=RESTORED, restored=true, originalPermission=1 y corresponde exactamente a 141/2/nivel 215/lote 18/fila 629. Después de restaurar se consultó el lote y se verificó de nuevo la instantánea. No se afirma que esta revisión documental haya abierto otra conexión de control.

Presupuesto ejecutado: una preparación, una confirmación/un lote, dos recepciones, una solicitud de creación denegada y dos escrituras temporales al único permiso. Cambios permanentes de permiso: cero. Salida denialUnchanged=true, permissionRestored=true, rootsCreated=0, documents=0, replayUnchanged=null; no hay replay mutante.

Las huellas antes/después coinciden en las seis tablas operativas para recursos anteriores: 33 planes, 13 lotes, 22 raíces, 23 archivos, 37 intentos y 133 eventos; límites plan 33/lote 17. Conservados lotes 16/17 y expedientes 657/658/659.

Evidencia: `tools/e2e/artifacts/doc116-exp03-20261105-authorized.json`, SHA256 bbbc4ca22c390cf7d76d5fc5c8694958ff95e10701599c5e26918f80a4ad1070. Código revisado 68e5680c4eaefc997614e610919738610b30fc45025fc13ae7d86e8e9e467666. Backend sin cambios 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5. Diario: `doc116-exp03-recuperacion-restaurada-20261105.json`, SHA del original 4c0a6c53f9158c2283e79e9912b1318ea3e7e2393153b4f27b381819d3d02733. Resumen saneado: `doc116-exp03-resultados-20261105.json`. Pruebas locales previas: 95/95 PASS. No repetir esta sesión ni revocar nuevamente el permiso.

Progreso: 19/20 tareas; 4.1 y 4.4 cerradas para los escenarios viables/autorizados. Pendiente 4.5 de consolidación y validación final. La variante interna autenticada de EXP02 sigue NOT_COMPOSED/NO_EJECUTADO; este PASS no la acredita y DOC-116 sigue abierto.
