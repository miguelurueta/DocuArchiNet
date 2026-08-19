## 1. Helper de sesión autenticada

- [x] 1.1 Crear `tools/e2e/tests/support/authenticated-workflow-session.cjs` con el bootstrap de `gestor.aspx`, configuración explícita por variables de entorno y errores sanitizados. Verificación: el módulo no contiene valores de credenciales, cookies ni almacenamiento de sesión.
- [x] 1.2 Agregar pruebas Node locales para configuración, selectores, postback y cierre del contexto ante fallo usando dobles de Playwright. Verificación: la suite no requiere red ni variables con secretos.

## 2. Reutilización en suites existentes

- [x] 2.1 Migrar `tools/e2e/tests/doc10-preview.spec.cjs` y `tools/e2e/scripts/run-doc10-concurrency.cjs` para usar el helper, preservando nombres de variables, aislamiento de contexto y cobertura de preview. Verificación: no quedan implementaciones locales de login duplicadas.
- [x] 2.2 Migrar `tools/e2e/tests/doc11-execution.spec.cjs` y `tools/e2e/scripts/run-doc11-concurrency.cjs` para usar el helper, preservando los controles explícitos de ejecución y las consultas de solo lectura. Verificación: no cambia ningún payload ni endpoint de DOC-11.

## 3. Documentación y verificación local

- [x] 3.1 Documentar en `tools/e2e/README.md` el contrato del helper, la propiedad del contexto y la prohibición de versionar secretos, cookies o `storageState`. Verificación: las instrucciones permiten añadir una suite sin copiar el login.
- [x] 3.2 Ejecutar pruebas CJS del helper y las comprobaciones estáticas de las suites afectadas, sin E2E autenticada. Verificación: los comandos, resultado y limitaciones quedan registrados en el resultado de implementación.
