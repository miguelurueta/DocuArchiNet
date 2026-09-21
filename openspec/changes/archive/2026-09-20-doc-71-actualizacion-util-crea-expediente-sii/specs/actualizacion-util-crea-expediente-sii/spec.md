<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->

## Purpose

Permitir importar documentos SII cuando el trámite no gestiona expedientes, preservando el flujo completo para trámites que sí los gestionan.

## ADDED Requirements

### Requirement: RQ-01 Modo autoritativo de expediente
El sistema SHALL determinar `SinExpediente` o `GestionarExpediente` únicamente desde la configuración confiable del trámite, conforme a D-01.

#### Scenario: Configuración deshabilitada
- **WHEN** `util_Estado_Crea_ExpedienteSII=0`
- **THEN** el modo efectivo es `SinExpediente`

### Requirement: RQ-02 Importación sin expediente
El sistema SHALL almacenar el documento en modo `SinExpediente` sin devolver `EXPEDIENT_CREATION_DISABLED`, conforme a D-02.

#### Scenario: Almacenamiento exitoso
- **WHEN** validaciones y almacenamiento se confirman
- **THEN** el item puede completar sin identificador de expediente

### Requirement: RQ-03 Estado NoAplica
El sistema SHALL persistir y exponer `NoAplica` para efectos deliberadamente omitidos, conforme a D-03.

#### Scenario: Relectura de intención
- **WHEN** se consulta una intención sin expediente después de persistirla
- **THEN** destino físico, relación, índices exclusivos de expediente y caché conservan `NoAplica`; los índices documentales conservan su resultado físico

### Requirement: RQ-04 Índices documentales sin expediente
El sistema SHALL consultar el expediente SII para resolver los datos de índices, omitir caché, búsqueda, creación y vínculo del expediente físico local, y actualizar NIT/cédula, razón social y matrícula de todos los documentos relacionados por `ENLASE`, conforme a D-04.

La consulta SHALL usar `consultarExpedienteMercantil` para MERCANTIL/ESAL y `consultarExpedienteProponente` para RUP. Los valores de NIT/cédula y razón social son opcionales porque algunas respuestas SII no los suministran; su ausencia SHALL NOT bloquear el almacenamiento ni sobrescribir con vacío un valor documental existente. Solo los campos presentes y válidos se incluyen en la actualización física, pero la identidad consultable —matrícula para MERCANTIL/ESAL o proponente para RUP— es obligatoria.

#### Scenario: Actualización de documentos relacionados
- **WHEN** se ejecuta una importación en modo `SinExpediente`
- **THEN** cada documento del mismo `ENLASE` actualiza y confirma sus índices por `ID` y radicado sin requerir ni modificar `ID_EXPEDIENTE`

#### Scenario: Dependencias de expediente omitidas
- **WHEN** se actualizan índices documentales sin expediente
- **THEN** se consulta el expediente SII para construir índices, pero no se invocan caché, búsqueda, creación ni vínculo del expediente físico local

#### Scenario: Identidad parcial recibida desde SII
- **WHEN** el sello no contiene NIT/cédula o razón social
- **THEN** se consulta el expediente SII y los valores que continúen ausentes se omiten sin bloquear ni borrar índices existentes

#### Scenario: Metadatos disponibles en el sello descargado
- **WHEN** matrícula, NIT/cédula o razón social están presentes en los metadatos descargados del item pero ausentes en la inscripción agregada
- **THEN** esos valores completan únicamente los campos vacíos de la inscripción antes de actualizar todos los documentos del mismo `ENLASE`

#### Scenario: Ningún índice suministrado
- **WHEN** la consulta de expediente SII no permite resolver la identidad consultable
- **THEN** la actualización se bloquea y nunca se representa como `NoAplica` ni como una escritura confirmada

### Requirement: RQ-05 Finalización coherente
El sistema SHALL completar items e intención cuando almacenamiento e índices documentales estén confirmados y los efectos exclusivos de expediente sean `Confirmado` o `NoAplica` según el plan, conforme a D-05.

#### Scenario: Sin reconciliación falsa
- **WHEN** todos los efectos planeados están satisfechos
- **THEN** la intención termina `Completada` sin afirmar efectos físicos inexistentes

### Requirement: RQ-06 Configuración incluida en fingerprint
El sistema SHALL incluir el modo autoritativo en la huella de preflight, conforme a D-06.

#### Scenario: Cambio entre preparación y creación
- **WHEN** la configuración cambia entre 0 y 1
- **THEN** la creación se rechaza antes de persistir por contexto obsoleto

### Requirement: RQ-07 Regresión de gestión de expediente
El sistema SHALL conservar la secuencia DOC-67 en modo `GestionarExpediente`, conforme a D-07.

#### Scenario: Expediente ausente
- **WHEN** creación está habilitada y no existe expediente
- **THEN** se crea una vez, verifica, vincula, indexa y registra en caché

### Requirement: RQ-08 Evidencia integral
El sistema SHALL mantener evidencia automatizada y E2E saneada de ambas ramas, conforme a D-08.

#### Scenario: Verificación final
- **WHEN** se valida DOC-71
- **THEN** pruebas y controles SELECT demuestran efectos esperados y gate restaurado
