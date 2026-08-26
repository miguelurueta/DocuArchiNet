<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Context

DOC-36: BACKEND-DEVOLUCION-USUARIO-ANTERIOR

## Jira Details

> # 02 — Backend seguro de devolución a usuario anterior
> 
> ## ROL ESPERADO
> 
> Actúa como desarrollador senior VB.NET de capas Domain, Application e Infrastructure para Workflow, con experiencia en concurrencia MySQL y encapsulación de motores existentes.
> 
> ## OBJETIVO
> 
> Implementar el corte completo de servidor de **Devolver a usuario anterior**: contrato exclusivo, autorización, `PreviewDevolverUsuarioAnterior` de solo lectura, `EjecutarDevolverUsuarioAnterior`, lock por tarea, adaptador, auditoría y pruebas focales. Preview y ejecución se implementan juntos porque comparten el mismo historial, token y límite de seguridad.
> 
> ## CONTEXTO OBLIGATORIO
> 
> - Requiere 01 aprobado, con decisiones explícitas sobre historial, token, lock, parámetros de `Terminar_Tarea_Workflow`, notificación y eventos dinámicos.
> - Leer `00-contexto-obligatorio.md`, los documentos de `../Exploracion/` y la evidencia de 01.
> - La salida habilita 03; no implementa UI, activación ni liberación.
> 
> ## REQUISITOS POSITIVOS
> 
> - Crear DTOs, modelos, puertos, códigos públicos y servicio exclusivos de devolución a usuario anterior; no reutilizar contratos de actividades, conectores ni envíos.
> - Exponer en el ASMX moderno existente `PreviewDevolverUsuarioAnterior(idTarea)` y `EjecutarDevolverUsuarioAnterior({ IdTarea, TokenVersion })`, con sesión habilitada y contexto autenticado revalidado en servidor.
> - Calcular el permiso de devolución específico en servidor, fail-closed; no reutilizar `PuedeCambioRuta` ni aceptar permisos, destino, Ruta, Flujo, actividad, grupo o historial desde el navegador.
> - Ejecutar el preview exclusivamente con `SELECT` parametrizados. Resolver cero o un registro histórico mediante el algoritmo aprobado en 01, validar tarea activa y accesible, usuario elegible, actividad y Ruta/Flujo consistentes, y devolver datos mínimos para confirmación junto con token opaco.
> - Emitir un token que vincule versión de tarea e identificador del registro histórico confirmado. El endpoint de ejecución recibe solo ese token, no un identificador histórico adicional.
> - Adquirir un lock exclusivo por `IdTarea`, independiente del token. Dentro del lock releer y validar contexto, permiso, tarea, token, registro histórico, usuario, actividad, Ruta/Flujo y auto-devolución frente al usuario Workflow autenticado.
> - Crear puerto y adaptador exclusivos. Solo ese adaptador invoca una vez `Terminar_Tarea_Workflow` con `Page = Nothing`, actualización de interfaz legacy desactivada y los parámetros de notificación/eventos aprobados en 01.
> - La política aprobada de notificación no permite que el adaptador ni los componentes nuevos construyan o invoquen componentes de respuestas. Si el motor legacy los construye internamente, los parámetros aprobados deben impedir llamar sus métodos y una prueba focal debe demostrarlo. Normalizar éxito, bloqueo, error reintentable y advertencias; auditar con `ASMX_DEVOLVER_USUARIO_ANTERIOR`, sin datos sensibles.
> 
> ## RESTRICCIONES CRÍTICAS
> 
> - No implementar UI, feature flags, directorio global, búsqueda, paginación, selector de destinos ni cambios de configuración.
> - No modificar contratos o comportamiento de Devolver a actividad anterior, Continuar flujo, Enviar a usuario, Enviar a grupo, `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `ServicioTransicionTarea` ni `IdConector`.
> - No invocar el método legacy `Devolver_tarea_workflow_usuario_anterior`, postbacks, handlers Web Forms, `GridView`, `UpdatePanel`, `ModalPopupExtender` ni ejecutores de otras operaciones.
> - Los componentes nuevos no referencian `Classgestionrespuesta`, `Verifica_respuesta_*`, `Reasigna_respuesta_envia_tarea_usuario` ni componentes de respuestas.
> - No modificar el guard genérico de transiciones si su contrato actual usa token; crear o parametrizar un guard exclusivo que preserve la exclusión por tarea sin alterar las demás operaciones.
> - Historial ausente, de grupo, no elegible, distinto del confirmado, auto-devolución, token vencido, permiso retirado o lock ocupado bloquean antes del motor.
> - No ejecutar E2E autenticada, carga ni una tarea real sin autorización explícita de ambiente y cuentas de prueba.
> 
> ## REGLAS DE ANTIRREGIÓN
> 
> - El preview no revela registros de otras tareas ni destinos alternativos y no escribe tarea, estado, auditoría, eventos ni datos de negocio.
> - Existe un único punto mutante directo y el navegador nunca autoriza ni el destino ni el registro histórico.
> - Una falla de auditoría no revierte una transición ya confirmada; solicitudes concurrentes, incluso con tokens distintos, no devuelven dos veces la tarea.
> - Las demás operaciones preservan endpoints, payloads, adaptadores, feature gates y pruebas existentes.
> 
> ## CRITERIOS DE ACEPTACIÓN
> 
> - El preview devuelve cero o un usuario histórico elegible, nunca grupo, actividad alternativa, conector ni datos de respuestas.
> - Si el historial cambia desde el preview, la ejecución bloquea; nunca devuelve a un destino distinto del confirmado.
> - La auto-devolución compara el usuario histórico con el usuario Workflow autenticado real, nunca con `Id_Ruta_Workflow`.
> - Errores y bloqueos públicos no exponen SQL, sesión, controles Web Forms, credenciales ni excepciones internas.
> 
> ## PRUEBAS OBLIGATORIAS
> 
> Agregar y ejecutar pruebas focales para permiso específico, preview sin escritura, orden/desempate de historial, historial válido/ausente/de grupo, usuario retirado, Ruta/Flujo inconsistente, auto-devolución, SQL parametrizado, token vencido, token con historial cambiado, lock por tarea con tokens distintos, concurrencia, advertencia aprobada, auditoría fallida, éxito simulado y ausencia de referencias a componentes de respuestas. Ejecutar MSBuild disponible y pruebas afectadas; registrar comando, resultado, cobertura y limitaciones. No E2E.
> 
> ## DOCUMENTACIÓN TÉCNICA
> 
> Actualizar `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md` y `04-pruebas-y-evidencia.md` bajo `../01-implementacion-devolver-usuario-anterior/` con algoritmo de historial, token, lock, punto mutante, parámetros aprobados, auditoría, estados/error y relevo a 03.
> 
> ## ENTREGABLE FINAL
> 
> Reportar ticket, cambios backend, archivos, pruebas, compilación, evidencia de no escritura, trazabilidad sanitizada, decisiones aplicadas y confirmación de no regresión. No implementar UI ni cambiar configuración de ambiente.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

### D-01 — Último usuario histórico real

La fila actual se identifica por su `id_Estado` revalidado y el antecedente se resuelve en servidor como el registro histórico más reciente con `Id_Usuario > 0` y `id_Estado` menor al actual, ordenado por `id_Estado DESC LIMIT 1`. Los snapshots grupales se saltan porque no son un usuario destino; si no existe ningún usuario histórico elegible, se bloquea. `id_Estado` evita depender de fechas y no permite que el cliente indique un usuario.

### D-02 — Token exclusivo

Un protector exclusivo de usuario anterior emitirá un token opaco de cinco minutos con tarea, `id_Estado` actual y `id_Estado` histórico. Ejecución recibe solo `{ IdTarea, TokenVersion }` y exige coincidencia completa dentro del lock.

### D-03 — Concurrencia por tarea

El guard exclusivo usa `GET_LOCK('workflow-return-user-' + IdTarea, 0)` y retiene su conexión hasta liberar en `Finally`. No comparte contrato ni nombre de lock con otras operaciones y no se deriva del token.

### D-04 — Autorización y destino de servidor

El contexto autenticado calcula el permiso específico. Preview y ejecución validan tarea accesible, usuario histórico positivo/elegible y consistencia de Ruta, flujo y actividad de flujo según las reglas existentes de recuperación; no dependen de `ESTADO_RECUPERACION_FLUJO_TRABAJO` como permiso. La auto-devolución compara el histórico contra `IdUsuarioWorkflow` autenticado; no usa Ruta ni valores del cliente. Cuando el antecedente de flujo trae `ID_USUARIO_WORKFLOW_FLUJO_TRABAJO` positivo, se conserva aunque sea diferente del usuario destino; únicamente se completa con el usuario destino si viene en cero, igual que el motor legado.

### D-05 — Adaptador mutante exclusivo

El adaptador nuevo es el único invocador de `Terminar_Tarea_Workflow`. No llama el método legacy de devolución, postbacks ni controles Web Forms. La llamada usa `Page = Nothing`, usuario/actividad/ruta/flujo revalidados y una sola invocación.

### D-06 — Política de motor sin respuestas

El adaptador fija `notifica = 0`, `notifica_envio_correo = 0`, actualización legacy `0`, eventos dinámicos `0`, reasignación SII `0` y reasignación de tarea `0`. No se referencian componentes ni métodos de respuestas.

### D-07 — Auditoría posterior saneada

La auditoría registra la acción `ASMX_DEVOLVER_USUARIO_ANTERIOR` con referencia opaca. Una falla de auditoría luego de éxito se devuelve como advertencia y no revierte la transición.


## Risks / Trade-offs

- El refinamiento debe identificar compatibilidad, riesgos y limites del modulo afectado antes de iniciar cambios.

## Migration Plan

1. Completar y aprobar `refinement.md` antes de marcar tareas de implementacion.
2. Sincronizar cada decision con design, spec y tasks mediante `opsxj:refine --sync`.

## Open Questions

- Ninguna para la implementación de servidor. UI, activación y liberación permanecen en etapas posteriores.
