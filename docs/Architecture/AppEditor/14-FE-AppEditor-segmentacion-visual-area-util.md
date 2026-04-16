# TICKET 02 FE

# =========================================

## Titulo

Implementar segmentación automática visual por área útil de página en `AppEditor`

---

## Rol

Desarrollador Frontend Senior especializado en:

* React 19 + TypeScript estricto
* Tiptap / ProseMirror
* Medición DOM
* Layout paginado
* Performance en editores
* Clean Architecture
* Testing con Vitest + Testing Library

---

## Objetivo

Implementar una capa de segmentación visual que permita que el contenido de
`AppEditor` se perciba distribuido hoja a hoja, respetando el área útil de página
y los márgenes definidos, sin dividir el documento internamente.

Esta segmentación debe integrarse con `PageBreak` manual como punto de corte
obligatorio.

---

## Contexto obligatorio

Este ticket depende de que `AppEditor` ya tenga:

* hojas visuales reales (A4)
* márgenes visibles
* paginación visual base
* `PageBreak` manual funcional
* contador de página
* soporte de zoom visual

Archivos base relevantes:

* `src/app/Components/UI/AppEditor/application/usePaginationMetrics.ts`
* `src/app/Components/UI/AppEditor/application/usePageContext.ts`
* `src/app/Components/UI/AppEditor/presentation/AppEditor.tsx`
* `src/app/Components/UI/AppEditor/AppEditor.module.css`

---

## Problema actual

Aunque existen hojas visuales, el contenido sigue siendo una superficie continua:

* no hay salto visual real entre páginas;
* el contenido puede “cruzar” visualmente el límite;
* la experiencia no simula correctamente un documento paginado.

---

## Alcance exacto

* Calcular el área útil de cada página:

  * altura útil = pageHeight - top - bottom
  * ancho útil = pageWidth - left - right

* Medir contenido renderizado dentro de `.ProseMirror`

* Implementar segmentación visual basada en altura acumulada

* Generar modelo de páginas, por ejemplo:

```ts
type PageSegment = {
  pageNumber: number;
  startOffset: number;
  endOffset: number;
  top: number;
  bottom: number;
};
```

* Renderizar el contenido dentro de hojas visuales sin dividir el editor

* Integrar `PageBreak` como:

  * corte forzado de nueva página
  * reinicio de cálculo de altura acumulada

---

## Regla crítica

El documento sigue siendo único en Tiptap.

PROHIBIDO:

* dividir el documento en múltiples editores
* mover nodos entre páginas
* clonar contenido
* modificar el HTML persistido

La segmentación debe ser exclusivamente visual.

---

## Estrategia técnica esperada

* Medición por bloques del DOM (`getBoundingClientRect`)
* Acumulación de alturas por flujo
* Cálculo de puntos de corte (breakpoints)
* Uso de:

  * `ResizeObserver`
  * `requestAnimationFrame`
  * debounce controlado

---

## Casos especiales obligatorios

### Imágenes grandes

* Si una imagen excede el alto de página:

  * debe mantenerse íntegra
  * puede desbordar la hoja visual
  * NO debe romper el editor

---

### Zoom

* La segmentación debe adaptarse al `zoomLevel`
* Las métricas deben recalcularse correctamente

---

## Reglas obligatorias

* No alterar HTML persistido
* No romper cursor ni selección
* No romper undo/redo
* No introducir múltiples instancias de ProseMirror
* Mantener scroll continuo en el `canvas`
* Evitar recalculos costosos en cada input

---

## Compatibilidad obligatoria

Debe seguir funcionando sin romper:

* hojas visuales reales
* márgenes
* contador de página
* zoom visual
* `PageBreak` manual
* imágenes:

  * resize
  * alineación
* modo controlled/uncontrolled
* toolbar

---

## Resultado esperado

* El contenido se percibe distribuido hoja a hoja
* No hay texto cruzando límites de página
* `PageBreak` genera una nueva hoja
* La experiencia se acerca a Word/Docs
* El documento sigue siendo continuo internamente

---

## Validaciones obligatorias

1. El contenido no cruza visualmente páginas
2. Existe salto claro entre hojas
3. `PageBreak` fuerza nueva página
4. El scroll sigue siendo continuo
5. El contador de página es coherente
6. No se rompe selección ni edición
7. No hay parpadeos visuales críticos

---

## Pruebas esperadas

* documento de 1 página
* documento de múltiples páginas
* inserción de `PageBreak`
* integración con contador
* integración con zoom
* regresión de imágenes y bloques

---

## Restricciones

* No implementar split real de párrafos
* No exportación
* No persistencia estructural de páginas
* No modificar modelo del documento

---

## Instrucción final

Implementar una capa de segmentación visual precisa, performante y desacoplada
para `AppEditor`, permitiendo una experiencia paginada realista sin romper el
modelo continuo del documento ni la arquitectura existente.
