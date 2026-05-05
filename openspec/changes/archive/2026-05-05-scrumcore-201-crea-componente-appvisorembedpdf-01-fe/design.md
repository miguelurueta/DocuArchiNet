## Context

El ticket SCRUMCORE-201 solicita crear un componente reusable `AppVisorEmbedPdf` (incremento 01-FE) basado en EmbedPDF para visualización de PDFs dentro de la SPA React (DocuArchiCore.react).

Este componente fue previamente explorado en el repo, pero en SCRUMCORE-200 se removió el componente y todas las dependencias `@embedpdf/*`. Para implementar este ticket se deberá reintroducir dependencias (o definir explícitamente una alternativa) y dejar una base estable y reutilizable para integraciones posteriores.

El stack actual: React 19 + Vite + TypeScript estricto + Axios. Testing con Vitest + Testing Library.

## Goals / Non-Goals

**Goals:**
- Definir e implementar una capability reusable `app-appvisorembedpdf-01-fe` con un componente `AppVisorEmbedPdf` base.
- Reintroducir dependencias permitidas para 01-FE (`@embedpdf/core`, `@embedpdf/engines`, `@embedpdf/plugin-document-manager`, `@embedpdf/plugin-viewport`, `@embedpdf/plugin-scroll`, `@embedpdf/plugin-render`) con versión fijada y lockfile actualizado.
- Exponer la API pública obligatoria `AppVisorEmbedPdfProps` (`fileUrl?`, `className?`, `style?`) y mantener EmbedPDF encapsulado.
- Implementar virtualización nativa con `Scroller` + lazy rendering nativo de EmbedPDF.
- Agregar pruebas de comportamiento para el contrato del componente según los specs.

**Non-Goals:**
- Implementar features avanzadas (zoom, rotate, toolbar, search, thumbnails, annotations, signatures, password, print/download).
- Integración profunda en todos los módulos consumidores; solo un wiring mínimo si es necesario para validar.
- Rediseñar arquitectura de UI/rutas/layout.

## Decisions

### 1) Mantener el componente dentro de `src/app/Components/UI`
**Decision:** Ubicar el componente reusable bajo `src/app/Components/UI/AppVisorEmbedPdf` (o nombre final definido en specs) con export público vía `index.ts`.

**Rationale:** Es consistente con el patrón existente de componentes UI compartidos.

**Alternatives considered:**
- Colocarlo en un módulo de dominio específico: rechazado porque se requiere reutilizable transversal.

### 2) Usar EmbedPDF como engine (reintroducción controlada)
**Decision:** Reintroducir `@embedpdf/*` como dependencias de runtime si el spec mantiene EmbedPDF como requerimiento.

**Rationale:** El ticket lo solicita explícitamente y permite acelerar el visor base.

**Alternatives considered:**
- Usar `react-pdf` o visor nativo del navegador: viable, pero solo si los specs permiten cambiar proveedor.

### 3) Contrato mínimo y testeable
**Decision:** Mantener el contrato público mínimo exactamente como lo pide el ticket: `fileUrl?`, `className?`, `style?`. Mantener callbacks/extra props fuera de la API pública de 01-FE salvo que el spec lo amplíe.

**Rationale:** Facilita adopción y reduce acoplamiento. Permite pruebas de comportamiento sin depender del engine real (mock).

## Risks / Trade-offs

- [Riesgo] Reintroducir `@embedpdf/*` aumenta bundle y superficie de vulnerabilidades → **Mitigación**: dependencias mínimas, revisión `npm audit`, evitar extras no usados.
- [Riesgo] Tests frágiles por depender del engine real/WebAssembly → **Mitigación**: encapsular engine y mockear en tests; probar “render contract” y estados.
- [Trade-off] Cambios recientes removieron el componente, puede haber inconsistencias de naming/ubicación → **Mitigación**: alinear con specs y actualizar docs/exports coherentemente.

## Migration Plan

1. Crear specs para `app-appvisorembedpdf-01-fe` (requisitos y escenarios).
2. Implementar estructura del componente y exports.
3. Reintroducir dependencias `@embedpdf/*` necesarias y correr `npm install`.
4. Agregar tests Vitest para los escenarios del spec (mock de engine si aplica).
5. Validar `npm run build` y `npm test`; documentar evidencia en el cambio.

**Rollback:** Revertir PR/commits del ticket y remover dependencias nuevamente.

## Open Questions

- ¿El proveedor es obligatoriamente EmbedPDF o el spec permite usar otra librería/approach?
- ¿Dónde se consumirá inicialmente para validar (módulo/pantalla) o basta con pruebas unitarias?
- ¿Qué props mínimas son requeridas para 01-FE (solo render básico vs. toolbar/zoom inicial)?

## Validation Evidence (2026-05-05)

- `npm.cmd test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`: PASS
- `npm.cmd run build`: el repo falla por errores TypeScript preexistentes en `AppEditor`/`gestionCorrespondencia`; sin errores adicionales provenientes de `AppVisorEmbedPdf`.
