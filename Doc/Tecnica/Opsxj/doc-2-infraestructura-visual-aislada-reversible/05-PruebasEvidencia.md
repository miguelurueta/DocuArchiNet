# DOC-2 — Pruebas y evidencia

## Evidencia local — 2026-08-10

- `npm.cmd --prefix tools/opsxj test`: **PASS**, 12 archivos y 74 pruebas. Incluye `doc2WorkflowActivation.test.js`, que verifica la preservación del baseline manual, flag apagado, piloto de servidor, orden de recursos, scope CSS y adaptador sin mutaciones prohibidas.
- `MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m`: **PASS**. Persisten advertencias de dependencias y variables sin inicializar preexistentes; no hay errores DOC-2.
- `openspec.cmd validate doc-2-infraestructura-visual-aislada-reversible --strict`: **PASS**.
- `manual_qa`: pendiente. El 2026-08-10 se recibió la URL de inicio de Gestión; el acceso automatizado alcanza `localhost:443`, pero el handshake TLS exige credenciales/certificado de cliente que el runner no posee. Siguen pendientes una cuenta piloto, una cuenta fuera del piloto y datos Workflow controlados.

## Intento de acceso al ambiente — 2026-08-10

- URL informada: `https://localhost/GestionDocumental-Docuarchi.net/Defaul/WebFormInicioDocuarchiGestion.aspx`.
- Resultado: no se recibió respuesta HTTP. La negociación TLS falló antes de autenticar contra la aplicación con `SEC_E_NO_CREDENTIALS`; no se declara una prueba funcional ni visual como aprobada.
- Alcance del intento: comprobación de disponibilidad sin credenciales ni modificación de `Web.config`, IIS, banderas o perfiles piloto.
- Para reanudar: habilitar el certificado o mecanismo de acceso del ambiente para la sesión de QA y suministrar cuentas piloto/no piloto con datos Workflow controlados.

## Activación local controlada — 2026-08-10

- Configuración efectiva solicitada: `WorkflowCentroTrabajoModernEnabled=true`, piloto cerrado `yadira.duque-abo` y capas `layout,actions,documents,a11y`.
- El valor seguro de código permanece en `false` cuando la clave no está configurada; la activación actual es una decisión de ambiente y no elimina la validación de piloto.
- Esta configuración no sustituye la QA manual ni autoriza la promoción a un ambiente compartido.

## Hallazgo visual de piloto — 2026-08-10

El piloto confirma que la activación funciona, pero la primera capa no alcanza aún la fidelidad del HTML modelo: faltan shell de trabajo, tratamiento completo de barra/menús, densidad de documentos, superficie de visor e índice. Se reabren las tareas 3.5 a 3.8 y la verificación visual 6.2; no se declara paridad visual aprobada hasta completar esos elementos sobre los contenedores WebForms existentes.

## Refuerzo estructural respecto al HTML patrón — 2026-08-10

- El contexto de tarea reutiliza `#content_pie_seleccion_tarea` y sus dos labels funcionales; `ctw-task-context` es su estructura visual en el mismo `UpdatePanel`, sin acciones, postbacks ni datos de negocio duplicados.
- La capa de piloto reposiciona visualmente esa única franja sobre documentos, visor e índice mediante grid. Dos labels decorativos de servidor separan título y estado sin reemplazar HTML ASP.NET ni mover nodos.
- La prueba estática verifica que el contexto estructurado no crea una consulta paralela ni interpreta en cliente la cadena legacy concatenada.
- La evidencia local histórica de esta sección se conserva como referencia; las pruebas del refinamiento vigente se registran al final de este documento.

## Reordenamiento de menús y botones — 2026-08-10

- `ctw-layer-actions` ahora aplica sobre `#menucab` y `#nav_menu`. Las dos barras conservan sus nodos y `UpdatePanel`, pero se presentan como franjas continuas: navegación, operaciones y acciones terminales.
- La clasificación se aplica desde controles hijos estables y clases `ctw-action-slot-*`, no desde el ClientID de un `Panel`; así se mantiene tras los `UpdatePanel`. Los iconos existentes se hacen visibles y alineados en el piloto. El control real `#pendiente_selec_tarea` se presenta como cierre de peligro solo cuando la lógica legacy lo muestra; no modifica su handler, foco, permiso, etiqueta ni visibilidad. Los envíos visibles reciben énfasis primario.
- La prueba estática cubre ambas barras, los paneles reales de pendientes, notas, autorización, devolución y envío, y rechaza creación de markup o invocación artificial de clics.
- Resultado local del refuerzo: `npm.cmd --prefix tools/opsxj test` 12/12 archivos y 74/74 pruebas; `node --check js/workflow/centro-trabajo-visual.js` y compilación Debug aprobados. Persisten solo advertencias históricas de conflictos de ensamblados en MSBuild.

## Corrección quirúrgica frente al DOM del piloto — 2026-08-10

- El DOM confirmó que `#pendiente_selec_tarea` es el control real que la lógica legacy etiqueta como `Cerrar tarea` cuando corresponde. La corrección posterior restauró también para piloto su handler `E-ETP`, icono y etiqueta operativa; `Panel_tareas_estado_pendiente` sigue sujeto a la visibilidad que determine legacy.
- El helper baseline de cabecera de documentos conserva los accesos rápidos existentes de carga e índice en `#div_label` solo con raíz piloto y `ctw-layer-documents`; fuera de esa combinación conserva exactamente la reubicación baseline. No cambia IDs ni manejadores y las opciones restantes permanecen en el dropdown contextual.
- El contexto del piloto se actualiza en servidor con una proyección codificada de los labels de estado existentes. No crea una fuente de datos ni control funcional paralelo.
- Verificación local: `node --check` de ambos scripts, `npm.cmd --prefix tools/opsxj test` (12/12 archivos, 74/74 pruebas) y `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m` (0 errores). Se mantienen advertencias históricas de ensamblados y variables no inicializadas.

## Barra contextual de documento — 2026-08-11

- `#div_label` conserva el título `Documentos` dentro del panel lateral; el visor ocupa su columna y `#Panel_tolbar_pdf` queda inmediatamente sobre el documento abierto.
- La barra del visor usa el contexto de la selección actual: `Versiones`, firma, cambio de tipología, reemplazo y eliminación conservan `tip_event`, `id_wf`, `idd_wf` y el handler `prevent`. `Cargar` usa el handler legacy de adjunto y `Metadatos` reutiliza `#id_indice_wf_pdf` cuando el visor PDF lo entrega.
- Verificación local: `npm.cmd --prefix tools/opsxj test -- doc2WorkflowActivation.test.js` (7/7), `node --check js/workflow/centro-trabajo-visual.js` y compilación Debug (0 errores). Las advertencias de ensamblados y variables no inicializadas son históricas.

## Recorrido QA obligatorio

1. Con `WorkflowCentroTrabajoModernEnabled=false`, confirmar la lista moderna de documentos y la reubicación de iconos baseline, junto con ausencia de clase y recursos DOC-2 y de errores durante carga/postback.
2. Con flag `true` y perfil fuera de lista, comprobar el mismo resultado apagado.
3. Con flag `true` y perfil piloto, comprobar clase raíz, cuatro subcapas y recursos después de `Webworkflow.js`.
4. Retirar individualmente `actions`, `documents` y `a11y`; comprobar que retrocede solo esa presentación.
5. Restaurar `false`, recargar forzado y navegar internamente; comprobar que no quedan mutaciones visuales ni se habilitan acciones ocultas por servidor.

Las capturas de 1366, 1024, 768 y 375 px y la ejecución manual se asociarán al SHA desplegado.

## Refinamiento Documental Workbench — 2026-08-12

- El contexto de tarea se muestra una sola vez antes de documentos, visor e índice; procede de los mismos labels legacy y no crea una consulta ni estado nuevo.
- En el piloto, `CheckBox_auturiza` conserva la autorización o revocación inmediata. Su presentación visible es `Autorizada`; el desplegable separado se denomina `Historial` y conserva exclusivamente la lista de autorizaciones.
- Las dos barras de comandos reducen padding y tipografía en escritorio, sin reparentar controles; en el breakpoint táctil mantienen objetivos de al menos 44 px.
- El contador se normaliza como `Documentos (N)` solo cuando `WorkflowCentroTrabajoModernActive` es verdadero. La ruta baseline mantiene `Documentos N`.
- Verificación local 2026-08-12: `npm.cmd test -- scripts/lib/doc2WorkflowActivation.test.js` aprobó 18/18; `node --check` aprobó el adaptador y la prueba; `MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m` terminó con 0 errores y 277 advertencias históricas; `openspec.cmd validate doc-2-infraestructura-visual-aislada-reversible --type change --strict` y `git diff --check` aprobaron.
- Pendiente: QA visual en los cuatro viewports indicados y los recorridos con cuentas piloto/no piloto de las tareas 6.2 a 6.6.

## Contexto estructurado de trámite — 2026-08-12

- Solo en el piloto DOC-2, la franja existente presenta el trámite como título visual en mayúsculas, el estado como distintivo separado, `Radicado <número> · <solicitante>` y `Flujo · <nombre>` o `Ruta · <nombre>`. La variación se decide en el servidor con los mismos campos ya disponibles para la tarea.
- No se normalizan ni alteran los valores de negocio: las mayúsculas del título son una regla CSS de presentación. Tampoco se ejecutan consultas nuevas, postbacks, ni parsing en cliente de la cadena legacy.
- El baseline conserva su texto técnico previo y los mismos controles funcionales. Los dos labels adicionales del piloto son exclusivamente de lectura y permanecen dentro del `UpdatePanel` original.
- Cada label funcional (`Label_estado_tarea_selecion` y `Label_estado_selecion`) se declara una única vez; las condicionales solo abren o cierran sus contenedores visuales. La prueba estática exige esa unicidad y evita una regresión de ID duplicado en WebForms.
- En escritorio, el contexto usa dos líneas y 46 px mínimos: encabezado de trámite/estado y metadatos/proceso. En el breakpoint angosto vuelve a una columna para no truncar información; el cambio recupera altura útil para el visor en la resolución objetivo.
- Las herramientas `Opciones`, `Detalle`, notas, historial y vuelta a tareas eliminan el borde persistente solo en piloto; obtienen fondo suave y foco visible al interactuar. Devolución y transferencias conservan borde suave, mientras el avance de flujo permanece como única acción sólida. No cambian controles, handlers, permisos ni objetivos táctiles.
- El menú `Detalle` del piloto usa un estado abierto ligero, ancho adaptable de 360–380 px y etiquetas multilínea. Sus siete accesos existentes se muestran en los grupos Información, Trazabilidad y Documentos, que no pueden quedar vacíos porque las entradas son estáticas. Solo en piloto se diferencian sus iconos; los `onclick`, IDs y rutas legacy permanecen idénticos.
- Verificación local 2026-08-12: `npm.cmd test -- scripts/lib/doc2WorkflowActivation.test.js` aprobó 21/21; `node --check` aprobó el adaptador y la prueba; `MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m` terminó con 0 errores y una advertencia histórica de referencias. La validación estricta OpenSpec y `git diff --check` aprobaron; los avisos de conversión LF/CRLF pertenecen a archivos ya modificados del árbol y no son errores de diff.

## Reintento de QA de ambiente — 2026-08-12

- Se confirmó que la configuración local contiene `WorkflowCentroTrabajoModernEnabled=true`, los perfiles piloto cerrados `yadira.duque-abo,melbaa` y las capas `layout,actions,documents,a11y`. Esto solo demuestra la preparación de la configuración; no autentica ninguna cuenta ni sustituye la evidencia de piloto.
- IIS registra la aplicación en `Default Web Site` bajo `/GestionDocumental-Docuarchi.net`, con pool iniciado y binding HTTPS `*:443`; `sslFlags=0`, por lo que el sitio no exige certificado de cliente. La misma ruta respondió `HTTP 200` sin autenticación.
- Desde el proceso estándar del runner, la solicitud HTTPS falla antes de HTTP con `SEC_E_NO_CREDENTIALS`. La comprobación equivalente en contexto elevado respondió `HTTPS 200` con verificación TLS correcta. El impedimento es, por tanto, la disponibilidad de credenciales TLS del proceso no elevado, no la publicación de la aplicación ni una configuración de certificado de cliente en IIS.
- No se usó un bypass de certificado, ni se modificaron IIS, configuración, banderas, perfiles, datos ni controles de Workflow. La disponibilidad HTTPS no equivale a una validación funcional: faltan una cuenta piloto, una no piloto y una tarea documental controlada. Por tanto, las tareas manuales 6.2 a 6.6 permanecen pendientes y no se declara paridad ni promoción aprobada.
- Para continuar la aceptación se deben proporcionar las dos cuentas y el caso Workflow controlado; con ellas se ejecutará el recorrido documentado, los rollback por capa y las capturas asociadas al SHA desplegado.

## Cuentas asignadas para QA — 2026-08-12

- Cuenta piloto designada: `melbaa`. La configuración local la incluye en `WorkflowCentroTrabajoModernPilotProfiles`; la comparación del servidor es exacta e insensible a mayúsculas/minúsculas, por lo que el valor de sesión `MELBAA` activa el piloto solo si el flag maestro está habilitado.
- Cuenta no piloto designada: `carolina.cruz`. No figura en la lista cerrada de perfiles piloto, de modo que debe conservar la línea base aun con el flag maestro activo.
- El ingreso autorizado se realiza en `gestor.aspx` mediante módulo, usuario y contraseña. La contraseña no se proporcionó ni se almacenó; no se intentaron autenticaciones incompletas ni se alteró una sesión para simular cualquiera de las cuentas.
- Sigue pendiente asociar ambas sesiones a una tarea con documentos controlados para ejecutar 6.2–6.6 y capturar la evidencia por SHA.

## Intento de autenticación QA — 2026-08-12

- Se efectuó un único postback aislado por cada cuenta asignada en el módulo `GESTOR`, cuyo valor se resuelve en servidor como `GESTOR DOCUMENTAL`. Las credenciales nunca se escribieron en archivos, evidencia ni salida de comandos.
- Tanto `melbaa` como `carolina.cruz` permanecieron en `gestor.aspx` y el servidor devolvió el mensaje funcional: `La contraseña no es válida para el módulo GESTOR DOCUMENTAL`. Cada sesión solo obtuvo la cookie de sesión ASP.NET; no se emitió la cookie de Forms Authentication ni se abrió Workflow.
- No se realizaron más intentos, no se modificaron cuentas, contraseñas, IIS, configuración, banderas o datos Workflow y no hubo efectos de negocio. Hasta que se corrijan o habiliten las credenciales de QA, no se puede demostrar en ambiente la activación del piloto, el baseline no piloto, los rollback ni las capturas; las tareas 6.2 a 6.6 continúan pendientes.

## Evidencia visual de piloto — 2026-08-12

- Captura aportada por el operador: `prueba.png`, viewport de escritorio de 1914 × 938 px. La cabecera de la sesión identifica `MELBAA (Asistente Contable)` y `Login: MELBAA`; es evidencia manual de que la cuenta piloto sí alcanzó el centro de trabajo, aun cuando el cliente HTTP aislado no reprodujo su autenticación.
- Se confirma en esta vista: contexto estructurado de tarea (`FACTURA CONTRATACION SERVICIO`, estado `CERRADO`, radicado, solicitante y flujo); jerarquía de acciones ghost, secundaria y primaria; `Documentos (5)`; selección inequívoca de `Rótulo Radicado`; título y distintivo PDF del visor; y el documento abierto en el visor. No se observaron controles de negocio duplicados.
- Hallazgo UX: para un único PDF, la página ocupa una proporción demasiado pequeña del lienzo del visor en escritorio, dejando una superficie gris excesiva y reduciendo la legibilidad. Debe evaluarse un ajuste inicial de `page-width` o equivalente dentro del visor PDF, preservando sus controles y sin alterar la ruta de apertura legacy.
- La captura no muestra el índice abierto, hover/foco/deshabilitado, el menú `Detalle` desplegado, postback parcial, caché ni el baseline de `carolina.cruz`. Por ello, es evidencia parcial: no cierra 6.2 ni las tareas 6.3 a 6.6.

## Evidencia visual fuera del piloto — 2026-08-12

- Captura aportada por el operador: `prueba2.png`, viewport de escritorio de 1914 × 938 px. La sesión identifica `CAROLINA.CRUZ`; la cuenta no pertenece a la lista cerrada de perfiles piloto mientras el flag maestro local está habilitado.
- La pantalla conserva visualmente el baseline esperado: `Opciones`, `Detalle`, `Servicios` y `Documentos` aparecen como menús legacy; las operaciones muestran `Cerrar tarea`; el conteo mantiene `Documentos 5`; y el contexto técnico permanece en la franja inferior. No se entrega en esta vista el cromo estructurado ni la jerarquía DOC-2 mostrada a `MELBAA`.
- La comparación de ambas capturas confirma la segmentación visual piloto/no piloto sin afectar la apertura del PDF: cada sesión abre su documento seleccionado en el visor. La evidencia no inspecciona el DOM ni la red, por lo que no prueba por sí sola la ausencia de recursos DOC-2 ni que una clase añadida en cliente no pueda conceder acceso; esas garantías siguen cubiertas por las pruebas estáticas y requieren la comprobación manual restante.
- Continúan pendientes la prueba con flag maestro apagado, los rollback individuales, postback/recarga/navegación, estados interactivos y los viewports de aceptación. No se marca 6.3 como completada todavía.

## Rollback maestro y matriz de activación — 2026-08-12

- Se modificó temporalmente solo `WorkflowCentroTrabajoModernEnabled` de `true` a `false`; perfiles piloto y capas permanecieron sin cambios. La aplicación recicló y respondió `HTTP 200` antes de la comprobación manual.
- Captura aportada por el operador: `prueba3.png`, viewport de escritorio de 1914 × 938 px. Con sesión `MELBAA`, antes piloto, se observa el mismo baseline: menús `Servicios` y `Documentos`, conteo `Documentos 5`, `Cerrar tarea` y contexto técnico inferior; no aparece el cromo DOC-2 ni el contexto estructurado.
- Matriz confirmada en ambiente: flag `true` + `MELBAA` entrega DOC-2 (`prueba.png`); flag `true` + `CAROLINA.CRUZ` conserva baseline (`prueba2.png`); flag `false` + `MELBAA` conserva baseline (`prueba3.png`). La prueba estática `doc2WorkflowActivation.test.js` ya cubre que añadir clases en el cliente no puede entregar recursos ni autorización del piloto.
- Resultado: se acepta la tarea 6.3. El rollback maestro está evidenciado, pero 6.4 sigue pendiente hasta validar las capas `actions`, `documents` y `a11y` por separado. El flag maestro será restaurado a `true` para continuar esas pruebas.

## Rollback parcial de `actions` — 2026-08-12

- Configuración aplicada: flag maestro `true`; capas `layout,documents,a11y`. Solo se retiró `actions`; no se modificaron perfiles, handlers, permisos ni datos.
- Captura aportada por el operador: `prueba4.png`, viewport de escritorio de 1914 × 938 px, con sesión piloto `MELBAA` y documento abierto. Se preservan el contexto estructurado, `Documentos (5)`, la fila seleccionada y el visor PDF, demostrando que `layout` y `documents` siguen activos.
- Frente a la captura con todas las capas, `Continuar flujo` deja de ser sólido y la acción terminal vuelve al tratamiento secundario; las barras pierden la jerarquía propia de `actions`. La apertura del documento y sus controles permanecen disponibles, sin postback ni cambio de datos.
- Se acepta el rollback individual de `actions`. A continuación se restaura esa capa y se retira solo `documents`.

## Rollback parcial de `documents` — 2026-08-12

- Configuración aplicada: flag maestro `true`; capas `layout,actions,a11y`. Solo se retiró `documents`; no se modificaron perfiles, handlers, permisos ni datos.
- Captura aportada por el operador: `prueba5.png`, viewport de escritorio de 1914 × 938 px, sesión `MELBAA` y PDF abierto. Se conservan el contexto estructurado, las barras con `Continuar flujo` sólido y la acción terminal neutral, por lo que `layout` y `actions` siguen activos.
- Frente a la captura de todas las capas, las filas documentales dejan el tratamiento de tarjetas/redondeado y la cabecera contextual del visor devuelve `Metadatos` al flujo baseline junto al título. El documento continúa abriendo en el visor y la selección sigue disponible; no hubo postback ni modificación de datos.
- Se acepta el rollback individual de `documents`. La capa `a11y` continúa habilitada y se debe capturar primero su foco visible antes de retirarla.

## Refinamiento de cabecera documental y checks — 2026-08-12

- Se atendió la observación de QA posterior a `prueba5.png`: bajo la capa `documents`, `#div_label.ctw-document-bar` conserva explícitamente fondo blanco, como superficie de trabajo del panel documental.
- La fila seleccionada ya no usa el `border-left` legacy de 3 px en su primera celda. La señal azul pasa a ser un `box-shadow` interno; por tanto, no cambia el ancho de la celda ni desplaza el check respecto al check maestro de la cabecera.
- El alcance permanece aislado a `.workflow-centro-trabajo-moderno.ctw-layer-documents`; no cambia markup, datos, handlers, postbacks ni el baseline. Se restauró la configuración completa `layout,actions,documents,a11y` para el piloto.
- Verificación automática posterior: `npm.cmd test -- scripts/lib/doc2WorkflowActivation.test.js` aprobó 22/22; la validación estricta OpenSpec y `git diff --check` aprobaron. Falta la captura manual del piloto con el CSS `20260812-documentwhite35` para cerrar la comprobación visual de este refinamiento dentro de 6.2/6.6.

## Sin parpadeo de la selección documental — 2026-08-12

- Hallazgo de QA: al seleccionar una fila se alcanzaba a percibir el borde izquierdo azul con radio heredado. La causa era que la señal visual dependía de la celda durante el recálculo de estilos legacy.
- Corrección: la primera celda seleccionada neutraliza `border-left`, radios y sombra. Una pseudo-barra `::before` rectangular de 3 px, sin interacción, dibuja el marcador azul sin variar la geometría ni mostrar el cromo temporal.
- El cambio está aislado a la subcapa `documents`, preserva el fondo blanco de `Documentos (N)` y no modifica el activador legacy, handlers, postbacks ni datos. La hoja usa la versión `20260812-documentselection36`; prueba DOC-2 22/22, validación OpenSpec estricta y `git diff --check` aprobados. Pendiente: comprobación visual manual en el piloto.

## Fondo blanco forzado en cabecera Documentos — 2026-08-12

- La revisión del CSS efectivo identificó que `#div_label.ctw-document-bar` continuaba heredando `--ctw-pale`. Se corrigió dentro de `ctw-layer-documents` con `background: #fff !important`, dejando el azul suave para barras documentales distintas de esa cabecera.
- Se reforzó la prueba estática para verificar el contenido del bloque exacto de `#div_label`, sin aceptar coincidencias posteriores del archivo. La hoja queda versionada como `20260812-documentwhiteforce37`; prueba DOC-2 22/22, validación OpenSpec estricta y `git diff --check` aprobados. Pendiente: confirmación visual manual del piloto.

## Hover puntual de la barra del visor — 2026-08-12

- Hallazgo de QA: el hover de una acción contextual del visor podía ocupar visualmente toda la franja superior. Se fijó el grupo de acciones y cada enlace visible a ancho de contenido, mientras título, formato y espacio flexible quedan sin interacción.
- No se modificó el enlace, su id, handler ni su disponibilidad de servidor. La hoja queda versionada como `20260812-viewerhover38`; prueba DOC-2 23/23, validación OpenSpec estricta y `git diff --check` aprobados. Pendiente: confirmación visual manual del piloto.

## Anclaje de acción en barra del visor — 2026-08-12

- Ante la persistencia del hover extendido, se eliminó la dependencia de la acción respecto al espacio flexible: el enlace de acción se ancla a la derecha de la barra contextual, mientras título, formato y franja neutralizan hover explícitamente.
- En ancho reducido la acción vuelve al flujo estático para no superponerse. No se alteran id, handler, disponibilidad, postback ni el visor. La hoja queda versionada como `20260812-viewerhoverfix39`; prueba DOC-2 23/23, validación OpenSpec estricta y `git diff --check` aprobados. Pendiente: confirmación visual manual del piloto.

## Restauración de cierre de tarea en piloto — 2026-08-12

- Hallazgo funcional de QA: la variante piloto de `#pendiente_selec_tarea` llamaba a `hide_area_workflow_seleccion()` y dejaba la tarea activa, a diferencia de la línea base.
- Se restauró el markup baseline para todos los modos: icono `fa-check-circle`, etiqueta `Cerrar tarea` y handler `inicializa_tipo_adjunto_documento(event,this,'E-ETP')`. `inicializa_estado_pendiente()` vuelve a resolver la etiqueta `Cerrar tarea` o `Enviar a pendientes` de la lógica legacy; no se alteraron permisos, modal, postback ni transición de negocio.
- El adaptador ya no clasifica ese control como cierre local ghost; conserva solo la clasificación terminal de presentación. Las versiones de caché quedan en `Webworkflow.js` `20260812-taskclose53`, CSS `20260812-taskclose40` y adaptador `20260812-taskclose12`.
- La prueba estática cubre la ausencia del retorno local y exige el handler `E-ETP`: `npm.cmd --prefix tools/opsxj test -- scripts/lib/doc2WorkflowActivation.test.js` aprobó 23/23 y la suite completa aprobó 12/12 archivos, 91/91 pruebas. `node --check` aprobó para ambos scripts, la validación OpenSpec estricta aprobó y la compilación Debug terminó con 0 errores (1 advertencia histórica de ensamblados).
- Falta la comprobación manual controlada de la transición real por el piloto, porque cerrar una tarea modifica su estado de negocio.

## Recorrido de cierre y permanencia del radicado — 2026-08-12

- Evidencia aportada por el piloto: `prueba6.png` muestra el radicado `2500496700021` abierto con la acción operativa `Cerrar tarea`; `prueba7.png` muestra el retorno a la lista después de accionarla; `prueba8.png` busca el mismo radicado y obtiene un único resultado.
- El resultado es el esperado para Workflow: cerrar una actividad no elimina el radicado de la bandeja ni cierra el trámite. El radicado permanece trazable y el estado visible `En proceso` es compatible con que el trámite conserve o avance en su flujo.
- Esta evidencia confirma que el piloto ya no queda dentro de la vista documental tras la acción y que no se pierde el radicado. Aún falta comprobar una actualización parcial, recarga forzada y navegación interna antes de aceptar 6.5.

## Video de interacción documental — 2026-08-12

- Se recibió `Grabación 2026-08-12 194244.mp4` (61 s). El fotograma de referencia extraído por el proveedor nativo de miniaturas muestra el radicado abierto, la fila `Factura` seleccionada, el PDF cargado en el visor y el menú `Detalle` desplegado.
- Esto aporta evidencia de apertura documental y del menú agrupado sin sustituir las rutas legacy. El proveedor local no expone fotogramas temporales reproducibles, por lo que el video no se usa para afirmar recarga forzada, postback ni navegación interna; esos puntos continúan pendientes de confirmación visual u operativa explícita.

## Carga, recarga y navegación interna — 2026-08-12

- Confirmación explícita del piloto: después de `Ctrl+F5`, el mismo radicado puede abrirse de nuevo y `Factura` vuelve a cargar correctamente en el visor. Junto con el video de interacción, se valida carga inicial, actualización de la selección documental, recarga forzada y reapertura interna sin error de JavaScript reportado.
- Se acepta la tarea 6.5. Esta aceptación no cubre la comparación integral de paridad visual (6.2), el rollback individual de `a11y` (6.4) ni los cuatro viewports de evidencia (6.6).

## Rollback parcial de `a11y` — 2026-08-12

- Configuración temporal aplicada: flag maestro `true`; capas `layout,actions,documents`. Solo se retiró `a11y`; no se modificaron perfiles, handlers, permisos ni datos.
- Evidencia aportada por el piloto: `Grabación 2026-08-12 195543-m.mp4`. El fotograma de referencia muestra el radicado con `Factura` cargada y `Detalle` abierto mientras `a11y` está retirado; no se reportaron errores ni pérdida de interacción durante el recorrido con teclado.
- Se restauró inmediatamente la configuración completa `layout,actions,documents,a11y`. Con las evidencias previas de rollback maestro, `actions` y `documents`, se acepta la tarea 6.4. Los estados de foco se siguen verificando como parte de la evidencia visual de 6.6.

## Video responsive — 2026-08-12

- Se recibió `Grabación 2026-08-12 200407-resp.mp4` (118 s). La extracción temporal verificable muestra los anchos emulados 430 × 932, 412 × 915, 540 × 720 y 820 × 1180 px; en los recorridos se preservan el documento cargado y el modal `Información tarea` sin pérdida visible de interacción.
- Esta es evidencia parcial favorable para el comportamiento angosto y mediano. No contiene los anchos exactos de 1366, 1024, 768 y 375 px requeridos por 6.6, ni un estado de foco de teclado identificable; por ello no se declara completada 6.6 ni la paridad integral 6.2.

## Diagnóstico responsive de anchos exactos — 2026-08-12

- Se recibió `Grabación 2026-08-12 210802-otros.mp4` (3 min 27 s). El recorrido aporta los tamaños exactos 1366 × 768, 1024 × 768, 768 × 1024 y 375 × 812 px, con documento seleccionado y menú/modal abierto en distintos puntos.
- A 375 px se observa que documentos y visor siguen lado a lado y el conjunto queda reducido como escritorio. El CSS DOC-2 ya contiene el reflujo a una columna bajo 900 px; la causa es que `Webworkflow.aspx` no declaraba `viewport`, de modo que el navegador móvil calculaba un viewport CSS de escritorio y no activaba el media query.
- Primera corrección descartada: añadir el elemento a `Page.Header` durante `OnPreInit` evitó el error de AjaxControlToolkit, pero `Grabación 2026-08-12 213911-375-812.mp4` (34 s) confirmó que el meta no quedaba emitido y el layout seguía reducido como escritorio a 375 px.
- La captura posterior `prueba9.png`, con emulación exacta de 375 × 812 px, confirma el mismo hallazgo: el workbench se renderizaba reducido en una única composición de escritorio, por lo que no se considera evidencia de aceptación responsive.
- Diagnóstico de la segunda corrección: `Webworkflow.aspx` está dentro del iframe de `Defaul/WebFormInicioDocuarchiGestion.aspx`. Aunque el Workbench reciba su `HtmlMeta`, no puede cambiar el viewport del documento principal, que conserva el ancho de escritorio y se lo transfiere al iframe.
- Corrección vigente: el host superior declara `workflowCentroTrabajoModernShellViewport` como `HtmlMeta` estático, tipado e invisible, y lo habilita en `PreRender` con el mismo flag y piloto cerrado. `Webworkflow.aspx` conserva su meta para acceso directo y lo habilita en `Page_Load`. No hay bloques ejecutables `<% If %>` ni adiciones a la colección de `head`, por lo que AjaxControlToolkit puede registrar sus recursos y el baseline no piloto no cambia.
- Incidente corregido: la primera variante introdujo un bloque `<% If %>` directamente en `head`; AjaxControlToolkit no pudo añadir sus referencias CSS durante `OnLoad` y la página falló. Se retiró de inmediato. La segunda variante no falló, pero no emitió el viewport; también se sustituyó antes de aceptar la evidencia.
- Verificación local de la corrección de host y del reflujo: `npm.cmd test -- scripts/lib/doc2WorkflowActivation.test.js` aprobó 25/25 y cubre los `HtmlMeta` tipados del Workbench y del host, además de las columnas de acciones en el breakpoint angosto; la última compilación Debug incremental terminó con 0 errores y una advertencia conocida de dependencia. Falta repetir el punto exacto de 375 × 812 px con la barra de acciones completa; ningún resultado previo se usa como aceptación de esta corrección.
- Se recibió `Grabación 2026-08-12 223753-375.mp4` (33 s). Sus fotogramas a 375 × 812 px ya muestran texto de tamaño normal, lista documental a una sola columna y el inicio del visor debajo: confirma que el navegador principal ahora usa el viewport real y no una versión de escritorio reducida.
- Hallazgo derivado: la barra `#nav_menu` conserva columnas Bootstrap de base ancha y recorta las acciones a la derecha. Se añadió un reflujo scoped a 767 px o menos para que ambos grupos y sus hosts existentes ocupen el ancho disponible y envuelvan. Esta mejora requiere una nueva captura antes de aceptar 6.6; 6.2 y 6.6 continúan pendientes.
- Se recibió `Grabación 2026-08-13 073646-375-812.mp4` (88 s). Confirma a 375 px el viewport real, la fila documental seleccionada y la carga del visor en una sola columna. También revela que la primera corrección de acciones fue insuficiente: `Enviar a usuario`, `Enviar a grupo` y `Continuar flujo` aún se proyectan más allá del borde derecho.
- Corrección posterior a esa evidencia: dentro del mismo breakpoint se neutralizan los márgenes negativos de la `.row` legacy, devolución y avance pasan a filas completas y las dos transferencias comparten únicamente el espacio disponible. Los enlaces se limitan al ancho de su host. No se creó, movió ni sustituyó ningún control WebForms; se requiere una nueva captura para aceptar el resultado.
- Verificación local posterior: `npm.cmd test -- scripts/lib/doc2WorkflowActivation.test.js` aprobó 25/25 y la compilación Debug terminó con 0 errores y una advertencia conocida de dependencia. La validación comprueba los límites y el wrapping; no sustituye la próxima evidencia visual a 375 px.
- Se recibió `Grabación 2026-08-13 080455-375-812.mp4` (42 s). Verifica que devolución y avance ya se separan, pero confirma que el reparto `flex` de los `Panel` directos de la columna deja la transferencia a grupo fuera del borde. Se sustituye exclusivamente ese reparto, en el mismo breakpoint y para el piloto, por una cuadrícula de dos columnas: devolución y avance abarcan la fila completa; cada transferencia ocupa una celda real. Se necesita una tercera captura antes de aceptar 6.2 o 6.6.
- Las dos grabaciones son evidencia diagnóstica previa a la corrección definitiva, no una aceptación del layout a 375 px. Falta recargar el piloto y repetir el punto de 375 px para confirmar el apilado y registrar foco visible; 6.2 y 6.6 permanecen pendientes.

## Acotación del host de acciones en 375 px — 2026-08-13

- Se recibió `Grabación 2026-08-13 084302-375-812.mp4` (35 s). Conserva el reflujo de lista y visor, pero evidencia que el `collapse` de la barra operativa mantiene ancho intrínseco de escritorio: `Enviar a usuario` se ve completo y `Enviar a grupo` queda fuera del borde derecho. La cuadrícula interna no basta mientras su contenedor mida más que el viewport.
- Corrección vigente: dentro de `ctw-layer-actions` y solo a 767 px o menos, `#Menutol`, `#updatemenu`, `#nav_menu`, `#navbarNavDropdown_` y la columna existente se acotan a la anchura disponible con `box-sizing: border-box`; la cuadrícula de acciones sigue siendo de dos columnas reales y las decisiones de devolución/avance abarcan la fila completa. No se agrega, oculta, mueve ni reemplaza ningún control WebForms, ID, permiso, handler o postback.
- La hoja queda versionada como `20260813-mobilebound44`. Esta grabación es evidencia del defecto previo, no de aceptación: se requiere una nueva prueba con recarga forzada para confirmar que los cuatro controles quedan visibles sin scroll horizontal. Las tareas 6.2 y 6.6 continúan pendientes.

## Reemplazo del layout móvil de acciones — 2026-08-13

- Se recibió `Grabación 2026-08-13 090631-375-812.mp4` (39 s). Confirma que el defecto persiste: el Workbench conserva scroll horizontal a 375 px. El origen ya no se atribuye a una acción individual: la raíz del iframe se expande por el ancho intrínseco heredado y cualquier porcentaje interno se calcula sobre esa superficie más ancha.
- Se reemplaza, en lugar de acumular otra distribución de botones, la composición móvil de la subcapa `actions`: la raíz DOC-2 queda acotada a `100vw` y cada acción existente ocupa una fila de una única cuadrícula a 767 px o menos. La presentación de escritorio no cambia y no se ocultan, crean, mueven ni modifican controles WebForms, IDs, permisos, handlers o postbacks.
- La hoja queda versionada como `20260813-mobileviewport45`. El siguiente recorrido debe confirmar únicamente dos condiciones: ausencia de scroll horizontal del Workbench y visibilidad completa de devolución, ambos envíos y avance. La evidencia 6.2 y 6.6 continúa pendiente hasta obtenerla.

## Desbordamiento del documento iframe — 2026-08-13

- Se recibió `Grabación 2026-08-13 094314-375-812.mp4` (51 s). Confirma que la composición vertical ya muestra completas `Devolver`, `Enviar a usuario`, `Enviar a grupo` y `Continuar flujo`. Persiste, no obstante, una barra horizontal inferior: por tanto el origen ya no es una acción ni la cuadrícula del Workbench, sino el área scrollable del documento iframe.
- Corrección vigente: a 767 px o menos, el documento `html`, `body` y su `form` se acotan mediante `:has(.workflow-centro-trabajo-moderno.ctw-layer-layout)`. La regla solo existe cuando se entregó CSS DOC-2 a un piloto autorizado; evita scroll horizontal global y conserva los scrolls internos de los modales legacy. No se cambia markup, control WebForms, ID, permiso, handler, postback ni comportamiento baseline.
- La hoja queda versionada como `20260813-mobileframe46`. La grabación aporta evidencia favorable de visibilidad de acciones, pero no acepta aún 6.2/6.6: falta confirmar que desaparece la barra horizontal del documento en una recarga con esta versión.

## Confirmación del Workbench móvil — 2026-08-13

- Se recibió `Grabación 2026-08-13 094952-375-812.mp4` (45 s), en emulación de 375 × 812 px. Los fotogramas verificables muestran `Devolver`, `Enviar a usuario`, `Enviar a grupo` y `Continuar flujo` completos, sin proyección horizontal fuera del borde del Workbench.
- La barra horizontal inferior ya no está presente. El indicador vertical del borde derecho corresponde al desplazamiento vertical normal del contenido, necesario por la altura disponible; no hay desplazamiento lateral del documento ni de la barra operativa.
- Se acepta este punto de control responsive. Las tareas 6.2 y 6.6 continúan pendientes únicamente de la revisión integral de paridad y de los estados requeridos en todos los anchos, no por este defecto móvil ya corregido.

## Evidencia parcial de escritorio — 2026-08-13

- Se recibió `Grabación 2026-08-13 103843-1366-768.mp4` (42 s), con Chrome en 1366 × 768 px. Confirma la composición de escritorio: jerarquía de acciones, contexto, cabecera documental blanca y fila `Factura` seleccionada.
- El visor conserva la superficie gris en los fotogramas de inicio, intermedios y final; la grabación no demuestra la carga completa del PDF. Tampoco muestra el menú `Detalle` abierto, foco de teclado ni un control deshabilitado resuelto naturalmente por el servidor.
- La evidencia queda registrada como parcial. No se acepta 6.2 ni 6.6 hasta contar con esos estados y con la identificación del artefacto desplegado; no se observó un defecto nuevo de presentación en esta composición.

## Escritorio con visor y menú — 2026-08-13

- Se recibió `Grabación 2026-08-13 104244-1366-768.mp4` (29 s), en 1366 × 768 px. A diferencia de la toma anterior, el documento `Factura` carga en el visor, conserva la fila activa y la barra del visor; el índice permanece disponible mediante su control lateral existente.
- El menú `Detalle` se abre con estado activo ligero y borde fino; muestra las agrupaciones de información, trazabilidad y documentos. La entrada `Información de la tarea` abre el modal legacy correspondiente sin que la capa DOC-2 altere su contenido ni la operación. El aviso de datos mostrado durante el recorrido y el menú contextual nativo del navegador no se usan como resultado de una acción DOC-2.
- Pendiente para este ancho: evidencia inequívoca de foco por teclado y de un control deshabilitado resuelto naturalmente. La grabación tampoco identifica el SHA desplegado; por tanto refuerza 6.2/6.6 pero no permite completar esas tareas todavía.

## Tableta horizontal — 2026-08-13

- Se recibió `Grabación 2026-08-13 121105-1024-768.mp4` (20 s), con emulación de 1024 × 768 px. La aplicación conserva su navegación lateral y el Workbench usa el ancho útil restante: las acciones existentes se disponen en filas completas (`Devolver`, ambos envíos y avance) sin corte lateral; `Cerrar tarea`, contexto y cabecera documental permanecen dentro del viewport.
- El modal legacy `Información tarea` abre contenido desplazable dentro de la ventana sin desbordar el área emulada. Es comportamiento de la ruta existente y no una mutación DOC-2.
- En el material no queda una fila documental activa ni el PDF cargado: el área documental conserva el mensaje de selección. Tampoco hay foco demostrado mediante teclado ni control deshabilitado natural. Se registra como evidencia parcial de 1024 px; 6.2 y 6.6 continúan pendientes.

## Ancho operativo del Workbench con sidebar — 2026-08-13

- Se recibió `Grabación 2026-08-13 122049-1366-768.mp4` (40 s). La propia barra de emulación muestra 1024 × 768 px. Con el sidebar abierto, sus 290 px dejan aproximadamente 734 px al iframe y activan el breakpoint táctil de acciones: las decisiones pasan a filas completas y desplazan la lista documental fuera de la altura útil. Al cerrar el sidebar, la lista, su menú y el visor recuperan acceso inmediato.
- Corrección aplicada: `menu-vertical-responsivo.js` detecta únicamente la combinación de meta de piloto emitido por servidor y ruta iframe `workflow/Webworkflow.aspx`. Tras la carga del iframe, a 1199 px o menos aplica el toggle legacy para cerrar el sidebar; fuera de Workflow, sin meta de piloto o sobre ese ancho mantiene exactamente el umbral baseline de 992 px. El usuario conserva el mismo control para abrir navegación cuando lo requiera.
- La hoja de host referencia `menu-vertical-responsivo.js?v=20260813-workbench-shell47`. Se requiere una nueva captura a 1024 × 768 con el sidebar inicialmente cerrado y la fila documental visible antes de aceptar 6.2/6.6.
- Validación local de la corrección: `npm.cmd test -- scripts/lib/doc2WorkflowActivation.test.js` finaliza con 26 pruebas correctas; la compilación Debug finaliza con 0 errores (1 advertencia preexistente) y la validación estricta de OpenSpec concluye correctamente. `git diff --check` no reporta errores de espacios.

## Confirmación del ancho operativo con sidebar — 2026-08-13

- Se recibió `Grabación 2026-08-13 124000-1366-768.mp4` (82 s). Aunque el nombre contiene `1366-768`, la barra de emulación visible confirma 1024 × 768 px.
- Desde el primer estado funcional del Workbench, el menú lateral del host está contraído (se conserva su icono de apertura). `Documentos (1)`, su botón `Acciones` y la fila `Factura` quedan a la vista sin que el operador deba cerrar manualmente la navegación. La superficie del visor conserva ancho operativo.
- La toma además muestra el menú `Detalle` con sus grupos e integra las acciones del flujo dentro del viewport. Confirma la corrección puntual de accesibilidad de la lista documental a 1024 px. No sustituye la evidencia pendiente de foco por teclado, control deshabilitado natural ni la verificación integral de paridad necesarias para 6.2/6.6.

## Tableta vertical — 2026-08-13

- Se recibió `Grabación 2026-08-13 130904-768-1024.mp4` (39 s). La barra de emulación confirma 768 × 1024 px.
- El Workbench conserva dentro del viewport sus acciones, contexto, cabecera blanca `Documentos (1)`, botón `Acciones` y fila `Factura`. Al seleccionar la fila se muestra el marcador azul rectangular sin desplazar el check; la barra contextual, `Metadatos`, toolbar y contenido PDF cargan con normalidad. El activador lateral de índice permanece disponible.
- `Detalle` abre su menú agrupado de información, trazabilidad y documentos sin corte lateral. `Información de la tarea` abre el modal legacy, que mantiene el contenido dentro del área emulada y su scroll interno.
- El estado activo de `Detalle` visible puede proceder del clic del puntero, por lo que no acredita foco obtenido con teclado. Tampoco se presentó un control deshabilitado decidido naturalmente por servidor ni se abrió el índice. La evidencia confirma la composición y estados mostrados a 768 px, pero 6.2/6.6 siguen pendientes de esos casos específicos y de la evidencia asociada al artefacto desplegado.

## Escritorio integral — 2026-08-13

- Se recibió `Grabación 2026-08-13 140212-1366-768.mp4` (77 s), con la barra de emulación visible en 1366 × 768 px. A este ancho el sidebar conserva su estado de escritorio y el Workbench mantiene acciones, contexto, cabecera documental, fila `Factura` seleccionada, visor PDF y toolbar sin recortes.
- `Detalle` muestra los grupos de información, trazabilidad y documentos. La apertura de `Información de la tarea` conserva el modal legacy y su cierre recibe un contorno de foco visible. El panel lateral `Índice` se abre sobre el visor, mantiene sus campos y puede coexistir con el menú `Detalle` y con la lista documental.
- El menú contextual `Acciones` de documentos se despliega completo, respeta las separaciones semánticas y expone sus estados visuales —incluida la opción de eliminación condicionada por selección— sin alterar la fila activa. La grabación también muestra un hover localizado en una opción del menú.
- Hallazgo funcional fuera de DOC-2: al abrir `Adjuntar digitalizado`, la superficie legacy `Digitalización` permanece sin contenido durante aproximadamente 10 s de la toma. DOC-2 no crea ni altera esa ruta; se requiere confirmar si depende de servicio, dispositivo o configuración externa antes de tratarlo como incidencia funcional.
- La grabación no identifica el SHA o versión exacta del artefacto desplegado. El contorno de foco es visible, pero el vídeo no permite demostrar de forma concluyente que se obtuvo mediante navegación con `Tab`. Se conserva 6.2/6.6 como pendientes hasta asociar el artefacto y completar esos criterios en las resoluciones restantes.

## Móvil final — 2026-08-13

- Se recibió `Grabación 2026-08-13 141106-375-812.mp4` (38 s). Aunque el contenedor de la grabación mide 788 × 916 px, la barra de emulación visible confirma 375 × 812 px al 100 %.
- Las acciones del flujo se presentan en filas completas y visibles —`Devolver`, ambos envíos y `Continuar flujo`— sin desplazamiento horizontal. El contexto, cabecera `Documentos (1)`, botón `Acciones`, fila `Factura` seleccionada, barra contextual y visor PDF permanecen dentro del viewport.
- `Detalle` abre el menú agrupado y ajustado al ancho móvil, sin corte lateral. `Información de la tarea` abre el modal legacy dentro de la pantalla; `Enviar a usuario` abre su selector legado dentro de un modal desplazable, sin alterar la capa DOC-2.
- La secuencia muestra el foco visible azul alrededor de `Enviar a usuario` antes de abrir su modal. Esta vez el contorno se observa separado del hover del puntero y acredita el estado de foco presentado por la capa de accesibilidad.
- La paridad visual de acciones, lista documental, visor e índice queda confirmada con las evidencias de 1366, 1024, 768 y 375 px. El único hallazgo de funcionalidad observado sigue siendo la superficie vacía de `Digitalización`, fuera del alcance de DOC-2. Para 6.6 todavía falta asociar el SHA/versión del artefacto desplegado y evidenciar un control que el servidor renderice deshabilitado naturalmente; no se debe simular ni forzar ese estado.

## Trazabilidad pendiente de artefacto — 2026-08-13

- La revisión local identifica `4dba10b` como `HEAD`, pero los archivos del piloto DOC-2 —incluidos host, Workbench, CSS, adaptadores y esta evidencia— continúan modificados sin commit. Ese SHA no representa el artefacto que se está validando y no debe asociarse a las grabaciones.
- Para completar la trazabilidad se requiere que el cambio validado tenga un commit o versión de build desplegada. Tras ello, la misma referencia debe anotarse junto a la evidencia visual.
- El control deshabilitado natural se puede observar sin completar una operación: con un documento PDF seleccionado, abrir `Acciones` → `Adjuntar a la lista`; en el modal legacy `Adjuntar`, `CheckBox_relacionado_radicado_adj` se emite visible, marcado y deshabilitado por servidor para extensiones distintas de TIF/BMP/JPG. Se debe registrar únicamente su estado y cerrar el modal, sin seleccionar archivos ni confirmar carga.

## Estado deshabilitado natural en móvil — 2026-08-13

- Se recibió `Grabación 2026-08-13 142257-inactivo.mp4` (12 s). Aunque el contenedor de vídeo mide 818 × 932 px, la barra de emulación visible confirma 375 × 812 px al 100 %.
- Con `Factura` (PDF) ya seleccionada y cargada en el visor, el recorrido abre `Acciones` → `Adjuntar a la lista`. El modal legacy `Adjuntar` se presenta íntegro dentro del viewport y muestra `Adjuntar como documento relacionado` marcado en gris e inactivo; el estado se observa sin seleccionar archivos ni confirmar una carga.
- La inspección de la ruta legacy coincide con la evidencia: para extensiones distintas de `.TIF`, `.BMP` o `.JPG`, `Webworkflow.aspx.vb` emite `CheckBox_relacionado_radicado_adj` visible, `Enabled = False` y `Checked = True` (por ejemplo, líneas 3819–3824). DOC-2 no habilita, oculta ni simula ese control.
- Con esta toma quedan evidenciados el estado deshabilitado natural, el documento seleccionado, el menú contextual y el viewport de 375 px. La asociación al artefacto se resolvió posteriormente con el commit `1879221`, descrito en la sección siguiente; `4dba10b` continúa excluido porque no contiene los cambios DOC-2.

## Asociación de evidencia al artefacto local desplegado — 2026-08-13

- El commit local de release candidate `187922143fbe1a2436163807d1935b5128c8d1e9` (`feat(DOC-2): modernizar workbench piloto reversible`) contiene los 31 archivos del alcance DOC-2, incluidas las capas visuales, activación servidor, pruebas y trazabilidad.
- La configuración de IIS de `Default Web Site` resuelve `/GestionDocumental-Docuarchi.net` a `D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net`, exactamente el directorio de trabajo del commit. Por ello, las capturas y vídeos realizados contra `https://localhost/GestionDocumental-Docuarchi.net` corresponden al código fijado en `1879221`; no se publicó ni desplegó a un ambiente remoto.
- Queda completada la evidencia de 1366, 1024, 768 y 375 px: hover localizado y menú abierto (1366), foco visible (1366 y 375), documento seleccionado y visor (1366, 768 y 375), sidebar operativo/lista accesible (1024) y control deshabilitado natural (375). La tarea 6.6 se acepta para el ambiente local piloto.
