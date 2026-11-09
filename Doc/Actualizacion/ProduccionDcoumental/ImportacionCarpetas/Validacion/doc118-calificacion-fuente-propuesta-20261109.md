# DOC-118 — comprobación propuesta del enlace de fuente

**Propuesta P4-B sin autorizar ni ejecutar.** Apoya la tarea 4.1/D-10/RQ-10; no agrega tareas. La DLL/PDB ordinaria ya está instalada. La lectura de módulos del worker no mostró el módulo objetivo: no demuestra que el CLR no lo haya cargado y no autoriza cambiar el pin histórico del runner.

Se propone una única petición **HEAD** sin autenticación a `https://localhost/GestionDocumental-Docuarchi.net/gestor.aspx`, timeout 10 s, maxRetries=0, maxRedirects=0 y excepción TLS limitada al contexto local. Reutiliza el transporte API Playwright del repo; no abre navegador, carga assets, sigue Location, imprime cabeceras/cookies o lee cuerpos de negocio. Solo 200/302 permiten avanzar. El presupuesto es 1 HTTP nuevo, 0 login, 0 SELECT y 0 efectos documentales; no reutiliza las 9 aperturas históricas ni concede ejecutar Q/COORD.

Después se resuelve el único worker actual de `DefaultAppPool` y se usa el controlador SDK existente en una IDE privada/oculta: **Initialize→Inspect→Dispose**. Initialize crea su punto de fuente con condición `False`; no se envía ARM, no hay pausa intencional/Freeze ni llamada de almacenamiento. Se comprueban fuente enlazada, worker vivo, otros hilos en ejecución, liberado y cero entradas/fallo. Finally retira solo el breakpoint propio, desconecta y cierra su IDE. No se recicla el pool, termina un worker o modifica almacenamiento/SII, archivos instalados o configuración.

El pool comparte 6 aplicaciones. Adjuntar el depurador puede afectarlas transitoriamente: esa conexión requiere aprobación expresa del operador, además de la petición HEAD y TLS local. Objetivo total 90 s comprobado entre fases; attach/bind del controlador tienen esperas de 10 s. No se interrumpe a la fuerza una llamada COM para cumplir el objetivo ni se mata el proceso si el cierre no se confirma; se registra para revisión y no se continúa.

Alcance/hashes completos: `v2-runtime-smoke-proposal.json`. Comprobación local sin red/attach:

`powershell.exe -NoProfile -STA -File tools/e2e/scripts/run-doc118-runtime-smoke.ps1 -CheckOnly`

La ejecución futura usa el mismo comando con `-Execute` y exige el manifiesto dedicado `v2-runtime-smoke-authorization.json` con aprobación y hash de esta propuesta. El helper HEAD también exige esa aprobación. Los helpers están versionados en tools/e2e/scripts y fijados por SHA256; reutilizan el controlador existente sin modificarlo. No se ejecuta ahora el comando real.

Resultado máximo: `SOURCE_BINDING_SMOKE_PASS_NO_ACTIVE_CALL` con cierre confirmado. Es evidencia del enlace de fuente del SDK, no prueba de identidad de bytes cargados, invocación activa, MySQL o coordinación. No completa 4.1 ni declara runtimeQualified/integrationPass. El runner histórico conserva su pin; tras revisar esta evidencia se decidirá el binding y la siguiente calificación con recursos/topes completos. 28/38, mismas 10 tareas pendientes.

El helper HEAD consume una capacidad exclusiva bajo el directorio de resultado antes del transporte; una segunda invocación se rechaza. Un fallo de arranque posterior conserva la capacidad consumida para revisión, sin retry ni reinicio del contador.
