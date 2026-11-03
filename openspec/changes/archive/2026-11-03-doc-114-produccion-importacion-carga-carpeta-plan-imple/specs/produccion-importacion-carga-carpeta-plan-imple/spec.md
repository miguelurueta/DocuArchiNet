<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Purpose

Acreditar preparación de planes de importación autorizada, determinista y sin efectos documentales mediante escenarios focales del servicio existente, sin duplicar reglas, persistencia o infraestructura.

## ADDED Requirements

### Requirement: RQ-01 — Preparación con autoridad servidor

El sistema SHALL resolver identidad, ámbito y destino autorizado en servidor en cada apertura y preparación, reutilizando la capacidad existente y sin invocar efectos documentales.

Trazabilidad: D-01, RQ-01.

#### Scenario: Contexto válido
- **WHEN** una cuenta autorizada prepara metadatos en un nivel y publicación compatibles
- **THEN** el plan corresponde a su contexto efectivo y no crea expediente ni documento

#### Scenario: Nivel ajeno o permiso revocado
- **WHEN** se solicita preparación con destino ajeno o autoridad revocada
- **THEN** el servidor rechaza sin persistir una nueva preparación ni efectos documentales

### Requirement: RQ-02 — Manifiesto consistente e identidades preservadas

El sistema SHALL validar consistencia interna, cuotas e identidades del manifiesto completo del contrato vigente y conservar los elementos declarados al informar incidencias.

Trazabilidad: D-02, RQ-02.

#### Scenario: Homónimos y subcarpetas
- **WHEN** se preparan raíces homónimas con identidades distintas y subcarpetas declaradas
- **THEN** cada raíz y archivo conserva su correlación y las subcarpetas cumplen la política publicada

#### Scenario: Manifiesto incoherente o excedido
- **WHEN** hay referencias de raíz ausentes, raíces vacías, identificadores/rutas duplicados o cuotas excedidas
- **THEN** el servidor rechaza o informa la incidencia correspondiente sin inventar elementos ni aceptar una revisión inconsistente

### Requirement: RQ-03 — Resolución publicada y fuentes diferidas

El sistema SHALL resolver valores e incidencias con las reglas publicadas, distinguir código de tipología de ID y mantener explícitas las fuentes que dependen de identidades aún inexistentes.

Trazabilidad: D-03, RQ-03.

#### Scenario: Dos formatos configurables
- **WHEN** se revisan metadatos válidos en dos formatos configurados y publicados
- **THEN** los valores respetan las reglas de cada formato y conservan ceros iniciales cuando corresponda

#### Scenario: Errores por archivo
- **WHEN** un archivo tiene tipo desconocido, nombre no interpretable, requerido ausente o conversión/longitud inválida
- **THEN** se devuelve incidencia correlacionada y no se descarta silenciosamente el archivo

#### Scenario: Identidad futura y contenido no recibido
- **WHEN** un valor depende de ID o consecutivo aún inexistente y no se recibió contenido binario
- **THEN** la fuente permanece diferida y tipada sin valor definitivo ficticio ni afirmar validación física o recepción

### Requirement: RQ-04 — Elegibilidad por política y ámbito

El sistema SHALL evaluar elegibilidad conforme a la publicación, conservando el inventario original y bloqueando siempre los errores globales impeditivos.

Trazabilidad: D-04, RQ-04.

#### Scenario: Completo por expediente
- **WHEN** A es válida y B contiene un error individual bajo COMPLETO_OBLIGATORIO/EXPEDIENTE
- **THEN** A puede ser elegible y B permanece visible y bloqueada, sin excluirla del inventario

#### Scenario: Completo por lote
- **WHEN** A es válida y B contiene un error impeditivo bajo COMPLETO_OBLIGATORIO/LOTE
- **THEN** la confirmación global no es elegible y se mantienen ambas raíces e incidencias

#### Scenario: Parcial con rechazados
- **WHEN** hay unidades o archivos válidos y rechazados bajo PARCIAL_PERMITIDO/NO_APLICA
- **THEN** se permite elegibilidad de los válidos y cada rechazado conserva su incidencia visible

#### Scenario: Error global
- **WHEN** cualquier política recibe un error global impeditivo o una solicitud globalmente inválida
- **THEN** ninguna unidad queda habilitada por ignorar el error y la respuesta no afirma recepción completa

### Requirement: RQ-05 — Plan protegido y preparación determinista

El sistema SHALL conservar planes asociados a contexto, publicación, inventario y destino, con revisión/huella y expiración servidor, sin confiar en valores finales reenviados por cliente.

Trazabilidad: D-05, RQ-05.

#### Scenario: Contenido determinista
- **WHEN** se prepara el mismo inventario canónico y configuración sin cambios externos
- **THEN** el resultado de negocio es estable salvo identidades, tiempos y campos de control documentados

#### Scenario: Autoridad o referencias alteradas
- **WHEN** el cliente inyecta valores/autoridad no admitidos o usa referencias incompatibles o revisión/huella discordantes para un plan protegido
- **THEN** el servidor rechaza sin dar autoridad a esos datos ni producir efectos documentales

#### Scenario: Cambio compatible de publicación
- **WHEN** el usuario elige una publicación distinta compatible y autorizada para otra preparación
- **THEN** se calcula un plan nuevo sin mutar el anterior ni clasificar toda selección de versión como manipulación

### Requirement: RQ-06 — Evidencia focal y herramientas compartidas

La entrega SHALL proporcionar PLAN-01/02/03 reproducibles con infraestructura E2E compartida, autorización específica y evidencia por escenario que distinga inspección, ejecución local y servidor real.

Trazabilidad: D-06, RQ-06.

#### Scenario: Sin alcance de runtime aprobado
- **WHEN** falta autorización de ambiente, cuenta, preparaciones o revocación necesaria
- **THEN** la fase dependiente no se ejecuta y queda NO_EJECUTADO con motivo y responsable

#### Scenario: Ejecución autorizada
- **WHEN** se ejecuta un caso PLAN con recursos y efectos concretos aprobados
- **THEN** se registran resultado, SHA/huellas, pasos, referencias y controles SELECT saneados sin confirmar planes válidos ni recibir/crear/almacenar documentos

#### Scenario: Cierre con dependencia pendiente
- **WHEN** algún caso o fase requerida no tiene evidencia real o restauración de permiso comprobada
- **THEN** el informe conserva FALLIDO/NO_EJECUTADO y no declara aceptación integrada a partir de mocks o evidencia REC/UI histórica
