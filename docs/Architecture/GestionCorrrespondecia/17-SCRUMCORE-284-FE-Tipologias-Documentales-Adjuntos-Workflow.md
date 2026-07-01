# SCRUMCORE-284 - Tipologias documentales por adjunto en Gestion Correspondencia

## Metadata

- Scrum: SCRUMCORE-284
- Modulo: Gestion Correspondencia / Gestion Respuesta
- Tipo: Frontend implementation
- Fecha: 2026-07-01
- Estado tecnico: Implementado
- Dependencia backend: SCRUM-304 resuelve tipologias por `WORKFLOW + IdTareaWf + IdRutaWf`
- Backend modificado: No
- Endpoints inventados: No
- Componentes shared acoplados al dominio: No

## Problema

SCRUMCORE-277 dejo integrado `AppUploadDocumental` en Gestion Respuesta con carga documental por StorageEngineV2, metadata por archivo, modal, cola visual, guardado individual y guardado por lote. Sin embargo, el loader de tipologias de Gestion Respuesta seguia usando una opcion seed/hardcoded:

```ts
{ idTipoDocumento: 43, nombreTipoDocumento: "Comprobante De Egreso" }
```

Ese fallback no era aceptable para produccion porque la tipologia documental debe salir del catalogo real del workflow.

## Objetivo

Conectar el flujo ya implementado de `GestionRespuestaUploadDocumental` con el catalogo real de tipologias documentales, usando el endpoint confirmado:

```txt
GET /api/gestor-documental/tipologias-documentales
```

El frontend envia exclusivamente:

```ts
{
  Contexto: "WORKFLOW",
  IdTareaWf: number,
  IdRutaWf: number
}
```

No se resuelve `IdTipoTramite` en frontend y no se envia `IdTipoTramite=0`.

## Implementacion

### Contexto workflow

Se propago `idRutaWf` junto con `idTareaWf` en:

- `src/modules/gestionCorrespondencia/types/gestionRespuestaEstructura.types.ts`
- `src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.ts`
- `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx`
- `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
- `src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx`
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentos.ts`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx`

El mapper acepta variantes comunes del backend: `idRutaWf`, `IdRutaWf`, `idRutaWorkflow`, `IdRutaWorkflow`, `ID_RUTA_WF` e `id_ruta_wf`.

### Servicio tipado

Se agrego:

```txt
src/modules/gestionCorrespondencia/services/tipologiasDocumentalesWorkflow.service.ts
src/modules/gestionCorrespondencia/types/tipologiasDocumentalesWorkflow.types.ts
```

Responsabilidades:

- validar `idTareaWf > 0`;
- validar `idRutaWf > 0`;
- consumir `clienteApi.get`;
- enviar `Contexto=WORKFLOW`, `IdTareaWf`, `IdRutaWf`;
- no enviar `IdTipoTramite`;
- normalizar `{ Id, Descripcion }` a `{ value, label, idTipoDocumento, nombreTipoDocumento }`;
- aceptar `success=true` con `data=[]`;
- lanzar errores funcionales tipados ante `success=false` o shape invalido.

### Hook tipado

Se agrego:

```txt
src/modules/gestionCorrespondencia/hooks/useTipologiasDocumentalesWorkflow.ts
```

Expone:

```ts
{
  options,
  loading,
  error,
  empty,
  reload
}
```

Politicas:

- no llama backend si faltan ids validos;
- usa `AbortController`;
- ignora respuestas stale cuando cambia tarea/ruta;
- permite retry con `reload`;
- diferencia catalogo vacio exitoso de error funcional.

### Integracion con AppUploadDocumental

Se actualizo:

```txt
src/modules/gestionCorrespondencia/services/gestionRespuestaUploadDocumental.service.ts
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx
```

El loader `loadGestionRespuestaTiposDocumentales` ahora delega en `getTipologiasDocumentalesWorkflow` y mapea al contrato de `AppUploadDocumental`.

`GestionRespuestaUploadDocumental` incluye ahora:

```ts
{
  idTareaWorkflow: idTareaWf,
  idRutaWorkflow: idRutaWf
}
```

Si no existe `idRutaWf`, el upload se bloquea con mensaje funcional. No hay fallback hardcoded.

## Relacion con SCRUMCORE-277

SCRUMCORE-284 no reconstruye la UI. Complementa lo ya implementado:

- se conserva `GestionRespuestaUploadDocumentalModal`;
- se conserva `AppUploadDocumental`;
- se conserva `AppUploadBatchView`;
- se conserva `AppInputSelect`;
- se conserva metadata independiente por archivo;
- se conserva StorageEngineV2;
- se conserva refresh del Workbench cuando `AnexoRespuesta.Created=true`.

La mejora es la fuente de verdad del catalogo documental.

## Flujo funcional

1. `GestionCorrespondenciaRoute` resuelve estructura de respuesta.
2. La estructura expone `idRutaWf` si el backend lo entrega.
3. `GestionRespuesta` recibe `idTareaWf`, `idRutaWf`, `radicado` e `idRespuestaRadicado`.
4. `GestionRespuestaDocumentosProvider` publica el contexto documental.
5. `GestionRespuestaUploadDocumental` arma `UploadDocumentalContext`.
6. `AppUploadDocumental` ejecuta `loadTiposDocumentales`.
7. `loadGestionRespuestaTiposDocumentales` llama el servicio workflow.
8. El servicio consume `GET /api/gestor-documental/tipologias-documentales`.
9. Cada archivo conserva su propia seleccion de tipologia.
10. Cada guardado final envia el `Trd` del archivo actual.

## Politica de errores

- Si falta `idTareaWf` o `idRutaWf`, no se llama el endpoint.
- Si el backend retorna `success=false`, se muestra error funcional.
- Si el catalogo retorna vacio, la respuesta es valida, pero el guardado queda bloqueado cuando la tipologia es obligatoria.
- Si el shape del backend no corresponde a `{ Id, Descripcion }`, se lanza error de contrato.
- No se loguean payloads sensibles.

## Pruebas ejecutadas

```txt
npm.cmd test -- --run src/modules/gestionCorrespondencia/tests/tipologiasDocumentalesWorkflow.service.test.ts src/modules/gestionCorrespondencia/tests/useTipologiasDocumentalesWorkflow.test.tsx src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentos.test.tsx src/modules/gestionCorrespondencia/tests/GestionRespuestaUploadDocumental.test.tsx src/modules/gestionCorrespondencia/pages/GestionRespuesta.test.tsx src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.test.ts
```

Resultado:

```txt
7 test files passed
51 tests passed
```

## Validaciones adicionales

OpenSpec:

```txt
openspec.cmd validate scrumcore-284-implementacion-tipologias-documentales-gestioncorrespondencia --strict
```

Resultado:

```txt
Change 'scrumcore-284-implementacion-tipologias-documentales-gestioncorrespondencia' is valid
```

Build global:

```txt
npm.cmd run build
```

Resultado: no finalizo por errores preexistentes fuera del alcance de SCRUMCORE-284 en `src/modules/digitalizacion` y `src/modules/radicacion`. No se detectaron errores de compilacion reportados en los archivos modificados para esta integracion.

## Confirmaciones

- Backend no modificado.
- No se inventaron endpoints.
- No se envio ni resolvio `IdTipoTramite` en frontend.
- No se agrego fallback hardcoded de tipologias.
- `AppUploadBatchView` sigue generico.
- `AppInputSelect` sigue siendo el dropdown de tipologia dentro de `AppUploadDocumental`.
- El flujo de almacenamiento documental por chunks queda intacto.
