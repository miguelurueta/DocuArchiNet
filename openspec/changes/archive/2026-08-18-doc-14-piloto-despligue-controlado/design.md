<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Contexto

DOC-14 prepara un piloto controlado para el centro de trabajo moderno ya existente. La raíz canónica es `D:\imagenesda\DocuachiNet\DocuArchiNet`; la ruta histórica incluida en el ticket se conserva solo como contexto previo.

El flujo legacy de `workflow/Webworkflow.aspx`, `ClassWorkflow.Terminar_Tarea_Workflow` y `ClassWorkflow.Cambia_Estado` continúa siendo el respaldo operativo. La experiencia moderna ya dispone de:

- `ConfiguracionWorkflowModernFeatureGate`, que evalúa la bandera, inclusiones y exclusiones en servidor.
- `WorkflowModernPresentationBootstrap`, que puede exponer la decisión a Presentation sin conceder autorización.
- `WebServiceWorkflowModern.asmx`, que revalida la sesión y el gate antes de preview y ejecución.
- `ServicioTransicionTarea`, guard de concurrencia, adaptador del ejecutor legacy y `WorkflowLegacyAuditoriaAdapter`.

La configuración inició apagada. Tras la aprobación de promoción, la raíz canónica opera con `WorkflowCentroTrabajoModernActive=true` y `WorkflowCentroTrabajoModernOfficialMode=true`, con listas de usuarios/grupos vacías y metadatos operativos completos. Esta activación oficial no autoriza E2E autenticada ni carga.

## Objetivos

- Operar un único gate con alcance de piloto explícito o modo oficial explícito, y reversa inmediata a legacy.
- Hacer consistente la decisión entre Web Forms, preview y ejecución ASMX.
- Registrar telemetría mínima, trazable y no sensible usando la frontera de auditoría existente.
- Definir evidencia, criterios de bloqueo y documentación para decidir un piloto, sin modificar reglas legacy.

## No objetivos

- No sustituir `Terminar_Tarea_Workflow`, `Cambia_Estado`, autorización, firma, expediente, correo, eventos ni trazabilidad legacy.
- No habilitar a toda la población de forma implícita, migrar datos, revertir transiciones confirmadas ni añadir reintentos automáticos. El modo oficial requiere su bandera explícita, listas piloto vacías y metadatos válidos.
- No almacenar SQL, cadenas de conexión, tokens, Session completa, documentos ni payloads sensibles en telemetría.
- No ejecutar E2E real, carga, cambios de gate o consultas no autorizadas en un ambiente.

## Decisiones

### D-01 — Gate único, explícito y *fail closed*

`Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` seguirá siendo la única implementación de habilitación. `WorkflowCentroTrabajoModernActive` solo habilita una evaluación; el alcance se declara explícitamente como piloto o como modo oficial.

Orden de evaluación:

1. Contexto inválido o bandera ausente/falsa: `inactivo`.
2. Usuario o grupo excluido: `excluido`.
3. `WorkflowCentroTrabajoModernOfficialMode=true` con listas vacías y metadatos válidos: `activo` para todo contexto Workflow válido no excluido.
4. Modo oficial con usuario o grupo configurado: `fallback-legacy` por configuración ambigua.
5. Modo piloto con bandera verdadera pero sin usuario ni grupo incluido: `fallback-legacy`.
6. Usuario o grupo incluido en modo piloto: `activo`.
7. Cualquier otro perfil: `inactivo`.

La misma fuente `appSettings` contendrá los metadatos operativos de inicio, responsable y motivo. Si falta un metadato obligatorio para un piloto u operación oficial activa, la decisión será `fallback-legacy` con código funcional seguro. No se crea una segunda fuente de configuración.

### D-02 — Presentation consume el mismo gate que los ASMX

`workflow/Webworkflow.aspx.vb` sustituirá la decisión visual basada en `WorkflowCentroTrabajoModernEnabled` y `WorkflowCentroTrabajoModernPilotProfiles` por `WorkflowModernPresentationBootstrap`. El bootstrap solo condiciona assets y presentación; no expone permisos, Session, SQL ni decisiones de negocio.

`WebServiceWorkflowModern.asmx.vb` y `ServicioTransicionTarea.vb` conservarán la revalidación del mismo `IWorkflowModernFeatureGate` para preview y ejecución. Una llamada moderna directa fuera del alcance permitido devolverá el código funcional de gate y nunca hará fallback automático que ejecute el motor legacy.

La selección de un destino del preview debe abrir una confirmación accesible que conserve `idTarea`, `idConector` y `tokenVersion`; únicamente su confirmación puede invocar `EjecutarEnvioTarea`. Al responder éxito, la página elimina la tarea ya procesada y restablece el contexto; ante bloqueo o error conserva el contexto para una acción explícita del usuario. Estos assets se cargan solo desde el mismo bootstrap permitido.

### D-03 — Telemetría mínima mediante la auditoría existente

Se extenderá el modelo `AuditoriaTransicion` y su uso en `ServicioTransicionTarea` para producir una entrada estructurada y sanitizada por intento relevante. `WorkflowLegacyAuditoriaAdapter` seguirá siendo el único adaptador que escribe en la bitácora legacy existente.

Cada entrada incluirá referencia de correlación, identificador de usuario autorizado, tarea, ruta o flujo, conector, actividad destino, canal `MODERNO`, duración, resultado (`EXITO`, `BLOQUEADO` o `ERROR`), código funcional y referencia de auditoría. No incluirá login, SQL, credenciales, Session, documento ni payload de la solicitud.

### D-04 — Rollback solo por configuración y sin reversión de negocio

El rollback desactiva `WorkflowCentroTrabajoModernActive` y `WorkflowCentroTrabajoModernOfficialMode`, vacía las listas de piloto y metadatos operativos, y registra responsable, motivo, hora y correlación en la auditoría. Las nuevas aperturas muestran legacy; preview y ejecución ASMX posteriores quedan bloqueados por gate.

No se intenta deshacer una transición ya confirmada. Cualquier reversión de negocio usa exclusivamente el procedimiento legacy autorizado. La verificación posterior confirma que no existen transiciones duplicadas ni pérdida de contexto.

### D-05 — La promoción depende de evidencia y umbrales aprobados

El reporte del piloto comparará volumen, éxito, bloqueos, errores, duración, abandonos y divergencias por canal. Los umbrales cuantitativos, el alcance (listas de piloto o modo oficial), el responsable y la fecha de activación se registrarán en el paquete documental antes de activar el gate.

Una transición duplicada, pérdida de datos/contexto, filtración sensible, incumplimiento de autorización o fallo de rollback bloquea la promoción, sin importar métricas de éxito o duración.

### D-06 — Evidencia reproducible y documentación en la ruta obligatoria

La implementación añadirá pruebas focales para gate, serialización de auditoría y rollback; ejecutará compilación compatible con .NET Framework cuando esté disponible; y preparará una matriz manual. E2E o carga autenticada solo se ejecutan con la autorización explícita exigida por `AGENTS.md`.

La documentación final se ubicará únicamente en `Doc/Actualizacion/workflow/Terminar/06-piloto-pruebas-rollout/`, con su índice, contrato, flujo, evidencia y diagramas requeridos.

## Riesgos y mitigaciones

| Riesgo | Mitigación |
| --- | --- |
| Activación accidental para toda la población | D-01 falla a legacy salvo que el modo oficial, separado y explícito, tenga listas vacías y metadatos válidos. |
| Divergencia entre página y servicios | D-02 reutiliza el gate servidor y conserva revalidación ASMX. |
| Exposición de datos sensibles en métricas | D-03 limita el contrato y el adaptador de persistencia. |
| Doble transición o rollback de negocio | D-04 mantiene guard de concurrencia y no revierte estados confirmados. |
| Promoción sin evidencia | D-05 y D-06 requieren matriz, métricas, aprobación y paquete documental. |

## Plan de migración y reversa

1. Implementar y probar el gate *fail closed* con la configuración inicial desactivada.
2. Unificar el bootstrap de página con la decisión de servidor y completar telemetría mínima.
3. Crear el reporte, runbook y paquete documental sin cambiar configuración de ambiente.
4. Obtener aprobación de alcance, responsable y umbrales antes de la activación oficial explícita.
5. Ante bloqueo, dejar `WorkflowCentroTrabajoModernActive` y `WorkflowCentroTrabajoModernOfficialMode` en `false`, vaciar listas y atender únicamente reversión mediante el procedimiento legacy autorizado.

## Preguntas resueltas por esta planificación

- El piloto inició apagado; el estado actual es modo oficial explícito con listas piloto vacías.
- La auditoría usa la bitácora legacy existente a través de su adaptador, no una tabla o motor paralelo.
- La promoción no se automatiza: requiere aprobación explícita y evidencia documentada.
