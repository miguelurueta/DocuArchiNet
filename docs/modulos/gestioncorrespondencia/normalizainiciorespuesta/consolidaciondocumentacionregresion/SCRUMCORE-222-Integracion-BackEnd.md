# SCRUMCORE-222 - Integracion BackEnd

## Contratos relevantes

### Gabinete por tarea de workflow

Endpoint vigente (sin cambios):

- `GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`

Request:
- `idTareaWorkflow`: número de tarea (contextual, proveniente de `GestionRespuesta`).

Response esperado:
- `success` y `data` con atributos de gabinete (`NombreGabinete`, `EstadoExistenciaRadicado`, etc.).
- Error de negocio (`success=false` con `errors`) o error HTTP/transport.

### Lista documental (SCRUM-205)

- Se mantiene contrato existente de query y actions.
- El flujo documental no altera payload de `load/loadChildren`.
- `useListaDocumentosRadicadosTreeTable` aporta el `NombreGabinete` desde contexto y lo usa como insumo requerido por la acción existente, sin modificar su esquema.

## Request/Response y fallback

- Si `EstadoExistenciaRadicado = "NO"`:
  - Se publica `gabineteError` funcional.
  - `nombreGabinete` queda `undefined`.
- Si falla backend o hay fallo de red:
  - Se publica `gabineteError` descriptivo.
  - Se mantiene render de árbol y se bloquean acciones que requieran gabinete.
- Si no hay `idTareaWf` válido:
  - No se ejecuta request de gabinete.
  - Estado vuelve a `idle` con campos de gabinete limpios.

## Relación FE-BE y estabilidad de integración

- `GestionRespuestaDocumentosProvider` es único consumidor de la consulta de gabinete.
- `useListaDocumentosRadicadosTreeTable` deja de consultar `getSolicitaGabinetePorTareaWorkflow`.
- Esto elimina divergencias entre consumidores y evita llamadas duplicadas.

## Errores y fallback

- Mensajes funcionales en estado de error no rompen layout.
- Se puede ejecutar `reloadGabinete` para reintentar consulta.
- Se evita `stale state` con secuencia de request y abort.

## Matriz FE-BE

| Capa FE                                 | Fuente BE / flujo                    | Estado final |
|-----------------------------------------|--------------------------------------|-------------|
| `GestionRespuestaDocumentosProvider`      | `solicitaGabinetePorTareaWorkflow`   | Estado transversal (gabinete + flags) |
| `useListaDocumentosRadicadosTreeTable`   | Contexto transversal                  | Query actions documentales (`load`, `loadChildren`, `ver_documento`) |
| `AppVisorEmbedPdf` (consumo indirecto)  | Acciones de documento desde árbol      | Flujo de apertura sin cambios en contrato |
| Adjuntos (`files`)                      | Contexto transversal                  | Sin cambios |

## JSONs de referencia

### Éxito (gabinete)

```json
{
  "success": true,
  "data": {
    "EstadoExistenciaRadicado": "SI",
    "NombreGabinete": "BÁCULO GENERAL"
  },
  "message": "OK"
}
```

### No existe radicado

```json
{
  "success": true,
  "data": {
    "EstadoExistenciaRadicado": "NO",
    "NombreGabinete": ""
  }
}
```

### Error funcional

```json
{
  "success": false,
  "errors": [
    {
      "errorMessage": "No fue posible resolver gabinete."
    }
  ]
}
```
