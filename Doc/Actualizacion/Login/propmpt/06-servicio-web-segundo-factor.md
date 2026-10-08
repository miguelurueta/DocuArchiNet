# 06 — Servicio web productivo e integración incremental

## ROL ESPERADO

Actúa como arquitecto de APIs ASMX y desarrollador senior de ASP.NET Web Forms, con foco en límites de confianza y pruebas tempranas.

## OBJETIVO

Crear la frontera productiva `WebServiceLoginSegundoFactor.asmx` para consultar el estado seguro, verificar, reenviar y cancelar el segundo factor. Probar esta frontera antes de construir la interfaz visual.

El servicio no sustituye el formulario de login: `gestor.aspx` continúa siendo la única entrada de credenciales y el único origen permitido para crear la preautenticación inicial.

## PATRON DEL REPOSITORIO A REUTILIZAR

- Seguir la forma estructural de `webservice/WebServiceWorkflowModern.asmx.vb`: `ScriptService`, métodos JSON tipados, `WebMethod(EnableSession:=True)`, respuestas seguras y ASMX delgado.
- Reutilizar la separación DTO/Application/Infrastructure creada en etapas anteriores.
- Incluir `.asmx`, code-behind, DTOs y servicios nuevos explícitamente en `GestionDocumental-Docuarchi.net.vbproj` porque el proyecto no usa inclusión automática.
- No agregar los métodos a `WebServiceInicioGestor`: ese servicio tiene otras responsabilidades y contratos legacy.

## CONTRATO PUBLICO PROPUESTO

Confirmar los nombres contra las convenciones reales del proyecto antes de implementarlos:

1. `ConsultarEstadoSegundoFactor(request)` — solo retorna estado público, expiración relativa y destino enmascarado cuando corresponda.
2. `VerificarSegundoFactor(request)` — valida el código, consume atómicamente y solicita la finalización del login una sola vez.
3. `ReenviarSegundoFactor(request)` — aplica límites, invalida el código anterior y usa el adaptador de correo existente.
4. `CancelarSegundoFactor(request)` — revoca el challenge pendiente y limpia únicamente el contexto de preautenticación.

Cada solicitud transporta solo un DTO tipado con nonce antifalsificación y, al verificar, el código. El cliente no selecciona ni envía `challengeId`: un `LoginSecondFactorSessionContextGate` resuelve en servidor la referencia del challenge pendiente ligada a la misma Session. Usuario, módulo, empresa, correo, propósito, vigencia e intentos se obtienen del servidor.

## RESTRICCIONES CRITICAS

- Leer y aplicar `00-contexto-obligatorio.md` y exigir etapas 01–05 verificadas.
- No crear `IniciarSesion`, `ValidarCredenciales` ni otro endpoint que acepte usuario/contraseña; evitar un segundo camino de login.
- No crear endpoints de diagnóstico, administración, listado de challenges o consulta de configuración.
- No aceptar `challengeId`, usuario, módulo, empresa o correo aportados por el navegador como selector del challenge.
- No aceptar un challenge que no esté ligado a la misma sesión WebForms, propósito, usuario, empresa y módulo.
- No confiar únicamente en `EnableSession=True`: exigir POST JSON, nonce de la página de login, validación de origen según capacidades existentes y controles contra session fixation.
- No devolver OTP, hash, correo completo, IDs internos, SQL, stack trace, configuración o diferencias que permitan enumerar usuarios.
- El code-behind ASMX no contiene SQL, SMTP, criptografía ni reglas; solo valida el borde, compone dependencias y delega en Application.
- Capturar excepciones en la frontera y traducirlas a códigos públicos estables sin marcar éxito.
- No habilitar CORS ni acceso cross-origin.

## PRUEBAS INCREMENTALES OBLIGATORIAS

### A. Locales, sin red ni secretos

- Contrato fuente del `.asmx` y alta correcta en el proyecto.
- Métodos, DTOs y serialización JSON con contenedor `d` esperado por ASMX.
- Delegación delgada mediante dobles de Application; ausencia de SQL/SMTP en la frontera.
- Solicitud nula, challenge vacío, código inválido y excepción interna con respuesta sanitizada.
- Context gate y estado/verificación/reenvío/cancelación para Session coincidente, ausente, expirada o sustituida.
- Doble verificación y reenvío concurrente delegados a los controles atómicos del repositorio.

### B. Contrato HTTP temprano

- Sin sesión: todos los métodos bloquean sin revelar estado.
- Sin nonce o con origen inválido: bloqueo fail-closed.
- Método/verbo/payload incorrectos: error seguro, no HTML de excepción.
- La respuesta es JSON tipado y no contiene secretos ni detalles internos.

Estas comprobaciones anónimas pueden ejecutarse contra un ambiente cuya URL esté autorizada. La creación/verificación real de un challenge escribe en base y puede enviar correo; requiere autorización expresa de ambiente, cuenta y buzón de prueba, incluso antes del E2E de la etapa 09.

### C. Integración autorizada sin UI final

Cuando exista autorización, usar el postback real de `gestor.aspx` para crear la preautenticación y llamar directamente al ASMX con el mismo `BrowserContext`. Verificar con `SELECT` que no existe login final antes del OTP, que los intentos cambian correctamente y que el consumo ocurre una sola vez. No crear un atajo para inyectar challenges.

## CRITERIOS DE ACEPTACION

- El ASMX productivo es la misma frontera que consumirá la UI; no existe servicio paralelo de pruebas.
- Una llamada anónima, de otra sesión o sin nonce nunca consulta ni muta un challenge.
- Los cuatro métodos tienen pruebas locales de éxito, bloqueo y error seguro.
- La verificación correcta finaliza como máximo una vez y la incorrecta nunca crea Forms Authentication.
- Se puede probar el contrato HTTP antes de modificar la presentación.
- Las pruebas previas de repositorio, módulos y correo continúan pasando.

## DOCUMENTACION Y ENTREGABLE

Documentar ruta, métodos, DTOs, códigos, autorización, vínculo de sesión, efectos y pruebas en `Doc/Actualizacion/Login/Implementacion/`. Entregar archivos, comandos/resultados, evidencia sanitizada y riesgos. No avanzar a UI si el ASMX permite enumeración, bypass, doble consumo o una ruta alternativa de credenciales.

## Correcciones opsxj:prompt-review

Estas reglas proceden de `opsxj:prompt-review` y fueron ajustadas a ASP.NET Web Forms, VB.NET, MySQL y la infraestructura real de este repositorio.

## Contexto obligatorio
Leer `00-contexto-obligatorio.md`, la exploración, los entregables de etapas previas y el código vigente de `gestor.aspx`, `Defaul`, `Modelo`, `DTOs`, `Services`, `Infrastructure`, `webservice`, proyecto y pruebas que resulten afectados.

## Pruebas obligatorias
Ejecutar pruebas focales con la infraestructura existente, `msbuild .\\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug` cuando esté disponible y Playwright solo en las etapas y ambientes expresamente autorizados; registrar comandos, códigos de salida y resultados reales.

## Documentacion tecnica
Actualizar únicamente el paquete canónico `Doc/Actualizacion/Login/Implementacion/` y la exploración cuando cambie una decisión comprobada.

## Entregable final
Entregar archivos modificados, pruebas y compilación ejecutadas, documentación, evidencia sanitizada, limitaciones y riesgos coherentes con lo realmente implementado en la etapa.

Preservar firmas y recorridos actuales no relacionados; prohibir endpoints alternos de credenciales, bypass del OTP, duplicación SMTP, SQL dentro del ASMX y cambios silenciosos de configuración para hacer pasar pruebas.

Exigir MSBuild del proyecto WebForms afectado y registrar comando, código de salida y errores; si MSBuild no está disponible, documentar el bloqueo y una verificación reproducible sin afirmar éxito.

Usar las pruebas existentes del repositorio y `node --test` para contratos CJS cuando corresponda; no introducir Vitest, Testing Library ni otro runner sin necesidad técnica aprobada.
