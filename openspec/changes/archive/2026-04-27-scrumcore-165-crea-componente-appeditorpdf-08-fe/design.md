## Context

El ticket `SCRUMCORE-165` extiende el componente reusable `AppEditorPdf` con la capacidad "08-FE - contador de pagina y contexto". El change anterior (`SCRUMCORE-164`) formalizo contrato base, guias visuales y publicacion de metricas. Falta consolidar la experiencia de contexto de lectura mostrando `Pagina X de Y` de forma estable, incluso cuando el cursor no esta activo.

Estado actual relevante:
- El editor reutilizable opera sobre un modelo continuo con paginacion visual.
- Existen guias y metricas de pagina, pero el contador de contexto no esta formalizado como parte del contrato reusable FE-08.
- Hay lineamientos previos de arquitectura para calcular pagina por prioridad de fuente: cursor y fallback por scroll.

Restricciones:
- Mantener React 19 + TypeScript estricto + ESM.
- No introducir dependencias nuevas para resolver el contador.
- Preservar API reusable de `src/app/Components/UI/AppEditorPdf/` y compatibilidad con consumidores existentes.

Stakeholders: equipo FE que consume `AppEditorPdf` en modulos de gestion documental, QA para regresion de paginacion, arquitectura para consistencia del contrato reusable.

## Goals / Non-Goals

**Goals:**
- Definir diseno tecnico para calcular y renderizar el contador `Pagina X de Y` en `AppEditorPdf`.
- Garantizar estabilidad del contexto de pagina con prioridad de cursor y fallback por scroll.
- Mantener costo de render bajo en interacciones frecuentes (scroll, seleccion, zoom).
- Habilitar cobertura automatizada de comportamiento del contador sin dependencia de red.

**Non-Goals:**
- Reescribir motor de paginacion visual del editor.
- Implementar persistencia cross-session del contexto de pagina.
- Incorporar controles nuevos de navegacion (botones next/prev) como parte de FE-08.
- Cambiar arquitectura de carpetas o framework de UI/testing.

## Decisions

1. El contador se implementa como capa de presentacion en `AppEditorPdf` y no como concern del modulo consumidor.
Rationale: el contexto `Pagina X de Y` pertenece al contrato reusable; evita duplicacion y divergencia entre modulos.
Alternativa considerada: que cada consumidor pinte su propio contador con metricas expuestas.
Por que no: multiplica logica y aumenta riesgo de inconsistencias de UX.

2. Resolver pagina activa con estrategia de prioridad: cursor enfocado primero, scroll como fallback.
Rationale: cuando hay foco de edicion, `selection` representa mejor el contexto real del usuario; en lectura o sin foco, scroll es la senal disponible.
Alternativa considerada: usar solo scroll para todos los casos.
Por que no: en documentos largos el scroll puede no reflejar con precision el punto de edicion activo.

3. Encapsular calculo de contexto en un hook interno (por ejemplo `usePageContext`) y exponer al componente solo estado derivado.
Rationale: separa infraestructura (coordenadas DOM/editor) de presentacion (overlay contador), facilita pruebas y mantenimiento.
Alternativa considerada: calcular en-line dentro del componente principal.
Por que no: mezcla responsabilidades y dificulta testear reglas de prioridad/debounce.

4. Limitar actualizaciones de estado con debounce de scroll y guards de igualdad para evitar rerenders innecesarios.
Rationale: scroll y cambios de seleccion son eventos de alta frecuencia; se requiere estabilidad visual sin degradar rendimiento.
Alternativa considerada: actualizar estado en cada evento sin control.
Por que no: incremento de render churn y riesgo de jitter del contador.

5. Mantener contrato de salida opcional para consumidores avanzados (ej. callback de contexto) sin volverlo obligatorio.
Rationale: conserva simplicidad para consumo basico y habilita instrumentacion en modulos que lo necesiten.
Alternativa considerada: forzar callback obligatorio.
Por que no: rompe adopcion incremental y agrega friccion a usos simples.

## Risks / Trade-offs

- [Riesgo] Desfase entre pagina reportada por cursor y por scroll en transiciones rapidas -> Mitigacion: definir regla deterministica de prioridad y pruebas con eventos consecutivos.
- [Riesgo] Dependencia de medicion DOM sensible a cambios de layout/zoom -> Mitigacion: recalculo controlado en cambios relevantes (zoom, resize, source) y tolerancias en tests.
- [Riesgo] Overlay del contador puede interferir con interaccion en viewports pequenos -> Mitigacion: estilo discreto, no bloqueante (pointer-events), y validacion responsive.
- [Riesgo] Regresion en guias/metricas de FE-07 por compartir fuentes de estado -> Mitigacion: suite focal de regresion para guias, metricas y contador en conjunto.

## Migration Plan

1. Definir contrato FE-08 en specs de la capability `app-appeditorpdf-08-fe` (escenarios de contador y contexto).
2. Implementar logica de contexto de pagina en capa reusable de `AppEditorPdf` (hook + presentacion de contador).
3. Agregar/actualizar pruebas de componente para:
   - calculo por cursor;
   - fallback por scroll;
   - estabilidad ante eventos rapidos;
   - no regresion de guias/metricas existentes.
4. Integrar en consumidor piloto y validar comportamiento visual en modulo real.
5. Ejecutar validaciones (`npm.cmd run test -- --run` y `npm.cmd run spec:validate`) antes de merge.

Rollback:
- Revertir integracion del contador en `AppEditorPdf` manteniendo contrato previo de FE-07.
- Mantener rama feature aislada hasta recuperar estabilidad de calculo/rendimiento.

## Open Questions

- El contador debe mostrarse siempre o solo cuando `paginationMode="visual"` este activo?
- Se requiere callback dedicado `onPageContextChange` o basta extender payload de metricas existente?
- Cual es el debounce objetivo para scroll segun UX (ej. 50ms, 100ms)?
- En modo lectura sin seleccion, se prioriza centro del viewport o borde superior para determinar pagina activa?
