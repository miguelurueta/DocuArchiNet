# DOC127: autoridad y lectura reales aprobadas

**1/1 E2E real PASS; seis rechazos de seguridad PASS.** Se completa 5.1 con evidencia clasificada de autoridad, lectura y preparación; **29/33 tareas**. Capacidad conserva sus 18/18 casos aprobados. No se archiva ni cierra el cambio.

Ventana `b0bd002d3ee84dc38d2bc883fb9c1739`, autorizada expresamente por el usuario mediante «autorizado». SHA de ejecución `95d7c8d377a960e551b62757c21c50d441e205d5`. Prueba finalizada 2026-11-26T17:31:13.4044259Z; control posterior 2026-11-26T17:31:16.2308387Z. Una ejecución, sin reintentos; consola cerrada naturalmente.

## Resultado real

IIS local HTTPS/GESTOR, empresa 2, DA 11/RA 12. Cuenta 194 con sólo permiso de cargas y cuenta 196 sin él, verificadas antes del login. Selección válida: conexión 6, plantilla 60 y destino imp_02_doc126_e2e_asistente_c38a2522. Conexión 1/plantilla 1 existentes y ajenas; resultado histórico del fixture del usuario 141 y un UUID ausente verificados por SELECT. No se autenticó al usuario 141.

| Caso | Operación | Código real |
| --- | --- | --- |
| CSRF_AUSENTE | ConsultarPlantilla | SOLICITUD_NO_VERIFICADA |
| CSRF_INVALIDO | ConsultarPlantilla | SOLICITUD_NO_VERIFICADA |
| CONEXION_AJENA | ListarPlantillas | ACCESO_DENEGADO |
| PLANTILLA_AJENA | ConsultarPlantilla | NO_ENCONTRADO |
| CARGA_AJENA | ConsultarResultado | RESULTADO_INCIERTO |
| CARGA_INEXISTENTE | ConsultarResultado | RESULTADO_INCIERTO |

Los seis rechazos devolvieron ok=false y datos nulos. La consulta válida posterior funcionó. Menú/servicio denegados para 196; búsqueda, detalle y Escape correctos para 194. Aislamiento de form1 antes/después y los tres assets HTTP coincidentes. No se envió un postback del gestor.

## Conservación

La suite comparó las huellas completas de las 38 tablas protegidas y las cabeceras/campos de plantillas: sin cambios. El marcador de lectura apareció antes del último bucle de comparación; se exigieron además38/38 verificaciones, el PASS final de Playwright y el control posterior, evitando atribuir éxito sólo a un marcador intermedio.

SELECT antes/después:78 perfiles,41 cargas existentes,38 cabeceras,134 campos y catálogo de38 tablas idénticos a la preparación y entre sí. Se guardan conteos y hashes, sin filas de negocio ni resultados ajenos. Los12 SELECT de línea base/postflight se contabilizan aparte de los controles internos de la suite, cuyo total no se instrumentó.

Efectos: dos sesiones autenticadas; cero archivos recibidos, preparaciones, confirmaciones, guardados de plantilla, INSERT/DELETE de negocio, cambios de permisos o despliegues. Todos los controles SQL fueron SELECT. Los18 hashes de runtime/scripts/recursos permanecen iguales a los autorizados.

## Cierre de 5.1 y clasificación de cobertura

| Aspecto | Evidencia | Nivel y límite |
| --- | --- | --- |
| Permisos exclusivo/restringido, CSRF, conexión/plantilla ajenas y propiedad/ID de carga | Esta ventana, seis rechazos y lectura UI | Real vigente; sin cambios de permisos |
| Cambio de permiso, usuario, empresa, módulo, referencia e identidad física antes de escribir y antes de COMMIT |36 casos de doc127-pruebas-preparadas-20261125.json | Servicio/writer reales con ADO sintético; ocho hashes de fuentes conservados |
| Preparación conserva destino completo y valida autoridad/configuración persistidas |18 casos actuales de la matriz de capacidad | Real reutilizada;17 hashes de runtime/soporte/scripts conservados; no se repitieron cargas |
| Revocación física de permisos | No ejecutada | Sin mecanismo administrativo autorizado para esta ventana; cobertura de revocación local identificada |

La preparación en los18 casos reutilizados comparó el destino antes/después de Preparar y antes de Confirmar; al terminar se verificaron configuración, perfiles y tablas protegidas. Esta ventana de lectura no hizo preparaciones nuevas. Esta clasificación cumple el criterio de5.1 y D-03/RQ-03, que exige distinguir los niveles de evidencia y reservar la revocación física a gestión autorizada.

## Pendientes y trazabilidad

Quedan2.4 (EXPLAIN manual no aportado),5.2 (plantillas/asistente por formato),7.1 (hallazgos del corte integral) y7.5 (revisión, QA manual y gates finales). La autorización de esta ventana quedó consumida y no habilita importaciones ni reintentos.

El JSON adjunto conserva autorización, SHA, hashes, códigos, controles y clasificación. Scope, marcadores, línea base y log saneado permanecen en .tmp/doc127-authority-ventana/, excluidos de Git. Credenciales efímeras en consola/memoria, contextos cerrados y variables limpiadas mediante finally; sin cookies/tokens ni contenidos de respuestas en la evidencia.
