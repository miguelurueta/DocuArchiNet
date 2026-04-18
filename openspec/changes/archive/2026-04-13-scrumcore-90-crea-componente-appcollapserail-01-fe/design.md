## Context

El ticket SCRUMCORE-90 solicita crear un componente reusable de panel lateral
colapsable para la capa UI. El comportamiento y la apariencia deben replicar el
panel de herramientas usado en `GestionRespuesta` (workbench) sin acoplarse al
modulo. Existe una arquitectura maestra documentada en
`docs/Architecture/AppCollapseRail/AppCollapseRail-Architecture.md` que define
contratos, responsive y accesibilidad.

La propuesta generada automaticamente trae un naming incorrecto
(`AppAppcollapserail01Fe`), por lo que este diseno fija el nombre canonico
`AppCollapseRail` y la capability `app-collapse-rail`.

## Goals / Non-Goals

**Goals:**
- Definir la implementacion de `AppCollapseRail` en `src/app/Components/UI`.
- Mantener comportamiento colapsable y rail de restauracion.
- Soportar responsive: desktop inline, tablet colapsado por defecto, mobile overlay.
- Accesibilidad: `aria-expanded`, `aria-controls`, foco visible.
- Reutilizable y desacoplado (sin logica de negocio).

**Non-Goals:**
- Persistir estado en backend o localStorage.
- Implementar logica de permisos/roles.
- Reemplazar componentes de GestionRespuesta en esta etapa.

## Decisions

1. **Nombre y capability canonicos**
   - Decision: renombrar a `AppCollapseRail` y capability `app-collapse-rail`.
   - Razon: alinear naming con la convención del repo (`AppTabs`, `AppUpload`).
   - Alternativas: mantener `AppAppcollapserail01Fe` (rechazada por inconsistencia).

2. **Control de estado externo**
   - Decision: `collapsed` y `onToggle` controlados desde el contenedor.
   - Razon: permite integracion flexible con reglas de negocio del modulo.
   - Alternativas: estado interno (rechazada por menor control).

3. **No desmontar contenido**
   - Decision: el panel se oculta visualmente (opacity/transform) pero no se desmonta.
   - Razon: preservar estado interno de herramientas.
   - Alternativas: desmontar para performance (rechazada por perdida de estado).

4. **Responsive con variantes**
   - Decision: `variant="inline"` (desktop) y `variant="overlay"` (mobile).
   - Razon: misma UX de GestionRespuesta y control explicito por CSS.
   - Alternativas: media queries implicitas sin variant (menos control).

5. **Rail flotante como chip en mobile**
   - Decision: rail con label visible solo en mobile.
   - Razon: mejorar descubribilidad en pantallas pequenas.
   - Alternativas: solo icono (menos claro).

## Risks / Trade-offs

- [Duplicacion de estilos con GestionRespuesta] -> Mitigacion: extraer tokens y
  replicar solo lo necesario en CSS Modules del componente.
- [Overlay en mobile puede cubrir contenido] -> Mitigacion: limite de altura y
  rail visible siempre.
- [Colapso mal sincronizado] -> Mitigacion: estado controlado + tests de toggle.

## Migration Plan

- No requiere migracion ni cambios en backend.
- Introducir componente en UI shared.
- Integraciones futuras pueden reemplazar paneles existentes.

## Open Questions

- Confirmar si el panel debe soportar `placement="left"` desde el inicio.
- Definir si se requiere persistencia de estado por usuario en el futuro.
