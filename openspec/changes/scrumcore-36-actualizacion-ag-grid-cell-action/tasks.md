## 1. Renderer base de acciones dinámicas

- [x] 1.1 Crear `src/app/Components/UI/AppTable/renderers/AppTableActionCellRenderer.tsx`
- [x] 1.2 Definir un contrato de params compatible con AG Grid para recibir `appGridColumn`, `actions` y contexto mínimo de fila
- [x] 1.3 Renderizar acciones inline preservando el orden recibido desde backend
- [x] 1.4 Soportar visualmente `Presentation = icon_button` como caso mínimo
- [x] 1.5 Implementar fallback neutro para acciones con presentación no soportada

## 2. Integración con el adapter final de columnas

- [x] 2.1 Ajustar `src/app/Components/UI/AppTable/adapters/appGridToAppTableColumns.ts` para detectar columnas `isActionColumn`
- [x] 2.2 Asignar `cellRenderer` en columnas de acción sin romper el contrato actual de `ColDef`
- [x] 2.3 Inyectar `actions` y `appGridColumn` en `cellRendererParams`
- [x] 2.4 Mantener estable el comportamiento actual de columnas no dinámicas

## 3. Reutilización de la action layer existente

- [x] 3.1 Integrar `useDynamicUiTableActions` dentro del renderer sin duplicar lógica
- [x] 3.2 Construir `DynamicUiActionContext` usando datos realmente disponibles (`row`, `columnKey`, `userClaims?`)
- [x] 3.3 Aplicar la regla de render basada en disponibilidad: invisible no renderiza, visible no habilitada renderiza disabled
- [x] 3.4 Ejecutar el flujo `evaluateActionAvailability -> buildActionPayload -> executeAction`
- [x] 3.5 Clasificar `behavior` y `presentation` sin ejecutar navegación, modales ni descargas reales

## 4. Verificación automatizada

- [x] 4.1 Crear `src/app/Components/UI/AppTable/tests/AppTableActionCellRenderer.test.tsx`
- [x] 4.2 Cubrir render de la columna `acciones` con una acción visible y habilitada
- [x] 4.3 Cubrir acción no visible y acción disabled según el guard
- [x] 4.4 Cubrir construcción correcta de payload desde la fila
- [x] 4.5 Cubrir múltiples acciones y preservación de orden
- [x] 4.6 Cubrir fallback visual para presentaciones no soportadas
- [x] 4.7 Verificar que `GestionCorrespondencia` no tenga regresiones por la nueva columna renderizada

## 5. Documentación y evidencia

- [x] 5.1 Documentar el comportamiento visual mínimo de la columna `acciones` en `docs/Components/AppTable/Actions.md`
- [x] 5.2 Documentar límites actuales de `userClaims` y `selectedRows` en el renderer
- [x] 5.3 Ejecutar la suite relevante y dejar evidencia en el cambio OpenSpec

## Evidencia de validación

Comando ejecutado:

```bash
npm.cmd test -- src/app/Components/UI/AppTable/tests/AppTableActionCellRenderer.test.tsx src/app/Components/UI/AppTable/tests/appGridToAppTableColumns.test.ts src/app/Components/UI/AppTable/tests/useDynamicUiTableQuery.test.ts src/app/Components/UI/AppTable/tests/useDynamicUiTableActions.test.ts src/modules/gestionCorrespondencia/tests/useGestionCorrespondenciaTable.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondenciaRoutePage.test.tsx src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx
```

Resultado:

- `7` archivos de test
- `23` tests en verde
