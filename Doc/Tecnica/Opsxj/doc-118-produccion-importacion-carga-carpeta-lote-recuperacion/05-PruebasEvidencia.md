# DOC-118 — coordinación y recuperación

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: refinamiento aprobado; continuación detenida por tope HTTP,16/27 tareas. Primera sesión autenticada, sin pareja completa ni efectos documentales.

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

La regresión conjunta actual pasó120/120 (11 preflight,30 enlace,16 coordinación,63 drivers); se reemplazan las ejecuciones repetidas al contabilizar cobertura. Con las18 previas disjuntas de servidor/cliente son138 pruebas locales, no aceptación real. Los guards prueban CSRF, presupuesto acumulado, cierre, entrada segura y omisión optativa de subrecursos de producción conservando el login. No se ejecutó nuevamente la prueba del servidor por este cambio de herramientas.

doc118-continuation-preflight-20261107.json registra A autenticada/lote23 consultado, B sin confirmar al agotar425 HTTP C02 y3 aperturas acumuladas. C03=0, SELECT de control=0, efectos=0. Sesiones cerradas y secretos borrados. El rechazo anterior conserva su evidencia independiente. La autorización de redistribución sí fue concedida; la ampliación1000 HTTP/5 aperturas sigue propuesta. OpenSpec/refinement y las pruebas locales no completan COORD02/03.
