# interfaz-integracion-sii-enlace Specification

## Purpose

Ofrecer una experiencia moderna, segura y adaptable para consultar, preparar e importar anexos SII antes de la asignacion explicita de una tarea ENLASE.

## Requirements
### Requirement: RQ-01 Composición moderna ENLASE
El sistema SHALL abrir el modal moderno existente desde `a_adj_service_web` usando `INTEGRACIONSII/ANEXOS_RADICADO_ENLASE`, sin duplicar proveedor, credenciales ni modal.

#### Scenario: Gate moderno activo
- **WHEN** el usuario activa el servicio web en una preasignación ENLASE válida
- **THEN** la UI consulta la capacidad de anexos mediante el cliente API moderno

#### Scenario: Contexto inválido
- **WHEN** la sesión no aporta un contexto Workflow válido
- **THEN** la UI moderna no captura el disparador ni inicia una importación

### Requirement: RQ-02 Contexto aislado por tarea
El sistema SHALL ligar consulta, preview, intención, recuperación y proyección a la tarea, proveedor y capacidad capturados.

#### Scenario: Cambio de tarea
- **WHEN** la tarea visible cambia durante una operación
- **THEN** la UI bloquea la proyección y muestra un conflicto sin insertar documentos en la nueva tarea

### Requirement: RQ-03 Listado de anexos
El sistema SHALL representar cero, uno o múltiples anexos con títulos claros y selección limitada a elementos importables.

#### Scenario: Selección total
- **WHEN** el usuario marca o desmarca el control general
- **THEN** todos y solo los anexos importables adoptan ese estado

#### Scenario: Elemento no importable
- **WHEN** el backend publica un estado no importable
- **THEN** preparar y seleccionar quedan deshabilitados sin ocultar el estado ni las acciones autorizadas

### Requirement: RQ-04 Preview, tipología e intención única
El sistema SHALL reutilizar los contratos DOC-80/DOC-81 y crear una sola intención para toda la selección.

#### Scenario: Tipología inequívoca o ambigua
- **WHEN** el catálogo backend resuelve una única tipología
- **THEN** se predetermina; cuando no es inequívoca, el usuario debe elegir una opción autorizada

#### Scenario: Selección múltiple
- **WHEN** se confirman N anexos preparados
- **THEN** la UI crea una intención con N elementos y realiza una sola ejecución

#### Scenario: Preview seguro
- **WHEN** el usuario solicita vista previa
- **THEN** se consume un descriptor mediado y al volver se conservan selección, scroll y foco

### Requirement: RQ-05 Cierre gobernado por resultado
El sistema SHALL impedir el cierre normal mientras la ejecución carezca de resultado y conservar visibles los errores no resueltos.

#### Scenario: Ejecución pendiente
- **WHEN** la intención está ejecutándose o reconciliándose
- **THEN** X, Escape, backdrop y acciones incompatibles no cierran el modal

#### Scenario: Resultado terminal
- **WHEN** hay éxito completo
- **THEN** se refresca la lista y después se cierra; ante fallo, parcial o incierto el modal permanece abierto

### Requirement: RQ-06 Sincronización documental
El sistema SHALL sincronizar únicamente documentos confirmados de la tarea original y deduplicarlos por `DocumentId`.

#### Scenario: Documento confirmado repetido
- **WHEN** la reconciliación devuelve dos referencias al mismo `DocumentId`
- **THEN** la lista proyecta una sola referencia y ejecuta el refresco Web Forms

### Requirement: RQ-07 Asignación explícita y revalidada
El sistema SHALL mantener la asignación separada de la importación y delegar la decisión final a `Buttonaceptar_Click`.

#### Scenario: Documentos obligatorios incompletos
- **WHEN** el usuario pulsa Asignar y la validación del servidor no devuelve `YES`
- **THEN** la tarea no se asigna y se muestra el requisito pendiente

#### Scenario: Importación exitosa
- **WHEN** termina la importación
- **THEN** la UI no dispara automáticamente la asignación ni afirma preventivamente que los obligatorios están completos

### Requirement: RQ-08 Adaptabilidad y accesibilidad
El sistema SHALL mantener el diálogo y la tabla utilizables con teclado y viewports representativos sin desplazar la página anfitriona.

#### Scenario: Viewport reducido
- **WHEN** ancho o alto son limitados
- **THEN** el cuerpo usa scroll vertical, la tabla scroll horizontal y los controles permanecen alcanzables

#### Scenario: Navegación por teclado
- **WHEN** el usuario navega con Tab o cierra un estado permitido
- **THEN** el foco permanece dentro del diálogo y luego vuelve al disparador original

### Requirement: RQ-09 Compatibilidad y evidencia
El sistema SHALL conservar constancias y legacy y demostrar la integración mediante pruebas focales y E2E reutilizadas.

#### Scenario: Validación técnica
- **WHEN** se ejecutan las suites afectadas
- **THEN** se cubren contexto, listado, preview, preparación, cierre, refresco, asignación y CSS sin regresión

#### Scenario: E2E real
- **WHEN** existe autorización expresa para ambiente, cuenta y recurso aplicable
- **THEN** se reutiliza `tools/e2e`, se sanea evidencia y se verifica que no se modificó configuración de rollout
