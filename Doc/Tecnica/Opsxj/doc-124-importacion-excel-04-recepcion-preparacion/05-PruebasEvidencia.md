# DOC-124 — 05-PruebasEvidencia

- Ticket: DOC-124
- Cambio OpenSpec: doc-124-importacion-excel-04-recepcion-preparacion
- Clasificacion: cross_cutting (Transversal)

## Evidencia requerida

Harness DOC122/DOC110 ampliado, lector/fixtures DOC123 reutilizados. DOC124 acredita490 comprobaciones locales de tokens, archivos, estados, fallos antes/después de commit, CAS, metadatos, temporales y descarte dirigido con dobles ADO. DOC122246 y DOC123312 se conservan:1048 comprobaciones nativas. Build net461 aislado; sin sustituir DLL IIS. Detalle y huellas en Doc/Actualizacion/Importacion/Validacion/doc124-validacion-local-20261123.md/json.

## QA/E2E WebForms

importacion-excel.spec.cjs y runner/consola existentes incorporan scope DOC124 independiente y controles SELECT reutilizados. Descubrimiento local3 casos: dos DOC122 y uno DOC124; no ejecutó login. La E2E real ASHX/ASMX/MySQL/archivo sigue NO_EJECUTADA por falta de autorización nueva y fixtures/destinos concretos confirmados. No se hereda DOC122 ni se declara PASS con dobles. Tests no instalan/siembran/limpian bases. Esta entrega no autenticó, cargó o ejecutó SQL real; tampoco acredita CI de la suite nativa nueva.
