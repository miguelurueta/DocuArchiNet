# DOC-2 — Contrato CSS e impacto visual

## Encapsulamiento y tokens

Toda regla comienza con `.workflow-centro-trabajo-moderno`. Los tokens son `--ctw-navy`, `--ctw-blue`, `--ctw-ink`, `--ctw-muted`, `--ctw-line`, `--ctw-pale`, `--ctw-bg`, `--ctw-danger`, `--ctw-warning`, `--ctw-radius` y `--ctw-control-height`.

Componentes entregados: `.ctw-btn`, `.ctw-btn--primary`, `.ctw-btn--danger`, `.ctw-icon-btn`, `.ctw-menu`, `.ctw-menu__trigger`, `.ctw-menu__panel`, `.ctw-menu__item`, `.ctw-badge`, `.ctw-action-bar`, `.ctw-document-bar`, `.ctw-panel`, `.ctw-pane-head` y `.ctw-document-row--selected`.

## Capas y alcance

- `ctw-layer-layout`: panel y jerarquía base sin cambiar tamaño, posición o visibilidad.
- `ctw-layer-actions`: plano de comandos formado por `#menucab`, `#nav_menu` y sus dropdowns existentes.
- `ctw-layer-documents`: cabecera y fila activa de documentos relacionados.
- `ctw-layer-a11y`: foco visible, estado deshabilitado y botón de icono de 40px en móvil.

Los popovers usan `z-index: 1100`, por encima del contenido local sin alterar visor o modales legacy. El breakpoint es `767px`; ningún selector habilita una acción que el servidor haya ocultado.

## Segundo corte de fidelidad del modelo

La capa `layout` aplica el shell visual sobre `#content_selecion_tarea` y sus contenedores ya existentes: documentos, visor e índice. La capa `actions` completa densidad, fondo, separadores y dropdowns de `#menucab`; `documents` completa cabecera, toolbar y filas de documentos. En 900 px o menos los mismos contenedores pasan a una sola columna sin crear, mover, ocultar o sustituir nodos WebForms. La ruta de pantalla usa el iframe `WebFormInicioDocuarchiGestion → Webworkflow`: para que ese breakpoint se evalúe contra el ancho real, ambos extremos declaran un `HtmlMeta` tipado e invisible y lo hacen visible solo para el piloto DOC-2 —el host en `PreRender` y el Workbench al inicio de `Page_Load`. La cabecera baseline no se modifica.

A 767 px o menos, los dos grupos Bootstrap existentes de `#nav_menu` pasan a ancho completo y sus acciones se envuelven entre líneas. El ajuste solo reordena el flujo visual dentro de sus propios hosts: no reemplaza nodos ni cambia ID, handler, permiso o objetivo táctil.

En piloto, `#content_pie_seleccion_tarea` se reutiliza como la única franja de contexto antes de documentos, visor e índice. Conserva `Label_estado_tarea_selecion` para `Radicado · Solicitante` y `Label_estado_selecion` para `Flujo|Ruta · nombre`; dos labels decorativos de solo lectura muestran título del trámite y estado. No añade consulta, estado de negocio, botones ni postbacks, ni duplica el contexto en otra región. El título se transforma visualmente a mayúscula sin modificar el valor fuente. Con modo apagado conserva su posición y presentación baseline.

## Reordenamiento visual de comandos

La capa `actions` trata `#menucab` y `#nav_menu` como dos franjas visualmente continuas y compactas del mismo plano de comandos. `Opciones`, `Detalle` y `Servicios` permanecen al inicio de la primera; `Pendientes` se alinea al final solo cuando existe. En la segunda, `Notas`, el estado `Autorizada` y `Historial` forman el grupo operativo; `Devolver` seguido de los envíos que el servidor haya renderizado se alinean al final. El control real `#pendiente_selec_tarea` conserva en piloto la transición legacy `E-ETP` y la etiqueta resuelta por esa lógica (`Cerrar tarea` o `Enviar a pendientes`); no se sustituye por una vuelta local. Los envíos de flujo o gestión reciben énfasis primario.

El adaptador añade únicamente clases `ctw-*` a elementos que ya existen dentro de `#div_content_general_wf`, incluso tras `UpdatePanel.endRequest`. Resuelve cada host desde controles hijos estables —Notas, Autorizar, Pendientes, Devolver, `pendiente_selec_tarea` y los handlers de envío— y no depende del ClientID que ASP.NET genere para un `Panel`. Los iconos ya presentes, incluso los que el markup legacy mantenía ocultos, se muestran y alinean solo en la subcapa `actions`. No crea, mueve, oculta ni activa controles. El helper baseline de título de documentos conserva fuera del piloto su reubicación aprobada; en piloto mantiene los enlaces rápidos existentes sin reparentarlos.

## Barra contextual del documento seleccionado

En piloto, `#div_label` conserva la cabecera `Documentos` en el panel lateral. El visor `#contenido_imagen` ocupa la columna central y su toolbar `#Panel_tolbar_pdf` queda sobre el documento abierto. Los accesos rápidos de la cabecera lateral se ocultan solo visualmente para evitar duplicados; no se reubican nodos ni IDs.

La barra sobre el visor muestra el título y formato de la selección actual. `Cargar` usa el handler existente de adjunto y `Metadatos` reutiliza `#id_indice_wf_pdf` solo cuando el visor PDF ya lo hizo disponible. `Versiones` y el menú `Más acciones` conservan `tip_event`, `id_wf` e `idd_wf` emitidos desde la selección actual y siguen entrando por `prevent`; no hay handler, permiso, postback ni clic artificial adicional.

## Evidencia visual pendiente

QA debe comparar modo apagado/encendido en 1366, 1024, 768 y 375 px, incluyendo hover, foco, deshabilitado, menú abierto y documento seleccionado. Requiere ambiente, piloto y datos de Workflow controlados.
