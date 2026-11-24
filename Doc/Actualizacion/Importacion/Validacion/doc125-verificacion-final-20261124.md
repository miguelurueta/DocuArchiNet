# DOC-125: verificación final

Revisión técnica de Codex, 2026-11-24T14:43:15.282Z; no se atribuye aprobación humana. Se revisaron propuesta, diseño, especificación original, 31 tareas, las 34 fuentes identificadas por SHA256 UTF-8/LF y las evidencias locales y físicas.

| Dimensión | Resultado |
| --- | --- |
| Completitud | 31 tareas verificadas, 15 requisitos y 44 escenarios revisados |
| Corrección | 15/15 requisitos con implementación y pruebas trazadas |
| Coherencia | 15 decisiones conservadas; net461/MySQL5.1.37/MySql.Data6.2.3 |

Sin hallazgos CRITICAL, WARNING ni SUGGESTION de implementación. Los alcances físicos y sintéticos se declaran por requisito; los 44 escenarios no representan 44 ejecuciones reales.

| Requisito | Implementación | Evidencia | Alcance |
| --- | --- | --- | --- |
| RQ-01 | ServicioConfirmacionExcel.vb:Contexto/Identidad; ImportacionExcelRequestGuard.vb:Validar | Rejections, Transactions; controles HTTP de CSRF, cuenta restringida y hashes | Permiso revocado a mitad de escritura: sintético. |
| RQ-02 | ServicioConfirmacionExcel.vb:Documento/ValidarActual/Confirmar; AlmacenArchivoExcel.vb:Retener | Rejections; lector real y snapshots v1; preconfirmación SELECT de 1000 filas | Mutaciones de archivo/configuración: local. |
| RQ-03 | MySqlConfirmacionExcelWriter.vb:Sesion.Ejecutar; MySqlPlantillaExcelRepository.vb:LeerCarga | Modes, Transactions; tres commits físicos y rollback físico | Bloqueo de edición de plantilla: ADO sintético. |
| RQ-04 | MySqlConfirmacionExcelWriter.vb:BloquearDestino/VerificarLocks; MySqlCargaExcelRepository.vb:NombreLockDestino | Locks; dos sesiones HTTP independientes en AGREGAR; ambos locks observados/liberados en carrera | Alias y destinos diferentes: sintético. |
| RQ-05 | MySqlConfirmacionExcelWriter.vb:Agrupar/Escribir/SinAdvertencias | BoundariesAndTypes, Transactions; texto físico de 1000 filas por modo | DATE/DECIMAL/NULL y advertencias del proveedor: sintético. |
| RQ-06 | ServicioConfirmacionExcel.vb:Confirmar; MySqlConfirmacionExcelWriter.vb:Ejecutar | Modes, Transactions; AGREGAR real y escritor externo con INSERT observado/commit/1062 | Igualdad efectiva y conflictos previos: harness DOC124; carrera física acreditada. |
| RQ-07 | MySqlConfirmacionExcelWriter.vb:Escribir | Rejections, Transactions, BoundariesAndTypes; REEMPLAZAR físico y rollback tras DELETE | Política COLUMNA: local; ambiente real usa CONEXION. |
| RQ-08 | MySqlEstructuraExcel.vb:Marca/ConsultarPropia/Asegurar | Creation, Transactions; CREATE físico con marca de 56 ASCII | Pérdida de respuesta DDL, tabla ajena y supervivencia tras rollback: sintético. |
| RQ-09 | MySqlConfirmacionExcelWriter.vb:Ejecutar/RegistrarFallo; MySqlCargaExcelRepository.vb:Dispose | Transactions; dos fallos físicos con CONFLICTO_CLAVE y cero filas propias | COMMIT incierto y fallo de limpieza: sintético. |
| RQ-10 | ServicioConfirmacionExcel.vb:Confirmar/ConsultarResultado/Historico; ComposicionCargaExcel.vb | Modes; consulta/replay físicos de tres IDs históricos y rechazo de hashes diferentes | Archivo ausente o límites posteriores inválidos: local. |
| RQ-11 | MySqlEstructuraExcel.vb:VerificarSeguridad; MySqlPreparacionExcelValidator.vb | Rejections, Modes; diez controles SELECT SQL18; FK por operación y relectura de TRIGGER | Motores/índices/FK/TRIGGER adversos: sintético; sin DDL administrativo concurrente. |
| RQ-12 | ServicioConfirmacionExcel.vb:ValidarActual/Confirmar; MySqlConfirmacionExcelWriter.vb:Agrupar | BoundariesAndTypes; tiempos reales y recuperación por ID entre ventanas | 20000/20001 local; confirmación física medida en 1000 filas por modo, sin pausa parcial. |
| RQ-13 | Sql/18-preflight-doc125.sql; 19-fixtures-da-doc125.sql; 20-fuentes-e2e-ra-doc125.sql; 21-plantillas-e2e-da-doc125.sql | Aplicación delegada fuera de tests, relecturas SELECT en sesiones nuevas y huellas íntegras | No grants, instalación desde tests ni reedición de SQL aplicados. |
| RQ-14 | importacion-excel.spec.cjs; run-doc110-profile-session.cjs; doc125-excel-scope.cjs; doc115-repository-integration.ps1 | Suite real PASS 5/5; tests/doc125-e2e-reuse.test.cjs; controles DOC32 existentes | Acuse ignorado por cliente no acredita corte físico de red. |
| RQ-15 | GestionDocumental-Docuarchi.net.vbproj; documentos técnicos y evidencia actual | Build net461, 94/94 tests, 4492 checks nativos sintéticos y regresión heredada | Sin UI fase06, CSV ni publicación automática de fuentes. |

Las rutas de implementación se resuelven en las fuentes del JSON adjunto. Los métodos de prueba Modes, Rejections, Transactions, Creation, Locks y BoundariesAndTypes pertenecen a tests/ImportacionExcelConfirmacionTests.cs. Se inspeccionaron las precondiciones y resultados de los 44 escenarios del spec; el JSON conserva sus nombres, fuentes y límites.

Aceptación real cerrada: crear/agregar/reemplazar/rollback/carrera, 5/5. Las ventanas5/6/7 conservan las mismas identidades: tres confirmaciones físicas en5, rollback físico en6 y carrera física en7. La última ventana consulta y verifica los resultados previos sin volver a importar ni repetir DELETE; no presenta cinco importaciones nuevas. Conserva una sola fila del escritor externo, cero filas propias de la carrera, y los hashes completos de los destinos protegidos. Las peticiones HTTP mantienen30s y cero reintentos.

[Resultado y mediciones](doc125-ejecucion-seguimiento-20261124.md). La tarea5.7 queda verificada por esta revisión, OpenSpec estricto y gobierno ligado al SHA de entrega. La revisión y las evidencias se registran para ese SHA después del commit, sin variables de bypass; los registros locales no contienen secretos y quedan excluidos de Git. No se ejecuta archivo ni cierre de Jira en esta entrega.

Validación documental final: OpenSpec estricto96/96 y auditoría de refinamiento15/15 PASS; git diff --check PASS. Las34 fuentes conservan exactamente los hashes de la evidencia probada.
