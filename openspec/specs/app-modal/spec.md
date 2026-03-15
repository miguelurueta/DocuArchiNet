## ADDED Requirements

### Requirement: AppModal abstrae el modal estandar del proyecto
El sistema SHALL exponer un componente `AppModal` reusable en `src/app/Components/UI` como wrapper tipado sobre el componente de dialogo base de la libreria UI, de forma que las vistas consumidoras no dependan directamente del modal nativo del proveedor para overlays, confirmaciones o dialogos comunes.

#### Scenario: Vista consume el modal estandar desde la capa UI
- **WHEN** una vista necesita mostrar un dialogo de confirmacion o contenido modal
- **THEN** la implementacion SHALL poder importar `AppModal` desde la capa UI compartida sin acoplarse al proveedor UI subyacente

### Requirement: AppModal estandariza estructura y estados de dialogo
El sistema SHALL permitir configurar apertura, cierre, titulo, contenido y acciones mediante una API propia del proyecto, preservando consistencia visual con la interfaz actual sin exponer directamente la semantica del proveedor UI.

#### Scenario: Modal cerrado por defecto
- **WHEN** una vista renderiza `AppModal` con la propiedad de apertura desactivada
- **THEN** el componente SHALL mantenerse oculto sin dejar contenido interactivo accesible en pantalla

#### Scenario: Modal visible con titulo y contenido
- **WHEN** una vista activa `AppModal` con titulo y contenido definidos
- **THEN** el componente SHALL mostrar una estructura consistente con cabecera, cuerpo y contenedor overlay alineados al design system del proyecto

#### Scenario: Modal deshabilita acciones mientras procesa
- **WHEN** una vista marca una accion modal en estado de carga o bloqueo
- **THEN** el componente SHALL reflejar el estado visual correspondiente y evitar interacciones duplicadas en las acciones afectadas

### Requirement: AppModal soporta acciones primarias y secundarias reutilizables
El sistema SHALL soportar acciones de cierre, confirmacion y cancelacion mediante props explicitas y composicion segura con contenido adicional cuando el flujo lo requiera.

#### Scenario: Confirmacion explicita del usuario
- **WHEN** una vista provee una accion primaria y el usuario la ejecuta desde `AppModal`
- **THEN** el componente SHALL invocar el callback asociado sin alterar el contrato de apertura/cierre definido externamente

#### Scenario: Cancelacion o cierre secundario
- **WHEN** el usuario acciona el boton secundario o el mecanismo de cierre habilitado
- **THEN** el componente SHALL invocar el callback de cancelacion o cierre configurado por la vista

### Requirement: AppModal conserva accesibilidad en overlays y dialogos
El sistema SHALL mantener atributos de dialogo accesibles, foco controlado, cierre por teclado cuando aplique y asociacion correcta entre titulo, descripcion y contenido del modal.

#### Scenario: Dialogo accesible con titulo asociado
- **WHEN** una vista renderiza `AppModal` con titulo visible
- **THEN** el componente SHALL asociar el titulo al dialogo mediante atributos accesibles compatibles con lectores de pantalla

#### Scenario: Cierre por teclado cuando el flujo lo permite
- **WHEN** el usuario interactua con `Escape` y la configuracion del modal permite cierre por teclado
- **THEN** el componente SHALL ejecutar el flujo de cierre definido por la vista sin romper el manejo de foco

#### Scenario: Foco inicial dentro del dialogo
- **WHEN** `AppModal` se abre
- **THEN** el componente SHALL mover el foco a un elemento interactivo relevante del dialogo o al contenedor principal del modal
