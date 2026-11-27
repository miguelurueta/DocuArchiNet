# DOC-127 — Capacidad real XLSX de 1.000 filas

**3/3 casos reales PASS: AGREGAR, REEMPLAZAR y CREAR.** Conjunto `4ae84cb89ebd4d77a3a962a0ef623106`, corte `a4bb16a917f2ec2a6fd16f24583058a794e4f6ac`, finalizado 2026-11-26T15:18:51.884Z. Se completa 6.4; avance **26/33 tareas**.

## Resultados y mediciones

IIS local HTTPS/GESTOR, empresa 2, DA11/RA12, conexión 6 y asesor.comercial (194), con sólo permiso de cargas. Runner DOC110, suite Excel, controles SELECT DOC32 y monitor IIS existentes. Tres scopes nuevos, independientes y secuenciales. Una contraseña privada, mantenida en memoria.

| Modo | Plantilla | Filas previas → finales | Filas insertadas / borradas | Bytes | Preparar s | Confirmar s |
| --- | --- | --- | --- | --- | --- | --- |
| AGREGAR | 58 r1 | 1002 → 2002 | 1000 / 0 | 16359 | 0.272 | 11.261 |
| REEMPLAZAR | 59 r1 | 1000 → 1000 | 1000 / 1000 | 16632 | 0.194 | 10.870 |
| CREAR | 66 r1 | 0 → 1000 | 1000 / 0 | 16356 | 0.164 | 11.573 |

- AGREGAR: carga `db73c0b6-3885-4365-b19a-e1d2d27dafa0`, destino `imp_02_doc126_e2e_agregar`.
- REEMPLAZAR: carga `50dc01ef-40e2-42f5-8a94-c06c79d38c8c`, destino `imp_02_doc126_e2e_reemplazar`.
- CREAR: carga `801a4f3c-683f-4042-8f00-03d90a163bef`, destino `imp_02_doc127_e2e_crear_xlsx_1000`.

Cada carga quedó CONFIRMADA/1000, con propietario, empresa, conexión, plantilla y hash de archivo comprobados por SELECT. AGREGAR conservó las 1.002 filas anteriores. REEMPLAZAR sustituyó sus 1.000 filas de prueba por otras 1.000: el mismo conteo final se acompaña de ID nuevo, archivo, estado persistido, consulta y replay verificados. CREAR generó únicamente el nuevo destino autorizado.

Los tiempos son HTTP por etapa e incluyen transporte, metadatos, validación y escritura. La duración de la ventana también incluye navegación y controles íntegros de tablas; no es tiempo SQL de importación.

## Memoria y conservación

| Modo | RSS inicial MB decimales | RSS máximo observado MB | Privados máximos MB | Muestras | Mayor hueco ms |
| --- | --- | --- | --- | --- | --- |
| AGREGAR | 0.00 | 202.17 | 237.97 | 256 | 2059 |
| REEMPLAZAR | 197.90 | 205.72 | 241.47 | 255 | 2061 |
| CREAR | 172.18 | 178.65 | 254.53 | 260 | 2067 |

Muestreo solicitado cada 2 segundos, con baseline, intervalo observado, fases y cobertura registrados en el JSON. RSS y memoria privada agregan procesos IIS compartidos; no representan asignación exclusiva de la carga ni garantizan capturar picos. Un baseline sin workers representa esa ausencia observable.

Las 34/34/35 tablas protegidas por caso, cabeceras/campos y perfiles conservaron sus huellas completas. El control final observó 78 perfiles, permisos de194 intactos y tres plantillas sin cambios. Controles SELECT internos: AGREGAR=303, REEMPLAZAR=303, CREAR=308. Postflight adicional: 8 SELECT directos y 6 del lector de plantillas; cero SQL modificador de control.

Efectos funcionales: 3.000 inserciones, 1.000 borrados, aumento neto de 2.000 filas y una tabla nueva. Cero escrituras de plantilla/permisos, instalación, limpieza o despliegues. Los hashes de runtime, soporte y scripts privados coinciden con el alcance autorizado.

## Captura y límites

La salida saneada conserva los tres marcadores de test aprobado y el cierre PASS del lote. La sesión privada finalizó FINISHED/código0, sin cierre forzado. El proceso terminó y liberó su memoria; el script incluye limpieza de secretos en finally. Claims, resultados originales, logs y muestras permanecen excluidos de Git.

Esta evidencia completa el corte actual XLSX1000, conservando por separado su historia previa. Las nueve combinaciones XLS ya acreditadas permanecen vigentes. Continúan pendientes XLSX5000/10000, alcances funcionales restantes, EXPLAIN29 manual y gates finales. Los tres scopes están consumidos; no repetir estas cargas para obtener otra medición.
