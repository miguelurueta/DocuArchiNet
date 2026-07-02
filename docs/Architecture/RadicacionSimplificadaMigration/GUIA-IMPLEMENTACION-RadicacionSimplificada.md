# Guia de prompts - Migracion Radicacion Simplificada

## Objetivo

Ordenar los prompts arquitectonicos de migracion del modulo `src/modules/radicacion` para ejecutarlos uno a uno, conservando el mismo nivel de detalle tecnico del analisis base y evitando que queden redactados como tickets JIRA.

Documento fuente:

```txt
docs/Architecture/RadicacionSimplificadaMigration/Analisis-Migracion-Legacy-RadicadorSimplificado.md
```

Documentacion tecnica frontend:

```txt
docs/Architecture/RadicacionSimplificadaMigration/FrontendTechnicalDocumentation/00-Indice-Documentacion-Tecnica-Frontend.md
```

## Prompts disponibles

```txt
PROMPT-FE-01-Conectar-Registro-Radicacion-Entrante.md
PROMPT-FE-02-Navegacion-Contextual-Post-Radicacion.md
PROMPT-FE-03-Panel-Documental-Post-Radicacion.md
PROMPT-FE-04-Pendientes-Radicacion-Gestion-Documental.md
PROMPT-FE-05-Modal-Pendientes-AppTable-Asignacion-Radicado.md
PROMPT-FE-06-Inicio-Modulo-Estado-Activo-Contexto-Documental.md
PROMPT-FE-07-Enviar-Tramite-Activo-A-Pendiente.md
PROMPT-TD-FE-01-Unificar-Fuente-Plantilla-Radicacion.md
PROMPT-TD-FE-02-Contexto-Documental-Unico-Guards.md
PROMPT-TD-FE-03-Refactor-RadicacionForm-Secciones-Hooks.md
PROMPT-TD-FE-04-Rutas-Tabs-Limpieza-UI-Prototipo.md
PROMPT-TD-FE-05-Limpiar-Formulario-Radicacion-Entrante.md
PROMPT-BE-01-Consultas-Pendientes-Radicacion.md
PROMPT-BE-02-Mutaciones-Pendientes-Radicacion.md
PROMPT-BE-API-01-Listado-Radicados-Pendientes.md
PROMPT-BE-API-02-Estado-Activo-Radicacion.md
PROMPT-BE-API-03-Contador-Pendientes-Radicacion.md
PROMPT-BE-API-04-Enviar-Radicado-Pendiente.md
PROMPT-BE-API-05-Tomar-Radicado-Pendiente.md
```

## Orden recomendado

## Tabla ejecutiva de implementacion

| Orden | Frente | Prompt | Implementa | Depende de | Criterio de aceptacion |
|---:|---|---|---|---|---|
| 1 | Front deuda tecnica | `PROMPT-TD-FE-01-Unificar-Fuente-Plantilla-Radicacion.md` | Unifica la fuente de plantilla en `RadicacionRoutePage -> RadicacionPage -> RadicacionTabs -> RadicacionForm`; elimina doble carga de `/api/PlantillaRadicado/listaPlantilla`. | Codigo actual de `src/modules/radicacion`. | `RadicacionPage` usa `plantilla`; `RadicacionForm` no duplica `useCamposPlantilla`; no hay doble request; se elimina `console.log` de `useCamposPlantilla`. |
| 2 | Front deuda tecnica | `PROMPT-TD-FE-02-Contexto-Documental-Unico-Guards.md` | Crea contexto documental unico y guard para `Documentos`. | TD-FE-01. | Existe contexto documental; `Documentos` no renderiza sin `estado = 0`; FE-05, FE-06 y FE-07 tienen el mismo punto de integracion. |
| 3 | Backend | `PROMPT-BE-API-02-Estado-Activo-Radicacion.md` | Crea `GET /api/radicacion/pendientes/estado-activo` para detectar activo `estado = 0` y devolver contexto documental. | Contexto legacy validado; usuario/plantilla resueltos por claims. | Retorna `TieneActivoEstado0=true` con contexto para `Documentos`; retorna `false` sin error si no hay activo; no usa ASMX ni Session. |
| 4 | Front funcional | `PROMPT-FE-06-Inicio-Modulo-Estado-Activo-Contexto-Documental.md` | Al iniciar radicacion consulta `estado-activo`; restaura contexto; navega directo a `Documentos` si hay `estado = 0`. | TD-FE-02 y BE-API-02. | Con activo navega a `/dashboard/radicacion/registro/{idEstadoRadicado}/documentos`; sin activo inicia normal; error no activa `Documentos`. |
| 5 | Front deuda tecnica | `PROMPT-TD-FE-04-Rutas-Tabs-Limpieza-UI-Prototipo.md` | Limpia tabs/rutas/prototipos: keys semanticas, sin `console.log`, rutas objetivo y mocks no confundibles como datos reales. | TD-FE-02 y FE-06. | Tabs usan keys semanticas; rutas objetivo quedan implementadas o centralizadas; `CapDocument`/`Modalpendiente` no muestran mocks como datos reales. |
| 6 | Backend | `PROMPT-BE-API-01-Listado-Radicados-Pendientes.md` | Evoluciona `apListaRadicadosPendientes` para `AppTable`: `DynamicUiTableDto`, `id_tarea_workflow`, `tramite`, payload completo de `asignacion-tarea`; `POST` con query server si aplica. | BE-API-02 recomendado; tabla legacy `ra_rad_estados_modulo_radicacion`. | Lista solo `estado = 1`; incluye `id_estado_radicado`, `id_tarea_workflow`, `consecutivo_radicado`, `tramite`; accion `asignacion-tarea` transporta payload completo. |
| 7 | Backend | `PROMPT-BE-API-03-Contador-Pendientes-Radicacion.md` | Crea contador liviano `GET /api/radicacion/pendientes/contador`. | Usuario radicador y plantilla default. | Usa `COUNT(*)`; cuenta solo `estado = 1`; retorna `totalPendientes = 0` sin error cuando no hay registros. |
| 8 | Backend | `PROMPT-BE-API-05-Tomar-Radicado-Pendiente.md` | Crea `POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar`; cambia `estado 1 -> 0`; crea/relaciona workflow si `id_tarea_workflow = 0`. | BE-API-01 y BE-API-02. | Bloquea si ya hay activo `estado = 0`; deja `estadoActual = 0`; retorna contrato compatible con `estado-activo`; no deja cambios parciales si falla workflow. |
| 9 | Front funcional | `PROMPT-FE-05-Modal-Pendientes-AppTable-Asignacion-Radicado.md` | Reemplaza `Modalpendiente` mock/AntD Table por `AppTable`; lista pendientes y ejecuta `asignacion-tarea` para tomar radicado. | TD-FE-02, BE-API-01, BE-API-03 y BE-API-05. | Usa `AppTable`; no usa datos mock; `asignacion-tarea` llama `tomar`; con `estadoActual = 0` actualiza contexto y navega a `Documentos`; bloqueo backend no cierra modal. |
| 10 | Backend | `PROMPT-BE-API-04-Enviar-Radicado-Pendiente.md` | Crea `POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente`; cambia estrictamente `estado 0 -> 1`. | BE-API-02 y contexto documental. | Solo permite origen `estado = 0`; retorna `estadoActual = 1`; no exige `id_tarea_workflow`; no borra documentos/gabinete. |
| 11 | Front funcional | `PROMPT-FE-07-Enviar-Tramite-Activo-A-Pendiente.md` | Implementa accion UI `Enviar a pendiente` solo para tramite activo `estado = 0`; confirma, llama API, limpia contexto y desactiva `Documentos`. | TD-FE-02, FE-06, BE-API-04 y BE-API-03. | Boton solo aparece/aplica con `estado = 0`; exito limpia contexto; `Documentos` queda inactivo; contador/listado se refresca; error conserva contexto. |
| 12 | Front deuda tecnica | `PROMPT-TD-FE-05-Limpiar-Formulario-Radicacion-Entrante.md` | Centraliza la funcion `Limpiar` del formulario de radicacion entrante para resetear campos y estado local sin borrar contexto documental activo. | TD-FE-02 y formulario actual. | `Limpiar` deja el formulario listo para nueva radicacion; limpia tramite/flujo/remitente/destinatario/asunto/autocompletes; no borra `RadicacionDocumentalContext`; tests cubren limpieza y preservacion de contexto. |
| 13 | Front deuda tecnica | `PROMPT-TD-FE-03-Refactor-RadicacionForm-Secciones-Hooks.md` | Refactor incremental de `RadicacionForm` por secciones, hooks y mappers tipados. | Flujos funcionales estabilizados y TD-FE-05. | Footer/secciones extraidas; mappers centralizados; menos `any`; tests existentes siguen pasando y se agregan tests de hooks/mappers. |

Regla de lectura:

```txt
Los prompts TD-FE pagan deuda estructural.
Los prompts FE implementan comportamiento de negocio.
Los prompts BE-API implementan contratos backend atomicos.
```

### 1. FE-01 - Registro de radicacion entrante

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-01-Conectar-Registro-Radicacion-Entrante.md
```

Resultado esperado:

- `RadicacionPage` usa `plantilla`.
- `RadicacionForm` no duplica carga de campos.
- Existe mapper a `RegistrarRadicacionEntranteRequestDto`.
- Existe service para `POST /api/radicacion/registrar-entrante`.
- Existe hook `useRegistrarRadicacion`.
- Boton `Radicar` registra contra backend moderno.
- Se conserva estado post-registro:
  - `ConsecutivoRadicado`;
  - `IdRadicado`;
  - `IdEstadoRadicado`.
- Si backend/metadata indica gestion documental activa, se conserva:
  - `requiereGestionDocumental`;
  - `tieneTramiteDocumentalActivoEstado0`;
  - `destinoPostRegistro = "documentos"`.

No debe incluir:

- panel contextual;
- documentos;
- pendientes;
- workflow manual.

### 2. FE-02 - Navegacion contextual post-radicacion

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-02-Navegacion-Contextual-Post-Radicacion.md
```

Resultado esperado:

- Existe shell tipo `gestionCorrespondencia`.
- Existe ruta hija:

```txt
/dashboard/radicacion/registro/:idEstadoRadicado
```

- La ruta base sigue funcionando.
- El panel contextual muestra metadata post-registro.
- El panel contextual puede abrir directamente en `Documentos` solo cuando existe tramite documental activo en estado `0`.
- Si no existe tramite documental activo en estado `0`, `Documentos` queda inactivo.
- Hay boton de retorno a la pantalla base.
- No se depende de `idTareaWf` como llave obligatoria.

No debe incluir:

- workbench documental completo;
- upload;
- digitalizacion;
- visor.

### 3. FE-03 - Panel documental post-radicacion

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-03-Panel-Documental-Post-Radicacion.md
```

Resultado esperado:

- Existe contexto documental de radicacion.
- Existe service de gabinete por radicado.
- El panel post-registro muestra tabs `Resumen` y `Documentos`.
- Se resuelve gabinete desde:

```txt
GET /api/workflow/ruta-trabajo/radicados/{consecutivoRadicado}/gabinete
```

- El panel maneja loading, error, sin gabinete y ready.
- Si el usuario sale del modulo y reingresa a un radicado con tramite documental activo en estado `0`, el sistema debe determinarlo y entrar directamente a `Documentos`.
- Si no hay tramite documental activo en estado `0`, el panel `Documentos` no debe activarse ni para consulta.

No debe incluir:

- digitalizacion real;
- upload real;
- cambio de tipologia;
- eliminacion documental;
- workflow manual.

### 4. FE-04 - Pendientes de radicacion y toma de tramite documental

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-04-Pendientes-Radicacion-Gestion-Documental.md
```

Resultado esperado:

- Se migra el flujo legacy de subir a pendiente y tomar pendiente.
- La lista de pendientes consume API moderna y no datos mock.
- `estado = 1` representa radicado pendiente.
- `estado = 0` representa radicado activo/asignado para gestion documental.
- `Documentos` solo se activa despues de tomar/re-radicar un pendiente y dejarlo en `estado = 0`.
- Si el usuario ya tiene un radicado activo en `estado = 0`, no puede tomar otro pendiente.

No debe incluir:

- upload real;
- digitalizacion real;
- visor PDF final;
- reemplazo completo del workbench documental;
- consumo de ASMX.

### 5. BE-01 - APIs de consulta para pendientes

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-01-Consultas-Pendientes-Radicacion.md
```

Resultado esperado:

- Se reutiliza `GET /api/tramite/tramites/apListaRadicadosPendientes`.
- El listado incluye `id_tarea_workflow` y tramite.
- El listado queda alineado con `DynamicUiTableDto` para `AppTable`.
- La accion `asignacion-tarea` transporta `id_estado_radicado`, `id_tarea_workflow` y `consecutivo_radicado`.
- Existe `GET /api/radicacion/pendientes/estado-activo` para detectar activo `estado = 0` y restaurar contexto documental.
- Se define si se implementa o no `GET /api/radicacion/pendientes/contador`.
- No se hacen mutaciones de estado.

Uso recomendado:

```txt
Este prompt queda como contexto agrupador.
Para implementacion uno a uno usar BE-API-01, BE-API-02 y BE-API-03.
```

### 6. BE-02 - APIs de mutacion para pendientes

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-02-Mutaciones-Pendientes-Radicacion.md
```

Resultado esperado:

- Existe `POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente`.
- Existe `POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar`.
- `enviar-pendiente` cambia `estado 0 -> 1`.
- `tomar` cambia `estado 1 -> 0`.
- Si `id_tarea_workflow = 0`, backend crea/relaciona workflow antes de activar documentos.
- No se activa `Documentos` si no queda `estado = 0`.

Uso recomendado:

```txt
Este prompt queda como contexto agrupador.
Para implementacion uno a uno usar BE-API-04 y BE-API-05.
```

### 7. Prompts backend atomicos por API

Archivos:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-API-01-Listado-Radicados-Pendientes.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-API-02-Estado-Activo-Radicacion.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-API-03-Contador-Pendientes-Radicacion.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-API-04-Enviar-Radicado-Pendiente.md
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-BE-API-05-Tomar-Radicado-Pendiente.md
```

Orden recomendado backend atomico:

```txt
BE-API-01 listado DynamicUiTable/AppTable
  -> BE-API-02 estado activo/contexto documental
  -> BE-API-03 contador
  -> BE-API-04 enviar a pendiente
  -> BE-API-05 tomar pendiente
```

Decision sobre contador:

```txt
Se crea API propia de contador para no cargar la tabla completa cuando frontend solo necesita badge.
```

Orden recomendado frontend/backend integrado:

```txt
BE-API-02 estado activo/contexto documental
  -> FE-06 inicio con estado activo
  -> BE-API-01 listado DynamicUiTable/AppTable
  -> BE-API-03 contador
  -> BE-API-05 tomar pendiente
  -> FE-05 modal AppTable y asignacion
  -> BE-API-04 enviar pendiente
  -> FE-07 enviar activo a pendiente
```

### 8. FE-05 - Modal de pendientes con AppTable y asignacion de radicado

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-05-Modal-Pendientes-AppTable-Asignacion-Radicado.md
```

Resultado esperado:

- `Modalpendiente.tsx` conserva el punto de entrada visual existente.
- Se elimina `antd/Table` para la lista de pendientes.
- Se elimina la data mock hardcodeada.
- El modal renderiza `src/app/Components/UI/AppTable`.
- El listado consume `POST /api/tramite/tramites/apListaRadicadosPendientes` si existe paginacion server; `GET` queda como compatibilidad temporal.
- El contador consume `GET /api/radicacion/pendientes/contador` si esta disponible.
- La accion DynamicUiTable `asignacion-tarea` dispara la toma del radicado.
- La toma consume `POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar`.
- La tabla transporta `id_estado_radicado`, `id_tarea_workflow` y `consecutivo_radicado`.
- `Documentos` solo se activa cuando la toma responde `estadoActual = 0`.
- Si backend bloquea por tarea activa existente, no se cierra el modal ni se navega.

No debe incluir:

- upload documental;
- digitalizacion;
- visor PDF;
- endpoint backend nuevo;
- envio a pendiente desde formulario;
- nueva abstraccion de tabla;
- consumo de ASMX.

### 9. FE-06 - Inicio del modulo con estado activo y contexto documental

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-06-Inicio-Modulo-Estado-Activo-Contexto-Documental.md
```

Resultado esperado:

- Al iniciar `src/modules/radicacion`, se consulta `GET /api/radicacion/pendientes/estado-activo`.
- Si existe activo `estado = 0`, se restaura contexto documental.
- Si existe activo `estado = 0`, se navega directo a `Documentos`.
- Si no existe activo, el modulo inicia normal.
- `Documentos` queda inactivo sin `estado = 0`.
- Se bloquea tomar otro pendiente cuando ya hay activo.

No debe incluir:

- listado AppTable;
- tomar pendiente;
- enviar a pendiente;
- upload;
- digitalizacion.

### 10. FE-07 - Enviar tramite activo a pendiente

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-07-Enviar-Tramite-Activo-A-Pendiente.md
```

Resultado esperado:

- La accion `Enviar a pendiente` solo aparece/aplica con tramite documental activo `estado = 0`.
- La accion confirma antes de mutar.
- Consume `POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente`.
- Si backend responde `estadoActual = 1`, se limpia contexto documental activo.
- `Documentos` queda inactivo despues de enviar a pendiente.
- Se refresca contador/listado de pendientes.

No debe incluir:

- tomar pendiente;
- listado AppTable;
- backend;
- carga documental.

### 11. TD-FE-01 - Unificar fuente de plantilla

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-TD-FE-01-Unificar-Fuente-Plantilla-Radicacion.md
```

Resultado esperado:

- `RadicacionPage` usa `plantilla`.
- `RadicacionForm` no duplica `useCamposPlantilla`.
- No hay doble request a lista de plantilla.
- Se elimina `console.log` de `useCamposPlantilla`.

### 12. TD-FE-02 - Contexto documental unico y guards

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-TD-FE-02-Contexto-Documental-Unico-Guards.md
```

Resultado esperado:

- Existe contexto documental unico.
- `Documentos` queda bloqueado sin `estado = 0`.
- FE-05, FE-06 y FE-07 comparten el mismo punto de integracion.

### 13. TD-FE-03 - Refactor RadicacionForm

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-TD-FE-03-Refactor-RadicacionForm-Secciones-Hooks.md
```

Resultado esperado:

- `RadicacionForm` queda dividido por secciones/hooks.
- Mappers de opciones se centralizan.
- Se reduce uso de `any` y casts repetidos.
- Tests existentes siguen pasando.

### 14. TD-FE-04 - Rutas, tabs y limpieza de prototipo

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-TD-FE-04-Rutas-Tabs-Limpieza-UI-Prototipo.md
```

Resultado esperado:

- Tabs usan keys semanticas.
- No hay `console.log` runtime.
- Rutas objetivo quedan implementadas o centralizadas.
- `CapDocument` y `Modalpendiente` no muestran mocks como datos reales.

### 15. TD-FE-05 - Limpiar formulario de radicacion entrante

Archivo:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-TD-FE-05-Limpiar-Formulario-Radicacion-Entrante.md
```

Resultado esperado:

- Existe una funcion/hook unico para limpiar el formulario.
- El boton `Limpiar` usa esa funcion.
- Se limpian campos, tramite, flujo, remitente, destinatario, asunto, autocompletes y validaciones.
- No se borra contexto documental activo `estado = 0`.
- Tests cubren limpieza completa y preservacion de contexto.

## Dependencias entre prompts

```txt
FE-01
  └─ produce estado post-registro
       └─ FE-02 usa idEstadoRadicado para abrir panel
            └─ FE-03 usa ConsecutivoRadicado para resolver gabinete/documentos
                 └─ FE-04 gobierna pendientes y activacion real de Documentos por estado 0

Backend paralelo para FE-04:

BE-01 Consultas pendientes
  └─ BE-02 Mutaciones pendientes
       └─ FE-04 consume listado, estado activo y transiciones

Atomicos para ejecucion backend:

BE-API-01 listado DynamicUiTable/AppTable
  └─ BE-API-02 estado activo/contexto documental
       └─ BE-API-03 contador
            └─ BE-API-04 enviar pendiente
                 └─ BE-API-05 tomar pendiente
                      ├─ FE-06 inicio con estado activo
                      ├─ FE-05 modal AppTable y asignacion
                      └─ FE-07 enviar activo a pendiente
```

Ejecucion integrada recomendada:

```txt
TD-FE-01
  └─ TD-FE-02
      └─ BE-API-02
          └─ FE-06
              └─ TD-FE-04
                  └─ BE-API-01
                      └─ BE-API-03
                          └─ BE-API-05
                              └─ FE-05
                                  └─ BE-API-04
                                      └─ FE-07
                                          └─ TD-FE-05
                                              └─ TD-FE-03
```

Diagrama:

```txt
┌─────────────────────┐
│ FE-01 Registro      │
│ API registrar       │
│ estado post-reg     │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ FE-02 Navegacion    │
│ shell contextual    │
│ /registro/:idEstado │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ FE-03 Documentos    │
│ gabinete por radic. │
│ tabs Resumen/Docs   │
└─────────────────────┘
```

## Regla de uso

Cada prompt debe usarse como una unidad tecnica independiente.

No mezclar fases salvo que una dependencia tecnica sea inevitable y quede justificada dentro del mismo prompt de trabajo.

## APIs principales

Registro:

```txt
POST /api/radicacion/registrar-entrante
POST /api/radicacion/validar-entrante
```

Plantilla/autocomplete:

```txt
GET  /api/PlantillaRadicado/listaPlantilla
POST /api/PlantillaRadicado/autoCompleteTercero
POST /api/PlantillaRadicado/solicitaAutoCompleteDestinatarioRestriccion
POST /api/PlantillaRadicado/solicitaAutoCompleteCampos
POST /api/PlantillaRadicado/solicitaAutoCompleteTokenRadicado
POST /api/PlantillaRadicado/solicitaAutoCompleteTokenExpedienteRadicado
```

Tramite:

```txt
GET /api/tramite/tramites/empsolicitaListaflujosRelacionadosTramite
GET /api/tramite/tramites/solicitaEstructuraRelacionTipoRestriccion
GET /api/tramite/tramites/solicitaFechaLimiteRespuesta
GET /api/tramite/tramites/apListaRadicadosPendientes
```

Gabinete:

```txt
GET /api/workflow/ruta-trabajo/radicados/{consecutivoRadicado}/gabinete
GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete
```

## Criterio de avance

Avanzar a FE-02 solo cuando FE-01 pueda registrar y producir `IdEstadoRadicado`.

Avanzar a FE-03 solo cuando FE-02 pueda abrir el panel contextual con metadata post-registro.

La entrada directa a `Documentos` debe quedar contemplada desde FE-02, aunque la resolucion completa del gabinete se implemente en FE-03.
La condicion de activacion es estricta: debe existir tramite documental activo en estado `0`.

Avanzar a FE-04 cuando exista shell contextual y se pueda inactivar/activar `Documentos` desde contexto tipado.

Para implementar FE-04 completo, BE-01 y BE-02 deben existir o quedar simulados solo en tests. No cerrar FE-04 contra datos reales si faltan las mutaciones backend.

## Riesgos a controlar

- Duplicar carga de plantilla.
- Mezclar registro con documentos.
- Depender de `idTareaWf` cuando no siempre existe.
- Perder la senial de tramite documental activo en estado `0` al salir del modulo.
- Activar `Documentos` sin tramite documental activo en estado `0`.
- Confundir `estado = 1` con activo documental; en legacy `estado = 1` es pendiente y no debe activar `Documentos`.
- Tomar un pendiente sin validar primero si el usuario ya tiene activo `estado = 0`.
- Abrir siempre `Resumen` aunque el radicado requiera completar documentos por tramite documental activo en estado `0`.
- Volver a introducir patrones legacy: jQuery, ASMX, variables globales.
- Crear endpoints nuevos sin validar que ya existen.
- Hacer cambios visuales grandes junto con cambios transaccionales.

## Corte posterior sugerido

Despues de FE-03, crear un nuevo prompt:

```txt
PROMPT-FE-04-Integrar-Digitalizacion-Upload-Radicacion.md
```

Objetivo futuro:

- integrar `AppDigitalizador`;
- integrar `AppUpload`;
- listar documentos;
- ver PDF;
- cambiar tipologia documental.

