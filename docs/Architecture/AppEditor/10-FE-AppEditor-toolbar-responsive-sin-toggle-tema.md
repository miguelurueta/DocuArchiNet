# =========================================
# TICKET FE
# =========================================

## Titulo
Simplificar `AppEditor` removiendo toggle dark/light y optimizando toolbar responsive

---

## Rol
Desarrollador Frontend Senior especializado en:
- React 19 + TypeScript estricto
- Clean Architecture
- UI/UX responsive
- Performance en componentes complejos
- Testing con Vitest + Testing Library

---

## Objetivo

Ajustar `AppEditor` para:

1. eliminar el toggle visible de cambio de tema (dark/light);
2. optimizar la toolbar para mobile y tablet, reduciendo su impacto visual sin perder funcionalidad ni claridad.

---

## Contexto obligatorio

Repo:
`C:\Users\SEBASTIAN FORERO\Documents\Docuarchi. net\DocuArchiCore.react`

Archivos base:

- `src/app/Components/UI/AppEditor/presentation/AppEditor.tsx`
- `src/app/Components/UI/AppEditor/presentation/AppEditorToolbar.tsx`
- `src/app/Components/UI/AppEditor/domain/editor.types.ts`
- `src/app/Components/UI/AppEditor/AppEditor.module.css`

---

## Problema actual

- Existe un toggle de tema innecesario en la toolbar.
- La toolbar ocupa demasiado espacio en resoluciones reducidas.
- Se reduce el área útil del editor.

---

## Alcance exacto

### 1. Eliminación del toggle dark/light

- Remover el botón de cambio de tema del toolbar.
- Eliminar handlers, estado y props internas asociadas al toggle si quedan sin uso.
- Limpiar estilos asociados al toggle.

### Regla crítica

El soporte de tema NO debe eliminarse.

- `AppEditor` debe seguir siendo compatible con el tema global de la aplicación.
- Solo se elimina el control manual (toggle), no el theming.

### Compatibilidad

- Si existen props relacionadas al tema:
  - mantenerlas si son usadas externamente
  - o deprecarlas sin romper API

---

### 2. Optimización de toolbar responsive

Optimizar para:

#### Mobile
- 667px
- 896px
- 932px

#### Tablet
- 1024px

---

## Estrategia de responsive (obligatoria)

Se debe aplicar una combinación de:

- agrupación de acciones
- uso de iconos claros
- colapso de acciones secundarias en dropdown
- reducción de padding y spacing
- posible wrap controlado (sin overflow horizontal)

### Reglas UX

- acciones principales siempre visibles (bold, italic, undo, redo)
- acciones secundarias pueden colapsarse
- mantener usabilidad táctil
- evitar toolbar de múltiples filas excesivas

---

## Reglas arquitectónicas

- `presentation`:
  - eliminación del toggle
  - composición responsive de toolbar

- `domain`:
  - limpiar contratos relacionados al toggle si quedan obsoletos

- `AppEditor.module.css`:
  - layout responsive
  - control de spacing y distribución

---

## Reglas de performance (obligatorio)

- No provocar re-render completo del editor al interactuar con toolbar
- Memoizar toolbar si es necesario
- No perder foco del editor al interactuar
- No recrear instancia de Tiptap

---

## Reglas de API (crítico)

- No romper API pública existente de `AppEditor`
- No eliminar props sin validación de uso
- No introducir breaking changes silenciosos

---

## Resultado esperado

- Toolbar más compacta en mobile y tablet
- Editor con mayor área útil visible
- Eliminación del toggle sin afectar theming global
- UX más limpia y enfocada

---

## Validaciones obligatorias

1. El toggle dark/light no aparece en la toolbar
2. El editor sigue respetando el tema global
3. La toolbar no se desborda en:
   - 667px
   - 896px
   - 932px
   - 1024px
4. Las acciones siguen siendo entendibles
5. Dropdowns y popovers funcionan correctamente
6. No se rompe la experiencia en desktop
7. No se pierde foco del editor
8. No hay regresión visual

---

## Pruebas esperadas

- pruebas del toolbar sin toggle
- pruebas de render del editor sin dependencia del tema
- pruebas responsive de toolbar
- pruebas de interacción táctil
- regresión completa del editor

---

## Instrucción final

Implementar la simplificación de `AppEditor` eliminando el toggle de tema y optimizando la toolbar responsive, manteniendo compatibilidad con el sistema de theming global, sin romper API ni comportamiento del editor.

El resultado debe mejorar la experiencia en mobile/tablet sin introducir regresiones.
