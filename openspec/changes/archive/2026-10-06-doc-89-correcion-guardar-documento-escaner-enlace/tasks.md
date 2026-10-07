<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## 1. Caracterizacion y guardas de alcance

- [x] 1.1 Crear una prueba estructural DOC-89 en `tools/e2e/tests` que lea `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb`, `js/workflow/WebFormEscan.js` y los estilos relacionados. Origen: D-06, RQ-01, RQ-02, RQ-04
- [x] 1.2 Fijar en la prueba que `ButtonAlmacenar` permanece dentro de `UpdatePanel_boton_tool` y que no existe `PostBackTrigger` ni `RegisterPostBackControl` para ese control. Origen: D-03, RQ-01. Cobertura adicional: D-06
- [x] 1.3 Fijar en la prueba la causa actual: la rama `ButtonAlmacenar` no puede llamar `posicion_update_pogres_modal` ni agregar `overlay_`, mientras `Button_guardar_desicion` debe conservar ambas operaciones. Origen: D-01, RQ-02. Cobertura adicional: D-02, D-06, RQ-03
- [x] 1.4 Fijar una sola llamada productiva a `UploadSaveFileScan` y una sola proyeccion por `Hidden_result_load_` mediante `insert_row_documento_relacionado`. Origen: D-03, RQ-05. Cobertura adicional: D-06
- [x] 1.5 Caracterizar en la prueba las cinco ramas `Hidden21`, `Gurdar_documento_htpp_server`, `OnHttpUploadSuccess` y la continuacion `-2003`, sin corregirlas ni cambiar sus contratos. Origen: D-03, RQ-04. Cobertura adicional: D-06

## 2. Implementacion quirurgica

- [x] 2.1 Separar en `InitializeRequest` de `workflow/Webworkflow.aspx` la rama de `ButtonAlmacenar` de la rama de `Button_guardar_desicion`. Origen: D-01, RQ-02. Cobertura adicional: D-02, RQ-03
- [x] 2.2 Implementar, localmente en la pagina padre, la presentacion compacta de `#progres_bar` para `ButtonAlmacenar`, sin `overlay_`, dimensiones completas, recarga ni temporizador. Origen: D-01, RQ-02. Cobertura adicional: D-05, RQ-07
- [x] 2.3 Implementar la limpieza idempotente del indicador compacto en `CheckStatus/endRequest` para exito y error, restaurando solo el estado transitorio introducido. Origen: D-04, RQ-02, RQ-06
- [x] 2.4 Conservar sin cambios la llamada y limpieza modal de `Button_guardar_desicion` y el comportamiento de todos los demas postbacks. Origen: D-02, RQ-03
- [x] 2.5 Confirmar por diff que no se editaron `WebFormEscan*`, `online_demo_operation.js`, `Webform_save_digital_image*`, `ClassAlmacenamiento.vb`, `Webworkflow.aspx.vb`, `.overlay_` ni las reglas globales de `.ctw-loading-indicator`; detener y volver a refinar si fuera indispensable tocarlos. Origen: D-03, RQ-04. Cobertura adicional: D-05, RQ-07

## 3. Pruebas focales y regresion

- [x] 3.1 Ejecutar la prueba estructural DOC-89 y registrar comando, codigo de salida y conteos saneados. Origen: D-06, RQ-01, RQ-02, RQ-03, RQ-04, RQ-05, RQ-06
- [x] 3.2 Ejecutar las suites existentes de activacion visual moderna, Workflow, documentos relacionados y carga SII/ENLASE que cubran `Webworkflow.aspx`. Origen: D-05, RQ-07. Cobertura adicional: D-06, RQ-03, RQ-04
- [x] 3.3 Verificar de forma focal que exito y rechazo ocultan el progreso, que no queda `overlay_` residual y que `Button_guardar_desicion` sigue usando el overlay. Origen: D-02, RQ-03. Cobertura adicional: D-04, RQ-02, RQ-06
- [x] 3.4 Verificar que no se introdujeron `location.reload`, recarga de iframe, `DataBind`, segundo clic programatico, retry ni `setTimeout` en el recorrido. Origen: D-03, RQ-01. Cobertura adicional: D-04, RQ-05
- [x] 3.5 Ejecutar `git diff --check` y revisar el diff completo contra la lista de paths excluidos. Origen: D-03, RQ-04. Cobertura adicional: D-06, RQ-07

## 4. Compilacion y QA manual

- [x] 4.1 Compilar `GestionDocumental-Docuarchi.net.sln` con MSBuild en Debug y registrar comando, codigo de salida, errores y advertencias. Origen: D-07, RQ-08
- [x] 4.2 Reproducir manualmente Enlace de documentos: abrir digitalizacion, elegir tipologia, aceptar, observar interfaz/progreso y verificar un unico nodo sin perdida del visor o seleccion. Origen: D-01, RQ-02. Cobertura adicional: D-03, D-07, RQ-05, RQ-08
- [x] 4.3 Reproducir el rechazo de almacenamiento y confirmar mensaje existente, ausencia de nodo y limpieza del indicador. Origen: D-04, RQ-06. Cobertura adicional: D-07, RQ-08. Cierre por aceptacion de riesgo autorizada por el responsable el 2026-10-06, sin provocar una nueva mutacion; se conserva la evidencia focal de limpieza en error y no se declara una reproduccion manual adicional.
- [x] 4.4 Verificar manualmente `Button_guardar_desicion`, adjunto tradicional y al menos un recorrido compartido no-Workflow del escaner. Origen: D-02, RQ-03. Cobertura adicional: D-06, D-07, RQ-04, RQ-08. Cierre por aceptacion de riesgo autorizada por el responsable el 2026-10-06, sustentado en invariancia estructural y regresiones existentes; no se declara una ejecucion manual adicional de esos consumidores.

## 5. E2E integrada y autorizada

- [x] 5.1 Leer `tools/e2e/AGENT-RUNBOOK.md` y localizar la suite/helper autenticado existente que navega `workflow/Webworkflow.aspx`; no crear autenticacion, `.env` ni proyecto paralelo. Origen: D-07, RQ-08
- [x] 5.2 Extender o crear la suite DOC-89 dentro de la infraestructura existente para observar una sola navegacion inicial, un async postback de `ButtonAlmacenar`, ausencia de superficie blanca, una escritura/nodo y limpieza final. Origen: D-01, RQ-01. Cobertura adicional: D-04, D-06, D-07, RQ-02, RQ-05, RQ-06, RQ-08
- [x] 5.3 Agregar a la E2E el intento de doble interaccion y las aserciones de conservacion del visor, pagina seleccionada y ausencia de efectos fuera del recurso autorizado. Origen: D-03, RQ-04. Cobertura adicional: D-06, D-07, RQ-05, RQ-08
- [x] 5.4 Solicitar y registrar autorizacion explicita del ambiente, cuenta, tarea/expediente y archivo descartable antes de ejecutar cualquier escritura; limitar controles de datos a `SELECT` parametrizados. Origen: D-07, RQ-08
- [x] 5.5 Ejecutar la E2E real autorizada y guardar unicamente evidencia saneada de codigos, conteos, tiempos y huellas no reversibles; si falta autorizacion, dejar la tarea bloqueada sin simularla. Origen: D-07, RQ-08. Cierre aceptado por el responsable el 2026-10-06 con evidencia funcional observada y comprobaciones instrumentadas previas al falso negativo `SCANNER_LINK_E2E_VIEWER_NOT_PRESERVED`; el artefacto automatico conserva `success: false` y no se reclasifica como pase integral.

## 6. Documentacion, validacion y reversa

- [x] 6.1 Actualizar la documentacion tecnica canonica con causa, recorrido real, consumidores verificados, decision de aislamiento, garantias de no regresion y hallazgos fuera de alcance. Origen: D-03, RQ-04. Cobertura adicional: D-06, RQ-07
- [x] 6.2 Documentar resultados de pruebas, build, QA/E2E y el plan de reversa limitado a la rama visual de `ButtonAlmacenar`. Origen: D-07, RQ-08
- [x] 6.3 Ejecutar `opsxj:refine -- DOC-89 --sync`, validacion OpenSpec estricta y los gates OPSXJ aplicables; corregir cualquier perdida de trazabilidad. Origen: D-06, RQ-08. Cobertura adicional: D-07
- [x] 6.4 Revisar que cada tarea completada tenga evidencia y que ninguna deuda fuera de alcance (`OnHttpUploadSuccess`, `-2003`, `heigth`) se haya mezclado en el diff. Origen: D-03, RQ-04. Cobertura adicional: D-07, RQ-08
