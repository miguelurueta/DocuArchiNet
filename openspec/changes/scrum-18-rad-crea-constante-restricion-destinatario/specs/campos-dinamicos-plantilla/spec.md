## MODIFIED Requirements

### Requirement: Comportamiento de control por tipo de campo
El sistema SHALL definir y utilizar una constante tipada `CDeRelacionEstadoRetriccionDto` en el dominio de radicacion para gestionar reglas de restriccion del campo destinatario en `RadicacionForm.tsx`. La constante SHALL ser reutilizable, mantener consistencia estructural y permitir aplicar validaciones/estados sin romper el flujo actual ni los atributos declarativos existentes del formulario.

#### Scenario: Declara constante tipada para restriccion de destinatario
- **WHEN** se inicializa el formulario de radicacion
- **THEN** existe una definicion tipada de `CDeRelacionEstadoRetriccionDto` con estructura estable para reglas de restriccion

#### Scenario: Aplica restriccion de destinatario usando la constante
- **WHEN** el formulario evalua el comportamiento del campo destinatario
- **THEN** utiliza `CDeRelacionEstadoRetriccionDto` como fuente de reglas y aplica el resultado en el control sin logica duplicada inline

#### Scenario: Conserva funcionalidades y atributos existentes
- **WHEN** se integra `CDeRelacionEstadoRetriccionDto` al flujo actual
- **THEN** el campo destinatario conserva el comportamiento vigente y mantiene atributos declarativos como `required`, `disabled`, `title` y `tooltipAyuda` cuando correspondan

#### Scenario: Maneja estructura invalida sin romper el formulario
- **WHEN** la constante no contiene datos esperados o llega una configuracion incompleta
- **THEN** el formulario maneja el caso de forma controlada y mantiene el flujo operativo sin errores no controlados
