# Metadata

- Ticket: DOC-75
- Rama: `feature/DOC-75`
- Cambio: `doc-75-importacion-interfaz-integracion-sii`
- Fecha: 2026-09-22
- Estado: implementación en validación
- Dependencias: B03, B09, B11, DOC-72, DOC-74
- Riesgo: disponibilidad productiva de B11
- Rollout: gate existente, piloto controlado
- Rollback: retirar popup, listeners, estilos y registros aditivos

## Funciones creadas o modificadas

| Función | Ruta | Responsabilidad |
| --- | --- | --- |
| `transition`, `canConfirm` | `importar-servicio-web-requirements.js` | Estado y habilitación cerrada |
| `individual`, `multiple`, `assignDocumentType` | `importar-servicio-web-preparation.js` | Colección y catálogo |
| `preflight`, `confirm` | `importar-servicio-web-intent-client.js` | Mediación B03 e intención única |
| `openPreparation`, `closePreparation`, `renderPreparationPlan` | `importar-servicio-web-ui.js` | Popup, plan y foco |
