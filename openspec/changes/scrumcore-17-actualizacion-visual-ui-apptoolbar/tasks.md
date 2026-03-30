## 1. Refinamiento del cambio visual

- [x] 1.1 Corregir la propuesta generada desde Jira para describir que `SCRUMCORE-17` solo ajusta la altura responsive del contenedor `AppToolbar`
- [x] 1.2 Definir la capability del cambio alrededor del comportamiento visual del toolbar en `GestionCorrespondencia`

## 2. Ajuste de layout del modulo consumidor

- [x] 2.1 Revisar el CSS actual de `GestionCorrespondencia` para detectar restricciones de altura, wrap u overflow en la toolbar
- [x] 2.2 Aplicar al contenedor `toolbar` reglas de flex-wrap, altura automatica, overflow visible y alineacion multilinea
- [x] 2.3 Ajustar el wrapper de acciones y el contenedor padre del modulo para no bloquear el crecimiento vertical del toolbar
- [x] 2.4 Corregir el breakpoint base de `AppToolbar` a `1100px` para evitar que `.context` y `.actions` mantengan `flex-basis` que inflen la altura del contenedor

## 3. Verificacion de no regresion

- [x] 3.1 Confirmar que no se modifica JSX, botones ni logica de navegacion del modulo
- [x] 3.2 Ejecutar la prueba de ruta de `GestionCorrespondencia` como smoke test del ajuste visual
- [x] 3.3 Registrar evidencia de prueba en la documentacion OpenSpec

## Test Evidence

- `npm test -- src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx`
- Result: 1 test file passed, 2 tests passed, 0 failed.
