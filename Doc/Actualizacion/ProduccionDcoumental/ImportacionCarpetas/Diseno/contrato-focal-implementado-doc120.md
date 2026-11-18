# Contrato focal implementado — 2026-11-18

La instrucción «implementa» autoriza este corte local. Script09 preparado para aplicación manual; DLL y SQL de este corte todavía sin instalar. Se conservan las once tareas históricas pendientes y las nuevas pruebas reales3.187/3.196.

## Procedencia y lectura

`IControlArchivosIndependientesImportacion.PrepararControlArchivos` se ejecuta dentro de una fase mutante ya autorizada. Nunca lo invocan consultar, exportar o una tarea en segundo plano. Antes de la primera recepción nueva, creación, materialización o almacenamiento de un lote, valida el índice manual, bloquea el lote y contrasta una vez el inventario y los valores contra el plan confirmado, incluidos los planes1/2. Los lotes terminales conservan su consulta/repetición histórica.

Reutiliza `ra_integracion_evento`, mediante INSERT en la misma transacción:

| Tipo / acción | Contenido |
|---|---|
| LOTE / SELLAR_CONTROL_ARCHIVOS | Cabecera versión1: lote/plan/hash del plan, huellas del snapshot completo y contexto, cantidades/bytes originales y raíz Merkle del inventario. |
| ARCHIVO / SELLAR_CONTROL_ARCHIVO | ID, índice y hashes hermanos de su prueba. Hoja: IDs y datos originales de raíz/archivo, valores previstos, bytes/ruta/modificación y las huellas anteriores. |
| DOCUMENTO o INVENTARIO / FIJAR_IDENTIDAD_DOCUMENTAL | Archivo propietario, origen/gabinete e IDs documentales confirmados. Consultables por referencia, fuera del diario del intento. |

El índice no único `ix_integracion_evento_recurso(lote_id,recurso_tipo,recurso_id,accion)` es el único DDL nuevo. No se crean tablas o columnas. El script09 comprueba orden, ausencia de prefijos y carácter no único; un índice incompatible se informa sin sustituirlo. Su aplicación puede bloquear/escanear la tabla y requiere una ventana manual propia.

La cabecera y sus pruebas son una proyección interna emitida después de comprobar el plan. El módulo agrega eventos y no los reescribe. No son un permiso, reserva, resultado ni comprobación permanente del PDF. Cada lectura focal contrasta el hash/estado/propietario/versión actuales del plan, comprueba la hoja del objetivo y, si existe expediente, su intento de creación confirmado. Cambiar datos originales, valores, snapshot o parentesco invalida esa comprobación. La autorización vigente y la validación física continúan en sus fronteras originales.

La proyección de referencias se inicializa también para documentos históricos completos; una colisión hace rollback. Las confirmaciones nuevas validan y registran sus referencias en la transacción del checkpoint. Una referencia ya asociada a otro archivo degrada a INCIERTO y conserva los IDs observados. `intento_id=NULL` en los eventos auxiliares conserva el diario V2 de reserva/inicio/autorización/retorno/resultado. Repetir una confirmación no duplica la proyección.

## Contrato público

`ConsultarRecursoLote` conserva la operación y añade `versionLectura As Integer`:

| Versión | Resultado |
|---|---|
|1, predeterminada | Recurso/padre, estado y huella/progreso globales anteriores. |
|2, solicitada por el bootstrap nuevo | Recurso/padre solicitados y estado autorizado; `huellaRecursos=null`, `progreso=null`, `versionLectura=2`. Sin enumerar todos los IDs para producir una huella. |

Otras versiones:400/SOLICITUD_INVALIDA. El cliente2 exige respuesta2 y el parentesco/revisión/contexto esperado; una respuesta antigua o incompatible no autoriza el siguiente efecto. Al iniciar/retomar sigue enumerando el inventario completo paginado. Los conteos entre efectos se derivan de los recursos y acuses confirmados de esa ejecución; otra vista exige actualizar para ver todos sus cambios. No son una promesa de snapshot global tras cada efecto.

`EstadoLoteImportacionResultado` añade `procesamientoIndependiente` y `escritorVivo`. Las capacidades son indicativas; el servidor vuelve a validarlas al reclamar. Un RESERVADO/EN_CURSO, recepción incierta, marcador huérfano o autorización sin retorno bloquea. Un archivo INCIERTO solo puede aislarse de otro si conserva intento correlacionado y eventos únicos de inicio, gate y retorno V2; jamás se reclama de nuevo ese mismo archivo por esta excepción.

El original ausente/alterado/ilegible anterior a la invocación produce422/ARCHIVO_NO_DISPONIBLE, con `datos` cerrados:

```text
loteId, archivoId, raizId: String, ID canónico autorizado
fase: CREAR_EXPEDIENTE | ALMACENAR
causa: ORIGINAL_AUSENTE | ORIGINAL_ALTERADO | ORIGINAL_ILEGIBLE | ORIGINAL_NO_COMPROBADO
certeza: NO_INICIADO
```

Nombre y ruta relativa se toman del inventario ya autorizado, nunca del mensaje de excepción o de una ruta física del error. Falta de File local se diagnostica en el cliente como ORIGINAL_LOCAL_NO_SELECCIONADO/RECEPCION, sin petición ni intento remoto.

Antes de publicar cero invocación, se persiste `estado_efecto=NO_INICIADO` y `resultado_doc` RECHAZADO/ORIGINAL_NO_DISPONIBLE, sin DocumentoId/InventarioId/Recepcion/Creacion. Solo una reserva propia todavía RESERVADO, con token/hash/revisión coincidentes, evidencia vacía y un único evento de reserva del propietario puede cerrarse RECHAZADO. Un fallo de persistencia no publica la decisión como segura. Después de iniciar V2, los fallos conservan incertidumbre y sus evidencias. La creación que ya conserva un intento mantiene su recuperación conservadora existente.

Una revisión explícita permite comprobar de nuevo el original. La reclamación consume exclusivamente el negativo tipado bajo CAS, después de verificarlo físicamente; los eventos anteriores permanecen. Materializar índices conserva el negativo hasta esa comprobación. Un `.bin` recibido perdido no se reemplaza mediante la reselección ni un reenvío automático.

Falta de índice:503/CONTROL_FOCAL_NO_CONFIGURADO, con la instrucción manual del script09. Sesión401, autoridad/CSRF403, revisión/actividad/incertidumbre409 y demás traducciones previas se conservan.

## Agregados, medición y retorno

La consolidación habitual usa recuentos SQL sin reconstruir los JSON de los archivos. Los agregados pueden recorrer filas del lote en MySQL; no equivalen a una consulta por PK ni a cero lecturas del motor. Antes de declarar COMPLETADO se ejecuta la auditoría integral de identidades, valores, inventario y resultados una vez; no reabre los PDF de otros archivos.

Las capturas optativas `X-DOC120-Medicion` conservan sus guardas. Añaden categorías de SELECT ejecutados en los adaptadores/repositorio de importación, decodificaciones/caracteres JSON de control, aperturas instrumentadas y bytes PDF procesados por el staging común. Esos tiempos pueden solaparse con las etapas principales: no se suman para simular latencia total. El writer compartido y sus SELECT internos permanecen fuera de estos contadores. Las comprobaciones hash V2 conservan su métrica propia. Enumeración, sello inicial y auditoría final se reportan aparte del costo de un archivo con control ya preparado.

Presupuesto local: cero cargas y aperturas de originales hermanos por guardado; una verificación/retención por pasada del coordinador y tres cargas del objetivo en su recorrido completo normal; prueba por archivo de hasta diez hermanos para1.000 archivos. El adaptador V2 mantiene sus comprobaciones adicionales del mismo objetivo. No se fija un número total de SELECT ni una mejora porcentual de IIS antes de medir la ventana nueva.

Desplegar DLL y assets del mismo corte, con respaldo y script09 previamente revisado/aplicado de forma manual. El código no ejecuta ALTER. Un retorno de DLL conserva índice, eventos, recepciones, intentos y documentos. El lector anterior puede leer sus datos previos; puede bloquear un negativo nuevo o volver a la ruta de lectura extensa. Eso requiere recuperar la versión compatible, nunca vaciar resultados, rotar tokens ni repetir writers por SQL. Cliente2 con servidor anterior se detiene ante contrato incompatible. No bajar el índice o borrar la evidencia como mecanismo de rollback.
