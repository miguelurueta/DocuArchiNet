# Evidence - SCRUMCORE-254

## Validation

```txt
npx tsc --noEmit
Resultado: OK
```

```txt
npx eslint src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace src/modules/digitalizacion/components/DigitalizacionDocumentalModal/DigitalizacionDocumentalModal.test.tsx src/app/Components/UI/AppDigitalizador/tests/AppDigitalizador.test.tsx
Resultado: OK
```

```txt
npm run spec:validate
Resultado: OK
Total specs: 16
Specs missing: 0
Unknown tags in tests: 0
```

```txt
npm run test -- src/app/Components/UI/AppDigitalizador/tests/AppDigitalizador.test.tsx --run
Resultado: OK
Test Files: 1 passed
Tests: 7 passed
```

```txt
npm run test -- src/app/Components/UI/AppCollapseRail/AppCollapseRail.test.tsx --run
Resultado: OK
Test Files: 1 passed
Tests: 5 passed
```

```txt
npm run test -- src/modules/digitalizacion src/app/Components/UI/AppDigitalizador --run
Resultado: OK
Test Files: 10 passed
Tests: 72 passed
Nota: Vitest reporto warnings existentes de jsdom/getComputedStyle y React act en DigitalizacionDocumentalModal, sin fallos.
```

## Implemented Scope

- Miniaturas and Configuracion wrapped with reusable `AppCollapseRail` inline rails.
- CSS Grid column collapse to `0` for hidden side panels.
- Preview PDF expands when one or both side panels are hidden.
- `localStorage` persistence for `showThumbnails` and `showConfiguration`.
- Side panels remain mounted to preserve miniatures, selection, drag and drop, scroll and scanner configuration state.
- Audit documentation added at `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-265-collapsible-panels.md`.
