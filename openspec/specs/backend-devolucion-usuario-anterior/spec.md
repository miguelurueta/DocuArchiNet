# backend-devolucion-usuario-anterior Specification

## Purpose
TBD - created by archiving change doc-36-backend-devolucion-usuario-anterior. Update Purpose after archive.
## Requirements
### Requirement: BACKEND-DEVOLUCION-USUARIO-ANTERIOR
El sistema SHALL implementar el alcance definido para DOC-36.
#### Scenario: Flujo principal
- **WHEN** se ejecuta el caso de uso principal del ticket
- **THEN** el comportamiento coincide con las reglas funcionales esperadas
#### Scenario: No-regresion
- **WHEN** se valida el modulo afectado
- **THEN** no se rompen flujos existentes

### Requirement: Detalle funcional Jira
El sistema SHALL considerar las reglas detalladas del ticket.

#### Scenario: Reglas del ticket
- # 02 — Backend seguro de devolución a usuario anterior
- 
- ## ROL ESPERADO
- 
- Actúa como desarrollador senior VB.NET de capas Domain, Application e Infrastructure para Workflow, con experiencia en concurrencia MySQL y encapsulación de motores existentes.
- 
- ## OBJETIVO
- 
- Implementar el corte completo de servidor de **Devolver a usuario anterior**: contrato exclusivo, autorización, `PreviewDevolverUsuarioAnterior` de solo lectura, `EjecutarDevolverUsuarioAnterior`, lock por tarea, adaptador, auditoría y pruebas focales. Preview y ejecución se implementan juntos porque comparten el mismo historial, token y límite de seguridad.
- 
- ## CONTEXTO OBLIGATORIO
- 
- - Requiere 01 aprobado, con decisiones explícitas sobre historial, token, lock, parámetros de `Terminar_Tarea_Workflow`, notificación y eventos dinámicos.
- - Leer `00-contexto-obligatorio.md`, los documentos de `../Exploracion/` y la evidencia de 01.
- - La salida habilita 03; no implementa UI, activación ni liberación.
- 
- ## REQUISITOS POSITIVOS
- 
- - Crear DTOs, modelos, puertos, códigos públicos y servicio exclusivos de devolución a usuario anterior; no reutilizar contratos de actividades, conectores ni envíos.
- - Exponer en el ASMX moderno existente `PreviewDevolverUsuarioAnterior(idTarea)` y `EjecutarDevolverUsuarioAnterior({ IdTarea, TokenVersion })`, con sesión habilitada y contexto autenticado revalidado en servidor.
- - Calcular el permiso de devolución específico en servidor, fail-closed; no reutilizar `PuedeCambioRuta` ni aceptar permisos, destino, Ruta, Flujo, actividad, grupo o historial desde el navegador.
- - Ejecutar el preview exclusivamente con `SELECT` parametrizados. Resolver cero o un registro histórico mediante el algoritmo aprobado en 01, validar tarea activa y accesible, usuario elegible, actividad y Ruta/Flujo consistentes, y devolver datos mínimos para confirmación junto con token opaco.
- - Emitir un token que vincule versión de tarea e identificador del registro histórico confirmado. El endpoint de ejecución recibe solo ese token, no un identificador histórico adicional.
- - Adquirir un lock exclusivo por `IdTarea`, independiente del token. Dentro del lock releer y validar contexto, permiso, tarea, token, registro histórico, usuario, actividad, Ruta/Flujo y auto-devolución frente al usuario Workflow autenticado.
- - Crear puerto y adaptador exclusivos. Solo ese adaptador invoca una vez `Terminar_Tarea_Workflow` con `Page = Nothing`, actualización de interfaz legacy desactivada y los parámetros de notificación/eventos aprobados en 01.
- - La política aprobada de notificación no permite que el adaptador ni los componentes nuevos construyan o invoquen componentes de respuestas. Si el motor legacy los construye internamente, los parámetros aprobados deben impedir llamar sus métodos y una prueba focal debe demostrarlo. Normalizar éxito, bloqueo, error reintentable y advertencias; auditar con `ASMX_DEVOLVER_USUARIO_ANTERIOR`, sin datos sensibles.
- 
- ## RESTRICCIONES CRÍTICAS
- 
- - No implementar UI, feature flags, directorio global, búsqueda, paginación, selector de destinos ni cambios de configuración.
- - No modificar contratos o comportamiento de Devolver a actividad anterior, Continuar flujo, Enviar a usuario, Enviar a grupo, `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `ServicioTransicionTarea` ni `IdConector`.
- - No invocar el método legacy `Devolver_tarea_workflow_usuario_anterior`, postbacks, handlers Web Forms, `GridView`, `UpdatePanel`, `ModalPopupExtender` ni ejecutores de otras operaciones.
- - Los componentes nuevos no referencian `Classgestionrespuesta`, `Verifica_respuesta_*`, `Reasigna_respuesta_envia_tarea_usuario` ni componentes de respuestas.
- - No modificar el guard genérico de transiciones si su contrato actual usa token; crear o parametrizar un guard exclusivo que preserve la exclusión por tarea sin alterar las demás operaciones.
- - Historial ausente, de grupo, no elegible, distinto del confirmado, auto-devolución, token vencido, permiso retirado o lock ocupado bloquean antes del motor.
- - No ejecutar E2E autenticada, carga ni una tarea real sin autorización explícita de ambiente y cuentas de prueba.
- 
- ## REGLAS DE ANTIRREGIÓN
- 
- - El preview no revela registros de otras tareas ni destinos alternativos y no escribe tarea, estado, auditoría, eventos ni datos de negocio.
- - Existe un único punto mutante directo y el navegador nunca autoriza ni el destino ni el registro histórico.
- - Una falla de auditoría no revierte una transición ya confirmada; solicitudes concurrentes, incluso con tokens distintos, no devuelven dos veces la tarea.
- - Las demás operaciones preservan endpoints, payloads, adaptadores, feature gates y pruebas existentes.
- 
- ## CRITERIOS DE ACEPTACIÓN
- 
- - El preview devuelve cero o un usuario histórico elegible, nunca grupo, actividad alternativa, conector ni datos de respuestas.
- - Si el historial cambia desde el preview, la ejecución bloquea; nunca devuelve a un destino distinto del confirmado.
- - La auto-devolución compara el usuario histórico con el usuario Workflow autenticado real, nunca con `Id_Ruta_Workflow`.
- - Errores y bloqueos públicos no exponen SQL, sesión, controles Web Forms, credenciales ni excepciones internas.
- 
- ## PRUEBAS OBLIGATORIAS
- 
- Agregar y ejecutar pruebas focales para permiso específico, preview sin escritura, orden/desempate de historial, historial válido/ausente/de grupo, usuario retirado, Ruta/Flujo inconsistente, auto-devolución, SQL parametrizado, token vencido, token con historial cambiado, lock por tarea con tokens distintos, concurrencia, advertencia aprobada, auditoría fallida, éxito simulado y ausencia de referencias a componentes de respuestas. Ejecutar MSBuild disponible y pruebas afectadas; registrar comando, resultado, cobertura y limitaciones. No E2E.
- 
- ## DOCUMENTACIÓN TÉCNICA
- 
- Actualizar `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md` y `04-pruebas-y-evidencia.md` bajo `../01-implementacion-devolver-usuario-anterior/` con algoritmo de historial, token, lock, punto mutante, parámetros aprobados, auditoría, estados/error y relevo a 03.
- 
- ## ENTREGABLE FINAL
- 
- Reportar ticket, cambios backend, archivos, pruebas, compilación, evidencia de no escritura, trazabilidad sanitizada, decisiones aplicadas y confirmación de no regresión. No implementar UI ni cambiar configuración de ambiente.

### Requirement: Preview de usuario histórico inmediato

El sistema SHALL exponer un preview autenticado y de solo lectura que resuelva como máximo un usuario anterior de la misma tarea mediante el historial determinista aprobado.

#### Scenario: Historial válido

- **WHEN** una tarea activa accesible tiene dos snapshots consistentes y el segundo corresponde a un usuario elegible distinto del autenticado
- **THEN** el preview devuelve actividad y usuario mínimos junto con un token opaco
- **AND** no escribe estado, tarea, auditoría, evento ni datos de negocio.

#### Scenario: Historial no elegible

- **WHEN** falta el antecedente o este representa grupo, usuario retirado, inconsistencia Ruta/Flujo o el usuario autenticado
- **THEN** el preview devuelve un bloqueo público específico
- **AND** no devuelve alternativas ni abre la capacidad de actividad anterior.

#### Scenario: Búsqueda automática del usuario anterior

- **WHEN** existen snapshots grupales entre la tarea actual y un snapshot histórico de usuario de la misma tarea
- **THEN** el servidor ignora los snapshots grupales y selecciona el usuario real más reciente anterior al estado actual
- **AND** el cliente no proporciona ni selecciona el identificador del usuario.

#### Scenario: Usuario de flujo histórico diferenciado

- **WHEN** el antecedente válido contiene un `ID_USUARIO_WORKFLOW_FLUJO_TRABAJO` positivo distinto de `Id_Usuario`
- **THEN** el preview conserva ese usuario de flujo para la ejecución revalidada
- **AND** solo sustituye ese valor con el usuario destino cuando el campo histórico es cero.

### Requirement: Token que compromete el historial

El sistema SHALL proteger el token de usuario anterior con tarea, estado actual, estado histórico y vencimiento de cinco minutos.

#### Scenario: Historial cambia después del preview

- **WHEN** la ejecución recibe token válido pero el estado actual o antecedente ya no coincide
- **THEN** bloquea con conflicto de versión
- **AND** nunca resuelve una fila histórica distinta.

### Requirement: Ejecución exclusiva por tarea

El sistema SHALL serializar la ejecución con un lock exclusivo por `IdTarea`, independiente del token.

#### Scenario: Intentos concurrentes

- **WHEN** dos solicitudes para la misma tarea compiten, incluso con tokens distintos
- **THEN** una sola puede adquirir el lock y alcanzar el motor
- **AND** la otra recibe bloqueo en progreso sin mutación.

### Requirement: Punto mutante sin UI, correo, eventos ni respuestas

El sistema SHALL ejecutar la transición mediante un adaptador exclusivo que llama una vez a `Terminar_Tarea_Workflow` con `Page = Nothing`, notificación, interfaz legacy y eventos dinámicos desactivados.

#### Scenario: Ejecución revalidada

- **WHEN** contexto, permiso, token, historial, usuario y Ruta/Flujo pasan la revalidación dentro del lock
- **THEN** el adaptador usa solo valores reconstruidos del servidor y ejecuta una transición
- **AND** los componentes nuevos no referencian tratamientos de respuestas.

### Requirement: Auditoría saneada posterior

El sistema SHALL registrar el resultado bajo la acción `ASMX_DEVOLVER_USUARIO_ANTERIOR` sin secretos ni token.

#### Scenario: Falla de auditoría posterior al éxito

- **WHEN** el motor confirma la transición y la auditoría adicional falla
- **THEN** el resultado conserva éxito y agrega una advertencia saneada.

