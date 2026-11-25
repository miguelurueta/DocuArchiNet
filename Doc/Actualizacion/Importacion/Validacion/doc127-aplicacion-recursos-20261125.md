# DOC-127 — Aplicación delegada de recursos

**SQL30–32 aplicados y verificados; avance20/33.** El usuario solicitó «aplica por mi» después de revisar su finalidad. Codex ejecutó el mantenimiento sobre empresa2, DA11/RA12, referencia6, actor de configuración141, fuera de E2E. Esta delegación expresa corresponde sólo a estos archivos/ambiente; no se atribuye ejecución manual del usuario ni autorización para otras escrituras o pruebas reales.

## Resultado

- SQL30: dos tablas XLS nuevas InnoDB/utf8_general_ci, vacías, con columnas NOTNULL e índices UNIQUE completos sobre CLAVE y VALOR.
- SQL31: fuentes **10/11**, activas/revisión2, dos columnas cada una e historial ALTA/ACTIVAR. Se comprobó LISTO_PARA_COMMIT antes del COMMIT; relectura desde conexión nueva.
- SQL32: plantillas **61–68**, activas/revisión1 y16 campos, No/No/No/0. Se comprobó LISTO_PARA_COMMIT y8/16/8 antes del COMMIT.
- SQL33:13 SELECT desde nuevas conexiones para verificar cabeceras/campos, metadatos/índices, filas, políticas e historial. **119 comprobaciones de resultado PASS**; los seis destinos CREAR siguen ausentes.

| Plantilla | Destino | Formato / intención |
| --- | --- | --- |
| 61 | imp_02_doc127_e2e_agregar_xls | 8.0 xls / AGREGAR |
| 62 | imp_02_doc127_e2e_reemplazar_xls | 8.0 xls / REEMPLAZAR |
| 63 | imp_02_doc127_e2e_crear_xls_1000 | 8.0 xls / CREAR |
| 64 | imp_02_doc127_e2e_crear_xls_5000 | 8.0 xls / CREAR |
| 65 | imp_02_doc127_e2e_crear_xls_10000 | 8.0 xls / CREAR |
| 66 | imp_02_doc127_e2e_crear_xlsx_1000 | 12.0 xlsx / CREAR |
| 67 | imp_02_doc127_e2e_crear_xlsx_5000 | 12.0 xlsx / CREAR |
| 68 | imp_02_doc127_e2e_crear_xlsx_10000 | 12.0 xlsx / CREAR |

## Conservación y alcance

Se compararon huellas completas antes/después de **30 tablas de datos** y **78 perfiles**, además de todas las cabeceras/campos anteriores, ejecuciones y catálogo/columnas/historial de fuentes excluyendo únicamente las nuevas identidades autorizadas. Igualdad de filas y hashes en todos los conjuntos protegidos; no se guardaron valores de celdas en la evidencia. Se conserva el respaldo local de metadatos previo a los cambios.

Los controles explícitos fueron11 del preflight fresco,77 del preflight completo,150 del proceso de aplicación y13 de SQL33:251 SELECT. Se distinguen de subconsultas/resultados de los propios lotes y comandos internos del conector. El preflight completo comprobó ausencia de objetos nuevos, actor/permiso y ausencia de triggers en los catálogos afectados antes de escribir.

Los originales SQL30–32 conservan sus hashes y parámetros seguros; se ejecutaron copias locales con empresa2/DA11/ref6 y actor141. Sus comentarios de entrega «no aplicado» conservan el corte histórico: este informe y la guía actualizada registran la aplicación posterior. Las credenciales proceden del resolver existente y sólo estuvieron en memoria; ejecución, copias y respaldo permanecen en `.tmp/`, excluido de Git.

DLL, assets y Web.config conservan sus hashes. No hubo sesiones autenticadas de aplicación, importaciones, cambios de permisos o proveedores, limpieza ni EXPLAIN29. Las tablas XLS nuevas tienen cero filas; los datos anteriores mantienen sus huellas.

La tarea4.1 queda completada: las18 combinaciones ya tienen referencias de plantilla identificadas. Sus borradores privados siguen sin fechas/autorización/huellas finales de ventana y no son ejecutables. Los checkpoints previstos se sustituyen por controles frescos antes de cada ejecución autorizada. Siguen abiertas la aceptación real vigente, capacidad, EXPLAIN y cierre.

Validación documental posterior: refinamiento OPSXJ15/15 PASS, OpenSpec del cambio en modo estricto PASS y `git diff --check` PASS. Esta entrega no modifica código de producto ni acredita nuevos resultados funcionales.
