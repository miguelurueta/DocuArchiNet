# SCRUMCORE-211 — Firma Personal (API temporal Workflow) en modal de firmas de `AppVisorEmbedPdf`

## Why

Actualmente el visor permite:

- Dibujar una firma (canvas) y usarla como anotación.
- Subir una imagen (ej. PNG) y usarla como firma.

Pero no existe un flujo enterprise para reutilizar una **firma personal** ya registrada en el backend (Workflow) y exponerla en el modal del visor como una opción adicional, sin duplicar lógica ni mover responsabilidades al `DocumentosWorkbench`.

Este ticket agrega una integración **tipada, desacoplada y testeable** con un contrato API existente (SCRUM-201), consumiendo metadata temporal y descargando el binario (Blob) para usarlo en el plugin oficial de firmas de EmbedPDF.

## What changes

1) **UI (modal de firmas)**
- Agregar una pestaña nueva: **“Firma personal”** dentro del modal de firmas existente.
- Mostrar estados enterprise: `loading`, `empty/no-config`, `error`, `ready`.
- Mostrar información mínima: `FileName`, `ExpiresAt` (si aplica).
- Botón principal: **“Usar firma personal”** (usa la imagen descargada como firma actual del modal, sin implementar render custom de firma).

2) **Integración API (sin lógica de negocio en Workbench)**
- Consumir exclusivamente el contrato:
  - `GET /api/workflow/usuarios/firma-temporal` (metadata)
  - `GET /api/workflow/usuarios/firma-temporal/download/{token}` (binario)
- Reglas obligatorias:
  - En ambos endpoints: `Authorization: Bearer <JWT>` (sin “workarounds”).
  - Usar `data.UrlTemporal` **tal como llega**; si es relativa, concatenar con `baseUrl`.
  - No parsear, reconstruir ni “recortar” tokens manualmente.
  - No cachear la URL temporal más allá de `ExpiresAt`.
  - Si el download responde `404`, re-solicitar metadata y **reintentar 1 vez**.

3) **Arquitectura / encapsulación**
- Todo queda encapsulado dentro de `AppVisorEmbedPdf` (y sus subcarpetas existentes).
- `DocumentosWorkbench` permanece limpio: no recibe estados, lógica ni dependencias de plugins/firmas.
- No agregar wrappers innecesarios; reusar estructura modular actual (hooks/types/presentation).

4) **Testing + documentación enterprise**
- Agregar pruebas unit/integration (Vitest + RTL) para:
  - Render de la pestaña “Firma personal”.
  - Estados (loading/error/ready).
  - Flujo: obtener metadata → descargar Blob → “usar firma personal” llama callback esperado (sin red real).
- Actualizar documentación enterprise de SCRUMCORE-211 en `docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/` (archivos `SCRUMCORE-211-*.md`), incluyendo:
  - APIs utilizadas y reglas `UrlTemporal`.
  - Diagrama Mermaid del flujo “modal → metadata → download → plugin signature”.
  - Evidencias de testing (unit + rerender si aplica).

## New capabilities (kebab-case)

- `workflow-personal-signature-tab` — Pestaña “Firma personal” en el modal.
- `workflow-personal-signature-fetch` — Metadata + download blob con JWT.
- `workflow-personal-signature-use-in-viewer` — Usar firma descargada en el pipeline de firma del visor.

## Impact / non-goals

### Impacto esperado
- Mejor UX enterprise: reutiliza firma del usuario sin re-subir o re-dibujar.
- Mantiene arquitectura modular y performance (no agrega render custom ni virtualización manual).
- Reduce errores 404 por token al respetar la regla `UrlTemporal` sin manipulación.

### No objetivos explícitos (no se implementan en este ticket)
- Persistencia de firma en el backend (solo consumo de firma temporal).
- “Firma digital certificada” (PKI) o bloqueo criptográfico del PDF.
- Gestión avanzada de múltiples firmas personales o historiales.
- Cambios en otros módulos fuera de `AppVisorEmbedPdf` salvo ajustes necesarios de tipado/cliente HTTP reutilizable.

