# importar-servicio-enlace-sii Specification

## Purpose

Define la consulta y vista previa seguras, no mutadoras y autorizadas de anexos SII durante la preasignación de tareas ENLASE.

## Requirements

### Requirement: RQ-01 Capacidad SII para anexos ENLASE

El sistema SHALL publicar `ANEXOS_RADICADO_ENLASE` bajo el proveedor canónico `INTEGRACIONSII` sin duplicar proveedor, credenciales ni autenticación. (D-01)

#### Scenario: Capacidad conocida
- **WHEN** un contexto autorizado solicita capacidades de `INTEGRACIONSII`
- **THEN** el sistema informa `ANEXOS_RADICADO_ENLASE` con sus operaciones habilitadas

#### Scenario: Capacidad desconocida
- **WHEN** se solicita una capacidad no registrada
- **THEN** el sistema rechaza la operación sin redirigirla a constancias ni SII por defecto

### Requirement: RQ-02 Contexto autoritativo ENLASE

El sistema SHALL reconstruir y validar en servidor el contexto de preasignación antes de consultar SII. (D-02)

#### Scenario: Tarea ENLASE operable
- **WHEN** usuario, tarea, actividad, ruta, trámite y proveedor coinciden
- **THEN** el sistema permite continuar hacia la consulta

#### Scenario: Contexto incompatible
- **WHEN** la tarea no es `ENLASE`, no es operable o no coincide con el contexto autenticado
- **THEN** el sistema rechaza la solicitud antes de llamar al proveedor

### Requirement: RQ-03 Consulta y normalización de anexos

El sistema SHALL consultar `consultarRadicado`, mapear la colección `imagenes` y utilizar `idanexo` como identidad externa. (D-03)

#### Scenario: Lista vacía
- **WHEN** el proveedor no devuelve imágenes
- **THEN** el sistema responde una lista vacía válida

#### Scenario: Lista válida
- **WHEN** el proveedor devuelve uno o varios anexos con `idanexo` único
- **THEN** cada anexo se representa una vez con identidad y metadatos normalizados

#### Scenario: Identidad inválida
- **WHEN** `idanexo` está vacío o duplicado dentro de la respuesta
- **THEN** el sistema devuelve un error contractual seguro y no sintetiza una identidad

### Requirement: RQ-04 Preview seguro por identidad

El sistema SHALL resolver preview por identidad externa y entregar contenido mediante descriptor temporal y streaming seguro. (D-04)

#### Scenario: Preview autorizado
- **WHEN** el usuario solicita un `idanexo` perteneciente al contexto vigente
- **THEN** el backend reconsulta el universo, crea un descriptor temporal y entrega contenido permitido sin exponer la URL externa como autoridad

#### Scenario: Descriptor inválido o vencido
- **WHEN** el descriptor no pertenece al usuario/tarea, fue consumido o expiró
- **THEN** el handler rechaza el acceso sin filtrar rutas, tokens o URL técnica

#### Scenario: Descriptor ENLASE con selección estándar distinta
- **WHEN** el descriptor pertenece a la tarea indicada por `ID_TAREA_SELECCIONDA_ENLACE` y `SELECCIONTEMPORAL` declara la misma tarea `ENLASE`
- **THEN** el handler usa esa tarea como autoridad, ignora `ID_TAREA_SELECCIONDA` y rechaza con 404 cualquier ausencia o discrepancia

### Requirement: RQ-05 Operaciones de solo lectura observables

El sistema SHALL mantener consulta y preview libres de mutaciones de negocio y registrar telemetría técnica saneada por operación. (D-05)

#### Scenario: Consulta repetida
- **WHEN** se ejecuta `QueryItems` o preview
- **THEN** no cambian tarea, documentos, expedientes, índices, caché ni auditoría funcional

#### Scenario: Intento externo
- **WHEN** el sistema llama consulta, preview o descarga
- **THEN** registra operación, correlación y resultado sin secretos ni respuesta cruda

### Requirement: RQ-06 Compatibilidad aditiva

El sistema SHALL preservar el flujo de constancias y el recorrido legacy durante DOC-80. (D-06)

#### Scenario: Consulta de constancias
- **WHEN** se usa la capacidad existente de constancias
- **THEN** sus contratos y resultados permanecen compatibles

#### Scenario: Recorrido legacy
- **WHEN** la capacidad moderna no está activa o disponible
- **THEN** los endpoints y controles legacy conservan su comportamiento vigente

### Requirement: RQ-07 Evidencia reproducible

La entrega SHALL incluir pruebas unitarias e integración sin red y una E2E reutilizable de lectura/preview cuya ejecución requiera autorización. (D-07)

#### Scenario: Validación local
- **WHEN** se ejecutan las suites focales
- **THEN** se cubren capacidades, contexto, mapping, errores, preview y regresión de constancias

#### Scenario: E2E no autorizada
- **WHEN** no existe autorización explícita para ambiente y cuentas
- **THEN** la prueba real no se ejecuta y la limitación se registra sin inventar evidencia
