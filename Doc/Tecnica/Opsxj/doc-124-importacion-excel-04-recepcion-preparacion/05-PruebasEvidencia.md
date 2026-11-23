# DOC-124 — 05-PruebasEvidencia

- Ticket: DOC-124
- Cambio OpenSpec: doc-124-importacion-excel-04-recepcion-preparacion
- Clasificacion: cross_cutting (Transversal)

Estado vigente2026-11-23: E2E real de Órdenes aprobada1/1,12759 filas PREPARADA,
ventana cerrada y recursos conservados. La matriz completa sigue pendiente. Los
cortes locales y de preparación que siguen son históricos; el resultado real
vigente está en doc124-e2e-ordenes-resultado-20261123.md/JSON.

## Evidencia requerida

Harness DOC122/DOC110 ampliado, lector/fixtures DOC123 reutilizados. DOC124 acredita490 comprobaciones locales de tokens, archivos, estados, fallos antes/después de commit, CAS, metadatos, temporales y descarte dirigido con dobles ADO. DOC122246 y DOC123312 se conservan:1048 comprobaciones nativas. Build net461 aislado; sin sustituir DLL IIS. Detalle y huellas en Doc/Actualizacion/Importacion/Validacion/doc124-validacion-local-20261123.md/json.

## QA/E2E WebForms

importacion-excel.spec.cjs y runner/consola existentes incorporan scope DOC124 independiente y controles SELECT reutilizados. Descubrimiento local3 casos: dos DOC122 y uno DOC124; no ejecutó login. La E2E real ASHX/ASMX/MySQL/archivo sigue NO_EJECUTADA por falta de autorización nueva y fixtures/destinos concretos confirmados. No se hereda DOC122 ni se declara PASS con dobles. Tests no instalan/siembran/limpian bases. Esta entrega no autenticó, cargó o ejecutó SQL real; tampoco acredita CI de la suite nativa nueva.

Corrección posterior2026-11-23 solicitada por el usuario: reutilizar fuentes/tablas existentes,
sin exigir otras descartables. Se retiró la exclusión por nombre y se reutilizó
queryFingerprint DOC32 para comprobar todas las filas, sin topar en10000 registros
del control Rows.14/14 pruebas focales aprobadas. El inspector DOC110, modo Doc124Excel,
ejecutó34 SELECT de resolución del módulo/metadatos en su última lectura completa; eso acredita inventario,
no E2E real. Fuentes consumidas EGRESOS/CRÉDITOS identificadas; condiciones de claves,
plantillas y actividad se documentan en Validacion/doc124-reutilizacion-recursos-20261123.md.

Nueva solicitud del usuario: plantilla de Órdenes2025 y20000 filas. Alta27/revisión1,
siete campos, verificada por SELECT mediante repositorio existente; sin tabla física
o filas importadas. La copia privada con encabezados en fila1 pasó lectura completa:
12759 órdenes,0 incidencias. Original intacto. Configuración20000 sin override local;
16/16 pruebas y1054 controles nativos (246+318+490), incluyendo20.000/20.001. Este
corte sí tiene escritura administrativa de configuración autorizada; no es E2E real.
Detalle en Validacion/doc124-ordenes-plantilla-20000-20261123.md/JSON.

E2E real posterior autorizada: PASS_REAL_DOC124_FILE1/1,0 omisiones/reintentos;
plantilla27,12759 órdenes en PREPARADA,0 confirmadas/importadas. Dos sesiones reales
con un ganador/OPERACION_EN_CURSO y cuenta restringida rechazada.12 llamadas Excel,
27 SELECT de ventana+5 posteriores; huellas de plantillas/mapeos/EGRESOS/CRÉDITOS/
CONTRATOS iguales y tabla Órdenes ausente. Ventana cerrada/claves eliminadas.
Resultado en Validacion/doc124-e2e-ordenes-resultado-20261123.md/JSON. No sustituye
la matriz completa de tres modos, pendiente2.6;21/23 tareas conservadas.
