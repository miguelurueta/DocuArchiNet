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
- mover a mini el análisis, la preparación y la implementación pequeña siempre que no bloquee una decisión de integración

## Distinción Jira y Git

Usar subagente mini para:

- lectura de tickets Jira
- consulta de estados, comentarios y trazabilidad
- consulta de ramas, commits y PRs
- preparación de mensajes de commit, PR o comentario
- pruebas focales y creación de carpetas o artefactos locales
- implementaciones pequeñas o fixes focales con alcance acotado y write scope claro

Usar agente principal por defecto para:

- cambios de estado en Jira
- creación o cierre de PRs
- commit final, push final y merge
- `opsxj:archive`
- `opsxj:close`

Mini puede conectarse técnicamente a Jira y Git, pero como política del flujo solo se recomienda para consulta y preparación. La escritura y el cierre de flujo quedan en principal salvo que exista una razón puntual para delegarlo.

## Recomendación por comando

### `opsxj:new`

Usar subagente mini para:

- leer y resumir ticket Jira
- redactar `design.md`
- redactar `spec.md`
- redactar `tasks.md`
- revisar referencias del repo necesarias para el cambio
- crear carpetas o artefactos locales del cambio
- aplicar cambios pequeños en archivos acotados cuando la responsabilidad este bien delimitada

Usar agente principal para:

- validar coherencia global del cambio
- decidir alcance final de implementación
- integrar cambios transversales o sensibles del ticket

### `opsxj:archive`

Usar subagente mini para:

- revisión acotada de artefactos OpenSpec
- chequeos focales de pruebas o archivos específicos
- preparación de notas de trazabilidad
- consulta de estado remoto en GitHub/Jira
- diagnosticar fallos puntuales de build o tests y proponer fix acotado

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
3. mini para implementación pequeña y chequeos focales cuando el cambio sea acotado
4. principal para integración o implementación transversal
5. mini para chequeos focales si hacen falta
6. principal para `opsxj:archive`
7. principal para `opsxj:close`

## Objetivo

Reducir consumo del agente principal en fases documentales y de exploración, manteniendo la calidad en integración, archive y cierre.
