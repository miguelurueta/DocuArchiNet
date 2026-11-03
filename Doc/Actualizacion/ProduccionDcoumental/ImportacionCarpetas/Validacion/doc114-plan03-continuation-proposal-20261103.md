# DOC-114: continuación PLAN-03 y un rechazo adicional

El primer intento creó el plan base 22 y se detuvo en injected-result. IIS registró HTTP 400; el cuerpo autenticado de ese fallo no se retuvo. El validador de la DLL servida, invocado localmente sin HTTP/BD, rechaza ambos campos desconocidos con **CONFIGURACION_INVALIDA**, mientras el fixture pedía SOLICITUD_INVALIDA. Se corrigió el código exacto esperado; no cambia el producto ni se aceptan códigos alternativos indiscriminados. El plan 22 pasó la verificación de respuesta/persistencia antes de avanzar al caso fallido.

El permiso no llegó a revocarse: la lectura posterior confirmó 1 y otros campos intactos. Se mantienen las autorizaciones ya dadas para la revocación/restauración de esa misma cuenta; no se amplían permisos/usuarios.

Propuesta concreta: conservar el plan 22 y revalidarlo por SELECT/huella; ejecutar **seis** solicitudes: repetir injected-result con la expectativa corregida y continuar los otros cinco casos (snapshot inyectado, nivel inexistente, versión inexistente, publicación compatible y permiso revocado/restaurado). Solo la publicación compatible puede crear un plan adicional. No se repite la preparación base. Preservar planes 11–22.

Presupuesto: dos solicitudes operativas consumidas; de las siete originales quedan cinco. Repetir el negativo requiere **un intento operativo adicional**, total acumulado ocho (dos anteriores + seis próximos). El límite de planes nuevos del escenario se mantiene en dos (22 + uno). La aprobación adicional ya quedó registrada; la continuación real está pendiente. Las dos consultas HTTP diagnósticas anónimas devolvieron 401 sin credenciales; se registran por separado del presupuesto de la cuenta operativa.

La continuación relee el plan base y su huella antes de nuevos POST. La suite ahora conserva código HTTP/código de dominio/forma seguros al fallar un rechazo, sin cuerpos ni mensajes privados; registra asimismo ID/huella de cada plan verificado. Reutiliza el flujo de restauración existente, incluida relectura final y diario. Regresión local **88/88 PASS**: reanudación PLAN-03 con seis solicitudes y un solo plan nuevo, restauración bajo fallo y diagnóstico acotado.

[Solicitudes exactas y presupuesto](doc114-plan03-continuation-proposal-20261103.json). Estado: autorizada y preparada, sin ejecutar.

## PLAN-03: continuación adicional autorizada y lista — 2026-11-03

El usuario autorizó específicamente un rechazo negativo adicional: seis solicitudes próximas (cinco pendientes originales y la repetición de injected-result), total ocho intentos de la cuenta operativa contando los dos ya consumidos. Se mantiene el máximo de dos planes del escenario: plan base 22 conservado y hasta uno nuevo. Revocación/restauración sigue limitada al alcance previamente autorizado IMPORTAR_CARPETAS 1→0→1 del usuario 141/empresa 2.

Se preservaron estado y fuente de consola del primer intento. Intento 2 tiene archivo propio; exige permiso concedido y hash de los otros campos igual al primer intento, planes 11–22 correctos y solo referencias esperadas. Recalcula huella del plan 22 con los documentos canónicos persistidos, verifica valores/contexto/manifiesto y lo reanuda por SELECT. El resultado esperado distingue attempts=6, preparations=1, rejections=5 y resumedPlans=1. Se revalida el plan base al cierre junto con el nuevo, los planes 11–22 permanecen intactos y se acredita permiso restaurado por bridge/diario/SELECT.

Regresión 88/88 PASS, precondiciones de continuación 4/4 PASS, sintaxis y --check del recurso con reanudación PASS (seis intentos, un plan conservado; sin ejecución real). [Manifiesto](doc114-plan03-continuation-ready-20261103.json). La expectativa de inyecciones se corrigió a CONFIGURACION_INVALIDA conforme al validador de la DLL actual; los rechazos discordantes conservan observación HTTP/código/forma saneada. Ningún caso nuevo ni revocación real de la continuación ha ocurrido todavía; avance 14/16.
