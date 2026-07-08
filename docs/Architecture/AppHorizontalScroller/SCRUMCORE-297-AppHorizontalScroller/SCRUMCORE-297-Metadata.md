# SCRUMCORE-297 - Metadata

## Jira

- Ticket: `SCRUMCORE-297`
- Nombre: `CREA-COMPONENTE-APPHORIZONTALSCROLLER`
- Tipo: Tarea
- Prioridad: Medium
- Labels: `APPHORIZONTALSCROLLER`, `COMPONENTE`, `CREA`
- Alcance: Frontend / UI shared primitive

## Git

- Rama: `feature/SCRUMCORE-297`
- Commits de refinamiento:
  - `149f64d feat(SCRUMCORE-297): proposal inicial OpenSpec`
  - `6dfecae docs(SCRUMCORE-297): refine app horizontal scroller specs`
  - `46f0148 docs(SCRUMCORE-297): align tasks with scroller contract`
  - `6b4e2bf docs(SCRUMCORE-297): tighten scroller task checklist`
- Commits de implementacion:
  - `bb53f4e feat(SCRUMCORE-297): add app horizontal scroller`
  - `7ce6949 fix(SCRUMCORE-297): harden scroller dimension handling`
- Commits de documentacion:
  - `0131d40 docs(SCRUMCORE-297): expand scroller enterprise documentation`
  - `ccc3ba4 docs(SCRUMCORE-297): split scroller enterprise documentation`

## Archivos Creados

- `src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.tsx`
- `src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.module.css`
- `src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx`
- `src/app/Components/UI/AppHorizontalScroller/index.ts`
- `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/00-indice.md`
- `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/01-arquitectura.md`
- `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/02-api-contrato-visual.md`
- `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/03-responsive-accesibilidad-css.md`
- `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/04-uso-e-integracion.md`
- `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/05-pruebas-validacion.md`
- `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/06-riesgos-checklist.md`
- `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/SCRUMCORE-297-Metadata.md`

## Archivos Modificados

- `src/app/Components/UI/index.ts`
- `openspec/changes/scrumcore-297-crea-componente-apphorizontalscroller/proposal.md`
- `openspec/changes/scrumcore-297-crea-componente-apphorizontalscroller/design.md`
- `openspec/changes/scrumcore-297-crea-componente-apphorizontalscroller/tasks.md`
- `openspec/changes/scrumcore-297-crea-componente-apphorizontalscroller/specs/crea-componente-apphorizontalscroller/spec.md`

## Decisiones

- `AppHorizontalScroller` es un primitive UI puro.
- El componente no consume APIs ni conoce DTOs, endpoints o estados de dominio.
- El consumidor compone los `children`; el primitive solo resuelve layout horizontal.
- El viewport usa `role="region"` y `aria-label` obligatorio.
- No se agrega `tabIndex` para evitar un foco extra innecesario.
- No se interceptan eventos de teclado.
- Las dimensiones se aplican mediante custom properties.
- Números positivos se normalizan a `px`.
- Strings vacíos y strings negativos se ignoran.
- Strings CSS avanzados no negativos se permiten para mantener flexibilidad.
- `scrollSnap` usa `x proximity`; no se usa `mandatory`.
- `edgeFade` usa pseudo-elementos con `pointer-events: none`.
- No se agregan dependencias nuevas.

## Confirmaciones De Alcance

- Backend NO modificado.
- Endpoints NO modificados.
- `GestionCorrespondencia` NO modificado.
- `AppTable` NO modificado.
- `AppTreeTable` NO modificado.
- Servicios HTTP NO modificados.
- Hooks de dominio NO modificados.
- SCRUM-162 NO implementado en este ticket.
- No hay `axios` ni `fetch` en el componente.
- No hay integración directa con modulos funcionales.

## Estado De Tareas

Las tareas OpenSpec fueron marcadas como completadas en:

```txt
openspec/changes/scrumcore-297-crea-componente-apphorizontalscroller/tasks.md
```

## Evidencia De Validacion

- Tests focalizados:

```powershell
npm.cmd run test -- src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx
```

Resultado: `14 passed`.

- Lint focalizado:

```powershell
node_modules\.bin\eslint.cmd src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.tsx src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx
```

Resultado: OK.

- OpenSpec/test validator:

```powershell
npm.cmd run spec:validate
```

Resultado: OK.

- Diff check:

```powershell
git diff --check
```

Resultado: OK.

- Auditoria de acoplamiento:

```powershell
rg "axios|fetch\(|services|hooks|AppTable|AppTreeTable|GestionCorrespondencia" src\app\Components\UI\AppHorizontalScroller
```

Resultado: sin coincidencias.

- Auditoria CSS:

```powershell
rg "max-width: 100%|min-width: 0|overflow-x: auto|overflow-y: hidden|-webkit-overflow-scrolling|flex-flow: row nowrap|width: max-content|scroll-snap-type: x proximity|pointer-events: none" src\app\Components\UI\AppHorizontalScroller\AppHorizontalScroller.module.css
```

Resultado: reglas esperadas presentes.

## Build Global

`npm.cmd run build` falla por un error TypeScript preexistente fuera del alcance:

```txt
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx(8,3): error TS2724: "../../../almacenamientoDocumental/components/AppUploadDocumental" has no exported member named 'UploadDocumentalStoredContext'. Did you mean 'UploadDocumentalContext'?
```

El error pertenece a `gestionCorrespondencia`/`AppUploadDocumental`, no a `AppHorizontalScroller`.

## Riesgos Residuales

- El primer consumidor de pantalla debe validar visualmente responsive con datos reales.
- El primitive no implementa botones prev/next; si se requieren, deben entrar por otro ticket o por composición externa.
- El ancho mínimo/máximo aplica a hijos directos; consumidores deben evitar wrappers innecesarios que rompan el contrato visual.
- Strings CSS avanzados no negativos se permiten; consumidores deben usarlos con criterio.

## Referencias Cruzadas

- OpenSpec: `openspec/changes/scrumcore-297-crea-componente-apphorizontalscroller/`
- Índice documental: `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/00-indice.md`
- Arquitectura: `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/01-arquitectura.md`
- API y contrato visual: `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/02-api-contrato-visual.md`
- Responsive, accesibilidad y CSS: `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/03-responsive-accesibilidad-css.md`
- Uso e integración futura: `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/04-uso-e-integracion.md`
- Pruebas y validación: `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/05-pruebas-validacion.md`
- Riesgos y checklist: `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller/06-riesgos-checklist.md`
