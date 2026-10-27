# DOC-110: proveedor legado sintético fuera del prefijo

Aceptación de 15.19, 2026-10-27. Estado: **PASS ASMX**; 178/178 tareas completas.

El usuario autorizó utilizar las tablas del ambiente actual de pruebas. La inspección anterior no encontró proveedores fuera de `imp_02`; su corrida quedó NOT_RUN, nunca PASS. Para cubrir el contrato persistido anterior se prepara un fixture sintético explícito, sin afirmar que sea un proveedor histórico real.

`tools/e2e/scripts/prepare-doc110-legacy-provider.ps1` verifica por SELECT la identidad autorizada LUZ.AGUILERA, sus permisos vigentes, ausencia del código reservado, tabla física inexistente, tablas InnoDB y ausencia de excepciones configuradas. Sin `-Apply` solo inspecciona. Con `-Apply` inserta exactamente una cabecera inactiva y una versión BORRADOR en una transacción; no modifica permisos ni proveedores previos. Si el código existe se detiene sin modificarlo ni reintentar. No se utiliza para dar de alta proveedores operativos.

Preparación ejecutada: `CREATED`, 2026-10-27T20:34:28.6654089Z. Proveedor **14**, versión **29**, código `E2E-LEGACY-SCOPE-DOC110`. Referencia RA a `doc110_fuera_prefijo_no_existe`, verificada inexistente; revisión de cabecera/versión 1, estado BORRADOR e inactivo. Reporte saneado: `tools/e2e/artifacts/doc110-legacy-provider-fixture-result.json`.

No se crea una tabla, autorización de fuente, publicación o perfil; no se consultan filas de negocio RA/DA ni se cambian configuración o excepciones. El fixture permanece descartable e inactivo. Representa una configuración antigua guardada que la validación actual debe rechazar y que la UI actual no permitiría guardar.

La corrida `--provider-scope --headed` reutiliza el login oficial. Verifica listados de metadatos paginados RA/DA solo con prefijo literal, peticiones de columnas ajenas con 403 y ausencia de datos; luego lee el proveedor persistido, exige `ValidarProveedor` 403 ACCESO_DENEGADO y vuelve a leerlo. Compara el documento completo y exige encontrar este fixture con código/referencias originales, inactivo y BORRADOR. Nunca llama a `EnsayarProveedor`, guardar, publicar o activar. Al superar todo escribe `doc110-provider-scope-result.json` identificando expresamente el fixture sintético.

Cuatro pruebas Node PASS: ausencia de candidato no acredita rechazo; 403 y documento intacto; validación indebidamente permitida falla; fixture ausente o activo falla. Estas comprobaciones del soporte local se distinguen de la aceptación autenticada siguiente.

Resultado real aportado por el usuario: **1 passed (18.2 s)**, caso 17.1 s. Reporte local verificado `doc110-provider-scope-result.json`: PASS, 2026-10-27T20:37:38.791Z, un proveedor comprobado, fixture sintético 14/29, metadatos/peticiones ajenas verificados y documento sin cambios. La corrida emitió SOURCE_PREFIX_PAGINATION_RA/DA, SOURCE_OUTSIDE_LITERAL_PREFIX_DENIED_RA/DA, SOURCE_EGRESOS_METADATA_ONLY, PREEXISTING_PROVIDER_OUTSIDE_SCOPE_VALIDATION_DENIED, PREEXISTING_PROVIDER_UNCHANGED y SYNTHETIC_LEGACY_PROVIDER_DENIED_REMAINS_INACTIVE. También pasó el autocomplete por código/nombre, teclado, resultado vacío y reapertura.

Se cierra 15.19 con un contrato sintético persistido antes de la validación, explícitamente inactivo y BORRADOR. No se atribuye antigüedad histórica al fixture ni PASS a la corrida anterior sin candidato. No se habilitaron excepciones ni se desplegó código del servidor para obtener este resultado. No se ejecutaron consultas de datos del proveedor.
