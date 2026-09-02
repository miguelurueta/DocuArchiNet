<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04 -->
## Context

DOC-44: ESTABILIZACION-WORKFLOW

## Jira Details

> # Prompt 05 — Estabilización del consumidor Workflow
> 
> ## Prompt para ejecutar
> 
> ```text
> Aplica primero el contexto común de Prompt/00-guia-de-uso-y-contexto-comun.md. Requiere que el Centro de Trabajo tenga la adaptación moderna de Notas revisada y que la bandera continúe deshabilitada.
> 
> Objetivo: estabilizar únicamente el consumidor de Notas del Centro de Trabajo Workflow, verificando que use el contrato y cliente moderno únicos, conserve un rollback preciso y no duplique operaciones con la ruta legacy.
> 
> Rol esperado: arquitecto y desarrollador senior de ASP.NET Web Forms/VB.NET y JavaScript, responsable de cerrar la adaptación del módulo `workflow/` de forma reversible y sin ampliar el alcance.
> 
> Contexto obligatorio: revisa el adaptador de Centro de Trabajo aprobado, los contratos modernos, `workflow/Webworkflow.aspx`, su code-behind, scripts, estilos, endpoints ASMX y pruebas de Notas bajo `workflow/`. Ubica los cambios únicamente en esas rutas y en las utilidades E2E existentes que las cubren.
> 
> Restricciones críticas:
> - No modificar, migrar, inventariar para retiro ni ejecutar pruebas de consumidores o módulos ajenos a `workflow/`.
> - No habilitar gates, audiencias, usuarios/grupos piloto, endpoints legacy ni E2E autenticada sin autorización explícita.
> - No copiar CRUD/serialización, reactivar permisos comentados, depender de tarea mutable de sesión ni de `Session("ID_TAREA_SELECCIONDA")`, ni producir doble escritura, doble modal o doble actualización.
> - No cambiar la semántica de borrado, retención, supervisión o visibilidad sin una decisión de negocio aprobada.
> 
> Pruebas obligatorias: ejecuta pruebas focales o QA manual reproducible del Centro de Trabajo para autorización, tarea/nota cruzada, conflictos, contenido, cursor, rollback y ausencia de doble operación. Integra en este mismo cambio la E2E de regresión exclusiva de Workflow y ejecútala solo cuando ambiente, cuentas y tarea descartable estén autorizados. Compila con MSBuild o `dotnet` si se modifica código VB y registra comandos, resultados y evidencia saneada.
> 
> Documentación técnica: actualiza bajo `Doc/Actualizacion/workflow/Notas/` la propuesta OpenSpec, matriz de pruebas, contrato cliente, fallback, resultado, deuda legacy de Workflow y rollback. No crear documentación en la raíz.
> 
> Entregable final: entrega la estabilización verificable del único consumidor Workflow, comandos y resultados, matriz de regresión, riesgos y la deuda que pasa a la fase 06.
> 
> E2E integrada obligatoria: incorpora y cumple `bloque-e2e-integrado-en-modernizacion.md` con este cambio. Reutiliza autenticación, configuración, validadores y evidencias existentes; ejecuta pruebas reales solo sobre ambiente, cuentas y tarea Workflow autorizados, con secretos efímeros, controles `SELECT` y evidencia saneada. Ante ausencia de ambiente, datos o autorización, registra el bloqueo explícito; no uses mocks, simulaciones, resultados inventados ni habilitación arbitraria de gates.
> 
> Alcance: RF-11, RF-14, RF-19 y RF-20; RN-08, RN-10, RN-15 y RN-16; RNF-04, RNF-05 y RNF-07, aplicados exclusivamente al consumidor Centro de Trabajo Workflow.
> 
> 1. Inventaría los puntos de entrada de Notas bajo `workflow/`: identidad explícita de tarea, permisos, scripts AJAX, contador, eventos y fallback. No amplíes la búsqueda a módulos ajenos.
> 2. Verifica que el Centro de Trabajo use el cliente y contrato moderno únicos, sin volver a copiar CRUD ni serialización dentro de Workflow.
> 3. Comprueba que listar, consultar, contar, crear, editar y eliminar usan `idTarea` explícito y resultados funcionales del backend. La ruta moderna y la legacy no pueden ejecutar una misma operación dos veces.
> 4. Revisa que ninguna ruta moderna dependa de la última tarea guardada en sesión ni de `Session("ID_TAREA_SELECCIONDA")`; la autorización y la tarea proceden del backend.
> 5. Valida el rollback del consumidor: con la bandera en `false`, GridView, scripts y eventos legacy continúan disponibles sin modificar datos ni activar dos rutas simultáneas.
> 6. Agrega pruebas de regresión de Workflow para autorización directa, tarea ajena/inactiva, nota cruzada, contenido de texto, conflicto, cursor, rollback de bandera y ausencia de doble operación. Integra la E2E indicada, ejecútala solo con autorización y no actives el gate.
> 7. Registra la deuda legacy restante dentro de `workflow/` y el criterio de referencias cero que deberá cumplir la fase 06.
> 
> Fuera de alcance: cualquier módulo ajeno a `workflow/`, migraciones de datos/esquema no aprobadas, activación de usuarios/grupos piloto y retiro de rutas legacy.
> 
> Criterios de aceptación:
> - Existe un único contrato y cliente de Notas para el consumidor Centro de Trabajo Workflow.
> - El consumidor puede volver individualmente al flujo legacy sin afectar datos ni crear duplicados.
> - Las rutas modernas no usan tarea mutable de sesión ni autorización solo visual.
> - La matriz de pruebas y rollback cubre exclusivamente las rutas del módulo `workflow/`.
> 
> Entrega una matriz de Workflow con contrato usado, flag/fallback, pruebas, resultado y deuda legacy para fase 06.
> ```

## Goals / Non-Goals

**Goals**
- Estabilizar exclusivamente el consumidor de Notas en `workflow/Webworkflow.aspx`.
- Demostrar contrato moderno único, tarea explícita, exclusión de canales y rollback seguro.
- Dejar una matriz de regresión y deuda legacy verificable para fase 06.

**Non-Goals**
- Consumidores ajenos a `workflow/`, cambios de esquema o semántica de negocio.
- Activación de gates/audiencias o retiro de la ruta legacy.

## Decisions

1. **D-01 / RQ-01 — Alcance aislado.** Los cambios de producto se limitan a `workflow/Webworkflow.aspx(.vb)`, su cliente y estilos de Notas y la configuración necesaria para entregar el gate apagado. El ASMX y contrato compartidos se validan y reutilizan sin alterar consumidores ajenos; las utilidades E2E y la documentación DOC-44 sí forman parte del entregable. Los demás consumidores quedan fuera de inventario, modificación y ejecución.
2. **D-02 / RQ-02 — Contrato moderno único.** `WorkflowNotesModern` conserva JSON real e invoca el ASMX moderno con `idTarea` explícito; autorización, propiedad, cursor y conflicto se resuelven en backend.
3. **D-03 / RQ-03 — Exclusión y rollback.** `ConfigureWorkflowNotesModernPresentation` mantiene mutuamente excluyentes el panel moderno y el disparador legacy. La entrega conserva el gate apagado y las audiencias vacías.
4. **D-04 / RQ-04 — Regresión verificable.** Se amplían pruebas focales, se integra una regresión E2E exclusiva de Workflow reutilizando autenticación, configuración, validadores y evidencia existentes, y se cubren autorización, cruces, contenido, cursor, conflicto y doble operación. Integrar la prueba no autoriza su ejecución real: la corrida requiere autorización expresa y debe restaurar el gate.


## Risks / Trade-offs

- La página contiene numerosas referencias de sesión para funciones no relacionadas; la verificación debe limitarse al bloque moderno de Notas para no confundir deuda global con dependencia del cliente nuevo.
- El fallback mantiene código legacy con SQL/JS histórico; retirarlo en DOC-44 rompería la reversibilidad y queda como deuda explícita de fase 06.
- La E2E real muta una tarea descartable; sin autorización se registra el bloqueo y no se sustituye por simulaciones.

## Migration Plan

1. Mantener `WorkflowCentroTrabajoModernActive=false` y audiencias vacías durante desarrollo y entrega.
2. Inventariar y probar el consumidor moderno y su exclusión con legacy.
3. Ejecutar QA/E2E solo con autorización y restaurar la configuración exacta.
4. Rollback operativo: gate `false`; no revertir datos, contratos ni migraciones DOC-42.

## Open Questions

- Ninguna pregunta bloqueante. El retiro de handlers, GridView, modal y `Class_anotacion_tarea` pertenece a fase 06 y requiere criterio de referencias cero.
