# DOC-124 — E2E real de Órdenes2025 aprobada

**PASS_REAL_DOC124_FILE:1/1 recorrido aprobado,0 omitidos,0 reintentos.**
Ventana `7a82614ded3c44f8b45af3d95ab4e70b` autorizada por «lanza la E2E» y cerrada
con credenciales eliminadas. [Evidencia JSON](doc124-e2e-ordenes-resultado-20261123.json).
Se reutilizaron runner DOC110, suite Excel DOC122/DOC124, perfil DOC88, autenticación,
consola privada y controles SELECT DOC32. No se creó otra infraestructura de pruebas.

## Resultado real

| Comprobación | Resultado |
|---|---|
| Plantilla27/revisión1, conexión6, Hoja1/NROORDEN | Coinciden en la sesión y en el registro de ejecución. |
| Límite comunicado por servidor |20000 filas. |
| ID conocido antes de enviar bytes | Consulta previa sin fila devuelve RESULTADO_INCIERTO. |
| Recepción ASHX y recuperación en otra sesión | RECIBIDA, bytes/hash correctos y mismo ID. |
| Replay explícito del mismo archivo/ID | Reutiliza recepción, sin crear una segunda carga. |
| Preparar con CSRF inválido | SOLICITUD_NO_VERIFICADA. |
| Dos sesiones independientes del propietario | Cookies de sesión distintas verificadas en memoria. |
| Preparar concurrente | Un ganador y un OPERACION_EN_CURSO. |
| Lectura y preparación |12759 filas, PREPARADA y muestra acotada. |
| Cuenta restringida | Acceso rechazado. |
| Consulta posterior y SELECT de ejecución | PREPARADA/CREAR,12759 previstas y0 confirmadas. |
| Archivo privado y documento efectivo | Hashes verificados; límite20000/empresa2/módulo11/huella de plantilla correctos. |
| Lock del escritor después del cierre | Libre, mediante IS_FREE_LOCK de su identidad física. |
| Plantillas/mapeos y EGRESOS/CRÉDITOS/CONTRATOS | Huellas completas iguales antes/después. |
| Tabla física imp_02_ordenes_2025 | Sigue ausente;0 filas importadas. |

Carga: `7c5d70b7-ecb6-45ca-9698-deac40b61db8`, propietario141/empresa2/módulo11.
Un único archivo completo de607575 bytes, correspondiente a la copia compatible
con12759 órdenes. Se conservan el archivo, ejecución PREPARADA y evidencia local;
no se limpian para fabricar otro recorrido.

## Operación y controles

12 solicitudes a servicios Excel;3 sesiones autenticadas (dos del propietario y
una restringida). El conteo12 no incluye login, navegación o recursos estáticos.
27 SELECT externos de la ventana y5 SELECT posteriores mediante recurso de módulo
existente:32 controles explícitos. Los SELECT internos de los servicios se distinguen
de estos controles. Las verificaciones posteriores sólo leen ejecución, identidad,
actividad del lock y ausencia del destino; no reintentan preparación o recepción.

El alcance permite hasta40 solicitudes de servicios/60 SELECT y20min. Se cumplieron
los presupuestos;0 solicitudes GuardarPlantilla,0 activaciones/permisos,0 DDL
permanente,0 confirmación/importación y0 limpieza. Las temporales y la fila de carga
son efectos de la preparación autorizada. No se recuperaron secretos de ventanas
anteriores ni se guardaron cookies/tokens/contraseñas en los informes.

Se desplegó la DLL verificada y dos dependencias del lector antes de autenticar,
con respaldo de la anterior; WSDL200 confirmó los nuevos métodos. MySql.Data y
Web.config se conservaron durante ese despliegue. La configuración20000 corresponde
al ajuste previo autorizado. La DLL queda disponible para consultar esta carga.

## Alcance de la aceptación

Este PASS acredita el archivo real y el modo CREAR hasta PREPARADA. No ejecuta
CREATE de negocio, INSERT/DELETE de órdenes ni activación de fuente de índices.
La contención se observó entre dos sesiones reales, sin confundirla con serialización
de una sesión ASP.NET. IS_FREE_LOCK acredita liberación del escritor; no se afirma
una inspección global de temporales o del estado de todas las conexiones del pool.

La matriz completa de tres modos, duplicados internos/existentes y sus condiciones
de destinos sigue pendiente. No sustituirla por este caso ni marcar2.6/3.2 completas:
DOC124 conserva21/23 tareas. Focal16/16 y descubrimiento4 casos acreditaron el arnés
antes del recorrido; los informes locales anteriores no se presentan como E2E real.
