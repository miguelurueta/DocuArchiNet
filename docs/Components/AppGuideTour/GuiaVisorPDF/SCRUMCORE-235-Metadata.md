# SCRUMCORE-235 - AppGuideTour - Metadata

| Elemento | Archivo | Evidencia | Estado |
| --- | --- | --- | --- |
| Dependencia Driver.js | `package.json`, `package-lock.json` | `npm install driver.js` | Implementado |
| Tipos publicos | `src/app/Components/UI/AppGuideTour/AppGuideTour.types.ts` | Tests de hook/componente | Implementado |
| Constantes | `src/app/Components/UI/AppGuideTour/AppGuideTour.constants.ts` | Tests de hook | Implementado |
| Servicio de filtrado | `src/app/Components/UI/AppGuideTour/AppGuideTour.service.ts` | `AppGuideTour.service.test.ts` | Implementado |
| Adapter Driver.js | `src/app/Components/UI/AppGuideTour/drivers/DriverJsAdapter.ts` | `DriverJsAdapter.test.ts` | Implementado |
| Hook | `src/app/Components/UI/AppGuideTour/hooks/useAppGuideTour.ts` | `useAppGuideTour.test.tsx` | Implementado |
| Componente headless | `src/app/Components/UI/AppGuideTour/AppGuideTour.tsx` | `AppGuideTour.test.tsx` | Implementado |
| Provider placeholder | `src/app/Components/UI/AppGuideTour/providers/AppGuideTourProvider.tsx` | Decision documentada: no contexto compartido requerido | Implementado |
| Exports | `src/app/Components/UI/AppGuideTour/index.ts` | Import usado por AppVisorEmbedPdf | Implementado |
| Configuracion PDF steps | `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.guideTour.ts` | Playwright guide tour | Implementado |
| Targets toolbar | `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx` | `AppPdfToolbar.test.tsx` | Implementado |
| Boton ayuda | `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx` | Vitest + Playwright | Implementado |
| Integracion visor | `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx` | `AppVisorEmbedPdf.test.tsx` | Implementado |
| Playwright smoke | `playwright/appvisorEmbedPdfGuideTour.spec.ts` | `1 passed` | Implementado |
| Regresion Playwright | `playwright/appvisorEmbedPdfZoom.spec.ts`, `appvisorEmbedPdfThumbnails.spec.ts`, `appvisorEmbedPdfRotate.spec.ts`, `appvisorEmbedPdfPrintExport.spec.ts` | `4 passed` | Ejecutado |
| Popover Driver.js | `src/app/Components/UI/AppGuideTour/AppGuideTour.css` | Borde, radius, shadow y override de overflow del toolbar | Implementado |
| Estado vacio visor | `src/app/Components/UI/AppVisorEmbedPdf/presentation/States.tsx`, `AppVisorEmbedPdf.module.css`, `AppVisorEmbedPdfProps.ts`, `AppVisorEmbedPdf.tsx` | Icono documento + flecha diagonal, boton accesible y callback `onEmptyDocumentHintRequest` | Implementado |
| Hint listado documentos | `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`, `DocumentosWorkbench.module.css` | Primera fila completa titila al solicitar ayuda desde el visor vacio | Implementado |
| Panel derecho documentos | `src/app/Components/UI/AppCollapseRail/AppCollapseRail.module.css` | Padding externo 0, borde externo none, borde interno gris suave | Implementado |
| Header detalle respuesta | `src/modules/gestionCorrespondencia/style/GestionCorrespondenciaRoute.module.css` | Padding vertical reducido en `.detailHeader` | Implementado |
| DocumentContext estable | `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx` | `useMemo` sobre campos primitivos + `lastVisorLoadKeyRef` para evitar cargas repetidas | Implementado |
| Permisos visor PDF | `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`, `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx` | Load managed con `idImagen`, `nombreGabinete`, `idTareaWorkflow`, `radicado`, `nombre_modulo` | Implementado |
| Indicador ayuda PDF | `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`, `AppPdfToolbar.module.css` | Badge azul con `1` sobre boton de guia, sin cambiar `onStartGuideTour` | Implementado |
| Semaforo por ESTADO | `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`, `GestionCorrespondencia.module.css` | Mapeo normalizado de estados a tonos enterprise | Implementado |
| Tabla sin checks visibles | `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx` | `rowSelectionCheckboxes={false}` y `rowSelectionHeaderCheckbox={false}` conservando `rowSelection="single"` | Implementado |
| Fila seleccionada enterprise | `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css` | Fondo gris notable, texto negro `#111827`, peso `600`, sin outline de celda | Implementado |
| Toolbar Gestion Correspondencia | `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css` | Input compacto, foco sin ruido visual, boton actualizar primario, layout alineado | Implementado |
| Correcciones TypeScript AppEditor | `src/app/Components/UI/AppEditor/application/*`, `src/app/Components/UI/AppEditor/presentation/*` | `npx tsc -b` sin errores | Implementado |
| Correcciones TypeScript visor | `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`, `plugins/pluginRegistration.ts`, `presentation/AppPdfSignatureModal.tsx` | Tasks, `BlobPart`, `saveAsCopy`, enum de firma y `exclusive` corregidos | Implementado |
| Correcciones TypeScript Gestion | `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentos.ts`, `useGestionRespuestaDocumentosTable.ts`, `useListaDocumentosRadicadosTreeTable.ts` | Fallback mutable, lectura segura de errores, helper no usado removido | Implementado |
| Skips firma personal reemplazados | `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`, `src/app/Components/UI/AppVisorEmbedPdf/hooks/useWorkflowPersonalSignature.test.tsx` | 21 tests visor/hook passed, 0 skipped en archivos ejecutados | Implementado |
| Warnings de test visor limpiados | `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`, `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Pruebas.md` | Mock de `Scroller` completo + `waitFor` en demo PDF; 18 tests passed sin warnings `NaN width` ni `act(...)` | Implementado |
| Commit de cierre SCRUMCORE-235 | Worktree actual | Mensaje: `SCRUMCORE-235: stabilize visor tests and gestion styles`; validado con `npx tsc -b` y suite enfocada de visor/firma/tabla | Implementado |
| Documentacion arquitectura | `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Arquitectura.md` | Diagramas Mermaid | Implementado |
| Documentacion implementacion | `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Implementacion-Detallada.md` | Detalle tecnico | Implementado |
| Documentacion pruebas | `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Pruebas.md` | Evidencia de comandos | Implementado |
| Metadata | `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Metadata.md` | Tabla trazable | Implementado |

## Decisiones

- No se agrega Search.
- No se agrega Fit Width.
- No se agrega Fit Page.
- No se toca logica funcional del visor PDF.
- Driver.js no se importa desde consumidores.
- El provider existe como placeholder liviano porque no se requiere contexto compartido en esta primera integracion.
- El hint del estado vacio no selecciona filas ni abre documentos; solo abre el panel derecho y aplica una animacion temporal a la primera fila visible.
- El borde visual del listado y del visor usa el mismo gris suave para consistencia.
- La UI de checks en la tabla de Gestion Correspondencia se oculta sin retirar `rowSelection="single"` ni `onSelectionChanged`.
- La fila seleccionada de Gestion Correspondencia se resuelve solo por CSS sobre `aria-selected="true"`.
- El foco visual de celda en Gestion Correspondencia se elimina solo en CSS para evitar espacios blancos; no se toca la seleccion interna de AG Grid.
- El semaforo de Gestion Correspondencia deriva de la columna `ESTADO` o candidatos equivalentes, con normalizacion de acentos.
- Los permisos del visor PDF usan el load managed con contexto documental, pero el render del visor mantiene el `fileUrl` existente para no alterar la experiencia actual.
- Las correcciones TypeScript se hicieron con wrappers defensivos y eliminacion de codigo no usado, sin cambiar contratos publicos.
- Los skips heredados de firma personal se corrigieron con mocks de frontera y pruebas del hook; no se cambio codigo productivo para hacer pasar esas pruebas.
- Los warnings `NaN width` y `act(...)` se corrigieron ajustando el contrato del mock de `Scroller` y la espera async del test demo PDF; no se cambio comportamiento runtime.
