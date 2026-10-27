# PRODUCCION-IMPORTACION-CARPETA-PERFIL

- Ticket: DOC-110
- Cambio OpenSpec: doc-110-produccion-importacion-carpeta-perfil
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

`ServicioPerfilImportacion` coordina crear, guardar, ensayar, publicar, versionar, cambiar disponibilidad y seleccionar. `ValidadorPerfilImportacion` exige configuración coherente; `ResolvedorReglasPerfilImportacion` procesa fuentes declarativas, conversiones, obligatoriedad y valores diferidos. `PerfilImportacionJson` aplica esquema cerrado y huellas. Ninguno ejecuta importaciones o crea objetos documentales.

`MySqlPerfilImportacionRepository` persiste cabecera, versiones y reglas con IDs incrementales y revisiones. Publicar bloquea y comprueba revisiones/huella antes de actualizar versión y referencia de cabecera en una transacción. Una publicación se conserva inmutable; editar exige un nuevo borrador. Desactivar impide nuevas selecciones y conserva historial.

La integridad completa requiere elegir EXPEDIENTE o LOTE antes de publicar; parcial explícito usa NO_APLICA. Las reglas rechazan destinos duplicados, referencias ajenas y valores incompatibles, conservan ceros iniciales y diferencian ausencia de dependencia futura. La clasificación heredada se proyecta desde catálogos autorizados y el ensayo la identifica como propuesta del perfil, no como expediente confirmado.

`CoordinadorProveedorIndices` reutiliza respuestas solo dentro del ensayo actual. `MySqlProveedorDatosIndices` valida identificadores/metadatos, parametriza claves y limita la consulta a dos filas para distinguir ausencia, coincidencia única y ambigüedad. `ServicioAdministracionProveedor` exige versiones guardadas/publicadas según la operación; un BORRADOR no puede consumirse desde un perfil. `PoliticaFuentesProveedor` aplica la autorización vigente sin fallback al archivo.

`ServicioFuenteImportacion` y `MySqlFuenteImportacionRepository` mantienen identidad estable por ámbito/conexión/tabla, columnas e historial transaccional. Revisiones obsoletas y duplicados no dejan cambios parciales. Revocar una fuente deniega usos nuevos sin reescribir contratos de proveedores o perfiles publicados. Omitir columna empresarial solo es válido con autorización exclusiva expresa del servidor.

La fachada devuelve errores contractuales 400/401/403/409/503 sin SQL, cadenas de conexión o stack. El cliente no reenvía mutaciones inciertas y exige reconciliar estado. [Matriz de implementación y pruebas](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc110-verificacion-final-20261027.md): arnés .NET de diez familias, Node, Chromium y aceptación autenticada diferenciados.
