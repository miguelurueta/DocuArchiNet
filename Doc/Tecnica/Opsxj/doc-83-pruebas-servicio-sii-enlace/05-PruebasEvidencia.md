# DOC-83 — Pruebas y evidencia

- Ticket: DOC-83
- Cambio OpenSpec: doc-83-pruebas-servicio-sii-enlace
- Clasificacion: cross_cutting

## Evidencia requerida

Fecha de ejecución determinística y E2E DOC-83: 2026-09-27 y 2026-09-28. Cierre visual `fix18`: 2026-09-28.

| Tipo | Comando | Resultado |
|---|---|---|
| unit/policy | `node tools/e2e/scripts/run-doc83-regression.cjs` | PASS 193/193 |
| OpenSpec/refinement | `openspec validate ... --strict` y `opsxj:refine` | PASS |
| build | MSBuild.exe GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m | PASS, 0 errores; advertencias preexistentes |
| manual_qa | `import-sii-enlase-layout-review` | PASS; 7 controles invariantes, sin mutaciones |

Estado de validación previo al rollout: regresión `193/193`, compilación Release sin errores, OpenSpec estricto y refinement PASS, aceptación visual `fix18` PASS y gate local restaurado a `false` con audiencias vacías. Tras la aprobación funcional, `Web.Release.config` activa globalmente el gate con audiencias vacías; las secciones siguientes preservan cronológicamente los fallos y correcciones intermedios y no describen el valor final transformado en producción.

## QA/E2E WebForms

La corrida inicial de regresión falló 26 casos porque `npm --prefix` ubicó el proceso en `tools/e2e`; el lanzador se corrigió para usar la raíz y la repetición fue totalmente satisfactoria.

E2E real reutilizada: DOC-80 lectura sin cambios; DOC-81 importación múltiple 2/2 sin asignación; DOC-82 UI con nueve anexos y siete controles invariantes. Las referencias exactas están en la documentación canónica DOC-83.

Acceso negativo real: PASS, sin sesión, cero elementos y código opaco `FEATURE_DISABLED`. La asignación explícita confirmó ambos caminos con validación autoritativa: `BLOCKED` dejó `workflow-assignment-state` intacto cuando faltaba `Recibo De Caja`, y `ASSIGNED` produjo la transición después de una importación real y autorizada del tipo 186. Importación y asignación conservaron autorizaciones y reservas E2E independientes; la misma tarea descartable se reutilizó por autorización expresa. El artefacto canónico conserva el último resultado (`ASSIGNED`) y la salida saneada de consola conserva el `BLOCKED` previo. El gate quedó restaurado. No se guardaron secretos, textos de alertas ni contenido documental.

Después de corregir la política de tipología opcional se ejecutó `import-sii-enlase-ui` sobre la tarea 220588 y el trámite 244: PASS, siete controles sin cambios, preparación individual/preview/foco/responsive confirmados y ninguna mutación. El perfil usó `sampleSize=1` porque el proveedor devolvió un solo anexo; la cobertura múltiple permanece en la evidencia real DOC-81/82.

Una validación manual controlada mantuvo el error después de la primera corrección. `fix2` inhibe el listener legado cuando el trigger está enlazado a la interfaz moderna, evita que la E2E inyecte `CodigoBarras` y renueva las versiones de caché.

El video manual posterior corresponde a la tarea 220589, ya asignada por la E2E positiva anterior. También reveló una prioridad incorrecta: la heurística elegía `Constancia De Inscripción` antes del único tipo obligatorio `Recibo De Caja` (186). `fix3` prioriza `Required` y muestra el código funcional seguro del rechazo de preflight.

El refinamiento `fix4` formaliza devoluciones y correcciones: la tarea ENLASE usa exclusivamente su estado activo actual; un antecedente sin recurso físico vuelve a `Disponible`; y un antecedente físicamente confirmado ofrece `Reimportar`. Esta acción transporta `ReimportRequested=true`, queda vinculada a la huella de preflight y agrega el requisito `DOCUMENT_REIMPORT_EXPLICIT`. Dos `ExternalKey` distintas con la misma tipología siguen siendo dos documentos distintos. La reimportación es aditiva; no elimina ni reemplaza físicamente el documento anterior.

Estado de cierre: E2E REAL `fix2` PASS; REGRESIÓN `fix4` PASS 155/155; MSBuild PASS; APROBACIÓN MANUAL DE REIMPORTACIÓN PENDIENTE. La aceptación debe ejecutarse sobre una tarea ENLASE operable, devuelta/reabierta o con un antecedente descartable; la tarea consumida 220589 no demuestra el flujo corregido. La evidencia `manual_qa=fail` no debe reemplazarse todavía.

## Evidencia fix5

La observacion manual de la tarea 220586 confirmo que el documento persistia pero solo aparecia tras recargar. La correccion proyecta cada resultado reconciliado con el inserter JavaScript existente y deduplica por `documentId`; ENLASE no usa el postback del grid convencional. Las pruebas focales pasaron 9/9 y la bateria DOC-83 ampliada paso 164/164.

## Evidencia fix10

La prueba manual detectó que una importación ENLASE sin tipología persistía, pero no se proyectaba. La rama legacy deja `DBT` y `tipodocumental` vacíos y sí retorna `extension`; el adaptador ahora usa `DBT` cuando existe y esa extensión física cuando falta, sin segunda consulta ni cambios en `ClassAlmacenamiento.vb`. Verificación: focales `49/49` PASS, regresión DOC-83 `177/177` PASS y MSBuild Visual Studio 18 PASS con 0 errores (310 advertencias preexistentes). Gate final `false`; usuarios/grupos vacíos. La aceptación manual final continúa pendiente.
## Evidencia fix11

Se añadió el escenario gobernado `import-sii-enlase-manual-visual` para sustituir la activación manual aislada del gate. Exige autorizaciones mutadoras, tarea descartable, navegador visible, siete controles, dos confirmaciones con plazo compartido de diez minutos y restauración en `finally`. La corrida real y el resultado `manual_qa` siguen pendientes; no se declara PASS antes de ejecutarla.

La primera ejecución del escenario falló correctamente con `IMPORT_E2E_MANUAL_VISUAL_ROW_UNAVAILABLE`: no cambió ningún control y la fila confirmada ya estaba presente en la línea base. Un reajuste intermedio retiró las filas del DOM, pero la segunda corrida demostró que la lista SII no había cargado y terminó con `IMPORT_E2E_MANUAL_VISUAL_PROJECTION_NOT_OBSERVED`, también sin cambios en controles. El runner definitivo no altera la lista existente: espera estabilidad WebForms, abre y valida la lista SII antes del hito manual, y luego contabiliza llamadas efectivas a `insert_row_documento_relacionado(..., rad, ...)` sin postback. La nueva corrida autorizada sigue pendiente.

## Evidencia fix13

La corrida siguiente alcanzó una lista SII válida (`initialRows=3`, `siiActions=8`) y generó cambios en intención, ítem y auditoría de transición de importación, pero no proyectó una fila nueva. El operador respondió `NO`; el resultado `IMPORT_E2E_MANUAL_VISUAL_REJECTED` se conserva y el gate quedó restaurado. El diagnóstico encontró que la reconciliación devolvía `Completado`, mientras el adaptador de lista solo recolectaba `Disponible`, aunque el cierre ya reconocía ambos como estados confirmados.

La corrección acepta exclusivamente `Disponible` y `Completado` para proyección, conserva la proyección ENLASE tipada y mantiene rechazados estados inciertos. Pruebas focales: `18/18` PASS. Regresión DOC-83: `181/181` PASS. La aceptación visual continúa pendiente de una nueva corrida real autorizada.

## Evidencia fix14

La siguiente corrida observó una fila, pero fue rechazada con `IMPORT_E2E_MANUAL_VISUAL_POSTBACK_OBSERVED`. `initialRows=4` representa documentos existentes antes de importar y `siiActions=8` representa acciones disponibles en SII; no son conteos equivalentes. Se identificaron URLs de caché anteriores a los cambios de proyección: UI `doc83fix8` y adaptador de lista `doc83fix9`. Se actualizaron ambas a `doc83fix13` y se reforzó la prueba de registro y orden. Resultado local: focales `20/20` PASS, regresión `181/181` PASS y OpenSpec estricto PASS. El gate final está apagado y la aceptación real continúa pendiente.

## Evidencia fix15

La repetición comenzó con cinco filas, confirmando que la importación anterior solo se reflejó tras recargar. La nueva importación volvió a persistir sin fila inmediata y terminó en `IMPORT_E2E_MANUAL_VISUAL_REJECTED`. Se identificó que `ProjectExecutionResult` descartaba el DTO efímero creado por el almacenamiento al sustituir la respuesta por el snapshot autoritativo. La corrección preserva ese DTO únicamente cuando coinciden capacidad ENLASE, identidades de ítem, documento, tarea y contrato visual completo. Pruebas focales `55/55` PASS, regresión DOC-83 `186/186` PASS, MSBuild PASS con 0 errores y OpenSpec estricto PASS; la E2E real aún no ha aprobado.

La corrida posterior comenzó con seis filas y el operador confirmó una fila nueva inmediata, pero terminó en `IMPORT_E2E_MANUAL_VISUAL_POSTBACK_OBSERVED` antes del segundo hito. El runner se amplió para clasificar de forma saneada si el origen fue refresco documental, interacción anticipada u otro control, y para informar proyecciones/filas antes de fallar. La prueba permanece abierta hasta obtener esa clasificación y completar la apertura en el orden gobernado.

## Evidencia de aceptación visual final

La corrida autorizada definitiva de `import-sii-enlase-manual-visual` terminó PASS. Con línea base de siete filas y ocho acciones SII, observó una fila nueva sin recarga antes de permitir la interacción; el documento recién agregado abrió después sin error. El artefacto saneado registra una proyección y `manualVisual`, `gridProjection` y `rowInteraction` en `CONFIRMED`.

Los controles de intención, ítem y transición cambiaron conforme a la importación; índice, caché, relación y expediente permanecieron invariantes. El recurso quedó consumido y el gate fue restaurado a `false`, con usuarios y grupos vacíos. La evidencia satisface la tarea OpenSpec 3.5; las compuertas OPSXJ de 5.3 y el traspaso de 5.4 continúan pendientes.

## Evidencia de reimportación explícita

La inspección previa de la tarea 220587 observó nueve anexos: siete disponibles, dos importados y cero en revisión, sin mutaciones. Después, una corrida mutadora independiente terminó PASS y el operador confirmó que utilizó específicamente `Reimportar`. Se observó una fila nueva sin recarga y el documento abrió sin error. El artefacto saneado confirmó una proyección, los cambios esperados en intención/ítem/transición, ausencia de efectos sobre expediente/índice/caché/relación, consumo del recurso y restauración del gate. El tipo de botón es evidencia manual declarada por el operador; los efectos y controles proceden del runner.

## Evidencia fix17

La tabla SII contenía solo la columna “Noticia”; otras celdas extensas podían desbordarse visualmente. La corrección incorpora columnas semánticas, anchos explícitos, elipsis, título completo y fondo opaco de acciones en ambas variantes del listado, con invalidación conjunta de caché CSS/JavaScript. Pruebas focales `26/26`, regresión DOC-83 `187/187`, caché/UI `17/17` y OpenSpec estricto PASS.

## Evidencia fix18

La preparación masiva ahora confina el scroll a sus documentos y mantiene visibles `Cancelar`/`Crear intención`; el preflight continúa siendo obligatorio, pero ya no se duplica en la lista “Plan previsto”. El preview sustituye la lista, muestra siempre `Volver a documentos` y alinea el iframe en el alto restante. La regresión ampliada pasó `193/193` y OpenSpec estricto pasó.

La primera revisión real confirmó los tres hitos, pero el runner falló al intentar pulsar por segunda vez el retorno ya ejercido por el operador. Tras hacer el cierre idempotente, la repetición autorizada terminó PASS: nueve filas, 45 celdas, siete truncadas, nueve elementos preparados y preview con iframe visible. El artefacto registra `layoutReview`, `cellContainment`, `internalScroll`, `preparationActions` y `previewNavigation` en `CONFIRMED`, `mutations=NOT_OBSERVED`, siete controles invariantes y gate restaurado a `false` con audiencias vacías. Esta evidencia acredita la aceptación visual de `fix17/fix18` sin producir documentos ni intenciones.
