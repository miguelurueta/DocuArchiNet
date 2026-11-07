# DOC-118 — preparación de COORD-02 y COORD-03

Fecha: 2026-11-07. Origen: D-02/RQ-02, D-04/RQ-04, D-05/RQ-05, D-06/RQ-06 y D-10/RQ-10. Preparación local sobre `efd7a6b4ec4d981ca4ded4f0efbd08ce03082875`; no es evidencia de ejecución real ni autorización de ambiente/cuentas/recursos.

## E2E y perfil existentes localizados

La búsqueda puntual solicitada por el usuario encontró la suite real `tools/e2e/tests/doc111-importacion-carpetas.spec.cjs` y su entrada `npm.cmd --prefix tools/e2e run test:doc111:rec -- ...`. El soporte `tools/e2e/tests/support/doc111-importacion-carpetas.cjs` exporta openSession, createDriver, createSelectControls, runRec, runResume y runReconciliationResume; usa el helper de autenticación común y el puente ODBC existente. DOC118 debe reutilizar esas piezas y el helper acotado de coordinación, sin otro login ni plataforma.

| Dato encontrado | Base reutilizable |
| --- | --- |
| Perfil LOTE local | `tools/e2e/profiles/doc111-importacion-carpetas-lote-20261102.runtime.json` |
| Ambiente | `https://localhost/GestionDocumental-Docuarchi.net/` |
| Módulo/nivel/versión | GESTOR / 7 / 38; perfil validado localmente, disponibilidad actual aún sin consultar |
| DSN RA y DA | MembershipUsers, con credenciales de lectura efímeras |
| PDF existente | `tools/e2e/fixtures/doc110-carga-individual.pdf` |
| Referencia de cuenta/destino histórico | Usuario141/empresa2, gabinete10, tipología79, índice14 TEXTO EEE, documentados en `doc117-alm03c-preparacion-lista-20261106.md` y ejecución histórica de DOC117 |
| TLS local | El perfil tiene ignoreHttpsErrors=true; requiere autorización específica para esa excepción antes de autenticar |

Se encontraron también perfiles locales EXPEDIENTE (versión37) y PARCIAL (versión38). Para C02/C03 se selecciona el LOTE como base de preparación porque ambos originales estarán recibidos y ambas raíces elegibles antes de disputar almacenamiento. Su configuración se validó con el runner existente:

```text
node tools/e2e/scripts/run-doc111-importacion-carpetas.cjs --check --profile tools/e2e/profiles/doc111-importacion-carpetas-lote-20261102.runtime.json --scenario LOTE
DOC111_CHECK=PASS; REAL_EXECUTION=false
```

El check solo valida argumentos, contrato del perfil y fixture. No inicia sesión, consulta datos ni comprueba permisos/publicación actuales. Los perfiles runtime permanecen locales e ignorados; no se copian a Git ni se imprimen secretos. Ya no hace falta pedir al usuario la ruta de ese perfil ni el identificador histórico de cuenta. Su autoridad vigente se comprobará con la sesión autorizada correspondiente.

Los nombres/recursos DOC111/117 del perfil y los reportes no se reutilizan para ejecutar DOC118. Preparar en memoria nombres nuevos `DOC118-C02-A-20261107`/`DOC118-C02-B-20261107` y `DOC118-C03-A-20261107`/`DOC118-C03-B-20261107`, sujetos a comprobar ausencia por preflight autorizado. Los IDs de planes/lotes se obtendrán solo de sus nuevas confirmaciones. No ejecutar runRec completo para simular COORD02/03: su recorrido es secuencial y ya tiene crédito histórico en su alcance.

## Observación externa candidata

El método ordinario está en `workflow/ClassAlmacenamiento.vb`, `AlmacenamientoDocumentoProduccionDocumental` (declaración en línea 8893 de la base revisada). El enlace real es `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.Legacy.vb:InvocarAlmacenamiento`. Los generadores DOC117 producen una copia `ClassAlmacenamiento.Laboratory.vb` con observador dentro del método y no cumplen la restricción DOC118 de mantener ese almacenamiento intacto.

Se propone calificar un depurador administrado externo: breakpoint dentro del cuerpo real, selección del hilo del recurso nuevo, Freeze de ese hilo, Continue para los demás y Thaw al terminar el paso coordinado. No se cambia código fuente, DLL, configuración, datos ni variables mediante el depurador; no se habilita Edit and Continue ni se evalúan funciones del producto. Una pausa global no permite acreditar la competencia.

Se encontró Visual Studio en `C:\Program Files\Microsoft Visual Studio\18\Enterprise\Common7\IDE\devenv.exe` mediante vswhere. Esto solo acredita disponibilidad local del ejecutable. No se abrió Visual Studio, no se adjuntó depurador a un proceso y no se inspeccionó memoria del sitio. La compatibilidad de símbolos, binario, permisos de attach y ejecución de otro hilo sigue SIN_CALIFICAR.

La documentación oficial describe inspección de pila, Freeze/Thaw de hilos seleccionados y breakpoints de función. La aplicación de esas capacidades a este runtime es una propuesta que necesita calificación; no una garantía ya demostrada:

- [Microsoft: ventana Threads, pila y Freeze/Thaw](https://learn.microsoft.com/en-us/visualstudio/debugger/walkthrough-debugging-a-multithreaded-application?view=visualstudio).
- [Microsoft: breakpoints y condiciones](https://learn.microsoft.com/en-us/visualstudio/debugger/using-breakpoints?view=vs-2022).

## Calificación previa, con autorización específica

1. Identificar ambiente de pruebas, proceso exacto, binario y símbolos correspondientes al código aprobado. Resolver y verificar un breakpoint dentro del cuerpo de `AlmacenamientoDocumentoProduccionDocumental`, antes del primer efecto documental. Si no se resuelve con correspondencia verificable, detener la calificación; no recompilar ni desplegar otra variante por inferencia.
2. Correlacionar el recurso nuevo con su solicitud, lote, raíz, archivo e intento a través del llamador. Inspeccionar solo parámetros sintéticos/IDs necesarios y pila; no expandir Session, conexiones, credenciales ni memoria ajena. No guardar dumps ni capturas generales del depurador.
3. Con la primera petición dentro del método, registrar PID, hilo administrado/nativo, símbolo y ubicación. Congelar únicamente ese hilo y reanudar los demás. El estado EN_CURSO por SELECT y un intervalo HTTP abierto son controles adicionales; por sí solos no prueban el interior del método.
4. Verificar que la segunda sesión atiende la petición prevista mientras el hilo sigue congelado dentro de la misma invocación. Registrar el orden causal de freeze → petición B → respuesta B → comprobación del hilo → thaw. Evitar restar relojes de dominios distintos.
5. Descongelar el hilo dentro del límite aprobado, permitir finalizar normalmente y verificar resultado/checkpoint. Si el segundo cliente no progresa, el proceso completo queda detenido o se pierde la correlación, clasificar NO_ACREDITADO; no confundir timeout con bloqueo productivo válido.
6. Retirar breakpoints y verificar que no queda ningún hilo congelado antes de desadjuntar. No matar/reiniciar procesos ni modificar la base para recuperar la prueba. Una operación con efecto incierto conserva su reclamación y detiene nuevos efectos.

Esta coordinación altera deliberadamente el orden de ejecución mediante depurador y debe autorizarse como tal. Su evidencia solo acredita esa variante de planificación; no rendimiento, concurrencia natural, commits simultáneos ni aislamiento XML/SII.

## Sesiones, originales y elegibilidad

Usar dos sesiones independientes de la misma cuenta propietaria autorizada, obtenidas por el helper de autenticación existente. No compartir cookies, contextos de navegador ni tokens de sesión. Otra cuenta no propietaria solo probaría denegación de acceso; la misma sesión puede serializar las peticiones y no satisface la independencia exigida.

Usar lotes nuevos, uno por caso, con dos raíces y un PDF sintético por raíz. Ambos archivos deben estar recibidos y preparados con sus valores efectivos; ambos expedientes ya confirmados. El segundo archivo debe poder avanzar por su política de barrera si no existiera la reclamación activa. No alterar políticas para forzar esa condición.

Antes de cada petición distinta releer la revisión vigente del recurso correspondiente; para repetir exactamente una solicitud se conserva su clave y revisión originales. Las claves de operación y hashes completos permanecen solo en memoria del conductor. Un rechazo por revisión obsoleta, permiso o barrera ajenos a la reclamación no acredita exclusión documental.

La preparación reutiliza perfil, manifiesto, sesión, ASMX/ASHX, PDF y puente ODBC existentes. No crear login alternativo, worker, endpoint de reanudar o segundo repositorio. La consola DOC111 es secuencial y no ejecuta este protocolo completo por sí sola.

Se añadió `tools/e2e/tests/support/doc118-active-call.cjs` como extensión de coordinación acotada. Recibe la primera petición ya autorizada, acciones de la segunda sesión y el observador externo calificado. COORD02 permite exactamente SAME_COMMAND/OTHER_FILE; COORD03 permite STOP. Exige prueba interna correlacionada antes y después de cada acción, conserva PID/hilo, limita el intervalo a 30 segundos, intenta liberar una sola vez incluso ante fallo y nunca reenvía la primera petición. Errores externos se convierten a códigos cerrados sin mensajes privados.

El helper no inicia sesión, adjunta depurador, consulta SQL, instala binarios ni ejecuta por sí solo una corrida real. No es otro motor de producto ni una certificación del observador: recibe callbacks del conductor/operador autorizado. Su resultado siempre conserva businessEvidenceVerified=false e integrationPass=false; el resultado durable, la autoridad y los conteos se verifican separadamente. `firstOutcome=UNKNOWN` exige consulta posterior, nunca replay automático. El adaptador concreto de depurador/sesiones y el procedimiento de presupuesto deben revisarse antes del uso real; no se presenta un CLI listo para ejecutar.

## COORD-02: mismo comando, otro archivo y retorno perdido

1. Registrar snapshot, inventario, revisiones e identidades base del lote nuevo C02. Preparar el avance ALMACENAR del archivo A en sesión A y guardar su solicitud original en memoria.
2. Iniciar A y calificar la pausa dentro del método real. Mantenerla mientras se ejecutan los pasos B autorizados. Registrar instante de inicio persistido y tiempo transcurrido: no hay TTL documental que pueda vencer; no actualizar timestamps ni liberar reservas por SQL.
3. En sesión B repetir exactamente el comando A. Debe obtener historial aún no confirmado o bloqueo seguro sin otra entrada al almacenamiento. No aceptar CONFIRMADO si no existe checkpoint concordante.
4. En sesión B solicitar ALMACENAR del archivo B del mismo lote con su revisión vigente y elegibilidad previamente demostrada. Debe bloquearse sin una segunda entrada al escritor. Si alcanza el breakpoint documental, el caso falla y se corta la ejecución sin promover más efectos.
5. Simular pérdida del retorno al cliente A cortando su transporte, sin cancelar explícitamente el trabajo del servidor. No cambiar el método compartido ni inyectar error en su persistencia. Descongelar A y consultar desde B hasta el límite de lectura aprobado, sin repetir el escritor para averiguar qué ocurrió.
6. Solo si SELECT y consulta pública prueban checkpoint CONFIRMADO concordante, realizar un único replay del comando A original. Debe conservar documento/inventario y no entrar al método de almacenamiento. Si queda EN_CURSO/INCIERTO o faltan IDs/evidencia, no realizar ese replay y registrar incertidumbre/bloqueo.
7. Si el alcance autorizado incluye terminar el archivo B, ejecutar una única fase nueva después de probar la finalización durable de A y ausencia de reclamación activa. Verificar el inventario completo sin repetir A. Si hubo incertidumbre, ese cierre no se ejecuta.

La reclamación «antigua/vencida» se contrasta registrando tiempo transcurrido con A aún activa y comprobando que el guard no usa edad. No se afirma expiración de una lease inexistente. Una permanencia superior al intervalo aprobado exige nueva autorización, no una espera indefinida.

El fallo efecto-sin-checkpoint conserva el crédito histórico DOC117-ALM02 y sus límites. Este plan no autoriza provocarlo otra vez ni reparar el intento53. Cualquier incertidumbre que ocurra en un caso nuevo se documenta como tal; no se declara recuperación probada sin usar una vía soportada y específicamente autorizada.

## COORD-03: detención y consulta posterior

1. Preparar otro lote nuevo C03 con A/B elegibles y snapshot fijo. Iniciar ALMACENAR A desde sesión A y observarlo dentro del método real mediante la pausa calificada.
2. Desde sesión B consultar la revisión vigente del lote y enviar una sola SolicitarDetencion. Exigir señal persistida y respuesta mientras A sigue dentro del método. Un rechazo por revisión obsoleta no acredita la detención y no autoriza reintentar por defecto.
3. Verificar reclamación/IDs conocidos y señal por SELECT; descongelar A. Su finalización puede conservar un efecto CONFIRMADO y debe mantener la señal. El trabajo aún sin completar de B debe impedir declarar el lote COMPLETADO.
4. Cerrar el navegador A y consultar desde B con la referencia conocida. Esa consulta debe conservar snapshot, señal, IDs y estado durable. No interpretar cierre como cancelación ni llamar a un endpoint de reanudar inexistente.
5. Enviar, si está autorizado expresamente, un único intento de avance B con revisión vigente. Debe rechazarse sin entrada al escritor. Registrar el estado final real: con A confirmada, B sin iniciar, sin actividad y señal persistida se espera DETENIDO; cualquier incertidumbre sigue exigiendo verificación.
6. Conservar el lote detenido y sus originales. Este plan no elimina la señal, no cierra una reserva detenida ni promete procesar B posteriormente: el contrato actual no ofrece reanudación general.

## Propuesta de límites para autorización

Estos son límites propuestos, todavía no concedidos. Los descriptores reales de ambiente, cuenta, nivel, versión, destinos y nombres deben completarse desde configuración no sensible y preflight autorizado antes de ejecutar.

| Recurso/acción | Máximo propuesto |
| --- | --- |
| Lotes nuevos | 2: C02 y C03, sin reutilizar históricos |
| Raíces y archivos | 4 raíces y 4 PDFs sintéticos en total |
| Creaciones documentales de expediente | 4, una por raíz nueva |
| Efectos de almacenamiento | 3: A/B de C02 y A de C03; B de C03 permanece sin ejecutar |
| Peticiones ALMACENAR | C02: 5 (A, replay en vuelo, B bloqueada, replay confirmado, B final opcional); C03: 2 (A y B bloqueada) |
| Detenciones | 1 sobre C03 |
| Sesiones de aplicación | 2 independientes por caso, máximo 4; sin reintentos automáticos de login |
| Pausa individual del hilo | 30 segundos por caso, nunca ampliar automáticamente |
| Controles RA/DA | 240 sentencias SELECT para todo el plan, con contador acumulado desde preflight; sin locks, espera ni escrituras |
| Duración | 10 minutos por caso; timeout no prueba ausencia de efecto |

El presupuesto HTTP de negocio incluye preparación y controles públicos, no solo avances:

| Operación | C02 máximo | C03 máximo |
| --- | --- | --- |
| AbrirImportacion | 2 | 2 |
| ListarPerfilesImportacion | 2 | 2 |
| PrepararPlan | 1 | 1 |
| ConfirmarPlan | 1 | 1 |
| Recepción ASHX de archivo | 2 | 2 |
| AvanzarLote/CREAR_EXPEDIENTE | 2 | 2 |
| AvanzarLote/MATERIALIZAR_VALORES | 2 | 2 |
| AvanzarLote/ALMACENAR | 5 | 2 |
| SolicitarDetencion | 0 | 1 |
| ConsultarLote | 20 | 20 |
| ConsultarRecursosLote | 16 | 16 |
| ReconciliarRecurso/CerrarReservaSinInicio/otras mutaciones | 0 | 0 |
| Total HTTP de negocio | 53 | 51 |

Tope adicional de tráfico HTTP total del navegador/API: 250 peticiones por caso, incluyendo login, apertura/postbacks y recursos estáticos. No se suman otras 250 peticiones a las de negocio: son límites simultáneos. Máximo una autenticación por sesión; MFA no completado detiene sin bypass. Las lecturas ASMX y SELECT no son intercambiables en sus presupuestos. No agotar el presupuesto en polling automático; reservar controles para observar el desenlace y proteger recursos ante un corte. El conductor real debe aplicar los contadores antes de despacho; el helper de coordinación no controla tráfico por sí mismo.

## Registro necesario antes de solicitar autorización de ejecución

El perfil local, ambiente, nivel/versión, DSN y referencia histórica de cuenta ya están localizados arriba. Falta verificar su vigencia mediante preflight autorizado, identificar proceso/binario/símbolos y revisar el enlace del observador externo con las dos sesiones y contadores. Los nombres nuevos propuestos deben comprobar ausencia antes de preparar recursos. No se heredan permisos ni autorizaciones de DOC117; los secretos se capturan efímeramente con los helpers existentes y nunca se escriben en este documento.

La persona responsable debe autorizar por separado el ambiente/cuenta y la observación mediante depurador antes de la calificación conectada. Solo después de disponer del conductor, recursos y límites completos se solicita la ejecución de cada caso. La aprobación editorial de DOC118 no sustituye esas autorizaciones.

## Corte y evidencia

Ante entrada inesperada de otro escritor, cambio de autoridad/destino, presupuesto agotado, estado no correlacionable o incertidumbre: impedir nuevas acciones, descongelar el hilo autorizado y observar el resultado dentro del presupuesto restante. No retry, reparación, cleanup, rollback global ni restauración de datos por inferencia.

Conservar únicamente nombres sintéticos aprobados, IDs, revisiones, códigos públicos, conteos, hashes, correspondencia de binario/símbolos, hilo y secuencia causal saneada. No cookies, tokens, credenciales, conexiones, cuerpos HTTP, memoria ni capturas del escritorio. Registrar separadamente autoridad de la sesión B, checkpoint durable, bloqueo, inventario original y alcance instrumental de la prueba.

No declarar completo un caso que mantiene efecto incierto. Los intentos41/lote18 y53/lote21 quedan intactos, al igual que recursos históricos fuera del alcance nuevo.
