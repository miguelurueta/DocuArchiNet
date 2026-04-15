## ADDED Requirements

### Requirement: Alineacion horizontal persistida para imagen
El sistema SHALL permitir que las imagenes dentro de `AppEditor` se alineen horizontalmente a la izquierda, centro o derecha, persistiendo esa alineacion en el contenido serializado.

#### Scenario: Imagen alineada a la izquierda
- **WHEN** el usuario aplica alineacion `left` sobre una imagen seleccionada
- **THEN** el HTML SHALL persistir `data-align="left"`

#### Scenario: Imagen alineada al centro o derecha
- **WHEN** el usuario aplica alineacion `center` o `right`
- **THEN** el HTML SHALL persistir respectivamente `data-align="center"` o `data-align="right"`

### Requirement: Rehidratacion de alineacion desde HTML
El sistema SHALL restaurar correctamente la alineacion de imagen al rehidratar contenido que incluya el atributo `data-align`.

#### Scenario: Rehidratacion de imagen alineada
- **WHEN** `AppEditor` carga HTML con `<img data-align="center" />`
- **THEN** la imagen SHALL renderizarse centrada dentro del editor

#### Scenario: Compatibilidad hacia atras
- **WHEN** el editor carga una imagen sin `data-align`
- **THEN** la imagen SHALL comportarse como alineada a la izquierda por defecto

### Requirement: Comando setImageAlign sobre nodo imagen
El sistema SHALL exponer un comando `setImageAlign` para actualizar la alineacion de una imagen activa o seleccionada sin romper foco ni seleccion.

#### Scenario: Imagen seleccionada como nodo
- **WHEN** existe una imagen seleccionada o activa en el editor
- **THEN** `setImageAlign('left' | 'center' | 'right')` SHALL actualizar el atributo `align` del nodo imagen

#### Scenario: Sin imagen activa
- **WHEN** el usuario ejecuta `setImageAlign` sin una imagen valida activa
- **THEN** el comando SHALL no modificar el documento

### Requirement: Controles visibles de alineacion solo para imagen
El sistema SHALL mostrar controles de alineacion horizontal en la toolbar solo cuando exista una imagen activa o seleccionada.

#### Scenario: Imagen activa
- **WHEN** `editor.isActive('image')` o la seleccion corresponde a un nodo imagen
- **THEN** la toolbar SHALL mostrar controles `left`, `center` y `right`

#### Scenario: Sin imagen activa
- **WHEN** no existe imagen activa ni seleccionada
- **THEN** la toolbar SHALL ocultar los controles de alineacion de imagen

### Requirement: Compatibilidad con resize persistido
El sistema SHALL conservar la compatibilidad con el resize de imagen y con atributos persistidos existentes como `data-width`.

#### Scenario: Imagen con width y align
- **WHEN** una imagen tiene `data-width` y `data-align`
- **THEN** el render y la serializacion SHALL preservar ambos atributos

#### Scenario: Sin regresion de resize
- **WHEN** el usuario redimensiona una imagen y luego cambia su alineacion
- **THEN** el ancho persistido SHALL mantenerse intacto

### Requirement: Render visual via atributo persistido
El sistema SHALL resolver la representacion visual de la alineacion mediante CSS basado en `data-align`, sin depender de estilos inline ni clases externas.

#### Scenario: Render via CSS
- **WHEN** una imagen contiene `data-align`
- **THEN** su posicion horizontal SHALL derivarse de reglas CSS asociadas a ese atributo

#### Scenario: Sin estilos inline de alineacion
- **WHEN** la imagen se serializa a HTML
- **THEN** la alineacion SHALL no representarse mediante estilos inline
