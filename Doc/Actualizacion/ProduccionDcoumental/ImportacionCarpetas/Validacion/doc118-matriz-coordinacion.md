# DOC-118 — matriz de coordinación y recuperación

Revisión: 2026-11-07. Base local `5162b92996c7652a30fc39f3b4142a5dd17c166c`, con DOC-117 integrado en main `ee36f5ae540c02fd9986670f0a7c0305a7db47f5`. Rutas relativas a esta carpeta salvo indicación. Los hashes de fuentes/reportes están en `openspec/changes/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion/source-review.json`.

## Cobertura y brechas

| Criterio | Evidencia reutilizable | Alcance exacto y conclusión |
| --- | --- | --- |
| COORD-01 EXPEDIENTE A lista/B por recibir | doc111-rec-expediente-pass-20261101.md | REAL histórica: plan5/lote3, raíces5/6, documentos16206/16207 e inventarios23295/23296. A avanza antes de recibir B; B incompleta queda bloqueada. Caso exacto acreditado históricamente, no requiere repetir la carga por defecto. |
| COORD-01 LOTE | doc111-rec-lote-pass-20261102.md | REAL histórica: plan6/lote4, documentos16208/16209. Barrera global antes de ambas recepciones; replay y reapertura sin otro efecto. |
| COORD-01 PARCIAL | doc111-rec-parcial-pass-20261102.md | REAL histórica: plan7/lote5, documentos16210/16211. PDF elegible avanza antes del otro PDF; TIFF rechazado sigue contado y lote PARCIAL. |
| Dos clientes de recepción/confirmación | doc115-07a-resultados.md | REAL histórica con conexiones físicas independientes. No es ALMACENAR ni demuestra exclusión de dos escritores documentales. |
| COORD-02 respuesta perdida | doc117-alm02-interno-pass-20261106.md | REAL histórica de variante laboratorio: intento52/lote21 con documento16222/inventario23320, checkpoint confirmado y replay exacto. Intento53 produce16223/23321 sin checkpoint, queda EN_CURSO durable/INCIERTO público: preservar sin replay ni reparación. No acredita dos clientes con primera llamada activa. |
| Coexistencia con individual | doc117-alm03c-coexistencia-pass-20261106.md | REAL histórica de variante laboratorio: lote23, intentos63/64, documentos16225/16226/16227. Superposición con carga individual; no disputa de la misma reclamación entre dos clientes importadores. Su excepción de instrumentación no se hereda. |
| COORD-02 exclusión, antigüedad y pérdida en vuelo | tests/ImportacionCarpetasBehaviorTests.cs: ControlCases, DocumentPhaseCases, DocumentGlobalBarrierCases | LOCAL con dobles. Integración específica NO_EJECUTADA: faltan clientes/sesiones independientes disputando avance, prueba dentro de llamada real y observación del bloqueo/único efecto. |
| COORD-03 detención en vuelo y consulta posterior | doc111-reserva-detencion-local-20261028.md; pruebas DocumentAdapterCases/StopDuringWriter y QueryCases | LOCAL con dobles. Integración específica NO_EJECUTADA: detención mientras sigue activa la llamada real, cierre navegador y consulta desde otra sesión autorizada. |
| Recuperación acotada | doc111-reconciliacion-local-20261030.md; pruebas preinicio/ReconciliationCases | LOCAL: cierre preinicio solo sin detención, y reconciliación de INCIERTO con evidencia conocida. No habilita reparar EN_CURSO por fecha ni vaciar reclamaciones detenidas. |
| Consolidación original | doc111-consolidacion-local-20261030.md; ConsolidationCases; REC históricos | LOCAL y REAL histórica en sus ámbitos. RegistrarResultadoDocumento invoca consolidación en su transacción exterior; no añadir endpoint de consolidación. |

## Seguridad: contraste antes de agregar negativos

Mapear ControlCases, QueryCases, OperationHttpCases, DocumentPhaseCases, DocumentGlobalBarrierCases y ReconciliationCases contra permiso revocado, identidad ajena, destino incompatible, revisión obsoleta y publicación posterior. El código reautoriza consulta/mutación/verificación y conserva snapshot; esa inspección no sustituye el inventario de pruebas por fase. Agregar solo la combinación no acreditada y únicamente si corresponde a un comportamiento modificado o brecha reproducida. No ejecutar cambios reales de permisos sin autorización.

## Revalidación local actual

Comando: `node --test tests/doc110-perfil-importacion-behavior.test.cjs`, ejecutado el 2026-11-07 sobre las fuentes de la base indicada. Resultado: 2/2 pruebas Node, 0 fallos. El arnés compila y ejecuta reglas VB/.NET Framework y verifica columnas UPDATE contra el DDL; usa dobles, sin HTTP ni conexión a datos. Los grupos C# ejecutados no se suman como pruebas Node ni acreditan integración real.

## Condiciones para preparar integración

Primero caracterizar una observación externa compatible que pruebe entrada y permanencia dentro del método legacy. No valen una pausa antes/después ni solo timestamps HTTP. No se modifica AlmacenamientoDocumentoProduccionDocumental ni SII y no se hereda ninguna excepción DOC117. Si el método no es observable de forma compatible, registrar la limitación y revisar el alcance; no simular cumplimiento.

Preparar recursos nuevos y dos sesiones autorizadas independientes, correlación lote/archivo/intento/solicitud/snapshot/revisiones, punto de fallo preciso, evidencia de entrada única e identidades durables, presupuestos de peticiones/SELECT y reglas de corte. Solicitar autorización específica después de concretar ese plan. Controles RA/DA solo SELECT; secretos efímeros y evidencia saneada. No hay ambiente, cuenta ni presupuesto aprobado para DOC-118 en esta entrega.

## Recursos protegidos y cierre

Intento41/lote18 (DOC116) e intento53/lote21 (DOC117) quedan intactos: no replay, cierre, reconciliación ni reparación. Los demás recursos históricos tampoco se reutilizan para nuevas cargas por inferencia.

COORD-01 conserva crédito REAL histórico. COORD-02 y COORD-03 siguen abiertos para su integración específica. Toda incertidumbre de los casos nuevos impide declararlos completos; consignar IDs conocidos, estado durable y público, bloqueo y recuperación realmente soportada. No declarar completitud global ni ocultar intentos históricos no resueltos. La revisión editorial y autorización de ejecución son decisiones separadas.


## Continuación de preparación aprobada — 2026-11-07

Aprobación editorial del usuario registrada tras el detalle de implementación. Mapa aplicado: 27 tareas, 11 S y 16 M, con cobertura D-01–D-10/RQ-01–RQ-10. Preparación y contraste local completan las tareas 1.x–3.x; no completan las tareas de integración 4.x.

Revalidación conjunta: `node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc111-importacion-carpetas-client.test.cjs`, 18/18 pruebas Node sin fallos, servicios sintéticos sin login/HTTP real. El resultado anterior 2/2 conserva su ejecución y alcance.

Se añadieron `doc118-plan-integracion.md` y `doc118-seguridad-recuperacion.md`. Visual Studio disponible localmente permite proponer observación externa con breakpoint e hilo seleccionado, pero su compatibilidad en el runtime sigue SIN_CALIFICAR. No se adjuntó depurador, autenticó ni ejecutó ningún efecto real. COORD-02/03 siguen NO_EJECUTADOS.


## Preflight real y continuación

Progreso16/27. El primer rechazo consumió98 HTTP/1 apertura; la redistribución425/75 fue autorizada. La continuación autenticó A y consultó lote23, pero B se detuvo al alcanzar425 HTTP acumulados C02. Consumo actual:425/0 HTTP,3 aperturas,0 SELECT de control,0 efectos; sesiones cerradas y secretos borrados. doc118-continuation-preflight-20261107.json conserva el resultado real separado de120/120 pruebas locales. COORD02/03 siguen NO_EJECUTADOS para aceptación. La ampliación1000 HTTP/5 aperturas solo está propuesta. El observador y las fases documentales siguen pendientes.
