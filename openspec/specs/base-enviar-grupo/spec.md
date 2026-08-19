# base-enviar-grupo Specification

## Purpose

Define el envío directo y seguro de una tarea hacia una actividad destino de grupo, con preview de solo lectura y fallback legacy controlado.

## Requirements

### Requirement: RQ-01 — Operación directa, autenticada y cerrada por defecto

El sistema SHALL exponer `PreviewEnviarGrupo(idTarea)` y `EjecutarEnvioGrupo(idTarea, idActividadDestino, tokenVersion)` en `WebServiceWorkflowModern`. Ambas operaciones SHALL reconstruir el contexto desde la sesión autenticada y reevaluar `IWorkflowModernFeatureGate`. `IdConector` no forma parte de sus contratos ni se modifican los endpoints existentes por conector.

#### Scenario: Gate, sesión o permiso no autorizan

- **WHEN** el gate está inactivo, la sesión no es válida o el usuario no tiene `Cambio_Ruta`
- **THEN** se devuelve un código y mensaje funcional seguro
- **AND THEN** no se revelan destinos ni se invoca el motor legacy

### Requirement: RQ-02 — Preview exclusivamente de lectura

El sistema SHALL resolver para preview solo las actividades de destino permitidas en la ruta actual y publicar un `TokenVersion`. El preview SHALL usar únicamente consultas de lectura y no deberá modificar tarea, estado, auditoría, eventos ni configuración.

#### Scenario: Ruta, flujo o destino no disponibles

- **WHEN** la tarea no está activa, la ruta está cerrada, el flujo/actividad aplicable está cerrado o no hay actividades válidas
- **THEN** el preview devuelve un bloqueo funcional sin destinos
- **AND THEN** no se ejecuta una llamada mutante

### Requirement: RQ-03 — Ejecución revalidada y sin conector

El sistema SHALL tratar `IdTarea`, `IdActividadDestino` y `TokenVersion` como intención no confiable. Dentro de un guard compartido SHALL releer tarea y versión, verificar `Cambio_Ruta`, ruta/flujo/actividad y resolver de nuevo la actividad destino de la ruta. SHALL delegar una única vez al adaptador de envío directo con conector e identificadores de flujo en cero.

#### Scenario: Token vencido, destino retirado o solicitud concurrente

- **WHEN** la tarea cambia, el destino deja de pertenecer a la ruta o dos solicitudes usan la misma tarea y versión
- **THEN** como máximo una solicitud llega a `Terminar_Tarea_Workflow`
- **AND THEN** las demás devuelven conflicto, destino no disponible o envío en progreso sin cambiar estado

#### Scenario: Envío directo exitoso

- **WHEN** la tarea, actividad destino y requisitos de grupo son válidos
- **THEN** el adaptador invoca el motor legacy sin `Page`, sin conector y sin actualización de controles Web Forms
- **AND THEN** el resultado declara éxito solo cuando el motor confirma la transición

### Requirement: RQ-04 — Requisitos propios y preservación del motor legacy

El sistema SHALL bloquear solicitudes de aprobación pendientes antes de terminar la tarea. La operación SHALL conservar la semántica actual de grupo y no añadirá validación de respuesta radicada, reasignación de respuesta, `Cambia_Estado` directo ni una transacción alternativa.

#### Scenario: Aprobación pendiente

- **WHEN** existe una solicitud de aprobación pendiente
- **THEN** la operación devuelve bloqueo funcional sin invocar el adaptador
- **AND THEN** no se reasigna ni modifica una respuesta

### Requirement: RQ-05 — Presentación progresiva y fallback legacy

El sistema SHALL enlazar la experiencia moderna de grupo solo mediante el bootstrap existente. Con el gate inactivo SHALL conservar exactamente el postback y modal Web Forms de Enviar a grupo. Continuar flujo SHALL conservar `PreviewEnviarTarea`, `EjecutarEnvioTarea`, `IdConector`, sus destinos y su adaptador actual.

#### Scenario: Gate inactivo

- **WHEN** el gate existente no está activo para la solicitud
- **THEN** la página no registra la interacción ASMX moderna de grupo
- **AND THEN** el usuario continúa por el camino legacy sin una llamada ASMX de fallback

### Requirement: RQ-06 — Resultado público y auditoría sanitizada

El sistema SHALL normalizar resultados de éxito, bloqueo y error sin exponer SQL, Session, token, credenciales, documentos ni excepciones. SHALL registrar auditoría adicional con `Canal=MODERNO`, `Mecanismo=ASMX_ENVIO_GRUPO`, tarea, ruta, origen, actividad destino, resultado, código, duración y conector cero.

#### Scenario: Advertencia posterior a éxito

- **WHEN** correo, evento o auditoría adicional produce una advertencia tras una transición confirmada
- **THEN** el resultado conserva el éxito y agrega una advertencia sanitizada
- **AND THEN** no revierte la transición ya confirmada

### Requirement: RQ-07 — Evidencia y reversa controlada

El sistema SHALL documentar y ejecutar pruebas focales, compilación y QA manual de seguridad, concurrencia, accesibilidad, fallback y no regresión. E2E autenticada, carga o edición del gate SHALL requerir autorización explícita del ambiente y cuentas descartables. El rollback SHALL usar el gate existente sin migrar ni revertir transiciones confirmadas.

#### Scenario: Validación sin autorización de ambiente

- **WHEN** no existe autorización explícita para E2E, carga o activación
- **THEN** se ejecutan solo build, pruebas locales y QA manual autorizados
- **AND THEN** la evidencia registra la limitación y el gate queda en su estado seguro requerido
