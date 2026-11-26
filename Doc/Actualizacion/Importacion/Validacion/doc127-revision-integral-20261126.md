# DOC127: revisión integral del corte y dependencias de cierre

**Revisión realizada; cierre no aprobado.**31/33 tareas,14 requisitos y42 escenarios inventariados. Se localizaron implementación y evidencia clasificada para40 escenarios; los otros dos dependen del plan manual y del cierre final. No son42 pruebas nuevas ni una certificación de cobertura exhaustiva.

Corte de código `f6d3ba2e37ee298afcf972eff11ae6afaeab58c5`, revisión por Codex mediante openspec-verify-change. No se repitieron E2E, importaciones, build, SQL de instalación o despliegue.

| Dimensión | Resultado |
| --- | --- |
| Completitud |31/33 tareas; pendientes2.4 y7.5 |
| Corrección |12 requisitos con cobertura clasificada;RQ-10/RQ-14 parciales por evidencia pendiente |
| Coherencia | Reutilización, runner compartido, alcance por ventana, SQL manual y límites de medición conservados |

## Correspondencia de requisitos

| Requisito | Implementación / verificación | Prueba o evidencia | Resultado y límite |
| --- | --- | --- | --- |
| RQ-01: Base y reutilización | [doc125-excel-scope.cjs](../../../../tools/e2e/tests/support/doc125-excel-scope.cjs#L94) | tests/doc127-e2e-reuse.test.cjs | Hashes, SHA, historia y diferencias conservados; no se suman conteos solapados. |
| RQ-02: Infraestructura compartida | [run-doc110-profile-session.cjs](../../../../tools/e2e/scripts/run-doc110-profile-session.cjs#L7) | tests/doc127-e2e-reuse.test.cjs | Ventanas consumidas y presupuestos; sin instalación/limpieza administrativa en E2E. |
| RQ-03: Autoridad y preparación | [ReglasCargaExcel.vb](../../../../Services/ProduccionDocumental/ImportacionExcel/ReglasCargaExcel.vb#L14) | tests/ImportacionExcelConfirmacionTests.cs | Permisos/CSRF/referencias reales;36 cambios sintéticos y18 preparaciones reales clasificados. |
| RQ-04: Instalación y SQL manual | [README.md](../../../../Doc/Actualizacion/Importacion/Sql/README.md#L1) | Doc/Actualizacion/Importacion/Validacion/doc127-aplicacion-recursos-20261125.json | Aplicaciones previas confirmadas; no se reejecutaron scripts de instalación. |
| RQ-05: Plantillas, formatos y modos | [ServicioPlantillasExcel.vb](../../../../Services/ProduccionDocumental/ImportacionExcel/ServicioPlantillasExcel.vb#L148) | tests/ImportacionExcelBehaviorTests.cs | 12 modelos locales, seis usos reales y dos altas web actuales; cruces fuera de alcance explícitos. |
| RQ-06: Integridad del destino | [MySqlPreparacionExcelValidator.vb](../../../../Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPreparacionExcelValidator.vb#L141) | tests/ImportacionExcelConfirmacionTests.cs | Claves, collation, motor, índices completos, FK/trigger y aislamiento; COLUMNA real no ejecutada. |
| RQ-07: Recuperación y concurrencia | [ServicioConfirmacionExcel.vb](../../../../Services/ProduccionDocumental/ImportacionExcel/ServicioConfirmacionExcel.vb#L40) | tests/ImportacionExcelConfirmacionTests.cs | Replay/acuse ignorado y carrera observada diferenciados de COMMIT/red físicos; DDL no se borra. |
| RQ-08: Límites efectivos | [ReglasCargaExcel.vb](../../../../Services/ProduccionDocumental/ImportacionExcel/ReglasCargaExcel.vb#L29) | tests/ImportacionExcelLecturaTests.cs | Fronteras exactas locales;20000 configuradas no equivalen a20000 confirmadas reales. |
| RQ-09: Capacidad medible | [doc127-iis-memory.ps1](../../../../tools/e2e/scripts/support/doc127-iis-memory.ps1#L6) | tests/doc127-e2e-reuse.test.cjs | 18/18 reales con tiempos HTTP y memoria IIS compartida muestreada; sin asignación exclusiva. |
| RQ-10: Proveedor y consumidores | [MySqlProveedorDatosIndices.vb](../../../../Infrastructure/ProduccionDocumental/ImportacionCarpetas/MySqlProveedorDatosIndices.vb#L47) | tests/ProveedorIndicesBehaviorTests.cs | Consulta y política actuales coinciden con SQL29; falta el plan manual del servidor. |
| RQ-11: Interfaz vigente | [ImportacionExcelVista.js](../../../../js/gestion/ImportacionExcel/ImportacionExcelVista.js#L59) | tests/doc126-importacion-excel-ui.test.cjs | Vista actual con formulario auxiliar; lectura/aislamiento y asistente reales; QA humana actual pendiente. |
| RQ-12: Build y hallazgos | [MySqlConfirmacionExcelWriter.vb](../../../../Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlConfirmacionExcelWriter.vb#L158) | tests/ImportacionExcelConfirmacionTests.cs | Build aislado histórico, fuentes VB sin cambios y hallazgo form1 cerrado con cobertura clasificada. |
| RQ-13: Operación y despliegue | [Operacion-Excel.md](../../../../Doc/Actualizacion/Importacion/Operacion-Excel.md#L27) | Doc/Actualizacion/Importacion/Sql/README.md | Recuperación por ID, sin pausa por fila; respaldo de código y SQL estructural separados. |
| RQ-14: Cierre verificable | [legacyGovernanceService.js](../../../../tools/opsxj/scripts/lib/legacyGovernanceService.js#L347) | openspec/changes/doc-127-importacion-excel-07-verificacion-integral-cierre/tasks.md | La revisión no aprueba el cierre: faltan plan manual y QA/gates; no se archiva. |

El JSON enumera los42 escenarios WHEN/THEN, sus rutas, líneas, hashes y evidencias originales. La revisión usa inspección de fuentes y evidencia existente; no convierte heurísticas de correspondencia en ejecución real. Se conservan las fechas y diferencias de los81/141 casos históricos,55 locales reutilizados,18 casos reales de capacidad,6 rechazos y2 asistentes actuales.

## Críticos que impiden archivo

**C-01 /2.4: falta el plan EXPLAIN.** El contrato actual sigue siendo proveedor1, versión1, revisión4; fuente2, revisión2, política CONEXION; columnas CONSECUTIVO/NIT/BENEFICIARIO varchar(250), latin1_swedish_ci. La consulta que genera el proveedor coincide exactamente con [SQL29](../Sql/29-explain-proveedor-doc127.sql), incluido el alias histórico BENFICIARIO. El operador debe ejecutar manualmente los dos SELECT y EXPLAIN en DA/empresa2/módulo11/ref6, y aportar key/possible_keys/type/rows/Extra y metadatos, sin credenciales ni filas de negocio.

**C-02 /7.5: falta completar aceptación y gates.** El manifiesto cross_cutting exige unit y manual_qa. La autorización para ejecutar las dos pruebas no acredita QA humana. Se conserva la declaración histórica de creación/importación; no se pide repetir cargas. Falta la observación humana actual de apertura/cierre con Escape/Cerrar y navegación del gestor tras cerrar Excel, seguida de revisión/gates del SHA final una vez resuelto2.4.

## Advertencias y controles actuales

**W-01:**7 SELECT de control confirmaron ambiente/contrato y metadatos; information_schema.STATISTICS devolvió cero filas para imp_02_egresos. Es el resultado observado de metadatos, no un EXPLAIN ni una conclusión de rendimiento. No se creó índice ni se alteró tabla/clave/collation. El plan aportado debe contrastarse antes de decidir una adecuación SQL.

**W-02:** revocación física, política COLUMNA real, COMMIT/red físicos y20000 filas confirmadas reales no se ejecutaron. Mantienen cobertura local o límites declarados conforme al diseño. Los tiempos de capacidad son HTTP; la memoria IIS es compartida y muestreada. No se amplía la aceptación por inferencia.

Build: reutilizado el MSBuild aislado net461 con0 errores y308 advertencias históricas. No hay cambios Git en VB/proyecto desde el corte local y las fuentes compiladas inventariadas conservan sus hashes. El ensamblado aislado y el instalado tienen hashes distintos registrados: no se desplegó ni se sustituyó bin de IIS.

## Siguiente paso mínimo del operador

1. Ejecutar manualmente [SQL29](../Sql/29-explain-proveedor-doc127.sql) en la DA indicada y pegar sólo sus metadatos/plan. No cambiar el SQL, índices o datos para forzar un resultado.
2. Informar si la interfaz actual abre/cierra Excel con Escape/Cerrar y permite navegar en el gestor después. No crear otra plantilla ni importar de nuevo; si hay fallo, informar la acción y el mensaje.
3. Con ambas evidencias, revisar las limitaciones, actualizar2.4 y reevaluar7.5/OPSXJ sobre el SHA entregado. Esta revisión no activa aprobaciones de entorno, no archiva y no cierra Jira.

La regla manual de EXPLAIN está en D-10 del diseño y en el encabezado de SQL29. El requisito de QA está en opsxj-governance.json (requiredEvidence: unit, manual_qa). No se solicitan credenciales, cookies ni cadenas de conexión. Los datos técnicos de ejecución permanecen en .tmp/doc127-cierre-revision/, excluidos de Git.
