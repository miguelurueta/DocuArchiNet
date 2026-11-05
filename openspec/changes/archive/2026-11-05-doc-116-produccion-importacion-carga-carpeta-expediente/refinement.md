<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento DOC-116

## Fuente y alcance

Jira refinado en specs/produccion-importacion-carga-carpeta-expediente/jira-context.md. Perfil legacy-webforms-vb. Revisión: 2026-11-05. Directorio autoritativo: D:/imagenesda/DocuachiNet/DocuArchiNet. El directorio .tmp/doc116-work es aislamiento de Git; no decide qué copia de producto prevalece.

## Contexto inspeccionado

- Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb
- Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.Legacy.vb
- Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Creacion.vb
- Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb
- Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.ResultadoCreacion.vb
- tests/ImportacionCarpetasBehaviorTests.cs
- tools/e2e/scripts/support/doc110-permission-db.ps1
- tools/e2e/tests/support/doc110-permission-revocation.cjs
- tools/e2e/tests/support/doc111-importacion-carpetas.cjs
- tools/e2e/tests/support/authenticated-workflow-session.cjs
- tools/e2e/scripts/support/interactive-e2e-console.cjs

Se inspeccionaron Crear/RegistrarSalida, binding InvocarConservacion con IDs ByRef retenidos en Finally, creación coordinada con histórico antes de reservar y autoridad del nivel. La rama propietaria de ResolverDestino concede acceso por propietario O permiso: un propietario no sirve para demostrar revocación del permiso de nivel. El puente local administra perfiles y difiere del soporte integrado; no se reemplaza.

## Decisiones aprobadas para implementación local

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | La copia principal local prevalece para inspección y revisión. La copia aislada solo protege Git; una divergencia se registra y nunca se resuelve sobrescribiendo producto local. | Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb; tools/e2e/scripts/support/doc110-permission-db.ps1 | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Conservar adaptador, binding, barreras, autoridad y repositorio únicos. Histórico confirmado devuelve identidad sin otro escritor; IDs incompletos o checkpoint dudoso no autorizan recreación ni búsqueda por nombre. | Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb Crear/RegistrarSalida; Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Creacion.vb CrearExpediente | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Preparar dos raíces para A, cambiar la selección visible a B y crear solo bajo A. Comprobar expediente/relación/ciclo/clasificación por SELECT e identidad histórica; no almacenar documentos definitivos. | Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Creacion.vb CrearExpediente; Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.Legacy.vb InvocarConservacion | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Distinguir respuesta confirmada descartada en cliente, fallo después de creación antes del checkpoint y pérdida de respuesta del checkpoint. Los dobles locales no acreditan falla real. No añadir hook productivo o endpoint; si no hay mecanismo interno seguro autorizado, registrar NO_EJECUTADO y bloquear cierre del caso. | tests/ImportacionCarpetasBehaviorTests.cs CreationAdapterCases/CoordinatedCreationCases; Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb RegistrarSalida | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Probar agregar_expediente en ra_pro_permisos_niveles para un actor que NO sea propietario de A y cuya vía efectiva sea esa concesión. IMPORTAR_CARPETAS es una facultad distinta; revocarla no sustituye el caso. Restaurar el valor exacto y comprobar cero creaciones. | Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb ResolverDestino/AutorizarCreacionLegacy | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Reutilizar sesión, safePost, consola, DSN MembershipUsers y controles SELECT. No usar runRec completo porque almacena documentos. El puente local es administrativo y la copia aislada tiene extensión operativa: ninguno acredita revocación de permiso de nivel. Una extensión de soporte requiere contrato cerrado, diario y restauración antes de cualquier ejecución. | tools/e2e/tests/support/doc111-importacion-carpetas.cjs createPlanControls; tools/e2e/tests/support/doc110-permission-revocation.cjs startDatabaseBridge | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | EXP01/02/03 conservan resultado, SHA, comandos, ambiente y procedencia. No hay aceptación real nueva en este refinamiento. No modificar almacenamiento legacy/SII, aislamiento XML descartado, formatos, DDL o configuración; cambiar producto solo tras reproducir un defecto del alcance. | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-matriz-final-20261102.md | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Fuente local y procedencia | Fuente autorizada; Cambios posteriores | Reutilizar código/evidencia y preservar límites |
| RQ-02 | Reutilización del creador | Reejecución confirmada; Identidad insuficiente | Reutilizar código/evidencia y preservar límites |
| RQ-03 | Destino fijo EXP-01 | EXP-01 nivel original; Efectos delimitados | Reutilizar código/evidencia y preservar límites |
| RQ-04 | Incertidumbre EXP-02 | EXP-02 respuesta cliente; EXP-02 ventana interna | No confundir doble o descarte cliente con falla real |
| RQ-05 | Autoridad de nivel EXP-03 | EXP-03 no propietario; Restauración verificable | No revocar facultad equivocada ni usar propietario |
| RQ-06 | Infraestructura y mutaciones acotadas | Presupuesto y transporte; Soporte divergente | Reutilizar código/evidencia y preservar límites |
| RQ-07 | Cierre por evidencia y límites | Caso sin autorización; Cierre trazable | Reutilizar código/evidencia y preservar límites |

## Dependencias de aceptación

EXP01 requiere niveles A/B y publicación compatible, recursos y cuenta autorizados. EXP02 interno no dispone de un hook HTTP de fallo; el constructor Friend soporta pruebas locales, no una licencia para alterar el servidor. Debe demostrarse una vía interna con contexto real antes de autorizar ese caso; si no es viable, no se ejecuta y no acredita cierre. EXP03 requiere fila efectiva de permiso de un actor no propietario y puente/manual seguro con diario. No se eligen cuentas, nombres o IDs ni se cambia su autorización en este corte.

## Resultado del refinamiento

El usuario indicó «continua» después del informe que presentó este refinamiento para aprobación. Se registra conformidad editorial para trabajo local; no se atribuye una autorización de ambiente, credenciales, creación real o revocación de permisos. Los resultados de integración continúan sin ejecutar hasta sus autorizaciones específicas.
