# PROMPT ARQUITECTONICO - Backend Radicacion
# Fase BE-01 - Completar consultas existentes para pendientes de radicacion

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL ESPERADO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Backend .NET senior especializado en:

- migracion legacy VB/WebForms hacia API .NET moderna;
- arquitectura por capas Controller -> Service -> Repository;
- DapperCrudEngine y QueryOptions;
- contratos `AppResponses<T>`;
- DynamicUiTable;
- claims de usuario (`defaulalias`, `usuarioid`);
- consultas parametrizadas;
- trazabilidad funcional de radicacion;
- compatibilidad con frontend React.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Completar la API de consulta ya existente para el flujo de pendientes de radicacion:

1. reutilizar y ampliar el listado moderno de pendientes que ya existe en el repo;
2. exponer si el usuario ya tiene un radicado activo para gestion documental (`estado = 0`);
3. exponer contador de pendientes si no se decide calcularlo desde el listado.

Esta fase NO debe cambiar estados. Solo debe consultar estado remoto con contratos suficientes para que frontend decida si puede mostrar pendientes, tomar un radicado o activar `Documentos`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO OBLIGATORIO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Prompt funcional origen:

```txt
docs/Architecture/RadicacionSimplificadaMigration/PROMPT-FE-04-Pendientes-Radicacion-Gestion-Documental.md
```

Controllers modernos:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\Radicacion\Tramite\TramiteController.cs
```

Services modernos:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services\Service\Radicacion\Tramite\ListaRadicadosPendientesService.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services\Service\Mapping\Radicacion\Tramite\ListaRadicadosPendientesMapping.cs
```

Repositories modernos:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\Radicador\Tramite\ListaRadicadosPendientesRepository.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\Radicador\PlantillaRadicado\RaRadEstadosModuloRadicacionR.cs
```

DTO moderno:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\Radicacion\Tramite\ListaRadicadosPendientesDto.cs
```

Documentacion tecnica de referencia:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## INVENTARIO ACTUAL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Ya existe en el repo:

```txt
GET /api/tramite/tramites/apListaRadicadosPendientes
```

Controller:

```txt
TramiteController.ApListaRadicadosPendientes()
```

Dependencias actuales:

```txt
IListaRadicadosPendientesService
IListaRadicadosPendientesRepository
IRemitDestInternoR
ISystemPlantillaRadicadoR
IDynamicUiTableBuilder
IMapper
```

El endpoint ya:

- lee `defaulalias` desde claims;
- lee `usuarioid` desde claims;
- resuelve usuario radicador relacionado al usuario de gestion;
- resuelve plantilla de radicacion default;
- consulta `ra_rad_estados_modulo_radicacion` filtrando `estado = 1`;
- retorna `AppResponses<DynamicUiTableDto>`.

Por tanto, esta fase NO debe crear un nuevo listado paralelo. Debe ajustar lo existente donde falte contrato para el flujo FE-04.

Estado actual confirmado:

```txt
Controller existe:
TramiteController.ApListaRadicadosPendientes()

Service existe:
ListaRadicadosPendientesService.SolicitaListaRadicadosPendientes(...)

Repository existe:
ListaRadicadosPendientesRepository.SolicitaListaRadicadosPendientes(...)

DTO existe:
ListaRadicadosPendientesDto
```

Brechas reales del listado actual:

- el repository actual no selecciona `id_tarea_workflow`;
- el repository actual no hace join con `tipo_doc_entrante`;
- el DTO actual no expone `id_tarea_workflow`;
- el DTO actual no expone `tramite`;
- las columnas DynamicUiTable no incluyen `tramite`;
- la accion `asignacion-tarea` solo transporta `id_estado_radicado`;
- no existe API de `estado-activo`;
- no existe API de `contador`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO LEGACY EXTRAIDO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Legacy listado:

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

Legacy validacion activo:

```txt
Service_solicita_estado_radicado_asignado_usuario_gestion_documentos
Class_ra_rad_estados_modulo_radicacion.Solicita_estado_radicado_asignado_usuario_gestion_documentos
```

SQL funcional:

```sql
SELECT id_estado_radicado
FROM ra_rad_estados_modulo_radicacion
WHERE estado = 0
  AND id_usuario_radicado = @idUsuarioRadicador
  AND system_plantilla_radicado_id_Plantilla = @idPlantilla
```

Legacy contador:

```txt
Service_solicita_numero_radicados_pendientes
Class_ra_rad_estados_modulo_radicacion.Solicita_numero_radicados_pendientes
```

SQL funcional:

```sql
SELECT id_estado_radicado
FROM ra_rad_estados_modulo_radicacion
WHERE estado = 1
  AND id_usuario_radicado = @idUsuarioRadicador
  AND system_plantilla_radicado_id_Plantilla = @idPlantilla
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## SEMANTICA DE ESTADOS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

```txt
estado = 1
  Radicado pendiente.
  Se muestra en lista de pendientes.
  No activa Documentos.

estado = 0
  Radicado activo/asignado para gestion documental.
  Bloquea tomar otro pendiente.
  Es el unico estado que habilita Documentos.
```

Esta semantica es obligatoria. No invertirla.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## APIS A ENTREGAR EN ESTA FASE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

### 1. Listado de pendientes existente, ampliado

Endpoint:

```txt
GET /api/tramite/tramites/apListaRadicadosPendientes
```

Accion requerida:

- reutilizar endpoint actual;
- ampliar DTO/mapper/repository para incluir `id_tarea_workflow`;
- incluir descripcion de tramite;
- mantener retorno `AppResponses<DynamicUiTableDto>`;
- mantener accion de fila `asignacion-tarea`;
- asegurar que la accion transporte:

```txt
id_estado_radicado
id_tarea_workflow
consecutivo_radicado
```

Contrato de fila esperado:

```ts
type ListaRadicadosPendientesDto = {
  id_estado_radicado: number;
  consecutivo_radicado: string;
  remitente: string;
  tramite: string;
  fecha_registro: string;
  id_tarea_workflow: number;
};
```

### 2. Estado activo del usuario

Endpoint nuevo:

```txt
GET /api/radicacion/pendientes/estado-activo
```

Uso:

Validar si el usuario ya tiene un radicado activo en `estado = 0`.

Response:

```ts
type RadicacionPendienteEstadoActivoDto = {
  tieneActivoEstado0: boolean;
  idEstadoRadicadoActivo?: number;
  consecutivoRadicado?: string;
  idTareaWorkflow?: number;
  mensaje?: string;
};
```

Regla:

- si `tieneActivoEstado0 = true`, frontend no debe permitir tomar otro pendiente;
- si no hay activo, retornar `success=true`, `data.tieneActivoEstado0=false`;
- no retornar error cuando no existan filas.

### 3. Contador de pendientes

Endpoint opcional:

```txt
GET /api/radicacion/pendientes/contador
```

Response:

```ts
type RadicacionPendientesContadorDto = {
  totalPendientes: number;
};
```

Decision:

- implementarlo solo si frontend necesita badge sin cargar la tabla completa;
- si se omite, documentar que el contador se deriva de `apListaRadicadosPendientes`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## UBICACION ESPERADA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Controller:

```txt
DocuArchi.Api\Controllers\Radicacion\Tramite\RadicacionPendientesController.cs
```

Tambien es valido extender `TramiteController` solo para `apListaRadicadosPendientes`, pero los nuevos endpoints deben quedar en un controller de dominio claro.

Services:

```txt
MiApp.Services\Service\Radicacion\Tramite\RadicacionPendientesConsultaService.cs
```

Repositories:

```txt
MiApp.Repository\Repositorio\Radicador\Tramite\RadicacionPendientesConsultaRepository.cs
```

DTOs:

```txt
MiApp.DTOs\DTOs\Radicacion\Tramite\RadicacionPendientesConsultaDtos.cs
MiApp.DTOs\DTOs\Radicacion\Tramite\ListaRadicadosPendientesDto.cs
```

Si existe una convencion mas especifica en el repo, respetarla.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLAS TECNICAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

PROHIBIDO:

- crear endpoint duplicado para listar pendientes si el actual puede ampliarse;
- usar SQL concatenado;
- usar Session legacy;
- usar `.asmx`;
- depender de parametros de usuario enviados por frontend si ya existen claims;
- retornar error cuando no hay pendientes;
- invertir semantica `estado 0/1`;
- romper contrato `DynamicUiTableDto`;
- ocultar errores de claim o alias.

OBLIGATORIO:

- usar claims `defaulalias` y `usuarioid`;
- resolver usuario radicador relacionado igual que el listado actual;
- resolver plantilla de radicacion default igual que el listado actual;
- usar `QueryOptions`;
- retornar `AppResponses<T>`;
- agregar XML comments en metodos publicos;
- mantener `try/catch` controlado;
- seleccionar solo columnas necesarias;
- agregar tests unitarios de service y repository;
- si hay pruebas de integracion existentes para radicacion, extenderlas.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CRITERIOS DE ACEPTACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- `GET /api/tramite/tramites/apListaRadicadosPendientes` conserva compatibilidad y agrega `id_tarea_workflow` y `tramite`.
- La accion `asignacion-tarea` transporta `id_estado_radicado` e `id_tarea_workflow`.
- `GET /api/radicacion/pendientes/estado-activo` retorna `tieneActivoEstado0`.
- Si el usuario no tiene activo, retorna `success=true`, no error.
- Si el usuario tiene activo, retorna metadata minima del activo.
- `GET /api/radicacion/pendientes/contador` queda implementado o explicitamente descartado por decision tecnica documentada.
- No se consume legacy.
- No se concatena SQL.
- Tests cubren:
  - listado con pendientes;
  - listado sin pendientes;
  - listado incluye `id_tarea_workflow`;
  - estado activo true;
  - estado activo false;
  - error claim `usuarioid`;
  - alias faltante.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FUERA DE ALCANCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No implementar en esta fase:

- `POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente`;
- `POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar`;
- creacion de workflow;
- cambio de estado `0 -> 1`;
- cambio de estado `1 -> 0`;
- integracion frontend;
- carga documental.
