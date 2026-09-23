<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## ADDED Requirements

### Requirement: Reconciliación autoritativa de la intención
El sistema SHALL obtener y reconciliar el estado de una intención exclusivamente mediante la API moderna, sin consultar SII ni realizar persistencia desde el navegador. **Origen: D-01, RQ-01.**

#### Scenario: Resultado incierto
- **WHEN** una ejecución termina con resultado incierto y existe contexto autorizado de intención, tarea, proveedor e identidad externa
- **THEN** el frontend solicita `ReconcileImportIntent` mediante `importar-servicio-web-api.js` y proyecta la respuesta estructurada

#### Scenario: Recuperación sin sondeo
- **WHEN** ocurre timeout, pérdida de respuesta o reapertura autorizada
- **THEN** el frontend realiza una consulta explícita de snapshot sin polling ni acceso directo a SII

### Requirement: Estados visibles conservadores
El sistema SHALL mostrar los estados backend normativos y SHALL NOT convertir timeout, ausencia de respuesta o estado desconocido en importación confirmada. **Origen: D-02, RQ-02.**

#### Scenario: Respuesta ausente
- **WHEN** no existe confirmación autoritativa de un elemento
- **THEN** permanece `Verificando` y no se marca `Disponible` ni importado

#### Scenario: Resultado parcial
- **WHEN** la respuesta contiene elementos con estados diferentes
- **THEN** cada elemento conserva su estado real y el resultado global permanece parcial

### Requirement: Aislamiento estricto de tarea
El sistema SHALL actualizar la lista únicamente para items `Disponible` con `DocumentId` autorizado cuyo `TaskId` coincida con la tarea actualmente visible. **Origen: D-03, RQ-03.**

#### Scenario: Tarea diferente
- **WHEN** el resultado corresponde a una tarea distinta de la visible
- **THEN** no inserta documentos ni modifica la lista actual

#### Scenario: Documento autorizado
- **WHEN** el item está `Disponible`, tiene `DocumentId` positivo y coincide la tarea
- **THEN** puede agregarse a la lista y ofrecer “Ver documento importado”

### Requirement: Actualización en lote sin duplicados
El sistema SHALL recorrer una sola vez el resultado final y SHALL deduplicar por `DocumentId`, sin depender del orden de `Items`. **Origen: D-04, RQ-04.**

#### Scenario: Intención múltiple
- **WHEN** finaliza una intención con varios elementos
- **THEN** prepara una única actualización de lote aunque la proyección visual delegue item por item

#### Scenario: Documento duplicado
- **WHEN** dos items confirman el mismo `DocumentId`
- **THEN** la lista recibe una sola actualización para ese documento

### Requirement: Compatibilidad visual encapsulada
El sistema SHALL actualizar la lista existente mediante un adaptador y SHALL solicitar refresco autoritativo completo cuando el DTO moderno no permita construir el contrato visual con seguridad. **Origen: D-05, RQ-05.**

#### Scenario: Datos visuales insuficientes
- **WHEN** faltan campos necesarios para delegar de forma segura
- **THEN** refresca la lista completa sin inventar campos ni interpretar `dato_lista`

#### Scenario: Invariancia legacy
- **WHEN** se integra DOC-77
- **THEN** no modifica `insert_row_documento_relacionado(...)`, `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento`, ASMX ni scripts legacy

### Requirement: Conservación del contexto visual
El sistema SHALL conservar filtros y scroll, limpiar la selección transitoria y restaurar un foco predecible al volver del documento; la reapertura SHALL usar estado persistido. **Origen: D-06, RQ-06.**

#### Scenario: Volver del documento
- **WHEN** el usuario vuelve desde “Ver documento importado”
- **THEN** se restauran filtros y scroll, se limpia selección y el foco vuelve a una acción conocida

#### Scenario: Cerrar y reabrir
- **WHEN** se cierra y vuelve a abrir el recorrido
- **THEN** el estado visible se reconstruye desde el snapshot autoritativo persistido
