# DOC-120 - parametros del video aplicados a la E2E visible

Video214029 (19,77s): lote52, destino PRUEBA-IMPORT,1 carpeta/1000 archivos,42,8 MB y EN_CURSO. Egresos version3 aparece en la seleccion previa a Consultar ultimo lote52: **no demuestra que esa sea la version confirmada del lote**. La E2E consulta sus datos vigentes y no prepara ni confirma otra importacion. [Fuentes y resultados](doc120-e2e-parametros-video-20261119.json).

El ejecutor existente acepta `--lot 52 --headed`. Busca el ID exacto dentro del catalogo autorizado de la cuenta; si falta, se detiene con mensaje propio, sin elegir otra importacion. La seleccion opcional de recuperacion queda limitada al mismo lote. No reemplaza el perfil confirmado ni fuerza el estado/tamano redondeado del video. Parametros no secretos; credenciales solo en consola privada.

La ventana36a221bf ya no tenia consola ni proceso activo: se preserva su estado WAITING_IMPORT_SELECTION y consumo189 HTTP/2 SELECT/1 sesion, sin afirmar limpieza final registrada o aprobacion. No se recuperan sus credenciales ni se cambia su archivo para aparentar cierre. Nueva consola97c9d4c3/PID15952 abierta con lote52; observacion WAITING_ACCOUNT y autorizacion aceptada. Resultado real pendiente, no certificado por configurar/arrancar.

41/41 locales, dos casos nuevos: formato canonico/compatibilidad de argumentos y seleccion por ID dentro del catalogo, sin fallback. Sintaxis, OpenSpec estricto y diff aprobados. Mismos limites/guardias DOC119 solo lectura, sin confirmar, recibir, crear, almacenar, reanudar, borrar o DDL. No cambia producto, DLL o esquema.3.241 completa en preparacion;250/261 tareas,45 S/216 M y107 escenarios, once pendientes anteriores conservadas. Fotogramas analizados en memoria, sin guardarlos ni extraer credenciales.
