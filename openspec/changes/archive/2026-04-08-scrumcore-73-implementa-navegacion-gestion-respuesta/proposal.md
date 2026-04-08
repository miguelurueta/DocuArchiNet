## Why

IMPLEMENTA-NAVEGACION-GESTION-RESPUESTA. PROMPT ARQUITECTÓNICO — Navegación a GestionRespuesta desde acción de fila en GestionCorrespondencia

## What Changes

- Se mueve la apertura de `GestionRespuesta` desde un boton de toolbar hacia la accion contextual de cada fila en `GestionCorrespondencia`.
- Se introduce una forma reutilizable de notificar acciones de fila desde `AppTable` sin acoplarlo al dominio del modulo.
- Se actualiza el routing de `gestion-correspondencia` para soportar `respuesta/:id`.
- Se elimina el entry point redundante de toolbar para el mismo flujo de detalle contextual.

## Capabilities

### Modified Capabilities
- `gestion-correspondencia`: navegar a `GestionRespuesta` desde accion de fila con parametro de ruta.
- `app-table`: propagar eventos reutilizables de accion contextual sin hardcodear navegacion de modulos.

## Impact

- Cambios en `GestionCorrespondencia`, `routes.tsx` y pruebas del modulo.
- Extension controlada del contrato reusable de `AppTable` o su renderer de acciones.
- Eliminacion del boton de toolbar `Abrir respuesta contextual`.
