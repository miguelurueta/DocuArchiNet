## ADDED Requirements

### Requirement: Contrato declarativo de estructura modular
El sistema SHALL mantener un catálogo versionado de rutas y límites arquitectónicos para DTOs, servicios Application, modelos e interfaces de dominio, componentes compartidos, infraestructura por módulo y repositorios de negocio. Todo cambio nuevo creado con gobierno OPSXJ SHALL declarar módulo y caso de uso cuando aplique, y SHALL guardar el manifiesto arquitectónico asociado dentro de su cambio OpenSpec.

#### Scenario: Cambio modular declarado
- **WHEN** se inicia un cambio de un módulo y caso de uso con `opsxj:orchestrate:new`
- **THEN** el cambio registra las rutas esperadas para sus componentes y los límites de capa aplicables en su manifiesto arquitectónico

#### Scenario: Entrada arquitectónica incompleta
- **WHEN** un cambio modular no declara el módulo, el caso de uso o una ruta requerida
- **THEN** OPSXJ falla antes de publicar artefactos de gobierno incompletos e identifica el dato faltante

### Requirement: Revisión coherente de arquitectura y trazabilidad
El sistema SHALL revisar que prompt, refinement, design, specification, tareas y manifiesto arquitectónico expresen las mismas rutas, responsabilidades y fronteras legacy. La revisión SHALL rechazar rutas ambiguas, componentes en capas incompatibles y uso no justificado de elementos compartidos.

#### Scenario: Ruta incompatible en documentación o tareas
- **WHEN** un artefacto declara un repositorio, servicio o adaptador en una ruta distinta de la convención o del manifiesto
- **THEN** la revisión técnica informa un hallazgo bloqueante con la ruta esperada y la fuente en conflicto

#### Scenario: Componente compartido con dependencia de módulo
- **WHEN** un cambio declara un componente de `Shared` que conoce símbolos, modelos o reglas de un módulo concreto
- **THEN** la revisión técnica falla y exige reubicarlo en la infraestructura o dominio del módulo correspondiente

### Requirement: Validación estructural antes de cierre
El sistema SHALL validar los archivos nuevos o modificados de un cambio contra su manifiesto arquitectónico antes de que `opsxj:validate`, publicación o archivo puedan informar estado satisfactorio. La validación SHALL verificar rutas, límites de dependencia y fronteras legacy declaradas.

#### Scenario: Implementación conforme
- **WHEN** los archivos afectados coinciden con el manifiesto, respetan las restricciones de capa y no introducen llamadas legacy fuera de la frontera declarada
- **THEN** la validación estructural informa PASS como parte del resultado de gobierno OPSXJ

#### Scenario: Excepción explícita y vigente
- **WHEN** una ruta o dependencia necesita apartarse temporalmente de la convención y existe una excepción con motivo, responsable, archivos afectados y fecha de expiración vigente
- **THEN** la validación registra la excepción y permite únicamente el alcance autorizado

#### Scenario: Cambio histórico sin manifiesto arquitectónico
- **WHEN** se valida un cambio histórico que no contiene manifiesto arquitectónico
- **THEN** OPSXJ conserva la compatibilidad histórica y no agrega un bloqueo retroactivo
