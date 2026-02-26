## Context

El formulario de Radicacion tiene campos dinamicos y componentes `Select` con autocompletado remoto. En el estado actual, algunos eventos de cambio pueden dispararse durante el montaje inicial o por sincronizacion interna del componente, provocando consumo de API sin una accion explicita del usuario.

Para SCRUM-21, el objetivo es impedir llamadas de red en primer render causadas por eventos `change` y garantizar que las consultas dependan solo de interaccion real (escritura/seleccion) y de condiciones de habilitacion claras.

Restricciones:
- Mantener arquitectura actual (React + hooks + React Query).
- No romper comportamiento de autocompletado ni restricciones existentes.
- Evitar regresiones en `RadicacionForm` y hooks de Radicacion.

## Goals / Non-Goals

**Goals:**
- Evitar ejecucion de llamadas API durante primer render por eventos `change` no iniciados por usuario.
- Asegurar que los hooks de consulta usen condiciones de habilitacion (`enabled`) basadas en estado derivado de entrada real.
- Estandarizar guardas de entrada para que `null`, vacio o valores iniciales no detonen consultas.
- Dejar cobertura de pruebas para evitar regresiones.

**Non-Goals:**
- Redisenar `RadicacionForm` o reemplazar componentes UI.
- Cambiar contratos backend.
- Reestructurar globalmente todos los hooks del modulo fuera del alcance del ticket.

## Decisions

1. Gating explicito de consultas por interaccion de usuario.
- Decision: mantener/ajustar `shouldQuery` y `enabled` para que dependan de texto digitado o seleccion valida, no de estado inicial.
- Rationale: evita llamadas en primer render y mantiene consultas bajo control del usuario.
- Alternativa descartada: cancelar respuestas en backend; corrige sintoma pero no la causa en frontend.

2. Normalizacion defensiva de valores de entrada antes de llamar hooks.
- Decision: aplicar normalizacion (`trim`, null-safe) y no consultar cuando el valor es vacio o invalido.
- Rationale: reduce ejecuciones por cambios transitorios del componente.
- Alternativa descartada: confiar en valores por defecto del `Select`/Form.

3. Mantener componente en modo estandar sin autoseleccion automatica.
- Decision: no seleccionar automaticamente el primer item de resultados.
- Rationale: la seleccion automatica introduce cambios de estado encadenados y puede reactivar eventos no deseados.
- Alternativa descartada: autoseleccion con heuristicas; aumenta complejidad y riesgo de loops.

4. Cobertura de pruebas enfocada en no-regresion de triggers.
- Decision: pruebas de formulario/hook validando que el primer render no dispara llamadas no esperadas y que la consulta solo ocurre tras accion del usuario.
- Rationale: asegura estabilidad del comportamiento a futuro.
- Alternativa descartada: validacion solo manual.

## Risks / Trade-offs

- [Riesgo] Guardas demasiado estrictas pueden impedir consultas validas. -> Mitigacion: cubrir escenarios de escritura real y seleccion esperada en tests.
- [Riesgo] Diferencias de comportamiento de `Select` entre versiones de AntD. -> Mitigacion: pruebas por comportamiento observable (llamadas a hook y estado final).
- [Riesgo] Dependencias cruzadas entre campos dinamicos. -> Mitigacion: cambios puntuales y pruebas de no-regresion en `RadicacionForm`.

## Migration Plan

1. Identificar puntos donde `change` inicial puede disparar consultas (componente + hooks involucrados).
2. Aplicar guardas de entrada y habilitacion de consultas por interaccion real.
3. Ajustar pruebas existentes y agregar cobertura de no-disparo en primer render.
4. Ejecutar suite de Radicacion y validar manualmente en UI.
5. Si hay regresion, rollback parcial al comportamiento anterior del componente afectado manteniendo pruebas.

## Open Questions

- Confirmar si el criterio de "interaccion real" para este ticket incluye solo digitacion o tambien seleccion de opcion precargada.
- Confirmar si hay otros campos (ademas de `Destinatario_Cor`) con el mismo patron para incluirlos en un siguiente ticket.
