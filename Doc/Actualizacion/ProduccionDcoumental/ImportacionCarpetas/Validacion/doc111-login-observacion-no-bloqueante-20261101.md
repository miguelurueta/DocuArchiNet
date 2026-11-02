# DOC-111: observación del postback y verificación de sesión

Resultado real posterior: sesión 17 **DIAGNOSTIC_PASS** a las 17:40:06 UTC. Postback 200/FINISH COMPLETE, cuerpo BODY_NOT_RETAINED, campos enviados MATCHED, Producción 200 y control/token listos. El acceso está confirmado pese a que Chromium no conservó el cuerpo; no se atribuye el fallo anterior a credenciales. No se creó ningún recurso. SELECT de las 17:40:50 UTC conserva lote 2 COMPLETADO/revisión 25 y cero coincidencias para los nombres EXPEDIENTE aprobados. Se prepara sesión 18 del REC completo con la misma autorización específica `doc111-expediente-authorization-15.json`, perfil 24/versión 37 y nombres originales; no se repite un lote/plan existente. Resultado pendiente de credenciales/corrida, sin REC PASS todavía.

La sesión 16 de diagnóstico terminó a las 16:37:33 UTC con `DOC111_LOGIN_POSTBACK_UNAVAILABLE`, sin importación o referencias. El consumidor anterior agrupaba excepciones de finalización/lectura del postback y detenía el flujo antes de comprobar el acceso protegido. Ese resultado no prueba rechazo de credenciales ni permite determinar retrospectivamente cuál de las dos lecturas falló.

## Corrección del observador

Se separa la disponibilidad de la observación del resultado de autenticación. Si no se puede terminar/leer el postback, se registra su etapa/categoría y se consulta el acceso protegido mediante el flujo existente, una sola vez. Solo se acepta sesión cuando Producción, el control operativo y su token están presentes. Un 401 de la consulta protegida sigue rechazado; un rechazo explícito del servidor, HTTP de error o segundo factor pendiente también detienen el flujo. No se reenvían credenciales ni se omite MFA.

La salida saneada incorpora únicamente:

- HTTP del postback, Producción y sonda, cuando estén disponibles.
- FINISH/BODY y estados de lectura; categorías cerradas de target cerrado, navegación abortada, cuerpo no retenido, timeout o sin clasificar.
- MATCHED/MISSING/DIFFERENT para comparar en memoria los campos enviados con la entrada efímera de la aplicación. No contiene ni modifica usuario/contraseña ni guarda formulario, viewstate, cuerpo HTTP, cookies, texto del diálogo o excepción.

El runner y la suite filtran esta estructura por catálogo antes de guardarla; no aceptan campos adicionales privados. Una lectura fallida no concede sesión y tampoco invalida por sí sola un control/token autenticado comprobado. La inspección del código local de Playwright 1.62.1 confirma que `finished()` y la lectura del cuerpo son operaciones distintas.

## Validación y continuación

**57 pruebas locales PASS**, incluyendo observación fallida con sesión válida, observación fallida con sonda 401 que sigue denegada, postback/diálogo de rechazo explícito, segundo factor, error HTTP, redirect, ausencia de contenido privado y una sola invocación del login. OpenSpec estricto PASS. Los 27 archivos protegidos siguen idénticos a HEAD normalizando finales de línea.

Manifiesto `doc111-sha-login-observacion-20261101.json`: fuentes SHA-256 `0f8b54f33129c5102b09f00099d6a9c7d769e5576046428a858bc6824092be96`; DLL activa sin cambios `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`. No hubo despliegue, cambios de permisos/configuración o efectos documentales.

Consola preparada `.tmp/doc111-diagnostic-console-17.ps1`, reutilizando `--diagnose`, perfil EXPEDIENTE y autorización de ambiente/cuenta existentes. Solo dos entradas privadas de aplicación, enmascaradas; no solicita MySQL ni conecta controles ODBC. Efectos deshabilitados y sin REC automático después del diagnóstico. Estado/resultado en `tools/e2e/artifacts/doc111-diagnostic-session-17.json`. El siguiente resultado permitirá distinguir la lectura no disponible de la respuesta del acceso protegido; un PASS solo acredita acceso, no REC completo.

**123/130**. La autorización del REC EXPEDIENTE sigue vigente, su ejecución permanece pendiente y el lote 2 aprobado se conserva.
