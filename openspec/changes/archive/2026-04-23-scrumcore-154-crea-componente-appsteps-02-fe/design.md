## Context

El ticket `SCRUMCORE-154` extiende el componente reusable `AppSteps` para cubrir capacidades visuales y de interacción avanzadas: variante de progreso global, variante timeline, comportamiento responsive y criterios de accesibilidad.

El repositorio ya dispone de:
- React 19 + TypeScript estricto
- Ant Design como librería base de componentes
- patrón de componentes compartidos en `src/app/Components/UI/*`
- cobertura de pruebas con Vitest + Testing Library

En el ticket anterior (`SCRUMCORE-153`) se implementó la base de `AppSteps` con `variant="default"` y `variant="form"`. Este cambio debe construir sobre esa base sin romper su API pública, manteniendo componente único por `variant`.

## Goals / Non-Goals

**Goals:**
- Implementar `variant="progress"` para mostrar progreso global desacoplado del cálculo de negocio.
- Implementar `variant="timeline"` con metadatos temporales (`timestamp`) y layout vertical.
- Definir comportamiento responsive consistente (horizontal/vertical según variante y viewport).
- Reforzar accesibilidad mínima obligatoria: foco visible, semántica del paso actual y navegación usable por teclado.
- Mantener contrato tipado y reutilizable en la capa `src/app/Components/UI/AppSteps/`.

**Non-Goals:**
- No calcular porcentaje de progreso internamente; solo consumir `progressPercent`.
- No introducir lógica de dominio ni persistencia de hitos temporales.
- No implementar i18n/formateo regional avanzado de `timestamp` dentro del componente.
- No integrar en módulo consumidor en este ticket; la adopción queda para ticket de integración.

## Decisions

1. Componente único `AppSteps` con render condicional por `variant`
Se mantiene una sola entrada `AppSteps` y se activan bloques visuales por `variant`.

Rationale:
- preserva consistencia de API y reduce fragmentación (`AppStepsProgress`, `AppStepsTimeline`, etc.).
- facilita mantenimiento y pruebas centralizadas.

Alternativa descartada:
- crear componentes separados por variante. Incrementa duplicación y rompe el principio de arquitectura definido para AppSteps.

2. `progress` como bloque complementario sobre `Steps`
`variant="progress"` renderiza un bloque `Progress` (Ant Design) cuando existe `progressPercent`, además del flujo de pasos.

Rationale:
- separa visualización global de progreso del control de navegación.
- evita acoplar la fuente de porcentaje a reglas internas del componente.

Alternativa descartada:
- inferir porcentaje desde steps completados. No cubre casos de negocio con ponderaciones o estados externos.

3. `timeline` fuerza orientación vertical y expone `timestamp`
`variant="timeline"` impone orientación vertical y mapea metadatos de tiempo en descripción/metadata del item.

Rationale:
- alinea UX de historial temporal con lectura secuencial vertical.
- evita combinaciones inválidas de layout para timeline.

Alternativa descartada:
- permitir timeline horizontal configurable. Incrementa complejidad de diseño sin beneficio claro para el caso principal.

4. Responsive con reglas explícitas y fallback seguro
Se prioriza horizontal en `default/form/progress` para desktop y fallback a vertical cuando el ancho sea insuficiente; `timeline` permanece vertical.

Rationale:
- mantiene legibilidad en mobile y evita colisiones de títulos largos.
- reutiliza capacidad responsive nativa y reglas CSS del componente.

Alternativa descartada:
- responsive totalmente manual por breakpoint custom en JS. Mayor complejidad con menor valor inmediato.

5. Accesibilidad como contrato mínimo transversal
Se mantiene y refuerza:
- `aria-current="step"` en el paso activo
- foco visible en interacción
- estados de error/proceso no dependientes solo de color

Rationale:
- evita deuda técnica en un componente reutilizable transversal.
- facilita cumplimiento en módulos consumidores sin reimplementación.

Alternativa descartada:
- tratar a11y como post-proceso. Aumenta riesgo de regresiones y retrabajo.

6. Pruebas de comportamiento por variante
Se amplía matriz de tests con cobertura específica para `progress` y `timeline`, más escenarios responsive/a11y observables.

Rationale:
- preserva trazabilidad OpenSpec ↔ tests.
- reduce riesgo de regresión sobre variantes ya existentes.

Alternativa descartada:
- validación solo manual de variantes. Insuficiente para componente de librería compartida.

## Risks / Trade-offs

- [Inconsistencia entre barra de progreso y estado de steps] -> Mitigación: documentar que `progressPercent` es fuente externa y validar rango/representación visual.
- [Layouts rotos en pantallas pequeñas con labels extensos] -> Mitigación: fallback vertical, control de wrapping y pruebas en viewport reducido.
- [Complejidad creciente del componente único por múltiples variantes] -> Mitigación: mantener helpers reutilizables y separar responsabilidades de mapeo/render.
- [Brechas de accesibilidad en estados visuales nuevos] -> Mitigación: checklist a11y en tests y revisión de semántica/foco por variante.

## Migration Plan

1. Extender `AppSteps` para soportar `progress` y `timeline` manteniendo compatibilidad con variantes existentes.
2. Ajustar tipos/normalización de items para soportar `timestamp` y metadatos de timeline.
3. Implementar reglas responsive y estilos específicos por variante.
4. Añadir/actualizar pruebas de comportamiento para nuevas variantes y a11y básica.
5. Ejecutar pruebas focales del componente y validación OpenSpec (`spec:validate`).

Rollback:
- revertir cambios de variantes nuevas manteniendo la implementación base (`default/form`) si aparece regresión crítica.

## Open Questions

- ¿Se requiere formateo de `timestamp` por locale dentro del componente o se mantiene responsabilidad del consumidor?
- ¿El fallback responsive a vertical debe activarse por ancho de contenedor además de viewport?
- ¿Qué nivel de personalización visual por variante será permitido públicamente en esta iteración (tokens/clases/props)?
