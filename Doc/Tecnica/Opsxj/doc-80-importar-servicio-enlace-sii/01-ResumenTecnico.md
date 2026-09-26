# IMPORTAR-SERVICIO-ENLACE-SII

- Ticket: DOC-80
- Cambio OpenSpec: doc-80-importar-servicio-enlace-sii
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Publicar consulta y preview seguros de anexos SII para preasignacion ENLASE, reutilizando el proveedor INTEGRACIONSII y preservando el flujo legacy.

## Alcance y compatibilidad

- [x] Afectados: DTO base, proveedor/cliente/mapper SII, ASMX moderno y plataforma E2E. No se modifican paginas WebForms ni legacy.
- [x] Preservado: constancias, almacenamiento y endpoints legacy. Reversa: retirar despacho por capacidad, mapper y escenario aditivos.
