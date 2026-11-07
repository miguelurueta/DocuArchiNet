# DOC-110: prueba sin esquema en el ambiente autorizado

## Aceptación confirmada

Resultado aportado por el usuario: **1 passed (34.3 s)**, caso 33.1 s. Reporte doc110-missing-schema-result.json PASS, helper iniciado 2026-10-27T20:23:16.371Z y finalizado 2026-10-27T20:23:31.654Z. Ese intervalo no incluye todo el login/búsqueda. El preflight ajustado registró lectura máxima de metadatos FK de 3762 ms.

MISSING_PROFILE_TABLES_ADMIN_HTTP_503_AND_UI_NOTICE confirma ausencia de los tres nombres originales, HTTP 503 CAPACIDAD_NO_DISPONIBLE sin datos y aviso visible en administración. SINGLE_DOCUMENT_STORED_WITHOUT_PROFILE_SCHEMA_SELECT_CONFIRMED confirma un envío con almacenamiento/proyección CONFIRMED del PDF sintético doc110-sin-tablas-perfil.pdf en el expediente descartable 467; SELECT documental aumentó exactamente en 1, sin navegación ni reintento.

PROFILE_TABLE_NAMES_DATA_SCHEMA_AND_IDENTITIES_RESTORED confirma renombrado inverso y coincidencia de conteos/huellas de contenido completo, columnas, índices, FK y próximos AUTO_INCREMENT. Después ListarPerfiles respondió 200. Registro de recuperación: RESTORED, restored true, 2026-10-27T20:23:31.5777000Z.

Inspección SELECT independiente posterior: **3 tablas originales, 0 temporales, 0 FK externas, 24 perfiles, 38 versiones y 155 reglas**, mismos conteos que antes. No se borró ninguna tabla ni se modificaron permisos/SII. El único efecto documental previsto es el PDF nuevo confirmado; no se repite su carga para registrar el cierre.

Se completa 7.5 bajo la autorización expresa de usar el ambiente de pruebas actual. No se atribuye a esta corrida un sitio aislado ni se convierte el primer intento fallido en PASS. Las secciones inferiores conservan preparación y antecedentes. 15.19 permanece independiente y pendiente.

Actualización expresa del usuario: «UTILIZA LAS TABLAS DE LA BASE ... TODO ES PRUEBA». Autoriza usar las tablas del ambiente configurado para esta verificación. Se sustituye el requisito operativo de obtener otro sitio aislado por el ensayo reversible descrito aquí; no se interpreta como permiso para borrar datos ni alterar SII.

## Operación preparada

Modo exclusivo --missing-schema del runner existente, LUZ.AGUILERA. Primero inicia sesión y verifica el contexto; las tablas no se renombran mientras se esperan credenciales. Comprueba tres tablas InnoDB, ausencia de nombres temporales y cero FK externas al conjunto. Captura conteos y huellas de contenido completo, columnas, índices, FK y siguientes AUTO_INCREMENT.

Renombrado temporal en una sola instrucción RENAME TABLE:

| Original | Temporal |
|---|---|
| ra_integracion_perfil | doc110_p75_perfil |
| ra_integracion_perfil_version | doc110_p75_version |
| ra_integracion_perfil_regla | doc110_p75_regla |

Se mantienen todas las filas. No se usa DROP/TRUNCATE/DELETE, no se quitan FK ni se deshabilita FOREIGN_KEY_CHECKS. Los permisos y tablas legacy no cambian. La referencia de sintaxis consultada es el [manual oficial MySQL 5.1, RENAME TABLE](https://docs.oracle.com/cd/E19078-01/mysql/mysql-refman-5.1/sql-syntax.html#rename-table); no se presenta DDL como una transacción de datos con rollback general.

Con los tres nombres originales ausentes, exige ListarPerfiles 503 CAPACIDAD_NO_DISPONIBLE sin datos y aviso visible de indisponibilidad. Reutiliza inspectProductionDocumentUploadPreviewUi del runner DOC-88 oficial, en la misma sesión y base de prueba: un PDF sintético nuevo doc110-sin-tablas-perfil.pdf, expediente descartable 467 y tipo 50 ya autorizados. El SELECT de registro_producion_documental del expediente debe aumentar exactamente en una fila; el handler y la proyección UI deben quedar CONFIRMED. No se repite el envío si falla o queda incierto.

El módulo DOC-88 se puede importar ahora sin ejecutar main automáticamente; su ejecución por CLI y flujo de almacenamiento se conservan. No se crea otro uploader/login/framework ni se cambian sus huellas protegidas.

## Restauración

RENAME inverso en finally, EOF de la entrada del auxiliar o cinco minutos sin comando. Verifica nombres originales, desaparición de temporales y coincidencia de todas las huellas/conteos/contadores iniciales; después ListarPerfiles vuelve a 200. El registro saneado doc110-schema-recovery.json se escribe antes de intentar renombrar; contiene hashes y metadatos, no contenido documental ni secretos.

Si se interrumpe forzosamente el proceso o cae la BD no se promete restauración instantánea. Recuperación específica:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/e2e/scripts/support/doc110-profile-schema-db.ps1 -Recover
```

Verifica ambiente y disposición completa 0 originales/3 temporales o 3 originales/0 temporales; otra combinación exige conciliación manual, sin adivinar renombres. No ejecutar simultáneamente otra corrida que escriba perfiles. No volver a lanzar una carga de resultado incierto para recuperar tablas: usar solo el procedimiento de recuperación.

## Verificación previa

- Ensayo real de renombrado/restauración en doc110_instalacion_prueba con -ProbeSandbox: PASS, datos/esquema/FK/identidades conservados.
- Preflight -Inspect del ambiente actual: PASS, 3 originales, 0 temporales, 0 FK externas; 24 perfiles, 38 versiones y 155 reglas. Un primer preflight falló antes de renombrar; la lectura posterior pasó. No se reintentó DDL sobre la base actual.
- Auxiliar real READY→EOF en la base actual: PASS, sin renombrado/escrituras.
- 18 pruebas Node PASS: flujo sin esquema con dobles, restauración ante fallo UI/carga/resultado incierto/doble registro, detección de cambios de perfiles y contratos de revocación/DOC-88.
- Importación del módulo DOC-88 comprobada sin arranque interactivo ni login automático.

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --missing-schema --headed
```

Estado de la prueba HTTP/carga sin esquema actual: **NO_EJECUTADO**. 7.5 sigue abierta hasta resultado y restauración confirmados. 15.19 conserva su requisito independiente del proveedor fuera del prefijo; no se cierra con este ensayo.

## Primer intento y ajuste del preflight

Corrida aportada por el usuario: falló (caso 35.0 s) al comprobar READY del auxiliar, después del autocomplete y antes de LEGACY_COUNT/HIDE. Reporte FAIL iniciado 2026-10-27T20:12:11.677Z y terminado 20:12:24.686Z, sin uploadEntered ni registro de renombrado. La inspección posterior confirma tres originales, cero temporales y 24/38/155 filas de perfil/versión/regla; no se ejecutó recuperación ni carga.

El mensaje original no conservó la causa interna exacta. Se añaden diagnósticos saneados (tipo/código numérico/etapa, nunca cadena ni mensaje de proveedor) al reporte del preflight. Tres READY→EOF consecutivos pasaron sin cambios. La medición identificó un escaneo global de claves foráneas de 9848 ms, cercano al timeout de 10 s. Se divide la misma comprobación por esquema hijo con parámetro: continúa detectando referencias desde otros esquemas sin una única consulta global. La lectura más lenta observada baja a 3726 ms; no se aumenta timeout ni se agrega reintento automático de DDL/carga.

Preflight ajustado PASS y nuevo -ProbeSandbox PASS con restauración de datos/esquema/identidades; siete pruebas Node de flujo/restauración y revocación PASS. Se conservan los contratos DOC-88 ya comprobados. El auxiliar devuelve salida de error explícita si falla, y el padre distingue fallo previo al renombrado de restauración pendiente. Repetición HTTP/carga sin esquema pendiente; el fallo previo no se convierte en PASS por estas pruebas locales.
