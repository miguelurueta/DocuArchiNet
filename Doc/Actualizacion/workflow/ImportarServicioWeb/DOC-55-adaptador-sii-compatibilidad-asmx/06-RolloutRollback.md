# Rollout y rollback

- Ticket: DOC-55
- Gate: `WorkflowCentroTrabajoModernActive`

## Estado seguro

La configuración versionada mantiene el gate en `false`. DOC-55 no agrega usuarios o grupos de piloto y no altera las listas existentes. Con el gate apagado, cada método moderno retorna `FEATURE_DISABLED` antes de construir contexto, transporte o proveedor.

## Activación futura

La activación requiere una autorización operativa separada, URL SII válida y secreto suministrado por configuración segura. Esos valores no se documentan ni versionan aquí.

## Rollback

1. Establecer `WorkflowCentroTrabajoModernActive=false`.
2. Verificar que el ASMX moderno retorna `FEATURE_DISABLED`.
3. Mantener las rutas legacy sin redirección.
4. Si se requiere revertir código, retirar únicamente los archivos aditivos DOC-55 y sus entradas del proyecto.

El rollback no exige modificar `ClassAlmacenamiento`, ASMX históricos, `Integracionccv`, `ServiciosIntegracion` ni JavaScript.

