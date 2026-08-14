## Why

OPSXJ valida impacto, trazabilidad y perfiles, pero no puede demostrar que los archivos nuevos respetan una nomenclatura física, límites de capa y frontera legacy consistentes. Se necesita una convención ejecutable para que cada cambio nuevo declare su módulo y caso de uso, y para bloquear desvíos antes de publicar o archivar.

## What Changes

- Incorporar un catálogo versionado de rutas y restricciones arquitectónicas para DTOs, servicios, modelos, infraestructura, repositorios y componentes compartidos.
- Extender `opsxj:orchestrate:new` para registrar el módulo, caso de uso y manifiesto arquitectónico de los cambios nuevos, sin imponer requisitos retroactivos a cambios históricos.
- Extender refinement y la revisión técnica para comprobar coherencia entre prompt, design, spec, tasks y el manifiesto de arquitectura.
- Incorporar una validación de repositorio que compare los archivos nuevos o modificados con el manifiesto y aplique excepciones explícitas, acotadas y auditables.
- Integrar estos controles en `opsxj:validate` y documentar la convención, comandos y reglas de compatibilidad.

## Capabilities

### New Capabilities

### Modified Capabilities

- `legacy-opsxj-governance`: amplía el gobierno local con convención de estructura modular, manifiesto arquitectónico y compuertas de validación de rutas y dependencias.

## Impact

- `tools/opsxj/`: catálogo, creación de manifiestos, refinement, revisión técnica, validación, CLI, pruebas Vitest y README.
- `tools/validation/`: validación estructural reutilizable del repositorio.
- `Doc/Arquitectura/convenciones/`: norma técnica versionada.
- `openspec/changes/`: manifiestos de arquitectura para cambios nuevos; los históricos conservan compatibilidad.
