# DOC-110 — Revisión visual de la administración

Fecha: 2026-10-23. Solicitud: usar la interfaz moderna de SII y el HTML previamente explorado como base para mejorar botones, tablas y formularios. Trazabilidad D-09/RQ-12, tareas 15.22–15.24.

## Referencias revisadas

- `Styles/importar-servicio-web-modern.css`: cabecera, cuerpo con desplazamiento interno, tablas, jerarquía de acciones y adaptación a pantalla estrecha.
- `js/workflow/importar-servicio-web/importar-servicio-web-ui.js`: referencia de interacción; sin modificar ni importar el flujo de SII.
- `Prototipo/importacion-carpetas.html` y su CSS: tarjetas, contexto, ayudas, tipografía y separación de operaciones. Es un prototipo del flujo de importación; sus etapas de transferencia y ejecución no son capacidades del administrador de perfiles.
- Editores productivos `PerfilImportacionEditor.js`, `FuenteAdminEditor.js` y `ProveedorAdminEditor.js`.

## Hallazgos y corrección

| Zona | Hallazgo | Implementación |
|---|---|---|
| Ventana | Cabecera y avisos se perdían al desplazarse; cerrar quedaba lejos | Cabecera y estado fuera del cuerpo desplazable, cierre visible, fondo atenuado |
| Listado de proveedores | Párrafos que mezclaban identidad, versión, disponibilidad y acción | Tabla con seis columnas, etiquetas de estado, búsqueda y alta agrupadas, conteo de página |
| Listado de fuentes | Texto corrido por registro | Tabla con conexión, tabla, estado, revisión y acción; estado vacío explícito |
| Fuente | No distinguía autorización de búsqueda | Contexto fuente → proveedor → perfil y aclaración de dónde se define la clave |
| Formulario de fuente | Controles y columnas sin agrupación | Secciones de origen/aislamiento, columnas y justificación/disponibilidad; selección en cuadrícula |
| Historial de fuente | JSON expuesto como presentación principal | Tabla de revisión, acción, usuario, fecha y motivo; detalle expandible con datos de configuración |
| Proveedor | Formulario vertical y propiedades repetidas sin estructura | Origen/búsqueda en cuadrícula y propiedades en tabla editable con acción por fila |
| Ensayo | Acción mezclada con publicación y disponibilidad | Tarjeta de prueba con clave, botón y resultado; conserva la explicación de ambigüedad |
| Acciones | Guardar, publicar, desactivar y navegar tenían igual presentación | Guardar/probar como acciones primarias; publicación diferenciada; desactivar/revocar con estilo de advertencia; navegación separada |
| Perfiles | Presentación distinta a fuentes/proveedores | Cabecera y estilos locales unificados; conserva formularios y reglas existentes |
| Pantallas pequeñas | Campos extensos y tablas ensanchaban la vista | Campos en una columna, tablas con desplazamiento propio y cierre visible |

## Límites y preservación funcional

Los cambios son de presentación y composición DOM. Se conservan las operaciones ASMX, los payloads, los permisos independientes, el control de revisiones, el bloqueo de escrituras inciertas, la protección de cambios pendientes y las versiones publicadas de solo lectura. No se cambia la política de coincidencias ni se selecciona una fila arbitraria.

CSS limitado a `#doc110-perfiles`. No se cambia SII, no se añade un framework ni se necesita SQL. No se despliega al servidor principal.

## Validación

- 18 pruebas Node del editor de perfiles, proveedores y fuentes: PASS.
- Recorridos Playwright sobre componentes reales con transporte simulado: administración de fuentes, recuperación de error, perfil guiado, proveedor completo, autocomplete y revisión adaptable.
- Revisión adaptable a 1366×900 y 390×844: cabecera y aviso visibles, tablas semánticas, seis propiedades, ausencia de desbordamiento del cuerpo, cierre por Escape y restauración del foco. Capturas en `Validacion/Ui/`.
- El recorrido simulado no acredita respuesta ASMX, estilos heredados de la página completa ni funcionamiento en una sesión IIS real. La aceptación integrada permanece pendiente.

## Correccion del punto de entrada - 2026-10-24
La captura real identifica tres botones por encima de la barra, alterando su distribucion. Tarea 15.25: accesos trasladados al nuevo desplegable Importacion dentro de #menu_var .navbar-nav; los dialogos permanecen en #doc110-perfiles. Se conservan los permisos por opcion y no hay fila adicional. Desplegable nativo details/summary con teclado, cierre exterior y restauracion de foco. Sin barra disponible (fixtures aislados), conserva los accesos de componente. Pruebas Chromium: 3 PASS con respuestas simuladas; pendiente revision en pagina IIS completa.

### Uniformidad del menu - video 115742
La implementacion 15.25 resolvia la posicion pero usaba un desplegable nativo con estilos propios. Se sustituye por el dropdown Bootstrap ya utilizado en la barra, incluidos clases, colores e iconos. Se elimina el CSS de panel alternativo y se versionan las dos referencias de recursos para evitar cache anterior. Pruebas 15.26: 2 Chromium PASS con Bootstrap del repo y transporte simulado; aceptacion visual IIS pendiente.
