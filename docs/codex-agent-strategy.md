# Política de agentes Codex para flujo `opsxj`

Este documento define una política operativa para usar agentes Codex de menor costo cuando convenga, sin acoplar esa decisión al runtime del repositorio.

## Límite técnico

Los comandos:

- `opsxj:new`
- `opsxj:archive`
- `opsxj:close`

son scripts locales del repositorio. No controlan el modelo o el tipo de agente que usa Codex. Esa decisión pertenece a la sesión externa de Codex que ejecuta el flujo.

Por tanto, esta política es una guía operativa y no un enforcement técnico del script.

## Regla general

- usar subagente mini para tareas acotadas, repetibles o de bajo riesgo
- usar agente principal para integración, validación final y decisiones sensibles

## Recomendación por comando

### `opsxj:new`

Usar subagente mini para:

- leer y resumir ticket Jira
- redactar `design.md`
- redactar `spec.md`
- redactar `tasks.md`
- revisar referencias del repo necesarias para el cambio

Usar agente principal para:

- validar coherencia global del cambio
- decidir alcance final de implementación
- iniciar implementación real del ticket

### `opsxj:archive`

Usar subagente mini para:

- revisión acotada de artefactos OpenSpec
- chequeos focales de pruebas o archivos específicos
- preparación de notas de trazabilidad

Usar agente principal para:

- verify del cambio
- revisión final del diff
- ejecución de archive
- coordinación del PR y validación de checks

### `opsxj:close`

Usar agente principal para:

- validar merge del PR correcto
- ejecutar cierre Jira
- confirmar sincronización final del flujo

No se recomienda delegar `opsxj:close` a mini salvo consulta puntual de contexto.

## Flujo recomendado

1. `opsxj:new`
2. mini para artefactos OpenSpec
3. principal para implementación
4. mini para chequeos focales si hacen falta
5. principal para `opsxj:archive`
6. principal para `opsxj:close`

## Objetivo

Reducir consumo del agente principal en fases documentales y de exploración, manteniendo la calidad en integración, archive y cierre.
