<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## Context

DOC-32: BACKEND-ACTIVIDAD-ANTERIOR

## Jira Details

> # 01 — Backend seguro de devolución a actividad anterior
> 
> ## ROL ESPERADO
> 
> Actúa como desarrollador senior VB.NET de capas Domain, Application e Infrastructure para Workflow, con experiencia en concurrencia MySQL y encapsulación de motores existentes.
> 
> ## OBJETIVO
> 
> Implementar el corte completo de servidor de **Devolver a actividad anterior**: contrato exclusivo, autorización, preview de solo lectura y búsqueda paginada, ejecución segura, lock por tarea, adaptador y auditoría. Preview y ejecución se implementan juntos porque comparten contexto, conector, token y límite de seguridad.
> 
> ## CONTEXTO OBLIGATORIO
> 
> - Requiere que las decisiones obligatorias de `../Exploracion/` estén aprobadas: semántica Ruta/Flujo de `IdConector`, consulta de Ruta, cursor, notificación/eventos y sustitución de recorrido legacy.
> - Leer `00-contexto-obligatorio.md`, los documentos de `../Exploracion/` y la arquitectura existente de `../Terminar/`.
> - La salida habilita 02; no implementa UI, activación ni liberación.
> 
> ## REQUISITOS POSITIVOS
> 
> - Crear DTOs, modelos, puertos, códigos públicos y servicio exclusivos de devolución; no reutilizar los contratos de conectores salientes, envíos o Usuario anterior.
> - Exponer en el ASMX moderno existente `PreviewDevolverActividad(idTarea, termino?, cursor?, tamanoPagina?)` y `EjecutarDevolverActividad({ IdTarea, IdConector, TokenVersion })`, con sesión habilitada y contexto autenticado revalidado en servidor.
> - Calcular el permiso específico de devolución en servidor, fail-closed; no reutilizar permisos de envío ni aceptar actividad, usuario, grupo, Ruta, Flujo, tipo de contexto o destino desde el navegador.
> - Resolver el tipo Ruta/Flujo exclusivamente desde la tarea. Para Ruta validar `id_actividades_disponibles_envio`, `id_Ruta`, actividad origen, actividad siguiente y actividad actual. Para Flujo validar el conector entrante, Flujo, actividad destino actual y pertenencia real de usuario/grupo.
> - Ejecutar el preview exclusivamente con `SELECT` parametrizados. Filtrar y paginar solo después de construir el universo autorizado; aplicar término mínimo, tamaño máximo, orden estable y cursor opaco vinculado a tarea, contexto, término y orden.
> - Devolver datos mínimos: `IdConector` con semántica contextual, actividad, usuario/grupo resumido cuando aplique, tipo de contexto, `hayMas`, cursor y token. El `IdConector` de Ruta no puede resolverse como identificador de Flujo ni viceversa.
> - Adquirir un lock exclusivo por `IdTarea`, independiente del token. Dentro del lock releer y validar contexto, permiso, tarea, token, Ruta/Flujo y conector entrante; reconstruir el destino sin usar valores cliente.
> - Crear puerto y adaptador exclusivos. Solo ese adaptador invoca una vez `Terminar_Tarea_Workflow` con `Page = Nothing`, actualización de interfaz legacy desactivada y los parámetros de notificación/eventos aprobados en Exploración.
> - El adaptador y componentes nuevos no construyen ni invocan componentes de respuestas. Si el motor legacy los construye internamente, los parámetros aprobados deben impedir ejecutar sus métodos y una prueba focal debe demostrarlo. Normalizar éxito, bloqueo, error reintentable y advertencias; auditar con `ASMX_DEVOLVER_ACTIVIDAD`, sin datos sensibles.
> 
> ## RESTRICCIONES CRÍTICAS
> 
> - No implementar UI, feature flags, directorio global, rutas Web Forms alternativas ni cambios de configuración.
> - No modificar `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `ServicioTransicionTarea`, `ServicioEnvioGrupoTarea`, `EjecutarEnvioGrupo`, Usuario anterior ni contratos existentes de `IdConector`.
> - No invocar `Activa_devolver_actividades_anteriores`, `Enviar_actividad_por_conector_flujo_de_trabajo_anterior`, postbacks, handlers Web Forms, `GridView`, `UpdatePanel`, `ModalPopupExtender` ni ejecutores de otras operaciones.
> - No modificar el guard genérico si su contrato actual usa token; crear o parametrizar un guard exclusivo que preserve exclusión por tarea sin alterar otras capacidades.
> - Conector inexistente, ajeno, retirado, de contexto distinto, token vencido, permiso retirado, Ruta/Flujo inconsistente o lock ocupado bloquean antes del motor.
> - ejecutar E2E autenticada,  tarea real con autorización explícita de ambiente y cuentas de prueba.
> 
> ## REGLAS DE ANTIRREGRESIÓN
> 
> - El preview no revela conectores, actividades, usuarios ni grupos fuera del universo autorizado y no escribe tarea, estado, auditoría, eventos ni datos de negocio.
> - Existe un único punto mutante de devolución; el navegador nunca autoriza el destino ni el contexto de la arista.
> - Una falla de auditoría no revierte una transición confirmada; solicitudes concurrentes, incluso con tokens distintos, no devuelven dos veces la tarea.
> - Continuar flujo conserva conector saliente, endpoints, payload, validaciones, adaptador y pruebas actuales.
> 
> ## CRITERIOS DE ACEPTACIÓN
> 
> - Ruta y Flujo solo devuelven aristas entrantes válidas del contexto actual, con semánticas aisladas detrás del mismo endpoint.
> - Cursor, término, conector o identificador manipulados devuelven resultado público seguro sin fuga ni escritura.
> - Errores y bloqueos públicos no exponen SQL, sesión, controles Web Forms, credenciales ni excepciones internas.
> 
> ## PRUEBAS OBLIGATORIAS
> 
> Agregar y ejecutar pruebas focales para permiso específico, preview sin escritura, Ruta con filtro de Ruta/arista, Flujo, filtro parametrizado, término mínimo, límite, orden, cursor de otro contexto, lista sintética extensa, token vencido, conector manipulado/retirado/de contexto distinto, lock por tarea con tokens distintos, concurrencia, política de eventos/notificación aprobada, auditoría fallida, éxito simulado y ausencia de invocaciones a métodos de respuestas. Ejecutar MSBuild disponible y pruebas afectadas; registrar comando, resultado, cobertura y limitaciones.  E2E obligatorio.
> 
> ## DOCUMENTACIÓN TÉCNICA
> 
> Actualizar `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md` y `04-pruebas-y-evidencia.md` bajo `../01-implementacion-devolver-actividad-anterior/` con identidad contextual de arista, cursor, punto mutante, lock, parámetros aprobados, auditoría, estados/error y relevo a 02.
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

1. Las decisiones funcionales y tecnicas se completan durante `opsxj:refine`; no se inyectan politicas de otro perfil tecnologico.


## Risks / Trade-offs

- El refinamiento debe identificar compatibilidad, riesgos y limites del modulo afectado antes de iniciar cambios.

## Migration Plan

1. Completar y aprobar `refinement.md` antes de marcar tareas de implementacion.
2. Sincronizar cada decision con design, spec y tasks mediante `opsxj:refine --sync`.

## Open Questions

- TBD

## Diseño refinado

### D-01 — Capacidad aislada de devolución

Se crearán modelos, DTOs, puertos, validador, servicio, repositorios, guard y adaptador bajo una capacidad `Devolver`. Los contratos existentes de `Terminar` y de Enviar a usuario/grupo no se reutilizan ni se modifican. El ASMX moderno añade dos métodos y solo compone la capacidad nueva.

### D-02 — Autorización e identidad contextual

El endpoint obtiene el contexto autenticado con sesión habilitada y calcula el permiso específico de devolver en el servidor. La tarea determina el tipo: en Ruta, `IdConector` es exclusivamente `actividades_disponibles_envio.id_actividades_disponibles_envio`; en Flujo, es exclusivamente el registro de conector entrante del Flujo. Cada repositorio consulta y valida solo su semántica.

### D-03 — Preview de solo lectura y cursor ligado

El servicio construye primero el universo autorizado y solo después aplica el término mínimo, límite máximo, orden determinista y paginación. El cursor se protege y contiene identidad de tarea, usuario/contexto, tipo, término normalizado, orden y la última clave; un cursor ajeno se trata como bloqueo funcional. Las consultas de preview usan únicamente SELECT parametrizados.

### D-04 — Ejecución exclusiva por tarea

La capacidad usa un guard nuevo que adquiere `GET_LOCK` con un nombre derivado solo de `IdTarea`. Dentro del lease vuelve a evaluar permiso, contexto, tarea activa, token, tipo Ruta/Flujo y conector entrante antes de invocar el adaptador. El guard actual permanece sin cambios porque su identidad contiene token.

### D-05 — Adaptador único con efectos legacy preservados

`WorkflowLegacyDevolverActividadExecutorAdapter` será el único componente nuevo que llame `ClassWorkflow.Terminar_Tarea_Workflow`. Su llamada usa `Page = Nothing`, desactiva actualización de interfaz y reasignaciones, conserva eventos dinámicos y conserva la notificación de asignación a partir del destino reconstruido. No llama `Activa_devolver_actividades_anteriores`, `Enviar_actividad_por_conector_flujo_de_trabajo_anterior`, postbacks ni controles.

### D-06 — Resultado y auditoría sanitizados

El servicio devuelve códigos funcionales, mensaje visible, estado, advertencias, token y referencia de auditoría sin serializar excepciones ni datos técnicos. La auditoría utiliza `ASMX_DEVOLVER_ACTIVIDAD`; si falla después de una transición exitosa, añade una advertencia y no intenta revertir el motor.

### D-07 — Compatibilidad y evidencia

No se implementa UI, feature flag ni configuración de ambiente. Las pruebas estáticas y focales verifican ausencia de dependencias de respuestas, no escritura en preview, semánticas Ruta/Flujo, concurrencia y la no modificación de contratos existentes. La documentación técnica describe el relevo al ticket 02 y la evidencia E2E protegida.

### D-08 — E2E real protegida, concurrencia y rendimiento acotado

DOC-32 incorpora una suite E2E propia que reutiliza exclusivamente `tools/e2e/tests/support/authenticated-workflow-session.cjs` para iniciar sesión. La suite recibe secretos efímeros por entorno, usa una cuenta MySQL de solo lectura para dos controles `SELECT` con un parámetro `?`, y nunca escribe configuración ni datos directamente. La ejecución real requiere una tarea descartable y autorización explícita; la carrera usa exactamente dos solicitudes sobre una tarea descartable distinta, comprueba una sola transición, un bloqueo seguro y registra las latencias observadas. Los presupuestos máximos de latencia se declaran por ambiente y bloquean la corrida cuando no están presentes o se exceden.
