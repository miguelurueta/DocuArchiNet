# DOC124: E2E real REEMPLAZAR aprobada

**PASS_REAL_DOC124_REPLACE:2/2 casos en1/1 test Playwright,0 omitidos.**
Ventana468f13144505a8faf2dfdd111bcaddbb, autorizada por «lanza de nuevo me equivoque»,
cerrada con claves eliminadas. [Evidencia JSON](doc124-e2e-reemplazar-resultado-20261123.json).
El intento anterior no superó la captura de claves y no tuvo SELECT ni servicios.

| Caso real | Resultado |
|---|---|
|8000 filas, una clave existente y7999 nuevas | OK/PREPARADA/REEMPLAZAR;8000 previstas y0 confirmadas. La coincidencia existente se acepta. |
|2 claves equivalentes por mayúsculas/espacio final | CLAVES_INVALIDAS y CLAVE_DUPLICADA_ARCHIVO; RECIBIDA. |

Propietario141/empresa2, DA11/conexión6, fuente5/revisión2, plantilla28/revisión2.
SQL17 había cambiado únicamente flags/modo y revisión de plantilla, por autorización
delegada; no se aplicó SQL desde la E2E ni se modificaron sus mapeos.
Se reutilizaron runner DOC110, suite Excel, generador Book/lector DOC123, sesión,
safePost, consola privada y controles SELECT DOC32, con el mismo backend servido.

La suite comprobó ID previo sin registro ficticio, recepción ASHX, recuperación
en otra sesión, replay idéntico, CSRF inválido, consulta propia, cuenta restringida
y muestras acotadas. Dos sesiones ASP.NET independientes prepararon el caso válido:
un ganador y un OPERACION_EN_CURSO. La comparación de duplicados se realizó en
MySQL con tipo/collation físicos, no en un doble local.

Dos archivos privados y dos ejecuciones quedaron conservados: una PREPARADA y
una RECIBIDA con código principal CLAVES_INVALIDAS. El código específico se
comprobó en la respuesta; no se atribuye su persistencia detallada al esquema
que solo guarda el principal. Se verificaron archivos y documento efectivo
válido por hash, empresa, módulo, plantilla y límite20000.

Huellas completas antes/después conservaron CONTRATOS, EGRESOS, CRÉDITOS,
plantillas/campos y fuentes/columnas/historial. La suite comprobó además esquema
y datos por caso. La verificación posterior independiente confirmó625 registros
de CONTRATOS, cero claves nulas/vacías y ambos locks propios libres.

20 llamadas a servicios Excel y48 SELECT de ventana más7 posteriores:55 controles
externos explícitos, dentro del máximo80. Login/navegación/estáticos y SQL interno
de servicios se distinguen de esos conteos. El test completo duró93846ms; no
acredita rendimiento aislado del lector ni una carga de20000 filas.

Cero confirmación/importación, DELETE/INSERT de negocio, DDL permanente, guardados
de plantillas, cambios de fuentes/permisos, despliegue o limpieza. Las seis cargas
de aceptación de los tres modos se conservan. Con CREAR y AGREGAR anteriores,
los tres modos y las incidencias requeridas ya tienen evidencia real.
