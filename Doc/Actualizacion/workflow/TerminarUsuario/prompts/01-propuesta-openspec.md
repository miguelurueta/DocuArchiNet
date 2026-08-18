# 01 — Propuesta OpenSpec y alineación arquitectónica

## ROL ESPERADO

Actúa como arquitecto de software senior para .NET Framework, ASP.NET Web Forms, VB.NET y modernización gradual de flujos legacy.

## OBJETIVO

Formalizar el cambio `modernize-workflow-send-user` antes de escribir código. Debe extender la modernización de `Doc/Actualizacion/workflow/Terminar/` sin recrear fundación, ASMX, componentes comunes ni gate.

## RESTRICCIONES CRÍTICAS

- Lee y aplica `prompts/00-contexto-obligatorio.md`, `Doc/Actualizacion/workflow/Terminar/README.md` y la exploración de `TerminarUsuario`.
- No implementar código, activar gate ni modificar configuración.
- No crear bandera, fuente de evaluación ni ASMX alterno.
- No modelar el envío directo a usuario como conector ni cambiar el contrato de Continuar flujo.
- La respuesta pendiente se bloquea; no introducir ni conservar reasignación de respuesta en la operación moderna.

## REQUISITOS POSITIVOS

Crear propuesta, diseño, especificaciones y tareas atómicas que definan:

1. `PreviewEnviarUsuario(idTarea)` y `EjecutarEnvioUsuario(idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion)` dentro del ASMX moderno existente.
2. Destinos como pares usuario–actividad activos de la ruta, no conectores ni listas de usuario no autorizadas.
3. Revalidación de `CAMBIO_USUARIO`, tarea activa, ruta/flujo abierto, usuario activo, `UTIL_ASIGNA_TAREA`, token y pertenencia a ruta.
4. Estado de respuesta: `YES` permite continuar; confirmación/radicado pendiente se bloquea sin mutación.
5. Reutilización de lock, auditoría, componentes de confirmación y motor legacy mediante adaptador especializado.
6. Fallback Web Forms y rollback con el gate existente, sin configuración paralela.

## ESTADOS Y ERRORES CONTROLADOS

Definir estados observables `preview-inactivo`, `preview-disponible`, `respuesta-pendiente`, `sin-destinos`, `confirmando`, `enviando`, `exito`, `bloqueo-funcional` y `error-tecnico-controlado`. Para cada uno definir mensaje seguro, acción permitida, recuperación y código público para permiso denegado, tarea no disponible, ruta/flujo cerrado, destino no disponible, respuesta pendiente y conflicto de versión.

## CRITERIOS DE ACEPTACIÓN

- La propuesta separa `IdUsuarioWorkflowDestino` e `IdActividadDestino` de `IdConector`.
- La exclusión de reasignación de respuesta queda como requisito verificable, con código de bloqueo previsto.
- Cada requisito tiene trazabilidad con diseño, tarea y prueba prevista.
- El cambio no duplica límites entregados por DOC-9 a DOC-14.

## PRUEBAS OBLIGATORIAS

Definir matriz mínima de contrato, preview de lectura, autorización, respuesta pendiente, token/concurrencia, fallback legacy y no regresión de Continuar flujo. Ejecutar como línea base `msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug` y las pruebas focales existentes; registrar comando, código y resultado. No ejecutar E2E ni carga.

## DOCUMENTACIÓN TÉCNICA

Crear los artefactos OpenSpec y registrar que las etapas posteriores se documentarán en `Doc/Actualizacion/workflow/TerminarUsuario/<NN>-<slug>/` bajo la convención de `Terminar`.

## ENTREGABLE FINAL

Entregar rutas de artefactos, decisiones, dependencias, tareas atómicas, línea base y bloqueos que requieran una decisión funcional. No iniciar implementación sin aprobación del cambio.

