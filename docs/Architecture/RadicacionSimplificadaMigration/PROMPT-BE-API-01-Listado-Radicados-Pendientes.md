# PROMPT ARQUITECTONICO - Backend Radicacion
# API 01 - Evolucion del listado de radicados pendientes para AppTable

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Backend senior especialista en:

- .NET API por capas Controller -> Service -> Repository;
- DapperCrudEngine y QueryOptions;
- AppResponses<T> como contrato estandar;
- DynamicUiTableDto y DynamicUiTableBuilder;
- integracion backend para `src/app/Components/UI/AppTable`;
- migracion legacy WebForms/ASMX hacia API moderna;
- consultas parametrizadas;
- paginacion server, busqueda, ordenamiento y acciones de tabla;
- pruebas unitarias y de contrato.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Evolucionar la API existente de listado de radicados pendientes para que pueda alimentar directamente el modal frontend de pendientes implementado con `AppTable`.

API existente:

```txt
GET /api/tramite/tramites/apListaRadicadosPendientes
```

Esta API ya existe. No crear una API paralela para listar pendientes.

El objetivo es convertirla en un contrato `DynamicUiTableDto` suficientemente completo para:

1. renderizar la tabla con los datos funcionales del legacy;
2. transportar los identificadores necesarios para tomar/asignar un pendiente;
3. soportar el consumo desde `AppTable`;
4. quedar alineada, hasta donde aplique, con el patron backend usado por `gestionCorrespondencia`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Frontend consumidor:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-05-Modal-Pendientes-AppTable-Asignacion-Radicado.md
src/modules/radicacion/components/Modalpendiente.tsx
src/app/Components/UI/AppTable/AppTable.tsx
src/app/Components/UI/AppTable/AppTable.types.ts
```

Backend actual de pendientes:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\Radicacion\Tramite\TramiteController.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services\Service\Radicacion\Tramite\ListaRadicadosPendientesService.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\Radicador\Tramite\ListaRadicadosPendientesRepository.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\Radicacion\Tramite\ListaRadicadosPendientesDto.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services\Service\Mapping\Radicacion\Tramite\ListaRadicadosPendientesMapping.cs
```

Patron de referencia `gestionCorrespondencia`:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\WorkflowInboxGestion\WorkflowInboxController.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\Workflow\BandejaCorrespondencia\WorkflowInboxApiRequestDto.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services\Service\Workflow\BandejaCorrespondencia\WorkflowInboxService.cs
src/modules/gestionCorrespondencia/hooks/useGestionCorrespondenciaTable.ts
```

Pruebas actuales relacionadas:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\tests\TramiteDiasVencimiento.Tests\ListaRadicadosPendientesServiceTests.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\tests\TramiteDiasVencimiento.Tests\ListaRadicadosPendientesRepositoryTests.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\tests\TramiteDiasVencimiento.Tests\TramiteControllerContractTests.cs
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## HALLAZGO QUIRURGICO ACTUAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

La API actual ya retorna:

```cs
AppResponses<DynamicUiTableDto>
```

Esto es correcto y debe conservarse.

La brecha no es crear tabla dinamica desde cero. La brecha es que el contrato actual no lleva todos los datos necesarios para la asignacion/toma del radicado pendiente.

### Controller actual

Ubicacion:

```txt
DocuArchi.Api\Controllers\Radicacion\Tramite\TramiteController.cs
```

Metodo:

```txt
ApListaRadicadosPendientes()
```

Ruta:

```txt
[HttpGet("tramites/apListaRadicadosPendientes")]
```

Comportamiento actual:

- valida claim `defaulalias`;
- valida claim `usuarioid`;
- convierte `usuarioid` a `int`;
- llama `IListaRadicadosPendientesService.SolicitaListaRadicadosPendientes`;
- retorna `AppResponses<DynamicUiTableDto>`.

### Service actual

Ubicacion:

```txt
MiApp.Services\Service\Radicacion\Tramite\ListaRadicadosPendientesService.cs
```

Problemas detectados:

- arma `TableId = "lista-radicados-pendientes"`;
- usa `Page = 1`;
- usa `PageSize = dto.Count`;
- no recibe request de busqueda/paginacion/ordenamiento;
- no expone `id_tarea_workflow`;
- no expone `tramite`;
- la accion `asignacion-tarea` solo transporta `id_estado_radicado`.

### Repository actual

Ubicacion:

```txt
MiApp.Repository\Repositorio\Radicador\Tramite\ListaRadicadosPendientesRepository.cs
```

Columnas actuales:

```txt
id_estado_radicado
system_plantilla_radicado_id_Plantilla
consecutivo_radicado
remitente
fecha_registro
id_usuario_radicado
estado
```

Faltan:

```txt
id_tarea_workflow
tipo_doc_entrante_id_Tipo_Doc_Entrante
tramite / Descripcion_Doc
```

### DTO actual

Ubicacion:

```txt
MiApp.DTOs\DTOs\Radicacion\Tramite\ListaRadicadosPendientesDto.cs
```

Campos actuales:

```txt
id_estado_radicado
consecutivo_radicado
remitente
fecha_registro
opciones
```

Faltan:

```txt
id_tarea_workflow
tramite
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO LEGACY FUNCIONAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Legacy revisado:

```txt
Service_Solicita_radicados_pendientes_radicacion
Class_ra_rad_estados_modulo_radicacion.Solicita_radicados_pendientes_radicacion
```

SQL funcional legacy:

```sql
SELECT
  id_estado_radicado,
  rre.consecutivo_radicado AS RADICADO,
  rre.remitente AS REMITENTE,
  tde.Descripcion_Doc AS TRAMITE,
  rre.fecha_registro AS FECHA,
  rre.id_tarea_workflow AS id_tarea_wf
FROM ra_rad_estados_modulo_radicacion AS rre
LEFT OUTER JOIN tipo_doc_entrante AS tde
  ON tde.id_Tipo_Doc_Entrante = rre.tipo_doc_entrante_id_Tipo_Doc_Entrante
WHERE rre.id_usuario_radicado = @idUsuarioRadicador
  AND rre.system_plantilla_radicado_id_Plantilla = @idPlantilla
  AND rre.estado = 1
ORDER BY id_estado_radicado DESC
```

Semantica:

```txt
estado = 1
  pendiente; aparece en la lista.

estado = 0
  activo/asignado para gestion documental; no aparece en esta lista.
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## DECISION ARQUITECTONICA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No crear endpoint paralelo de listado.

Evolucionar la API existente:

```txt
GET /api/tramite/tramites/apListaRadicadosPendientes
```

Y, si se decide alinear plenamente con `gestionCorrespondencia`, agregar variante `POST` sobre la misma responsabilidad:

```txt
POST /api/tramite/tramites/apListaRadicadosPendientes
```

La variante `POST` debe recibir request de tabla y permitir paginacion server, busqueda y ordenamiento.

Regla de compatibilidad:

- mantener el `GET` temporalmente si ya hay consumidores;
- usar el `POST` desde el nuevo frontend con `AppTable` si se implementa paginacion server real;
- ambos deben retornar `AppResponses<DynamicUiTableDto>`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

DTO minimo:

```cs
public sealed class ListaRadicadosPendientesDto
{
    public long id_estado_radicado { get; set; }
    public string consecutivo_radicado { get; set; } = string.Empty;
    public string remitente { get; set; } = string.Empty;
    public string tramite { get; set; } = string.Empty;
    public string fecha_registro { get; set; } = string.Empty;
    public long id_tarea_workflow { get; set; }
}
```

Request opcional recomendado si se implementa `POST`:

```cs
public sealed class ListaRadicadosPendientesQueryRequestDto
{
    public int SearchType { get; set; } = 1;
    public string Search { get; set; } = string.Empty;
    public string SortField { get; set; } = "id_estado_radicado";
    public string SortDir { get; set; } = "DESC";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public bool IncludeConfig { get; set; } = true;
    public List<DynamicUiStructuredFilterDto> StructuredFilters { get; set; } = [];
}
```

Si ya existe un DTO comun para query DynamicUiTable en el repo, reutilizarlo. No duplicar tipos si `DynamicUiTableQueryRequestDto` ya cubre el caso.

Respuesta:

```cs
AppResponses<DynamicUiTableDto>
```

TableId recomendado:

```txt
lista-radicados-pendientes
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## COLUMNAS DYNAMIC UI TABLE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Columnas esperadas:

```txt
id_estado_radicado
  hidden
  number
  requerido para RowIdField

id_tarea_workflow
  hidden
  number
  requerido para tomar/asignar pendiente

consecutivo_radicado
  visible
  text
  header: Numero Radicado

remitente
  visible
  text
  header: Remitente

tramite
  visible
  text
  header: Tramite

fecha_registro
  visible
  date
  header: Fecha

actions
  visible
  action column
  contiene accion asignacion-tarea
```

No exponer campos tecnicos visibles salvo que sean necesarios para operacion.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ACCION DE FILA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

La tabla debe entregar accion:

```txt
ActionId = "asignacion-tarea"
Placement = "row"
Presentation = "button"
Behavior = "client_event"
Tone = "primary"
```

Request de accion:

```cs
new DynamicUiActionRequestDto
{
    RowIdField = "id_estado_radicado",
    PayloadFields = new Dictionary<string, string>
    {
        ["id_estado_radicado"] = "id_estado_radicado",
        ["id_tarea_workflow"] = "id_tarea_workflow",
        ["consecutivo_radicado"] = "consecutivo_radicado"
    }
}
```

No ejecutar la asignacion dentro del endpoint de listado.

El listado solo debe declarar la accion para que frontend la capture con:

```ts
onActionTriggered
```

La mutacion real corresponde a:

```txt
POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REPOSITORY
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Requisitos:

- mantener filtro `estado = 1`;
- filtrar por `idUsuarioRadicacion`;
- filtrar por plantilla default resuelta por service;
- incluir `id_tarea_workflow`;
- incluir `tipo_doc_entrante_id_Tipo_Doc_Entrante`;
- resolver `tramite` desde `tipo_doc_entrante.Descripcion_Doc`.

Si `QueryOptions` permite joins de forma segura, usarlo.

Si `QueryOptions` no soporta join para esta forma, usar el mecanismo SQL parametrizado ya aceptado por el repo. No concatenar valores manualmente.

Consulta objetivo funcional:

```sql
SELECT
  rre.id_estado_radicado,
  rre.system_plantilla_radicado_id_Plantilla,
  rre.consecutivo_radicado,
  rre.remitente,
  COALESCE(tde.Descripcion_Doc, '') AS tramite,
  rre.fecha_registro,
  rre.id_usuario_radicado,
  rre.estado,
  COALESCE(rre.id_tarea_workflow, 0) AS id_tarea_workflow
FROM ra_rad_estados_modulo_radicacion AS rre
LEFT JOIN tipo_doc_entrante AS tde
  ON tde.id_Tipo_Doc_Entrante = rre.tipo_doc_entrante_id_Tipo_Doc_Entrante
WHERE rre.system_plantilla_radicado_id_Plantilla = @idPlantilla
  AND rre.id_usuario_radicado = @idUsuarioRadicacion
  AND rre.estado = 1
ORDER BY rre.id_estado_radicado DESC
```

Si se implementa paginacion:

- aplicar `LIMIT/OFFSET` o mecanismo equivalente del engine;
- retornar `TotalRecords`;
- no cargar toda la tabla para luego paginar en memoria.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## SERVICE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Responsabilidades:

- validar `idUsuarioGestion > 0`;
- validar `defaultDbAlias`;
- resolver usuario radicador relacionado desde usuario gestion;
- resolver plantilla default;
- consultar pendientes en repository;
- mapear a `ListaRadicadosPendientesDto`;
- construir `DynamicUiTableDto`;
- declarar columnas;
- declarar accion `asignacion-tarea`;
- retornar `Sin resultados` con `success = true` cuando no haya registros.

Si se implementa request paginado:

- normalizar `Page`;
- normalizar `PageSize`;
- limitar `PageSize` maximo;
- normalizar `SortField` contra allowlist;
- normalizar `SortDir` a `ASC|DESC`;
- normalizar `Search`;
- pasar esos parametros a repository;
- construir `DynamicUiTableBuildInput.Total` con total real.

Allowlist de ordenamiento recomendada:

```txt
id_estado_radicado
consecutivo_radicado
remitente
tramite
fecha_registro
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTROLLER
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Mantener comportamiento del `GET` actual:

```txt
GET /api/tramite/tramites/apListaRadicadosPendientes
```

Agregar `POST` solo si se implementa query server completa:

```txt
POST /api/tramite/tramites/apListaRadicadosPendientes
```

Ambos deben:

- validar `defaulalias`;
- validar `usuarioid`;
- retornar `BadRequest` si faltan claims;
- retornar error controlado si `usuarioid` no es entero;
- delegar reglas al service;
- no contener SQL;
- no contener logica de construccion de tabla.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## COMPATIBILIDAD CON GESTIONCORRESPONDENCIA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

`gestionCorrespondencia` ya usa este patron:

```txt
Controller POST
  -> Service recibe request
  -> Repository consulta con page/search/sort
  -> Service arma DynamicUiTableBuildInput
  -> DynamicUiTableBuilder
  -> AppResponses<DynamicUiTableDto>
  -> frontend mapAppGridRowsToAppTableRows/mapAppGridColumnsToAppTableColumns
```

Pendientes debe acercarse a ese mismo patron.

Diferencia aceptable:

- pendientes puede conservar `GET` de compatibilidad;
- para el nuevo modal con `AppTable`, se recomienda `POST` si se necesita paginacion server real.

No aceptable:

- endpoint paralelo con otro nombre para la misma lista;
- respuesta plana sin `DynamicUiTableDto`;
- accion sin payload suficiente;
- paginar en frontend cargando todos los pendientes si el volumen puede crecer.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## PRUEBAS REQUERIDAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actualizar:

```txt
DocuArchiCore\tests\TramiteDiasVencimiento.Tests\ListaRadicadosPendientesServiceTests.cs
DocuArchiCore\tests\TramiteDiasVencimiento.Tests\ListaRadicadosPendientesRepositoryTests.cs
DocuArchiCore\tests\TramiteDiasVencimiento.Tests\TramiteControllerContractTests.cs
```

Casos minimos:

- retorna `DynamicUiTableDto` con datos;
- incluye columna hidden `id_estado_radicado`;
- incluye columna hidden `id_tarea_workflow`;
- incluye columna visible `tramite`;
- filas incluyen `id_estado_radicado`;
- filas incluyen `id_tarea_workflow`;
- filas incluyen `consecutivo_radicado`;
- accion `asignacion-tarea` existe;
- accion tiene `RowIdField = id_estado_radicado`;
- accion tiene payload `id_estado_radicado`;
- accion tiene payload `id_tarea_workflow`;
- accion tiene payload `consecutivo_radicado`;
- no hay registros: `success = true`, mensaje `Sin resultados`;
- alias faltante: `BadRequest`;
- usuarioid faltante: `BadRequest`;
- usuarioid no entero: error controlado;
- repository filtra `estado = 1`;
- repository no retorna radicados `estado = 0`;
- si se implementa POST, respeta `Page`, `PageSize`, `Search`, `SortField`, `SortDir`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CRITERIOS DE ACEPTACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- No se crea una API paralela para listar pendientes.
- El endpoint existente conserva `AppResponses<DynamicUiTableDto>`.
- La respuesta incluye `id_tarea_workflow`.
- La respuesta incluye `tramite`.
- La tabla conserva `id_estado_radicado` oculto.
- La tabla conserva `id_tarea_workflow` oculto.
- La accion `asignacion-tarea` transporta:
  - `id_estado_radicado`;
  - `id_tarea_workflow`;
  - `consecutivo_radicado`.
- El listado solo lista `estado = 1`.
- El listado no activa documentos.
- El listado no toma/asigna radicados.
- La asignacion queda reservada para `POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar`.
- Las pruebas actuales quedan actualizadas al nuevo contrato.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FUERA DE ALCANCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No implementar en este prompt:

- contador de pendientes;
- estado activo;
- tomar pendiente;
- enviar a pendiente;
- carga documental;
- digitalizacion;
- cambios en frontend;
- endpoints ASMX;
- workflow de creacion/asignacion.

APIs separadas:

```txt
GET  /api/radicacion/pendientes/contador
GET  /api/radicacion/pendientes/estado-activo
POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar
POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## NOTA FINAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Este prompt desbloquea el frontend `FE-05`.

Sin `id_tarea_workflow` y sin payload completo en `asignacion-tarea`, el modal puede mostrar la lista, pero no puede tomar/asignar el radicado pendiente con trazabilidad suficiente ni activar correctamente el panel `Documentos` despues de la mutacion.
