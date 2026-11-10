# DOC-118 — carrera del observador corregida y parada en el primer login

## Comprobación del observador

Se reprodujo localmente una carrera: assertObserverReady tomaba Date.now al entrar, antes de leer los archivos de estado. Si el controlador publicaba un heartbeat durante esa lectura, se calculaba una edad negativa y se rechazaba un observador disponible. La prueba reprodujo el mismo código OBSERVER_NOT_READY_BEFORE_CAPTURE con la versión anterior. La corrección toma la hora después de leer cada candidato. Pasan tanto la reproducción como el rechazo de un heartbeat realmente futuro; se conserva el límite de antigüedad. No se relajaron las condiciones de proceso, fuente, worker o liberación.

La parada previa ocurrió en RUNTIME_RECHECK, sin login. Sus datos son compatibles con la carrera, pero no conservaron la edad exacta; no se atribuye retrospectivamente una medición inexistente. Ambas capturas previas se preservaron por ruta y SHA256. El usuario había pedido «lanza» y el alcance de captura se revisó únicamente con cero logins y cero efectos anteriores.

## Ejecución posterior

Se contabilizó una HEAD adicional de preparación, sin redirects o retries. Las tres HEAD permanecen dentro del máximo de1000 HTTP de esta ventana. Se conectó el observador3 con fuente enlazada y la captura visible completó las cuatro entradas. El flujo superó RUNTIME_RECHECK y solicitó PREFLIGHT; se intentó la primera sesión. Se detuvo antes de confirmar la autenticación, sin ConsultarLote, SELECT, planes, lotes o documentos nuevos. No hubo ARM, Freeze o entrada al escritor.

El wrapper ejecutado conservó únicamente PHASE_UNCONFIRMED y perdió el código específico del helper de sesión. No se concluye contraseña incorrecta, cuenta inválida ni MFA a partir de ese dato. En el intervalo observado el log IIS muestra GET y POST de la página de login con HTTP200; esa correlación por hora/endpoint no acredita autenticación ni identifica el motivo. No se guardaron usuarios, querystrings, formularios o mensajes privados. No se reintenta automáticamente el login.

Se reutilizó después el proyector cerrado de fallos de preflight ya existente en DOC117. La versión candidata conserva códigos y loginObservation saneados, descarta causas privadas e incluye esa dependencia en su sello. No se atribuye este diagnóstico posterior al proceso terminado. Regresión candidata completa:274/274 Node;157 casos V2 internos incluidos sin sumarlos.

## Estado y consumo

SDK CLOSED con cleanupConfirmed=true, contextos cerrados, credenciales eliminadas y lease liberado. Consumo de la ventana:93 HTTP/0 SELECT/1 intento de sesión, cero efectos. Acumulado1671 HTTP/172 SELECT/12 intentos/aperturas. Quedan907 HTTP,240 SELECT y1 apertura del cupo de2; eso no alcanza por sí solo para repetir el recorrido completo que exige dos sesiones nuevas. No se reinicia ni amplía ese libro para ocultar el fallo.

32/38 tareas, seis comprobaciones reales pendientes. El lote25 conserva el documento16235/inventario23339 verificados, sin replay; no se creó otro recurso. Almacenamiento compartido/SII, adaptadores protegidos, DLL/PDB, Web.config y los38 archivos del cierre administrativo siguen intactos. La continuación requiere revisar el fallo de acceso y aclarar si se usó la cuenta habitual de GESTOR, sin pedir credenciales por chat. Jira permanece En curso. Fuente:doc118-observador-preflight-20261110.json.
