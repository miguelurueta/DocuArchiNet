# Contrato y adaptadores

El cliente publica las ocho operaciones de `WebServiceImportarServicioWebModern.asmx` y envía `{ request: dto }` con credenciales de mismo origen. Solo acepta el envelope ASMX `d` como objeto o JSON válido.

El registro normaliza el proveedor con trim y mayúsculas. Un adaptador expone `queryItems`, `executeImportIntent` y capacidades copiadas defensivamente. Ausencia, proveedor conocido no migrado y desconocido producen respectivamente `PROVIDER_NOT_CONFIGURED`, `PROVIDER_NOT_MIGRATED` y `PROVIDER_NOT_SUPPORTED`; no hay fallback implícito.
