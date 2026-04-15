## ADDED Requirements

### Requirement: Persistencia temporal de imagenes locales en IndexedDB
El sistema SHALL almacenar en `IndexedDB` las imagenes locales insertadas en `AppEditor`, usando una infraestructura reusable desacoplada de React y del backend.

#### Scenario: Guardado de imagen local
- **WHEN** el usuario selecciona un archivo de imagen local desde la toolbar del editor
- **THEN** el sistema SHALL crear un registro temporal en `IndexedDB` con id unico, nombre, tipo, tamano, blob y fecha de creacion

#### Scenario: Recuperacion por id
- **WHEN** una imagen temporal existe en `IndexedDB`
- **THEN** la infraestructura SHALL permitir recuperarla por su id para rehidratarla en la sesion

### Requirement: Modelo temporal de imagen con identificador estable
El sistema SHALL representar cada imagen local temporal con un identificador unico y estable con el formato `img_local_<uuid>`.

#### Scenario: Generacion de id local
- **WHEN** se inserta una nueva imagen local
- **THEN** el sistema SHALL generar un `localImageId` unico con el prefijo `img_local_`

#### Scenario: Metadatos minimos
- **WHEN** el registro temporal se persiste en `IndexedDB`
- **THEN** el sistema SHALL conservar `id`, `fileName`, `contentType`, `size`, `blob`, `createdAt` y metadatos opcionales de scope

### Requirement: Insercion de imagen local usando Blob URL valida
El sistema SHALL renderizar imagenes locales en `AppEditor` mediante `blob:` URLs validas generadas desde el navegador, sin usar esquemas custom en el atributo `src`.

#### Scenario: Insercion en el documento
- **WHEN** una imagen local ha sido guardada en `IndexedDB`
- **THEN** el editor SHALL insertar un nodo `<img>` con `src="blob:..."`, `data-local-image-id` y `data-source="local"`

#### Scenario: Compatibilidad con HTML serializado
- **WHEN** el contenido del editor se serializa a HTML
- **THEN** la imagen local SHALL preservar `data-local-image-id`, `data-source`, `data-width` y `data-align` cuando existan

### Requirement: Gestion explicita de Object URLs
El sistema SHALL crear y liberar `Object URLs` de forma controlada para evitar fugas de memoria durante la edicion.

#### Scenario: Creacion de Object URL
- **WHEN** una imagen temporal se recupera para renderizarse en el editor
- **THEN** el sistema SHALL usar `URL.createObjectURL(blob)` para producir un `src` valido

#### Scenario: Liberacion de Object URL
- **WHEN** una imagen local se elimina, se reemplaza o el componente editor se desmonta
- **THEN** el sistema SHALL llamar `URL.revokeObjectURL(...)` para liberar la referencia temporal

### Requirement: Limpieza por alcance de borrador o sesion
El sistema SHALL exponer operaciones para limpiar imagenes temporales por alcance de borrador o sesion sin afectar otras imagenes temporales no relacionadas.

#### Scenario: Limpieza por borrador
- **WHEN** existe un `documentDraftId` asociado a imagenes temporales
- **THEN** la infraestructura SHALL permitir eliminar todas las imagenes de ese borrador

#### Scenario: Limpieza por sesion
- **WHEN** existe un `sessionId` asociado a imagenes temporales
- **THEN** la infraestructura SHALL permitir eliminar todas las imagenes de esa sesion

### Requirement: Compatibilidad con funcionalidades actuales del editor
El sistema SHALL introducir `IndexedDB` para imagenes locales sin romper la insercion por URL remota, el resize persistido, la alineacion horizontal ni el modo controlled/uncontrolled del editor.

#### Scenario: Insercion por URL remota
- **WHEN** el usuario inserta una imagen por URL remota
- **THEN** el flujo SHALL seguir funcionando sin depender de `IndexedDB`

#### Scenario: Imagen local con resize y alineacion
- **WHEN** una imagen local temporal tiene `data-width` y `data-align`
- **THEN** el editor SHALL preservar esos atributos durante render y serializacion

### Requirement: Rehidratacion basica en sesion
El sistema SHALL permitir rehidratar imagenes locales temporales dentro de la sesion actual mientras los registros sigan existiendo en `IndexedDB`.

#### Scenario: Reapertura del contenido en la misma sesion
- **WHEN** el editor vuelve a cargar HTML que contiene `data-local-image-id`
- **THEN** el sistema SHALL intentar resolver el blob correspondiente desde `IndexedDB` y regenerar un `blob:` URL para renderizar la imagen

#### Scenario: Imagen temporal inexistente
- **WHEN** el HTML referencia un `data-local-image-id` que ya no existe en `IndexedDB`
- **THEN** el sistema SHALL fallar de forma segura sin romper la carga del resto del documento
