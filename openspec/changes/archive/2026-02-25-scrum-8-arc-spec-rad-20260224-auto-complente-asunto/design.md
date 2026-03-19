## Context

El ticket `SCRUM-8` solicita habilitar autocompletado para el campo `ASUNTO` en radicacion, usando la metadata disponible en `camposPlantilla` (`name_campo = "ASUNTO"`) y la API `/api/PlantillaRadicado/solicitaAutoCompleteCampos`. Actualmente el campo existe en el formulario pero su comportamiento no esta estandarizado con el flujo declarativo de campos dinamicos.

## Goals / Non-Goals

**Goals:**
- Resolver `ASUNTO` desde `camposPlantilla` para conservar enfoque declarativo.
- Integrar `ASUNTO` al flujo de autocompletado existente en radicacion.
- Mantener fallback de ingreso manual ante error de API.
- Cubrir escenarios con pruebas de comportamiento.

**Non-Goals:**
- Cambiar contratos backend o endpoint de autocompletado.
- Rediseñar el formulario completo de radicacion.
- Modificar comportamientos no relacionados a `ASUNTO`.

## Decisions

### Decision: reutilizar infraestructura existente de autocompletado
Se reutilizara `CampoPlantillaAutoCompleteField` y `useAutocompleteCamposPlantilla` para `ASUNTO`, en lugar de crear un componente ad-hoc.

Alternativas:
- Crear componente exclusivo para `ASUNTO`: descartado por duplicar logica y aumentar mantenimiento.
- Resolver sugerencias fuera del renderer dinamico: descartado por inconsistencias con otros campos.

### Decision: resolver el campo por metadata (`name_campo = "ASUNTO"`)
El formulario localizara el registro en `camposPlantilla` y renderizara el control con metadatos declarativos (tooltip, placeholder, required, disabled).

Alternativas:
- Hardcode de label/validaciones: descartado por perder trazabilidad con plantilla.

### Decision: evitar render duplicado de ASUNTO
Para evitar doble render, `ASUNTO` se excluira del renderer generico cuando se renderiza explicitamente en `RadicacionForm`.

Alternativas:
- Mantener doble render y filtrar por UI: descartado por errores de accesibilidad y testabilidad.

## Risks / Trade-offs

- [Metadata incompleta para ASUNTO] -> Mitigacion: fallback manual cuando no exista campo en plantilla.
- [Errores intermitentes de API] -> Mitigacion: mensaje amigable no bloqueante y campo editable.
- [Regresiones en renderer dinamico] -> Mitigacion: pruebas de no regresion + escenarios de ASUNTO.

## Migration Plan

1. Localizar `ASUNTO` en `camposPlantilla` dentro de `RadicacionForm`.
2. Renderizar `ASUNTO` con `CampoPlantillaAutoCompleteField` cuando exista metadata.
3. Excluir `ASUNTO` del renderer generico para evitar duplicados.
4. Agregar/actualizar pruebas `[SPEC:ASA-001]` y `[SPEC:ASA-002]`.
5. Ejecutar tests de radicacion y registrar evidencia en `tasks.md`.

## Open Questions

- Confirmar si la API de autocompletado para `ASUNTO` requiere longitud minima distinta a otros campos.
- Definir si `ASUNTO` debe persistir valor de etiqueta o identificador cuando backend entregue ambos.
