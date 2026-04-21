## ADDED Requirements

### Requirement: Bloqueo de gestion respuesta sin estructura valida
El sistema SHALL bloquear la vista operativa de Gestion Respuesta cuando no exista estructura valida asociada al `idTareaWf` del detalle de correspondencia.

#### Scenario: Bloqueo por idTareaWf invalido
- **WHEN** el usuario abre el detalle de Gestion Respuesta con un `idTareaWf` ausente, no numerico o no positivo
- **THEN** el sistema MUST mostrar estado de bloqueo y no MUST renderizar la superficie editable de gestion

#### Scenario: Bloqueo por estructura vacia
- **WHEN** la consulta de estructura por `idTareaWf` responde en exito pero sin datos de estructura
- **THEN** el sistema MUST mostrar estado de bloqueo y no MUST habilitar interacciones de edicion/envio

#### Scenario: Bloqueo por error de integracion
- **WHEN** la consulta de estructura por `idTareaWf` falla por error HTTP o de conectividad
- **THEN** el sistema MUST mostrar estado de bloqueo con mensaje de indisponibilidad y no MUST habilitar interacciones de edicion/envio

### Requirement: Estado de carga previo a decision de bloqueo
El sistema SHALL mantener un estado de carga transitorio mientras la estructura por `idTareaWf` se encuentra en consulta y antes de resolver si el detalle queda habilitado o bloqueado.

#### Scenario: Carga inicial de estructura
- **WHEN** el usuario abre un detalle con `idTareaWf` potencialmente valido y la consulta de estructura esta en progreso
- **THEN** el sistema MUST mostrar metadata en estado de carga y no MUST clasificar el detalle como bloqueado hasta completar la consulta

### Requirement: Accion de salida desde estado bloqueado
El sistema SHALL ofrecer una accion explicita para regresar a la bandeja cuando el detalle este bloqueado.

#### Scenario: Retorno a bandeja desde bloqueo
- **WHEN** el usuario activa la accion "Volver a bandeja" en estado bloqueado
- **THEN** el sistema MUST navegar a la ruta de listado de gestion de correspondencia sin mantener el detalle abierto
