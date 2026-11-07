# Verificación final DOC-116

Estado vigente: ESCENARIOS VERIFICADOS, ENTREGA FORMAL PENDIENTE. Fuente: copia principal local. Fecha: 2026-11-05. El cuerpo conserva la revisión inicial y sus bloqueos históricos; su resolución autenticada y los resultados vigentes figuran en los cortes finales y en doc116-entrega-20261105.md.

## Evaluación

| Dimensión | Resultado |
| --- | --- |
| Completitud del trabajo | 20/20 tareas de implementación, ejecución viable y consolidación; no equivale a cerrar todos los casos reales |
| Corrección | EXP01 PASS; EXP02 cliente PASS parcial; EXP03 PASS con restauración; RQ04 interno pendiente |
| Coherencia | D01–D07 seguidas; creador único y producto preservados; ningún hook público de fallo |
| Integridad local | Nueve archivos protegidos coinciden con el inventario inicial; backend igual en las tres ejecuciones |
| Estructura | OpenSpec estricto PASS; auditoría de refinamiento 15/15 PASS |
| Gobierno de cierre | No aprobado: openspec_review, evidence:unit y evidence:manual_qa pendientes de registro formal para entrega |

Los 95 tests Node seleccionados y las tres ejecuciones autenticadas tienen evidencia independiente de los registros formales de cierre pendientes. No se han simulado aprobaciones ni se han usado variables de revisión para convertir el cierre en PASS. La frescura respecto a un commit publicado no se certifica: el trabajo sigue sin publicar. Framework 2/2 del corte previo se reutiliza; no se repite al cambiar únicamente documentación.

## Criterios y evidencia real

| Caso | Plan/lote | Resultado |
| --- | --- | --- |
| EXP01 | 32/16 | 657/658 en nivel 7 después de seleccionar 8; relaciones 1082/1083, replay sin cambios |
| EXP02 cliente | 33/17 | 659/1084, raíz restante sin crear; CLIENT_RESULT_DISCARDED, caseComplete=false |
| EXP03 | 34/18 | No propietario en nivel 215; fila 629 1→0→1; una creación denegada; diario RESTORED |

Cero documentos definitivos nuevos. Conservados seis PDF en staging, tres expedientes, planes y lotes nuevos y anteriores. El estado permanente del permiso quedó intacto. Las huellas previas se conservaron en cada ejecución.

La matriz completa de siete requisitos, decisiones, fuentes y límites está en [doc116-expedientes-matriz.md](doc116-expedientes-matriz.md). Los informes de cada caso conservan los SHA de su código/recursos/backend/checkpoint exactos; no se atribuye una corrida antigua a código posterior. El manifiesto [doc116-verificacion-final-20261105.json](doc116-verificacion-final-20261105.json) agrega SHA de resultados, comparación de archivos protegidos y variantes de utilidades principal/aislada.

## Hallazgo crítico DOC116-VERIFY-01

EXP02 ventana interna no tiene composición autenticada segura en este runtime: NOT_COMPOSED / NO_EJECUTADO. Falta ejecutar el fallo postlegacy/precheckpoint o la pérdida de respuesta del checkpoint, con contexto de autoridad real y recuperación revisada. Los dobles locales y el descarte de resultado confirmado en cliente no lo acreditan.

Referencias del requisito: `openspec/changes/doc-116-produccion-importacion-carga-carpeta-expediente/design.md:29`, `specs/produccion-importacion-carga-carpeta-expediente/spec.md:42` dentro del cambio, y `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb:18` y `:73`. El constructor Friend permite pruebas locales; no constituye una vía HTTP autenticada de fallo.

Acción necesaria: responsable del ambiente y arquitectura deben presentar una vía interna segura que conserve contexto real, punto exacto de fallo, presupuesto de efectos y recuperación. Solo después de revisarla se solicita autorización específica para ejecutarla. Una exclusión de esta aceptación necesita una decisión explícita del usuario y actualización aprobada del alcance; no se infiere de completar las tareas. No añadir endpoint/header/flag productivo, fabricar HttpContext, borrar checkpoints ni recrear por nombre.

Según D04/RQ04/RQ07 este hallazgo impide afirmar aceptación completa. No se archiva ni se marca Jira Listo. La implementación/consolidación no demuestra por sí sola esta ventana crítica.

## Comprobaciones de consolidación

OpenSpec: `openspec.cmd validate doc-116-produccion-importacion-carga-carpeta-expediente --strict` (PASS). Auditorías de repo `auditRefinement` y `validateLegacyGovernance` sin overrides, acceso a red ni E2E nuevos: [doc116-auditoria-consolidacion-20261105.json](doc116-auditoria-consolidacion-20261105.json). `git diff --check`: PASS. El comando de la suite local de 95 casos figura en la matriz; no se repite por este corte documental.

No se ejecutaron nuevas cargas, creaciones o cambios de permisos, ni se publicó Git, se transitó Jira o se archivó OpenSpec.

## Seguimiento del hallazgo crítico

La vía interna de laboratorio ya compila y dispone de ejecutor/restauración, con 108/108 pruebas locales PASS. Sigue sin activación autorizada ni ejecución real. Binarios, límites y recuperación concretos: `doc116-exp02-interno-ejecucion-20261105.md`. La causa actual del bloqueo es aceptación autenticada pendiente, no ausencia de una candidata compilable.

## Resolución autenticada de DOC116-VERIFY-01

PASS revisado: las dos ventanas internas se ejecutaron bajo candidata de laboratorio autorizada e26242b901043394bd1dc38063becfe399cd093738cfd10b7ebca24785d8944a. Raíz 29/intento 40: expediente 660/relación 1085 y replay confirmado sin duplicar. Raíz 30/intento 41: expediente 661/relación 1086, intento EN_CURSO/public INCIERTO conservado sin repetición ni checkpoint manual. Evidencia: `doc116-exp02-interno-resultados.md` y JSON fechado.

DLL original 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5 restaurada; dos respaldos coinciden byte por byte. Hallazgo crítico funcional resuelto; aceptación de escenarios verificada. Siguen pendientes entrega/publicación y registros formales de cierre. Las secciones anteriores describen el corte previo y ya no son el estado vigente del hallazgo. No archivar ni transitar Jira automáticamente.
