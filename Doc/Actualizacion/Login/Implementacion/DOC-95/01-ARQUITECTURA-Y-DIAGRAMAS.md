# Arquitectura y diagramas

## Recorrido implementado

`ClassGestorSesion.InicioAplicacionWebGestorDocumental` valida primero las credenciales con el código legacy. La contraseña se vacía antes de crear `SecondFactorPreAuthenticationRequest`. `SecondFactorPreAuthenticationService.Execute` resuelve la configuración central y el principal del módulo:

- si 2FA está apagado, invoca el mismo `FinalizeLogin` y conserva el redirect legacy;
- si 2FA está activo, exige correo y delega `Start`; no finaliza sesión ni emite cookie.

`SecondFactorAuthenticationService` no referencia HTTP, Session, SQL, SMTP ni Forms Authentication concretos. Coordina puertos. `LoginSecondFactorCompositionFactory` es el composition root WebForms: toma las conexiones ya resueltas de la Session, reutiliza repositorios y adaptadores existentes, y entrega el servicio al postback o al ASMX.

La implementación incorpora observabilidad saneada mediante `ISecondFactorDiagnosticSink`. Un `CorrelationId` aleatorio atraviesa preautenticación y `Start`; `GetStatus`, `Verify`, `Resend` y `Cancel` generan una correlación interna por operación. Solo se registran etapas y resultados definidos por enum. No se entrega al sink login, correo, OTP, challenge, nonce, HMAC, conexión ni excepción. `EventLogSecondFactorDiagnosticSink` persiste únicamente rechazos, fallas y el inicio completado en el registro `Application` de Windows, con evento 9501; si ese destino no está disponible usa `TraceSecondFactorDiagnosticSink`. Ambos adaptadores absorben cualquier fallo propio, por lo que el diagnóstico no cambia el resultado del login.

## Persistencia y estados

No existe una columna de estado lógico. Se reutilizan las señales físicas de `ra_auth_second_factor_challenge`:

| Estado lógico | Señal física existente |
| --- | --- |
| creado/enviado/pendiente | `Consumed=0`, `Attempts<5`, `ExpiresAtUtc` vigente |
| bloqueado por intentos | `Attempts>=5` |
| expirado | `ExpiresAtUtc<=UTC` y consumo defensivo cuando aplica |
| adquirido/finalizado/revocado/fallo de entrega | `Consumed=1` |

Por compatibilidad, esos estados terminales no son distinguibles entre sí en la tabla. `AuthPayloadJson` permanece nulo. Session conserva la distinción temporal necesaria durante la interacción.

## Seguridad y orden de efectos

1. El cliente nunca envía empresa, módulo, usuario interno, login, challenge, nonce, correo ni `returnUrl`.
2. `LoginSecondFactorSessionGate` acepta exclusivamente `PendingSecondFactorContext` de Session y limpia contexto inválido.
3. `Verify` rechaza un formato distinto de seis dígitos antes de leer persistencia.
4. La identidad canónica y el nonce de Session delimitan el HMAC.
   Una llave desconocida o un valor protegido inválido se clasifica como falla interna y no consume intentos; solo una comparación HMAC negativa representa OTP incorrecto.
5. `TryBeginFinalization` realiza la adquisición exclusiva `Consumed=0 -> 1`.
6. Solo el ganador ejecuta `FinalizeLogin`, luego exige `Complete`, resuelve la ruta fija y finalmente emite la cookie.
7. La ruta es `~/Defaul/WebFormInicioDocuarchiGestion.aspx`; no se acepta `returnUrl`.
8. La correlación diagnóstica no se publica al navegador ni se persiste en la tabla de challenges.
9. El DTO conserva `Codigo` y `MensajeVisible`: el mapper admite únicamente códigos de catálogo y reemplaza cualquier código interno desconocido por `SECOND_FACTOR_UNAVAILABLE`.

## Configuración reutilizada

- La configuración del módulo proviene de `gestor_modulos` por DOC-94.
- Los challenges usan `ra_auth_second_factor_challenge` por DOC-92.
- El correo usa `MySqlSecondFactorSmtpConfigurationRepository`, que selecciona la fila activa/default con `ESTADO_ENVIO=1`, y `SecondFactorSmtpEmailSender`/`SecondFactorSmtpTransport` por DOC-93.
- El material HMAC lo obtiene `AppSettingsSecondFactorKeyProvider`. Las claves no están versionadas en `Web.config`; su provisión segura en el ambiente es una precondición de despliegue que no pudo verificarse en el repositorio.

## Inventario de diagramas

| Archivo | Tipo | Propósito | Fuentes principales |
| --- | --- | --- | --- |
| `Diagramas/01-componentes-clases.mmd` | clases | Frontera, factoría, servicio y puertos. | interfaces, servicio, composición y ASMX |
| `Diagramas/02-secuencia-inicio-estado.mmd` | secuencia | Postback, rama activa/inactiva, envío y estado. | `ClassGestorSesion`, preautenticación, servicio y ASMX |
| `Diagramas/03-secuencia-verificacion-concurrencia.mmd` | secuencia | Formato, HMAC, intentos, adquisición, finalizador y cookie. | servicio, repositorio, finalizador y ASMX |
| `Diagramas/04-reenvio-cancelacion-errores.mmd` | flujo | Cooldown, límite, reemplazo, revocación y fallos. | servicio, recipient resolver y mapper |

Cada diagrama declara `Fuentes` y `Referencias CODE`. `diagram-contract.json` es el inventario explícito: un archivo ausente no queda oculto por una búsqueda dinámica.
