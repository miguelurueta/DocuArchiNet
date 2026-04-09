## 1. Validacion de contrato y documentacion

- [x] 1.1 Revisar `AppTabs` existente y confirmar API estable con fase 02
- [x] 1.2 Actualizar README de `AppTabs` con iconos, badges, variantes, tamanos y responsive
- [x] 1.3 Verificar exportaciones publicas en `src/app/Components/UI/index.ts`

## 2. Ajustes funcionales (si aplica)

- [x] 2.1 Alinear nombres/props con el spec `app-apptabs-02-fe`
- [x] 2.2 Implementar iconos + badges con `Badge` y layout del label
- [x] 2.3 Implementar variantes `default|card|underline|pills` y tamanos `sm|md|lg` usando tokens CSS
- [x] 2.4 Implementar responsive: desktop, tablet con spacing reducido, mobile con overflow-x
- [x] 2.5 Implementar overflow `more` con label "Mas" + contador (+N) y dropdown alineado a la derecha
- [x] 2.6 Validar que `beforeChange` y `disabled` bloqueen cambios segun spec
- [x] 2.7 Aplicar feedback visual disabled (opacidad, cursor not-allowed, sin hover)
- [x] 2.8 Optimizar performance visual: memo de items y evitar rerender si no cambia `activeKey`

## 3. Pruebas y evidencia

- [x] 3.1 Agregar/ajustar tests de comportamiento con tag `[SPEC:APP-TABS-002]`
- [x] 3.2 Test: renderiza iconos y badges
- [x] 3.3 Test: aplica clase `customTabs`
- [x] 3.4 Test: estado visual disabled
- [x] 3.5 Ejecutar pruebas `npx vitest --run src/app/Components/UI/AppTabs/AppTabs.test.tsx`
- [x] 3.6 Registrar evidencia de tests en `tasks.md`

Evidencia: `npx vitest --run src/app/Components/UI/AppTabs/AppTabs.test.tsx` (2026-04-09).
