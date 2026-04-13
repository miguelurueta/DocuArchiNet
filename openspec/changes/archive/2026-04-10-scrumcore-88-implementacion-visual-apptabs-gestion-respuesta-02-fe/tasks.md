## 1. Definicion de items en GestionRespuesta

- [x] 1.1 Definir `AppTabItem[]` con al menos 2 tabs coherentes y keys unicas
- [x] 1.2 Mantener el boton "Volver a la bandeja" visible fuera del contenido de tabs
- [x] 1.3 Agregar contenido base para cada tab segun el contexto actual

## 2. Estilos locales y responsive

- [x] 2.1 Ajustar estilos via CSS Modules del modulo (`src/modules/gestionCorrespondencia/style/`)
- [x] 2.2 Validar overflow horizontal en mobile sin estilos globales

## 3. Pruebas y evidencia

- [x] 3.1 Ajustar tests para validar render de tabs y bloqueo `disabled` si aplica
- [x] 3.2 Registrar evidencia de ejecucion de tests en el change OpenSpec

Evidencia de tests:
- `npm.cmd test -- GestionCorrespondenciaRoute.spec.test.tsx` (OK, 5 tests, 2026-04-10)
