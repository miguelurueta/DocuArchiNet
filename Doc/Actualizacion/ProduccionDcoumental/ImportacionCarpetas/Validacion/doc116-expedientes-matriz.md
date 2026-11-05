# DOC-116 — matriz consolidada de cobertura

Estado vigente al 2026-11-05: implementación/consolidación 20/20 tareas y escenarios verificados, incluidas las ventanas internas EXP02 de laboratorio. Entrega formal pendiente. Se conservan los cortes históricos; la copia principal local prevalece y el DLL original está restaurado.

| Requisito | Decisión | Estado | Evidencia y alcance |
| --- | --- | --- | --- |
| RQ-01 | D-01 | VERIFICADO | 9 archivos protegidos coinciden con la línea base local; utilidades divergentes conservadas; [doc116-refinamiento-local-20261105.json](doc116-refinamiento-local-20261105.json) |
| RQ-02 | D-02 | VERIFICADO_CON_REUTILIZACION | Creador/coordinador/repositorio existentes; replay EXP01/02 por IDs; negativos de identidad con dobles; [doc116-exp01-resultados-20261105.json](doc116-exp01-resultados-20261105.json) |
| RQ-03 | D-03 | PASS_REAL | Plan 32/lote 16: 657/658 en nivel 7 tras selección visible 8; relaciones 1082/1083; [doc116-exp01-resultados-20261105.json](doc116-exp01-resultados-20261105.json) |
| RQ-04 | D-04 | PASS_AUTENTICADO_LABORATORIO | Cliente PASS 33/17/659; internas PASS lote 18, 660/661; intento 41 sin recreación; [doc116-exp02-interno-resultados-20261105.json](doc116-exp02-interno-resultados-20261105.json) |
| RQ-05 | D-05 | PASS_REAL_RESTAURADO | Plan 34/lote 18: no propietario nivel 215; fila 629 1→0→1; cero creaciones; [doc116-exp03-resultados-20261105.json](doc116-exp03-resultados-20261105.json) |
| RQ-06 | D-06 | VERIFICADO | Login/cliente/DSN/SELECT/puente existentes; 95 pruebas locales; sin VB/DDL/configuración nuevos; [doc116-exp03-preparacion-20261105.md](doc116-exp03-preparacion-20261105.md) |
| RQ-07 | D-07 | CONSOLIDADO_CON_BLOQUEO | Evidencia/SHAs/límites y bloqueo explícito; no se declara aceptación global ni archivo; [doc116-verificacion-final-20261105.json](doc116-verificacion-final-20261105.json) |

## Casos reales y procedencia

| Caso | Plan/lote | Efectos | Límite de aceptación |
| --- | --- | --- | --- |
| EXP01 | 32/16 | Expedientes 657/658, relaciones 1082/1083 en nivel 7; selección visible 8 y cuatro solicitudes | PASS real del destino/replay; sin documentos definitivos |
| EXP02 cliente | 33/17 | Expediente 659, relación 1084; dos solicitudes; segunda raíz sin crear | PASS CLIENT_RESULT_DISCARDED; caseComplete=false, internalFailure=NO_EJECUTADO |
| EXP03 | 34/18 | Fila 629/nivel 215/no propietario; una creación denegada; permiso 1→0→1 | PASS real; diario RESTORED; cero expedientes nuevos |

Los JSON de resultados contienen huellas del código revisado, recursos, backend y checkpoint original. El manifiesto final contiene SHA256 de cada resultado y cotejo de los nueve archivos protegidos. Las seis tablas operativas anteriores conservaron huellas antes/después de cada ejecución.

## Cobertura local y ejecución

95/95 pruebas Node seleccionadas PASS; incluyen Chromium local interceptado y protocolo PowerShell/SQL con dobles y diario físico. Framework 2/2 PASS del corte previo reutilizado, sin nueva ejecución porque no cambió VB ni el arnés. Una prueba local no se presenta como integración real. Los tres escenarios reales usaron credenciales efímeras en consola privada, DSN MembershipUsers y SELECT.

Comando local: `node --test tests/doc116-exp03-setup.test.cjs tests/doc116-exp01-setup.test.cjs tests/doc116-exp01-discovery.test.cjs tests/doc116-expedientes-contracts.test.cjs tests/doc116-level-permission.test.cjs tests/doc116-tree-selector.test.cjs tools/e2e/tests/doc110-permission-revocation.test.cjs tools/e2e/tests/doc111-importacion-carpetas-driver.test.cjs tools/e2e/tests/doc110-safe-request.test.cjs`. Validación estructural: `openspec.cmd validate doc-116-produccion-importacion-carga-carpeta-expediente --strict`. No repetir las ejecuciones reales concluidas.

## Bloqueo crítico del corte anterior — resuelto

RQ04/D04: EXP02 ventana interna sigue NOT_COMPOSED/NO_EJECUTADO. Falta una vía interna segura con contexto autenticado real para fallo postlegacy/precheckpoint o pérdida de respuesta del checkpoint. Descarte cliente y dobles locales no la sustituyen. Responsable del ambiente y arquitectura deben presentar procedimiento, efectos y recuperación antes de pedir autorización específica. Un cambio de alcance requiere aprobación explícita antes de cambiar el diseño/especificación.

No hay autorización para endpoint/flag público de fallo, borrar resultados, recrear por nombre ni modificar almacenamiento/SII/XML descartado. No se declara aceptación global ni se archiva/cierra Jira. Informe final: [doc116-verificacion-final-20261105.json](doc116-verificacion-final-20261105.json).

Validación de consolidación: OpenSpec estricto PASS y refinamiento 15/15 PASS. Gobierno de cierre no aprobado: faltan revisión de cierre y registros formales unit/manual_qa del commit de entrega. Esto no invalida los resultados conservados de pruebas; no se certifica publicación/frescura de un commit. Informe: `doc116-verificacion-final-20261105.md`.

## Estado vigente tras aceptación EXP02 interno

DOC116-VERIFY-01 resuelto: ambas ventanas autenticadas de laboratorio PASS. RQ04 pasa de parcial a PASS con variante y SHA explícitos. Plan 34/lote 18: raíz 29/intento 40/660/1085 confirma y repite sin duplicación; raíz 30/intento 41/661/1086 conserva incertidumbre y no se recrea. El intento 41 sigue pendiente de verificación por diseño del ensayo, no reconciliado.

DLL original y dos respaldos iguales byte por byte, diario RESTORED. Informe: `doc116-exp02-interno-resultados.md`. 110/110 pruebas locales previas. No hay críticos funcionales abiertos; entrega/publicación y revisión formal de cierre pendientes. Los estados bloqueados previos son históricos.

## Verificación de entrega

Suite aislada 130/130 PASS; prueba de restauración en copia principal 9/9 PASS tras eliminar dependencia de un DLL instalado. No cambia el ejecutor ni se repiten E2E reales por este ajuste de test. La copia principal conserva DLL y respaldos. No se atribuyen las corridas históricas a cambios posteriores. Informe: `doc116-entrega-20261105.md`.
