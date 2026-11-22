<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## MODIFIED Requirements

Esta adecuación corresponde a D-02/RQ-02 de DOC122. Las decisiones D-01 (autoridad viva), D-03 (metadatos), D-04 (atomicidad), D-05 (compatibilidad), D-06 (solicitud) y D-07 (evidencia) se conservan; sus escenarios completos están en ../importacion-excel-02-autorizacion-conexion-plantillas/spec.md. Este delta modifica únicamente la elección de conexión del contrato anterior.

### Requirement: D-04 RQ-04 Autoridad propia y configuración
El contrato SHALL exigir ADMINISTRAR_CARGAS_IMPORTACION y resolver en servidor ámbito, DA e ID histórico mediante los recursos de conexión del módulo existentes. SHALL utilizar la cuenta DA de gestor_modulos cargada en sesión, sin exigir otra cuenta ni configuración de conexión exclusiva en Web.config, con límites positivos independientes de 10000 filas y 10 MB decimales. No afirma separación de privilegios mediante una cuenta dedicada.

#### Scenario: Permiso exclusivo
- **WHEN** el usuario tiene el permiso propio de carga y no administra perfiles
- **THEN** el contrato permite el recorrido de carga autorizado sin exigir permisos ajenos ni conceder administración de fuentes

#### Scenario: Límites efectivos
- **WHEN** se recibe, prepara o confirma una carga
- **THEN** se comprueban respectivamente bytes, filas y vigencia de los límites, contando1000000 bytes por MB y excluyendo el encabezado

#### Scenario: Conexión manipulada
- **WHEN** una referencia no corresponde a la DA configurada para la empresa y módulo de sesión
- **THEN** se deniega la operación y nunca se usa un host, base o credencial enviados por el navegador

#### Scenario: Reutilización de conexión del repositorio
- **WHEN** la sesión tiene un módulo DA vigente de la empresa y el catálogo histórico coincide con ese destino
- **THEN** se usa ModuleSessionConnectionStringResolver sin exigir otra conexión en Web.config ni consultar credenciales del catálogo histórico
