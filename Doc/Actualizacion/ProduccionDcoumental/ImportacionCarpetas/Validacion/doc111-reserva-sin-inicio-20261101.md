# DOC-111: cierre de reserva sin inicio y continuación del lote 2

Corrección posterior: la sesión 11 rechazó el cierre porque el guard confundía ejecutor técnico con usuario. El lote quedó intacto; se corrigió y contrastó con metadatos reales sin escritura. [Estado, pruebas y SHA vigentes](doc111-ejecutor-reserva-20261101.md). Las referencias de binario/fuentes inferiores corresponden al corte anterior.

Estado: **121/130**. Implementación y comprobaciones locales PASS; cierre real y continuación pendientes de credenciales en consola. No es aceptación REC LOTE ni se cierran tareas pendientes.

## Estado real comprobado

SELECT del 2026-11-01 a las 10:24:52 UTC: plan 4/lote 2 EN_CURSO, revisión 13; expedientes 648/649 y relaciones 1073/1074 confirmados. Ambos PDF recibidos. Archivo 3 EN_CURSO, revisión 5, intento activo 5 ALMACENAR/RESERVADO sin evidencia ni evento de inicio. Archivo 4 NO_INICIADO, revisión 3. Se conservan las dos raíces y el historial. Evidencia saneada: `tools/e2e/artifacts/doc111-lot2-readonly.json`.

## Corrección

Se reutilizan el coordinador, repositorio único, guard HTTP, composición de autoridad y contrato cerrado de referencias de reconciliación. La acción explícita `CerrarReservaSinInicio` solo confirma el cierre de una reserva, nunca un documento. Revalida identidad/destino/permisos, la revisión y la vista persistida. El repositorio toma el mismo mutex de lote que `IniciarAlmacenamientoDocumento`, contrasta intento/raíz/archivo/plan y exige un único evento de reserva, sin inicio ni resultado.

Una transacción registra intento RECHAZADO con evidencia `RESERVA_CERRADA_SIN_INICIO` y fin, libera exclusivamente su reclamación del archivo a NO_INICIADO, aumenta revisiones de archivo/lote y agrega evento. No cambia expediente, valores fijados, original ni resultados ajenos. Si un ejecutor ya inició, el cierre falla; si el cierre ganó, la revisión y el CAS impiden que el comando anterior inicie. Repetir el cierre devuelve su prueba sin liberar una reclamación posterior. No se usa antigüedad ni ausencia de documentos como permiso suficiente. La tabla general no permite reiniciar intentos EN_CURSO/INCIERTO.

El runner existente incorpora `--resume-lot 2 --close-attempt 5`, solo LOTE. Usa la sesión y los controles SELECT existentes, verifica que los dos expedientes no tengan documentos en gabinete/inventario/índice antes del cierre, conserva sus identidades, materializa únicamente valores pendientes y solicita cada nuevo almacenamiento una sola vez. Un error o respuesta perdida detiene el recorrido. Verifica resultados, índices, repetición de comandos confirmados y reapertura. No prepara/confirma otro lote ni recibe/crea otra vez. Su resultado es `DOC111_RESUME_RESULT`, separado de `DOC111_REC_RESULT`; no acredita otra vez la barrera inicial ni REC completo.

## Verificación

- Arnés Framework existente y coherencia de UPDATE contra DDL: PASS. Nuevos casos de cierre, estado/evidencia/actor/diario/revisión incorrectos, rechazo del comando anterior, repetición sin liberar propietario nuevo, autoridad vigente y fachada HTTP autenticada con dobles explícitos.
- Driver, controles SELECT y sesión compartida: **40 pruebas PASS**, con continuación del mismo lote, ausencia documental por los tres controles y detención sin retry ante resultado incierto o cierre perdido.
- Cliente DOC-111 y regresiones de carga individual/SII: **34 pruebas PASS**, incluyendo Chromium local con servicios sintéticos; no acreditan carga real individual o importación SII.
- Build Debug completo en `.tmp/doc111-reserva-sin-inicio-build`: **0 errores, 308 advertencias existentes**.
- OpenSpec estricto y selector `--check` con referencias 2/5: PASS, sin ejecución real.
- Los 27 archivos protegidos, incluido Web.config y almacenamiento legacy, siguen idénticos a HEAD normalizando finales de línea.

SHA de fuentes: `7a7a70022753ba20c0d47ebdda67899cd7fee638f1658d4b6d2d2a3a884fc209`, 110 archivos. Manifiesto `doc111-sha-reserva-20261101.json`. HEAD `b2e93b726f221868cf2b82d43200a8e568871117`; workspace sin commit.

## Despliegue y siguiente ejecución

DLL desplegada en el sitio local ya autorizado a las 10:24:00 UTC, SHA-256 `c35831b4086eaf367e28910d5afd49d826ec70061b1855b67b29deb33bf2c8d6`. Backup `.tmp/doc111-reserva-deployment-20261101-102400/previous.dll`, SHA-256 `7d82c02c68ef04b786413332ae3b832521cbf68287e0e8fec774f928b3b5a977`. Login y Producción responden HTTP 200 sin indicadores de error ASP.NET a las 10:24:39 UTC. Reporte `tools/e2e/artifacts/doc111-reserva-deployment.json`. El despliegue no cambió el lote, comprobado después por SELECT.

Consola preparada `.tmp/doc111-lote2-resume-console.ps1`, con ventana persistente y salida saneada a `tools/e2e/artifacts/doc111-lote2-resume-session-11.json`. Conserva la autorización explícita previa del usuario para el ambiente/cuenta/recursos y la continuación del lote 2; exige coincidencia exacta del perfil y DLL antes de pedir credenciales. No repite preguntas de autorización ya respondidas. Solo solicita datos privados enmascarados de aplicación y cuenta MySQL exclusivamente SELECT para los DSN MembershipUsers; nunca URL MySQL, archivo de secretos o datos privados en chat. El guard de pertenencia del lote rechaza otra cuenta.

**Pendiente real:** ejecutar y comprobar cierre 5 y ambos almacenamientos con sesión autenticada; conservar evidencia y revisar cualquier resultado incierto antes de otro comando. Los escenarios EXPEDIENTE/PARCIAL y las demás verificaciones de aceptación siguen pendientes. No se habilita limpieza ni se retiran recursos.
