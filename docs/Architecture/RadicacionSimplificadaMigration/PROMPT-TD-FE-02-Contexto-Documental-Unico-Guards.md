# PROMPT DE DEUDA TECNICA - Frontend Radicacion
# TD-FE-02 - Contexto documental unico y guards de Documentos

## Objetivo

Crear una base tecnica unica para gobernar el estado documental del modulo `radicacion`.

Esta deuda debe pagarse antes de integrar completamente pendientes, tomar pendiente o enviar a pendiente.

## Problema

Actualmente:

```txt
RadicacionTabs siempre muestra Captura de Documentos.
CapDocument siempre renderiza.
No existe RadicacionDocumentalContext.
No hay guard para estado = 0.
No hay ruta hija /registro/:idEstadoRadicado/documentos.
```

Regla obligatoria:

```txt
Documentos solo se activa con tramite documental activo estado = 0.
```

## Alcance

Crear o consolidar:

```txt
src/modules/radicacion/context/RadicacionDocumentalContext.tsx
src/modules/radicacion/hooks/useRadicacionDocumentalContext.ts
src/modules/radicacion/components/RadicacionDocumentosGuard.tsx
src/modules/radicacion/components/RadicacionStartupGuard.tsx
```

El contexto debe soportar como minimo:

```ts
type RadicacionDocumentalContextValue = {
  idEstadoRadicado: number | null;
  idRadicado?: number | null;
  consecutivoRadicado?: string | null;
  idTareaWorkflow?: number | null;
  estadoActual?: 0 | 1 | null;
  requiereGestionDocumental: boolean;
  tieneTramiteDocumentalActivoEstado0: boolean;
  destinoPostRegistro?: "resumen" | "documentos";
  setContextoDocumental: (value: RadicacionDocumentalState) => void;
  clearContextoDocumental: () => void;
};
```

## Integracion Esperada

- FE-06 debe usar este contexto para restaurar `estado-activo`.
- FE-05 debe usar este contexto al tomar pendiente.
- FE-07 debe limpiar este contexto al enviar a pendiente.
- `RadicacionTabs` debe leer este contexto para habilitar/bloquear `Documentos`.

## Guard

`RadicacionDocumentosGuard` debe permitir renderizar documentos solo si:

```txt
estadoActual === 0
tieneTramiteDocumentalActivoEstado0 === true
idEstadoRadicado > 0
```

Si no cumple:

- no renderizar `CapDocument`;
- no cargar gabinete;
- mostrar estado no disponible o redirigir a `Resumen`.

## Criterios de Aceptacion

- Existe contexto documental unico.
- `Documentos` no renderiza sin estado `0`.
- `RadicacionTabs` no activa documentos por defecto.
- FE-05, FE-06 y FE-07 tienen un punto comun de integracion.
- Tests cubren:
  - contexto inicial vacio;
  - set contexto estado `0`;
  - clear contexto;
  - guard permite documentos con estado `0`;
  - guard bloquea documentos sin estado `0`.

## Fuera de Alcance

- consumir APIs backend;
- implementar AppTable;
- implementar upload/digitalizacion.
