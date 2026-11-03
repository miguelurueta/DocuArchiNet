# DOC-114: PLAN-03 propuesto

Siete intentos en nivel 7, gabinete 10, publicaciones 40/42; hasta dos planes nuevos conservados. Se preservan los planes 11–21. Sin confirmaciones, lotes, documentos ni cambios de perfiles/publicaciones. Esta matriz incluye una revocación temporal específica, ya autorizada y pendiente de ejecución.

| Orden | Caso | Resultado esperado |
| --- | --- | --- |
| 1 | Preparación base válida (40) | Plan nuevo, valores/persistencia verificados |
| 2 | Resultado inyectado | SOLICITUD_INVALIDA, sin plan nuevo |
| 3 | Snapshot inyectado | SOLICITUD_INVALIDA, sin plan nuevo |
| 4 | Nivel inexistente 4294967295 | ACCESO_DENEGADO, sin plan nuevo |
| 5 | Versión inexistente 4294967295 | ACCESO_DENEGADO, sin plan nuevo |
| 6 | Publicación compatible distinta (42) | Segundo plan nuevo; el anterior se conserva |
| 7 | Misma solicitud base con permiso revocado | ACCESO_DENEGADO, sin plan nuevo; restauración comprobada |

Cambio temporal exacto: `IMPORTAR_CARPETAS` en la fila de perfil de la cuenta de pruebas LUZ.AGUILERA, usuario 141 / empresa 2, de **1 → 0 → 1**. Máximo dos UPDATE mediante el puente administrativo existente; los controles MySQL aportados por el operador siguen siendo exclusivamente SELECT. El bloqueo afecta a la importación de esa cuenta durante el último rechazo. La base administrativa se vincula al entorno operativo usando el ID y huella de la preparación base recién creada, sin asumir IDs futuros.

El puente exige usuario/fila/clave primaria únicos, permiso inicialmente activo y hash de todos los otros campos. Registra recuperación antes de revocar; usa compare-and-set y restaura en finally/cierre. No se acepta PASS hasta comprobar permiso restaurado, otros campos intactos y reapertura de importación. Cualquier recuperación previa pendiente detiene la prueba. No se toca Web.config, permisos administrativos, otras cuentas ni catálogos.

[Propuesta y solicitudes exactas](doc114-plan03-proposal-20261103.json). Estado: autorización específica PLAN-03 registrada para los siete intentos y esta revocación/restauración; ejecución real pendiente. La autorización PLAN-02 quedó separada y sus siete intentos se consumieron.

Verificaciones locales: contrato y --check PASS (siete intentos); consumidor compartido de revocación 4/4 PASS; prueba PowerShell AST del puente con proveedor en memoria PASS, sin inicialización, conexión o SQL reales. No equivalen a ejecución/recuperación real.

## PLAN-03 autorizado y listo — 2026-11-03

El usuario respondió “autorizado” a la propuesta específica de siete intentos, hasta dos planes nuevos y revocación/restauración temporal IMPORTAR_CARPETAS 1→0→1 para LUZ.AGUILERA, usuario 141/empresa 2. La autorización se vincula al recurso revisado SHA256 0cb40d8e745bafc5aef6bb440316272d5ff082c942e66e99584c18d484fb1db3. No se amplía a otras cuentas, permisos, publicaciones o efectos documentales.

Consola preparada con revisión previa del diario de recuperación, permiso activo/fila única, cuenta/empresa, once planes preservados, publicaciones/reglas 40/42 y referencias negativas ausentes. Backend debe conservar el SHA de PLAN-02. El soporte compartido guarda fases PERMISSION_READY/REVOKING/REVOKED/RESTORING/RESTORED en el checkpoint, incluso cuando falla la solicitud bajo revocación. El puente y las funciones de restauración son los existentes. Al finalizar la consola exige SELECT del permiso concedido, hash de otros campos intacto, diario restaurado del objetivo correcto y planes 11–21 sin cambio. Ante parada intenta registrar una auditoría SELECT segura del permiso; no modifica campos ajenos ni repite preparaciones.

Regresión 86/86 PASS (incluye consumidor DOC-110), precondiciones 3/3 PASS, prueba PowerShell del puente con proveedor en memoria PASS sin BD; contrato/--check y sintaxis PASS. [Manifiesto local](doc114-plan03-ready-20261103.json). Ninguna revocación/preparación real de PLAN-03 se ha ejecutado; las tareas 6.3 y 7.1 conservan su estado abierto, avance 14/16.
