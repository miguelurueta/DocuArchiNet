<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->

## Purpose

Acreditar la creación de expedientes del importador existente con destino fijo, autoridad vigente y recuperación segura, conservando la procedencia de cada evidencia y evitando efectos duplicados.

## ADDED Requirements

### Requirement: RQ-01 Fuente local y procedencia
El sistema SHALL usar la copia principal local como fuente de inspección y revisión. La copia aislada solo protege Git; una divergencia se registra y nunca se resuelve sobrescribiendo producto local. Decisión: D-01.

#### Scenario: Fuente autorizada
- **WHEN** se inspecciona DOC-116
- **THEN** se usan rutas y huellas locales y se conserva cualquier diferencia con main

#### Scenario: Cambios posteriores
- **WHEN** un archivo local cambia después del inventario
- **THEN** se revalida la evidencia afectada antes de aplicarla a la entrega

### Requirement: RQ-02 Reutilización del creador
El sistema SHALL conservar adaptador, binding, barreras, autoridad y repositorio únicos. Histórico confirmado devuelve identidad sin otro escritor; IDs incompletos o checkpoint dudoso no autorizan recreación ni búsqueda por nombre. Decisión: D-02.

#### Scenario: Reejecución confirmada
- **WHEN** la raíz tiene checkpoint confirmado verificable
- **THEN** se devuelven sus IDs sin reservar ni crear otro expediente

#### Scenario: Identidad insuficiente
- **WHEN** falta expediente, relación o atribución fiable
- **THEN** se conserva incertidumbre y no se recrea por nombre

### Requirement: RQ-03 Destino fijo EXP-01
El sistema SHALL preparar dos raíces para A, cambiar la selección visible a B y crear solo bajo A. Comprobar expediente/relación/ciclo/clasificación por SELECT e identidad histórica; no almacenar documentos definitivos. Decisión: D-03.

#### Scenario: EXP-01 nivel original
- **WHEN** dos raíces se preparan en A y el usuario selecciona B antes del avance
- **THEN** ambas relaciones quedan en A y B no recibe expedientes de esas raíces

#### Scenario: Efectos delimitados
- **WHEN** termina la prueba de creación
- **THEN** no se materializa almacenamiento ni incorporación documental definitiva

### Requirement: RQ-04 Incertidumbre EXP-02
El sistema SHALL distinguir respuesta confirmada descartada en cliente, fallo después de creación antes del checkpoint y pérdida de respuesta del checkpoint. Los dobles locales no acreditan falla real. No añadir hook productivo o endpoint; si no hay mecanismo interno seguro autorizado, registrar NO_EJECUTADO y bloquear cierre del caso. Decisión: D-04.

#### Scenario: EXP-02 respuesta cliente
- **WHEN** el cliente descarta un resultado de creación ya confirmado
- **THEN** consulta y repetición segura mantienen identidad y número de efectos; no se atribuye un fallo de servidor

#### Scenario: EXP-02 ventana interna
- **WHEN** falla después del efecto legacy o se pierde el checkpoint
- **THEN** solo evidencia conocida permite confirmar; EN_CURSO o IDs insuficientes bloquean replay y el caso real queda no ejecutado si no existe una vía segura autorizada

### Requirement: RQ-05 Autoridad de nivel EXP-03
El sistema SHALL probar agregar_expediente en ra_pro_permisos_niveles para un actor que NO sea propietario de A y cuya vía efectiva sea esa concesión. IMPORTAR_CARPETAS es una facultad distinta; revocarla no sustituye el caso. Restaurar el valor exacto y comprobar cero creaciones. Decisión: D-05.

#### Scenario: EXP-03 no propietario
- **WHEN** se retira agregar_expediente al actor no propietario antes de crear
- **THEN** el servidor deniega y los conteos prueban cero nuevos expedientes y relaciones

#### Scenario: Restauración verificable
- **WHEN** termina o falla la prueba de revocación
- **THEN** se restaura el valor original exacto y se comprueba el diario y la lectura final

### Requirement: RQ-06 Infraestructura y mutaciones acotadas
El sistema SHALL reutilizar sesión, safePost, consola, DSN MembershipUsers y controles SELECT. No usar runRec completo porque almacena documentos. El puente local es administrativo y la copia aislada tiene extensión operativa: ninguno acredita revocación de permiso de nivel. Una extensión de soporte requiere contrato cerrado, diario y restauración antes de cualquier ejecución. Decisión: D-06.

#### Scenario: Presupuesto y transporte
- **WHEN** se ejecuta una suite autorizada de expedientes
- **THEN** solo usa las operaciones y recursos revisados y no llama al recorrido documental completo

#### Scenario: Soporte divergente
- **WHEN** el puente local difiere del integrado o solo administra perfiles
- **THEN** se conserva y caracteriza el soporte local y no se usa como si revocara permisos de nivel

### Requirement: RQ-07 Cierre por evidencia y límites
El sistema SHALL conservar resultado, SHA, comandos, ambiente y procedencia de EXP01/02/03. El refinamiento por sí mismo no autoriza ni acredita ejecuciones reales. No modificar almacenamiento legacy/SII, aislamiento XML descartado, formatos, DDL o configuración; cambiar producto solo tras reproducir un defecto del alcance. Decisión: D-07.

#### Scenario: Caso sin autorización
- **WHEN** no hay permiso específico de ambiente/cuentas/recursos o falla una dependencia
- **THEN** se declara NO_EJECUTADO con causa/responsable y no se ejecutan mutaciones

#### Scenario: Cierre trazable
- **WHEN** se solicita finalizar DOC-116
- **THEN** cada criterio acredita su evidencia exacta y un caso crítico no ejecutado impide afirmar aceptación completa
