# Seguridad

- Contexto de tarea y trámite resuelto en servidor.
- `ExternalKey` opaco; no se expone NIT, razón social, matrícula, token, URL, ruta ni payload crudo.
- Campos presentables limitados a 200 caracteres.
- Gate apagado retorna `FEATURE_DISABLED` antes de consultar SII o bases.
- Mapeos ambiguos fallan de forma cerrada.
