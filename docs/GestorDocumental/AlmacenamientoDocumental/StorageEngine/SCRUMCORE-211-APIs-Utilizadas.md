# SCRUMCORE-211 — APIs Utilizadas (Contrato SCRUM-201)

## Endpoints

### 1) Metadata temporal

- Método: `GET`
- Ruta: `/api/workflow/usuarios/firma-temporal`
- Auth: `Authorization: Bearer <JWT>`

Respuesta esperada (wrapper):

- `success: boolean`
- `message: string`
- `data: FirmaTemporalUsuarioWorkflowDto | null`
- `meta?: AppMeta`
- `errors?: array`

### 2) Descargar binario

- Método: `GET`
- Ruta: `/api/workflow/usuarios/firma-temporal/download/{token}`
- Auth: `Authorization: Bearer <JWT>`

## Regla obligatoria (evitar 404 por token)

- `data.UrlTemporal` **se consume tal como llega**.
- Si `UrlTemporal` es relativa:
  - `downloadUrl = baseUrl + UrlTemporal`
- El frontend **NO** reconstruye / parsea / recorta el token manualmente.

## Manejo recomendado de errores

- `400`: claims inválidos/faltantes o firma no configurada (según mensaje del backend).
- `404`: token expirado/no encontrado → re-solicitar metadata y reintentar 1 vez.
- `500`: error inesperado → fallback y registro de incidente.

