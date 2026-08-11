# contrato-regresion-workflow Specification

## Purpose

Define el contrato verificable de regresion del centro de trabajo Workflow para preservar su comportamiento funcional durante futuras modernizaciones visuales.

## Requirements

### Requirement: contrato de regresión de Workflow

El cambio SHALL documentar la línea base del centro de trabajo Workflow sin modificar el comportamiento funcional del módulo.

#### Scenario: inventario estático verificable

- **WHEN** se revisan los artefactos de DOC-1
- **THEN** se identifican controles, selectores, eventos, propietarios técnicos y riesgos a partir de la página, su code-behind y JavaScript asociados.

#### Scenario: sin alteración funcional

- **WHEN** se aplica DOC-1
- **THEN** no se modifican IDs WebForms, eventos, `UpdatePanel`, hidden inputs, permisos, rutas de visor ni servicios existentes.

### Requirement: contrato de selección

El cambio SHALL distinguir la tarea candidata, la tarea consolidada y los dos contextos de selección documental.

#### Scenario: selección de tarea

- **WHEN** el usuario selecciona una tarea
- **THEN** el contrato diferencia el valor candidato en cliente de la sesión canónica posterior al postback y de sus campos espejo.

#### Scenario: selección documental

- **WHEN** el usuario abre un documento o marca documentos para una acción masiva
- **THEN** el contrato separa descriptor/identificador del documento activo de la selección por checkbox.

### Requirement: límites de actualización parcial y layout

El cambio SHALL mapear los contenedores de layout y los límites `UpdatePanel` que los cambios visuales futuros no pueden reubicar ni colapsar.

#### Scenario: postback parcial

- **WHEN** menú, selección, visor o índice se actualizan parcialmente
- **THEN** la matriz identifica el panel reemplazado, el disparador y la comprobación de regresión correspondiente.

### Requirement: evidencia reproducible

El cambio SHALL definir perfiles, datos, casos y nomenclatura de evidencia para repetir la validación visual y funcional.

#### Scenario: captura de regresión

- **WHEN** exista un ambiente autorizado con corte JIRA-00, cuentas y datos de prueba
- **THEN** las capturas pueden registrarse en 1366, 1024, 768 y 375 px con versión, fecha y resultado trazables.
