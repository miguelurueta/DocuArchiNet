# Evidencia COR-01/COR-02 — 2026-10-20

Implementación: Repository omite tres PK en INSERT y usa LastInsertedId; reglas serializadas con el ID generado en la misma transacción. Verificador exige INT UNSIGNED y AUTO_INCREMENT solo en PK. Service/resolvedor rechazan IDs GUID y decimales no canónicos. DTO/modelos/editor conservan String decimal sin coerción ni cambios de autorización. No se modificó SII.

SQL 01 corregido para instalación nueva; SQL 03 prepara copias/correspondencias y SQL 04 cambia nombres/FK conservando respaldos. Utilidad manual transforma ReglaId, verifica huellas originales y nuevas y registra auditoría. Contrato y operación en Diseno/08-migracion-identidad-incremental.md y Sql/03-aplicacion-migracion-identidades.md.

## Comprobaciones ejecutadas

- Inventario real SELECT: 5 perfiles, 6 versiones, 13 reglas, 2 publicaciones; cero huérfanos o FK externas detectadas. Evidencia: doc110-inventario-migracion-identidades.json.
- Verificación SELECT de las 2 huellas originales con el conversor: PASS. Evidencia: doc110-huellas-antes-migracion.json. No escribe datos.
- MSBuild del proyecto VB real: PASS con OutputPath/OutDir .tmp/doc110-incremental-build/ e IntermediateOutputPath .tmp/doc110-incremental-obj/. El hash del DLL activo de bin se mantuvo; no despliegue sobre esquema antiguo.
- Node: 26 pruebas PASS, 0 fallos, entre las suites tests/doc110-perfil-importacion-{behavior,client,editor}.test.cjs, seis suites tests/doc88-produccion-*.test.cjs y soporte E2E doc110-safe-request/doc110-publication-fixture. Las de DOC-88 verifican contratos estáticos, no una carga real.
- El harness behavior compila y ejecuta VB/C# Framework; incluye rango 1..4294967295, rechazo de GUID/cero/signos/ceros iniciales, conversión JSON y comparación de hashes con el codec real. Son pruebas locales, no sustituyen transacciones MySQL.
- Parse PowerShell del migrador y node --check de ambos recorridos E2E: PASS. E2E incorpora aserciones de IDs incrementales y pertenencia de versión; no ejecutado aún contra la nueva base.
- openspec.cmd validate doc-110-produccion-importacion-carpeta-perfil --strict: PASS.

## Pendiente, sin atribuir PASS

Tareas 10.16, 10.18 y 10.19: instalación/migración manual en MySQL 5.1, controles posteriores, E2E completo incremental y concurrencia/aislamiento con sesiones independientes. No se aplicaron SQL 03/04 ni modo -Apply. Las corridas E2E históricas con GUID siguen siendo evidencia de la versión anterior. La corrección no está desplegada y DOC-110 no está terminado ni archivado.

## Aplicación local autorizada — 2026-10-20
El usuario autorizó explícitamente al agente a actualizar la base del ambiente de trabajo. Se aplicaron SQL 03, transformación -Apply y SQL 04 en localhost/RA docuarchi, MySQL 5.1.37-community. No se intervino el servidor principal.

Se detuvo el sitio con app_offline.htm durante la ventana, se respaldaron datos/DDL de las tres tablas y DLL anterior en .tmp/doc110-before-incremental, y se conservaron tablas GUID, correspondencias y auditoría en RA. No versionar los respaldos de datos. No se imprimieron conexiones, credenciales ni contenido documental.

Resultado: 5 perfiles, 6 versiones, 13 reglas conservados; verificación posterior de relaciones/JSON/huellas PASS (doc110-migracion-incremental-aplicada.json). Se corrigió el comprobador para normalizar las referencias a UInt32 antes de comparar, debido a la materialización Int32 del proveedor; el tipo físico se comprueba por metadatos.

El repositorio compilado comprobó el esquema completo y leyó los 5 perfiles con pertenencia e identidades incrementales: PASS, solo SELECT. Se desplegó el DLL corregido en bin y se retiró el archivo de mantenimiento creado para esta operación. Respuesta del sitio local HTTP 200. Esto no acredita login, creación incremental real ni concurrencia de usuarios.

SQL de instalación nueva para entrega: Sql/01-perfil-importacion-mysql51.sql. Incluye tres CREATE con PK AUTO_INCREMENT y el ALTER del permiso. Si el servidor principal ya tiene las tablas o el permiso, no ejecutar ese archivo completo: determinar su estado y preparar la migración correspondiente. SQL 03/04 solos no transforman JSON/huellas de publicaciones; no presentar esa combinación como migración autónoma de un solo archivo.

Tarea 10.16 avanza con migración real comprobada, pero sigue abierta por instalación nueva e inserciones pendientes; 10.18/10.19 siguen pendientes. Evidencias anteriores “no aplicada/no desplegada” son históricas y quedan sustituidas para este ambiente local por esta entrada.

Después del despliegue local: Playwright existente, comando tools/e2e/node_modules/.bin/playwright.cmd test tests/doc110-perfil-importacion-anonymous.spec.cjs --workers=1 desde tools/e2e: 13/13 PASS en 9.4s. Login disponible y 12 operaciones rechazan sesión anónima. No acredita el recorrido autenticado incremental.
