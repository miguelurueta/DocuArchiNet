## Context

`SCRUMCORE-128` corresponde a la fase 18 FE de `AppEditor`, enfocada en
auditar su comportamiento funcional y diagnosticar, con evidencia, si existen
re-renders innecesarios o zonas de arquitectura que convenga refinar.

`AppEditor` ya soporta:
- edicion enriquecida basada en TipTap y HTML serializado;
- modo controlled/uncontrolled;
- toolbar propia con comandos de formato;
- insercion de links;
- insercion y rehidratacion de imagenes locales/remotas;
- paginacion visual multi-hoja;
- `PageBreak`;
- zoom visual;
- dirty state y accion de guardar.

El problema actual no es una regresion funcional evidente sino la necesidad de
subir el nivel tecnico de observabilidad y criterio sobre performance real.
Antes de introducir optimizaciones, el cambio debe separar claramente:
- renders esperables por la naturaleza de React o TipTap;
- renders redundantes que si afectan costo de UI;
- optimizaciones aparentes que solo agregarian complejidad.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/18-FE-AppEditor-auditoria-tecnica-optimizacion-rerender.md`.

## Goals / Non-Goals

**Goals:**
- auditar integralmente `AppEditor` y sus piezas relacionadas;
- identificar causas concretas de re-render innecesario;
- distinguir evidencia real de sospechas o falsos positivos;
- preservar completamente la funcionalidad actual del editor;
- proponer mejoras con severidad, costo, beneficio y riesgo de regresion;
- definir un plan incremental y verificable para futuras optimizaciones.

**Non-Goals:**
- no introducir optimizaciones preventivas sin evidencia;
- no reescribir la arquitectura del editor solo por limpieza teorica;
- no degradar UX, foco, seleccion, toolbar, imagenes, zoom o paginacion;
- no forzar `memo`, `useMemo` o `useCallback` por estilo;
- no mezclar esta fase con features nuevas ajenas a performance y auditoria.

## Decisions

1. **Tratar la auditoria como un cambio de diagnostico estructurado**
   - **Decision:** Este cambio debe producir un diagnostico tecnico accionable
     antes de cualquier refactor amplio, usando el codigo actual y sus pruebas
     como fuente de verdad.
   - **Rationale:** `AppEditor` es un componente con varias fases acumuladas:
     TipTap, imagenes, paginacion, zoom y dirty state. Optimizar sin
     diagnostico previo es una forma rapida de romper interacciones sutiles.
   - **Alternatives considered:** Entrar directo a refactorizar `AppEditor.tsx`
     o aislar componentes "porque parecen grandes". Se descarta por alto riesgo
     de sobre-ingenieria y regresiones invisibles.

2. **Usar evidencia antes que intuicion para hablar de re-render**
   - **Decision:** Toda conclusion debe apoyarse en profiling, trazas
     observables o lectura de flujo suficientemente concreta del codigo.
   - **Rationale:** En React complejo es facil confundir renders legitimos con
     problemas reales. TipTap, estado controlado y layout visual pueden generar
     renders necesarios que no deben optimizarse agresivamente.
   - **Alternatives considered:** Basar recomendaciones en reglas generales
     tipo "si hay inline callbacks hay problema". Se descarta porque lleva a
     optimizaciones cosmeticas con poco o nulo retorno.

3. **Separar el analisis por capas del editor**
   - **Decision:** La auditoria debe revisar por separado:
     - shell de presentacion (`AppEditor.tsx`);
     - toolbar (`AppEditorToolbar.tsx`);
     - orquestacion del editor (`useAppEditor.ts`);
     - hooks de layout (`usePaginationMetrics.ts`, `usePageContext.ts`);
     - estilos y pruebas relacionadas.
   - **Rationale:** No todos los renders tienen el mismo costo ni el mismo
     riesgo. Un re-render barato en toolbar no se prioriza igual que uno que
     reprocesa metricas visuales o induce trabajo extra sobre ProseMirror.
   - **Alternatives considered:** Tratar `AppEditor` como una sola caja negra.
     Se descarta porque oculta la causa raiz y vuelve vagas las decisiones.

4. **Preservar comportamiento funcional como restriccion principal**
   - **Decision:** Ninguna optimizacion se considera valida si amenaza:
     escritura, seleccion, comandos, imagenes, `PageBreak`, paginacion, zoom,
     dirty state, save state o accesibilidad basica.
   - **Rationale:** En un editor enriquecido, la estabilidad funcional vale mas
     que una micro-mejora de render. Un ahorro pequeno no compensa romper foco,
     cursor, overlays o sincronizacion del HTML.
   - **Alternatives considered:** Priorizar agresivamente performance aun con
     cambios de arquitectura grandes. Se descarta por el costo de regresion.

5. **Diferenciar optimizacion de aislamiento**
   - **Decision:** El cambio debe evaluar si conviene:
     - aislar visualmente componentes;
     - estabilizar props especificas;
     - mover trabajo derivado fuera del render;
     - o no tocar nada si el costo no se justifica.
   - **Rationale:** El remedio depende de la causa. No todo se resuelve con
     `React.memo`; a veces el problema real es un efecto redundante, una
     derivacion costosa o una frontera de responsabilidades mal ubicada.
   - **Alternatives considered:** Hacer memoizacion por defecto en todo el
     arbol. Se descarta por complejidad adicional y riesgo de stale props.

6. **Mantener las pruebas actuales como red minima de seguridad**
   - **Decision:** Las recomendaciones del analisis deben mapearse a una
     estrategia de validacion basada en pruebas existentes y, solo si hace
     falta, nuevos tests focalizados.
   - **Rationale:** El editor ya tiene cobertura amplia por fases. Esa suite es
     el ancla para evaluar si un refactor de performance conserva contratos.
   - **Alternatives considered:** Confiar solo en inspeccion manual del editor.
     Se descarta porque no cubre bien regressions en hooks y estados derivados.

7. **Documentar explicitamente lo que no vale la pena optimizar**
   - **Decision:** El entregable debe incluir una seccion donde se indiquen
     optimizaciones descartadas por bajo impacto o costo excesivo.
   - **Rationale:** En performance frontend, saber que no tocar es tan valioso
     como saber que cambiar. Evita churn tecnico y discusiones recurrentes.
   - **Alternatives considered:** Reportar solo problemas. Se descarta porque
     deja abierta la puerta a micro-optimizaciones futuras sin criterio comun.

## Risks / Trade-offs

- [Riesgo] Confundir un render normal de React o TipTap con un problema real de
  performance.
  Mitigacion: exigir evidencia concreta y clasificar hallazgos por nivel de
  certeza.

- [Riesgo] Proponer `memo`, `useMemo` o `useCallback` donde el costo mental sea
  mayor que el beneficio.
  Mitigacion: exigir costo/beneficio y explicitar cuando no valen la pena.

- [Riesgo] Introducir optimizaciones que rompan foco, seleccion, page context,
  toolbar o sincronizacion del editor.
  Mitigacion: mantener la validacion funcional como restriccion principal y
  apoyar cada recomendacion en pruebas de regresion.

- [Riesgo] Mezclar en el mismo cambio problemas de performance real con deuda
  de legibilidad o gusto de estilo.
  Mitigacion: limitar el alcance a performance observable, bugs potenciales y
  decisiones arquitectonicas con impacto real.

## Migration Plan

- Inventariar piezas de `AppEditor` y sus dependencias relevantes.
- Revisar implementacion y pruebas para identificar puntos de render y trabajo
  derivado sospechoso.
- Ejecutar o documentar profiling/medicion equivalente para separar evidencia
  real de sospechas.
- Priorizar hallazgos por severidad, costo, beneficio y riesgo.
- Proponer fases pequenas de refactor, cada una con validacion antes/despues.
- Reservar la implementacion de optimizaciones para tickets posteriores o para
  una fase controlada si la evidencia lo justifica.

## Open Questions

- ¿Conviene dejar el resultado de esta auditoria solo como diagnostico y plan,
  o agrupar tambien un primer refactor de bajo riesgo si se detecta una mejora
  evidente y bien acotada?
- ¿La medicion principal debe centrarse en escritura interactiva, cambios de
  toolbar, insercion de imagenes o paginacion visual, o en una combinacion de
  escenarios representativos del uso real?
