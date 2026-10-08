# 05 — Envío OTP mediante la función de correo existente

## ROL ESPERADO

Actúa como desarrollador senior responsable de integrar correo sin duplicar infraestructura SMTP.

## OBJETIVO

Implementar un adaptador de aplicación que envíe el OTP usando exclusivamente `ClassCorreo.Envio_Correo_recuperacion_pasword`.

## RESTRICCIONES CRITICAS

- Leer y aplicar `00-contexto-obligatorio.md`.
- No crear otro `SmtpClient`, proveedor, configuración SMTP o cola inexistente.
- No modificar el comportamiento público de recuperación de contraseña.
- No escribir OTP, destinatario completo, credenciales SMTP ni cuerpo sensible en logs.
- No declarar challenge enviado si la función existente reporta fallo.

## REQUISITOS POSITIVOS

1. Encapsular la firma legacy en una interfaz pequeña consumible por Application.
2. Construir asunto/cuerpo aprobados, indicando vigencia sin exponer usuario, contraseña o datos internos.
3. Interpretar correctamente el contrato real de éxito/error devuelto por la función existente.
4. Ante fallo, dejar el challenge en estado coherente, sin habilitar bypass ni autenticar.
5. Enmascarar correo en la respuesta y evitar enumeración de usuarios.
6. Aplicar límites de reenvío definidos y garantizar que solo el último challenge/código sea válido.

## REGLAS DE ANTIRREGRESION

- Los llamadores actuales de recuperación deben seguir compilando y funcionando igual.
- No cambiar configuración SMTP ni plantillas ajenas.
- El adaptador no conoce Session, controles Web Forms ni SQL.

## PRUEBAS OBLIGATORIAS

Usar dobles para éxito, error y excepción sin enviar correo real. Probar sanitización, enmascaramiento, actualización coherente del challenge y no regresión del llamador de recuperación. El envío real requiere autorización separada.

## ENTREGABLE FINAL

Entregar adaptador, pruebas, contrato de errores, evidencia de reutilización de la función existente y documentación. No avanzar si un fallo de correo puede terminar en login de un factor.

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

Preservar firmas y recorridos actuales no relacionados; prohibir endpoints alternos de credenciales, bypass del OTP, duplicación SMTP, SQL dentro del ASMX y cambios silenciosos de configuración para hacer pasar pruebas.

Exigir MSBuild del proyecto WebForms afectado y registrar comando, código de salida y errores; si MSBuild no está disponible, documentar el bloqueo y una verificación reproducible sin afirmar éxito.

Usar las pruebas existentes del repositorio y `node --test` para contratos CJS cuando corresponda; no introducir Vitest, Testing Library ni otro runner sin necesidad técnica aprobada.
