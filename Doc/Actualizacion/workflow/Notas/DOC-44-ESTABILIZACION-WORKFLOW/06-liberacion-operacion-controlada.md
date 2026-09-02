# Liberación y operación controlada

## Estado de entrega

- `WorkflowCentroTrabajoModernActive=false`.
- `WorkflowCentroTrabajoModernUsers` y `WorkflowCentroTrabajoModernGroups` vacíos.
- No se habilitan audiencias ni endpoints legacy.

## Rollback

Establecer o conservar el gate en `false` y audiencias vacías. Esto oculta el panel moderno, evita su bootstrap y deja disponible botón/modal/GridView legacy. No revertir datos, tablas, contratos ni migraciones DOC-42.

## Operación de prueba

La corrida real requiere consola TTY y autorización separada para ambiente, mutación y habilitación temporal. Al finalizar —éxito o error— el runner restaura la configuración original segura. La evidencia no contiene credenciales, cookies, contenido de notas ni cadenas de conexión.
