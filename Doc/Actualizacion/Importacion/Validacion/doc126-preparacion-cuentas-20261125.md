# DOC-126: preparación de cuentas para la prueba de permisos

Estado **PENDIENTE_IDENTIFICAR_CUENTAS / PENDIENTE_EJECUCION_MANUAL**;60/62 tareas,6.9 y7.5 abiertas. Base 3f73d6d94e58f86f0d5735a20b6d056dd9ec1ff9. Continúa la prueba de permisos solicitada por el usuario; los recorridos reales de tres modos/asistente/rollback/concurrencia ya aprobados no se repiten.

Diagnóstico actualizado 2026-11-25T05:23:17.1258891Z: empresa2, DA11/RA12,75 perfiles activos,**0 exclusivos y74 restringidos**, seis SELECT y cero mutaciones/sesiones de aplicación. No se encontró una cuenta que pueda acreditar exclusividad. No se selecciona arbitrariamente ninguna de las cuentas restringidas ni se modifican los cinco permisos vigentes de LUZ.AGUILERA, usuario141. Se solicita al usuario identificar las dos cuentas dedicadas; no se solicitan contraseñas por chat.

Metadatos 2026-11-25T05:24:21.8682407Z: un SELECT del catálogo y tres SELECT adicionales sobre metadatos RA; tablas de usuarios/perfiles InnoDB, cinco permisos tinyint NOT NULL default0, referencia de usuario int NOT NULL. El índice por usuario del perfil no es UNIQUE: la adecuación manual comprueba un perfil por cuenta y bloquea sus rangos en transacción antes de actualizar. No se añaden índices ni se modifica el esquema. Las conexiones proceden del catálogo/configuración vigentes; secretos sólo en memoria, sin imprimirlos ni guardarlos. Los manifiestos de ejecución y consultas quedan excluidos de Git.

Se entregan archivos nuevos revisables en Sql, **no aplicados**:

- **26:** SELECT para revisar dos IDs de prueba elegidos por el usuario y los metadatos. Parámetros NULL devuelven cero cuentas; bloquea incluir141. No modifica permisos.
- **27:** sólo si es necesario, habilitación manual de cargas0→1 para un perfil existente de una cuenta dedicada. Exige dos usuarios distintos, activos, empresa2, un perfil cada uno, otra cuenta con cargas0 y las otras cuatro capacidades del propietario ya en0. No retira capacidades ni crea usuarios/perfiles. Cuenta141 excluida; parámetros NULL/confirmación0 bloquean el UPDATE. Si ya está correctamente habilitada, se omite. No forma parte de la E2E.

Orden, destino RA12, precondiciones, efecto máximo y comprobación SELECT desde conexión nueva en **Sql/README.md**, sección DOC126 cuentas de prueba. Después se prepara el scope read del runner existente y se autentican únicamente las cuentas identificadas/autorizadas, con contraseñas efímeras en consola privada. La recepción, preparación, confirmación y configuración de plantillas quedan bloqueadas en ese alcance. No hay instalación, siembra o limpieza desde el test.

Revisión de archivos contra metadatos reales y guards; no se ejecutaron SQL26/27 ni se presenta su revisión como una prueba de aplicación. Sólo el diagnóstico y los metadatos se consultaron realmente. No se ha acreditado una nueva navegación autenticada, una cuenta exclusiva, rechazo de la cuenta restringida ni el cierre de gobierno.

| Archivo | SHA256 UTF8/LF entregado |
| --- | --- |
| `26-cuentas-prueba-ra-doc126.sql` | `d77ee1e9ce33bc5aaebed2f1fb6b1802378877b546e89c341e8fcc9eadcd1d24` |
| `27-permiso-cuenta-prueba-ra-doc126.sql` | `09d18d9a3ede530970ac1632fc2d9d0d978ad63c7b4f0798854d95b53dfbfba5` |

Validación de esta preparación: OpenSpec estricto97/97, refinamiento15/15 y diff sin errores. No se repitieron las83 pruebas locales ni las cargas reales anteriores: este corte sólo incorpora SQL manual/documentación, sin modificar sus adaptadores o código productivo.

Se conserva la evidencia anterior por su SHA, sin reinterpretarla como una nueva ejecución. SQL23–25 ya aplicados permanecen intactos; plantilla60, carga9d7a1ee7-f7f8-4592-ad34-0625bb77f80c y datos previos se conservan. No se modificaron código productivo, DLL, Web.config, cuentas o permisos. El siguiente dato necesario son las identidades de las dos cuentas de prueba; generar archivos no acredita su aplicación ni autoriza elegir otras cuentas.
