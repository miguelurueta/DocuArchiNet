# DOC-125 — Confirmación transaccional y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: implementación y aceptación real5/5 verificadas.

## Evidencia requerida

La aceptación local vigente pasa94/94tests. El ejecutable existente reúne DOC122246, DOC123318, DOC124492 y DOC1253436:4492comprobaciones con ADO/autoridad sintéticos y libros/lector locales reales. Incluye modos, CAS, SQL estricto, parámetros, rollback,1062del tipo real de excepción del conector, CREATE identificado, incertidumbre, limpieza, marca56y sesiones sin caché FK compartida. La regresión heredada DOC110/DOC120 pasó. MSBuild net461:0errores,308advertencias previas y0en ImportacionExcel; DLL desplegada con respaldo/WSDL200. Fuentes y cortes en Validacion/doc125-ejecucion-seguimiento-20261124.md/JSON.

## QA/E2E WebForms

Un test @doc125-confirm con cinco casos reutiliza runner DOC110, suite importacion-excel, autenticación Web Forms/diagnóstico DOC111, controles SELECT DOC32 y actor externo DOC115. Se validan autorización DOC125, conteos, máximos, archivos/hashes, destinos descartables y DLL. Los libros se comprueban antes de las credenciales. SQL18–21 fueron verificados fuera de tests antes de los recorridos; el test no instala, siembra ni limpia.

CREAR/AGREGAR/REEMPLAZAR confirmaron1000filas cada uno. Se recuperaron esos IDs tras un timeout del protocolo sin repetir sus escrituras. Rollback300 por UNIQUE VALOR produjo FALLIDA/CONFLICTO_CLAVE y restauró íntegramente1000filas. La carrera10 produjo FALLIDA/CONFLICTO_CLAVE, cero filas propias y una sola fila externa conservada; el actor acreditó lock, INSERT del importador observado y commit externo. La última ventana pasó5/5 y136controles SELECT en427861ms. El ejecutor usa parámetros ODBC BigInt/Int64. Las continuaciones verifican identidad/estado/hashes/huella del destino y no repiten el DELETE acreditado. Se preservan625contratos y huellas completas de EGRESOS/CRÉDITOS/configuración previa. El acuse ignorado no equivale a corte físico de red ni COMMIT incierto real.

Se registraron Preparar/Confirmar y muestreo de procesos IIS compartidos, con cobertura temporal limitada.1000filas por modo son la capacidad física medida; no certifican20000. Se conservan límites20000/10000000bytes, SQL20s sólo para el barrido global, otrasSQL15s, HTTP30s y prueba15min por los múltiples controles SELECT. COLUMNA físico, cortes reales de COMMIT y DDL superviviente tras fallo tienen límites explícitos; su cobertura restante es local/sintética. Unit/e2e/manual_qa permanecen evidencias distintas.
