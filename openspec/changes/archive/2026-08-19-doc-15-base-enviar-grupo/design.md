<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
# Diseño técnico — DOC-15: Enviar a grupo

## Alcance y límites

La modernización agrega una operación de envío directo a actividad al límite existente `WebServiceWorkflowModern.asmx`. No cambia la transición moderna por conector ni el motor legacy. El navegador aporta intención; el servidor resuelve la autorización y todos los argumentos de ejecución.

## Decisiones de diseño

### D-01 — Contrato directo de grupo

Se crearán contratos específicos para preview y ejecución de grupo. La ejecución acepta exclusivamente `IdTarea`, `IdActividadDestino` y `TokenVersion`; el tipo de destino se identifica como `ENVIO_GRUPO_DIRECTO`. Los contratos de `SolicitudTransicionWorkflow`, `PreviewEnviarTarea`, `EjecutarEnvioTarea` y `ServicioTransicionTarea` continúan reservados a `IdConector > 0`.

### D-02 — Único límite de habilitación

El ASMX construye el contexto desde `WorkflowPreviewSessionContextGate` y tanto preview como ejecución reevalúan `IWorkflowModernFeatureGate`. `WorkflowModernPresentationBootstrap` solo determina si se enlaza la experiencia visual. No se agrega capacidad, bandera, `appSetting` ni evaluación paralela; configuración ausente, inválida o fuera de alcance falla cerrada.

### D-03 — Preview de lectura y destino específico

Un repositorio de preview de grupo obtiene actividades válidas de la ruta con consultas `SELECT` y devuelve datos sanitizados. No llama `Class_Listado_Actividades_workflow` con controles Web Forms, no registra auditoría y no invoca el motor. La actividad que llega del navegador vuelve a resolverse durante ejecución.

### D-04 — Ejecución revalidada y serializada

`ServicioEnvioGrupoTarea` coordina validación de solicitud, gate, `GET_LOCK`, relectura de tarea y comparación de `TokenVersion`. Dentro del lock valida `Cambio_Ruta`, tarea activa, ruta, flujo/actividad de flujo cuando apliquen y pertenencia actual de `IdActividadDestino` a la ruta. Un repositorio de ejecución resuelve todos los valores del destino desde servidor.

### D-05 — Requisitos del caso de grupo

El adaptador de requisitos bloquea aprobaciones pendientes y normaliza su resultado. No reutiliza sin revisión el adaptador de transición por conector si incorpora la regla de respuesta radicada, porque esa regla no existe en el flujo legacy de grupo. La respuesta radicada permanece fuera del alcance hasta una aprobación funcional explícita.

### D-06 — Frontera exclusiva con el motor legacy

Un adaptador directo recibe `DestinoEjecucionWorkflow` autorizado e invoca `ClassWorkflow.Terminar_Tarea_Workflow` sin `Page`, sin controles y con actualización de interfaz desactivada. Para grupo usa conector e identificadores de flujo en cero, preservando el comportamiento de la llamada legacy, sus eventos y correo. El ASMX, Application y JavaScript no llaman al motor.

### D-07 — Presentación progresiva

La página enlaza assets y manejadores modernos de grupo solo cuando el bootstrap existente está activo. El componente de selección/confirmación recibe el identificador de actividad y llama los endpoints de grupo. Con gate inactivo no registra ese comportamiento y conserva el postback/modal actual. La ruta de Continuar flujo no se modifica.

### D-08 — Resultado y auditoría

La aplicación retorna resultados públicos normalizados y registra auditoría sanitizada con `Canal=MODERNO`, `Mecanismo=ASMX_ENVIO_GRUPO` y conector cero. Fallas o advertencias posteriores se expresan sin datos técnicos; una advertencia no revierte un éxito del motor.

### D-09 — Validación y reversa

Las pruebas cubren autorización, preview sin escritura, estado de ruta/flujo, destino retirado, aprobación pendiente, token, concurrencia, fallback y no regresión de conector. E2E autenticada, carga y cambios del gate están prohibidos salvo autorización expresa. El rollback consiste en usar el gate existente inactivo para que los nuevos intentos vuelvan al postback legacy, sin alterar transiciones confirmadas.

## Secuencia de ejecución

```text
UI Enviar a grupo
  -> PreviewEnviarGrupo(idTarea) [solo SELECT]
  -> selección y confirmación de IdActividadDestino + TokenVersion
  -> EjecutarEnvioGrupo(idTarea, idActividadDestino, tokenVersion)
  -> contexto autenticado + gate existente
  -> validación de solicitud + GET_LOCK
  -> relectura de tarea, permiso, ruta/flujo y destino
  -> requisitos de grupo
  -> adaptador directo a Terminar_Tarea_Workflow
  -> auditoría sanitizada + actualización visual
```

## Áreas afectadas

| Capa | Área | Responsabilidad |
| --- | --- | --- |
| Presentation | `workflow/Webworkflow.aspx`, `.aspx.vb` y JavaScript asociado | Bootstrap, interacción accesible y fallback; sin SQL ni motor. |
| ASMX | `webservice/WebServiceWorkflowModern.asmx.vb` | Contexto, composición y respuesta JSON segura. |
| Application | `Services/Workflow/Terminar/` | Orquestación de preview/ejecución y códigos públicos. |
| Domain/Modelo | `Modelo/Workflow/Terminar/` | DTOs y puertos de grupo, sin Web Forms ni Infrastructure. |
| Infrastructure | `Infrastructure/Workflow/Terminar/`, `Infrastructure/Repositories/Workflow/` | SELECT de destinos, revalidación, guard, auditoría y adaptador legacy. |

## Riesgos y mitigaciones

- El destino de preview puede cambiar antes de confirmar: se resuelve nuevamente dentro del lock.
- El contrato actual exige conector: la operación hermana impide relajar esa regla y evita un conector artificial.
- El motor legacy puede informar advertencias posteriores: el resultado separa éxito confirmado de advertencias sanitizadas.
- El gate no autoriza negocio: `Cambio_Ruta` y estado de tarea/ruta/destino se revalidan en servidor.
