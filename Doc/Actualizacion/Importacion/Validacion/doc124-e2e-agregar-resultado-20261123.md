# DOC124: E2E real de AGREGAR aprobada

**PASS_REAL_DOC124_ADD:3/3 casos dentro de1/1 test Playwright,0 omitidos.**
Ventana d28f908a93171ecf8f8d413d90293604 autorizada por «lanza de nuevo», cerrada
con claves eliminadas. [Evidencia JSON](doc124-e2e-agregar-resultado-20261123.json).
Reutiliza la suite Excel, runner DOC110, sesión autenticada, safePost y controles
SELECT DOC32. No se creó otro sistema E2E ni se instaló un backend nuevo.

| Caso real | Resultado |
|---|---|
|8000 claves nuevas | PREPARADA/AGREGAR,8000 previstas y0 confirmadas. |
|2 claves con diferencia de mayúsculas y espacio final | CLAVES_INVALIDAS/CLAVE_DUPLICADA_ARCHIVO, RECIBIDA. La comparación se realizó en MySQL con latin1_swedish_ci. |
|1 clave existente en CONTRATOS | CLAVES_INVALIDAS/CLAVE_EXISTENTE_DESTINO, RECIBIDA. |

Propietario141, empresa2, DA11, conexión6, fuente5/revisión2 y plantilla28/revisión1.
El lector y el servicio comunicaron máximo20000 filas. Se verificaron ID previo,
recuperación desde otra sesión, replay idéntico de recepción, rechazo CSRF y cuenta
restringida. Para las8000 filas hubo dos sesiones ASP.NET independientes y una
contención real: un ganador y un OPERACION_EN_CURSO.

Se conservaron tres archivos privados y tres ejecuciones nuevas: una PREPARADA y
dos RECIBIDA con codigo_incidencia CLAVES_INVALIDAS. Los códigos específicos de los
duplicados se acreditaron en las respuestas comprobadas por la suite; el esquema
de ejecución solo persiste el código principal, no el detalle de incidencias.
Las tres ejecuciones tienen0 filas confirmadas. Sus IDs y hashes están en el JSON;
no se publicaron claves, NIT, nombres, muestras, credenciales o cuerpos de respuesta.

Huellas completas antes/después confirmaron que CONTRATOS, EGRESOS, CRÉDITOS,
plantillas/mapeos y fuentes/columnas/historial conservaron sus valores. Los controles
de la suite también comprobaron esquema y datos del destino por caso. La verificación
posterior independiente por SELECT confirmó625 registros de CONTRATOS, cero claves
nulas/vacías y los tres locks propios libres. No se inspeccionó globalmente el pool.

28 solicitudes a servicios Excel;57 SELECT de la ventana,1 de la parada anterior,
2 de diagnóstico y9 posteriores:69 controles explícitos acumulados, bajo el máximo80.
Los SELECT internos de los servicios y el preflight local de generación de archivos
se distinguen de ese conteo. Login, navegación y recursos estáticos tampoco se
presentan como solicitudes de los servicios Excel. El test duró118374ms incluyendo
autenticación/controles; no es una medición de rendimiento del lector o de importación.

Cero confirmación/importación, DDL permanente, guardados de plantillas, cambios de
fuentes/permisos, SQL17, despliegues o limpieza. El archivo de Órdenes y su carga
previa no se reutilizaron como cargas de CONTRATOS. Se conservaron los estados de las
ventanas anteriores; no hubo repetición de un recorrido que ya hubiese recibido archivos.

Junto con CREAR/Órdenes ya aprobado, esta evidencia cubre AGREGAR y ambos tipos de
duplicados. Falta REEMPLAZAR válido con coincidencia existente y su rechazo de
duplicados internos, después del cambio controlado de plantilla por SQL17.
Las tareas2.6/3.2 continúan abiertas,21/23; este PASS no autoriza archivo/cierre.
