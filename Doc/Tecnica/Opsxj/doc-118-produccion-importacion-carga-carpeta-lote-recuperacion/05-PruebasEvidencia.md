# DOC-118 — coordinación y recuperación

## Regresión local tras RV-03

223/223 Node:18 comportamiento/cliente y205 herramientas. El harness incorpora20 casos del adaptador y conserva19 de contratos y21 del proveedor. Prueba retención anterior al inicio, respuesta perdida/alterada, una invocación, IDs parciales, concurrencia y cancelación/ThreadAbort propagados. Persistencia y escritor son dobles explícitos; no acredita MySQL o legacy real. Progreso20/30. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-adaptador-v2-local-20261108.json`.

## Regresión anterior tras RV-02-B

223/223 Node actuales:18 comportamiento/cliente y205 herramientas. El harness añade21 casos del proveedor real y conserva19 casos RV-01, incluidos en el contador del harness.3 procesos sintéticos cerrados, exclusión frente a lectores de otro proceso, identidad, enlaces y Dispose comprobados. La producción, otro nodo/reinicio y recuperación siguen sin acreditar. Progreso19/29. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-puerto-staging-v2-local-20261108.json`.

## Regresión anterior tras RV-01

18/18 pruebas Node del harness/cliente y205/205 de herramientas,223 disjuntas actuales. El harness compila y ejecuta19 casos nuevos V2; no se suman al contador Node. Incluyen esquema cerrado, vínculos/fases, compatibilidad V1, privacidad, datos incompletos y ausencia de efectos/capacidad. No hubo nuevas sesiones, HTTP de negocio o SELECT. Código interno sin conexión a recuperación. Progreso18/28. Fuente:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-contratos-v2-local-20261108.json`.

## Regresión anterior tras RV-02-A

205/205 pruebas en16 archivos. El probe nuevo aporta13 aserciones sobre NTFS local con6 procesos independientes y limpieza confirmada. No usa el staging real, sesiones, HTTP, SELECT o datos de negocio. Puerto del proveedor y recuperación V2 siguen pendientes; la prueba no habilita69 ni acredita identidad entre nodos. Fuente:Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-staging-lock-probe-20261108.json.

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: ejecutor de siete fases verificado localmente; disponibilidad/cierre de observador PASS, retención SIN_CALIFICAR. Progreso16/27.

## Evidencia requerida

unit: el 2026-11-07 se ejecutó node --test tests/doc110-perfil-importacion-behavior.test.cjs; 2/2 pruebas Node, cero fallos, VB/.NET Framework y DDL locales sin HTTP ni conexión. No se suman grupos internos C# al contador Node.

manual_qa: no se ejecutó QA real nuevo para DOC-118. La matriz Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-matriz-coordinacion.md distingue REC reales históricos, dobles actuales y las integraciones COORD-02/03 NO_EJECUTADAS. source-review.json registra hashes de fuentes y reportes y la base de revisión. Los resultados previos no se trasladan a los binarios actuales.

## QA/E2E WebForms

Antes de ejecutar: resolver observación compatible dentro de llamada legacy real, documentar ambiente/cuentas/recursos nuevos, puntos de fallo, límites y corte, leer AGENTS.md y el runbook, y obtener autorización específica. No se concede por este documento. Intentos41/lote18 y53/lote21 permanecen intactos. No repetir cargas acreditadas ni efectos inciertos. La revisión editorial, pruebas locales y validación OpenSpec no completan las brechas reales ni autorizan su ejecución.


## Revalidación de preparación DOC-118

El 2026-11-07 se ejecutó `node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc111-importacion-carpetas-client.test.cjs`: 18/18 pruebas Node, cero fallos. Incluye compilación/reglas/DDL y cliente Chromium local con servicios sintéticos, sin login ni HTTP real. Se verificaron nuevamente 14 huellas de fuentes y 10 reportes históricos.

Protocolo y dependencia real: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-plan-integracion.md`. El mapa de seguridad/recuperación está en `doc118-seguridad-recuperacion.md` de la misma carpeta. La aprobación editorial y refinement PASS no acreditan observador calificado ni COORD-02/03 real.


El helper nuevo se verificó con `node --test tests/doc118-active-call.test.cjs`: 16/16 pruebas, cero fallos. Verifica rechazo de observación no correlacionada, hilo cambiado, primera petición ya terminada, timeout, respuesta perdida y fallo de liberación sin reenvíos ni exposición de errores privados. Sumadas a las 18 anteriores son 34 pruebas locales en dos ejecuciones disjuntas; no acreditan el depurador real ni efectos documentales nuevos.


## Regresión vigente y resultado real

El preflight de dos sesiones pasó con publicación38/gabinete10 y17 SELECT. Consumo acumulado697 HTTP C02,0 C03 y5 aperturas;0 fases documentales y0 efectos. El worker26244 dejó de estar activo por causa no determinada. La consola terminó sin reconocer la orden CLOSE ni el finally; no quedan consola/shell ni hijos directos detectados. Las instancias privadas de depuración ya no están presentes y los dos IDE originales quedaron sin depuración activa, con12 breakpoints del usuario conservados. La observación interna sigue SIN_CALIFICAR. Evidencia: doc118-authenticated-preflight-20261107.json.

130 pruebas locales:12 preflight,31 enlace,16 coordinación,63 drivers,4 símbolos y4 adaptador externo. Más18 previas disjuntas:148, sin sumar repeticiones. El C# compila y hubo source binding real, pero no Freeze/Thaw ni ARM. La limpieza de una instancia y el finally de la consola no tuvieron reconocimiento; conservar ambas limitaciones y su inspección posterior.


## Ejecutor local y prueba SDK de disponibilidad

Las siete fases del ejecutor están implementadas y verificadas localmente. El observador real pasó disponibilidad, breakpoint de fuente, INSPECT y CLOSE con ack de limpieza y salida0 en sesión supervisada. No se ejecutaron ARM/Freeze/Thaw, fases documentales ni nuevas sesiones autenticadas. Consumo acumulado699 HTTP C02,0 C03,17 SELECT y5 aperturas;0 aperturas disponibles. Progreso16/27: COORD02/03 siguen NO_EJECUTADOS para aceptación.

160/160 locales y178 pruebas disjuntas contando18 previas; sin sumar repeticiones. doc118-observer-readiness-20261108.json conserva HEAD699 acumulado, fuente resuelta e INSPECT/CLOSE con ack. La regresión PowerShell incluye el ArgumentException reproducido del reemplazo atómico. No se ejecutaron las ventanas documentales.


## Corte posterior a la preparación del lote24

La regresión vigente pasa177/177. El corte real acumulado es995 HTTP/40 SELECT/7 aperturas, con dos expedientes y0 solicitudes ALMACENAR; limpieza confirmada. ARM/RESUME/CLOSE pasó sin Freeze ni escritor. COORD02/03 no están aceptados; progreso16/27. Propuesta de9 aperturas/1500 HTTP pendiente. Véase `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-preparacion-recuperacion-20261108.md`.


## Ventana real y auditoría posterior

RUN02 se detuvo con fallo del depurador. Lote24/archivo40/intento69 permanece EN_CURSO, con reclamación activa y sin resultado durable; archivo41 mantiene NO_INICIADO y ausencia física verificada. No se puede afirmar cero efectos de A ni repetirlo. Acumulado1276 HTTP/95 SELECT/9 aperturas; C03 no iniciado. La candidata local sincroniza Break/Thaw/Go y conserva historial propio. Regresión187/187, SDK compilado, sin prueba documental posterior de esa corrección. Progreso16/27. Detalle:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-ventana-activa-auditoria-20261108.md`.


## Ausencia física y prueba sintética de liberación

La auditoría encontró0 registros físicos de A/B y mantuvo intacto el intento69 EN_CURSO; no se permite replay. La contención4.6 queda acreditada:17/27. La mecánica SDK Freeze/peer/Break/Thaw/Go pasó en un proceso propio con PDB y fuente sintéticos, sin IIS ni efectos documentales; legacy sigue sin calificar. Regresión192/192 y2/2 comportamiento existentes. Acumulado1276 HTTP/113 SELECT/9 aperturas. La evaluación de un diseño de recuperación EN_CURSO está propuesta, sin implementación o mutación autorizadas. Véase `doc118-auditoria-fisica-liberacion-20261108.md`.
