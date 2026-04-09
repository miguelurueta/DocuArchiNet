## Context

`SCRUMCORE-63` formaliza el Ticket 01 FE para crear `AppInputTags` como componente UI reusable en `src/app/Components/UI/AppInputTags/`.

El contexto funcional viene de los documentos `docs/Architecture/SelectDestinatario-Reusable/AppInputTags-reqs.md` y `docs/Architecture/SelectDestinatario-Reusable/Ticket-01-FE-AppInputTags.md`. La base actual vive en `RadicacionForm.tsx`, donde el flujo de destinatario usa select/autocomplete con tags, acciones de informacion y estado controlado por el formulario.

El componente nuevo debe extraer esa capacidad hacia la capa UI compartida sin consumir APIs, sin acoplarse a `RadicacionForm` y sin depender de `KeyPress` para agregar o eliminar tags. Debe alinearse visualmente con `AppInput` y puede apoyarse en Ant Design `AutoComplete`, `Input`, `Tag`, `Spin` y en componentes locales como `AppDropdown` cuando se requieran acciones secundarias.

Nota de naming: el proposal generado automaticamente mencionaba `AppAppinputtagsFe01`; antes de archivar se corrigio a `AppInputTags` y la capability `app-input-tags` para evitar crear una spec principal con nombre tecnico incorrecto.

## Goals / Non-Goals

**Goals:**

- Crear `AppInputTags` como componente reusable y presentacional de la capa UI.
- Soportar modos `single` y `multiple` sobre un arreglo de tags.
- Permitir uso controlado con `value` y no controlado con `defaultValue`.
- Permitir agregar tags por seleccion de sugerencia, boton/accion de confirmacion y Enter, sin depender de `KeyPress`.
- Permitir eliminar un tag y eliminar todos mediante callbacks explicitos.
- Exponer busqueda con `onSearch`, `minLength` y `debounceMs` sin consumir APIs.
- Mostrar `loading` sin bloquear el input ni perder foco.
- Mantener estilos y estados visuales alineados a `AppInput`.
- Mantener accesibilidad para input, tags, clear, remove y acciones de dropdown.
- Exportar el componente desde `src/app/Components/UI/index.ts` y cubrirlo con pruebas de comportamiento.

**Non-Goals:**

- No implementar hooks de conexion a endpoints.
- No consultar APIs dentro de `AppInputTags`.
- No migrar todos los consumidores existentes en este ticket salvo que el spec lo exija expresamente.
- No rediseñar `RadicacionForm`.
- No reemplazar `AppInputSearch` ni compartir estado interno con ese componente.
- No introducir dependencias nuevas si Ant Design y componentes UI locales cubren el caso.
- No usar estilos globales ni selectores globales de Ant Design fuera del CSS module del componente.

## Decisions

### 1. Componente presentacional y sin API

`AppInputTags` sera responsable de renderizar el control, manejar su input local cuando opere no controlado, disparar callbacks y mostrar tags/opciones. No conocera endpoints, DTOs ni servicios.

Alternativa considerada: incluir el hook de autocomplete dentro del componente. Se descarta porque acoplaria un reusable UI a contratos de dominio y haria mas dificil usarlo con diferentes fuentes de datos.

### 2. Contrato controlado/no controlado

El componente aceptara `value?: string[]` y `defaultValue?: string[]`. Si `value` esta definido, el arreglo visible proviene siempre del consumidor. Si no esta definido, el componente mantiene el estado interno inicializado con `defaultValue`.

Los callbacks `onAddTag`, `onRemoveTag` y `onRemoveAll` seran el canal estable para notificar cambios. En modo `single`, agregar un tag reemplaza la seleccion visible; en modo `multiple`, agrega sin duplicar si el valor ya existe.

Alternativa considerada: exponer solo `onChange(tags)`. Se descarta para el ticket inicial porque los requerimientos piden callbacks explicitos por accion y porque facilita probar adicion/eliminacion como eventos separados.

### 3. Autocomplete desacoplado

`options` alimentara `AutoComplete` con elementos `{ label: string; value: string }`. La busqueda se notificara por `onSearch(query)` con `minLength` y `debounceMs`, pero el consumidor sera quien traiga datos y actualice `options`.

Enter y click en icono/accion de busqueda deben cancelar cualquier debounce pendiente y disparar `onSearch` de forma inmediata si el texto cumple `minLength`.

Alternativa considerada: usar `Input.Search`. Se descarta por restriccion del ticket y para mantener control sobre tags, suffix, loading y accesibilidad.

### 4. Sin dependencia de KeyPress

La confirmacion de tags debe basarse en eventos modernos y explicitos: `onKeyDown` para Enter, seleccion de `AutoComplete`, y acciones de boton/dropdown. No se usara `KeyPress`, porque esta deprecado y puede comportarse distinto entre navegadores/IME.

### 5. Acciones secundarias con AppDropdown

El componente puede recibir un `toolbar?: { render: () => React.ReactNode }` para permitir acciones externas o renderizar una accion local de eliminacion masiva con `AppDropdown` cuando aplique. Las acciones deben mantenerse presentacionales y delegar la logica a callbacks.

Alternativa considerada: incrustar acciones de dominio como `abrirInformacion` en cada tag. Se permite como callback de presentacion cuando el tag tenga un id resoluble por el consumidor, pero el componente no debe conocer la fuente de esos ids ni abrir modales directamente.

### 6. Estilos alineados con AppInput

El CSS module de `AppInputTags` debe reutilizar las reglas visuales de `AppInput`: border radius `12px`, estados hover/focus/error/disabled, spacing consistente y variantes `sm | md | lg`.

No se modificara `AppInput` salvo que falte una extension imprescindible y reusable. Cualquier ajuste compartido debe justificarse en el cambio.

### 7. Accesibilidad y foco

El componente debe exigir nombre accesible mediante `label`, `aria-label` o `aria-labelledby`. Los botones de eliminacion deben tener nombres accesibles como `Eliminar <tag>` y la accion masiva debe exponer `aria-label="Eliminar todos"`.

`loading` no deshabilita el input; `disabled` o `selectDisabled` si bloquean interaccion. El foco no debe perderse al recibir nuevas opciones o activar loading.

## Risks / Trade-offs

- [Riesgo] El proposal podia dejar una capability con nombre generado incorrecto -> [Mitigacion] corregir el proposal y la carpeta de spec a `app-input-tags` antes de archivar.
- [Riesgo] Duplicar logica que ya existe en `RadicacionForm` -> [Mitigacion] Extraer comportamiento reusable y cubrir con pruebas antes de migrar consumidores.
- [Riesgo] Dobles ejecuciones de `onSearch` por debounce + Enter/click -> [Mitigacion] cancelar timers pendientes antes de disparos inmediatos y probar con fake timers.
- [Riesgo] Mezclar modo controlado y no controlado -> [Mitigacion] derivar el valor visible de una unica fuente y documentar precedencia de `value`.
- [Riesgo] Mutar `options` o `value` -> [Mitigacion] tratar props como inmutables y crear arreglos nuevos al normalizar.
- [Riesgo] Loading bloquea la interaccion -> [Mitigacion] mostrar `Spin`/indicador visual sin aplicar disabled salvo que `selectDisabled` lo indique.
- [Riesgo] Accesibilidad incompleta en tags y dropdown -> [Mitigacion] pruebas con Testing Library por rol/nombre accesible y navegacion por teclado.

## Migration Plan

1. Crear `src/app/Components/UI/AppInputTags/` con componente, CSS module, tests y barrel export.
2. Exportar `AppInputTags` desde `src/app/Components/UI/index.ts`.
3. Mantener `RadicacionForm` sin migracion funcional inicial salvo que el spec/tasks lo requieran.
4. Agregar pruebas unitarias para contrato core: controlado/no controlado, add/remove, debounce, minLength, loading, options, size y accesibilidad.
5. Si se migra un consumidor, hacerlo en un commit o tarea separada verificando que el payload del formulario no cambie.

Rollback: si la migracion de consumidor introduce regresion, conservar el componente reusable pero revertir solo el uso en el consumidor, ya que el nuevo componente no debe alterar rutas ni servicios.

## Open Questions

- El contrato final debe decidir si `abrirInformacion` recibe siempre `id: number` o si `options` necesita metadata adicional para asociar value/id de forma segura.
- Falta definir si `onAddTag` debe recibir solo `value` o tambien el option completo seleccionado.
- Falta decidir si `onRemoveAll` debe ser requerido siempre o solo cuando se renderice una accion masiva.
- Falta decidir si `rules` y `name` deben estar en `AppInputTags` o en un wrapper de formulario para no acoplar el componente base a Ant Design `Form.Item`.
