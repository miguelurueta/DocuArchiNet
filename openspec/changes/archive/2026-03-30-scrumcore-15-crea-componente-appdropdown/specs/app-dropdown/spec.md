## ADDED Requirements

### Requirement: AppDropdown abstrae el menu desplegable reusable del proyecto
El sistema SHALL exponer un componente `AppDropdown` reusable en `src/app/Components/UI` para encapsular menus desplegables y acciones contextuales sin acoplar a las vistas consumidoras al contrato crudo del proveedor UI.

#### Scenario: Vista importa AppDropdown desde la capa compartida
- **WHEN** un modulo necesita renderizar un menu desplegable para acciones o seleccion contextual
- **THEN** la implementacion SHALL poder consumir `AppDropdown` desde la capa UI compartida sin depender directamente de `Dropdown` de Ant Design

### Requirement: AppDropdown acepta trigger e items mediante API tipada del proyecto
El sistema SHALL permitir configurar un trigger reusable y una coleccion tipada de items con `key`, `label` y comportamiento de seleccion, preservando una API estable del proyecto para menus simples de acciones.

#### Scenario: Dropdown muestra acciones configuradas
- **WHEN** una vista renderiza `AppDropdown` con un trigger y una lista de items validos
- **THEN** el componente SHALL presentar esas acciones dentro del overlay desplegable respetando el orden definido por el consumidor

#### Scenario: Seleccion de item dispara el comportamiento asociado
- **WHEN** el usuario selecciona un item habilitado del dropdown
- **THEN** `AppDropdown` SHALL ejecutar el callback asociado o la accion configurada para ese item sin exigir al consumidor manipular eventos internos del vendor UI

### Requirement: AppDropdown conserva estados disabled y control de apertura
El sistema SHALL soportar estado deshabilitado del trigger y un mecanismo consistente para apertura no controlada o controlada cuando el consumidor necesite observar o gobernar el estado visible del overlay.

#### Scenario: Trigger deshabilitado no abre el menu
- **WHEN** una vista renderiza `AppDropdown` en estado `disabled`
- **THEN** el componente MUST impedir la apertura del overlay y bloquear la ejecucion de acciones derivadas del trigger

#### Scenario: Consumidor observa cambios de apertura
- **WHEN** la vista suministra propiedades para observar o controlar la apertura del dropdown
- **THEN** `AppDropdown` SHALL reflejar ese estado de forma consistente y notificar cambios mediante el callback definido por el contrato del proyecto

### Requirement: AppDropdown soporta metadata visual y semantica por item
El sistema SHALL permitir que cada item declare metadata visual y de estado como iconografia, riesgo visual o deshabilitacion sin obligar al consumidor a reconstruir manualmente la estructura del menu.

#### Scenario: Item con icono o estilo de peligro
- **WHEN** una vista define un item con icono o marcado como accion destructiva
- **THEN** `AppDropdown` SHALL renderizar esa metadata de forma consistente dentro del menu reutilizable

#### Scenario: Item deshabilitado permanece visible sin ejecutarse
- **WHEN** el dropdown contiene un item marcado como `disabled`
- **THEN** el componente MUST mostrarlo como no interactivo y evitar que su accion se ejecute al intentar seleccionarlo

### Requirement: AppDropdown mantiene accesibilidad base para trigger y menu
El sistema SHALL preservar accesibilidad por teclado y nombres accesibles en el trigger y las acciones del dropdown, apoyandose en el proveedor UI como detalle interno pero manteniendo el contrato observable del proyecto.

#### Scenario: Trigger icon-only requiere nombre accesible
- **WHEN** una vista usa un trigger representado solo por iconografia para abrir `AppDropdown`
- **THEN** el componente MUST exigir o propagar un nombre accesible que permita identificar la accion mediante tecnologias asistivas

#### Scenario: Usuario interactua con el dropdown por teclado
- **WHEN** el usuario navega hasta el trigger y abre el menu mediante teclado
- **THEN** `AppDropdown` SHALL mantener un flujo de interaccion accesible para recorrer y activar items visibles del overlay
