# DOC-127 — Capacidad real XLSX de 5.000 filas

**3/3 casos reales PASS: AGREGAR, REEMPLAZAR y CREAR.** Conjunto `6ea858a269824b47930f29ca340277d1`, corte `96dfc85e3a78ecba9653eaaa7a6a55f0723b0d39`, finalizado 2026-11-26T16:01:42.798Z. Se completa 6.5; avance **27/33 tareas**.

## Resultados y mediciones

IIS local HTTPS/GESTOR, empresa 2, DA11/RA12, conexión 6 y asesor.comercial (194), con sólo permiso de cargas. Runner DOC110, suite Excel, controles SELECT DOC32 y monitor IIS existentes. Tres scopes nuevos, independientes y secuenciales. Una contraseña privada, mantenida en memoria.

| Modo | Plantilla | Filas previas → finales | Filas insertadas / borradas | Bytes | Preparar s | Confirmar s |
| --- | --- | --- | --- | --- | --- | --- |
| AGREGAR | 58 r1 | 2002 → 7002 | 5000 / 0 | 73872 | 0.716 | 11.828 |
| REEMPLAZAR | 59 r1 | 1000 → 5000 | 5000 / 1000 | 75297 | 0.685 | 12.555 |
| CREAR | 67 r1 | 0 → 5000 | 5000 / 0 | 73869 | 0.485 | 11.835 |

- AGREGAR: carga `f5d81f53-961f-445c-8ce1-7c7be6135db8`, destino `imp_02_doc126_e2e_agregar`.
- REEMPLAZAR: carga `4f5081dd-259a-469d-9239-2226431afcb7`, destino `imp_02_doc126_e2e_reemplazar`.
- CREAR: carga `58c15448-7ac9-4b2a-8ab1-8d92f72339f2`, destino `imp_02_doc127_e2e_crear_xlsx_5000`.

Cada carga quedó CONFIRMADA/5000, con propietario, empresa, conexión, plantilla y hash de archivo comprobados por SELECT. AGREGAR conservó las 2.002 filas anteriores. REEMPLAZAR sustituyó sus 1.000 filas de prueba por 5.000 nuevas; se comprobaron ID, archivo, estado persistido, conteo final, consulta y replay. CREAR generó únicamente el nuevo destino autorizado.

Los tiempos son HTTP por etapa e incluyen transporte, metadatos, validación y escritura. La duración de la ventana también incluye navegación y controles íntegros de tablas; no es tiempo SQL de importación.

## Memoria y conservación

| Modo | RSS inicial MB decimales | RSS máximo observado MB | Privados máximos MB | Muestras | Mayor hueco ms |
| --- | --- | --- | --- | --- | --- |
| AGREGAR | 0.00 | 220.50 | 255.32 | 264 | 2058 |
| REEMPLAZAR | 212.91 | 214.10 | 260.08 | 264 | 2056 |
| CREAR | 138.93 | 145.04 | 265.85 | 264 | 2053 |

Muestreo solicitado cada 2 segundos, con baseline, intervalo observado, fases y cobertura registrados en el JSON. RSS y memoria privada agregan procesos IIS compartidos; no representan asignación exclusiva de la carga ni garantizan capturar picos. Un baseline sin workers representa esa ausencia observable.

Las 35/35/36 tablas protegidas por caso, cabeceras/campos y perfiles conservaron sus huellas completas. El control final observó 78 perfiles, permisos de194 intactos y tres plantillas sin cambios. Controles SELECT internos: AGREGAR=311, REEMPLAZAR=311, CREAR=316. Postflight adicional: 8 SELECT directos y 6 del lector de plantillas; cero SQL modificador de control.

Efectos funcionales: 15.000 inserciones, 1.000 borrados, aumento neto de 14.000 filas y una tabla nueva. Cero escrituras de plantilla/permisos, instalación, limpieza o despliegues. Los hashes de runtime, soporte y scripts privados coinciden con el alcance autorizado.

## Captura y límites

La salida saneada conserva los tres marcadores de test aprobado y el cierre PASS del lote. La sesión privada finalizó FINISHED/código0, sin cierre forzado. El proceso terminó y liberó su memoria; el script incluye limpieza de secretos en finally. Claims, resultados originales, logs y muestras permanecen excluidos de Git.

Esta evidencia completa XLSX5000, junto con las nueve combinaciones XLS y las tres XLSX1000 ya acreditadas. Continúan pendientes XLSX10000, alcances funcionales restantes, EXPLAIN29 manual y gates finales. Los tres scopes están consumidos; no repetir estas cargas para obtener otra medición.
