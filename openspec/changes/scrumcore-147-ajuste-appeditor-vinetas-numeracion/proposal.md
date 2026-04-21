## Why

`AppEditor` ya permite usar listas con viñetas y numeracion, pero hoy su
render visual no respeta correctamente la margen ya establecida del editor. La
sangria propia de `ul/ol` se suma al padding base de la superficie editable y
termina desplazando el contenido mas de lo esperado.

Adicionalmente, la jerarquia actual del componente mantiene un wrapper visual
intermedio entre la estructura principal del editor y `TiptapEditorContent`.
Ese wrapper ya no aporta suficiente valor estructural y complica el DOM, los
estilos y las pruebas.

Este cambio busca corregir el comportamiento visual de listas y simplificar la
estructura renderizada sin romper la arquitectura necesaria del modo continuo ni
la del modo `paginationMode="visual"`.

## What Changes

- Ajustar estilos de listas (`ul`, `ol`, `li`) para que viñetas y numeracion
  respeten la margen visual del editor.
- Normalizar el render de items multilinea para mantener legibilidad estable.
- Remover el wrapper intermedio redundante del contenido de `AppEditor` tanto
  en modo continuo como en el flujo paginado donde aplique.
- Preservar capas estructurales necesarias como `frame`, `editorWrapper`,
  `canvas`, `sheet` y `contentFlow`.
- Actualizar pruebas del editor para reflejar la nueva jerarquia y proteger la
  no regresion del modo visual, toolbar, zoom y contador.

## Capabilities

### New Capabilities
- `ajuste-appeditor-vinetas-numeracion`: Ajuste visual y estructural de
  `AppEditor` para corregir sangria de listas y remover wrapper intermedio
  redundante sin romper la experiencia paginada.

### Modified Capabilities
- Ninguna capability existente del arbol principal fue identificada como dueña
  unica de este ajuste; el delta se mantiene scoped a este cambio.

## Impact

- Se modificara `src/app/Components/UI/AppEditor/presentation/AppEditor.tsx`
  para simplificar la jerarquia JSX del contenido editable.
- Se modificara `src/app/Components/UI/AppEditor/AppEditor.module.css` para
  corregir margenes, padding y sangria de listas.
- Se modificaran pruebas focalizadas de `AppEditor` para reflejar la nueva
  estructura y validar no regresion visual/funcional.
- No hay impacto backend ni cambios en contratos HTTP.
