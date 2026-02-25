## Why

Hoy el campo `ra_tipo_tramite` en `RadicacionForm.tsx` no carga su lista desde la fuente oficial de campos de plantilla. Esto causa inconsistencias entre la UI de radicación y los datos configurados en `useCamposPlantilla`.

## What Changes

- Poblar la lista de tipos de trámite del campo `ra_tipo_tramite` usando los datos provistos por `useCamposPlantilla`.
- Asegurar que la UI refleje dinámicamente los tipos de trámite configurados en plantillas sin hardcode.

## Capabilities

### New Capabilities
- `radicacion-tipos-tramite`: Exponer y consumir tipos de trámite de plantillas en el formulario de radicación.

### Modified Capabilities
- (none)

## Impact

- UI de radicación: `RadicacionForm.tsx`.
- Hook de datos: `useCamposPlantilla`.
- Posible impacto en tests del formulario de radicación (agregar/ajustar cobertura de SPEC).

## Restricciones

- Si los tests obligatorios no pasan, los cambios no se aplican.
- Es obligatorio dejar evidencia de ejecución de tests en la documentación OpenSpec.
