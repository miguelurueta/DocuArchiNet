## ADDED Requirements

### Requirement: Eliminar el control manual de tema en AppEditor
El sistema SHALL remover el toggle visible de cambio de tema `dark/light` de la toolbar de `AppEditor`, manteniendo compatibilidad con el theming global de la aplicacion.

#### Scenario: Toolbar sin toggle de tema
- **WHEN** el usuario renderiza `AppEditor`
- **THEN** la toolbar SHALL no mostrar ningun boton de cambio manual entre tema claro u oscuro

#### Scenario: Compatibilidad con tema global
- **WHEN** `AppEditor` recibe tema desde el contexto o desde props ya existentes
- **THEN** el editor SHALL seguir respetando ese tema sin requerir control manual en la UI

### Requirement: Compatibilidad API sin breaking changes silenciosos
El sistema SHALL evitar romper la API publica existente de `AppEditor` al remover la affordance visual del toggle.

#### Scenario: Props de tema externas
- **WHEN** existen props relacionadas con tema usadas por consumidores externos
- **THEN** el sistema SHALL mantenerlas o deprecarlas sin romper integraciones existentes

#### Scenario: Sin logica muerta visible
- **WHEN** el toggle ya no se usa visualmente
- **THEN** el sistema SHALL limpiar handlers, estado y wiring interno que ya no aporten valor real

### Requirement: Toolbar responsive compacta en mobile y tablet
El sistema SHALL renderizar una toolbar mas compacta y legible en mobile y tablet, reduciendo su impacto visual sin perder claridad funcional.

#### Scenario: Mobile compacta
- **WHEN** el editor se renderiza en anchos equivalentes a `667px`, `896px` o `932px`
- **THEN** la toolbar SHALL reducir spacing, padding y composicion para ocupar menos espacio sin desbordarse

#### Scenario: Tablet compacta
- **WHEN** el editor se renderiza en `1024px`
- **THEN** la toolbar SHALL mantener una disposicion compacta y estable sin robar area util excesiva al contenido editable

### Requirement: Priorizacion visible de acciones principales
El sistema SHALL mantener visibles las acciones principales del toolbar en resoluciones reducidas y permitir colapsar acciones secundarias en mecanismos compactos.

#### Scenario: Acciones principales visibles
- **WHEN** el usuario navega la toolbar en mobile o tablet
- **THEN** acciones como `bold`, `italic`, `undo` y `redo` SHALL permanecer visibles

#### Scenario: Acciones secundarias compactadas
- **WHEN** el espacio horizontal no sea suficiente
- **THEN** acciones secundarias SHALL poder agruparse o colapsarse en dropdowns o affordances equivalentes sin perder comprension

### Requirement: Usabilidad tactil y estabilidad visual
El sistema SHALL mantener usabilidad tactil, foco del editor y estabilidad visual durante la interaccion con la toolbar responsive.

#### Scenario: Interaccion sin perdida de foco
- **WHEN** el usuario interactua con botones, dropdowns o popovers del toolbar
- **THEN** el editor SHALL conservar foco funcional y no recrear la instancia de Tiptap

#### Scenario: Sin overflow ni filas excesivas
- **WHEN** el toolbar se adapta a mobile o tablet
- **THEN** SHALL evitar overflow horizontal y multiples filas excesivas que degraden la experiencia

### Requirement: Sin regresion en desktop
El sistema SHALL preservar la experiencia actual del toolbar en desktop mientras aplica optimizaciones a mobile y tablet.

#### Scenario: Desktop estable
- **WHEN** `AppEditor` se renderiza en desktop
- **THEN** la toolbar SHALL mantener una experiencia completa y no degradada respecto al comportamiento actual
