# PROMPT ARQUITECTONICO - Backend Radicacion
# API 04 - Enviar radicado activo a pendiente

## Objetivo

Crear la API:

```txt
POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente
```

Debe subir un radicado activo a pendiente.

```txt
estado 0 -> estado 1
```

La transicion valida es estricta. No actualizar registros que no esten actualmente en `estado = 0`.

## Contexto Legacy

```txt
envia_tarea_pendiente_radicado(id_registro_estado)
Service_actualiza_estado_registro_radicado_pendiente(id_registro_estado, 1)
Actualiza_estado_registro_modulo_radicacion(id_registro_estado, 1)
```

## Contrato

```ts
type EnviarRadicadoPendienteRequestDto = {
  motivo?: string;
};

type EnviarRadicadoPendienteResponseDto = {
  idEstadoRadicado: number;
  consecutivoRadicado?: string;
  estadoAnterior: 0;
  estadoActual: 1;
  tieneTramiteDocumentalActivoEstado0: false;
  destinoPostRegistro: "resumen";
  mensaje: string;
};
```

## Ubicacion Esperada

```txt
DocuArchi.Api\Controllers\Radicacion\Tramite\RadicacionPendientesController.cs
MiApp.Services\Service\Radicacion\Tramite\EnviarRadicadoPendienteService.cs
MiApp.Repository\Repositorio\Radicador\Tramite\EnviarRadicadoPendienteRepository.cs
MiApp.DTOs\DTOs\Radicacion\Tramite\EnviarRadicadoPendienteDtos.cs
```

## Reglas

- Validar `idEstadoRadicado > 0`.
- Validar que el registro exista y pertenezca al usuario radicador.
- Validar que el registro este actualmente en `estado = 0`.
- Actualizar exclusivamente `estado 0 -> 1`.
- Si ya esta en `estado = 1`, responder con politica explicita:
  - idempotente controlado, o
  - error funcional controlado.
- Si esta en un estado distinto de `0` o `1`, rechazar la transicion.
- No exigir `id_tarea_workflow`.
- No borrar documentos ni gabinete.
- Retornar `destinoPostRegistro = "resumen"`.
- Retornar `tieneTramiteDocumentalActivoEstado0 = false`.
- No consumir ASMX.

## Criterios

- Existe endpoint `enviar-pendiente`.
- Cambia `estado 0 -> 1`.
- Rechaza o controla registros que no esten en `estado = 0`.
- No exige `id_tarea_workflow`.
- Tests cubren success, id invalido, no encontrado, usuario no autorizado, estado distinto de 0, ya pendiente y error repository.
