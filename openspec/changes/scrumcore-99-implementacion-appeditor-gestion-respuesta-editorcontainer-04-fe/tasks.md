## 1. Integracion del editor en el contenedor

- [x] 1.1 Reemplazar el placeholder de `editorSurface` por `AppEditor`
- [x] 1.2 Mantener `GestionRespuestaEditorContainer` como shell visual del modulo
- [x] 1.3 Evitar duplicacion de header entre el container y `AppEditor`

## 2. Estado y layout

- [x] 2.1 Implementar estado controlled para el contenido del editor en el modulo
- [x] 2.2 Validar que `AppEditor` ocupe el 100% del area disponible dentro de `editorSurface`
- [x] 2.3 Confirmar que el layout `workbenchBody` y el panel lateral sigan funcionando sin regresion
- [x] 2.4 Confirmar scroll interno y ausencia de overflow horizontal

## 3. Pruebas y regresion

- [x] 3.1 Agregar o ajustar pruebas de integracion para `GestionRespuestaEditorContainer` y/o `GestionRespuestaMainTabContent`
- [x] 3.2 Ejecutar pruebas focalizadas de `AppEditor`
- [x] 3.3 Ejecutar pruebas del modulo `gestionRespuesta` si existen
- [x] 3.4 Ejecutar validacion TypeScript y reportar errores ajenos al cambio
- [x] 3.5 Registrar evidencia final en este archivo

## Evidencia

- `GestionRespuestaEditorContainer` mantiene el shell visual y ahora renderiza `children` dentro de `editorSurface` sin fallback placeholder.
- `GestionRespuestaMainTabContent` integra `AppEditor` en modo controlled con `value` y `onChange`, usando variante embebida sin duplicar `title` ni `description`.
- Se ajusto el layout del modulo para que `AppEditor` ocupe el 100% del area disponible en el recuadro principal.
- Se agrego `GestionRespuestaMainTabContent.test.tsx` para validar reemplazo del placeholder e interaccion del panel lateral.
- Pruebas ejecutadas:
  - `node .\node_modules\vitest\vitest.mjs --run src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.test.tsx src/app/Components/UI/AppEditor/AppEditor.test.tsx src/app/Components/UI/AppEditor/AppEditorToolbar.test.tsx src/app/Components/UI/AppEditor/useAppEditor.test.tsx src/app/Components/UI/AppEditor/AppEditor.integration.test.tsx` -> `15 passed`
  - `npx tsc -p tsconfig.app.json --noEmit` -> mantiene errores preexistentes ajenos al cambio en `src/app/Components/UI/AppTabs/AppTabs.tsx` y `src/setupTests.ts`
