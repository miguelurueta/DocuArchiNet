# transacciones-notas Specification

## Purpose

Definir mutaciones modernas de Notas Workflow con autorización de servidor, integridad transaccional, concurrencia, idempotencia y auditoría privada, sin modificar consumidores WebForms ni activar el flujo moderno.

## Requirements

### Requirement: RQ-01 Contratos de escritura y contexto de servidor

El sistema SHALL publicar `CrearNota(idTarea, contenido, idSolicitudCliente)`, `ActualizarNota(idTarea, idNota, contenido, version)` y `EliminarNota(idTarea, idNota, version)` sólo en el ASMX especializado. Actor, grupo, ruta, actividad y fecha SHALL derivarse del contexto autorizado y del snapshot de tarea del servidor; la solicitud no podrá aportar esos atributos ni usar una tarea almacenada en sesión.

#### Scenario: Solicitud con tarea explícita y contexto autorizado

- **WHEN** un actor autorizado crea, actualiza o elimina una nota con `idTarea` válido
- **THEN** el servicio valida tarea, ruta y actividad desde el contexto de servidor
- **AND** no usa una tarea mutable de sesión ni acepta identidad, grupo, ruta o actividad del cliente.

#### Scenario: Contexto ausente o tarea no operativa

- **WHEN** falta permiso, el contexto no coincide con ruta o la tarea no está activa para mutación
- **THEN** el servicio devuelve un código funcional seguro
- **AND** no cambia nota, idempotencia ni auditoría.

### Requirement: RQ-02 Creación idempotente y contenido seguro

El sistema SHALL aceptar creación sólo con texto plano no vacío, sin NUL, de hasta 16.000 unidades UTF-16 y sin caracteres fuera de Unicode BMP. `idSolicitudCliente` SHALL ser UUID y la persistencia SHALL asegurar unicidad por tarea, autor e identificador durante 30 días. Un reintento SHALL devolver respuesta original sin crear otra nota ni otra auditoría.

#### Scenario: Doble envío de la misma creación

- **WHEN** el mismo actor reintenta `CrearNota` con igual tarea e igual `idSolicitudCliente` dentro de 30 días
- **THEN** obtiene identificador, versión y resultado originales
- **AND** sólo existe una nota y una auditoría efectiva para esa solicitud.

#### Scenario: Contenido no compatible con MySQL 5.1

- **WHEN** el contenido es vacío, excede límite, contiene NUL o par sustituto
- **THEN** el servicio devuelve `InvalidContent`
- **AND** no abre mutación ni registra auditoría de éxito.

### Requirement: RQ-03 Actualización y eliminación condicionadas

El sistema SHALL calcular y devolver una versión ETag SHA-256 de valores persistidos canónicos en .NET y conservar la versión vigente en un libro InnoDB distinto de la respuesta original de idempotencia. Actualizar y eliminar SHALL usar condición atómica que incluya nota, tarea, actor propietario, estado aplicable y versión esperada, sin depender de `SHA2()` de MySQL. Eliminar SHALL ser físico y no permitirá recuperación ni lectura posterior del contenido eliminado.

#### Scenario: Conflicto por versión desactualizada

- **WHEN** dos solicitudes intentan actualizar o eliminar con la misma versión inicial
- **THEN** como máximo una mutación confirma
- **AND** la otra recibe `VersionConflict` sin contenido ni versión actual.

#### Scenario: Nota o tarea cruzada

- **WHEN** nota no pertenece a tarea, actor no es propietario o estado deja de permitir mutación
- **THEN** la mutación condicionada no cambia filas
- **AND** el resultado no revela contenido ni existencia fuera del contexto autorizado.

### Requirement: RQ-04 Transacción y auditoría consistente

El sistema SHALL confirmar en una sola transacción InnoDB el cambio de nota, respuesta idempotente cuando aplique y auditoría. La auditoría SHALL registrar metadatos de actor, tarea, actividad, nota, operación, correlación, fecha de servidor, resultado, versión, longitud y SHA-256, y SHALL NOT almacenar texto completo. Cualquier fallo SHALL revertir transacción y liberar recursos.

#### Scenario: Error al registrar auditoría

- **WHEN** nota o auditoría falla antes del commit
- **THEN** se revierte toda transacción
- **AND** no queda nota, idempotencia ni auditoría parcial.

### Requirement: RQ-05 Preflight y migración por esquema

El sistema SHALL rechazar toda escritura con `Unavailable` si preflight no confirma `ANOTACION_TAREA` InnoDB, `Dato_Anotacion TEXT utf8`, auditoría InnoDB, índices requeridos, almacenamiento InnoDB de idempotencia y libro InnoDB de versiones. Una migración SHALL ser revisable y reversible y sólo podrá aplicarse después de inspección de sólo lectura y autorización explícita del ambiente.

#### Scenario: Preflight fallido

- **WHEN** falta precondición transaccional o de índice en esquema objetivo
- **THEN** operación devuelve `Unavailable`
- **AND** no intenta insertar, actualizar, eliminar ni auditar.

### Requirement: RQ-06 Verificación integrada sin habilitación implícita

El sistema SHALL incluir pruebas locales para autorización, idempotencia, concurrencia, aislamiento, auditoría y rollback. La E2E SHALL reutilizar exclusivamente `tools/e2e` y controles `SELECT`; una E2E de escritura SHALL ejecutarse sólo con autorización explícita de ambiente, cuenta y tarea descartable. Gates, usuarios y grupos SHALL permanecer en su estado seguro requerido.

#### Scenario: Ausencia de autorización de escritura

- **WHEN** no se autoriza ambiente, cuenta o tarea descartable para E2E
- **THEN** la evidencia registra el bloqueo operacional
- **AND** no se ejecuta E2E real, no se habilita gate y no se sustituyen resultados por simulaciones.
