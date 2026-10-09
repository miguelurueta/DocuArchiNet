## Why

El segundo factor necesita entregar OTP usando la configuración SMTP vigente sin acoplarse al correo legacy, elegir arbitrariamente entre varias filas activas ni filtrar secretos ante fallos.

## What Changes

- Agregar modelos y puertos SMTP tipados compatibles con los contratos DOC-91.
- Leer explícitamente la fila activa de `Config_Smpt_Side` usando las abstracciones ADO.NET existentes.
- Rechazar configuración ausente, ambigua o inválida antes de abrir una conexión SMTP.
- Agregar un transporte exclusivo para OTP con timeout acotado, recursos descartables y errores sanitizados.
- Mantener sin cambios `ClassCorreo`, recuperación de contraseña, login, UI, ASMX y esquema MySQL.

## Jira Details

> # 03 — Infraestructura SMTP dedicada para 2FA
> 
> ## ROL ESPERADO
> 
> Actúa como arquitecto senior de infraestructura y desarrollador VB.NET.
> 
> ## OBJETIVO
> 
> Implementar, dentro del OpenSpec autónomo de esta Jira, un lector tipado de `Config_Smpt_Side` y un transporte SMTP exclusivo para OTP.
> 
> ## CONTEXTO
> 
> Lee la exploración y contrasta `radicador/ClassCorreo.vb`, `ClassRaEnvioCorrespondencia.vb`, las abstracciones ADO.NET y el `.vbproj`. Reutiliza la conexión existente y conserva completamente intacta la clase de correo legacy.
> 
> ## PRECONDICIONES DE RUTAS
> 
> Inventaría en `design.md` los archivos exactos, clases y firmas antes de crearlos. Usa:
> 
> - `Modelo/Login/SegundoFactor/` solo para puertos/modelos de configuración y resultados independientes de SMTP.
> - `Infrastructure/Repositories/Login/SegundoFactor/` para el lector MySQL de `Config_Smpt_Side`.
> - `Infrastructure/Login/SegundoFactor/Smtp/` para `MailMessage`, `SmtpClient` y mapeo técnico.
> - `tests/` para contratos y dobles; `GestionDocumental-Docuarchi.net.vbproj` para todas las altas.
> 
> No crear componentes SMTP en `Services/Login/SegundoFactor/`, `DTOs/Login/SegundoFactor/`, `webservice/WebServiceLoginSegundoFactor.asmx(.vb)` o `radicador`; `radicador/ClassCorreo.vb` y sus rutas permanecen fuera del diff funcional.
> 
> ## REQUISITOS POSITIVOS
> 
> Implementa:
> 
> 1. Modelo inmutable de configuración SMTP y resultados sanitizados `Submitted`, `Disabled`, `InvalidConfiguration`, `AmbiguousConfiguration`, `Failed`.
> 2. Repositorio que use el snapshot de conexión de Radicación ya resuelto por Presentation y seleccione columnas explícitas de `Config_Smpt_Side` con `ESTADO_ENVIO = 1`.
> 3. Regla exacta: cero filas = `Disabled`; una = validar; más de una = `AmbiguousConfiguration`. Nunca elegir la primera.
> 4. Validación de host, puerto, remitente, banderas 0/1, credenciales y timeout antes de abrir SMTP.
> 5. Transporte con `Using` para `MailMessage`/`SmtpClient`, SSL y credenciales según tabla, cuerpo OTP mínimo y resultado sin detalles internos.
> 6. Conversión observable `SMTP_TIEMPO * 100000` con aritmética comprobada y máximo efectivo 120.000 ms. Leer `DOMINIO_SMTP`, pero no aplicarlo a `NetworkCredential` mientras no exista evidencia de uso legacy.
> 
> ## Flujo obligatorio
> 
> ```text
> Application
>   -> ISecondFactorSmtpConfigurationRepository
>   -> IDataExecutor + snapshot de conexión existente
>   -> SELECT explícito WHERE ESTADO_ENVIO = 1
>   -> 0 / 1 / múltiples filas
>   -> validar configuración única
>   -> ISecondFactorSmtpTransport
>   -> MailMessage + SmtpClient
>   -> Submitted o Failed sanitizado
> ```
> 
> El transporte no crea/revoca challenges, no maneja cooldown, no autentica y no decide si se omite 2FA.
> 
> ## RESTRICCIONES CRITICAS Y REGLAS DE ANTIRREGRESION
> 
> - No modificar ni invocar `ClassCorreo`; no cambiar recuperación de contraseña o sus llamadores.
> - Preservar firmas y resultados de todos los envíos existentes; el transporte 2FA no debe convertirse en fallback de recorridos legacy.
> - No crear configuración paralela, valores SMTP embebidos, fallback permisivo, `SELECT *`, `Task.Run`, fire-and-forget, cola inexistente ni dependencia nueva.
> - No registrar/retornar contraseña SMTP, destinatario completo, OTP, cuerpo, conexión o texto del servidor.
> - No enviar correo real sin autorización vigente de ambiente, cuenta y buzón.
> 
> ## PRUEBAS OBLIGATORIAS
> 
> Con dobles sin red, prueba cero/una/múltiples filas, columnas nulas, puertos y banderas inválidos, overflow/cota de timeout, SSL, credenciales, disposición de recursos y sanitización de excepciones. Agrega una prueba que demuestre que `ClassCorreo.vb` y sus recorridos no fueron alterados. Ejecuta pruebas focales, regresiones previas y MSBuild.
> 
> Si se autoriza envío real descartable, úsalo solo como integración adicional y no expongas el correo/OTP; si no se autoriza, no lo simules como exitoso.
> 
> ## CRITERIOS DE ACEPTACION
> 
> - 2FA dispone de una única infraestructura SMTP nueva y tipada sobre la fila `ESTADO_ENVIO = 1`.
> - Configuración ausente/ambigua/inválida falla de forma cerrada.
> - Los recursos se liberan y los errores públicos no filtran secretos.
> - El correo legacy tiene diff funcional cero.
> - El OpenSpec contiene decisiones, requisitos, tareas y trazabilidad.
> 
> ## DOCUMENTACION TECNICA
> 
> Actualiza el inventario existente de correo/configuración y documenta el contrato de cada interfaz: nombre, parámetros/tipos, retorno tipado, estados/error, columnas, validaciones y semántica del timeout. Registra evidencia en `Doc/Actualizacion/Login/Implementacion/<JIRA>/`.
> 
> ## ENTREGABLE FINAL
> 
> Entrega clases, interfaces, firmas, pruebas unitarias con rutas/casos, compilación, comandos y evidencia real o bloqueo explícito del envío integrado.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DOBLE, FACTOR, SMTP

## Capabilities

### New Capabilities
- `segundo-factor-smtp`: Resolución y transporte SMTP interno, tipado y fail-closed para OTP.

### Modified Capabilities
- 

## Impact

- Nuevas fuentes en `Modelo/Login/SegundoFactor`, `Infrastructure/Repositories/Login/SegundoFactor` e `Infrastructure/Login/SegundoFactor/Smtp`.
- Alta de fuentes en el `.vbproj`, pruebas sin red y documentación DOC-93.
- Sin endpoint, UI, activación del login, cambio de datos/esquema ni modificación del correo legacy.
