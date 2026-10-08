# 09 — E2E autenticado y correo real bajo autorización

## ROL ESPERADO

Actúa como responsable senior de pruebas E2E seguras en ambiente compartido.

## OBJETIVO

Validar el flujo real por módulo, únicamente después de superar la etapa 08 y contar con autorización explícita.

## PRECONDICIONES Y AUTORIZACION

- Leer `00-contexto-obligatorio.md` y `tools/e2e/AGENT-RUNBOOK.md` completos.
- Confirmar por escrito ambiente, URL, base, cuentas autorizadas, buzones de prueba, ventana y responsable.
- Obtener autorización separada para: autenticación real, envío real de correo, escritura de challenge y cualquier cambio temporal de configuración.
- No reutilizar credenciales o cuentas aportadas para otro propósito sin confirmación vigente.

## RESTRICCIONES CRITICAS

- No imprimir ni capturar contraseña, cookie, conexión, OTP completo o contenido sensible.
- Las consultas de control deben ser `SELECT`.
- No cambiar configuración para hacer pasar la prueba; si se autoriza un cambio temporal, registrar estado previo sanitizado y restaurarlo en `finally`.
- No usar usuarios reales no descartables ni correo de terceros.
- No ejecutar carga o paralelismo agresivo en ambiente compartido.

## ESCENARIOS E2E

1. Confirmar login sin 2FA en un módulo configurado como inactivo, sin modificar su configuración.
2. Para cada módulo autorizado con 2FA activo: credenciales, recepción del correo, OTP y destino final esperado.
3. Verificar que antes del OTP no existe autenticación final utilizable.
4. Probar OTP incorrecto y reenvío solo si están autorizados y no bloquean una cuenta compartida.
5. Comprobar por `SELECT` el estado del challenge, sin exponer hash/payload.
6. Confirmar que el código anterior no funciona después del reenvío y que el consumido no se reutiliza.
7. Verificar restauración de toda configuración temporal y ausencia de sesiones abiertas al terminar.
8. Confirmar que la UI usa `WebServiceLoginSegundoFactor.asmx` y que sus respuestas coinciden con el contrato ya probado directamente en la etapa 06.

## CRITERIOS DE ACEPTACION

- Cada resultado identifica módulo, escenario y evidencia sanitizada.
- El flujo final conserva permisos y página destino de la línea base.
- No quedan configuraciones temporales, challenges activos de prueba innecesarios ni sesiones abiertas.
- Una falla produce diagnóstico y artefactos seguros; no se parchea el test para forzar éxito.

## ENTREGABLE FINAL

Entregar autorizaciones referenciadas sin secretos, comandos, resultados por módulo, artefactos sanitizados, restauración y riesgos. Si falta autorización, detenerse e informar el bloqueo; no sustituir E2E por una afirmación manual.

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

Exigir MSBuild del proyecto WebForms afectado y registrar comando, código de salida y errores; si MSBuild no está disponible, documentar el bloqueo y una verificación reproducible sin afirmar éxito.
