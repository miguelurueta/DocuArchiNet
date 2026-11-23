# DOC-125 — Confirmación transaccional y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: implementación local verificada, aceptación real abierta.

## Evidencia requerida

La aceptación local final pasa 27/27 tests. El ejecutable existente reúne DOC122246, DOC123318, DOC124492 y DOC1253292: 4348 comprobaciones con ADO/autoridad sintéticos y libros/lector locales reales. Incluye modos, CAS, SQL estricto, parámetros, rollback, 1062 real del tipo de excepción del conector, CREATE identificado, incertidumbre y limpieza. La regresión heredada DOC110/DOC120 pasó. MSBuild net461 aislado: cero errores, 308 advertencias previas y ninguna de ImportacionExcel; sin despliegue.

## QA/E2E WebForms

Se descubrió un test @doc125-confirm con cinco casos, sin abrir navegador o sesión autenticada. Reutiliza runner DOC110, suite importacion-excel, controles SELECT DOC32 y actor externo en el ejecutor DOC115. La autorización DOC125, conteos iniciales, máximos de inserciones/eliminaciones, archivos/hash, destino descartable y DLL se validan; no se hereda un scope de preparación. Los libros se comprueban antes de pedir credenciales.

La matriz prepara CREAR1000/AGREGAR1000/REEMPLAZAR1000, rollback300 por UNIQUE VALOR y carrera10 con una fila externa. Se exigirán dos sesiones reales, resultado durable/replay, acuse ignorado recuperado, locks propios observados activos/libres y huellas protegidas. El acuse ignorado no se presenta como corte físico de red o COMMIT incierto real. No se ejecutó aún la matriz; unit/e2e/manual_qa siguen siendo evidencias distintas. SQL19–21 instalan fixtures sólo mediante ejecución manual confirmada; el test no instala, siembra ni limpia. Las mediciones de capacidad de escritura siguen abiertas.
