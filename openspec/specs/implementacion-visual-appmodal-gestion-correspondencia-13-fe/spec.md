# implementacion-visual-appmodal-gestion-correspondencia-13-fe Specification

## Purpose
TBD - created by archiving change scrumcore-138-implementacion-visual-appmodal-gestion-correspondencia-13-fe. Update Purpose after archive.
## Requirements
### Requirement: Apertura del modal desde accion de dropdown en tabla
El sistema SHALL abrir un modal de reasignacion cuando el usuario ejecute la opcion `Reasignar Tramite` dentro del dropdown de acciones de una fila en Gestion Correspondencia.

#### Scenario: Apertura desde accion de fila
- **WHEN** el usuario selecciona `Reasignar Tramite` en el dropdown de opciones de una fila
- **THEN** el sistema SHALL mostrar el modal `Reasignar Respuesta` en estado abierto con el contexto de la fila seleccionada

#### Scenario: Integracion sin modificar la tabla base
- **WHEN** se implemente la apertura del modal en el modulo
- **THEN** el sistema SHALL mantener intacta la implementacion base de la tabla (columnas, paginacion y render principal)

### Requirement: Estructura visual del modal Reasignar Respuesta
El sistema SHALL renderizar el modal usando `AppModal` con una estructura de encabezado, selector de responsables por tags, bloque de nota y acciones de cierre/envio.

#### Scenario: Header con titulo y radicado
- **WHEN** el modal este abierto
- **THEN** el sistema SHALL mostrar un encabezado con icono, titulo `Reasignar Respuesta` y el radicado asociado

#### Scenario: Campo de responsables con AppInputTags
- **WHEN** el modal este abierto
- **THEN** el sistema SHALL renderizar `AppInputTags` para agregar y remover responsables en formato chip

#### Scenario: Seccion de nota visible
- **WHEN** el modal este abierto
- **THEN** el sistema SHALL mostrar una seccion `Nota` con contenido legible y separador visual

#### Scenario: Acciones del modal
- **WHEN** el modal este abierto
- **THEN** el sistema SHALL mostrar botones `Cancelar` y `Enviar` con variantes del Design System y alineacion consistente

### Requirement: Comportamiento responsive del modal
El sistema SHALL mantener una experiencia responsive estable para desktop, tablet y mobile, evitando overflow fuera de pantalla y degradacion de legibilidad.

#### Scenario: Layout desktop estable
- **WHEN** el usuario visualiza el modal en desktop
- **THEN** el sistema SHALL mostrar el modal centrado con ancho medio y jerarquia visual clara

#### Scenario: Layout mobile adaptable
- **WHEN** el usuario visualiza el modal en mobile
- **THEN** el sistema SHALL mostrar un layout compacto con controles legibles y acciones adaptadas a ancho reducido

#### Scenario: Scroll interno al exceder altura
- **WHEN** el contenido del modal excede la altura disponible del viewport
- **THEN** el sistema SHALL habilitar scroll interno en el contenido del modal sin desplazar todo el overlay

### Requirement: Accesibilidad basica del modal
El sistema SHALL cumplir navegacion basica por teclado y etiquetado accesible para el modal de reasignacion.

#### Scenario: Focus inicial al abrir
- **WHEN** el modal se abre
- **THEN** el sistema SHALL ubicar el foco inicial en un control interactivo relevante del modal

#### Scenario: Cierre por teclado
- **WHEN** el usuario presiona `Escape` con el modal abierto
- **THEN** el sistema SHALL cerrar el modal

#### Scenario: Etiquetado accesible de titulo e iconografia
- **WHEN** el modal renderiza su encabezado
- **THEN** el sistema SHALL asociar el titulo al contenedor dialogo y marcar iconos decorativos como no informativos para lectores de pantalla

### Requirement: Modal desacoplado de logica de negocio
El sistema SHALL mantener el modal como componente de UI controlado por props y callbacks, sin llamadas directas a API ni persistencia.

#### Scenario: Envio desacoplado
- **WHEN** el usuario pulsa `Enviar`
- **THEN** el sistema SHALL ejecutar un callback de UI sin invocar persistencia de backend en el componente presentacional

#### Scenario: Cierre desacoplado
- **WHEN** el usuario pulsa `Cancelar` o cierre del modal
- **THEN** el sistema SHALL ejecutar el callback de cierre controlado por el contenedor

### Requirement: Cobertura minima de pruebas UI
El sistema SHALL contar con pruebas automatizadas que validen apertura, cierre, render de secciones y callbacks principales del modal.

#### Scenario: Pruebas de apertura y cierre
- **WHEN** se ejecute la suite de pruebas del modal
- **THEN** el sistema SHALL validar que el modal abre y cierra segun su estado controlado

#### Scenario: Pruebas de tags y acciones
- **WHEN** se ejecute la suite de pruebas del modal
- **THEN** el sistema SHALL validar render de `AppInputTags`, interaccion de eliminacion y ejecucion de callbacks `Cancelar` y `Enviar`

