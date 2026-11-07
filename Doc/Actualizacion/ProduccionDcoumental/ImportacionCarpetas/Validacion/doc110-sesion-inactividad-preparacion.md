# DOC-110: aceptación de token ajeno y expiración por inactividad

7.4 completa con la corrida autenticada y las denegaciones de permisos previas. 7.8 conserva criterios pendientes, incluida revocación externa de permisos. Sin modificación de permisos, timeout, perfiles, fuentes, SII ni DDL. Las secciones de preparación inferiores describen el estado anterior a esta aceptación.

## Aceptación autenticada

Resultado aportado por el usuario: **1 passed (11.9 min)**. Reporte local saneado doc110-session-expiry-result.json: PASS, configuredIdleMinutes 10, observedIdleMilliseconds 675041 (11 min 15.041 s), checks 4; inicio del helper 2026-10-27T14:44:43.970Z y fin 2026-10-27T14:56:15.460Z. El intervalo del helper incluye el segundo login y las verificaciones, además de la inactividad medida.

SESSION_EXPIRY_INDEPENDENT_AUTHENTICATED_SESSIONS confirma dos sesiones distintas y tokens distintos. La segunda sesión listó correctamente antes de la espera; no fue un contexto anónimo recién creado.

OTHER_SESSION_TOKEN_READ_AND_WRITE_DENIED_PROFILE_HISTORY_UNCHANGED confirma 403 SOLICITUD_NO_VERIFICADA en listado y guardado inválido con token de otra sesión; la sesión receptora sigue autenticada y su perfil/historial permanecen iguales.

PREVIOUSLY_AUTHENTICATED_IDLE_SESSION_DENIED_PROFILE_HISTORY_UNCHANGED confirma 401 SESION_REQUERIDA al listar e intentar guardar desde la sesión que estuvo inactiva. Durante los 675041 ms observados se cerraron páginas y se conservaron cookies/contexto; solo la sesión control recibió lecturas periódicas. Esta última siguió autorizada y releyó perfil/historial intactos al terminar. No se ejecutó logout ni se forzó Abandon, limpieza de cookies, cambio de timeout o revocación de permisos.

Para 7.4, la aceptación se combina con la segunda cuenta autenticada sin facultad del recorrido previo --isolation y los rechazos HTTP de mutaciones ya acreditados; [evidencia anterior](doc110-continuacion-20261024.md). La prueba real entre empresas es NO_APLICA por decisión del usuario. Se completa el criterio sin atribuir a la expiración una concesión/revocación externa de permisos: esta última sigue pendiente en 7.6/7.8/9.6. Tampoco se atribuye a esta corrida un 503 real o una respuesta no JSON.

## Recorrido preparado

Modo exclusivo --session-expiry del runner existente. Lee únicamente el timeout explícito de sessionState en el Web.config local (10 minutos), con rango admitido de 1 a 60. No modifica configuración ni presume que el valor desplegado sea idéntico.

Abre dos sesiones independientes de la cuenta autorizada; compara solo el booleano de IDs ASP.NET y tokens distintos. Lee un perfil existente e historial desde la sesión control y confirma que la segunda sesión puede listar con su propio token antes de dejarla inactiva.

Con el token de la segunda sesión, la primera debe recibir 403 SOLICITUD_NO_VERIFICADA al listar y al intentar GuardarBorrador. El payload de guardado lleva versionId 0 y configuración null, por lo que nunca representa un guardado válido, aun ante fallo del guard. Se relee perfil/historial intactos.

Después cierra las páginas de ambos contextos para detener temporizadores de navegador, sin borrar cookies, cerrar contextos ni ejecutar logout/Abandon. Espera timeout más 75 segundos (11 min 15 s con configuración actual). Cada 30 segundos solo la sesión control consulta ListarPerfiles para conservarse activa. La sesión que debe expirar no recibe ninguna solicitud durante ese intervalo.

Tras la espera, la misma segunda sesión y su token anterior deben recibir 401 SESION_REQUERIDA en ListarPerfiles y en el guardado inválido. La sesión control debe seguir leyendo perfil/historial idénticos al snapshot inicial. Una sesión anónima recién creada o el borrado de cookies no sustituyen este caso.

El timeout de Playwright se amplía únicamente para este modo a la espera configurada más margen de ejecución; el timeout normal 180 s se conserva en los demás. Si IIS usa otro tiempo y la sesión sigue vigente, se informa fallo; no se invalida artificialmente para obtener PASS. El reporte saneado doc110-session-expiry-result.json guarda resultado, tiempo configurado, duración observada de inactividad y fechas, sin cookies/tokens/credenciales ni documentos.

## Verificación local

```powershell
node --test tools/e2e/tests/doc110-session-expiry.test.cjs tools/e2e/tests/doc110-safe-request.test.cjs
```

**5 pruebas PASS**: verificador con transporte y espera simulados, detección de sesión aún vigente/perfil cambiado/código de error incorrecto, parser acotado y transporte sin reintento ni exposición de cabeceras. No se atribuye a estos dobles una expiración real ni se espera once minutos en la suite local.

## Ejecución autenticada preparada

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --session-expiry --headed
```

Ambiente y cuenta ya autorizados; credenciales efímeras en consola. La espera larga es parte del criterio de inactividad; la consola informa minutos restantes. HTTP nuevo **NO_EJECUTADO**. No acredita concesión/revocación externa de permisos ni indisponibilidad real de RA. La aceptación real entre empresas es NO_APLICA conforme al alcance confirmado.
