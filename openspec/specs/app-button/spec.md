# app-button Specification

## Purpose
Define el boton estandar reusable del proyecto para desacoplar a las vistas consumidoras de Ant Design y mantener consistencia visual, semantica y de accesibilidad.

## Requirements

### Requirement: AppButton abstrae el boton estandar del proyecto
El sistema SHALL exponer un componente `AppButton` reusable en `src/app/Components/UI` como wrapper tipado sobre Ant Design, de forma que las vistas consumidoras no dependan directamente de `Button` ni `Tooltip` para acciones comunes de UI.

#### Scenario: Vista consume el boton estandar desde la capa UI
- **WHEN** una vista necesita renderizar una accion primaria, secundaria o contextual
- **THEN** la implementacion SHALL poder importar `AppButton` desde la capa UI compartida sin acoplarse a componentes base de Ant Design

### Requirement: AppButton normaliza variantes y tamanos del design system
El sistema SHALL permitir configurar `variant` y `size` mediante una API propia del proyecto, mapeando esos valores a una presentacion visual consistente con la interfaz actual sin exponer directamente la semantica visual nativa de Ant Design.

#### Scenario: Variante primaria con tamano por defecto
- **WHEN** una vista renderiza `AppButton` sin personalizacion adicional de tamano
- **THEN** el componente SHALL aplicar la variante estandar del proyecto y un tamano por defecto coherente con el resto de la UI

#### Scenario: Variantes visuales estandarizadas
- **WHEN** una vista define `variant` como `primary`, `secondary`, `success`, `warning`, `danger`, `ghost` o `link`
- **THEN** el componente SHALL reflejar una semantica visual estable para esa accion y conservar estados `hover`, `focus` y `active`

#### Scenario: Tamanos soportados por contrato
- **WHEN** una vista define `size` como `sm`, `md` o `lg`
- **THEN** el componente SHALL ajustar altura, espaciado e iconografia de forma consistente con el tamano solicitado

### Requirement: AppButton controla estados interactivos y seguridad de accion
El sistema SHALL soportar `loading`, `disabled`, `htmlType` y `fullWidth` como parte del contrato publico, evitando acciones duplicadas y preservando el comportamiento semantico esperado del boton.

#### Scenario: Tipo HTML por defecto
- **WHEN** una vista renderiza `AppButton` sin indicar `htmlType`
- **THEN** el componente SHALL usar `button` como tipo HTML por defecto

#### Scenario: Estado loading bloquea interaccion duplicada
- **WHEN** `AppButton` se renderiza con `loading=true`
- **THEN** el componente SHALL mostrarse deshabilitado visualmente, exponer indicador de carga y MUST NOT ejecutar `onClick`

#### Scenario: Estado disabled bloquea interaccion
- **WHEN** `AppButton` se renderiza con `disabled=true`
- **THEN** el componente MUST NOT ejecutar `onClick` y SHALL reflejar semantica accesible de deshabilitado

#### Scenario: Expansion a ancho completo
- **WHEN** una vista define `fullWidth=true`
- **THEN** el componente SHALL ocupar el ancho horizontal disponible sin alterar su semantica de boton

### Requirement: AppButton define precedencia de iconos e icon-only
El sistema SHALL soportar `leftIcon`, `rightIcon` e `icon` con reglas explicitas de precedencia para evitar composiciones inconsistentes, incluyendo modo `icon-only` cuando se suministra `icon` sin `children`.

#### Scenario: Icon-only con accesibilidad obligatoria
- **WHEN** una vista renderiza `AppButton` con `icon` y sin `children`
- **THEN** el componente SHALL entrar en modo `icon-only`, centrar el icono, conservar el tamano solicitado y requerir un nombre accesible como `aria-label`

#### Scenario: Iconos laterales con texto
- **WHEN** una vista renderiza `AppButton` con `children` y define `leftIcon` o `rightIcon`
- **THEN** el componente SHALL mostrar los iconos junto al texto respetando el orden visual esperado sin duplicar iconografia de forma inconsistente

### Requirement: AppButton conserva accesibilidad y soporte de tooltip
El sistema SHALL mantener accesibilidad de teclado y foco visible, y permitir `tooltip` como abstraccion segura sobre Ant Design incluso cuando el boton este en estado `disabled` o `loading`.

#### Scenario: Tooltip sobre boton deshabilitado o cargando
- **WHEN** una vista define `tooltip` y el boton esta `disabled` o `loading`
- **THEN** el componente SHALL seguir mostrando ayuda contextual sin romper el comportamiento del estado no interactivo

#### Scenario: Foco visible y semantica de boton
- **WHEN** el usuario navega con teclado hacia `AppButton`
- **THEN** el componente SHALL exponer foco visible, semantica correcta de boton y atributos accesibles compatibles con su estado actual
