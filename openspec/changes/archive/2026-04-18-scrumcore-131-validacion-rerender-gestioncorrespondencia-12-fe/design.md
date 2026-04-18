## Context

`SCRUMCORE-131` busca formalizar una revision de performance enfocada en
rerenders innecesarios sobre
`src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`.

La pantalla ya cumple funcionalmente su objetivo:
- renderiza la bandeja con `AppToolbar`, `AppContent`, `AppTableQueryWrapper`
  y `AppTable`;
- mantiene busqueda con `AppInputSearch` y autocomplete desacoplado;
- soporta exportacion por medio de `AppTableExport`;
- navega al detalle `respuesta/:id` por click de celda o accion contextual.

El problema a resolver no es una falla funcional sino una duda de performance:
hay estados locales, callbacks y objetos inline que pueden introducir rerenders
amplios o props inestables. Sin embargo, en React ese patron no siempre implica
un problema real. El cambio necesita dejar un marco de evaluacion riguroso para:

- distinguir comportamiento esperado vs rerender evitable;
- separar hallazgos confirmables por lectura de codigo de hallazgos que exigen
  profiling;
- priorizar optimizaciones de bajo riesgo;
- evitar sobre-optimizacion con `React.memo`, `useMemo` o `useCallback`
  indiscriminados.

La referencia funcional del cambio es
`docs/Architecture/GestionCorrrespondecia/12-FE-Revision-performance-rerenders-GestionCorrespondencia.md`.

## Goals / Non-Goals

**Goals:**
- Documentar un modelo de analisis centrado en `GestionCorrespondencia.tsx` y su
  arbol inmediato.
- Evaluar explicitamente el impacto de `selectedRows`, `searchDraft`,
  callbacks recreadas y objetos inline.
- Dejar claro que hallazgos pueden sostenerse solo leyendo codigo y cuales
  requieren React DevTools Profiler o `why-did-you-render`.
- Producir recomendaciones con costo, beneficio y riesgo de regresion.
- Preservar el comportamiento actual de busqueda, exportacion, seleccion y
  navegacion.

**Non-Goals:**
- No implementar aun optimizaciones de codigo.
- No rediseñar `GestionCorrespondencia`, `AppTable`, `AppToolbar` ni
  `AppInputSearch`.
- No extender el alcance a componentes nietos salvo cuando el origen del
  rerender salga claramente de un hijo directo o de un hook proveedor.
- No asumir que la presencia de callbacks u objetos inline implica por si sola
  una regresion observable.
- No introducir benchmarking sintetico ni telemetria permanente en este ticket.

## Decisions

1. **El analisis se centra en fronteras de render, no en micro-optimizaciones aisladas**
   - **Decision:** Evaluar el page component como frontera principal de rerender:
     estado local, props emitidas y contratos hacia hijos directos.
   - **Rationale:** El costo real de un rerender en esta pantalla depende menos
     de una closure aislada y mas de que subarboles relevantes reciban nuevas
     props en cascada.
   - **Alternatives considered:** Auditar todos los descendientes profundos del
     modulo. Se descarta por inflar el alcance y mezclar problemas locales con
     problemas shared ajenos a este cambio.

2. **Distinguir evidencia estructural de evidencia runtime**
   - **Decision:** Clasificar hallazgos en dos grupos:
     a) confirmables por lectura de codigo;
     b) dependientes de medicion.
   - **Rationale:** En `GestionCorrespondencia.tsx` si puede confirmarse por
     codigo que `selectedRows`, `searchDraft`, callbacks y objetos inline
     recrean render o identidad referencial; lo que no puede confirmarse sin
     medir es si hijos como `AppTableExport` o `AppTable` realmente pagan un
     costo significativo por ello.
   - **Alternatives considered:** Marcar todo como potencial problema. Se
     descarta porque degrada la calidad del diagnostico y empuja memoizacion sin
     evidencia.

3. **`selectedRows` y `searchDraft` se tratan primero como rerenders esperados**
   - **Decision:** Considerar de entrada que ambos estados provocan rerender del
     componente padre por diseño, y solo elevarlos a problema si el analisis o
     profiling demuestra costo amplio evitable.
   - **Rationale:** React rerenderiza el componente que posee el estado. El
     hallazgo importante no es esa realidad basica, sino si el estado vive en la
     frontera equivocada o arrastra props costosas hacia hijos sensibles.
   - **Alternatives considered:** Declarar automaticamente que el estado debe
     extraerse a componentes hijos. Se descarta por prematuro y por posible
     aumento innecesario de complejidad.

4. **Los objetos inline y callbacks inestables se modelan como sospecha tecnica inicial**
   - **Decision:** Tratar `actionContent`, `paginationActions`, `dataSource`,
     `responsivePresentation`, `applySearch`, `handleSearchChange`,
     `handleSearchClear`, `navigateToRowDetail`, `handleTableAction` y
     `handleTableCellClick` como focos obligatorios de revision, pero no como
     bugs confirmados por si mismos.
   - **Rationale:** Su identidad cambia por render y eso es objetivamente
     verificable; sin embargo, el impacto depende de si los consumidores aplican
     memoizacion o hacen trabajo costoso cuando cambia la referencia.
   - **Alternatives considered:** Recomendar `useMemo` y `useCallback` a todo el
     conjunto. Se descarta porque puede introducir ruido, dependencias mas
     fragiles y ganancia nula.

5. **El contrato `table` se considera frontera de incertidumbre externa**
   - **Decision:** Evaluar `table.rows`, `table.columns`, `table.queryState`,
     `table.onQueryChange`, `table.refetch`, `table.getAllMatchingRows` y
     `table.getBackendExportFile` como posibles orígenes externos de inestabilidad
     referencial.
   - **Rationale:** `GestionCorrespondencia` recibe ese contrato ya construido.
     Si cambia de identidad con frecuencia desde `useGestionCorrespondenciaTable`,
     el page component puede estar reaccionando correctamente a un problema que
     nace aguas arriba.
   - **Alternatives considered:** Atribuir toda inestabilidad al page component.
     Se descarta porque ocultaria causas reales en el hook proveedor.

6. **La recomendacion de optimizar solo procede con preservacion funcional explicita**
   - **Decision:** Toda optimizacion propuesta debe preservar:
     busqueda, autocomplete, boton actualizar, exportacion, seleccion de filas y
     navegacion a `respuesta/:id`.
   - **Rationale:** El principal riesgo de una optimizacion de render en esta
     pantalla es romper sincronizacion entre UI, query state y acciones de tabla.
   - **Alternatives considered:** Permitir refactors estructurales agresivos si
     prometen menos renders. Se descarta por alto riesgo y baja trazabilidad.

7. **Profiling dirigido antes/despues es obligatorio para cambios que toquen fronteras de render**
   - **Decision:** Si la recomendacion propone mover estado, memoizar props
     complejas o aislar subarboles, debe definirse una validacion con React
     DevTools Profiler y, cuando aplique, `why-did-you-render`.
   - **Rationale:** El valor de estas optimizaciones esta en reducir trabajo
     observable, no solo en producir codigo "mas estable" teoricamente.
   - **Alternatives considered:** Confiar solo en razonamiento estatico o en
     tests unitarios. Se descarta porque los tests validan comportamiento, no
     costo de rerender.

## Risks / Trade-offs

- [Riesgo] Tratar toda identidad nueva como problema real.
  Mitigacion: clasificar por evidencia y separar sospecha de confirmacion.

- [Riesgo] Recomendar memoizacion en masa y volver mas fragil el componente.
  Mitigacion: exigir costo/beneficio y rechazar optimizaciones sin impacto claro.

- [Riesgo] Ocultar la causa real en `useGestionCorrespondenciaTable` o en hijos
  shared como `AppTableExport`.
  Mitigacion: modelar el contrato `table` y los hijos directos como fronteras de
  analisis separadas.

- [Riesgo] Romper sincronizacion entre `searchDraft`, autocomplete y
  `table.onQueryChange` al intentar aislar renders.
  Mitigacion: declarar ese flujo como area de no regresion obligatoria.

- [Riesgo] Optimizar seleccion de filas y degradar exportacion o comportamiento
  de `selectedRows`.
  Mitigacion: validar especificamente `getSelectedRows`, exportacion y seleccion
  tras cualquier recomendacion.

- [Riesgo] Sacar conclusiones fuertes sin profiling sobre `AppTable` o
  `AppTableExport`.
  Mitigacion: marcar como requeriente de medicion todo lo que dependa del costo
  interno de hijos memoizados o complejos.

## Migration Plan

- Consolidar la spec de auditoria de rerender para el capability
  `validacion-rerender-gestioncorrespondencia-12-fe`.
- Ejecutar la revision del codigo de `GestionCorrespondencia.tsx` y de sus
  contratos inmediatos.
- Clasificar hallazgos en confirmables por codigo vs dependientes de medicion.
- Construir recomendaciones priorizadas con costo, beneficio y riesgos.
- Definir estrategia de profiling antes/despues para optimizaciones candidatas.
- Convertir el diagnostico en `tasks.md` accionables y de bajo riesgo.

## Open Questions

- `useGestionCorrespondenciaTable` estabiliza realmente `rows`, `columns` y
  callbacks entre renders o recrea el contrato completo en cada actualizacion?
- `AppTableExport` y `AppTableQueryWrapper` aplican memoizacion interna o tratan
  cada prop nueva como un rerender completo del subarbol?
- El flujo de autocomplete dispara rerenders costosos por cada tecla o el costo
  dominante esta fuera del page component?
- `selectedRows` necesita vivir en el page component para exportacion o podria
  aislarse sin empeorar trazabilidad y simplicidad?
