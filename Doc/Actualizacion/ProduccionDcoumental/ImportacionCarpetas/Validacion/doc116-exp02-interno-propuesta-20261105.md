# DOC-116 EXP02 interno — propuesta de laboratorio compilada

Estado: candidata local desactivada, no desplegada y no autorizada para ejecución. No constituye PASS del caso. La ruta permite preparar una prueba autenticada sin inventar HttpContext ni añadir un WebMethod/header/flag de fallo al producto; requiere una compilación distinta y aprobación de la vía de laboratorio antes de preparar su ejecución.

## Evidencia nueva

Se compiló el proyecto WebForms/VB real con MSBuild /t:Compile y un target externo que sustituye únicamente el binding por una copia instrumentada dentro de .tmp/doc116-internal-candidate. El proyecto y el binding de producto permanecen sin cambios. Un helper privado de laboratorio aplica alcance cerrado y marca de un solo uso antes de lanzar la excepción. La autoridad y catálogo originales siguen ejecutándose.

La inspección ReflectionOnly confirmó que la clase está dentro del ensamblado y que LoteAutorizado=NO_AUTORIZADO: todas las creaciones reales se rechazan antes del legacy. La candidata no debe instalarse. SHA de la candidata 512c7eb2e97e980dbe0f8bf222bb85e830c06131b49e779c085d8823e0d35f33. El DLL de IIS no cambió: 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5. Compilación no equivale a aceptación autenticada.

## Reutilización propuesta

Usar lote 18/plan 34, nivel 215, contexto 141/2, versión 38, raíces 29/30 y sus dos originales PDF recibidos. EXP03 terminó sin crear expedientes y restauró permiso 629. Antes de cualquier activación debe comprobarse de nuevo por SELECT y consulta autenticada que las dos raíces sigan frescas y los originales estén intactos. No se necesita otro plan/lote ni recibir otros PDF.

## Ventanas de fallo y orden

Primero raíz 29: ejecutar repositorio real y lanzar la excepción al devolver el checkpoint confirmado. El coordinador debe consultar la evidencia histórica; comprobar IDs/relación y replay confirmado sin otra creación. No se afirma pérdida de un paquete de red.

Después raíz 30: provocar excepción tras legacy con IDs conocidos, antes de llamar al checkpoint. La marca privada conserva IDs/intento y el cliente verifica por SELECT; el servidor debe conservar incertidumbre. No se reintenta la creación ni se escriben IDs de forma manual para reparar. Se ejecuta esta variante al final porque su intento activo puede impedir otros avances del lote.

Presupuesto propuesto para una autorización posterior: como máximo dos expedientes nuevos, tres solicitudes de creación/replay, cero documentos definitivos, cero cambios de permisos y todos los recursos conservados. Debe aceptarse expresamente que la segunda raíz pueda quedar pendiente de verificación; se conservan los IDs fiables, sin búsqueda por nombre.

## Condiciones para preparar ejecución

Aprobar explícitamente la vía de laboratorio, distinta del runtime normal ya acreditado. A continuación preparar driver interno, candidata con IDs exactos, marca durable de un solo uso, copia comprobada del DLL original, activación/restauración acotadas y recuperación ante cierre de consola. Presentar los binarios y scripts terminados para autorización de activación y efectos antes de usar IIS. Nada de eso se ha ejecutado ni se autoriza con esta propuesta.

No se cambia la especificación para dar la prueba por cumplida ni se importa código remoto sobre la copia local. RQ04/D04 sigue bloqueado. La compilación local y la candidata son un resultado de viabilidad técnica, no un mecanismo instalado en el sitio.
