<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05 -->
## Purpose

Permitir que los consumidores presenten los sellos SII y seleccionen tipologías autorizadas usando contratos seguros, enriquecidos y compatibles, sin consultas externas redundantes.

## ADDED Requirements

### Requirement: RQ-01 Contratos aditivos
El sistema SHALL extender capabilities e items sin cambiar campos 1.0, conforme a D-01.
#### Scenario: Compatibilidad
- **WHEN** un consumidor ignora los campos añadidos
- **THEN** deserializa los campos 1.0 con igual significado
#### Scenario: Presentación 1.1
- **WHEN** admite la extensión
- **THEN** recibe catálogo, metadatos, estado y acciones versionados

### Requirement: RQ-02 Metadatos saneados
El sistema SHALL crear metadatos desde la misma inscripción/imagen de `ExternalKey`, conforme a D-02.
#### Scenario: Valores disponibles
- **WHEN** SII entrega libro, registro, fecha, acto, noticia o referencia
- **THEN** cada valor no vacío aparece como `Code/Label/Value`
#### Scenario: Ausente o sensible
- **WHEN** falta un campo o es NIT, razón social, matrícula, token, URL, ruta o payload
- **THEN** no se inventa ni expone
#### Scenario: Solo sello de inscripción
- **WHEN** una inscripción contiene anexos de varios tipos
- **THEN** `QueryItems` presenta exclusivamente anexos con `tipoanexo=505`
- **AND** preview y descarga rechazan una clave fabricada para cualquier otro tipo

### Requirement: RQ-03 Catálogo por trámite
El sistema SHALL leer tipologías con el `IdTramite` autoritativo, conforme a D-03.
#### Scenario: Catálogo válido
- **WHEN** hay relaciones no ambiguas
- **THEN** devuelve exclusivamente ID TRD, nombre, obligatoriedad y orden
#### Scenario: Contexto o mapeo inválido
- **WHEN** falta trámite, falla lectura o hay contradicción
- **THEN** responde error seguro sin consultar SII ni escoger arbitrariamente

### Requirement: RQ-04 Estado y acciones locales
El sistema SHALL calcular `Disponible`, `Importado` o `ConNovedad` desde persistencia moderna, conforme a D-04.
#### Scenario: Confirmado
- **WHEN** tarea, proveedor y clave tienen documento confirmado/reconciliado
- **THEN** es `Importado` y no permite duplicar
#### Scenario: Ausente o con novedad
- **WHEN** no existe antecedente confirmado
- **THEN** es `Disponible`
- **AND WHEN** hay fallo, parcialidad, conflicto o incertidumbre
- **THEN** es `ConNovedad` con acciones conservadoras

### Requirement: RQ-05 Llamada externa única
El sistema SHALL ejecutar una sola `consultarInformacionSello` por `QueryItems`, conforme a D-05.
#### Scenario: Cualquier cardinalidad
- **WHEN** hay cero, uno o múltiples items
- **THEN** mapea, enriquece, filtra y pagina sin otra llamada SII
#### Scenario: Gate o contexto rechazado
- **WHEN** el gate está apagado o cambia la tarea
- **THEN** no llama SII ni catálogo y devuelve código seguro
