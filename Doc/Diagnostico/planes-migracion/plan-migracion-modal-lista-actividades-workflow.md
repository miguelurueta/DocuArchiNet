# Plan de migración: modal de actividades de ruta Workflow

**Estado:** implementado en el repositorio local el 2026-08-07; pendiente de validación funcional y publicación en el precompilado. El requerimiento posterior de tamaño fija este modal en 50% de ancho y alto disponible.  
**Repositorio destino:** `D:\imagenesda\DocuachiNet\DocuArchiNet`
**Página:** `workflow\Webworkflow.aspx`  
**Modal:** `modal_content_lista_actividades_worflow_ruta` (se conserva el identificador histórico, incluido `worflow`).

## Objetivo

Modernizar exclusivamente la presentación del modal **Enviar tarea** y de su tabla de actividades, manteniendo la estructura Web Forms, la lógica de ruta y toda la interacción existente.

El resultado debe reutilizar la capa opt-in `gridview-moderno` para la tabla y añadir solo reglas CSS encapsuladas en el modal.

## Inventario técnico confirmado

| Elemento | Identificador / clase | Responsabilidad actual |
|---|---|---|
| Panel emergente | `Panel_lista_actividades_worflow_ruta` | Contenedor del `ModalPopupExtender`. |
| Modal | `#modal_content_lista_actividades_worflow_ruta` | Contenedor visual de cabecera, contenido y pie. |
| Cabecera | `#divcabecer2_lista_actividades_worflow_ruta` | Título y cierre. |
| Área de contenido | `#contenido_procesa_lista_actividades_workflow` | Cuerpo cuyo alto ajusta JavaScript existente. |
| Actualización parcial | `UpdateGeneral_documentos` | Renderiza de nuevo contenido y tabla. |
| Área desplazable | `#div_gred` | Alto calculado por `auto_zise_popup_lista_actividades_ruta`. |
| Tabla | `GridView_envia_flujo` | Lista de actividades posibles. |
| Pie | `#div_contenido_procesa_lista_actividades_worflow_ruta_botones_desicion` | Acciones emitidas por la vista. |

La tabla no declara paginación ni ordenamiento. El backend puede ocultar columnas por índice según `WF_ESTADO_FLUJO_RUTA`; por tanto, no se deben usar selectores `nth-child`, anchos fijos ni reordenamiento de columnas.

## Cambios implementados

- `GridView_envia_flujo` conserva `filtrar table font-weight-light` y añade `gridview-moderno` como capa visual opt-in.
- `Panel_lista_actividades_worflow_ruta` usa `width: 50%` y `height: 50%` como tamaño inicial.
- La función exclusiva del modal en `js\workflow\Webworkflow.js` calcula `Math.round(espacio_iframe * 0.5)` antes de asignar la altura al panel y a su contenido; por tanto, el ajuste dinámico no vuelve a expandirlo al 98%.
- `#modal_content_lista_actividades_worflow_ruta` y sus subelementos reciben estilo encapsulado: borde tenue, radio, sombra discreta, cabecera `#eef1fb`, título y cierre compacto.
- El botón de cierre conserva `da_event_captive` y su valor de botón de servidor; solo se presenta como control de `28px × 28px`, alineado a la derecha, con hover y foco visibles.
- `#div_gred` no tiene relleno lateral y `#GridView_envia_flujo.gridview-moderno` usa `width: 100% !important`, para que la tabla ocupe todo el ancho disponible.
- No se modificaron IDs, backend, eventos, `ModalPopupExtender`, `UpdatePanel`, columnas ni cálculos internos del alto de la tabla.

## Límites inalterables

- No modificar archivos `.vb`, clases de negocio, consultas, sesiones ni servicios.
- No modificar `ModalPopupExtender_edition_lista_actividades_worflow_ruta`, `TargetControlID`, `CancelControlID` ni `PopupControlID`.
- No cambiar los IDs de paneles, tablas, botones, labels, `UpdatePanel` ni elementos ocultos.
- No modificar `auto_zise_popup_lista_actividades_ruta` ni los cálculos de alto sobre `#div_gred`.
- Excepción aprobada: el cálculo exclusivo de `Panel_lista_actividades_worflow_ruta` usa `Math.round(espacio_iframe * 0.5)` para conservar el modal a la mitad de la altura disponible; no modifica los cálculos internos de contenido ni tabla.
- No incluir JavaScript nuevo para este cambio.
- No cambiar columnas, datos, orden ni visibilidad que entrega backend.
- No añadir reglas globales para `.modal-content`, `.modal-header`, `.modal-footer`, `.table`, `th` o `td`.

## Archivos autorizados

```text
workflow\Webworkflow.aspx
Styles\workflow-tareas-modernas.css
js\workflow\Webworkflow.js
```

No se requiere modificar `Styles\gridview-moderno.css`: la tabla adopta una clase ya disponible. La modificación de `Webworkflow.js` está limitada al cálculo de altura del modal solicitado.

## Implementación detallada

### 1. Activar la capa genérica solo en la tabla del modal

En `workflow\Webworkflow.aspx`, conservar las clases actuales y añadir `gridview-moderno` al `GridView`:

```aspx
<asp:GridView ID="GridView_envia_flujo" runat="server"
    CssClass="filtrar table font-weight-light gridview-moderno" ...>
```

No añadir esta clase al panel, al `UpdatePanel`, a `#div_gred` ni a otras tablas de la página.

### 2. Capa visual local del modal

Al final de `Styles\workflow-tareas-modernas.css`, añadir reglas encapsuladas bajo `#modal_content_lista_actividades_worflow_ruta`:

```css
/* Modal Enviar tarea: alcance exclusivo. */
#modal_content_lista_actividades_worflow_ruta {
  overflow: hidden;
  border: 1px solid #dce6f1;
  border-radius: 12px;
  box-shadow: 0 18px 42px rgba(30, 53, 83, .18);
}

#modal_content_lista_actividades_worflow_ruta
  #divcabecer2_lista_actividades_worflow_ruta {
  min-height: 50px;
  padding: 0 18px;
  background: #eef1fb;
  border-bottom: 1px solid #dce2f1;
  color: #35477f;
}

#modal_content_lista_actividades_worflow_ruta
  #contenido_titulo_data_grid_dos_title {
  margin: 0 !important;
  padding: 13px 14px !important;
  border-bottom-color: #e7eaf0 !important;
}

#modal_content_lista_actividades_worflow_ruta #div_gred {
  padding: 0 8px 8px;
  scrollbar-color: #b9c4e5 #f7f8fc;
}

#modal_content_lista_actividades_worflow_ruta
  #GridView_envia_flujo.gridview-moderno {
  margin: 0;
  font-size: 13px;
}
```

Las reglas no deben establecer `height`, `min-height`, `max-height`, `width`, `position`, `top`, `left` ni `display` sobre `#div_gred`, `#contenido_procesa_lista_actividades_workflow` o el modal, porque el JavaScript existente calcula esas dimensiones.

### 3. Cabecera, cierre y pie

Las reglas locales pueden mejorar color, espaciado y foco visual del botón `.close`, siempre con el prefijo del modal. El botón debe conservar la clase `da_event_captive` y su atributo `value="Button_cerrar_lista_actividades_worflow_ruta"`.

El pie puede recibir únicamente borde superior, fondo y separación visual mediante su ID. No crear botones, no ocultar acciones ni cambiar eventos.

### 4. Tamaño del modal y ancho de tabla

El tamaño inicial del panel debe ser `width: 50%; height:50%`. Como el JavaScript existente asignaba 98% de altura al abrir el modal, el cálculo exclusivo de `Panel_lista_actividades_worflow_ruta` se ajusta a:

```javascript
var heig_porcent = Math.round(espacio_iframe * 0.5);
```

No modificar las dos asignaciones posteriores que calculan el alto del contenido y de `#div_gred`.

Para evitar espacios laterales, mantener:

```css
#modal_content_lista_actividades_worflow_ruta #div_gred { padding: 0; }
#modal_content_lista_actividades_worflow_ruta #GridView_envia_flujo.gridview-moderno {
  width: 100% !important;
}
```

### 5. Versionado de recursos

Si cambia `workflow-tareas-modernas.css`, actualizar solo la versión del recurso en `workflow\Webworkflow.aspx`.

Versión anterior vigente al redactar este plan:

```html
<link href="../Styles/workflow-tareas-modernas.css?v=20260807-grid34" rel="stylesheet" />
```

Versión implementada: `20260807-grid38`; el panel del modal usa 50% de ancho y alto, la tabla ocupa todo el ancho útil y el cierre se mantiene compacto y alineado. `Webworkflow.js` se versiona como `20260807-modal50`. No duplicar referencias CSS.

## Validación funcional

1. Abrir **Enviar tarea** desde una tarea de flujo y desde una ruta.
2. Confirmar que el modal abre, centra y cierra con el botón `×` existente.
3. Confirmar que se conservan las columnas visibles que emite backend en ambos estados de `WF_ESTADO_FLUJO_RUTA`.
4. Confirmar que la tabla se muestra después de la actualización de `UpdateGeneral_documentos`.
5. Verificar que ninguna acción del pie, selección de actividad o envío de tarea cambia su comportamiento.
6. Cambiar el tamaño de la ventana y comprobar que el cálculo de alto y el desplazamiento de `#div_gred` siguen funcionando.
7. Confirmar que el modal permanece aproximadamente al 50% de la altura disponible después de abrirse y de redimensionar la ventana.
8. Confirmar que la tabla no deja espacios laterales dentro de `#div_gred`.
9. Revisar consola: sin errores JavaScript ni recursos CSS 404.

## Validación visual

- Modal con borde tenue, radio consistente y sombra discreta.
- Cabecera clara `#eef1fb`, texto `#35477f` y botón de cierre legible.
- Tabla con encabezado, espaciado, filas alternas y hover de `gridview-moderno`.
- Sin desbordamiento horizontal artificial ni pérdida de columnas ocultas por backend.
- Área de tabla desplazable dentro del alto calculado existente.
- Modal de 50% de ancho y alto, con tabla ocupando el ancho completo del área desplazable.
- Pie visualmente separado, sin alterar sus acciones.

## Riesgos y mitigaciones

| Riesgo | Mitigación |
|---|---|
| El cálculo JavaScript de alto se rompe | El único cambio permitido es el factor `0.5` del panel solicitado; conservar los cálculos internos de contenido y tabla. |
| Se modernizan otras tablas por accidente | Usar `gridview-moderno` solo en `GridView_envia_flujo` y prefijos por ID del modal. |
| Backend oculta columnas y se desalinean reglas | No usar `nth-child`, ancho fijo ni reglas basadas en cantidad de columnas. |
| El cierre del modal deja de responder | Preservar el botón, clase `da_event_captive`, valor y `ModalPopupExtender`. |
| Una actualización parcial elimina estilos | La clase se declara en el GridView Web Forms y las reglas están en CSS estático. |

## Publicación y reversión

1. Validar primero en el repositorio local.
2. Antes de copiar al precompilado, crear respaldo fechado individual de cada archivo:

```text
<archivo>.<yyyyMMdd-HHmmss>.modal-actividades-ruta.bak
```

3. Copiar únicamente `workflow\Webworkflow.aspx`, `Styles\workflow-tareas-modernas.css` y `js\workflow\Webworkflow.js` a `D:\temfile\Gestion`.
4. Validar el precompilado; publicar a IIS solo tras aprobación visual y funcional.
5. Para revertir, restaurar exclusivamente los tres respaldos y devolver los parámetros `v` de CSS y JavaScript anteriores.

## Criterios de cierre

- La tabla `GridView_envia_flujo` tiene la clase `gridview-moderno` sin perder sus clases existentes.
- El modal se moderniza únicamente dentro de `#modal_content_lista_actividades_worflow_ruta`.
- El comportamiento de cierre, actualización parcial, ocultamiento de columnas y cálculo de tamaños se conserva.
- El modal conserva el 50% de ancho y alto tras abrirse y redimensionar la ventana; la tabla ocupa el ancho completo de `#div_gred`.
- No se modifican backend, IDs, estructura de columnas ni eventos; JavaScript se limita al factor de altura aprobado para este modal.
- Existe respaldo puntual antes de publicar en el precompilado.
