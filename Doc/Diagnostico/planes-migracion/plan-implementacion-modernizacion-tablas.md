# Plan y registro de implementación: modernización gradual de tablas GridView

## Estado de referencia

**Fecha de actualización:** 2026-08-07  
**Repositorio fuente:** `D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net`  
**Precompilado de validación:** `D:\temfile\Gestion`  
**Tabla piloto de Fase 2:** `workflow/Webworkflow.aspx` → `GridView2` (lista de tareas Workflow).

Este documento refleja el contenido validado en `D:\temfile\Gestion` y, desde esta fecha, también incorporado en el repositorio local. La publicación en IIS se hace solo después de validar el precompilado.

## Objetivo y límites inalterables

Modernizar visualmente los GridView Web Forms conservando el comportamiento emitido por servidor.

- No se modifican archivos `.vb`, DLL, consultas, handlers, procedimientos, `GridView`, `BoundField`, orden de columnas, datos ni IDs.
- No se modifica `__doPostBack`, los enlaces de ordenamiento, paginación, eventos `onclick` ni `UpdatePanel`.
- No se reordenan, ocultan, renombran ni interpretan las columnas generadas por backend.
- Los cambios se limitan a CSS, JavaScript de presentación y referencias de recursos en páginas `.aspx`.
- Cada publicación se hace con copia de respaldo fechada del archivo concreto.

## Inventario publicado en `D:\temfile\Gestion`

| Archivo | Estado | Responsabilidad |
|---|---|---|
| `Styles\Aplicaction.css` | Fase 1 implementada globalmente | Normaliza la tabla interna de `pagination-ys` sin cambiar su HTML ni postbacks. |
| `Styles\gridview-moderno.css` | Fase 2 disponible, opt-in | Capa genérica reutilizable que solo actúa sobre tablas con clase `gridview-moderno`. |
| `Styles\workflow-tareas-modernas.css` | Fase 3 implementada | Capa local, exclusiva de la bandeja `#GridView2` y sus controles asociados. |
| `js\workflow\workflow-paginacion-visual.js` | Complemento local activo | Reaplica la clase visual y normaliza el paginador de `GridView2` después de actualizaciones parciales ASP.NET. |
| `workflow\Webworkflow.aspx` | Página piloto Fase 2 y Fase 3 | Carga los recursos y declara la clase opt-in en la lista de tareas. |

### Referencias y versiones vigentes en `workflow\Webworkflow.aspx`

```html
<link href="../Styles/gridview-moderno.css?v=20260807-phase2-5" rel="stylesheet" />
<link href="../Styles/workflow-tareas-modernas.css?v=20260807-grid33" rel="stylesheet" />
<script src="../js/workflow/workflow-paginacion-visual.js?v=20260807-pager6" type="text/javascript"></script>
```

La tabla piloto mantiene su definición de servidor y añade únicamente la clase de presentación:

```aspx
<asp:GridView ID="GridView2"
    class="table font-weight-light ml-1 gridview-moderno"
    PagerSettings-Position="Top" AllowSorting="true" AllowPaging="true"
    PageSize="7" runat="server" ...>
```

El orden de carga es intencional: la capa genérica se carga primero y la capa específica de Workflow después. Así, el estilo local aprobado de la bandeja tiene prioridad sin afectar otras tablas.

## Fase 1 — Normalización global del paginador

**Estado:** implementada en `D:\temfile\Gestion`.

### Alcance real

Archivo: `Styles\Aplicaction.css`.

El selector global se limita a `.pagination-ys table > tbody > tr > td` y sus controles `a` y `span`. Conserva la tabla interna que ASP.NET genera para el paginador; no la reemplaza ni la mueve.

### Comportamiento aplicado

- Las celdas internas quedan como `table-cell` de `38px` de ancho, con alineación centrada vertical y horizontalmente.
- Los enlaces y la página activa usan la misma caja visual de `38px × 34px` y no conservan `float` ni márgenes residuales.
- Se mantiene la página activa como `span` y las demás como enlaces `a`; por tanto, los postbacks nativos siguen intactos.
- Se mantienen bordes redondeados en los extremos del grupo de páginas.
- Los enlaces de página usan el color de lectura `#304eac`; la página activa conserva el estilo de control activo.

### Exclusiones

- No se cambia el `PagerStyle`, `PagerSettings`, el número de páginas, los eventos ni el HTML anidado del GridView.
- No se aplica ninguna regla de encabezado global en esta fase.

### Validación requerida al extender cambios globales

Probar primera, intermedia y última página en Workflow, Radicación y Gestión; confirmar que no se fragmenten los `<td>` internos después de un postback y que todos los enlaces sigan generando `__doPostBack`.

## Fase 2 — Clase visual reutilizable y opt-in

**Estado:** implementada y en validación únicamente sobre la lista de tareas Workflow (`GridView2`). No está habilitada en las demás tablas.

### Hoja genérica

Archivo: `Styles\gridview-moderno.css`.

La hoja no usa IDs, nombres de columnas, `nth-child`, texto de encabezados ni eventos. Solo se activa mediante la clase explícita `gridview-moderno`.

### Estilos genéricos vigentes

- Tipografía: `Segoe UI, Arial, sans-serif`.
- Encabezado `.GridviewScrollHeader_line_boot th`: fijo dentro de su contenedor de desplazamiento, fondo `#eef1fb`, texto `#35477f`, borde inferior `#dce2f1`, 11px, mayúsculas, espaciado `.055em` y peso `700`.
- Enlaces de encabezado: mismo color, fondo transparente y sin subrayado; el ordenamiento de ASP.NET se conserva.
- Celdas de datos: espaciado `11px 14px` y divisor inferior `#e7eaf0`.
- Filas alternas y hover: `#f8f9fe` y `#f1f4ff` respectivamente.

### Regla reutilizable implementada — estado de revisión por peso tipográfico

El backend de la lista de tareas ya emite el estado visual mediante clases en la fila: `font-weight-bold` para una tarea no revisada y `font-weight-light` para una tarea revisada. Esta semántica no debe calcularse ni persistirse con JavaScript.

La modernización local de Workflow forzaba `font-weight: 400 !important` en las celdas y ocultaba esa señal. La regla se añadió a `Styles\gridview-moderno.css` y la capa local de Workflow deja de anularla:

```css
.gridview-moderno {
  --gv-peso-no-revisada: 700;
  --gv-peso-revisada: 300;
}

.gridview-moderno tr.font-weight-bold > td {
  font-weight: var(--gv-peso-no-revisada) !important;
}

.gridview-moderno tr.font-weight-light > td {
  font-weight: var(--gv-peso-revisada) !important;
}
```

- Los valores se pueden ajustar por tabla redefiniendo las dos variables CSS, sin alterar backend.
- La regla solo actúa sobre filas dentro de una tabla con `gridview-moderno`; no modifica los usos generales de Bootstrap para `font-weight-bold` o `font-weight-light`.
- En `workflow-tareas-modernas.css` se retiró el `font-weight: 400 !important` y se añadió un adaptador limitado a `#GridView2` para que las reglas cromáticas por columna no prevalezcan sobre el estado de revisión.
- Antes de publicar, validar una tarea con estado `0` y otra con estado distinto de `0`, incluida una actualización parcial del `UpdatePanel`.

### Decisión de diseño vigente

La propuesta inicial contemplaba texto de encabezado `#101010`. El color validado e implementado es `#35477f`, coherente con la maqueta aprobada y la bandeja Workflow. Cualquier nueva adopción debe usar el valor implementado, no el valor histórico de la propuesta.

### Procedimiento obligatorio para una nueva tabla

1. Identificar el GridView y confirmar que usa `GridviewScrollHeader_line_boot`; excluir variantes `_none`, `_gren`, `_gray` y `_black` salvo aprobación explícita.
2. Cargar `../Styles/gridview-moderno.css` en la página, con una versión nueva solo si cambia el CSS.
3. Añadir `gridview-moderno` a la clase existente del GridView sin eliminar sus clases actuales.
4. No añadir estilos por posición de columna ni JavaScript específico en esta fase.
5. Validar vista con datos, vacía, texto largo, ordenamiento y paginación antes de llevar la misma página a IIS.

### Alcance actual y exclusiones

- La adopción persistente actual es solo `GridView2` de `workflow/Webworkflow.aspx`.
- Las demás GridView de la misma página —y las otras páginas Workflow, Radicación, Gestión, Público y demás módulos— no han sido aprobadas para Fase 2.
- Hay múltiples páginas que reutilizan `GridviewScrollHeader_line_boot`; no se debe cambiar esa clase global para intentar modernizarlas en bloque.

## Fase 3 — Bandeja de tareas Workflow

**Estado:** implementada en `D:\temfile\Gestion` y limitada a `workflow/Webworkflow.aspx`.

### Archivo local

`Styles\workflow-tareas-modernas.css` usa selectores de `#GridView2`, `#Panelactividad` y de los controles propios de la bandeja. No afecta otros GridView.

### Cambios locales implementados

- `#Panelactividad`: contenedor blanco, borde tenue, radio de 10px, sombra sutil y barras de desplazamiento visualmente integradas.
- `#GridView2`: tipografía Segoe UI, bordes separados, filas con mayor legibilidad, truncamiento de texto sin modificar el contenido y resaltado hover.
- Encabezado: fondo `#eef1fb`, texto `#35477f`, borde `#dce2f1`, mayúsculas, 11px y peso `700`.
- Primera columna de acciones: se mantiene fija durante el desplazamiento horizontal; no se reducen ni reordenan columnas.
- Botones de acciones: 34px, circulares, con foco visible y hover; se conservan los atributos, iconos y eventos originales.
- Jerarquía cromática local: se aplica exclusivamente a posiciones existentes de `GridView2`; no cambia valores ni nombres de columnas.
- Paginador nativo de la bandeja: compatibilidad local adicional para sus celdas y controles internos después de renderizados parciales.
- Buscador: se modernizan `#auto_complex`, el botón de restaurar `#td-boton` y el botón de consulta, sin cambiar autocompletado ni eventos.
- Filtros: se modernizan `#DropDownListseleccionfiltro`, refrescar y búsqueda avanzada. `#LabelEspera` permanece dentro de `UpdatePanelnumeroespera`; solo recibe estilo visual y el rótulo CSS `Total registros:`. No se mueve, no se reescribe y continúa recibiendo su valor desde backend.
- Se retiró el indicador decorativo de dirección de ordenamiento. Los enlaces de ordenamiento y su postback permanecen disponibles.

### JavaScript local

Archivo: `js\workflow\workflow-paginacion-visual.js`.

- Identifica exclusivamente `GridView2` y le garantiza la clase `gridview-moderno`.
- Normaliza, mediante estilos en línea limitados a la tabla interna del paginador de `GridView2`, las celdas y controles de página que ASP.NET vuelve a renderizar.
- Se ejecuta al cargar el documento y se registra en `Sys.WebForms.PageRequestManager.endRequest`, por lo que reaplica la presentación después de postbacks parciales.
- No altera texto, filas, columnas, contador, eventos, atributos `idd`, `tip_event` ni el resultado de backend.

## Fase 4 — Publicación y control de cambios

**Estado:** procedimiento activo.

1. Modificar primero el archivo fuente dentro del repositorio local.
2. Respaldar exclusivamente los archivos que se publicarán en `D:\temfile\Gestion`.
3. Copiar al precompilado solo CSS, JS o `.aspx` aprobados; nunca carpetas completas.
4. Cambiar el parámetro de versión del recurso únicamente si su contenido cambió.
5. Validar funcional y visualmente en el precompilado.
6. Respaldar y sincronizar los mismos archivos aprobados a `C:\inetpub\GestorII`.
7. Comprobar por HTTP los recursos versionados servidos por IIS.

### Respaldos existentes en el precompilado

Los respaldos están junto al archivo intervenido y siguen el patrón:

```text
<archivo>.<yyyyMMdd-HHmmss>.<motivo>.bak
```

Ejemplos existentes:

- `workflow-tareas-modernas.css.20260807-015220.etiqueta-contador.bak`
- `workflow-tareas-modernas.css.20260807-014851.contador-alineado.bak`
- `workflow-tareas-modernas.css.20260807-013908.reversion-contador.bak`
- `gridview-moderno.css.20260807-070840.fase2-gridview2.bak`
- `workflow-tareas-modernas.css.20260807-070840.fase2-gridview2.bak`

La reversión se realiza restaurando el respaldo fechado del archivo específico y devolviendo el parámetro de versión de la página a la versión correspondiente. No usar restauraciones masivas del repositorio ni de la carpeta de publicación.

## Matriz de validación por tabla candidata

| Caso | Resultado esperado |
|---|---|
| Carga inicial | Se ven todas las columnas y datos emitidos por backend. |
| Ordenamiento | Los enlaces del encabezado siguen ejecutando `__doPostBack`. |
| Paginación | Página activa y enlaces quedan alineados; no aparecen `<td>` fuera de la fila tras postback. |
| Texto largo | Se conserva el dato; la visualización no desborda ni altera columnas. |
| Vista vacía | No se rompe el encabezado ni el contenedor. |
| Scroll horizontal | En Workflow, las acciones quedan visibles; en otras tablas no se agrega fijación de columnas por defecto. |
| Actualización parcial | Se mantienen estilos y acciones; no se pierden enlaces ni handlers. |
| Caché | La URL del CSS/JS muestra la versión publicada esperada. |

## Criterios de aceptación para extender Fase 2

- La tabla candidata adopta explícitamente `gridview-moderno`.
- No se modifica ninguna clase global de encabezado para obtener el nuevo diseño.
- No cambian columnas, datos, `CssClass` existentes, IDs, eventos ni postbacks.
- La paginación nativa conserva su tabla interna y sus enlaces.
- La página está validada en `D:\temfile\Gestion` antes de copiarla a IIS.
- Existe respaldo fechado de cada archivo publicado.

## Riesgos y mitigaciones

| Riesgo | Mitigación |
|---|---|
| Una regla global modifica módulos no revisados | Fase 2 es estrictamente opt-in mediante `gridview-moderno`. |
| La página contiene varios GridView | Agregar la clase solo al GridView aprobado, no al contenedor general. |
| ASP.NET vuelve a generar el paginador | Fase 1 conserva su HTML; el JS local de Workflow reaplica únicamente el formato de `GridView2`. |
| Caché de navegador o IIS | Versionar solo recursos modificados y comprobar la URL servida. |
| Diferencia entre repositorio y precompilado | Comparar y publicar archivos puntuales; mantener este documento como inventario de `D:\temfile\Gestion`. |
| Reversión amplia elimina cambios ajenos | Restaurar exclusivamente el `.bak` fechado del archivo intervenido. |
