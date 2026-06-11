## Why

SCRUMCORE-246 formaliza la entrada corporativa reutilizable para digitalizacion documental. Los tickets SCRUMCORE-239 a SCRUMCORE-245 dejaron implementado `src/modules/digitalizacion`, incluyendo `DigitalizacionDocumentalWorkspace`, `DigitalizacionDocumentalModal`, hooks, servicios y `DynamsoftTwainClient`; este cambio no debe rehacer esa capa ni duplicar su logica.

## What Changes

- Crear `AppDigitalizador` como componente publico unico en `src/app/Components/UI/AppDigitalizador`.
- Construir `AppDigitalizador` encima de `DigitalizacionDocumentalWorkspace`.
- Encapsular creacion de `scannerClient`, configuracion Dynamsoft/licencia, `apiClient`, defaults corporativos, modulo origen y callbacks simplificados.
- Crear sandbox visual en `/__sandbox/app-digitalizador` que monte `<AppDigitalizador />`.
- Mantener compatibilidad con `DigitalizacionDocumentalModal`, `DigitalizacionDocumentalWorkspace`, hooks, servicios, adapter Dynamsoft y contratos existentes.
- Documentar API publica, arquitectura final, pendientes y evidencia de validacion.

## Non-Goals

- No reimplementar SCRUMCORE-239 a SCRUMCORE-245.
- No montar `DigitalizacionDocumentalWorkspace` directamente en la sandbox.
- No exponer `DWObject`, adapters, orchestrator ni infraestructura Dynamsoft a modulos consumidores.
- No resolver bloqueos backend pendientes de la auditoria legacy.

## Impact

- Nuevo componente UI corporativo reutilizable.
- Nuevo provider opcional para configuracion corporativa compartida.
- Nueva pagina sandbox de pruebas funcionales con scanner fisico.
- Export publico desde `src/app/Components/UI`.
