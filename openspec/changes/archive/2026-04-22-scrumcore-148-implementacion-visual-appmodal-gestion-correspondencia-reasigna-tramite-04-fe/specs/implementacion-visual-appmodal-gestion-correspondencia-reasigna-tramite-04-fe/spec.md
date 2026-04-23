## ADDED Requirements

### Requirement: Confirmacion visual de tramite reasignado con AppModal
El sistema SHALL mostrar un modal de confirmacion de exito cuando la reasignacion de tramite se complete en el flujo de `ReasignarRespuestaModal`, usando `AppModal` como contenedor principal y sin acoplar el nuevo modal a logica de negocio.

#### Scenario: Apertura de confirmacion despues de envio valido
- **WHEN** el usuario completa un envio valido desde `ReasignarRespuestaModal`
- **THEN** el sistema abre `TramiteReasignadoModal` y presenta un estado visual de confirmacion

#### Scenario: Cierre de confirmacion por accion primaria
- **WHEN** el usuario pulsa el boton `Aceptar` en `TramiteReasignadoModal`
- **THEN** el sistema ejecuta `onClose` y cierra el modal de confirmacion

### Requirement: Contenido minimo obligatorio del modal de confirmacion
El sistema SHALL renderizar en el modal de confirmacion el titulo `Tramite Reasignado`, un icono de exito junto al titulo y los datos `Usuario Asignado` y `Radicado` con etiquetas destacadas.

#### Scenario: Render de datos de confirmacion
- **WHEN** `TramiteReasignadoModal` recibe `usuarioAsignado` y `radicado`
- **THEN** el sistema muestra ambos valores en dos lineas separadas con etiquetas en negrita

#### Scenario: Header de exito coherente con design system
- **WHEN** se renderiza el header del modal de confirmacion
- **THEN** el sistema muestra el titulo centrado con iconografia de exito y separador visual sutil

### Requirement: Componente desacoplado y reusable en modulo de gestion de correspondencia
El sistema SHALL implementar el modal de confirmacion como componente dedicado en `src/modules/gestionCorrespondencia/components/modalTramiteReasignado/` con TypeScript estricto, CSS Modules y sin modificar componentes shared.

#### Scenario: Estructura desacoplada del componente
- **WHEN** se crea la implementacion del modal de confirmacion
- **THEN** el sistema ubica `TramiteReasignadoModal.tsx` y `TramiteReasignadoModal.module.css` en la carpeta dedicada del modulo

#### Scenario: Restricciones de implementacion
- **WHEN** se revisa el codigo del cambio
- **THEN** el sistema no introduce `any`, no modifica `AppModal` y no agrega estilos globales

### Requirement: Responsive y accesibilidad del modal de confirmacion
El sistema SHALL mantener comportamiento responsive y accesible del modal de confirmacion para desktop, tablet y mobile, con foco inicial en la accion primaria.

#### Scenario: Presentacion responsive en mobile
- **WHEN** el modal de confirmacion se renderiza en viewport mobile
- **THEN** el sistema usa ancho cercano al contenedor y renderiza el boton `Aceptar` en ancho completo

#### Scenario: Foco inicial y navegacion por teclado
- **WHEN** el modal de confirmacion se abre
- **THEN** el foco inicial recae en el boton `Aceptar` y la navegacion por teclado permanece funcional

### Requirement: Cobertura minima de pruebas del flujo de confirmacion
El sistema SHALL incluir pruebas unitarias e integracion para validar apertura, contenido y cierre del modal de confirmacion sin regresiones del flujo existente.

#### Scenario: Pruebas del componente de confirmacion
- **WHEN** se ejecutan pruebas de `TramiteReasignadoModal`
- **THEN** se valida render con `open=true`, visibilidad de `Usuario Asignado` y `Radicado`, y ejecucion de `onClose` con `Aceptar`

#### Scenario: Prueba de integracion desde modal de reasignacion
- **WHEN** se ejecuta la prueba del flujo de submit valido en `ReasignarRespuestaModal`
- **THEN** se valida que la confirmacion se abra con los datos esperados y pueda cerrarse correctamente
