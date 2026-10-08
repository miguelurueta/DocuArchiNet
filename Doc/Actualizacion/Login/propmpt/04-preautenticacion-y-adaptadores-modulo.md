# 04 — Preautenticación y adaptadores para los cuatro módulos

## ROL ESPERADO

Actúa como arquitecto de aplicación y especialista en el login legacy de DocuArchiNet.

## OBJETIVO

Separar credenciales válidas de sesión autenticada e implementar la resolución segura de identidad/correo para los cuatro módulos.

## RESTRICCIONES CRITICAS

- Leer y aplicar `00-contexto-obligatorio.md`.
- No duplicar las reglas de credenciales ni reescribir consultas existentes sin necesidad demostrada.
- No crear Forms Authentication, auditoría de éxito, permisos finales o redirección durante preautenticación.
- No permitir que el cliente entregue el correo o ID autoritativo.
- No conectar aún la interfaz final ni realizar envío real.

## REQUISITOS POSITIVOS

1. Extraer o envolver la validación actual para producir un resultado de preautenticación mínimo y no secreto.
2. Implementar adaptadores explícitos para `usuarios_da`, `remit_dest_interno`, `usuario_radicador` y `usuario_workflow`.
3. Resolver nombres de columna, casing, longitudes y correo nullable según el esquema real.
4. Consultar configuración 2FA exclusivamente en `docuarchi.gestor_modulos`.
5. Si 2FA está desactivado, continuar por el mismo finalizador legacy y demostrar equivalencia.
6. Si está activado, crear el challenge y devolver estado pendiente sin dejar autenticación parcial.
7. Asegurar que Radicación Documental sea una rama explícita y no se trate como Gestor Documental.
8. Encapsular la finalización actual para que se invoque una sola vez, después del OTP o directamente cuando 2FA esté desactivado.
9. Guardar en Session únicamente un contexto de preautenticación mínimo: referencia interna al challenge pendiente, nonce antifalsificación y datos no secretos estrictamente necesarios. No conservar contraseña, OTP, correo completo ni conexiones.

## REGLAS DE ANTIRREGRESION

- Con 2FA desactivado deben conservarse permisos, Session, auditoría, página destino y mensajes de cada módulo.
- No cambiar el resultado de credenciales inválidas, usuarios inactivos/bloqueados ni empresa inexistente salvo sanitización aprobada.
- Recuperación de contraseña permanece independiente.

## PRUEBAS OBLIGATORIAS

Crear pruebas por módulo para credenciales válidas/inválidas, correo presente/ausente, configuración activa/inactiva/inválida, identidad duplicada entre empresas y ausencia de sesión final antes del OTP. Ejecutar también la línea base de etapa 01 y compilar.

## ENTREGABLE FINAL

Entregar mapa antes/después, adaptadores, pruebas por módulo, equivalencia con 2FA apagado y riesgos. No avanzar si algún módulo carece de resolución autoritativa de usuario y correo.

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

Esta etapa no cierra por sí sola el cambio: el E2E real forma parte integral del cierre en la etapa 09. Antes de esa autorización, ejecutar pruebas focales y documentar formalmente por qué no se realizó todavía una corrida autenticada.
