# SCRUMCORE-251 - Metadata

## Identificacion

- Ticket: `SCRUMCORE-251`
- Nombre tecnico: `tabs-workbench-gestion-correspondencia`
- Rama: `feature/SCRUMCORE-251`
- Tipo: Mejora UX enterprise / responsive hardening / composicion de workbench
- Modulo funcional: Gestion Correspondencia
- Alcance principal: frontend
- Backend: no modificado
- Fecha base de ticket: 2026-06-16
- Actualizacion responsive documentada: 2026-06-19
- Autor operativo: Codex, guiado por usuario

## Estado actual

- Implementacion de vista paralela del workbench realizada previamente en la rama.
- Ajustes responsive actuales aplicados sobre tabs, toolbar, editor, upload, visor PDF, DocumentosWorkbench y header de detalle.
- TypeScript verificado con `npx.cmd tsc --noEmit --pretty false`.
- Commit de este bloque: el commit que contiene esta metadata y los cambios UI descritos en este documento.
- Push/PR: no ejecutados en este bloque.

## Commits previos relevantes

- `2e6c00f feat(SCRUMCORE-251): proposal inicial OpenSpec`
- `33d1273 docs(SCRUMCORE-251): normalize Jira OpenSpec context`
- `11c4624 docs(SCRUMCORE-251): refine OpenSpec artifacts`
- `c2d4527 docs(SCRUMCORE-251): align tasks with architectural prompt`
- `6c19128 feat(SCRUMCORE-251): add parallel workbench tabs`
- `7bae54a fix(SCRUMCORE-251): refine mobile document workbench`
- `f77db8a fix(SCRUMCORE-251): polish mobile documents overlay`
- `d02fc0c fix(SCRUMCORE-251): refine tablet responsive workbench`
- `d77d3f5 fix(SCRUMCORE-251): tune mobile document panel height`
- `70ad45b fix(SCRUMCORE-251): refine document overlay breakpoints`
- `6f4111c fix(SCRUMCORE-251): support nest hub document overlay`
- `9d78901 fix(SCRUMCORE-251): compact mobile editor actions`

## Archivos de codigo modificados en este bloque

- `src/app/Components/UI/AppTabs/AppTabs.module.css`
- `src/app/Components/UI/AppToolbar/AppToolbar.tsx`
- `src/app/Components/UI/AppToolbar/AppToolbar.module.css`
- `src/app/Components/UI/AppToolbar/AppToolbar.test.tsx`
- `src/app/Components/UI/AppUpload/AppUpload.tsx`
- `src/app/Components/UI/AppUpload/AppUpload.module.css`
- `src/app/Components/UI/AppUpload/AppUpload.test.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/styles/AppVisorEmbedPdf.module.css`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.module.css`
- `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx`
- `src/modules/gestionCorrespondencia/style/GestionCorrespondenciaRoute.module.css`

## Documentacion enterprise actualizada

- `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/SCRUMCORE-251-Arquitectura.md`
- `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/SCRUMCORE-251-Implementacion-Detallada.md`
- `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/SCRUMCORE-251-Pruebas.md`
- `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/SCRUMCORE-251-Metadata.md`

## Resumen tecnico del bloque

1. AppToolbar
   - Se agrego la prop `density?: "default" | "compact"`.
   - Se agrego clase `compactDensity` para densidad visual compacta.
   - Se elimino el comportamiento CSS que forzaba columna global en `max-width: 1100px`.
   - El modo compacto queda controlado por media query interna del componente y por `density`.

2. Gestion tab / AppEditor shell
   - Se compacto la toolbar superior de acciones de Gestion.
   - Se redujeron gaps y padding verticales.
   - Se ajustaron alturas del area del editor por desktop, tablets y mobile.
   - Se preservo el flujo de `AppEditor` sin alterar su contrato funcional.

3. AppUpload / Adjuntos
   - Se agrego `className` a `AppUpload` para customizacion scoped.
   - Se agrego `role="listitem"` en cards para listas de archivos.
   - Se corrigio la persistencia visual de archivos con estrategia `auto` usando `filesRef`.
   - Se agrego test para asegurar que un archivo cargado queda visible con estado `done`.
   - En Gestion se configura `layout="list"`, `previewOnClick={false}` y acciones custom.
   - En cards de adjuntos se deja solo accion de eliminar.
   - Las cards se agrupan lado a lado, compactas, con wrap y menor altura en mobile.

4. Header de detalle
   - Los items de metadata (`Radicado`, `Remitente`, `Tramite`) reciben `title` completo.
   - En mobile la metadata se alinea a la derecha junto al boton de retorno con layout vertical.
   - Se protege legibilidad mediante wrapping, text-align right y tooltip nativo.

5. DocumentosWorkbench / AppVisorEmbedPdf
   - Se agregaron breakpoints especificos para mobile y tablet.
   - El alto de `workbenchBody`, `.viewer` y `.root` del visor PDF se sincroniza por rango.
   - Se ajustaron vistas objetivo: iPhone SE, Samsung Galaxy S8+, iPhone XR, iPhone 12 Pro, iPhone 14 Pro Max e iPad Mini.
   - Se mantuvo el overlay lateral de documentos y el rail sin tocar contratos ni services.

6. AppTabs
   - Se redujo padding general de `panelContent`.
   - En tablets se reducen gaps y padding.
   - El panel de Documentos tiene reglas responsive scoped mediante `:has([data-testid="documentos-workbench"])`.
   - El override de iPad Mini se ubico al final para evitar que el bloque mobile general lo pise.

## Evidencia tecnica

- TypeScript:
  - Comando: `npx.cmd tsc --noEmit --pretty false`
  - Resultado: OK
- Tests enfocados:
  - `npx.cmd vitest run src/app/Components/UI/AppUpload/AppUpload.test.tsx`: 11 tests OK.
  - `npx.cmd vitest run src/app/Components/UI/AppToolbar/AppToolbar.test.tsx`: 4 tests OK.
- Diff check:
  - Comando: `git diff --check`
  - Resultado: sin errores de whitespace; Git aviso conversion LF/CRLF por configuracion local.
- Tests agregados/modificados:
  - `AppUpload.test.tsx`: caso de archivo visible con estrategia `auto`.
  - `AppToolbar.test.tsx`: assertion para que desktop no se marque como compacto.

## Riesgos residuales

- La matriz responsive depende de alto/ancho CSS reportado por DevTools; QA debe validar en dispositivos o emuladores equivalentes.
- Las reglas `:has()` requieren soporte moderno del navegador; el proyecto ya usa navegadores modernos y el selector queda scoped a UI interna.
- El uso de breakpoints por dispositivo es intencional por requerimiento visual, pero debe revisarse si se incorpora una estrategia fluida posterior.
- No se ejecuto build completo en este bloque; la evidencia primaria es TypeScript.
