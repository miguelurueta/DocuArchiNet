# DOC-119 — selector de aceptación de lectura

Base de preparación: `f605f0ada91dc7173317fd2592df9d665dc3c5bb`, con cambios locales del selector. El usuario indicó «autorizado» para el plan SEG-01/02 sobre https://localhost/GestionDocumental-Docuarchi.net/. Las cuentas y recursos se fijan por consola; no se heredan credenciales, sesiones o permisos de DOC-118.

## Implementación preparada

Nuevo `tools/e2e/scripts/run-doc119-seguimiento.cjs`, con soporte en `tools/e2e/tests/support/doc119-seguimiento.cjs`. Reutiliza consola enmascarada, openSession/observación de login, createSelectControls, queryRows, captura de evidencia y selectLevel existentes. El export nuevo de selectLevel en soporte DOC-113 no altera su ejecución. Sin DTO/backend/login/puente SQL/coordinador alternativos.

Solo admite la URL del plan, recursos capturados y operaciones de lectura. No permite confirmación, preparación, fases, reconciliación, detención, upload, cambios de permisos ni limpieza. Los postbacks de Producción se limitan a selección de nivel original/alterno mediante Button_inline_trevie y HiddenField_0003; otros botones se bloquean. Una violación cierra la guardia para las siguientes solicitudes.

Por ventana: SEG-01 máximo 120 HTTP/80 SELECT/2 sesiones; SEG-02 máximo 80 HTTP/60 SELECT/2 sesiones. HTTP cuenta también recursos estáticos permitidos, login, diagnóstico y comprobación de fuentes. SELECT cuenta controles explícitos RA/DA, no se inventa una cuenta de consultas internas del servidor. Cada consumo se registra antes de ejecutar; no se reutiliza runId o reinicia contador.

Estado y evidencia saneada únicamente en `.tmp/doc119-seguimiento/<runId>/state.json`, ignorado por Git. No guarda cuentas, claves, cookies, tokens, formularios, CSV ni cuerpos de datos. La ruta del PDF solo vive en memoria. DEBUG/PWDEBUG se desactivan en el proceso de prueba y no se generan trazas, video o capturas de login.

## Comprobaciones locales

Archivo cliente: pruebas de filtros/CSV previas más tres recorridos nuevos del selector: propietario (filtros, segunda página, descarga y vacío), cuenta ajena (lote/CSV denegados) y SEG-02 (recarga real de navegador, reselección sin envío, homónimos y rama original sin duplicación/selección alterada).

Los dos primeros usan DOM/cliente/descarga reales con XHR sintético y un adaptador de eventos de respuesta exclusivamente local; no se usa ese adaptador en la corrida autenticada. SEG-02 usa solicitudes interceptadas, cliente/asistente/helpers reales y un archivo sintético conservado en .tmp. Ninguno acredita IIS, cuenta real o SQL real.

Pruebas de política: 13 casos de CLI/autorización, perfiles sin secretos, origen/recursos/DTO, exclusión de efectos/ASHX/postbacks de negocio, presupuesto, guardia que permanece cerrada, CSV y controles SELECT/fingerprint. `--check` y sintaxis Node pasan sin red ni credenciales.

Corrida consolidada: `node --test tests/doc111-importacion-carpetas-client.test.cjs tools/e2e/tests/doc119-seguimiento-policy.test.cjs`: **34/34 PASS** (21 cliente/selector y 13 de política), cero fallos/omitidas. OpenSpec estricto y auditoría de refinamiento PASS. No se repite el arnés backend sin cambios ni se suman sus casos internos a esta cifra. A este corte aún no hubo autenticación, HTTP de aplicación ni SELECT reales DOC-119.

## Límites de aceptación

SEG-01 exige un filtro con segunda página real bajo el tamaño de la política actual y estados mixtos; no se fabrican filas ni se reduce la política del servidor. Lote5 es solo candidato y puede ser insuficiente. SEG-02 requiere un archivo sin recibir, raíz confirmada, rama original cargada, nivel alterno y PDF correspondiente. Puede omitirse si no existe recurso y se declara NO_EJECUTADO.

Homónimos/nuevas relaciones no observados o ausencia de alcance documental DA se informan expresamente. PASS_READONLY no cambia tareas ni certifica por sí solo toda la aceptación. Reselección solo valida metadatos/identidad local: no se afirma igualdad de bytes/hash servidor sin recepción autorizada aparte. Datos/temporales y recursos previos se conservan; sin retry automático.
