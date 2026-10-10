## Why

El login privado valida credenciales y, en el mismo método, inicializa sesiones, relaciones, permisos, auditoría y la cookie de autenticación. Ese acoplamiento impide detener el recorrido con seguridad cuando un módulo requiere segundo factor. DOC-94 introduce la frontera de preautenticación sin alterar todavía challenge, correo, servicios web ni interfaz.

## What Changes

- Separar el bloque posterior a `ValidaUserAplicacion` en un único finalizador legacy tipado y reutilizable, sin redirect interno.
- Mantener un wrapper compatible que preserve el flujo actual cuando 2FA sea `0` o `NULL` y redirija únicamente tras finalizar con éxito.
- Incorporar un servicio de preautenticación que consulte autoritativamente la configuración central y decida entre finalización inmediata o `SECOND_FACTOR_REQUIRED`.
- Incorporar un repositorio central y cuatro adaptadores parametrizados de identidad/correo: DocuArchi, Gestor, Radicación y Workflow.
- Evolucionar el contrato de finalización creado en DOC-91 para que no dependa de un challenge todavía inexistente.
- Añadir caracterización, pruebas unitarias y documentación técnica trazable.

No se crea challenge, no se envía correo, no se expone ASMX y no se modifica UI en DOC-94.

## Capabilities

### New Capabilities

- `doble-factor-preautenticacion`: validación legacy seguida de decisión 2FA y resolución autoritativa de identidad para cuatro módulos.

### Modified Capabilities

- `doble-factor-contrato`: desacopla el contexto del finalizador legacy del contexto persistido del challenge.

## Impact

- Código: `gestor.aspx.vb`, `Defaul/ClassGestorSesion.vb`, contratos/modelos de segundo factor, nuevos servicios y repositorios, proyecto VB y pruebas.
- Datos: solo lectura de tablas existentes; no agrega tablas, columnas ni migraciones.
- Compatibilidad: `ValidaUserAplicacion`, recuperación de contraseña y `ClassCorreo` permanecen intactos.
- Operación: una configuración activa (`1`) detiene el login hasta que una Jira posterior implemente challenge/UI; despliegue condicionado a módulos en `0`/`NULL`.
