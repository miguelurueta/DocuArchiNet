## Context

El cambio `SCRUMCORE-15` nace desde Jira con el objetivo de crear un componente `AppDropdown` reusable para la capa UI compartida. El repositorio ya tiene wrappers consistentes en `src/app/Components/UI` como `AppButton`, `AppInput`, `AppModal`, `AppTabs`, `AppToolbar` y `AppContent`, por lo que este cambio no debe introducir un patron nuevo sino extender la misma estrategia: encapsular el proveedor UI detras de una API propia del proyecto.

Hoy el proyecto usa Ant Design y MUI en paralelo. Eso vuelve especialmente relevante un `Dropdown` reusable, porque menus contextuales, acciones secundarias, selecciones simples y overflow menus pueden aparecer en varios modulos, pero no existe aun una primitive tipada que preserve consistencia visual, semantica y de accesibilidad. El `AppToolbar` actual incluso depende directamente de `Dropdown` de Ant Design como detalle interno, lo que confirma que el comportamiento ya es util, pero no esta formalizado como capability reutilizable independiente.

La propuesta generada desde Jira contiene ruido de naming (`AppAppdropdown`), asi que este diseno fija el alcance real y el nombre tecnico consistente: el capability y el componente se documentaran como `app-dropdown` y `AppDropdown`.

## Goals / Non-Goals

**Goals:**
- Exponer un componente `AppDropdown` reusable desde `src/app/Components/UI/AppDropdown/`.
- Definir una API tipada que cubra apertura por trigger, render de items, estados `disabled` y callbacks de seleccion sin filtrar tipos del vendor UI a consumidores.
- Soportar los casos de uso prioritarios del repo: menu de acciones, overflow menu y seleccion contextual simple.
- Mantener accesibilidad por teclado y nombres accesibles para trigger e items.
- Permitir integracion progresiva en componentes existentes, especialmente puntos como `AppToolbar`, sin romper su contrato actual.

**Non-Goals:**
- No construir en esta primera iteracion un select completo con busqueda, multiseleccion, virtualizacion o carga remota.
- No reemplazar todas las apariciones actuales de `Dropdown` en el repo dentro de este mismo cambio salvo que se defina un caso consumidor puntual.
- No exponer directamente `MenuProps`, `DropdownProps` ni otros tipos crudos de Ant Design como API publica principal.
- No resolver logica de negocio de permisos, routing o fetch dentro del componente.

## Decisions

### 1. Ubicar `AppDropdown` en `src/app/Components/UI` y exportarlo desde el barrel compartido

El componente seguira la misma organizacion de `AppToolbar` y `AppContent`: carpeta propia, `index.ts`, estilos aislados y export desde `src/app/Components/UI/index.ts`. Esto preserva descubribilidad y mantiene la UI reusable del proyecto en una sola capa.

Alternativas consideradas:
- Crear el componente en `src/shared`: descartado porque el patron vigente para primitives visuales reutilizables esta en `src/app/Components/UI`.
- Dejar el uso de `Dropdown` disperso por modulo: descartado porque duplica contratos, accesibilidad y estilos.

### 2. Diseñar una API propia basada en `items` tipados y `trigger` controlado

`AppDropdown` expondra una API centrada en:
- `items`: lista tipada de opciones con `key`, `label`, `icon`, `danger`, `disabled` y `onSelect`
- `trigger`: `ReactNode` usado para abrir el menu
- `placement`, `disabled`, `open`, `defaultOpen`, `onOpenChange`
- soporte opcional para agrupaciones o separadores mediante tipos discriminados si el spec lo exige

La razon es desacoplar al consumidor del shape especifico de Ant Design y al mismo tiempo evitar una API demasiado libre basada solo en `children`, que seria mas dificil de validar y documentar.

Alternativas consideradas:
- Reexportar `Dropdown` de Ant Design casi sin cambios: descartado por acoplamiento directo al vendor y por fuga de tipos inestables a toda la app.
- Exigir render props complejos para todos los items: descartado porque sube la friccion de uso en escenarios simples de acciones contextuales.

### 3. Implementar sobre Ant Design como detalle interno, manteniendo contrato neutral

La implementacion usara `Dropdown` y `menu` de Ant Design internamente porque ya estan disponibles, integrados y alineados con el stack actual. Sin embargo, el contrato publico del componente no debe exponer dependencias estructurales del vendor salvo donde sea inevitable y justificado.

Esto sigue el mismo criterio aplicado en otros wrappers del repo: aprovechar el proveedor visual existente sin trasladar su complejidad a cada consumidor.

Alternativas consideradas:
- Construir el overlay completamente desde cero con portales y manejo manual de foco: descartado por costo, riesgo de accesibilidad y falta de necesidad en esta iteracion.
- Implementar sobre MUI: descartado porque el repo ya usa Ant Design en wrappers UI clave y el caso inmediato encaja mejor con su `Dropdown`.

### 4. Separar claramente el rol del trigger del rol de seleccion

El `trigger` sera un nodo controlado por el consumidor y `AppDropdown` sera responsable de asociarle semantica de apertura, estado disabled y comportamiento de overlay. Los items del menu dispararan callbacks de seleccion del proyecto, no eventos crudos del vendor.

Esta separacion permite reutilizar `AppButton`, icon buttons u otros triggers futuros sin duplicar la logica de overlay. Tambien facilita integrar el componente en `AppToolbar` para overflow menus o acciones secundarias compactadas.

Alternativas consideradas:
- Hacer que `AppDropdown` siempre renderice su propio boton: descartado porque limita composicion y no cubre bien triggers ya existentes.
- Permitir cualquier `children` sin contrato para usar como overlay: descartado porque dificulta pruebas, accesibilidad y consistencia.

### 5. Tratar accesibilidad y pruebas como parte del capability, no como detalle opcional

El contrato de `AppDropdown` debe exigir:
- trigger accesible con nombre visible o `aria-label`
- apertura por teclado cuando el trigger sea interactivo
- navegacion y seleccion de items conforme al patron base del proveedor
- soporte para estados deshabilitados sin callbacks accidentales

Las pruebas deben cubrir comportamiento observable con Vitest + Testing Library, referenciando el spec correspondiente con `[SPEC:<ID>]`. El enfoque principal sera validar trigger, apertura/cierre, render de items y ejecucion de callbacks.

Alternativas consideradas:
- Confiar solo en pruebas del vendor: descartado porque no valida el contrato ni la ergonomia especifica del wrapper del proyecto.

## Risks / Trade-offs

- [API demasiado estrecha para casos reales] -> Mitigacion: modelar primero acciones contextuales y overflow menu, pero prever extensiones compatibles como separadores o secciones.
- [API demasiado amplia desde la primera version] -> Mitigacion: limitar el alcance inicial a dropdown de acciones y seleccion simple, dejando fuera busqueda y multiseleccion.
- [Fuga accidental de tipos de Ant Design al contrato publico] -> Mitigacion: definir tipos propios (`AppDropdownItem`, `AppDropdownProps`) y mapear internamente al vendor.
- [Naming inconsistente heredado de Jira] -> Mitigacion: fijar en `design`, `specs` y codigo el nombre canonico `AppDropdown`.
- [Integracion parcial con `AppToolbar` genera duplicacion temporal] -> Mitigacion: documentar `AppToolbar` como consumidor objetivo posterior o puntual, evitando refactor masivo prematuro.

## Migration Plan

1. Crear specs para `app-dropdown` con requisitos de trigger, items, seleccion, disabled y accesibilidad.
2. Implementar `AppDropdown` en `src/app/Components/UI/AppDropdown/` y exportarlo desde el barrel de UI.
3. Agregar pruebas de comportamiento con Vitest + Testing Library para apertura, render, callbacks y estados disabled.
4. Integrar un consumidor inicial del repo, preferiblemente un caso de overflow o acciones secundarias donde hoy se use `Dropdown` directamente.
5. Validar que la adopcion no rompa estilos ni contratos existentes y ajustar el wrapper antes de ampliar otros casos de uso.

Rollback:
- Si el wrapper introduce regresiones visuales o de interaccion, revertir la adopcion en consumidores y mantener el componente aislado hasta estabilizar el contrato.

## Open Questions

- Si el scope inicial debe incluir separadores y grupos de items o si basta con items planos.
- Si la primera adopcion real debe ocurrir dentro de `AppToolbar` o en otro modulo consumidor con menor acoplamiento.
- Si el trigger permitira modo completamente controlado (`open`) desde el primer release o solo modo no controlado con `defaultOpen`.
