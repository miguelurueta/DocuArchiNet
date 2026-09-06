# Estados, errores y antirregresión

| Código | Condición |
|---|---|
| `EXTERNAL_ACCESS_DENIED` | HTTP 401/403 |
| `EXTERNAL_TIMEOUT` | Vence timeout sin cancelación externa |
| `EXTERNAL_CANCELLED` | El llamador solicita cancelación |
| `EXTERNAL_UNAVAILABLE` | Fallo de transporte o HTTP 5xx |
| `EXTERNAL_INVALID_RESPONSE` | Status restante, MIME, tamaño o contenido inválido |

Los mensajes conservan el `correlationId` como dato separado. No incorporan excepción cruda, URL/query, autorización, token, credenciales, ruta o cuerpo.

Antirregresión: no cambian `Integracionccv/Class_ClassResfull.vb`, endpoints de `webservice/`/`App_Code/`, `ServiciosIntegracion/`, `AlmacenaDocumentoTareaWorkflow`, base de datos, PDF ni gate.
