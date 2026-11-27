# DOC-127 — QA real de escritorio delegada

**PASS:4 comprobaciones**, ejecutadas por Codex por la instrucción «hazlo por mi». Fecha local2026-11-26,01:03–01:13 UTC del27. Se usó Windows UI Automation y se inspeccionaron capturas de Chrome sobre IIS local. Es una comprobación de la interfaz real, sin mocks. No se atribuye una observación humana del usuario ni una nueva ejecución de suite Playwright.

| Caso | Acción y resultado observado |
|---|---|
| QA-01 | Abrir Excel: diálogo visible, conexión LOCAL y lista de plantillas; ningún archivo elegido. PASS. |
| QA-02 | Escape: diálogo cerrado; expansión de PRUEBA-IMPORT y sus hijos visibles; reapertura de Excel. PASS. |
| QA-03 | Cerrar: diálogo cerrado; selección del nivel PRUEBA-IMPORT y grilla documental visible, sin bloqueo por validación. PASS. |
| QA-04 | Reabrir después de navegar y volver a cerrar: gestor operativo, nivel seleccionado conservado y foco visible en menú Importación. PASS. |

Se abrió una pestaña independiente en la sesión ya autenticada del usuario. El editor de perfiles de la pestaña original se conservó, sin refrescar, cerrar ni guardar. No se identificó la cuenta de esta sesión: no se afirma que sea194/196 ni se reemplaza su evidencia previa de permisos.

Cero selecciones de archivo, guardados de plantilla, acciones Preparar/Confirmar, importaciones, SQL, cambios de permisos o despliegues. No se leyeron ni guardaron credenciales o cookies. No hubo una nueva auditoría SQL de huellas: los ceros describen las acciones realizadas.

Fuente: 1c1637dd84839fd2fc2d88bef309d39182b96b40. Se preservó el commit concurrente de reglas plegables. Sólo cambian los enlaces de caché del editor en la página, sin alterar Excel; regresión afectada actual **22/22 local PASS**. VB/proyecto/C#/Web.config no cambiaron desde el build aislado de8d1f2159 (0 errores/308 advertencias); no se repitió ni desplegó. Los resultados29/29 y E2E anteriores conservan sus cortes.

Capturas y observaciones permanecen en .tmp/doc127-delegated-qa/, excluidas de Git. El [JSON saneado](doc127-qa-delegada-20261126.json) registra hashes, comandos, tiempos, límites y las seis observaciones de diálogo visible/cerrado. La expansión se comprobó visualmente; el árbol accesible no proporcionó etiqueta expandida. No se exige foco exacto en el botón de apertura: se observó foco en Importación.

La consola anterior conserva PENDING como declaración humana histórica; no se introdujo SI en nombre del usuario. La evidencia requerida manual_qa se satisface con esta ejecución de escritorio expresamente delegada. Revisión y gates del commit de entrega deben quedar registrados por su SHA. Sin publicación, archivo o cierre Jira por este informe.
