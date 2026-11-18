# DOC-120 — tiempos reales de tres archivos del lote50

**PASS_REAL_THREE_FILES.** Run `23e18a403aae4dd4b455adc86ab787ad`, ejecutado por instrucción «ejecuta la prueba». Consola y prueba entre2026-11-17 21:00:07 y21:06:03, Bogotá; UTC2026-11-18 02:00:07–02:06:03. Ambiente `https://localhost/GestionDocumental-Docuarchi.net/`, propietario141/empresa2/nivel223, lote50/plan90/versión45, gabinete10 y expediente693 existentes. Política COMPLETO_OBLIGATORIO/EXPEDIENTE,1.000 originales.

## Mediciones observadas

Valores en segundos. HTTP mide la solicitud ALMACENAR completa; las otras columnas son intervalos del servidor. La captura conserva milisegundos y conteos completos en [JSON](doc120-medicion-tres-20261117.json) y [CSV](doc120-medicion-tres-20261117.csv).

| Archivo ID | Documento creado | HTTP total | Metadatos de originales | Autoridad de originales | Comprobación física de originales | Escritor habitual |
| --- | --- | --- | --- | --- | --- | --- |
|104|16264|93,177|64,011|9,827|13,801|0,152|
|105|16265|74,219|55,149|8,003|6,554|0,023|
|106|16266|70,900|53,142|7,772|5,567|0,020|

Promedio HTTP79,432s; mediana74,219s. Acumulado HTTP de las tres solicitudes238,296s; el tiempo completo de la consola también incluye entrada de credenciales, login y controles y no es tiempo de almacenamiento.

En cada documento se midieron **1.000 cargas de metadatos en la pasada previa y1.000 en la retenida**, además de1.000 comprobaciones físicas en cada pasada. Total de la muestra:6.000 cargas de metadatos y6.000 comprobaciones físicas. Las comprobaciones de permisos dentro de esas pasadas también fueron2.000 por documento. Los tamaños de los tres PDF objetivo fueron44.683,44.884 y44.884 bytes.

Respecto a238,246s acumulados del coordinador:

- Metadatos:172,302s, **72,321%**.
- Autoridad dentro de las pasadas:25,601s,10,746%.
- Comprobación física dentro de las pasadas:25,922s,10,880%.
- Las dos pasadas completas, incluyendo sus otros trabajos:230,048s, **96,559%**. Este porcentaje contiene los tres anteriores; no se suma a ellos.
- Escritor habitual:0,195s acumulados, **0,082%**.

Las métricas también separan original/copia/verificaciones V2, inicio/autorización/retorno de persistencia, comprobación posterior, checkpoint y lectura final. Son intervalos anidados: no sumar el adaptador con sus hijos como etapas independientes. Los nueve controles V2 del original objetivo por documento permanecieron activos.

## Diagnóstico respaldado por la prueba

El mayor costo observado está en **cargar y reconstruir repetidamente los metadatos de todos los originales para cada documento**. La medición de metadatos incluye conexión/transacción, consultas y reconstrucción; no demuestra que toda esa duración sea ejecución SQL en el motor. `ObtenerArchivo` vuelve a comprobar/cargar el lote y carga el archivo en cada llamada. Las2.000 llamadas por documento son un conteo real de esa frontera, no un conteo exacto de sentencias SQL.

El writer compartido fue rápido en esta muestra. La primera optimización a evaluar es agrupar la lectura de metadatos de cada pasada, manteniendo el alcance completo, revisiones, autorización y comprobaciones físicas. Eliminar revisiones físicas o cambiar a política parcial no está justificado por esta medición ni se aplicó aquí. Tres documentos son una muestra diagnóstica; no certifican capacidad de1.000 documentos ni un tiempo futuro garantizado.

## Controles y consumo

- Tres solicitudes ALMACENAR, una por archivo, confirmadas y comprobadas por SELECT en RA y por sus IDs conocidos/expediente/inventario/empresa/usuario en DA. Sin replay.
- Documentos completados: **7→10**,990 pendientes. Se conserva el expediente693. No se prepararon/confirmaron lotes, recibieron PDF, crearon expedientes, materializaron valores, reanudaron detenciones ni ejecutaron SQL correctivos.
- Huella de los997 archivos ajenos a la selección idéntica antes/después: `30895436f7705d81c4369e4dced64b689c58368452803db8ca52f3fdeb780820`. Incluye los siete resultados previamente completados.
- Ventana de medición:104 HTTP,14 SELECT de control,1 sesión,3 almacenamientos;134.451 bytes de originales objetivo procesados y0 nuevos PDF recibidos. Los14 SELECT no incluyen las consultas internas de la aplicación.
- Instalación técnica separada:1 HTTP anónimo401/SESION_REQUERIDA,0 SELECT/efectos. Total de este corte:105 HTTP,14 SELECT de control,1 sesión,3 almacenamientos.
- El ejecutor reportó cierre de contexto/navegador y eliminación de secretos; después no se observó proceso Node hijo de la consola, solo su conhost. La ventana queda cerrada y no autoriza repetir otros tres archivos.

## Versión y alcance de entrega

DLL/PDB instrumentadas instaladas con respaldo, por autorización actual; DLL SHA-256 `f667ea0ffe1b6f2ef111ac5afd52f50bab92c5edcd340a0421b9ea0931aef689`, comprobada antes de cada escritor y al terminar. Pareja anterior preservada en `.tmp/doc120-three-deploy/23e18a403aae4dd4b455adc86ab787ad/`, sin reciclar el pool compartido o modificar Web.config. La captura detallada depende de la cabecera optativa de cada solicitud; no se añade un paso a la interfaz.

8/8 pruebas locales previas y compilación aislada conservan su evidencia en [preparación](doc120-medicion-tres-preparacion-20261117.md). La ejecución actual completa3.169: **178/189 tareas**. Las once abiertas anteriores permanecen, incluida aceptación/capacidad integral y carrera/rollback MySQL. No se aplicó ninguna optimización, ni esta prueba sustituye esas verificaciones.
