# 08 — Pruebas y verificación transversal de no regresión

## ROL ESPERADO

Actúa como arquitecto de calidad y revisor de seguridad independiente de la implementación.

## OBJETIVO

Verificar estructural y funcionalmente la implementación completa antes de cualquier E2E real o activación.

## RESTRICCIONES CRITICAS

- Leer y aplicar `00-contexto-obligatorio.md`.
- No corregir fallas ocultándolas, relajando asserts o eliminando pruebas de línea base.
- No ejecutar E2E autenticado, correo real, carga ni migraciones de ambiente sin autorización.
- No considerar suficiente una prueba visual del camino feliz.

## MATRIZ OBLIGATORIA

Verificar en cada módulo:

1. 2FA apagado con equivalencia de login, Session, permisos, auditoría y redirección.
2. 2FA activo con correo válido y OTP correcto.
3. Credenciales inválidas, usuario inactivo/bloqueado y correo ausente.
4. OTP incorrecto, expirado, revocado, consumido y máximo de intentos.
5. Reenvío, vigencia del último código y límites aprobados.
6. Refresh/back, doble POST y verificaciones concurrentes.
7. Cambio de empresa/módulo/sesión y reutilización cruzada del challenge.
8. Fallo de correo y configuración `NULL`, cero, inválida o proveedor no soportado.
9. Recuperación de contraseña sin regresión ni cruce de propósito.
10. Ausencia de secretos y OTP en logs, errores, HTML, Session persistida y base de datos.

## VERIFICACIONES TECNICAS

- Compilar el proyecto completo.
- Ejecutar pruebas unitarias, integración, repositorio y JavaScript afectadas.
- Ejecutar las pruebas de contrato y seguridad de `WebServiceLoginSegundoFactor.asmx`, incluida sesión/origen/nonce inválidos.
- Revisar consultas atómicas y filas afectadas bajo concurrencia.
- Validar el script de migración y rollback sin aplicarlo a ambientes no autorizados.
- Comparar contratos y comportamiento con la línea base de etapa 01.
- Ejecutar análisis de dependencias para confirmar que Domain/Application no dependen de Web Forms, SMTP o SQL concreto.

## CRITERIOS DE ACEPTACION

- Cero regresiones conocidas con 2FA desactivado.
- Los cuatro módulos tienen evidencia, no inferencia por similitud.
- No existe sesión autenticada antes del OTP.
- Un challenge se consume como máximo una vez.
- No hay bypass ante errores o configuración incompleta.
- Hallazgos críticos o altos bloquean las siguientes etapas.

## ENTREGABLE FINAL

Actualizar la evidencia con comandos, resultados, cobertura por escenario, limitaciones y recomendación fundada: continuar, corregir o bloquear. No afirmar cobertura de un ambiente que no fue probado.

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
