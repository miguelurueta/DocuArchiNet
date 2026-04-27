## 1. Contrato FE-10 (spec) y definicion de alcance

- [ ] 1.1 Confirmar el spec delta `app-appeditorpdf-10-fe` (toolbar responsive sin toggle de tema).
- [ ] 1.2 Alinear el alcance con el codigo actual: `AppEditorToolbar` ya tiene modo compacto y wrapping CSS.

## 2. Responsividad basada en contenedor

- [ ] 2.1 Implementar deteccion de modo compacto en `AppEditorToolbar` usando `ResizeObserver` sobre el contenedor de toolbar.
- [ ] 2.2 Mantener fallback a `window.innerWidth` cuando `ResizeObserver` no este disponible.
- [ ] 2.3 Verificar que el layout con `toolbarActions` externos siga siendo estable (no rompe orden ni accesibilidad).

## 3. Sin toggle de tema (por defecto)

- [ ] 3.1 Confirmar que `AppEditorPdf` no renderiza ningun control de theme toggle por defecto.
- [ ] 3.2 Mantener compatibilidad con `themeMode` / `defaultThemeMode` (solo control externo).

## 4. Pruebas y validacion

- [ ] 4.1 Agregar pruebas FE-10 para modo compacto por ancho reducido (mock `ResizeObserver`).
- [ ] 4.2 Agregar prueba que confirme ausencia de toggle de tema en toolbar por defecto.
- [ ] 4.3 Ejecutar `npm.cmd run test -- --run` y `npm.cmd run spec:validate`, dejando evidencia en el cambio.

