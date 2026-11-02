# DOC-111: adaptador de creación y verificación local, 2026-10-29

Se completan las implementaciones de **9.3 y 9.4: 97/130 tareas**, con evidencia local y límites explícitos. No se ejecuta una creación real ni se certifica el flujo integral. XML físico, CAS/rollback MySQL real, barreras/coordinación, persistencia final de fase y recuperación permanecen pendientes en sus tareas.

El puerto estrecho ICreacionExpedienteImportacion recibe contexto, lote persistido, raíz e intento. El lote aporta el snapshot; no se fabrica un lote para adaptar la firma anterior. El puerto completo conserva la capacidad de reconciliación aún pendiente. AdaptadorExpedienteImportacion implementa creación; su constructor productivo en el parcial Legacy compone los lectores/repositorio/autoridad existentes sin endpoint o repositorio paralelo.

Secuencia: componer comando inmutable, validar compatibilidad, autorizar, leer y capturar catálogos, volver a autorizar, iniciar duraderamente, reautorizar e invocar una sola vez, conservar IDs, volver a autorizar y verificar. El coordinador debe comprobar la barrera antes de llamar; esta entrega no habilita avanzar/ejecutar desde HTTP.

IniciarCreacionExpediente del repositorio único usa su transacción corta y orden de bloqueo de lote/raíz. Contrasta estado, detención, revisión, identidad previa, contexto/snapshot y valores; el UPDATE del intento exige IDs/fase, RESERVADO, token/huella y fin ausente. Solo una fila concede inicio, conservando ciclo/intento en evidencia preliminar, revisión de lote y evento dentro de la transacción. No mantiene esa transacción durante legacy. Un intento ya EN_CURSO no consigue otro inicio. La carrera/rollback real de este nuevo UPDATE no se probó contra MySQL.

La llamada productiva usa parámetros nombrados de Registrar_Expediente_Conservacion. Pasa usuario/empresa/nivel e instrumento explícitos; código manual, fechas, clasificación, gabinete y responsable del comando/catálogo, y Archivo Gestión. Tema y alias visible reciben el nombre exacto. Contadores iniciales cero, volumen inicial uno, sin contenedor físico, expediente padre, autorregistro o tarea/ruta SII fabricados. Los IDs ByRef se capturan en Finally incluso si la llamada lanza una excepción. El binding se compila contra la clase real, pero no se invoca en el arnés ni en una BD real.

Tras iniciar, un fallo, respuesta distinta de YES, ID faltante, autoridad revocada o lectura no concluyente devuelve INCIERTO. Se conservan los IDs positivos conocidos y la evidencia del ciclo/intento, sin mensajes internos ni replay. El inicio durable no equivale a persistir el resultado final: raíz e intento pueden seguir CREANDO/EN_CURSO hasta implementar 10.7. Un crash en esa ventana sigue necesitando reconciliación, no una repetición.

ComprobarResultadoCreacion reutiliza el lector SQL por IDs exactos, alcance y clasificación, en el mismo repositorio. Agrega comparación de código, tema, alias visible, fechas canónicas, ciclo ID/nombre, nombre de gabinete, responsable e identificación y volumen uno. Ausencias opcionales exigen SQL NULL; no se normalizan cero/vacío a ausencia. No infiere identidad por nombres. True permite confirmar el resultado local de la llamada concluida; no prueba integridad física del XML o una causalidad recuperada después de un crash.

La revisión del árbol de Producción encontró que presenta ALEAS_EXPEDIENTE (ClassGaProducionDocumental alrededor de 727/768), y el creador limita ese argumento a 120. Se valida el nombre visible en preflight y antes de crear, sin truncar o cambiar el valor aprobado.

Validación PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: familias previas y nuevos casos Framework. Invocación única, inicio rechazado/repetido, revocación después de iniciar, errores y respuestas no canónicas, IDs parciales y fallos de verificación conservan incertidumbre/evidencia; no se filtran los mensajes centinela. Se prueban cambios de revisión/snapshot/valores/destino y detección temprana de nombre visible demasiado largo.
- Lector SQL con registros sintéticos: alcance y cada campo esperado alterado/nulo, cardinalidad 0/1/2 y columna faltante. No acredita ejecución MySQL real.
- Build Debug completo sin despliegue, salida/intermedios `.tmp/doc111-adaptador-creacion-build/{bin,obj}`, log `.tmp/doc111-adaptador-creacion-build.log`, código 0. Incluye el binding real; el arnés compila solo el núcleo inyectable y no sustituye la clase legacy.

Sin SQL/HTTP/E2E reales, creación, almacenamiento, limpieza, cambios de datos/configuración o despliegue en este corte. ClassGaExpediente, ClassGaProducionDocumental, ClassAlmacenamiento, ClassNeodynamic y conect permanecen intactos. No se alteran SII o carga individual.

Corrección posterior: el SQL inicial del inicio usaba resultado_doc, pero el intento define evidencia_doc. El siguiente corte corrige ese enlace y agrega regresión contra el DDL. La compilación/arnés de este corte no habían ejecutado ese UPDATE real; no se atribuye una persistencia MySQL exitosa al resultado local anterior. La persistencia final de creación implementada después se documenta en `doc111-resultado-creacion-local-20261029.md`.
