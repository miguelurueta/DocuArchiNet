# FE-01 - Conectar Registro De Radicacion Entrante

## Ticket Asociado

```text
SCRUMCORE-303
```

## Que Hace

Conecta el boton `Radicar` del formulario React con la API moderna de registro de radicacion entrante:

```text
POST /api/radicacion/registrar-entrante
```

La fase reemplaza la dependencia funcional del flujo legacy ASMX desde el frontend. El frontend no reconstruye la transaccion, no calcula consecutivos y no replica reglas SQL; solamente captura, normaliza, construye el DTO esperado y delega el registro al backend moderno.

## Casos De Uso Cubiertos

- Validar el formulario de radicacion antes de enviar.
- Construir `RegistrarRadicacionEntranteRequestDto` desde valores de Ant Design Form.
- Resolver labels/ids de `TipoRadicado`, `Descripcion_Documento` y `RE_flujo_trabajo`.
- Incluir remitente, destinatario, asunto, anexos, expediente y campos dinamicos.
- Incluir `NÃºmero Folios` como campo fijo requerido en `Medio de RecepciÃ³n del TrÃ¡mite`, enviando `numeroFolios` al backend.
- Derivar `Tipo_radicado_plantilla` desde el select `TipoRadicado` cuando backend lo exige como campo dinamico.
- Ejecutar `POST /api/radicacion/registrar-entrante`.
- Manejar `success=false` como error funcional visible.
- Guardar estado post-registro con `ConsecutivoRadicado`, `IdRadicado` e `IdEstadoRadicado`.
- Sincronizar `RadicacionDocumentalContext` solo si backend informa explicitamente tramite documental activo en estado `0`.

## Arquitectura Implementada

```text
RadicacionForm
  -> Form.onFinish
  -> buildRegistrarRadicacionEntranteRequest()
  -> useRegistrarRadicacion()
  -> registrarRadicacionEntrante()
  -> POST /api/radicacion/registrar-entrante
  -> RadicacionPostRegistroState
  -> RadicacionDocumentalContext si aplica
```

## Campos Complementarios Protegidos

### NÃºmero Folios

`NÃºmero Folios` queda como campo fijo del formulario dentro de la tarjeta `Medio de RecepciÃ³n del TrÃ¡mite`.

Reglas vigentes:

- Es requerido para registrar.
- Se captura como numero entero mayor o igual a `1`.
- Se envia en el request como `numeroFolios`.
- Si la carga de plantilla trae un campo equivalente (`Numero_Folios`, `NÃºmero Folios`, `NUMERO_FOLIOS`, `NumFolios` o variante normalizable) con `value_campo`, el formulario lo precarga y lo bloquea.
- Si la carga de plantilla trae el campo sin valor, el usuario lo diligencia.
- Si la carga de plantilla no trae el campo, el formulario lo muestra igualmente porque backend puede exigirlo.
- Si se renderiza fijo, debe excluirse de `Datos Especializados` para evitar duplicidad visual y doble captura.
- El mapper conserva tolerancia para reflejar el valor en `Campos` si la plantilla o `DetallePlantillaRadicadoDTO` lo requieren.

### Tipo_radicado_plantilla

`Tipo_radicado_plantilla` no es un segundo desplegable visible. Se deriva del select principal `TipoRadicado`.

Reglas vigentes:

- El select visible `TipoRadicado` alimenta `TipoRadicado.IdTipoRadicado` y `TipoRadicado.TipoRadicacion`.
- La misma opcion seleccionada alimenta `TipoPlantillaRadicado`: `TipoPlantillaRadicado.TipoPlantillaRadicado = selected.Value` e `IdTipoPlantillaRdicado = selected.idValue`.
- No se debe enviar `TipoPlantillaRadicado.IdTipoPlantillaRdicado` en `0`; si el select no resuelve un `idValue > 0`, el request debe tratarse como invalido.
- Si backend valida `Campo Tipo_radicado_plantilla`, el mapper debe poblar un item en `Campos` con `NombreCampo: "Tipo_radicado_plantilla"` y `Valor` igual al valor seleccionado de `tipoRadicado`.
- `TipoPlantillaRadicado.IdTipoPlantillaRdicado` no debe tomarse de `plantilla.IdPlantillaRadicado` en este flujo; backend espera el `idValue` seleccionado en `TipoRadicado`.
- No crear un control visual duplicado para `Tipo_radicado_plantilla` salvo que producto lo solicite explicitamente.

### ModuloRegistro Y Q07

El frontend no envia `ModuloRegistro` en el payload ni por query string de `registrar-entrante`.

Reglas vigentes:

- Mantener la llamada como `POST /api/radicacion/registrar-entrante?tipoModuloRadicacion=1`.
- No agregar `ModuloRegistro` ni `moduloRegistro` por query string.
- No agregar `ModuloRegistro` al DTO frontend; backend lo resuelve internamente.
- Si backend responde `RAD_TXN_Q07` con `ModuloRegistro invalido para radicacion: RADICACION SIMPLIFICADA`, el request frontend ya esta alineado y la correccion queda en backend/Q07.
- No cambiar `tipoModuloRadicacion=1` salvo confirmacion explicita del backend.

### Sincronizacion De Validaciones Con Backend

El frontend debe leer las reglas desde la metadata de campos retornada por `/api/PlantillaRadicado/listaPlantilla` y traducirlas a reglas de Ant Design Form mediante un helper central.

Reglas vigentes:

- `obligatorio_campo` define `required`.
- `max_leng_campo` define longitud maxima solo cuando el valor enviado es texto libre.
- `tipo_campo` y `tipo_control` definen si el campo se trata como texto, numero, fecha o correo.
- `disable_campo` define bloqueo del control.
- `aleas_campo` o `name_campo` definen el nombre visible en mensajes.
- Los campos `SELECCION` no validan longitud del label porque envian `idValue`.
- Los campos numericos no usan `max_leng_campo` como longitud textual.
- Los campos `AUTOCOMPLETE` deben enviar `idValue` cuando backend lo entrega y usar `texValue` solo como texto visible.
- Nuevas reglas backend como `min_leng_campo`, `regex_campo` o `mensaje_validacion` deben agregarse primero al helper central, no al JSX de cada campo.

Implementacion actual:

```text
src/modules/radicacion/utils/radicacionCampoValidation.ts
  -> buildCampoPlantillaRules()
  -> getCampoMaxLength()
  -> shouldValidateCampoMaxLength()
```

## Archivos Principales

- `src/modules/radicacion/types/radicacionRegistro.types.ts`
- `src/modules/radicacion/services/radicacionRegistro.service.ts`
- `src/modules/radicacion/adapters/radicacionRegistroRequest.mapper.ts`
- `src/modules/radicacion/hooks/useRegistrarRadicacion.ts`
- `src/modules/radicacion/components/RadicacionForm.tsx`
- `src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.tsx`
- `src/modules/radicacion/components/RadicacionFormFooter.tsx`

## Pruebas Asociadas

- `src/modules/radicacion/adapters/radicacionRegistroRequest.mapper.test.ts`
- `src/modules/radicacion/services/radicacionRegistro.service.test.ts`
- `src/modules/radicacion/hooks/useRegistrarRadicacion.spec.test.tsx`
- `src/modules/radicacion/components/RadicacionForm.spec.test.tsx`
- `src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx`

## Validaciones Ejecutadas

```bash
npm test -- --run --testTimeout 10000 src/modules/radicacion/adapters/radicacionRegistroRequest.mapper.test.ts src/modules/radicacion/services/radicacionRegistro.service.test.ts src/modules/radicacion/hooks/useRegistrarRadicacion.spec.test.tsx src/modules/radicacion/components/RadicacionForm.spec.test.tsx src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx
```

Resultado:

```text
5 test files passed
55 tests passed
```

Lint focal de archivos nuevos:

```bash
npx eslint src/modules/radicacion/types/radicacionRegistro.types.ts src/modules/radicacion/services/radicacionRegistro.service.ts src/modules/radicacion/adapters/radicacionRegistroRequest.mapper.ts src/modules/radicacion/hooks/useRegistrarRadicacion.ts src/modules/radicacion/services/radicacionRegistro.service.test.ts src/modules/radicacion/adapters/radicacionRegistroRequest.mapper.test.ts src/modules/radicacion/hooks/useRegistrarRadicacion.spec.test.tsx
```

Resultado:

```text
Sin errores.
```

## Observaciones De Validacion

`npx tsc -b` sigue fallando por una deuda externa existente:

```text
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx
UploadDocumentalStoredContext no existe exportado por AppUploadDocumental.
```

Lint sobre archivos grandes tocados conserva deudas previas:

```text
RadicacionForm.tsx: no-explicit-any y react-hooks/set-state-in-effect.
CamposPlantillaAutoCompleteRenderer.tsx: react-hooks/set-state-in-effect.
```

## Reglas Vigentes

- No consumir endpoints `.asmx`.
- No usar jQuery ni WebForms.
- No calcular consecutivos en frontend.
- No llamar `clienteApi` desde componentes.
- No activar Documentos si backend no informa tramite documental activo en estado `0`.
- No implementar navegacion contextual completa en esta fase.
- No implementar workbench documental, scanner, visor PDF ni tipologias.

## Si Falla, Revisar

- Si el boton `Radicar` no llama backend, revisar `Form.onFinish` y `useRegistrarRadicacion`.
- Si el request llega incompleto, revisar `radicacionRegistroRequest.mapper.ts` y que los campos dinamicos tengan `name` de Ant Design Form.
- Si backend responde `Campo NÃºmero Folios: requerido`, revisar que `RadicacionForm` mantenga el campo fijo `numeroFolios` en `Medio de RecepciÃ³n del TrÃ¡mite` y que no se haya vuelto a depender solo de campos dinamicos.
- Si backend responde `Campo Tipo_radicado_plantilla: requerido`, revisar que el mapper derive ese campo desde `tipoRadicado` dentro de `Campos`.
- Si backend responde `Campo X: supera la longitud maxima permitida`, revisar `max_leng_campo` en la respuesta de plantilla y `buildCampoPlantillaRules`; si el campo es `SELECCION` o numerico, no debe validarse por longitud textual en frontend.
- Si `TipoPlantillaRadicado.IdTipoPlantillaRdicado` aparece en `0`, revisar que `TipoRadicado` haya seleccionado una opcion con `idValue > 0` y que el mapper use esa opcion para ambos objetos.
- Si backend responde `RAD_TXN_Q07` y `ModuloRegistro invalido para radicacion: RADICACION SIMPLIFICADA`, validar que el frontend solo envie `tipoModuloRadicacion=1`; si es asi, corregir normalizacion backend/Q07.
- Si backend responde `success=false`, revisar el mensaje funcional mostrado por `useRegistrarRadicacion`.
- Si Documentos no se activa tras radicar, revisar que `MetadataOperativa` incluya seniales explicitas de gestion documental y tramite activo estado `0`.
- Si aparece una ruta/documentos automaticamente sin senial backend, revisar que no se este infiriendo desde textos de UI.
