# SCRUMCORE-287 - Configuracion Upload Adjuntos CORRESPO

## Metadata

- Ticket: SCRUMCORE-287
- Modulo: Gestion Correspondencia / Gestion Respuesta
- Fecha: 2026-07-03
- Alcance: frontend
- Estado documentado: implementacion local
- Backend modificado: no
- Endpoints modificados: no
- Componentes shared reemplazados: no

## Documentos Relacionados

- `SCRUMCORE-287-Metadata.md`: metadata tecnica, alcance, contratos y archivos tocados.
- `SCRUMCORE-287-Diagrama-Secuencia.md`: diagramas de secuencia Mermaid para configuracion, guardado, lote y cancelacion.
- `SCRUMCORE-287-Flujo-API-Estados.md`: detalle de APIs consumidas, estados de archivo, cierre de modal, cancelacion y retry.

## Objetivo

Reforzar el flujo de adjuntos documentales ya integrado con `AppUploadDocumental` para que las reglas finales de seleccion de archivos salgan de backend y no de constantes locales.

El flujo actual se conserva:

```txt
GestionRespuestaUploadDocumental
  -> loadGestionRespuestaUploadConfig
  -> AppUploadDocumental
  -> AppUploadBatchView / AppUpload
```

SCRUMCORE-287 solo cambia la fuente final de:

- extensiones permitidas;
- tamano maximo por archivo.

## Endpoint Consumido

```http
GET /api/gestor-documental/configuracion-upload?nameProceso=CORRESPO
```

El consumo queda encapsulado en:

```txt
src/modules/gestionCorrespondencia/services/configuracionUploadCorrespondencia.service.ts
```

El componente React no consume `clienteApi` directamente.

## Contrato Backend

Respuesta esperada:

```ts
type ConfiguracionUploadCorrespondenciaResponse = {
  success: boolean;
  message: string;
  data: ConfiguracionUploadCorrespondenciaBackendItem[];
  meta?: unknown;
  errors?: unknown[];
};
```

Se soportan campos PascalCase y camelCase:

```ts
type ConfiguracionUploadCorrespondenciaBackendItem = {
  IdConfigUploadGestion?: number;
  ExtensionUpload?: string;
  LengUpload?: number;
  NameProceso?: string;
  EstadoProceso?: number;
  idConfigUploadGestion?: number;
  extensionUpload?: string;
  lengUpload?: number;
  nameProceso?: string;
  estadoProceso?: number;
};
```

## Mapeo FE-BE

| Backend | Frontend | Regla |
| --- | --- | --- |
| `ExtensionUpload` / `extensionUpload` | `allowedExtensions` | Separar por coma, trim, lowercase, agregar punto si falta, eliminar vacios y duplicados. |
| `ExtensionUpload` / `extensionUpload` | `accept` | `allowedExtensions.join(",")`. |
| `LengUpload` / `lengUpload` | `maxSizeBytes` | Debe ser numero mayor que cero. |
| `EstadoProceso` / `estadoProceso` | seleccion de fila | Usar primera fila activa con valor `1`; si no hay activa, usar la primera fila valida. |
| `success=false` | error funcional | No habilitar seleccion. |
| `data=[]` | empty/fail-closed | No habilitar seleccion. |

Ejemplo:

```txt
.PDF,.DOC,.DOCX,.ZIP,.XLS,.XLSX
```

se normaliza a:

```txt
.pdf,.doc,.docx,.zip,.xls,.xlsx
```

## Integracion Con AppUploadDocumental

El loader existente:

```txt
src/modules/gestionCorrespondencia/services/gestionRespuestaUploadDocumental.service.ts
```

delega ahora en:

```ts
getConfiguracionUploadCorrespondencia()
```

El resultado se transforma a `UploadDocumentalConfig` conservando las reglas documentales ya existentes:

```ts
multiple: true
requiereTipologia: true
requiereFechaCarga: false
fechaCargaObligatoria: false
validationMode: "queue-with-error"
```

Los valores de backend reemplazan la fuente hardcodeada anterior:

```ts
accept
allowedExtensions
maxSizeBytes
```

## Transporte Por Chunks

SCRUMCORE-287 tambien deja documentada la diferencia entre dos limites que no deben mezclarse:

| Concepto | Fuente | Uso |
| --- | --- | --- |
| `LengUpload` / `maxSizeBytes` | Backend `configuracion-upload` | Tamano maximo funcional permitido para seleccionar/almacenar el archivo. |
| `preferredChunkSizeBytes` | Configuracion documental existente | Tamano inicial que se declara al iniciar la carga temporal. |
| `ChunkSizeBytes` | Respuesta backend de `upload-temporal/init` | Tamano de chunk sugerido por backend para la transferencia. |
| `storageOptions.maxChunkSizeBytes` | Configuracion tecnica del flujo frontend | Tope superior aplicado por el frontend para evitar resets de conexion en ambientes IIS/proxy. |

El servicio de almacenamiento resuelve el tamano efectivo asi:

```ts
effectiveChunkSizeBytes = Math.min(backendChunkSizeBytes, maxChunkSizeBytes);
```

Si `maxChunkSizeBytes` no se define, se mantiene el comportamiento historico: el frontend usa directamente el `ChunkSizeBytes` retornado por backend.

Para Gestion Respuesta se configura:

```ts
storageOptions={{
  backendPayloadCase: "pascal",
  validateStatusBeforeComplete: true,
  maxChunkSizeBytes: 4 * 1024 * 1024,
}}
```

Razon tecnica:

- en pruebas locales con archivos grandes, backend devolvio `ChunkSizeBytes=10485760` (10 MB);
- el primer `PUT /chunk/0` fallo con `net::ERR_CONNECTION_RESET`;
- el frontend no recibio respuesta HTTP, por lo que la falla ocurre durante transporte, antes de que backend pueda devolver un error funcional;
- reducir el chunk efectivo a 4 MB mantiene el contrato de chunks y evita depender de una constante oculta dentro del servicio shared.

Este tope no cambia `LengUpload`, no reduce el tamano maximo permitido del archivo y no modifica backend.

## Progreso Visual Del Almacenamiento

El progreso del modal no representa solamente la transferencia de chunks. Para evitar que la UI marque 100% mientras backend todavia esta completando o almacenando, el frontend reserva rangos por fase:

| Fase | Rango visual | Descripcion |
| --- | --- | --- |
| `initializing` | 0% - 5% | Creacion de carga temporal. |
| `uploading` | 5% - 82% | Transferencia de chunks. |
| `completing` | 84% - 92% | Validacion y cierre del archivo temporal. |
| `storing` | 94% - 98% | Registro documental en gabinete, inventario, TRD/workflow/anexo. |
| `Actualizando documentos` | 99% | Ejecucion de `onStored`, refresh de documentos y cierre del flujo consumidor. |
| `Guardado` | 100% | Archivo confirmado como almacenado para la UI. |

Esto mantiene alineado el feedback visual con el proceso real:

```txt
init -> chunks -> status -> complete -> almacenamiento -> refreshDocumentos/onClose
```

El cambio se implementa en `useAppUploadDocumentalActions`, no en `AppProgressBatch`, porque las fases son propias del dominio documental.

## Prevalidacion De Lote

`Guardar todo` ejecuta una validacion previa antes de abrir el modal de progreso y antes de iniciar `upload-temporal/init`.

La validacion cubre:

- archivo existente en cola;
- extension permitida;
- tamano maximo funcional (`maxSizeBytes`);
- tipologia requerida (`metadata.idTipoDocumento`);
- fecha documental requerida cuando aplique.

Razon:

- backend exige `StorageTrd` para `RADICACION` cuando el flujo usa `CabinetIndexSeed`;
- si falta tipologia, backend responde `CABIDX_SEED_INVALID` en `POST /api/gestor-documental/almacenamiento`;
- en archivos grandes, esperar hasta esa fase implica subir todos los chunks antes de descubrir que falta TRD;
- la UI debe marcar el archivo invalido en la cola y no consumir red/tiempo con un lote que ya se sabe incompleto.

Cuando algun archivo falla prevalidacion:

- no se abre `AppProgressBatch`;
- no se llama `uploadAndStoreOneDocument`;
- el archivo queda en `state="error"`;
- el dropdown de tipologia queda marcado en error sin mostrar mensajes duplicados dentro del article.

## Cierre Del Modal En Guardar Todo

El flujo distingue el origen del evento `onStored`:

```ts
type UploadDocumentalStoredContext = {
  source: "single" | "batch";
};
```

Regla:

- `source="single"`: Gestion Respuesta puede refrescar documentos y cerrar el modal inmediatamente.
- `source="batch"`: Gestion Respuesta no cierra por cada archivo almacenado; espera `onBatchComplete`.

Razon:

- en lotes mixtos, un archivo pequeno puede terminar antes que un archivo grande;
- si el modal se cierra por el primer `onStored`, React desmonta el upload y aborta el archivo grande que sigue en curso;
- al abortar, el frontend intenta limpiar el temporal con `DELETE upload-temporal`;
- backend puede rechazar la cancelacion porque el archivo `.part.tmp` todavia esta bloqueado por el proceso de escritura;
- el resultado visible es un `STO-TMP-UPLOAD-FAILED` en `uploadTemporal.cancel`, aunque la causa real es el cierre anticipado del lote.

Con la separacion por origen:

```txt
Guardar uno:
  stored -> refreshDocumentos -> onClose

Guardar todo:
  item stored -> continuar lote
  batch complete -> refreshDocumentos -> onClose
```

Esto evita abortar cargas grandes activas por el cierre de un item pequeno ya almacenado.

## Hook Reusable

Se crea:

```txt
src/modules/gestionCorrespondencia/hooks/useConfiguracionUploadCorrespondencia.ts
```

Expone:

```ts
{
  config?: ConfiguracionUploadCorrespondencia;
  loading: boolean;
  error?: string;
  empty: boolean;
  reload: () => Promise<void>;
}
```

El hook permite que otras pantallas simples con `AppUpload` puedan usar la misma configuracion sin duplicar logica.

## Estados UI

### Loading

Mientras se carga la configuracion, la seleccion debe permanecer deshabilitada.

### Error

Si backend responde error funcional o la respuesta no es usable, la seleccion queda bloqueada. El mensaje se normaliza desde `errors[0].UserMessage`, `message` o fallback funcional.

### Empty

Si `success=true` pero `data=[]`, se trata como configuracion no disponible para `CORRESPO` y se aplica fail-closed.

### Retry

El hook expone `reload`. En el flujo actual, `AppUploadDocumental` recarga mediante su ciclo de loader cuando cambia el contexto o se vuelve a montar el componente.

## Restricciones Cumplidas

- No se modifico backend.
- No se inventaron endpoints.
- No se cambio el endpoint de tipologias.
- No se mezclo con metadata por archivo.
- No se modifico `renderMetadata`.
- No se cambio el flujo `init -> chunks -> status -> complete -> almacenamiento`; solo se agrego un tope tecnico opcional al tamano efectivo del chunk.
- No se reemplazo `AppUploadDocumental`.
- No se reemplazo `AppUploadBatchView`.
- No se reemplazo `AppUpload`.
- No se introdujo `any`.

## Archivos Implementados

```txt
src/modules/gestionCorrespondencia/types/configuracionUploadCorrespondencia.types.ts
src/modules/gestionCorrespondencia/services/configuracionUploadCorrespondencia.service.ts
src/modules/gestionCorrespondencia/hooks/useConfiguracionUploadCorrespondencia.ts
src/modules/gestionCorrespondencia/services/gestionRespuestaUploadDocumental.service.ts
src/modules/gestionCorrespondencia/tests/configuracionUploadCorrespondencia.service.test.ts
src/modules/gestionCorrespondencia/tests/useConfiguracionUploadCorrespondencia.test.tsx
src/modules/gestionCorrespondencia/tests/gestionRespuestaUploadDocumental.service.test.ts
src/modules/gestionCorrespondencia/tests/GestionRespuestaUploadDocumental.test.tsx
src/modules/almacenamientoDocumental/types/almacenamientoDocumental.types.ts
src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.types.ts
src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.ts
src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts
src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.test.ts
```

## Pruebas

### Ejecutadas

```bash
npm test -- --run src/modules/gestionCorrespondencia/tests/configuracionUploadCorrespondencia.service.test.ts src/modules/gestionCorrespondencia/tests/useConfiguracionUploadCorrespondencia.test.tsx src/modules/gestionCorrespondencia/tests/gestionRespuestaUploadDocumental.service.test.ts src/modules/gestionCorrespondencia/tests/GestionRespuestaUploadDocumental.test.tsx
```

Resultado:

```txt
Test Files 4 passed (4)
Tests 18 passed (18)
```

```bash
npm test -- --run src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.test.ts src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.test.ts src/modules/gestionCorrespondencia/tests/GestionRespuestaUploadDocumental.test.tsx
```

Resultado:

```txt
Test Files 3 passed (3)
Tests 23 passed (23)
```

Despues del ajuste de progreso visual:

```bash
npm test -- --run src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.test.ts src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.test.ts src/modules/gestionCorrespondencia/tests/GestionRespuestaUploadDocumental.test.tsx
```

Resultado:

```txt
Test Files 3 passed (3)
Tests 24 passed (24)
```

Despues del ajuste de prevalidacion de lote:

```bash
npm test -- --run src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalState.test.ts src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.test.ts src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.test.ts src/modules/gestionCorrespondencia/tests/GestionRespuestaUploadDocumental.test.tsx
```

Resultado:

```txt
Test Files 4 passed (4)
Tests 27 passed (27)
```

Despues del ajuste de cierre por lote:

```bash
npm test -- --run src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.test.ts src/modules/gestionCorrespondencia/tests/GestionRespuestaUploadDocumental.test.tsx src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.test.ts
```

Resultado:

```txt
Test Files 3 passed (3)
Tests 26 passed (26)
```

Cobertura focal agregada:

- conserva el uso de `ChunkSizeBytes` de backend cuando no existe `maxChunkSizeBytes`;
- aplica `maxChunkSizeBytes` cuando Gestion Respuesta necesita dividir chunks mas pequenos;
- mantiene `X-Total-Chunks` coherente con el tamano efectivo;
- verifica que `GestionRespuestaUploadDocumental` entregue `storageOptions.maxChunkSizeBytes` a `AppUploadDocumental`.
- mantiene el progreso visual por debajo de 100% hasta que termina almacenamiento y se ejecuta la actualizacion documental.
- valida tipologia requerida antes de abrir `Guardar todo`.
- evita llamadas backend cuando el lote tiene archivos incompletos.
- evita cerrar el modal por `onStored` de un item individual cuando el guardado viene de lote.
- cierra Gestion Respuesta despues de `onBatchComplete` cuando al menos un archivo fue almacenado.

```bash
openspec validate scrumcore-287-configuracion-upload-gestioncorrespondencia --strict
```

Resultado:

```txt
Change 'scrumcore-287-configuracion-upload-gestioncorrespondencia' is valid
```

```bash
npm run build
```

Resultado:

```txt
Falla por errores TypeScript preexistentes fuera del alcance de SCRUMCORE-287.
No se reportaron errores en los archivos nuevos o modificados de configuracion upload CORRESPO.
```

Errores externos observados:

- `src/app/Components/UI/AppDigitalizador/index.ts`
- `src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.ts`
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx`
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts`
- `src/modules/radicacion/components/CapDocument.tsx`

## Limites Conocidos

- Si el endpoint no esta desplegado en el ambiente, el upload queda bloqueado por fail-closed.
- Si backend configura un tamano alto, el frontend lo aceptara y el flujo por chunks existente sera el responsable de transferir el archivo.
- La seleccion de tipologias sigue siendo responsabilidad de SCRUMCORE-284.
- El almacenamiento documental final sigue siendo responsabilidad del flujo ya implementado de `AppUploadDocumental`.
