## 1. Baseline visual responsive de AppEditorPdf

- [ ] 1.1 Revisar el baseline actual de `AppEditorPdf` y documentar los puntos UI heredados de `AppEditor` que aplican a `02-FE`.
- [x] 1.2 Definir ajustes de layout responsive para desktop/tablet/mobile sin crear componentes paralelos.
- [x] 1.3 Mantener la ruta canonica `src/app/Components/UI/AppEditorPdf/` y naming tecnico estable en archivos/exportaciones.

## 2. Theming y consistencia visual

- [x] 2.1 Alinear estilos de `AppEditorPdf` al theme global de la aplicacion sin overrides contradictorios.
- [ ] 2.2 Verificar estados visuales clave (normal, readOnly, error, helper) bajo temas y breakpoints.
- [x] 2.3 Garantizar que ajustes visuales no cambian contrato funcional ni serializacion de contenido.

## 3. Estabilidad UX durante adaptacion responsive

- [ ] 3.1 Validar que no exista flicker perceptible al cambiar viewport o estado visual del editor.
- [ ] 3.2 Validar continuidad de cursor y seleccion bajo cambios de layout responsive.
- [ ] 3.3 Confirmar comportamiento de scroll continuo unico sin doble scroll en contenedores.

## 4. Pruebas y cierre del cambio

- [x] 4.1 Crear/ajustar pruebas unitarias focalizadas en reglas visuales responsive de `AppEditorPdf`.
- [ ] 4.2 Ejecutar pruebas de integracion UI para verificar compatibilidad con consumidores existentes.
- [ ] 4.3 Documentar evidencia de pruebas y actualizar artefactos de cierre para `SCRUMCORE-159`.
