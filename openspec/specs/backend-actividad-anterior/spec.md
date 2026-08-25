# backend-actividad-anterior Specification

## Purpose

Permitir devolver de manera segura una tarea Workflow a una actividad anterior autorizada. La capacidad ofrece preview de solo lectura y ejecución revalidada, preservando la compatibilidad de las transiciones existentes y evitando efectos duplicados bajo concurrencia.

## Requirements

### Requirement: BACKEND-ACTIVIDAD-ANTERIOR
El sistema SHALL implementar la capacidad DOC-32 de devolución segura a actividad anterior como un corte de servidor aislado.

#### Scenario: Flujo principal
- **WHEN** un usuario autorizado solicita el preview y ejecuta una devolución vigente
- **THEN** el sistema devuelve únicamente destinos válidos y confirma a lo sumo una transición hacia el destino reconstruido.

#### Scenario: No-regresión
- **WHEN** se valida el módulo Workflow afectado
- **THEN** las capacidades existentes de envío y sus contratos permanecen compatibles.

### Requirement: Detalle funcional Jira
El sistema SHALL cumplir las restricciones funcionales, de seguridad y de compatibilidad de DOC-32, concretadas en los requisitos RQ-01 a RQ-08 de esta especificación.

#### Scenario: Reglas del ticket
- **WHEN** se evalúa el alcance DOC-32
- **THEN** se comprueba contrato exclusivo, autorización de servidor, preview sin escritura, revalidación con lock, adaptador aislado, auditoría saneada, compatibilidad y E2E protegida.

### Requirement: RQ-01 Contrato exclusivo de devolución (D-01)
El sistema SHALL exponer contratos, DTOs, puertos y endpoints exclusivos para devolver a actividad anterior, sin reutilizar contratos de conectores salientes, envío a usuario, envío a grupo ni Usuario anterior.

#### Scenario: Transporte limitado
- **WHEN** se invocan `PreviewDevolverActividad` o `EjecutarDevolverActividad`
- **THEN** los contratos no exponen `Page`, `Session`, HTML, SQL, credenciales ni excepciones internas.

### Requirement: RQ-02 Autorización e identidad contextual (D-02)
El sistema SHALL calcular el permiso de devolución, tipo Ruta/Flujo e identidad del conector desde la tarea y el contexto autenticado, con rechazo fail-closed.

#### Scenario: Cliente manipulado
- **WHEN** el cliente publica una actividad, usuario, grupo, Ruta, Flujo o destino que no corresponde a la tarea
- **THEN** el servidor lo ignora, reconstruye el contexto propio y devuelve un bloqueo funcional sin invocar el motor.

### Requirement: RQ-03 Preview autorizado y paginado (D-03)
El sistema SHALL resolver solo aristas entrantes autorizadas mediante `SELECT` parametrizados, filtro mínimo, límite máximo, orden estable y cursor opaco ligado al contexto de preview.

#### Scenario: Cursor de otro contexto
- **WHEN** el cursor pertenece a otra tarea, usuario, tipo de contexto, término u orden
- **THEN** el preview devuelve un código funcional seguro y no escribe tarea, estado, auditoría, eventos ni datos de negocio.

### Requirement: RQ-04 Ejecución exclusiva y revalidada (D-04)
El sistema SHALL adquirir un lock por `IdTarea`, independiente del token, y revalidar los datos de servidor dentro del lock antes de mutar.

#### Scenario: Dos solicitudes concurrentes
- **WHEN** dos solicitudes usan la misma tarea con tokens diferentes
- **THEN** como máximo una alcanza el adaptador y la otra recibe un bloqueo funcional estable.

### Requirement: RQ-05 Adaptador legacy de devolución (D-05)
El sistema SHALL invocar `Terminar_Tarea_Workflow` una sola vez desde un adaptador exclusivo, con `Page = Nothing`, actualización de interfaz y reasignaciones desactivadas, y con eventos dinámicos y notificación de asignación conservados.

#### Scenario: Ruta y Flujo válidos
- **WHEN** la resolución final identifica un conector entrante válido de Ruta o Flujo
- **THEN** el adaptador recibe únicamente el destino reconstruido y preserva el perfil aprobado de eventos y notificación.

### Requirement: RQ-06 Resultado, auditoría y aislamiento de respuestas (D-06)
El sistema SHALL normalizar éxito, bloqueo, error reintentable y advertencias, registrar auditoría saneada con mecanismo `ASMX_DEVOLVER_ACTIVIDAD` y mantener fuera de los componentes nuevos cualquier tratamiento de respuestas.

#### Scenario: Auditoría posterior fallida
- **WHEN** el motor confirma la transición y la auditoría adicional falla
- **THEN** la transición no se revierte y la respuesta incluye una advertencia sin detalle técnico.

### Requirement: RQ-07 Compatibilidad y evidencia local (D-07)
El sistema SHALL preservar endpoints, guard y recorridos de capacidades existentes, y SHALL documentar y verificar el aislamiento de DOC-32 sin activar UI, feature flags ni configuración de ambiente.

#### Scenario: Verificación de regresión
- **WHEN** se ejecutan las pruebas focales y la compilación disponible
- **THEN** se comprueba que preview no escribe, que no hay referencias nuevas a componentes de respuestas y que las capacidades existentes no cambiaron.

### Requirement: RQ-08 E2E real, concurrencia y rendimiento protegido (D-08)
El sistema SHALL proporcionar una suite E2E DOC-32 que reutilice el helper de sesión autenticada existente, realice controles MySQL exclusivamente con `SELECT` parametrizados, ejecute una transición real autorizada sobre una tarea descartable y pruebe una carrera fija de dos solicitudes sobre otra tarea descartable.

#### Scenario: Evidencia de ejecución autorizada
- **WHEN** el ambiente, la cuenta Workflow, las tareas descartables, los controles de estado y auditoría de solo lectura y los presupuestos de latencia están autorizados y configurados
- **THEN** la suite obtiene destino y token del preview vigente, registra evidencia saneada de estado, auditoría, resultado, concurrencia y latencias, y mantiene sin cambios el gate y los archivos legacy.
