# SCRUM-250 Integracion Frontend End-to-End - Anexos de Respuesta

## 1. Objetivo

Este documento describe el flujo completo que debe ejecutar el frontend para almacenar un archivo y asociarlo opcionalmente como anexo de una respuesta de radicado mediante `AnexoRespuesta`.

El flujo no inicia en `POST /api/gestor-documental/almacenamiento`. Antes de esa llamada, el frontend debe crear y completar un upload temporal:

```text
login/token
  -> init upload temporal
  -> subir chunk(s)
  -> consultar status
  -> complete upload temporal
  -> almacenamiento final con AnexoRespuesta
```

## 2. Seguridad

Todos los endpoints requieren JWT:

```http
Authorization: Bearer {jwt}
```

Claims esperados por la API:

| Claim | Uso |
|---|---|
| `usuarioid` | Dueño del upload temporal y usuario de almacenamiento. |
| `defaulalias` | Alias de base de datos para resolver gabinete, metadata y transaccion. |

El mismo usuario/token que inicia el upload temporal debe usar `chunk`, `status`, `complete` y el almacenamiento final.

## 3. Flujo Completo

| Paso | Endpoint | Resultado que alimenta el siguiente paso |
|---:|---|---|
| 1 | `POST /api/gestor-documental/almacenamiento/upload-temporal/init` | Devuelve `RutaTemporalId`, `ArchivoTemporalId`, `ChunkSizeBytes`. |
| 2 | `PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}` | Sube bytes del archivo. |
| 3 | `GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status` | Confirma chunks recibidos/pendientes. |
| 4 | `POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete` | Ensambla y deja el archivo temporal en estado `Completed`. |
| 5 | `POST /api/gestor-documental/almacenamiento` | Consume el temporal y, si viene `AnexoRespuesta`, inserta `ra_anexos_respuesta`. |

## 4. Paso 1 - Inicializar Upload Temporal

Endpoint:

```http
POST /api/gestor-documental/almacenamiento/upload-temporal/init
Content-Type: application/json
```

Request:

```json
{
  "NombreOriginal": "respuesta-firmada-usuario-final-version-larga.pdf",
  "TamanoBytes": 48216,
  "Extension": ".pdf",
  "HashSha256Esperado": null,
  "NumeroChunks": 1
}
```

Campos:

| Campo | Requerido | Regla |
|---|---:|---|
| `NombreOriginal` | Si | Nombre del archivo seleccionado por el usuario, sin ruta local. |
| `TamanoBytes` | Si | Tamano real del archivo en bytes. |
| `Extension` | Si | Extension real, por ejemplo `.pdf`. |
| `HashSha256Esperado` | No | Si se envia, `complete` valida el hash final. |
| `NumeroChunks` | Si | Total de chunks que el frontend enviara. |

Response esperado:

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "RutaTemporalId": "usr_141_9e9a4fcd8f7a4f0bb7f7f4df0e7a1234",
    "ArchivoTemporalId": "af_89a11a7c81424d4e8f9bb5a821f6b901.pdf",
    "ChunkSizeBytes": 10485760,
    "Estado": "Initialized"
  },
  "errors": []
}
```

El frontend debe conservar:

- `RutaTemporalId`
- `ArchivoTemporalId`
- `ChunkSizeBytes`
- `NombreOriginal`

`NombreOriginal` se reutiliza en:

- `Documentos[0].NombreOriginal`
- `AnexoRespuesta.NombreArchivo`

## 5. Paso 2 - Subir Chunks

Endpoint:

```http
PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
Content-Type: application/octet-stream
X-Total-Chunks: {numeroChunks}
```

Reglas:

- `chunkIndex` inicia en `0`.
- Cada request envia solo los bytes del chunk.
- `X-Total-Chunks` debe coincidir con `NumeroChunks` enviado en `init`.
- El tamano de cada chunk debe respetar `ChunkSizeBytes`, salvo el ultimo chunk, que puede ser menor.

Ejemplo para archivo de un solo chunk:

```http
PUT /api/gestor-documental/almacenamiento/upload-temporal/usr_141_9e9a4fcd8f7a4f0bb7f7f4df0e7a1234/af_89a11a7c81424d4e8f9bb5a821f6b901.pdf/chunk/0
Authorization: Bearer {jwt}
Content-Type: application/octet-stream
X-Total-Chunks: 1
```

Response esperado:

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "chunkIndex": 0
  },
  "errors": []
}
```

## 6. Paso 3 - Consultar Status

Endpoint:

```http
GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status
```

Response esperado:

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "Estado": "Uploading",
    "ChunksRecibidos": [0],
    "ChunksPendientes": [],
    "TamanoRecibidoBytes": 48216
  },
  "errors": []
}
```

El frontend debe validar:

- `ChunksPendientes` vacio antes de llamar `complete`.
- `TamanoRecibidoBytes` igual al tamano del archivo.

## 7. Paso 4 - Completar Upload Temporal

Endpoint:

```http
POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
```

Response esperado:

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "Estado": "Completed"
  },
  "errors": []
}
```

Despues de `Completed`, el frontend ya puede llamar el almacenamiento final usando el mismo `RutaTemporalId` y `ArchivoTemporalId`.

## 8. Paso 5 - Almacenamiento Final con Anexo

Endpoint:

```http
POST /api/gestor-documental/almacenamiento
Content-Type: application/json
```

Request completo:

```json
{
  "NombreGabinete": "Correspo",
  "RutaTemporalId": "usr_141_9e9a4fcd8f7a4f0bb7f7f4df0e7a1234",
  "NombreDocumento": "Respuesta radicado con soporte",
  "RequestId": "SCRUM-250-E2E-20260603-001",
  "Documentos": [
    {
      "IdDocumento": "doc-scrum-250-001",
      "ArchivoTemporalId": "af_89a11a7c81424d4e8f9bb5a821f6b901.pdf",
      "NombreOriginal": "respuesta-firmada-usuario-final-version-larga.pdf",
      "Extension": ".pdf",
      "NumeroPaginas": 1
    }
  ],
  "CamposIndexacion": [
    {
      "NombreCampo": "REMITENTE",
      "Valor": "KARINA URUETA MIRANDA",
      "EsObligatorio": true
    },
    {
      "NombreCampo": "ENLASE",
      "Valor": "2500466700035",
      "EsObligatorio": false
    },
    {
      "NombreCampo": "DESTINATARIO",
      "Valor": "Carolina Cruz Villareal Nunes",
      "EsObligatorio": false
    },
    {
      "NombreCampo": "NUMERORADICA",
      "Valor": "2500466700035",
      "EsObligatorio": false
    }
  ],
  "Inventario": {
    "IdUsuarioGestion": 169,
    "IdEmpresa": 1,
    "Radicado": "2600466700019",
    "FechaElaboracion": "2026-04-20"
  },
  "Trd": {
    "IdArea": 49,
    "IdSerie": 5,
    "IdSubSerie": 9,
    "IdTipoDocumento": 43,
    "NombreSerie": "COMPROBANTES",
    "NombreSubSerie": "Comprobantes De Egresos",
    "NombreTipoDocumento": "Comprobante De Egreso"
  },
  "Expediente": {
    "IdExpediente": 143,
    "IdTipoExpediente": null,
    "NombreExpediente": "1800000176",
    "IdUnidadConservacion": 27,
    "IdTipoUnidadConservacion": null,
    "NombreUnidadConservacion": "161200000019",
    "IdTipoUnidadDocumental": null,
    "IdClaseDocumento": 10,
    "ClaseDocumento": "DOCUMENTO DIGITALIZADO"
  },
  "Workflow": {
    "IdTareaWorkflow": 933,
    "IdRuta": 9
  },
  "AnexoRespuesta": {
    "IdRespuestaRadicado": 640,
    "NombreArchivo": "respuesta-firmada-usuario-final-version-larga.pdf",
    "TipoAdjunto": "respuesta",
    "Observacion": "Archivo cargado desde el front para anexar a respuesta"
  },
  "FullText": "Documento asociado como anexo de respuesta",
  "NumeroPaginasDeclaradas": 1
}
```

Relacion entre pasos:

| Valor | De donde sale | Donde se usa |
|---|---|---|
| `RutaTemporalId` | Response de `init` | Raiz del request final. |
| `ArchivoTemporalId` | Response de `init` | `Documentos[].ArchivoTemporalId`. |
| `NombreOriginal` | Archivo seleccionado por usuario | `Documentos[].NombreOriginal` y `AnexoRespuesta.NombreArchivo`. |
| `NumeroPaginas` | Analisis del front o metadato conocido | `Documentos[].NumeroPaginas`. |
| `IdRespuestaRadicado` | Contexto de respuesta de radicado | `AnexoRespuesta.IdRespuestaRadicado`. |

El ejemplo anterior corresponde a una corrida E2E real de SCRUM-250 con `NombreGabinete=Correspo`, `IdRespuestaRadicado=640`, `IdTareaWorkflow=933`, `IdRuta=9`, `IdExpediente=143`, `IdUnidadConservacion=27` y `NombreUnidadConservacion=161200000019`.

## 8.1 Almacenamiento con Cabinet Index Provider RADICACION

`CabinetIndexSeed` permite que el backend complete campos de indice del gabinete desde proveedores internos, sin que el frontend tenga que enviar manualmente todos los `CamposIndexacion`.

La documentacion tecnica canonica del provider RADICACION, incluyendo ejemplos frontend, estrategias, prompts, diagramas y semillas E2E vigentes, queda en [SCRUM-282 Integracion Frontend RADICACION](../Core/ProviderRadicacion/SCRUM-282-integracion-provider-radicacion/Integracion-Frontend/README.md). Esta seccion conserva el resumen operativo dentro del documento general de integracion de anexos.

El upload temporal no cambia. El cambio aplica solo en el request final de:

```http
POST /api/gestor-documental/almacenamiento
```

Reglas para frontend:

- `CabinetIndexSeed` es opcional.
- `SourceModule` debe ser `RADICACION`.
- `ProviderKey` debe ser `RADICACION`.
- `Version` debe ser semver, actualmente `1.0.0`.
- `Payload` solo debe contener parametros especializados del provider.
- No duplicar en `Payload` datos que ya existen en bloques comunes como `Inventario`, `Trd`, `Expediente`, `Workflow` o `AnexoRespuesta`.
- `CamposIndexacion` puede ir vacio cuando el provider debe resolver los campos.
- `IdTipoDocumento` y `NombreTipoDocumento` siguen saliendo de `Trd`; no son propiedad del provider.
- Si el provider resuelve `IdExpediente`, el frontend debe enviar `Expediente.IdClaseDocumento` cuando el flujo documental lo requiere.
- Los campos con `VISIBLE=0` en metadata del gabinete no deben mostrarse al usuario, pero siguen siendo indexables por backend. El provider puede llenarlos y almacenamiento puede persistirlos si existen fisicamente en el gabinete.
- Para UI se debe usar metadata visible (`VISIBLE=1`). Para almacenamiento/provider se usa metadata indexable, que lista todos los campos reales del gabinete.

### 8.1.1 Contrato base del seed RADICACION

```json
{
  "CabinetIndexSeed": {
    "SourceModule": "RADICACION",
    "ProviderKey": "RADICACION",
    "Version": "1.0.0",
    "Payload": {
      "ModoResolucion": "RespuestaRadicado"
    }
  }
}
```

### 8.1.2 Ejemplo: radicado interno por plantilla y anexo real

Este contrato corresponde al caso donde el provider usa `InternalRadicadoPlantillaStrategy`: toma `Inventario.Radicado`, busca la plantilla legacy de radicacion y desde la tabla dinamica obtiene el expediente.

En la E2E SCRUM-282 se uso:

- `NombreGabinete=CORRESPO`
- `Inventario.Radicado=2600466700021`
- `AnexoRespuesta.IdRespuestaRadicado=672`
- `CamposIndexacion=[]`
- sin `Expediente.IdExpediente`
- sin `ProveedorExterno`
- sin `RadicadoExterno`
- sin `MatriculaSII`

Request final:

```json
{
  "NombreGabinete": "CORRESPO",
  "RutaTemporalId": "{{rutaTemporalId}}",
  "NombreDocumento": "Provider RADICACION CORRESPO interno",
  "RequestId": "radicacion-interno-{{rutaTemporalId}}",
  "Documentos": [
    {
      "IdDocumento": "15416",
      "ArchivoTemporalId": "{{archivoTemporalId}}",
      "NombreOriginal": "small.pdf",
      "Extension": ".pdf",
      "NumeroPaginas": 1
    }
  ],
  "CamposIndexacion": [],
  "Inventario": {
    "IdUsuarioGestion": 169,
    "IdEmpresa": 1,
    "Radicado": "2600466700021",
    "FechaElaboracion": "2026-04-20"
  },
  "Trd": {
    "IdArea": 45,
    "IdSerie": 11,
    "IdSubSerie": 0,
    "IdTipoDocumento": 43,
    "NombreTipoDocumento": "Comprobante De Egreso"
  },
  "CabinetIndexSeed": {
    "SourceModule": "RADICACION",
    "ProviderKey": "RADICACION",
    "Version": "1.0.0",
    "Payload": {
      "ModoResolucion": "RespuestaRadicado"
    }
  },
  "AnexoRespuesta": {
    "IdRespuestaRadicado": 672,
    "NombreArchivo": "small.pdf",
    "TipoAdjunto": "respuesta",
    "Observacion": "Anexo de respuesta almacenado con provider RADICACION"
  },
  "Expediente": {
    "IdClaseDocumento": 10,
    "ClaseDocumento": "DOCUMENTO DIGITALIZADO"
  },
  "NumeroPaginasDeclaradas": 1
}
```

Resultado esperado del provider:

| Campo | Fuente |
|---|---|
| `ID_EXPEDIENTE` | Tabla dinamica de plantilla, por ejemplo `RAD_GESTION.id_Expediente`. |
| `EXPEDIENTE` | `expediente_archivo.CODIGO_UNICO`. |
| `ID_TIPO_EXPEDIENTE` | `expediente_archivo.RA_TIP_EXPE_ID_TIPO_EXPEDIENTE`. |
| `ID_TIPO_UNIDAD_DOCUMENTAL` | `expediente_archivo.ID_TIPO_UNIDAD_DOCUMENTAL`. |
| `NUMERORADICA` o campo radicado del gabinete | `Inventario.Radicado`. |
| `ENLASE` | `Inventario.Radicado`. |
| `ID_AREA`, `ID_SERIE`, nombres TRD | `expediente_archivo` cuando existan. |
| `ID_TIPODOCUMENTO`, `TIPODOCUMENTO` | `Trd`, propiedad del frontend. |

En esta ruta `CamposIndexacion=[]` no significa que no existan campos para el documento. Significa que el frontend delega al provider RADICACION la construccion de campos indexables. La validacion backend filtra contra metadata indexable del gabinete, no contra metadata visible UI.

Evidencia E2E de referencia:

| Tabla | Valor |
|---|---|
| `CORRESPO.ID` | `9967` |
| `CORRESPO.ID_EXPEDIENTE` | `576` |
| `CORRESPO.EXPEDIENTE` | `TUTELA-2026-06-17` |
| `registro_producion_documental.ID_REGISTRO_PRODUCION_DOCUMENTAL` | `23040` |
| `ra_anexos_respuesta.id_anexo_respuesta` | `150` |

### 8.1.3 Ejemplo: radicado externo con ProviderSpecificExternalStrategy

Este contrato corresponde al caso donde la integracion externa debe indicar datos especializados del proveedor. El backend usa `ProviderSpecificExternalStrategy`.

En la E2E SCRUM-282 se uso:

- `NombreGabinete=ESAL`
- `Inventario.Radicado=S002189729`
- `Payload.ProveedorExterno=SII`
- `Payload.RadicadoExterno=S002189729`
- `CamposIndexacion=[]`
- sin `Expediente.IdExpediente`

Request final:

```json
{
  "NombreGabinete": "ESAL",
  "RutaTemporalId": "{{rutaTemporalId}}",
  "NombreDocumento": "Provider RADICACION externo ESAL",
  "RequestId": "radicacion-externo-{{rutaTemporalId}}",
  "Documentos": [
    {
      "IdDocumento": "15416",
      "ArchivoTemporalId": "{{archivoTemporalId}}",
      "NombreOriginal": "small.pdf",
      "Extension": ".pdf",
      "NumeroPaginas": 1
    }
  ],
  "CamposIndexacion": [],
  "Inventario": {
    "IdUsuarioGestion": 169,
    "IdEmpresa": 1,
    "Radicado": "S002189729",
    "FechaElaboracion": "2026-04-20"
  },
  "Trd": {
    "IdArea": 49,
    "IdSerie": 5,
    "IdSubSerie": 9,
    "IdTipoDocumento": 43,
    "NombreTipoDocumento": "Comprobante De Egreso"
  },
  "CabinetIndexSeed": {
    "SourceModule": "RADICACION",
    "ProviderKey": "RADICACION",
    "Version": "1.0.0",
    "Payload": {
      "ModoResolucion": "RadicadoExterno",
      "ProveedorExterno": "SII",
      "RadicadoExterno": "S002189729"
    }
  },
  "Expediente": {
    "IdClaseDocumento": 10,
    "ClaseDocumento": "DOCUMENTO DIGITALIZADO"
  },
  "NumeroPaginasDeclaradas": 1
}
```

Resultado esperado del provider:

| Campo | Fuente |
|---|---|
| `ID_EXPEDIENTE` | `ra_relacion_radicado_externo_expediente.RadicadoExterno`. |
| `EXPEDIENTE`, TRD y unidad | `expediente_archivo`. |
| `RADICADO` / `ENLASE` | `Inventario.Radicado` o plantilla legacy cuando aplique. |
| `ID_TIPODOCUMENTO`, `TIPODOCUMENTO` | `Trd`, propiedad del frontend. |

Evidencia E2E de referencia:

| Tabla | Valor |
|---|---|
| `ESAL.ID` | `5235` |
| `ESAL.ID_EXPEDIENTE` | `557` |
| `ESAL.EXPEDIENTE` | `510812` |
| `registro_producion_documental.ID_REGISTRO_PRODUCION_DOCUMENTAL` | `23037` |

### 8.1.4 Ejemplo: proveedor SII por matricula

Cuando el proveedor externo es SII y la integracion conoce la matricula, el frontend puede enviar `MatriculaSII` dentro del `Payload`. El backend consulta la cache SII del expediente.

```json
{
  "CabinetIndexSeed": {
    "SourceModule": "RADICACION",
    "ProviderKey": "RADICACION",
    "Version": "1.0.0",
    "Payload": {
      "ModoResolucion": "RadicadoExterno",
      "ProveedorExterno": "SII",
      "MatriculaSII": "123456"
    }
  }
}
```

Reglas:

- `MatriculaSII` solo debe enviarse cuando la integracion realmente la conoce.
- No copiar `MatriculaSII` a la raiz del request.
- Si tambien se envia `RadicadoExterno`, el provider puede usarlo como fallback cuando la cache SII no resuelve expediente.

### 8.1.5 Ejemplo: sin provider, request tradicional

Si el frontend no envia `CabinetIndexSeed`, el backend mantiene el comportamiento tradicional. En ese caso, los campos del gabinete que se quieran persistir deben venir en `CamposIndexacion` o en los bloques comunes existentes.

```json
{
  "NombreGabinete": "CORRESPO",
  "RutaTemporalId": "{{rutaTemporalId}}",
  "NombreDocumento": "Almacenamiento sin provider",
  "Documentos": [
    {
      "IdDocumento": "doc-001",
      "ArchivoTemporalId": "{{archivoTemporalId}}",
      "NombreOriginal": "soporte.pdf",
      "Extension": ".pdf",
      "NumeroPaginas": 1
    }
  ],
  "CamposIndexacion": [
    {
      "NombreCampo": "NUMERORADICA",
      "Valor": "2500466700035",
      "EsObligatorio": false
    },
    {
      "NombreCampo": "ENLASE",
      "Valor": "2500466700035",
      "EsObligatorio": false
    }
  ],
  "Inventario": {
    "IdUsuarioGestion": 169,
    "IdEmpresa": 1,
    "Radicado": "2500466700035",
    "FechaElaboracion": "2026-04-20"
  },
  "NumeroPaginasDeclaradas": 1
}
```

## 8.2 Campos que no deben duplicarse en `CabinetIndexSeed.Payload`

Estos datos ya tienen lugar en el contrato comun y no deben repetirse en el payload especializado:

| Dato | Donde debe ir |
|---|---|
| Radicado comun | `Inventario.Radicado` |
| Tipo documental | `Trd.IdTipoDocumento`, `Trd.NombreTipoDocumento` |
| Area, serie, subserie | `Trd` |
| Expediente explicito | `Expediente.IdExpediente` |
| Clase documental | `Expediente.IdClaseDocumento`, `Expediente.ClaseDocumento` |
| Workflow | `Workflow.IdTareaWorkflow`, `Workflow.IdRuta` |
| Respuesta radicado | `AnexoRespuesta.IdRespuestaRadicado` |

## 9. `AnexoRespuesta.NombreArchivo`

`AnexoRespuesta.NombreArchivo` debe venir del frontend y representa el nombre original del archivo seleccionado por el usuario.

Reglas:

- Enviar solo el nombre, no la ruta local.
- No enviar `C:\...`, `/...` ni subdirectorios.
- Puede superar 120 caracteres; backend lo trunca a 120 antes de persistir.
- No enviar vacio o solo espacios.
- No enviar caracteres invalidos de nombre de archivo.

El backend no usa como fallback `DIG00000000.pdf`, porque ese nombre es fisico interno de gabinete y no representa el nombre visto por el usuario.

## 10. Metadata Opcional de Gabinete

`Trd` y `Expediente` son metadata opcional del gabinete. Que el gabinete tenga columnas como `ID_AREA`, `ID_SERIE`, `ID_EXPEDIENTE`, `EXPEDIENTE`, `ID_UNIDAD_CONSERVACION` o `UNIDADCONSERVA` no significa que el frontend este obligado a enviar valores.

### 10.1 `Trd`

| Campo | Destino legacy | Requerido |
|---|---|---|
| `IdArea` | `ID_AREA` | No |
| `IdSerie` | `ID_SERIE` | No |
| `IdSubSerie` | `ID_SUB_SERIE` | No |
| `IdTipoDocumento` | `ID_TIPODOCUMENTO` | No |
| `NombreSerie` | `NOMBRESERIE` | No |
| `NombreSubSerie` | `NOMBRESUBSERIE` | No |
| `NombreTipoDocumento` | `TIPODOCUMENTO` | No |

Si el usuario no selecciona TRD, el frontend puede enviar `Trd: null`.

### 10.2 `Expediente`

| Campo | Destino legacy | Requerido |
|---|---|---|
| `IdExpediente` | `ID_EXPEDIENTE` | No |
| `NombreExpediente` | `EXPEDIENTE` | No |
| `IdTipoExpediente` | `ID_TIPO_EXPEDIENTE` | No |
| `IdUnidadConservacion` | `ID_UNIDAD_CONSERVACION` | No |
| `NombreUnidadConservacion` | `UNIDADCONSERVA` | No |
| `IdTipoUnidadConservacion` | `ID_TIPO_UNIDAD_CONSERVACION` | No |
| `IdTipoUnidadDocumental` | `ID_TIPO_UNIDAD_DOCUMENTAL` | No |
| `IdClaseDocumento` | `ID_CLASE_DOCUMENTO` | Solo si se envia expediente o unidad |
| `ClaseDocumento` | `CLASEDOCUMENTO` | No |

Reglas:

- `Expediente` puede ser `null`.
- Todos los campos internos pueden ser `null`.
- `IdExpediente` e `IdUnidadConservacion` pueden venir juntos.
- Si viene `IdExpediente`, enviar tambien `NombreExpediente` cuando el front lo tenga.
- Si viene `IdUnidadConservacion`, enviar tambien `NombreUnidadConservacion` cuando el front lo tenga.
- Si viene `IdExpediente` o `IdUnidadConservacion`, `IdClaseDocumento` debe venir mayor a cero.

## 11. Respuesta Esperada del Almacenamiento Final

```json
{
  "success": true,
  "message": "Documento almacenado correctamente",
  "data": {
    "IdAlmacen": 9931,
    "IdRegistroProduccionDocumental": 5001,
    "NombreArchivoFinal": "DIG00009931.pdf",
    "RequestId": "SCRUM-250-E2E-20260603-001",
    "Estado": "Completed"
  },
  "meta": {
    "Status": "success",
    "RequestId": "SCRUM-250-E2E-20260603-001"
  },
  "errors": []
}
```

Cuando `AnexoRespuesta` viene informado y la transaccion confirma, debe existir una fila en `ra_anexos_respuesta` con:

| Columna | Valor |
|---|---|
| `ra_respuesta_radicado_ID_RESPUESTA_RADICADO` | `AnexoRespuesta.IdRespuestaRadicado` |
| `id_imagen_gabinete` | `data.IdAlmacen` |
| `nombre_gabinete` | `NombreGabinete` |
| `nombre_archivo` | `AnexoRespuesta.NombreArchivo` normalizado/truncado |

## 12. Limpieza Temporal

Despues de almacenamiento exitoso:

- El backend consume el archivo temporal.
- La fase fisica ejecuta limpieza del upload temporal completado.
- El frontend no debe reutilizar el mismo `RutaTemporalId`/`ArchivoTemporalId` para otro almacenamiento.
- Si el almacenamiento final falla antes de consumir el archivo, el temporal puede quedar disponible para diagnostico/reintento segun la politica del backend.

## 13. Errores Esperados

| Codigo | HTTP | `meta.status` | Causa comun |
|---|---:|---|---|
| `STORAGE_ANEXO_RESPUESTA_INVALID` | 400 | `validation` | ID respuesta invalido, gabinete invalido, nombre archivo vacio o ruta. |
| `STORAGE_ANEXO_RESPUESTA_NOT_FOUND` | 404 | `business` | No existe `ra_respuesta_radicado`. |
| `STORAGE_ANEXO_RESPUESTA_INACTIVE` | 409 | `business` | Respuesta padre no elegible. |
| `STORAGE_ANEXO_RESPUESTA_DUPLICATED` | 409 | `business` | Ya existe relacion para respuesta + documento + gabinete. |
| `STORAGE_ANEXO_RESPUESTA_FK_INVALID` | 409 | `business` | Fallo de llave foranea. |
| `STORAGE_ANEXO_RESPUESTA_INSERT_FAILED` | 500 | `error` | Fallo al insertar relacion. |
| `TemporaryUploadFailed` | 400 | `validation/error` | Init, chunk, status o complete fallo. |

## 14. Checklist Frontend

- Obtener JWT vigente antes de iniciar.
- Calcular `TamanoBytes`, `NumeroChunks` y, si aplica, `HashSha256Esperado`.
- Ejecutar `init` y conservar `RutaTemporalId`/`ArchivoTemporalId`.
- Enviar chunks con `Content-Type: application/octet-stream`.
- Enviar `X-Total-Chunks` en cada chunk.
- Consultar `status` hasta que no haya pendientes.
- Ejecutar `complete`.
- Usar `RutaTemporalId` y `ArchivoTemporalId` devueltos por `init` en el almacenamiento final.
- Enviar `AnexoRespuesta` solo si se quiere crear relacion con respuesta.
- Enviar `AnexoRespuesta.NombreArchivo` desde el archivo seleccionado por el usuario.
- No enviar rutas locales completas.
- Guardar `RequestId` para soporte.
