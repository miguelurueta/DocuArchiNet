# IMPORTAR-SERVICIO-ENLACE-SII

- Ticket: DOC-80
- Cambio OpenSpec: doc-80-importar-servicio-enlace-sii
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

`SiiImportProvider` despacha `ANEXOS_RADICADO_ENLASE`; `SiiExternalImportProviderClient` reutiliza token/transporte y llama `consultarRadicado`; `SiiEnlaseAnnexContractMapper` exige `idanexo` único. El ASMX reconstruye tarea y código de barras en servidor. Los errores se reducen a códigos seguros.
