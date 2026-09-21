# Estados

La máquina admite `cerrado`, `resolviendo-proveedor`, `consultando`, `vacio`, `resultados`, `preparando`, `ejecutando`, `reconciliando`, `completado` y `error`. Toda transición no declarada falla con `IMPORT_STATE_TRANSITION_INVALID`.

Durante `ejecutando` existe una única promesa compartida por intención y una espera global indeterminada. Solicitudes concurrentes reciben esa misma promesa, por lo que el navegador no duplica `ExecuteImportIntent`.
