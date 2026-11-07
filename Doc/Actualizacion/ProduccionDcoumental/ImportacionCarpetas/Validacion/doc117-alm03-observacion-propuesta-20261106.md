# DOC-117 — Decisión propuesta sobre observabilidad ALM-03

Estado: PROPUESTO, sin aprobación editorial ni ejecución E2E ALM-03. Progreso 16/18; ALM-01 y ALM-02 se conservan. Esta propuesta no autoriza autenticación, SELECT reales, preparación de recursos, almacenamiento, profiler sobre IIS, reciclado o instalación de DLL.

## Brecha comprobada

D-06/RQ-06 requieren ejecución solapada de los escritores, además de identidad y conservación de recursos. La importación llama directamente a `ClassAlmacenamiento.AlmacenamientoDocumentoProduccionDocumental` desde `AdaptadorDocumentoImportacion.Legacy.vb`. La carga individual oficial entra por `FileUploadHandler_ → UploadSaveFile → PreAlmacenaDocumentoProduccion → AlmacenamientoDocumentoProduccionDocumental`.

Marcar el handler completo o `UploadSaveFile` incluiría preparación y no mediría el escritor. Incluso un par de marcas inmediatamente fuera de cada llamada puede dar un falso positivo: el hilo A registra entrada y se suspende antes de llamar; B ejecuta y termina; A ejecuta después. Los intervalos exteriores se solapan aunque las ejecuciones internas sean secuenciales. No se cambiará el significado de RQ-06 para aprobar ese resultado.

El analizador local `tools/e2e/tests/support/doc117-alm03-observation.cjs` distingue intervalos de llamadores, rechaza duraciones HTTP/preparación, otros relojes/procesos/AppDomains, sesiones no comprobadas, recursos ajenos, trazas incompletas e identidades duplicadas. Usa enteros de 64 bits representados como cadenas, sin redondearlos en JavaScript. Sus **23 pruebas PASS** no producen un PASS ALM-03: siempre conserva `writerBodyOverlapVerified=false` y `alm03ConcurrentPass=false`.

## Alternativa externa investigada, todavía no seleccionada

La instalación local contiene VSDiagnostics e InstrumentationAgent. Se hizo un ensayo únicamente con `Doc117Alm03TimingProbe.exe`, compilado desde una fixture sintética, sin ensamblado de aplicación, IIS, red, cuentas o base de datos. Se creó y cerró la sesión propia 217 y se obtuvo una `.diagsession`; la verdad de control contiene una pareja solapada y otra secuencial dentro de los métodos sintéticos. La captura es válida, pero no se ha acreditado extracción automatizada de eventos de método/hilo/tiempo ni correlación con los recursos. No se presenta como un observador listo para ALM-03.

Microsoft documenta instrumentación dinámica para .NET Framework sin cambiar archivos en disco, pero las vistas descritas agrupan tiempos por función/árbol. Su CLI indica que Instrumentation necesita lanzar el proceso y no admite attach. Los hooks Enter/Leave del CLR son otra capacidad, registrada durante Initialize; su mera existencia no acredita un productor/exportador disponible ni una restauración segura de IIS. Fuentes: [instrumentación](https://learn.microsoft.com/en-us/visualstudio/profiling/instrumentation-overview?view=visualstudio), [CLI y restricciones de attach](https://learn.microsoft.com/en-us/visualstudio/profiling/profile-apps-from-command-line?view=visualstudio), [vistas agregadas](https://learn.microsoft.com/en-us/visualstudio/profiling/instrumentation?view=visualstudio), [hooks CLR](https://learn.microsoft.com/en-us/dotnet/framework/unmanaged-api/profiling/icorprofilerinfo3-setenterleavefunctionhooks3-method).

Los dos ensayos iniciales no acreditaron captura: uno falló al crear sesión y el siguiente se detuvo al manejar el diagnóstico de sesión ausente. Se conservan como fallidos. No se confía en el exit code del colector: el intento correcto comprueba la existencia de sesión, salida de la fixture y archivo final. Ninguno tocó IIS ni datos de aplicación.

## Ajuste editorial mínimo propuesto

Permitir una **excepción exclusiva de laboratorio** a la prohibición de instrumentar la función compartida: en una copia fuera del proyecto, añadir observación de entrada/salida **dentro del cuerpo de `AlmacenamientoDocumentoProduccionDocumental`**, manteniendo sus operaciones, argumentos, decisiones, errores y retornos. Las fuentes productivas y el método ordinario permanecen byte a byte intactos. No se propone aplicar este cambio al producto ni distribuir una bandera, endpoint o header de prueba.

La candidata tendría dos partes revisables: guardas en los llamadores para correlacionar el recurso y sesión autorizados; y observación dentro del método para medir el intervalo real de esa invocación. La validación y cualquier fijación preliminar ocurren antes de tomar el tiempo de entrada. La salida se observa en un finally que cubra retornos y excepciones, sin repetir el efecto para resolver un error de observación.

Contrato propuesto para la implementación local posterior a aprobación:

1. Alcance cerrado compilado: ambiente, actor 141/empresa 2/nivel 7, versión 38/gabinete 10, recursos nuevos de importación y expediente individual 467/tipología 50 solo si se verifica y autoriza. Lotes 18–21, intentos 41/52/53, documentos existentes y expediente 664/665 quedan protegidos.
2. Dos sesiones de aplicación independientes, comparadas en memoria antes del segundo escritor. Nunca guardar SessionID, cookies, cuentas o contraseñas; usar etiquetas fijas de flujo y referencias numéricas autorizadas. La instrumentación no cambia la sesión para obtener concurrencia.
3. Mismo dominio de contador monotónico, frecuencia, PID y AppDomain; entrada/salida interna e hilo nativo/administrado correlacionados con la operación exacta. Una única entrada por recurso, marcas durables y huellas de fuente/candidata/alcance. No sumar tiempos de otros procesos o después de un reinicio.
4. Sin Sleep, espera entre participantes, barrera artificial, retención de respuesta o bloqueo global dentro del escritor. Si se serializa naturalmente, informar SERIALIZADO/NO_ACREDITADO. El ensayo no se reintenta automáticamente para buscar un PASS.
5. El intervalo demuestra actividad simultánea del método instrumentado, incluidos bloqueos que éste encuentre; no demuestra simultaneidad de commits, ausencia de bloqueos, capacidad o rendimiento del binario ordinario, ni aislamiento XML/SII. La instrumentación puede perturbar los tiempos; no se exporta ese resultado a un binario sin instrumentar.
6. Los efectos se acreditan separadamente por SELECT e IDs conocidos: un documento/inventario/incorporación por PDF, valores/nombres/destino propios y originales íntegros. Un intervalo solapado sin esas pruebas no concede PASS.
7. Primero prototipo desactivado, pruebas locales de tiempos/retornos/errores/alcances y ensayo de respaldo/restauración con guardián independiente; después presentar recursos, candidata y presupuesto cerrados para una autorización real distinta. Ninguna instalación o carga se deriva de esta decisión editorial.

Esta excepción **aún no se ha aplicado** a D-06, D-08, RQ-06, al proyecto, al escritor ni a una candidata. Si se mantiene la prohibición también para copias de laboratorio, seguir investigando un observador CLR externo sin ejecutar cargas; ALM-03 permanece NO_EJECUTADO hasta tener observación suficiente. No se reduce la aceptación por inferencia.

## Evidencia y decisión

`doc117-alm03-observabilidad-local-20261106.json` registra huellas de los llamadores/método compartido, analizador, tests, fixture, captura sintética y runtime conservado. La preparación local de observabilidad no completa 17.1 ni 18.1.

Decisión solicitada: aprobar la excepción de observación interna únicamente para desarrollar y verificar la variante de laboratorio descrita, o mantener la exclusión y continuar por un observador externo. Se requiere resolver esta contradicción de diseño antes de implementar las marcas internas. La ejecución real y las cuentas/recursos seguirán requiriendo su propia autorización explícita.
