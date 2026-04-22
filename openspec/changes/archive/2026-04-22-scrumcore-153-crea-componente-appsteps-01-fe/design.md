## Context

El cambio `SCRUMCORE-153` crea un componente reusable de pasos para flujos de formulario en la capa compartida de UI. El repositorio ya centraliza componentes reutilizables en `src/app/Components/UI/*` con patrones consistentes:

- carpeta por componente (`AppTabs`, `AppInput`, etc.)
- export local (`index.ts`) y export agregado en `src/app/Components/UI/index.ts`
- estilos desacoplados con CSS Modules
- pruebas unitarias y de comportamiento con Vitest + Testing Library

Actualmente no existe un componente estándar `AppSteps` en la librería UI, por lo que cada módulo consumidor tendría que resolver este patrón por su cuenta, generando inconsistencia de interacción y de accesibilidad.

## Goals / Non-Goals

**Goals:**
- Crear `AppAppsteps01Fe` como componente reusable bajo `src/app/Components/UI/AppAppsteps01Fe/`.
- Definir una API tipada para representar pasos, estado activo y estado de completitud.
- Soportar una variante base para formularios multi-paso con semántica y foco accesible.
- Publicar el componente en el barrel de UI para adopción inmediata en módulos consumidores.
- Cubrir contrato y comportamiento con pruebas etiquetadas por SPEC.

**Non-Goals:**
- No rediseñar la navegación global ni el router.
- No introducir dependencia nueva fuera de las librerías ya instaladas.
- No migrar todos los flujos existentes en este ticket; la adopción inicial será controlada por un consumidor puntual.
- No cubrir analytics/telemetría avanzada en esta iteración.

## Decisions

1. Estructura de componente alineada al estándar UI del repo
Se implementará en `src/app/Components/UI/AppAppsteps01Fe/` con:
- `AppAppsteps01Fe.tsx`
- `AppAppsteps01Fe.module.css`
- `AppAppsteps01Fe.test.tsx`
- `index.ts`

Rationale:
- minimiza fricción de mantenimiento al seguir el mismo patrón de `AppTabs`/`AppInput`.
- facilita descubribilidad y export central.

Alternativas consideradas:
- ubicar el componente en un módulo de negocio.
Motivo de descarte: reduce reusabilidad y rompe la estrategia de librería UI compartida.

2. API controlada y tipada
El componente será mayormente controlado por props (pasos y paso activo), con callbacks explícitos para cambios de paso. La forma base de datos de pasos será un arreglo tipado con id/label/estado.

Rationale:
- permite a cada formulario gestionar validaciones y navegación sin acoplar lógica interna.
- evita estado oculto difícil de testear.

Alternativas consideradas:
- componente no controlado con estado interno dominante.
Motivo de descarte: complica sincronización con reglas de negocio y validaciones del formulario anfitrión.

3. Base visual con estilos propios y sin dependencia adicional
Se usarán CSS Modules para tokenizar estados visuales (activo, completado, pendiente, deshabilitado), manteniendo consistencia con los componentes UI actuales.

Rationale:
- el repositorio ya usa CSS Modules como patrón principal en la capa UI.
- evita añadir costo de dependencia.

Alternativas consideradas:
- encapsular directamente un componente externo de steps.
Motivo de descarte: mayor acoplamiento a API externa y menor control de estilo/semántica.

4. Accesibilidad por contrato mínimo obligatorio
El componente incluirá atributos y comportamiento de accesibilidad desde la versión inicial:
- semántica de lista de pasos
- estado activo visible por teclado y lector de pantalla
- navegación por teclado para cambio de paso cuando aplique

Rationale:
- reduce deuda técnica temprana en un componente de uso transversal.
- mantiene coherencia con objetivos de UX en formularios multi-paso.

Alternativas consideradas:
- postergar a11y para una iteración posterior.
Motivo de descarte: incrementa costo de corrección y riesgo de regresión al escalar adopción.

5. Integración incremental en un consumidor real
Se conectará inicialmente en un módulo concreto para validar API y ergonomía, sin forzar migración masiva.

Rationale:
- permite feedback rápido sin ampliar alcance del ticket.
- reduce riesgo en una primera entrega de componente base.

Alternativas consideradas:
- migrar múltiples módulos en el mismo cambio.
Motivo de descarte: eleva riesgo de regresiones y complica revisión.

6. Estrategia de pruebas orientada a comportamiento
Las pruebas cubrirán:
- render de pasos y estado activo
- interacción de cambio de paso
- estados deshabilitados/completados
- señales de accesibilidad visibles en DOM

Se incluirán tags `[SPEC:<ID>]` en los tests vinculados.

Rationale:
- alinea con política del repo (Vitest + Testing Library, foco en comportamiento observable).
- fortalece trazabilidad OpenSpec <-> tests.

## Risks / Trade-offs

- [API demasiado rígida para casos futuros] -> Mitigación: mantener contrato mínimo y puntos de extensión por props opcionales.
- [Divergencia visual entre módulos si se sobreescribe estilo localmente] -> Mitigación: documentar variantes permitidas y evitar overrides ad hoc.
- [Integración inicial detecta casos no contemplados] -> Mitigación: validar en consumidor real antes de promover adopción masiva.
- [Cobertura insuficiente de teclado/a11y en la primera pasada] -> Mitigación: incluir casos de navegación y estado semántico en tests del componente.

## Migration Plan

1. Crear el componente y exportarlo en `src/app/Components/UI/index.ts`.
2. Implementar pruebas unitarias/behavior del componente.
3. Integrar en un consumidor piloto.
4. Ejecutar `npm.cmd run test -- --run` y `npm.cmd run spec:validate`.
5. Ajustar contrato del componente según retroalimentación del piloto.

Rollback:
- revertir uso en consumidor piloto y mantener el componente fuera de integración activa si aparece regresión crítica.

## Open Questions

- ¿El contrato inicial necesita soporte de orientación vertical/horizontal desde la primera versión?
- ¿El cambio de paso debe permitir validación asíncrona previa (guard de navegación) en esta iteración o en una siguiente?
- ¿Qué módulo será el consumidor piloto definitivo para validar UX y accesibilidad?
