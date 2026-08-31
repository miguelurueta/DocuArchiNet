## Why

Las E2E Workflow repiten por DOC la captura TTY, login, TLS local, controles ODBC, evidencia y cierre seguro, lo que eleva el tiempo de construcción y propaga fallos de infraestructura. Se necesita una plataforma declarativa que haga reutilizable esa infraestructura y deje a cada DOC solo sus operaciones y expectativas funcionales.

## What Changes

- Incorporar un registro declarativo de escenarios, controles y recursos E2E que no acepte comandos, SQL ni secretos arbitrarios.
- Incorporar perfiles JSON no sensibles y un ejecutor común para etapas anónimas, de lectura, preview, ejecución, concurrencia y bloqueo UI.
- Centralizar TTY efímero, sesión Workflow, TLS local autorizado, ODBC de solo lectura, saneamiento de salida/evidencia y controles de cierre.
- Migrar `notes-read` como piloto sin reemplazar ni romper los comandos E2E existentes.
- Documentar para agentes y mantenedores cómo declarar y reutilizar un escenario en futuros DOC.

## Capabilities

### New Capabilities

- `workflow-e2e-platform`: Plataforma segura y declarativa para ejecutar escenarios E2E Workflow reutilizables con perfiles no sensibles, autorizaciones, recursos y evidencia saneada.

### Modified Capabilities

- Ninguna.

## Impact

- Afecta `tools/e2e` (registro, validadores, ejecutor, perfiles, comandos npm y pruebas de política).
- Reutiliza los helpers de sesión autenticada, consola interactiva, ODBC y orquestación existentes; no crea una infraestructura paralela.
- Agrega un adaptador piloto de Notas y documentación bajo `tools/e2e/E2E-TEST/`.
- No modifica páginas Web Forms, endpoints Workflow existentes, gates, contratos funcionales de DOC-41 ni comandos actuales de los DOC ya cubiertos.
