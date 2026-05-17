# Design — SCRUMCORE-211 (Firma Personal / API temporal Workflow)

## Context

El visor `AppVisorEmbedPdf` ya cuenta con un modal de firmas (tabs) que permite:

- Dibujar una firma (canvas) y usarla como anotación.
- Subir una firma como imagen (PNG) y usarla como anotación.

Este cambio agrega una tercera fuente: **Firma personal** proveniente del backend (Workflow) vía API temporal (SCRUM-201).

Restricciones clave:

- No mover lógica al `DocumentosWorkbench`.
- Mantener arquitectura modular del visor.
- No implementar lógica custom de firma/annotación: la inserción sigue siendo responsabilidad de EmbedPDF (plugin oficial de signature/annotation existente).
- Consumir `UrlTemporal` exactamente como llega; no parsear token.
- Autorización obligatoria con JWT (Bearer).

## Goals / Non-Goals

### Goals
- Agregar pestaña **“Firma personal”** en el modal existente.
- Consumir endpoints de metadata + download para obtener un `Blob` (firma) y transformarlo al formato ya soportado por el modal (misma ruta que upload/draw).
- Implementar manejo de expiración/404 del download con reintento controlado (1 vez).
- Mantener performance: no provocar rerenders del visor ni añadir wrappers de viewport/plugins.
- Agregar tests (Vitest + RTL) con mocks (sin red real).
- Actualizar documentación enterprise SCRUMCORE-211.

### Non-Goals
- Firma digital certificada (PKI), validación criptográfica, sellos de tiempo.
- Persistencia en backend (solo consumo de temporal).
- Gestión avanzada de múltiples firmas personales (solo una “temporal” por usuario).

## Decisions

### D1 — Encapsulación en el visor (sin Workbench)
**Decisión:** toda la integración API vive dentro del árbol del visor: `src/app/Components/UI/AppVisorEmbedPdf/**`.

**Rationale:** preserva la arquitectura enterprise ya establecida: `DocumentosWorkbench` no conoce plugins/estados internos. Reduce riesgos de acoplamiento y regresiones en otros flujos.

**Alternativas consideradas:**
- Implementarlo en Workbench para compartir “firma personal” entre vistas. Rechazada: rompe encapsulación y complica el ownership.

### D2 — Cliente HTTP: reutilizar infraestructura existente
**Decisión:** implementar un hook/servicio local que use el cliente HTTP ya existente del proyecto (Axios) y el mecanismo actual de JWT (p. ej. `ManejadorJWT`/infra compartida), sin introducir un nuevo “SDK”.

**Rationale:** consistencia con el resto de la app (headers, interceptores, baseURL), y testabilidad vía mocks/spies.

**Alternativas consideradas:**
- `fetch` directo local. Rechazada si el repo ya centraliza axios (riesgo de duplicar configuración).

### D2.1 — Fuente de `baseUrl` y JWT (sin ambigüedad)
**Decisión:** para construir URLs y headers:

- `baseUrl` **MUST** provenir del `baseURL` configurado en el cliente HTTP central (Axios instance) ya usado por la app.
- El JWT **MUST** obtenerse del helper de sesión existente (p. ej. `ManejadorJWT` o equivalente) y enviarse como `Authorization: Bearer <JWT>` en **ambos** endpoints.

**Rationale:** evita divergencias de configuración, respeta interceptores y reduce riesgo de errores al concatenar URLs.

### D3 — Regla `UrlTemporal` sin manipulación
**Decisión:** construir `downloadUrl` así:
- Si `UrlTemporal` inicia con `http`: usarla tal cual.
- Si es relativa: `downloadUrl = baseUrl + UrlTemporal`.

**Rationale:** evita 404 por reconstrucción incorrecta del token. Cumple el contrato SCRUM-201.

### D4 — Reintento controlado ante 404 de descarga
**Decisión:** ante `404` del download:
1) Re-solicitar metadata.
2) Reintentar download una sola vez.

**Rationale:** tokens temporales pueden expirar; un reintento evita UX frágil sin convertirlo en bucle.

### D5 — Integración con modal: convertir Blob a “firma usable” reutilizando pipeline existente
**Decisión:** el tab “Firma personal” produce el mismo output que el tab “Upload”, reutilizando:
- `URL.createObjectURL(blob)` para preview/uso inmediato.
- “Use signature” existente del modal para setear la firma actual (sin introducir un nuevo tipo de firma).

**Rationale:** evita lógica custom de firma; se integra con el flujo ya probado.

### D6 — Momento de carga (just-in-time)
**Decisión:** la carga de “Firma personal” ocurre **just-in-time**:

- Al entrar a la pestaña “Firma personal” (o al abrir el modal si esa pestaña está activa por defecto).
- **MUST NOT** pre-cargar en background cuando el usuario no ha solicitado esa pestaña.

**Rationale:** reduce expiración de token temporal y evita requests innecesarios.

### D7 — Accesibilidad mínima
**Decisión:** el tab y los controles principales del flujo “Firma personal” **MUST** incluir accesibilidad mínima:

- Etiquetas/atributos (`aria-label`) consistentes con el resto del modal.
- Tooltips para acciones (Reintentar / Usar firma personal) cuando aplique.

**Rationale:** mantiene experiencia enterprise y evita regresiones de navegación/lectores.

## Technical approach

### UI
- `AppPdfSignatureModal.tsx`: agregar `Tabs.TabPane`/equivalente para “Firma personal”.
- Estados UI:
  - `loading`: spinner + “Cargando firma personal…”
  - `empty`: mensaje de negocio (“No hay firma personal configurada”)
  - `error`: mensaje + botón “Reintentar”
  - `ready`: preview + “Usar firma personal”
  - Accesibilidad: labels/tooltips en controles clave

### Hook / service
Crear un hook tipo `useWorkflowPersonalSignature()` que expone:
- `status`: `"idle" | "loading" | "ready" | "error" | "empty"`
- `meta`: `{ fileName, contentType, expiresAt, urlTemporal } | null`
- `blobUrl`: `string | null`
- `load()` / `reload()`
- `clear()` (revoca objectURL para evitar leaks)

Internamente:
- `GET /api/workflow/usuarios/firma-temporal` → validación wrapper `{ success, message, data }`.
- Construcción `downloadUrl` con regla de `UrlTemporal`.
- `GET downloadUrl` como `blob`.
- En `404` del download: re-metadata + retry 1 vez.
- `URL.createObjectURL(blob)` y `URL.revokeObjectURL` en cleanup.

### Token/JWT
- Obtener JWT del mecanismo actual (sin copiar tokens a logs, sin exponerlos en UI).
- Inyectar `Authorization: Bearer <JWT>` en ambos requests.

## Risks / Trade-offs

- [Expiración del token temporal] → Mitigación: reintento 404 + solicitud “just-in-time” al abrir pestaña.
- [Memory leaks por ObjectURL] → Mitigación: `URL.revokeObjectURL` en cleanup y al reemplazar.
- [Inconsistencias del wrapper `success/message`] → Mitigación: tipado estricto + fallback a `error` con mensaje controlado.
- [BaseURL incorrecta para `UrlTemporal` relativa] → Mitigación: usar baseURL del cliente HTTP ya configurado.

## Migration plan

1) Implementar hook + tipos.
2) Integrar pestaña “Firma personal” en el modal.
3) Añadir tests (mocks de servicios).
4) Actualizar documentación enterprise SCRUMCORE-211.
5) Verificar manualmente en entorno local (con JWT real) y validar flujo de descarga/preview/uso.

Rollback:
- Revertir el tab + hook sin tocar plugins core del visor; cambios son localizados.

## Open questions

- ¿Cuál es la fuente única de `baseUrl` para concatenar `UrlTemporal` relativa (Axios `baseURL` vs config central)?
- ¿Cómo se obtiene JWT actualmente en el módulo del visor (helper ya existente vs dependencia compartida)?
