# 01 — Especificación ejecutable y línea base

## ROL ESPERADO

Actúa como arquitecto senior responsable de convertir la exploración en una especificación verificable antes de modificar lógica.

## OBJETIVO

Confirmar el flujo real de login de los cuatro módulos, cerrar decisiones funcionales pendientes y capturar la línea base de compatibilidad.

## RESTRICCIONES CRITICAS

- Leer y aplicar `00-contexto-obligatorio.md`.
- No modificar todavía lógica, esquema ni configuración.
- No inventar duración, intentos, reenvío, bloqueo, asunto o contenido funcional del correo.
- No usar únicamente la exploración: contrastar rutas, métodos, Session, cookies, auditoría y consultas con el código actual.

## REQUISITOS POSITIVOS

1. Trazar para cada módulo: entrada, validación de credenciales, resolución del ID/correo, Session, permisos, auditoría, Forms Authentication y redirección.
2. Localizar el punto exacto donde se puede detener el flujo tras credenciales válidas sin dejar estado autenticado parcial.
3. Documentar el contrato de configuración de `RequiereSegundoFactor`, `SecondFactorProviderType` y `SegundoFactorTiempoExpira`, incluidos `NULL`, cero, valores inválidos y proveedor no soportado.
4. Solicitar decisión explícita para: vigencia, máximo de intentos, espera de reenvío, máximo de reenvíos, mensaje ante correo ausente y política ante configuración inválida.
5. Definir estados y códigos públicos sin revelar si un usuario existe ni datos internos.
6. Crear o identificar pruebas de caracterización del login actual con 2FA desactivado.
7. Crear `Doc/Actualizacion/Login/Implementacion/` con índice, arquitectura, contratos y registro de evidencia.

## CRITERIOS DE ACEPTACION

- Los cuatro módulos tienen trazabilidad a métodos y tablas reales.
- Todas las decisiones funcionales bloqueantes están aprobadas o marcadas como bloqueo; ninguna queda convertida tácitamente en un valor técnico.
- Existe una línea base reproducible del comportamiento vigente.
- Se demuestra dónde ocurre el límite preautenticación/finalización sin alterar todavía producción.

## PRUEBAS Y ENTREGA

Ejecutar solamente pruebas locales de caracterización y compilación no mutante. Entregar matriz por módulo, decisiones aprobadas, bloqueos, línea base, comandos y resultados. No avanzar si falta una política que afecte seguridad o compatibilidad.

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

Usar las pruebas existentes del repositorio y `node --test` para contratos CJS cuando corresponda; no introducir Vitest, Testing Library ni otro runner sin necesidad técnica aprobada.
