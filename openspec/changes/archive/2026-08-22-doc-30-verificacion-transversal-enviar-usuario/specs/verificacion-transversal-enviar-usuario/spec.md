<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04 -->
## ADDED Requirements

### Requirement: Verificación no mutante y reproducible

DOC-30 SHALL ejecutar la verificación sobre el snapshot versionado sin modificar código de producción, configuración, tareas, auditoría, datos ni contratos.

#### Scenario: Evidencia local disponible

- **WHEN** se valida la entrega DOC-28/DOC-29
- **THEN** se registran comandos, resultado, cobertura y límites de la compilación, pruebas CJS, inspección estática y QA visual no autenticada.

#### Scenario: Operación fuera del alcance

- **WHEN** una comprobación requiere E2E autenticado, carga, activación de gate o despliegue
- **THEN** DOC-30 no la ejecuta ni infiere autorización para realizarla.

Trazabilidad: D-01, RQ-01.

### Requirement: Contrato y seguridad del envío directo

DOC-30 SHALL confirmar que la evidencia del snapshot conserva preview de solo lectura y ejecución por `IdTarea`, usuario destino, actividad destino y token de versión.

#### Scenario: Preview y ejecución

- **WHEN** se inspeccionan `PreviewEnviarUsuario` y `EjecutarEnvioUsuario`
- **THEN** el preview no muta estado ni auditoría, y la ejecución revalida autorización, tarea, destino, respuesta, token y lock en servidor.

#### Scenario: Respuesta o destino inválido

- **WHEN** la respuesta exige confirmación/radicado o el destino deja de ser válido
- **THEN** se bloquea de forma funcional, sin reasignar la respuesta ni revelar detalles internos.

Trazabilidad: D-02, RQ-02.

### Requirement: Experiencia moderna aislada y accesible

DOC-30 SHALL verificar que Enviar a usuario conserva su ruta moderna oficial y que la lista de destinos mantiene sus garantías de búsqueda, foco y cierre seguro.

#### Scenario: Búsqueda y selección

- **WHEN** se revisan búsqueda, límite, orden, cursor y respuestas obsoletas
- **THEN** solo se muestra el universo autorizado, la selección vencida se invalida y el modal conserva geometría, foco y comportamiento de teclado.

#### Scenario: Integración con otros comandos

- **WHEN** se compara el envío a usuario con Grupo y Continuar flujo
- **THEN** usuario no usa `IdConector`, postback legacy ni listeners de transición, y los contratos existentes de los otros comandos permanecen intactos.

Trazabilidad: D-03, RQ-03.

### Requirement: Dictamen técnico previo a liberación

DOC-30 SHALL consolidar una matriz sanitizada de resultados, riesgos y correcciones, con un único dictamen para la etapa operativa posterior.

#### Scenario: Todos los controles aprobados

- **WHEN** compilación, pruebas, inspección y QA cumplen los criterios
- **THEN** el dictamen es `apto para solicitar aprobación operativa`, sin iniciar despliegue ni cambios de ambiente.

#### Scenario: Control no aprobado

- **WHEN** un escenario crítico falla o carece de evidencia reproducible
- **THEN** el dictamen es `bloqueado` o `requiere corrección`, y se registra el ticket correctivo antes de cualquier liberación.

Trazabilidad: D-04, RQ-04.
