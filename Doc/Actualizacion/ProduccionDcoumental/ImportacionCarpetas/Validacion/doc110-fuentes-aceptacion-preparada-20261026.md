# DOC-110: aceptación autenticada de fuentes preparada

Estado vigente al 2026-10-27: **aceptación autenticada 15.16 PASS**, continuidad con imp_02_creditos aprobada, 1 passed (23.8 s). Fuente, proveedor y perfil descartables confirmados inactivos. Los estados BLOCKED/FAIL, aceptación pendiente y disponibilidad sin confirmar siguientes son historial de preparación/diagnóstico superado por la evidencia acumulada de alta, edición, recuperación y continuidad; detalle final al final del documento.

Estado vigente: **BLOCKED — DOC110_SOURCE_ALREADY_REGISTERED** en corrida autenticada autorizada. La comprobación previa encontró DA/imp_02_egresos existente y se detuvo sin escribir; no se ejecutó el ciclo de alta/edición/revocación. Detalle al final; el NO_EJECUTADO por autorización descrito abajo corresponde a la preparación previa.

Fecha: 2026-10-26. Avance de 15.16, con cobertura de los contratos 15.5–15.9/15.13/15.14. No acredita ejecución real ni cierra esas tareas.

## Recorrido preparado

Se añade `--source-profile` al runner existente `run-doc110-profile-session.cjs` y una rama en la suite autenticada existente. Reutiliza perfil runtime, consola/secretos efímeros, autenticación compartida, sesión/CSRF, configuración y reporteros. No añade login, proyecto Playwright ni dependencias. El runner exige confirmación adicional explícita para crear/activar/editar/revocar una autorización de fuente; la autorización anterior de proveedores no sustituye esa confirmación.

`support/doc110-source-profile.cjs` comprueba por ListarFuentes, con filtro y paginación hasta 100 páginas, que no exista la identidad DA/imp_02_egresos, incluyendo registros inactivos. Si existe, aborta **antes de escribir** con DOC110_SOURCE_ALREADY_REGISTERED. Si el listado no termina dentro del presupuesto, también aborta sin inferir ausencia. No elimina ni reutiliza fuentes previas, no cambia permisos ni habilita excepciones.

Después comprueba metadatos de CONSECUTIVO/FECHA. Desde UI crea fuente CONEXION con esas dos columnas, activa y edita retirando FECHA. Comprueba que metadatos ordinarios de proveedor ya no ofrezcan la columna retirada. Ejecuta el recorrido de proveedor/perfil ya aprobado: proveedor visual, clave unívoca/ausente/ambigua, publicación/versionado, regla de índice y ensayo real; ambos consumidores quedan inactivos al completar su ciclo.

Revoca la nueva fuente desde UI, exige ValidarProveedor 403 ACCESO_DENEGADO sin datos, conserva documentos completos del proveedor y de ambas publicaciones del perfil e historial. Exige historial de fuente con revisiones 4/3/2/1, acciones REVOCAR/EDITAR/ACTIVAR/ALTA, actor/fecha/motivo y snapshots coherentes. En finally consulta la fuente creada y revoca si sigue activa; no modifica una fuente que existía antes de la corrida. Una respuesta perdida se reconcilia mediante lectura, sin repetir la mutación inicial. Si lectura o revocación final fallan, la prueba falla y no acredita limpieza.

Un alta cuya respuesta se pierde no se repite: al ser creada inactiva, debe localizarse por UI antes de intentar otro registro. El escenario no elimina el registro ni su historial al finalizar; por unicidad no puede repetirse sobre esa misma identidad sin preparar otro ambiente/fixture. La creación concurrente por otro usuario puede producir conflicto; no se resuelve modificando su registro.

Reporte propio saneado en `tools/e2e/artifacts/doc110-source-profile-result.json`: estado, fechas UTC y código seguro de bloqueo/fallo; sin documento, identificadores de negocio, credenciales, cookies ni tokens. Se preservan los marcadores de verificación de consola. Screenshots/trazas siguen deshabilitados por la suite autenticada existente.

## Pruebas locales

```powershell
node --test tools/e2e/tests/doc110-source-profile.test.cjs tools/e2e/tests/doc110-provider-revalidation.test.cjs tools/e2e/tests/doc110-provider-history.test.cjs
```

**10 PASS**. Cubren recorrido coordinado con dobles, identidad existente activa/inactiva incluso en otra página, límite de paginación, metadatos incompatibles, exclusión de columna, recuperación tras activación de respuesta perdida o fallo de consumidor, preservación de documentos, auditoría requerida y rechazo por motivo correcto. También prueban autorización adicional antes de acceder al helper autenticado. No prueban selectores de la UI nueva contra IIS ni transacciones reales.

Sintaxis Node comprobada para runner, suite autenticada y soportes de fuente/proveedor. Regresión de soporte de proveedores se incluye en las diez pruebas. No cambia código productivo, SQL ni SII; sin despliegue.

## Ejecución pendiente

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --source-profile --headed
```

Requiere esquema SQL 07 completo, cuenta con las tres capacidades administrativas y ambiente/cuenta/datos expresamente autorizados para este ciclo, además de CONSECUTIVO unívoco introducido de forma oculta. La confirmación interactiva describe las mutaciones antes de iniciar login. No imprime NIT ni beneficiarios; no modifica filas DA ni crea expedientes/documentos.

Estado **NO_EJECUTADO**: no se autorizaron aún estas mutaciones nuevas del catálogo. Si la fuente compartida de imp_02_egresos ya está registrada, la prueba no puede acreditar alta/revocación nuevas allí; se necesita un fixture/ambiente aislado autorizado, sin borrar ni revocar la fuente compartida para forzar evidencia. No se asume que el historial ausente en la observación del 2026-10-23 continúe ausente ni que se haya corregido.

La tarea 15.16 y las verificaciones de persistencia/aislamiento/concurrencia reales siguen abiertas. Los PASS previos de proveedores y la UI simulada de 15.15 no sustituyen esta corrida.

## Corrida autorizada: bloqueo de origen de prueba

El usuario autorizó expresamente el ciclo en el mismo ambiente/cuenta y aportó salida: **1 failed, caso 22.7 s**. Autocompletado aprobado; ListarFuentes devolvió la identidad DA/imp_02_egresos y la prueba se detuvo con DOC110_SOURCE_ALREADY_REGISTERED. El reporte local confirma **BLOCKED**, inicio `2026-10-27T00:34:09.072Z`, fin `2026-10-27T00:34:09.143Z` (2026-10-26 en America/Bogota). Base Git `b2e93b726f221868cf2b82d43200a8e568871117`, con cambios locales sin commit.

No se invocaron CrearFuente/ActivarFuente/GuardarFuente/RevocarFuente ni el ciclo de proveedor/perfil desde este escenario. Se acredita el bloqueo preventivo sobre la identidad registrada; no se infiere su disponibilidad ni se acredita esquema completo, alta, historial transaccional o revocación. La corrida Playwright falla por precondición, mientras el reporte del escenario identifica BLOCKED; no es un PASS funcional ni un fallo demostrado del producto.

Para continuar hace falta un ambiente aislado autorizado con DA/imp_02_egresos disponible y sin esa autorización registrada en RA, o acordar otro origen descartable y adaptar explícitamente el escenario. No borrar la fuente existente ni su historial, ni revocarla para liberar la identidad. La autorización recibida sigue vigente para el alcance acordado; otro ambiente/origen requiere identificación y autorización explícitas. 15.16 continúa abierta.

## Origen alternativo propuesto por el usuario: imp_02_creditos

El usuario aportó el DDL de `imp_02_creditos` como respuesta a la solicitud de otro origen de prueba. Se adapta el runner en el mismo ambiente a `--source-profile --source-table imp_02_creditos --headed`, con confirmación interactiva que identifica explícitamente la tabla. No se ejecuta el CREATE TABLE: el DDL describe estructura propuesta/aportada, no acredita por sí solo existencia ni contenido de DA.

Contrato del fixture: CONSECUTIVO VARCHAR(250) como clave TEXTO; FECHADOCUMENTO DATE como columna FECHA que se autoriza inicialmente y se retira al editar. El proveedor continúa consultando únicamente CONSECUTIVO; no lee NITBENEFICIARIO, BENEFICIARIO ni DESCRIPCION, ni utiliza la PK incremental para sustituir silenciosamente la clave del escenario. El charset latin1 y las columnas restantes se conservan; se contrastan los metadatos reales por ASMX antes de crear la autorización.

El escenario no supone duplicación de CONSECUTIVO=44 en esta tabla. El runner solicita clave unívoca y, por entrada oculta, clave duplicada real o NO_APLICA. En este último caso imprime marcador NOT_RUN de ambigüedad y el reporte registra ambiguityStatus=NO_EJECUTADO; no se afirma aprobación de ese escenario. No se insertan filas para fabricar duplicados. Las otras comprobaciones del ciclo de fuente pueden ejecutarse, pero 15.16 conserva cualquier criterio de datos reales que no quede acreditado.

Se mantiene el comportamiento anterior de egresos como default y su regresión. El selector de fixture admite únicamente los dos contratos conocidos y rechaza nombres libres/SQL. Preflight de fuente ya registrada, límites y cleanup se aplican a la tabla elegida, sin modificar egresos. Once pruebas locales PASS, incluida regresión de FECHADOCUMENTO FECHA y rechazo de créditos ya registrado. Sintaxis Node y OpenSpec estricto comprobados; aceptación nueva aún sin resultado.

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --source-profile --source-table imp_02_creditos --headed
```

## Alta de créditos y fallo de navegación; corrección 2026-10-27

El usuario aportó corrida **1 failed, caso 32.2 s**. Preflight de identidad/metadatos PASS; el alta de créditos devuelve fuente inactiva de revisión 1. Antes de activarla falla el cierre mediante Escape: queda un diálogo visible. El finally consulta la fuente dos veces y confirma que permanece inactiva, sin revocación ni continuación de consumidores. Reporte local FAIL/CHECK_FAILED, inicio `2026-10-27T00:43:05.828Z`, fin `2026-10-27T00:43:21.493Z`. No acredita activación, edición ni revocación del catálogo.

Se corrige únicamente el driver E2E: cierra el diálogo visible mediante su botón Cerrar y comprueba que desaparezca antes de navegar. El teclado Escape requiere foco dentro del panel; regenerar el detalle elimina el control pulsado y puede dejar foco en body. La regresión Chromium coloca el foco explícitamente en body después del alta y comprueba cierre/reapertura. No se cambia el producto ni se elimina la comprobación de diálogo cerrado.

La autorización ya creada no se elimina para repetir. El modo explícito `--resume-source` exige: reporte local previo FAIL/CHECK_FAILED para la misma tabla; fuente DA inactiva, revisión 1, modalidad CONEXION y columnas originales; un único historial ALTA de revisión 1 con motivo exacto de la prueba, snapshot íntegro y fecha dentro de la ventana UTC de esa corrida (tolerancia de un segundo por DATETIME). Si el reporte tiene sourceId, también debe coincidir. Cualquier diferencia aborta antes de mutar. Sin fuente encontrada no crea otra; sin flag de reanudación el modo normal sigue rechazando cualquier identidad existente.

Las futuras corridas guardan el ID técnico de fuente en su reporte saneado; la reanudación conserva la referencia temporal del reporte original. No registra datos de negocio, credenciales ni tokens. Tras activación/edición/revocación sigue exigiendo las mismas revisiones 2/3/4 y comprobaciones del consumidor; no convierte una fuente ajena en fixture ni atribuye de nuevo el alta a la reanudación.

**14 pruebas Node PASS**, incluidos rechazo de fuente modificada/historial ajeno/ventana distinta, ausencia de fuente, fechas ASMX y reanudación sin CrearFuente. **4 Chromium PASS** con transporte simulado, incluido cierre tras pérdida de foco. Sintaxis Node, OpenSpec estricto y diff de espacios comprobados. Aceptación real de la corrección pendiente, autorización de este ciclo ya recibida.

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --source-profile --source-table imp_02_creditos --resume-source --headed
```

## Corrección de selección de fila, 2026-10-27

Reanudación autorizada aportada por el usuario: **1 failed, caso 31.8 s**. ListarFuentes, ObtenerFuente e HistorialFuente verifican identidad/auditoría del alta fallida; metadatos PASS. Falla el driver antes de activar al encontrar cero filas con un selector incorrectamente anclado al diálogo dentro de `filter({has:...})`. Playwright evalúa ese locator desde cada fila, por lo que el ancestro dialog no puede formar parte del filtro interno.

Reporte local FAIL/CHECK_FAILED, inicio `2026-10-27T11:05:52.947Z`, fin `2026-10-27T11:06:08.304Z`, sourceOrigin=RESUMED_FAILED_RUN. El finally confirma fuente inactiva; no acredita activación, edición, consulta de proveedor ni revocación. La ventana UTC original del alta sigue conservada en resumeOrigin. A partir de la siguiente reanudación se conserva también el ID técnico ya observado para exigir identidad exacta además de auditoría/tiempo.

Se extrae `sourceRow` usado por el driver y por una regresión Chromium. Los filtros internos parten de celdas relativas a la fila; el conjunto exterior sigue restringido al diálogo. La prueba distingue DA/créditos de RA/créditos, un sufijo parecido, egresos y otra tabla fuera del diálogo, y comprueba que el botón de la fila correcta recibe foco. También comprueba ausencia de otra identidad. No se amplían permisos ni se cambia el producto.

Validación local: **14 Node PASS y 5 Chromium PASS**, sintaxis y OpenSpec estricto PASS. Corrida real corregida pendiente; 15.16 continúa abierta. Se puede repetir el mismo modo --resume-source bajo la autorización vigente: la fuente permanece en revisión 1 y no se repite el alta.

## Activación/edición aprobadas y timeout al pasar a proveedores, 2026-10-27

El usuario aporta corrida que agota **180000 ms**. Se verifican identidad/auditoría, activación y edición de fuente desde UI; el marcador SOURCE_UI_EDIT_REMOVES_COLUMN_FROM_PROVIDER_METADATA confirma que FECHADOCUMENTO deja de ofrecerse a proveedores y CONSECUTIVO sigue disponible. También pasa el preflight directo de metadatos del proveedor. No se alcanza PROVIDER_CREATE_CONFIGURE_SAVE_UI.

El bloqueo se produce al esperar apertura/listado del administrador de proveedores. La causa exacta en IIS no queda acreditada por la salida. El driver anterior alternaba el menú sin comprobar si ya estaba abierto y esperaba respuesta sin plazo local; se sustituye por navegación compartida que abre solo cuando aria-expanded no es true, cierra diálogo visible explícitamente y limita click/espera a 15 s. No se aumenta el timeout global ni se reintenta la operación. Una espera corta permite que el finally disponga de tiempo antes del límite de la prueba.

La recuperación de esta corrida **no queda confirmada**: el finally falla en ObtenerFuente al finalizar la prueba. El reporte local es FAIL/DOC110_TRANSPORT_FAILED, inicio `2026-10-27T11:09:26.472Z`, fin `2026-10-27T11:12:11.081Z`. No afirmar que la fuente esté inactiva ni que se haya revocado; debe reconciliarse antes de otro ciclo. Como ya pasó activación/edición, tampoco es válido el resume anterior limitado a revisión 1.

Se prepara un modo separado --cleanup-source: lee el ID técnico y origen del reporte fallido, verifica alta original con snapshot/fecha/motivo, secuencia exacta de estados ALTA/ACTIVAR/EDITAR y, si ya consta, REVOCAR; exige mismo actor y motivos de prueba, revisiones consecutivas y documento actual igual al último snapshot. Solo entonces revoca la fuente si sigue activa, con su revisión actual, y confirma lectura inactiva. Si ya está inactiva no escribe. Si el historial/estado cambió, aborta sin modificarlo. No inicia proveedores/perfiles ni consulta datos DA. Reporte dedicado `doc110-source-cleanup-result.json`, sin sobrescribir la evidencia de la corrida fallida.

Validación local: **16 pruebas Node PASS y 5 Chromium PASS**. Incluye recuperación propia activa/already inactive, rechazo de actor/motivo/estado/fecha alterados, navegación con menú abierto/cerrado y fallo de respuesta acotado sin duplicar clic. Transporte simulado; recuperación HTTP aún pendiente. Sin código productivo, DDL, permisos ni cambios a SII.

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --source-profile --source-table imp_02_creditos --cleanup-source --headed
```

Se mantienen pendientes el ciclo consumidor integrado y los demás criterios de 15.16. La autorización vigente incluye revocar exclusivamente esta fuente de prueba; la consola describe que esta corrida solo verifica/recupera esa fuente. No reutilizar aún el modo --resume-source de revisión 1 sobre el estado posterior a activación/edición.

## Recuperación detenida por sesión no vigente, 2026-10-27

Resultado aportado por el usuario al ejecutar --cleanup-source: **1 failed, caso 12.4 s**. La sonda ListarPerfiles devuelve HTTP 401 y la suite emite DOC110_SESSION_REQUIRED antes de entrar al helper de recuperación. No se invocan ObtenerFuente/HistorialFuente/RevocarFuente en este intento. El reporte de recuperación aún no existe; el reporte de la corrida anterior permanece intacto.

No se acredita consulta ni revocación de la fuente. Su disponibilidad actual continúa **sin confirmar**. La causa del login no queda determinada: la respuesta de postback por sí sola no prueba sesión de Gestión vigente; no inferir contraseña incorrecta, segundo factor, expiración o un problema del catálogo a partir de ese 401. Se conserva la autorización recibida y la validación de sesión; repetir requiere ingresar los datos correctos por la misma consola efímera, sin copiar credenciales al chat ni al reporte. No se modifica login, permisos o protección CSRF para eludir el rechazo.

## Recuperación autenticada aprobada, 2026-10-27

Resultado aportado por el usuario: **1 passed (17.2 s)**, caso **16.2 s**, ejecutando --source-profile --source-table imp_02_creditos --cleanup-source --headed. Autocompletado aprobado; ObtenerFuente e HistorialFuente verifican pertenencia al fixture de la corrida fallida, RevocarFuente confirma la revocación y un ObtenerFuente posterior confirma estado inactivo. Marcador VERIFIED_FAILED_RUN_SOURCE_RECOVERED_INACTIVE aprobado.

El reporte local dedicado confirma PASS, tabla imp_02_creditos, inicio `2026-10-27T11:27:36.831Z`, fin `2026-10-27T11:27:36.921Z`. Base Git `b2e93b726f221868cf2b82d43200a8e568871117` con cambios locales sin commit. Se conserva el reporte de la aceptación fallida; no se registran claves de consulta, datos personales, contraseñas, cookies ni tokens.

Supera la incertidumbre de estado posterior al timeout y el intento fallido por sesión. No se crean fuentes/proveedores/perfiles, no se consulta DA ni se ejecuta DDL en la recuperación. Se conserva fuente e historial; no se elimina el registro para liberar unicidad.

La recuperación no equivale a PASS del ciclo fuente → proveedor → perfil → ensayo ni de la ambigüedad con créditos. 15.16 sigue abierta. No ejecutar el modo --resume-source limitado a revisión 1 sobre esta fuente ya activada/editada/revocada; el siguiente avance requiere preparar continuidad desde este estado verificado o un fixture distinto aprobado, sin borrar historial ni repetir alta.

## Continuidad desde recuperación PASS preparada, 2026-10-27

Se añade el modo explícito --continue-source al runner existente. Exige el reporte dedicado de recuperación PASS para la misma tabla y posterior al reporte de origen, además de la fuente propia actualmente inactiva en revisión 4. Revalida por lecturas autorizadas la identidad técnica y las cuatro entradas de historial: alta original dentro de su ventana UTC, actor, motivos, configuración y estados consecutivos ALTA/ACTIVAR/EDITAR/REVOCAR. Cambios externos, reporte fallido/de otra tabla o estado diferente abortan antes de escribir.

No repite alta ni edición, ni elimina historial. Reactiva por UI a revisión 5, verifica de nuevo metadatos CONSECUTIVO disponible/FECHADOCUMENTO excluida y ejecuta el recorrido existente de proveedor/perfil. Al terminar revoca por UI a revisión 6. Exige historial de seis revisiones con acciones REVOCAR/ACTIVAR/REVOCAR/EDITAR/ACTIVAR/ALTA y compara íntegramente las cuatro entradas previas con el snapshot leído al comienzo. Comprueba rechazo posterior del proveedor y documentos/historiales de consumidores inalterados. El finally sigue reconciliando y revocando el fixture si queda activo; el modo de recuperación admite verificar también esas dos entradas adicionales conocidas, sin mutar una fuente ya inactiva.

La navegación proveedor/fuente se hace con el helper compartido que comprueba menú abierto/cerrado, cierra el diálogo visible y acota click/espera de listado sin aumentar el timeout global. El callback de transición no hace un cierre adicional antes del helper. No hay reintentos automáticos de escritura.

Validación local: **19 pruebas Node PASS**, incluyendo continuidad sin alta/edición, inmutabilidad de las cuatro revisiones previas, rechazo de reporte de recuperación inválido o auditoría cambiada y limpieza si falla el consumidor. **7 Chromium PASS** con transporte simulado: navegación compartida, selección de fila, fuente/proveedor/perfil y errores. Sintaxis Node y OpenSpec estricto PASS; sin código productivo, SQL ni despliegue. Ejecución nueva IIS pendiente; tarea 15.16 permanece abierta.

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --source-profile --source-table imp_02_creditos --continue-source --headed
```

Conserva el mismo ambiente/cuenta/alcance autorizado y solicita credenciales/claves solo por consola. La confirmación identifica expresamente la continuidad desde recuperación PASS, reactivación y revocación de la fuente de prueba. NO_APLICA para clave ambigua sigue significando escenario de ambigüedad NO_EJECUTADO, no aprobado.

## Aceptación integrada aprobada: continuidad con créditos, 2026-10-27

El usuario aporta resultado **1 passed (23.8 s)**, caso **22.8 s**, de --source-profile --source-table imp_02_creditos --continue-source --headed. El reporte local de fuentes confirma PASS, sourceOrigin=CONTINUED_AFTER_RECOVERY y ambiguityStatus=EJECUTADO_OK; inicio `2026-10-27T11:41:57.210Z`, fin `2026-10-27T11:42:04.392Z`. El reporte de publicación también confirma PASS, final `2026-10-27T11:42:03.717Z`. Base Git `b2e93b726f221868cf2b82d43200a8e568871117`, con cambios locales sin commit.

Se verifica nuevamente el alta original y la auditoría de la fuente; la edición previa queda acreditada por su historial íntegro y los metadatos actuales. La nueva activación incrementa a revisión 5. El recorrido crea/configura/valida un proveedor por UI, comprueba SIN_COINCIDENCIA y AMBIGUO con claves reales y confirma la clave unívoca sin registrarlas. Publica dos versiones del proveedor y conserva la primera.

El perfil selecciona la versión publicada y propiedad, guarda/relee referencias y resuelve el índice real. La versión BORRADOR del proveedor permanece excluida del catálogo y los IDs directos se rechazan al guardar/ensayar sin cambios. Desactivar el proveedor impide publicar el perfil sin alterar su documento/historial; restaurarlo permite continuar. Dos versiones del perfil se publican con EXPEDIENTE/LOTE, conservando la primera inmutable. Perfil y proveedor descartables terminan inactivos, con sus referencias e historiales intactos.

La revocación final de la fuente por UI incrementa a revisión 6. ValidarProveedor retorna 403 ACCESO_DENEGADO sin datos; las lecturas posteriores comprueban documento completo del proveedor, ambas publicaciones del perfil e historial sin cambios. HistorialFuente conserva las seis revisiones y las cuatro primeras entradas comparadas íntegramente contra su lectura inicial. ObtenerFuente final confirma fuente inactiva. Marcadores SOURCE_UI_REVOCATION_DENIES_USE_PRESERVES_CONSUMERS_AND_AUDIT y SOURCE_OWNED_BY_TEST_FINALLY_INACTIVE aprobados.

La evidencia acumulada de las corridas de alta/edición y esta continuidad completa 15.16 usando **imp_02_creditos**, propuesta por el usuario como alternativa al origen compartido imp_02_egresos. No se repitió el alta ni se borró historial. Esta sustitución es de datos de prueba: no cambia contratos del producto ni habilita una tabla fuera de la restricción imp_02. Se preserva DA, sin ejecutar el CREATE aportado ni insertar/modificar datos de negocio.

Persistencia observada mediante respuestas y lecturas ASMX autenticadas; no hubo controles SELECT nuevos en esta sesión. No se atribuye a esta aceptación cobertura de aislamiento entre empresas, rollback transaccional ante fallos inyectados, concurrencia de fuentes o concesión/revocación de permisos externos; esas tareas siguen abiertas. La recuperación PASS aislada no se confundió con aceptación completa: el cierre de 15.16 se apoya ahora en el ciclo consumidor y revocación aprobados, incluida ambigüedad real.
