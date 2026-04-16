# TICKET FE

# =========================================

## Titulo

Refinar interacción, selección y estabilidad del modo multi-hoja en `AppEditor`

---

## Rol

Desarrollador Frontend Senior especializado en:

* React 19 + TypeScript estricto
* Tiptap / ProseMirror
* UX de editores enriquecidos
* Manejo de selección/cursor
* Performance y rendering
* Testing con Vitest + Testing Library

---

## Objetivo

Estabilizar completamente el comportamiento del `AppEditor` en modo multi-hoja,
garantizando que cursor, selección, scroll, imágenes, contador de página y zoom
funcionen de forma consistente, fluida y sin regresiones.

Este ticket es de hardening: NO introduce nuevas capacidades, solo asegura
robustez del sistema existente.

---

## Contexto obligatorio

Este ticket depende de:

* hojas visuales reales con márgenes
* segmentación automática por página
* `PageBreak` manual
* contador de página
* zoom visual

Archivos relevantes:

* `usePaginationMetrics.ts`
* `usePageContext.ts`
* `AppEditor.tsx`
* `AppEditor.module.css`

---

## Problemas a estabilizar

La implementación multi-hoja puede introducir errores en:

* cursor y selección (ProseMirror mapping)
* scroll y `scrollIntoView`
* cálculo de página actual
* overlays (guías, contador)
* imágenes (resize + align)
* rendimiento en documentos largos
* sincronización con zoom

---

## Alcance técnico

### 1. Cursor y selección

* Asegurar que el cursor:

  * no salta incorrectamente entre páginas
  * mantiene posición tras re-render
* Asegurar que la selección:

  * funciona entre límites de página
  * no se rompe con overlays visuales

---

### 2. Scroll y navegación

* Garantizar que `scrollIntoView`:

  * posiciona correctamente dentro del `canvas`
  * no rompe el offset por zoom
* Evitar saltos bruscos de scroll
* Mantener scroll continuo único

---

### 3. Página actual (Page Context)

* Refinar cálculo de página actual basado en:

  * posición de scroll
  * posición del cursor
* Definir prioridad:

  * cursor > scroll (si aplica)
* Evitar jitter en cambios de página

---

### 4. Imágenes

Validar compatibilidad completa con:

* resize (`data-width`)
* alineación (`data-align`)
* imágenes locales (IndexedDB)
* imágenes remotas

Asegurar:

* selección correcta
* no pérdida de foco
* no colisión con segmentación visual

---

### 5. PageBreak

* No romper navegación al cruzar un `PageBreak`
* Mantener consistencia en:

  * cálculo de página
  * scroll
  * selección

---

### 6. Zoom

* Asegurar que zoom NO rompe:

  * cálculo de páginas
  * selección
  * scroll
* Recalcular métricas correctamente al cambiar zoom

---

### 7. Performance

* Reducir:

  * flicker visual
  * reflows innecesarios
  * recalculos excesivos

* Optimizar:

  * uso de `requestAnimationFrame`
  * uso de `ResizeObserver`
  * memoización de métricas

---

## Reglas obligatorias

* No modificar HTML persistido
* No cambiar modelo del documento
* No rehacer arquitectura de páginas
* No introducir nuevas features
* No romper undo/redo
* No romper controlled/uncontrolled

---

## Compatibilidad obligatoria

Debe seguir funcionando sin regresión:

* hojas visuales
* segmentación automática
* contador de página
* zoom
* PageBreak
* imágenes (locales/remotas)
* toolbar

---

## Resultado esperado

* El editor multi-hoja se comporta de forma estable
* No hay saltos de cursor inesperados
* La selección es consistente
* El scroll es fluido
* El contador es preciso
* La experiencia se siente sólida tipo producto profesional

---

## Validaciones obligatorias

1. Cursor no salta entre páginas inesperadamente
2. Selección funciona correctamente en todo el documento
3. Scroll es continuo y sin saltos bruscos
4. `PageBreak` no rompe navegación
5. Imágenes siguen siendo editables
6. Contador de página es estable
7. Zoom no rompe interacción
8. No hay flicker visible crítico
9. Performance aceptable en documentos largos

---

## Pruebas esperadas

* navegación entre múltiples páginas
* selección cruzando límites de página
* interacción con imágenes
* scroll + contador + PageBreak
* pruebas con zoom
* pruebas con documentos largos
* regresión completa del editor

---

## Restricciones

* No exportación
* No impresión
* No cambios de persistencia
* No cambios en HTML
* No nuevas capacidades funcionales

---

## Instrucción final

Refinar y estabilizar completamente el modo multi-hoja de `AppEditor`,
asegurando una experiencia fluida, consistente y lista para producción,
sin alterar la arquitectura ni el modelo de datos del editor.
