# DOC127: ventana de autoridad y lectura preparada

Estado actual: **EJECUTADA, PASS; AUTORIZACIÓN CONSUMIDA**. [Resultado real](doc127-autoridad-real-20261126.md):1/1 E2E, seis rechazos y controles finales PASS;29/33 tareas. La descripción siguiente conserva el corte histórico de preparación.

Estado al preparar: **PREPARADA, PENDIENTE DE AUTORIZACIÓN**. No se ejecutó una nueva E2E ni se autenticaron cuentas. El avance sigue en **28/33 tareas**; la matriz de capacidad permanece aprobada en sus 18/18 casos. Esta ventana cubre parte de la aceptación 5.1, sin completar automáticamente esa tarea ni el cierre integral.

Ventana `b0bd002d3ee84dc38d2bc883fb9c1739`, preparada el 2026-11-26. El JSON adjunto conserva el SHA de preparación y las huellas de los recursos y scripts locales. Las autorizaciones de lectura y capacidad anteriores ya fueron consumidas.

## Ambiente, cuentas y recursos

- IIS local: `https://localhost/GestionDocumental-Docuarchi.net/`, módulo GESTOR, empresa 2, DA 11, RA 12.
- Usuario 194: activo, permiso de cargas habilitado y los otros cuatro permisos de importación deshabilitados. Usuario 196: activo, sin permiso de cargas. No se modifican permisos.
- Selección válida: conexión 6, plantilla 60, revisión 1, destino `imp_02_doc126_e2e_asistente_c38a2522`.
- Referencia ajena: conexión 1 y plantilla 1, existentes en el catálogo. Un SELECT confirmó que su destino no coincide con el de la conexión 6. La prueba comprobará además que no aparece entre las referencias autorizadas de la sesión antes de usarla.
- Resultado ajeno: una carga CONFIRMADA del fixture histórico, propiedad del usuario 141, empresa 2 y referencia `DA:11:6`. No se autentica al usuario 141 ni se muestra su resultado. Su ID exacto y el UUID ausente están fijados en el scope privado y el JSON adjunto.
- Conservación: las 38 tablas actuales `imp_02_`, todas las plantillas y campos, los 78 perfiles y las 41 cargas existentes. Las filas se comparan mediante huellas; no se publican sus contenidos.

## Casos y resultado esperado

| Caso | Operación de consulta | Resultado exigido |
| --- | --- | --- |
| CSRF ausente | ConsultarPlantilla válida, sin token | SOLICITUD_NO_VERIFICADA |
| CSRF inválido | ConsultarPlantilla válida, token inválido | SOLICITUD_NO_VERIFICADA |
| Conexión ajena | ListarPlantillas de conexión 1 | ACCESO_DENEGADO |
| Plantilla ajena | ConsultarPlantilla 1 desde conexión 6 | NO_ENCONTRADO |
| Carga ajena | ConsultarResultado del fixture de usuario 141 | RESULTADO_INCIERTO |
| Carga inexistente | ConsultarResultado de UUID comprobado ausente | RESULTADO_INCIERTO |

Cada rechazo debe devolver `ok=false`, el código exacto y datos nulos. Después se exige que la consulta válida de la plantilla 60 siga funcionando. También se verifica menú y servicio denegados para 196, acceso de 194, búsqueda/detalle, cierre con Escape, aislamiento de `form1` y coincidencia de los tres assets HTTP con sus hashes locales.

## Efectos y ejecución preparada

**Cero cargas recibidas, preparadas o confirmadas; cero filas insertadas o borradas; cero tablas, plantillas o campos creados; cero cambios de permisos.** La aplicación sólo recibe operaciones de consulta y los accesos normales de las dos cuentas. Los controles SQL son exclusivamente SELECT. No se instala, despliega ni limpia nada.

Se reutilizan el runner DOC110 y la suite existente `importacion-excel.spec.cjs`, modo `--excel-ui-read`, con `DOC127_AUTHORITY_E2E_AUTHORIZED=YES` únicamente después de la autorización expresa. Una ejecución, sin reintentos; timeout del test 15 minutos y vigencia máxima de ventana 30 minutos desde la autorización. Si vence o falla, se conserva la evidencia y no se repite automáticamente.

Recursos locales excluidos de Git:

- `.tmp/doc127-authority-ventana/`: manifiesto, scope, huellas preparadas, controles locales y futuros marcadores/resultados.
- `.tmp/doc127-authority-session.ps1`: consola privada para las dos contraseñas efímeras; compara la línea base antes de solicitar credenciales y cierra al terminar.
- `.tmp/doc127-authority-guard.cjs` y `.tmp/doc127-authority-run.cjs`: verifican autorización, vigencia, hashes y consumo único antes del runner.
- `.tmp/doc127-authority-snapshots.ps1`: SELECT antes/después de perfiles, resultados, configuración y catálogo de tablas. La suite compara además el contenido de cada tabla protegida.

La consola se abrirá tras aprobar este alcance. Las contraseñas se introducen allí, enmascaradas; no se reciben por chat ni se guardan. Después de capturarlas, la salida se conserva saneada en archivo; no se mantiene una consola con `-NoExit`.

## Comprobaciones ya realizadas

El último preflight registró 8 SELECT de identidad, permisos, referencias, carga ajena, UUID ausente, tablas y plantilla. Otros 6 SELECT obtuvieron las huellas preparadas: 78 perfiles, 41 cargas y 38 tablas. No hubo SQL de escritura ni sesiones autenticadas.

Scope y runtime válidos. Cuatro comprobaciones de bloqueo pasaron: sesión sin interruptor, guardia pendiente, sesión pendiente aun con interruptor y runner sin bandera de autoridad. Ninguna creó marcadores de consumo o inició login. Sintaxis correcta en tres scripts Node y dos scripts PowerShell. Estas son comprobaciones locales; no son resultados de los seis casos reales.

## Pendientes que se conservan

5.1 seguirá abierta hasta revisar el resultado real junto con la cobertura de preparación/autoridad ya existente; no se afirma una revocación física de permisos. 5.2 conserva la aceptación del asistente por formato y la distinción entre historia, pruebas sintéticas y ejecución vigente. 2.4 necesita el EXPLAIN manual de SQL29: no se encontró un plan aportado ni se ejecutó desde el agente. 7.1 y 7.5 conservan el cierre de hallazgos, revisión técnica, QA manual y gates finales.

La preparación no concede autorización. AGENTS.md exige: «No ejecutar E2E real ni carga sin autorización explícita para el ambiente y las cuentas de prueba».
