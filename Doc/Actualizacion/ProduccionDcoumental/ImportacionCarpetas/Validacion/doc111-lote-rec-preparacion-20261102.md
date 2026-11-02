# DOC-111: preparación de aceptación LOTE, pendiente de autorización

Avance 124/130, tarea 13.13 abierta. El lote 2 quedó completo y la sesión 21 confirmó por SELECT y reapertura que sus datos permanecen correctos. Sigue faltando la repetición final de comandos de la sesión 13.

## Límite de recuperación

El driver genera claves de operación aleatorias en memoria (`crypto.randomBytes(16)`) y conserva los comandos solo durante esa ejecución. La persistencia guarda `solicitud_hash BINARY(32)`, no la clave original ni el request. `CargarRepeticionDocumental` busca por ese hash; `AlmacenarArchivo` lo calcula incluyendo contexto, lote, archivo, revisión, clave y fase. Por tanto no se puede reconstruir el reenvío exacto de la sesión terminada a partir de sus IDs. No se intentó invertir hashes, extraer memoria o enviar otra clave para atribuirle la repetición anterior.

## Corrida preparada

- Ambiente local y cuenta operativa previamente autorizados; nivel 7, perfil 24/versión 38, gabinete 10.
- Dos nombres nuevos: `DOC111-20261102-LOTE-01` y `DOC111-20261102-LOTE-02`.
- Dos copias del PDF de prueba existente, 677 bytes cada una; índice 14/TIPO=EEE y tipología 79.
- Un nuevo plan/lote y dos expedientes/documentos, conservados al terminar. Lotes 2 y 3 y todos sus recursos se conservan.
- Perfil: `tools/e2e/profiles/doc111-importacion-carpetas-lote-20261102.runtime.json`.
- Consola preparada: `.tmp/doc111-lote-console-22.ps1`, con estado saneado en `tools/e2e/artifacts/doc111-lote-session-22.json`.
- Reutiliza `runRec`, que prueba barrera global, confirmación repetida, creación/valores/almacenamiento, repetición de los comandos originales mientras siguen en memoria, SELECT y reapertura. No se crea otra suite.
- Controles RA/DA mediante MembershipUsers con cuenta exclusivamente SELECT. Usuario de aplicación visible solo en consola, contraseñas enmascaradas; sin persistir secretos.

`--check` y `node --check` PASS. SELECT previo no encuentra expedientes con los nuevos nombres/códigos; lote 2 continúa COMPLETADO. Evidencia: `tools/e2e/artifacts/doc111-lote-newnames-precheck-20261102.json`. Esta comprobación previa no sustituye la revalidación y detección de conflictos del servidor.

La autorización concreta de estos nombres/efectos está pendiente. El launcher exige `doc111-lote-authorization-22.json` con autorización y alcance concordantes antes de pedir credenciales; ese archivo no se ha creado. No se abrió la consola ni se ejecutaron efectos. El runbook exige ambiente/cuenta/nivel/perfil/nombres descartables aprobados para cada corrida real; las autorizaciones anteriores no se amplían a los recursos nuevos por inferencia.
