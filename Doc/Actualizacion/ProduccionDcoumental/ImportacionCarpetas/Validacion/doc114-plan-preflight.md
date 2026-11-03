# DOC-114 — Caracterización local de preparación

Fecha: 2026-11-03. Base de rama feature/DOC-114: 4461453f. Este HEAD es de inicialización y no representa cambios locales sin commit.

## Resultados ejecutados

| Comprobación | Resultado | Alcance |
|---|---|---|
| node --test tests/doc110-perfil-importacion-behavior.test.cjs | 2/2 PASS; 15.139 s | Arnés .NET Framework existente y contraste de columnas DDL; dobles/archivos sintéticos, sin servidor ni base real. |
| node --test tools/e2e/tests/doc111-importacion-carpetas-driver.test.cjs | 46/46 PASS | Incluye tres regresiones DOC-114 y conserva DOC-111/DOC-113. Sin login ni consultas reales. |

El primer intento del arnés falló por ausencia de bin/MySql.Data.dll en el worktree. Se reutilizó la dependencia local de la aplicación; no se modificó el arnés para ocultar el fallo. Packages y node_modules existentes se reutilizan mediante junctions locales ignorados. No se instaló otra plataforma y la aplicación servida permanece intacta.

## Caracterización por requisito

| Requisito | Aserciones existentes ejecutadas por el arnés |
|---|---|
| RQ-01 | OpeningCases, ContextAuthorityCases, AuthenticatedRevocationCases y PreflightCalculationCases: autoridad/permiso revocado y cuerpo cerrado. |
| RQ-02 | ManifestCases y PreflightCalculationCases: rutas, cuotas, correlaciones, homónimos, subcarpetas y conservación del inventario rechazado. |
| RQ-03 | PreflightCalculationCases/DeferredValuesCases: dos formatos, ceros, fechas, nombre largo, tipología y fuente de identidad futura nula/tipada. |
| RQ-04 | PreflightCalculationCases: preparación integrada en EXPEDIENTE/LOTE/PARCIAL y error global. |
| RQ-05 | PlanStorageCases/ConfirmationServiceCases: plan sellado, revisión/huella, persistencia, TTL, nuevas preparaciones y determinismo del cálculo. |

Las aserciones de preflight se encuentran en tests/ImportacionCarpetasBehaviorTests.cs:1885. El wrapper exige explícitamente las salidas de metadata preflight y service plan persistence; por tanto el PASS del arnés incluye esos grupos. No se ha demostrado un defecto funcional nuevo.

## Soporte DOC-114 implementado localmente

El cliente reutiliza safePost y el perfil base cerrado DOC-111. Solo permite apertura/listado/preparación con autorización y presupuesto explícitos; bloquea confirmación/recepción/avance/administración. Conserva referencia antes de validar el resto de la respuesta y detiene operaciones después de un resultado incierto.

Las aserciones comprueban contexto, inventario completo sin duplicados, valores/ceros, diferidos y elegibilidad esperada. La huella de resultado excluye identidad y expiración del sobre; no inventa igualdad de campos de control. Los controles usan SELECT acotados a actor/ámbito/nivel/publicación, verifican resultado/manifiesto/revisión/huella/TTL persistidos y ausencia de cambios en lotes/expedientes/documentos. El conteo de lotes usa solicitante_ref conforme al DDL. Pruebas negativas locales demuestran detección de datos alterados y efectos documentales.

Los fixtures son solo metadatos sintéticos; sus nombres/códigos/reglas requieren contraste y autorización antes de cualquier uso real. No son perfiles publicados ni aprobación de recursos.

## Dependencia para PLAN-03

El puente doc110-permission-db.ps1 y startDatabaseBridge ya existen, pero revocan ADMINISTRAR_PERFILES_IMPORTACION, no IMPORTAR_CARPETAS. No deben reutilizarse sin adaptación como prueba del permiso operativo. Falta definir extensión compatible o procedimiento aprobado que revoque/restaure exactamente el permiso operativo y conserve los demás campos. No se ejecutó ni modificó el puente, no se habilitaron permisos y la fase real permanece pendiente.

## Estado de ejecución real

PLAN-01, PLAN-02 y PLAN-03: NO_EJECUTADO. Especificación automatizada y selector aún por implementar; publicaciones/recursos/cuentas/preparaciones y revocación sin autorización específica DOC-114. Responsable de implementación: Codex; autorización y recursos: responsable del ambiente. El resultado local no acredita aceptación integrada ni permite cierre. Sin cargas, confirmaciones, creación ni almacenamiento documental.

## Compilación y estado del corte

Comando: MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /p:OutputPath=.tmp/doc114-build/bin/ /p:IntermediateOutputPath=.tmp/doc114-build/obj/ /p:BaseIntermediateOutputPath=.tmp/doc114-build/obj/ /p:BuildProjectReferences=false /p:UseSharedCompilation=false /v:minimal /nologo. Herramienta: C:/Program Files/Microsoft Visual Studio/18/Enterprise/MSBuild/Current/Bin/MSBuild.exe. Salida 0; log local .tmp/doc114-msbuild.log. DLL y objetos solo en salida aislada; advertencias legacy existentes. No se desplegó DLL ni se modificó Web.config. Sintaxis del soporte y driver PASS. Fixtures JSON legibles; no se certifica publicación/configuración real.

Corte: 6/16 tareas completas (inventario, caracterización, fixtures, soporte, SELECT y build). Suite PLAN, selector, política final, regresión del corte final, corridas reales y consolidación siguen sin completar. La prueba de revocación operativa necesita adaptar el puente compartido o concretar procedimiento existente; usar el puente administrativo tal cual sería una prueba del permiso equivocado.

## Suite focal local — 2026-11-03

Estado posterior al corte inicial: 12/16 tareas completadas. Existen suite PLAN-01/02/03, selector fino, soporte/controles y ejemplo de recursos; ninguna corrida real se ha ejecutado. Tres escenarios descubiertos y tres omitidos antes de login sin autorización. Driver 53/53 y regresión compartida 62/62 PASS; sintaxis PASS. Arnés Framework 2/2 y build aislado PASS del corte anterior siguen aplicando: sus fuentes backend no cambiaron.

El puente compartido conserva el modo administrativo por defecto y añade -OperationalImport para IMPORTAR_CARPETAS. Exige autorización específica, cuenta de pruebas acotada, runtimeRoot explícito y planId/hash de una preparación recién contrastada para comprobar que el origen administrativo es el operativo correcto antes de revocar. Usa diario separado y compare-and-set; la restauración solo se acredita después de verificar el permiso y los otros campos. Prueba PowerShell por AST con proveedor en memoria cubre ambos modos y rechazo de una restauración con campos ajenos cambiados, sin conexión/SQL reales. La suite restaura y cierra en finally incluso ante error HTTP.

El recurso de sesión es un archivo de datos cerrado: perfiles base DOC-111 sin ampliarlos, casos/metadatos/esperados e identidad de control; no contiene credenciales ni configura otra plataforma. runtimeRoot identifica la aplicación servida y se registra SHA256 de su DLL antes/después; el SHA del candidato no se confunde con despliegue de backend. El selector exige número exacto de intentos y los recursos revisados, sin confirmación válida, recepción o avance. Referencias de planes se escriben inmediatamente en un JSON saneado propio de la corrida dentro de artifacts, además del reportero compartido, sin cuerpos o filas privadas.

Ejemplo reproducible sin login (desde el worktree): npm.cmd --prefix tools/e2e run test:doc114:plan -- --session profiles/doc114-plan-session.profile.example.json --scenario PLAN-01 --check. Los IDs, DSN, URL y reglas del ejemplo son ilustrativos; no acreditan recursos reales. [Manifiesto local del corte](doc114-local-suite-20261103.json). Log de regresión: .tmp/doc114-shared-regression.tap en el worktree. La suite usa workers=1/retries=0 por comando, sin cambiar la configuración global.

PLAN-01/02/03 siguen NO_EJECUTADO. Antes de ejecutarlos faltan publicaciones/reglas e inventarios revisados y autorizados, identidad de cuenta/ambiente/DSN, runtimeRoot y presupuesto de preparaciones. PLAN-03 requiere además autorización específica de revocación y restauración, no derivada de DOC-111/DOC-113. No se cambiaron permisos reales, Web.config, SQL, almacenamiento, SII ni archivos VB. Las tareas 6.1/6.2/6.3/7.1 conservan su casilla abierta; no se declara aceptación integrada.

## Publicación real verificada — 2026-11-03

PASS de la sesión autorizada: formato A perfil 26 / versión 40; formato B perfil 27 / versión 41. Se realizaron seis operaciones, sin reintentos: CrearPerfil, GuardarBorrador y PublicarVersion para cada copia. Ambas quedaron publicadas y disponibles operativamente en nivel 7, gabinete 10, ámbito EXPEDIENTE. Se contrastaron contenido y referencias por lectura administrativa y catálogo operativo; las huellas de 24/38 y 25/39 se conservaron antes y después.

Cero planes, documentos o cambios de permisos. Nuevos recursos conservados. [Evidencia saneada](doc114-publication-result-20261103.json). Este resultado acredita recursos previos; no equivale a aceptación PLAN-01/02/03. Tareas permanecen 12/16. Próximo paso: contrastar la tipología real (ID para A y código para B), preparar y revisar los siete casos PLAN-01 con estas versiones y solicitar autorización específica para esas preparaciones antes de ejecutarlas. No se deriva autorización de planes ni revocación de la publicación ya realizada.

## PLAN-01: detención previa y corrección del catálogo — 2026-11-03

El primer intento se detuvo en READONLY_TYPE con DOC114_PLAN01_TYPE_UNAVAILABLE antes de generar recursos o llamar al runner de preparaciones. No se consumieron intentos de PrepararPlan. El registro original no retuvo número de filas/tipo de columna, por lo que no se atribuye la causa a un código nulo como hecho comprobado.

Se corrigió el preparador de recursos: el formato A conserva la referencia interna 79; el formato B puede usar otra tipología de la misma clasificación publicada si 79 no tiene código utilizable. Primero se leen las configuraciones publicadas 40/41 y sus filtros de instrumento/serie/subserie/empresa; luego las tipologías activas de ese ámbito. Se usan alias explícitos y CAST AS CHAR, se comprueba la referencia 79 y se selecciona un nombre de archivo seguro. La unicidad del código y ausencia de DOC114-UNKNOWN se contrastan por SELECT con la comparación MySQL real. Ninguna consulta modifica catálogos o perfiles.

Prueba local 2/2 PASS: referencia válida sin código, selección por código de otra tipología, expectativas separadas ID/código, ausencia/ambigüedad/código inseguro y negativo ocupado. Se conserva el intento 1; el intento 2 escribe otro estado y recursos. Continúa vigente la autorización de siete preparaciones máximas; no se amplía a permisos, confirmaciones ni documentos. Si no existe código apto, se detiene antes de preparar y registra una categoría segura.

## PLAN-01: recurso por código no disponible — 2026-11-03

Intento 2 detenido en READONLY_TYPE antes del runner: seis tipologías, referencia 79 presente con código NULL y ningún candidato por código utilizable y único. Cero preparaciones y planes nuevos en ambos intentos. No se atribuye código NULL a las otras cinco filas, cuyos valores no se retuvieron. Evidencia saneada: [indisponibilidad](doc114-plan01-type-unavailability-20261103.json).

Se preparó [propuesta de nueva versión del perfil B](doc114-format-b-reference-proposal-20261103.md), cambiando solo TipoValor TEXTO a REFERENCIA. Validación estructural Framework local PASS. Versión 41 quedaría conservada como negativo que distingue ID/código; el caso positivo por código no se declara acreditado. Esta publicación adicional no se ejecutó ni está autorizada. PLAN-01 y tareas 6.1/6.2/6.3/7.1 siguen pendientes.

## Publicación adicional autorizada y preparada — 2026-11-03

Autorización específica del usuario: “autorizado”, en respuesta a la propuesta de una versión adicional del perfil 27. El JSON aprobado queda ligado por SHA256 a la autorización local. Ejecutor en `tools/e2e/tests/support/doc114-reference-test-version.cjs`; reutiliza el cliente administrativo y solo añade CrearVersion a su lista cerrada de operaciones.

Antes de escribir, contrasta las cuatro versiones publicadas, la configuración de 41 frente a la copia revisada y todas sus reglas. Crea una versión, anuncia el ID inmediatamente, guarda únicamente el cambio TipoValor y publica con revisiones devueltas. Lee nuevamente contenido e historial y contrasta el catálogo operativo. Tres mutaciones máximas, una sola vez cada una; sin planes, documentos ni cambios de permisos.

Regresión 73/73 PASS (incluye 5 casos de esta versión): autorización/delta, origen alterado, fallos intermedios sin retry, preservación del historial y ausencia de publicación operativa. Sintaxis de consola PASS. [Manifiesto local](doc114-b-version-ready-20261103.json). La publicación real aún no se ejecutó; no se declara aceptación PLAN-01.

## Versión 42 publicada; PLAN-01 ajustado listo — 2026-11-03

PASS de publicación adicional: perfil 27 / versión 42. Tres mutaciones confirmadas: CrearVersion, GuardarBorrador y PublicarVersion. Historial 41 y publicaciones 38/39/40 conservados por lectura comparada; 42 disponible en catálogo operativo con la única regla cambiada a REFERENCIA. Cero planes, documentos o cambios de permisos. [Resultado saneado](doc114-b-version-result-20261103.json).

PLAN-01 conserva siete intentos autorizados y cero consumidos hasta este punto. Formato A válido: versión 40, 000114-20261103/DOC114-79.pdf. Formato B válido: versión 42, 000114#03-11-2026/79.pdf. Negativo de tipología: versión histórica 41 TEXTO, 79.pdf, exige TIPOLOGIA_NO_UNIVOCA; antes de prepararlo se comprueba por SELECT que el código 79 no es único en la clasificación. Los otros cuatro casos conservan segmento ausente, requerido vacío, fecha inválida y longitud de nombre 121. Se verifican ceros, fecha, nombre, ID de tipología y descriptor diferido índice 14 con valor null.

Recursos locales comprobados con validateSession: PASS. Regresión de selección/generación 3/3 PASS. Consola de intento 3 con archivo de estado propio, sin sobrescribir los intentos previos detenidos antes de preparar. Esta preparación local aún no acredita ejecución PLAN-01; tareas siguen 12/16. El positivo por código de tipología continúa fuera de evidencia por falta de ese dato en este ambiente.

## PLAN-01: intento 3 detenido y control corregido — 2026-11-03

Checkpoint 9100c2b5073d16c07197952072f60a12: fallo genérico DOC114_E2E_FAILED en PLAN, sin referencias. No se retuvo la causa SELECT original y no se afirma ausencia de planes solo por esa lista vacía. [Registro de corrección](doc114-plan01-controls-fix-20261103.json).

Se identificó un defecto del control documental: ID_EMPRESA_DOCUMENTO se consultaba sobre el gabinete físico. Ahora se usa registro_producion_documental con la relación documental ya probada por DOC-111: ID de inventario, documento, expediente y nombre de gabinete; la empresa se filtra en el inventario. Backend y datos no cambiaron. Errores SELECT se convierten a etiquetas cerradas por control sin SQL, filas o secretos. El checkpoint guarda fase/caso/versión antes de controles y preparación para distinguir fallos previos de respuestas inciertas.

Regresión 76/76 PASS (tres pruebas nuevas del control), sintaxis de consola PASS. Antes del intento 4 se exige SELECT de planes existentes para actor/empresa/nivel y versiones 40/41/42. Cualquier plan encontrado detiene sin preparar y conserva sus IDs. Si no hay planes, se prueban todos los snapshots SELECT antes de abrir el runner para los siete casos autorizados. El intento 4 todavía no se ha ejecutado. PLAN-01 continúa sin aceptación y tareas 12/16.

## PLAN-01: plan 11 creado; comparación de serializadores — 2026-11-03

Intento 4: auditoría previa sin planes y todos los controles SELECT pasaron. Se creó el plan 11 para valid-format-a, versión 40. El checkpoint conserva VERIFY_PLAN y DOC114_STORED_RESULT_CHANGED, tras pasar las aserciones de respuesta/política. Consumo confirmado: un intento; quedan seis de los siete autorizados. No se repetirá el primero ni se reemplazará el plan.

Defecto localizado en el comparador E2E: ReglaPerfilImportacion.Proveedor y ValorReglaPerfilImportacion.Procedencia tienen NullValueHandling.Ignore en el modelo; ASMX puede incluir esos campos null mientras Newtonsoft los omite. Se normalizan únicamente esos dos campos nulos en sus rutas tipadas, conservando IDs, proveedores/procedencias no nulos y toda otra diferencia. Regresión 78/78 PASS; huella de documentos canónicos contrastada con PerfilImportacionJson.HuellaPlan del Framework existente: 1/1 PASS. Backend sin cambios.

Se preparó verificación SOLO LECTURA del plan 11: auditoría de referencias del alcance, huella recalculada de documentos canónicos persistidos, valores esperados, contexto/manifiesto/TTL original, snapshot contra publicación 40 por HTTP y controles sin cambios. No realiza ninguna preparación. No se retuvo el cuerpo HTTP original del intento 4: la verificación posterior no se presentará como comparación retroactiva de ese cuerpo. Resultado diagnóstico aún pendiente. [Manifiesto](doc114-plan11-review-ready-20261103.json). PLAN-01 continúa sin aceptación; tareas 12/16.

## Plan 11: huella/valores verificados; orden de reglas en lectura — 2026-11-03

El primer diagnóstico SOLO LECTURA comprobó huella recalculada, valores esperados y contexto/manifiesto/expiración persistidos del plan 11. Se detuvo al comparar el snapshot con la publicación HTTP; no preparó nada.

El repositorio publica la lista por Orden (ORDER BY orden,destino_tipo,destino_ref) y PrepararPersistencia fija el orden ordinal DestinoTipo/DestinoRef en el snapshot del plan. Se corrigió únicamente la comparación con publicaciones: orden canónico de reglas por destino, rechazando destinos duplicados y conservando el campo Orden, IDs, huella y contenido. La comparación respuesta-persistencia de una preparación mantiene su comprobación de orden original. Regresión 79/79 PASS: equivalencia de la representación y rechazo de cambios de prioridad/ID/huella o duplicados. [Evidencia local](doc114-plan11-rule-order-20261103.json).

Segundo diagnóstico de lectura preparado con archivo propio y categorías booleanas de comparación. No es aceptación todavía: queda comprobar el snapshot publicado y el control final sin cambios. Plan 11 intacto; seis preparaciones autorizadas pendientes, ninguna repetida.

## Plan 11 verificado; continuación de seis casos preparada — 2026-11-03

PASS del diagnóstico SOLO LECTURA: huella recalculada, valores esperados, contexto/manifiesto/TTL persistido, snapshot vivo 40 y controles sin cambios. La comparación confirmó exactArrayOrderEqual=false y canonicalRuleOrderEqual=true: la diferencia era el orden de representación de las reglas. Plan 11 conservado. [Resultado saneado](doc114-plan11-readonly-result-20261103.json). El cuerpo HTTP original no se retuvo y no se declara recuperado.

Continuación preparada con el runner/suite existentes: referencia cerrada de primer caso, ID 11 y huella aprobada. El soporte relee ese plan por SELECT, verifica expectativa/política/persistencia y lo incluye en la relectura final; no envía PrepararPlan para ese caso. Solo los seis casos restantes generan nuevas preparaciones. Resultado y selector distinguen resumedPlans=1 de attempts/preparations=6. Rechazan una huella alterada antes de cualquier nuevo POST. Archivo de estado/recursos de intento 5, conservando intentos anteriores y el diagnóstico. La auditoría previa exige que no exista ningún otro plan en las versiones 40/41/42 para la identidad autorizada; backend debe conservar su SHA original.

Regresión 82/82 PASS: caso con un plan previo y seis POST, preservación íntegra del primero, bloqueo por huella/alcance incorrecto y presupuesto/env del selector. Sintaxis PASS. [Manifiesto de continuación](doc114-plan01-resume-ready-20261103.json). Esta continuación permanece NO_EJECUTADA hasta la consola; se conserva autorización original de siete, una consumida y seis pendientes. No se modifica autorización de permisos/publicaciones ni se amplía el presupuesto. PLAN-01 todavía no aceptado, tareas 12/16.

## PLAN-01 EJECUTADO_OK — 2026-11-03

PASS real confirmado en checkpoint bca19b343b61147850e9dcf79a4e450a y estado de consola 5. Planes 11–17 conservados. Se revalidó el plan 11 exclusivamente por lectura y se crearon seis planes nuevos (12–17), total siete preparaciones dentro del presupuesto original. Cero confirmaciones, lotes o efectos documentales; controles RA/DA SELECT sin cambios documentales. Backend SHA256 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5, conservado. Scope de código real 0b3bf1b95684894cfd114f712a4089840f88ea55e84d34a59e9dec9b52351600; HEAD del worktree 4461453f100e96e47c122d7a0f7222034b8e93da identifica el commit base, sin confundirlo con el contenido candidato o DLL desplegada.

| Caso | Versión | Plan | Resultado esperado verificado |
| --- | --- | --- | --- |
| Formato A válido | 40 | 11 | Código con ceros, fecha, tipología 79 y diferido null; plan conservado/releído |
| Formato B válido | 42 | 12 | Segundo formato y fecha configurados; tipología por referencia |
| Tipología no unívoca | 41 | 13 | TEXTO 79 no se trata como ID; incidencia TIPOLOGIA_NO_UNIVOCA |
| Nombre no interpretable | 40 | 14 | SEGMENTO_AUSENTE |
| Requerido vacío | 40 | 15 | VALOR_REQUERIDO |
| Conversión inválida | 40 | 16 | FECHA_INVALIDA |
| Longitud funcional | 40 | 17 | LONGITUD_NOMBRE_EXCEDIDA |

Los cinco negativos son incidencias de negocio en planes persistidos; no rechazos HTTP del contrato (rejections=0). Todos conservaron inventario/correlación y se releyeron al cierre. [Resultado saneado por caso](doc114-plan01-result-20261103.json). Limitación explícita: no se retuvo el cuerpo HTTP original de 11; su evidencia combina aserciones originales y verificación posterior de huella/valores/snapshot/persistencia. No se afirma comparación retroactiva de ese cuerpo ni un positivo por código inexistente en este catálogo.

Tarea 6.1 completada, avance 13/16. PLAN-02, PLAN-03 y consolidación permanecen pendientes; no se declara aceptación integrada DOC-114. PLAN-02 ya tiene [propuesta de siete solicitudes](doc114-plan02-proposal-20261103.md), contrato local y --check PASS sin login. Requiere autorización específica antes de preparar: hasta cuatro planes nuevos y tres rechazos globales, sin cambios de permisos o recursos documentales.

## PLAN-02 autorizado y listo — 2026-11-03

El usuario respondió “autorizado” a la propuesta concreta PLAN-02: siete intentos, hasta cuatro planes nuevos, tres rechazos globales; preservar 11–17 y cero confirmaciones/documentos/cambios de permisos. Autorización local ligada al SHA256 del recurso revisado f97b3d7c78558d0c6a0da44c445157fda80fa011e03f98265b7e8f6b7952d469.

Consola preparada reutilizando selector/sesión/SELECT existentes. Antes de cualquier preparación verifica perfiles 26/40 EXPEDIENTE, 24/38 LOTE, 25/39 PARCIAL; contenido de reglas revisadas; misma clasificación/gabinete; extensiones pdf/tif; referencia de tipología 79; ausencia del nivel negativo 4294967295. Ejercita los controles de cada perfil. Conserva por lectura la identidad/versión/estado/revisión/huella de 11–17 y los compara nuevamente al cierre. El SHA del backend debe coincidir con PLAN-01. No crea catálogos, perfiles ni publicaciones y no modifica permisos. El selector devuelve ahora su resultado saneado al invocador para acreditar también la comprobación final de preservación.

Contrato local y --check PASS, sintaxis PASS, regresión compartida 82/82 y precondiciones 3/3 PASS. [Manifiesto local](doc114-plan02-ready-20261103.json). Aún NO_EJECUTADO; ninguna tarea real adicional se marca completa. Avance 13/16. La consola conserva referencias inmediatas y estado propio; sin reintentos automáticos.

## PLAN-02 EJECUTADO_OK — 2026-11-03

PASS real: siete intentos, cuatro preparaciones (planes 18–21) y tres rechazos globales sin plan nuevo. Cero confirmaciones/efectos documentales. Controles RA/DA exclusivamente SELECT y planes 11–17 comprobados intactos por identidad/estado/revisión/huella antes y después. Backend SHA256 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5 sin cambios; scope de código ejecutado 2b0c75629be41dc6e5a90a96e5019453641ae102894a25fca75c347f6179b728. [Resultado saneado](doc114-plan02-result-20261103.json).

| Caso | Versión | Evidencia |
| --- | --- | --- |
| Completo EXPEDIENTE | 40 | Plan 18, r1 elegible, r2 TIF rechazado conservado |
| Completo LOTE | 38 | Plan 19, ninguna raíz elegible |
| PARCIAL | 39 | Plan 20, solo r1 elegible |
| Ruta duplicada global | 40 | SOLICITUD_INVALIDA, sin plan nuevo |
| Raíz parcial mixta | 39 | Plan 21, r1/r2 elegibles; PDF válido y TIF rechazado conservados en r2 |
| Destino inexistente | 40 | ACCESO_DENEGADO, sin plan nuevo |
| Referencia de raíz inexistente | 40 | SOLICITUD_INVALIDA, sin plan nuevo |

Tarea 6.2 completada: avance 14/16. PLAN-03 y consolidación siguen abiertas. PLAN-03 está [propuesto con siete solicitudes exactas](doc114-plan03-proposal-20261103.md), hasta dos planes nuevos y cinco rechazos. Requiere autorización específica para la revocación temporal IMPORTAR_CARPETAS 1→0→1 de LUZ.AGUILERA (141/2), mediante puente existente. No se ejecutó ningún cambio de permiso. Contrato/--check de propuesta PASS; regresión del consumidor compartido de revocación 4/4 PASS y prueba PowerShell del puente por AST con proveedor en memoria PASS, sin BD real. La restauración y otros campos intactos son condiciones de aceptación, no se presuponen.

## PLAN-03 autorizado y listo — 2026-11-03

El usuario respondió “autorizado” a la propuesta específica de siete intentos, hasta dos planes nuevos y revocación/restauración temporal IMPORTAR_CARPETAS 1→0→1 para LUZ.AGUILERA, usuario 141/empresa 2. La autorización se vincula al recurso revisado SHA256 0cb40d8e745bafc5aef6bb440316272d5ff082c942e66e99584c18d484fb1db3. No se amplía a otras cuentas, permisos, publicaciones o efectos documentales.

Consola preparada con revisión previa del diario de recuperación, permiso activo/fila única, cuenta/empresa, once planes preservados, publicaciones/reglas 40/42 y referencias negativas ausentes. Backend debe conservar el SHA de PLAN-02. El soporte compartido guarda fases PERMISSION_READY/REVOKING/REVOKED/RESTORING/RESTORED en el checkpoint, incluso cuando falla la solicitud bajo revocación. El puente y las funciones de restauración son los existentes. Al finalizar la consola exige SELECT del permiso concedido, hash de otros campos intacto, diario restaurado del objetivo correcto y planes 11–21 sin cambio. Ante parada intenta registrar una auditoría SELECT segura del permiso; no modifica campos ajenos ni repite preparaciones.

Regresión 86/86 PASS (incluye consumidor DOC-110), precondiciones 3/3 PASS, prueba PowerShell del puente con proveedor en memoria PASS sin BD; contrato/--check y sintaxis PASS. [Manifiesto local](doc114-plan03-ready-20261103.json). Ninguna revocación/preparación real de PLAN-03 se ha ejecutado; las tareas 6.3 y 7.1 conservan su estado abierto, avance 14/16.

## PLAN-03: rechazo por código distinto — 2026-11-03

Plan 22 base creado/verificado; parada en injected-result por EXPECTED_REJECTION_NOT_VERIFIED. HTTP 400 observado en IIS, cuerpo original no retenido. Prueba local del validador de la DLL actual sin HTTP/BD confirma CONFIGURACION_INVALIDA para ambos campos inyectados; fixture exigía SOLICITUD_INVALIDA. Expectativa exacta corregida. Permiso sigue concedido y otros campos intactos; revocación no alcanzada. [Diagnóstico](doc114-plan03-code-correction-20261103.json).

Preparada [continuación de seis solicitudes](doc114-plan03-continuation-proposal-20261103.md) conservando 22 por lectura y hasta un plan nuevo; requiere autorizar un rechazo operativo adicional para total acumulado ocho solicitudes (las otras cinco pendientes ya estaban autorizadas). Regresión 88/88 PASS, diagnóstico seguro y referencias/huellas verificadas retenidas en checkpoint para próximos casos. No se ejecutó continuación, no se marca 6.3, avance 14/16.

## PLAN-03: continuación adicional autorizada y lista — 2026-11-03

El usuario autorizó específicamente un rechazo negativo adicional: seis solicitudes próximas (cinco pendientes originales y la repetición de injected-result), total ocho intentos de la cuenta operativa contando los dos ya consumidos. Se mantiene el máximo de dos planes del escenario: plan base 22 conservado y hasta uno nuevo. Revocación/restauración sigue limitada al alcance previamente autorizado IMPORTAR_CARPETAS 1→0→1 del usuario 141/empresa 2.

Se preservaron estado y fuente de consola del primer intento. Intento 2 tiene archivo propio; exige permiso concedido y hash de los otros campos igual al primer intento, planes 11–22 correctos y solo referencias esperadas. Recalcula huella del plan 22 con los documentos canónicos persistidos, verifica valores/contexto/manifiesto y lo reanuda por SELECT. El resultado esperado distingue attempts=6, preparations=1, rejections=5 y resumedPlans=1. Se revalida el plan base al cierre junto con el nuevo, los planes 11–22 permanecen intactos y se acredita permiso restaurado por bridge/diario/SELECT.

Regresión 88/88 PASS, precondiciones de continuación 4/4 PASS, sintaxis y --check del recurso con reanudación PASS (seis intentos, un plan conservado; sin ejecución real). [Manifiesto](doc114-plan03-continuation-ready-20261103.json). La expectativa de inyecciones se corrigió a CONFIGURACION_INVALIDA conforme al validador de la DLL actual; los rechazos discordantes conservan observación HTTP/código/forma saneada. Ningún caso nuevo ni revocación real de la continuación ha ocurrido todavía; avance 14/16.

## PLAN-03 EJECUTADO_OK; permiso restaurado — 2026-11-03

PASS real confirmado en checkpoint 5224c4e57a326acb96993c25505d77c5, consola 2 y diario operativo. Plan 22 conservado por lectura/huella y plan 23 creado con publicación compatible 42. Continuación: seis solicitudes, una preparación, cinco rechazos, un plan revalidado; total operativo del escenario ocho, con repetición negativa adicional expresamente autorizada. Dos planes nuevos totales, ningún plan base repetido.

Inyecciones de resultado/snapshot rechazadas con CONFIGURACION_INVALIDA; nivel/versión inexistentes y permiso revocado con ACCESO_DENEGADO. Rechazos sin nueva preparación. IMPORTAR_CARPETAS del usuario 141/empresa 2 pasó temporalmente 1→0→1 y quedó RESTORED/restored=true en el diario. La consola acreditó por SELECT permiso concedido, hash de otros campos intacto y planes 11–22 conservados; la suite comprobó reapertura con la misma sesión y relectura final de 22/23. Cero confirmaciones/efectos documentales. [Resultado y recuperación saneados](doc114-plan03-result-20261103.json).

Backend SHA256 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5 sin cambios; scope de código 7f5f17515b39859c1038492ae19c9f7c3f619af8f55ef6fb6ef113728d3876ad. Los fallos previos se mantienen en el historial y no se renombraron como éxitos. La comprobación previa de rechazos se corrigió conforme al validador real, sin cambiar producto ni ampliar a cualquier código.

Tarea 6.3 completada, avance 15/16; resta consolidación técnica 7.1. PLAN-01/02/03 tienen aceptación focal real; aprobación editorial/publicación siguen separadas.

## Consolidación técnica verificada — 2026-11-03

16/16 tareas completadas; seis requisitos/diecisiete escenarios con procedencia explícita, PLAN-01/02/03 reales PASS y permiso restaurado. OpenSpec estricto y trazabilidad técnica PASS; aprobación editorial refinement:approved todavía pendiente, sin inferirla de runtime. El scope entregado coincide con PLAN-03 real y el backend conserva SHA. [Informe actual](doc114-final-report-20261103.md), [verificación](doc114-verification-20261103.md) y [manifiesto](doc114-final-manifest-20261103.json). Los cortes anteriores son históricos. No se publicó ni cerró Jira.

## Aprobación editorial posterior — 2026-11-03

El usuario aprobó la continuación de publicación del cierre técnico verificado: confirmación «continua» y reporte revisado SHA256 63571253664acd4d2026827bdef91b204fcd3d3efd65a6e1a833fe6b336785f8. La aprobación pendiente descrita antes es histórica; publicación/merge/cierre remoto aún no se atribuyen como hechos. Especificación principal sincronizada con seis requisitos y diecisiete escenarios.
