# 01 — Propuesta OpenSpec y alineación arquitectónica

## ROL ESPERADO

Actúa como arquitecto de software senior para .NET Framework, ASP.NET Web Forms, VB.NET y modernización gradual de flujos legacy.

## OBJETIVO

Formalizar el cambio `modernize-workflow-send-group` antes de escribir código. Debe extender la modernización ya implementada en `Doc/Actualizacion/workflow/Terminar/`, sin recrear su fundación, ASMX, componentes genéricos ni gate.

## RESTRICCIONES CRITICAS

- Lee y aplica `prompts/00-contexto-obligatorio.md` y `Doc/Actualizacion/workflow/Terminar/README.md`.
- No implementar código, no activar gates y no modificar configuración.
- No crear una segunda bandera ni fuente de evaluación: la habilitación reutiliza `IWorkflowModernFeatureGate` existente y falla cerrada.
- No tratar el envío directo a grupo como conector ni cambiar el contrato de continuar flujo.
- No aprobar como requisito nuevo la validación de respuesta radicada sin decisión funcional explícita.

## REQUISITOS POSITIVOS

Crear propuesta, diseño, especificaciones y tareas atómicas que definan:

1. `PreviewEnviarGrupo(idTarea)` y `EjecutarEnvioGrupo(idTarea, idActividadDestino, tokenVersion)` en el ASMX moderno existente.
2. Destinos como actividades permitidas de la ruta, no conectores salientes.
3. Revalidación de `Cambio_Ruta`, tarea activa, ruta/flujo/actividad abiertos, token y pertenencia a ruta.
4. Reutilización de lock, auditoría, componentes de confirmación y adaptador legacy; nueva lógica de dominio solo donde el reenvío directo lo requiere.
5. Fallback Web Forms y rollback por el gate existente, sin configuración paralela.

## ESTADOS Y ERRORES CONTROLADOS

Definir estados observables de `preview-inactivo`, `preview-disponible`, `sin-destinos`, `confirmando`, `enviando`, `exito`, `bloqueo-funcional` y `error-tecnico-controlado`. Para cada estado definir mensaje seguro, acción permitida, recuperación y códigos públicos como permiso denegado, tarea no disponible, ruta/flujo cerrado, destino no disponible y conflicto de versión.

## CRITERIOS DE ACEPTACION

- La propuesta separa con precisión `IdActividadDestino` de `IdConector`.
- Cada requisito tiene trazabilidad con diseño, tarea y prueba prevista.
- Las decisiones pendientes, en especial requisitos que difieran del legacy, quedan explícitas y sin asumir aprobación.
- El cambio no duplica los límites ya entregados por DOC-9 a DOC-14.

## PRUEBAS OBLIGATORIAS

Definir la matriz mínima de contrato, preview de solo lectura, autorización, token/concurrencia, fallback legacy, no regresión de continuar flujo y QA manual. Ejecutar la línea base antes de proponer cambios: `msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug` y las pruebas focales existentes aplicables; registrar comando, código de salida y resultado. No ejecutar E2E ni carga en esta fase.

## REGLAS DE ANTIRREGRESION

- Mantener sin cambios las firmas, payloads y códigos públicos de `PreviewEnviarTarea` y `EjecutarEnvioTarea`.
- Mantener `IdConector` como identificador exclusivo de continuar flujo y `IdActividadDestino` como identificador exclusivo de grupo.
- Con gate inactivo, conservar el enlace, postback y modal legacy de ambos comandos.
- Incluir en la propuesta pruebas existentes que deben seguir pasando y el criterio de detener el cambio ante una divergencia.

## DOCUMENTACION TECNICA

Crear los artefactos OpenSpec y registrar que toda la documentación técnica posterior se consolidará en `Doc/Actualizacion/workflow/TerminarGrupo/01-implementacion-envio-grupo/`, con la convención `00` a `04` y `Diagramas/` de `Terminar`; no crear un paquete por etapa.

## ENTREGABLE FINAL

Entregar rutas de los artefactos, decisiones tomadas, dependencias con la modernización existente, tareas atómicas, comandos de línea base con resultado y bloqueos que requieran decisión funcional. No iniciar implementación sin aprobación del cambio.
