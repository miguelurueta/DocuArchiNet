## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira + contexto de codigo.
- [x] 1.2 Ajustar design/spec con decisiones y riesgos definitivos.

## 2. Implementacion

- [x] 2.1 Implementar cambios funcionales del ticket.
- [x] 2.2 Mantener compatibilidad y evitar regresiones.

## 3. Pruebas

- [x] 3.1 Agregar/ajustar pruebas unitarias e integracion.
- [x] 3.2 Ejecutar suite afectada y registrar evidencia.

## 4. Cierre

- [ ] 4.1 Validar OpenSpec. Bloqueado localmente: `npx openspec validate scrumcore-264-modulo-reusable-digitalizaciondocumental-captura --strict` falla con `could not determine executable to run`.
- [x] 4.2 Documentar diff final y decisiones de arquitectura.

## Evidencia

- `npx vitest run src/modules/digitalizacion/tests/DynamsoftTwainClient.test.ts --reporter=verbose`: OK, 27 tests.
- `npx vitest run src/modules/digitalizacion/components/DigitalizacionDocumentalModal/DigitalizacionDocumentalModal.test.tsx --reporter=dot`: OK, 11 tests. Advertencias jsdom/Ant Design conocidas: `getComputedStyle` con pseudo-elementos y act warnings.
- `npx tsc --noEmit`: OK.
- `npx eslint <archivos tocados SCRUMCORE-264>`: OK.
- `npm run lint`: falla por deuda existente fuera del cambio, 89 errores y 36 warnings en archivos no tocados como `AppEditor`, `AppSteps`, `RadicacionForm`, `useGestionCorrespondenciaTable`, entre otros.
