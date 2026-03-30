## ADDED Requirements

### Requirement: GestionCorrespondencia SHALL mostrar exactamente dos acciones principales en su AppToolbar
El sistema SHALL refactorizar la zona de acciones del `AppToolbar` consumido por `GestionCorrespondencia` para mostrar exactamente dos controles visibles: un `AppDropdown` rotulado `Exportar` y un `AppButton` rotulado `Abrir respuesta contextual`.

#### Scenario: Ruta base muestra solo Exportar y Abrir respuesta contextual
- **WHEN** un usuario navega a la ruta base de `GestionCorrespondencia`
- **THEN** la toolbar SHALL mostrar exactamente los controles `Exportar` y `Abrir respuesta contextual` como acciones visibles del modulo

### Requirement: La accion Exportar SHALL usar AppDropdown jerarquico basado en AppButton
El sistema SHALL construir la accion `Exportar` con `AppDropdown` usando `AppButton` como trigger primario, sin exponer directamente `Dropdown` de Ant Design en la pagina consumidora.

#### Scenario: Trigger Exportar abre el menu contextual
- **WHEN** el usuario activa el boton `Exportar`
- **THEN** la vista SHALL abrir un dropdown contextual construido sobre `AppDropdown` y disparado por `AppButton`

#### Scenario: Menu Exportar presenta opciones de Excel y Pdf
- **WHEN** el dropdown de exportacion se despliega
- **THEN** el menu SHALL mostrar las opciones `Exportar en Excel` y `Exportar en Pdf` con su iconografia correspondiente

### Requirement: El dropdown de exportacion SHALL soportar submenu jerarquico por formato
El sistema SHALL modelar `Exportar en Excel` y `Exportar en Pdf` como opciones con submenu, cada una con las acciones hijas `Exportar Todo` y `Exportar Seleccionados`.

#### Scenario: Opcion Exportar en Excel expone submenu
- **WHEN** el usuario inspecciona la opcion `Exportar en Excel`
- **THEN** el dropdown SHALL permitir acceder a `Exportar Todo` y `Exportar Seleccionados` como opciones hijas

#### Scenario: Opcion Exportar en Pdf expone submenu
- **WHEN** el usuario inspecciona la opcion `Exportar en Pdf`
- **THEN** el dropdown SHALL permitir acceder a `Exportar Todo` y `Exportar Seleccionados` como opciones hijas

### Requirement: Abrir respuesta contextual SHALL mantenerse como AppButton con navegacion relativa
El sistema SHALL reconstruir `Abrir respuesta contextual` usando `AppButton`, `EyeFilled` y `useNavigate`, conservando la navegacion relativa hacia `respuesta`.

#### Scenario: Boton abre la subruta contextual del modulo
- **WHEN** el usuario activa `Abrir respuesta contextual`
- **THEN** la pagina SHALL navegar a la subruta relativa `respuesta` sin usar un `Button` nativo de Ant Design

### Requirement: La toolbar consumidora SHALL aplicar estilo enterprise y responsive del ticket
El sistema SHALL aplicar al `AppToolbar` consumido por `GestionCorrespondencia` un tratamiento visual enterprise con `border-radius`, `background`, `box-shadow`, padding y separacion entre acciones, manteniendo una disposicion horizontal en desktop y apilada en mobile.

#### Scenario: Desktop muestra acciones en fila alineadas a la izquierda
- **WHEN** la vista se renderiza en desktop o tablet
- **THEN** las acciones `Exportar` y `Abrir respuesta contextual` SHALL mostrarse en una fila horizontal alineada hacia la izquierda con separacion visible

#### Scenario: Mobile apila acciones y preserva ancho util
- **WHEN** la vista se renderiza en mobile
- **THEN** las acciones SHALL apilarse verticalmente, cada una en su propia linea, ocupando el ancho definido por el contrato responsive del modulo

### Requirement: El refactor SHALL preservar accesibilidad y continuidad del flujo de ruta
El sistema SHALL mantener labels claros, activacion por teclado del dropdown y continuidad del `Drawer` contextual ya existente en `GestionCorrespondenciaRoute`.

#### Scenario: Trigger Exportar conserva nombre accesible
- **WHEN** el usuario navega por teclado o lector de pantalla hasta el control `Exportar`
- **THEN** el trigger SHALL exponer un nombre accesible claro para abrir el menu de exportacion

#### Scenario: Ruta contextual sigue abriendo el Drawer
- **WHEN** el usuario navega a `respuesta` desde la accion principal
- **THEN** la vista principal SHALL permanecer visible y el `Drawer` contextual SHALL seguir funcionando como en la implementacion previa
