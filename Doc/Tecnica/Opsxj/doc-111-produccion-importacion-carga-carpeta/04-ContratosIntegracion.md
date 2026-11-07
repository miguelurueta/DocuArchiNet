# PRODUCCION-IMPORTACION-CARGA-CARPETA

- Ticket: DOC-111
- Cambio OpenSpec: doc-111-produccion-importacion-carga-carpeta
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

La propuesta amplía el ASMX de ImportacionCarpetas con dispatcher operativo y un ASHX propio para multipart, conservando contratos administrativos DOC-110 y handler individual. IMPORTAR_CARPETAS no concede administración; usuario/empresa/nivel se verifican por acción y JSON/multipart llevan protección ligada al contexto.

El plan completo congelado precede a confirmación idempotente y recepción. SQL 02 de operación se entrega manualmente en 07-A; no existe todavía ni se aplica por este refinamiento. No duplicar las seis entidades del modelo común o los catálogos/perfil. Los adaptadores 08/09 son internos y no eluden coordinación. Contratos detallados en Diseno/02-contratos-operacion.md y 15-integracion-carga-doc111.md.
