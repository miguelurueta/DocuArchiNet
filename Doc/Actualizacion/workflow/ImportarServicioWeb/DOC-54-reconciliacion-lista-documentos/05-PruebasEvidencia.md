# Pruebas y evidencia

- Ticket: DOC-54
- Cambio OpenSpec: doc-54-reconciliacion-lista-documetos
- Clasificacion: cross_cutting

La suite focal cubre reconstrucción completa/parcial, consulta por identidad, SQL parametrizado/read-only, autorización opaca, tarea distinta, relación ausente/duplicada, tabla de fases, incertidumbre, contrato saneado, deduplicación y correlación.

Comando focal: `node --test Tests/importar-servicio-web-reconciliation.test.cjs Tests/importar-servicio-web-document-list-contract.test.cjs Tests/importar-servicio-web-reconciliation-authorization.test.cjs`. Resultado inicial: 11/11 aprobado. El build VB.NET compila sin errores y conserva advertencias históricas. No se ejecutaron base real, DDL, E2E autenticado, carga ni gates.
