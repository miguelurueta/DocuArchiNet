## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira y contexto del codigo.
- [x] 1.2 Corregir nomenclatura a `AppHorizontalScroller` y capability `app-horizontal-scroller`.
- [x] 1.3 Definir no objetivos: sin API, sin dominio, sin SCRUM-162, sin cambios en `AppTable`/`AppTreeTable`.
- [x] 1.4 Ajustar design/spec con decisiones, riesgos y criterios verificables.

## 2. Implementacion

- [ ] 2.1 Crear `src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.tsx`.
- [ ] 2.2 Definir tipos publicos `AppHorizontalScrollerDensity`, `AppHorizontalScrollerGap`, `AppHorizontalScrollerSnap` y `AppHorizontalScrollerProps`.
- [ ] 2.3 Implementar props completas: `children`, `ariaLabel`, `className`, `viewportClassName`, `contentClassName`, `density`, `gap`, `itemMinWidth`, `itemMaxWidth`, `scrollSnap`, `edgeFade` y `testId`.
- [ ] 2.4 Aplicar defaults: `density="comfortable"`, `gap="md"`, `scrollSnap="none"` y `edgeFade=false`.
- [ ] 2.5 Usar function component tipado, `import type`, sin `React.FC`, sin `forwardRef` salvo necesidad real, sin estado interno y sin efectos.
- [ ] 2.6 Implementar layout con region accesible, fila horizontal, `overflow-x: auto`, `overflow-y` controlado y soporte responsive mobile/tablet/desktop.
- [ ] 2.7 Implementar normalizacion segura de dimensiones `number | string`: convertir numeros positivos a `px`, aceptar strings no vacios e ignorar vacios, cero, negativos, `NaN` e infinitos.
- [ ] 2.8 Exponer custom properties `--app-horizontal-scroller-item-min-width` y `--app-horizontal-scroller-item-max-width` sin mutar ni clonar `children`.
- [ ] 2.9 Crear `AppHorizontalScroller.module.css` con CSS Modules, sin estilos globales, sin paleta de negocio, sin cards internas, sin `position: fixed`, con `box-sizing`, `max-width: 100%`, `min-width: 0` y `-webkit-overflow-scrolling: touch`.
- [ ] 2.10 Implementar densidades `compact` y `comfortable`, y gaps `xs`, `sm`, `md`, `lg`.
- [ ] 2.11 Implementar scroll snap opcional con `scroll-snap-type: x proximity`, align `start`/`center` y sin usar `mandatory`.
- [ ] 2.12 Implementar `edgeFade` con capa/pseudo-elemento no bloqueante y `pointer-events: none`.
- [ ] 2.13 Revisar patron de export de componentes vecinos (`AppButton`, `AppContent`, `AppInputSearch`) y replicarlo sin inventar estructura nueva.
- [ ] 2.14 Crear `index.ts` del componente y actualizar barrel superior solo si el patron local lo requiere.

## 3. Pruebas

- [ ] 3.1 Agregar pruebas unitarias de render, `role`, `aria-label` y `testId`.
- [ ] 3.2 Probar densidades `compact` y `comfortable`.
- [ ] 3.3 Probar gaps `xs`, `sm`, `md`, `lg`.
- [ ] 3.4 Probar props `className`, `viewportClassName` y `contentClassName`.
- [ ] 3.5 Probar custom properties de `itemMinWidth` e `itemMaxWidth` con conversion de numeros a px.
- [ ] 3.6 Probar strings no vacios para dimensiones.
- [ ] 3.7 Probar que strings vacios, cero, negativos, `NaN` e infinitos no aplican custom properties invalidas.
- [ ] 3.8 Probar que no se mutan ni clonan hijos para inyectar props.
- [ ] 3.9 Probar scroll snap `none`, `start`, `center`.
- [ ] 3.10 Probar que el snap usa comportamiento de proximidad y no `mandatory`.
- [ ] 3.11 Probar `edgeFade`, incluyendo clase/estilo que garantice `pointer-events: none`.
- [ ] 3.12 Probar render sin fallar con `children={null}`.
- [ ] 3.13 Probar que no se agrega `tabIndex` al viewport y no se interceptan eventos de teclado.
- [ ] 3.14 Probar o verificar por fuente que no hay imports/calls de `axios`, `fetch`, servicios HTTP, modulos de negocio, `AppTable` ni `AppTreeTable`.
- [ ] 3.15 Ejecutar suite focalizada del componente.
- [ ] 3.16 Validar visualmente el scroller en anchos mobile, tablet y desktop usando un ejemplo local, fixture de prueba o evidencia documentada.

## 4. Documentacion

- [ ] 4.1 Crear `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller.md`.
- [ ] 4.2 Documentar objetivo, alcance y no objetivos.
- [ ] 4.3 Documentar arquitectura y separacion entre consumidor de dominio y `AppHorizontalScroller`.
- [ ] 4.4 Documentar API completa de props y defaults.
- [ ] 4.5 Documentar contrato visual, reglas responsive y manejo de dimensiones.
- [ ] 4.6 Documentar accesibilidad: `role="region"`, `aria-label`, sin `tabIndex`, sin interceptar teclado y foco delegado a hijos.
- [ ] 4.7 Documentar reglas CSS: CSS Modules, no globales, no paleta de negocio, no cards internas, `min-width: 0`, `max-width: 100%`, scroll nativo y mobile touch.
- [ ] 4.8 Documentar decisiones tecnicas, restricciones, riesgos y mitigaciones.
- [ ] 4.9 Documentar ejemplos de uso y diagrama de composicion.
- [ ] 4.10 Documentar estrategia de pruebas, checklist de aceptacion y relacion futura con SCRUM-162 dejando claro que este SCRUM no consume ese contrato.

## 5. Cierre

- [ ] 5.1 Validar OpenSpec.
- [ ] 5.2 Ejecutar lint/typecheck/build si aplica y registrar errores preexistentes si aparecen.
- [ ] 5.3 Ejecutar `git diff --check`.
- [ ] 5.4 Revisar que no haya cambios en `AppTable`, `AppTreeTable`, `GestionCorrespondencia`, servicios HTTP ni modulos de negocio.
- [ ] 5.5 Confirmar que no se agregaron dependencias nuevas.
- [ ] 5.6 Auditar que el componente no importe `axios`, no llame `fetch`, no use servicios HTTP, no importe modulos de negocio y no dependa de `AppTable` ni `AppTreeTable`.
