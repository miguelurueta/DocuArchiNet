# DOC-110 — Ejecución real HTTP y navegador

## Estado vigente: publicación y versionado aprobados

Corrida `npm.cmd --prefix tools/e2e run test:doc110:session -- --publish --headed`: PASS, 15,3 segundos de caso y 16,3 segundos totales. Resultado comunicado por el usuario y contrastado con `tools/e2e/artifacts/doc110-publication-result.json`, finalizado el 2026-10-20 a las 21:03:16 UTC.

Ocho comprobaciones aprobadas: catálogos autorizados RA/DA, configuración e índices válidos, publicación EXPEDIENTE desde UI, rechazo de edición publicada, segunda versión publicada LOTE, primera versión inmutable tras recarga HTTP, historial con dos versiones publicadas y desactivación del perfil descartable. Once consultas de catálogo respondieron entre 8 y 123 ms. Las restantes llamadas HTTP cronometradas respondieron entre 16 y 51 ms; no se midió por separado la publicación UI.

Esta ejecución supera el pendiente de publicación válida descrito en los apartados históricos siguientes. No acredita el conjunto completo de aceptación DOC-110: siguen pendientes cuenta restringida/aislamiento entre empresas, revocación externa, concurrencia entre sesiones independientes, esquema indisponible, controles SELECT independientes y regresión funcional de carga individual. No creó expedientes ni almacenó documentos. No se cierra ni archiva el cambio por esta corrida.

## Navegador visible

Etapa de publicación: `npm.cmd --prefix tools/e2e run test:doc110:session -- --publish --headed`. No combinar `--publish` con `--draft`. Reutiliza la misma consola, login, navegador y runner. Consulta catálogos autorizados y busca una combinación válida con presupuesto de 120 solicitudes y diez páginas por catálogo; si no encuentra referencias compatibles, falla antes de crear el perfil. Nunca crea catálogos ni modifica documentos.

La etapa publica EXPEDIENTE desde el editor, comprueba el rechazo de modificación de esa versión, crea/publica otra con LOTE y verifica por HTTP la inmutabilidad de la primera y dos versiones en historial. Al completar desactiva el perfil descartable. Guarda exclusivamente estado, nombres de comprobaciones y fecha en `tools/e2e/artifacts/doc110-publication-result.json`. No acredita SELECT independiente, aislamiento entre cuentas o concurrencia entre sesiones. La publicación real queda pendiente hasta ejecutar y revisar su resultado.

Ejecutar `npm.cmd --prefix tools/e2e run test:doc110:session -- --draft --headed` desde la consola local. Solicita las credenciales por el mecanismo existente y abre el navegador visible mediante la opción nativa `--headed` de Playwright. Sin `--headed`, conserva la ejecución sin ventana. No cambia el modo de otras suites.

La ventana muestra login, Producción, listado y creación del perfil. Los pasos posteriores de guardado, ensayo, concurrencia e historial siguen siendo llamadas HTTP autenticadas; sus resultados se muestran en consola. El modo visible no convierte esas comprobaciones en interacciones de formulario ni introduce pausas. El navegador se cierra al terminar la prueba.

Fecha: 2026-10-20. Ambiente autorizado por el usuario, identificado mediante el perfil local DOC-88 existente. Se reutilizaron Playwright, navegador, configuración y helper de autenticación del repositorio.

| Ejecución | Resultado | Alcance |
|---|---|---|
| `npm.cmd --prefix tools/e2e run test:doc110:anonymous -- --reporter=list` | 13 PASS, 9,2 segundos | Navegador real abre el login. Los doce métodos ASMX responden HTTP 401 con código SESION_REQUERIDA, sin datos y con no-store ante solicitudes anónimas. |
| `npm.cmd --prefix tools/e2e run test:doc110:session` | BLOCKED, salida no cero | Se intentó iniciar la prueba administrativa. No hay credenciales efímeras ni consola interactiva del usuario en esta ejecución. Se detuvo antes del login. No es PASS ni una prueba omitida presentada como éxito. |
| `node --test tools/e2e/tests/authenticated-workflow-session.test.cjs tools/e2e/tests/authenticated-workflow-session-usage.test.cjs` | 12 PASS | Regresión aislada del helper compartido; no acredita sesión real. |
| `node --check tools/e2e/scripts/run-doc110-profile-session.cjs` | PASS | Sintaxis del iniciador. |

SHA-256 de la suite anónima ejecutada: `2cae3e44ae542b750003e4c8784b8930d7845be5eb831b96e1951958755d8393`.

## Continuación autenticada

Ejecutar `npm.cmd --prefix tools/e2e run test:doc110:session` desde la consola local del usuario. El iniciador recupera URL/módulo del perfil DOC-88 y usa `collectValue`/`promptSecret` existentes; solicita cuenta de prueba y contraseña oculta. También admite esas credenciales mediante variables efímeras DOC110_E2E_USER/PASSWORD. No crea `.env`, no guarda sesión ni asigna permisos.

La prueba administrativa abre Producción, verifica el acceso al editor y consulta ListarPerfiles mediante la UI real. No crea perfiles. Una cuenta sin permiso administrativo o con un desafío adicional de login no se sustituye automáticamente por otra ni se considera aprobada.

Las cuatro pruebas funcionales de creación/publicación/historial/concurrencia continúan pendientes de cuentas, fixture y control SELECT configurados. La corrida anónima no satisface esos escenarios ni permite cerrar las tareas de aislamiento entre usuarios, CSRF autenticado, persistencia o aceptación externa.

## Corrida interactiva posterior

El usuario ingresó la cuenta en consola visible y ejecutó la prueba administrativa. Resultado: FAIL en apertura del editor (35,8 segundos). La captura de Playwright muestra navegación de Producción; no aparece el botón del perfil.

Diagnóstico SELECT en el origen RA, resuelto desde la configuración de módulos: una cuenta coincidente, activa, empresa positiva, una fila de permisos y cero filas con ADMINISTRAR_PERFILES_IMPORTACION=1. No se consultaron contraseñas de usuario ni se concedieron permisos. El editor omite su contenedor cuando el adaptador rechaza la autorización.

Se mejoró la prueba: si falta el contenedor, consulta ListarPerfiles sin mutación para distinguir rechazo de acceso, sesión ausente e indisponibilidad; no guarda cuerpos ni tokens. El diagnóstico mejorado todavía requiere otra corrida autenticada. La habilitación corresponde a la administración externa acordada; después debe repetirse la prueba. No marcar este fallo como capacidad administrativa aprobada.

## Corridas posteriores y corrección del tiempo acumulado

Tras la habilitación manual del permiso, el usuario ejecutó la sesión administrativa: PASS (46,2 segundos). La siguiente corrida con borrador confirmó CREATE_UI, SAVE_DRAFT, RELOAD_PERSISTED y STALE_REVISION_REJECTED; agotó los 180 segundos globales mientras esperaba EnsayarReglas. No prueba que esa llamada aislada consumiera 180 segundos ni que el resolvedor tenga un bucle.

La comprobación independiente de esquema, usando SELECT y el ensamblado real, tardó 23.945 ms. La verificación de FK cruzaba KEY_COLUMN_USAGE y REFERENTIAL_CONSTRAINTS. Se sustituyó ese JOIN por consultas parametrizadas acotadas por tabla y restricción; conserva comprobación del destino, unicidad y acciones RESTRICT, sin introducir cache global. Tras compilar, la misma comprobación pasó en 3.368 ms (ambos tiempos incluyen arranque PowerShell/CLR). Falta medir el recorrido HTTP completo después del cambio.

El error de Playwright incluyó cabeceras sensibles en la consola del usuario. No se reproducen aquí. Se añadió safePost: timeout por solicitud de 30 segundos, sin reintentos, error nuevo sin causa/stack original y medición por operación sin URL, cuerpo ni cabeceras. Se aplica al ciclo de borrador, sonda autenticada y suite completa. Nueve pruebas locales PASS, incluidas dos que verifican que un fallo sintético no revela secretos ni repite la petición. Compilación real PASS.

El perfil descartable creado por la corrida fallida pudo quedar activo: la desactivación final no se alcanzó. No se borran ni reenvían operaciones de resultado incierto. La siguiente corrida usa otro código único y no implica recuperación o limpieza del perfil anterior.

## Repetición posterior a la corrección: PASS

Confirmada con la salida compartida por el usuario y `tools/e2e/artifacts/doc110-draft-result.json`, finalizada el 2026-10-20 a las 20:41:18 UTC. Comando: `npm.cmd --prefix tools/e2e run test:doc110:session -- --draft`. Resultado: una prueba con nueve comprobaciones aprobadas; 22,6 segundos de caso y 23,3 segundos totales.

Comprobaciones: creación desde UI, guardado, recarga persistida por HTTP, rechazo de revisión obsoleta, dos patrones con ceros iniciales, rechazo de publicación incompleta conservando BORRADOR, dos guardados con un único ganador, historial y desactivación del perfil descartable de esta corrida.

EnsayarReglas respondió en 34 y 19 ms; las solicitudes medidas estuvieron entre 18 y 533 ms. El recorrido ya no agotó el presupuesto de 180 segundos. Son mediciones de esta corrida, no una garantía de rendimiento ni prueba de carga.

Límite de concurrencia: dos solicitudes del mismo contexto autenticado; ASP.NET puede serializarlas mediante el bloqueo de sesión. Acredita el rechazo de revisión obsoleta por HTTP y la recarga del ganador, no una carrera de transacciones entre sesiones independientes. No hubo controles SELECT independientes del contenido persistido en esta corrida.

Pendientes: publicación válida con clasificación/gabinete DA, nueva versión e inmutabilidad publicada, cuenta restringida, revocación, aislamiento empresarial y concurrencia entre sesiones independientes. La desactivación aprobada corresponde únicamente al perfil creado por esta corrida; no acredita limpieza de intentos anteriores. DOC-110 no se cierra por este resultado.
