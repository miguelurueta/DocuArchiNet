# DOC-125 — Confirmación transaccional y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: implementación local verificada, aceptación real abierta.

## Objetivo

Confirmar una carga PREPARADA/FALLIDA con identidad y snapshots íntegros, tres modos y resultado durable. Confirmar y ConsultarResultado están en el ASMX existente; el backend retorna la confirmación histórica antes de acceder a archivos/configuración actuales. 26/31 tareas completas; faltan diagnóstico manual, aceptación real de rollback/carrera, matriz/capacidad y revisión final.

## Alcance y compatibilidad

Mantener net461, MySQL5.1.37, MySql.Data6.2.3, recursos RA/DA y lector DOC123. Reutilizar las 22 columnas operativas, incluidos conteo/UTC; no hay migración nueva de resultado. Build aislado sin errores, 27/27 pruebas locales y regresión heredada. Los 4348 checks nativos son sintéticos y no acreditan MySQL real. No se desplegó ni ejecutó SQL real. Contratos y límites en Doc/Actualizacion/Importacion/Diseno/05-confirmacion-recuperacion-doc125.md; evidencia en Validacion/doc125-implementacion-local-20261123.md/JSON.
