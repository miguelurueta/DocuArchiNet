# importacion-excel-02-autorizacion-conexion-plantillas

## Purpose
Administrar el acceso y las plantillas de importación Excel sobre la DA autorizada, conservando los catálogos históricos, la configuración del módulo y la integridad de los guardados. La recepción e importación de archivos pertenecen a las fases posteriores.

## Requirements

### Requirement: RQ-01 Autorización vigente propia
Origen: D-01.
El sistema SHALL consultar RA en cada operación: usuario activo, empresa y permiso exacto ADMINISTRAR_CARGAS_IMPORTACION=1. Indicador de sesión solo para presentación.

#### Scenario: Permiso exclusivo
- **WHEN** solo tiene permiso del cargador
- **THEN** opera sin permiso de perfiles

#### Scenario: Acceso revocado
- **WHEN** está inactivo, cambia empresa o pierde permiso
- **THEN** se deniega antes de consultar plantillas

### Requirement: RQ-02 Conexión DA del módulo
Origen: D-02.
El sistema SHALL resolver DA mediante ModuleSessionConnectionStringResolver y la sesión del módulo cargada desde gestor_modulos. Exigir empresa/módulo vigentes y correspondencia exacta del ID histórico con servidor/base/proveedor de esa DA. No leer credenciales de imp01_xmlconexion ni exigir otra conexión en Web.config.

#### Scenario: Vínculo manipulado
- **WHEN** el ID, servidor o base difieren del ámbito autorizado
- **THEN** se deniega sin revelar secretos

#### Scenario: Configuración ausente
- **WHEN** falta una sesión DA válida de la empresa y módulo
- **THEN** se rechaza sin conexiones alternativas ni credenciales del navegador

#### Scenario: Cambio de módulo durante una operación
- **WHEN** cambia la empresa, módulo o conexión DA antes de abrir el repositorio
- **THEN** la fábrica capturada deniega el acceso y no reutiliza la conexión anterior

### Requirement: RQ-03 Contrato y metadatos
Origen: D-03.
El sistema SHALL exponer DTO por ID, paginación acotada y destino imp_02_ validado en information_schema. CREAR admite destino ausente; AGREGAR/REEMPLAZAR requieren columnas existentes. Guardar no altera destino.

#### Scenario: Consulta acotada
- **WHEN** lista o consulta por ID
- **THEN** recibe solo plantillas de la conexión autorizada

#### Scenario: Destino inválido
- **WHEN** tabla o columnas no corresponden a metadatos elegibles
- **THEN** se rechaza sin modificar tablas

### Requirement: RQ-04 Guardado atómico
Origen: D-04.
El sistema SHALL guardar cabecera y campos en una transacción InnoDB; bloqueo y comparación de revisión más huella vigente. Un ganador, conflicto para el segundo, rollback completo ante fallo.

#### Scenario: Concurrencia
- **WHEN** dos guardados comparten revisión y huella
- **THEN** hay un ganador y un CONFLICTO_REVISION

#### Scenario: Fallo de mapeo
- **WHEN** falla cualquier escritura de campos
- **THEN** se revierte también cabecera

#### Scenario: Cambio antiguo
- **WHEN** cambia un campo histórico sin incrementar revisión
- **THEN** la huella detecta conflicto

### Requirement: RQ-05 Compatibilidad histórica
Origen: D-05.
El sistema SHALL conservar ruta histórica, ID de mapeos y transformaciones no cambiadas. Snapshot v1 canónico DOC121; NULL explícito, orden ordinal, SHA256. Leer sin clave permitido; guardar exige clave mapeada. Flags inconsistentes dan error controlado.

#### Scenario: Ruta histórica
- **WHEN** se edita una plantilla existente
- **THEN** se conserva RUTA_ARCHIVO_FUENTE

#### Scenario: Alta web
- **WHEN** se crea plantilla XLS o XLSX
- **THEN** su ruta relativa es generada en servidor y mide como máximo120 caracteres

#### Scenario: Flags y clave
- **WHEN** hay flags inconsistentes o se guarda sin clave mapeada
- **THEN** se rechaza; la lectura legacy sin clave permanece disponible

#### Scenario: Canonización
- **WHEN** solo cambia el orden SQL de campos
- **THEN** la huella es idéntica, sin omitir NULL ni transformaciones

### Requirement: RQ-06 ASMX y solicitud
Origen: D-06.
El sistema SHALL exigir POST JSON, mismo origen, CSRF propio ligado a sesión/usuario/empresa; bootstrap valida origen antes de emitir token. DTO de respuesta tipados. Límites positivos con defaults 10000/10MB decimal.

#### Scenario: Solicitud forjada
- **WHEN** cambia origen, token o contexto
- **THEN** se rechaza antes de guardar

#### Scenario: Límites
- **WHEN** faltan límites o son inválidos
- **THEN** se usan10000/10 o se cierra capacidad respectivamente

### Requirement: RQ-07 Verificación y límites
Origen: D-07.
El sistema SHALL acreditar un build aislado y pruebas nativas sintéticas; E2E reutiliza tools/e2e y requiere autorización específica. Instalación SQL manual y datos EGRESOS intactos.

#### Scenario: Prueba sintética
- **WHEN** se ejecutan fixtures locales
- **THEN** no se afirma E2E real ni se escribe en base

#### Scenario: Operación real
- **WHEN** hace falta instalar o autenticar una prueba
- **THEN** se respeta SQL manual y autorización específica
