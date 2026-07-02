# PROMPT ARQUITECTONICO - Backend Radicacion
# API 05 - Tomar radicado pendiente para gestion documental

## Objetivo

Crear la API:

```txt
POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar
```

Debe tomar un radicado pendiente y dejarlo activo para gestion documental.

```txt
estado 1 -> estado 0
```

Si `id_tarea_workflow = 0`, backend debe crear/relacionar workflow antes de activar documentos.

## Contexto Legacy

```txt
asigna_tarea_pendiente_radicado(id_tarea_workflow, id_registro_estado)
Service_solicita_estado_radicado_asignado_usuario_gestion_documentos(...)
Service_registra_flujo_tarea_workflow_radicado_simple(id_registro_estado)
Service_actualiza_estado_registro_radicado_pendiente(id_registro_estado, 0)
```

## Contrato

```ts
type TomarRadicadoPendienteRequestDto = {
  idTareaWorkflow?: number | null;
};

type TomarRadicadoPendienteResponseDto = {
  idEstadoRadicado: number;
  idRadicado?: number;
  consecutivoRadicado: string;
  idTareaWorkflow: number;
  estadoAnterior: 1;
  estadoActual: 0;
  requiereGestionDocumental: true;
  tieneTramiteDocumentalActivoEstado0: true;
  destinoPostRegistro: "documentos";
  contextoDocumental?: {
    idGabinete?: number | null;
    nombreGabinete?: string | null;
    idTipoTramite?: number | null;
    nombreTramite?: string | null;
    utilEstadoPendienteRad?: boolean;
  } | null;
  metadataOperativa: {
    tramite?: string;
    remitente?: string;
    plantillaId?: number;
    workflowFueCreado: boolean;
  };
};
```

## Ubicacion Esperada

```txt
DocuArchi.Api\Controllers\Radicacion\Tramite\RadicacionPendientesController.cs
MiApp.Services\Service\Radicacion\Tramite\TomarRadicadoPendienteService.cs
MiApp.Repository\Repositorio\Radicador\Tramite\TomarRadicadoPendienteRepository.cs
MiApp.DTOs\DTOs\Radicacion\Tramite\TomarRadicadoPendienteDtos.cs
```

## Reglas

- Validar `idEstadoRadicado > 0`.
- Validar que el registro exista, pertenezca al usuario y este en `estado = 1`.
- Validar que el usuario no tenga otro activo `estado = 0`.
- Si `id_tarea_workflow > 0`, actualizar `estado = 0`.
- Si `id_tarea_workflow = 0`, crear/relacionar workflow y luego actualizar `estado = 0`.
- Si falla workflow, no dejar `estado = 0`.
- Retornar `destinoPostRegistro = "documentos"`.
- Retornar un contrato compatible con `GET /api/radicacion/pendientes/estado-activo`, para que FE-05 y FE-06 alimenten el mismo contexto documental.
- No consumir ASMX.

Mensaje funcional de bloqueo:

```txt
Tarea asignada para gestion y asignacion, debe terminar la tarea actual o subirla a estado pendiente para continuar con la asignacion.
```

## Criterios

- Existe endpoint `tomar`.
- Cambia `estado 1 -> 0`.
- Bloquea si ya hay activo `estado = 0`.
- Crea/relaciona workflow cuando `id_tarea_workflow = 0`.
- Tests cubren success con tarea, success creando workflow, activo existente, no encontrado, usuario no autorizado y rollback por error workflow.
