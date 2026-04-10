## 1. Preparacion y mapeo de contenido

- [x] 1.1 Revisar el contenido actual de `GestionRespuesta.tsx` y mapearlo a secciones de tabs
- [x] 1.2 Definir estructura inicial de `AppTabItem[]` con keys unicas y labels coherentes

## 2. Integracion de AppTabs y boton Volver

- [x] 2.1 Reemplazar el layout actual por `AppTabs` en `GestionRespuesta.tsx`
- [x] 2.2 Mantener el `AppButton` de "Volver a la bandeja" fuera de los `children` de tabs

## 3. Estilos y responsive

- [x] 3.1 Ajustar estilos locales del modulo (CSS Modules) si es necesario para spacing y layout
- [x] 3.2 Verificar que tabs y boton permanecen visibles en desktop y mobile

## 4. Pruebas y evidencia

- [x] 4.1 Agregar/ajustar tests de render de `AppTabs` y visibilidad del boton
- [x] 4.2 Registrar evidencia de ejecucion de tests en el cambio OpenSpec

Evidencia de tests:
- `npm.cmd test -- GestionCorrespondenciaRoute.spec.test.tsx` (OK, 5 tests, 2026-04-10)

Evidencia de layout:
- Inspeccion de layout y estilos: `AppTabs` con `fullWidth` para evitar width fijo y mantener responsive dentro de `detailBody`.
