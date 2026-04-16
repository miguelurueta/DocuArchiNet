# TICKET FE

# =========================================

## Titulo

Exponer control UI de zoom visual para la hoja paginada en `AppEditor`

---

## Rol

Desarrollador Frontend Senior especializado en:

* React 19 + TypeScript estricto
* Tiptap / ProseMirror
* CSS avanzado para layout escalado
* UX de editores enriquecidos
* Clean Architecture
* Testing con Vitest + Testing Library

---

## Objetivo

Implementar un control UI de zoom visual en `AppEditor` para el modo de paginación
visual, permitiendo acercar o alejar la vista de la hoja del editor sin alterar
el HTML persistido, el contenido del documento ni la arquitectura actual de
paginación multi-hoja.

El zoom debe mantenerse como una capacidad puramente visual de la UI.

---

## Contexto obligatorio

Repo:
`C:\Users\SEBASTIAN FORERO\Documents\Docuarchi. net\DocuArchiCore.react`

Ubicación de documentación de tickets (OBLIGATORIO):
`C:\Users\SEBASTIAN FORERO\Documents\Docuarchi. net\DocuArchiCore.react\docs\Architecture\AppEditor`

Este ticket parte de que `AppEditor` ya tiene:

* hoja paginada visual
* segmentación visual multi-hoja
* contador `Pagina X de Y`
* compatibilidad con `PageBreak`
* compatibilidad con imágenes
* hardening de scroll, selección y page context

Archivos base relevantes:

* `src/app/Components/UI/AppEditor/presentation/AppEditor.tsx`
* `src/app/Components/UI/AppEditor/presentation/AppEditorToolbar.tsx`
* `src/app/Components/UI/AppEditor/AppEditor.module.css`
* `src/app/Components/UI/AppEditor/application/usePaginationMetrics.ts`
* `src/app/Components/UI/AppEditor/application/usePageContext.ts`
* `src/app/Components/UI/AppEditor/domain/editor.types.ts`

---

## Problema actual

`AppEditor` ya contempla compatibilidad con zoom visual dentro del modo paginado,
pero actualmente no expone un control UI claro para que el usuario ajuste ese
zoom durante la edición.

En documentos largos o con imágenes, el usuario necesita poder acercar o alejar
la vista sin modificar el contenido ni romper la estabilidad del modo multi-hoja.

---

## Alcance exacto

### 1. Control UI de zoom visual

Agregar un control de zoom para `AppEditor` cuando:

* `paginationMode="visual"`

UI mínima obligatoria:

* botón `-`
* porcentaje actual visible
* botón `+`

Ejemplo:
`[-] 100% [+]`

Opcional:

* presets rápidos o dropdown si mejora la UX sin sobrecargar la UI

---

### 2. Rango de zoom

Valores mínimos soportados:

* 50%
* 75%
* 100%
* 125%
* 150%

Valor por defecto:

* 100% (`1`)

---

### 3. Ámbito del zoom

El zoom SOLO aplica cuando:

* `paginationMode="visual"`

NO debe afectar:

* `paginationMode="none"`

---

### 4. Comportamiento obligatorio

El zoom debe:

* escalar visualmente la experiencia paginada
* mantener legibilidad del contenido
* mantener scroll funcional en el `canvas`
* conservar toolbar funcional
* conservar segmentación multi-hoja
* conservar contador de página
* conservar compatibilidad con `PageBreak`
* conservar compatibilidad con imágenes
* NO alterar HTML serializado
* NO alterar atributos persistidos:
  * `data-width`
  * `data-align`
  * `data-page-break`

---

## Reglas técnicas obligatorias

### Zoom visual, no semántico

PROHIBIDO:

* cambiar `font-size` persistido
* mutar HTML del documento
* recalcular width persistido de imágenes
* alterar márgenes persistidos
* cambiar el modelo continuo del documento
* introducir múltiples instancias de ProseMirror

---

### Implementación esperada

Usar:

* estado/props de `zoomLevel`
* CSS variables, por ejemplo `--app-editor-zoom`
* una estrategia de escalado visual compatible con:
  * `usePaginationMetrics`
  * `usePageContext`
  * scroll del `canvas`
  * overlays existentes

### Regla crítica

NO asumir que `transform: scale(...)` por sí solo es suficiente.

Si se usa escalado con `transform`, debe integrarse correctamente con el cálculo
de métricas, offsets visuales, scroll y página actual para evitar desalineaciones,
jitter o regresiones del modo multi-hoja.

---

## Reglas de layout

Mantener estabilidad de:

* `editorWrapper`
* `canvas`
* `sheet`
* `pageStack`
* `contentFlow`
* overlays de página
* contador de página

---

## Reglas de scroll

* El scroll debe ocurrir en el `canvas`
* NO dentro de la hoja (`sheet`)
* NO debe haber saltos bruscos por cambio de zoom
* `scrollIntoView` debe seguir comportándose correctamente

---

## Reglas de recálculo

El sistema debe recalcular correctamente:

* métricas de paginación
* página actual
* offsets visuales
* overlays y capas de hoja

cuando cambie:

* zoom
* tamaño del contenedor
* contenido
* imágenes cargadas dinámicamente

---

## API esperada (sin breaking changes)

```tsx
<AppEditor
  paginationMode="visual"
  defaultZoomLevel={1}
  minZoomLevel={0.5}
  maxZoomLevel={1.5}
/>
```

También debe contemplarse modo controlado:

```tsx
<AppEditor
  paginationMode="visual"
  zoomLevel={1}
  onZoomChange={(zoom) => {}}
/>
```

Props esperadas:

* `zoomLevel?: number`
* `defaultZoomLevel?: number`
* `minZoomLevel?: number`
* `maxZoomLevel?: number`
* `onZoomChange?: (zoom: number) => void`

---

## Reglas arquitectónicas

* domain:
  * tipado de nuevas props relacionadas con zoom

* application:
  * manejo de estado de zoom
  * integración con recálculo de métricas y page context

* infrastructure:
  * NO requerido salvo utilidades puntuales

* presentation:
  * UI del control de zoom
  * integración visual con el editor
  * aplicación del escalado visual

* estilos:
  * definidos en `AppEditor.module.css`
  * sin estilos globales

---

## Compatibilidad obligatoria

NO debe romper:

* paginación visual existente
* segmentación visual por bloques
* contador `Pagina X de Y`
* saltos de página manuales
* imágenes:
  * locales (IndexedDB)
  * remotas
* resize de imagen
* alineación horizontal de imagen
* modo controlled/uncontrolled
* toolbar existente
* selección y cursor
* undo/redo

---

## Resultado esperado

* El usuario puede ajustar el zoom visual del editor
* El documento NO cambia
* El HTML NO cambia
* La experiencia visual es fluida y estable
* El modo multi-hoja sigue comportándose de forma coherente

---

## Validaciones obligatorias

1. El control de zoom aparece SOLO en modo visual
2. El valor por defecto es 100%
3. Se respeta el rango min/max
4. La vista paginada se escala correctamente
5. El scroll funciona correctamente
6. La segmentación visual no se desalinéa
7. El contador sigue funcionando correctamente
8. `PageBreak` sigue siendo compatible
9. El HTML serializado NO cambia
10. No hay parpadeos ni saltos visuales críticos
11. No hay regresión en cursor o selección
12. El cálculo de `Pagina X de Y` sigue siendo estable bajo zoom

---

## Pruebas esperadas

* test render del control de zoom
* test de límites de zoom (min/max)
* test modo visual vs modo continuo
* test de no regresión de HTML
* test de integración con:
  * segmentación visual
  * contador
  * `PageBreak`
  * imágenes
  * page context

---

## Pruebas de regresión obligatorias

Ejecutar y reportar:

* build
* lint
* test

Aclaración obligatoria:

* cualquier fallo preexistente fuera del alcance de `AppEditor` debe reportarse
  como preexistente y no confundirse con regresiones introducidas por este ticket

Validar además:

* no conflictos de dependencias
* no regresión visual en `AppEditor`
* no ruptura de exports en:
  `src/app/Components/UI/index.ts`
* no afectación de otros módulos

---

## Restricciones

* NO usar zoom del navegador como solución
* NO modificar contenido persistido
* NO cambiar semántica del documento
* NO agregar dependencias externas innecesarias
* NO mezclar con exportación o impresión
* NO rehacer la arquitectura multi-hoja existente

---

## Instrucción final

Implementar el control UI de zoom visual para `AppEditor` respetando:

* Clean Architecture
* desacoplamiento de capas
* estabilidad visual
* performance
* compatibilidad total con la paginación multi-hoja existente

El resultado debe exponer una capacidad ya prevista por la arquitectura del
editor, sin introducir regresiones en scroll, selección, métricas, contador
de página ni segmentación visual.
