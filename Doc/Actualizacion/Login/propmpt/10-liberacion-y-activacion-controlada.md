# 10 — Liberación, activación configurable y rollback

## ROL ESPERADO

Actúa como arquitecto responsable de liberación y seguridad operativa.

## OBJETIVO

Preparar una liberación reversible y decidir si se puede solicitar la activación gradual de 2FA por módulo, sin activarlo como consecuencia automática de este prompt.

## RESTRICCIONES CRITICAS

- Leer y aplicar `00-contexto-obligatorio.md` y la evidencia de etapas 08 y 09.
- No desplegar, ejecutar DDL ni cambiar `gestor_modulos` sin autorización específica por ambiente.
- No crear una segunda fuente de configuración paralela.
- No asumir que pruebas exitosas autorizan activación global.
- No habilitar simultáneamente todos los módulos como primera operación.

## PRECONDICIONES

1. Migración y rollback revisados, respaldos y responsables identificados.
2. Compilación, pruebas y matriz de los cuatro módulos sin bloqueos críticos/altos.
3. Fuente segura del secreto HMAC disponible por ambiente y fuera del repositorio/base.
4. Correo, monitoreo sanitizado y soporte operativo preparados.
5. Política aprobada para `NULL`, proveedor, vigencia, intentos y reenvío.
6. Procedimiento probado para desactivar 2FA sin romper el login existente.

## PLAN DE LIBERACION

- Separar despliegue de código, aplicación de migración y activación de configuración.
- Desplegar código compatible con `RequiereSegundoFactor=0` y validar línea base.
- Aplicar DDL autorizado y verificar esquema mediante consultas de solo lectura.
- Activar un módulo/empresa piloto autorizado, observar y luego decidir expansión.
- Para cada activación registrar ambiente, módulo, valor previo/nuevo, ventana, aprobador, ejecutor y evidencia sanitizada.
- Ante fallos, desactivar 2FA para nuevos intentos mediante la configuración central aprobada; conservar evidencia y no borrar challenges indiscriminadamente.

## ROLLBACK

Definir rollback independiente para configuración, aplicación y DDL. El rollback de aplicación no debe ocurrir dejando un esquema incompatible; el DDL destructivo solo se revierte cuando no pierda evidencia ni datos necesarios. Documentar cómo finalizar o revocar challenges pendientes durante un retroceso.

## CRITERIOS DE ACEPTACION

- La decisión distingue `bloqueada`, `lista para solicitar autorización` y `autorizada para ambiente/módulo específico`.
- El login de un factor sigue disponible cuando la configuración central está desactivada.
- No existe activación implícita por `NULL`, error de lectura o proveedor desconocido.
- El plan es gradual, observable y reversible.

## ENTREGABLE FINAL

Entregar matriz de ambientes/módulos, precondiciones, runbook, rollback, responsables, riesgos y decisión. Confirmar expresamente que este prompt no ejecutó despliegue, DDL ni activación.

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
