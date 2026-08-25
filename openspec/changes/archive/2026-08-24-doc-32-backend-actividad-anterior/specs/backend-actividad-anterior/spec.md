<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## ADDED Requirements
### Requirement: BACKEND-ACTIVIDAD-ANTERIOR
El sistema SHALL implementar el alcance definido para DOC-32.
#### Scenario: Flujo principal
- **WHEN** se ejecuta el caso de uso principal del ticket
- **THEN** el comportamiento coincide con las reglas funcionales esperadas
#### Scenario: No-regresion
- **WHEN** se valida el modulo afectado
- **THEN** no se rompen flujos existentes
### Requirement: Detalle funcional Jira
El sistema SHALL considerar las reglas detalladas del ticket.

#### Scenario: Reglas del ticket
- # 01 — Backend seguro de devolución a actividad anterior
- 
- ## ROL ESPERADO
- 
- Actúa como desarrollador senior VB.NET de capas Domain, Application e Infrastructure para Workflow, con experiencia en concurrencia MySQL y encapsulación de motores existentes.
- 
- ## OBJETIVO
- 
- Implementar el corte completo de servidor de **Devolver a actividad anterior**: contrato exclusivo, autorización, preview de solo lectura y búsqueda paginada, ejecución segura, lock por tarea, adaptador y auditoría. Preview y ejecución se implementan juntos porque comparten contexto, conector, token y límite de seguridad.
- 
- ## CONTEXTO OBLIGATORIO
- 
- - Requiere que las decisiones obligatorias de `../Exploracion/` estén aprobadas: semántica Ruta/Flujo de `IdConector`, consulta de Ruta, cursor, notificación/eventos y sustitución de recorrido legacy.
- - Leer `00-contexto-obligatorio.md`, los documentos de `../Exploracion/` y la arquitectura existente de `../Terminar/`.
- - La salida habilita 02; no implementa UI, activación ni liberación.
- 
- ## REQUISITOS POSITIVOS
- 
- - Crear DTOs, modelos, puertos, códigos públicos y servicio exclusivos de devolución; no reutilizar los contratos de conectores salientes, envíos o Usuario anterior.
- - Exponer en el ASMX moderno existente `PreviewDevolverActividad(idTarea, termino?, cursor?, tamanoPagina?)` y `EjecutarDevolverActividad({ IdTarea, IdConector, TokenVersion })`, con sesión habilitada y contexto autenticado revalidado en servidor.
- - Calcular el permiso específico de devolución en servidor, fail-closed; no reutilizar permisos de envío ni aceptar actividad, usuario, grupo, Ruta, Flujo, tipo de contexto o destino desde el navegador.
- - Resolver el tipo Ruta/Flujo exclusivamente desde la tarea. Para Ruta validar `id_actividades_disponibles_envio`, `id_Ruta`, actividad origen, actividad siguiente y actividad actual. Para Flujo validar el conector entrante, Flujo, actividad destino actual y pertenencia real de usuario/grupo.
- - Ejecutar el preview exclusivamente con `SELECT` parametrizados. Filtrar y paginar solo después de construir el universo autorizado; aplicar término mínimo, tamaño máximo, orden estable y cursor opaco vinculado a tarea, contexto, término y orden.
- - Devolver datos mínimos: `IdConector` con semántica contextual, actividad, usuario/grupo resumido cuando aplique, tipo de contexto, `hayMas`, cursor y token. El `IdConector` de Ruta no puede resolverse como identificador de Flujo ni viceversa.
- - Adquirir un lock exclusivo por `IdTarea`, independiente del token. Dentro del lock releer y validar contexto, permiso, tarea, token, Ruta/Flujo y conector entrante; reconstruir el destino sin usar valores cliente.
- - Crear puerto y adaptador exclusivos. Solo ese adaptador invoca una vez `Terminar_Tarea_Workflow` con `Page = Nothing`, actualización de interfaz legacy desactivada y los parámetros de notificación/eventos aprobados en Exploración.
- - El adaptador y componentes nuevos no construyen ni invocan componentes de respuestas. Si el motor legacy los construye internamente, los parámetros aprobados deben impedir ejecutar sus métodos y una prueba focal debe demostrarlo. Normalizar éxito, bloqueo, error reintentable y advertencias; auditar con `ASMX_DEVOLVER_ACTIVIDAD`, sin datos sensibles.
- 
- ## RESTRICCIONES CRÍTICAS
- 
- - No implementar UI, feature flags, directorio global, rutas Web Forms alternativas ni cambios de configuración.
- - No modificar `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `ServicioTransicionTarea`, `ServicioEnvioGrupoTarea`, `EjecutarEnvioGrupo`, Usuario anterior ni contratos existentes de `IdConector`.
- - No invocar `Activa_devolver_actividades_anteriores`, `Enviar_actividad_por_conector_flujo_de_trabajo_anterior`, postbacks, handlers Web Forms, `GridView`, `UpdatePanel`, `ModalPopupExtender` ni ejecutores de otras operaciones.
- - No modificar el guard genérico si su contrato actual usa token; crear o parametrizar un guard exclusivo que preserve exclusión por tarea sin alterar otras capacidades.
- - Conector inexistente, ajeno, retirado, de contexto distinto, token vencido, permiso retirado, Ruta/Flujo inconsistente o lock ocupado bloquean antes del motor.
- - ejecutar E2E autenticada,  tarea real con autorización explícita de ambiente y cuentas de prueba.
- 
- ## REGLAS DE ANTIRREGRESIÓN
- 
- - El preview no revela conectores, actividades, usuarios ni grupos fuera del universo autorizado y no escribe tarea, estado, auditoría, eventos ni datos de negocio.
- - Existe un único punto mutante de devolución; el navegador nunca autoriza el destino ni el contexto de la arista.
- - Una falla de auditoría no revierte una transición confirmada; solicitudes concurrentes, incluso con tokens distintos, no devuelven dos veces la tarea.
- - Continuar flujo conserva conector saliente, endpoints, payload, validaciones, adaptador y pruebas actuales.
- 
- ## CRITERIOS DE ACEPTACIÓN
- 
- - Ruta y Flujo solo devuelven aristas entrantes válidas del contexto actual, con semánticas aisladas detrás del mismo endpoint.
- - Cursor, término, conector o identificador manipulados devuelven resultado público seguro sin fuga ni escritura.
- - Errores y bloqueos públicos no exponen SQL, sesión, controles Web Forms, credenciales ni excepciones internas.
- 
- ## PRUEBAS OBLIGATORIAS
- 
- Agregar y ejecutar pruebas focales para permiso específico, preview sin escritura, Ruta con filtro de Ruta/arista, Flujo, filtro parametrizado, término mínimo, límite, orden, cursor de otro contexto, lista sintética extensa, token vencido, conector manipulado/retirado/de contexto distinto, lock por tarea con tokens distintos, concurrencia, política de eventos/notificación aprobada, auditoría fallida, éxito simulado y ausencia de invocaciones a métodos de respuestas. Ejecutar MSBuild disponible y pruebas afectadas; registrar comando, resultado, cobertura y limitaciones.  E2E obligatorio.
- 
- ## DOCUMENTACIÓN TÉCNICA
- 
- Actualizar `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md` y `04-pruebas-y-evidencia.md` bajo `../01-implementacion-devolver-actividad-anterior/` con identidad contextual de arista, cursor, punto mutante, lock, parámetros aprobados, auditoría, estados/error y relevo a 02.
- 
- ## ENTREGABLE FINAL
- 
- Reportar ticket, cambios backend, archivos, pruebas, compilación, evidencia de no escritura, trazabilidad sanitizada, decisiones aplicadas y confirmación de no regresión. No implementar UI ni cambiar configuración de ambiente.

### Requirement: RQ-01 Contrato exclusivo de devolución (D-01)
El sistema SHALL exponer contratos, DTOs, puertos y endpoints exclusivos para Devolver a actividad anterior, sin reutilizar los contratos de conectores salientes, envío a usuario, envío a grupo ni Usuario anterior.

#### Scenario: Transporte limitado
- **WHEN** se invocan `PreviewDevolverActividad` o `EjecutarDevolverActividad`
- **THEN** los contratos no exponen `Page`, `Session`, HTML, SQL, credenciales ni excepciones internas.

### Requirement: RQ-02 Autorización e identidad contextual (D-02)
El sistema SHALL calcular el permiso de devolución, tipo Ruta/Flujo e identidad del conector desde la tarea y el contexto autenticado, con rechazo fail-closed.

#### Scenario: Cliente manipulado
- **WHEN** el cliente publica una actividad, usuario, grupo, Ruta, Flujo o destino que no corresponde a la tarea
- **THEN** el servidor lo ignora, reconstruye el contexto propio y devuelve un bloqueo funcional sin invocar el motor.

### Requirement: RQ-03 Preview autorizado y paginado (D-03)
El sistema SHALL resolver solo aristas entrantes autorizadas mediante SELECT parametrizados, filtro mínimo, límite máximo, orden estable y cursor opaco ligado al contexto de preview.

#### Scenario: Cursor de otro contexto
- **WHEN** el cursor pertenece a otra tarea, usuario, tipo de contexto, término u orden
- **THEN** el preview devuelve un código funcional seguro y no escribe tarea, estado, auditoría, eventos ni datos de negocio.

### Requirement: RQ-04 Ejecución exclusiva y revalidada (D-04)
El sistema SHALL adquirir un lock por `IdTarea`, independiente del token, y revalidar todos los datos de servidor dentro del lock antes de mutar.

#### Scenario: Dos solicitudes concurrentes
- **WHEN** dos solicitudes usan la misma tarea con tokens diferentes
- **THEN** como máximo una alcanza el adaptador y la otra recibe un bloqueo funcional estable.

### Requirement: RQ-05 Adaptador legacy de devolución (D-05)
El sistema SHALL invocar `Terminar_Tarea_Workflow` una sola vez desde un adaptador exclusivo, con `Page = Nothing`, actualización de interfaz y reasignaciones desactivadas, y con eventos dinámicos y notificación de asignación conservados.

#### Scenario: Ruta y Flujo válidos
- **WHEN** la resolución final identifica un conector entrante válido de Ruta o Flujo
- **THEN** el adaptador recibe únicamente el destino reconstruido y preserva el perfil aprobado de eventos y notificación.

### Requirement: RQ-06 Resultado, auditoría y aislamiento de respuestas (D-06)
El sistema SHALL normalizar éxito, bloqueo, error reintentable y advertencias, registrar auditoría sanitizada con mecanismo `ASMX_DEVOLVER_ACTIVIDAD` y mantener fuera de los componentes nuevos cualquier tratamiento de respuestas.

#### Scenario: Auditoría posterior fallida
- **WHEN** el motor confirma la transición y la auditoría adicional falla
- **THEN** la transición no se revierte y la respuesta incluye una advertencia sin detalle técnico.

### Requirement: RQ-07 Compatibilidad y evidencia local (D-07)
El sistema SHALL preservar los endpoints, guard y recorridos de las capacidades existentes y SHALL documentar y verificar el aislamiento de DOC-32 sin activar UI, feature flags ni configuración de ambiente.

#### Scenario: Verificación de regresión
- **WHEN** se ejecutan las pruebas focales y la compilación disponible
- **THEN** se comprueba que preview no escribe, que no hay referencias nuevas a componentes de respuestas y que las capacidades existentes no cambiaron.

### Requirement: RQ-08 E2E real, concurrencia y rendimiento protegido (D-08)
El sistema SHALL proporcionar una suite E2E DOC-32 que reutilice el helper de sesión autenticada existente, realice controles MySQL exclusivamente con SELECT parametrizados, ejecute una transición real autorizada sobre una tarea descartable, y pruebe una carrera fija de dos solicitudes sobre otra tarea descartable.

#### Scenario: Evidencia de ejecución autorizada
- **WHEN** el ambiente, la cuenta Workflow, las tareas descartables, los controles de estado y auditoría de solo lectura, y los presupuestos de latencia están autorizados y configurados
- **THEN** la suite obtiene el destino y token del preview vigente, registra evidencia saneada de estado, auditoría, resultado, concurrencia y latencias, y mantiene sin cambios el gate y los archivos legacy.
