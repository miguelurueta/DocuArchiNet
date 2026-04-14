## ADDED Requirements

### Requirement: AppEditor reusable desacoplado del dominio
El sistema SHALL exponer un componente `AppEditor` reusable en `src/app/Components/UI/AppEditor` para encapsular la superficie principal de edicion sin acoplarla a reglas de negocio ni a modulos consumidores especificos.

#### Scenario: Consumo desde un modulo de negocio
- **WHEN** una vista necesita una superficie principal de composicion o edicion enriquecida
- **THEN** la implementacion SHALL poder importar `AppEditor` desde la capa UI compartida sin depender de `GestionRespuesta` ni de contratos del dominio

#### Scenario: Encabezado contextual reusable
- **WHEN** una vista provee `title` y opcionalmente `description` o `headerActions`
- **THEN** `AppEditor` SHALL renderizar un encabezado estable con esos elementos sin dejar espacios visuales incorrectos cuando `description` no exista

### Requirement: Layout dominante con scroll interno controlado
El sistema SHALL mantener a `AppEditor` como superficie principal del workbench con scroll interno en su area editable para convivir con layouts master-detail y paneles laterales responsivos.

#### Scenario: Scroll dentro de la superficie del editor
- **WHEN** el contenido del editor excede el alto disponible del contenedor padre
- **THEN** el scroll SHALL ocurrir dentro de `EditorSurface` y no en el body del modulo ni en el tab completo

#### Scenario: Adaptacion a workbench responsive
- **WHEN** el layout cambia entre desktop, tablet y mobile
- **THEN** `AppEditor` SHALL conservar padding, borde, jerarquia visual y altura flexible con `min-height: 0` sin depender de anchos fijos

### Requirement: Core basado en Tiptap con arquitectura en capas
El sistema SHALL implementar la base de edicion enriquecida usando Tiptap MIT a traves de una separacion `domain`, `application`, `infrastructure` y `presentation`, evitando uso directo de Tiptap en la capa UI.

#### Scenario: Hook desacoplado de la presentacion
- **WHEN** `AppEditor` necesita inicializar y operar la instancia del editor
- **THEN** la UI SHALL consumir un hook `useAppEditor` y la configuracion de Tiptap SHALL permanecer encapsulada en `infrastructure`

#### Scenario: Sin uso directo de Tiptap en presentacion
- **WHEN** se revisa la implementacion de `presentation/AppEditor.tsx`
- **THEN** no existe integracion directa con APIs de Tiptap desde esa capa

### Requirement: Contrato controlado y no controlado sin perder estado
El sistema SHALL soportar uso controlado mediante `value` y `onChange`, y uso no controlado con estado interno, sincronizando cambios externos sin perder cursor ni romper `undo/redo`.

#### Scenario: Sincronizacion controlada
- **WHEN** el contenedor actualiza externamente `value`
- **THEN** `AppEditor` SHALL reflejar el contenido actualizado y mantener una experiencia de edicion estable

#### Scenario: Modo no controlado
- **WHEN** una vista renderiza `AppEditor` sin `value`
- **THEN** el componente SHALL administrar su estado interno y emitir cambios con una API consistente

### Requirement: Toolbar y capacidades base de edicion enriquecida
El sistema SHALL exponer un toolbar reusable para las operaciones base del editor enriquecido, alineado al comportamiento esperado del editor de referencia definido para este ticket.

#### Scenario: Formato y estructura de texto
- **WHEN** el usuario interactua con el toolbar
- **THEN** el editor SHALL soportar `bold`, `italic`, `underline`, listas `bullet`, listas `ordered`, `task list` y seleccion de headings

#### Scenario: Acciones avanzadas de edicion
- **WHEN** el usuario necesita modificar la estructura o navegacion del contenido
- **THEN** el editor SHALL soportar alineacion izquierda, centro, derecha y justificada, `undo`, `redo`, insercion y edicion de enlaces, e insercion de imagenes

### Requirement: Accesibilidad, estados y tipado estricto
El sistema SHALL mantener tipado estricto y soportar `placeholder`, `disabled`, `readOnly`, `label`, `helperText`, `error`, `className` y `aria-label`, preservando accesibilidad y composicion segura.

#### Scenario: Estado de solo lectura o deshabilitado
- **WHEN** una vista renderiza `AppEditor` con `disabled=true` o `readOnly=true`
- **THEN** el componente SHALL impedir edicion segun corresponda y conservar feedback visual y semantica accesible coherentes

#### Scenario: Soporte de ayuda y semantica accesible
- **WHEN** una vista provee `label`, `helperText`, `error` o `aria-label`
- **THEN** `AppEditor` SHALL asociar correctamente esos elementos al area editable y mantener foco visible y navegacion por teclado
