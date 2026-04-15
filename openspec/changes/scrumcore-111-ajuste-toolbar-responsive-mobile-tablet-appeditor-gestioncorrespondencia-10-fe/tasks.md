## 1. Remocion del toggle visible de tema

- [x] 1.1 Revisar el contrato actual de tema en `AppEditor` y `AppEditorToolbar`
- [x] 1.2 Remover el boton visible de cambio `dark/light` del toolbar
- [x] 1.3 Eliminar handlers, wiring y estado interno que queden sin uso real
- [x] 1.4 Conservar compatibilidad con el theming global y con props externas aun vigentes
- [x] 1.5 Limpiar estilos asociados al toggle que ya no sean necesarios

## 2. Compactacion responsive de la toolbar

- [x] 2.1 Identificar acciones principales que deben permanecer visibles (`bold`, `italic`, `undo`, `redo`)
- [x] 2.2 Reagrupar acciones secundarias para mobile y tablet
- [x] 2.3 Implementar colapso de acciones secundarias en dropdowns o affordances equivalentes
- [x] 2.4 Reducir padding, spacing y densidad visual en `667px`, `896px`, `932px` y `1024px`
- [x] 2.5 Evitar overflow horizontal y multiples filas excesivas

## 3. Estabilidad funcional del editor

- [x] 3.1 Confirmar que la interaccion con toolbar no recrea la instancia de Tiptap
- [x] 3.2 Confirmar que no se pierde foco del editor al usar botones, dropdowns o popovers
- [x] 3.3 Confirmar que desktop no sufre regresion visual o funcional

## 4. Pruebas y evidencia

- [x] 4.1 Ajustar pruebas del toolbar sin toggle de tema
- [x] 4.2 Agregar o ajustar pruebas responsive del toolbar
- [x] 4.3 Validar que el editor sigue respetando el tema global sin toggle manual
- [x] 4.4 Ejecutar pruebas focalizadas del editor y registrar resultados
- [x] 4.5 Ejecutar validacion TypeScript o equivalente y registrar residuos ajenos si aparecen
- [x] 4.6 Registrar evidencia final en este archivo

## Evidencia

- Se removio el toggle visible de tema del toolbar en `AppEditorToolbar.tsx`.
- `AppEditor` ya no mantiene wiring interno para cambio manual de tema, pero conserva compatibilidad con `themeMode` y `defaultThemeMode` para respetar el theming global.
- La toolbar ahora expone `data-toolbar-mode="compact"` cuando el ancho es `<= 1024px`.
- En modo compacto se mantiene visible el grupo principal de formato e historial y se colapsa la estructura en un dropdown dedicado para reducir densidad visual.
- `AppEditor.module.css` ajusta spacing, padding y distribucion para mobile y tablet sin forzar los grupos a ocupar todo el ancho.
- Se actualizaron pruebas del editor y del toolbar para cubrir:
  - ausencia del toggle de tema;
  - compatibilidad con `themeMode` externo;
  - entrada a modo compacto y colapso de estructura en `896px`.
- Pruebas ejecutadas:
  - `npm test -- AppEditor.test.tsx AppEditorToolbar.test.tsx useAppEditor.test.tsx AppEditor.integration.test.tsx` -> `4 files passed`, `23 tests passed`
  - `npx tsc -p tsconfig.app.json --noEmit` -> mantiene solo errores preexistentes ajenos al cambio en `src/app/Components/UI/AppTabs/AppTabs.tsx` y `src/setupTests.ts`
