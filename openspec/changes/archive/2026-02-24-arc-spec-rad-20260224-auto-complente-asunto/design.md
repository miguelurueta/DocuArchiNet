## Context

El formulario de radicacion renderiza campos dinamicos desde `camposPlantilla` y hoy el campo `ASUNTO` existe como control manual (`data-ident="pl-radicacion-spe-ASUNTO"`). El ticket `SCRUM-8` requiere usar la metadata de plantilla (`name_campo = "ASUNTO"`) para habilitar autocompletado via API `POST/GET /api/PlantillaRadicado/solicitaAutoCompleteCampos` sin cambiar el flujo general del formulario.

## Goals / Non-Goals

**Goals:**
- Resolver el campo `ASUNTO` desde la estructura `camposPlantilla` y activar comportamiento de autocompletado.
- Reutilizar el mecanismo de autocompletado existente del modulo de radicacion para mantener coherencia.
- Preservar validaciones, atributos declarativos y comportamiento de envio del formulario.
- Cubrir el comportamiento con pruebas Vitest + Testing Library.

**Non-Goals:**
- Rediseñar la arquitectura del renderer dinamico de campos.
- Cambiar contratos backend o crear endpoints nuevos.
- Alterar otros campos fuera de `ASUNTO`.

## Decisions

### Decision: resolver configuracion de ASUNTO desde metadata de plantilla
Se tomara el registro de `camposPlantilla` cuyo `name_campo` sea `ASUNTO` para construir placeholder, tooltip, reglas y parametros de consulta del autocompletado.

Alternativas consideradas:
- Hardcodear configuracion de ASUNTO en el formulario: descartado por romper el enfoque declarativo.
- Crear un mapper separado solo para ASUNTO: descartado por duplicar logica de resolucion.

### Decision: reutilizar hook/renderer de autocompletado ya existente
En lugar de un componente ad-hoc, se extiende el flujo actual de renderizado dinamico para que `ASUNTO` use el mismo patron de consumo API, manejo de estado y seleccion de opciones.

Alternativas consideradas:
- Implementar un autocomplete independiente con estado local: descartado por mayor deuda tecnica y divergencia de UX.
- Resolverlo solo en submit (sin sugerencias en UI): descartado por no cumplir requisito funcional del ticket.

### Decision: manejar error de API con degradacion segura
Si falla la consulta de autocompletado, el usuario mantiene posibilidad de escribir el `ASUNTO` manualmente y se muestra mensaje amigable no bloqueante.

Alternativas consideradas:
- Bloquear el campo ante error: descartado por impacto operativo.
- Silenciar errores: descartado por baja observabilidad y mala UX.

## Risks / Trade-offs

- [Dependencia de metadata inconsistente en plantillas] -> Mitigacion: fallback a modo manual cuando no existe `name_campo=ASUNTO`.
- [Incremento de llamadas por tipeo] -> Mitigacion: respetar debounce/umbral existente del flujo de autocompletado.
- [Regresiones en renderer dinamico] -> Mitigacion: tests de comportamiento enfocados en `ASUNTO` y en no regresion de flujo actual.

## Migration Plan

1. Extender resolucion de campo dinamico para detectar `ASUNTO` desde `camposPlantilla`.
2. Conectar `ASUNTO` al flujo de autocompletado con endpoint `solicitaAutoCompleteCampos`.
3. Validar fallback manual ante errores o ausencia de metadata.
4. Ejecutar pruebas del modulo radicacion y registrar evidencia en `tasks.md`.
5. Rollback: revertir cambios de renderer/hook y mantener campo `ASUNTO` en modo manual.

## Open Questions

- Confirmar estructura exacta de respuesta del endpoint para `ASUNTO` (label/value) cuando haya resultados vacios.
- Confirmar longitud minima de texto para disparar consulta si no existe regla ya definida en metadata.
