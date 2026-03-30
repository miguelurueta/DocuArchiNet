## 1. Refinamiento del cambio visual

- [x] 1.1 Corregir la propuesta generada desde Jira para describir el ajuste real de `AppContent` con altura restante y scroll interno
- [x] 1.2 Definir la capability del cambio alrededor del layout vertical del modulo y no de un cambio visual generico

## 2. Ajuste estructural del layout

- [x] 2.1 Revisar el contenedor padre de `AppToolbar` y `AppContent` para confirmar si ya usa flexbox correctamente
- [x] 2.2 Convertir `GestionCorrespondenciaLayout` y `.page` a una cadena flex vertical con `min-height: 0` y `overflow: hidden`
- [x] 2.3 Ajustar `AppContent` para usar `flex: 1`, `min-height: 0` y scroll interno vertical en su cuerpo

## 3. Verificacion de no regresion

- [x] 3.1 Confirmar que no se modifica logica de navegacion, toolbar ni drawer
- [x] 3.2 Ejecutar la prueba de ruta de `GestionCorrespondencia` como smoke test del ajuste estructural
- [x] 3.3 Registrar evidencia de prueba en la documentacion OpenSpec

## Test Evidence

- `npm test -- src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx`
- Result: 1 test file passed, 2 tests passed, 0 failed.
