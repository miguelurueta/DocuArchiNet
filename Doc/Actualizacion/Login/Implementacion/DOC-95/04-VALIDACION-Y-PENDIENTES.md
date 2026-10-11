# Validación, riesgos, rollback y pendientes

## Evidencia local ejecutada

| Comando | Resultado observado |
| --- | --- |
| Suites DOC-91 a DOC-95, comportamiento y documentación (`node --test ...`) | código 0; 43/43 pruebas aprobadas después de agregar observabilidad |
| `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal /clp:ErrorsOnly` | código 0 |
| `npm.cmd --prefix tools/e2e run test:doc95:policy` | código 0; 7/7 pruebas aprobadas; incluye escenario de estabilidad secuencial |
| `npm.cmd --prefix tools/e2e run test:doc95:second-factor -- --check --profile profiles/doc95-login-second-factor.profile.example.json` | código 0; registro/perfil válidos; ejecución real `false` |
| `node --test tests/doc95-technical-documentation.test.cjs` | código 0; 5/5 validaciones Mermaid/documentales aprobadas |
| `dotnet run --project ./tools/validation/Doc72SourceValidator/Doc72SourceValidator.csproj -- ./Doc/Actualizacion/Login/Implementacion/DOC-95/diagram-contract.json .` | código 0; 21 declaraciones, 6 relaciones, 3 enums, 28 firmas y 5 tipos aprobados por Roslyn |
| `openspec.cmd validate doc-95-doble-factor-orquestacion --strict` | código 0; cambio válido |
| `node --test tests/login-second-factor-smtp.test.cjs tests/doc93-technical-documentation.test.cjs tools/e2e/tests/doc95-login-second-factor-policy.test.cjs` | código 0; 12/12 pruebas aprobadas después de retirar la referencia incompatible a `ESTADO_BODY` |
| `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /v:minimal /nologo` | código 0 después de la corrección SMTP; permanecen advertencias legacy preexistentes |
| `node --test --test-concurrency=1 tests/login-second-factor-*.test.cjs tests/doc95-technical-documentation.test.cjs` (selección explícita DOC-91–95) | código 0; 27/27 pruebas aprobadas después de completar catálogo, diagnóstico y manejo HMAC |

`npm.cmd --prefix tools/opsxj run opsxj:technical-review -- <README DOC-95>` se ejecutó, pero la versión instalada encaminó el archivo a `prompt-review` y exigió secciones de un prompt, no de documentación técnica. Ese resultado no se presenta como revisión aprobada; se preservaron las puertas específicas Mermaid, Roslyn, comportamiento, compilación y OpenSpec indicadas arriba, además de la revisión cruzada manual del inventario contra código.

## E2E real ejecutada

El 2026-10-11 se ejecutó el runner oficial en TTY después de obtener dentro del propio runner las autorizaciones específicas para ambiente, cuenta, buzón, creación/consumo del challenge, carrera exacta y datos descartables. Antes de la ejecución final se corrigió una incompatibilidad comprobada: `Config_Smpt_Side` no contiene `ESTADO_BODY`; el lector 2FA dejó de seleccionar esa columna y construye el OTP como texto plano, sin DDL ni cambios en `ClassCorreo`.

Evidencia saneada observada:

```text
DOC95_LOGIN_POSTBACK_OK=true
DOC95_PENDING_STATE_OK=true
DOC95_MASKED_DESTINATION_OK=true
DOC95_CONCURRENCY_WINNERS=1
DOC95_AUTH_COOKIE_AFTER_COMPLETION=true
DOC95_E2E_RESULT=PASSED
```

También se ejecutaron dos escenarios independientes adicionales:

```text
DOC95_EXPIRY_STATE_CLEARED=true
DOC95_AUTH_COOKIE_AFTER_EXPIRY=false
DOC95_EXPIRY_RESULT=PASSED
DOC95_OTP_VALIDATION_OK=true
DOC95_AUTH_COOKIE_AFTER_COMPLETION=true
DOC95_VERIFY_RESULT=PASSED
```

El escenario de expiración esperó el tiempo real configurado y comprobó que la Session dejara de ofrecer el challenge sin emitir cookie. El escenario de verificación utilizó un challenge nuevo y confirmó un OTP vigente mediante el ASMX. El transporte conserva UTC para cálculos y persistencia, pero presenta el vencimiento del correo en hora Colombia (`UTC-5`), formato de 12 horas, y separa visualmente el OTP de la puntuación.

El correo 2FA conserva `USUARIO_SMTP` como dirección y usa `Sistema de gestión Docuarchi.net web` como nombre visible del remitente, replicando la convención implementada por `ClassCorreo` sin invocarlo ni modificarlo.

Después de aplicar el nombre visible y el formato horario se repitió la E2E completa: postback, estado pendiente, destino enmascarado, dos verificaciones concurrentes con un solo ganador y cookie posterior resultaron nuevamente `PASSED`. No se conservó el OTP usado.

Después de completar el catálogo seguro de errores, ampliar el diagnóstico a las operaciones posteriores al inicio y separar los fallos HMAC internos del OTP incorrecto, se ejecutó nuevamente la E2E completa autorizada. El resultado fue código 0 con `DOC95_LOGIN_POSTBACK_OK=true`, `DOC95_PENDING_STATE_OK=true`, `DOC95_MASKED_DESTINATION_OK=true`, `DOC95_CONCURRENCY_WINNERS=1`, `DOC95_AUTH_COOKIE_AFTER_COMPLETION=true` y `DOC95_E2E_RESULT=PASSED`. El OTP se capturó oculto y no se registró.

Antes de esa corrida aprobada se observó una falla transitoria `DOC95_PENDING_STATE_NOT_OBSERVED`. El diagnóstico exclusivamente `SELECT` confirmó cero challenges recientes, una configuración SMTP estructuralmente válida, módulo 2FA activo y principal/correo válidos. Por tanto, la operación se detuvo después de resolver el principal y antes de insertar el challenge. No existe evidencia suficiente para atribuirla a IIS, HMAC, conexión u otra causa concreta: la implementación previa descartaba la excepción en un `Catch` genérico. La repetición, sin cambiar lógica de aplicación, resultó aprobada.

Para que una recurrencia sea localizable sin revelar información, se agregó correlación diagnóstica al inicio. Las únicas señales permitidas tienen la forma `Login2FA correlation=<guid> stage=<enum> outcome=<enum>`. No contienen cuenta, ID interno, correo, OTP, challenge, nonce, HMAC, SQL, conexión ni texto/tipo de excepción. La telemetría se protege con `Try/Catch` propio y no puede cambiar la respuesta del login.

No se registraron cuenta, contraseña, OTP, correo completo, cookie, challenge, host SMTP ni cadena de conexión.

Cuando exista autorización, el runner oficial es:

```powershell
npm.cmd --prefix tools/e2e run test:doc95:second-factor -- --profile profiles/<perfil-runtime.json>
```

## Riesgos y límites confirmados

- Session es InProc: un despliegue con múltiples nodos necesita afinidad; la pérdida de Session obliga a reiniciar login.
- `Consumed=1` representa varios terminales; la tabla no distingue completado, revocado, fallo de entrega o fallo de finalización.
- Los efectos de Session/auditoría del finalizador legacy no son compensables si `Complete`, ruta o cookie fallan después; se falla cerrado y el usuario reinicia.
- Las claves HMAC deben proveerse externamente; la E2E utilizó configuración local no versionada y no expuso su valor.
- Una llave HMAC ausente o un valor protegido inválido ya no se registra como OTP incorrecto ni consume intentos; produce falla cerrada y etapa diagnóstica `CODE_VERIFICATION`.
- La interfaz visual OTP no pertenece a DOC-95. Solo se implementó la orquestación y frontera HTTP.
- La prueba documental valida existencia/correspondencia estructural, no demuestra por sí sola fidelidad total del comportamiento ni cobertura completa de casos.
- El destino primario es Windows Event Log `Application`, evento 9501. La fuente existente se comprobó de forma read-only en el host local, pero todavía no se ha comprobado escritura/retención con la identidad real del AppPool; `Trace` es solo fallback.

## Rollback sin SQL

1. Revertir el descriptor/code-behind `WebServiceLoginSegundoFactor.asmx*` y sus entradas de proyecto.
2. Revertir adaptadores de `webservice/Login/SegundoFactor/` agregados por DOC-95.
3. Revertir `SecondFactorAuthenticationService`, recipient resolver, contratos/modelos/DTO y la conexión en preautenticación/ClassGestorSesion.
4. Recompilar y ejecutar las regresiones DOC-91 a DOC-94.

No se requiere rollback de base de datos: DOC-95 no agrega ni modifica tablas, columnas, índices o datos de configuración.

## Checklist de consistencia

- [x] Cinco operaciones internas: `Start`, `GetStatus`, `Verify`, `Resend`, `Cancel`.
- [x] Cuatro endpoints ASMX exactos; Session y JSON habilitados.
- [x] Identidad/challenge/nonce no provienen del cliente.
- [x] Cookie posterior a finalizador y `Complete`.
- [x] Reenvío 60 segundos y máximo dos.
- [x] Cancelación idempotente.
- [x] Rama 2FA apagada preservada para cuatro módulos.
- [x] Sin DDL, `ClassCorreo`, recovery ni otros ASMX modificados.
- [x] Runner E2E registrado con TTY y evidencia saneada.
- [x] E2E real autorizada DOC-95.
- [x] Etapas de preautenticación/inicio instrumentadas con correlación saneada y pruebas de no interferencia.
- [x] Estado, verificación, reenvío, cancelación y compensaciones instrumentados con etapas cerradas; códigos públicos limitados por allowlist y descripciones seguras.
- [x] Escritura/retención del evento 9501 verificada con la identidad real de la aplicación en el host de pruebas.
- [x] Corrida controlada después de reciclado del AppDomain con tres sesiones nuevas consecutivas.

## Pendientes de validación externa

1. Confirmar en cada ambiente objetivo las claves HMAC externas y una única fila SMTP default con `ESTADO_ENVIO=1`.
2. Validar la futura UI de captura OTP en su propio cambio; no se declara cubierta aquí.
3. Corregir o aclarar el enrutamiento de `opsxj:technical-review`, que actualmente ejecuta reglas de `prompt-review` sobre documentación técnica.
4. Repetir la estabilidad en cada ambiente objetivo cuando la identidad/permisos del AppPool o la política de retención de Windows Event Log sean diferentes.

El runner ya soporta `--scenario stability` y pasó su modo `--check`; no recicla IIS por sí mismo. La corrida real crea tres challenges/correos y solicita tres OTP distintos mediante TTY.

## Estabilidad posterior al reciclado

Con autorización explícita se actualizó únicamente la marca de tiempo del `Web.config` de esta aplicación para reciclar su AppDomain. No se recicló `DefaultAppPool`, que es compartido. Inmediatamente después se ejecutó `--scenario stability` en una ventana TTY visible. Las tres iteraciones observaron postback, Session pendiente, destino enmascarado, OTP válido y cookie posterior; el resultado final fue `DOC95_STABILITY_RESULT=PASSED`.

Una consulta read-only al registro `Application` encontró 6 eventos 9501 con formato saneado, agrupados en 3 correlaciones distintas, todos `SUCCEEDED`, sin fallas ni rechazos. Cada sesión produce una señal del orquestador y otra de preautenticación bajo la misma correlación. La consulta solo emitió conteos; no mostró GUID ni mensajes.
