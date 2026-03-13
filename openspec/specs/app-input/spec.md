# app-input Specification

## Purpose
Define el input estandar reusable del proyecto para desacoplar a las vistas consumidoras del proveedor UI y mantener consistencia visual, semantica y de accesibilidad en formularios y filtros.

## Requirements

### Requirement: AppInput abstrae el input estandar del proyecto
El sistema SHALL exponer un componente `AppInput` reusable en `src/app/Components/UI` como wrapper tipado sobre el control de entrada base de la libreria UI, de forma que las vistas consumidoras no dependan directamente del input nativo del proveedor para campos de texto comunes.

#### Scenario: Vista consume el input estandar desde la capa UI
- **WHEN** una vista necesita capturar texto corto en un formulario o filtro
- **THEN** la implementacion SHALL poder importar `AppInput` desde la capa UI compartida sin acoplarse al proveedor UI subyacente

### Requirement: AppInput normaliza variantes visuales y estados de campo
El sistema SHALL permitir configurar variantes visuales y estados de campo mediante una API propia del proyecto, preservando consistencia visual con la interfaz actual sin exponer directamente la semantica del proveedor UI.

#### Scenario: Variante por defecto de campo de texto
- **WHEN** una vista renderiza `AppInput` sin personalizacion adicional de variante
- **THEN** el componente SHALL aplicar la presentacion estandar del proyecto para entradas de texto

#### Scenario: Estado de error visible y consistente
- **WHEN** una vista marca `AppInput` en estado de error
- **THEN** el componente SHALL reflejar feedback visual consistente y exponer semantica accesible asociada al error

#### Scenario: Estado deshabilitado
- **WHEN** una vista renderiza `AppInput` con `disabled=true`
- **THEN** el componente SHALL impedir interaccion del usuario y conservar semantica accesible de campo deshabilitado

### Requirement: AppInput soporta contrato de captura y ayuda contextual
El sistema SHALL soportar `value`, `defaultValue`, `onChange`, `placeholder`, `label`, `helperText` y atributos de accesibilidad necesarios para integracion segura con formularios del proyecto.

#### Scenario: Campo controlado por valor externo
- **WHEN** una vista provee `value` y `onChange` a `AppInput`
- **THEN** el componente SHALL propagar cambios de texto sin perder sincronizacion con el estado externo

#### Scenario: Campo con ayuda contextual
- **WHEN** una vista provee `helperText` o texto de apoyo a `AppInput`
- **THEN** el componente SHALL mostrar esa ayuda y vincularla con el control mediante atributos accesibles cuando corresponda

#### Scenario: Placeholder descriptivo
- **WHEN** una vista define `placeholder` en `AppInput`
- **THEN** el componente SHALL mostrar el texto de ayuda inicial sin alterar la semantica del label asociado

### Requirement: AppInput conserva accesibilidad y composicion reutilizable
El sistema SHALL mantener foco visible, asociacion correcta entre label y control, soporte de teclado y composicion segura con clases CSS Modules y clases externas.

#### Scenario: Asociacion entre label y control
- **WHEN** una vista renderiza `AppInput` con label visible
- **THEN** el componente SHALL asociar correctamente el label con el control para lectores de pantalla y navegacion por teclado

#### Scenario: Composicion de estilos
- **WHEN** una vista provee `className` adicional a `AppInput`
- **THEN** el componente SHALL combinar estilos propios del componente con clases externas sin perder su presentacion base
