# Ticket 03 FE

## Título

Accesibilidad, integración, calidad y pruebas de `AppEditor`

## Rol

Desarrollador Frontend Senior especializado en:

- React 19 + TypeScript estricto
- Clean Architecture
- Testing con Vitest + Testing Library
- Accesibilidad (a11y)
- Componentes UI reutilizables

## Objetivo

Completar accesibilidad, exportaciones, documentación y cobertura de pruebas
para garantizar que `AppEditor` sea un componente reusable, estable,
accesible y listo para producción a nivel shared UI.

## Contexto existente

- Arquitectura: `docs/Architecture/AppEditor/AppEditor-Architecture.md`
- Implementación core: `docs/Architecture/AppEditor/01-FE-AppEditor-core.md`
- Implementación UI/UX: `docs/Architecture/AppEditor/02-FE-AppEditor-ui-ux.md`
- Implementación base en `src/app/Components/UI/AppEditor/`

## Restricciones (obligatorio)

- No romper API del core
- No agregar editor distinto a Tiptap
- Mantener tipado estricto
- Sin dependencias fuera del alcance definido
- No introducir lógica de negocio en UI

## Regla arquitectónica (obligatoria)

Este ticket debe respetar estrictamente la arquitectura existente:

- No modificar `domain`
- No modificar `infrastructure` sin justificación
- No introducir lógica en `presentation`
- Mantener separación de capas (`domain`, `application`, `infrastructure`,
  `presentation`)

Las pruebas deben respetar esta separación.

## Reglas de accesibilidad (obligatorio)

### Accesibilidad básica

- Toolbar navegable por teclado
- Focus visible en controles interactivos
- `aria-label` en botones icon-only
- Integración correcta de `label`, `helperText` y `error`
- Estados `disabled` y `readOnly` accesibles

### Accesibilidad avanzada

- Navegación completa por teclado (`tab`, `enter`, shortcuts)
- Roles ARIA correctos en toolbar y editor
- Compatibilidad básica con screen readers
- Orden de foco lógico y consistente

## Integración (obligatorio)

- Exportar desde `src/app/Components/UI/index.ts`
- Mantener integración limpia con componentes UI existentes

## Documentación (obligatorio)

Crear archivo:

`src/app/Components/UI/AppEditor/README.md`

Debe incluir:

- Descripción del componente
- Props documentadas
- Ejemplo básico
- Ejemplo en modo controlled
- Ejemplo con `disabled` y `readOnly`
- Buenas prácticas de uso
- Limitaciones conocidas

## Estructura de testing (obligatoria)

`presentation`

- Testing Library (`render` + interacción)

`application`

- pruebas unitarias del hook `useAppEditor`

`infrastructure`

- mock de Tiptap (no testear internals)

No mezclar capas en los tests.

## Pruebas de calidad (obligatorio)

- Render estable en light/dark mode
- Alternancia del boton visible `light/dark`
- Toolbar responsive y usable en touch
- Serialización consistente del contenido HTML
- Accesibilidad: focus, labels y navegación por teclado
- Inserción y edición de enlaces
- Carga e inserción de imágenes
- Undo/redo en flujos consecutivos

## Validaciones de performance (obligatorio)

- No recreación del editor en cada render
- No pérdida de estado del editor
- Escritura fluida sin lag
- Toolbar no provoca re-render completo del editor

## Pruebas de regresión después de la instalación (obligatorio)

- Ejecutar `build`
- Ejecutar `lint`
- Ejecutar `test`
- Confirmar que Tiptap no introduce conflictos de dependencias
- Confirmar que no se rompen exports compartidos
- Confirmar que no hay regresiones visuales
- Confirmar que formularios y layouts existentes no se afectan
- Ejecutar pruebas de humo sobre:
  - renderizado
  - escritura
  - formato
  - enlaces
  - imágenes

## Pruebas obligatorias

### Funcionales

- Render básico con props mínimas
- Render con `label`, `helperText` y `error`
- Cambio de contenido vía `onChange`
- Formato `bold`, `italic` y `underline`
- Listas `bullet`, `ordered` y `task list`
- Heading dropdown
- Trigger de encabezados basado en `AppButton` + `AppDropdown`
- Alineación `left`, `center`, `right` y `justify`
- Inserción y edición de links
- Inserción de imágenes
- Undo y redo
- `disabled` y `readOnly`
- Compatibilidad responsive y de tema

## Regla de regresion obligatoria

Toda evolucion futura del componente debe probar explicitamente que no se
rompen:

- toolbar completa;
- boton visible de tema;
- dropdown de encabezados;
- formularios visibles para enlaces e imagenes;
- scroll interno del contenido editable;
- integracion shared desde `src/app/Components/UI/index.ts`.

### Arquitectura

- `useAppEditor` funciona de forma independiente
- No existe uso directo de Tiptap en UI
- No hay pérdida de estado en re-render
- Controlled y uncontrolled funcionan correctamente

## Validación de integración (obligatorio)

- Uso de `AppEditor` dentro de un formulario real
- Validar comportamiento en submit
- Validar integración con otros componentes UI
- Confirmar que no rompe layouts existentes

## Criterios de aceptación

- Tests pasan en Vitest
- README completo con ejemplos de uso
- Accesibilidad básica y avanzada validada
- Pruebas de calidad ejecutadas
- Pruebas de regresión completadas
- Performance validada
- Integración funcional en contexto real
- Componente listo para producción

## Instrucción final

Implementar respetando estrictamente:

- Clean Architecture
- Separación de capas
- Accesibilidad (WCAG)
- Performance
- Testing por capas
- Documentación clara

El resultado debe ser un componente robusto, accesible, testeado y listo para
uso en un sistema SaaS escalable.
