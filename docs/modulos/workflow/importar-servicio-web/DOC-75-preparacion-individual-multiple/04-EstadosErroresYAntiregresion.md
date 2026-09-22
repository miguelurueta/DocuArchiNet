# Estados, errores y antirregresión

## Estados

`edicion` → `preparando` → `listo` → `creando` → `creado`. `bloqueado` cubre dependencias/planes no utilizables; `error` presenta mensajes funcionales sin detalles internos.

Confirmar permanece deshabilitado con selección vacía, tipología faltante, catálogo no autorizado, preflight inválido o `Executable=false`. `PREFLIGHT_STALE` borra la preparación y exige repetirla.

## Antirregresión

- Sin segundo `fetch`/XHR ni `ExecuteImportIntent`.
- Sin `ExpedientId`, caché, índices o efectos afirmados como realizados.
- Sin cambios a `JSExpediente.js`, `JSProgresBar.js`, almacenamiento o `ClassAlmacenamiento`.
- Gate apagado conserva flujo legacy.
- Foco, scroll y selección se restauran al cancelar.
