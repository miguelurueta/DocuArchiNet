# Errores y pendientes DOC-105

## Hallazgo de revisión corregido — tarea 13.1

La limpieza aceptaba cualquier PDF antiguo en la carpeta configurada, sin acreditar propiedad. Corregido con nombre de caché reconocido, marca versionada creada solo al publicar, rechazo de rutas redirigidas y conservación ante incertidumbre. También se protege la eliminación de caché inválida frente a archivos sin propiedad. Prueba compilada roja/verde y resultados en 03-PruebasLocales.md; no hubo limpieza de datos reales. El hallazgo focal se considera corregido, no una aprobación automática de todo DOC-105. Estado actual: 87/92 tareas completas, mismas cinco salvedades en DOC-106–109. Las menciones 86/91 describen el estado anterior.

## Seguimientos creados para cierre funcional — 2026-10-18

El usuario seleccionó expresamente la opción 1: cierre funcional con salvedades y creación de seguimientos. Se crearon las siguientes tareas Jira, vinculadas a DOC-105 mediante `Relates`, en estado `Por hacer`. No se modificó el estado de DOC-105 ni se aprobaron validaciones pendientes.

| Origen DOC-105 | Seguimiento | Alcance pendiente |
| --- | --- | --- |
| 7.5 y 7.5.5 | [DOC-106](https://contasoftcompany.atlassian.net/browse/DOC-106) | Consulta del destino ya convertido en Migración; no repetir ni confundir el original ya validado |
| 7.6 | [DOC-107](https://contasoftcompany.atlassian.net/browse/DOC-107) | Guardado único instrumentado, POST=1 y controles SELECT autorizados |
| 8.6 | [DOC-108](https://contasoftcompany.atlassian.net/browse/DOC-108) | Ensayo recuperable de rollback y smoke test en ambiente autorizado |
| 9.6 y limitación de 7.2 | [DOC-109](https://contasoftcompany.atlassian.net/browse/DOC-109) | Redes comparables de Workflow derivado y Correspondencia, sin sustituir PDF ni consumidores |

Esta tabla prevalece sobre las menciones históricas inferiores de ausencia de tickets. Los cinco checkboxes originales permanecen abiertos como evidencia de trabajo no ejecutado en DOC-105; transferir su seguimiento no significa completarlo. Cada ticket incluye alcance, criterios de aceptación, autorización de ambiente/cuentas/datos y restricciones de saneamiento. La autorización para crear tickets no autoriza ejecutar sus pruebas, mutaciones o rollback.

Para tramitar el cierre funcional todavía debe registrarse la revisión técnica y la aceptación de QA del alcance efectivamente probado, citando estos cuatro seguimientos y sus exclusiones. La conformidad con crear seguimientos no se registra como QA global aprobada. OPSXJ no reconoce automáticamente esta transferencia como cumplimiento de tareas; no se altera el validador ni se fuerzan confirmaciones para omitir sus controles. Sin merge, transición Jira ni despliegue por esta actualización.

## Cierre con salvedades autorizado — 2026-10-18

Archivo local de OpenSpec autorizado expresamente por el usuario con 86/91 tareas completas, cinco pendientes y sin sincronizar especificaciones principales. Destino: `openspec/changes/archive/2026-10-18-doc-105-regresion-transversal/`. Este estado prevalece sobre las menciones históricas a «no se archiva»; no cambia resultados de pruebas, no declara cobertura completa ni autoriza producción. No se afirma transición de Jira, publicación, commit ni cierre de las tareas pendientes.

Pendientes conservados: 7.5/7.5.5 (cobertura simple y convertido de Migración), 7.6 (POST único del grafo), 8.6 (ensayo de rollback), 9.6 (comparación remota Workflow pendiente). No se transfirieron a tickets nuevos. Última E2E de Migración convertido: ambas celdas BLOCKED con DOC105_MIGRATION_EXISTING_TARGET_UNAVAILABLE, después de búsqueda y apertura del panel. No prueba que falte el documento. Migración usa handler/visores propios, no express; su pendiente es compatibilidad del handler y no invalida la aceptación focal express. Original de Migración conserva PASSED histórico.

## Consolidación vigente — 2026-10-18

Estado OpenSpec: **86/91 tareas completas; 5 abiertas**. 11.12 y 11.13 completadas con evidencia Workflow del 2026-10-18T12:33:40.576Z PASSED: 34 páginas, tres aperturas (2.266/350/347 ms), mediana 350 ms, snapshot inicial de 390.850 bytes del preview en cada muestra, principal de 9.646.444 bytes, transporte/feedback/scroll/zoom correctos y cero errores clasificados. Pruebas previas: visor 73/73, DOC-105 70/70, DOM 1/1 y MSBuild código 0. Las corridas anteriores permanecen FAILED históricas. Aceptación focal no equivale a cobertura transversal terminada ni acredita redes adicionales. Este resumen prevalece sobre los estados históricos inferiores. No se archiva ni se activa producción.

Revisión retrospectiva completa 12.4 y 12.12; grabación adicional de las 18:56 confirma recarga/reapertura y completa 12.10/12.11. Detalle y límites en 03-PruebasLocales.md. No sustituye POST único de 7.6.

- 12.14 completa por confirmación explícita del usuario de que el grafo es funcional tras corregir inserción visible y controles legibles. No acredita por sí sola geometría/transparencia persistidas.
- 12.15 completa por revisión del video final: aviso y apariencia deshabilitada durante preparación; vista previa visible hacia el segundo 35; al cancelar entre 38 y 39 se retira el aviso y vuelve la apariencia habilitada. Las pruebas locales cubren duplicados; el video no mide solicitudes ni impresión física. La demora sigue presente.
- 12.13 ya completa por diagnóstico E2E PASSED de barra horizontal y borde inferior en Producción.

| Tareas abiertas | Qué falta realmente | Siguiente acción acotada |
| --- | --- | --- |
| 7.5, 7.5.5 | Representación convertida existente desde Migración | Probar solo ese destino; original ya PASSED y Preview externo Gestión NOT_APPLICABLE |
| 7.6 | Guardado único instrumentado, autorización, POST=1 y controles permitidos | Ejecución específica con datos descartables y autorización vigente; videos funcionales no cuentan POST |
| 8.6 | Ensayo de rollback y smoke test | Acordar ambiente y autorización antes de restaurar archivos/configuración |
| 9.6 | Derivado Workflow: faltan muestras comparables a 10/20/50 Mbps | Recuperar evidencia o autorizar solo esas celdas; Producción preview no sustituye este escenario |

Correspondencia conserva aceptación remote-100; 10/20/50 Mbps siguen sin acreditarse, aunque la tarea 7.2 documenta su aceptación acotada. Optimizar la latencia de impresión no fue el alcance de 12.15 y no se declara resuelto. Para cierre global siguen pendientes cobertura, evidencias y rollback; no se convierten en PASSED por conformidad general.

## Incidencias del arnés corregidas

- Se preserva el contexto WebForms autenticado completo para cada par frío/caliente.
- Se exporta y reutiliza `waitForFirstCanvas`; antes la importación indefinida convertía la medición en un fallo diagnóstico incorrecto.
- La evidencia conserva categorías seguras provenientes del runner Workflow y separa el resultado Workflow del bloqueo transversal.
- La sonda del visor conserva únicamente estados booleanos del indicador de carga y valida su orden respecto al primer canvas.
- Los cuatro adaptadores pendientes ya recorren entradas distintas: contexto/documento en Correspondencia, grilla/botón en Producción, iframe contenedor y visor anidado en aprobación, e iframe directo sin PDF.js en handler simple.
- Aprobación espera el postback que reemplaza el iframe antes de resolver su contenido. Los mensajes de consola se clasifican sin guardar texto ni URL; un error de recurso auxiliar queda como observación y las excepciones JavaScript continúan fallando la celda.

Las correcciones iniciales del arnés afectan `tools/e2e`. La regresión reproducible de primera página justificó después cambios productivos localizados en preparación temporal, emisores y visor; el original y el almacenamiento no se reemplazan.

## Estado operativo

- Actualización 7.5.7: Preview externo Gestión queda NOT_APPLICABLE para las dos rutas identificadas del repositorio, que sobrescriben su src antes del render con el visor de clasificación. Prueba focal y suite 67/67; no se eliminó código ni se alteraron permisos. No equivale a E2E PASSED ni excluye accesos externos desconocidos. De los consumidores simples queda pendiente el destino convertido de Migración; el original ya pasó. Permanecen además guardado único, ensayo operativo de rollback y cierre comparativo de 9.6. Las anotaciones anteriores de clasificación pendientes se conservan como historia, no como estado vigente.

- Migración, tarea 7.5.6 completa: repetición automática del recorrido observado, desde menú lateral hasta visor del original, `PASSED` en remote-100 frío/caliente. HTTP 200, STREAM, 48.691 bytes declarados y una solicitud por celda, sin errores clasificados. Categoría `DOC105_MIGRATION_SOURCE_PREVIEW`; guardado de grafo NOT_APPLICABLE, POST de grafo=0. Esta evidencia posterior resuelve el bloqueo de navegación del original registrado abajo. No acredita el destino convertido de 7.5.5 ni el historial de versiones desde Migración; ambas coberturas permanecen separadas.

- Ajuste al recorrido de Migración (7.5.6): el video del usuario acredita acceso por menú lateral, búsqueda por campo, visualización del original e historial de versiones. La corrida anterior buscaba una tarjeta de inicio y terminó `DOC105_MIGRATION_MENU_ACTION_UNAVAILABLE`; no acredita falta de permisos. El runner ahora abre los ancestros del menú lateral, usa el campo primario de búsqueda (no el campo de límite superior ni búsqueda general) y permite `view=source` para medir el visor del original con categoría `DOC105_MIGRATION_SOURCE_PREVIEW`. La conversión existente permanece separada en 7.5.5; tampoco se declara automatizada la consulta del historial desde Migración. Política local 66/66; pendiente repetición autenticada.

- Migración, tarea 7.5.5: primera E2E fría/caliente `FAILED` con `DOC105_OPEN_CONSUMER_FAILED`, HTTP=0 y solicitudes documentales=0. Solo llegó al hito `MODULE_OPEN`; no hay evidencia para atribuir la causa a permisos ni al archivo. Se añade espera de navegación del iframe, se admite un único gabinete válido y se clasifican por separado menú, navegación, lista de gabinetes y formulario. Pendiente repetición; no se declara corregida la causa real sin observarla.

- Regresión Word en Producción (tarea 7.5.2): corregida y verificada manualmente. La rama express no filtraba extensión. Se añadió comparación `.PDF` sin distinguir mayúsculas antes de preparar/emitir referencias PDF; otros formatos conservan `WebFormDaVisorExterno.aspx` y su pantalla de descarga. Pruebas focales 15/15 y MSBuild Debug código 0. Las grabaciones del 2026-10-16 confirman botón «Descargar Archivo», descarga DOCX completada y apertura en Word, sin PDF.js ni el error anterior. Word abrió una copia anterior: una comparación SHA-256 de solo lectura confirmó igualdad con la copia recién descargada, ambas de 91.272 bytes. No se conservaron contenido, nombres ni hashes en esta evidencia. Esto acredita el recorrido manual y equivalencia entre copias locales, no integridad contra el original del servidor ni códigos HTTP. La matriz automatizada de transporte de Descargas Docuarchi permanece pendiente.

- Tarea 7.5.3 completa: política local 57/57 y E2E autorizada `remote-100` fría/caliente `PASSED`. Selección de gabinete, búsqueda exacta y descarga automáticas; HTTP 200, `STREAM`, 91.272 bytes y una solicitud por celda, sin errores clasificados. La descarga terminó y su longitud coincidió con Content-Length. El temporal propio del navegador se elimina al terminar; no se conserva contenido en evidencia. Guardado `NOT_APPLICABLE`, POST de grafo=0. Esta ejecución posterior resuelve el pendiente HTTP de la validación manual anterior.

- Workflow: `PASSED`, ocho de ocho celdas.
- Correspondencia: `remote-100` frío/caliente `PASSED` mediante `Handler_image_wf.ashx`; faltan 10/20/50 Mbps para completar su matriz comparable.
- Producción Documental: la línea base `remote-100` fue `FAILED` con 79,74 %, 223 solicitudes y 48.774/56.662 ms. Tras la corrección, la matriz 10/20/50/100 Mbps fría/caliente terminó ocho de ocho `PASSED`, con 1.479–4.284 ms, 1.629.367 bytes físicos y 5–6 solicitudes antes del canvas.
- Aprobación/externo: 10/20/50/100 Mbps frío/caliente `PASSED` sobre el fallback autenticado; HTTP 200, una solicitud por celda, cero mutaciones y recurso auxiliar fallido registrado como observación saneada.
- Handler simple: `workflowOtherFormats`, `docuarchiExternal`, `docuarchiDownload` y `versions` pasaron sus E2E `remote-100` fría/caliente: cuatro de seis clases simples del inventario actual. Falta Migración y resolver la aplicabilidad de Preview externo Gestión. Las muestras pequeñas acreditan cobertura funcional, no rendimiento de PDF pesado.
- Otros formatos Workflow: `PASSED` en frío y caliente. El runner seleccionó automáticamente tarea y documento antes de abrir `WebFormVisor_Otros_Formatos`; ambas filas observaron HTTP 200, `STREAM`, una solicitud, ausencia de errores clasificados y cero mutaciones. El control de permisos se conservó y no se llamó directamente al handler.
- Guardado de grafo: pendiente de autorización específica y datos descartables; no se ejecutó en esta matriz.
- Rollback: pendiente de ambiente autorizado.
- Versiones, tarea 7.5.4 completa: política local 61/61 y E2E `remote-100` fría/caliente `PASSED`. Plantilla, radicado previamente asignado, documento único e historial seleccionados automáticamente; HTTP 200, `STREAM`, 182.480 bytes declarados y una solicitud por celda, sin errores clasificados. Guardado de grafo `NOT_APPLICABLE`, POST de grafo=0. No se ejecutó asignación, carga, restauración, reemplazo ni eliminación. No se afirma descarga parcial ni rendimiento de PDF pesado.

El resultado global consolidado sigue siendo `BLOCKED` por las dos clases simples aún pendientes de ejecución o clasificación, el guardado único y el ensayo de rollback. En las corridas aisladas de descarga y Versiones, los bloqueos de Workflow, Correspondencia, Producción y Aprobación significan consumidores deshabilitados para esa ejecución; no invalidan sus resultados previos. El contador de consola de tres consumidores bloqueados excluye Workflow, cuyo estado se imprime por separado. La descarga parcial y el estado visible de carga quedaron confirmados para Workflow y Producción; no se extrapolan a los consumidores pendientes ni a los fallbacks observados.

La corrección obligatoria está trazada en D-13/RQ-13 y tareas 11.1–11.11. Producción completó 7.3 y 11.5 con el mismo PDF pesado y navegación automática; la cobertura transversal no se cierra con ese resultado aislado.
