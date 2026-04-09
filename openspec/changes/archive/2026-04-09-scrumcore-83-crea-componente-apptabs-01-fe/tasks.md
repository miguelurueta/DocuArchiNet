## 1. Preparacion y estructura

- [x] 1.1 Verificar convencion de componentes en `src/app/Components/UI`
- [x] 1.2 Crear carpeta `src/app/Components/UI/AppTabs/`
- [x] 1.3 Definir tipos `AppTabItem` y `AppTabsProps` sin `any`
- [x] 1.4 Exportar `AppTabs` en `src/app/Components/UI/index.ts`

## 2. Core y contrato (wrapper AntD)

- [x] 2.1 Implementar wrapper base de `Tabs` (sin acoplar a vistas ni APIs)
- [x] 2.2 Implementar controlado vs no controlado: `activeKey` domina y anula `defaultActiveKey`, no mezclar ambos modos
- [x] 2.3 Implementar bloqueo `disabled`: no click ni teclado, no ejecutar `onChange`, no cambiar `activeKey`
- [x] 2.4 Implementar `beforeChange` (sync/async) y bloquear cambio si retorna `false`
- [x] 2.5 Definir `mapToAntdItems(items)` sin mutar `items`
- [x] 2.6 Mapear icono + badge + label dentro del mapper

## 3. Accesibilidad concreta

- [x] 3.1 Mantener `role="tablist"` en el contenedor principal (sin duplicar roles)
- [x] 3.2 Manejo de foco programatico al cambiar de tab
- [x] 3.3 Tabs disabled con `aria-disabled="true"` y sin focus por teclado

## 4. Riesgos a evitar (validacion)

- [x] 4.1 Confirmar no mezclar controlado/no controlado
- [x] 4.2 Confirmar no ejecutar `onChange` cuando `disabled` o `beforeChange` bloquea
- [x] 4.3 Confirmar no mutar `items` en el mapper
- [x] 4.4 Confirmar no usar `any` ni estilos globales

## 5. Pruebas unitarias y evidencia

- [x] 5.1 Test: respeta `activeKey` controlado
- [x] 5.2 Test: `defaultActiveKey` en modo no controlado
- [x] 5.3 Test: `disabled` bloquea click/teclado y no ejecuta `onChange`
- [x] 5.4 Test: `beforeChange` bloquea cambios
- [x] 5.5 Ejecutar tests y registrar evidencia

Evidencia: `npx vitest --run src/app/Components/UI/AppTabs/AppTabs.test.tsx` (2026-04-09).

## 6. Criterios de aceptacion

- [x] 6.1 Componente reusable en `src/app/Components/UI/AppTabs`
- [x] 6.2 Contrato estable y tipado estricto
- [x] 6.3 Bloqueo por `disabled` y `beforeChange` funcional
