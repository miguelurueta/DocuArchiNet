# 02 — Contratos, estados y seguridad del challenge

## ROL ESPERADO

Actúa como arquitecto de seguridad y desarrollador senior .NET Framework.

## OBJETIVO

Definir contratos internos compatibles para iniciar, reenviar y verificar un challenge, sin conectar todavía interfaz ni envío real.

## RESTRICCIONES CRITICAS

- Leer `00-contexto-obligatorio.md` y exigir las decisiones cerradas de la etapa 01.
- No copiar `UserAuthContext`, JWT, DTOs o servicios del Core sin adaptación.
- No introducir datos secretos ni contexto serializado completo.
- No finalizar sesión ni enviar correo en esta etapa.

## REQUISITOS POSITIVOS

1. Modelar una identidad inequívoca: usuario interno, login normalizado, módulo, empresa, propósito `LOGIN` y vínculo de sesión no reversible.
2. Definir estados: creado, enviado, consumido, expirado, bloqueado, revocado y fallo de entrega, sin estados ambiguos.
3. Definir respuestas públicas estables para requerido, verificado, inválido, expirado, bloqueado, no disponible y error temporal.
4. Implementar una abstracción criptográfica con OTP aleatorio seguro, HMAC-SHA256 con secreto externo y comparación constante.
5. Definir interfaces pequeñas para configuración central, repositorio, reloj, generador/HMAC, destinatario por módulo y correo existente.
6. Mantener Domain/Application independientes de controles Web Forms, Session, SMTP y SQL.
7. Definir sanitización de logs y correlación sin OTP, correo completo ni identificadores sensibles innecesarios.

## REGLAS DE ANTIRREGRESION

- No cambiar firmas del login público durante esta etapa.
- No tocar recuperación de contraseña ni su contrato de correo.
- Las nuevas clases deben ser internas al nuevo flujo hasta estar integradas deliberadamente.

## PRUEBAS OBLIGATORIAS

Probar generación, HMAC, comparación constante a nivel contractual, expiración con reloj controlado, validación de estados, aislamiento de identidad y ausencia de secretos en serialización/logs. Compilar el proyecto.

## ENTREGABLE FINAL

Entregar contratos, mapa de dependencias, pruebas, compilación, decisiones criptográficas y actualización de documentación. Detenerse si el secreto HMAC no tiene una fuente de configuración segura definida.

## Correcciones opsxj:prompt-review

Estas reglas proceden de `opsxj:prompt-review` y fueron ajustadas a ASP.NET Web Forms, VB.NET, MySQL y la infraestructura real de este repositorio.

## Rol esperado
Aplicar el rol técnico definido por esta etapa y detenerse si el código real contradice sus límites.

## Objetivo
Completar únicamente el objetivo verificable de esta etapa sin adelantar implementación, activación o pruebas de etapas posteriores.

## Restricciones criticas
- No introducir cambios fuera del alcance de Login 2FA declarado en esta carpeta.
- Preservar el login, recuperación de contraseña, contratos públicos y comportamiento de los cuatro módulos cuando 2FA esté desactivado.

## Criterios de aceptacion
- Se cumplen los criterios específicos de la etapa, las pruebas anteriores afectadas continúan pasando y no existe regresión conocida con 2FA desactivado.

## Contexto obligatorio
Leer `00-contexto-obligatorio.md`, la exploración, los entregables de etapas previas y el código vigente de `gestor.aspx`, `Defaul`, `Modelo`, `DTOs`, `Services`, `Infrastructure`, `webservice`, proyecto y pruebas que resulten afectados.

## Pruebas obligatorias
Ejecutar pruebas focales con la infraestructura existente, `msbuild .\\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug` cuando esté disponible y Playwright solo en las etapas y ambientes expresamente autorizados; registrar comandos, códigos de salida y resultados reales.

## Documentacion tecnica
Actualizar únicamente el paquete canónico `Doc/Actualizacion/Login/Implementacion/` y la exploración cuando cambie una decisión comprobada.

## Entregable final
Entregar archivos modificados, pruebas y compilación ejecutadas, documentación, evidencia sanitizada, limitaciones y riesgos coherentes con lo realmente implementado en la etapa.

Exigir MSBuild del proyecto WebForms afectado y registrar comando, código de salida y errores; si MSBuild no está disponible, documentar el bloqueo y una verificación reproducible sin afirmar éxito.

Usar las pruebas existentes del repositorio y `node --test` para contratos CJS cuando corresponda; no introducir Vitest, Testing Library ni otro runner sin necesidad técnica aprobada.

Registrar comandos, códigos de salida, resultados y evidencia sanitizada en `Doc/Actualizacion/Login/Implementacion/04-pruebas-y-evidencia.md`.
