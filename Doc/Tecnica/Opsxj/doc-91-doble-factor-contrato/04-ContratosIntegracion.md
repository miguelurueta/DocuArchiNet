# DOBLE-FACTOR-CONTRATO

- Ticket: DOC-91
- Cambio OpenSpec: doc-91-doble-factor-contrato
- Clasificacion: cross_cutting

## Contratos e integraciones

No se publica handler ni endpoint y no se toca el esquema. El HMAC usa `v1:<keyId>:<base64mac>` y configuración externa inyectable mediante `LoginSecondFactorHmacActiveKeyId` y `LoginSecondFactorHmacKey.<keyId>`. El adaptador WebForms conserva únicamente contexto pendiente sanitizado; dominio, DTO y puertos no dependen de Session, SMTP o SQL.

Por ausencia de endpoint, verbo HTTP, ruta, autorización y DTO de transporte son “No aplica”. Los DTO definidos aún no tienen consumidor HTTP.
