# SCRUMCORE-284 - Metadata tecnica

## Identificacion

- Scrum: SCRUMCORE-284
- Modulo principal: Gestion Correspondencia / Gestion Respuesta
- Area tecnica: Integracion de `AppUploadDocumental` con anexos workflow, tipologias documentales, almacenamiento StorageEngineV2 y Workbench de documentos.
- Fecha de documentacion: 2026-07-02
- Estado tecnico: Implementacion en curso con validaciones reales contra backend local.
- Backend modificado: No.
- Endpoints inventados: No.
- Componentes shared acoplados al dominio: No.

## Objetivo consolidado

Complementar la integracion de `AppUploadDocumental` en Gestion Respuesta para que el flujo de anexos documentales:

- cargue tipologias documentales desde workflow;
- conserve metadata independiente por archivo;
- ejecute almacenamiento temporal por chunks;
- haga almacenamiento final por archivo;
- refresque el Workbench de documentos al confirmar `AnexoRespuesta.Created`;
- cierre el modal al almacenar correctamente;
- soporte eliminacion visual coherente desde `AppTreeTable`;
- deje trazabilidad temporal en consola para depurar contratos backend de tipologia/TRD.

## Cambios funcionales cubiertos

- Tipologias documentales por workflow usando `Contexto=WORKFLOW`, `IdTareaWf` e `IdRutaWf`.
- Propagacion de `idRutaWf` hasta el contexto documental del upload.
- Adaptador de `GestionRespuestaUploadDocumental` con `idUsuarioGestion`, `idEmpresa` y `fechaElaboracion`.
- Request final de almacenamiento con `Inventario`, `Workflow`, `CabinetIndexSeed` y `AnexoRespuesta`.
- Cierre automatico del modal cuando backend confirma anexo creado.
- Refresh de `DocumentosWorkbench` tras almacenamiento exitoso.
- Eliminacion desde acciones del `AppTreeTable` usando endpoint propio del Workbench, no endpoint generico inexistente.
- Remount del `AppTreeTable` tras acciones exitosas para reflejar eliminaciones.
- Ajustes UX del modal, cola de archivos, dropdown de tipologias, preview PDF y botones.
- Diagnostico temporal de almacenamiento sin tipologia documentado y retirado del runtime.

## Archivos principales modificados

### UI shared / almacenamiento documental

```txt
src/app/Components/UI/AppUploadBatchView/AppUploadBatchView.tsx
src/app/Components/UI/AppUploadBatchView/AppUploadBatchView.test.tsx
src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.tsx
src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.types.ts
src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.ts
src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalState.ts
src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalState.test.ts
src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts
```

### Gestion Correspondencia

```txt
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumentalModal.tsx
src/modules/gestionCorrespondencia/adapters/gestionRespuestaUploadDocumental.mapper.ts
src/modules/gestionCorrespondencia/adapters/gestionRespuestaUploadDocumental.mapper.test.ts
src/modules/gestionCorrespondencia/services/gestionRespuestaUploadDocumental.service.ts
src/modules/gestionCorrespondencia/services/tipologiasDocumentalesWorkflow.service.ts
src/modules/gestionCorrespondencia/hooks/useTipologiasDocumentalesWorkflow.ts
src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx
src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts
src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts
src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts
src/modules/gestionCorrespondencia/tests/GestionRespuestaUploadDocumental.test.tsx
src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx
src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx
```

### Autenticacion / contexto runtime

```txt
src/app/auth/Infraestructura/ManejadorJWT.ts
```

## Endpoints consumidos

### Tipologias documentales

```txt
GET /api/gestor-documental/tipologias-documentales
Query:
  Contexto=WORKFLOW
  IdTareaWf={idTareaWf}
  IdRutaWf={idRutaWf}
```

### StorageEngineV2

```txt
POST   /api/gestor-documental/almacenamiento/upload-temporal/init
PUT    /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
GET    /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status
POST   /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
DELETE /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}
POST   /api/gestor-documental/almacenamiento
```

### Workbench documentos

```txt
POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action
```

## Restricciones cumplidas

- No se modifico backend.
- No se inventaron endpoints.
- No se uso `.ashx`.
- No se uso `XMLHttpRequest`.
- No se uso `FormData` para chunks.
- No se uso jQuery.
- No se uso Bootstrap manual.
- No se introdujo dependencia legacy runtime.
- Los componentes shared no conocen reglas de Gestion Respuesta.
- `AppUploadBatchView` sigue siendo generico.
- `AppUploadDocumental` concentra la experiencia documental reusable.

## Pendientes tecnicos declarados

- Reemplazar configuracion local de tamano maximo de Gestion Respuesta por endpoint real `GET /api/gestor-documental/configuracion-upload?nameProceso=CORRESPO`.
- Logs temporales retirados antes del cierre del ticket.
- Definir si frontend debe volver a bloquear tipologia antes del upload o si el backend seguira siendo la unica validacion visible.
- Validar archivos pesados luego de consumir `LengUpload` real desde backend.
