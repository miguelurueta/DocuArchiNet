# Hallazgos de profiling de GestionCorrespondencia

## Alcance

- Componente objetivo: `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
- Evidencia automatizada: `src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.profiling.test.tsx`
- Escenarios cubiertos:
  - escritura de 5 caracteres en busqueda
  - limpiar busqueda
  - cambiar seleccion de fila
  - abrir flujo de exportacion
  - refresh

## Hallazgos priorizados

### Problema real

1. `AppTableExport` sigue rerenderizando durante typing y clear de busqueda local.
   - Causa raiz probable: `formats` y `enabledModes` siguen siendo arrays inline nuevos por render en `GestionCorrespondencia`.
   - Evidencia automatizada: en la prueba de profiling `AppTableExport` rerenderiza en cada cambio de draft mientras `AppTable` permanece estable.
   - Impacto UX: bajo a medio; frecuente por ocurrir durante typing.
   - Recomendacion: este es el siguiente ticket con mejor relacion costo/beneficio.

2. `selectedRows` sigue provocando rerender de `AppTableExport`.
   - Causa raiz: `dataSource.getSelectedRows` cambia cuando cambia la seleccion.
   - Impacto UX: bajo a medio.
   - Justificacion: es un rerender semantico porque el modo `selectedRows` depende de ese dato.
   - Recomendacion: no optimizar mas aqui salvo que el profiler visual muestre costo alto dentro de `AppTableExport`.

### Sospecha razonable

1. `AppToolbar`, `AppContent` y `AppTableQueryWrapper` pueden rerenderizar cuando cambia `searchDraft`.
   - Causa raiz: el page component se rerenderiza por estado local y vuelve a crear `actionContent`/`paginationActions` como JSX.
   - Impacto UX: probablemente bajo.
   - Recomendacion: solo medir con React DevTools Profiler real si typing presenta lag perceptible.

### Falsos positivos comunes

1. Abrir el flujo de exportacion rerenderiza `AppTableExport`.
   - Causa raiz: estado interno/local del propio componente de exportacion.
   - Impacto UX: esperado.
   - Recomendacion: no atacar desde `GestionCorrespondencia`.

2. `AppTable` durante typing, clear y refresh.
   - Evidencia automatizada: no rerenderiza en la prueba de profiling en esos escenarios.
   - Conclusión: la memoizacion de props derivadas del ticket 16 redujo ruido referencial en este borde.

3. `AppTableExport` durante refresh.
   - Evidencia automatizada: el click de refresh no introduce rerender local en `GestionCorrespondencia` cuando solo se invoca `refetch`.
   - Conclusión: el costo real del refresh debe medirse en el hook de datos o en la respuesta de la query, no en el page component.

## Recomendacion de prioridad

1. Implementar primero un ticket pequeño para estabilizar `formats` y `enabledModes` que hoy siguen invalidando `AppTableExport` durante typing.
2. Si se detecta lag real adicional, perfilar internamente `AppTableExport` durante apertura/export real.
3. Despues de eso, revisar `AppInputSearch` y `AppToolbar` solo si React DevTools muestra commits costosos durante typing.

## Limitaciones de la evidencia

- Esta evidencia automatizada mide rerenders y cambios de props en un entorno de prueba con hijos memoizados.
- No reemplaza completamente React DevTools Profiler para commit time real.
- No se uso `why-did-you-render` porque el arbol ya quedo suficientemente acotado con instrumentacion dirigida.

## Conclusion

La mayor parte de los rerenders que quedaban como sospecha en `GestionCorrespondencia` ya no muestran impacto claro en `AppTable` para typing, clear o refresh. El hallazgo relevante que sí queda abierto es `AppTableExport` durante typing y clear, probablemente por arrays inline que siguen cambiando referencia. Ese es el siguiente candidato real de optimizacion; el resto del arbol hoy luce mas cerca de falso positivo o rerender esperado.
