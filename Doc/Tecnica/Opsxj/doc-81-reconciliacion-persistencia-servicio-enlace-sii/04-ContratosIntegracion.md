# RECONCILIACION-PERSISTENCIA-SERVICIO-ENLACE-SII

- Ticket: DOC-81
- Cambio OpenSpec: doc-81-reconciliacion-persistencia-servicio-enlace-sii
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

- Endpoints ASMX: `PreflightImport`, `CreateImportIntent`, `ExecuteImportIntent`, `GetImportIntent` y `ReconcileImportIntent`; autenticación por sesión Workflow y gate existente.
- Proveedor único: `INTEGRACIONSII`; capacidad `ANEXOS_RADICADO_ENLASE`.
- Autoridad: recibo y código de barras se reconstruyen en servidor; `ProviderReference` persistida no se acepta libremente del navegador.
- Legacy: `PreAlmacenaDocumentoAnexosEnlaceIntegracionSII(..., Optional RutaArchivoPreparada As String = Nothing)`; llamadores anteriores omiten el nuevo argumento.
- Esquema: migraciones `001-add-intent-capability.sql` y rollback `002-rollback-intent-capability.sql` sobre tablas modernas.
- E2E: escenario `import-sii-enlase-execution`, perfil sin secretos, autorizaciones `environment,gate,execution,discardable-resource`, controles registrados exclusivamente `SELECT`.
- Errores: códigos públicos saneados; ASMX normalmente transporta el error funcional dentro del envelope HTTP 200.