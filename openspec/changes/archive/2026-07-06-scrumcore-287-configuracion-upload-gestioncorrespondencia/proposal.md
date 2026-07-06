## Why

Gestion Respuesta ya usa `AppUploadDocumental` para adjuntos y carga tipologias workflow desde backend, pero la configuracion de carga de archivos sigue hardcodeada en frontend:

- extensiones locales: `.pdf,.png,.jpg,.jpeg,.tif,.tiff`;
- tamano maximo local: `25 * 1024 * 1024`;
- fuente actual: `loadGestionRespuestaUploadConfig()` en `gestionRespuestaUploadDocumental.service.ts`.

Esto impide respetar la configuracion real del proceso `CORRESPO`, que backend expone mediante:

```txt
GET /api/gestor-documental/configuracion-upload?nameProceso=CORRESPO
```

El ticket SCRUMCORE-287 corrige esa brecha sin cambiar el flujo documental ya implementado en SCRUMCORE-277/SCRUMCORE-284.

## What Changes

- Crear tipos, servicio y hook de configuracion upload para Gestion Correspondencia.
- Consumir `GET /api/gestor-documental/configuracion-upload` con `nameProceso=CORRESPO`.
- Normalizar `ExtensionUpload` hacia `accept` y `allowedExtensions`.
- Normalizar `LengUpload` hacia `maxSizeBytes`.
- Reemplazar el loader hardcodeado `loadGestionRespuestaUploadConfig()` por configuracion backend.
- Mantener `AppUploadDocumental` como componente consumidor final.
- Mantener `AppUploadBatchView` y `AppUpload` sin cambios si ya reciben `accept` y `maxSize`.
- Manejar loading, error, empty y retry desde el loader/hook de modulo.
- Agregar pruebas focales de servicio, hook e integracion.
- Documentar la implementacion enterprise del ticket.

## Out Of Scope

- No implementar tipologias documentales.
- No cambiar `GET /api/gestor-documental/tipologias-documentales`.
- No crear metadata por archivo.
- No modificar `renderMetadata`.
- No cambiar almacenamiento por chunks.
- No modificar backend.
- No inventar endpoints.
- No introducir `any`.

## Impact

### Frontend

- `src/modules/gestionCorrespondencia/services/gestionRespuestaUploadDocumental.service.ts`
- `src/modules/gestionCorrespondencia/services/configuracionUploadCorrespondencia.service.ts`
- `src/modules/gestionCorrespondencia/hooks/useConfiguracionUploadCorrespondencia.ts`
- `src/modules/gestionCorrespondencia/types/configuracionUploadCorrespondencia.types.ts`
- Tests bajo `src/modules/gestionCorrespondencia/tests/`

### Componentes Reusables

No se espera modificar `AppUploadDocumental`, `AppUploadBatchView` ni `AppUpload` salvo que una prueba demuestre que no propagan correctamente `accept`/`maxSize`.

### Documentacion

Crear:

```txt
docs/Architecture/GestionCorrrespondecia/Integracion-AppUploadDocumental/SCRUMCORE-287-FE-ConfiguracionUpload-Adjuntos-Correspo.md
```

## Success Criteria

- La configuracion de upload de Gestion Respuesta viene de backend.
- Se usa `nameProceso=CORRESPO`.
- Las extensiones permitidas salen de `ExtensionUpload`.
- El tamano maximo sale de `LengUpload`.
- La seleccion queda bloqueada si no hay configuracion usable.
- No quedan extensiones/tamano hardcodeados como fuente final en UI.
- Tests focales pasan.
- Backend no se modifica.
