# SCRUMCORE-284 - Pruebas y validaciones

## Pruebas automatizadas ejecutadas

### Tipologias, upload documental y storage

```powershell
npm.cmd test -- --run src/modules/gestionCorrespondencia/adapters/gestionRespuestaUploadDocumental.mapper.test.ts src/modules/gestionCorrespondencia/tests/GestionRespuestaUploadDocumental.test.tsx src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.test.ts
```

Resultado:

```txt
Test Files: 3 passed
Tests: 24 passed
```

### AppUploadDocumental y servicio storage

```powershell
npm.cmd test -- --run src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.test.tsx src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalState.test.ts src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.test.ts
```

Resultado:

```txt
Test Files: 3 passed
Tests: 22 passed
```

### AppUploadDocumental state y componente

```powershell
npm.cmd test -- --run src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalState.test.ts src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.test.tsx
```

Resultado:

```txt
Test Files: 2 passed
Tests: 6 passed
```

### Workbench refresh y acciones

```powershell
npm.cmd test -- --run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx
```

Resultado:

```txt
Test Files: 2 passed
Tests: 24 passed
```

### Adapter Workbench y hook de tabla

```powershell
npm.cmd test -- --run src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx
```

Resultado:

```txt
Test Files: 3 passed
Tests: 31 passed
```

### Cierre automatico del modal

```powershell
npm.cmd test -- --run src/modules/gestionCorrespondencia/tests/GestionRespuestaUploadDocumental.test.tsx
```

Resultado:

```txt
Test Files: 1 passed
Tests: 3 passed
```

## Validaciones manuales realizadas

### Almacenamiento exitoso

Se valido con backend real que al guardar un documento con tipologia:

- se ejecuta `POST /api/gestor-documental/almacenamiento`;
- backend retorna `AnexoRespuesta.Created=true`;
- `refreshDocumentos()` se dispara;
- `DocumentosWorkbench` recarga;
- el documento queda disponible en el listado;
- el modal se cierra automaticamente.

Evidencia observada durante diagnostico temporal:

```txt
[GestionRespuestaUploadDocumental] stored result
anexoCreated: true
idAlmacen: 9930
idRegistroProduccionDocumental: 22189
IdAnexoRespuesta: 122

[GestionRespuestaUploadDocumental] refreshDocumentos triggered
idRespuestaRadicado: 640
idTareaWf: 933
idRutaWf: 9
radicado: 2500466700035
```

### Eliminacion desde AppTreeTable

Se valido que el click de eliminar:

- llega a `DocumentosWorkbench`;
- pasa a `useGestionRespuestaDocumentosTable`;
- construye request propio de Workbench;
- backend responde `success=true`;
- el arbol se remonta para reflejar eliminacion.

Evidencia observada durante diagnostico temporal:

```txt
[DocumentosWorkbench] action triggered
actionId: eliminar_item
rowId: doc-9931

[useGestionRespuestaDocumentosTable] performAction input
documentId: 9931
gabinete: CORRESPO

[useGestionRespuestaDocumentosTable] action request
Payload.DocumentId: 9931
Payload.NombreGabinete: CORRESPO

[useGestionRespuestaDocumentosTable] performAction response
success: true
Operation: deleted
RequiresReloadNode: true
```

### Falta de tipologia

Se valido que el frontend ya no corta antes del storage final.

Comportamiento:

- upload temporal se ejecuta;
- chunks se suben;
- complete se ejecuta;
- `POST /almacenamiento` llega al backend con `trd: null`;
- backend responde `400`;
- UI no muestra mensaje tecnico duplicado de tipologia;
- la respuesta backend fue revisada durante diagnostico temporal.

Respuesta backend observada:

```txt
Status: 400
message: Cabinet index seed is invalid: StorageTrd requerido
```

Interpretacion:

- backend requiere TRD para el provider/cabinet index seed;
- regla backend existe;
- mensaje backend actual es tecnico;
- si se requiere UX final, backend debe enviar `UserMessage` claro.

### Archivos pesados

Se identifico limitacion actual:

```txt
DEFAULT_MAX_SIZE_BYTES = 25 * 1024 * 1024
```

Esto bloquea archivos mayores a 25 MB en frontend.

No es una limitacion del cliente por chunks. El cliente tecnico soporta archivos grandes, pero la config de Gestion Respuesta aun no consume `LengUpload`.

Pendiente asociado:

```txt
GET /api/gestor-documental/configuracion-upload?nameProceso=CORRESPO
```

## Riesgos y pendientes de cierre

- Trazas temporales retiradas antes del PR final.
- Definir regla final de tipologia:
  - bloquear frontend sin mensaje, o
  - dejar backend como unica autoridad visible.
- Implementar configuracion upload por backend para archivos pesados.
- Validar flujo completo de `Guardar todo` con multiples archivos reales.
- Validar que al cerrar modal por exito no se pierda feedback necesario para usuario.
- Confirmar con backend si `StorageTrd requerido` debe convertirse en `UserMessage`.
