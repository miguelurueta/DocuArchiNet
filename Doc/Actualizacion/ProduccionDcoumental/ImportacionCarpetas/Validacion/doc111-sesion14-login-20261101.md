# DOC-111: sesión 14 detenida al iniciar sesión

La comprobación final de lectura/reapertura terminó el 2026-11-01 a las 12:21:22 UTC: etapa LOGIN, código `DOC111_SESSION_NOT_ESTABLISHED`. No hubo resultado de verificación final ni reapertura. Se conserva `tools/e2e/artifacts/doc111-lote2-verify-session-14.json` con salida saneada; `realExecutionStarted` indica que arrancó el proceso hijo, no que se hayan ejecutado efectos.

El código significa que, tras el flujo de login, la sonda operativa de apertura devolvió HTTP 401/SESION_REQUERIDA. No permite atribuir el fallo a contraseña, MFA, permisos o una carrera de navegación. No se conservó el postback privado ni se recuperan secretos/cookies de la sesión terminada.

Comprobaciones posteriores de solo lectura:

- 12:24:59 UTC: lote 2 **COMPLETADO, revisión 25**. Expedientes 648/649, documentos 16204/16205 e inventarios 23293/23294 siguen confirmados; ambas raíces COMPLETADA, archivos COMPLETADO y sin reclamaciones activas. El diario sigue terminando en evento 29, LOTE_COMPLETADO.
- 12:25:57 UTC: cuenta histórica 141/empresa 2 activa, permisos de importación y administración concedidos, perfil 24 publicado disponible. Este control no autentica las credenciales concretas introducidas en la sesión 14.
- Diagnósticos Windows del intervalo de la sesión: sin eventos de catálogo Login2FA encontrados; no hay prueba de una causa concreta ni correlación de cuenta.

La variante `--verify-lot 2` no autoriza efectos y su driver prohíbe cargas/fases/reconciliaciones. No se ha repetido el comando ni abierto otra sesión a ciegas. El estado completado y las verificaciones productivas SELECT de ambos documentos, registradas antes, permanecen válidos. La verificación autenticada con el harness corregido y la reapertura siguen pendientes; no es REC/QA PASS.

Siguiente dato solicitado al operador: si puede entrar manualmente a `gestor.aspx` con la misma cuenta de aplicación, sin comunicar credenciales en chat. Esa comparación permitirá separar un rechazo de acceso de un problema del recorrido automatizado antes de elegir otro diagnóstico o solicitar otra entrada privada.

No hubo cambios de código, permisos, configuración, DLL o datos en este diagnóstico. **121/130**.
