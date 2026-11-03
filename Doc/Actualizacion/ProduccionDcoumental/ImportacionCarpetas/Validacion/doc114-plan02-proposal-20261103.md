# DOC-114: PLAN-02 propuesto

Siete intentos de PrepararPlan en el mismo ambiente/cuenta/nivel 7/gabinete 10. Se conservarían los planes nuevos y los planes 11–17. No confirma lotes, crea expedientes, recibe archivos, almacena documentos, modifica perfiles/publicaciones ni cambia permisos. La autorización de PLAN-01 no se extiende a esta matriz.

| Caso | Versión | Resultado esperado |
| --- | --- | --- |
| Completo por expediente | 40 | r1 PDF válido elegible; r2 TIF rechazado, conservado en el inventario |
| Completo por lote | 38 | Ninguna raíz elegible por el TIF rechazado en r2 |
| Parcial | 39 | r1 elegible; r2 con solo TIF rechazado no elegible |
| Error global | 40 | Ruta normalizada duplicada: SOLICITUD_INVALIDA, sin plan nuevo |
| Raíz parcial mixta | 39 | r1 y r2 elegibles; r2 contiene PDF válido y TIF rechazado, ambos conservados |
| Destino no autorizado | 40 | Nivel inexistente 4294967295: ACCESO_DENEGADO, sin plan nuevo |
| Manifiesto incoherente | 40 | Archivo referencia raíz inexistente: SOLICITUD_INVALIDA, sin plan nuevo |

Inventario normal: carpetas 000114-20261103 y 000115-20261103, archivos DOC114-79.pdf / DOC114-79.tif. Solo metadatos: no se suben binarios. Hasta cuatro planes nuevos y tres rechazos globales. Las precondiciones publicadas, tipología, extensiones y ausencia del nivel negativo se comprobarán por lectura antes de preparar; se detiene si no corresponden. Los controles SQL siguen siendo exclusivamente SELECT mediante MembershipUsers.

[Propuesta y solicitudes exactas](doc114-plan02-proposal-20261103.json). Estado: sin autorización, sin ejecución real.

## PLAN-02 autorizado y listo — 2026-11-03

El usuario respondió “autorizado” a la propuesta concreta PLAN-02: siete intentos, hasta cuatro planes nuevos, tres rechazos globales; preservar 11–17 y cero confirmaciones/documentos/cambios de permisos. Autorización local ligada al SHA256 del recurso revisado f97b3d7c78558d0c6a0da44c445157fda80fa011e03f98265b7e8f6b7952d469.

Consola preparada reutilizando selector/sesión/SELECT existentes. Antes de cualquier preparación verifica perfiles 26/40 EXPEDIENTE, 24/38 LOTE, 25/39 PARCIAL; contenido de reglas revisadas; misma clasificación/gabinete; extensiones pdf/tif; referencia de tipología 79; ausencia del nivel negativo 4294967295. Ejercita los controles de cada perfil. Conserva por lectura la identidad/versión/estado/revisión/huella de 11–17 y los compara nuevamente al cierre. El SHA del backend debe coincidir con PLAN-01. No crea catálogos, perfiles ni publicaciones y no modifica permisos. El selector devuelve ahora su resultado saneado al invocador para acreditar también la comprobación final de preservación.

Contrato local y --check PASS, sintaxis PASS, regresión compartida 82/82 y precondiciones 3/3 PASS. [Manifiesto local](doc114-plan02-ready-20261103.json). Aún NO_EJECUTADO; ninguna tarea real adicional se marca completa. Avance 13/16. La consola conserva referencias inmediatas y estado propio; sin reintentos automáticos.
