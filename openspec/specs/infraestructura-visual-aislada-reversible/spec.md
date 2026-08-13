# infraestructura-visual-aislada-reversible Specification

## Purpose

Define un Centro de Trabajo Workflow visual, reversible y activado por servidor para pilotos, sin alterar operaciones WebForms ni la línea base fuera del piloto.

## Requirements

### Requirement: Activación servidor y piloto cerrado
El sistema SHALL activar la capa DOC-2 solo cuando `WorkflowCentroTrabajoModernEnabled` sea verdadero y el login de gestión de sesión (`GA_LOGINUSUARIOGESTION`) coincida exactamente con un perfil configurado en `WorkflowCentroTrabajoModernPilotProfiles`.

#### Scenario: Modo apagado o usuario fuera de piloto
- **WHEN** el flag maestro es falso, falta la lista de perfiles o el login no pertenece a ella
- **THEN** `Webworkflow.aspx` no entrega los recursos DOC-2 ni la clase raíz moderna.

#### Scenario: Usuario piloto
- **WHEN** el flag maestro es verdadero y el login de gestión coincide con un perfil configurado en servidor
- **THEN** el contenedor `#div_content_general_wf` recibe la clase raíz y las subcapas calculadas por el servidor.

### Requirement: Capas reversibles y dependencia explícita
El sistema SHALL emitir `ctw-layer-layout`, `ctw-layer-actions`, `ctw-layer-documents` y `ctw-layer-a11y` desde `WorkflowCentroTrabajoModernLayers`, con `layout` como dependencia de las demás.

#### Scenario: Rollback parcial
- **WHEN** un piloto tiene el modo moderno activo y se retira una subcapa de la configuración
- **THEN** solo los estilos de esa subcapa dejan de aplicar sin retirar la hoja, cambiar eventos o modificar datos.

#### Scenario: Layout deshabilitado
- **WHEN** la configuración no contiene `layout`
- **THEN** no se emite ninguna subcapa y los componentes scoped quedan inertes.

### Requirement: Recursos aislados y ordenados
El sistema SHALL cargar `workflow-centro-trabajo-moderno.css` y `centro-trabajo-visual.js` después de `Webworkflow.js` y de los recursos legacy relevantes, con versión de caché explícita.

#### Scenario: Preservación de la línea base visual previa
- **WHEN** se entrega `Webworkflow.aspx`
- **THEN** los recursos manuales aprobados antes de DOC-2 conservan su ruta de carga y la capa DOC-2 se entrega después de ellos solo para un piloto autorizado.

### Requirement: Viewport responsive aislado al piloto
El sistema SHALL declarar un `HtmlMeta` estático y tipado, invisible por defecto, tanto en el host superior `Defaul/WebFormInicioDocuarchiGestion.aspx` como en el acceso directo `workflow/Webworkflow.aspx`. El host lo habilita en `PreRender` y el Workbench en `Page_Load`, solo para el mismo flag y perfil piloto cerrados. Así el navegador principal y sus iframes evalúan los breakpoints DOC-2 contra el ancho real del dispositivo, sin modificar la colección de controles de `head` ni insertar bloques ejecutables de código en ella.

#### Scenario: Piloto en dispositivo angosto
- **WHEN** un perfil piloto abre Workflow en un viewport de 375 px
- **THEN** el navegador usa 375 px como ancho CSS y aplica el reflujo scoped de DOC-2; la lista documental y el visor no se presentan como una versión de escritorio reducida.

#### Scenario: Baseline fuera de piloto
- **WHEN** el flag está apagado o el usuario no pertenece al piloto
- **THEN** ni el host ni la página Workbench emiten el `meta viewport` DOC-2 y la cabecera baseline conserva su comportamiento sin cambios por esta capa.

#### Scenario: Navegación del host en ancho intermedio
- **WHEN** un piloto autorizado carga `workflow/Webworkflow.aspx` dentro del iframe del shell con un ancho de 1199 px o menor
- **THEN** el shell usa el toggle legacy para iniciar el sidebar cerrado y conservar ancho útil para la lista documental y sus acciones; en otro módulo, sin piloto o por encima de ese ancho conserva el umbral baseline de 992 px.

### Requirement: Inercia estricta fuera de activación
Sin flag maestro y piloto de servidor aprobados, el sistema SHALL conservar la línea base visual aprobada y no entregar una segunda ruta de recursos, clase raíz o mutación DOC-2.

#### Scenario: Modo apagado sin efectos colaterales
- **WHEN** el flag es falso o el usuario no pertenece al piloto
- **THEN** la interfaz conserva los recursos manuales previos, no recibe CSS/JavaScript DOC-2 y no cambia DOM, controles, permisos, postbacks ni comportamiento por causa de DOC-2.

### Requirement: Adaptador sin efecto funcional
El sistema SHALL limitar el adaptador DOC-2 a añadir clases de presentación dentro del contenedor moderno, incluso tras una actualización parcial de ASP.NET AJAX.

#### Scenario: Integridad de WebForms
- **WHEN** se ejecuta un postback o una actualización de `UpdatePanel`
- **THEN** el adaptador no mueve controles, no reemplaza IDs, atributos de negocio o foco, no habilita acciones ocultas por el servidor y no bloquea los scripts legacy.

### Requirement: Activación coherente de documentos relacionados
El sistema SHALL abrir un documento relacionado al pulsar la zona no interactiva de su fila mediante el handler legacy de visualización ya renderizado. La selección visual SHALL reflejar únicamente una activación que haya actualizado la selección legacy.

#### Scenario: Clic sobre la fila de documento
- **WHEN** el usuario pulsa el contenido o espacio no interactivo de una fila de `#GridView_list_documento_relacion_wf`
- **THEN** el sistema reutiliza los atributos `tip_event`, `id_wf` e `idd_wf` del activador renderizado, ejecuta el handler legacy `prevent` y conserva el postback existente que abre el visor.

#### Scenario: Controles propios de la fila
- **WHEN** el usuario pulsa el checkbox de selección, su celda, un enlace, botón o el menú desplegable de una fila
- **THEN** el control conserva su comportamiento propio y no se abre el visor por la delegación de fila.

### Requirement: Contrato CSS reutilizable
El sistema SHALL definir tokens y componentes `.ctw-btn`, `.ctw-icon-btn`, `.ctw-menu`, `.ctw-menu__panel`, `.ctw-badge`, `.ctw-action-bar` y `.ctw-document-bar` únicamente bajo `.workflow-centro-trabajo-moderno`.

#### Scenario: Clase agregada manualmente desde cliente
- **WHEN** un usuario fuera del piloto agrega una clase manualmente desde el navegador
- **THEN** no obtiene recursos ni autorización adicionales porque la decisión y la entrega fueron resueltas por servidor.

### Requirement: Fidelidad visual sobre contenedores existentes
El sistema SHALL aplicar el lenguaje visual del HTML modelo a los contenedores WebForms existentes de acciones, documentos, visor e índice, sin sustituir controles ni alterar IDs, permisos, handlers, `UpdatePanel` o postbacks. La capa `actions` SHALL coordinar visualmente `#menucab` y `#nav_menu` por clases de presentación. Para un piloto aprobado podrá consolidar las entradas duplicadas de `Panel_documentos_tarea` en `#ctw-document-actions-menu`, usando las mismas rutas legacy y distinguiendo la operación sobre el documento actual de la operación sobre selección múltiple; fuera del piloto el menú superior baseline permanece sin cambios. Puede emitir únicamente para el piloto una cabecera y contexto de tarea decorativos de servidor, sin acciones, postbacks ni datos de negocio paralelos.

#### Scenario: Piloto con un documento abierto
- **WHEN** un piloto abre una tarea con documento relacionado e índice disponible
- **THEN** `#menucab`, `#nav_menu`, `#div_label`, la lista de documentos, `#contenido_imagen` y `#contenido_indice` reproducen jerarquía, bordes, fondos, densidad, estado seleccionado y respuesta móvil del modelo dentro del contenedor moderno. `#content_pie_seleccion_tarea` se presenta como una única franja de contexto de tarea sobre documentos, visor e índice y conserva sus labels funcionales existentes; `#div_label` conserva la cabecera de documentos en su panel; `#Panel_tolbar_pdf`, sobre el visor, forma la barra contextual con título, formato, carga, metadatos disponibles, versiones y menú de acciones mediante los valores de la selección actual y los handlers legacy existentes. Si la lógica existente muestra `#pendiente_selec_tarea`, el piloto conserva su icono, etiqueta, id, visibilidad y handler `E-ETP`; la transición de cierre o envío a pendientes sigue siendo la que determine legacy, igual que fuera de piloto.

#### Scenario: Cabecera y selección documental alineadas
- **WHEN** un piloto muestra documentos bajo `ctw-layer-documents`
- **THEN** la cabecera de documentos conserva fondo blanco y el check maestro comparte la misma guía horizontal con los checks de fila; al seleccionar una fila, su marca azul rectangular se dibuja independientemente, sin borde, sombra, radio ni variación del ancho o posición de la primera celda.

#### Scenario: Proyección de acciones del documento seleccionado
- **WHEN** el servidor ya renderizó una acción individual para el documento seleccionado
- **THEN** la barra DOC-2 puede emitir su acceso contextual usando los mismos `tip_event`, `id_wf`, `idd_wf` y handler legacy; no crea lógica de negocio, no dispara clics artificiales y no proyecta una acción que la fila original no contiene.

#### Scenario: Hover puntual en contexto del visor
- **WHEN** un piloto apunta a la barra contextual de un documento abierto
- **THEN** el título, formato y espacio sobrante permanecen neutros; la acción visible queda anclada fuera del área flexible y solo su enlace recibe hover o foco. En el breakpoint angosto vuelve al flujo estático para no superponerse al contexto.

#### Scenario: Todas las opciones de acción renderizadas
- **WHEN** el servidor muestra cualquiera de las opciones, envíos, iconos o dropdowns de `#menucab` o `#nav_menu`
- **THEN** la subcapa `actions` los resuelve desde sus controles hijos existentes, conserva sus handlers y los presenta en el grupo visual correspondiente sin depender del ClientID de un `Panel`. Para piloto, `#pendiente_selec_tarea`, si es visible, conserva la acción `E-ETP` de cierre o envío a pendientes; `Panel_tareas_estado_pendiente` sigue sujeto a la visibilidad decidida por servidor/legacy. Fuera de piloto, el control conserva el mismo handler baseline.

#### Scenario: Jerarquía de controles de acción
- **WHEN** un piloto visualiza las barras de herramientas de tarea
- **THEN** `Opciones`, `Detalle`, notas e historial se presentan como controles ghost con hover y foco visible; devolución, transferencias y la acción terminal `#pendiente_selec_tarea` usan borde suave; solo la acción de avance `ctw-action-slot--send` es sólida. Los IDs, iconos, handlers, permisos y objetivos táctiles existentes permanecen sin cambio.

#### Scenario: Acciones en viewport angosto
- **WHEN** el host piloto se muestra a 375 px
- **THEN** los grupos existentes de `#nav_menu` ocupan el ancho disponible y sus acciones envuelven entre líneas, sin recorte horizontal, reparenting, nuevos botones ni cambio de handlers.

#### Scenario: Dropdown Detalle estructurado
- **WHEN** un piloto abre `Detalle`
- **THEN** su estado activo usa fondo suave y borde fino sin halo persistente; el dropdown dispone de ancho adaptable de 360–380 px, permite etiquetas largas en varias líneas y agrupa las siete entradas existentes como información, trazabilidad y documentos. Los iconos de esas entradas se diferencian por función y los handlers/rutas legacy permanecen sin modificación; fuera del piloto no se emiten los grupos ni se cambia la iconografía baseline.

#### Scenario: Simplificación de Opciones solo para piloto
- **WHEN** `WorkflowCentroTrabajoModernActive` es verdadero
- **THEN** el dropdown `Opciones` presenta `Recuperar tarea` y `Servicio default`, pero no emite `Detalle de la sesión`, `Grupo relacionado` ni `Estado de paginación`. Sus controles y handlers permanecen en el markup baseline no piloto.

#### Scenario: Traslado de Servicio default solo para piloto
- **WHEN** `WorkflowCentroTrabajoModernActive` es verdadero
- **THEN** el menú visual `Servicios` no se emite, mientras `bnt_eval_event_default` se presenta una única vez dentro de `Opciones` con su handler `event_element_clic(event,this)`. `Panel_tramitar_tarea` y `Panel_documentos_tarea` se conservan como controles WebForms para las búsquedas de servidor; solo sus listas visuales se omiten en piloto.

#### Scenario: Consolidación documental solo para piloto
- **WHEN** `WorkflowCentroTrabajoModernActive` es verdadero
- **THEN** la lista visual de `Panel_documentos_tarea` no se emite y `#ctw-document-actions-menu` reúne adjunto a lista, carga desde servicio, digitalización, eliminación de selección, firma, compartición y una única instancia de las acciones masivas de copia o vinculación. El menú no incluye `Cargar desde visor` ni `Eliminar documento actual`; cada entrada restante conserva su handler legacy. El panel se conserva como control WebForms; en modo no piloto presenta su menú baseline sin cambios.

#### Scenario: Cromo de contexto solo para piloto
- **WHEN** `WorkflowCentroTrabajoModernActive` es verdadero
- **THEN** la página presenta `#content_pie_seleccion_tarea` como una única franja contextual. La franja conserva los labels funcionales existentes y, solo en piloto, emite dos labels decorativos de solo lectura para título de trámite y estado ya calculados en servidor; se ubica antes del área documental y no crea consultas, postbacks ni acciones. Cuando es falso, su posición y presentación baseline no cambian.

#### Scenario: Jerarquía estructurada de tarea
- **WHEN** un piloto selecciona una tarea con trámite, flujo o ruta y estado resueltos por la lógica existente
- **THEN** el contexto presenta título del trámite en mayúscula visual, estado separado, `Radicado <valor> · <solicitante>` y `<Flujo|Ruta> · <nombre>` en campos diferenciados. La capitalización CSS no modifica el valor de negocio y no se parsean cadenas concatenadas en cliente.

#### Scenario: Contexto compacto de escritorio
- **WHEN** un piloto visualiza una tarea en un viewport de escritorio
- **THEN** la franja de contexto ocupa dos líneas como máximo: título/estado y metadatos/proceso, con un mínimo de 46 px; en el breakpoint angosto los campos pueden apilarse para conservar su lectura sin reducir el área del visor en escritorio.

### Requirement: Claridad de autorización y conteo en piloto
El sistema SHALL diferenciar en el piloto el estado persistente de autorización de su historial de consulta y SHALL presentar el contador documental con la forma `Documentos (N)`, sin modificar el contrato baseline fuera del piloto.

#### Scenario: Estado e historial de autorización
- **WHEN** la lógica legacy muestra `Panel_autoriza` para un piloto
- **THEN** `CheckBox_auturiza` conserva `prevent_autoriza_tarea` y se rotula como estado `Autorizada`; el trigger `A11` se rotula `Historial` y su único menú conserva el acceso legacy a la lista de autorizaciones.

#### Scenario: Usuario no piloto
- **WHEN** el flag está apagado o el usuario no pertenece al piloto
- **THEN** los textos, el handler y el formato de conteo baseline permanecen sin modificación por DOC-2.

### Requirement: Contrato CSS y orden de caché verificables
El sistema SHALL reproducir el contrato de `CONTRATO-CSS-COMPONENTES-REUTILIZABLES.md` exclusivamente dentro de `.workflow-centro-trabajo-moderno` y emitir sus recursos con versión de caché después de `Webworkflow.js` y de los recursos legacy relevantes.

#### Scenario: Recarga y postback de un piloto
- **WHEN** un piloto realiza carga inicial, recarga forzada, navegación interna o una actualización parcial de ASP.NET AJAX
- **THEN** los recursos versionados permanecen en el orden documentado, el adaptador no bloquea scripts legacy y no genera cambios fuera del contenedor moderno.

### Requirement: Evidencia de activación y reversión
Antes de aprobar el despliegue, el cambio SHALL contar con evidencia por SHA de la comparación modo apagado/encendido, usuario dentro/fuera de piloto y rollback maestro/parcial.

#### Scenario: QA manual autorizada
- **WHEN** se dispone de ambiente WebForms, acceso TLS, cuentas piloto/no piloto y datos Workflow controlados
- **THEN** se ejecutan los recorridos de activación, rollback, caché, navegación interna y los viewports 1366, 1024, 768 y 375 px; un resultado pendiente no se declara aprobado.

### Requirement: Politica Frontend AppResponses
La política de `AppResponses<T>` SHALL aplicar solo si un ticket crea o modifica consumidores de ese envelope.

#### Scenario: Ticket WebForms sin consumidor AppResponses
- **WHEN** DOC-2 no crea servicios, hooks ni componentes que consuman `AppResponses<T>`
- **THEN** no se crea un helper frontend ni parser local fuera de alcance.
