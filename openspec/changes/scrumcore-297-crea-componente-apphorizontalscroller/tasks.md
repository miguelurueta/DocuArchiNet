## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira y contexto del codigo.
- [x] 1.2 Corregir nomenclatura a `AppHorizontalScroller` y capability `app-horizontal-scroller`.
- [x] 1.3 Definir no objetivos: sin API, sin dominio, sin SCRUM-162, sin cambios en `AppTable`/`AppTreeTable`.
- [x] 1.4 Ajustar design/spec con decisiones, riesgos y criterios verificables.

## 2. Implementacion

- [ ] 2.1 Crear `src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.tsx`.
- [ ] 2.2 Crear `AppHorizontalScroller.module.css` con layout horizontal, densidades, gaps, snap y edge fade no bloqueante.
- [ ] 2.3 Crear `index.ts` del componente y actualizar barrel superior solo si el patron local lo requiere.
- [ ] 2.4 Implementar normalizacion segura de dimensiones `number | string` sin aceptar valores invalidos.
- [ ] 2.5 Mantener el componente sin estado interno, sin efectos, sin HTTP y sin imports de modulos de negocio.

## 3. Pruebas

- [ ] 3.1 Agregar pruebas unitarias de render, `role`, `aria-label` y `testId`.
- [ ] 3.2 Probar densidades `compact` y `comfortable`.
- [ ] 3.3 Probar gaps `xs`, `sm`, `md`, `lg`.
- [ ] 3.4 Probar custom properties de `itemMinWidth` e `itemMaxWidth`, conversion a px e ignorar valores invalidos.
- [ ] 3.5 Probar scroll snap `none`, `start`, `center`.
- [ ] 3.6 Probar `edgeFade` y render sin fallar con `children={null}`.
- [ ] 3.7 Ejecutar suite focalizada del componente.

## 4. Documentacion

- [ ] 4.1 Crear `docs/Architecture/AppHorizontalScroller/SCRUMCORE-297-AppHorizontalScroller.md`.
- [ ] 4.2 Documentar objetivo, alcance, no objetivos, API, responsive behavior, accesibilidad, decisiones, riesgos, pruebas y relacion futura con SCRUM-162.

## 5. Cierre

- [ ] 5.1 Validar OpenSpec.
- [ ] 5.2 Ejecutar lint/typecheck/build si aplica y registrar errores preexistentes si aparecen.
- [ ] 5.3 Revisar que no haya cambios en `AppTable`, `AppTreeTable` ni modulos de negocio.
