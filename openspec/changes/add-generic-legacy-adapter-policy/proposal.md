## Why

Las modernizaciones del monolito legacy pueden acoplar código nuevo directamente a clases `Class*`, `Session`, controles Web Forms y métodos de cambio de estado. Sin una política generada y validable, cada ticket interpreta ese límite de forma distinta y aumenta el riesgo de regresiones funcionales y dependencias irreversibles.

## What Changes

- Extender `opsxj:new` —y agregar su alias explícito `opsxj:orchestrate:new`— para aceptar un perfil de arquitectura de modernización legacy.
- Incorporar el perfil general `enterprise-legacy-modernization` en los artefactos OpenSpec generados: propuesta, diseño, especificación y tareas.
- Exigir para modernizaciones que el código nuevo acceda a capacidades legacy mediante Gateways/Adapters tipados por dominio, en lugar de hacerlo directamente desde Presentation o Application.
- Establecer requisitos reutilizables de separación por capas, infraestructura de datos compartida, repositorios específicos de dominio, consultas parametrizadas, compatibilidad, piloto y rollback.
- Persistir el perfil, el contrato técnico documental y sus requisitos verificables en el manifiesto de gobierno para que `opsxj:validate` aplique una validación bloqueante de existencia, estructura, contenido mínimo y cierre documental.

## Capabilities

### New Capabilities

- Ninguna.

### Modified Capabilities

- `legacy-opsxj-governance`: el gobierno de `opsxj` incorporará perfiles de arquitectura reutilizables que se siembran al crear un cambio y se validan durante su ciclo de vida.

## Impact

- `tools/opsxj/package.json` y el registro de comandos de `opsxj`.
- Generación de `proposal.md`, `design.md`, `spec.md`, `tasks.md` y `opsxj-governance.json`.
- Validación local de gobierno y sus pruebas automatizadas.
- Los cambios existentes e históricos conservarán compatibilidad: el perfil solo aplicará cuando se solicite explícitamente en cambios nuevos.
