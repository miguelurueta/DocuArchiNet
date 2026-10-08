# 07 — Interfaz WebForms y finalización segura del login

## ROL ESPERADO

Actúa como desarrollador full stack senior de ASP.NET Web Forms, VB.NET y JavaScript legado accesible.

## OBJETIVO

Integrar la captura, reenvío y verificación del OTP en el login actual y finalizar la autenticación exactamente una vez.

La interfaz debe consumir exclusivamente `WebServiceLoginSegundoFactor.asmx`, ya verificado en la etapa 06; no debe duplicar sus reglas ni volver a implementar operaciones mediante postbacks paralelos.

## RESTRICCIONES CRITICAS

- Leer y aplicar `00-contexto-obligatorio.md`.
- Exigir que la etapa 06 y sus pruebas de contrato estén completas.
- Mantener el formulario actual cuando 2FA esté desactivado.
- No confiar en campos ocultos para identidad, módulo, empresa, correo, expiración o intentos.
- No almacenar contraseña ni OTP en ViewState, Session persistente, HTML, query string o logs.
- No crear cookie Forms Authentication ni redirigir antes del consumo válido.
- No alterar interfaces internas de otros módulos.

## REQUISITOS POSITIVOS

1. Mostrar el paso OTP solo cuando el servidor responda segundo factor requerido.
2. Conservar en el navegador solo el nonce antifalsificación y el estado mínimo de presentación; la referencia del challenge permanece en la Session del servidor.
3. Implementar consulta de estado, verificación, reenvío y cancelación consumiendo los DTOs y códigos públicos del ASMX, con nonce antifalsificación y validación autoritativa en servidor.
4. Impedir doble submit; el servidor continúa siendo la defensa autoritativa.
5. En OTP correcto, consumir atómicamente y ejecutar una sola vez el finalizador legacy del módulo.
6. Restaurar permisos, Session, auditoría, Forms Authentication y redirección equivalentes a la línea base.
7. En refresh/back/challenge expirado, volver a un estado seguro sin reutilizar credenciales ni sesión parcial.
8. Proporcionar mensajes accesibles y neutros, foco correcto y navegación por teclado.

## REGLAS DE ANTIRREGRESION

- El flujo sin 2FA debe conservar markup funcional, postback, validaciones y destino existentes.
- Los cuatro módulos deben finalizar mediante sus reglas actuales, incluida Radicación como módulo explícito.
- Recuperación de contraseña no puede aceptar challenges `LOGIN` ni quedar afectada por la nueva UI.

## PRUEBAS OBLIGATORIAS

Agregar pruebas de servidor y JavaScript para render inicial, requerido/no requerido, contrato ASMX, OTP correcto/incorrecto/expirado, reenvío, cancelación, doble click, refresh/back, CSRF y accesibilidad básica. Reejecutar las pruebas directas del ASMX, la regresión de los cuatro módulos y la compilación.

## ENTREGABLE FINAL

Entregar flujo completo, archivos, pruebas, equivalencia de sesión/redirección, QA manual reproducible y riesgos. No declarar terminado si el cliente puede saltar el OTP o si cambia el login con 2FA apagado.

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

Registrar comandos, códigos de salida, resultados y evidencia sanitizada en `Doc/Actualizacion/Login/Implementacion/04-pruebas-y-evidencia.md`.

Esta etapa no cierra por sí sola el cambio: el E2E real forma parte integral del cierre en la etapa 09. Antes de esa autorización, ejecutar pruebas focales y documentar formalmente por qué no se realizó todavía una corrida autenticada.
