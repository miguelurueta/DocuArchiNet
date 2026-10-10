# DOBLE-FACTOR-PREAUTENTICACION

- Ticket: DOC-94
- Cambio OpenSpec: doc-94-doble-factor-preautenticacion
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Integrar una compuerta de preautenticación común para los módulos Gestor,
DocuArchi, Radicación y Workflow. La contraseña se valida por el mecanismo
legacy existente y luego se descarta; el servicio resuelve configuración e
identidad por módulo y solo ejecuta el finalizador legacy cuando el segundo
factor está desactivado (`0` o `NULL`). Si está activo (`1`), devuelve
`SECOND_FACTOR_REQUIRED` sin crear sesión ni ejecutar auditoría.

## Alcance y compatibilidad

- [x] Superficie afectada identificada: `ClassGestorSesion`, modelos e
  interfaces 2FA, repositorios por módulo, servicio de preautenticación,
  proyecto VB, pruebas, CI y documentación técnica.
- [x] Compatibilidad registrada: no cambia la validación de credenciales, la
  recuperación de contraseña, `ClassCorreo`, el esquema de base de datos ni
  el finalizador de sesión existente. El rollback consiste en revertir el
  artefacto de aplicación; no requiere rollback SQL.
