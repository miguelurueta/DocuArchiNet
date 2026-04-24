## Context

El cambio `SCRUMCORE-158` nace para crear el componente reusable del editor PDF en la
capa UI compartida. La propuesta generada automaticamente contiene naming
inconsistente (`AppAppeditorpdf01Fe`), por lo que este diseno fija criterios
tecnicos estables antes de implementar.

Estado actual relevante:
- Existe necesidad de un editor reusable para integrarse luego en
  `gestionCorrespondencia` (tab Documentos), sin acoplar logica de dominio.
- El repositorio ya usa React 19, TypeScript estricto y arquitectura modular con
  componentes shared en `src/app/Components/UI/`.
- Los tickets arquitectonicos del equipo ya definieron que la ruta objetivo del
  componente es `src/app/Components/UI/AppEditorPdf/`.

Stakeholders:
- Equipo Frontend (implementacion y pruebas)
- QA (validacion de UX, foco, scroll, estabilidad)
- Equipo funcional de GestionRespuesta (consumidor del shared)

Restricciones:
- No romper arquitectura actual ni mover logica de negocio al shared.
- Mantener TypeScript estricto (sin `any`).
- Mantener compatibilidad con flujo OpenSpec (specs + tasks despues de design).

## Goals / Non-Goals

**Goals:**
- Definir la arquitectura tecnica del componente `AppEditorPdf` como pieza shared.
- Unificar naming y convenciones para evitar deriva entre artefactos.
- Delimitar contratos base y boundaries entre shared UI y modulo consumidor.
- Establecer una ruta de migracion segura desde placeholders/integraciones actuales.

**Non-Goals:**
- Implementar el componente en este artefacto.
- Definir detalle de cada control avanzado del editor (zoom, paginacion, etc.).
- Cerrar decisiones de backend/persistencia final en este ticket core.

## Decisions

### 1) Naming canonico y ruta objetivo
**Decision:** El nombre oficial del componente sera `AppEditorPdf` y su ubicacion
obligatoria sera `src/app/Components/UI/AppEditorPdf/`.

**Rationale:** Evita nombres derivados del resumen Jira que generan componentes
ilegibles o no reutilizables.

**Alternatives considered:**
- Mantener `AppAppeditorpdf01Fe`: descartado por baja claridad y deuda tecnica.
- Ubicarlo en `src/modules/...`: descartado porque rompe el principio de shared UI.

### 2) Boundary de responsabilidades
**Decision:** `AppEditorPdf` se implementa como componente shared agnostico del
dominio; `gestionCorrespondencia` solo integra por props/callbacks.

**Rationale:** Preserva Clean Architecture y reduce acoplamiento.

**Alternatives considered:**
- Inyectar reglas de negocio dentro del componente: descartado por reutilizacion
  limitada y alto riesgo de regresion cruzada.

### 3) Contrato minimo inicial y evolucion incremental
**Decision:** Definir un contrato base pequeno (contenido, estado de edicion,
callbacks principales, estados visuales) y evolucionarlo por tickets posteriores.

**Rationale:** Permite implementar un core estable sin bloquear iteraciones.

**Alternatives considered:**
- Contrato amplio desde inicio: descartado por complejidad y riesgo de cambios
  tempranos incompatibles.

### 4) Reglas de UX y performance desde baseline
**Decision:** Incluir desde el core reglas no negociables:
- sin flicker,
- sin salto de cursor,
- scroll continuo unico,
- evitar recalculos pesados por keypress.

**Rationale:** Son condiciones base para que tickets futuros no degraden la
experiencia de escritura.

**Alternatives considered:**
- Posponer estas reglas para hardening: descartado porque genera deuda temprana.

## Risks / Trade-offs

- [Riesgo] Divergencia entre naming en OpenSpec y naming en codigo
  -> Mitigacion: fijar naming canonico en specs y tasks antes de implementar.

- [Riesgo] Sobrecargar el ticket core con features avanzadas
  -> Mitigacion: limitar este change a baseline y delegar avanzados a tickets
     posteriores.

- [Riesgo] Acoplamiento accidental con `gestionCorrespondencia`
  -> Mitigacion: revisar contrato y ubicacion de archivos en PR checklist.

- [Trade-off] Contrato inicial pequeno puede requerir extensiones posteriores
  -> Aceptado para reducir riesgo y permitir entrega incremental.

## Migration Plan

1. Normalizar artefactos del cambio para que usen `AppEditorPdf` como naming
   canonico (specs/tasks posteriores).
2. Definir spec de capability del core reusable con contrato base.
3. Crear tasks de implementacion tecnica minima (estructura, contrato y pruebas).
4. Implementar en ruta shared `src/app/Components/UI/AppEditorPdf/`.
5. Integrar en modulo consumidor solo cuando el core pase validaciones base.

Rollback:
- Si la implementacion inicial rompe consumidores, mantener fallback al componente
  actual/placeholder y reactivar integracion por feature flag o branch aislada.

## Open Questions

- Cual sera el shape exacto del valor del editor (HTML, JSON, ambos) para el
  contrato inicial del core?
- Se requiere exponer API imperativa (ref) desde el ticket core o se difiere a
  tickets avanzados?
- Que subset minimo de toolbar se considera obligatorio para cerrar el core?
