## Why

DOC-91 a DOC-94 entregaron contratos, seguridad, persistencia compatible, SMTP y preautenticación por módulo, pero el login con 2FA activo todavía se detiene en `SECOND_FACTOR_REQUIRED`: no crea el challenge, no conserva un contexto pendiente ni ofrece una frontera HTTP para verificarlo. DOC-95 conecta esas piezas sin crear tablas o columnas y sin alterar el login cuando 2FA está apagado.

## What Changes

- Agregar un servicio de aplicación que inicia, consulta, verifica, reenvía y cancela el challenge usando exclusivamente los puertos existentes.
- Integrar el inicio del challenge después de que `SecondFactorPreAuthenticationService` resuelve módulo e identidad y antes de cualquier efecto autenticado.
- Extender de forma compatible el contexto pendiente y los contratos internos necesarios para conservar módulo, contador de reenvío, nonce de sesión y ruta local autorizada.
- Crear únicamente `WebServiceLoginSegundoFactor.asmx(.vb)` con las operaciones `ObtenerEstado`, `Verificar`, `Reenviar` y `Cancelar`, todas con Session habilitada y sin identidad aportada por el cliente.
- Emitir la cookie solo después de adquisición exclusiva, finalización legacy exitosa y confirmación del repositorio; devolver únicamente una ruta local fija del servidor.
- Mantener la semántica física de DOC-92: `Consumed=0` representa elegibilidad y `Consumed=1` adquisición/terminalidad. No se inventan estados persistidos que el esquema compartido no puede representar.
- Agregar pruebas locales del servicio y contrato ASMX, regresiones DOC-91 a DOC-94, documentación técnica y una etapa E2E reutilizando `tools/e2e` cuando exista autorización específica.
- Completar el catálogo público de errores con descripciones seguras y observabilidad por etapa para verificación, reenvío y cancelación, sin exponer excepciones ni consumir intentos por fallos de llave HMAC.

## Capabilities

### New Capabilities

- `doble-factor-orquestacion`: ciclo de challenge, finalización exclusiva y frontera ASMX de Login 2FA.

### Modified Capabilities

- `doble-factor-preautenticacion`: cuando 2FA está activo, inicia y guarda un challenge válido antes de responder `SECOND_FACTOR_REQUIRED`.
- `segundo-factor-persistencia`: consume las operaciones existentes con su semántica compatible, sin DDL y con `AuthPayloadJson=NULL`.

## Impact

- Código: `Services/Login/SegundoFactor/`, `Modelo/Login/SegundoFactor/`, `DTOs/Login/SegundoFactor/`, `webservice/Login/SegundoFactor/`, `webservice/WebServiceLoginSegundoFactor.asmx(.vb)`, `Defaul/ClassGestorSesion.vb` y el proyecto VB.
- Datos: escrituras autorizadas únicamente sobre `ra_auth_second_factor_challenge`; sin cambios de esquema ni configuración productiva.
- Compatibilidad: el flujo con `RequiereSegundoFactor=0` o `NULL`, recuperación de contraseña, `ClassCorreo` y los demás ASMX permanecen intactos.
- Operación: una validación real exige autorización vigente de ambiente, cuenta, buzón y datos descartables; su ausencia bloquea el cierre, no se sustituye con mocks.
