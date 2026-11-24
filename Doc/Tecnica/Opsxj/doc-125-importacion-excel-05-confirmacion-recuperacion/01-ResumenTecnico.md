# DOC-125 — Confirmación transaccional y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: implementación y aceptación real5/5 verificadas.

## Objetivo

Confirmar una carga PREPARADA/FALLIDA con identidad y snapshots íntegros, tres modos y resultado durable. Confirmar y ConsultarResultado están en el ASMX existente; el backend retorna la confirmación histórica antes de acceder a archivos/configuración actuales.31/31 tareas completas; CREAR/AGREGAR/REEMPLAZAR confirmaron1000filas cada uno y el rollback real restauró1000filas. La carrera real y la revisión técnica final también quedaron verificadas.

## Alcance y compatibilidad

Mantener net461, MySQL5.1.37, MySql.Data6.2.3, recursos RA/DA y lector DOC123. Reutilizar las22columnas operativas, incluidos conteo/UTC. Build net4610errores,94/94pruebas y4492checks nativos sintéticos con regresión heredada. DLL desplegada con respaldo y WSDL200. SQL18–21 aplicados/verificados fuera de tests por delegación expresa «ejecuta», sin grants ni cambios de Web.config. Se conservaron625contratos, EGRESOS, CRÉDITOS y configuración previa. Datos/resultados descartables permanecen; no hay limpieza. Contratos/límites en Diseno/05-confirmacion-recuperacion-doc125.md; evidencia vigente en Validacion/doc125-ejecucion-seguimiento-20261124.md/JSON, con cortes históricos separados.
