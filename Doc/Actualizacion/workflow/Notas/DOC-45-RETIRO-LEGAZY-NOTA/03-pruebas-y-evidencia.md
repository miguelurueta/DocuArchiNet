# Pruebas y evidencia

| Control | Comando | Resultado |
| --- | --- | --- |
| Política DOC-45 | `npm.cmd --prefix tools/e2e run test:doc45:policy` | PASS 3/3 |
| Regresión Notas/DOC-43/DOC-44 | `node --test tests/notes-workflow-policy.test.cjs tests/doc43-notes-ui-policy.test.cjs tests/doc44-workflow-notes-policy.test.cjs` desde `tools/e2e` | PASS 22/22 |
| Compilación posterior al retiro ampliado | MSBuild `GestionDocumental-Docuarchi.net.sln`, Debug | PASS, 0 errores, 309 advertencias preexistentes |
| Regresión posterior al retiro ampliado | Políticas Notas/DOC-43/DOC-44/DOC-45 | PASS 25/25 |
| E2E real previa a la reapertura | `npm.cmd --prefix tools/e2e run test:doc44:workflow-notes` | Histórica: PASS 1/1; no constituye evidencia final del retiro ampliado |
| E2E real posterior al retiro | El mismo comando oficial reutilizado | PASS 1/1, 20.4 s totales, tarea descartable autorizada 627 |
| E2E final de selección parcial, propiedad y CRUD | `npm.cmd --prefix tools/e2e run test:doc44:workflow-notes` | PASS 1/1, 22.6 s totales; sin `page.reload` |
| E2E final de estado vacío | `npm.cmd --prefix tools/e2e run test:doc45:empty-notes` | PASS 1/1, 17.1 s totales; crea y elimina la nota temporal |

La evidencia final cubrió lectura negativa, tarea explícita, CRUD, panel moderno visible y controles legacy inexistentes, sin modificar `Web.config`. No contiene credenciales, cookies, cadenas de conexión, identificadores de nota ni contenido de notas; el gate quedó `false`, con usuarios y grupos vacíos.

## Incidencia de caché posterior

Una grabación manual mostró que el navegador conservaba `Webworkflow.js?v=20260812-taskclose53` y ejecutaba inicializadores de los modales retirados, produciendo errores por elementos `null`. Se actualizó el recurso a `v=20260902-doc45-notes-retirement1`, se retiró la referencia residual del adaptador visual y se añadieron controles de política. Resultado automatizado: política DOC-45 PASS 3/3, activación DOC-2 PASS 26/26 y MSBuild PASS con cero errores. La validación manual posterior confirmó que los errores dejaron de aparecer.

## Incidencia de descubribilidad posterior

La misma validación manual reveló que la sección moderna tampoco aparecía. La causa fue una contradicción entre D-05 y el code-behind: `Panel_notas_modernas`, su hoja de estilo y su inicializador seguían condicionados por `WorkflowCentroTrabajoModernActive`, aunque el gate debe permanecer en `false` y la presentación oficial está habilitada. Se sustituyó esa condición por `WorkflowCentroTrabajoModernPresentationEnabled`, sin modificar `Web.config`, usuarios ni grupos. Política DOC-45 PASS 3/3, regresión DOC-2 PASS 26/26 y MSBuild PASS con cero errores.

La primera repetición E2E confirmó visualmente el panel, la creación y la actualización, pero agotó el timeout global de 90 segundos antes de cerrar la eliminación y dejó una nota de prueba residual. Se amplió el presupuesto técnico a 180 segundos, se sincronizó explícitamente el diálogo nativo y se evitó que el cierre de un navegador ya terminado ocultara la causa primaria. La repetición final autorizada pasó 1/1 en 15.9 segundos, incluido CRUD completo, lecturas negativas, tarea explícita y ausencia física del consumidor legacy. La evidencia permanece saneada y no conserva contenido ni identificadores de notas.

Una revisión manual posterior aclaró que “visible en DOM” no garantizaba un acceso descubrible dentro de la barra de acciones. Se añadió el control cliente moderno `workflow-notes-modern-access` con contador y `Panel_notas_modernas` se convirtió en un diálogo superpuesto accesible. La E2E exige que el botón sea visible, que el contador sea numérico, que el modal empiece cerrado, abra y reciba foco, complete CRUD, cierre y devuelva el foco al disparador. No usa postback, no invoca endpoints legacy y no altera el gate. La corrida real autorizada finalizó PASS 1/1 en 19.6 segundos.

Una estabilización posterior reemplazó el confirmador nativo de eliminación por un `alertdialog` moderno con cancelar/confirmar, fijó la altura del modal principal al viewport con scroll interno en la lista y convirtió los mensajes exitosos en anuncios transitorios de 3.5 segundos. La E2E comprueba cancelación sin mutación, confirmación única, limpieza del mensaje y estabilidad dimensional. Políticas DOC-44/DOC-45 y MSBuild pasan; la repetición E2E real autorizada finalizó PASS 1/1 en 22.1 segundos.

## Reapertura D-07/RQ-07 — propiedad y lectura ampliada

Se incorporó la capacidad `PuedeGestionar` calculada por el backend, el código funcional `NotOwner`, el ocultamiento de edición/eliminación ajena y un diálogo accesible de lectura completa para contenido extenso. Validación local posterior: políticas DOC-43/44/45 PASS 18/18; MSBuild Debug PASS con 0 errores y 309 advertencias preexistentes. La evidencia histórica anterior no se reutilizó como cierre; se exigió una nueva corrida autorizada sobre la implementación final.

La corrida E2E definitiva posterior a D-07/RQ-07 finalizó PASS 1/1 en 23.1 segundos (22.4 segundos de prueba). Usó una tarea descartable expresamente autorizada y una nota ajena de la misma tarea; la suite seleccionó la tarea mediante la UI oficial, verificó lectura negativa, ausencia de acciones mutantes ajenas, rechazo `NotOwner`, versión ajena intacta, visor accesible sobre una nota propia extensa y CRUD propio completo. La evidencia no conserva credenciales, contenido de notas, cookies ni cuerpos HTTP. El gate no fue modificado.

La revisión manual en video del estado vacío confirmó inicialmente la corrección D-08/RQ-08: el acceso visible muestra `Nueva nota 0`, el primer clic abre directamente el editor enfocado, el guardado cambia el acceso a `Notas 1` y este vuelve a abrir el listado en el mismo modal. La grabación se utilizó únicamente para inspección visual y no se incorporó al repositorio porque contiene información operativa de pantalla. El cierre automatizado posterior se registra al final de este documento.

Una segunda revisión en video detectó la regresión D-09/RQ-09: las acciones dinámicas de `GridView2` conservaban su círculo, pero quedaban con fondo blanco y glifo blanco invisible; el disparador lateral del índice sufría la misma pérdida de contraste. Se añadieron reglas locales para las variantes primaria, informativa y de advertencia, así como para mostrar/ocultar índice, sin modificar eventos ni permisos. Las hojas afectadas recibieron una nueva versión de caché. La comprobación visual posterior confirmó la corrección y la E2E final comprobó que Notas siguiera operativa.

Validación focal posterior a la corrección: `npm.cmd --prefix tools/e2e run test:doc45:policy` finalizó PASS 4/4; `openspec.cmd validate doc-45-retiro-legazy-nota --strict` finalizó correctamente y `git diff --check` no reportó errores. Estos controles prueban alcance y cascada declarada, pero la tarea 8.2 continúa abierta hasta comprobar el resultado renderizado después de publicar y recargar las nuevas hojas.

La comprobación visual posterior confirmó los colores, pero reveló D-10/RQ-10: al seleccionar otra tarea, el `UpdatePanel` reemplazaba el botón de Notas y el listener ligado al nodo anterior desaparecía. El cliente ahora delega el clic, relee el control de tarea explícita y sincroniza la lectura en `PageRequestManager.endRequest`. La E2E oficial dejó de usar `page.reload` tras la selección para que el defecto no pueda quedar oculto. La corrida final PASS descrita abajo cerró la tarea 8.4.

Validación local posterior a D-10/RQ-10: sintaxis JavaScript PASS; política DOC-44 PASS 7/7; política DOC-45 PASS 4/4; OpenSpec estricto PASS y `git diff --check` sin errores. En ese punto la evidencia de ambiente seguía pendiente porque estos controles no sustituían la interacción real después del postback parcial; la corrida final siguiente cerró esa brecha.

La repetición real autorizada posterior a D-09/D-10 finalizó PASS 1/1 en 22.6 segundos (22.0 segundos de prueba) sobre la tarea descartable autorizada. Sin recarga completa, la suite seleccionó la tarea mediante la UI, confirmó el acceso moderno recién renderizado y completó lecturas negativas, protección de propiedad, visor y CRUD propio. Esta corrida cierra 8.2 y 8.4; no cierra 7.2 porque la tarea utilizada no partía de un estado realmente vacío. La salida compartida no expuso credenciales, cookies, cuerpos HTTP ni contenido persistido.

La primera corrida específica de estado vacío sobre la tarea autorizada confirmó `Nueva nota 0` y la apertura del modal principal, pero el editor permaneció oculto. La causa fue una carrera: el acceso estaba disponible mientras `ContarNotas` aún no había consolidado `totalNotesLoaded`. El cliente ahora conserva la promesa de carga y, ante un clic temprano, espera su finalización antes de decidir entre editor y listado. La prueba final de 7.2 debe repetirse sobre esta corrección.

La repetición final autorizada del modo `test:doc45:empty-notes` terminó PASS 1/1 en 17.1 segundos (16.4 segundos de prueba) sobre una tarea descartable inicialmente vacía. Confirmó `Nueva nota 0`, apertura directa del editor tras completar la lectura autorizada, creación única, transición a `Notas 1`, eliminación mediante el diálogo moderno y restauración de `Nueva nota 0`. La salida quedó saneada y la prueba eliminó la nota temporal, por lo que no dejó contenido de validación persistido. Con este resultado se cierra D-08/RQ-08 y la tarea 7.2.

Una inspección posterior detectó que las tareas todavía no asignadas usan `btn-success`, variante que no estaba incluida en la primera corrección cromática D-09. Por ello la acción `Asignar y gestionar` aparecía como círculo blanco aunque conservaba su evento. Se añadió una regla local verde con glifo blanco, se versionó nuevamente `workflow-tareas-modernas.css` y la política DOC-45 pasó a exigir los cuatro estados `primary`, `info`, `warning` y `success`. La corrección no modifica estado, permisos ni lógica de asignación; queda pendiente su comprobación visual después de publicar la nueva hoja.

La comprobación manual posterior confirmó fondos verde, amarillo, azul y turquesa con glifos visibles. La E2E específica no mutante `test:doc45:unassigned-color` finalizó PASS 1/1 en 15.7 segundos (15.0 segundos de prueba) con una cuenta autorizada que tenía tareas no asignadas visibles. La prueba verificó mediante estilos computados el fondo verde y el glifo blanco, sin seleccionar, asignar ni modificar ninguna tarea. Con este resultado se cierra la tarea 8.6.

Después de separar el dato cromático de la regresión CRUD, se repitió el ejecutor definitivo `test:doc44:workflow-notes`. La corrida autorizada finalizó PASS 1/1 en 19.6 segundos (18.9 segundos de prueba) sobre la tarea descartable aprobada. Confirmó que una tarea ya seleccionada continúa directamente, sin abrir innecesariamente la búsqueda, y completó la regresión de propiedad, lecturas negativas, visor y CRUD. Este resultado reemplaza como evidencia final del ejecutor a las corridas anteriores a su estabilización.
