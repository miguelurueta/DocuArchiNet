# verificacion-transversal Specification

## Purpose

Consolidar evidencia reproducible y una decisión de aptitud para la liberación controlada de Devolver → Usuario anterior, sin introducir una ruta funcional ni modificar el ambiente.

## Requirements

### Requirement: Verificación sin mutación

Trazabilidad: D-01.

La verificación transversal SHALL obtener evidencia sin modificar código de producción, configuración, contratos, datos de negocio, estado de tareas ni auditoría.

#### Scenario: Control local sin efectos de negocio

- **WHEN** DOC-38 ejecuta sus controles autorizados
- **THEN** registra solo evidencia reproducible y no realiza una transición, despliegue, liberación automática ni operación autenticada sobre una tarea real

### Requirement: Matriz de evidencia reproducible

Trazabilidad: D-02.

La verificación transversal SHALL registrar comando, resultado, cobertura y limitaciones de compilación, pruebas CJS/VB, análisis estático y QA manual no autenticada.

#### Scenario: Exclusiones explícitas

- **WHEN** E2E autenticada, carga o despliegue no están autorizados
- **THEN** el informe los declara excluidos y no los reemplaza con una afirmación de cobertura inexistente

### Requirement: Contrato seguro de devolución

Trazabilidad: D-03.

La verificación transversal SHALL demostrar que el preview es solo lectura y que historial, token y lock conservan la operación determinista y exclusiva aprobada.

#### Scenario: Historial o token cambia antes de ejecutar

- **WHEN** el antecedente confirmado, el token o la versión de tarea ya no coinciden
- **THEN** la ejecución se bloquea dentro del lock sin elegir otro antecedente ni producir una segunda transición

### Requirement: Aislamiento del motor y auditoría

Trazabilidad: D-04.

La verificación transversal SHALL confirmar que la devolución usa el adaptador exclusivo, no trata respuestas y mantiene auditoría funcional saneada.

#### Scenario: Revisión del límite legacy

- **WHEN** se analiza la ruta mutante y sus pruebas focales
- **THEN** no aparecen componentes de respuestas, notificaciones ni eventos dinámicos no aprobados en la capacidad nueva

### Requirement: Interfaz exclusiva y accesible

Trazabilidad: D-05.

La verificación transversal SHALL confirmar que la UI de usuario anterior no depende del feature gate ni mantiene postback o fallback a actividad anterior.

#### Scenario: Confirmación y bloqueo desde la bandeja

- **WHEN** el usuario confirma, cancela o encuentra un preview bloqueado
- **THEN** la interfaz conserva la ruta exclusiva, evita duplicados durante espera y restablece la bandeja sin invocar la devolución a actividad anterior

### Requirement: No regresión de operaciones vecinas

Trazabilidad: D-06.

La verificación transversal SHALL ejecutar o analizar controles focales de devolución a actividad anterior, continuar flujo, enviar a usuario y enviar a grupo junto con la capacidad nueva.

#### Scenario: Comparación de contratos

- **WHEN** se revisan los contratos y suites de Workflow afectadas
- **THEN** ninguna operación vecina acepta datos de destino de usuario anterior ni cambia su comportamiento

### Requirement: Recomendación de liberación verificable

Trazabilidad: D-07.

La verificación transversal SHALL producir una recomendación “apto para 05”, “bloqueado” o “requiere corrección” sustentada en la matriz de evidencia.

#### Scenario: Hallazgo crítico

- **WHEN** falla un control de contrato, concurrencia, aislamiento o no regresión
- **THEN** el informe adjunta evidencia reproducible y bloquea la recomendación hasta que exista corrección validada
