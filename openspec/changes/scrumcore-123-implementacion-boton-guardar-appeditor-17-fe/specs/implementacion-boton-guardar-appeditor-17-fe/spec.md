## ADDED Requirements

### Requirement: Boton Guardar con dirty state visible
El sistema SHALL exponer una accion visible de `Guardar` asociada al uso de `AppEditor`, fuera de la toolbar de formato del editor.

#### Scenario: Boton visible en el shell del editor
- **WHEN** una pantalla consume `AppEditor` con soporte de guardado simulado
- **THEN** el sistema SHALL renderizar un boton `Guardar` en `headerActions` o en el contenedor inmediato del editor

#### Scenario: Guardar no forma parte de herramientas de formato
- **WHEN** el usuario inspecciona la toolbar interna del editor
- **THEN** la accion `Guardar` SHALL permanecer fuera de esa toolbar y no mezclarse con comandos de formato

### Requirement: Estado visual gris cuando no hay cambios pendientes
El sistema SHALL mostrar el boton `Guardar` en estado gris cuando el contenido actual coincida con el ultimo estado guardado.

#### Scenario: Estado inicial sin cambios
- **WHEN** el contenedor carga el contenido inicial del editor y no existen modificaciones pendientes
- **THEN** el boton `Guardar` SHALL renderizarse en gris

#### Scenario: Estado sincronizado tras guardado simulado
- **WHEN** el usuario ejecuta el guardado simulado y el baseline se actualiza al valor actual
- **THEN** el boton `Guardar` SHALL volver a gris

### Requirement: Estado visual negro cuando existen cambios pendientes
El sistema SHALL mostrar el boton `Guardar` en negro cuando el contenido actual difiera del ultimo estado guardado.

#### Scenario: Dirty tras editar contenido
- **WHEN** el usuario modifica el contenido del editor respecto al baseline guardado
- **THEN** el boton `Guardar` SHALL pasar a negro

#### Scenario: Dirty tras re-editar contenido guardado
- **WHEN** el usuario modifica nuevamente el contenido despues de un guardado simulado
- **THEN** el boton `Guardar` SHALL volver a negro

### Requirement: Dirty state basado en comparacion real de contenido
El sistema SHALL derivar el estado `dirty` a partir de la comparacion entre `currentValue` y `savedValue` normalizados, sin usar heuristicas simplistas.

#### Scenario: Dirty por diferencia de contenido real
- **WHEN** `normalize(currentValue) !== normalize(savedValue)`
- **THEN** el sistema SHALL considerar el estado como dirty

#### Scenario: No dirty por equivalencia normalizada
- **WHEN** `normalize(currentValue) === normalize(savedValue)`
- **THEN** el sistema SHALL considerar el estado como no dirty aunque el HTML bruto difiera superficialmente

### Requirement: Normalizacion consistente de HTML vacio
El sistema SHALL normalizar representaciones equivalentes de HTML vacio para evitar falsos positivos en el dirty state.

#### Scenario: HTML vacio equivalente
- **WHEN** el contenido actual o guardado sea `""`, `<p></p>` o `<p><br></p>`
- **THEN** el sistema SHALL tratarlos como equivalentes

#### Scenario: Normalizacion compartida reutilizable
- **WHEN** el contenedor del editor necesite calcular dirty state o resetear baseline
- **THEN** el sistema SHALL usar una funcion compartida de normalizacion en lugar de logica duplicada

### Requirement: Guardado simulado sin persistencia real
El sistema SHALL permitir guardado simulado actualizando solo el baseline local sin llamar backend, API ni base de datos.

#### Scenario: Guardado local actualiza baseline
- **WHEN** el usuario hace clic en `Guardar` en esta fase
- **THEN** el sistema SHALL actualizar `savedValue` con el contenido actual y resetear el dirty state

#### Scenario: Sin efectos externos de persistencia
- **WHEN** el usuario ejecuta el guardado simulado
- **THEN** el sistema SHALL evitar llamadas a backend, API o base de datos

### Requirement: Sincronizacion correcta con cambios externos
El sistema SHALL resetear el dirty state cuando el contenido controlado cambie externamente y pase a ser la nueva fuente de verdad del editor.

#### Scenario: Carga externa de contenido
- **WHEN** el consumidor actualiza `value` desde una carga externa o refresh
- **THEN** el sistema SHALL actualizar `savedValue` y dejar el estado como no dirty

#### Scenario: Sin falsos positivos tras rehidratacion
- **WHEN** el editor se inicializa o recibe contenido externo equivalente al baseline esperado
- **THEN** el boton `Guardar` SHALL permanecer gris

### Requirement: Preparacion para futura integracion con backend
El sistema SHALL dejar una estructura clara para futura persistencia real sin acoplar `AppEditor` directamente a backend.

#### Scenario: Persistencia desacoplada del editor
- **WHEN** se implemente backend en una fase futura
- **THEN** el sistema SHALL permitir conectar un contrato tipo `saveDraft` sin mover la responsabilidad de persistencia dentro de `AppEditor`

#### Scenario: AppEditor agnostico a persistencia
- **WHEN** se use `AppEditor` dentro de este cambio
- **THEN** el componente SHALL seguir siendo agnostico a la persistencia real y limitarse a exponer contenido y slots visuales
