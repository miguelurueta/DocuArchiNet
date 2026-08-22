# Jira Context - DOC-29

## Summary

INTERFAZ-MODERNA-ENVIAR-USUARIO

## Description

> # 02 — Interfaz moderna oficial
> 
> ## ROL ESPERADO
> 
> Actúa como desarrollador senior de ASP.NET Web Forms y JavaScript legacy accesible.
> 
> ## OBJETIVO
> 
> Conectar solo el comando **Enviar a usuario** a los endpoints ya implementados, con búsqueda paginada, confirmación accesible y experiencia moderna oficial para todo contexto Workflow válido.
> 
> ## CONTEXTO OBLIGATORIO
> 
> - Requiere 01 aprobado y que el ticket actual enlace este archivo.
> - Leer `00-contexto-obligatorio.md`, evidencia de 01, CSS/componentes de confirmación existentes y exploración arquitectónica.
> - Habilita 03 únicamente cuando no haya listeners, estado ni payload compartido con Continuar flujo.
> 
> ## REQUISITOS POSITIVOS
> 
> - Registrar trigger y bootstrap de forma uniforme, con adaptador JavaScript exclusivo de usuario que no evalúe feature gate para este comando.
> - Consumir `PreviewEnviarUsuario` y `EjecutarEnvioUsuario`; aplicar debounce, páginas, descarte de respuesta obsoleta e invalidación de selección antigua.
> - Reutilizar confirmación, foco, trampa de foco, teclado, Escape, ARIA, responsive, cancelación, doble clic y mensaje de éxito correlacionado.
> - Tras éxito, actualizar solo tarea afectada, visor y contador mediante componentes modernos existentes.
> 
> ## RESTRICCIONES CRÍTICAS
> 
> - No crear ASMX, banderas de habilitación, framework, bundler, modal alterno ni autorización en JavaScript.
> - No invocar controles ocultos, motor, handlers, SQL, `Cambia_Estado`, reasignación de respuesta ni endpoints/payload de Continuar flujo.
> - No habilitar un enlace, postback ni modal Web Forms alternativo para `ImageButtonEnviarUsuario`.
> - No ejecutar E2E autenticado sin autorización explícita de ambiente y cuentas de prueba.
> 
> ## REGLAS DE ANTIRREGRESIÓN
> 
> - Enviar a usuario y Continuar flujo no comparten selectores, eventos, estado, `IdConector` ni requests.
> - Todo contexto Workflow válido usa el mismo adaptador y recorrido moderno de Enviar a usuario.
> 
> ## CRITERIOS DE ACEPTACIÓN
> 
> - Modal y búsqueda representan solo JSON autorizado, soportan teclado/foco/Escape y no materializan la lista completa.
> - Error, bloqueo, cancelación y respuesta obsoleta restauran contexto sin iniciar flujo legacy de reasignación.
> 
> ## PRUEBAS OBLIGATORIAS
> 
> Agregar pruebas CJS de contratos, eventos aislados, búsqueda, debounce, páginas, respuesta obsoleta, vacío, error, selección, éxito, bloqueo, cancelación, doble clic, teclado, foco y bootstrap uniforme. Ejecutar MSBuild y pruebas focales; registrar evidencia. No E2E sin autorización explícita.
> 
> ## DOCUMENTACIÓN TÉCNICA
> 
> Actualizar `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md`, `04-pruebas-y-evidencia.md` y diagramas necesarios con selectores, UI, accesibilidad, recorrido moderno y relevo a 03.
> 
> ## ENTREGABLE FINAL
> 
> Reportar ticket, archivos UI, pruebas, compilación y evidencia de ruta moderna/no regresión. No cambiar configuración de ambiente ni realizar QA autenticado sin autorización.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: ENVIAR, INTERFAZ, USUARIO
