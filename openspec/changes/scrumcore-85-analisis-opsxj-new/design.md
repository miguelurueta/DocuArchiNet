## Context

`opsxj:new` resuelve hoy tres pasos distintos en una sola operacion: lee Jira, crea una carpeta OpenSpec y genera un `proposal.md` inicial. El problema no esta en la orquestacion Git/Jira sino en la calidad del proposal resultante: cuando la inferencia falla, el cambio arranca con capabilities artificiales o texto heredado que obliga a corregir manualmente el artifacto antes de poder continuar el refinement.

El repositorio ya tiene evidencia de la intencion correcta del flujo:
- `docs/opsxj-new.md` define que el resumen Jira debe ser la fuente de la carpeta y del proposal.
- `scripts/lib/proposalGenerator.test.js` y `scripts/lib/jiraProposalService.test.js` ya exigen que el contenido final se alinee al ticket y no a `jira-proposal-generator`.

El diseno de este ticket no busca corregir el generador todavia. Su objetivo es dejar el diagnostico tecnico ordenado: donde falla la inferencia, que evidencia ya existe en el repo y que restricciones deben respetarse cuando se abra el ticket de correccion.

## Goals / Non-Goals

**Goals:**
- Diagnosticar por que `opsxj:new` produce proposals genericos o capabilities artificiales en ciertos tickets.
- Delimitar las capas implicadas (`proposalGenerator`, `jiraProposalService`, docs y pruebas actuales).
- Dejar un marco claro para un cambio posterior de correccion sin tocar comportamiento productivo en este ticket.
- Identificar evidencia existente en tests y documentacion que ya contradice el comportamiento observado.

**Non-Goals:**
- No corregir `opsxj:new` ni `jira-proposal-generator` en este ticket.
- No modificar heuristicas, tests, scripts ni documentacion operativa como parte de la ejecucion.
- No introducir IA adicional ni dependencias nuevas para redactar proposals.

## Decisions

### 1. Tratar el cambio como una modificacion de `jira-proposal-generator`

La capability real ya existe en el repo y cubre exactamente este problema: generacion automatica de propuestas desde Jira. Crear una capability nueva para `opsxj:new` fragmentaria el modelo y ocultaria que el defecto esta en la inferencia/contenido del proposal, no en el comando como orchestration shell.

Alternativas consideradas:
- Crear una capability nueva `opsxj-new`: descartado porque duplicaria el alcance ya cubierto por `jira-proposal-generator`.
- Tratarlo como mera documentacion: descartado porque el repositorio ya tiene pruebas y codigo que deben cambiar para corregir el resultado.

### 2. Analizar la inferencia antes de proponer cualquier correccion

El diagnostico debe centrarse en la capa que interpreta `summary` y `description` (`proposalGenerator` / `jiraProposalService`), porque ahi es donde probablemente nace el proposal generico. Todavia no corresponde decidir la implementacion final; corresponde aislar la fuente del defecto y explicitar por que un postprocesamiento posterior seria una mala direccion.

Alternativas consideradas:
- Reescribir `proposal.md` despues de generado: se documenta como alternativa debil porque duplica logica y haria mas dificil razonar sobre la fuente de verdad.
- Permitir proposal generico y depender del refinement manual: se documenta como comportamiento insuficiente porque degrada el objetivo de `opsxj:new`.

### 3. Basar el diagnostico en tickets y pruebas reales del repo

El repositorio ya contiene ejemplos de tickets como `CREA-COMPONENTE-TOOLBAR` y varios cambios archivados donde la capability final fue corregida manualmente. Esos casos deben usarse como evidencia del problema y como insumo para el ticket posterior, sin convertirlos todavia en cambios de comportamiento.

Alternativas consideradas:
- Usar una heuristica completamente abierta basada en palabras sueltas: descartado porque aumenta falsos positivos y hace el resultado menos predecible.
- Basarse solo en descripcion Jira: descartado porque muchos tickets tienen descripciones pobres o ausentes; el resumen ya es la señal principal del naming del flujo.

## Risks / Trade-offs

- [El diagnostico derive en cambios prematuros] -> Mitigacion: dejar explicito en proposal, design y tasks que este ticket no modifica tooling.
- [Evidencia insuficiente para un ticket posterior] -> Mitigacion: referenciar codigo, docs y pruebas ya presentes en el repo.
- [Confusion entre bug del comando y bug del generador] -> Mitigacion: separar orquestacion `opsxj:new` de la inferencia que produce `proposal.md`.

## Migration Plan

1. Revisar implementacion actual de `proposalGenerator` y `jiraProposalService`.
2. Levantar los patrones de falla observables y los casos del repo que los evidencian.
3. Consolidar recomendaciones para un ticket posterior de correccion.
4. Cerrar este ticket sin cambios funcionales en scripts, tests ni documentacion.

Rollback:
- No aplica rollback funcional porque este ticket no debe introducir cambios de comportamiento.

## Open Questions

- Si no hay señal suficiente en `summary` ni `description`, el ticket posterior debe decidir si el fallback sigue siendo generico o si debe fallar con una salida mas explicita.
- El ticket posterior debe confirmar la prioridad exacta entre `summary` y `description`, pero este analisis ya deja indicado que el `summary` es hoy la mejor señal operativa.
