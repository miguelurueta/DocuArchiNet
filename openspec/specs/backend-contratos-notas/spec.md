# backend-contratos-notas Specification

## Purpose

TBD: Definir el propósito estable de la capacidad de contratos internos de Notas Workflow.

## Requirements

### Requirement: Alcance interno y compatibilidad de DOC-40

El sistema SHALL definir la fundación de Notas exclusivamente para Workflow sin alterar páginas, clientes, contratos públicos legacy, feature gates, datos ni esquema durante esta fase.

**Trazabilidad:** D-01, D-06.

#### Scenario: Conservación del recorrido legacy

- **WHEN** se revisa el cambio DOC-40 antes de autorizar implementación
- **THEN** no contiene modificación de UI, consumidor, endpoint publicado, configuración de gate, base de datos o módulo distinto de Workflow
- **AND** los recorridos legacy quedan como comportamiento vigente.

### Requirement: Tarea explícita y validada por el servidor

El sistema SHALL modelar toda operación moderna de Notas con `idTarea` explícito y deberá exigir `idNota` para una nota concreta; la tarea no podrá suplirse con una selección mutable de sesión.

**Trazabilidad:** D-02.

#### Scenario: Dos solicitudes de tareas distintas

- **WHEN** dos solicitudes del mismo actor indican tareas distintas
- **THEN** cada una se resuelve mediante el puerto de tarea con su propio `idTarea`
- **AND** una selección previa de sesión no cambia el recurso que se valida.

### Requirement: Contexto y permiso de Notas fail-closed

El sistema SHALL definir un gate de Notas que resuelva identidad, grupo y permiso desde sesión autenticada del servidor, sin aceptar esos atributos desde el cliente.

**Trazabilidad:** D-03.

#### Scenario: Contexto incompleto o permiso ausente

- **WHEN** no se puede resolver sesión autenticada, identidad, grupo, permiso o contexto de tarea
- **THEN** el gate entrega un contexto no autorizado
- **AND** el servicio no procesa una operación de Notas.

### Requirement: Contratos tipados de operaciones de Notas

El sistema SHALL definir solicitudes y respuestas tipadas para listar, contar, crear, consultar, actualizar y eliminar, con el recurso de tarea explícito en todas las firmas.

**Trazabilidad:** D-04.

#### Scenario: Resultado seguro de una operación inválida

- **WHEN** una operación no puede completarse por autorización, estado, pertenencia, versión o contenido
- **THEN** el contrato expresa uno de `Forbidden`, `TaskNotActive`, `NoteNotFound`, `NotOwner`, `VersionConflict`, `InvalidContent` o `Unavailable`
- **AND** no expone SQL, excepciones ni detalles de infraestructura.

### Requirement: Separación de dominio y persistencia de Workflow

El sistema SHALL mantener la lógica de Notas fuera de `Class_anotacion_tarea` y de controles WebForms, con puertos y repositorios que utilicen parámetros para datos de negocio.

**Trazabilidad:** D-05.

#### Scenario: Dependencias de la capa moderna

- **WHEN** se inspeccionan modelos, servicios y repositorios de Notas que se autoricen posteriormente
- **THEN** no dependen de `Page`, `GridView`, `UpdatePanel` ni `HttpContext`
- **AND** la persistencia usa el patrón parametrizado de infraestructura Workflow.

### Requirement: Política de futura escritura definida y ejecución diferida

El sistema SHALL conservar sin cambios los datos y consumidores legacy en DOC-40, pero las futuras mutaciones SHALL aplicar esta política: borrado físico con auditoría atómica; mutación exclusivamente por propietario; histórico de solo lectura para cualquier usuario Workflow autorizado a consultar la tarea; contenido de texto plano no vacío de máximo 16.000 unidades UTF-16 del plano básico multilingüe, clasificado y retenido junto con la tarea o documento padre; auditoría sin texto completo, usando huella SHA-256 y longitud; e idempotencia por `idTarea`, autor e identificador de solicitud durante 30 días.

Antes de activar una mutación en MySQL 5.1, cada esquema objetivo SHALL pasar el preflight de motor InnoDB para nota y auditoría, `TEXT utf8` para contenido, índices de listado y tabla de idempotencia. El servicio SHALL rechazar caracteres fuera del plano básico multilingüe. Si falta una precondición, el resultado será `Unavailable` y no habrá escritura.

**Trazabilidad:** D-06.

#### Scenario: Escritura bloqueada por preflight de esquema

- **WHEN** un esquema conserva `ANOTACION_TAREA` en MyISAM, no tiene auditoría InnoDB o carece de la tabla de idempotencia requerida
- **THEN** el servicio no publica una mutación efectiva y devuelve `Unavailable`
- **AND** no cambia nota, auditoría ni estado de tarea.

### Requirement: Evidencia local antes de exposición

El sistema SHALL prever pruebas unitarias focales de gate, contratos y resultados funcionales antes de publicar el primer comportamiento de Notas.

**Trazabilidad:** D-07.

#### Scenario: Validación de la fundación

- **WHEN** una autorización posterior habilite código de DOC-40
- **THEN** la evidencia se genera con dobles locales, sin base real ni E2E autenticada
- **AND** una E2E se planifica junto con la primera fase que exponga un recorrido de usuario.

### Requirement: Ruta de negocio incluida en la autorización

El sistema SHALL tratar la ruta Workflow como parte del contexto autorizado y del snapshot de tarea. La solicitud de Notas no podrá indicar ruta, nombre de ruta, tabla ni metadato dinámico.

**Trazabilidad:** D-08.

#### Scenario: Ruta inválida, ausente o incoherente

- **WHEN** el gate no puede resolver `IdRutaWorkflow`, la tarea no tiene `IdRuta` válida o la ruta no corresponde al contexto autorizado
- **THEN** la operación se rechaza con un resultado funcional seguro
- **AND** no se consulta ni construye una tabla o identificador a partir de datos enviados por el cliente.
