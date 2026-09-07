# reconciliacion-lista-documetos Specification

## Purpose

Definir reconciliación autoritativa, autorizada y conservadora desde persistencia.

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
