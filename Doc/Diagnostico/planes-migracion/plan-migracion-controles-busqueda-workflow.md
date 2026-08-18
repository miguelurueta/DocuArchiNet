# Plan de migración: buscador, filtros y contador de tareas Workflow

**Estado:** implementado en el repositorio local el 2026-08-07. La hoja `Styles\workflow-tareas-modernas.css` se incorporó conservando el contenido aprobado; la página ya la carga con la versión `20260807-grid33`.

## Objetivo

Migrar al repositorio local la modernización visual de los controles de búsqueda, filtro, actualización, búsqueda avanzada y conteo de la bandeja de tareas Workflow.

La implementación es exclusivamente de presentación. No modifica consultas, code-behind, `UpdatePanel`, eventos, postbacks ni datos emitidos por backend.

## Origen y destino

```text
Origen validado: D:\temfile\Gestion
Repositorio destino: D:\imagenesda\DocuachiNet\DocuArchiNet
Página: workflow\Webworkflow.aspx
Estilo local: Styles\workflow-tareas-modernas.css
```

## Controles incluidos

| Control | Función existente | Cambio visual |
|---|---|---|
| `auto_complex` | Campo de búsqueda de tareas | Alto, tipografía, borde y foco. |
| `td-boton` | Restaurar la lista | Botón compacto secundario. |
| Botón `preven_event_search` | Consultar lista | Botón de búsqueda azul compacto. |
| `DropDownListseleccionfiltro` | Filtrar tareas, `AutoPostBack=true` | Selector moderno y foco visible. |
| Botón `preven_event_search_new_task` | Buscar/actualizar tareas | Acción primaria compacta. |
| `bnt_search_avance` | Abrir búsqueda avanzada | Acción secundaria compacta. |
| `LabelEspera` | Conteo emitido por backend | Alineación, color y etiqueta visual. |
| `UpdatePanelnumeroespera` | Actualización parcial del contador | Alineación y separación visual; no se mueve. |

## Archivos a modificar

```text
Styles\workflow-tareas-modernas.css
workflow\Webworkflow.aspx   (solo si aún no carga la hoja CSS versionada)
```

No se requiere modificar JavaScript ni archivos `.vb` para esta implementación.

## Dependencias que deben conservarse

Los siguientes elementos ya existen y no deben cambiarse:

```text
onclick="preven_event_restor_search(event,this)"
onclick="preven_event_search(event,this)"
onclick="preven_event_search_new_task(event,this)"
onclick="event_element_clic(event,this)"
AutoPostBack="true" en DropDownListseleccionfiltro
UpdatePanelnumeroespera
UpdatePanelseleccionfiltro
LabelEspera
```

El `UpdatePanel` debe permanecer en su posición original. El valor de `LabelEspera` lo entrega el backend y no debe ser reformateado ni reescrito con JavaScript.

## Implementación detallada

### 1. Confirmar la carga de la hoja local

En `workflow\Webworkflow.aspx` debe existir una referencia posterior a `gridview-moderno.css`:

```html
<link href="../Styles/workflow-tareas-modernas.css?v=20260807-grid33" rel="stylesheet" />
```

Si el contenido de `workflow-tareas-modernas.css` cambia durante la migración, actualizar únicamente el parámetro `v` con una nueva versión. No duplicar referencias al mismo CSS.

### 2. Campo y acciones de búsqueda

Aplicar reglas exclusivas a:

```css
#auto_complex
#auto_complex::placeholder
#auto_complex:focus
#td-boton
#auto_complex + .input-group-append .btn
```

Resultado esperado:

- Campo de `36px` de alto, fuente Segoe UI y color legible.
- Foco con borde `#8da4df` y halo tenue.
- Restaurar: apariencia secundaria, fondo claro y radio izquierdo de `8px`.
- Consultar: apariencia primaria `#4057a8`, radio derecho de `8px`.
- Ambos botones conservan sus iconos, título y eventos existentes.

### 3. Filtro y botones de tareas

Aplicar reglas a:

```css
#UpdatePanelseleccionfiltro
#DropDownListseleccionfiltro
#DropDownListseleccionfiltro:focus
button[onclick*="preven_event_search_new_task"]
#bnt_search_avance
```

Resultado esperado:

- Selector de 36px de alto, ancho mínimo 178px, tipografía 13px y borde `#d5ddea`.
- Botón de actualizar: 36×36px, azul `#4057a8`; hover/foco `#304583`.
- Búsqueda avanzada: 36×36px, fondo claro; hover/foco azul tenue.
- Separación de 12px entre el filtro y su contenedor, sin cambiar columnas Bootstrap existentes.

El selector basado en `onclick*="preven_event_search_new_task"` debe validarse antes de publicar: hoy identifica un único botón de esta bandeja. Si en el futuro aparece otro botón con la misma función dentro de la página, sustituirlo por una clase específica sin alterar el evento.

### 4. Contador de registros

Aplicar reglas a:

```css
#UpdatePanelnumeroespera
#LabelEspera
#LabelEspera::before
```

Resultado esperado:

- `UpdatePanelnumeroespera`: `display:flex`, centrado vertical, mínimo 36px y separación izquierda de 20px.
- `LabelEspera`: conserva su texto backend, usa `#304eac`, Segoe UI, 15px y peso 700.
- `#LabelEspera::before`: añade visualmente `Total registros:` en 12px, color `#71819a` y peso 600.

No usar JavaScript para mover, envolver, modificar, formatear o eliminar el contenido del contador. El pseudo-elemento CSS debe ser el único origen de la etiqueta adicional.

## Validación funcional

1. Cargar la lista de tareas y verificar que aparece el campo de búsqueda, filtro, contador y dos botones de acción.
2. Buscar por texto con el botón y con la tecla Enter, si el comportamiento existe en la página.
3. Restaurar la lista y comprobar que ejecuta `preven_event_restor_search`.
4. Cambiar el filtro; confirmar que `AutoPostBack` sigue actualizando la lista.
5. Usar “Buscar nuevas tareas” y confirmar que conserva `preven_event_search_new_task`.
6. Abrir búsqueda avanzada y confirmar el evento `event_element_clic`.
7. Completar, asignar o actualizar una tarea; confirmar que `LabelEspera` recibe el nuevo valor desde backend y sigue dentro de su `UpdatePanel`.
8. Revisar consola: no debe haber errores JavaScript ni recursos CSS 404.

## Validación visual

- Campo, selector y botones comparten altura visual de 36px.
- El contador se alinea verticalmente con el selector, queda separado del borde izquierdo y no invade el combo.
- El texto `Total registros:` acompaña al conteo, sin duplicar ni ocultar el valor real.
- Estados hover y focus son visibles con teclado.
- En pantallas reducidas no se desbordan los controles ni cambian sus eventos.

## Exclusiones

- No modificar `LabelConteo`, controles de visor, buscadores de pendientes, actividades o usuarios.
- No cambiar los `div` de columnas (`col3`, `col6`, `col-3`) ni su estructura.
- No modificar `UpdatePanelnumeroespera`, `UpdatePanelseleccionfiltro` ni el code-behind.
- No incluir reglas genéricas de `input`, `button` o `.btn` que afecten otras vistas.

## Reversión

1. Restaurar únicamente el bloque de controles de búsqueda y filtro en `workflow-tareas-modernas.css` desde el commit o respaldo previo.
2. Si se añadió o cambió la referencia CSS, devolver solo su parámetro de versión anterior.
3. No retirar la hoja completa si contiene estilos aprobados de tabla, paginador o panel Workflow.

## Criterios de cierre

- Los cinco eventos de búsqueda/filtro siguen funcionando.
- El contador continúa siendo actualizado por backend dentro del mismo `UpdatePanel`.
- No se modificaron archivos `.vb`, handlers o estructura de controles.
- La presentación coincide con `D:\temfile\Gestion`.
- Existe commit o respaldo que permite revertir solo este paquete.
