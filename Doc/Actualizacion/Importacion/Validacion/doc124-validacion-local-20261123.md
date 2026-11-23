# DOC-124 — Validación local2026-11-23

Estado: implementación local verificada,21/23 tareas. Permanecen2.6 (E2E real) y3.2 (revisión final de entrega). Rama feature/DOC-124, base87d155b7; hashes normalizados de28 archivos en el [JSON](doc124-validacion-local-20261123.json).

## Pruebas y compilación

- Comando: `node --test tests/doc122-importacion-excel-behavior.test.cjs tests/doc122-e2e-reuse.test.cjs tests/doc124-e2e-reuse.test.cjs`.12/12,0 fallos.
- Ejecutable VB/C# existente: DOC122246 + DOC123312 + DOC124490 =1048 comprobaciones. Archivo/lector locales reales, autoridad/estados/ADO sintéticos.
- Regresión heredada DOC110/DOC120: PerfilImportacionTests.exe passed. No ejecutó HTTP/SQL reales.
- MSBuild net461 aislado:0 errores,308 advertencias históricas,0 en ImportacionExcel. OutDir/obj bajo .tmp; DeployOnBuild=false y Disable_CopyWebApplication=true.
- Descubrimiento Playwright:3 tests (dos DOC122, uno DOC124), sin login.
- OpenSpec estricto PASS; auditoría de refinamiento15/15. La compuerta de cierre permanece abierta por tareas/revisión/E2E pendientes.

Huella de DLL IIS preservada: `79da3a8841e22d8c5835b2495ceb0179946c67b204effcf326c2d8030c722779`. Build aislado: `e1767f58a8d784bc0f958b8c141efad7363669a793eb0eb5c164aefed071e730`. Sin despliegue. No se certifica CI de la suite nativa nueva.

## Revisión por requisito

| RQ | Evidencia y alcance |
|---|---|
| RQ-01 | Guarda JSON/multipart y parser ASP.NET sintético; revocación en contexto inyectado |
| RQ-02 | XLS/XLSX físicos, nombre, bytes/hash, .part exclusivo y retención contra escritura |
| RQ-03 | Referencia MachineKey/reloj inyectado, replay mismo cuerpo, escritor y commit incierto |
| RQ-04 | Políticas propias/exclusivas/compartidas contradictorias; filtros revisados; MySQL real pendiente |
| RQ-05 | Lector DOC123 reutilizado: límites exactos/excedidos y ninguna fila parcial preparada |
| RQ-06 | Metadatos/proyección ADO e índices únicos completos; capacidad767 sin DDL |
| RQ-07 | Protocolo temporal/tipos/lotes y tres modos con ADO sintético; igualdad real pendiente |
| RQ-08 | Vector Node→VB, NULL/orden/hoja/política y relectura final |
| RQ-09 | Transiciones del repositorio con rollback/commit antes/después, CAS y ganador preservado |
| RQ-10 | Vista25, totales/truncamiento, consulta sin reparación e ID ajeno sin actividad revelada |
| RQ-11 | DROP/restauración/liberación y camino de descarte dirigido con fallos inyectados |
| RQ-12 | SQL11 sólo SELECT, sin instalación; net461/conector/catálogos históricos conservados |
| RQ-13 | Harness reutilizado,12/12,1048 comprobaciones nativas y build aislado |
| RQ-14 | E2E extendida/descubierta; recorrido real pendiente, no sustituido por mocks |

## Revisión semántica local

Se revisaron exposición ASMX/ASHX, propiedad previa a lock, política de servidor, metadatos exactos y tipos de temporales, transacciones cortas, publicación y fallos inciertos. Cambiar intención de una PREPARADA exige hash esperado; repetir intención revalida y puede invalidar un snapshot obsoleto mediante CAS capturado. Se añadieron pruebas para ambos casos. Nunca degradar un commit incierto o CONFIRMADA.

El replay compara los mismos bytes del libro: regenerar un XLSX equivalente puede cambiar bytes del ZIP y no es el mismo archivo. Pruebas conservan el cuerpo original para verificar la idempotencia. No se guarda un reporte durable de incidencias ni catálogo de hojas en columnas inexistentes.

## E2E real y SQL

NO_EJECUTADA:0 autenticaciones,0 HTTP,0 SQL reales. Falta autorización nueva para ambiente/cuentas/archivos/destinos descartables y despliegue acordado del build aislado. El sitio conserva la DLL anterior. Scope obligatorio con tres modos, duplicados interno/existente, identidad, hashes y caso8000–10000 para dos sesiones. Controles SELECT ODBC existentes y cuenta restringida, sin siembra/limpieza/guardado de plantilla o DDL permanente desde tests.

SQL [11-preflight-doc124.sql](../Sql/11-preflight-doc124.sql): ENTREGADO, PENDIENTE_EJECUCION_MANUAL. No se espera nueva migración; cualquier diferencia permanente requiere otro archivo y confirmación. No se modificaron EGRESOS/CONTRATOS ni Web.config. No archivar/cerrar DOC124 ni marcar E2E como no aplicable.
