## Why

PILOTO-DESPLIGUE-CONTROLADO. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-14.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> # Prompt 06 — Piloto, pruebas y salida controlada
> 
> ```text
> Rol esperado:
> Arquitecto de software senior y responsable de calidad/release para ASP.NET Web Forms .NET Framework 4.6.1, VB.NET, operaciones controladas, trazabilidad y migración gradual de workflows legacy.
> 
> Contexto:
> - Repositorio: `D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net`.
> - La modernización incluye la fundación paralela, preview de destinos, ejecución segura, lista moderna y confirmación especializada descritas en los prompts 01 a 05.
> - El flujo legacy de `workflow/Webworkflow.aspx`, `ClassWorkflow.Terminar_Tarea_Workflow`, `ClassWorkflow.Cambia_Estado`, autorización, firma, expediente, eventos dinámicos, correo y trazabilidad sigue siendo el respaldo operativo.
> - El contrato base de `WorkflowCentroTrabajoModernActive` se crea en Prompt 01. La bandera habilita Presentation moderna de forma reversible y los ASMX modernos la revalidan en servidor; no sustituye la autorización ni las validaciones de negocio.
> 
> Objetivo:
> Conservar el flujo anterior como respaldo mientras se valida la versión moderna mediante un piloto controlado, métricas trazables, criterios objetivos de salida y rollback inmediato sin migración de datos.
> 
> Restricciones críticas:
> - No debe retirarse, degradarse ni modificarse el flujo legacy, sus reglas de negocio, transacciones, autorizaciones, firma, expediente, eventos, correo o trazabilidad durante el piloto.
> - No debe habilitarse la experiencia moderna para toda la población sin cumplir criterios de entrada, evidencia de pruebas y aprobación explícita del responsable funcional/técnico.
> - No debe requerirse migración, reparación o reversión de datos para desactivar la bandera; el rollback debe volver a la interfaz legacy por configuración.
> - No debe registrar telemetría con SQL, credenciales, Session completa, tokens, documentos, payloads sensibles ni datos personales innecesarios.
> - No debe ocultarse un fallo con reintentos automáticos, cambios directos de estado o alteraciones manuales del motor legacy.
> - No debe considerarse éxito una prueba visual si falta validación de resultado funcional, trazabilidad, concurrencia, seguridad o compatibilidad con el flujo anterior.
> 
> Validar además la separación arquitectónica: Presentation no contiene SQL ni reglas de negocio; Application no usa controles Web Forms; Domain no depende de infraestructura; y únicamente el adaptador legado invoca el núcleo actual de terminación.
> 
> Implementar:
> 1. Configurar y operar la bandera base `WorkflowCentroTrabajoModernActive` creada en Prompt 01; no crear una segunda implementación ni una segunda fuente de configuración.
> 2. Activación por usuario piloto, grupo o configuración, con precedencia y comportamiento documentados para activación, exclusión y desactivación, consumidos por Presentation y revalidados por ambos ASMX modernos.
> 3. Telemetría/auditoría con contrato mínimo: identificador de correlación, usuario anonimizado o identificador autorizado, tarea, ruta/flujo, conector, destino, versión moderna/legacy, duración, éxito/bloqueo/error, código funcional y referencia de auditoría.
> 4. Rollback inmediato a la interfaz antigua sin migración de datos, con procedimiento de desactivación, verificación posterior y responsable de ejecución.
> 5. Pruebas automatizadas, pruebas focales y matriz manual reproducible.
> 6. Tablero o reporte de piloto que permita comparar volumen, éxito, bloqueos, errores, duración, abandonos y divergencias respecto del camino legacy.
> 
> Contrato de activación y rollback:
> - Entrada de configuración: `WorkflowCentroTrabajoModernActive`, alcance de usuario/grupo/configuración, fecha de inicio, responsable y motivo de activación.
> - Resultado de evaluación: `activo`, `inactivo`, `excluido` o `fallback-legacy`, con causa visible solo para soporte autorizado y sin filtrar datos sensibles al usuario final.
> - El cliente consulta únicamente el bootstrap de bandera permitido; el servidor conserva la decisión de permisos, preview y ejecución y bloquea llamadas modernas directas fuera del piloto con `WORKFLOW_MODERN_INACTIVE`.
> - Rollback: desactivar la bandera para el alcance afectado, detener nuevas aperturas modernas, conservar las transiciones ya confirmadas por el servidor, abrir la interfaz legacy para los nuevos intentos y devolver bloqueo funcional a llamadas ASMX modernas posteriores; registrar correlación, motivo, hora y responsable.
> - No intentar revertir una transición ya confirmada mediante SQL, JavaScript o una nueva llamada a `Cambia_Estado`; cualquier reversión de negocio usa exclusivamente el procedimiento legacy autorizado.
> 
> Matriz mínima:
> - Ruta normal.
> - Flujo normal, con varios destinos y retorno por flujo.
> - Envío manual a usuario y a grupo.
> - Respuesta obligatoria.
> - Solicitud de aprobación pendiente.
> - Firma digital pendiente.
> - Documento sin expediente.
> - PRETERMINARACTIVIAD exitoso y fallido.
> - Correo exitoso y fallido.
> - Doble clic.
> - Conector alterado.
> - Dos sesiones sobre la misma tarea.
> - Resoluciones: 1366x768, 1024x768, 768x1024 y 375x812.
> 
> Pruebas obligatorias:
> - Ejecutar compilación del proyecto o solución afectada con MSBuild/.NET Framework y registrar comando, resultado y limitaciones reales.
> - Agregar o ajustar pruebas focales donde la arquitectura actual lo permita para evaluación de bandera, activación/exclusión, serialización de telemetría, normalización de errores y procedimiento de rollback.
> - Ejecutar toda la matriz mínima con piloto activado y validar al menos los flujos representativos con la bandera desactivada para confirmar continuidad legacy.
> - Ejecutar QA manual reproducible en las cuatro resoluciones, con evidencia de estados visuales, accesibilidad, lista, confirmación, éxito, bloqueo, error y retorno al flujo anterior.
> - Simular rollback durante un escenario sin éxito y durante una operación concurrente; verificar que no hay pérdida de datos ni transición duplicada.
> - E2E automatizada no aplica si el repositorio no cuenta con infraestructura compatible para Web Forms; documentar la justificación, pruebas focales y evidencia QA. Si existe infraestructura disponible, ejecutar recorridos end-to-end de activación, envío, bloqueo y rollback.
> - Registrar para cada prueba: ambiente, usuario/rol de prueba, configuración de bandera, pasos, resultado esperado, resultado observado, correlación y evidencia.
> 
> Documentación técnica:
> - Este prompt es autosuficiente: no depende de README ni de documentación externa para conocer su convención documental.
> - Raíz documental obligatoria, relativa a la raíz del repositorio: `Doc/Actualizacion/workflow/Terminar/06-piloto-pruebas-rollout/`.
> - Estructura obligatoria del paquete:
>     `Doc/Actualizacion/workflow/Terminar/06-piloto-pruebas-rollout/`
>     - `00-indice.md`
>     - `01-arquitectura.md`
>     - `02-contrato.md`
>     - `03-flujo-y-seguridad.md`
>     - `04-pruebas-y-evidencia.md`
>     - `Diagramas/`
> - `00-indice.md`: ticket, fecha, estado, alcance del piloto, responsables, grupos/usuarios piloto y resumen de cambios.
> - `01-arquitectura.md`: frontera de bandera, Presentation/Application/Domain/Infrastructure, telemetría, adaptador legacy, dependencias y alternativas descartadas.
> - `02-contrato.md`: configuración de bandera, evaluación de activación, campos de auditoría/telemetría, métricas, códigos funcionales y contrato de rollback.
> - `03-flujo-y-seguridad.md`: activación → preview → confirmación → ejecución → telemetría; fallback legacy, autorización, concurrencia, rollback, riesgos y escalamiento.
> - `04-pruebas-y-evidencia.md`: comando de compilación, pruebas focales, matriz manual, resoluciones, resultados, correlaciones, limitaciones, aprobación y evidencia.
> - `Diagramas/`: diagramas Mermaid o fuentes estructuradas de activación, decisión de bandera, flujo de rollback y estados del piloto cuando correspondan.
> - Incluir una tabla con: configuración, ruta, alcance, responsable, métrica, umbral, acción ante falla y dependencia legacy preservada.
> - El prompt fuente `06-piloto-pruebas-rollout.md` permanece en `Doc/Actualizacion/workflow/Terminar/`; no crear documentación de implementación junto a él, en la raíz del repositorio ni en rutas alternativas sin justificarlo expresamente en el entregable.
> 
> Criterios de aceptación:
> - La bandera creada en Prompt 01 permite activar, excluir y desactivar de forma auditable por el alcance autorizado; Presentation la consume como bootstrap y ambos ASMX modernos la revalidan sin alterar decisiones de permisos ni reglas del servidor.
> - El rollback a la interfaz legacy es ejecutable por configuración, no requiere migración de datos y deja registro verificable de responsable, motivo, hora y correlación.
> - La telemetría permite comparar la experiencia moderna y legacy sin filtrar información sensible; registra éxito, bloqueo, error, duración, concurrencia y divergencias.
> - La matriz mínima y las resoluciones requeridas tienen resultado documentado, evidencia y trazabilidad; los escenarios críticos no presentan transición duplicada ni pérdida de contexto.
> - Solo se avanza del piloto cuando compilación, pruebas focales, QA, métricas y aprobación explícita cumplen los umbrales acordados; de lo contrario se conserva o restaura el flujo legacy.
> - La separación arquitectónica se valida: Presentation no contiene SQL ni reglas de negocio; Application no usa controles Web Forms; Domain no depende de infraestructura; únicamente el adaptador legacy invoca el núcleo de terminación.
> 
> Entregable final:
> - Entregar la implementación/configuración de bandera, activación piloto, telemetría, reporte de métricas y procedimiento de rollback con sus rutas y responsables.
> - Entregar evidencia de compilación, pruebas focales, matriz manual, resoluciones, QA, E2E o justificación, resultados, correlaciones y limitaciones.
> - Entregar diferencias verificadas frente a la interfaz anterior, tabla de configuraciones/métricas, documentación del paquete obligatorio y diagramas aplicables.
> - Entregar criterio explícito y aprobado para pasar de piloto a producción, más criterios de bloqueo y reversa; declarar qué comportamiento legacy se preservó y qué no se modificó.
> ```

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DESPLIEGUE, PILOTO, TERMINAR

## Capabilities

### New Capabilities
- `piloto-despligue-controlado`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

