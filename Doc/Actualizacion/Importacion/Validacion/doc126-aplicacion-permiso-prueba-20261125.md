# DOC-126: permiso de cuenta de prueba aplicado y verificado

Estado **AUTHORIZED_SQL27_APPLIED_VERIFIED**, 2026-11-25T05:34:38.9835229Z. Base a2d6f19237d8eeaa7e1c6e1e1d300ea96fbee653; hashes de fuente y copia aplicada en JSON. El usuario autorizó continuar después de proponer específicamente habilitar sólo cargas en asesor.comercial194 y usar194/196 para E2E de lectura en IIS local. Esta autorización posterior permite aplicar la copia parametrizada de SQL27 fuera del test; no instala ni concede permisos desde el runner.

Se localizaron por SELECT dos cuentas cuyo nombre visible es Prueba Asesor Comercial/Prueba Oficial Cumplimiento. Activas en empresa2, cada una con un perfil y las cinco capacidades de importación inicialmente en0. Se utilizó RA12 del catálogo/configuración vigente, sin nueva conexión Web.config o credenciales persistidas. El administrador ejecutó el SQL revisado con usuario194/196 y confirmación1; parámetros y guion se contrastaron con el hash de la propuesta. Sin triggers de esas tablas. Transacción InnoDB, aislamiento REPEATABLE READ, precondiciones1 y una fila modificada.

| Usuario | Cargas | Carpetas | Perfiles | Fuentes | Proveedores |
| --- | --- | --- | --- | --- | --- |
| asesor.comercial194 | **1** |0|0|0|0|
| oficial.cumplimiento196 |0|0|0|0|0|
| LUZ.AGUILERA141 |1|1|1|1|1|

Una conexión nueva verificó los tres usuarios activos e identidades. Se contrastó la huella de todas las columnas de los78 perfiles de empresa2 antes/después, normalizando en memoria únicamente cargas194 de1 a su0 anterior. Coincidieron íntegramente; todos los demás campos y perfiles permanecieron iguales. Esa normalización es sólo de la copia en memoria para comparar: **no revoca el permiso en MySQL**. La cuenta exclusiva queda habilitada para su prueba autorizada; no hay limpieza automática.

La aplicación administrativa no creó usuarios/perfiles, cambió las otras cuatro capacidades, alteró tablas ni importó filas. SQL23–25 y las cargas previas no se reaplicaron. La copia propuesta con confirmación0 y la aplicada con confirmación1 se conservan localmente excluidas; el SQL público mantiene su plantilla segura. Secretos sólo en memoria, sin imprimirlos o guardarlos.

Reutilizada la E2E read existente con cuentas194/196, perfil HTTPS local/GESTOR y plantilla60 sólo para consulta. El scope de lectura admite proteger los tres destinos DOC126 anteriores:30 tablas en total, sin autorizar ningún efecto de escritura.44/44 pruebas de adaptadores PASS, incluidos guards contra recepción/escritura y conservación de restricciones en modos escritores. No cambió producto, DLL o Web.config.

**60/62 tareas**: la concesión y preparación no cierran6.9 ni7.5. Su ejecución autenticada tiene un resultado separado y requiere las dos contraseñas efímeras en consola privada. Este registro no acredita por sí solo menú exclusivo, rechazo de la cuenta restringida, huellas del recorrido autenticado o cierre de gobierno. No repetir SQL27 para iniciar la prueba; el permiso ya está verificado.
