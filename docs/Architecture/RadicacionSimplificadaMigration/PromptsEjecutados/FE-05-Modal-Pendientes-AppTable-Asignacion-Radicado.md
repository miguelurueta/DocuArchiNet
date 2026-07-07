# FE-05 - Modal De Pendientes Con AppTable Y Asignacion

## Que Hace

Migra el modal de pendientes a `AppTable` e implementa el flujo transaccional para tomar un radicado pendiente.

## Casos De Uso Cubiertos

- Abrir modal y cargar listado de pendientes bajo demanda.
- Buscar/paginar pendientes usando infraestructura de `AppTable`.
- Ejecutar accion `asignacion-tarea` desde fila.
- Bloquear toma si ya existe tramite activo.
- Llamar backend para tomar pendiente.
- Validar que backend confirme `estadoActual === 0`.
- Actualizar `RadicacionDocumentalContext`.
- Refrescar contador/listado.
- Cerrar modal y navegar a `Documentos`.

## Arquitectura Implementada

```text
Modalpendiente
  -> RadicacionPendientesModal
  -> useRadicacionPendientesTable()
  -> useRadicacionPendientesContador()
  -> AppTableQueryWrapper
  -> AppTable
  -> onActionTriggered(asignacion-tarea)
  -> useTomarRadicadoPendiente()
  -> tomarRadicacionPendiente()
  -> RadicacionDocumentalContext
  -> RADICACION_ROUTES.documentos(idEstadoRadicado)
```

## Endpoints Consumidos

```text
POST /api/tramite/tramites/apListaRadicadosPendientes
GET  /api/radicacion/pendientes/contador
POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar
```

## Archivos Principales

- `src/modules/radicacion/components/Modalpendiente.tsx`
- `src/modules/radicacion/components/RadicacionPendientesModal.tsx`
- `src/modules/radicacion/hooks/useRadicacionPendientesTable.ts`
- `src/modules/radicacion/hooks/useRadicacionPendientesContador.ts`
- `src/modules/radicacion/hooks/useTomarRadicadoPendiente.ts`
- `src/modules/radicacion/adapters/radicacionPendientesTableRequestMapper.ts`
- `src/modules/radicacion/types/radicacionPendientes.types.ts`
- `src/modules/radicacion/services/radicacionPendientes.service.ts`
- `src/app/routes/routes.tsx`

## Si Falla, Revisar

- Si Vite muestra `ECONNREFUSED`, el API no esta escuchando en `127.0.0.1:5055`.
- Si no aparecen acciones, revisar que backend entregue `actionId = asignacion-tarea`.
- Si no navega, revisar que la respuesta de `tomar` traiga `estadoActual: 0` e `idEstadoRadicado`.
- Si se bloquea la asignacion, revisar si ya existe `tieneTramiteDocumentalActivoEstado0`.
- Si no refresca, revisar invalidacion de query keys `radicacionPendientes` y contador.

## Restricciones

- No usar `antd/Table`.
- No usar datos mock.
- No actualizar contexto antes de respuesta exitosa.
- No navegar si backend no confirma estado `0`.
- No hardcodear rutas en el modal.

