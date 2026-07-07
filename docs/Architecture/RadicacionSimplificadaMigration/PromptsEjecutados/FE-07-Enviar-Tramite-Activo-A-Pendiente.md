# FE-07 - Enviar Tramite Activo A Pendiente

## Que Hace

Implementa el caso de uso que permite devolver el tramite documental activo de estado `0` a estado `1` pendiente, con confirmacion explicita del usuario y validacion de respuesta del backend.

## Casos De Uso Cubiertos

- Mostrar la accion solo cuando existe tramite documental activo en `RadicacionDocumentalContext`.
- Solicitar confirmacion antes de ejecutar la mutacion.
- Ejecutar `POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente`.
- Validar que backend confirme `estadoActual === 1`.
- Mantener intacto el contexto si backend responde error o no confirma estado `1`.
- Limpiar `RadicacionDocumentalContext` solo despues de exito confirmado.
- Deshabilitar `Documentos` al quedar sin tramite activo.
- Refrescar estado activo, contador y listado de pendientes.
- Navegar a resumen usando rutas centralizadas.

## Arquitectura Implementada

```text
RadicacionTabs
  -> EnviarPendienteConfirmModal
  -> useEnviarRadicadoPendiente()
  -> enviarRadicacionPendiente()
  -> Backend
  -> RadicacionDocumentalContext.clearContextoDocumental()
  -> invalidate estado-activo / contador / AppTable pendientes
  -> RADICACION_ROUTES.root
```

## Regla De Visibilidad

La accion solo queda disponible cuando se cumple:

```text
requiereGestionDocumental === true
AND tieneTramiteDocumentalActivoEstado0 === true
AND estadoActual === 0
AND idEstadoRadicado > 0
```

Si la regla no se cumple, el componente no renderiza el boton y el hook bloquea la mutacion.

## Endpoints Consumidos

```text
POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente
```

## Archivos Principales

- `src/modules/radicacion/components/EnviarPendienteConfirmModal.tsx`
- `src/modules/radicacion/hooks/useEnviarRadicadoPendiente.ts`
- `src/modules/radicacion/hooks/RadicacionTabs.tsx`
- `src/modules/radicacion/components/RadicacionForm.tsx`
- `src/modules/radicacion/services/radicacionPendientes.service.ts`
- `src/modules/radicacion/types/radicacionPendientes.types.ts`

## Pruebas Asociadas

- `src/modules/radicacion/hooks/useEnviarRadicadoPendiente.spec.test.tsx`
- `src/modules/radicacion/services/radicacionPendientes.service.test.ts`
- `src/modules/radicacion/hooks/RadicacionTabs.spec.test.tsx`

## Si Falla, Revisar

- Si el boton no aparece, revisar que el contexto tenga `estadoActual: 0`, `requiereGestionDocumental: true`, `tieneTramiteDocumentalActivoEstado0: true` e `idEstadoRadicado`.
- Si no limpia Documentos, revisar que backend retorne `estadoActual: 1` y `tieneTramiteDocumentalActivoEstado0: false`.
- Si aparece error funcional, revisar la regla backend del tramite activo antes de reintentar.
- Si el contador/listado no cambia, revisar invalidacion de `RADICACION_PENDIENTES_CONTADOR_QUERY_KEY` y `dynamic-ui-table/radicacionPendientes`.
- Si no navega, revisar que se use `RADICACION_ROUTES.root` y que la mutacion haya terminado en exito.

## Restricciones

- No ejecutar la mutacion desde `RadicacionForm`.
- No limpiar contexto antes de respuesta exitosa.
- No crear stores paralelos.
- No hardcodear rutas en componentes.
- No modificar contratos backend desde esta fase.
