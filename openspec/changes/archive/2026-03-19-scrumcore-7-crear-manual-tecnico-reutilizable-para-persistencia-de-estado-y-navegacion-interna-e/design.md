## Context

El ticket `SCRUMCORE-7` busca formalizar en la documentacion del proyecto una metodologia reutilizable para resolver navegacion interna entre componentes `tsx` sin perder el estado critico del flujo. La necesidad surgio al revisar el modulo de `radicacion`, donde coexisten un layout persistente a nivel de dashboard y vistas internas que hoy alternan entre tabs y componentes funcionales con estado local.

El repositorio ya usa una SPA con React Router v7, layouts con `Outlet`, React Query para cache de datos y modulos organizados por dominio. Sin embargo, no existe hoy un manual tecnico que explique con claridad cuando conviene usar un contenedor persistente local del modulo frente a un layout con subrutas internas, ni como aplicar esa decision de forma consistente en otros modulos.

El cambio es transversal aunque no modifica codigo productivo. Define una referencia arquitectonica y operativa que debe ser util para futuros tickets de implementacion, refactors modulares y automatizacion asistida por IA o Codex.

## Goals / Non-Goals

**Goals:**
- Crear un manual tecnico en formato Markdown, claro, estructurado y reutilizable.
- Documentar dos metodos de implementacion:
  - Metodo A: contenedor persistente con vistas internas sin cambio de ruta.
  - Metodo B: layout persistente con subrutas internas y `Outlet`.
- Explicar objetivo, diferencias, casos de uso, arquitectura, flujo paso a paso, ejemplos practicos, buenas practicas, riesgos y criterios de decision.
- Incluir lineamientos para aplicar la metodologia en otros modulos del sistema.
- Incluir un prompt profesional final para Jira orientado a Codex o IA que permita implementar la metodologia respetando la arquitectura vigente.

**Non-Goals:**
- Implementar en este ticket una refactorizacion funcional en `radicacion` u otro modulo.
- Introducir nuevas dependencias, patrones de routing o stores globales en codigo productivo.
- Reemplazar la documentacion tecnica existente de OpenSpec, Jira o integracion GitHub; este manual la complementa.

## Decisions

### Decision 1: Tratar el manual como referencia arquitectonica reusable y no como nota puntual del modulo
- **Decision:** el documento se redactara como manual transversal para la SPA y no como un analisis exclusivo de `radicacion`.
- **Rationale:** la necesidad aparecio en un modulo concreto, pero el patron aplica igual a `workflow`, `gestion documental`, paneles multi-step y otros modulos con ownership de estado y navegacion interna.
- **Alternatives considered:** documentarlo solo como caso especifico de `radicacion`; se descarta porque reduce la reutilizacion y obliga a reinterpretar el criterio en cada modulo futuro.

### Decision 2: Formalizar la comparacion entre Metodo A y Metodo B como decision arquitectonica explicita
- **Decision:** el manual incluira una comparativa clara entre ambos metodos, con ventajas, limites, riesgos y una matriz de uso.
- **Rationale:** el problema recurrente no es tecnico en una sola dimension; depende de si se necesita URL, historial, deep-linking, preservacion estricta del formulario o escalabilidad del subdominio. Documentar solo un enfoque dejaria huecos de criterio.
- **Alternatives considered:** recomendar un unico patron por defecto; se descarta porque algunos modulos requieren rutas internas reales mientras otros funcionan mejor como flujo local persistente.

### Decision 3: Centrar la metodologia en ownership del estado y evitar la falsa meta de eliminar todo re-render
- **Decision:** el manual dejara explicito que el objetivo no es suprimir todos los re-render, sino evitar desmontar el contenedor que posee el estado critico.
- **Rationale:** esa distincion evita decisiones tecnicamente incorrectas, hacks de keep-alive prematuros y optimizaciones sin impacto real. Tambien alinea el criterio con el comportamiento esperado de React Router y React.
- **Alternatives considered:** formular el problema como "evitar rerender del principal"; se descarta porque induce soluciones equivocadas y no describe bien la causa de perdida de estado.

### Decision 4: Incluir un prompt profesional orientado a implementacion asistida por IA
- **Decision:** el manual cerrara con un prompt estructurado para Jira/Codex/IA, listo para reutilizar en tickets de implementacion.
- **Rationale:** el repositorio ya integra flujos Jira -> OpenSpec -> Codex, asi que convertir la metodologia en instruccion operativa reusable reduce ambiguedad y mejora consistencia de futuras implementaciones.
- **Alternatives considered:** dejar solo narrativa humana del manual; se descarta porque el equipo ya usa automatizacion y necesita una salida directamente accionable.

### Decision 5: Ubicar el manual en `docs/` y redactarlo en Markdown con estructura estable
- **Decision:** el artefacto final se escribira como documento Markdown en la carpeta de documentacion del proyecto, con secciones de referencia estables y lenguaje tecnico claro.
- **Rationale:** `docs/` ya concentra manuales operativos y tecnicos del repositorio. Mantener el manual alli facilita descubribilidad, versionado y uso por parte del equipo sin mezclarlo con specs funcionales.
- **Alternatives considered:** dejar el contenido solo en OpenSpec o en comentarios del ticket Jira; se descarta porque la referencia debe permanecer accesible despues del cierre del cambio.

## Risks / Trade-offs

- **[Risk]** El manual puede quedar demasiado teorico y no servir como guia de implementacion real.  
  **Mitigation:** incluir ejemplos practicos, flujos paso a paso, criterios de decision y un prompt final reutilizable.
- **[Risk]** Documentar dos metodos puede generar dudas si no se explican con suficiente contraste.  
  **Mitigation:** agregar tabla comparativa, casos de uso y matriz de seleccion arquitectonica.
- **[Risk]** El documento puede desalinearse de la arquitectura real si no referencia el patron actual de layouts con `Outlet`.  
  **Mitigation:** redactar el manual anclado al stack actual del repo: React 19, React Router v7, SPA modular y layouts persistentes.
- **[Risk]** La existencia de un manual no garantiza adopcion consistente por futuros tickets.  
  **Mitigation:** cerrar el documento con un prompt estandar para Jira/Codex y dejar criterios de aceptacion tecnicos reutilizables.

## Migration Plan

1. Crear el manual tecnico en `docs/` con la estructura acordada.
2. Validar que el contenido cubra Metodo A, Metodo B, diferencias, arquitectura, flujo, ejemplos, buenas practicas, riesgos y reutilizacion en otros modulos.
3. Incluir el prompt final para Jira/Codex/IA en el mismo documento.
4. Referenciar el manual en futuros tickets o cambios que involucren navegacion interna con preservacion de estado.

Rollback:
- Si el documento no cumple el alcance, basta con ajustar o revertir el archivo Markdown; no hay impacto runtime ni migraciones de codigo.

## Open Questions

- Confirmar el nombre final del archivo y su ubicacion exacta dentro de `docs/`.
- Confirmar si el manual debe incluir referencias explicitas a archivos actuales del modulo `radicacion` como ejemplo de contexto, o mantenerse totalmente generico.
- Validar si se desea enlazar este manual desde otro documento existente de arquitectura o desde la ayuda de comandos OpenSpec/Jira.
