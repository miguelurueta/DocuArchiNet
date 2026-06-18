# SCRUMCORE-251 - Metadata

## Ticket

- ID: `SCRUMCORE-251`
- Nombre: `TABS-WORKBENCH-GESTION-CORRESPONDENCIA`
- Tipo: Mejora UX / productividad enterprise
- Alcance: Frontend
- Backend: No modificado

## Rama

- `feature/SCRUMCORE-251`

## Fecha

- 2026-06-16
- Actualizacion responsive: 2026-06-18

## Estado

- Implementacion frontend inicial realizada.
- Ajustes responsive iterativos aplicados para mobile, tablets, iPad mini/Air/Pro, Surface Pro 7, Nest Hub landscape y desktop pequeno.
- Tests enfocados OK.
- TypeScript OK con `npx.cmd tsc --noEmit --pretty false`.
- Build general bloqueado por deuda preexistente fuera del alcance.
- QA manual responsive en progreso.
- PR pendiente.

## Commits relevantes

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
- Pendiente en este bloque: ajuste mobile AppUpload y flotantes compactos AppEditor.

## Evidencia tecnica

- Dependencia agregada: `react-resizable-panels`.
- Componente nuevo: `GestionWorkbenchParallelTabs`.
- Integracion: `GestionRespuesta.tsx`.
- `DocumentosWorkbench` usa overlay en mobile/tablets altas y excepciones controladas para iPad mini landscape `1024x768` y Nest Hub landscape `1024x600`.
- Desktop pequeno conserva rail inline normal y reduce el alto del panel de documentos para evitar scroll innecesario.
- `AppCollapseRail` conserva overlay lateral en tablets; se retiro el bottom-sheet tablet para alinear con desktop.
- Boton flotante de lista de documentos mantiene foreground visual en hover/active/focus.
- AppEditor mobile:
  - Toolbar compacta deja visible zoom/trailing controls y mueve acciones de editor a overflow.
  - Controles flotantes inferiores separan paginador y contador para evitar solapamiento.
  - Paginador y contador flotantes se compactan y reposicionan para ocupar menos area en mobile.
- AppToolbar contextless mobile:
  - Padding, gaps y tamano de botones reducidos para ganar espacio en acciones internas de Gestion.
- AppUpload mobile:
  - Dropzone, cards, gaps, metadata y acciones visuales se compactan en mobile.
  - El bloque de adjuntos elimina copy auxiliar para reducir alto vertical.
- Tests agregados:
  - `GestionWorkbenchParallelTabs.test.tsx`
  - `GestionRespuesta.test.tsx`

## Riesgos residuales

- Validar manualmente matrix responsive final: iPhone SE, iPhone XR/12 Pro, Galaxy S8+, iPad mini portrait/landscape, iPad Air/Pro, Surface Pro 7, Nest Hub landscape y desktop pequeno.
- Validar que `DocumentosWorkbench` recalcula correctamente dentro de panel redimensionable.
- Validar que no hay doble carga evidente al alternar modo.
- Resolver o aislar deuda de build en `DynamsoftTwainClient.ts` si se requiere build completo verde.

## PR

- Pendiente.
