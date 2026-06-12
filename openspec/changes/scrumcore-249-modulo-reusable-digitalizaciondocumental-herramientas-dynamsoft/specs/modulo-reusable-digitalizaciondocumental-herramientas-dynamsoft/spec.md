## ADDED Requirements

### Requirement: Auditoria tecnica Dynamsoft
El cambio SHALL documentar el estado real de capacidades Dynamsoft, Fujitsu fi-7160 y flujo actual de digitalizacion, y SHALL aplicar el rediseno de toolbar/panel/layout autorizado despues de la auditoria.

#### Scenario: Auditoria previa a implementacion
- **WHEN** se cierre SCRUMCORE-249
- **THEN** existira evidencia documentada del estado anterior y recomendaciones tecnicas
- **AND** las decisiones implementadas deberan corresponder a esa auditoria

#### Scenario: Duplex documentado
- **WHEN** se revise el flujo actual de escaneo
- **THEN** la auditoria documentara la configuracion actual de `AcquireImage`
- **AND** explicara que `IfDuplexEnabled` queda en `false` cuando la UI solo llama `scan({ deviceId })`
- **AND** el workspace permitira activar duplex desde Configuracion de Escaneo sin hacerlo default

#### Scenario: Orientacion documentada
- **WHEN** se revise la visualizacion de paginas
- **THEN** la auditoria documentara como se obtienen dimensiones desde `GetImageWidth`, `GetImageHeight` y `GetImageURL`
- **AND** explicara como interpretar si el problema nace en scanner/Dynamsoft/CSS/preview
- **AND** no aplicara rotacion automatica sin evidencia de scanner fisico

#### Scenario: Matriz de capacidades
- **WHEN** se revise el entregable final
- **THEN** existira una matriz de capacidades para escaneo, procesamiento, visualizacion y documentos
- **AND** cada capacidad indicara si esta implementada, parcialmente disponible, descartada o requiere desarrollo posterior

#### Scenario: Toolbar auditado
- **WHEN** se revise el toolbar actual de digitalizacion
- **THEN** la auditoria documentara botones existentes, acciones, estados disabled/loading y ubicacion
- **AND** el toolbar implementado contendra acciones de comando: escanear, rotar izquierda, rotar derecha, eliminar, limpiar y generar PDF

#### Scenario: Panel derecho implementado
- **WHEN** se revise el panel derecho actual
- **THEN** la auditoria explicara por que Metadata consume espacio durante captura
- **AND** el workspace reemplazara Metadata por un panel compacto de Configuracion de Escaneo
- **AND** mantendra la restriccion de no usar modal, popup ni toolbar para configuracion avanzada

#### Scenario: Modos de captura implementados
- **WHEN** se revise la configuracion de escaneo futura
- **THEN** el panel diferenciara modo DocuArchi y modo Driver Scanner
- **AND** modo DocuArchi expondra ADF, duplex, color y resolucion
- **AND** modo Driver Scanner usara `IfShowUI: true` para abrir configuracion PaperStream IP

#### Scenario: Layout implementado
- **WHEN** se revise la propuesta UX
- **THEN** el workspace mantendra columnas de Miniaturas, Preview PDF y Configuracion
- **AND** priorizara el Preview PDF como area principal de trabajo

#### Scenario: Validaciones
- **WHEN** se finalice la auditoria
- **THEN** se ejecutaran validaciones tecnicas focales del repositorio
- **AND** se documentara cualquier validacion no ejecutada o bloqueada
