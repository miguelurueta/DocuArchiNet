# ACTUALIZACION-UTIL-CREA-EXPEDIENTE-SII

- Ticket: DOC-71
- Cambio OpenSpec: doc-71-actualizacion-util-crea-expediente-sii
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

- `MySqlImportExpedientConfigurationRepository` deriva el modo exclusivamente del trámite confiable.
- `ImportExpedientCoordinator` bifurca antes de sujeto, caché, búsqueda y creación.
- `ImportRelatedDocumentCoordinator` descubre por `ENLASE` y procesa cada `IdImagen` secuencialmente.
- `SiiDocumentIndexAdapter` actualiza solo NIT/cédula, razón social y matrícula efectivamente presentes.
- Sin expediente se usa `ID + ENLASE`; con expediente se conserva `ID + ID_EXPEDIENTE`.
- Razón social, NIT/cédula y matrícula ausentes no bloquean ni borran valores existentes.
- Cada efecto se persiste y confirma por postlectura; fallos e incertidumbre usan códigos estables sin SQL
  ni datos personales.
