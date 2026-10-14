# DOC-102 — pruebas y evidencia

- Ticket: DOC-102
- Cambio OpenSpec: doc-102-streamer-http-rangos
- Clasificacion: cross_cutting

Fecha local: 2026-10-14.

## Evidencia requerida

### Resultados automatizados

- `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-streaming:policy`: PASS, 11/11. Compila y ejecuta codec, rangos, condiciones y escritor sobre .NET Framework; valida integración JS, fronteras legacy y aislamiento de la prueba de linearización.
- `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-performance:policy`: PASS, 38/38. Incluye carga diferida previa, arranque, sanitización, contrato DOC-102 y política del experimento temporal.
- `MSBuild.exe GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m /nologo /v:minimal /clp:ErrorsOnly`: PASS, código 0; genera `GestionDocumental-Docuarchi.net.dll`.

La compilación completa conserva advertencias históricas de ensamblados/código legacy; no se observó una advertencia atribuible a los archivos DOC-102.

## QA/E2E WebForms

### Primera estrategia Range: criterio de carga parcial no cumplido

Se ejecutaron tres mediciones autenticadas autorizadas y saneadas. Las dos primeras sirvieron para endurecer la medición: la inicial mezcló un `200` completo con rangos y la segunda todavía transfirió el archivo completo. La corrida final terminó con código 2 y `WORKFLOW_PDF_PERF_RESULT=FAILED`; no se presenta como evidencia aprobatoria.

La corrida final confirmó:

- selección, primer canvas en las tres iteraciones, scroll y zoom funcionales;
- mediana de primer canvas de 1.123 ms, 34 páginas y una sola tarea larga;
- `disableAutoFetch=true` en las tres iteraciones;
- contrato HTTP correcto: `Accept-Ranges`, `Content-Range`, `ETag`, `Last-Modified`, validadores, caché privada y `Vary: Cookie`;
- un `200` y 61 respuestas `206` por iteración, sin fallos de red;
- 33.608.355 bytes transferidos y 33.589.755 bytes codificados por iteración para un documento de 33.589.755 bytes;
- `partialFirstCanvas=false` en las tres iteraciones.

Solo se había renderizado un canvas al capturar cada muestra, por lo que la transferencia completa no provino del scroll ni del planificador visual. La evidencia es compatible con un PDF cuya estructura obliga a PDF.js a resolver objetos distribuidos por todo el archivo para producir la primera página. El streamer, los rangos y la desactivación de precarga están operativos, pero el criterio de bytes parciales antes del primer canvas no se cumple con el documento aprobado. Garantizarlo para este archivo requeriría optimizar o linearizar el PDF, lo cual está fuera del alcance actual.

### Estrategia adaptativa posterior

Sin cambiar el almacenamiento, se añadió selección de transporte. Los PDF grandes que declaran `/Linearized` al inicio conservan Range; los PDF no linearizados, pequeños o cuya inspección falla usan una única respuesta `200` transmitida por bloques. El resultado anterior de 1.123 ms y 61 rangos por iteración queda como línea base.

El primer reintento autorizado se detuvo antes de seleccionar tarea con `WORKFLOW_PDF_PERF_TASK_LIST_PERMISSION_DENIED`. El runner no abrió el documento ni reemplazó la evidencia anterior. Este resultado indica que la cuenta usada no obtuvo permiso para listar tareas en el módulo configurado; no demuestra un defecto del transporte y no se modificaron permisos, sesión, tarea ni configuración para sortearlo.

La repetición autorizada posterior terminó `PASSED` con evidencia esquema 4 capturada el 2026-10-14:

- modo `stream` en las tres iteraciones;
- una única respuesta `200` y cero solicitudes Range por iteración;
- primer canvas en las tres iteraciones, mediana de 624 ms para 34 páginas;
- scroll y zoom completados, una tarea larga agregada y cero fallos clasificados;
- política de transporte aprobada en las tres muestras;
- 33.589.755 bytes codificados por iteración, transmitidos secuencialmente mediante el buffer fijo del servidor.

Frente a la línea base Range de 1.123 ms, la mediana disminuyó 499 ms (44,4 %) y se eliminaron 61 solicitudes parciales por iteración. No se afirma ahorro de bytes para este PDF no linearizado; la mejora comprobada es menor latencia y sobrecarga HTTP, sin materialización completa en memoria del servidor o del JavaScript del visor.

No se ejecutó carga ni escritura documental. Cualquier control de persistencia debe ser exclusivamente `SELECT`. La evidencia no persiste credenciales, cookies, tokens, URL, ruta ni nombre documental.

### Prueba local de linearización sin persistencia

Comando: `npm.cmd --prefix tools/e2e run test:doc102:linearization-poc`.

Resultado `PASSED` sobre un fixture público repetido hasta 168 páginas, bajo un perfil local controlado aproximado de 26,21 Mbps:

| Métrica hasta primer canvas | No linearizado | Linearizado |
|---|---:|---:|
| Tamaño total | 1.092.272 bytes | 1.153.756 bytes |
| Bytes únicos consultados | 305.840 (28 %) | 105.180 (9,12 %) |
| Bytes físicos escritos | 633.520 | 695.004 |
| Solicitudes totales / Range | 4 / 3 | 4 / 3 |
| Tiempo | 488 ms | 536 ms |
| Páginas | 168 | 168 |

La huella renderizada de la primera página fue equivalente, el fixture mantuvo hash y fecha, y el directorio temporal fue eliminado. La linearización redujo 65,6 % la cobertura única requerida, pero no redujo bytes físicos ni tiempo en este fixture pequeño debido a solicitudes solapadas. No se presenta como garantía de rendimiento productivo ni se probó sobre el documento autorizado de 33,6 MB.

Ghostscript permaneció como herramienta de estación y no fue añadido a la aplicación. Antes de una adopción deben resolverse licenciamiento/redistribución y regresiones sobre cifrado, firmas, formularios, anotaciones, metadatos, salida iText y equivalencia visual completa.
