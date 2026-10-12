# DOC-97 — Seguridad y regresión transversal Login 2FA

## Alcance

Auditoría del incremento DOC-91 a DOC-96 sin DDL, endpoints nuevos ni cambios en `ClassCorreo` o recuperación de contraseña. La implementación productiva no cambió: las pruebas no reprodujeron un defecto del contrato aprobado.

## Entregables

- `arquitectura-y-matriz.md`: flujo real, módulos, escenarios y riesgos.
- `validacion-y-evidencia.md`: comandos, resultados y bloqueos.
- `tests/Fixtures/Login/SegundoFactor/doc97-regression-manifest.json`: inventario ejecutable.
- `tests/login-second-factor-regression.test.cjs`: guardas transversales.

## Resultado

La matriz determinista aprobó los cuatro módulos. La E2E DOC-97 aprobó GESTOR con 2FA activo y carrera de dos verificaciones. Los escenarios reales que necesitarían modificar flags quedaron bloqueados para preservar la configuración del ambiente.
