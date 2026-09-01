# Liberación, operación y rollback

## Precondiciones

1. Aplicar y verificar las migraciones DOC-42.
2. Compilar y ejecutar pruebas focales.
3. Confirmar que el gate sigue apagado y las audiencias están vacías.

## Activación controlada

La configuración final entregada es:

```text
WorkflowCentroTrabajoModernActive=false
WorkflowCentroTrabajoModernUsers=
WorkflowCentroTrabajoModernGroups=
```

Una activación posterior requiere aprobación operativa, audiencia explícita y monitoreo. Nunca se habilita globalmente como parte de este cambio.

## Rollback

1. Establecer `WorkflowCentroTrabajoModernActive=false`.
2. Vaciar usuarios y grupos piloto.
3. Reciclar la aplicación según el procedimiento vigente.
4. Verificar que reaparece el botón/modal/GridView legacy y que el panel moderno no se renderiza.

No se revierte DOC-42 ni se eliminan tablas: el rollback del consumidor es exclusivamente el gate.
