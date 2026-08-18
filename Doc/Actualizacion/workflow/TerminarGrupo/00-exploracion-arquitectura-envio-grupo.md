# Exploración arquitectónica: modernización de "Enviar a grupo"

## Estado y alcance

Este documento registra una exploración técnica. No contiene una decisión de implementación aprobada ni modifica el comportamiento actual.

La operación analizada es el comando **Enviar a grupo** de `workflow/Webworkflow.aspx`. Se compara con **Continuar flujo**, modernizado previamente mediante preview, confirmación y ejecución controlada.

## Conclusión

Es viable reutilizar componentes transversales y de presentación de `Continuar flujo`, pero **Enviar a grupo no debe modelarse como una transición RUTA/FLUJO ni como un conector ficticio**.

`Continuar flujo` selecciona un conector autorizado desde la actividad actual. `Enviar a grupo` realiza un reenvío directo a una actividad de la ruta. Sus identificadores, reglas de selección y semántica de ejecución son diferentes.

```text
Continuar flujo                         Enviar a grupo
──────────────────────────────          ─────────────────────────────────
IdConector                              IdActividadDestino
Solo conectores salientes autorizados   Actividades disponibles de la ruta
Transición RUTA o FLUJO                 Reenvío directo a actividad
Configuración del conector              Permiso Cambio_Ruta y reglas legacy
```

## Mapa de los flujos actuales

| Aspecto | Continuar flujo moderno | Enviar a grupo legacy |
| --- | --- | --- |
| Disparador | `workflow-transition-trigger` | `ImageButtonEnviaActividad` |
| Lista de destinos | Conectores RUTA o FLUJO desde el estado actual | Actividades de la ruta seleccionada |
| Identificador cliente | `idConector` | Id de actividad destino |
| Preview | ASMX de solo lectura | Postback que carga `GridView` |
| Confirmación | `ConfirmationDialog` | `window.confirm` |
| Ejecución | `EjecutarEnvioTarea` | `Button_tool_enviar_actividad` |
| Motor final | Adaptador a `Terminar_Tarea_Workflow` | Llamada directa a `Terminar_Tarea_Workflow` |

Referencias de implementación:

- El menú conserva comandos diferentes en `workflow/Webworkflow.aspx`.
- `ImageButtonEnviaActividad_Click` verifica permiso, estado de flujo y ruta antes de listar actividades.
- `Class_Listado_Actividades_workflow.Solicita_listado_actividades_ruta` lista las actividades de la ruta, no conectores salientes.
- `ServicioTransicionTarea` valida y resuelve un `IdConector`; ese contrato no representa el reenvío a grupo.

## Diferencias que impiden reutilización directa

1. **Destino.** `MySqlTransicionRutaRepository` consulta `actividades_disponibles_envio` para el grupo y actividad de origen. Esa lista es deliberadamente más restrictiva que el listado legacy de todas las actividades de la ruta.
2. **Contrato de ejecución.** `SolicitudTransicionWorkflow`, `ValidadorTransicionTarea`, `MySqlTransicionEjecucionRepository` y `WorkflowLegacyExecutorAdapter` requieren un conector positivo.
3. **Semántica de flujo.** El envío legacy a grupo llama a `Terminar_Tarea_Workflow` con los identificadores de flujo en cero y sin conector. Debe tratarse como reenvío directo, incluso cuando la tarea proviene de un flujo abierto.
4. **Requisitos.** El envío legacy valida aprobaciones pendientes, pero no aplica la comprobación de respuesta radicada que sí está en `WorkflowLegacyRequisitosAdapter`. Reutilizar ese adaptador íntegro cambiaría el comportamiento funcional.
5. **Autorización.** El permiso `Cambio_Ruta`, la apertura de ruta y la apertura de flujo/actividad se comprueban hoy antes de mostrar el listado. En un camino moderno deben revalidarse en el servidor durante la ejecución, dentro del lock.

## Elementos reutilizables

| Componente | Reutilización | Tratamiento recomendado |
| --- | --- | --- |
| Feature gate | Parcial | No activar el envío a grupo automáticamente al habilitar `Continuar flujo`; usar una capacidad u opt-in específico, inicialmente desactivado. |
| Contexto autenticado | Parcial | Reutilizar `WorkflowPreviewSessionContextGate`; exponer de manera segura el permiso efectivo `Cambio_Ruta`. |
| Tarea y token de versión | Sí | Reutilizar `ITareaWorkflowRepository` y la relectura de tarea activa. |
| Concurrencia | Sí | Reutilizar `MySqlTransicionConcurrencyGuard` con lock por tarea y token. |
| Auditoría adicional | Sí | Reutilizar el adaptador, registrando un mecanismo distintivo como `ASMX_ENVIO_GRUPO`. El conector será `0`. |
| Motor legacy | Sí, con adaptador nuevo | Crear un adaptador explícito de envío directo que llame a `Terminar_Tarea_Workflow` sin `Page` ni controles Web Forms. |
| Lista/modal de destinos | Parcial | Reutilizar HTML, CSS, accesibilidad y diálogo de confirmación; parametrizar trigger, endpoint y tipo de identificador. |
| Presentación posterior al éxito | Sí | Reutilizar la eliminación de fila, limpieza de contexto y mensaje de éxito. |
| Repositorios de destino RUTA/FLUJO | No | Crear un repositorio específico de destinos de envío a grupo. |
| `ServicioTransicionTarea` | No, como servicio completo | Mantenerlo para conectores. No relajar su regla de `IdConector > 0`. |
| `WorkflowLegacyRequisitosAdapter` | Parcial | Extraer o crear validaciones específicas de grupo; no importar accidentalmente la regla de respuesta. |

## Arquitectura propuesta

Crear una operación hermana, por ejemplo `ServicioEnvioGrupoTarea`, sin modificar el significado de la transición moderna existente.

```text
UI Enviar a grupo
        |
        +--> PreviewEnviarGrupo (solo SELECT)
        |       +--> autorización Cambio_Ruta
        |       +--> tarea/ruta/flujo abiertos
        |       +--> destinos de actividad de la ruta
        |
        +--> Confirmación moderna
        |
        +--> EjecutarEnvioGrupo
                +--> gate y contexto autenticado
                +--> valida idTarea + idActividadDestino + token
                +--> GET_LOCK por tarea/version
                +--> relee tarea y reautoriza destino
                +--> valida aprobación y reglas propias
                +--> adaptador legacy de envío directo
                +--> auditoría adicional
                +--> actualización visual de éxito
```

### Contratos sugeridos

```text
PreviewEnviarGrupo(idTarea)
  -> PrevisualizacionEnvioGrupoDto

EjecutarEnvioGrupo(idTarea, idActividadDestino, tokenVersion)
  -> ResultadoTransicionDto
```

El destino recibido desde el navegador será informativo. Durante la ejecución se debe volver a resolver y autorizar en el servidor. El preview debe hacer exclusivamente lecturas y no modificar tarea, estado ni auditoría.

## Invariantes de seguridad y compatibilidad

- No usar un conector simulado para representar una actividad destino.
- No invocar handlers Web Forms desde el ASMX ni manipular controles de página en la ejecución moderna.
- Mantener `Terminar_Tarea_Workflow` como único motor que modifica estados y conserva eventos dinámicos legacy.
- Revalidar permiso `Cambio_Ruta`, tarea activa, token, ruta/flujo abiertos y pertenencia de la actividad destino a la ruta dentro del lock.
- Mantener el fallback legacy cuando la nueva capacidad esté deshabilitada.
- El gate debe permanecer desactivado fuera de un piloto autorizado.
- No ejecutar E2E autenticado, carga ni activación de gate sin autorización expresa.

## Matriz mínima de pruebas

| Caso | Resultado esperado |
| --- | --- |
| Usuario sin `Cambio_Ruta` | Preview y ejecución bloqueados sin listar destinos. |
| Ruta cerrada | Bloqueo controlado. |
| Flujo o actividad de flujo cerrados | Bloqueo controlado. |
| Actividad destino fuera de la ruta | Rechazo durante ejecución. |
| Solicitud de aprobación pendiente | Bloqueo funcional. |
| Conflicto de token | Solicitud rechazada sin cambio de estado. |
| Dos envíos simultáneos | Solo uno ejecuta; el otro recibe conflicto o envío en progreso. |
| Actividad con notificación | Conserva la configuración de correo de la actividad destino. |
| Éxito | El motor legacy finaliza la tarea; la interfaz limpia selección y listado. |
| Regresión de continuar flujo | Conserva `idConector`, destinos por conector y contrato ASMX actual. |

## Prompts para planificar e implementar

| Objetivo | Prompt sugerido | Resultado esperado |
| --- | --- | --- |
| Crear propuesta | "Crea una propuesta OpenSpec para modernizar `Enviar a grupo` en Workflow como operación hermana de `Continuar flujo`. No reutilices `ServicioTransicionTarea` como está ni inventes conectores. El destino es `IdActividadDestino`, no `IdConector`." | Propuesta, diseño, especificación y tareas. |
| Definir contrato | "Documenta el contrato de `Enviar a grupo`: preview de solo lectura y ejecución. El preview lista actividades de la ruta; la ejecución recibe `idTarea`, `idActividadDestino` y `tokenVersion`. Define códigos de bloqueo estables." | DTOs, endpoints y reglas de validación. |
| Implementar backend | "Implementa `ServicioEnvioGrupoTarea` paralelo a `ServicioTransicionTarea`. Reutiliza gate, relectura de tarea, `GET_LOCK`, token, auditoría y motor legacy; conserva un resolver de destino específico para actividad directa." | Orquestación segura. |
| Preservar autorización | "Implementa validación de envío a grupo en servidor: permiso `Cambio_Ruta`, tarea activa, ruta abierta, flujo abierto y actividad de flujo abierta. Revalida todo dentro del lock antes de ejecutar." | Paridad de seguridad. |
| Resolver destinos | "Crea un repositorio de lectura específico para envío a grupo que retorne las actividades permitidas de la ruta. No reutilices `MySqlTransicionRutaRepository`, porque solo retorna conectores salientes." | Preview correcto. |
| Ejecutar sin conector | "Crea un adaptador legacy explícito para `ENVIO_GRUPO_DIRECTO`. Debe invocar `Terminar_Tarea_Workflow` con la actividad destino, sin conector y sin manipular controles Web Forms." | Ejecución aislada. |
| Requisitos | "Implementa requisitos propios de envío a grupo. Conserva aprobaciones pendientes. No agregues validación de respuesta radicada salvo aprobación explícita, porque el flujo legacy actual no la aplica." | Sin regresión funcional. |
| Modernizar UI | "Extrae un núcleo reutilizable del modal de destinos y `ConfirmationDialog`, parametrizable por trigger, endpoint, evento e identificador. Conserva compatibilidad total con `Continuar flujo`, que usa `idConector`." | Reutilización visual controlada. |
| Pruebas | "Añade pruebas para permiso, ruta/flujo cerrado, destino fuera de ruta, aprobación pendiente, conflicto de versión, concurrencia, notificación, éxito y regresión de `Continuar flujo`." | Cobertura de seguridad y regresión. |

## Recomendación de siguiente paso

Formalizar primero el cambio con una propuesta OpenSpec. La decisión central que debe aprobarse es mantener la semántica legacy de envío directo, incluyendo la ausencia actual de validación de respuesta radicada, o declarar explícitamente ese endurecimiento como requisito funcional nuevo.
