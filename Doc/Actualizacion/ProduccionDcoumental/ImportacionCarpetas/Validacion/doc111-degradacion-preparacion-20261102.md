# DOC-111: degradación operativa y carga individual, propuesta pendiente

Actualización: el usuario autorizó expresamente la propuesta («autorizado»). Alcance registrado en `tools/e2e/artifacts/doc111-degradation-authorization-26.json`. Sesión 26 preparada y abierta; estado saneado `doc111-degradation-session-26.json`, detalle cerrado de ejecución/restauración `doc111-degradation-state.json` y recuperación del puente `doc111-operational-schema-recovery.json`. La aceptación real sigue pendiente; las referencias inferiores de aprobación pendiente son históricas.

Selector agregado al runner existente: `--degradation-profile`, excluyente con REC de publicación, diagnóstico o recuperación. Reutiliza helper de sesión, puente administrativo, flujo verifyMissingSchema y carga individual oficial inspectProductionDocumentUploadPreviewUi. Selecciona nivel 7 mediante el árbol real antes de ocultar la tabla, comprueba AbrirImportacion disponible; exige 503 y aviso de UI durante la degradación. Antes/después de la carga confirma por SELECT que la tabla sigue oculta. Restaura y verifica snapshot, filas/identidades/lotes 2–5 y un nuevo documento en gabinete/incorporación del expediente 467. Las consultas de control usan la cuenta exclusivamente SELECT; el puente separado realiza solo el DDL aprobado y sus snapshots. Entorno operativo fijado por huella y nombres constantes. **88 pruebas locales PASS**, más selector --check y validación OpenSpec estricta. Sin cambios productivos, Web.config o DLL por esta extensión E2E.

Avance 126/130. Tareas 13.15/13.16 pendientes de ejecución real y autorización específica. No se alteró el esquema ni se cargaron documentos durante esta preparación.

## Evidencia previa

SELECT del 2026-11-02 a las 20:59:20 UTC: ra_integracion_evento InnoDB con 98 filas, AUTO_INCREMENT 99, sin claves foráneas entrantes en los esquemas inspeccionados. El nombre temporal doc111_rec_evento_20261102 no existe. Expediente 467, empresa 2/usuario 141, con 10 documentos previos; se reutiliza como recurso de pruebas de carga individual del recorrido DOC-110 existente. Lotes 2/3/4 completos y 5 parcial conservan sus revisiones. Evidencia `tools/e2e/artifacts/doc111-degradation-precheck-20261102.json`.

Se reutiliza el puente `tools/e2e/scripts/support/doc110-profile-schema-db.ps1`, ahora con `-Operational`. Sus objetivos son constantes: una sola tabla ra_integracion_evento, temporal doc111_rec_evento_20261102, clave evento_id y registro de recuperación independiente doc111-operational-schema-recovery.json. Los objetivos predeterminados DOC-110 siguen siendo sus tres tablas de perfiles. El recorrido `verifyMissingSchema` admite una o tres tablas y conserva el contrato predeterminado. **15 pruebas locales PASS**, incluidas restauración en fallos UI/carga/ocultación incierta/doble registro y regresión estática DOC-88. No acreditan E2E o DDL reales.

Inspección real `-Inspect -Operational`: PASS, una tabla original, cero temporales, cero FK externas, snapshot leído; solo SELECT, sin ejecutar RENAME. Esta inspección vuelve a exigirse antes de la alteración.

## Acción concreta a autorizar

1. Guardar el snapshot de contenido, columnas, índices, claves foráneas y próximo ID; comprobar nombres, dependencias y ambiente de pruebas.
2. Renombrar temporalmente `ra_integracion_evento` a `doc111_rec_evento_20261102`. No borrar tablas, registros o restricciones. La capacidad operativa DOC-111 estará indisponible durante la prueba; no ejecutar importaciones concurrentes.
3. Comprobar respuesta 503/CAPACIDAD_NO_DISPONIBLE de la importación nueva y Producción disponible. Reutilizar el recorrido de carga individual oficial para almacenar/incorporar exactamente un PDF `doc111-carga-individual.pdf` de 677 bytes en expediente 467, tipología 50/Comprobante de Egreso. Contrastar por SELECT un documento nuevo y su incorporación; no repetir el envío ante respuesta incierta.
4. Restaurar inmediatamente el nombre original en finally y verificar igualdad del snapshot y AUTO_INCREMENT; confirmar recuperación de DOC-111. Si el proceso termina, el puente intenta restauración al salir; queda registro previo para recuperación explícita `-Operational -Recover` si hace falta. No se promete rollback transaccional de DDL.
5. Conservar el documento individual y todos los recursos de los lotes 2–5/perfiles 24–25. Revisar preservación SII con evidencia estática/local separada; no ejecutar importación SII ni atribuirle aceptación real.

Runtime preparado: `tools/e2e/profiles/doc111-production-upload-no-schema.runtime.json`. Misma URL HTTPS/ambiente, cuenta de aplicación y certificado previamente autorizados; usuario visible solo en consola, contraseñas enmascaradas. Controles RA/DA con cuenta MySQL exclusivamente SELECT. La operación DDL del puente es una capacidad administrativa separada del control SELECT y requiere esta aprobación concreta. No se modifica Web.config, permisos o función legacy de almacenamiento.

Los únicos cambios SQL previstos son los dos RENAME del mismo recurso. La autorización de los REC anteriores no se amplía a esta alteración ni al documento individual por inferencia. La aprobación específica está pendiente.
