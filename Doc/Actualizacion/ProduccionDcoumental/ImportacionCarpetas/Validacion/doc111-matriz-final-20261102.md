# DOC-111: matriz final de capacidades y evidencia

**130/130 tareas**. La matriz distingue ejecución real, comprobación local/estática y casos no ejecutados. El usuario indicó «continua» en respuesta a la solicitud de aprobación de QA/revisión de esta matriz; se registra conformidad con los resultados y límites presentados, sin atribuir una ejecución manual adicional. [Cierre técnico](doc111-cierre-tecnico-20261102.md).

## Aceptaciones reales

| Capacidad | Resultado y recursos | Evidencia |
|---|---|---|
| REC-01/02 EXPEDIENTE | EJECUTADO_OK, sesión 18; plan 5/lote 3; expedientes 650/651; documentos 16206/16207 | [Reporte](doc111-rec-expediente-pass-20261101.md) |
| REC-01/02 LOTE | EJECUTADO_OK, sesión 22; plan 6/lote 4; expedientes 652/653; documentos 16208/16209 | [Reporte](doc111-rec-lote-pass-20261102.md) |
| REC-01/02 PARCIAL | EJECUTADO_OK, sesión 25; perfil 25/versión 39; plan 7/lote 5; expedientes 654/655; documentos 16210/16211; rechazado conservado entre tres archivos | [Reporte](doc111-rec-parcial-pass-20261102.md) |
| Degradación operativa | EJECUTADO_OK, sesión 26; ausencia temporal de ra_integracion_evento, 503/aviso UI, restauración exacta y API recuperada | [Reporte](doc111-degradacion-individual-pass-20261102.md) |
| Carga individual durante degradación | EJECUTADO_OK, sesión 26; expediente 467, documento 16212/inventario 23301; un envío, gabinete e incorporación SELECT | [Reporte](doc111-degradacion-individual-pass-20261102.md) |
| Lote histórico 2 | Completado y recuperación comprobada; lectura/reapertura EJECUTADO_OK sesión 21. No se atribuye replay completo de su sesión interrumpida | [Reporte](doc111-lote2-verificacion-pass-20261102.md) |

Los REC combinan login/reapertura UI con operaciones ASMX/recepción del driver. No se presentan como selección manual de carpetas mediante el diálogo nativo. Cada aceptación compara IDs, índices, relaciones y fingerprint antes/después de repetir comandos y reabrir. Los controles RA/DA usan cuentas exclusivamente SELECT; el DDL autorizado de la sesión 26 usa el puente administrativo separado. Recursos conservados.

## Requisitos, implementación y nivel de comprobación

| Requisito | Implementación principal | Evidencia y límites |
|---|---|---|
| RQ-01 destino fijo | ServicioPreparacionImportacion, ImportacionCarpetasContextAdapter, adaptador de creación y cliente ImportacionCarpetas.js | REAL: expedientes bajo nivel 7 y relaciones conocidas. LOCAL: cambio de contexto/destino y selección. La sesión 26 seleccionó el nivel por el árbol real. |
| RQ-02 autoridad y transporte | Guards operativo/administrativo, WebServiceImportacionCarpetas y handler propio | REAL: sesión/token y operaciones autorizadas. LOCAL: revocación con sesión/token previos en doce ASMX y recepción, separación administrador/importador (5.9/6.6). Revocación real de permisos en estas corridas: NO_EJECUTADO, no necesaria para las aceptaciones registradas. |
| RQ-03 inventario/rutas | InventarioImportacion.js, validador de manifiesto y preflight servidor | REAL: totales de dos/tres archivos, PDF y rechazado PARCIAL. LOCAL: homónimos, traversal, límites y reselección. No se acredita soporte de directorios vacíos inventados. |
| RQ-04 perfil/valores | ServicioPerfilImportacion, resolvedor y CoordinadorImportacionCarpetas.Valores | REAL: publicaciones 37/38 y nueva 39; índices efectivos antes de almacenar y físicos después. LOCAL: plan manipulado, valores diferidos y reglas inválidas. |
| RQ-05 persistencia | MySqlImportacionCarpetasRepository y script operativo MySQL 5.1 | REAL: planes/lotes, confirmación repetida, identidad y restauración de esquema. Evidencia inicial de persistencia/rollback/carrera en reportes del 2026-10-27. DDL inicial aplicado manualmente por el usuario; no se confunde con un migrador automático. |
| RQ-06 recepción | ServicioRecepcionImportacion, staging privado y ImportacionCarpetasUpload | REAL: PDFs, originales válidos y checkpoints conservados. LOCAL: stream acotado, copia consumible, hash/replay y fallos. TIFF/Multi-TIFF no soportados por este adaptador; el .tif de PARCIAL fue rechazado sin recibirlo. |
| RQ-07 efectos | AdaptadorExpedienteImportacion, AdaptadorDocumentoImportacion y comandos de verificación | REAL: creación, Archivo Gestión resuelto por nombre, almacenamiento/incorporación, nombre original e índices según visibilidad. Función legacy intacta. No se promete una transacción distribuida entre SQL, disco y XML. |
| RQ-08 barreras | CoordinadorImportacionCarpetas.Barreras y sus puertos del repositorio único | REAL: EXPEDIENTE, LOTE y PARCIAL, con bloqueo sin mutación y avance de elegibles. Una petición no encadena fases documentales. |
| RQ-09 recuperación/detención | Coordinadores de reconciliación/control y repositorio CAS | REAL: cierre de reserva 5 sin inicio y reconciliación de intento 6 del lote 2 conservando documento 16204. LOCAL: carreras, detención y pérdida de respuesta. No se reenvía un almacenamiento incierto ni se usa expiración como prueba de ausencia. |
| RQ-10 seguimiento/mantenimiento | Consulta paginada, exportación, diagnóstico y capacidad interna de limpieza controlada | REAL: consulta de resultados persistidos en los REC. LOCAL: paginación/CSV seguro, retención, invariancia, recuperación y originales sintéticos. Limpieza real: NO_EJECUTADO por recursos conservados y política deshabilitada; sin endpoint/job habilitado. |
| RQ-11 interfaz/reanudación | ImportacionCarpetas.js, vista/API e integración con árbol existente | REAL: Producción, reapertura y recuperación por ID en REC, aviso de indisponibilidad y flujo individual. LOCAL: rama por IDs, selección/foco, reselección y cierre. No se acredita recorrido manual visual completo de todos los estados. |
| RQ-12 calidad/compatibilidad | Herramientas E2E y framework existentes; dossier y OpenSpec | REAL: tres modos REC, degradación y carga individual. LOCAL: arnés Framework/build de implementación y 88 pruebas de la última extensión. ESTÁTICO: 31 archivos protegidos idénticos a HEAD. SII real: NO_EJECUTADO; no se atribuye una carga individual a SII. |

Los informes históricos por componente y comandos concretos permanecen enlazados en el dossier 05-PruebasEvidencia.md y tasks.md. No se sustituyen integraciones reales por mocks; se identifica qué comportamiento solo fue probado localmente.

## Huellas por ejecución

HEAD común: `b2e93b726f221868cf2b82d43200a8e568871117`. Workspace con cambios sin commit; HEAD por sí solo no representa el código probado.

| Sesión | Huella de código | Huella del driver |
|---|---|---|
| 18 EXPEDIENTE | 96c3fafdd5e6254da8a6f3fbcb994908078cdd8ebff30c1ef1809ea4a6bf8e3d | 156cb2042c8c17334537970cb065360812c670c1f391efd6f08d627ee5455e2e |
| 22 LOTE | 96c3fafdd5e6254da8a6f3fbcb994908078cdd8ebff30c1ef1809ea4a6bf8e3d | 0f7e4378d438eaf462be85303425c8c0c688f0d7f2a25ad5f8b6110dc9ef8865 |
| 25 PARCIAL | 96c3fafdd5e6254da8a6f3fbcb994908078cdd8ebff30c1ef1809ea4a6bf8e3d | b94ded809fa2d7789089be0960068ca4e0d3d161a0170bf945e56433fbb4f4a3 |
| 26 degradación/individual | be095c78d774dc7f2d8b5fafb7f7dfb68c5eee0fe6623c8a9c8c94728e9a9a1e | cad1f02f242be858a6efe03ea865510a57617261801230bd2fa3fa08e8e3faa6 |

DLL activa sin despliegue durante las extensiones E2E: `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`. La diferencia de huellas entre sesiones corresponde a cambios del harness incluidos en su ámbito. No se comparan como si fueran el manifiesto independiente de fuentes. La sesión 26 y sus fuentes actuales coinciden; las sesiones previas conservan sus huellas históricas.

## Revisión final

Completitud: aceptaciones 13.12–13.16 documentadas; 13.17 consolidada por esta matriz y 13.18 con revisión/aceptación registradas. Corrección: resultados reales y lecturas de destino/restauración respaldan las capacidades ejercidas; se mantienen los límites locales/estáticos. Coherencia: repositorio/contratos y herramientas compartidos, almacenamiento/SII/configuración preservados, sin host desatendido opcional 13 ni aislamiento XML adicional descartado por el usuario.

Verificación y gobierno final: `doc111-gobierno-final-20261102.json`; evidencia OPSXJ y checklist de revisión persistidos para HEAD y huellas actuales. La aceptación de resultados no procede de inferir que las autorizaciones de efectos equivalían a QA: se registra la instrucción posterior del usuario sobre esta matriz. No se publica, archiva ni cierra Jira como parte de esta comprobación.
