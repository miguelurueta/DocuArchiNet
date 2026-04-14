## ADDED Requirements

### Requirement: Integracion de AppEditor dentro de GestionRespuestaEditorContainer
El sistema SHALL integrar `AppEditor` dentro de `GestionRespuestaEditorContainer` reemplazando el placeholder actual del area `editorSurface`, sin sustituir el shell visual del contenedor ni duplicar encabezados.

#### Scenario: Placeholder reemplazado por AppEditor
- **WHEN** el usuario accede al tab principal de gestion de respuesta
- **THEN** el texto placeholder del area `editorSurface` SHALL desaparecer y en su lugar SHALL renderizarse `AppEditor`

#### Scenario: Shell del contenedor preservado
- **WHEN** `GestionRespuestaEditorContainer` renderiza su contenido
- **THEN** el titulo, la descripcion y la estructura visual externa del contenedor SHALL mantenerse visibles una sola vez

### Requirement: Modo controlled para el editor embebido
El sistema SHALL usar `AppEditor` en modo controlled dentro del flujo de `gestionRespuesta`, evitando estado duplicado o uncontrolled en este contexto.

#### Scenario: Estado controlado por el modulo
- **WHEN** el usuario edita el contenido dentro de `AppEditor`
- **THEN** el valor HTML SHALL sincronizarse con un estado controlado definido en `GestionRespuestaEditorContainer` o `GestionRespuestaMainTabContent`

### Requirement: Integracion estable con el layout del workbench
El sistema SHALL preservar la convivencia entre el editor principal, el panel lateral derecho y el layout `workbenchBody` ya existente.

#### Scenario: Layout intacto con panel lateral
- **WHEN** `GestionRespuestaRightToolsPanel` se expande o colapsa
- **THEN** el editor principal SHALL mantenerse dentro de su columna dominante sin romper el layout del workbench

#### Scenario: Scroll interno del editor
- **WHEN** el contenido del editor excede el alto disponible
- **THEN** el scroll SHALL ocurrir dentro del editor sin romper el contenedor general ni el body del workbench

### Requirement: Regresion cero sobre AppEditor shared
El sistema SHALL mantener intacta la funcionalidad compartida previamente implementada de `AppEditor`.

#### Scenario: Funcionalidad base preservada
- **WHEN** se ejecutan las pruebas focalizadas de `AppEditor`
- **THEN** el componente SHALL seguir soportando modo controlled, toolbar, links, imagenes, `disabled` y `readOnly` sin regresiones

### Requirement: Integracion representativa cubierta por pruebas
El sistema SHALL contar con pruebas de integracion que validen el render de `AppEditor` dentro de `GestionRespuestaEditorContainer` y la estabilidad del modulo afectado.

#### Scenario: Render dentro de editorSurface
- **WHEN** se ejecutan las pruebas del modulo o del contenedor
- **THEN** se valida que `AppEditor` aparece dentro de `editorSurface` y que el placeholder ya no esta presente
