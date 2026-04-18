## 1. Inventario tecnico del flujo de render

- [ ] 1.1 Revisar `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx` y mapear que estado local, callbacks, objetos y props se recrean por render
- [ ] 1.2 Revisar `useWorkflowInboxAutocomplete` y `useGestionCorrespondenciaTable` para identificar posibles fuentes externas de inestabilidad referencial
- [ ] 1.3 Identificar que hijos directos consumen props potencialmente sensibles a rerender: `AppToolbar`, `AppInputSearch`, `AppButton`, `AppContent`, `AppTableQueryWrapper`, `AppTableExport` y `AppTable`
- [ ] 1.4 Registrar el flujo funcional que no puede romperse: busqueda, autocomplete, actualizar, exportacion, seleccion y navegacion a `respuesta/:id`

## 2. Diagnostico estructural por lectura de codigo

- [ ] 2.1 Confirmar por lectura de codigo que `selectedRows` y `searchDraft` rerenderizan el page component
- [ ] 2.2 Confirmar por lectura de codigo que existen callbacks recreadas en cada render y clasificar su impacto esperado
- [ ] 2.3 Confirmar por lectura de codigo que existen objetos o JSX inline con identidad nueva por render: `actionContent`, `paginationActions`, `dataSource` y `responsivePresentation`
- [ ] 2.4 Separar lo confirmable por codigo de lo que solo puede concluirse con profiling

## 3. Analisis de riesgo real vs falso positivo

- [ ] 3.1 Clasificar cada foco como rerender esperado, rerender sospechoso o rerender claramente evitable
- [ ] 3.2 Clasificar cada hallazgo como problema real, sospecha razonable o falso positivo comun en React
- [ ] 3.3 Identificar que hallazgos nacen realmente en `GestionCorrespondencia.tsx` y cuales dependen del comportamiento interno de hooks o hijos directos
- [ ] 3.4 Identificar optimizaciones aparentes que no valen la pena por costo o complejidad

## 4. Medicion y profiling

- [ ] 4.1 Definir escenarios de profiling para escritura en buscador, cambio de seleccion, exportacion y refresco de tabla
- [ ] 4.2 Indicar que hallazgos deben validarse con React DevTools Profiler
- [ ] 4.3 Indicar en que casos `why-did-you-render` aportaria evidencia util y que comportamiento se esperaria observar
- [ ] 4.4 Registrar explicitamente las limitaciones si alguna conclusion no puede confirmarse en esta sesion solo con lectura de codigo

## 5. Recomendaciones y riesgos

- [ ] 5.1 Priorizar recomendaciones por severidad, costo de implementacion y beneficio esperado
- [ ] 5.2 Definir un plan minimo de refactor en orden de prioridad, sin sobre-optimizacion
- [ ] 5.3 Registrar riesgos de regresion para cada optimizacion candidata
- [ ] 5.4 Registrar de forma explicita que comportamiento funcional debe preservarse en cada refactor sugerido

## 6. Validacion y cierre del analisis

- [ ] 6.1 Relacionar el diagnostico con pruebas existentes de `GestionCorrespondencia`
- [ ] 6.2 Sugerir pruebas nuevas solo donde aporten evidencia de no regresion funcional
- [ ] 6.3 Definir que medir antes y despues de cualquier optimizacion propuesta
- [ ] 6.4 Consolidar el resultado final en formato de revision tecnica con hallazgos priorizados, zonas sanas, riesgos y estrategia de validacion
