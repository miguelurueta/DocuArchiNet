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

## Estado

- Implementacion frontend inicial realizada.
- Tests enfocados OK.
- Build general bloqueado por deuda preexistente fuera del alcance.
- QA manual pendiente.
- PR pendiente.

## Commits relevantes

- `2e6c00f feat(SCRUMCORE-251): proposal inicial OpenSpec`
- `33d1273 docs(SCRUMCORE-251): normalize Jira OpenSpec context`
- `11c4624 docs(SCRUMCORE-251): refine OpenSpec artifacts`
- `c2d4527 docs(SCRUMCORE-251): align tasks with architectural prompt`

## Evidencia tecnica

- Dependencia agregada: `react-resizable-panels`.
- Componente nuevo: `GestionWorkbenchParallelTabs`.
- Integracion: `GestionRespuesta.tsx`.
- Tests agregados:
  - `GestionWorkbenchParallelTabs.test.tsx`
  - `GestionRespuesta.test.tsx`

## Riesgos residuales

- Validar manualmente que `DocumentosWorkbench` recalcula correctamente dentro de panel redimensionable.
- Validar que no hay doble carga evidente al alternar modo.
- Resolver o aislar deuda de build en `DynamsoftTwainClient.ts` si se requiere build completo verde.

## PR

- Pendiente.
