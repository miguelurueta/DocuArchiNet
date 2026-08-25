<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05 -->
## ADDED Requirements

### Requirement: RQ-01 — Verificación transversal no mutante

**Decisión de origen:** D-01.

La verificación DOC-34 SHALL obtener evidencia mediante compilación local, pruebas focales, análisis estático y QA manual no autenticada. SHALL NOT modificar código de producción, configuración, contratos, estado de tareas, datos o auditoría, ni ejecutar E2E autenticada, carga, despliegue, archivo, publicación o liberación automática.

#### Scenario: Controles locales sin alteración funcional

- **WHEN** se ejecutan los controles definidos para DOC-34
- **THEN** los resultados se limitan a evidencia local y documentación saneada
- **AND** no se solicitan ni se registran secretos del ambiente

### Requirement: RQ-02 — Evidencia del preview de devolución

**Decisión de origen:** D-02.

La verificación DOC-34 SHALL demostrar que `PreviewDevolverActividad` es una lectura sobre el universo autorizado y sobre conectores entrantes de Ruta o Flujo. SHALL comprobar la semántica aislada de `IdConector`, filtro, orden, cursor, límite y ausencia de cambios de tarea, estado o auditoría.

#### Scenario: Conector no perteneciente al historial entrante

- **WHEN** se revisa un conector saliente o no autorizado frente al contrato y las pruebas focales
- **THEN** no se acepta como destino de devolución
- **AND** la evidencia identifica que la fuente válida es el conector entrante de Ruta o Flujo

#### Scenario: Preview paginado y de solo lectura

- **WHEN** se revisan el servicio, repositorio y pruebas del preview
- **THEN** se conserva el filtro autorizado con orden, cursor y límite deterministas
- **AND** el preview no persiste estado ni auditoría

### Requirement: RQ-03 — Evidencia de ejecución segura y concurrente

**Decisión de origen:** D-03.

La verificación DOC-34 SHALL demostrar que `EjecutarDevolverActividad` requiere permiso, token vigente y conector entrante revalidado bajo un lock exclusivo por tarea. SHALL revisar auditoría, notificaciones/eventos aprobados y la evidencia de una única transición efectiva en concurrencia.

#### Scenario: Token o conector manipulado

- **WHEN** se revisan las validaciones de ejecución y sus pruebas focales
- **THEN** el contrato rechaza un token vencido, un permiso ausente o un conector no autorizado
- **AND** no se habilita una llamada nueva a los métodos de respuesta heredados

#### Scenario: Solicitudes simultáneas

- **WHEN** se revisan el lock y la evidencia de concurrencia previa de DOC-32
- **THEN** solo una transición puede ser efectiva para la misma tarea
- **AND** DOC-34 no repite una operación autenticada para obtener ese resultado

### Requirement: RQ-04 — Evidencia de UI moderna aislada y bloqueada

**Decisión de origen:** D-04.

La verificación DOC-34 SHALL demostrar que la UI de devolución consume el preview vigente, ejecuta solo el destino seleccionado y bloquea confirmación, cancelación y cierre mientras existe una respuesta pendiente. SHALL NOT depender de `WorkflowCentroTrabajoModernActive` ni dejar postback, handler o fallback Web Forms alcanzable para devolver actividad.

#### Scenario: Respuesta pendiente

- **WHEN** la confirmación de devolución espera respuesta del backend simulado en las pruebas locales
- **THEN** los controles de confirmación, cancelación y cierre permanecen bloqueados
- **AND** se conserva información accesible del estado de espera

#### Scenario: Aislamiento de UI y Web Forms

- **WHEN** se revisan scripts, marcado, código detrás y pruebas de política
- **THEN** no existe evaluación del feature gate para esta operación
- **AND** no existe una ruta heredada alcanzable para ejecutar devolución

### Requirement: RQ-05 — No regresión y recomendación reproducible

**Decisión de origen:** D-05.

La verificación DOC-34 SHALL comparar los contratos y pruebas de Continuar flujo, Enviar a usuario, Enviar a grupo y Usuario anterior. SHALL actualizar la documentación de `DebolverTarea` con matriz de resultados, cobertura, límites, riesgos y correlaciones saneadas, y SHALL emitir exactamente una recomendación para fase 04: apto, bloqueado o requiere corrección.

#### Scenario: Resultado completo de la verificación

- **WHEN** finalizan compilación, pruebas, análisis y QA manual no autenticada
- **THEN** cada escenario crítico queda aprobado o asociado a una corrección reproducible
- **AND** la documentación registra una recomendación inequívoca sin información sensible
