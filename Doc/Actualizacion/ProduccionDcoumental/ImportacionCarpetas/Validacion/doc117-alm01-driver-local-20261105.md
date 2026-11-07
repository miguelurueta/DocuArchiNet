# DOC-117 — Driver ALM-01 preparado localmente

Resultado: **65/65 PASS** en la copia principal y **85/85 PASS** en la copia aislada. No se suman: la copia aislada conserva veinte regresiones preexistentes de main que no se sobrescribieron con la copia local. La fuente local sigue siendo autoritativa para inspección.

## Reutilización y alcance

createDriver y createSelectControls de DOC-111 se amplían con nombre original de recepción y opciones de una raíz/nombre/fecha para verificación; su comportamiento predeterminado permanece cubierto por la regresión anterior. DOC-117 reutiliza el transporte sin reintentos, paginación, comparación canónica de publicaciones y motor existentes. No modifica VB productivo ni función compartida de almacenamiento.

runAlm01 ejecuta una raíz por publicación, respeta el presupuesto de dos planes/lotes/expedientes/documentos y usa exactamente los PDF del manifiesto. Antes del escritor comprueba valores efectivos materializados. Verifica por SELECT documento/inventario/incorporación, fecha, valores, original físico e histórico. Repite solo el almacenamiento ya confirmado y comprueba reapertura sin cambios. Compara huellas de los lotes previos observados durante ventana exclusiva. Los resultados inciertos, referencias existentes, fallo del diario, publicación alterada o discrepancia física detienen la sesión sin empezar el otro formato.

El diario registra referencias canónicas antes de validar el resto de la respuesta; nunca cabeceras, cookies, credenciales o rutas privadas. Una autorización ligada a manifiesto/código solo se consume una vez mediante marcador durable. Las operaciones de lectura y efectos requieren autorizaciones distintas; la autorización de publicaciones anterior no basta.

## Preflight SELECT preparado

El modo --preflight solicita solo cuenta/contraseña MySQL de lectura, usa MembershipUsers por RA/DA y comprueba ausencia de expedientes, planes y raíces previas con esos nombres/códigos; publicaciones 26/40 y 27/42 activas/publicadas; tipología 79 en la clasificación y gabinete 10/índice 14. Todas las consultas son SELECT parametrizadas. No inicia login ni prepara, confirma, recibe, crea o almacena. Aún NO_EJECUTADO.

La futura ejecución --run repite el preflight antes de efectos y reconsulta publicaciones ligadas a las huellas ya revisadas. Requiere autorización específica de dos expedientes/documentos nuevos; no está autorizada ni iniciada. La tarea 15.1 continúa abierta hasta su ejecución real y aceptación.

## Verificación

Comando en ambas copias: node --test tests/doc117-almacenamiento.test.cjs tests/doc117-alm01-driver.test.cjs tools/e2e/tests/doc111-importacion-carpetas-driver.test.cjs. Incluye motor .NET Framework offline, controles SELECT sintéticos, original temporal alterado, fallos de respuesta/checkpoint de cliente, replay, preservación, bloqueo por autorización y nombres de recepción. Ninguno acredita integración real o concurrencia externa.

Huellas de soporte en doc117-alm01-driver-local-20261105.json. La lectura real anterior de publicaciones conserva su código/resultado histórico: estos cambios no se presentan como una nueva corrida autenticada.
