## Context

El ticket `SCRUMCORE-164` define la necesidad de crear el componente reusable `AppEditorPdf` para cubrir la parte "07-FE - guias y metricas visuales" en la capa UI compartida. Actualmente existe `src/app/Components/UI/AppEditorPdf/README.md` e `index.ts`, pero no hay un contrato de comportamiento consolidado para:
- mostrar guias visuales de pagina;
- exponer metricas visuales utiles para pruebas y consumo por modulos;
- mantener un API estable de componente reusable.

Restricciones relevantes:
- mantener React 19 + TypeScript estricto + ESM;
- no introducir nuevas dependencias pesadas;
- preservar el patron reusable de `src/app/Components/UI/*`.

Stakeholders: equipo FE que consume el componente en modulos de workflow/gestion, QA (pruebas visuales/funcionales), y arquitectura (coherencia de contratos UI).

## Goals / Non-Goals

**Goals:**
- Definir el diseno tecnico para un `AppEditorPdf` reusable con soporte de guias visuales y metricas de pagina.
- Estandarizar props de entrada/salida para facilitar consumo en modulos existentes.
- Permitir pruebas unitarias de comportamiento sin depender de servicios remotos.
- Minimizar impacto en consumidores actuales y habilitar adopcion incremental.

**Non-Goals:**
- Reescribir la arquitectura del editor o del flujo completo de gestion de correspondencia.
- Implementar en este paso exportacion avanzada, anotaciones persistentes o colaboracion en tiempo real.
- Cambiar librerias base del proyecto (React Router, Axios, Vitest, Vite).

## Decisions

1. Mantener `AppEditorPdf` en `src/app/Components/UI/AppEditorPdf/` con API tipada via props.
Rationale: conserva la convencion actual de componentes reusables y reduce costo de adopcion.
Alternativa considerada: moverlo a `src/shared`.
Por que no: rompe la taxonomia actual de UI del proyecto y no aporta valor inmediato.

2. Modelar "guias y metricas visuales" como datos derivados del estado del visor, no como estado externo obligatorio.
Rationale: reduce coupling con modulos consumidores; el componente sigue siendo autocontenido.
Alternativa considerada: obligar a que el modulo consumidor calcule todas las metricas.
Por que no: duplica logica entre modulos y aumenta riesgo de inconsistencias.

3. Exponer callbacks opcionales para metricas (`onMetricsChange`) y eventos de navegacion de pagina.
Rationale: soporta casos simples (sin callbacks) y avanzados (telemetria local / instrumentacion de tests).
Alternativa considerada: emitir eventos globales.
Por que no: agrega complejidad y dificulta trazabilidad en pruebas unitarias.

4. Pruebas con Vitest + Testing Library enfocadas en contrato visible: render, cambios de pagina, actualizacion de metricas.
Rationale: valida comportamiento observable sin acoplarse a detalles internos.
Alternativa considerada: pruebas e2e unicamente.
Por que no: mayor costo de ejecucion y menor feedback para iteracion de componente reusable.

## Risks / Trade-offs

- [Riesgo] Deriva de metricas incompleta en documentos con layout atipico -> Mitigacion: definir contrato minimo de metricas requeridas y casos borde en tests.
- [Riesgo] Incremento de rerenders por recalculo de metricas -> Mitigacion: memoizacion selectiva y actualizacion por cambios relevantes (pagina/zoom/documento).
- [Riesgo] API del componente demasiado amplia desde el primer release -> Mitigacion: iniciar con props esenciales y extender de forma backward-compatible.
- [Riesgo] Divergencia entre comportamiento esperado y consumidor real -> Mitigacion: integrar al menos un modulo consumidor piloto y validar con pruebas de integracion.

## Migration Plan

1. Definir contrato de props/eventos de `AppEditorPdf` y comportamiento base de guias/metricas.
2. Implementar/ajustar el componente en `src/app/Components/UI/AppEditorPdf/`.
3. Crear pruebas unitarias con cobertura del contrato reusable.
4. Integrar un consumidor piloto (flujo de gestion respuesta) sin romper rutas existentes.
5. Validar `npm run test -- --run` y, cuando aplique, smoke de navegacion del modulo consumidor.

Rollback:
- mantener integracion via feature branch;
- si hay regresion, retirar uso del componente en el consumidor piloto y conservar API anterior hasta corregir.

## Open Questions

- Que metricas visuales son obligatorias en la primera version (ej. pagina actual/total, zoom, alto util)?
- Se requiere persistencia de estado de visualizacion por documento entre navegaciones?
- El consumidor necesita solo lectura de metricas o tambien comandos externos de navegacion/zoom?
