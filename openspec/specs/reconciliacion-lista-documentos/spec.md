# reconciliacion-lista-documentos Specification

## Purpose

Definir la reconciliación autoritativa, autorizada y conservadora de importaciones, así como la proyección segura de documentos confirmados en la lista de la tarea original.

## Requirements

### Requirement: RQ-01 Consulta autorizada (D-01)

El sistema SHALL validar contexto contra intención y tarea original antes de responder.

#### Scenario: Contexto ajeno

- **WHEN** usuario o tarea no coinciden
- **THEN** Get/Reconcile responden error seguro sin items

### Requirement: RQ-02 Reconstrucción persistida (D-02)

El sistema SHALL reconstruir intención, items y documentos desde fuentes persistidas con consultas parametrizadas.

#### Scenario: Respuesta perdida

- **WHEN** el cliente recarga tras perder la respuesta
- **THEN** recibe la última evidencia confirmada sin Session

### Requirement: RQ-03 Consulta focal (D-03)

El sistema SHALL limitar búsqueda por intención, proveedor, identidad externa y tarea autorizada.

#### Scenario: Identidad ajena

- **WHEN** la identidad existe fuera de la intención
- **THEN** no aparece en la respuesta

### Requirement: RQ-04 Relación inequívoca (D-04)

El sistema SHALL declarar `Disponible` solo un documento con relación única a la tarea original.

#### Scenario: Relación ausente o cruzada

- **WHEN** falta relación o pertenece a otra tarea
- **THEN** queda inconsistente y fuera de confirmados

#### Scenario: Relación duplicada

- **WHEN** hay relaciones incompatibles
- **THEN** no se elige una arbitrariamente

### Requirement: RQ-05 Mapping total (D-05)

El sistema SHALL mapear cada fase/consistencia a exactamente un estado visible.

#### Scenario: Fase soportada

- **WHEN** se proyecta cualquier fase válida
- **THEN** existe una única salida contractual segura

### Requirement: RQ-06 Incertidumbre (D-06)

El sistema SHALL evitar que timeout o persistencia desconocida se presenten como disponibles.

#### Scenario: Efecto no demostrable

- **WHEN** la lectura no confirma el efecto
- **THEN** retorna `Verificando`/`ResultadoIncierto` sin reintento inseguro

### Requirement: RQ-07 DTO saneado (D-07)

El sistema SHALL devolver mínimos de refresco en contrato v1.

#### Scenario: Serialización pública

- **WHEN** se serializa el resultado
- **THEN** omite ruta, secreto, excepción, SQL y `dato_lista`

### Requirement: RQ-08 Deduplicación trazable (D-08)

El sistema SHALL emitir una entrada por documento/tarea y conservar correlación en anomalías.

#### Scenario: Filas repetidas

- **WHEN** varias filas representan el mismo confirmado
- **THEN** la respuesta contiene una sola entrada

### Requirement: RQ-09 Compatibilidad aditiva (D-09)

El sistema SHALL agregar reconciliación sin modificar endpoints, escritura o legacy.

#### Scenario: Auditoría de regresión

- **WHEN** se revisa diff y pruebas
- **THEN** ASMX, JS, almacenamiento y cachés permanecen intactos

### Requirement: Reconciliación autoritativa de la intención

El sistema SHALL obtener y reconciliar el estado de una intención exclusivamente mediante la API moderna, sin consultar SII ni realizar persistencia desde el navegador. **Origen: D-01, RQ-01.**

#### Scenario: Resultado incierto

- **WHEN** una ejecución termina con resultado incierto y existe contexto autorizado de intención, tarea, proveedor e identidad externa
- **THEN** el frontend solicita la reconciliación mediante la API moderna y proyecta la respuesta estructurada

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
- **THEN** no modifica almacenamiento, ASMX ni scripts legacy

### Requirement: Conservación del contexto visual

El sistema SHALL conservar filtros y scroll, limpiar la selección transitoria y restaurar un foco predecible al volver del documento; la reapertura SHALL usar estado persistido. **Origen: D-06, RQ-06.**

#### Scenario: Volver del documento

- **WHEN** el usuario vuelve desde “Ver documento importado”
- **THEN** se restauran filtros y scroll, se limpia selección y el foco vuelve a una acción conocida

#### Scenario: Cerrar y reabrir

- **WHEN** se cierra y vuelve a abrir el recorrido
- **THEN** el estado visible se reconstruye desde el snapshot autoritativo persistido
