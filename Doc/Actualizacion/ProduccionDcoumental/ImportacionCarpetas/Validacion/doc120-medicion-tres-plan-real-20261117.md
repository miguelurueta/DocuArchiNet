# DOC-120 — medición real de tres archivos del lote50

Solicitud expresa2026-11-17: «puedes hacer una prueba real y determinar los tiempo de tres archivos de ese lote y medir los tiempos quirurgicamente». Alcance de negocio: tres almacenamientos existentes del lote50 en `https://localhost/GestionDocumental-Docuarchi.net/`, propietario141/empresa2/nivel223. La ventana anterior de5 SELECT permanece cerrada. Esta ventana no es la carga de1.000 archivos ni una autorización para optimizar el producto.

**Estado: PREPARADA; instalación instrumentada pendiente de aprobación y credenciales efímeras pendientes. No ejecutada.**

## Límites y ejecución

- Máximo600 HTTP,60 SELECT de control,1 sesión y3 solicitudes ALMACENAR, secuenciales y sin reintento.45min para iniciar nuevas operaciones; una solicitud en curso espera su respuesta, sin corte del cliente a30s. Fallo/acuse incierto detiene la prueba, conserva el consumo y requiere revisión antes de otra ventana.
- Seleccionar los tres primeros IDs pendientes del inventario original de1.000: RECIBIDO/NO_INICIADO, valores efectivos fijados y sin marcador activo. Hasta96MiB de originales procesados en total;0 nuevos PDF recibidos. El gabinete procede del snapshot confirmado; se comprueba su identidad en DA antes del primer escritor.
- Mantener pausada la coordinación del navegador. Se exige lote sin señal de detención, actividad ni incertidumbre. El runner no levanta detenciones, reconcilia o materializa valores: esas condiciones detienen antes de guardar.
- Cero planes/lotes/recepciones/expedientes nuevos, cambios de perfil, SQL correctivo, SII, borrados, fallos inyectados o carreras. No se afirma capacidad de1.000 documentos ni límite de crecimiento de derivados sin medirlo.
- Controles exclusivamente SELECT y parametrizados por propietario/empresa/lote; huella de los otros997 archivos antes/después, preservación de resultados ya confirmados y comprobación de los nuevos IDs conocidos en DA. Una diferencia detiene sin reparación automática.
- Consola privada para DSN y credenciales de aplicación/SELECT; claves y cuentas enmascaradas. Sin secretos en chat, argv, archivos, HAR, capturas o logs. Cierre elimina secretos/cookies; resultados saneados en `.tmp/doc120-three-files/<runId>/state.json` y `timings.csv`.

## Captura de tiempos

La instrumentación mantiene el mismo recorrido y writer. Una solicitud ALMACENAR que ya superó sesión/CSRF y lectura tipada puede solicitar métricas mediante X-DOC120-Medicion de32 hex. Esta cabecera **no autoriza** operaciones o recursos. La respuesta añade Server-Timing con categorías cerradas, milisegundos y número de llamadas; sin archivos/rutas, valores, SQL, credenciales, conexiones o tokens. Los acumuladores pertenecen al HttpContext de esa solicitud y los errores de observación no cambian el resultado de negocio.

Se distinguen: consulta/preflight; originales previos y retenidos; autorización, carga de metadatos y comprobación física dentro de cada pasada; reserva; adaptador; datos documentales; original V2; copia de trabajo; validaciones V2 del PDF actual; inicio/autorización/retorno de persistencia V2; writer legacy; comprobación posterior; checkpoint y lectura final. Las categorías anidadas no se suman como intervalos independientes. Los conteos son llamadas de esas fronteras, no número exacto de sentencias SQL ni bytes físicos.

El runner registra también HTTP total y referencia pública de operación. El tiempo HTTP incluye trabajo fuera del coordinador, serialización y transporte; su diferencia respecto a TOTAL no se atribuye exclusivamente a red. Si faltan las métricas requeridas, se conserva el acuse observado y se detiene sin volver a guardar el archivo.

## Instalación separada y retorno

Instalar DLL/PDB instrumentadas exige aprobación propia porque reinicia la aplicación. Guardar respaldo de la pareja activa, comprobar sus huellas inmediatamente antes de sustituir y preservar compilaciones ajenas. No reciclar el pool compartido ni cambiar Web.config, SQL, listeners globales o el escritor compartido. Tras la instalación, una comprobación HTTP anónima puede verificar el rechazo de acceso sin sesión; no es una prueba autenticada de negocio.

El binario instrumentado compilado tiene SHA-256 `f667ea0ffe1b6f2ef111ac5afd52f50bab92c5edcd340a0421b9ea0931aef689`; el observado en bin antes de preparar esta ventana es `234268c3dddabc2996b871718af508be8ab4a86842a4af770aa44535b78a319a`. El runner requiere la huella instrumentada exacta y la vuelve a comprobar antes de cada escritor. Una ventana previa impide iniciar otra con un runId nuevo sin revisión; no se resetean presupuestos por cambiar de ID.

Comando preparado, únicamente tras instalar la versión aprobada:

```text
node tools/e2e/scripts/measure-doc120-three-files.cjs --run <32-hex-nuevo> --authorized-three --binary f667ea0ffe1b6f2ef111ac5afd52f50bab92c5edcd340a0421b9ea0931aef689
```

Debe lanzarse en consola visible con entrada enmascarada. Preparar el comando no acredita su ejecución ni produce tiempos reales. El retorno usa exclusivamente la copia preservada de la instalación, sin editar datos ni eliminar evidencia del lote.
