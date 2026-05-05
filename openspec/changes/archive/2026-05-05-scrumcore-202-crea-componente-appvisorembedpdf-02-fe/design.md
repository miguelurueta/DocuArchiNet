## Context

SCRUMCORE-202 (02-FE) busca integrar visualmente el visor `AppVisorEmbedPdf` dentro de `DocumentosWorkbench` sin filtrar lógica de EmbedPDF hacia el módulo consumidor.

En SCRUMCORE-201 se creó `AppVisorEmbedPdf` como componente reusable (01-FE) y se definió la regla: `DocumentosWorkbench` no debe conocer detalles del engine/plugins. Esta iteración (02-FE) debe enfocarse en wiring/UI: layout, selección de documento, placeholder/empty state y una integración visual coherente.

## Goals / Non-Goals

**Goals:**
- Integrar `AppVisorEmbedPdf` en `DocumentosWorkbench` de forma visual (layout maestro/detalle) sin importar `@embedpdf/*` en el módulo.
- Asegurar que la pantalla mantiene scroll/virtualización dentro del visor sin romper el layout.
- Proveer un flujo mínimo: al seleccionar un documento, el visor renderiza el PDF correspondiente vía `fileUrl`.
- Agregar/ajustar tests de `DocumentosWorkbench` que verifiquen la integración sin depender del engine real (mock del componente).

**Non-Goals:**
- Implementar nuevas capacidades internas del visor (zoom/toolbar/search/etc.).
- Cambiar contratos del backend o lógica de carga de documentos fuera del alcance del workbench.

## Decisions

### 1) DocumentosWorkbench consume solo `AppVisorEmbedPdf`
**Decision:** `DocumentosWorkbench` solo usa `<AppVisorEmbedPdf fileUrl={...} />` y nunca importa `@embedpdf/*`.

**Rationale:** Alineado con la regla de arquitectura: encapsulación total del engine dentro del componente reusable.

### 2) Mock del visor en tests del Workbench
**Decision:** En tests de `DocumentosWorkbench` se mockea `AppVisorEmbedPdf` para evitar WASM/engine real.

**Rationale:** Reduce flakiness y mantiene el foco del test en integración de UI/estado.

## Risks / Trade-offs

- [Riesgo] Layout con altura/overflow incorrectos rompe scroll del visor → **Mitigación**: contenedor con `min-height: 0`, overflow controlado y pruebas visuales básicas.
- [Riesgo] Acoplar lógica de selección y construcción de URL al visor → **Mitigación**: el workbench solo pasa `fileUrl`, sin lógica EmbedPDF.

## Migration Plan

1. Especificar requisitos de integración (spec) y tareas.
2. Implementar wiring en `DocumentosWorkbench` y estilos/layout.
3. Ajustar/crear tests de integración (mock de visor).
4. Validar `npm test` (focal) y documentar evidencia.

## Open Questions

- ¿Cuál es la fuente exacta del `fileUrl` (campo/DTO) cuando el usuario selecciona un documento en el workbench?
- ¿El workbench ya tiene un panel “visor” existente que se debe reemplazar o solo se agrega uno nuevo?

## Validation Evidence (2026-05-05)

- `npm.cmd test -- src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`: PASS
