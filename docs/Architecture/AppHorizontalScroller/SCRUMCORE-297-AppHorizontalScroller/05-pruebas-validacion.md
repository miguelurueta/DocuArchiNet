# 05 Pruebas Y Validación

## Estrategia De Pruebas

- Render y accesibilidad: `children`, `role`, `aria-label`, `testId`.
- Variantes: densidades y gaps.
- Dimensiones: números a px, strings no vacíos y valores inválidos ignorados.
- Dimensiones inválidas: strings vacíos, strings negativos, cero, negativos numéricos, `NaN` e infinitos ignorados.
- Composición: no mutar ni clonar hijos.
- Scroll snap: `none`, `start`, `center`.
- Edge fade: clase visual y regla no bloqueante en CSS.
- Render defensivo: `children={null}`.
- Auditoría de acoplamiento: sin `axios`, `fetch`, servicios, negocio, `AppTable` ni `AppTreeTable`.

## Evidencia De Validación

- Tests focalizados: `npm.cmd run test -- src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx` (`14 passed`).
- Lint focalizado: `node_modules\.bin\eslint.cmd src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.tsx src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx`.
- Auditoría de acoplamiento: búsqueda sin resultados para `axios`, `fetch(`, servicios, hooks de dominio, `AppTable`, `AppTreeTable` y `GestionCorrespondencia` dentro de `src/app/Components/UI/AppHorizontalScroller`.
- CSS auditado: `scroll-snap-type: x proximity` y `pointer-events: none` para edge fade.
- Validación responsive mínima: el contrato visual se verificó contra CSS del componente (`max-width: 100%`, `min-width: 0`, `overflow-x: auto`, `overflow-y: hidden`, `-webkit-overflow-scrolling: touch`, fila flex sin wrap, `width: max-content` y anchos por custom properties). No se generó screenshot porque este ticket no integra un consumidor de pantalla.

## Comandos Usados

```powershell
npm.cmd run test -- src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx
node_modules\.bin\eslint.cmd src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.tsx src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx
npm.cmd run spec:validate
git diff --check
rg "axios|fetch\(|services|hooks|AppTable|AppTreeTable|GestionCorrespondencia" src\app\Components\UI\AppHorizontalScroller
```

## Build

`npm.cmd run build` falla por un error TypeScript preexistente fuera del alcance:

```txt
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx(8,3): error TS2724: "../../../almacenamientoDocumental/components/AppUploadDocumental" has no exported member named 'UploadDocumentalStoredContext'. Did you mean 'UploadDocumentalContext'?
```

El error pertenece a `gestionCorrespondencia`/`AppUploadDocumental` y no a `AppHorizontalScroller`.

## Observación De Runner

Cuando se ejecutó el test focalizado en paralelo con otros comandos, Vitest llegó a resolver un cwd temporal del sandbox y falló buscando `src/setupTests.ts`. Al reejecutar el comando de test en sesión única desde el workspace real, el resultado fue correcto: `14 passed`.
