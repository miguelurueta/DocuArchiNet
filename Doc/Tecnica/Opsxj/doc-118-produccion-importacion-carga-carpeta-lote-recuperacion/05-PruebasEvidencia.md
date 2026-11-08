# DOC-118 — coordinación y recuperación

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
