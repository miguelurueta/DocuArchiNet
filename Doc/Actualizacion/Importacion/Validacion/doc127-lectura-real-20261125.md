# DOC-127 — Lectura real e integración WebForms

**REAL_READ_AND_WEBFORMS_ISOLATION_PASS.** Una E2E Playwright real,1/1 PASS, sin reintentos. Corte `986f0f1cc9974dd81343e3f9227a264d53bba13c`, runtime y assets doc127-1 verificados. Ventana autorizada por el usuario mediante «autorizado» para la propuesta de lectura194/196. Estado final 2026-11-25T23:28:09.9136967Z.

## Alcance ejecutado

IIS local HTTPS/GESTOR, empresa2, DA11/RA12, conexión6 y plantilla60, destino existente imp_02_doc126_e2e_asistente_c38a2522. Dos sesiones: oficial.cumplimiento196 sin permiso de cargas y asesor.comercial194 con permiso exclusivo de cargas. Se reutilizaron runner DOC110, suite Excel, scopes DOC126, login compartido y controles SELECT DOC32.

La cuenta restringida no mostró el menú y AbrirCargaExcel respondió ACCESO_DENEGADO. La cuenta exclusiva abrió, buscó y consultó la plantilla, vio su detalle y cerró con Escape. La guarda compartida acreditó en el DOM real, antes de abrir y después de cerrar, que los controles Excel pertenecen al formulario auxiliar fuera de form1 y que la validación propia del gestor sigue activa. No se envió un postback del gestor.

Los tres assets servidos por HTTP coincidieron con sus hashes autorizados. La suite bloqueó operaciones de escritura y comparó íntegramente las32 tablas protegidas y la configuración de cabeceras/campos antes/después:PASS. Sus huellas completas se compararon en memoria; se conserva el resumen y hash del log, sin filas ni hashes individuales publicados.

## Conservación y diferencia previa de perfiles

Antes del login,11 SELECT verificaron permisos y recursos. El control heredado se detuvo tras2 SELECT porque la huella histórica de perfiles no coincidía. Un diagnóstico posterior de2 SELECT confirmó78 perfiles y una huella distinta; no determina la causa ni demuestra un cambio durante esta ventana. Se fijó una línea base fresca mediante4 SELECT y se exigió igualdad al terminar. No se modificaron o repararon permisos.

Los4 SELECT posteriores acreditaron78 perfiles idénticos a la línea base fresca y los cinco resultados anteriores sin cambios. CREAR1000, AGREGAR1002 y REEMPLAZAR1000 filas conservadas. Huella de perfiles antes/después: `bd5953665dfc848bef3f2064c82e3b575a83f20ef646c4952521d129ca8e7447`; histórica previa: `4e9135a2218b77ef6187df171cbad64952f6c393cc49ddc38135fd66d776b091`. La diferencia anterior se conserva expresamente y no se anuncia igualdad con ese corte histórico.

Los23 SELECT de pre/postflight se contabilizan aparte de los controles SELECT internos de la suite, cuyo total no se instrumentó. Controles SQL exclusivamente SELECT; cero archivos recibidos, plantillas guardadas, preparaciones, confirmaciones, filas insertadas/borradas, cambios de permisos o despliegues. DLL, página, Web.config y assets mantienen sus huellas autorizadas.

## Resultado y límites

**22/33 tareas.** Se acredita la lectura e integración doc127-1 que estaba pendiente.5.1 conserva pendientes los restantes alcances de autoridad/preparación y7.1 el corte integral. Esta ventana no autoriza capacidad, nuevas cargas, revocación física ni repetición automática; las18 combinaciones y EXPLAIN29 mantienen sus dependencias. No se archiva ni cierra Jira.

Credenciales introducidas en consola privada y memoria, sin impresión ni persistencia; contextos cerrados y variables de secretos limpiadas por finally. Capturas, trazas y video desactivados. Datos de ejecución, manifiesto, baseline, controles y salida saneada permanecen en .tmp/, excluidos de Git. El JSON adjunto registra autorización, SHA, hashes, controles y límites.
