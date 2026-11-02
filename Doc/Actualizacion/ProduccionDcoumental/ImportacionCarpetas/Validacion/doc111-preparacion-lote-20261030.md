# DOC-111: preparación del escenario LOTE

Fecha: 2026-10-30. Sin login HTTP, escrituras de negocio ni carga real en este corte.

Se reutilizó el perfil 24/versión publicada 38 habilitado por el usuario. Su configuración efectiva es `COMPLETO_OBLIGATORIO` con ámbito `LOTE`; no corresponde ejecutar el escenario EXPEDIENTE con esa versión. La preparación usa cuenta histórica 141, empresa 2, nivel 7 y gabinete 10.

## Comprobaciones SELECT

- `MySqlPerfilImportacionRepository.VerificarEsquema` y `MySqlImportacionCarpetasRepository.VerificarEsquema`: PASS en el destino histórico comprobado.
- `PerfilImportacionCatalogAdapter.ValidarOperativo`: cero incidencias; `NivelCompatibleOperativo`: verdadero.
- `AutorizarGabineteImportacion`: PASS; comprueba origen DA, correspondencia del usuario, permiso de almacenamiento y banderas de inventario/TRD/unidad.
- Nivel 7: cumple propiedad o permisos explícitos de creación y carga; cadena de ancestros activa, sin ciclos y dentro de la empresa.
- Nombres/códigos propuestos `DOC111-20261030-LOTE-01` y `DOC111-20261030-LOTE-02`: cero coincidencias actuales por código o alias en la empresa. Es una observación, no una reserva.

Se usaron adaptadores existentes contra RA/DA con conexiones solo en memoria. El contexto operativo de la consulta es local y no acredita una sesión HTTP autenticada ni una revocación real. La validación de esquema cubre el contrato de los repositorios; no certifica por sí sola todo el comportamiento legacy.

## Perfil preparado

Archivo local ignorado: `tools/e2e/profiles/doc111-importacion-carpetas-lote.runtime.json`. No contiene secretos.

| Campo | Valor |
| --- | --- |
| Ambiente | `https://localhost/GestionDocumental-Docuarchi.net/` |
| Módulo | GESTOR |
| Nivel / versión | 7 / 38 |
| Escenario | LOTE |
| Dos raíces propuestas | `DOC111-20261030-LOTE-01`, `DOC111-20261030-LOTE-02` |
| PDF reutilizado para cada raíz | `tools/e2e/fixtures/doc110-carga-individual.pdf` |
| Tipología esperada | 79 |
| Índice esperado | 14, TEXTO, `EEE` |

`npm.cmd --prefix tools/e2e run test:doc111:rec -- --check --profile profiles/doc111-importacion-carpetas-lote.runtime.json --scenario LOTE`: **PASS**, `REAL_EXECUTION=false`. El perfil se deriva de las cinco reglas publicadas existentes: código y nombre de carpeta, fecha inicial constante 2026-01-01, tipología 79 e índice 14. No se modificaron configuración ni reglas. Reporte saneado de las consultas: `tools/e2e/artifacts/doc111-selection-readonly.json`.

## Autorización de carga y sesión interactiva

Después del despliegue, el usuario autorizó la carga real propuesta de dos expedientes y dos documentos. A las 21:16:12 UTC se abrió la consola visible del runner existente, con el perfil LOTE preparado, para capturar las confirmaciones y credenciales enmascaradas que exige el runbook. La primera lectura de estado fue `WAITING_FOR_OPERATOR`, `realExecutionStarted=false`: todavía no acredita login, preparación de plan ni carga.

Estado saneado: `tools/e2e/artifacts/doc111-lote-authorized-session.json`. El lanzador local reutiliza `main`, consola, proceso hijo y saneamiento del runner existente; solo registra estado, referencias numéricas y resultado validado. No registra entradas, conexiones o credenciales. No abrir otra sesión ni repetir el selector ante fallo; consultar primero las referencias existentes. La autorización de carga ya está recibida; faltan las entradas privadas del operador en consola y el resultado de la corrida.

## Sesión detenida y control posterior

La sesión iniciada a las 21:16:12 UTC terminó a las 21:17:36 UTC con `STOPPED_REVIEW_REQUIRED`, sin referencias de plan/lote ni resultado REC. `realExecutionStarted=true` indica que se lanzó el proceso de pruebas; no prueba por sí solo login o efectos. El runner no conservó un código de fallo detallado, por lo que no se atribuye el problema a una credencial o fase concreta.

Ante el reporte del usuario sobre la entrada TTY se comprobó que el runner configura cuenta, contraseña y ambas conexiones con `{secret:true}`: los asteriscos también en cuentas son el comportamiento previsto. No se leyeron ni conservaron las entradas. Se solicitó distinguir si el problema era enmascaramiento, escritura/pegado o instrucciones RA/DA.

Control SELECT posterior a las 21:19:37 UTC: cero planes y lotes nuevos desde el inicio para usuario 141/empresa 2/versión 38; cero expedientes con los dos códigos/alias propuestos en esa empresa. Reporte saneado: `tools/e2e/artifacts/doc111-lote-stopped-select.json`. Es un control acotado a la identidad y recursos previstos, no una afirmación global sobre toda actividad del ambiente. No se repitió el selector, no se borraron recursos y la aceptación LOTE no se marca PASS.

## Diagnóstico y segunda sesión

El usuario aclaró que podía introducir datos y solo veía asteriscos. Se mantiene el enmascaramiento de todos los campos privados. Se añadió al runner existente un diagnóstico de fallo con únicamente modo, etapa cerrada (`PRECHECK`, `READONLY_RA`, `READONLY_DA`, `LOGIN`, `REC`) y código DOC111 saneado; se descartan campos adicionales, mensajes libres y salida privada. No altera el criterio PASS ni reintenta operaciones. Suite focal: 19/19 PASS, incluida la prueba de saneamiento y eliminación de secretos ante fallo; sintaxis de la suite Playwright y diff sin errores.

Una segunda consulta a las 21:21:38 UTC volvió a confirmar cero planes/lotes nuevos de la cuenta prevista y cero coincidencias de los expedientes propuestos. Después de esa revisión se reabrió la consola para el mismo alcance autorizado, conservando el registro anterior. Estado separado: `tools/e2e/artifacts/doc111-lote-authorized-session-02.json`. No se recuperó ni reenvió un comando incierto: no había referencias ni recursos nuevos de la preparación anterior para esa identidad y escenario. El resultado de la segunda sesión debe consultarse antes de otra acción.

Estos cambios solo afectan al diagnóstico de las pruebas; no se modificó ni desplegó de nuevo la aplicación. El manifiesto de fuentes anterior es histórico para los scripts de pruebas modificados; cualquier aceptación posterior registra las nuevas huellas del driver por el mecanismo existente.

## Corrección del TTY y controles ODBC

El usuario señaló que pedir URLs MySQL era incorrecto. Se sustituyeron por los DSN registrados RA/DA y el puente ODBC existente; el runtime usa `MembershipUsers`, comprobado contra ambos destinos. El operador introduce únicamente cuentas y contraseñas en TTY, sin URLs de conexión. [Implementación, pruebas y límites](doc111-correccion-controles-odbc-20261030.md). Se conservan las dos sesiones fallidas; la consola corregida registra su estado separado en `tools/e2e/artifacts/doc111-lote-authorized-session-03.json`. Primera lectura: `WAITING_FOR_OPERATOR`, sin inicio de ejecución real. No asumir éxito ni repetirla antes de consultar ese estado.

## Puente corregido tras tercera sesión

La tercera sesión se detuvo antes del login, sin referencias, con READONLY_RA/DOC111_SELECT_FAILED. Se reprodujo y corrigió un defecto de deserialización de parámetros del puente PowerShell; un SELECT técnico de literal y 49 pruebas locales pasaron. Véase [causa y cobertura](doc111-correccion-controles-odbc-20261030.md). Control SELECT a las 21:32:16 UTC: cero planes/lotes nuevos de la identidad/version previstas y cero coincidencias de los expedientes propuestos. Se abrió una cuarta consola para el mismo escenario autorizado; estado separado `tools/e2e/artifacts/doc111-lote-authorized-session-04.json`. No hay aceptación REC por esos resultados técnicos ni se usó la cuenta configurada del sitio para sustituir la cuenta de controles del operador.

## Continuación después de corregir la dependencia de Web.config

El usuario pidió continuar la prueba con la corrección ya desplegada. Control SELECT a las 22:41:17 UTC: cero planes/lotes nuevos de usuario 141/empresa 2/versión 38 y cero coincidencias de códigos/alias de los expedientes propuestos. Perfil LOTE `--check`: PASS. DLL activa SHA-256 `c85547f134de48f08eaccbba6e41ca477f8ff660a5f340af2dffa552dd8fcca9`, coincide con la compilación corregida. No se repitieron comandos de una operación incierta.

Se abrió la consola para el mismo escenario autorizado, conservando todas las evidencias anteriores. Estado separado: `tools/e2e/artifacts/doc111-lote-authorized-session-05.json`. Reutiliza el runner y DSN existentes; espera credenciales privadas del operador y no implica por sí solo ejecución o aceptación. Revisar su estado/referencias antes de cualquier otra corrida.

### Resultado de la sesión 05

La consola terminó a las 22:41:43 UTC con `NO_EJECUTADO`, `realExecutionStarted=false`, sin referencias ni fallos del proceso hijo: no llegó a iniciar Playwright/controles/login/carga. No se conserva una causa suficiente para distinguir entrada cancelada, confirmación o fallo del arranque interactivo; se solicitó al usuario el aviso de consola, sin credenciales. No se abrió otra sesión. El lanzador local incorpora para futuras sesiones únicamente el nombre del campo/confirmación pendiente, sin registrar su valor, para evitar perder esta información.

### Causa aclarada de la sesión 05 y continuación

El usuario compartió el aviso sin secretos: en la confirmación de cuenta introdujo `S`, mientras el runner exige `SI`. La sesión se detuvo conforme al contrato antes de solicitar credenciales o iniciar el hijo. No fue un rechazo HTTP ni un fallo de importación. Tras comprobar `NO_EJECUTADO`, ausencia de referencias y perfil `--check` PASS, se abrió la sesión 06 para el mismo escenario autorizado. Estado separado: `tools/e2e/artifacts/doc111-lote-authorized-session-06.json`. Se conserva la exigencia de escribir `SI`; no se omiten confirmaciones ni se repite un comando incierto.

### Sesión 06: token ausente y consola persistente

La sesión 06 terminó a las 23:00:20 UTC con `DOC111_TOKEN_UNAVAILABLE` en REC, sin referencias ni resultado. El proceso hijo se ejecutó y pasó los controles de lectura y el helper de login, pero no obtuvo el token del control de importación; ese helper por sí solo no prueba que la página final esté autenticada. No se identifica la causa como contraseña incorrecta ni pérdida de permisos sin evidencia adicional.

SELECT a las 23:09:20 UTC: cero planes/lotes nuevos para la identidad/versión previstas y cero coincidencias de los expedientes propuestos. Otra lectura confirmó que la cuenta histórica 141 conserva permiso de importación y que el perfil 24 está disponible; no demuestra que sea la cuenta efectivamente introducida en la sesión HTTP.

Se corrigió el lanzador para mantener PowerShell abierto (`-NoExit`, sin Read-Host final que pueda consumir una entrada pendiente). Se añadió preflight DOC-111 tras la navegación a Producción, reutilizando el helper de login y sus selectores: espera DOMContentLoaded, exige HTTP 200, ausencia de formulario de contraseña, presencia de un único control de importación y token no vacío. Cada fallo cierra la sesión y emite únicamente código fijo, sin cuerpo/token/cookies. No se envían operaciones REC si ese preflight falla. Suite focal y regresión de autenticación: 33/33 PASS locales.

Se reabrió la consola persistente para el escenario autorizado, con estado separado `tools/e2e/artifacts/doc111-lote-authorized-session-07.json`. La implementación de la aplicación no se modificó ni desplegó en este corte; solo el diagnóstico/navegación del driver y la permanencia de la consola. Revisar el estado antes de otra ejecución; no se marca aceptación real.

### Sesión 07: resultado de la espera

Ante el aviso del usuario «está esperando», se consultó el estado: terminó a las 23:12:29 UTC, `STOPPED_REVIEW_REQUIRED`, etapa LOGIN, `DOC111_IMPORT_CONTROL_UNAVAILABLE`. La página de Producción respondió sin mostrar formulario de contraseña, pero faltó el control `doc111-importacion`. Eso no acredita por sí solo una sesión de Gestión válida ni permite atribuirlo a permisos o contraseña. No hay referencias de plan/lote ni PASS. La consola persistente puede permanecer abierta después de finalizar el proceso hijo.

Se reutilizó el patrón de diagnóstico del contenedor ausente de `doc110-perfil-importacion-session.spec.cjs`: cuando el control falta, el driver intenta una sola consulta de lectura al ASMX `AbrirImportacion`, sin inventar token, para clasificar 401/SESION_REQUERIDA, 403/ACCESO_DENEGADO o 503/CAPACIDAD_NO_DISPONIBLE. El guard completo sigue activo; no se prepara ni confirma un plan durante ese diagnóstico. La suite conserva únicamente códigos fijos y cierra el contexto al fallar. Pruebas focales y autenticación: 34/34 PASS con dobles; el diagnóstico nuevo no se ejecutó contra la sesión ya cerrada.

SELECT a las 23:15:39 UTC: cero planes/lotes nuevos de la identidad/version previstas y cero coincidencias de los expedientes propuestos. No se abrió otra consola ni se repitió la importación en esta revisión; la causa real sigue pendiente del diagnóstico autenticado. No se cambió la aplicación, configuración o permisos.

### Diagnóstico aislado tras confirmación del fallo de sesión 07

El usuario compartió la salida completa saneada de la sesión 07: confirma LOGIN/IMPORT_CONTROL_UNAVAILABLE y finalización visible, sin nuevos efectos de importación. Se añadió `--diagnose` al mismo selector/suite para investigar el acceso sin volver a pedir secretos MySQL ni ejecutar la carga. Solo confirma ambiente/cuenta/TLS, captura credenciales de aplicación mediante TTY y termina tras el preflight. Las autorizaciones de efectos/descartables/SELECT se fuerzan a false; no se ejecutan consultas ODBC ni `runRec`. El resultado de diagnóstico tiene marcador distinto y nunca cuenta como PASS REC. Se reutilizan login, consola, clasificación de fallos y consulta ASMX de lectura existentes.

Verificación: 35/35 pruebas focales y regresión de autenticación PASS, incluida separación de secretos/autorizaciones/resultados de diagnóstico frente a ejecución. Se abrió una consola persistente exclusiva de diagnóstico; estado saneado `tools/e2e/artifacts/doc111-diagnostic-session-08.json`. La prueba real de carga sigue detenida. No se modificaron la aplicación, Web.config, permisos ni perfiles en este corte.

### Diagnóstico 08 aprobado y continuación de carga

El usuario compartió `DOC111_DIAGNOSTIC_RESULT` PASS y el artefacto confirma `DIAGNOSTIC_PASS`, `pageReady=true`, finalizado a las 23:23:23 UTC, sin referencias, efectos de importación ni fallos. La página autenticada de Producción expuso un único control y token válido para esa apertura. El fallo anterior no se reprodujo; no se atribuye retrospectivamente a una contraseña, cuenta, permiso o condición de navegación concreta. No es aceptación REC ni validación de almacenamiento.

Antes de continuar el alcance previamente autorizado, SELECT a las 23:25:01 UTC confirmó cero planes/lotes nuevos de la identidad/version previstas y cero coincidencias de los dos expedientes propuestos; perfil `--check` PASS y DLL corregida vigente. Se abrió la consola persistente de carga LOTE, estado separado `tools/e2e/artifacts/doc111-lote-authorized-session-09.json`. Requiere credenciales nuevas por TTY porque las de diagnóstico fueron eliminadas y su contexto cerrado. No se guardan secretos ni se reutiliza una sesión terminada. Consultar referencias/resultado de esta sesión antes de otro intento.

### Sesión 09: plan 3 y corrección de la confirmación

Se creó el plan 3; ConfirmarPlan devolvió 409/CONFLICTO_REVISION. El repositorio ordena las reglas al persistir y el servicio comparaba el array ordenado con el orden original de lectura. Se corrigió la comparación canónica, preservando todos los campos y el rechazo de cambios reales. Arnés Framework y build PASS; lectura real del plan/publicación equivalente confirma la causa, sin reenvío ni escrituras. La DLL corregida se desplegó y el sitio arrancó correctamente. [Causa, pruebas, SHA y alcance](doc111-confirmacion-orden-reglas-20261030.md).

El plan 3 se conserva REVISABLE/revisión 1, sin lote, con su vencimiento original ya cumplido. Se abrió la sesión 10 para una preparación nueva de la misma prueba autorizada, sin resucitar el plan vencido: `tools/e2e/artifacts/doc111-lote-authorized-session-10.json`. Consultar sus referencias y resultado antes de otra acción; la aceptación sigue pendiente.

### Sesión 10: lote 2 con efectos parciales y reserva pendiente

Se confirmaron plan 4/lote 2, ambas recepciones y expedientes 648/649 (relaciones 1073/1074). El intento documental 5 quedó RESERVADO y la respuesta fue RESULTADO_INCIERTO. Se investigó solo por SELECT y validación local: el catálogo PDF usa código firmado -2, y la columna automática NOMBRE no es un índice visible. Ambas incompatibilidades del adaptador se corrigieron, probaron y desplegaron sin modificar legacy, perfil o Web.config. La validación de catálogos reales pasó; las consultas acotadas muestran cero documentos. El estado del lote/reserva se conserva y no se abre otra carga. [Evidencia completa, versiones y recuperación pendiente](doc111-lote2-formato-nombre-20261030.md). Solo se cierran 13.10/13.11 por sus evidencias propias; progreso 121/130, sin PASS REC.

## Actualización posterior

El usuario autorizó el despliegue y la DLL se actualizó con arranque básico PASS. Véase [evidencia vigente](doc111-despliegue-local-20261030.md). La diferencia de binario descrita a continuación es histórica; la autorización de carga real continúa pendiente.

## Correspondencia de despliegue observada antes de la actualización

Inspección de metadatos sin instanciar tipos: `bin/GestionDocumental-Docuarchi.net.dll` carece de `PoliticaLimpiezaOriginalImportacion` y `ComposicionLimpiezaOriginalImportacion`; ambos existen en `.tmp/doc111-limpieza-build/bin/GestionDocumental-Docuarchi.net.dll`. Esto confirma una diferencia funcional entre el binario del sitio y el de la compilación validada, además de las huellas distintas ya registradas.

Antes de aceptar esta implementación debe actualizarse la DLL del sitio con la compilación validada y comprobar el arranque. No se hizo ese despliegue: reemplazar la DLL provocará un reinicio de la aplicación IIS. Los recursos de esta propuesta todavía requieren aprobación para crear dos expedientes y almacenar/incorporar dos documentos. No se ejecutaron confirmación, recepción, fases, limpieza, cambios de esquema ni REC. El progreso permanece 119/130; esta preparación no cierra aceptación real ni revisión final.
