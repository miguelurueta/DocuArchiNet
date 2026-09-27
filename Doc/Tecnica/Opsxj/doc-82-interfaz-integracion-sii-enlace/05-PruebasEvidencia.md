# INTERFAZ-INTEGRACION-SII-ENLACE

- Ticket: DOC-82
- Cambio OpenSpec: doc-82-interfaz-integracion-sii-enlace
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- [x] unit: 2026-09-26, suites afectadas 103/103; contrato documental 4/4; MSBuild Debug, 0 errores.
- [x] manual_qa: 2026-09-26, E2E autenticada `import-sii-enlase-ui` sobre tarea autorizada 220587, PASS.

## QA/E2E WebForms

La plataforma E2E real reutilizada observó 9 anexos (6 disponibles y 3 importados) y confirmó preview de una sola descarga, foco, tabla responsive, preparación individual y preparación múltiple. Los 7 controles registrados permanecieron sin cambios. La corrida no creó intención, no ejecutó importación y no asignó la tarea. El gate terminó en `false`, con usuarios y grupos vacíos; la evidencia saneada no contiene credenciales, cookies ni cuerpos del proveedor.
