# Design

## Context

`SCRUMCORE-138` implementa un modal de reasignacion de respuesta en el modulo **Gestion Correspondencia**.

El modal se abre desde la opcion **"Reasignar Trámite"** del dropdown de acciones dentro de la tabla del modulo.

Referencia de arquitectura (repo):
- `docs/Architecture/GestionCorrrespondecia/AppModal-Reasignar-Respuesta.md`

## Scope

Incluye:
- componente presentacional `ReasignarRespuestaModal` basado en `AppModal`
- integracion UI para apertura/cierre desde la opcion del dropdown (sin modificar la tabla)
- estilos responsive (desktop/tablet/mobile)
- accesibilidad basica (focus inicial, labels, teclado)
- pruebas unitarias/UI (Vitest + Testing Library)

No incluye:
- logica de negocio ni llamadas a backend
- validaciones de dominio o persistencia real de reasignacion
- cambios de API, endpoints o contratos de backend
- refactors del componente base de tabla

## Design Decisions

### 1) Modal controlado y desacoplado

`ReasignarRespuestaModal` debe ser controlado por props:
- `open`, `onClose`
- `radicado`, `nota`
- `users` y callbacks de tags
- `onSubmit` (solo callback UI)

Esto evita acoplar UI a detalles de tabla o dominio y permite testearlo de forma aislada.

### 2) Bridge de integracion sin tocar la tabla

La integracion se realiza en el contenedor del modulo que construye las acciones del dropdown.

Regla:
- no se modifica la tabla (columnas/render/paging)
- solo se agrega un handler para "Reasignar Trámite" y estado `open/context`

### 3) Responsive robusto y sin overflow

Requisitos:
- width desktop: `min(720px, 92vw)`
- mobile: layout compacto, acciones en columna y botones full width
- si el contenido crece, scroll interno del body del modal
- evitar scroll del overlay/wrapper en mobile usando `wrapClassName` + CSS Modules

### 4) Accesibilidad minima

- focus inicial al abrir (input de `AppInputTags` o boton primario)
- `Escape` cierra (AppModal)
- titulo ligado con `aria-labelledby`
- iconos decorativos `aria-hidden`

## Technical Approach

- Nuevo folder:
  - `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/`
- Se usara `AppModal` con `hideFooter` y footer custom (acciones).
- `AppInputTags` en modo chips para seleccionar/remover responsables.
- Pruebas con Testing Library enfocadas a comportamiento observable (apertura/cierre, callbacks, render).

