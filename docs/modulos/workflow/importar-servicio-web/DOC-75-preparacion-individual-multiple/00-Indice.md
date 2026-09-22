# DOC-75 — Preparación individual y múltiple

## Objetivo

Documentar la preparación previa a escritura para uno o varios recursos SII mediante catálogo autorizado, `PreflightImport` y una única `CreateImportIntent` idempotente.

## Alcance y componentes

- Estado: `importar-servicio-web-requirements.js`.
- Colección/adaptación: `importar-servicio-web-preparation.js`.
- Servicio cliente: `importar-servicio-web-intent-client.js` sobre el API existente.
- Presentación: popup aditivo de `Webworkflow.aspx`, UI y CSS modernos.
- Backend reutilizado: B03 (`PreflightImport`/`CreateImportIntent`), B09 (catálogo TRD) y B11 (`EffectPlans`).

## Documentos

Arquitectura, flujo, contrato/mapping, estados/antirregresión, pruebas, diagramas y metadata se describen en `01`–`07`; `Diagramas/` contiene secuencias individuales.
