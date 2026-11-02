# DOC-111: nombre efectivo del gabinete y reconciliación del intento 6

Estado posterior: la sesión 13 reconcilió el primer documento y almacenó el segundo; lote 2 COMPLETADO/revisión 25. Su control final detectó una colisión de nombres de columnas del harness, corregida con alias distintos. [Resultado persistido, comprobaciones y siguiente lectura](doc111-lote2-completado-control-20261101.md). No repetir almacenamiento ni reconciliación.

Estado: **121/130**. Corrección y comprobación real de solo lectura PASS; reconciliación transaccional y segundo almacenamiento pendientes de sesión autenticada. No es REC LOTE completo.

## Cambio

El verificador reutilizado consulta la configuración de NOMBRE del mismo gabinete en la lectura del documento por sus IDs. Si es visible, exige nombre original exacto. Si no existe como campo configurado o no es visible, admite únicamente el nombre exacto o vacío/NULL; otro nombre no vacío falla. Metadatos duplicados, visibilidad inválida/NULL configurada o inconsistencia mantienen incertidumbre. El nombre original sigue siendo obligatorio y exacto en inventario e índice electrónico, con todos los IDs, relaciones, clasificación, usuario/empresa, formato e índices efectivos anteriores.

No cambia la visibilidad del gabinete, el documento ya guardado, Web.config ni almacenamiento legacy. La precisión autorizada se registra en D-07/RQ-07 y el contrato de recuperación; sustituye la exigencia incondicional de NOMBRE de gabinete en los cortes históricos.

El runner existente admite `--resume-lot 2 --reconcile-attempt 6`, excluyente con `--close-attempt`. Comprueba lote/ámbito/versión/raíces/archivos y evidencia incierta con IDs persistidos. Invoca `ReconciliarRecurso` existente; solo continúa si confirma los mismos IDs. Comprueba su repetición sin cambios. Omite completamente las fases del primer archivo ya confirmado; materializa y almacena únicamente el segundo archivo NO_INICIADO. Un resultado perdido o incierto detiene el recorrido sin retry. Tras verificar dos documentos, repite solo el nuevo almacenamiento confirmado y reabre; comprueba huella y conservación de IDs del primer documento y ambos expedientes. No prepara/confirma otro lote, recibe otra vez o crea expedientes.

El marcador saneado es `DOC111_RECONCILE_RESUME_RESULT`, separado del cierre sin inicio y de REC completo. No acredita de nuevo la barrera ni fases de corridas anteriores.

## Verificación

- Arnés Framework y coherencia SQL/DDL PASS: campo visible obligatorio, invisible/no configurado vacío o NULL, otro nombre, nombres canónicos de inventario/índice, ambigüedad y visibilidad inválida. Composición/repositorio/reconciliación existentes conservados.
- **44 pruebas** de driver, controles SELECT y sesión compartida PASS. Incluyen reconciliar el primer documento sin invocarlo otra vez, almacenar solo el segundo, bloqueo ante evidencia incompleta y saneamiento de resultados separados.
- Build Debug completo: **0 errores, 308 advertencias existentes**. OpenSpec estricto y selector `--check` para 2/6 PASS.
- SELECT real con el nuevo binario aislado, 2026-11-01 11:11:13 UTC: el verificador productivo comprueba documento 16204/inventario 23293/expediente 648; vista de reconciliación válida y `PuedeReconciliar=True`. Sin checkpoint, mutaciones o writer. Reporte `tools/e2e/artifacts/doc111-attempt6-verification-readonly.json`.
- Los 27 archivos protegidos siguen idénticos a HEAD normalizando finales de línea.

Fuentes: SHA-256 `b3dc84db2813f2adf1f877f860458036d932f839dd5675b1ef9b97fa3e56fbb9`, 110 archivos; manifiesto `doc111-sha-nombre-visible-20261101.json`. DLL desplegada a las 11:10:37 UTC: `73e7bcc9476172ae6cf06be53099d06506b2f6ea89fe6a83b311296b1197f176`. Backup `.tmp/doc111-nombre-visible-deployment-20261101-111037/previous.dll`, SHA-256 `892853f896bf8784c751b73da7ed6c5344d78df8c00c8e84c5bb49dc8a530351`. Salud del sitio en `tools/e2e/artifacts/doc111-nombre-visible-deployment.json`.

## Continuación

Consola de sesión 13 `.tmp/doc111-lote2-reconcile-console-13.ps1`, alcance fijo a lote 2/intento 6, perfil 24/versión 38 y las dos raíces autorizadas. Conserva la autorización previa y solo acepta la DLL/perfil esperados antes de pedir credenciales privadas enmascaradas. Usa la cuenta de aplicación y una cuenta MySQL exclusivamente SELECT para MembershipUsers; nunca solicita URL MySQL ni guarda secretos. Estado/salida saneados en `tools/e2e/artifacts/doc111-lote2-reconcile-session-13.json`.

El lote permanece PENDIENTE_VERIFICACION hasta ejecutar la acción autenticada. El primer documento ya existe; no se puede aplicar cierre sin inicio, resetear estado ni volver a almacenarlo. La continuación real y las nueve tareas de aceptación/revisión permanecen pendientes; no se registra E2E/QA PASS por las pruebas locales o la lectura real.
