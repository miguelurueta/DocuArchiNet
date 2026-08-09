# Plan de actualización: documentos relacionados de Workflow

**Fecha:** 2026-08-08  
**Componente:** `GridView_list_documento_relacion_wf`  
**Página objetivo:** bandeja Workflow que emite la lista de documentos relacionados.  
**Alcance:** presentación HTML/CSS/JavaScript; no modifica el contrato del backend Web Forms.

**Estado de implementación local:** Fase 1 y Fase 2 implementadas el 2026-08-08. Fase 0 continúa pendiente de diagnóstico en navegador con la respuesta real de `UpdatePanel`.

| Recurso implementado | Ubicación | Responsabilidad |
|---|---|---|
| Hoja visual | `Styles/workflow-documentos-relacionados-modernos.css` | Presentación confinada a la tabla de documentos relacionados. |
| Adaptador de presentación | `js/workflow/documentos-relacionados-visual.js` | Clase visual, rótulos accesibles y reaplicación tras postback parcial. |
| Referencias | `workflow/Webworkflow.aspx` | Carga versionada `20260808-docrel1`. |

## Objetivo

Convertir la lista de documentos relacionados en una tabla compacta, legible y accesible, manteniendo íntegros sus IDs, atributos `id_wf`, `idd_wf`, `tip_event`, checkboxes, eventos `onclick`, acciones y postbacks existentes.

La maqueta de referencia está en [emulacion-modernizacion-documentos-relacionados-workflow.html](emulacion-modernizacion-documentos-relacionados-workflow.html).

## Hallazgos quirúrgicos

1. El HTML capturado contiene la tabla completa dos veces con el mismo ID `GridView_list_documento_relacion_wf` y las mismas filas (`9424` a `9434`). Es un defecto crítico de ciclo de renderizado, no de estilo. No se debe ocultar con CSS: duplicaría IDs, controles y eventos.
2. La tabla tiene una columna funcional de selección y una de documento. Cada fila conserva cinco acciones: ver, eliminar, cambiar tipología, firma digital, versiones y reemplazo.
3. La fila ya emite los datos funcionales necesarios. La modernización no requiere consultar ni recalcular información.
4. Existen etiquetas inválidas `<spam>` y atributos `style` repetidos. Deben corregirse en el generador cuando exista autorización de backend; no se intentará repararlos mediante reemplazos globales de JavaScript.
5. El disparador del menú desplegable no tiene texto ni nombre accesible. Puede recibir `aria-label`, `title` y foco visible desde JavaScript, sin cambiar su evento Bootstrap.

## Límites inalterables

- No tocar `.vb`, servicios, consultas, DLL, handlers ni controles `GridView` del servidor.
- No modificar `id_wf`, `idd_wf`, `tip_event`, `onclick`, IDs generados, `href`, `__doPostBack` ni el orden de las acciones.
- No ocultar ni eliminar registros para resolver la duplicación; primero se identifica su productor.
- No aplicar reglas globales a `.GridviewRow`, `.dropdown-menu` o `.Gridviewtable`.
- La adopción será exclusiva de `#GridView_list_documento_relacion_wf` dentro de la página aprobada.

## Diseño objetivo

| Elemento | Actualización visual | Preservación funcional |
|---|---|---|
| Encabezado | Fondo `#eef1fb`, 11px, mayúsculas y alto contraste | Conserva los dos `th` emitidos. |
| Selector | Columna fija de 44px, checkbox centrado y foco visible | Conserva `chek_id` y la clase `chek_selecion_list_wf`. |
| Documento | Icono PDF azul, título truncado en una línea y fila de 52px | Conserva el contenedor que ejecuta `vis_doc_selecion_wf`. |
| Acciones | Botón de tres puntos y menú de lectura clara | Conserva cada `tip_event` y sus atributos. |
| Filas | Divisor tenue, hover y foco dentro de la fila | No convierte la fila en un nuevo enlace ni agrega eventos. |

## Implementación por fases

### Fase 0 — Eliminar la causa de duplicación

Antes de publicar cualquier capa visual, localizar el punto que inserta dos veces el resultado del GridView. Revisar, con una sesión y una actualización parcial, en este orden:

1. El HTML recibido en la respuesta AJAX/UpdatePanel.
2. Los manejadores `endRequest`, `load` y `ready` que clonan o anexan contenido de documentos relacionados.
3. La coexistencia de un renderizado de servidor y un insert/append cliente sobre el mismo contenedor.

**Criterio de salida:** `document.querySelectorAll('[id="GridView_list_documento_relacion_wf"]').length` es exactamente `1` después de carga inicial, filtro, carga de archivo, eliminación y refresco parcial.

### Fase 1 — Hoja visual local

**Estado:** implementada en el repositorio local.

Crear una hoja local, por ejemplo `Styles/workflow-documentos-relacionados-modernos.css`, y cargarla solo en la página que contiene la tabla. Sus selectores deberán iniciarse en:

```css
[id="GridView_list_documento_relacion_wf"].gridview-documentos-relacionados
```

Se usa selector por atributo en vez de `#...` como medida defensiva durante la validación de la anomalía de IDs duplicados; una vez resuelta, sigue siendo compatible con la única tabla final.

La hoja deberá:

- Mantener `border-collapse: separate` y espaciado cero para no alterar el cálculo de GridView.
- Fijar la primera celda a 44px y reducir los márgenes Bootstrap solo dentro de ella.
- Aplicar truncamiento al título mediante `overflow`, `text-overflow` y `white-space`; nunca mediante modificación de texto.
- Dar al disparador Bootstrap de acciones un área mínima de 36px, foco `:focus-visible` y menú con `z-index` suficiente.
- Respetar el menú existente y no imponer `display` sobre `.dropdown-menu`.

### Fase 2 — Adaptador JavaScript de accesibilidad y renderizado parcial

**Estado:** implementado en el repositorio local.

Crear `js/workflow/documentos-relacionados-visual.js`, limitado a la tabla objetivo.

Responsabilidades permitidas:

- Añadir la clase `gridview-documentos-relacionados` a la tabla existente.
- Añadir al `th` vacío el texto accesible `Selección` mediante `aria-label`.
- Añadir `aria-label="Más acciones para: <nombre del documento>"` y `title="Más acciones"` al toggle del menú.
- Reaplicar los atributos después de `Sys.WebForms.PageRequestManager.endRequest` cuando esté disponible.

Prohibiciones:

- No clonar, mover, borrar ni ordenar filas.
- No interceptar `prevent(event,this)`, eventos Bootstrap, ni clics de checkboxes.
- No deduplicar tablas como solución final. Si Fase 0 detecta una tabla duplicada, debe registrar el defecto y detener el adaptador para evitar operar sobre un DOM ambiguo.

La implementación actual cumple esa última condición: cuando el selector encuentra cero o más de una tabla, no añade la clase visual ni altera atributos y registra una advertencia en consola.

### Fase 3 — Validación funcional

Validar en navegador, con carga inicial y después de actualización parcial:

| Caso | Resultado esperado |
|---|---|
| Ver documento | Se conserva `vis_doc_selecion_wf`. |
| Eliminar | Se conserva confirmación y `elim_doc_selecion_wf`. |
| Cambiar tipología | Se conserva `cambia_doc_selecion_wf`. |
| Firma, versiones y reemplazo | Cada opción conserva su `tip_event` e `idd_wf`. |
| Selección múltiple | Los checkboxes mantienen clase y `chek_id`. |
| Texto largo | Se trunca visualmente, sin pérdida del texto ni de `title`. |
| Teclado | Checkbox y toggle reciben foco visible; menú sigue operando con Bootstrap. |
| Actualización parcial | Una sola tabla, estilos reaplicados y acciones operativas. |

### Fase 4 — Publicación reversible

1. Versionar CSS y JS solo cuando cambie su contenido.
2. Respaldar los archivos concretos antes de desplegar a precompilado e IIS.
3. Publicar únicamente los recursos aprobados y la referencia de página, nunca carpetas completas.
4. Verificar en DevTools que se carga la versión esperada y que existe una sola tabla.
5. Revertir restaurando exclusivamente el respaldo del recurso intervenido y la URL versionada anterior.

## Criterios de aceptación

- Una única tabla y un único conjunto de IDs por carga.
- Ningún cambio de datos, eventos, atributos de integración o estructura generada por servidor.
- Los seis comportamientos disponibles por documento continúan funcionando.
- Presentación consistente en escritorio y ancho reducido.
- Estilo confinado a la lista de documentos relacionados, sin afectar `GridView2` ni otros módulos.

## Riesgos y decisiones

| Riesgo | Tratamiento |
|---|---|
| Ocultar la duplicación con CSS deja acciones duplicadas | Corregir el productor del DOM en Fase 0. |
| UpdatePanel elimina clases/ARIA tras postback | Reaplicar solo atributos de presentación en `endRequest`. |
| El menú queda recortado por un contenedor con overflow | Validar el contenedor real y ajustar su apilamiento local, sin mover el menú. |
| Una regla Bootstrap global cambia otros dropdowns | Todos los selectores se limitan a la tabla aprobada. |
