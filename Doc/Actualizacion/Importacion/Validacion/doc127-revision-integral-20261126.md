# DOC127: revisión integral del corte y dependencias de cierre

**Revisión actualizada; cierre no aprobado.**32/33 tareas,14 requisitos y42 escenarios inventariados;41 con evidencia clasificada y uno pendiente de cierre. Plan manual revisado; sólo falta7.5 por QA visual actual y gates. No son42 pruebas nuevas.

Corte de código actual `8d1f2159ac4fdfda9fab3b884cbf5daa22c96390`, revisión por Codex mediante openspec-verify-change. Sustituye la evaluación documental previa de `f6d3ba2e37ee298afcf972eff11ae6afaeab58c5`. Se incorporan el plan manual del operador,29 pruebas locales y un build aislado nuevo; sin nuevas E2E, importaciones, SQL de instalación o despliegue.

| Dimensión | Resultado |
| --- | --- |
| Completitud |32/33 tareas; pendiente7.5 |
| Corrección |13/14 requisitos con evidencia clasificada;RQ-14 parcial por cierre |
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
| RQ-10: Proveedor y consumidores | [MySqlProveedorDatosIndices.vb](../../../../Infrastructure/ProduccionDocumental/ImportacionCarpetas/MySqlProveedorDatosIndices.vb#L47) | tests/ProveedorIndicesBehaviorTests.cs | Plan SQL29 manual revisado: ALL, sin índice; regresión local actual29/29 PASS. |
| RQ-11: Interfaz vigente | [ImportacionExcelVista.js](../../../../js/gestion/ImportacionExcel/ImportacionExcelVista.js#L59) | tests/doc126-importacion-excel-ui.test.cjs | Vista actual con formulario auxiliar; lectura/aislamiento y asistente reales; QA humana actual pendiente. |
| RQ-12: Build y hallazgos | [MySqlConfirmacionExcelWriter.vb](../../../../Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlConfirmacionExcelWriter.vb#L158) | tests/ImportacionExcelConfirmacionTests.cs | Build aislado actual0 errores/308 advertencias y29 pruebas locales PASS; hallazgo form1 cerrado con cobertura clasificada. |
| RQ-13: Operación y despliegue | [Operacion-Excel.md](../../../../Doc/Actualizacion/Importacion/Operacion-Excel.md#L27) | Doc/Actualizacion/Importacion/Sql/README.md | Recuperación por ID, sin pausa por fila; respaldo de código y SQL estructural separados. |
| RQ-14: Cierre verificable | [legacyGovernanceService.js](../../../../tools/opsxj/scripts/lib/legacyGovernanceService.js#L347) | openspec/changes/doc-127-importacion-excel-07-verificacion-integral-cierre/tasks.md | La revisión no aprueba cierre: falta QA visual actual y gates; plan manual completado. |

El JSON enumera los42 escenarios WHEN/THEN, sus rutas, líneas, hashes y evidencias originales. La revisión usa inspección de fuentes y evidencia existente; no convierte heurísticas de correspondencia en ejecución real. Se conservan las fechas y diferencias de los81/141 casos históricos,55 locales reutilizados,18 casos reales de capacidad,6 rechazos y2 asistentes actuales.

## Críticos que impiden archivo

**C-01 /2.4 resuelto.** [Plan manual y metadatos](doc127-plan-consulta-20261126.md) iniciados por el operador: ALL, key/possible_keys NULL, rows estimadas25538, Using where. Sin índices observados. Proveedor1/v1 revisión4 y política CONEXION revisión2 coinciden con SQL29. Se completa revisión del acceso con advertencia; no se certifica latencia ni se exige crear índice.

**C-02 /7.5: falta completar aceptación y gates.** El manifiesto cross_cutting exige unit y manual_qa. La autorización para ejecutar las dos pruebas no acredita QA humana. Se conserva la declaración histórica de creación/importación; no se pide repetir cargas. Falta la observación humana actual de apertura/cierre con Escape/Cerrar y navegación del gestor tras cerrar Excel, seguida de revisión/gates del SHA final con2.4 ya completada.

## Advertencias y controles actuales

**W-01:** el plan manual muestra recorrido completo ALL, sin índice elegido, coherente con cero índices en metadatos. rows25538 es estimación del optimizador. No se mide latencia ni lecturas reales; LIMIT2 no garantiza pocas filas examinadas. Cualquier optimización requiere análisis y SQL manual separado.

**W-02:** revocación física, política COLUMNA real, COMMIT/red físicos y20000 filas confirmadas reales no se ejecutaron. Mantienen cobertura local o límites declarados conforme al diseño. Los tiempos de capacidad son HTTP; la memoria IIS es compartida y muestreada. No se amplía la aceptación por inferencia.

Build actual de8d1f2159: MSBuild net461 aislado PASS,0 errores/308 advertencias, DLL IIS y Web.config conservados. Los cambios concurrentes de proveedor/editor se verificaron con29/29 pruebas locales PASS y revisión de SQL sin cambios. La página cambió sólo caché de recursos del editor de perfiles. Los E2E previos conservan sus cortes originales: no se afirma igualdad total de sus fuentes ni se reutiliza el build antiguo como prueba del nuevo código. [Comandos, hashes y alcance](doc127-plan-consulta-20261126.json).

## Siguiente paso mínimo del operador

1. Plan manual SQL29 ya recogido y revisado; no repetirlo ni modificar índices o datos.
2. Informar si la interfaz actual abre/cierra Excel con Escape/Cerrar y permite navegar en el gestor después. No crear otra plantilla ni importar de nuevo; si hay fallo, informar la acción y el mensaje.
3. Con la QA humana actual, revisar las limitaciones y reevaluar7.5/OPSXJ sobre el SHA entregado. Esta revisión no activa aprobaciones de entorno, no archiva y no cierra Jira.

La regla manual de EXPLAIN está en D-10 del diseño y en el encabezado de SQL29. El requisito de QA está en opsxj-governance.json (requiredEvidence: unit, manual_qa). No se solicitan credenciales, cookies ni cadenas de conexión. Los datos técnicos de ejecución permanecen en .tmp/doc127-cierre-revision/, excluidos de Git.
