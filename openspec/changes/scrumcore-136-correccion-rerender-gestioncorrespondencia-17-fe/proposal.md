## Why

Los tickets anteriores corrigieron inestabilidades referenciales en hooks y props derivadas de `GestionCorrespondencia`, pero aun faltaba confirmar con evidencia cuales rerenders siguen teniendo impacto real en UX y cuales son falsos positivos comunes en React.

El ticket `SCRUMCORE-136` busca convertir ese analisis en evidencia priorizada para decidir si vale la pena abrir nuevas optimizaciones o si el arbol actual ya esta en un punto aceptable.

## What Changes

- Agregar profiling automatizado orientado a interacciones reales de `GestionCorrespondencia`.
- Medir rerenders en typing, clear, seleccion, apertura de export y refresh.
- Documentar hallazgos priorizados separando problema real, sospecha razonable y falso positivo.
- Dejar recomendacion concreta sobre si vale la pena seguir optimizando el page component.

## Capabilities

### New Capabilities
- Ninguna.

### Modified Capabilities
- `gestion-correspondencia-profiling`: el cambio aporta evidencia de rerender para decidir prioridades de performance sin alterar UX ni comportamiento funcional.

## Impact

- Reduce incertidumbre tecnica antes de abrir nuevas optimizaciones.
- Evita sobre-optimizar zonas que no muestran impacto real.
- No introduce cambios funcionales en la pantalla.
