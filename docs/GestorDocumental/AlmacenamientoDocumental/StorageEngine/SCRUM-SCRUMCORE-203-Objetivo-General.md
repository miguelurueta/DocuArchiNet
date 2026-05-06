# SCRUMCORE-203 — Objetivo general

## Qué se documenta

Documentación enterprise del componente reusable frontend `AppVisorEmbedPdf`.

## Problema que resuelve

Estandarizar el conocimiento técnico para que el componente sea:
- auditable y trazable
- consistente con arquitectura enterprise frontend
- fácil de mantener y extender (plugins futuros) sin filtrar EmbedPDF a consumers

## Alcance funcional

- Renderizado de PDF vía EmbedPDF + Pdfium.
- Scroll vertical con virtualización/lazy rendering.
- Estados base: loading engine / loading documento / empty / error / success.
- Configuración de demo PDF local cuando `fileUrl` no existe.

## Objetivo arquitectónico

- Encapsular engine/plugins/lógica de apertura dentro de `AppVisorEmbedPdf`.
- Evitar acoplamiento: consumers no importan `@embedpdf/*`.

## Impacto técnico

- Agrega documentación formal bajo `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`.
- Define reglas de soporte (troubleshooting, límites, performance, testing).

## Resultado esperado

- Set de documentos enterprise obligatorios completados y mantenibles.
- Evidencias de testing (Vitest/Playwright) anexadas o referenciadas.
