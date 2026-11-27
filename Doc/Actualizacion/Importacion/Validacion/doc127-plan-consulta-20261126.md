# DOC-127 — Plan manual de consulta revisado

**Tarea2.4 completada; advertencia de acceso sin índice.** El operador inició SQL29 escribiendo el comando en la consola local; Codex preparó la consola y revisó su resultado. Comando recibido 2026-11-27T00:41:14.5023131Z; plan obtenido 2026-11-27T00:41:16.7805108Z. Fecha local2026-11-26 (Bogotá). No se repitieron cargas.

Destino: empresa2, DA11/RA12, referencia6, MySQL5.1.37. Proveedor1/v1 PUBLICADA, activo, revisión4; fuente2/política CONEXION revisión2, sin columna empresarial. SQL29 conserva proyección NIT/BENEFICIARIO, alias histórico BENFICIARIO, igualdad textual CONSECUTIVO y LIMIT2. SHA-256 UTF-8/LF: eeb61ad01134374d4b58a42e537191627b8837c3bbaf19c52c42cbdac9937a0c.

| Tabla | type | possible_keys | key | key_len | ref | rows estimadas | Extra |
|---|---|---|---|---|---|---|---|
| imp_02_egresos | ALL | NULL | NULL | NULL | NULL |25538 | Using where |

Los metadatos no devolvieron índices. No hay componentes, orden, prefijo ni unicidad de índice que contrastar. CONSECUTIVO, NIT y BENEFICIARIO son varchar(250), latin1/latin1_swedish_ci, admiten NULL. El recorrido completo es coherente con esos metadatos. LIMIT2 limita la salida, sin garantizar cuántas filas examina el motor. rows es una estimación, sin medición de tiempo, lecturas reales ni cardinalidad de negocio. El literal es sintético; no certifica rendimiento de otras claves o políticas.

La revisión cumple D-10/RQ-10. La falta de índice queda como advertencia; SQL29 ordena registrar esa brecha, sin crear índices. No se exige una adecuación SQL nueva para dar por revisado el plan. Una optimización posterior necesita análisis de datos y entrega SQL manual independiente.

Efectos de esta sesión:5 SELECT de control,1 EXPLAIN iniciado por el operador,0 DDL/DML/importaciones/despliegues. Intento1 detenido antes del plan por guarda JSON privada corregida; intento2 detenido antes de acceder a base por cambio de huella. El intento3 conservó la confirmación manual obligatoria. Los archivos de ejecución quedan excluidos de Git, sin secretos en el informe.

Fuente evaluada: 8d1f2159ac4fdfda9fab3b884cbf5daa22c96390. Cambios concurrentes de diagnóstico de proveedor y editor de perfiles preservados; construcción SQL intacta. Regresión local de consumidores/editor/coordinación:29/29 PASS, sin base ni HTTP autenticado. Build MSBuild net461 nuevo:0 errores/308 advertencias, salidas aisladas; DLL IIS y Web.config conservados. El ensamblado nuevo no fue desplegado.

La página cambió referencias de caché del editor de perfiles. Los E2E anteriores mantienen su SHA/runtime original; no se afirma que todas sus fuentes sigan iguales ni se presenta la regresión local como E2E real. Huellas y comandos completos: [evidencia JSON](doc127-plan-consulta-20261126.json).

QA visual actual: **PENDIENTE**, consola local abierta. Abrir/cerrar Excel con Escape/Cerrar y comprobar navegación tras cerrar, sin nuevas altas o cargas. Sólo falta7.5: QA humana y revisión/gates del SHA final. Avance32/33; sin aprobación de cierre ni archivo.

Actualización posterior: la QA visual pendiente de este corte quedó satisfecha por [ejecución real de escritorio delegada](doc127-qa-delegada-20261126.md). No se altera el registro manual original de SQL29 ni se transforma PENDING en una declaración humana.
