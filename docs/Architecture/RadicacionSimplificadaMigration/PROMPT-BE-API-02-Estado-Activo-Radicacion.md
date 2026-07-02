# PROMPT ARQUITECTONICO - Backend Radicacion
# API 02 - Estado activo y contexto documental de radicacion

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## ROL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Actua como Arquitecto Backend senior especialista en:

- .NET API por capas Controller -> Service -> Repository;
- migracion legacy ASMX/WebForms hacia REST;
- restauracion de contexto transaccional;
- flujos de radicacion documental;
- consultas parametrizadas con DapperCrudEngine/QueryOptions;
- AppResponses<T>;
- seguridad por claims;
- pruebas unitarias y de contrato.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Crear la API:

```txt
GET /api/radicacion/pendientes/estado-activo
```

Esta API debe resolver si el usuario actual tiene un tramite documental activo en:

```txt
ra_rad_estados_modulo_radicacion.estado = 0
```

Si existe, no debe devolver solo `true`. Debe devolver el contexto minimo para que el frontend restaure el tramite abandonado y entre directamente al panel `Documentos`.

Regla funcional:

```txt
Al iniciar src/modules/radicacion, si existe estado = 0, el modulo no inicia como formulario limpio.
Debe restaurar contexto y navegar a Documentos.
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTEXTO LEGACY CONFIRMADO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

El repo viejo tiene dos piezas separadas.

### 1. Detectar si hay activo

WebMethod:

```txt
webservice/WebService_radicacion_Simplificada.asmx.vb
Service_solicita_estado_radicado_asignado_usuario_gestion_documentos
```

Clase:

```txt
radicador/Class_ra_rad_estados_modulo_radicacion.vb
Solicita_estado_radicado_asignado_usuario_gestion_documentos
```

SQL legacy funcional:

```sql
SELECT id_estado_radicado
FROM ra_rad_estados_modulo_radicacion
WHERE estado = 0
  AND id_usuario_radicado = @idUsuarioRadicador
  AND system_plantilla_radicado_id_Plantilla = @idPlantilla
```

El legacy retorna:

```txt
estado_asignado = "YES" | "NO"
```

### 2. Armar estructura/contexto del radicado

WebMethod:

```txt
webservice/WebService_radicacion_Simplificada.asmx.vb
Service_solicita_estructura_estado_radicado_radicacion_simple
```

Clase:

```txt
radicador/Class_ra_rad_estados_modulo_radicacion.vb
Solicita_estructura_estado_radicado_radicacion_simple
```

Esta funcion recibe:

```txt
id_registro_estado
id_usuario_radicacion
id_plantilla_radicacion
id_tipo_plantilla_radicacion
```

Y arma contexto operativo:

- numero de pendientes;
- datos del registro de estado;
- nombre de plantilla;
- opcion `util_estado_pendiente_rad`;
- tipo de tramite;
- nombre/id de tramite;
- gabinete asociado;
- datos necesarios para gestion documental.

Conclusion:

```txt
El legacy si tiene la logica base, pero no tiene una API unica moderna.
La API moderna debe unir deteccion estado=0 + contexto minimo de reentrada.
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## UBICACION ESPERADA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Crear o extender:

```txt
DocuArchi.Api\Controllers\Radicacion\Tramite\RadicacionPendientesController.cs
MiApp.Services\Service\Radicacion\Tramite\RadicacionPendienteEstadoActivoService.cs
MiApp.Repository\Repositorio\Radicador\Tramite\RadicacionPendienteEstadoActivoRepository.cs
MiApp.DTOs\DTOs\Radicacion\Tramite\RadicacionPendienteEstadoActivoDto.cs
```

Si ya existe un controller moderno para pendientes, usarlo. No crear controllers duplicados por cada endpoint si el repo ya agrupa esta responsabilidad.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTRATO OBJETIVO
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Endpoint:

```txt
GET /api/radicacion/pendientes/estado-activo
```

Response:

```cs
public sealed class RadicacionPendienteEstadoActivoDto
{
    public bool TieneActivoEstado0 { get; set; }
    public long? IdEstadoRadicado { get; set; }
    public long? IdRadicado { get; set; }
    public string? ConsecutivoRadicado { get; set; }
    public long? IdTareaWorkflow { get; set; }
    public int? EstadoActual { get; set; }
    public string? Tramite { get; set; }
    public string? Remitente { get; set; }
    public int? PlantillaId { get; set; }
    public int? TipoPlantillaId { get; set; }
    public bool RequiereGestionDocumental { get; set; }
    public bool TieneTramiteDocumentalActivoEstado0 { get; set; }
    public string DestinoPostRegistro { get; set; } = "resumen";
    public RadicacionPendienteContextoDocumentalDto? ContextoDocumental { get; set; }
}

public sealed class RadicacionPendienteContextoDocumentalDto
{
    public long? IdGabinete { get; set; }
    public string? NombreGabinete { get; set; }
    public long? IdTipoTramite { get; set; }
    public string? NombreTramite { get; set; }
    public bool UtilEstadoPendienteRad { get; set; }
}
```

Cuando existe activo:

```json
{
  "tieneActivoEstado0": true,
  "idEstadoRadicado": 123,
  "idRadicado": 456,
  "consecutivoRadicado": "RAD-2026-0001",
  "idTareaWorkflow": 789,
  "estadoActual": 0,
  "tramite": "Licencia de construccion",
  "remitente": "Juan Perez",
  "plantillaId": 10,
  "tipoPlantillaId": 1,
  "requiereGestionDocumental": true,
  "tieneTramiteDocumentalActivoEstado0": true,
  "destinoPostRegistro": "documentos"
}
```

Cuando no existe activo:

```json
{
  "tieneActivoEstado0": false,
  "estadoActual": null,
  "requiereGestionDocumental": false,
  "tieneTramiteDocumentalActivoEstado0": false,
  "destinoPostRegistro": "resumen"
}
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## REGLAS FUNCIONALES
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- `estado = 0` significa tramite activo/asignado para gestion documental.
- Si existe `estado = 0`, frontend debe restaurar contexto y entrar directo a `Documentos`.
- Si existe `estado = 0`, frontend no debe permitir tomar otro pendiente.
- Si no existe `estado = 0`, frontend puede iniciar normal y permitir tomar pendientes `estado = 1`.
- Esta API no debe cambiar estados.
- Esta API no debe crear workflow.
- Esta API no debe activar documentos por inferencia incompleta.
- `Documentos` solo queda habilitado si `TieneTramiteDocumentalActivoEstado0 = true`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONSULTA REPOSITORY
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

La consulta minima debe resolver el activo del usuario:

```sql
SELECT
  rre.id_estado_radicado,
  rre.id_radicado,
  rre.consecutivo_radicado,
  rre.id_tarea_workflow,
  rre.estado,
  rre.remitente,
  rre.system_plantilla_radicado_id_Plantilla,
  rre.tipo_plantilla_radicado,
  rre.tipo_doc_entrante_id_Tipo_Doc_Entrante,
  COALESCE(tde.Descripcion_Doc, '') AS tramite
FROM ra_rad_estados_modulo_radicacion rre
LEFT JOIN tipo_doc_entrante tde
  ON tde.id_Tipo_Doc_Entrante = rre.tipo_doc_entrante_id_Tipo_Doc_Entrante
WHERE rre.estado = 0
  AND rre.id_usuario_radicado = @idUsuarioRadicacion
  AND rre.system_plantilla_radicado_id_Plantilla = @idPlantilla
ORDER BY rre.id_estado_radicado DESC
LIMIT 1
```

Si hay mas de un activo `estado = 0`, no ocultar el problema. Retornar el mas reciente para no bloquear al usuario, pero incluir error/warning funcional o registrar inconsistencia segun patron del repo.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## SERVICE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Responsabilidades:

- validar `idUsuarioGestion > 0`;
- validar `defaultDbAlias`;
- resolver usuario radicador relacionado;
- resolver plantilla default;
- consultar activo `estado = 0`;
- si no hay activo, retornar `success=true` con `TieneActivoEstado0=false`;
- si hay activo, mapear contexto minimo;
- resolver datos documentales si estan disponibles:
  - tramite;
  - gabinete;
  - plantilla;
  - tipo plantilla;
  - `util_estado_pendiente_rad`;
- construir `DestinoPostRegistro = "documentos"` solo cuando `estado = 0`;
- no consumir ASMX;
- no usar `HttpContext.Session`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CONTROLLER
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Endpoint:

```txt
[HttpGet("pendientes/estado-activo")]
```

Ruta base recomendada:

```txt
api/radicacion
```

Debe:

- validar claim `defaulalias`;
- validar claim `usuarioid`;
- retornar `BadRequest` si falta claim;
- retornar error controlado si `usuarioid` no es entero;
- delegar toda la logica al service;
- retornar `Ok(result)` si `success=true`;
- retornar `BadRequest(result)` si `success=false`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## RELACION CON FRONTEND
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Al montar/iniciar `src/modules/radicacion`, frontend debe consumir esta API.

Si `TieneActivoEstado0 = true`:

```txt
1. Guardar contexto post-radicacion.
2. Activar Documentos.
3. Navegar a /dashboard/radicacion/registro/{idEstadoRadicado}/documentos.
4. Bloquear tomar otro pendiente.
```

Si `TieneActivoEstado0 = false`:

```txt
1. Iniciar modulo normal.
2. Mantener Documentos inactivo.
3. Permitir abrir modal de pendientes.
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## PRUEBAS REQUERIDAS
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Crear/actualizar pruebas:

```txt
RadicacionPendienteEstadoActivoServiceTests.cs
RadicacionPendienteEstadoActivoRepositoryTests.cs
RadicacionPendientesControllerTests.cs
```

Casos minimos:

- activo encontrado retorna `TieneActivoEstado0=true`;
- activo encontrado retorna `DestinoPostRegistro=documentos`;
- activo encontrado retorna `IdEstadoRadicado`;
- activo encontrado retorna `ConsecutivoRadicado`;
- activo encontrado retorna `IdTareaWorkflow`;
- activo encontrado retorna `TieneTramiteDocumentalActivoEstado0=true`;
- sin activo retorna `success=true` y `TieneActivoEstado0=false`;
- sin activo retorna `DestinoPostRegistro=resumen`;
- claim `defaulalias` faltante retorna `BadRequest`;
- claim `usuarioid` faltante retorna `BadRequest`;
- `usuarioid` no numerico retorna error controlado;
- usuario gestion sin usuario radicador relacionado retorna validacion controlada;
- repository error retorna `success=false`;
- si existen multiples activos, se controla la inconsistencia.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## CRITERIOS DE ACEPTACION
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

- Existe `GET /api/radicacion/pendientes/estado-activo`.
- No usa ASMX.
- No usa Session legacy.
- Consulta `ra_rad_estados_modulo_radicacion.estado = 0`.
- Resuelve usuario radicador desde usuario gestion.
- Resuelve plantilla default.
- Si hay activo, devuelve contexto suficiente para reentrada al modulo.
- Si no hay activo, devuelve respuesta exitosa con `TieneActivoEstado0=false`.
- La respuesta permite activar `Documentos` solo cuando `estado = 0`.
- La respuesta permite bloquear toma de otro pendiente cuando ya hay activo.
- Tests cubren activo, sin activo, claims invalidos y error repository.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
## FUERA DE ALCANCE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

No implementar aqui:

- listado de pendientes;
- contador de pendientes;
- tomar pendiente;
- enviar a pendiente;
- carga documental;
- digitalizacion;
- visor;
- cambios frontend.

APIs relacionadas:

```txt
GET  /api/tramite/tramites/apListaRadicadosPendientes
GET  /api/radicacion/pendientes/contador
POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar
POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente
```
