## ADDED Requirements

### Requirement: AppIconActionButton unifica las acciones iconográficas compactas
El sistema SHALL exponer `AppIconActionButton` como componente reusable basado en `AppButton` para representar acciones compactas icon-only en toolbar, celdas y triggers compatibles con dropdown.

#### Scenario: Render icon-only con accesibilidad obligatoria
- **WHEN** un consumidor renderiza `AppIconActionButton`
- **THEN** el sistema muestra el icono como contenido principal
- **AND** exige `aria-label`
- **AND** no requiere `children`

#### Scenario: Estados loading y disabled se preservan desde la base UI
- **WHEN** `AppIconActionButton` recibe `loading` o `disabled`
- **THEN** el sistema refleja esos estados reutilizando el comportamiento consistente de `AppButton`

#### Scenario: Tooltip opcional no reemplaza accesibilidad
- **WHEN** el consumidor provee `tooltip`
- **THEN** el sistema muestra el contenido auxiliar correspondiente
- **AND** mantiene `aria-label` como requisito obligatorio de accesibilidad

### Requirement: AppDropdown acepta triggers basados en la misma familia visual
El sistema SHALL permitir que `AppDropdown` reciba un trigger basado en `AppIconActionButton` sin romper sus integraciones actuales ni forzar un único tipo de trigger.

#### Scenario: Trigger iconográfico compatible con dropdown
- **WHEN** un flujo usa un `AppIconActionButton` como trigger de `AppDropdown`
- **THEN** el dropdown conserva su comportamiento actual de apertura y acciones
- **AND** mantiene compatibilidad con triggers preexistentes

### Requirement: AppTableQueryWrapper compone controles de consulta y tabla sin mezclar datos
El sistema SHALL exponer `AppTableQueryWrapper` como un wrapper reusable que compone header controls, tabla y paginación externa usando `AppTableQueryState` y `children`, sin ejecutar queries ni mantener un query state paralelo.

#### Scenario: Wrapper renderiza controles y tabla en un contenedor único
- **WHEN** un consumidor renderiza `AppTableQueryWrapper`
- **THEN** el sistema muestra controles de búsqueda, refresh opcional, acciones adicionales, el contenido `children` y los controles de paginación externa dentro del mismo contenedor visual

#### Scenario: Wrapper emite patches simples de query state
- **WHEN** el usuario interactúa con búsqueda, navegación prev/next o cambio de page size
- **THEN** el wrapper emite `onQueryChange` con un patch parcial de `AppTableQueryState`
- **AND** no aplica por sí mismo merge ni reglas de reset

#### Scenario: Refresh es una acción externa
- **WHEN** el usuario activa refresh y existe `onRefresh`
- **THEN** el wrapper invoca `onRefresh`
- **AND** no altera el `AppTableQueryState` por sí mismo

### Requirement: La capa visual reusable no se acopla a una pantalla específica
El sistema SHALL mantener `AppIconActionButton` y `AppTableQueryWrapper` desacoplados de `GestionCorrespondencia` y de cualquier módulo particular.

#### Scenario: Header actions genéricas
- **WHEN** un consumidor provee `headerActions`
- **THEN** el wrapper renderiza ese contenido adicional sin introducir naming o lógica de dominio

#### Scenario: Children preserva el renderer final de tabla
- **WHEN** el consumidor pasa `AppTable` u otro contenido compatible como `children`
- **THEN** el wrapper preserva ese renderer final sin reemplazar el grid ni crear uno paralelo
