## 1. Preparacion

- [x] 1.1 Revisar el contrato del tab **Documentos** en `GestionRespuesta.tsx`
- [x] 1.2 Definir/ajustar la estructura de componentes para el workbench visual

## 2. Layout y responsive

- [x] 2.1 Implementar layout tipo workbench con `AppToolbar`, area principal y panel lateral
- [x] 2.2 Aplicar comportamiento responsive (desktop inline, tablet colapsado, mobile overlay)
- [x] 2.3 Asegurar scroll independiente en area principal y panel

## 3. Accesibilidad

- [x] 3.1 Verificar `aria-expanded` y `aria-controls` en toggles
- [x] 3.2 Garantizar foco visible en rail y acciones
- [x] 3.3 Validar navegacion por teclado (Enter/Espacio) en toggles

## 4. Pruebas

- [x] 4.1 Crear/ajustar pruebas unitarias para render y toggle del workbench
- [x] 4.2 Verificar que el contenido del panel permanezca montado al colapsar
- [x] 4.3 Validar responsive (desktop/tablet/mobile)
- [x] 4.4 Registrar evidencia de tests ejecutados en el change

## Evidencia de pruebas

- `node .\\node_modules\\vitest\\vitest.mjs --run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx` (2026-04-13)
