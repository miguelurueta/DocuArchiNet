# DOC-111: reutilización de catálogos para creación, 2026-10-29

Avance parcial de 9.3, D-07/RQ-07. **95/130 completas, sin incremento**. Se implementa un parcial del catálogo compartido, registrado en Compile/arnés, sin repositorio paralelo, administración simulada o endpoint nuevo.

`ResolverDatosCreacionExpediente` recibe el comando inmutable existente y reutiliza autorización operativa, permiso del nivel, gabinete y origen DA, identidad recíproca y consultas por ID. Devuelve datos nuevos para la futura composición legacy: organigrama derivado del instrumento/área/empresa confirmados y nombres de clasificación, tipo, unidad y gabinete. La subserie no configurada permanece ausente.

Los nombres que legacy vuelve a resolver se contrastan mediante SELECT parametrizados y LIMIT 2 con los filtros reales de cada helper, incluidos registros inactivos que esos helpers no excluyen. La lectura inversa debe devolver exactamente un registro con el mismo ID y nombre. No se escapan o interpolan nombres, ni se elige una fila entre duplicados. La unidad que el Trim legacy alteraría se rechaza. La lectura no demuestra estabilidad frente a cambios concurrentes.

El nombre del gabinete también se contrasta en DA por nombre global, incluyendo coincidencias sin permiso que podrían ocultar una ambigüedad; su autorización por ID ya fue comprobada mediante el catálogo existente. No se interpreta un ID de gabinete en RA.

El responsable se lee por ID interno ya resuelto, estado activo y empresa. No se reinterpreta como identificación original de carpeta. Se conserva su identificación textual, incluidos ceros iniciales, y se exige valor presente de longitud máxima 60; sin regla, no se inventa responsable.

Validación PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: familias previas y casos nuevos con SELECT sintéticos. Nombres Unicode/apóstrofo conservados, valores ausentes, rechazo de cardinalidad 0/2, ID incorrecto, nombre cambiado y columna faltante en cada búsqueda inversa; gabinete denegado, responsable ajeno/sin identificación y Trim incompatible. El snapshot no se modifica. No certifica SQL MySQL real.
- Build Debug completo sin despliegue, salida/intermedios `.tmp/doc111-catalogos-creacion-build/{bin,obj}`, log `.tmp/doc111-catalogos-creacion-build.log`, código 0.

Hallazgo pendiente de decisión: legacy exige ciclo de archivo, pero el perfil no lo define. El instrumento tampoco aporta esa relación en la evidencia de esquema inspeccionada. La carga individual de Producción usa «Archivo Gestión»; el formulario de creación manual usa su selector. Se consultó al usuario si DOC-111 debe reutilizar la regla de Producción o configurar el ciclo por perfil. Esta entrega no adopta una opción por disponibilidad de catálogo.

Pendientes antes de invocar: resolver el ciclo, componer los argumentos legacy (incluido rango Integer), comprobar conexión/contexto RA, compatibilidad de texto con su SQL, autoridad vigente, barreras y resultado/correlación. La lectura no resuelve estos puntos ni habilita ejecución.

Sin consultas SQL/HTTP/E2E reales, creación, almacenamiento, cambios de datos/configuración o despliegue en este corte. ClassGaExpediente, ClassAlmacenamiento, ClassNeodynamic y conect permanecen intactos; sin alteraciones de SII.
