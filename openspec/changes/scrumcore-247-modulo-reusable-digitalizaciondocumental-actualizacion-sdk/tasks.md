## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira + contexto de codigo.
- [x] 1.2 Ajustar design/spec con decisiones y riesgos definitivos.

## 2. Implementacion

- [x] 2.1 Actualizar integracion Dynamsoft de `dwt@18.5.0` a `dwt@19.3.2`.
- [x] 2.2 Mantener compatibilidad y evitar regresiones en adapter, workspace, modal y AppDigitalizador.

## 3. Pruebas

- [x] 3.1 Agregar/ajustar pruebas unitarias e integracion.
- [x] 3.2 Ejecutar suite afectada y registrar evidencia.

## 4. Cierre

- [x] 4.1 Validar OpenSpec.
- [x] 4.2 Documentar diff final y decisiones de arquitectura.

## Evidence

- `npx tsc --noEmit`: PASS.
- `npx eslint src/modules/digitalizacion/infrastructure/dynamsoft src/modules/digitalizacion/tests/loadDynamsoftScripts.test.ts src/modules/digitalizacion/tests/DynamsoftTwainClient.test.ts --ext .ts,.tsx`: PASS.
- `npx vitest run src/modules/digitalizacion/tests/loadDynamsoftScripts.test.ts src/modules/digitalizacion/tests/DynamsoftTwainClient.test.ts`: PASS, 14 tests.
- `npx vitest run src/app/Components/UI/AppDigitalizador`: PASS, 3 tests.
- `npx vitest run src/modules/digitalizacion`: PASS, 53 tests.
