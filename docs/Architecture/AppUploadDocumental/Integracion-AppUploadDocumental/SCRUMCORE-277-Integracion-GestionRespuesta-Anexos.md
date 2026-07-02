# SCRUMCORE-277 - Integracion AppUploadDocumental en Gestion Respuesta

## 1. Estado Del Documento

Estado: implementacion aplicada con documentacion enterprise actualizada.

Este documento acompana el cambio OpenSpec:

```text
openspec/changes/scrumcore-277-implementacion-appuploaddocumental-gestionrespuesta
```

Commit de implementacion:

```text
14a4900 feat(SCRUMCORE-277): integrar AppUploadDocumental en Gestion Respuesta
```

La implementacion integra la carga documental moderna en el tab Gestion de Gestion Respuesta, usando `AppUploadDocumental` como experiencia especializada y `StorageEngineV2` como flujo tecnico de almacenamiento.

## 2. Objetivo Del Ticket

Permitir que el usuario cargue uno o varios archivos desde Gestion Respuesta y los almacene como anexos asociados a una respuesta de radicado.

El flujo funcional cubre:

- abrir una experiencia de carga documental desde el tab Gestion;
- seleccionar o arrastrar archivos;
- validar extension y tamano;
- exigir tipologia documental por archivo;
- guardar individualmente o guardar todos;
- ejecutar `init -> chunks -> status -> complete -> almacenar`;
- construir el request final de anexo workflow;
- refrescar el listado documental desde backend;
- permitir que el documento persistido aparezca en el tab Documentos cuando el backend lo retorne.

## 3. Fuente Contractual Oficial

Para este ticket se toma como contrato oficial el documento:

```text
docs/Architecture/AppUploadDocumental/SCRUM-250-Integracion-Frontend-AnexosRespuesta.md
```

Tambien aplica el prompt de SCRUMCORE-277 en Jira.

Decision contractual:

```text
Tipos internos frontend en camelCase
-> mapper explicito a payload backend PascalCase cuando el flujo lo requiere
-> respuesta backend PascalCase/camelCase tolerada por guards runtime
-> resultado frontend normalizado en camelCase
```

No se modifico backend. La estructura final enviada se adapto desde frontend mediante mapper especializado del modulo.

## 4. Arquitectura De Integracion

```text
GestionRespuesta
  -> GestionRespuestaDocumentosProvider
     -> GestionRespuestaMainTabContent
        -> GestionRespuestaUploadDocumentalModal
           -> GestionRespuestaUploadDocumental
              -> AppUploadDocumental
                 -> AppUploadBatchView
                 -> AppUpload
                 -> AppProgressBatch
              -> buildGestionRespuestaAlmacenarDocumentoRequest
              -> loadGestionRespuestaUploadConfig
              -> loadGestionRespuestaTiposDocumentales
              -> almacenamientoDocumentalUpload.service
     -> DocumentosWorkbench
        -> documentosRefreshKey
        -> AppTreeTable
        -> AppVisorEmbedPdf
```

Responsabilidades por capa:

| Capa | Responsabilidad |
|---|---|
| `GestionRespuestaUploadDocumentalModal` | Disparador visual y contenedor modal para la carga documental desde el tab Gestion. |
| `GestionRespuestaUploadDocumental` | Adapter del modulo. Lee contexto, conecta loaders, arma request final y dispara refresh del Workbench. |
| `AppUploadDocumental` | Experiencia reusable de carga documental: cola, metadata, tipologia, preview opcional, guardar uno, guardar todos, progreso y errores. |
| `AppUploadBatchView` | Vista reusable de lote: toolbar, cola, acciones por archivo, footer y panel de preview. No conoce reglas documentales. |
| `AppUpload` | Selector/drag-drop de archivos. |
| `almacenamientoDocumentalUpload.service` | Cliente tecnico StorageEngineV2. No depende de UI. |
| `gestionRespuestaUploadDocumental.mapper` | Construye payload de almacenamiento para anexo de Gestion Respuesta. |
| `GestionRespuestaDocumentosProvider` | Fuente compartida de contexto documental y `refreshDocumentos`. |
| `DocumentosWorkbench` | Recarga listado documental cuando cambia `documentosRefreshKey`. |

## 5. Archivos Implementados O Actualizados

### Gestion Correspondencia

```text
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumentalModal.tsx
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.module.css
src/modules/gestionCorrespondencia/adapters/gestionRespuestaUploadDocumental.mapper.ts
src/modules/gestionCorrespondencia/services/gestionRespuestaUploadDocumental.service.ts
src/modules/gestionCorrespondencia/tests/GestionRespuestaUploadDocumental.test.tsx
src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx
```

### Almacenamiento Documental / UI Reusable

```text
src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.tsx
src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.module.css
src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.test.tsx
src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.ts
src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalState.ts
src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts
src/modules/almacenamientoDocumental/types/almacenamientoDocumental.types.ts
src/modules/almacenamientoDocumental/utils/tipoDocumentalSuggestion.utils.ts
src/app/Components/UI/AppUploadBatchView/AppUploadBatchView.tsx
src/app/Components/UI/AppUploadBatchView/AppUploadBatchView.module.css
src/app/Components/UI/AppUpload/AppUpload.tsx
src/app/Components/UI/AppButton/AppButton.module.css
```

### Documentacion

```text
docs/Architecture/AppUploadDocumental/SCRUMCORE-277-Integracion-GestionRespuesta-Anexos.md
```

## 6. Flujo End-To-End

1. El usuario entra a Gestion Respuesta.
2. `GestionRespuestaDocumentosProvider` expone `nombreGabinete`, `radicado`, `idTareaWf`, `idRespuestaRadicado` y `refreshDocumentos`.
3. El tab Gestion muestra el disparador `Adjuntar documentos`.
4. El usuario hace click en el disparador.
5. Se abre `GestionRespuestaUploadDocumentalModal`.
6. Dentro del modal se renderiza `GestionRespuestaUploadDocumental`.
7. El adapter construye el contexto de `AppUploadDocumental`:

```ts
{
  nombreGabinete,
  idTareaWorkflow: idTareaWf,
  idRespuesta: idRespuestaRadicado,
  nameModulo: radicado
}
```

8. `AppUploadDocumental` carga configuracion y tipologias mediante loaders.
9. El usuario arrastra o selecciona archivos.
10. La cola crea un item por archivo y conserva metadata independiente.
11. El usuario selecciona tipologia por archivo.
12. Al guardar uno o todos, se valida metadata requerida.
13. Por cada archivo valido se ejecuta el cliente StorageEngineV2:

```text
POST upload-temporal/init
-> PUT upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
-> GET upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status
-> POST upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
-> POST /api/gestor-documental/almacenamiento
```

14. El mapper construye un request final por archivo.
15. Si la respuesta confirma `AnexoRespuesta.Created === true`, se llama `refreshDocumentos()`.
16. `DocumentosWorkbench` recarga desde backend mediante `documentosRefreshKey`.
17. Si el backend retorna el nuevo documento, queda disponible en el listado y puede abrirse en el visor PDF embebido del tab Documentos.

## 7. Endpoints Consumidos

### StorageEngineV2

| Paso | Metodo | Endpoint |
|---|---|---|
| Init temporal | `POST` | `/api/gestor-documental/almacenamiento/upload-temporal/init` |
| Subir chunk | `PUT` | `/api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}` |
| Status temporal | `GET` | `/api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status` |
| Complete temporal | `POST` | `/api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete` |
| Cancel temporal | `DELETE` | `/api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}` |
| Almacenamiento final | `POST` | `/api/gestor-documental/almacenamiento` |

Reglas aplicadas:

- todos los requests pasan por `clienteApi`;
- JWT lo maneja la configuracion central de `clienteApi`;
- chunks se envian como bytes crudos;
- `Content-Type` de chunk: `application/octet-stream`;
- header `X-Total-Chunks` obligatorio;
- `chunkIndex` inicia en `0`;
- se consulta status antes de complete;
- se bloquea complete si existen chunks pendientes;
- no se reutilizan temporales despues de exito.

### Tipologias Del Ticket Actual

En el estado actual de SCRUMCORE-277, `loadGestionRespuestaTiposDocumentales` entrega una opcion semilla:

```ts
[
  {
    idTipoDocumento: 43,
    nombreTipoDocumento: "Comprobante De Egreso",
  },
]
```

Esto permite validar el flujo completo de UI, metadata por archivo, mapper TRD y almacenamiento.

El prompt posterior de tipologias por adjunto complementa esta parte y debe reemplazar el loader semilla por:

```text
GET /api/gestor-documental/tipologias-documentales
Query:
  Contexto=WORKFLOW
  IdTareaWf={idTareaWf}
  IdRutaWf={idRutaWf}
```

Reglas del complemento:

- no resolver `IdTipoTramite` en frontend;
- no enviar `IdTipoTramite=0`;
- cargar una vez por contexto workflow;
- mapear `Id` a `idTipoDocumento`;
- mapear `Descripcion` a `nombreTipoDocumento`;
- conservar seleccion independiente por archivo.

## 8. Request Final De Anexo Workflow

El request final se construye por archivo desde `buildGestionRespuestaAlmacenarDocumentoRequest`.

Estructura funcional:

```json
{
  "NombreGabinete": "CORRESPO",
  "RutaTemporalId": "usr_141_...",
  "NombreDocumento": "Anexo workflow respuesta 2600466700021",
  "RequestId": "workflow-anexo-2600466700021-001",
  "Documentos": [
    {
      "IdDocumento": "wf-anexo-workflow-anexo-2600466700021-001",
      "ArchivoTemporalId": "af_89a11a7c81424d4e8f9bb5a821f6b901.pdf",
      "NombreOriginal": "soporte-respuesta.pdf",
      "Extension": ".pdf",
      "NumeroPaginas": null
    }
  ],
  "Trd": {
    "IdTipoDocumento": 43,
    "NombreTipoDocumento": "Comprobante De Egreso"
  },
  "Workflow": {
    "IdTareaWorkflow": 933,
    "IdRutaWorkflow": null
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
    "NombreArchivo": "soporte-respuesta.pdf",
    "TipoAdjunto": "respuesta",
    "Observacion": "Anexo cargado desde workflow"
  },
  "NumeroPaginasDeclaradas": null
}
```

Nota tecnica: internamente se modela camelCase y el servicio puede serializar PascalCase con `storageOptions.backendPayloadCase = "pascal"`.

## 9. Matriz Campo A Campo FE/BE

| Backend | Fuente frontend | Regla |
|---|---|---|
| `NombreGabinete` | `nombreGabinete` del provider | Requerido y no vacio. |
| `RutaTemporalId` | Response de init | Se inyecta luego del upload temporal. |
| `NombreDocumento` | Mapper Gestion Respuesta | `Anexo workflow respuesta {radicado/idTarea/fileName}`. |
| `RequestId` | Cliente por intento | Unico por intento; retry no reutiliza temporales. |
| `Documentos[0].IdDocumento` | Mapper | `wf-anexo-{requestId normalizado}`. |
| `Documentos[0].ArchivoTemporalId` | Response de init | Obligatorio en storage final. |
| `Documentos[0].NombreOriginal` | `file.name` | Se limpia cualquier ruta local. |
| `Documentos[0].Extension` | `normalizeFileExtension(fileName)` | Incluye punto, ejemplo `.pdf`. |
| `Documentos[0].NumeroPaginas` | Metadata del archivo | `number` positivo o `null`; nunca string. |
| `Trd.IdTipoDocumento` | Metadata `idTipoDocumento` | Requerido si `tipologiaObligatoria`. |
| `Trd.NombreTipoDocumento` | Metadata `nombreTipoDocumento` | Debe corresponder a la opcion elegida. |
| `Workflow.IdTareaWorkflow` | `idTareaWf` | Opcional si existe contexto workflow. |
| `Workflow.IdRutaWorkflow` | Contexto futuro `idRutaWf` | Pendiente de propagacion completa si el modulo lo entrega. |
| `CabinetIndexSeed.SourceModule` | Constante | `RADICACION`. |
| `CabinetIndexSeed.ProviderKey` | Constante | `RADICACION`. |
| `CabinetIndexSeed.Version` | Constante | `1.0.0`. |
| `CabinetIndexSeed.Payload.ModoResolucion` | Constante | `RespuestaRadicado`. |
| `AnexoRespuesta.IdRespuestaRadicado` | `idRespuestaRadicado` | Requerido, numerico y positivo. |
| `AnexoRespuesta.NombreArchivo` | `file.name` | Sin ruta local. |
| `AnexoRespuesta.TipoAdjunto` | Constante | `respuesta`. |
| `AnexoRespuesta.Observacion` | Mapper | `Anexo cargado desde workflow`. |
| `NumeroPaginasDeclaradas` | Metadata `numeroPaginas` | `number` positivo o `null`. |

## 10. Respuesta Backend Y Refresh

El mapper `normalizeWorkflowAnexoStorageResult` acepta respuesta envuelta o directa y valida:

- `Documento.IdAlmacen`;
- `Documento.IdRegistroProduccionDocumental`;
- `Documento.NombreArchivoFinal`;
- `AnexoRespuesta.IdAnexoRespuesta`;
- `AnexoRespuesta.IdRespuestaRadicado`;
- `AnexoRespuesta.IdAlmacen`;
- `AnexoRespuesta.NombreGabinete`;
- `AnexoRespuesta.NombreArchivo`;
- `AnexoRespuesta.Created === true`;
- `Indice.ProviderKey`;
- `Indice.Resolved`;
- `Indice.SourceTrace`;
- `Workflow.LogInserted`;
- `Workflow.IdTareaWorkflow`;
- `Workflow.IdRutaWorkflow`.

Regla de refresh:

```text
isWorkflowAnexoCreated(rawBackendResult) === true
-> GestionRespuestaUploadDocumental llama refreshDocumentos()
-> GestionRespuestaDocumentosProvider incrementa documentosRefreshKey
-> DocumentosWorkbench consulta backend nuevamente
```

No se inserta una fila local manual como fuente principal. El backend sigue siendo la fuente de verdad.

## 11. Experiencia Visual Implementada

### Modal

- El upload se abre en modal al hacer click en el disparador `Adjuntar documentos`.
- El modal usa `AppModal`, esta centrado y no usa footer propio.
- Ancho: `min(1040px, calc(100vw - 28px))`.
- `destroyOnHidden` evita retener estado cuando el modal se cierra.
- El body mantiene alto estable para que footer y contenido no salten.
- En desktop pequeno se habilita scroll vertical interno para que el visor PDF no se corte.
- En desktop grande no se fuerza scroll extra innecesario.
- El scroll usa estilo nativo del navegador, sin color personalizado.

### Disparador Visual Del Tab Gestion

- Boton full-width con borde dashed azul y fondo claro.
- Icono `InboxOutlined`.
- Texto principal: `Adjuntar documentos`.
- Texto secundario: `Haz click para cargar o arrastrar archivos en el modal.`
- Hover sutil en el disparador, sin alterar logica del tab.

### Toolbar Del Modal

- `AppUpload` queda alineado horizontalmente con `Guardar todo` y `Limpiar todo`.
- El dropzone se compacta a 32px de alto.
- Texto del upload: `Agregar archivos`.
- Se removieron transiciones pesadas del dragger para evitar sensacion de lentitud.
- Boton primario usa azul enterprise similar al inicio de sesion: `#2563eb`.
- Boton de limpieza usa rojo vivo: `#dc2626/#ef4444`.
- Los botones deshabilitados no aplican hover activo.
- El texto `Limpiar` se cambio a `Limpiar todo`.

### Cola De Archivos

- La cola vive arriba del visor.
- La cola tiene scroll propio para evitar crecimiento vertical excesivo.
- El panel de cola no muestra borde visible; mantiene contenido alineado.
- El header de cola es compacto.
- El texto de resumen usa `archivo(s)` en lugar de `pendiente(s)` cuando aplica.
- No hay scroll horizontal.

### Articles De Archivo

- Cada archivo se renderiza como `article` compacto.
- Layout por fila:

```text
nombre/tamano/estado | tipologia/fecha | acciones
mensaje/progreso debajo cuando aplica
```

- Padding reducido para disminuir alto visual.
- Alto maximo de fila reducido.
- Borde sobrio y moderno: `1.5px solid #cbd8e6`.
- Radio moderado: `10px`.
- Fila activa marcada con borde azul e indicador lateral interno.
- Se removieron hovers visuales de los articles para mantener una UI sobria.
- La animacion de entrada fue retirada porque afectaba la percepcion de rendimiento al seleccionar archivos.
- La animacion de eliminacion queda como collapse vertical sutil, sin generar scroll horizontal.

### Dropdown De Tipologia

- Se usa `AppInputSelect`.
- El dropdown esta alineado con el nombre del archivo y acciones.
- Se amplio el ancho de la columna de metadata para que el select respire mas.
- El selector usa borde mas moderno:
  - radio `8px`;
  - borde `#cbd8e6`;
  - foco azul con ring suave;
  - fondo blanco.
- En error:
  - borde rojo;
  - placeholder rojo;
  - label rojo;
  - flecha roja;
  - sin duplicar textos largos dentro de la fila.

### Acciones Por Archivo

- Acciones con botones icon-only:
  - `Ver`;
  - `Guardar`;
  - `Eliminar`.
- Se mantienen `aria-label` por archivo.
- Guardar usa color primario azul.
- Eliminar usa rojo vivo.
- Visualizar usa variante secundaria.

### Visor / Preview PDF

- El preview no se muestra inicialmente.
- El preview se activa solo al hacer click en `Ver`.
- El panel puede cerrarse y vuelve a liberar espacio a la cola.
- La aparicion/desaparicion del visor tiene animacion sutil.
- En desktop pequeno se reserva altura adicional y scroll del modal para ver el PDF completo.
- En desktop grande se mantiene altura normal para no desperdiciar espacio.
- Este preview es temporal antes de almacenar. El visor oficial del documento persistido sigue siendo el del tab Documentos (`AppVisorEmbedPdf`) despues del refresh backend.

### Footer

- El footer de `AppUploadBatchView` queda fijo dentro del contenedor mediante `position: sticky`.
- Mantiene resumen del lote y controles de progreso sin desplazarse fuera de vista.

## 12. Manejo De Errores UX

Se ajusto la presentacion de errores para no descuadrar las filas.

Reglas actuales:

- Error global del adapter se muestra en `Alert` compacto arriba del componente.
- Error por falta de tipologia no se duplica como parrafo largo dentro del article.
- La fila marca visualmente el dropdown de tipologia en rojo.
- El mensaje principal queda centralizado en el alert o helper controlado.
- Las filas conservan alineacion de nombre, dropdown y botones aunque exista error.

Mensajes normalizados:

| Caso | Mensaje funcional |
|---|---|
| Falta tipologia | `No se puede guardar: selecciona la tipologia documental del archivo.` |
| Falta fecha | `No se puede guardar: ingresa la fecha documental del archivo.` |
| Fecha invalida | `No se puede guardar: la fecha documental debe ser real, no futura y usar formato AAAA-MM-DD.` |
| Extension invalida | `No se puede guardar: la extension {extension} no esta permitida.` |
| Store falla | Mensaje de error tipado del servicio o `UserMessage` backend si existe. |
| Response invalida | Error controlado de contrato. |

## 13. Validaciones Runtime

### Contexto

- `nombreGabinete` requerido.
- `idRespuestaRadicado` requerido, numerico y positivo.
- Si no hay gabinete, se bloquea carga.
- Si no hay respuesta de radicado, se bloquea carga.

### Archivo

- nombre requerido;
- se elimina cualquier ruta local;
- extension normalizada;
- tamano mayor a `0`;
- tamano menor o igual a config;
- validacion en modo `queue-with-error`.

### Metadata

- tipologia obligatoria por archivo;
- `idTipoDocumento` debe ser positivo;
- `nombreTipoDocumento` no puede estar vacio;
- `numeroPaginas` debe ser number positivo o `null`;
- no se envia `NumeroPaginas` como string.

### StorageEngineV2

- init valida `rutaTemporalId`, `archivoTemporalId`, `chunkSizeBytes`;
- chunks se suben como bytes crudos;
- status valida que `ChunksPendientes` este vacio;
- complete se llama despues de status valido;
- storage valida respuesta final y conserva `rawBackendResult`.

## 14. Rendimiento Y Estabilidad

Cambios realizados:

- Se removieron animaciones de entrada de filas porque generaban sensacion de lentitud al seleccionar archivos.
- Se desactivaron transiciones del dragger de Ant Design en el upload compacto.
- La animacion de salida se limita al collapse vertical de la fila eliminada.
- El preview se renderiza solo cuando el usuario lo solicita.
- Se mejoraron dependencias de callbacks para evitar depender del estado completo.
- La sugerencia de tipologia usa preparacion previa de opciones para reducir trabajo repetido.
- Se evitan object URLs persistentes fuera del runtime del preview.
- Se conserva procesamiento secuencial por archivo para no saturar backend.

Decision revertida:

- Se probo renderizar un select liviano solo para la fila activa como mitigacion de rendimiento.
- Se revirtio porque no mejoro la experiencia esperada y cambiaba demasiado el comportamiento visual.
- Estado final: `AppInputSelect` visible por fila, con estilos compactos.

## 15. Accesibilidad

- El disparador del modal tiene `aria-label`.
- Los botones por archivo tienen `aria-label` con nombre del archivo.
- El select de tipologia recibe `aria-label` por archivo.
- El estado de fila activa usa `aria-pressed`.
- Los errores no dependen solo del color: se conserva estado de error y mensaje funcional global.
- El preview puede activarse por accion explicita.
- Se respeta `prefers-reduced-motion` para animaciones.

## 16. Seguridad Y Restricciones Cumplidas

Confirmaciones:

- Backend no modificado.
- Endpoints no modificados.
- No se inventaron endpoints de storage.
- No se uso `.ashx`.
- No se uso `XMLHttpRequest`.
- No se uso jQuery.
- No se uso WebForms.
- No se uso `FormData` legacy para chunks.
- No se llamaron callbacks globales legacy.
- No se actualizo DOM manualmente.
- No se persistieron URLs temporales.
- No se guardo `File` en storage global.
- No se loguearon bytes de archivo.
- No se loguearon tokens.
- No se agrego dependencia runtime legacy.

## 17. Pruebas Ejecutadas

Pruebas focalizadas ejecutadas durante el ticket:

```powershell
npm.cmd test -- --run src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalState.test.ts src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.test.ts src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.test.tsx src/app/Components/UI/AppUploadBatchView/AppUploadBatchView.test.tsx src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx
```

Resultado:

```text
Test Files: 5 passed
Tests: 23 passed
```

Tambien se ejecutaron pruebas focales posteriores sobre UI:

```powershell
npm.cmd test -- --run src/app/Components/UI/AppUploadBatchView/AppUploadBatchView.test.tsx src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.test.tsx
```

Resultado:

```text
Test Files: 2 passed
Tests: 14 passed
```

Y validacion del tab Gestion:

```powershell
npm.cmd test -- --run src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx
```

Resultado:

```text
Test Files: 1 passed
Tests: 4 passed
```

Warnings conocidos durante pruebas:

- advertencias React `act(...)` en escenarios de componentes con estado async;
- advertencias JSDOM por pseudo-elementos de Ant Design.

No se consideran regresiones funcionales del ticket.

## 18. Build Global

Build global ejecutado previamente:

```powershell
npm.cmd run build
```

Resultado: fallido por errores preexistentes fuera del alcance de SCRUMCORE-277:

- `src/app/Components/UI/AppDigitalizador/index.ts`: export `AppDigitalizadorMode` no existe.
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx`: variables no usadas.
- `src/modules/radicacion/components/CapDocument.tsx`: incompatibilidad de ref `HTMLDivElement | null` vs `HTMLElement`.

Estos errores no pertenecen al flujo de `AppUploadDocumental` ni a Gestion Respuesta.

## 19. Pendientes Reales / Siguiente Endurecimiento

Pendiente principal:

- Reemplazar el loader semilla de tipologias por el servicio real de tipologias workflow:

```text
GET /api/gestor-documental/tipologias-documentales
Contexto=WORKFLOW
IdTareaWf={idTareaWf}
IdRutaWf={idRutaWf}
```

Para completar ese endurecimiento se requiere:

- propagar `idRutaWf` hasta Gestion Respuesta si aun no llega al provider;
- crear servicio tipado `tipologiasDocumentalesWorkflow.service.ts`;
- crear hook `useTipologiasDocumentalesWorkflow`;
- usar el hook/loader desde `GestionRespuestaUploadDocumental`;
- manejar loading/empty/error/retry de catalogo;
- cubrir pruebas de servicio, hook e integracion UI;
- no resolver `IdTipoTramite` en frontend.

Pendientes de validacion manual recomendados:

- probar modal en desktop pequeno con PDF real;
- probar modal en desktop grande sin scroll innecesario;
- probar guardar individual;
- probar guardar todos;
- probar extension invalida;
- probar falta de tipologia;
- probar almacenamiento exitoso y refresh del tab Documentos contra backend real;
- confirmar que el documento nuevo se abre desde `AppVisorEmbedPdf` despues del refresh backend.

## 20. Criterios De Aceptacion Cubiertos

- `GestionRespuesta` integra carga documental en el tab Gestion.
- El flujo vive en modal al hacer click en el upload/disparador.
- `AppUploadDocumental` se reutiliza, no se reimplementa upload legacy.
- `AppUploadBatchView` se usa como base visual de lote.
- `AppUpload` se usa como selector/drag-drop.
- `AppProgressBatch` queda disponible para flujo de lote.
- Cada archivo tiene metadata independiente.
- Cada archivo requiere tipologia documental.
- El request final se hace por archivo.
- Storage usa `init -> chunks -> status -> complete -> almacenar`.
- Chunks se envian como bytes crudos.
- Se usa `Content-Type: application/octet-stream`.
- Se envia `X-Total-Chunks`.
- Se consulta status antes de complete.
- Se preserva `rawBackendResult`.
- Se valida `AnexoRespuesta.Created`.
- Se refresca `DocumentosWorkbench` desde backend.
- El visor oficial del documento persistido queda en el tab Documentos.
- La UI del modal fue compactada y refinada.
- El preview se oculta inicialmente y aparece bajo demanda.
- La cola y footer mantienen layout estable.
- Los errores no descuadran los articles.
- No hay dependencia runtime legacy.

## 21. Confirmacion Final

Confirmaciones del ticket:

- backend no modificado;
- endpoints de almacenamiento no modificados;
- no se uso `.ashx`;
- no se uso `FormData` legacy para chunks;
- no se uso `XMLHttpRequest`;
- no se uso jQuery;
- no se uso Bootstrap manual ni WebForms;
- `AppUpload` no fue reemplazado;
- `AppUploadBatchView` fue reutilizado;
- `AppUploadDocumental` queda reusable fuera de Gestion Respuesta;
- Gestion Respuesta queda con adapter propio, sin contaminar componentes shared con dominio documental;
- el listado de documentos se actualiza por refresh backend, no por insercion local sintetica.
