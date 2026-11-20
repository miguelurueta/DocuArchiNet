# DOC-121 — revisión de la fase 01, 2026-11-20

Entrega contractual de esquema/configuración y archivos SQL manuales. Cambio `doc-121-importacion-excel-01-esquema-contratos-configuracion`, perfil legacy-webforms-vb. El usuario eligió los prompts actuales y reiteró la ejecución manual propia del SQL. Esta revisión no incorpora las ampliaciones de pausa por fila, workers o versionado de datos de fuentes planteadas durante el análisis.

## Resultado y evidencia

Auditor local [revisar-doc121.mjs](revisar-doc121.mjs), **27/27 controles PASS**, y [resultado JSON](doc121-revision-fase01-20261120.json) con huellas SHA-256 del texto normalizado a UTF8/LF. El fingerprint corresponde a los archivos entregados; reviewBase identifica la base anterior a esta entrega. La revisión y evidencia finales se vinculan al SHA comprometido mediante registros OPSXJ locales excluidos de Git.

Comando reproducible desde la raíz:

```powershell
node Doc/Actualizacion/Importacion/Validacion/revisar-doc121.mjs
```

El auditor distingue sentencias ejecutables de comentarios/literales, verifica que 00/04/06 solo consulten, el DDL aditivo de 01/02, la tabla de 22 columnas, sus hashes/nulabilidad/índices acotados y la plantilla inerte de concesiones. Comprueba 8 requisitos, 24 escenarios, 8 decisiones trazables y los contratos de operaciones. Las 195 entradas de ejecución identificadas en la revisión final DOC-120 mantienen sus hashes; se verifica identidad, no se repiten sus pruebas.

No es un parser MySQL ni una prueba de compatibilidad contra un motor en ejecución. Los presupuestos de índice son cálculos conservadores de las columnas ASCII declaradas; no medición de servidor. 04 produce propuestas que requieren revisar datos, índices equivalentes e aislamiento antes de guardarlas como nuevo SQL específico y aplicarlas manualmente.

| Comprobación | Resultado | Alcance |
|---|---|---|
| Auditor de entregables | 27/27 PASS | Léxico/estructura/contratos y hashes locales. |
| Suites existentes refinementService y legacyGovernanceService | 19/19 PASS, 2 archivos | Tooling de gobierno reutilizado; no escritor Excel. |
| openspec validate DOC-121 --strict --no-interactive, usando el nombre del cambio | PASS | Contrato OpenSpec del ticket. |
| opsxj:refine DOC-121 --sync | 15/15 PASS | Ocho decisiones y requisitos distribuidos en design/spec/tasks. |
| Compilación VB/producto | No aplica | No cambian compilables, proyecto o DLL. |
| E2E/QA visual de producto | No aplica en fase 01 | No hay endpoint o recorrido nuevo; se acredita en fases ejecutables posteriores. |
| Instalación y comprobación SELECT de ambiente | PENDIENTE_EJECUCION_MANUAL | No se ejecutó SQL ni se abrió una conexión de base. |

Comandos de gobierno utilizados:

```powershell
npm.cmd --prefix tools/opsxj test -- scripts/lib/refinementService.test.js scripts/lib/legacyGovernanceService.test.js
npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-121 --sync
openspec.cmd validate doc-121-importacion-excel-01-esquema-contratos-configuracion --strict --no-interactive
```

## Revisión técnica de contrato y SQL

01 es exclusivo de RA y deja el permiso deshabilitado. 02 conserva las columnas previas y agrega dos al final. 03 es una sola tabla InnoDB en la DA de los datos, sin FK a legacy/otras bases, CHECK, JSON nativo, trigger o default de fecha no compatible. La preparación nullable permite RECIBIDA; sus invariantes se exigen antes de PREPARADA/CONFIRMADA.

04 no aplica DDL: deriva propuestas desde el tipo, charset, collation, default y comentario actuales de la clave. No toca BENEFICIARIO ni declara vigente el inventario del 11/11. El procedimiento exige cero nulos/vacíos/duplicados, revisar el índice completo y conservar aislamiento. 05 necesita una copia administrativa local con base/cuenta/host concretos y no contiene credenciales. Las comprobaciones 00/06 no leen filas de imp01_xmlconexion.

Los contratos distinguen permiso propio de carga, autorización de fuente y cuenta de conexión; limitan las solicitudes a referencias y el archivo multipart. Validan el archivo completo antes de escribir, fijan huellas, revalidan antes del commit y conservan el resultado histórico del mismo ID. DELETE/INSERT y resultado comparten transacción; CREATE es un efecto estructural separado con identidad verificable. No se promete rollback de DDL, continuación desde fila o repetición segura por timeout.

## Aplicación manual y límites de entrega

Siguiente operación de ambiente: ejecutar [00-preflight-doc121.sql](../Sql/00-preflight-doc121.sql) en las conexiones RA y DA correspondientes y revisar sus resultados. Seguir el [orden y precondiciones](../Sql/README.md); no aplicar todo como un lote ciego. Confirmar cada ejecución/omisión/error. La comprobación posterior del agente será exclusivamente SELECT; una corrección exige otro archivo.

Estado de instalación independiente de las tareas de entrega: **PENDIENTE_EJECUCION_MANUAL**. Archivos generados, revisión estructural o un commit no significan SQL aplicado. No se acreditan aún carga XLS/XLSX, capacidad real, rollback MySQL, concurrencia de escritor, permisos de cuentas del ambiente, índices específicos instalados o regresión real de SII/carpetas.

La entrega conserva los límites aceptados: subida completa, validación integral y confirmación transaccional sin pausa por fila. El reemplazo de una fuente durante una importación dependiente no obtiene versionado de datos por estos contratos; la aceptación posterior debe conservar y documentar ese alcance. Jira sigue En curso; archivo OpenSpec y cierre no forman parte de este registro.
