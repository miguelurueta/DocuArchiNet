# DOC-110: retirada temporal de permiso autorizada por el usuario

## Aceptación confirmada

Resultado aportado por el usuario: **1 passed (22.7 s)**, caso 21.6 s. Reporte saneado doc110-permission-revocation-result.json PASS, cuatro rechazos comprobados; helper iniciado 2026-10-27T15:13:44.850Z y finalizado 2026-10-27T15:13:48.430Z. El intervalo del helper no incluye todo el login y la búsqueda previa.

REVOKED_PROFILE_PERMISSION_DENIED_NEXT_REQUEST_SAME_SESSION confirma 403 ACCESO_DENEGADO en las cuatro operaciones con sesión/token vigentes y editor ausente tras recarga. ORIGINAL_PROFILE_PERMISSION_RESTORED_OTHER_FIELDS_UNCHANGED confirma restauración a 1 y huella de los demás campos idéntica. RESTORED_PERMISSION_SAME_SESSION_PROFILE_HISTORY_UNCHANGED confirma acceso recuperado sin otro login, documentos/historial intactos y editor visible nuevamente.

El registro de recuperación quedó stage RESTORED, restored true, originalPermission 1, actualizado 2026-10-27T15:13:48.0732198Z. Una inspección SELECT independiente posterior confirma permiso en 1, usuario activo inequívoco y una sola fila/PK. No quedó el permiso retirado. No se necesitó ejecutar -Recover.

Esta corrida acredita la vigencia del permiso y su restauración por el mecanismo directo expresamente autorizado. Completa 9.6 bajo esa alternativa de aceptación, sin atribuir entrega o integración a la aplicación externa. La regresión funcional de carga individual conserva su tarea propia 9.3; no se ejecutó una carga para esta comprobación.

Para 7.8, la matriz acumulada queda cubierta: token ausente/ajeno y HTTP 403 reales, expiración/401 reales, revisión/409 reales, múltiples pestañas reales; HTTP 503, respuesta no JSON y transporte sin replay se acreditan localmente con dobles del cliente/editor. No se presenta el 503 simulado como caída real de RA. Referencias: [sesión e inactividad](doc110-sesion-inactividad-preparacion.md), [pestañas y guard previos](doc110-continuacion-20261024.md), [guard administrativo](doc110-guard-proveedores-ampliado-20261027.md) y tests/doc110-perfil-importacion-client.test.cjs / tests/doc110-perfil-importacion-editor.test.cjs. 7.8 completa con esta distinción de evidencia. 7.6 conserva sus criterios de combinaciones/separación de facultades aún pendientes.

Las secciones inferiores conservan la preparación y sus límites anteriores a este PASS; los estados NO_EJECUTADO quedan superados únicamente para la prueba aquí identificada.

El usuario confirma que la aplicación administradora externa no está configurada y queda fuera de este proyecto; autoriza al agente a retirar temporalmente el permiso y realizar la prueba. Esta instrucción autoriza únicamente el cambio reversible descrito aquí, no concesiones masivas, DDL, cambios de SII ni un endpoint de administración nuevo.

## Cuenta, dato y precondiciones

- Cuenta fija LUZ.AGUILERA. El runner rechaza otra cuenta antes de iniciar el auxiliar de BD.
- Tabla RA remit_dest_interno_perfil_produccion, campo ADMINISTRAR_PERFILES_IMPORTACION; transición 1 → 0 → 1.
- RA se resuelve desde el módulo GESTOR DOCUMENTAL activo del catálogo configurado en Web.config, igual que las consultas de control previas. Las cadenas se materializan solo en memoria y no se imprimen ni persisten.
- Se exige módulo inequívoco, usuario activo inequívoco, una sola fila de permisos y PK simple. La consulta SELECT previa confirmó usuario 141, empresa 2 y permiso inicial 1. No se modificó aún el permiso.
- El UPDATE parametriza PK, referencia de usuario y valor anterior esperado. Solo asigna el campo aprobado. No inserta ni elimina filas ni altera otros permisos.

Se calcula una huella SHA-256 de todos los demás campos de la fila, sin persistir su contenido. Debe conservarse durante retirada y restauración. El registro de recuperación incluye solo referencias, PK, valor original 1, huella de otros campos, huella del ambiente, etapa y fecha; no incluye credenciales/cadena de conexión.

## Recorrido HTTP y UI

Modo --permission-revocation del runner existente. Tras login y búsqueda, lee perfil/historial originales y confirma acceso administrativo. Retira el permiso por BD y, sin cerrar sesión ni volver a iniciar login, exige 403 ACCESO_DENEGADO sin datos en listado, detalle, historial y GuardarBorrador. El guardado usa versionId 0 y configuración null para que tampoco represente una escritura válida si falla autorización. Recarga la pantalla y exige que no emita el editor de perfiles.

Restaura siempre el campo original. En la misma sesión/token, exige acceso recuperado, perfil/historial idénticos y editor visible de nuevo. Una respuesta 401 no sustituye el rechazo por revocación: esta prueba requiere sesión vigente.

## Restauración y recuperación

El auxiliar PowerShell espera comandos por una entrada privada del proceso, sin argumentos sensibles. Conserva la conexión/credenciales solo en memoria. Antes del intento de retirada escribe registro de recuperación y marca restauración obligatoria. Restaura en RESTORE y en finally ante error, EOF o cinco minutos sin comando. El padre cierra la entrada incluso si falla Playwright y exige salida confirmada del auxiliar.

La restauración solo cambia el campo aprobado de 0 a su valor original 1, con PK/usuario esperados. No sobrescribe otros campos si cambian; comprueba la huella y declara fallo si difiere. Si se interrumpe forzosamente el proceso o cae la BD, no promete restauración instantánea: queda registro para reconciliar el resultado sin retirar de nuevo el permiso.

Recuperación preparada, solo para el registro y objetivo verificados:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/e2e/scripts/support/doc110-permission-db.ps1 -Recover
```

Valida ambiente, usuario/empresa y PK contra el registro. Si ya está en 1, verifica sin repetir UPDATE. Si está en 0, restaura solo ese campo; otros valores/identidades se rechazan. Esta recuperación no se ha necesitado ni ejecutado durante la preparación.

## Verificación previa y ejecución

```powershell
node --test tools/e2e/tests/doc110-permission-revocation.test.cjs
npm.cmd --prefix tools/e2e run test:doc110:session -- --permission-revocation --headed
```

**4 pruebas locales PASS**, con dobles: flujo positivo, restauración ante caché indebida/sesión perdida/fallo de UI/resultado incierto/cambio de otros campos; rechazo de perfil cambiado y contexto distinto. El auxiliar real pasó -Inspect y READY→EOF con salida limpia, sin escrituras. HTTP y retirada real **NO_EJECUTADO** al preparar esta evidencia.

No se acredita que la aplicación externa esté instalada, modificada o integrada: se prueba la lectura vigente de permisos de este proyecto usando la modificación puntual autorizada como mecanismo de ensayo. La aprobación sustituye el mecanismo externo para esta prueba; no inventa entrega, pantalla ni API ajenas. Las demás comprobaciones de separación de facultades conservan su evidencia y límites propios.
