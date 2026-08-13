## ADDED Requirements

### Requirement: Perfiles declarativos de arquitectura legacy
El tooling `opsxj` SHALL permitir declarar un perfil de arquitectura al iniciar un cambio nuevo mediante `opsxj:new` o su alias `opsxj:orchestrate:new`. El perfil `enterprise-legacy-modernization` SHALL estar disponible para modernizaciones de capacidades legacy.

#### Scenario: Inicio con perfil de modernización

- **WHEN** se crea un cambio nuevo indicando el perfil `enterprise-legacy-modernization`
- **THEN** el cambio se crea con ese perfil declarado y el comando informa el perfil aplicado

#### Scenario: Perfil desconocido

- **WHEN** se solicita un perfil no soportado
- **THEN** el comando falla antes de crear artefactos, ramas o cambios externos y comunica los perfiles válidos

### Requirement: Perfiles tecnológicos para revisión sin falsos positivos
El tooling `opsxj` SHALL mantener separado el perfil de arquitectura del perfil tecnológico usado para revisar prompts. `opsxj:new` y `opsxj:orchestrate:new` SHALL aceptar opcionalmente `--tech-profile` y persistirlo de forma aditiva en los artefactos generados. `opsxj:technical-review` SHALL respetar un perfil explícito o detectar de manera conservadora `legacy-webforms-vb`, `tooling-node`, `frontend-react-ts` o `generic`, para aplicar únicamente reglas compatibles con la tecnología declarada.

#### Scenario: Prompt Web Forms/VB

- **WHEN** se revisa un prompt declarado o detectado como `legacy-webforms-vb`
- **THEN** el revisor conserva las reglas generales y las de Web Forms pertinentes, pero no informa requisitos propios de React, TypeScript, hooks, keys de listas ni re-renders

#### Scenario: Inicio orquestado con perfil tecnológico

- **WHEN** se crea un cambio mediante `opsxj:orchestrate:new --tech-profile legacy-webforms-vb`
- **THEN** propuesta, diseño, especificación, tareas y manifiesto registran el perfil tecnológico para que las revisiones posteriores tengan contexto verificable

### Requirement: Propagación de política de modernización enterprise
El perfil `enterprise-legacy-modernization` SHALL sembrar en los artefactos iniciales reglas verificables para separar presentación, aplicación, dominio e infraestructura; reutilizar infraestructura de datos sin mezclar dominios; y encapsular cada capacidad legacy mediante un Gateway o Adapter tipado por dominio.

#### Scenario: Artefactos de un cambio modernizador

- **WHEN** se genera un cambio con el perfil de modernización enterprise
- **THEN** su propuesta, diseño, especificación y tareas describen la separación de responsabilidades, la compatibilidad gradual, la validación servidor, la infraestructura reutilizable y el límite de acceso al comportamiento legacy

#### Scenario: Capacidad legacy específica

- **WHEN** un ticket moderniza una capacidad de cualquier módulo legacy
- **THEN** los artefactos exigen un límite tipado para esa capacidad y prohíben que código nuevo de presentación o aplicación invoque directamente el comportamiento legacy, el estado de sesión, controles de interfaz o cambios de estado

### Requirement: Gobierno validable y compatible de perfiles
El sistema SHALL persistir el perfil aplicado y sus requisitos de gobierno en el cambio, y `opsxj:validate` SHALL informar los incumplimientos del perfil antes de permitir su cierre. Para todo cambio con manifiesto de gobierno, `opsxj:validate` SHALL aplicar una validación bloqueante de la documentación técnica: existencia de cada archivo requerido, secciones obligatorias por tipo documental, contenido sustantivo mínimo, metadatos de identidad coherentes y ausencia de plantillas, `TBD` o checklists abiertos. Los cambios creados sin perfil y los cambios históricos sin manifiesto SHALL conservar el comportamiento de validación compatible vigente.

#### Scenario: Validación de cambio con perfil

- **WHEN** un cambio con perfil de modernización tiene requisitos de arquitectura, documentación o evidencia pendientes
- **THEN** `opsxj:validate` informa estado FAIL e identifica los requisitos pendientes sin modificar Jira, GitHub ni código de negocio

#### Scenario: Documentación técnica incompleta

- **WHEN** un cambio con manifiesto tiene un documento técnico faltante, vacío, sin una sección requerida, con `TBD`, con instrucciones de plantilla, con checklist abierto o con metadatos que no corresponden al ticket/cambio
- **THEN** `opsxj:validate` informa estado FAIL, identifica el archivo y la regla incumplida, y bloquea el cierre hasta que la documentación sea corregida

#### Scenario: Cambio anterior sin perfil

- **WHEN** se valida un cambio creado antes de la adopción del perfil o que no solicitó perfil
- **THEN** la validación no agrega bloqueos retroactivos por la política de modernización enterprise
