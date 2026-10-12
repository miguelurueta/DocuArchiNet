# doble-factor-despliegue Specification

## Purpose

Definir una entrega operativa, gradual y reversible del Login 2FA verificado, sin ejecutar despliegues ni activar ambientes por efecto de esta documentación.

## Requirements

### Requirement: RQ-01 Alcance operativo sin cambios de producto
DOC-98 SHALL limitarse a runbook, scripts operativos, validadores y evidencia, preservando binarios y contratos 2FA (D-01).

#### Scenario: Inventario de entrega
- **WHEN** se valida DOC-98
- **THEN** cada componente 2FA existente continúa registrado una vez
- **AND** no se agregan modelos, DTOs, servicios, repositorios, ASMX o UI

### Requirement: RQ-02 Preflight sin DDL duplicado
El preflight SHALL verificar el esquema existente por metadatos y detenerse ante ausencias, sin crear ni alterar objetos (D-02).

#### Scenario: Esquema compatible
- **WHEN** tabla, diez columnas e índices coinciden con DOC-92
- **THEN** la puerta se aprueba sin migración

#### Scenario: Diferencia de esquema
- **WHEN** falta una columna o índice
- **THEN** el resultado es NO-GO y ningún script lo corrige automáticamente

### Requirement: RQ-03 Configuración externa verificable
El runbook SHALL comprobar llaves externas, Session, SMTP y configuración 2FA sin revelar secretos (D-03).

#### Scenario: Configuración válida
- **WHEN** existe keyId activo, material Base64 de 32 bytes o más, Session=10 y una fila SMTP activa
- **THEN** la puerta informa solo estados y cardinalidades

#### Scenario: Configuración inválida
- **WHEN** una llave falta, SMTP no es único o el timeout difiere
- **THEN** se detiene sin imprimir valores

### Requirement: RQ-04 Secuencia go/no-go
La entrega SHALL imponer autorización, preflight, build, paquete/configuración, smoke OFF, piloto, E2E, observación y decisión (D-04).

#### Scenario: Fase fallida
- **WHEN** una fase falla o carece de autorización
- **THEN** no se ejecutan fases mutantes posteriores y se registra el bloqueo saneado

### Requirement: RQ-05 Activación piloto aislada
La activación SHALL afectar una fila autorizada del `gestor_modulos` central y conservar `0`/`NULL` como estado inicial compatible (D-05).

#### Scenario: Piloto válido
- **WHEN** empresa, módulo, proveedor, expiración, cuenta y snapshot coinciden
- **THEN** el cambio guardado afecta una fila y se verifica por lectura

#### Scenario: Alcance ambiguo
- **WHEN** la cardinalidad no es uno o el snapshot cambió
- **THEN** no se activa ningún módulo

### Requirement: RQ-06 Observabilidad sanitizada y fiel
La observación SHALL separar estados derivados de columnas de fallos emitidos por diagnósticos (D-06).

#### Scenario: Métricas de challenge
- **WHEN** se consultan métricas SQL
- **THEN** solo se agregan pendientes, consumidos, bloqueados y expirados sin identidades ni código protegido

#### Scenario: Fallo de entrega o finalización
- **WHEN** aparece `FAILED` o `REJECTED`
- **THEN** se usa correlation/stage/outcome y no se afirma una columna física de estado

### Requirement: RQ-07 Rotación compatible de llaves
La rotación SHALL conservar cada llave referenciada por un challenge elegible durante 10 minutos más el margen aprobado (D-07).

#### Scenario: Cambio de llave activa
- **WHEN** se activa un keyId nuevo
- **THEN** la anterior permanece hasta cero challenges elegibles y ninguna llave aparece en evidencia

### Requirement: RQ-08 Limpieza manual gobernada
La limpieza SHALL considerar solo terminales mayores de 30 días y requerir autorización separada, sin scheduler (D-08).

#### Scenario: Candidatos
- **WHEN** se ejecuta el control
- **THEN** primero se informa cardinalidad agregada y se excluyen elegibles

#### Scenario: Sin autorización
- **WHEN** no existe autorización específica vigente
- **THEN** no se elimina ninguna fila

### Requirement: RQ-09 Rollback por capas
El rollback SHALL desactivar primero el piloto, preservar datos/evidencia y restaurar después el paquete aprobado si continúa el incidente (D-09).

#### Scenario: Umbral de aborto
- **WHEN** falla configuración, OTP, login legacy o diagnósticos superan el umbral
- **THEN** se desactiva solo el piloto, los pendientes expiran naturalmente y no se borran esquema ni registros

### Requirement: RQ-10 Activación global transversal por módulo
La implementación SHALL culminar con 2FA activo en todas las filas centrales incluidas en el manifiesto autorizado de DocuArchi, Gestor, Radicación y Workflow. La decisión SHALL depender únicamente de la configuración central de cada módulo y aplicarse a cualquier principal que intente autenticarse, sin habilitación, lista, bandera ni excepción por usuario (D-10).

#### Scenario: Diagnóstico previo sin selector de usuarios
- **WHEN** se prepara la activación posterior al piloto
- **THEN** se obtiene únicamente un diagnóstico agregado de identidades y correos
- **AND** sus resultados no se convierten en banderas, listas de habilitación ni excepciones por usuario
- **AND** la activación se decide exclusivamente sobre las filas del módulo incluidas en el manifiesto autorizado

#### Scenario: Activación por oleadas
- **WHEN** el piloto y todas las puertas están aprobados
- **THEN** cada oleada modifica solo IDs explícitos con snapshot y conteo esperado
- **AND** se verifica `RequiereSegundoFactor=1`, proveedor `EMAIL=1` y expiración `1..10` antes de continuar

#### Scenario: Estado final
- **WHEN** finaliza la última oleada
- **THEN** todas las filas autorizadas de los cuatro módulos están activas
- **AND** ninguna fila fuera del manifiesto fue modificada
- **AND** no existe bypass ni mecanismo de activación para usuarios individuales
- **AND** las tablas de principales participan solo en la resolución de identidad y correo en tiempo de autenticación
- **AND** una identidad o un correo inválido produce fallo cerrado sin desactivar 2FA para otros usuarios
