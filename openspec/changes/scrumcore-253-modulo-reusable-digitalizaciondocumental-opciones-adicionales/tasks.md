## 1. Auditoria

- [x] 1.1 Auditar APIs disponibles en Dynamsoft Web TWAIN 19.3.2.
- [x] 1.2 Confirmar soporte/licencia para Deskew.
- [x] 1.3 Confirmar soporte/licencia para Auto Crop.
- [x] 1.4 Confirmar soporte/licencia para Auto Rotate.
- [x] 1.5 Documentar limitaciones y decisiones.

## 2. Configuracion UI

- [x] 2.1 Agregar seccion `Procesamiento automatico` en panel lateral.
- [x] 2.2 Agregar checkboxes Deskew, Auto Crop y Auto Rotate.
- [x] 2.3 Mantener opciones desactivadas por defecto.
- [x] 2.4 Mantener persistencia solo durante la sesion.

## 3. Integracion scanner

- [x] 3.1 Extender tipos de `ScanOptions`.
- [x] 3.2 Integrar flags en `useDigitalizacionScanner`.
- [x] 3.3 Implementar procesamiento en `DynamsoftTwainClient` si existe API nativa.
- [x] 3.4 Manejar capacidades no soportadas con error controlado o estado documentado.
- [x] 3.5 Actualizar solo paginas afectadas.

## 4. Preview, miniaturas y PDF

- [x] 4.1 Reflejar procesamiento en miniaturas.
- [x] 4.2 Reflejar procesamiento en preview.
- [x] 4.3 Invalidar PDF cuando una pagina se procese.
- [x] 4.4 Confirmar que el PDF final respeta el procesamiento.

## 5. Rendimiento

- [x] 5.1 Medir `DESKEW_TIME`.
- [x] 5.2 Medir `AUTOCROP_TIME`.
- [x] 5.3 Medir `AUTOROTATE_TIME`.
- [x] 5.4 Evitar re-renderizados y regeneraciones masivas innecesarias.

## 6. Pruebas y cierre

- [x] 6.1 Ajustar pruebas de adapter.
- [x] 6.2 Ajustar pruebas de hook.
- [x] 6.3 Ajustar pruebas de workspace/AppDigitalizador.
- [x] 6.4 Ejecutar TypeScript, ESLint y Vitest.
- [x] 6.5 Documentar evidencia, riesgos y archivos modificados.
