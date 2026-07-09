# Registro De Fases Ejecutadas - Radicacion Simplificada

## Objetivo

Documentar las fases de infraestructura frontend ya ejecutadas en el modulo `src/modules/radicacion`, dejando evidencia de alcance, archivos principales, validaciones y restricciones para las siguientes fases.

Este documento no reemplaza los prompts arquitectonicos. Resume el estado implementado para evitar repetir trabajo o mezclar fases.

## Fases Cerradas

| Fase | Ticket | Prompt | Estado | Resultado |
|---|---|---|---|---|
| TD-FE-01 | SCRUMCORE-290 | `PROMPT-TD-FE-01-Unificar-Fuente-Plantilla-Radicacion.md` | Ejecutado | La plantilla se carga desde el boundary del modulo y se propaga por props. |
| TD-FE-02 | SCRUMCORE-291 | `PROMPT-TD-FE-02-Contexto-Documental-Unico-Guards.md` | Ejecutado | Existe `RadicacionDocumentalContext` unico y `RadicacionDocumentosGuard`. |
| FE-06 | SCRUMCORE-292 | `PROMPT-FE-06-Inicio-Modulo-Estado-Activo-Contexto-Documental.md` | Ejecutado | El modulo consulta `estado-activo`, restaura o limpia contexto y controla el bootstrap. |
| TD-FE-04 | SCRUMCORE-293 | `PROMPT-TD-FE-04-Rutas-Tabs-Limpieza-UI-Prototipo.md` | Ejecutado y mergeado | Rutas/helpers centralizados, tabs semanticas y limpieza de UI mock. |
| FE-05 | SCRUMCORE-298 | `PROMPT-FE-05-Modal-Pendientes-AppTable-Asignacion-Radicado.md` | Ejecutado | Modal de pendientes usa `AppTable`, toma pendiente, actualiza contexto y navega a Documentos. |
| FE-07 | SCRUMCORE-299 | `PROMPT-FE-07-Enviar-Tramite-Activo-A-Pendiente.md` | Ejecutado | Devuelve tramite activo a pendiente con confirmacion, limpia contexto solo tras exito y refresca pendientes. |
| FE-01 | SCRUMCORE-303 | `PROMPT-FE-01-Conectar-Registro-Radicacion-Entrante.md` | Ejecutado | Conecta Radicar con `POST /api/radicacion/registrar-entrante`, adapter, service, hook y estado post-registro. |
| TD-FE-05 | SCRUMCORE-300 | `PROMPT-TD-FE-05-Limpiar-Formulario-Radicacion-Entrante.md` | Ejecutado | Centraliza la semantica de limpiar captura sin tocar contexto documental, rutas ni backend. |
| TD-FE-03 | SCRUMCORE-302 | `PROMPT-TD-FE-03-Refactor-RadicacionForm-Secciones-Hooks.md` | Ejecutado incremental | Extrae footer, hook de tramite/flujo y mappers centralizados sin cambiar comportamiento funcional. |

## TD-FE-01 - Fuente Unica De Plantilla

### Alcance Implementado

- `RadicacionRoutePage` conserva la responsabilidad de cargar la plantilla.
- `RadicacionPage` recibe `plantilla` y `camposPlantilla`.
- `RadicacionTabs` propaga los datos hacia `RadicacionForm`.
- `RadicacionForm` deja de ser fuente de carga inicial de plantilla.

### Resultado Arquitectonico

```text
RadicacionRoutePage
  -> useCamposPlantilla()
  -> RadicacionPage
  -> RadicacionTabs
  -> RadicacionForm
```

### Restricciones Vigentes

- No reintroducir `useCamposPlantilla()` dentro de componentes internos.
- No duplicar la carga de `/api/PlantillaRadicado/listaPlantilla`.
- Mantener `camposPlantilla` y `plantilla` derivados de la misma consulta.

## TD-FE-02 - Contexto Documental Unico

### Alcance Implementado

- `RadicacionDocumentalProvider` administra el estado documental del modulo.
- `useRadicacionDocumentalContext` es el unico hook de acceso al contexto.
- `RadicacionDocumentosGuard` centraliza la regla de acceso a Documentos.
- `setContextoDocumental()` y `clearContextoDocumental()` quedan como operaciones oficiales.

### Regla Vigente

`Documentos` solo puede renderizar cuando se cumple:

```text
estadoActual === 0
AND requiereGestionDocumental === true
AND tieneTramiteDocumentalActivoEstado0 === true
AND idEstadoRadicado > 0
```

### Restricciones Vigentes

- No crear stores paralelos para estado documental.
- No activar `CapDocument` por consecutivo, gabinete, workflow o seleccion de fila.
- No duplicar la regla documental dentro de `RadicacionTabs`.

## FE-06 - Startup Guard Y Restauracion De Estado

### Alcance Implementado

- `RadicacionStartupGuard` consulta `GET /api/radicacion/pendientes/estado-activo`.
- Si existe tramite activo, restaura el `RadicacionDocumentalContext`.
- Si no existe tramite activo, limpia el contexto.
- Mientras inicializa, bloquea el render funcional con loading.
- Ante error de bootstrap, muestra estado de error con reintento.

### Resultado Arquitectonico

```text
RadicacionRoutePage
  -> RadicacionDocumentalProvider
  -> RadicacionStartupGuard
  -> RadicacionPage
```

### Restricciones Vigentes

- Las paginas no deben consultar `estado-activo`.
- Las paginas no deben restaurar ni limpiar contexto.
- El bootstrap pertenece al `RadicacionStartupGuard`.

## FE-07 - Enviar Tramite Activo A Pendiente

### Alcance Implementado

- `EnviarPendienteConfirmModal` muestra la accion solo cuando existe tramite documental activo.
- `useEnviarRadicadoPendiente` concentra la mutacion y la regla transaccional.
- `RadicacionForm` deja de mostrar la accion global `Enviar a Pendientes`.
- El service consume `POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente`.
- El contexto documental se limpia unicamente cuando backend confirma `estadoActual = 1`.
- Se invalidan estado activo, contador y tabla de pendientes.
- La navegacion vuelve a `RADICACION_ROUTES.root`.

### Resultado Arquitectonico

```text
RadicacionTabs
  -> EnviarPendienteConfirmModal
  -> useEnviarRadicadoPendiente()
  -> enviarRadicacionPendiente()
  -> clearContextoDocumental()
  -> RADICACION_ROUTES.root
```

### Restricciones Vigentes

- No limpiar `RadicacionDocumentalContext` antes de respuesta exitosa.
- No ejecutar esta mutacion desde componentes de formulario.
- No navegar ni desactivar Documentos si backend no confirma `estadoActual = 1`.
- No crear modelos o stores paralelos para pendientes.

## TD-FE-05 - Limpiar Formulario De Radicacion Entrante

### Alcance Implementado

- `SCRUMCORE-300` queda asociado a `PROMPT-TD-FE-05-Limpiar-Formulario-Radicacion-Entrante.md`.
- El reset de captura queda centralizado en `useRadicacionFormReset`.
- `RadicacionForm` consume `handleClearRadicacionForm` para el boton `Limpiar`.
- El reset restaura `selectedTramiteId`, `hasUserChangedTramite`, `resetKey`, `modalVisible`, `usuarioSeleccionado` y remonta autocompletes/hijos con estado interno.
- `Documentos IA` ya no reutiliza el reset de captura.

### Resultado Arquitectonico

```text
RadicacionForm
  -> useRadicacionFormReset()
  -> handleClearRadicacionForm()
  -> form.resetFields()
  -> reset estado local y componentes hijos
```

### Restricciones Vigentes

- No limpiar `RadicacionDocumentalContext`.
- No cambiar `estadoActual`, `idEstadoRadicado` ni `idTareaWorkflow`.
- No navegar.
- No llamar backend.
- No invalidar consultas de pendientes.
- No confundir limpiar formulario con enviar a pendiente o abandonar tramite.

### Validaciones Ejecutadas

```bash
npm test -- --run src/modules/radicacion/hooks/useRadicacionFormReset.spec.test.tsx src/modules/radicacion/components/RadicacionForm.spec.test.tsx
```

Resultado documentado:

```text
2 test files passed
33 tests passed
```

## TD-FE-03 - Refactor RadicacionForm Por Secciones Y Hooks

### Alcance Implementado

- `SCRUMCORE-302` queda asociado a `PROMPT-TD-FE-03-Refactor-RadicacionForm-Secciones-Hooks.md`.
- El refactor debe ejecutarse de forma incremental, sin cambiar comportamiento funcional.
- `RadicacionForm` evoluciona hacia un componente orquestador.
- El footer queda extraido como componente presentacional que recibe callbacks por props.
- La seleccion de tramite/flujo queda movida a `useRadicacionTramiteSelection`.
- La tolerancia a variantes backend queda centralizada en `utils/radicacionOptionMappers.ts`.
- `resolveCampoIdScript` y `normalizeCampoName` quedan fuera del JSX principal.
- `CamposPlantillaAutoCompleteRenderer` consume el mapper central para opciones dinamicas `SELECCION`.
- `RadicacionForm` deja de importar directamente `useFlujosRelacionadosTramite`.

### Resultado Arquitectonico Implementado

```text
RadicacionForm
  -> useRadicacionTramiteSelection()
  -> RadicacionFormFooter
  -> utils/radicacionOptionMappers
```

### Validaciones Ejecutadas

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

### Riesgos Residuales

- Remitente, destinatario, metadata y tramite section siguen pendientes para un siguiente corte incremental.
- `RadicacionForm.tsx` conserva deuda previa de lint por `any` y `set-state-in-effect`.
- `CamposPlantillaAutoCompleteRenderer.tsx` conserva deuda previa de lint por `set-state-in-effect`.
- `npx tsc -b` sigue fallando por deuda externa en `GestionRespuestaUploadDocumental.tsx`.

### Restricciones Vigentes

- No reintroducir `useCamposPlantilla` dentro de `RadicacionForm`.
- No mover mutaciones al footer.
- No tocar `RadicacionDocumentalContext`.
- No modificar rutas.
- No llamar backend desde secciones presentacionales.
- No cambiar el comportamiento visual salvo ajustes necesarios por extraccion.

## FE-01 - Conectar Registro De Radicacion Entrante

### Alcance Implementado

- `SCRUMCORE-303` queda asociado a `PROMPT-FE-01-Conectar-Registro-Radicacion-Entrante.md`.
- Se crea contrato frontend para request, response, envelope `AppResponses` y estado post-registro.
- Se crea `radicacionRegistro.service.ts` para consumir `POST /api/radicacion/registrar-entrante`.
- Se crea `radicacionRegistroRequest.mapper.ts` para construir `RegistrarRadicacionEntranteRequestDto` desde Ant Design Form y metadata de plantilla.
- Se crea `useRegistrarRadicacion` para manejar `submitting`, `success`, `error`, errores funcionales y estado post-registro.
- `RadicacionForm` conecta `Form.onFinish` al mapper y al hook de registro.
- `CamposPlantillaAutoCompleteRenderer` registra campos dinamicos con `name` para que entren al DTO.
- `RadicacionFormFooter` expone loading de submit para el boton `Radicar`.
- `RadicacionForm` mantiene `Número Folios` como campo fijo requerido en `Medio de Recepción del Trámite`; si la plantilla lo trae con valor, se precarga y bloquea; si no lo trae, se diligencia manualmente.
- `radicacionRegistroRequest.mapper.ts` deriva `Tipo_radicado_plantilla` desde `tipoRadicado` cuando backend lo exige dentro de `Campos`.
- `radicacionRegistroRequest.mapper.ts` usa la opcion seleccionada de `TipoRadicado` para llenar tanto `TipoRadicado` como `TipoPlantillaRadicado`; ambos IDs salen de `selected.idValue`.
- El payload frontend no incluye `ModuloRegistro`; backend lo resuelve internamente por `tipoModuloRadicacion=1`.
- `Número Folios` no debe duplicarse en `Datos Especializados` cuando se renderiza como campo fijo.
- Las reglas de validacion de campos dinamicos se centralizan en `utils/radicacionCampoValidation.ts` para traducir metadata backend (`obligatorio_campo`, `max_leng_campo`, `tipo_campo`, `disable_campo`) a reglas Ant Design.

### Resultado Arquitectonico

```text
RadicacionForm
  -> Form.onFinish
  -> buildRegistrarRadicacionEntranteRequest()
  -> useRegistrarRadicacion()
  -> registrarRadicacionEntrante()
  -> Backend moderno
  -> RadicacionPostRegistroState
```

### Validaciones Ejecutadas

```bash
npm test -- --run --testTimeout 10000 src/modules/radicacion/adapters/radicacionRegistroRequest.mapper.test.ts src/modules/radicacion/services/radicacionRegistro.service.test.ts src/modules/radicacion/hooks/useRegistrarRadicacion.spec.test.tsx src/modules/radicacion/components/RadicacionForm.spec.test.tsx src/modules/radicacion/components/CamposPlantillaAutoCompleteRenderer.spec.test.tsx
```

Resultado:

```text
5 test files passed
55 tests passed
```

### Riesgos Residuales

- La navegacion contextual post-registro y el workbench documental quedan fuera de alcance para fases posteriores.
- `/api/PlantillaRadicado/listaPlantilla` entrega lista de campos; `mapCamposPlantillaToPlantillaRadicado` puede construir `IdPlantillaRadicado = 0` si la respuesta no trae metadata completa de plantilla, pero ese valor no debe alimentar `TipoPlantillaRadicado.IdTipoPlantillaRdicado` en el registro.
- `TipoPlantillaRadicado.IdTipoPlantillaRdicado = 0` indica que `TipoRadicado` no resolvio una opcion seleccionada con `idValue > 0` o que el mapper dejo de usar la opcion seleccionada.
- `RAD_TXN_Q07` con `ModuloRegistro invalido para radicacion: RADICACION SIMPLIFICADA` queda del lado backend/Q07 si el request frontend ya usa `tipoModuloRadicacion=1` sin `ModuloRegistro`.
- Si aparecen errores de longitud en campos diligenciados, validar que el campo use `buildCampoPlantillaRules`; los selects y campos numericos no deben aplicar longitud textual.
- `npx tsc -b` sigue fallando por deuda externa en `GestionRespuestaUploadDocumental.tsx`.
- `RadicacionForm.tsx` conserva deuda previa de lint por `any` y `set-state-in-effect`.
- `CamposPlantillaAutoCompleteRenderer.tsx` conserva deuda previa de lint por `set-state-in-effect`.

### Restricciones Vigentes Adicionales

- No eliminar el campo fijo `numeroFolios` aunque la plantilla no lo traiga; backend puede exigirlo.
- No volver a renderizar `Numero_Folios` en `Datos Especializados` si ya existe el campo fijo.
- No crear otro desplegable visible para `Tipo_radicado_plantilla`; debe derivarse de `tipoRadicado`.
- No usar `plantilla.IdPlantillaRadicado` para `TipoPlantillaRadicado.IdTipoPlantillaRdicado`; debe salir del `idValue` seleccionado en `TipoRadicado`.
- Mantener `tipoModuloRadicacion=1` para este flujo.
- No enviar `ModuloRegistro` ni `moduloRegistro` por query ni en payload frontend.
- Si Q07 rechaza `RADICACION SIMPLIFICADA`, backend debe normalizar ese alias antes o dentro de `RegistroLogRespuestalBuilder.Build`.
- No duplicar reglas de validacion por campo en JSX; extender `radicacionCampoValidation.ts` cuando backend agregue nuevas restricciones.
- No aplicar `max_leng_campo` al label visible de selects ni como longitud textual para campos numericos.

### Restricciones Vigentes

- No consumir ASMX legacy.
- No calcular consecutivo en frontend.
- No llamar `clienteApi` desde componentes.
- No activar Documentos sin senial explicita del backend.

## TD-FE-04 - Rutas, Tabs Y Limpieza De Prototipo

### Alcance Implementado

- Las keys numericas de tabs fueron reemplazadas por keys de dominio:

```text
ia
radicacion
documentos
gestion-radicados
```

- Las rutas del modulo quedaron centralizadas en `radicacionRoutes`.
- `CapDocument` ya no inicializa el digitalizador con contexto mock.
- `CapDocument` ya no muestra gabinete ni documentos ficticios.
- `Modalpendiente` ya no muestra tabla ni datos mock en runtime.

### Pull Request

```text
PR #318 - SCRUMCORE-293 consolidar navegacion de radicacion
Merge commit: 3ce62785c4fab16f2efd966aaec0dfb2a05eeb69
```

### Restricciones Vigentes

- No usar keys numericas para tabs.
- No hardcodear rutas en componentes.
- No mostrar datos mock en runtime productivo.
- No implementar pendientes/AppTable dentro de TD-FE-04.

## Validaciones Ejecutadas

### Suite Focalizada De TD-FE-04

```bash
npm test -- --run src/modules/radicacion/hooks/RadicacionTabs.spec.test.tsx src/modules/radicacion/components/CapDocument.spec.test.tsx src/modules/radicacion/components/Modalpendiente.spec.test.tsx src/modules/radicacion/routes/radicacionRoutes.test.ts
```

Resultado documentado:

```text
4 test files passed
10 tests passed
```

### Suite De Infraestructura De Radicacion

```bash
npm test -- --run src/modules/radicacion/pages/RadicacionRoutePage.spec.test.tsx src/modules/radicacion/components/RadicacionStartupGuard.spec.test.tsx src/modules/radicacion/hooks/RadicacionTabs.spec.test.tsx src/modules/radicacion/components/RadicacionDocumentosGuard.spec.test.tsx src/modules/radicacion/context/RadicacionDocumentalContext.spec.test.tsx src/modules/radicacion/services/radicacionPendientes.service.test.ts src/modules/radicacion/routes/radicacionRoutes.test.ts
```

Resultado documentado:

```text
7 test files passed
22 tests passed
```

## Estado De Dependencias Para Siguientes Fases

### Puede Ejecutarse Sin Backend Nuevo De Pendientes

- `PROMPT-TD-FE-05-Limpiar-Formulario-Radicacion-Entrante.md`
- `PROMPT-TD-FE-03-Refactor-RadicacionForm-Secciones-Hooks.md`, despues de TD-FE-05

### Puede Ejecutarse Si Existe Endpoint De Registro

- `PROMPT-FE-01-Conectar-Registro-Radicacion-Entrante.md`

FE-01 no debe implementar FE-02, FE-03, FE-04, FE-05, FE-06 ni FE-07.

### Ya Ejecutado Contra APIs Modernas De Pendientes

- `PROMPT-FE-05-Modal-Pendientes-AppTable-Asignacion-Radicado.md`
- `PROMPT-FE-07-Enviar-Tramite-Activo-A-Pendiente.md`

### No Cerrar Contra Datos Reales Sin Backend De Pendientes

- `PROMPT-FE-04-Pendientes-Radicacion-Gestion-Documental.md`

## Notas De Continuidad

- FE-05 debe reutilizar el `RadicacionDocumentalContext` existente.
- FE-05 no debe crear un contexto paralelo.
- FE-05 debe extender el modelo documental solo si el contrato de `tomar` lo requiere.
- Los endpoints de listado, contador y tomar pendiente pertenecen al alcance de FE-05 y sus prompts BE asociados, no a TD-FE-04.
