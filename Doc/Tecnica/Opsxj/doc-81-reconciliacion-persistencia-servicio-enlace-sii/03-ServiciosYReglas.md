# RECONCILIACION-PERSISTENCIA-SERVICIO-ENLACE-SII

- Ticket: DOC-81
- Cambio OpenSpec: doc-81-reconciliacion-persistencia-servicio-enlace-sii
- Clasificacion: cross_cutting (Transversal)

## Servicios y reglas

- `ServicioPreflightImportacion.Preflight`: selección no vacía ni duplicada, capacidad coincidente, tipología autorizada y bloqueo exclusivo de documento vigente.
- `ServicioIntencionImportacion.Crear/Canonical`: una intención por colección; huella con contexto, capacidad, referencia SII, identidad externa y tipología; URL temporal excluida.
- `ImportServiceOrchestrator.Execute`: guard, versión, revalidación y pasos secuenciales por elemento; una intención completada se omite idempotentemente.
- `SiiExternalImportProviderClient.DownloadAnnexAsync`: reconsulta `consultarRadicado`, resuelve `idanexo` y reutiliza allowlist/límite/formato.
- `LegacyEnlaseImportDocumentStorageAdapter.Almacenar`: valida sesión ENLASE y llama una vez la función legacy con archivo preparado.
- `VerifyStoredImportExecutionStep.Ejecutar`: exige un documento, una relación de la tarea y un recurso físico.
- `ServicioReconciliacionImportacion.ReconcileImportIntent`: `Disponible`, `Recuperable`, `Inconsistente`, `ResultadoIncierto` y agregado honesto.

Los resultados inciertos nunca se reintentan automáticamente. ENLASE usa `SinExpediente`; relación y caché de expediente son `NoAplica`.