# DOC-117 — ALM-02 interno listo para autorización específica

Revisión local del 2026-11-06. Las ventanas internas reales siguen NO_EJECUTADO. La preparación autorizada anterior terminó con PASS y se conserva; no se volverá a preparar ni recibir archivos. Progreso OpenSpec: 15/18.

## Alcance concreto propuesto

Ambiente `https://localhost/GestionDocumental-Docuarchi.net/`, actor de aplicación 141, empresa 2, nivel 7, versión 38, gabinete 10. Usar una cuenta MySQL exclusivamente SELECT para los DSN RA/DA registrados. Credenciales en consola enmascarada, efímeras y sin guardarlas. La ventana exige exclusividad: sin otras importaciones, cargas, despliegues o reinicios.

| Ventana | Recursos existentes | Efecto y comprobación |
| --- | --- | --- |
| A | Plan 37/lote 21, raíz 33, archivo 34, expediente 664, relación 1089 | Una llamada ALMACENAR. Inyectar excepción después del checkpoint confirmado y antes de retornar al adaptador. Leer checkpoint/IDs, verificar físicamente documento/inventario/incorporación e índices. Repetir exactamente una vez solo después de esa prueba durable; mismos IDs, eventos y una sola entrada al escritor. |
| B | Mismo plan/lote, raíz 34, archivo 35, expediente 665, relación 1090 | Una llamada ALMACENAR. Inyectar excepción después de verificar el resultado legacy y antes del checkpoint. Verificar IDs físicos conocidos y resultado público INCIERTO, con intento persistido EN_CURSO. No repetir ni reconciliar/reparar esta ventana. |

Límites nuevos: tres solicitudes ALMACENAR en total, dos documentos/inventarios/incorporaciones como máximo, hasta 240 controles SELECT; cero planes, lotes, recepciones, expedientes o materializaciones nuevos. Se conserva todo, incluidos los originales y los recursos anteriores (especialmente lote 18/intento 41 y lotes 19/20). No incluye ALM-03, cambios de permisos/configuración/esquema, limpieza, XML o SII.

## Candidata y restauración

Instalación temporal exclusiva de la candidata SHA-256 `85824b255a826664a8395754d8903e51218ba1a8303595ee6e181c8c0dc05a95`, compilada fuera de bin con los recursos exactos del lote 21. Sustituye para esta propuesta a la candidata histórica de preparación; no se reescriben sus evidencias.

Original a respaldar y restaurar: `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`. Dos respaldos separados, comprobación de bytes/SHA y ACL, guardián independiente de máximo 600 segundos y restauración en finally, incluso si falla el cierre de sesión. Reabrir sesión con la DLL original y verificar estado, documentos, originales y preservación. Un fallo impide PASS y cualquier continuación automática. El guardián restaura los bytes de la DLL; no revierte documentos ni se presenta como cancelación de una petición ya iniciada.

La instrumentación existe solo en la composición de laboratorio: autoridad, inicio, entrada al escritor y checkpoint. Cada archivo fija atómicamente el primer intento concordante antes del inicio real; otra marca durable cuenta la entrada al escritor antes de delegar a la función existente. No se cambia ClassAlmacenamiento ni se añade un selector de fallos al producto. Los 912 archivos originales de compilación/recursos conservan sus huellas. La candidata aún no está instalada.

## Verificación local y ejecutor

`tools/e2e/scripts/run-doc117-alm02-internal.cjs --check` verifica propuesta, recursos sellados, candidata, originales y calculador nativo sin login, SELECT, navegador, instalación o almacenamiento. El ejecutor real exige una autorización nueva ligada a las huellas de propuesta/código/recursos/candidata; una autorización de setup o DOC-116 se rechaza antes de cualquier efecto. No se ha creado esa concesión.

El soporte verifica A antes del replay y de B, no reintenta respuestas inciertas y registra identidades conocidas si se detiene. La comprobación física por IDs admite el estado incierto de B sin promoverlo a COMPLETADO; la verificación existente de lote completo mantiene su guard de estado final. Tras restaurar se comprueban otra vez ambas ventanas y la preservación de recursos anteriores.

La regresión local comprende 134 pruebas, incluidas las de .NET Framework, los dos fallos con dobles, replay, contador de escritor, rechazo de autorización ajena y restauración ante fallos. El ensayo de copia de bytes con esta candidata terminó PASS tras finalizar el propietario sin finally: guardián independiente, dos respaldos y ACL verificados. Se ejecutó solo en un sitio temporal; no acredita restauración real de IIS. OpenSpec estricto PASS. Huellas y detalle: `doc117-alm02-interno-local-20261106.json`.

## Decisión pendiente

Autorizar específicamente el ambiente, cuentas de prueba, exclusividad, instalación temporal/restauración e inyección descritos y los límites de almacenamiento/SELECT anteriores. La preparación previa no concedía esos efectos. AGENTS.md requiere autorización explícita para E2E real y D-08 exige ambiente, cuentas, recursos y fallos concretos. La tarea 16.1 solo se completará con evidencia de la ejecución real y restauración; 17.1 y 18.1 siguen pendientes.
