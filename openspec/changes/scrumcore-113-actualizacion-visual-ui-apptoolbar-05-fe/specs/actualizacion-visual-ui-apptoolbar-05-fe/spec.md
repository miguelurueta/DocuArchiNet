## ADDED Requirements

### Requirement: Toolbar del tab Gestion con tres acciones homogeneas
El sistema SHALL renderizar el `AppToolbar` del tab **Gestion** con exactamente
tres acciones visibles alineadas a la izquierda:

- `Solicitud de Aprobacion`
- `Guardar`
- `Enviar`

#### Scenario: Render del toolbar ajustado
- **WHEN** el usuario navega al tab **Gestion** en `GestionRespuesta`
- **THEN** el toolbar muestra las tres acciones requeridas en el bloque izquierdo

### Requirement: Acciones con contrato visual uniforme
El sistema SHALL renderizar las tres acciones del toolbar del tab **Gestion**
usando el mismo contrato visual con `size="sm"` y `variant="ghost"`.

#### Scenario: Consistencia visual de acciones
- **WHEN** el toolbar del tab **Gestion** se renderiza
- **THEN** las acciones `Solicitud de Aprobacion`, `Guardar` y `Enviar`
  comparten el mismo tamano y variante visual

### Requirement: Tinta gris solo en estado normal
El sistema SHALL renderizar el texto y los iconos de las acciones `ghost` del
toolbar del tab **Gestion** con una tinta gris en estado normal, manteniendo sin
cambios el hover, focus y el comportamiento general del boton.

#### Scenario: Estado base del boton ghost
- **WHEN** el toolbar del tab **Gestion** se renderiza en estado normal
- **THEN** `Solicitud de Aprobacion`, `Guardar` y `Enviar` muestran texto e
  iconos en gris
- **AND** el hover y focus mantienen la respuesta visual ya definida para
  `AppButton`

### Requirement: Enviar sin jerarquia primaria separada
El sistema SHALL dejar de representar `Enviar` como accion primaria separada en
el toolbar del tab **Gestion**, integrandola al mismo grupo visual que las
demas acciones.

#### Scenario: Agrupacion visual uniforme
- **WHEN** el toolbar del tab **Gestion** se renderiza
- **THEN** `Enviar` aparece junto a las otras acciones dentro del mismo grupo
- **AND** no existe una accion primaria destacada separada para ese toolbar

### Requirement: Ajuste encapsulado sin regresion de layout
El sistema SHALL aplicar este ajuste visual sin alterar la composicion general
del workbench del tab **Gestion** ni afectar el tab **Documentos**.

#### Scenario: Estabilidad del workbench
- **WHEN** se aplica el ajuste visual del toolbar
- **THEN** el editor principal, el panel de herramientas y la zona de adjuntos
  conservan su estructura actual
- **AND** el tab **Documentos** mantiene su comportamiento sin cambios
