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
