## ADDED Requirements

### Requirement: Activación servidor y piloto cerrado
El sistema SHALL activar la capa DOC-2 solo cuando `WorkflowCentroTrabajoModernEnabled` sea verdadero y el login de gestión de sesión (`GA_LOGINUSUARIOGESTION`) coincida exactamente con un perfil configurado en `WorkflowCentroTrabajoModernPilotProfiles`.

#### Scenario: Modo apagado o usuario fuera de piloto
- **WHEN** el flag maestro es falso, falta la lista de perfiles o el login no pertenece a ella
- **THEN** `Webworkflow.aspx` no entrega los recursos DOC-2 ni la clase raíz moderna.

#### Scenario: Usuario piloto
- **WHEN** el flag maestro es verdadero y el login de gestión coincide con un perfil configurado en servidor
- **THEN** el contenedor `#div_content_general_wf` recibe la clase raíz y las subcapas calculadas por el servidor.

### Requirement: Capas reversibles y dependencia explícita
El sistema SHALL emitir `ctw-layer-layout`, `ctw-layer-actions`, `ctw-layer-documents` y `ctw-layer-a11y` desde `WorkflowCentroTrabajoModernLayers`, con `layout` como dependencia de las demás.

#### Scenario: Rollback parcial
- **WHEN** un piloto tiene el modo moderno activo y se retira una subcapa de la configuración
- **THEN** solo los estilos de esa subcapa dejan de aplicar sin retirar la hoja, cambiar eventos o modificar datos.

#### Scenario: Layout deshabilitado
- **WHEN** la configuración no contiene `layout`
- **THEN** no se emite ninguna subcapa y los componentes scoped quedan inertes.

### Requirement: Recursos aislados y ordenados
El sistema SHALL cargar `workflow-centro-trabajo-moderno.css` y `centro-trabajo-visual.js` después de `Webworkflow.js` y de los recursos legacy relevantes, con versión de caché explícita.

#### Scenario: Preservación de la línea base visual previa
- **WHEN** se entrega `Webworkflow.aspx`
- **THEN** los recursos manuales aprobados antes de DOC-2 conservan su ruta de carga y la capa DOC-2 se entrega después de ellos solo para un piloto autorizado.

### Requirement: Adaptador sin efecto funcional
El sistema SHALL limitar el adaptador DOC-2 a añadir clases de presentación dentro del contenedor moderno, incluso tras una actualización parcial de ASP.NET AJAX.

#### Scenario: Integridad de WebForms
- **WHEN** se ejecuta un postback o una actualización de `UpdatePanel`
- **THEN** el adaptador no mueve controles, no reemplaza IDs, atributos de negocio o foco, no habilita acciones ocultas por el servidor y no bloquea los scripts legacy.

### Requirement: Contrato CSS reutilizable
El sistema SHALL definir tokens y componentes `.ctw-btn`, `.ctw-icon-btn`, `.ctw-menu`, `.ctw-menu__panel`, `.ctw-badge`, `.ctw-action-bar` y `.ctw-document-bar` únicamente bajo `.workflow-centro-trabajo-moderno`.

#### Scenario: Clase agregada manualmente desde cliente
- **WHEN** un usuario fuera del piloto agrega una clase manualmente desde el navegador
- **THEN** no obtiene recursos ni autorización adicionales porque la decisión y la entrega fueron resueltas por servidor.

### Requirement: Politica Frontend AppResponses
La política de `AppResponses<T>` SHALL aplicar solo si un ticket crea o modifica consumidores de ese envelope.

#### Scenario: Ticket WebForms sin consumidor AppResponses
- **WHEN** DOC-2 no crea servicios, hooks ni componentes que consuman `AppResponses<T>`
- **THEN** no se crea un helper frontend ni parser local fuera de alcance.
