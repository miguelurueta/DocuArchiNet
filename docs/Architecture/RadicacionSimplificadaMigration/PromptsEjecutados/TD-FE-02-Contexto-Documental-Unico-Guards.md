# TD-FE-02 - Contexto Documental Unico Y Guards

## Que Hace

Instala el contexto unico de radicacion documental y centraliza la regla para habilitar `Documentos`.

## Casos De Uso Cubiertos

- Guardar contexto documental despues de una radicacion o una toma de pendiente.
- Restaurar contexto documental al iniciar el modulo.
- Bloquear `Documentos` cuando no existe tramite activo en estado `0`.
- Limpiar contexto sin afectar otros estados del formulario.

## Arquitectura Implementada

```text
RadicacionDocumentalProvider
  -> RadicacionDocumentalContext
  -> useRadicacionDocumentalContext()
  -> RadicacionDocumentosGuard
  -> CapDocument
```

## Regla Funcional

`Documentos` solo puede renderizar si:

```text
estadoActual === 0
AND requiereGestionDocumental === true
AND tieneTramiteDocumentalActivoEstado0 === true
AND idEstadoRadicado > 0
```

## Archivos Principales

- `src/modules/radicacion/context/RadicacionDocumentalContext.tsx`
- `src/modules/radicacion/context/radicacionDocumentalContextValue.ts`
- `src/modules/radicacion/hooks/useRadicacionDocumentalContext.ts`
- `src/modules/radicacion/components/RadicacionDocumentosGuard.tsx`
- `src/modules/radicacion/types/radicacionDocumental.types.ts`

## Si Falla, Revisar

- Que ningun componente active documentos por seleccion de fila, consecutivo o gabinete.
- Que `setContextoDocumental()` reciba `estadoActual: 0`.
- Que `normalizeRadicacionDocumentalState()` no permita activos incompletos.

## Restricciones

- No crear stores paralelos.
- No duplicar la regla documental en tabs, modal o formularios.

