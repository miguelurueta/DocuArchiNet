## ADDED Requirements

### Requirement: Boton de retorno solo icono en Gestion Respuesta
El sistema SHALL renderizar el control de regreso en `GestionRespuestaMainTabContent` como boton solo icono usando `LeftOutlined`, sin texto visible.

#### Scenario: Render icon-only del control de retorno
- **WHEN** la vista de gestion de respuesta se renderiza en el tab principal
- **THEN** el control de retorno MUST mostrar solo el icono `LeftOutlined` y MUST NOT mostrar el texto `Volver a la bandeja`

#### Scenario: Accesibilidad del boton icon-only
- **WHEN** el usuario navega con teclado o lector de pantalla sobre el boton de retorno
- **THEN** el control MUST exponer `aria-label="Volver a la bandeja"` y MUST conservar foco visible

### Requirement: Metadata alineada a la derecha del boton de retorno
El sistema SHALL ubicar el bloque de `metadata` del header inmediatamente a la derecha del boton de retorno dentro del mismo contenedor visual.

#### Scenario: Alineacion horizontal en desktop
- **WHEN** la vista se muestra en ancho desktop
- **THEN** `metadata` MUST mostrarse en la misma linea horizontal a la derecha del boton icon-only

#### Scenario: Ajuste responsive del bloque metadata
- **WHEN** la vista se muestra en anchos reducidos (tablet o mobile)
- **THEN** el bloque `metadata` MUST poder envolver contenido sin salir del contenedor ni generar overflow horizontal

### Requirement: Eliminacion visual de headerDescription
El sistema SHALL ocultar los mensajes de `headerDescription` en `GestionRespuestaMainTabContent` para reducir ruido visual del header.

#### Scenario: Header sin descripcion textual
- **WHEN** el header de gestion respuesta se renderiza
- **THEN** la UI MUST no mostrar texto asociado a `headerDescription` ni reservar espacio visual para ese bloque

### Requirement: Variante visual compacta para AppUpload en gestion respuesta
El sistema SHALL presentar `AppUpload` con dimensiones visuales compactas en la vista de gestion respuesta, manteniendo la funcionalidad actual de carga.

#### Scenario: Reduccion de tamaño visual de AppUpload
- **WHEN** el componente `AppUpload` se renderiza en gestion respuesta
- **THEN** el contenedor MUST usar menor alto y menor padding respecto al estado anterior, preservando controles operativos

#### Scenario: Comportamiento funcional intacto con estilo compacto
- **WHEN** el usuario interactua con el `AppUpload` compacto
- **THEN** la carga y acciones del componente MUST conservar el mismo comportamiento funcional existente
