# DOC-118 — coordinación y recuperación

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: refinamiento aprobado y fase real autorizada, preflight inicial sin autenticación; sin despliegue ni efectos documentales nuevos.

## Evidencia requerida

unit: el 2026-11-07 se ejecutó node --test tests/doc110-perfil-importacion-behavior.test.cjs; 2/2 pruebas Node, cero fallos, VB/.NET Framework y DDL locales sin HTTP ni conexión. No se suman grupos internos C# al contador Node.

manual_qa: no se ejecutó QA real nuevo para DOC-118. La matriz Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-matriz-coordinacion.md distingue REC reales históricos, dobles actuales y las integraciones COORD-02/03 NO_EJECUTADAS. source-review.json registra hashes de fuentes y reportes y la base de revisión. Los resultados previos no se trasladan a los binarios actuales.

## QA/E2E WebForms

Antes de ejecutar: resolver observación compatible dentro de llamada legacy real, documentar ambiente/cuentas/recursos nuevos, puntos de fallo, límites y corte, leer AGENTS.md y el runbook, y obtener autorización específica. No se concede por este documento. Intentos41/lote18 y53/lote21 permanecen intactos. No repetir cargas acreditadas ni efectos inciertos. La revisión editorial, pruebas locales y validación OpenSpec no completan las brechas reales ni autorizan su ejecución.


## Revalidación de preparación DOC-118

El 2026-11-07 se ejecutó `node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc111-importacion-carpetas-client.test.cjs`: 18/18 pruebas Node, cero fallos. Incluye compilación/reglas/DDL y cliente Chromium local con servicios sintéticos, sin login ni HTTP real. Se verificaron nuevamente 14 huellas de fuentes y 10 reportes históricos.

Protocolo y dependencia real: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-plan-integracion.md`. El mapa de seguridad/recuperación está en `doc118-seguridad-recuperacion.md` de la misma carpeta. La aprobación editorial y refinement PASS no acreditan observador calificado ni COORD-02/03 real.


El helper nuevo se verificó con `node --test tests/doc118-active-call.test.cjs`: 16/16 pruebas, cero fallos. Verifica rechazo de observación no correlacionada, hilo cambiado, primera petición ya terminada, timeout, respuesta perdida y fallo de liberación sin reenvíos ni exposición de errores privados. Sumadas a las 18 anteriores son 34 pruebas locales en dos ejecuciones disjuntas; no acreditan el depurador real ni efectos documentales nuevos.


## Regresión del enlace de sesiones

`node --test tests/doc118-session-bridge.test.cjs tests/doc118-active-call.test.cjs tools/e2e/tests/doc111-importacion-carpetas-driver.test.cjs`: 102/102 pruebas, cero fallos (23 enlace,16 coordinación,63 drivers compartidos). Estas corridas reemplazan las repeticiones de las mismas pruebas al contabilizar cobertura; no se suman varias veces. La comprobación readOnly incluye el rechazo de SolicitarDetencion antes de HTTP. Se usaron dobles de sesión/HTTP/SQL y observador; cero login, SELECT o attach reales.


## Preflight real autorizado

`doc118-preflight-20261107.json` y `doc118-resultados-integracion.md` registran un HEAD HTTP200 y localización de DLL/PDB/IDE, sin attach, login, SELECT ni efectos. La autorización ya está concedida; falta captura enmascarada de secretos efímeros y calificar el observador. COORD02/03 específicos siguen NO_EJECUTADOS. No se suma esta petición como prueba Node ni como aceptación documental.


## Captura y preflight por fases

Regresión conjunta106/106 (4 preflight,23 enlace,16 coordinación,63 drivers), cero fallos. El resultado real independiente fue DOC111_LOGIN_CREDENTIALS_REJECTED:98 HTTP acumulados,1 intento,0 SELECT de control y0 efectos. La evidencia saneada está en doc118-login-preflight-20261107.json; no registra datos introducidos ni atribuye el rechazo a un campo específico. Se conserva la parada sin retry y la propuesta de continuación sin autorización.
