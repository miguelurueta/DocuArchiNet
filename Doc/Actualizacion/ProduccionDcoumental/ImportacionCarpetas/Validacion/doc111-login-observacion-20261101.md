# DOC-111: diagnóstico del login antes de continuar REC EXPEDIENTE

La sesión 15 terminó a las 16:25:17 UTC con `DOC111_SESSION_NOT_ESTABLISHED`, etapa LOGIN y ninguna referencia de plan/lote. No se ejecutó REC. SELECT de las 16:29:50 UTC confirma lote 2 COMPLETADO/revisión 25 y cero coincidencias para los nuevos expedientes DOC111-20261101-EXP-01/02. La autorización específica para estos recursos permanece registrada; no se repitió el comando.

El resultado disponible solo acredita ausencia de sesión operativa después del login. No demuestra contraseña incorrecta, MFA, cuenta bloqueada ni un problema de permisos. La comprobación de cuenta histórica activa realizada antes no valida los datos privados introducidos concretamente en esta consola.

## Observación segura añadida

DOC-111 reutiliza los callbacks `onPostbackCompleted` y `onDialogCompleted` del helper compartido. Espera el postback antes de ir a Producción y observa los mensajes emitidos por el flujo existente `gestor.aspx.vb`/`Showscripman`. No copia el login, modifica autenticación productiva o reintenta la solicitud.

Conserva exclusivamente códigos cerrados para rechazo explícito de credenciales, cuenta ausente/bloqueada, entrada requerida, segundo factor requerido, rechazo del servicio/HTTP o postback no disponible. Los textos originales, cuerpos HTTP, cookies y credenciales no se imprimen ni guardan. Un diálogo desconocido produce rechazo genérico. La presencia de scripts del widget MFA no basta para afirmar que se pidió segundo factor; se reconoce la llamada de inicio emitida por el servidor. Los redirects se esperan sin intentar leer su cuerpo como una respuesta ordinaria. No se omite ni deshabilita MFA.

## Comprobación y siguiente paso

**56 pruebas locales PASS**, incluidos callbacks reales del helper compartido con dobles, mensajes del servidor, ausencia de texto privado en resultados, rechazo antes de navegar, postback fallido, HTTP de error, redirect válido y login sin mensaje. OpenSpec estricto PASS. No se modificó la DLL ni se recompiló/desplegó el sitio por este cambio del harness.

Manifiesto `doc111-sha-login-diagnostico-20261101.json`, fuentes SHA-256 `b831505b3de48ac03d8921972692c426bbb0faed6a3e8e59ee8ca12773b4f1e6`. DLL activa `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`, cuya correspondencia con la compilación local y 776 métodos de importación previamente validados se registró antes de la sesión 15.

Se prepara sesión 16 mediante `.tmp/doc111-diagnostic-console-16.ps1`, variante `--diagnose` ya existente y perfil EXPEDIENTE autorizado. Solo pide cuenta y contraseña de aplicación en TTY enmascarado; no pide MySQL, no conecta controles ODBC y mantiene efectos/descartables/SELECT deshabilitados. No crea planes, lotes, expedientes ni documentos. Conserva resultado saneado en `tools/e2e/artifacts/doc111-diagnostic-session-16.json`. Un PASS aquí solo comprueba acceso/control, no aceptación REC. Un rechazo permitirá elegir el siguiente paso con evidencia del login sin almacenar mensajes privados.

Estado del flujo: **123/130**, REC EXPEDIENTE pendiente. La carga y los índices aprobados del lote 2 se conservan intactos.
