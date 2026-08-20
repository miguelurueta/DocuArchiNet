## 1. Arnes E2E oficial

- [x] 1.1 Sustituir el modo y script `authorization` por `contexts`, con las variables de una segunda cuenta válida y sin referencias a piloto/no piloto.
- [x] 1.2 Adaptar `doc10-preview.spec.cjs` para que los previews autenticados rechacen los códigos de contexto/gate retirados, conserven el helper común y separen disponibilidad de negocio de autorización.
- [x] 1.3 Simplificar la E2E completa a la cuenta principal, manteniendo las huellas `SELECT` de estado y auditoría antes/después y el código funcional esperado opcional.

## 2. Documentación y regresión

- [x] 2.1 Actualizar README y AGENT-RUNBOOK para describir `test:contexts`, prohibir cambios de gate durante las corridas y conservar la verificación de cierre apagado.
- [x] 2.2 Actualizar las pruebas estáticas del arnés para cubrir los nuevos nombres, la ausencia de la expectativa legacy y la reutilización del helper existente.

## 3. Verificación autorizada

- [x] 3.1 Ejecutar las pruebas estáticas y la comprobación de sintaxis del arnés.
- [x] 3.2 Ejecutar, con las cuentas y ambiente ya autorizados, las corridas anónima, de contexto secundario y E2E completa de solo lectura; comprobar que estado, auditoría, gate y pantallas legacy permanecen sin cambios.
