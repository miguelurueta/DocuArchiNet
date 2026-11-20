# Consulta del último lote sin referencia local — DOC-120

Corrección solicitada2026-11-17: «pero ahora no me deja consultar el ultimo lote», seguida del video170531-bloqueo. El video muestra el botón deshabilitado antes y después de elegir Egresosv3. La causa en código es la condición `!model.ultimoLoteId`; esa referencia solo existe en memoria/sessionStorage del contexto actual. No es una consulta fallida del servidor ni depende del perfil elegido. No se certifica por qué desapareció la referencia de aquella sesión.

## Contrato adicional de lectura

`WebServiceImportacionCarpetas.asmx/ConsultarUltimoLote`, POST JSON `{solicitud:{}}`. Usa sesión/CSRF, origen y guardas existentes. No acepta usuario, empresa, nivel, perfil o lote del cliente. Respuesta en envelope existente: `{encontrado:true,loteId}` o `{encontrado:false,loteId:null}`. La operación no se clasifica como mutación.

Un puerto aditivo del repositorio existente busca únicamente `solicitante_ref` y `ambito_ref` de la identidad autenticada, ordena `lote_id DESC` y limita a1. Último significa mayor consecutivo de lote confirmado de esa cuenta/empresa; no último expediente, última actualización, otro propietario ni borrador sin confirmar. No depende del nivel actualmente seleccionado: recupera el destino original y vuelve a autorizarlo con su snapshot antes de devolver el ID. Si ese destino ya no está permitido, deniega sin exponer la referencia ni buscar cuentas alternativas. La consulta normal vuelve a validar permisos/recursos al cargar sus páginas.

No hay tablas/columnas/DDL, escrituras, reservas, carga PDF, creación, almacenamiento ni levantamiento de detención. La composición reutiliza el servicio de consulta y conexiones efectivas existentes. Esta ampliación de D-04/D-08/D-09 y RQ-04/RQ-08/RQ-09 no habilita pruebas autenticadas o despliegue por sí misma.

## Interacción y compatibilidad

Consultar último lote permanece habilitado sin perfil ni referencia local cuando no hay una solicitud incompatible en curso. Si ya existe un ID guardado, conserva la consulta directa de esa referencia con autorización del servidor; no promete actualizarla por cada clic. Si falta el ID, el clic busca en el servidor y abre el seguimiento del resultado. Nada se busca automáticamente al seleccionar perfil o agregar carpeta.

La consulta desde una selección abierta conserva perfil/archivos/plan y permite Volver a mi importación. Desde el menú, sin nivel ni referencia, abre el modo consulta y busca. Pausa de coordinación conserva su secuencia previa; cerrar impide que una respuesta tardía abra otra consulta. Un cambio de contexto no hereda el ID anterior: el botón visible buscará bajo la nueva identidad, sin relajar el aislamiento de sessionStorage.

Sin lotes propios: aviso de ausencia, sin inventar ID ni reutilizar el de otra cuenta. Si no existe el método en el servidor, o falla la búsqueda: aviso de que puede usarse Consultar importación con una referencia conocida. No atribuye cualquier error de red a una versión antigua ni transforma denegación en «no hay lotes». La búsqueda requiere DLL compatible; los JS por sí solos solo habilitan el acceso y la alternativa manual.

## Despliegue separado

Compilar/probar en salida aislada. Para instalar, aprobar sustitución de DLL/PDB solo en el IIS local identificado, respaldo/huellas y una comprobación HTTP anónima del método; puede reiniciar la aplicación y su sesión. No reciclar el pool compartido, aplicar SQL ni consultar/reanudar el lote50 como parte de ese despliegue. El archivo activo se vuelve a comprobar antes de reemplazarlo para preservar compilaciones ajenas.
