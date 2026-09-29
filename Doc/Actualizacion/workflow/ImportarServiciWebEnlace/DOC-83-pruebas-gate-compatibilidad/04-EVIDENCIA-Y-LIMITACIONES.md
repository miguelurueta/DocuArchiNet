# Evidencia y limitaciones

## Ejecución determinística DOC-83

Fecha base: 2026-09-27. Refinamiento de reimportación: 2026-09-28.

```powershell
npm.cmd --prefix tools/e2e run test:doc83:closure
npm.cmd --prefix tools/e2e run test:doc83:regression
```

Resultados:

- Matriz ejecutable: `4/4` PASS, incluidos tres casos negativos controlados.
- Regresión consolidada después de `fix4`: `155/155` PASS.
- MSBuild de GestionDocumental-Docuarchi.net.sln: PASS, 0 errores; conserva advertencias preexistentes del proyecto.
- E2E real `import-sii-enlase-anonymous`: PASS; bloqueo opaco `FEATURE_DISABLED`, cero elementos y sin cambios.
- E2E real `import-sii-enlase-assignment`, camino negativo: PASS; `assignmentAction=EXPLICIT`, `assignmentResult=BLOCKED`, `authoritativeValidation=CONFIRMED`, un control intacto y `sinCambios=SI`.
- Diagnóstico de solo lectura previo a la importación: trámite 290 (`mutacionregmer`), checklist obligatorio 265, `Recibo De Caja` tipo 186, serie 15, subserie 20 y cero coincidencias físicas.
- E2E real `import-sii-enlase-execution` sobre la tarea 220589: PASS; un elemento persistido, siete controles, evidencia física confirmada, sin efectos de expediente ni transición de tarea.
- E2E real `import-sii-enlase-assignment`, camino positivo: PASS; `assignmentAction=EXPLICIT`, `assignmentResult=ASSIGNED`, `authoritativeValidation=CONFIRMED`, cambio en `workflow-assignment-state` y `sinCambios=NO`.
- E2E real posterior a `fix2`, `import-sii-enlase-ui` sobre tarea 220588/trámite 244: PASS; preparación individual sin tipología confirmada, preview/foco/responsive confirmados, una sola consulta, siete controles invariantes y ninguna mutación.
- La corrida `fix2` ya no inyecta `CodigoBarras` desde Playwright: observa y deja pasar la petición exacta del navegador; el servidor resuelve la referencia SII autoritativa desde la tarea.
- Importación y asignación usaron autorizaciones y reservas de ciclo de vida independientes. Al finalizar, gate `false`, usuarios/grupos vacíos e integridad legacy confirmada.
- La primera versión del comando consolidado ejecutó desde `tools/e2e` y produjo `26` fallos `ENOENT`; se corrigió el lanzador para fijar la raíz del repositorio. No fue un defecto productivo.

## Evidencia real reutilizada

| Fuente | Resultado registrado |
|---|---|
| DOC-80 `README.md` | Lectura real PASS, siete controles sin cambios, gate restaurado. |
| DOC-81 `04-PRUEBAS-SEGURIDAD-ROLLBACK.md` | Ejecución múltiple real PASS, una intención/ejecución y persistencia física 2/2, sin transición de tarea, gate restaurado. |
| DOC-82 `04-PRUEBAS-SEGURIDAD-Y-OPERACION.md` | UI real PASS, nueve anexos, flujo múltiple/preview/foco/responsive y siete controles sin cambios. |

La evidencia histórica demuestra esos recorridos en las fechas y recursos originales. No concede autorización para una nueva corrida ni demuestra disponibilidad actual del proveedor.

## Limitaciones

- La prueba manual posterior detectó que la preparación no abría para `correccionesregmer` (trámite 244). El diagnóstico autoritativo confirmó `OBLIGA_LISTA_CHEQUEO=0` y catálogo vacío: el legacy admite importar sin tipología, mientras el contrato moderno la exigía siempre.
- La corrección publica `DocumentTypeRequired`, consulta la misma bandera de Radicación en capabilities, preflight y almacenamiento, y permite una selección sin tipología únicamente cuando la política autoritativa lo indica. No crea una tipología ficticia. Cuando la bandera es `1`, se conserva la validación contra el catálogo.
- Una validación manual controlada posterior a esa primera corrección mantuvo el error. La revisión encontró competencia entre el listener legado de `a_adj_service_web` y el handler moderno, además de una E2E que reescribía la petición para agregar `CodigoBarras`.
- `fix2` inhibe el listener legado cuando el trigger está activo y enlazado a la UI moderna, invalida la caché de `Webworkflow.js`, del adaptador SII y de la UI, y elimina la reescritura de la petición en la E2E.
- La E2E UI real posterior a `fix2` pasó. El video manual posterior usó la tarea 220589, ya asignada por una E2E anterior, y mostró además que el selector predeterminaba `Constancia De Inscripción` aunque el trámite 290 tiene un único tipo obligatorio: `Recibo De Caja` (186).
- `fix3` prioriza el único tipo marcado `Required` antes de la heurística de constancia y publica en la UI el código funcional seguro del preflight; una tarea consumida podrá distinguirse de una tipología inválida sin exponer diagnósticos internos.
- La regresión posterior a `fix4` pasó `155/155` y MSBuild compiló sin errores, conservando advertencias preexistentes. El cierre conserva pendiente la aceptación manual sobre una tarea ENLASE operable; la tarea 220589 no puede acreditar ese cierre porque ya fue asignada.
- El refinamiento `fix4` elimina `DOCUMENT_ALREADY_IMPORTED` como veto terminal. Un antecedente físicamente vigente se presenta con `View`, `Preview` y `Reimport`; la preparación transporta `ReimportRequested=true` y el preflight registra `DOCUMENT_REIMPORT_EXPLICIT`. Sin esa acción explícita, la misma identidad externa falla con `DOCUMENT_REIMPORT_CONFIRMATION_REQUIRED` para evitar una duplicación accidental.
- La operabilidad ENLASE se calcula únicamente sobre el estado activo actual de tarea y ruta (`FECHA_FIN IS NULL`, `ESTADO_TAREA=0`), sin consultar ciclos históricos. Una devolución o reapertura que cree el estado activo correspondiente vuelve a ser operable.
- El repositorio de estado ya verifica existencia física: un antecedente confirmado cuyo documento desapareció del gabinete verificable vuelve a `Disponible` y no exige la marca de reimportación.
- Dos anexos con `ExternalKey` distintas pueden usar la misma tipología en una intención. La configuración legacy del trámite sigue siendo autoritativa. DOC-83 entrega `Sql/001-allow-multiple-cash-receipts-mutacionregmer.sql` y su rollback para conservar `OBLIGATORIO=1` y cambiar únicamente `UNICO=0` en checklist 265/trámite 290/tipo 186; el SQL no fue ejecutado automáticamente.
- `Reimportar` es aditivo y conserva el documento anterior para auditoría. La sustitución destructiva o creación automática de una nueva versión no forma parte de `fix4`.
- Las pruebas estructurales no prueban por sí solas comportamiento completo, calidad visual ni disponibilidad SII.
- La E2E confirmó tanto el rechazo por requisito faltante como la asignación posterior al satisfacerlo mediante una importación real.
- El artefacto canónico del escenario conserva el resultado más reciente (`ASSIGNED`); el resultado anterior `BLOCKED` permanece en la salida saneada de consola porque el artefacto por escenario se sobrescribe.
- La tarea 220589 se reutilizó por autorización expresa del usuario. Las reservas locales consumidas se respaldaron antes de rearmarlas; no se alteraron datos de negocio fuera de la importación y asignación autorizadas.
- La evidencia corresponde al ambiente y recurso ejecutados; no demuestra disponibilidad futura del proveedor ni generaliza la configuración documental a otros trámites.
- No se almacenan credenciales, cookies, tokens, conexiones ni contenido documental.

## Refinamiento fix5: proyeccion inmediata

- La prueba manual sobre la tarea 220586 demostro persistencia correcta: el documento aparecia despues de recargar completamente el sistema, pero no inmediatamente al cerrar el resultado.
- La causa era que el adaptador no tenia un `appendDocument` productivo y caia en `Button_actualiza_trevie_seleccion`, handler asociado a `ID_TAREA_SELECCIONDA` y no al contexto ENLASE.
- `fix5` conecta la reconciliacion confirmada con `insert_row_documento_relacionado(..., "wf", 1)`, evita duplicados por `documentId` y mantiene el resultado abierto si no puede comprobar la fila. Pruebas focales: 9/9 PASS. Regresion consolidada: 164/164 PASS.

## Refinamiento fix9: contrato visual ENLASE completo sin postback

La fila visible incorporada por `fix5` no era funcional. El video manual mostró que al interactuar aparecía `Inconsistencia funcion SolicitaIdTipoImagen: No se puede cambiar el nombre del DataSet a una cadena vacía`. La causa era contractual: `GridView_list_documento_relacion` usa `id_rad`/`idd_rad` y exige `gabinete|documentId|radicado|DBT|nombre|taskId|firma|icono`; el adaptador estaba enviando vacíos en gabinete, radicado y tipo físico, como si fuese `GridView_list_documento_relacion_wf`.

`fix9` no actualiza el GridView desde servidor, no ejecuta `DataBind`, no hace postback y no agrega una segunda consulta. El recorrido implementado es:

```text
PreAlmacenaDocumentoAnexosEnlaceIntegracionSII(..., ByRef imagen)
  -> LegacyEnlaseImportDocumentStorageAdapter.CrearProyeccion(comando, imagen, idImagen)
  -> ResultadoFaseImportacion.ProyeccionDocumentoEnlase
  -> ResultadoElementoImportacion.ProyeccionDocumentoEnlase
  -> ImportItemResultDto.EnlaseProjection
  -> progress adapter
  -> reconciliation (preserva la proyección efímera al reemplazar el estado)
  -> document-list-adapter valida documento/tarea/campos
  -> insert_row_documento_relacionado(contratoCompleto, "rad", 1)
  -> GridView_list_documento_relacion
```

La traducción a la cadena delimitada solo ocurre en la frontera legacy del navegador. El DTO público es tipado y no expone `dato_lista`. Si falta un campo, el documento o la tarea no coinciden, el adaptador falla cerrado: no inserta una fila parcial, no recarga la lista y mantiene la ventana de resultado abierta para diagnóstico.

Verificación local posterior a `fix9`:

- Pruebas focales de contrato, lista, reconciliación y registro de assets: `46/46` PASS.
- Regresión consolidada DOC-83: `177/177` PASS.
- MSBuild del proyecto con la instalación disponible de Visual Studio 18: PASS, 0 errores; 310 advertencias preexistentes.
- La invocación explícita con `VisualStudioVersion=17.0` falló porque esa instalación no contiene `v17.0/WebApplications`; la repetición con los targets instalados `v18.0` pasó. Fue una diferencia de herramienta local, no un error de compilación del código.
- `Web.config` queda con gate `false` y audiencias vacías.
- Sigue pendiente la aceptación manual del clic sobre la fila recién proyectada; no se declara una nueva E2E real para `fix9`.

## Refinamiento fix10: importación sin tipología proyectada inmediatamente

La regresión manual posterior a `fix9` mostró que el documento volvía a persistir sin aparecer en la interfaz. El almacenamiento legacy sin tipología retorna gabinete, identificador, radicado, extensión, firma e icono, pero deja vacíos `DBT` y `tipodocumental`. `CrearProyeccion` utilizaba `tipodocumental` como respaldo de `DBT`; ambos vacíos hacían que la validación cerrada descartara toda la proyección antes de enviarla al navegador.

`fix10` conserva `DBT` cuando existe y usa únicamente `stru_datos_image_lista.extension` cuando no existe. La extensión proviene del mismo almacenamiento exitoso, por lo que no se infiere tipología, no se consulta nuevamente el servidor y no se modifica la superficie legacy protegida. El adaptador cliente sigue exigiendo los ocho campos y mantiene el destino `rad`.

Verificación local de `fix10`: pruebas focales `49/49` PASS; regresión DOC-83 `177/177` PASS; MSBuild Visual Studio 18 PASS con 0 errores y 310 advertencias preexistentes. El gate final quedó en `false`, con usuarios y grupos vacíos. La aceptación manual en navegador sigue pendiente y por eso las tareas de cierre permanecen abiertas.
## Refinamiento fix11: aceptación visual dentro del runner

La activación temporal aislada usada durante diagnósticos manuales no satisface el runbook. `fix11` registra `import-sii-enlase-manual-visual` como ejecución mutadora: abre un navegador autenticado visible, reserva la tarea, captura siete controles, limita dos confirmaciones a un plazo total de diez minutos y restaura el gate en `finally`.

El primer hito exige una fila nueva sin recarga y valida el contrato completo `gabinete|documentId|radicado|tipoFisico|nombre|taskId|firma|icono`. El segundo permite abrir el documento y rechaza cualquier diálogo de error. No se ejecuta una importación API paralela: la única mutación es la iniciada por el usuario desde la interfaz oficial.

La prueba real de este escenario permanece pendiente hasta completar sus confirmaciones en TTY. No se declara aceptación manual anticipadamente.

## Refinamiento fix12: línea base inequívoca para recursos reutilizados

La primera corrida autorizada de `import-sii-enlase-manual-visual` terminó con `IMPORT_E2E_MANUAL_VISUAL_ROW_UNAVAILABLE`. Aunque el operador confirmó una fila visible, el runner observó el mismo conjunto de `id_rad` antes y después; además, los siete controles quedaron invariantes, incluida la ausencia de una nueva intención, ítem o transición. La fila confirmada era preexistente y no constituía evidencia de proyección.

Un primer reajuste retiró las filas preexistentes solo del DOM para intentar aislar la reproyección. La siguiente corrida demostró que ese enfoque era invasivo y ambiguo: dejó vacía la lista documental, la lista SII ni siquiera llegó a cargarse y las tres filas reaparecieron por fuera del inserter observado. Los siete controles permanecieron invariantes, por lo que tampoco existió una importación. El resultado `IMPORT_E2E_MANUAL_VISUAL_PROJECTION_NOT_OBSERVED` se conserva como fallo real y el gate quedó restaurado en `false`, con audiencias vacías.

El reajuste definitivo no retira ni oculta filas. Espera que `PageRequestManager` quede inactivo, abre automáticamente la ventana moderna y exige estado `resultados` con al menos una acción `Importar/Reimportar` antes de tomar la línea base. Después observa el inserter contra el GridView vivo y rechaza recarga o postback. Si SII no entrega la lista, la corrida termina con `IMPORT_E2E_MANUAL_VISUAL_ITEMS_UNAVAILABLE` antes de solicitar una confirmación visual. La corrida real con este orden continúa pendiente.

## Refinamiento fix13: estado reconciliado proyectable

La corrida real posterior sí mostró la lista SII (`initialRows=3`, `siiActions=8`) y el operador completó la incorporación, pero la fila no apareció sin recargar. El operador respondió `NO` y el runner conservó correctamente el fallo `IMPORT_E2E_MANUAL_VISUAL_REJECTED`. La evidencia saneada registró cambios en intención, ítem y transición de importación, sin cambios en expediente, relación, caché ni índices: la mutación ocurrió y la falla quedó aislada en la proyección cliente. El gate fue restaurado a `false`, con usuarios y grupos vacíos.

La causa fue una asimetría de estados. `reconciliation.complete(...)` normaliza `Completada`/`Completado` como `Completado`, y `shouldCloseAfterResult(...)` reconoce `Disponible` o `Completado` como confirmados. Sin embargo, `documentList.collect(...)` aceptaba únicamente `Disponible`; por ello descartaba el elemento persistido antes de invocar el adaptador ENLASE y `insert_row_documento_relacionado`.

`fix13` alinea el filtro de la lista con el conjunto cerrado de estados confirmados ya usado por el cierre: `Disponible` y `Completado`. No amplía estados inciertos, no agrega refresco, no hace postback y conserva la validación del contrato ENLASE completo. La regresión específica demuestra que `Completado` conserva `enlaseProjection`, mientras `Verificando` continúa rechazado. Verificación local: pruebas focales `18/18` PASS y regresión DOC-83 `181/181` PASS. La aceptación visual real sigue pendiente hasta una nueva corrida autorizada; no se declara PASS anticipadamente.

## Refinamiento fix14: invalidación de caché después del postback observado

La corrida posterior a `fix13` alcanzó la lista SII con `initialRows=4` y `siiActions=8`. El primer valor es la línea base de documentos relacionados antes de importar; el segundo es la cantidad de acciones SII disponibles y no significa que se hayan importado ocho documentos. El operador observó una fila y confirmó el hito, pero el runner detectó un `beginRequest` de WebForms y detuvo la corrida con `IMPORT_E2E_MANUAL_VISUAL_POSTBACK_OBSERVED`. La intención, el ítem y su transición cambiaron; los controles de expediente, relación, caché e índice no cambiaron. El recurso se liberó y el gate quedó restaurado.

La revisión comprobó que el servidor seguía publicando `importar-servicio-web-ui.js` con versión `doc83fix8` y `importar-servicio-web-document-list-adapter.js` con `doc83fix9`, aunque ambos módulos habían cambiado posteriormente. Eso permitía que el navegador reutilizara una implementación anterior compatible con el refresco oculto de WebForms. `fix14` cambia únicamente las versiones públicas de ambos assets a `doc83fix13` y actualiza la prueba de registro; no agrega un postback ni modifica la persistencia. Pruebas focales `20/20` PASS, regresión DOC-83 `181/181` PASS y OpenSpec estricto PASS. La relación causal completa debe confirmarse en una nueva corrida autorizada; el último E2E sigue siendo FAIL.

## Refinamiento fix15: proyección eliminada en la frontera autoritativa

La corrida posterior abrió con `initialRows=5`: el documento de la corrida anterior quedó persistido y apareció únicamente después de volver a cargar la página. Una nueva incorporación volvió a cambiar intención, ítem y transición, pero no produjo una fila inmediata; el operador respondió `NO` y se conservó `IMPORT_E2E_MANUAL_VISUAL_REJECTED`. Esto descartó la caché como causa suficiente.

El recorrido real reveló que `ImportServiceOrchestrator.Execute` sí construía `EnlaseProjection`, pero el endpoint pasaba inmediatamente ese resultado por `ServicioReconciliacionImportacion.ProjectExecutionResult`. El método reemplazaba todos los ítems por el snapshot persistido; como la proyección visual es deliberadamente efímera, se eliminaba antes de serializar la respuesta. El JavaScript nunca recibía los ocho campos y fallaba cerrado.

`fix15` conserva la proyección del resultado de almacenamiento después de la reconciliación autoritativa únicamente para capacidad ENLASE y cuando coinciden `ClientItemId`, `ExternalKey`, documento y tarea; además exige gabinete, radicado, tipo físico, nombre e icono. No persiste información visual, no reconstruye una cadena legacy y no la propaga en consultas posteriores. Cualquier diferencia mantiene `EnlaseProjection` vacía. Verificación: pruebas focales `55/55` PASS, regresión DOC-83 `186/186` PASS, MSBuild PASS con 0 errores y advertencias preexistentes, y OpenSpec estricto PASS. La aceptación E2E continúa pendiente.

## Refinamiento fix16: separación entre inserción e interacción

La corrida posterior a `fix15` inició con seis filas y el operador confirmó que apareció una fila nueva sin recargar. Esto acredita que la proyección inmediata ya alcanzó el DOM. La corrida se detuvo antes de validar el contrato porque observó un postback durante el primer hito. El escenario ahora registra una categoría saneada del origen (`DOCUMENT_REFRESH`, `DOCUMENT_INTERACTION`, `OTHER_CONTROL` o `UNKNOWN`), además del número de proyecciones y filas agregadas, y valida primero que la fila haya sido creada. Así podrá distinguir un refresco indebido de una apertura anticipada del documento sin guardar identificadores ni contenido. Regresión DOC-83 `186/186` PASS; el último E2E continúa FAIL hasta clasificar ese postback.

## Aceptación visual final autorizada

La corrida final siguió el orden gobernado y terminó correctamente. Partió de `initialRows=7` y `siiActions=8`; el operador importó o reimportó sin recargar, confirmó primero la aparición inmediata de la fila y solo después abrió una vez el documento recién agregado. El artefacto saneado registra `success=true`, una proyección y los códigos `manualVisual=CONFIRMED`, `gridProjection=CONFIRMED` y `rowInteraction=CONFIRMED`.

Los siete controles se evaluaron: intención, ítem y transición cambiaron por la mutación autorizada; índice, caché, relación y expediente no cambiaron. La reserva recorrió `READY`, `RESERVED` y `CONSUMED`. Al finalizar, el gate quedó en `false`, con usuarios y grupos vacíos. Esta corrida cierra la aceptación visual exigida por la tarea 3.5; no sustituye las compuertas OPSXJ todavía pendientes de 5.3 y 5.4.

## Aceptación manual de reimportación explícita

La tarea 220587 fue inspeccionada primero sin mutaciones: SII devolvió nueve anexos, siete disponibles, dos importados y ninguno en revisión. La corrida UI de diagnóstico se detuvo por una expectativa de tipología predeterminada del perfil, pero mantuvo invariantes los siete controles y restauró el gate; ese fallo no invalidó la disponibilidad de `Reimportar`.

Con autorización mutadora separada, el operador confirmó que accionó específicamente `Reimportar`. La corrida gobernada terminó PASS con una fila nueva inmediata y apertura sin error. El artefacto saneado registró una proyección, cambios en intención, ítem y transición, ausencia de cambios en índice, caché, relación y expediente, recurso consumido y gate restaurado. La identificación del botón `Reimportar` proviene de la confirmación explícita del operador; el artefacto actual conserva los efectos técnicos, pero no persiste el tipo de acción para evitar datos de selección innecesarios.

## Refinamiento fix17: contención visual de datos extensos

La revisión de cierre reportó datos superpuestos en la lista SII. La causa fue que las celdas combinaban `white-space: nowrap` y ancho máximo, pero solo “Noticia” recortaba el contenido; valores largos de naturaleza, referencia, anexo o tipo podían pintarse fuera de su celda y quedar bajo columnas vecinas o la columna fija de acciones.

`fix17` asigna clases y anchos semánticos a encabezados y datos en las variantes de constancias y ENLASE, aplica elipsis con el valor completo en `title`, fija un fondo opaco para acciones y conserva el scroll dentro de la tabla. CSS y ambos renderizadores renovaron su versión pública de caché. Verificación local inicial: focales `26/26` PASS, regresión DOC-83 `187/187` PASS, comprobación de caché `17/17` PASS y OpenSpec estricto PASS. La aceptación real quedó incorporada posteriormente en la corrida conjunta de `fix18`.

## Refinamiento fix18: acciones persistentes y preview navegable

La revisión visual posterior identificó tres defectos de composición: en preparación masiva las acciones podían quedar debajo de la lista, el detalle “Plan previsto” repetía información técnica sin aportar una decisión al usuario y el modo preview ocultaba `Volver a la lista` en escritorio mientras reintroducía la lista mediante CSS. El visor también heredaba alturas mínimas que producían desajuste vertical.

`fix18` conserva el preflight y el bloqueo de `Crear intención` hasta su respuesta satisfactoria, pero elimina la representación redundante del plan. Solo la lista de documentos preparados tiene scroll; el footer con `Cancelar` y `Crear intención` permanece visible. El preview sustituye la lista, muestra siempre `Volver a documentos` y usa una columna flex donde el iframe ocupa el espacio restante. CSS y UI renuevan su versión pública de caché. La validación focal pasa; la aceptación visual real de este refinamiento permanece pendiente y 5.3 continúa abierta.

La primera corrida visual de `fix18` confirmó manualmente los tres hitos con nueve documentos: tabla contenida, acciones de preparación visibles sin “Plan previsto” y preview alineado con retorno visible. Después de que el operador probó el retorno, el runner intentó pulsar nuevamente el botón ya oculto y terminó con timeout. El fallo pertenece a la automatización posterior a la aceptación, no a la interfaz ni a los controles observados. El runner acepta ahora tanto el retorno ejercido por el operador como el retorno automático, y exige en ambos casos que la lista quede nuevamente visible. La evidencia canónica continúa pendiente hasta repetir la corrida completa con cierre PASS.

La repetición autorizada terminó PASS. Observó nueve filas, 45 celdas de datos y siete celdas truncadas mediante elipsis; la preparación masiva presentó nueve elementos con sus dos acciones persistentes y sin el bloque retirado; el preview mostró el iframe, retorno visible y alineación vertical válida. El artefacto saneado registra `cellContainment`, `internalScroll`, `preparationActions` y `previewNavigation` en `CONFIRMED`, con `mutations=NOT_OBSERVED`. Los siete controles permanecieron invariantes y el gate quedó restaurado a `false`, con usuarios y grupos vacíos. La aceptación visual de `fix17/fix18` queda cerrada; 5.3 permanece abierta únicamente por las compuertas OPSXJ finales.
