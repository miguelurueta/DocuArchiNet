# SCRUMCORE-220 - Integracion BackEnd

## Endpoint gabinete

Se conserva el endpoint existente:

```http
GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete
```

No se cambio path, metodo, contrato ni response backend.

## Request

La llamada se ejecuta desde:

```ts
getSolicitaGabinetePorTareaWorkflow(idTareaWorkflow, { signal })
```

El segundo parametro es opcional y solo agrega soporte de cancelacion con `AbortSignal`.

## Response esperado

Tipo:

```ts
ApiResponse<RadicadoGabineteWorkflowDto | null>
```

DTO:

```ts
type RadicadoGabineteWorkflowDto = {
  EstadoExistenciaRadicado?: "YES" | "NO";
  NombreGabinete?: string;
  Radicado?: string;
  IdTareaWorkflow?: number;
};
```

## Normalizacion frontend

El provider expone solamente:

- `nombreGabinete?: string`
- `gabineteLoading: boolean`
- `gabineteError?: string`

`NombreGabinete` backend se normaliza a `nombreGabinete`.

`EstadoExistenciaRadicado` no se propaga como contrato publico del contexto para evitar ampliar el contexto fuera del alcance. Si backend retorna `NO`, se traduce a `gabineteError` funcional.

## Errores

Casos soportados:

- `success=false`: se lee `errors[0].errorMessage`, `errors[0].Message` o `message`.
- exception HTTP/cancelacion: se normaliza a string seguro sin exponer axios al contexto ni a consumidores.
- cancelacion: se ignora como flujo esperado, no se trata como error funcional.

## Retry / reload

No se implementa retry automatico.

`reloadGabinete()` es el mecanismo explicito de recarga:

- aborta request anterior si existe;
- ejecuta nueva request para el `idTareaWf` actual;
- actualiza `nombreGabinete` o `gabineteError`.

## Fallback

- `data: null`: `nombreGabinete` queda `undefined`.
- `NombreGabinete` vacio: `nombreGabinete` queda `undefined`.
- `idTareaWf` invalido: no hay request.

## Compatibilidad legacy

- El service mantiene el endpoint y contrato existentes.
- Consumers UI no llaman service directamente.
- El listado de documentos consume el `nombreGabinete` centralizado desde contexto.
- La resolucion de documentos sigue usando servicios existentes de `listaDocumentosRadicados`.
