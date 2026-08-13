## Context

DOC-2 afecta `workflow/Webworkflow.aspx`, una página ASP.NET WebForms con `UpdatePanel`, postbacks y permisos resueltos en servidor. El único contenedor autorizado para la capa es `#div_content_general_wf`.

La revisión encontró recursos visuales manuales aprobados antes de DOC-2: `gridview-moderno.css`, `workflow-tareas-modernas.css`, `workflow-documentos-relacionados-modernos.css`, `workflow-documentos-relacionados-titulo.css`, `workflow-paginacion-visual.js`, `documentos-relacionados-visual.js` y `documentos-relacionados-titulo-visual.js`. Dan forma a la lista moderna de documentos y reubican sus iconos/acciones; son la línea base que debe conservarse con el flag DOC-2 apagado.

## Goals / Non-Goals

**Goals**

- Aislar una capa visual que el servidor active solo para pilotos.
- Entregar tokens y componentes del contrato CSS sin alterar controles WebForms.
- Permitir rollback maestro y parcial por configuración.

**Non-Goals**

- Cambiar permisos, reglas Workflow, datos, ni la implementación de servidor del visor.
- Crear consumidores `AppResponses<T>` o una infraestructura frontend ajena a esta aplicación WebForms.

## Decisions

1. `WorkflowCentroTrabajoModernEnabled` se lee con `ConfigurationManager.AppSettings`; su valor efectivo predeterminado es `false`.
2. `WorkflowCentroTrabajoModernPilotProfiles` es una lista exacta, separada por coma o punto y coma, de valores servidor de `Session("GA_LOGINUSUARIOGESTION")`. No acepta parámetros, cookies, clases añadidas por cliente ni comodines.
3. La página emite `workflow-centro-trabajo-moderno` y los recursos DOC-2 solo si están activos flag maestro y perfil piloto. En cualquier otra combinación no añade clase ni entrega CSS/JavaScript nuevo.
4. `WorkflowCentroTrabajoModernLayers` controla `layout`, `actions`, `documents` y `a11y`; `layout` es dependencia de las demás. El valor predeterminado para un piloto es `layout,actions,documents,a11y`.
5. `workflow-centro-trabajo-moderno.css` y `centro-trabajo-visual.js` se cargan después de `Webworkflow.js` y el resto de scripts legacy antes del `body`, con versión de caché explícita.
6. Los siete recursos manuales inventariados permanecen en la ruta de carga inalterados como baseline aprobado. DOC-2 no los duplica ni los mueve: su hoja empieza todos los selectores con `.workflow-centro-trabajo-moderno` y su adaptador solo añade clases nuevas dentro del contenedor cuando el servidor habilita el piloto. Así la capa nueva se carga después de la base, con precedencia explícita y sin una segunda ruta visual no controlada.
7. La política `AppResponses<T>` sembrada por el flujo no es aplicable: DOC-2 no crea esos consumidores y el repositorio no contiene `src/shared/api`.
8. La fidelidad al HTML modelo incluye tratamiento visual del shell existente: las dos barras de acciones `#menucab` y `#nav_menu`, cabecera de documentos, lista, visor e índice. La subcapa `actions` coordina sus grupos por clases de presentación y `flex`, manteniendo cada nodo, ID, permiso, `UpdatePanel` y postback en su ubicación funcional. El control existente `#pendiente_selec_tarea` conserva para piloto y baseline su id, visibilidad, icono, etiqueta y handler `E-ETP`; la lógica legacy decide si cierra la tarea o la envía a pendientes. CSS puede aplicar grid, orden visual, ancho, borde, fondo, densidad y respuesta móvil sobre los contenedores existentes, sin mover controles WebForms.
9. `Webworkflow.aspx` emite para el piloto solo un contexto de tarea decorativo, sin botones, postbacks ni lógica de cliente. El contexto conserva `Label_estado_tarea_selecion` para radicado/solicitante y `Label_estado_selecion` para flujo o ruta; añade exclusivamente `Label_contexto_tramite` y `Label_contexto_estado` como labels de presentación que separan el título y estado ya calculados dentro de `Actualiza_interface_estado_flujo_ruta`. No consulta una fuente adicional ni crea un estado de negocio paralelo. Es un elemento de servidor condicional a `WorkflowCentroTrabajoModernActive`; el pie reutiliza los labels funcionales ya existentes.
10. El helper baseline `documentos-relacionados-titulo-visual.js` conserva su comportamiento aprobado fuera del piloto. Dentro de una raíz DOC-2 con la subcapa `documents`, deja los enlaces rápidos existentes (`#btnLoadFile`, `#btnloadservice` y actualización de índice) en la cabecera `#div_label`, sin moverlos al dropdown. Los IDs y manejadores se conservan.
11. Para reproducir la barra de documento del patrón sin alterar el adaptador, `#div_label` permanece dentro del panel lateral de documentos y el visor existente ocupa su columna. En el piloto, la cabecera lateral conserva el título y selección de documentos; sus accesos rápidos se ocultan solo visualmente porque la barra contextual equivalente se emite sobre el visor, sin reparentar esos nodos ni sus IDs.
12. La barra contextual se emite solo para el piloto dentro de `#Panel_tolbar_pdf`, sobre el visor. Toma título, formato e identificadores de la selección actual entregada por servidor. `Cargar` usa el handler existente de adjunto; metadatos sigue reutilizando `#id_indice_wf_pdf` cuando el visor PDF lo habilita. `Versiones`, firma, cambio de tipología, reemplazo y eliminación conservan `tip_event`, `id_wf`, `idd_wf` y el handler legacy `prevent` con los valores de la selección. No se añade una ruta de negocio ni se simula un clic.
13. Por decisión explícita de producto, el piloto omite la lista visual de `Panel_documentos_tarea` y consolida sus acciones útiles en `#ctw-document-actions-menu`, pero conserva el `Panel` WebForms porque la lógica legacy lo localiza por ID. Las acciones masivas canónicas son `a_copy_document_production_proceedings`, `a_copy_document_proceedings`, `a_link_document_proceedings` y `a_auto_link_document_proceedings`; las variantes con sufijo `_` quedan solo en baseline. El menú consolidado mantiene `C-DW-ENL` y `C-DW-AUTO`, pero no presenta `C-DW-VIS` ni `prevent_elimina_adjunto()`; `C-DW-DEL-IMAGE` sigue siendo la eliminación de selección múltiple. Las rutas retiradas permanecen disponibles únicamente en sus controles legacy fuera del menú. No se modifica ningún handler ni permiso.
14. Para el piloto, `Opciones` conserva solo `Recuperar tarea`. Las entradas `S-DDS`, `S-GAU` y `Button_activa_estado_paginacion` se dejan bajo la condición no piloto: no se borran sus handlers ni sus botones WebForms ocultos y el modo baseline los sigue renderizando.
15. Para el piloto, `Servicio default` se traslada de `Servicios` a `Opciones` con el mismo id `bnt_eval_event_default` y el handler `event_element_clic(event,this)`. `Panel_tramitar_tarea` se mantiene como control WebForms vacío, ya que `Classselecciotarea.vb` lo busca por ID; fuera de piloto conserva su menú `Servicios` original.
16. La lista baseline `#GridView_list_documento_relacion_wf` mantiene como única ruta de apertura el handler legacy `prevent` y el postback `Button_selecion_treview_documento`. El adaptador de lista puede delegar a ese handler cuando el usuario pulse una zona no interactiva de la fila; no usa `.click()` artificiales, no crea otra ruta de visor y excluye checkbox, primera celda de selección y menú desplegable. El estado visual de fila se actualiza únicamente después de esa activación real.
17. En piloto, `#content_pie_seleccion_tarea` deja de consumir una fila al final del workbench y se convierte visualmente en la primera fila de `#content_selecion_tarea`. Conserva el mismo `UpdatePanel` y los dos labels funcionales. Dentro de ese mismo `UpdatePanel`, el piloto emite dos labels decorativos adicionales para título de trámite y estado; no se duplica contexto, no se crean acciones ni se modifica el orden funcional del DOM.
18. El checkbox `CheckBox_auturiza` conserva el handler `prevent_autoriza_tarea` que persiste la autorización. Su etiqueta visible describe el estado `Autorizada`; el desplegable `A11` se nombra `Historial` y solo expone la consulta de autorizaciones. Esta diferenciación no agrega confirmación ni modifica el servicio, permisos o postback existentes.
19. La capa `actions` reduce únicamente densidad visual de las dos barras existentes: mantiene controles de al menos 44 px en el breakpoint táctil y no agrupa, oculta o reparenta acciones. El contexto superior y la barra contextual del documento son franjas distintas: la primera es de tarea y la segunda de documento.
20. El contador que `ClassDaGabinete` actualiza se formatea como `Documentos (N)` solo cuando la página que lo solicitó tiene `WorkflowCentroTrabajoModernActive`; la ruta baseline sigue emitiendo su texto original.
21. El contexto estructurado del piloto conserva los valores de negocio tal como se recuperan: no aplica correcciones lingüísticas ni capitalización destructiva. Solo el título recibe `text-transform: uppercase` de CSS; el campo de proceso muestra `Flujo` o `Ruta` según la rama legacy que resolvió el estado.
22. En escritorio, el contexto estructurado se presenta en dos líneas dentro de una franja de 46 px mínimos: título/estado y metadatos/proceso. En el breakpoint angosto esos campos pueden apilarse para conservar legibilidad; esta reducción es CSS scoped y no modifica los labels, `UpdatePanel` ni el visor.
23. La cabecera de documentos del piloto conserva fondo blanco, como superficie de trabajo separada del shell azul-gris. La fila documental seleccionada expresa su marca azul mediante una barra rectangular `::before` en la primera celda, no con `border-left` ni `box-shadow`; de ese modo el check de fila no cambia de posición y el borde/radio legacy no se revela durante la selección. Ningún tamaño, handler o dato baseline se modifica.
23. En piloto, las herramientas de contexto (`Opciones`, `Detalle`, notas, historial y vuelta a tareas) no mantienen borde persistente: usan estado ghost con fondo suave en hover y foco visible. `Devolver` y transferencias conservan borde suave; el botón de avance ya clasificado como `ctw-action-slot--send` es el único sólido. La agrupación de autorización conserva su checkbox, etiqueta y separador de historial, pero no una caja exterior persistente.
24. El dropdown `Detalle` conserva sus siete acciones existentes y las agrupa, solo en piloto, en información, trazabilidad y documentos mediante contenedores decorativos que no pueden quedar vacíos porque las siete entradas son estáticas. Mide 380 px (mínimo 360 px y máximo disponible en viewport), permite salto de línea y usa iconos semánticamente distintos. Su estado abierto usa borde fino y fondo suave sin halo; el foco de teclado conserva su contorno visible.
25. En la barra contextual del visor, el título y formato son informativos y no reciben interacción. El contenedor de acciones no crece (`flex: 0 0 auto`) y cada enlace de acción ajusta su ancho al contenido; por tanto, hover y foco se limitan a la acción concreta.
26. En escritorio, la acción contextual del visor se ancla a la derecha dentro de un contexto relativo; el título reserva su espacio y toda la franja neutraliza hover. En el breakpoint angosto la acción retorna al flujo estático para permitir wrapping sin superposición.
27. La ruta real es `WebFormInicioDocuarchiGestion → iframe → Webworkflow`. Por ello, tanto el host superior como `Webworkflow.aspx` declaran un `HtmlMeta` de viewport estático, tipado e invisible por defecto. En `PreRender` del host y `Page_Load` del Workbench sus code-behind ajustan directamente `Visible` con el mismo flag y piloto cerrado; así el navegador principal y el iframe usan el ancho CSS real sin bloques ejecutables `<% If %>` ni adiciones a la colección de `head`, que impedirían que AjaxControlToolkit registre sus recursos. La sesión no piloto no emite los metadatos y conserva la línea base.
28. A 767 px o menos, las columnas Bootstrap de `#nav_menu` dejan su base de escritorio y ocupan el 100 % disponible. Los hosts y las acciones existentes envuelven entre líneas; no se crea ni mueve ningún control, por lo que se preservan IDs, permisos y handlers.
29. En el rango intermedio del shell, el ancho disponible del iframe no puede deducirse solo del viewport superior: con sidebar abierto, 1024 px deja aproximadamente 734 px al Workbench y activa prematuramente el breakpoint táctil. `menu-vertical-responsivo.js` reconoce exclusivamente el meta ya emitido para piloto y la ruta activa `workflow/Webworkflow.aspx`; al terminar de cargar ese iframe usa el toggle legacy para iniciar el sidebar cerrado a 1199 px o menos. Fuera de esa combinación conserva el umbral baseline de 992 px. No mueve controles ni cambia rutas, permisos, postbacks o eventos.

## Brecha visual observada en piloto

La primera capa entregó tokens, botones, menús y selección de documentos, pero no alcanzó el nivel de composición del modelo `centro-trabajo-workflow-sin-bandeja.html`. El refinamiento pendiente cubre estos mapeos de presentación:

| Modelo | Contenedor WebForms existente | Tratamiento pendiente |
| --- | --- | --- |
| Barras de acciones | `#menucab`, `#nav_menu` y `#Menutol` | Densidad, fondo, botones, dropdowns y dos franjas visualmente continuas: navegación al inicio, operaciones al final. |
| Barra y lista de documentos | `#div_label`, `#Panel_scroll`, `#GridView_list_documento_relacion_wf` | Cabecera, filas, iconos/acciones y estado seleccionado. |
| Área de trabajo | `#content_selecion_tarea` | Shell con fondo, borde y grid adaptable; sin mover nodos. |
| Visor | `#contenido_imagen`, `#Panel_tolbar_pdf`, `#tollimage`, `#ifrm_visor_` | Toolbar, superficie de visor y separación visual del documento. |
| Índice | `#contenido_indice`, `#title_indice`, `#Panel_indice` | Panel lateral y cabecera cuando el comportamiento legacy lo muestra. |

El pie se resuelve con `#content_pie_seleccion_tarea` y sus labels existentes. El contexto se emite como cromo semántico de servidor únicamente para el piloto; no contiene acciones, controles ni datos de negocio paralelos. El patrón no se replica con botones ficticios.

El reordenamiento de acciones se limita a la presentación del piloto: `Opciones`, `Detalle` y `Servicios` permanecen al inicio de `#menucab`; `Pendientes` queda en su extremo final solo si el servidor o la lógica legacy lo muestra. En `#nav_menu`, `Notas` y `Autorizar` se agrupan como operaciones y `Devolver` seguido de todos los envíos que el servidor haya renderizado se alinea al final. Para el piloto, `#pendiente_selec_tarea` conserva su columna funcional, visibilidad resuelta por legacy y transición operativa `E-ETP`; el adaptador solo encuentra su host visual desde controles hijos estables (por ejemplo, `nota_db`, `pendiente_selec_tarea` y los handlers existentes), no desde el `ClientID` variable de un `Panel`. No crea ni simula acciones.

## Trazabilidad de entregables JIRA-02

| Entregable solicitado | Implementación o documento de referencia | Criterio que fija |
| --- | --- | --- |
| Arquitectura de activación | `03-ServiciosYReglas.md` y `01-ResumenTecnico.md` | Flag, piloto de servidor, punto de emisión, subcapas y rollback. |
| Contrato CSS | `02-ImpactoUI.md` y `CONTRATO-CSS-COMPONENTES-REUTILIZABLES.md` | Tokens, componentes scoped, breakpoint, estados y `z-index`. |
| Cutover de capas previas | `04-ContratosIntegracion.md` | Inventario baseline, orden de carga y prohibición de una segunda ruta visual. |
| Pruebas de activación | `05-PruebasEvidencia.md` | Flag, piloto, rollback, caché, navegación interna y evidencia por SHA. |

El contrato CSS fuente se conserva en `Doc/Diagnostico/planes-migracion/modernizacion-menu-botones-inicio-workflow/tickets-jira-centro-trabajo-workflow/CONTRATO-CSS-COMPONENTES-REUTILIZABLES.md`. La hoja DOC-2 debe reproducir ese contrato sobre controles existentes; no crea una interpretación alternativa ni markup paralelo.

## Matriz de aceptación operativa

| Condición | HTML/recursos esperados | Efecto permitido |
| --- | --- | --- |
| Flag `false` | No clase raíz ni CSS/JS DOC-2 | Línea base manual idéntica. |
| Flag `true`, usuario fuera de piloto | No clase raíz ni CSS/JS DOC-2 | Línea base manual idéntica. |
| Flag `true`, piloto con todas las capas | Clase raíz y las cuatro subcapas; recursos versionados después de legacy | Solo presentación scoped, incluyendo shell, documentos, visor e índice existentes. |
| Retiro de `actions`, `documents` o `a11y` | Hoja permanece, clase de la subcapa no se emite | Reversión de esa presentación, sin eventos ni datos. |
| Retiro de `layout` o flag `false` | Ninguna subcapa o ningún recurso DOC-2 | Rollback total sin redeploy de lógica. |

## Risks / Trade-offs

- ASP.NET relee `appSettings` con el reciclado normal de aplicación; no hay redeploy de lógica de negocio, datos ni eventos.
- Un piloto vacío bloquea deliberadamente la capa aunque el flag maestro sea verdadero.
- La base manual ya tiene adaptadores que cambian clases y reubican acciones. DOC-2 no los ejecuta por segunda vez; solo añade clases `ctw-*` para el piloto. Cualquier migración futura de la base a CSS scoped se hará en un ticket dedicado para no alterar la apariencia aprobada.

## Migration Plan

1. Publicar los recursos DOC-2 con flag maestro `false` y lista de pilotos vacía.
2. Comprobar que una sesión normal conserva los recursos y la apariencia baseline, pero no entrega clase ni recursos DOC-2.
3. Configurar temporalmente flag `true` y un login de gestión controlado; comprobar clase raíz, recursos y capas en HTML.
4. Retirar `actions`, `documents` o `a11y` de la configuración y comprobar rollback parcial. Para rollback total, restaurar el flag en `false`.

## Open Questions

- La evidencia visual requiere ambiente, cuenta piloto y datos de Workflow controlados. La URL de Gestión fue informada, pero el acceso del runner sigue bloqueado antes de HTTP por credenciales TLS.
