## Context

`SCRUMCORE-155` cierra la adopcion de `AppSteps` en fase 03 FE: pruebas obligatorias por variante e integracion en un modulo consumidor real.

El componente ya existe en `src/app/Components/UI/AppSteps/` con soporte para `default`, `form`, `progress` y `timeline`. Actualmente no hay consumo real fuera del propio paquete UI, por lo que falta validar la API en contexto de negocio y asegurar que el modulo consumidor no replique logica de steps.

El ticket requiere:
- matriz de pruebas minima estable por variante;
- integracion real en un modulo existente;
- evidencia de comportamiento sin duplicacion de logica de navegacion.

## Goals / Non-Goals

**Goals:**
- Consolidar una suite de pruebas estable para `AppSteps` cubriendo base, form, progress, timeline, controlado y no controlado.
- Integrar `AppSteps` en un consumidor real de `gestionCorrespondencia` para validar API y ergonomia.
- Mantener la regla arquitectonica: validacion de negocio fuera de `AppSteps`; el componente solo orquesta UI de pasos.
- Alinear README/documentacion del componente con el caso real de integracion.

**Non-Goals:**
- No redisenar la API publica de `AppSteps` ni reemplazar Ant Design `Steps`.
- No mover logica de dominio del consumidor hacia `AppSteps`.
- No introducir una capa global de estado para steps.
- No cubrir integracion en multiples modulos en este ticket; se valida un consumidor objetivo.

## Decisions

1. Consumidor objetivo: `GestionRespuestaMainTabContent`

Se integra `AppSteps` en `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx` como flujo visible del workbench.

Rationale:
- Es un flujo real y activo del sistema.
- Tiene puntos de avance claros (edicion, adjuntos, envio) y permite validar variantes sin crear un modulo nuevo.
- Minimiza riesgo de regresion global frente a integraciones en layout/rutas base.

Alternativa descartada:
- Integrar primero en un modulo demo aislado. No valida trazabilidad de negocio real.

2. Orquestacion local en consumidor, componente reusable sin logica de negocio

El consumidor define `items`, `current`, `onChange` y `validateStep` (si aplica), mientras `AppSteps` mantiene solo render, navegacion y guardas genericas.

Rationale:
- Cumple la regla del ticket: no duplicar ni reimplementar motor de steps fuera del componente reusable.
- Mantiene separacion de responsabilidades y evita acoplar `AppSteps` con reglas de `gestionCorrespondencia`.

Alternativa descartada:
- Mover reglas de avance del flujo a internals de `AppSteps`. Rompe reutilizacion y aumenta acoplamiento.

3. Validacion de pruebas por capas: unitarias del componente + prueba de integracion del consumidor

Se mantiene `AppSteps.test.tsx` como suite base de variantes y se agrega cobertura de integracion en el modulo consumidor para validar wiring real.

Rationale:
- Evita falsos positivos de pruebas solo unitarias.
- Confirma que la API se usa correctamente en contexto de negocio.

Alternativa descartada:
- Solo pruebas de modulo consumidor. Reduce visibilidad de regresiones en contrato reusable.

4. Matriz de pruebas alineada al ticket y nomenclatura estable

Se implementa/ajusta la matriz minima sugerida por Jira (`renderiza-items-base`, `form-validateStep-async-soportado`, `timeline-fuerza-vertical`, etc.) manteniendo etiquetas `[SPEC:...]` en pruebas OpenSpec.

Rationale:
- Facilita auditoria Jira -> OpenSpec -> test.
- Reduce ambiguedad sobre cobertura obligatoria de cierre.

Alternativa descartada:
- Cobertura parcial basada en smoke tests. No cumple definicion de terminado del ticket.

5. Integracion visual con Design System existente, sin nuevos tokens globales

La adopcion en consumidor reutiliza estilos/modificadores locales del modulo sin introducir un tema nuevo para steps.

Rationale:
- Mantiene bajo el costo de integracion.
- Evita impacto transversal en UI shared fuera del alcance del ticket.

Alternativa descartada:
- Crear una capa de theming global para `AppSteps` en esta fase. Exceso de alcance para cierre de pruebas e integracion.

## Risks / Trade-offs

- [Integracion visual inconsistente con el layout actual del modulo] -> Mitigacion: usar contenedor local y validar desktop/mobile en pruebas de integracion.
- [Duplicacion accidental de logica de navegacion en consumidor] -> Mitigacion: centralizar control en props de `AppSteps` y remover handlers redundantes.
- [Flakiness en pruebas async de `validateStep`] -> Mitigacion: usar `waitFor`/promesas controladas y evitar dependencias de timing real.
- [Cobertura de pruebas amplia incrementa tiempo de suite] -> Mitigacion: mantener tests unitarios deterministicos y una sola prueba de integracion enfocada.
- [Nombres generados automaticamente desde Jira (ej. `AppAppsteps03Fe`) generen deriva de contrato] -> Mitigacion: preservar nombre canonical `AppSteps` en codigo, docs y specs.

## Migration Plan

1. Revisar `AppSteps.test.tsx` y completar huecos contra la matriz minima de Jira.
2. Incorporar `AppSteps` en `GestionRespuestaMainTabContent` con contrato controlado/no controlado segun el flujo.
3. Agregar/actualizar test de integracion del consumidor para validar render, cambio de paso y reglas de bloqueo.
4. Actualizar README de `AppSteps` con ejemplo real del modulo integrado.
5. Ejecutar `npx.cmd vitest --run src/app/Components/UI/AppSteps/AppSteps.test.tsx` y pruebas del modulo integrado.
6. Ejecutar `npm.cmd run spec:validate` y registrar evidencia en el change.

Rollback:
- revertir integracion en consumidor y mantener `AppSteps` en modo reusable standalone si se detecta regresion funcional.

## Open Questions

- Que variante debe usar la integracion primaria del modulo (`form` para guardas o `default` para navegacion simple)?
- Se requiere persistir paso actual por ruta/query, o basta estado local del componente en esta fase?
- El ticket exige evidencia multimedia en PR; se documentara dentro del change o solo en descripcion del PR?
