# SCRUM-242 Pruebas

## Validaciones Ejecutadas

```txt
npx eslint src/modules/digitalizacion --ext .ts,.tsx
npx vitest run src/modules/digitalizacion
```

Resultado Vitest:

- 8 test files passed.
- 43 tests passed.

## Cobertura

La suite nueva cubre:

- configuracion OK;
- AppResponses `success=false`;
- payload parcial invalido;
- upload init invalido;
- upload chunk falla;
- upload complete sin confirmacion;
- upload exitoso por chunks;
- create documento OK;
- adjuntar documento OK;
- anti doble submit en hook base;
- stale response ignorada tras cancelacion.

## Pendientes

- E2E contra backend real.
- Contrato final de idempotencia `RequestId`.
- Integracion visual final del boton primario con create/attach.
