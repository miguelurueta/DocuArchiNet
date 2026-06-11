## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira + contexto de codigo.
- [x] 1.2 Ajustar design/spec con decisiones y riesgos definitivos.

## 2. Implementacion

- [x] 2.1 Implementar `AppDigitalizador` como fachada corporativa sobre `DigitalizacionDocumentalWorkspace`.
- [x] 2.2 Mantener compatibilidad y evitar regresiones en modal, workspace, hooks, servicios y adapter Dynamsoft.
- [x] 2.3 Crear sandbox visual que monte `<AppDigitalizador />`.

## 3. Pruebas

- [x] 3.1 Agregar pruebas de integracion focales para `AppDigitalizador`.
- [x] 3.2 Ejecutar suite afectada y registrar evidencia.

## 4. Cierre

- [x] 4.1 Validar OpenSpec localmente segun artefactos disponibles.
- [x] 4.2 Documentar diff final, API publica y decisiones de arquitectura.

## Evidence

- `npx eslint src/app/Components/UI/AppDigitalizador src/app/pages/AppDigitalizadorSandboxPage.tsx src/modules/digitalizacion --ext .ts,.tsx`: PASS.
- `npx tsc --noEmit`: PASS.
- `npx vitest run src/modules/digitalizacion`: PASS, 9 files, 50 tests.
- `npx vitest run src/app/Components/UI/AppDigitalizador`: PASS, 1 file, 3 tests.
