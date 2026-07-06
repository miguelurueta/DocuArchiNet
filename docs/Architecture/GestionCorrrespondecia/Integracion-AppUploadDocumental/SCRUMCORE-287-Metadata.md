# SCRUMCORE-287 - Metadata Tecnica

## Identificacion

| Campo | Valor |
| --- | --- |
| SCRUMCORE ID | SCRUMCORE-287 |
| Nombre | Configuracion upload para adjuntos en Gestion Correspondencia |
| Modulo | Gestion Correspondencia |
| Submodulo | Gestion Respuesta |
| Alcance | Frontend |
| Estado | Implementado |
| Backend modificado | No |
| Endpoints modificados | No |
| Componentes shared reemplazados | No |

## Objetivo Funcional

Consumir la configuracion de upload desde backend para el proceso `CORRESPO` y aplicar esas reglas al flujo de adjuntos de Gestion Respuesta.

La configuracion externa define:

- extensiones permitidas;
- tamano maximo funcional por archivo;
- estado activo de la configuracion.

## Alcance Implementado

| Area | Resultado |
| --- | --- |
| Tipos | Se agregaron tipos especificos para configuracion upload CORRESPO. |
| Servicio | Se creo servicio tipado que consume `configuracion-upload`. |
| Hook | Se creo hook reusable con loading, error, empty y reload. |
| Integracion | `loadGestionRespuestaUploadConfig` delega en el nuevo servicio. |
| UI documental | Se preserva `AppUploadDocumental` y `AppUploadBatchView`. |
| Validacion | Se mantiene fail-closed si backend no entrega configuracion usable. |
| Pruebas | Se agregaron pruebas focales de servicio, hook e integracion. |
| Documentacion | Se documenta contrato, flujo, estados y evidencia. |

## Fuera De Alcance

- No implementa tipologias documentales.
- No modifica el endpoint de tipologias.
- No modifica backend.
- No implementa upload por chunks nuevo.
- No cambia el contrato final de almacenamiento documental.
- No reemplaza componentes shared.
- No introduce `any`.

## Endpoints Involucrados

### Configuracion Upload

```http
GET /api/gestor-documental/configuracion-upload?nameProceso=CORRESPO
```

Responsabilidad:

- entregar extensiones permitidas;
- entregar tamano maximo funcional;
- indicar configuracion activa.

### Tipologias Workflow

```http
GET /api/gestor-documental/tipologias-documentales?Contexto=WORKFLOW&IdTareaWf={idTareaWf}&IdRutaWf={idRutaWf}
```

Responsabilidad:

- alimentar dropdown de tipologia por archivo;
- no forma parte del alcance directo de SCRUMCORE-287, pero convive en la misma UI.

### Almacenamiento Temporal

```http
POST /api/gestor-documental/almacenamiento/upload-temporal/init
PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status
POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
DELETE /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}
```

Responsabilidad:

- transferir archivo por chunks;
- cancelar temporales cuando aplica;
- validar estado antes de completar.

### Almacenamiento Final

```http
POST /api/gestor-documental/almacenamiento
```

Responsabilidad:

- registrar documento en gabinete;
- asociar inventario, TRD, workflow y anexo de respuesta.

## Archivos Frontend

### Nuevos

```txt
src/modules/gestionCorrespondencia/types/configuracionUploadCorrespondencia.types.ts
src/modules/gestionCorrespondencia/services/configuracionUploadCorrespondencia.service.ts
src/modules/gestionCorrespondencia/hooks/useConfiguracionUploadCorrespondencia.ts
src/modules/gestionCorrespondencia/tests/configuracionUploadCorrespondencia.service.test.ts
src/modules/gestionCorrespondencia/tests/useConfiguracionUploadCorrespondencia.test.tsx
src/modules/gestionCorrespondencia/tests/gestionRespuestaUploadDocumental.service.test.ts
```

### Modificados Relevantes

```txt
src/modules/gestionCorrespondencia/services/gestionRespuestaUploadDocumental.service.ts
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx
src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.tsx
src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.types.ts
src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.ts
src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts
src/app/Components/UI/AppUploadBatchView/AppUploadBatchView.tsx
```

## Contratos TypeScript

```ts
export type ConfiguracionUploadCorrespondencia = {
  nameProceso: "CORRESPO";
  accept: string;
  allowedExtensions: string[];
  maxSizeBytes: number;
};
```

```ts
export type UploadDocumentalStoredContext = {
  source: "single" | "batch";
  remainingFiles: number;
};
```

```ts
export type UploadDocumentalBatchSummary = {
  total: number;
  stored: number;
  failed: number;
  skipped: number;
  cancelled: number;
  remainingFiles: number;
  results: AlmacenarDocumentoStoredResult[];
};
```

## Reglas De Cierre Del Modal

| Caso | Resultado |
| --- | --- |
| Guardar uno y no quedan archivos | Refresca documentos y cierra. |
| Guardar uno y quedan archivos | Refresca documentos y mantiene abierto. |
| Guardar todo con todos exitosos | Refresca documentos y cierra. |
| Guardar todo con archivo pendiente/sin tipologia/cancelado | Refresca documentos si hubo guardados y mantiene abierto. |
| Cancelacion global | Aborta archivo activo, conserva pendientes y mantiene abierto. |
| Cancelacion unitaria | Aborta solo ese archivo, lo deja reintentable. |

## Evidencia

Pruebas focales ejecutadas durante el cierre:

```bash
npm.cmd test -- --run src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.test.ts src/app/Components/UI/AppUploadBatchView/AppUploadBatchView.test.tsx
```

```bash
npm.cmd test -- --run src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.test.ts
```

Validacion OpenSpec:

```bash
openspec validate scrumcore-287-configuracion-upload-gestioncorrespondencia --strict
```
