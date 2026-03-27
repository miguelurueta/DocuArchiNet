## ADDED Requirements

### Requirement: AppToolbar abstrae la barra de acciones y contexto del proyecto
El sistema SHALL exponer un componente `AppToolbar` reusable en `src/app/Components/UI` para encapsular encabezado contextual, acciones y contenido auxiliar sin acoplar a las vistas consumidoras a componentes base de una libreria UI especifica.

#### Scenario: Vista consume la toolbar desde la capa UI compartida
- **WHEN** un modulo necesita renderizar un encabezado con acciones contextuales
- **THEN** la implementacion SHALL poder importar `AppToolbar` desde la capa UI compartida sin depender directamente de primitives de Ant Design o MUI

### Requirement: AppToolbar compone regiones semanticas tipadas
El sistema SHALL permitir configurar titulo, descripcion o subtitulo, breadcrumbs opcionales, acciones primarias, acciones secundarias y contenido auxiliar mediante una API tipada del proyecto, conservando una jerarquia visual y semantica consistente.

#### Scenario: Toolbar con encabezado y acciones principales
- **WHEN** una vista renderiza `AppToolbar` con titulo, descripcion y una o mas acciones principales
- **THEN** el componente SHALL presentar el contexto principal en una region de encabezado y las acciones en una region separada sin perder legibilidad ni orden visual

#### Scenario: Toolbar con breadcrumbs y contenido auxiliar
- **WHEN** una vista define breadcrumbs y contenido auxiliar adicional
- **THEN** `AppToolbar` SHALL renderizar esas regiones como partes opcionales del layout sin obligar a la vista a reconstruir la estructura base del componente

### Requirement: AppToolbar adapta la disposicion a desktop y mobile
El sistema SHALL responder a cambios de ancho disponible reorganizando sus regiones para preservar jerarquia, legibilidad y acceso a las acciones relevantes tanto en desktop como en pantallas reducidas.

#### Scenario: Disposicion horizontal en espacio amplio
- **WHEN** `AppToolbar` se renderiza con ancho suficiente para layout de escritorio
- **THEN** el componente SHALL mostrar el contexto del encabezado y las acciones en regiones paralelas alineadas de forma consistente

#### Scenario: Reflujo vertical en pantallas estrechas
- **WHEN** `AppToolbar` se renderiza en un viewport reducido o con espacio horizontal insuficiente
- **THEN** el componente SHALL reorganizar sus regiones en una pila vertical priorizando el encabezado y la accion principal antes de las acciones secundarias

### Requirement: AppToolbar gestiona overflow de acciones sin degradar la experiencia
El sistema SHALL ofrecer una estrategia estable para manejar acciones que exceden el espacio visible, preservando acceso a las acciones secundarias sin producir wraps desordenados o solapamiento visual.

#### Scenario: Acciones secundarias exceden el espacio disponible
- **WHEN** la toolbar recibe mas acciones de las que pueden mostrarse de forma legible en el ancho actual
- **THEN** `AppToolbar` SHALL mantener visible la jerarquia principal y mover las acciones secundarias excedentes a un patron de overflow controlado

#### Scenario: Accion prioritaria permanece accesible
- **WHEN** existe una accion primaria definida y el espacio horizontal es limitado
- **THEN** `AppToolbar` MUST mantener esa accion accesible sin depender exclusivamente del overflow secundario

### Requirement: AppToolbar conserva accesibilidad y soporte de iconografia
El sistema SHALL mantener semantica accesible para su encabezado y sus acciones, incluyendo navegacion por teclado, nombres accesibles para acciones icon-only y compatibilidad con breadcrumbs o menus auxiliares.

#### Scenario: Accion icon-only requiere nombre accesible
- **WHEN** una vista suministra una accion de toolbar representada solo por icono
- **THEN** el componente MUST requerir o propagar un nombre accesible que permita identificar la accion mediante tecnologias asistivas

#### Scenario: Navegacion por teclado en toolbar compuesta
- **WHEN** el usuario interactua con `AppToolbar` mediante teclado
- **THEN** el componente SHALL preservar un orden de foco coherente entre encabezado, breadcrumbs y acciones renderizadas
