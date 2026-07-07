# TD-FE-04 - Rutas, Tabs Y Limpieza De UI Prototipo

## Que Hace

Centraliza rutas del modulo, reemplaza keys numericas de tabs por keys de dominio y elimina datos mock visibles en runtime.

## Casos De Uso Cubiertos

- Navegacion consistente dentro de radicacion.
- Tabs semanticas: `ia`, `radicacion`, `documentos`, `gestion-radicados`.
- `CapDocument` no muestra gabinete ni documentos falsos.
- `Modalpendiente` queda listo para reemplazo funcional posterior.

## Arquitectura Implementada

```text
radicacionRoutes
  -> RADICACION_ROUTES
  -> RADICACION_TAB_KEYS
  -> resolveRadicacionTabFromDestino()
  -> RadicacionTabs
```

## Archivos Principales

- `src/modules/radicacion/routes/radicacionRoutes.ts`
- `src/modules/radicacion/hooks/RadicacionTabs.tsx`
- `src/modules/radicacion/components/CapDocument.tsx`
- `src/modules/radicacion/components/Modalpendiente.tsx`

## Si Falla, Revisar

- Que no existan keys numericas en tabs.
- Que rutas no esten hardcodeadas en componentes.
- Que `CapDocument` dependa del contexto documental y no de mocks.
- Que el tab `Documentos` use la regla del contexto.

## Restricciones

- No volver a introducir datos mock en runtime.
- No activar documentos por UI local.

