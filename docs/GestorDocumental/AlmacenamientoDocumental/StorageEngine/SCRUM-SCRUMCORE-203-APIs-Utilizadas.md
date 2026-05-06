# SCRUMCORE-203 — APIs utilizadas

## Estado

N/A — `AppVisorEmbedPdf` no consume endpoints HTTP propios del backend.

## Alcance real

- `fileUrl` se resuelve por el navegador (fetch del PDF) y puede apuntar a:
  - un path local servido por Vite (`/demo/...`)
  - una URL externa

## Consideraciones (cuando `fileUrl` es remoto)

- CORS debe estar habilitado por el servidor remoto.
- Autenticación/headers especiales no están contemplados en el API público del componente.

## Estrategia de mocks / MSW

- N/A por ahora (no hay endpoints propios).
- Si en el futuro se incorpora API (p. ej. obtener URL firmada), se debe:
  - tipar request/response
  - mockear con MSW
  - documentar retries/caching/errores
