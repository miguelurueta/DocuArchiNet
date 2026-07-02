# PROMPT ARQUITECTONICO - Backend Radicacion
# API 03 - Contador de radicados pendientes

## Objetivo

Crear una API liviana para contador:

```txt
GET /api/radicacion/pendientes/contador
```

Se implementa como endpoint propio para no cargar la tabla completa cuando frontend solo necesita badge/contador.

## Contexto Legacy

```txt
Service_solicita_numero_radicados_pendientes
Class_ra_rad_estados_modulo_radicacion.Solicita_numero_radicados_pendientes
```

SQL legacy funcional:

```sql
SELECT id_estado_radicado
FROM ra_rad_estados_modulo_radicacion
WHERE estado = 1
  AND id_usuario_radicado = @idUsuarioRadicador
  AND system_plantilla_radicado_id_Plantilla = @idPlantilla
```

SQL moderno objetivo:

```sql
SELECT COUNT(*) AS totalPendientes
FROM ra_rad_estados_modulo_radicacion
WHERE estado = 1
  AND id_usuario_radicado = @idUsuarioRadicador
  AND system_plantilla_radicado_id_Plantilla = @idPlantilla
```

No cargar filas para contar.

## Contrato

```ts
type RadicacionPendientesContadorDto = {
  totalPendientes: number;
};
```

## Ubicacion Esperada

```txt
DocuArchi.Api\Controllers\Radicacion\Tramite\RadicacionPendientesController.cs
MiApp.Services\Service\Radicacion\Tramite\RadicacionPendientesContadorService.cs
MiApp.Repository\Repositorio\Radicador\Tramite\RadicacionPendientesContadorRepository.cs
MiApp.DTOs\DTOs\Radicacion\Tramite\RadicacionPendientesContadorDto.cs
```

## Reglas

- Contar solo `estado = 1`.
- `estado = 0` no cuenta como pendiente.
- Retornar `totalPendientes = 0` si no hay registros.
- Usar `COUNT(*)`, no `SELECT` de filas completas.
- Leer `defaulalias` y `usuarioid` desde claims.
- Resolver usuario radicador relacionado y plantilla default.
- Usar `AppResponses<RadicacionPendientesContadorDto>`.
- No consumir ASMX.
- No usar Session legacy.

## Criterios

- Existe `GET /api/radicacion/pendientes/contador`.
- Retorna `totalPendientes`.
- No cuenta registros `estado = 0`.
- Tests cubren contador con pendientes, contador cero, claim invalido y error repository.
