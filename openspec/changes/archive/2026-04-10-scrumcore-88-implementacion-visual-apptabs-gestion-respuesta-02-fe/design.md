## Context

El cambio `SCRUMCORE-88` continua la implementacion visual de `AppTabs` en la vista
`GestionRespuesta`. El ticket Jira es breve y requiere refinar el diseño de tabs
(separacion, bordes, hover) y mantener el boton de retorno visible en el header.
La base tecnica ya existe en `AppTabs` con CSS Modules y uso de Ant Design Tabs.

## Goals / Non-Goals

**Goals:**
- Ajustar la estetica de `AppTabs` en `GestionRespuesta` (espaciado, bordes superiores/laterales, hover sutil).
- Mantener el boton "Volver a la bandeja" visible y fuera del contenido de tabs.
- Preservar accesibilidad y comportamiento responsive de tabs.

**Non-Goals:**
- Cambios de backend o integraciones API.
- Reestructurar rutas o el layout master-detail.
- Crear nuevos componentes fuera de `AppTabs` o cambiar su API publica.

## Decisions

- Aplicar los ajustes visuales via `AppTabs.module.css` para mantener consistencia y aislamiento.
  - Alternativa: estilos locales por modulo. Rechazado para evitar divergencia.
- Ajustar solamente variantes `default`/`underline` en el componente reutilizable.
  - Alternativa: estilos ad-hoc en `GestionRespuesta`. Rechazado por consistencia global.
- Mantener el boton de retorno en el header del detail shell y no dentro de tabs.
  - Alternativa: mover el boton al contenido del tab. Rechazado por perder persistencia al cambiar tab.

## Risks / Trade-offs

- [Riesgo] Overwrite de estilos AntD podria afectar otras vistas → Mitigacion: scoping via CSS Modules y clases del componente.
- [Riesgo] Ajustes de borde/espaciado generan regresion visual en pantallas chicas → Mitigacion: validar responsive y mantener `fullWidth`.
