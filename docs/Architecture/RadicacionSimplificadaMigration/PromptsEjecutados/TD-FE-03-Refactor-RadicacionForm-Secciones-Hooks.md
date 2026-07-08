# TD-FE-03 - Refactor RadicacionForm Por Secciones Y Hooks

## Ticket Asociado

```text
SCRUMCORE-302
```

## Que Hace

Define la descomposicion incremental de `RadicacionForm.tsx` para convertirlo en un componente orquestador basado en composicion.

El objetivo de SCRUMCORE-302 no es cambiar comportamiento funcional ni introducir nuevas capacidades. El objetivo es reducir acoplamiento, extraer responsabilidades y preparar el formulario para mantenimiento seguro.

## Contexto De Dependencias

Este refactor debe ejecutarse despues de las fases que ya estabilizan responsabilidades externas:

- `TD-FE-01`: la plantilla se recibe por props desde el boundary del modulo.
- `TD-FE-02`: el estado documental vive en `RadicacionDocumentalContext`.
- `TD-FE-04`: rutas y tabs estan centralizadas.
- `TD-FE-05`: el reset del formulario vive en `useRadicacionFormReset`.
- `FE-07`: enviar a pendiente vive fuera del formulario como caso de uso transaccional.

La regla principal es no reintroducir logica documental, navegacion, backend ni mutaciones dentro de componentes presentacionales.

## Problema Que Resuelve

`RadicacionForm.tsx` concentra actualmente:

- layout general;
- render de secciones;
- estado UI local;
- seleccion de tramite;
- remitente;
- destinatario;
- autocompletes;
- footer;
- modales;
- mappers tolerantes a variantes backend;
- tipos amplios y casts.

Esto hace que cualquier ajuste pequeno tenga riesgo alto de tocar flujo documental, validaciones o comportamiento visual no relacionado.

## Arquitectura A Ejecutar

```text
RadicacionForm
  -> useRadicacionFormOptions()
  -> useRadicacionTramiteSelection()
  -> RadicacionMetadataSection
  -> RadicacionTramiteSection
  -> RadicacionRemitenteSection
  -> RadicacionDestinatarioSection
  -> RadicacionFormFooter
  -> utils/radicacionOptionMappers
```

`RadicacionForm` debe quedar como orquestador:

```text
RadicacionForm
  -> crea/conecta hooks
  -> prepara props
  -> compone secciones
```

Las secciones deben recibir datos y callbacks. No deben consultar backend documental, navegar, limpiar contexto ni ejecutar mutaciones.

## Orden De Ejecucion

### 1. Footer

Crear:

```text
src/modules/radicacion/components/RadicacionFormFooter.tsx
```

Responsabilidad:

- renderizar botones;
- recibir acciones por props;
- no ejecutar logica propia;
- no llamar `form.resetFields`;
- no conocer `RadicacionDocumentalContext`;
- no ejecutar `enviar-pendiente`.

Callbacks esperados:

```text
handleClearRadicacionForm
handleRadicar
handleDocumentosIa
```

### 2. Tramite Y Flujo

Crear:

```text
src/modules/radicacion/hooks/useRadicacionTramiteSelection.ts
src/modules/radicacion/components/RadicacionTramiteSection.tsx
```

Responsabilidad:

- administrar `selectedTramiteId`;
- administrar `hasUserChangedTramite`;
- resolver opciones de tramite y flujo;
- limpiar flujo cuando corresponde;
- exponer props para la seccion.

No debe conocer backend documental, rutas ni contexto documental.

### 3. Remitente Y Destinatario

Crear:

```text
src/modules/radicacion/components/RadicacionRemitenteSection.tsx
src/modules/radicacion/components/RadicacionDestinatarioSection.tsx
```

Responsabilidad:

- encapsular UI de remitente;
- encapsular UI de destinatario;
- aislar token/autocomplete/tag menus;
- recibir metadata y callbacks;
- no mezclar reglas del proceso documental.

### 4. Mappers

Crear:

```text
src/modules/radicacion/utils/radicacionOptionMappers.ts
```

Responsabilidad:

- normalizar `idValue`, `id_value`, `value`, `Value`, `value_campo`;
- centralizar `resolveCampoIdScript`;
- aislar tolerancia a contratos backend;
- evitar casts repetidos dentro del JSX.

### 5. Tipos

Crear o consolidar tipos para:

- opciones Ant Design;
- opciones backend;
- campos dinamicos;
- campos con `id_escript`;
- contratos de seleccion;
- parametros de tag render.

## Reglas Arquitectonicas

- No cambiar comportamiento funcional.
- No modificar flujo documental.
- No modificar `RadicacionDocumentalContext`.
- No modificar rutas.
- No llamar backend desde secciones presentacionales.
- No mover mutaciones al footer.
- No reintroducir `useCamposPlantilla` dentro del formulario.
- No hacer refactor masivo sin pruebas intermedias.

## Relacion Con SCRUMCORE-300

SCRUMCORE-300 ya separa la limpieza del formulario en `useRadicacionFormReset`.

SCRUMCORE-302 debe consumir esa separacion:

```text
RadicacionFormFooter
  -> props.handleClearRadicacionForm
```

El footer no debe implementar el reset ni duplicar `form.resetFields`.

## Archivos Principales Esperados

- `src/modules/radicacion/components/RadicacionForm.tsx`
- `src/modules/radicacion/components/RadicacionFormFooter.tsx`
- `src/modules/radicacion/components/RadicacionTramiteSection.tsx`
- `src/modules/radicacion/components/RadicacionRemitenteSection.tsx`
- `src/modules/radicacion/components/RadicacionDestinatarioSection.tsx`
- `src/modules/radicacion/hooks/useRadicacionTramiteSelection.ts`
- `src/modules/radicacion/hooks/useRadicacionFormOptions.ts`
- `src/modules/radicacion/utils/radicacionOptionMappers.ts`

## Implementacion Ejecutada En SCRUMCORE-302

### Alcance Implementado

- `RadicacionFormFooter` fue extraido como componente presentacional.
- `RadicacionFormFooter` recibe `onClear`, `onSubmit` y `onDocumentosIa` por props.
- `Documentos IA` conserva comportamiento neutral cuando no recibe callback.
- La seleccion de tramite/flujo fue movida a `useRadicacionTramiteSelection`.
- `useRadicacionTramiteSelection` administra `selectedTramiteId`, `hasUserChangedTramite`, `tramiteOptions`, `flujoOptions`, `isLoadingFlujosRelacionados` y limpieza de `flujo`.
- Los mappers de `TipoRadicado` y `Descripcion_Documento` fueron centralizados en `utils/radicacionOptionMappers.ts`.
- `resolveCampoIdScript` y `normalizeCampoName` quedan centralizados en `radicacionOptionMappers.ts`.
- Los mappers de campos dinamicos `SELECCION` en `CamposPlantillaAutoCompleteRenderer` consumen `mapCampoDrowlistOptions`.
- `RadicacionForm` deja de importar directamente `useFlujosRelacionadosTramite`.

### Archivos Implementados

```text
src/modules/radicacion/components/RadicacionFormFooter.tsx
src/modules/radicacion/components/RadicacionFormFooter.spec.test.tsx
src/modules/radicacion/hooks/useRadicacionTramiteSelection.ts
src/modules/radicacion/hooks/useRadicacionTramiteSelection.spec.test.tsx
src/modules/radicacion/utils/radicacionOptionMappers.ts
src/modules/radicacion/utils/radicacionOptionMappers.spec.test.ts
src/modules/radicacion/components/RadicacionForm.tsx
src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.tsx
```

### Antes Vs Despues

Antes:

```text
RadicacionForm
  -> render footer
  -> ejecuta callbacks inline del footer
  -> mantiene estado de tramite
  -> consulta flujos relacionados
  -> limpia flujo
  -> contiene mappers tolerantes a backend
  -> contiene resolveCampoIdScript
```

Despues:

```text
RadicacionForm
  -> compone RadicacionFormFooter
  -> conecta useRadicacionTramiteSelection
  -> consume mappers centralizados
  -> conserva coordinacion del formulario
```

### Riesgos Residuales

- `RadicacionTramiteSection`, `RadicacionRemitenteSection`, `RadicacionDestinatarioSection` y `RadicacionMetadataSection` quedan como siguiente corte incremental.
- `BaseSelectUsuarios`, `SelectRemitente`, `SelectDestinatario`, `SelectRemitenteToken` y `SelectDestinatarioToken` siguen dentro de `RadicacionForm.tsx`.
- Persisten deudas previas de lint en `RadicacionForm.tsx`: `any` y `react-hooks/set-state-in-effect`.
- Persiste deuda previa de lint en `CamposPlantillaAutoCompleteRenderer.tsx`: `react-hooks/set-state-in-effect`.

## Pruebas Esperadas

- pruebas unitarias de footer;
- pruebas unitarias de hooks extraidos;
- pruebas unitarias de mappers;
- pruebas de integracion de `RadicacionForm`;
- regresion de limpieza del formulario;
- regresion de seleccion de tramite, flujo, remitente y destinatario.

Comando focal sugerido:

```bash
npm test -- --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx src/modules/radicacion/hooks/useRadicacionFormReset.spec.test.tsx
```

## Validaciones Ejecutadas

```bash
npm test -- --run --testTimeout 10000 src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx src/modules/radicacion/components/RadicacionForm.spec.test.tsx src/modules/radicacion/components/RadicacionFormFooter.spec.test.tsx src/modules/radicacion/hooks/useRadicacionTramiteSelection.spec.test.tsx src/modules/radicacion/utils/radicacionOptionMappers.spec.test.ts
```

Resultado:

```text
5 test files passed
46 tests passed
```

Validacion adicional:

```bash
npm test -- --run src/modules/radicacion/components/RadicacionForm.spec.test.tsx
```

Resultado:

```text
1 test file passed
32 tests passed
```

Validaciones con observaciones:

```bash
npx eslint src/modules/radicacion/components/RadicacionFormFooter.tsx src/modules/radicacion/hooks/useRadicacionTramiteSelection.ts src/modules/radicacion/utils/radicacionOptionMappers.ts src/modules/radicacion/components/RadicacionFormFooter.spec.test.tsx src/modules/radicacion/hooks/useRadicacionTramiteSelection.spec.test.tsx src/modules/radicacion/utils/radicacionOptionMappers.spec.test.ts
```

Resultado:

```text
Los archivos nuevos no reportan errores.
RadicacionForm.tsx conserva errores previos de lint: no-explicit-any y set-state-in-effect.
CamposPlantillaAutoCompleteRenderer.tsx conserva una deuda previa de lint: set-state-in-effect.
```

```bash
npx tsc -b
```

Resultado:

```text
Falla por deuda externa existente:
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx
UploadDocumentalStoredContext no existe exportado por AppUploadDocumental.
```

## Si Falla, Revisar

- Si el footer empieza a ejecutar logica, revisar que todas las acciones lleguen por props.
- Si `Documentos` se activa o desactiva inesperadamente, revisar que ninguna seccion toque `RadicacionDocumentalContext`.
- Si se pierden opciones de tramite, revisar `useRadicacionTramiteSelection` y los mappers.
- Si autocompletes cambian comportamiento, revisar que los componentes extraidos conserven keys/reset y props originales.
- Si aparece duplicacion de mappers, mover la tolerancia a `radicacionOptionMappers`.

## Restricciones

- No implementar registro.
- No implementar pendientes.
- No implementar enviar a pendiente.
- No implementar tomar pendiente.
- No modificar backend.
- No modificar rutas.
- No introducir cambios visuales fuera de los necesarios por extraccion.
- No cerrar el ticket sin pruebas de regresion.
