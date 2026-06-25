## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira, prompt arquitectonico y documentos AppUploadDocumental.
- [x] 1.2 Refinar design/spec/tasks con decisiones, limites y riesgos definitivos.
- [x] 1.3 Validar OpenSpec estricto antes de implementar.

## 2. Implementacion shared UI

- [x] 2.1 Crear `src/app/Components/UI/AppUploadBatchView/`.
- [x] 2.2 Definir `AppUploadBatchView.types.ts` con estados, item, summary y props genericos sin `any`.
- [x] 2.3 Implementar `AppUploadBatchView.tsx` componiendo `AppUpload` y `AppButton`.
- [x] 2.4 Implementar toolbar global: agregar, guardar todos, limpiar todos.
- [x] 2.5 Implementar lista de archivos con fila activa, nombre truncado, tamano, estado, progreso, warning/error y acciones por archivo.
- [x] 2.6 Implementar acciones por archivo: seleccionar/ver, eliminar, guardar individual cuando `canSaveOne=true`.
- [x] 2.7 Implementar slots `renderMetadata`, `renderPreview`, `renderFileName` y `renderFooterExtra`.
- [x] 2.8 Implementar preview default para PDF, imagen y fallback de otros formatos.
- [x] 2.9 Implementar cleanup de object URLs al cambiar preview y desmontar.
- [x] 2.10 Implementar layout responsive desktop/mobile en CSS module sin cards anidadas ni estilos inline.
- [x] 2.11 Exportar desde `AppUploadBatchView/index.ts` y `src/app/Components/UI/index.ts`.

## 3. Documentacion

- [x] 3.1 Crear `src/app/Components/UI/AppUploadBatchView/README.md`.
- [x] 3.2 Documentar objetivo, props, estados, slots, ejemplos de metadata, preview custom, limites y relacion con `AppUploadDocumental`.
- [x] 3.3 Confirmar explicitamente que no hay backend, endpoints, almacenamiento, tipologias ni dominio documental en este ticket.
- [x] 3.4 Crear documentacion enterprise en `docs/Architecture/AppUploadBatchView/`.
- [x] 3.5 Crear `docs/Architecture/AppUploadBatchView/SCRUMCORE-270-Arquitectura.md` con contexto, alcance, responsabilidades, composicion con `AppUpload`, limites de dominio y decisiones de diseno.
- [x] 3.6 Crear `docs/Architecture/AppUploadBatchView/SCRUMCORE-270-Implementacion-Detallada.md` con estructura de archivos, contrato TypeScript, flujo de render, eventos, slots, preview, responsive y accesibilidad.
- [x] 3.7 Crear `docs/Architecture/AppUploadBatchView/SCRUMCORE-270-Pruebas.md` con matriz de pruebas unitarias/integracion, comandos ejecutados, resultados y riesgos residuales.
- [x] 3.8 Crear `docs/Architecture/AppUploadBatchView/SCRUMCORE-270-Metadata.md` con ticket, rama, PR, commits, archivos creados/modificados, validaciones, alcance/no alcance y estado final.
- [x] 3.9 Mantener alineada la documentacion enterprise con el README del componente y con `docs/Architecture/AppUploadDocumental/PROMPTS-CONSTRUCCION-AppUploadDocumental.md`.

## 4. Pruebas

- [x] 4.1 Agregar tests de lista vacia, resumen y contador.
- [x] 4.2 Agregar tests de render de archivos, nombre largo, tamano y estados.
- [x] 4.3 Agregar tests de fila activa por `selectedUid`.
- [x] 4.4 Agregar tests de eventos `onSelectFile`, `onPreviewFile`, `onRemoveFile`, `onSaveFile`, `onSaveAll`, `onClearAll` y composicion de selector AppUpload para `onFilesSelected`.
- [x] 4.5 Agregar tests de habilitacion/deshabilitacion por `disabled`, `loading`, `can*` e `item.disabled`.
- [x] 4.6 Agregar tests de `renderMetadata`, `renderPreview`, `renderFileName` y `renderFooterExtra`.
- [x] 4.7 Agregar tests de error/warning por archivo.
- [x] 4.8 Agregar tests de preview default PDF/imagen/fallback.
- [x] 4.9 Agregar test de revocacion de object URL al cambiar archivo o desmontar.
- [x] 4.10 Ejecutar suite enfocada de `AppUploadBatchView`.

## 5. Verificacion y cierre

- [x] 5.1 Ejecutar `npx.cmd openspec validate scrumcore-270-crea-componente-appuploadbatchview --strict`.
- [x] 5.2 Ejecutar TypeScript/lint enfocado si aplica.
- [x] 5.3 Registrar evidencia de comandos y resultados.
- [x] 5.4 Confirmar no modificacion de `AppUpload`, backend ni endpoints.
- [x] 5.5 Preparar commit local de implementacion cuando la implementacion este completa.
