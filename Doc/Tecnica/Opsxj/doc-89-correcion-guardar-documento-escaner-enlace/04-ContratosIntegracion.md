# DOC-89 - Contratos e integraciones

- Ticket: DOC-89
- Cambio OpenSpec: doc-89-correcion-guardar-documento-escaner-enlace
- Clasificacion: cross_cutting

## Contratos e integraciones

Se conservan el transporte Dynamsoft, los botones padres por contexto, el contrato de almacenamiento y la proyección incremental. La integración E2E añade únicamente observabilidad y controles de lectura.

## Escáner compartido

La prueba de política fija las continuaciones actuales de `Hidden21`:

| Valor | Contexto | Continuación preservada |
| --- | --- | --- |
| 1 | TRAMITE / TRAMITE_ADJUNTOWORKFLOW en Workflow | `window.parent.ButtonAlmacenar.click()` |
| 2 | Añadir a documento | `Button_añade_documento.click()` |
| 3 | MIGRACION | `window.parent.save_document_scan.click()` |
| 4 | TRAMITE SIMPLE | `window.parent.save_document_scan.click()` |
| 5 | REMPLAZAVERSION | `window.parent.Button_save_replace_dig.click()` |

PRODUCCION y las operaciones Nuevo, Agregar, Insertar y Reemplazar continúan usando el componente sin cambios. Tampoco cambian PDF/PDF-A/TIF, sesión, temporales o callbacks Dynamsoft.

## E2E registrada

El escenario `scanner-link-overlay-execution` reutiliza:

- `createAuthenticatedWorkflowSession` para autenticación efímera.
- La plataforma `test:workflow:platform` y su reserva local de tarea descartable.
- El control registrado `workflow-assignment-state`, un único `SELECT` parametrizado, esperado sin cambios.
- Navegador visible para operar el escáner real, sin login, `.env` ni proyecto Playwright paralelo.

El escenario exige autorizaciones `environment`, `account`, `execution` y `discardable-resource`. La evidencia contiene solo códigos, conteos y huellas.
