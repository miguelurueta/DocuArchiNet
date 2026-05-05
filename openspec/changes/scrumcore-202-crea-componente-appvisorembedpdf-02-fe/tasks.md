## 1. Alineación de naming y alcance

- [x] 1.1 Corregir naming del `proposal.md` para reflejar `AppVisorEmbedPdf` + `DocumentosWorkbench` (evitar `AppAppvisorembedpdf02Fe`)
- [x] 1.2 Confirmar que el módulo consumidor no importa `@embedpdf/*` (regla de arquitectura)

## 2. Integración visual en DocumentosWorkbench

- [x] 2.1 Integrar `<AppVisorEmbedPdf />` (sin `fileUrl`) en el layout de `DocumentosWorkbench` como panel/área de visor
- [x] 2.2 Remover/ocultar listado y lógica de selección del workbench (no debe existir UI seleccionable en 02-FE)
- [x] 2.3 Ajustar estilos/layout para mantener scroll correcto (min-height/overflow) sin romper virtualización del visor

## 3. Tests de integración (mock visor)

- [x] 3.1 Mockear `AppVisorEmbedPdf` en tests del workbench para evitar engine real
- [x] 3.2 Agregar/ajustar tests `[SPEC:SCRUMCORE-202]` que validen: panel visible y que se renderiza `AppVisorEmbedPdf` sin `fileUrl`

## 4. Validación y evidencia

- [x] 4.1 Ejecutar tests focales del workbench y corregir solo fallos del cambio
- [x] 4.2 Registrar evidencia de comandos/resultados en `design.md` (Validation Evidence con fecha)
