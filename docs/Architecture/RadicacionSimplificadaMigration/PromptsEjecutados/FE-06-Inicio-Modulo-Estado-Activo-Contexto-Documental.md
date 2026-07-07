# FE-06 - Inicio Del Modulo Con Estado Activo

## Que Hace

Al montar el modulo de radicacion, consulta si el usuario tiene un tramite activo en estado `0`. Si existe, restaura el contexto documental; si no existe, limpia el contexto.

## Casos De Uso Cubiertos

- Usuario entra al modulo y tiene radicado activo: se restaura contexto.
- Usuario entra sin radicado activo: inicia formulario limpio.
- Error consultando estado activo: se muestra error con opcion de reintento.
- Mientras el bootstrap no termina: se bloquea el render funcional.

## Arquitectura Implementada

```text
RadicacionRoutePage
  -> RadicacionDocumentalProvider
  -> RadicacionStartupGuard
  -> useRadicacionEstadoActivo()
  -> fetchRadicacionEstadoActivo()
  -> RadicacionPage
```

## Endpoint Consumido

```text
GET /api/radicacion/pendientes/estado-activo
```

## Archivos Principales

- `src/modules/radicacion/components/RadicacionStartupGuard.tsx`
- `src/modules/radicacion/hooks/useRadicacionEstadoActivo.ts`
- `src/modules/radicacion/services/radicacionPendientes.service.ts`
- `src/modules/radicacion/pages/RadicacionRoutePage.tsx`

## Si Falla, Revisar

- Que el backend responda `tieneActivoEstado0`.
- Que `mapEstadoActivoToDocumentalState()` retorne contexto solo cuando hay activo.
- Que `RadicacionStartupGuard` este dentro de `RadicacionDocumentalProvider`.
- Que el API este levantado y el proxy apunte al puerto correcto.

## Restricciones

- Las paginas no consultan `estado-activo`.
- El bootstrap pertenece solo a `RadicacionStartupGuard`.

