# DOC-118 — resultado del preflight autorizado

La autorización del plan real y la redistribución425/75 están concedidas en execution-authorization.json y execution-continuation-proposal.json. La continuación se detuvo al agotar C02; esa aprobación no autoriza repetir otra pareja ni ampliar topes automáticamente.

## Resultado observado

El primer intento fue rechazado explícitamente por credenciales; quedó registrado en doc118-login-preflight-20261107.json con98 HTTP y1 apertura acumulados, incluido el HEAD inicial. Una activación local anterior falló antes de autenticación, sin HTTP adicionales.

Tras la autorización de redistribución, la nueva captura permitió autenticar A, seleccionar nivel7 y consultar lote23 por el driver readOnly. La apertura de B alcanzó425 HTTP acumulados de C02 y no terminó; no se observó rechazo de sus credenciales. El código genérico de consola se conserva separado de la clasificación de presupuesto en doc118-continuation-preflight-20261107.json. No se ejecutaron controles SELECT ni fases documentales.

El guard cerró contextos/navegador y borró secretos. No se conserva sesión para reusar. Las3 aperturas/intentos son asignaciones antes de openSession, no una afirmación de tres postbacks de autenticación confirmados.

## Presupuesto acumulado vigente

| Concepto | Consumido | Disponible con autorización actual |
| --- | --- | --- |
| HTTP C02 |425 de425, incluida1 consulta de negocio |0 |
| HTTP C03 |0 de75 |75 |
| HTTP global |425 de500 |75 |
| Aperturas/intentos |3 de4 |1 |
| SELECT de control |0 de240 |240 |
| Planes/lotes/raíces/PDFs/expedientes/documentos nuevos |0 | Límites originales |
| Detenciones, attach, breakpoints y congelaciones |0 | Sin ejecutar |

## Corrección local y límites

El preflight por API conserva el login compartido y el host/token emitidos en el HTML del servidor. Omite subrecursos de la pantalla de producción y la selección visual del árbol. RenderImportacionCarpetas emite el token a partir de Resolver; VinculoContexto lo vincula a sesión/usuario/empresa y ConsultarLote resuelve el nivel desde el lote bajo autoridad vigente. Esto no elude el guard ni acredita QA visual. Recursos omitidos antes de despacho no consumen HTTP; los425 anteriores siguen íntegros. El ahorro real aún no se midió.

120/120 pruebas locales pasan:11 preflight,30 enlace,16 coordinación y63 drivers. Se verifican arrastre inmutable, ledger discordante, capacidad insuficiente, sesiones cerradas y recursos omitidos. El arranque actual rechaza nuevos logins porque quedó STOPPED_BUDGET_REVIEW_REQUIRED. La propuesta execution-budget-continuation-proposal.json plantea5 aperturas/1000 HTTP acumulados (925/75), conservando425 HTTP/3 aperturas consumidos. Sigue sin autorización.

## Estado de aceptación

Progreso16/27. COORD02/03 permanecen NO_EJECUTADOS para aceptación específica. La CLI solo implementa PREFLIGHT; sus otras fases y el observador real siguen pendientes. La DLL/PDB ordinarios y el IDE fueron localizados, pero no hay attach ni correspondencia de símbolos cargados acreditada. DefaultAppPool es compartido por6 aplicaciones. No se cambia almacenamiento compartido, SII, binario, permisos ni IIS. Los intentos41/53 y lotes18/21 no se mutaron.
