## Context

El ticket `SCRUMCORE-159` corresponde a `02-FE` del roadmap de `AppEditorPdf` y
se centra en UI/UX responsive del componente ya creado en `SCRUMCORE-158`.

La propuesta auto-generada trae naming tecnico inconsistente
(`AppAppeditorpdf02Fe`) y una ruta incorrecta; este diseno fija los criterios
canonicos:
- nombre oficial: `AppEditorPdf`
- ubicacion oficial: `src/app/Components/UI/AppEditorPdf/`

Estado actual:
- Ya existe un baseline funcional del componente shared en la ruta objetivo.
- Se requiere evolucion visual responsiva y de theming sin cambiar el contrato
  funcional principal.
- Debe mantenerse separacion entre shared UI y modulos consumidores.

Restricciones:
- No introducir features nuevas del editor en este ticket.
- No mover logica de negocio a `AppEditorPdf`.
- Mantener compatibilidad con React 19 y TypeScript estricto.

## Goals / Non-Goals

**Goals:**
- Definir lineamientos UI/UX responsive para `AppEditorPdf` en desktop, tablet y
  mobile.
- Unificar reglas de theming para evitar contradicciones entre tickets.
- Asegurar estabilidad visual: foco, seleccion, scroll continuo y ausencia de
  flicker.
- Mantener la API publica existente del componente sin breaking changes.

**Non-Goals:**
- Cambiar motor de edicion o arquitectura de persistencia.
- Implementar paginacion avanzada, zoom o features funcionales adicionales.
- Acoplar decisiones visuales del shared a un modulo especifico.

## Decisions

### 1) Reutilizar baseline existente y evolucionar por capas
**Decision:** Mantener `AppEditorPdf` como wrapper/shared existente y aplicar la
evolucion UI en presentation + estilos, preservando contratos de dominio.

**Rationale:** Minimiza riesgo de regresion funcional y acelera entrega.

**Alternatives considered:**
- Reescribir estructura completa del componente: descartado por costo y riesgo.
- Implementar ajustes visuales en modulo consumidor: descartado por romper shared.

### 2) Responsive por layout progresivo (no por branching de componentes)
**Decision:** Definir responsive con una sola superficie de editor y toolbar
adaptativa por breakpoints, evitando forks de UI por dispositivo.

**Rationale:** Reduce duplicidad de comportamiento y facilita pruebas.

**Alternatives considered:**
- Mantener versiones separadas desktop/mobile: descartado por deuda de mantenimiento.

### 3) Theming centralizado y consistente
**Decision:** `AppEditorPdf` respetara el theme global del sistema y evitara
reglas locales contradictorias (sin toggles ad-hoc fuera del contrato definido).

**Rationale:** Alinea experiencia visual en toda la app.

**Alternatives considered:**
- Tema aislado del editor: descartado por inconsistencia visual.

### 4) UX baseline no negociable
**Decision:** Mantener como criterio de aceptacion de UI:
- sin flicker perceptible,
- sin salto de cursor,
- sin perdida de seleccion,
- sin doble scroll.

**Rationale:** Asegura que mejoras visuales no degraden experiencia de edicion.

## Risks / Trade-offs

- [Riesgo] Ajustes CSS responsive rompen layout en workbenches existentes
  -> Mitigacion: matriz de validacion por breakpoints y snapshots visuales.

- [Riesgo] Cambios de toolbar afectan discoverability de acciones
  -> Mitigacion: priorizacion de acciones criticas y pruebas de navegacion.

- [Riesgo] Reglas de tema conflictuan con estilos heredados de AppEditor
  -> Mitigacion: normalizar tokens y evitar overrides dispersos.

- [Trade-off] Preservar API actual limita simplificaciones agresivas de UI
  -> Aceptado para evitar ruptura en consumidores.

## Migration Plan

1. Corregir naming/ruta en artefactos del cambio a `AppEditorPdf`.
2. Definir spec de comportamiento visual responsive y theming consistente.
3. Implementar ajustes UI en componente shared manteniendo contrato vigente.
4. Ejecutar pruebas unitarias e integracion visual focalizada.
5. Validar en modulo consumidor principal (gestionCorrespondencia) sin acoplar
   logica de dominio al shared.

Rollback:
- Revertir cambios visuales a baseline del componente compartido en caso de
  regresion severa de UX o layout.

## Open Questions

- Cual es el breakpoint oficial del design system para colapsar acciones de la
  toolbar sin perder discoverability?
- Se prioriza consistencia exacta con `AppEditor` existente o apariencia renovada
  siempre que se mantenga el contrato?
- Que escenarios visuales se consideran obligatorios para sign-off QA en mobile?
