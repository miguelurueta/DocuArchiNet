# Lote50 — repetición de la medición con el ejecutor existente

Solicitud2026-11-18: «realiza la prueba E2E con el lote50 y mide los tiempos; reutiliza la existente». Se reutiliza `measure-doc120-three-files.cjs`, su autenticación privada, consultas/validación RA/DA y límite de tres almacenamientos. No se reutiliza ni modifica el estado cerrado del run23e18a40. Sus104 HTTP/14 SELECT/1 sesión/3 resultados confirmados se revisaron antes de preparar la continuación.

**Estado actual:** run9dd8a1e963154c338fb8885c10ce8180 cerrado antes del login, sin solicitudes de almacenamiento. El índice09 aplicado manualmente fue verificado y la DLL/PDB nueva quedó instalada con respaldo. No hay tiempos nuevos de almacenamiento observados. No se ejecutó ALTER desde este runner.

Alcance: `https://localhost/GestionDocumental-Docuarchi.net/`, lote50, propietario141/empresa2/nivel223; los tres primeros pendientes recibidos con valores fijados, escogidos del inventario actual mediante SELECT. No se fijan IDs por inferencia ni se repiten104/105/106 ya confirmados. Máximos600 HTTP,60 SELECT externos,1 sesión,3 solicitudes ALMACENAR y96MiB de originales;45min antes de iniciar nuevas operaciones. Una solicitud iniciada espera su respuesta sin reintento ni timeout de30s.

Se mantienen cero preparaciones, recepciones, creaciones, materializaciones, cambios de perfil, reanudaciones, reconciliaciones, SII o borrados. Una inicialización interna del control focal puede agregar eventos de procedencia y referencias existentes, una sola vez; se distingue del guardado de los tres documentos. Estado detenido/activo/incierto, permisos insuficientes, revisión diferente o un tercero modificando binarios/inventario detienen la prueba. No se resetean estados para forzarla.

## Preparación y guardas verificadas localmente

- Respaldo de DLL/PDB activos en `.tmp/doc120-independent-deploy/477e135dfa97450a817f0795c9163371/`, con huellas comprobadas. Activa: DLL `f667ea0f…`; candidata compilada del commitb72f2dd6: `b3303603a03001e2a1f578d48ed4a3f6ea396c0b0faa162544941c7f9485e12f`. No sobrescribir una compilación intermedia cuyo hash difiera del activo respaldado.
- La continuación requiere `--reviewed-previous <runId>` y un estado previo cerrado/con credenciales eliminadas y consumo coherente. Conserva byte a byte su state.json/window.json, usa archivos nuevos y admite solo una continuación por padre. Repetición o padre activo/incoherente no abre otra prueba.
- Primer control: índice completo/no único/sin prefijos, mediante SELECT de metadata. Si falta, la consola explica el script manual y solo repite ese SELECT ante `REVISAR` explícito, hasta tres consultas. No autentica en la aplicación ni despacha documentos mientras falte.
- Después espera localmente la DLL candidata autorizada, sin HTTP. Una DLL ajena o plazo vencido detiene. Pide credenciales de aplicación únicamente después de estos prerrequisitos. Claves/cuentas enmascaradas, solo en memoria; sin cookies/HAR/transcripción o secretos en argumentos/archivos.
- Exige bootstrap y respuesta focal versión2, permisos vigentes, snapshot/expediente/gabinete conocidos. Antes de cada almacenamiento contrasta revisión e inventario ajeno. Resultado conocido en RA y DA, preservación de documentos ya confirmados y huella de los otros997 recursos antes de continuar.
- Captura HTTP, TOTAL, metadatos/esquema/autoridad/catálogos/eventos/JSON, comprobaciones/copias V2, writer y checkpoint. El sello inicial y los agregados tienen costo propio; no sumar etapas anidadas ni confundir SQL del ejecutor con SQL interno instrumentado. No se contabilizan los SELECT internos del writer compartido.

Pruebas de runner:11/11 locales, incluidos continuación/huellas, anti repetición, índices incompatibles, contrato2, espera de binario, límites, conservación de acuses y diagnóstico seguro de entrada de credenciales. Este corte no acredita tres almacenamientos reales por preparar o abrir la consola. El ensayo previsto mide tres documentos del lote existente; no cubre el caso A/B faltante/C, carreras MySQL o capacidad completa de1.000, por lo que no completa3.187/3.196.


## Indice verificado, instalacion pendiente

El usuario informa "ya corrio". La consola realizo una segunda consulta de metadata y valido el indice09: estado WAITING_DEPLOYMENT,2 SELECT/0 HTTP/0 sesiones/0 solicitudes de almacenamiento. La entrada REVISAR ya fue procesada por la consola; un intento auxiliar de enviarla se detuvo al detectar el cambio de fase, sin inyectar entrada. La DLL activa sigue siendo f667ea0f; la candidata b3303603 conserva su respaldo y requiere la aprobacion especifica de instalacion solicitada. No hay tiempos nuevos de almacenamiento observados todavia.

## Instalación y cierre observado, 2026-11-18

Tras «continua» y aprobación de la instalación, se sustituyeron PDB y DLL con reemplazo por archivo y respaldo verificado. Instalación terminada a las14:12:12 UTC: DLL `b3303603a03001e2a1f578d48ed4a3f6ea396c0b0faa162544941c7f9485e12f`, PDB `ebf1e48e0dee9f8c76f492ec37af062d510c7cbbc3fc5fcbadab76f60cc86806`, compilados del commitb72f2dd6. Sin cambios a Web.config, perfiles, esquema desde el ejecutor o pool IIS.

La consola alcanzó los controles del lote: plan90/versión45/revisión3057,10 archivos completados. Seleccionó107/108/109, recibidos y pendientes; no los despachó. Cerró a las14:12:20 UTC con `DOC120_MEASURE_STOPPED`,5 SELECT/0 HTTP/0 sesiones, credenciales eliminadas y estado/ventana histórica preservados. El código genérico anterior no permite certificar si faltó/canceló una entrada privada o falló el arranque del navegador; no se conserva ni reconstruye el texto privado para averiguarlo.

Se agregó al ejecutor una fase explícita para cuenta, clave y arranque del navegador, y códigos cerrados para cada fallo; la excepción original nunca se imprime. La prueba local verifica que un texto privado en la excepción no aparece en el diagnóstico. Este cambio posterior no modifica el estado cerrado ni atribuye una causa retrospectiva. La tarea3.198 sigue pendiente: no existe medición posterior para comparar todavía. No se relanza automáticamente la ventana cerrada.
