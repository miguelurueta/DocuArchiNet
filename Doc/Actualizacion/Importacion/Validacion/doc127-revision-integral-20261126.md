# DOC127: revisión integral del corte y dependencias de cierre

**Revisión aprobada con advertencias;33/33 tareas.**14 requisitos y42 escenarios con evidencia clasificada. QA real de escritorio delegada4/4 PASS; no son42 pruebas nuevas ni QA humana declarada por el usuario.

Corte actual `1c1637dd84839fd2fc2d88bef309d39182b96b40`, revisión por Codex con openspec-verify-change. [QA de escritorio delegada](doc127-qa-delegada-20261126.md),22 regresiones locales actuales y reutilización del build8d1f2159 por fuentes compiladas sin cambios. Se conservan las evidencias reales anteriores; sin nuevas cargas o despliegue.

| Dimensión | Resultado |
| --- | --- |
| Completitud |33/33 tareas; sin pendientes |
| Corrección |14/14 requisitos con evidencia clasificada y límites explícitos |
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
| RQ-11: Interfaz vigente | [ImportacionExcelVista.js](../../../../js/gestion/ImportacionExcel/ImportacionExcelVista.js#L59) | tests/doc126-importacion-excel-ui.test.cjs | Vista actual con formulario auxiliar; lectura/aislamiento y asistente reales; QA real de escritorio delegada4/4 PASS. |
| RQ-12: Build y hallazgos | [MySqlConfirmacionExcelWriter.vb](../../../../Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlConfirmacionExcelWriter.vb#L158) | tests/ImportacionExcelConfirmacionTests.cs | Build8d1f2159 reutilizado por fuentes compiladas intactas; regresión actual22/22 PASS,29 previas conservan su corte. |
| RQ-13: Operación y despliegue | [Operacion-Excel.md](../../../../Doc/Actualizacion/Importacion/Operacion-Excel.md#L27) | Doc/Actualizacion/Importacion/Sql/README.md | Recuperación por ID, sin pausa por fila; respaldo de código y SQL estructural separados. |
| RQ-14: Cierre verificable | [legacyGovernanceService.js](../../../../tools/opsxj/scripts/lib/legacyGovernanceService.js#L347) | openspec/changes/doc-127-importacion-excel-07-verificacion-integral-cierre/tasks.md | Revisión técnica aprobada; QA delegada y plan manual completados, con gates por SHA de entrega. |

El JSON enumera los42 escenarios WHEN/THEN, sus rutas, líneas, hashes y evidencias originales. La revisión usa inspección de fuentes y evidencia existente; no convierte heurísticas de correspondencia en ejecución real. Se conservan las fechas y diferencias de los81/141 casos históricos,55 locales reutilizados,18 casos reales de capacidad,6 rechazos y2 asistentes actuales.

## Críticos resueltos

**C-01 /2.4 resuelto.** [Plan manual y metadatos](doc127-plan-consulta-20261126.md) iniciados por el operador: ALL, key/possible_keys NULL, rows estimadas25538, Using where. Sin índices observados. Proveedor1/v1 revisión4 y política CONEXION revisión2 coinciden con SQL29. Se completa revisión del acceso con advertencia; no se certifica latencia ni se exige crear índice.

**C-02 /7.5 resuelto.** El usuario delegó la comprobación con «hazlo por mi». Codex ejecutó QA en Chrome/IIS reales por Windows UI Automation: abrir, Escape, Cerrar, expandir/seleccionar expediente de prueba y reabrir.4/4 PASS, sin acciones de escritura. manual_qa registra este recorrido de escritorio; no una respuesta SI o una observación humana inexistente. La consola histórica conserva PENDING. Gates persistidos deben corresponder al SHA entregado.

## Advertencias y controles actuales

**W-01:** el plan manual muestra recorrido completo ALL, sin índice elegido, coherente con cero índices en metadatos. rows25538 es estimación del optimizador. No se mide latencia ni lecturas reales; LIMIT2 no garantiza pocas filas examinadas. Cualquier optimización requiere análisis y SQL manual separado.

**W-02:** revocación física, política COLUMNA real, COMMIT/red físicos y20000 filas confirmadas reales no se ejecutaron. Mantienen cobertura local o límites declarados conforme al diseño. Los tiempos de capacidad son HTTP; la memoria IIS es compartida y muestreada. No se amplía la aceptación por inferencia.

Build8d1f2159: net461 aislado PASS,0 errores/308 advertencias. No hay cambios VB/proyecto/C#/Web.config hasta1c1637dd, por lo que se reutiliza; no se repite ni despliega. El editor de perfiles actual tiene22/22 regresiones locales PASS y su cambio de caché en la página quedó observado en la QA actual. Los resultados29/29 y E2E anteriores conservan sus cortes originales.

## Entrega técnica

No hay críticos pendientes. Se conservan W-01 (acceso ALL sin índice/SLA) y W-02 (variantes físicas sin ejecución). La QA delegada cuenta con capturas privadas, observaciones y hashes en el [informe saneado](doc127-qa-delegada-20261126.json). No se necesita repetir importaciones ni pedir al usuario que vuelva a hacer esta comprobación. La publicación/archivo/cierre Jira se ejecuta por su flujo; este informe acredita revisión técnica.

Actualización por cambio concurrente90faca10: revisión sobrea09a5440 y regresión local del editor **26/26 PASS**. La página sólo cambió el enlace de caché de PerfilImportacionEditor.js; al normalizar esa referencia coincide íntegramente con la página de la QA. Excel JS/CSS, DLL IIS y Web.config conservan sus hashes; no hay cambios compilados desde el build8d1f2159. La QA real conserva su corte1c1637dd; no se finge una repetición ni se reemplazan sus hashes. Auditoría actual y hashes en el JSON de QA.
