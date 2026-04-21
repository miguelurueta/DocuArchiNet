## 1. Analisis estructural y de estilos

- [x] 1.1 Revisar `presentation/AppEditor.tsx` para identificar la capa
  intermedia redundante entre la estructura principal y `TiptapEditorContent`
- [x] 1.2 Revisar `AppEditor.module.css` para localizar reglas de margen,
  padding y sangria que afectan `ul`, `ol` y el contenido editable
- [x] 1.3 Confirmar diferencias de comportamiento entre modo continuo y modo
  visual paginado antes de editar

## 2. Ajuste de jerarquia del editor

- [x] 2.1 Remover el wrapper intermedio redundante del modo continuo
- [x] 2.2 Remover el wrapper intermedio redundante del flujo paginado sin tocar
  `editorWrapper`, `canvas`, `sheet` ni `contentFlow`
- [x] 2.3 Reasignar al nodo correcto las responsabilidades de layout visual que
  hoy dependan de `surface` o `surfacePaged`

## 3. Ajuste visual de listas

- [x] 3.1 Ajustar sangria base de `ul` y `ol` para respetar la margen ya
  establecida del editor
- [x] 3.2 Normalizar render multilinea de items de lista (`li`, `li > p` o
  equivalente) para mantener legibilidad estable
- [x] 3.3 Confirmar que `bullet list`, `ordered list` y `task list` no sufren
  regresion visual evidente

## 4. Compatibilidad y no regresion

- [x] 4.1 Confirmar que el modo continuo mantiene `min-height`, scroll y
  accesibilidad del editor
- [x] 4.2 Confirmar que el modo visual mantiene alineacion con `pageMargins`,
  `pageShell`, zoom y contador
- [x] 4.3 Confirmar que toolbar, foco y serializacion HTML siguen estables

## 5. Pruebas y evidencia

- [x] 5.1 Actualizar `AppEditor.test.tsx` para reflejar la nueva jerarquia del
  DOM
- [x] 5.2 Agregar o ajustar pruebas para listas con viñetas y numeracion
- [x] 5.3 Agregar cobertura de no regresion para modo `paginationMode="visual"`
- [x] 5.4 Ejecutar pruebas focalizadas del modulo `AppEditor` y registrar
  resultados
- [ ] 5.5 Ejecutar validacion TypeScript o equivalente y registrar residuos
  ajenos si aparecen
- [x] 5.6 Registrar evidencia final en este archivo

## Backend

- [x] No aplica: este cambio no requiere integracion backend ni contratos HTTP

## Evidencia

- `src/app/Components/UI/AppEditor/presentation/AppEditor.tsx`: se removio la
  capa intermedia `surface` / `surfacePaged` y `TiptapEditorContent` ahora
  cuelga directamente de la estructura principal requerida en modo continuo y
  paginado.
- `src/app/Components/UI/AppEditor/infrastructure/TiptapEditorContent.tsx`:
  ahora acepta `style` para mantener el wiring del modo continuo sin reintroducir
  wrappers redundantes.
- `src/app/Components/UI/AppEditor/AppEditor.module.css`: se trasladaron
  responsabilidades de layout al nodo `editorContent`, se elimino la necesidad
  de `surface` / `surfacePaged` y se ajusto la sangria de `ul/ol` junto con la
  normalizacion de `li > p`.
- `src/app/Components/UI/AppEditor/AppEditor.test.tsx`: se actualizaron las
  expectativas estructurales para validar que `editorContent` cuelga
  directamente de `frame` o `contentFlow` segun el modo del editor.
- Pruebas ejecutadas:
  - `node .\\node_modules\\vitest\\vitest.mjs --run src/app/Components/UI/AppEditor/AppEditor.test.tsx src/app/Components/UI/AppEditor/AppEditorToolbar.test.tsx src/app/Components/UI/AppEditor/useAppEditor.test.tsx src/app/Components/UI/AppEditor/AppEditor.integration.test.tsx` -> `4 files passed`, `37 tests passed`
- Pendiente:
  - ejecutar validacion TypeScript completa si se requiere evidencia adicional
