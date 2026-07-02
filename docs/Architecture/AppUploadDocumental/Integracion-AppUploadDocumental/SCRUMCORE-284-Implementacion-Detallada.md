# SCRUMCORE-284 - Implementacion detallada

## 1. Contexto general

SCRUMCORE-284 complementa el trabajo previo de integracion de `AppUploadDocumental` en Gestion Respuesta. La implementacion actual ya permite seleccionar archivos, asignar metadata por archivo, ejecutar almacenamiento por chunks y asociar el resultado como anexo de una respuesta de radicado.

Este ticket endurece el flujo con tipologias reales de workflow, campos obligatorios de inventario, refresh del listado documental, cierre automatico del modal y diagnostico de contratos backend.

## 2. Tipologias documentales por workflow

Se reemplazo la fuente semilla/hardcoded de tipologias por el servicio workflow:

```txt
GET /api/gestor-documental/tipologias-documentales
```

Parametros enviados:

```ts
{
  Contexto: "WORKFLOW",
  IdTareaWf: query.idTareaWf,
  IdRutaWf: query.idRutaWf
}
```

Reglas implementadas:

- `idTareaWf` debe ser numero positivo.
- `idRutaWf` debe ser numero positivo.
- No se envia `IdTipoTramite`.
- No se envia `IdTipoTramite=0`.
- La respuesta backend `{ Id, Descripcion }` se normaliza a:

```ts
{
  value: Id,
  label: Descripcion,
  idTipoDocumento: Id,
  nombreTipoDocumento: Descripcion
}
```

El dropdown visible por archivo sigue usando `AppInputSelect`, pero la carga de opciones vive en servicios/hook del modulo, no dentro de componentes shared.

## 3. Contexto documental de Gestion Respuesta

`GestionRespuestaUploadDocumental` construye el contexto que consume `AppUploadDocumental`.

Campos relevantes:

```ts
{
  nombreGabinete,
  idTareaWorkflow: idTareaWf,
  idRutaWorkflow: idRutaWf,
  idRespuesta: idRespuestaRadicado,
  nameModulo: radicado,
  idUsuarioGestion,
  idEmpresa,
  fechaElaboracion
}
```

### Usuario autenticado

Se actualizo `ManejadorJWT` para conservar informacion de usuario autenticado y resolver `idUsuarioGestion`.

Fuentes soportadas:

- usuario guardado en local storage;
- claims del JWT como fallback;
- aliases como `usuarioId`, `UsuarioId`, `idUsuario`, `IdUsuario`, `IdUsuarioGestion`, `idUsuarioGestion`, `nameid`, `sub`, `uid`.

### Empresa actual

`GestionRespuestaUploadDocumental` usa `useEmpresaActual()` para resolver `IdEmpresa`.

Si la empresa esta cargando o no se puede resolver, el upload se bloquea con `Alert` funcional.

### Fecha de elaboracion

Se genera `fechaElaboracion` como fecha local de formato `yyyy-MM-dd` para completar el bloque de inventario requerido por backend.

## 4. Request final de almacenamiento

El request final se construye en:

```txt
src/modules/gestionCorrespondencia/adapters/gestionRespuestaUploadDocumental.mapper.ts
```

Estructura funcional enviada a StorageEngineV2:

```ts
{
  nombreGabinete,
  nombreDocumento,
  requestId,
  inventario: {
    IdUsuarioGestion,
    IdEmpresa,
    Radicado,
    FechaElaboracion
  },
  trd,
  workflow: {
    idTareaWorkflow,
    idRutaWorkflow
  },
  cabinetIndexSeed: {
    sourceModule: "RADICACION",
    providerKey: "RADICACION",
    version: "1.0.0",
    payload: {
      modoResolucion: "RespuestaRadicado"
    }
  },
  anexoRespuesta: {
    idRespuestaRadicado,
    nombreArchivo,
    tipoAdjunto: "respuesta",
    observacion: "Anexo cargado desde workflow"
  },
  documento: {
    idDocumento,
    nombreOriginal,
    extension,
    numeroPaginas
  }
}
```

El servicio de almacenamiento inyecta despues:

```ts
{
  rutaTemporalId,
  documentos: [
    {
      archivoTemporalId,
      nombreOriginal,
      extension,
      numeroPaginas
    }
  ]
}
```

## 5. Tipologia y TRD

Decision actual para diagnostico:

- El frontend no bloquea el guardado cuando falta tipologia.
- Si no hay `idTipoDocumento` ni `nombreTipoDocumento`, el mapper envia `trd: null`.
- El backend recibe el request real y decide si rechaza por `StorageTrd requerido`.
- El frontend deja logs de request/response para validar contrato backend.

Esto se hizo para comprobar el comportamiento real del backend. La validacion backend observada fue:

```txt
Cabinet index seed is invalid: StorageTrd requerido
```

Interpretacion:

- El backend requiere TRD/tipologia para este provider/cabinet index seed.
- La regla real existe en backend.
- El mensaje actual es tecnico; si se quiere UX funcional, backend deberia exponer `errors[0].UserMessage` con texto claro.

## 6. Manejo de errores de tipologia

Se ajusto `useAppUploadDocumentalActions` para:

- permitir que el backend valide ausencia de tipologia;
- suprimir mensaje visible cuando el error backend corresponde a tipologia/TRD;
- marcar visualmente el archivo/dropdown como error;
- no disparar mensaje funcional duplicado si el caso es tipologia;
- conservar otros errores visibles cuando no son de tipologia.

Deteccion defensiva:

- `tipologia`;
- `tipologias`;
- `tipo documental`;
- `tipo documento`;
- `trd`.

## 7. Diagnostico temporal retirado

Durante la validacion funcional se usaron trazas temporales para revisar contratos reales sin exponer tokens ni bytes:

```txt
[almacenamientoDocumentalUpload] almacenarDocumento missing typology request
[almacenamientoDocumentalUpload] almacenarDocumento missing typology response
[almacenamientoDocumentalUpload] almacenarDocumento backend validation
```

Contenido validado:

- `requestId`;
- endpoint;
- request frontend;
- payload backend;
- response backend;
- errores backend si existen.

Protecciones:

- No se loguean tokens.
- No se loguean bytes del archivo.
- `RutaTemporalId` y `ArchivoTemporalId` se enmascaran parcialmente.

Estado de cierre:

- Las trazas fueron retiradas del runtime antes del commit final.
- La evidencia se conserva solo en esta documentacion de arquitectura.

## 8. Flujo de almacenamiento

El flujo por archivo se mantiene:

```txt
init -> chunks -> status -> complete -> almacenar
```

Reglas conservadas:

- chunks como bytes crudos;
- `Content-Type: application/octet-stream`;
- `X-Total-Chunks`;
- `chunkIndex` base cero;
- recalculo de total chunks si backend responde otro `chunkSizeBytes`;
- status antes de complete cuando esta habilitado;
- request final por archivo.

## 9. Cierre automatico del modal

Se ajusto `GestionRespuestaUploadDocumental`:

```txt
AnexoRespuesta.Created === true
-> refreshDocumentos()
-> onClose()
```

Resultado:

- Al almacenar correctamente, se refresca el Workbench.
- El modal de carga documental se cierra automaticamente.
- El usuario vuelve al contexto del tab Gestion/Documentos con el listado actualizado.

## 10. Refresh del Workbench

`DocumentosWorkbench` usa `documentosRefreshKey` para forzar remount de `AppTreeTable`.

Motivo:

- llamar `documentosTable.load()` desde el hook no actualizaba el estado interno del `AppTreeTable`;
- remount por `key` obliga al componente a ejecutar su `load` inicial y renderizar la data nueva.

Clave actual:

```tsx
key={`documentos-${documentosRefreshKey}-${actionRefreshKey}`}
```

## 11. Eliminacion desde AppTreeTable

Se detecto que algunas acciones llegaban al endpoint generico inexistente:

```txt
POST /api/dynamic-ui-table/actions/execute
```

Este endpoint devolvia `404`.

Solucion:

- `documentosWorkbenchResponseAdapter` fuerza acciones de columnas/menu del Workbench a `behavior: "client_event"`;
- `AppTableActionCellRenderer` enruta hacia `onClientEvent`;
- `DocumentosWorkbench` ejecuta `documentosTable.onActionTriggered`;
- el hook llama el endpoint propio:

```txt
POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action
```

Trazas temporales usadas durante la validacion y retiradas antes del cierre:

```txt
[DocumentosWorkbench] action triggered
[useGestionRespuestaDocumentosTable] performAction input
[useGestionRespuestaDocumentosTable] action request
[useGestionRespuestaDocumentosTable] performAction response
```

Caso validado por consola:

```txt
ActionId: eliminar_item
RowId: doc-9931
Payload.DocumentId: 9931
NombreGabinete: CORRESPO
success: true
Operation: deleted
RequiresReloadNode: true
```

Despues del `success`, el Workbench incrementa `actionRefreshKey` para remonte visual y limpiar fila eliminada.

## 12. UX del upload documental

### Modal

- El upload se abre desde un disparador visual `Adjuntar documentos`.
- El modal queda centrado.
- El body tiene scroll cuando el alto no alcanza.
- El footer interno del batch queda fijo.
- El modal se cierra automaticamente al almacenar correctamente.

### Toolbar

- `AppUpload` compacto alineado con `Guardar todo` y `Limpiar todo`.
- Boton primario azul.
- Boton de limpiar en rojo vivo.
- Botones deshabilitados sin hover activo.

### Cola de archivos

- Cola arriba, visor abajo.
- Cola con scroll interno.
- Articles compactos.
- Dropdown mas ancho y alineado.
- Articles sin hover exagerado.
- Dropdown con borde moderno.
- Se removio el texto redundante de error de tipologia dentro de cada article.

### Preview

- Preview inicialmente oculto.
- Se activa solo con boton `Ver`.
- La fila se marca activa solo cuando el preview esta abierto.
- Cerrar preview libera seleccion visual.

## 13. Limite de archivos pesados

Estado actual:

```ts
const DEFAULT_MAX_SIZE_BYTES = 25 * 1024 * 1024;
```

Ubicacion:

```txt
src/modules/gestionCorrespondencia/services/gestionRespuestaUploadDocumental.service.ts
```

Impacto:

- El cliente tecnico soporta archivos pesados por chunks.
- El bloqueo actual de archivos pesados ocurre por configuracion frontend local de 25 MB.

Pendiente recomendado:

```txt
GET /api/gestor-documental/configuracion-upload?nameProceso=CORRESPO
```

Mapeo esperado:

```txt
ExtensionUpload -> accept / allowedExtensions
LengUpload       -> maxSizeBytes
```

Cuando se use `LengUpload`, el limite dejara de estar hardcodeado.
