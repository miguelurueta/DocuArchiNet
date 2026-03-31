## Context

El modulo GestionCorrespondencia usa un toolbar con variables CSS definidas en `GestionCorrespondencia.module.css`. El ticket solicita cambiar el color de superficie del toolbar a blanco.

## Goals / Non-Goals

**Goals:**
- Ajustar `--toolbar-surface` a `white` en el estilo del toolbar.
- Mantener el resto de estilos existentes.

**Non-Goals:**
- Cambiar layout o estructura del toolbar.
- Introducir nuevas dependencias o cambios de comportamiento.

## Decisions

- **Cambio minimo de CSS**: solo se actualiza el valor de la variable `--toolbar-surface` para evitar impactos colaterales.

## Risks / Trade-offs

- [Cambio visual no alineado con diseño global] -> Mitigacion: validar contraste y coherencia visual en UI.

## Migration Plan

- Cambio local en CSS, sin migraciones.
- Rollback: revertir el valor de la variable.

## Open Questions

- Confirmar si el color blanco debe aplicarse tambien a otros toolbars del modulo.
