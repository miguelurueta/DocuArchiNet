## MODIFIED Requirements

### Requirement: El modulo SHALL usar un shell de navegacion persistente controlado por routing
El sistema SHALL implementar `GestionCorrespondenciaRoute` como un shell de navegacion tipo Gmail para el modulo, manteniendo visible la vista principal y renderizando las vistas secundarias dentro de una region persistente del layout gobernada por la URL y no por un overlay modal acoplado a estado local.

#### Scenario: Ruta secundaria abre la region persistente
- **WHEN** el usuario navega a una subruta secundaria configurada del modulo
- **THEN** el sistema SHALL conservar visible la pagina principal y SHALL renderizar la vista secundaria dentro de una region persistente del shell del modulo

#### Scenario: Cierre navega a la ruta base
- **WHEN** el usuario cierra la vista secundaria desde la UI del modulo
- **THEN** el sistema SHALL navegar a la ruta base del modulo y SHALL ocultar la region secundaria sin desmontar el contexto principal

#### Scenario: Deep link a la vista secundaria
- **WHEN** el usuario entra directamente a la URL de la subruta secundaria
- **THEN** el sistema SHALL resolver la ruta, mostrar la vista principal y renderizar la vista secundaria dentro del shell persistente del modulo

#### Scenario: Comportamiento responsivo del shell
- **WHEN** el modulo se renderiza en pantallas reducidas
- **THEN** el sistema SHALL preservar la navegacion gobernada por routing y SHALL adaptar la region secundaria sin romper la experiencia principal del listado

### Requirement: GestionRespuesta SHALL renderizarse como vista secundaria desacoplada dentro del shell del modulo
El sistema SHALL implementar `GestionRespuesta` como una pagina secundaria preparada para mostrarse dentro de la region persistente del shell de `GestionCorrespondencia`, con estructura visual profesional y sin conocimiento directo del mecanismo de routing o de logica de negocio.

#### Scenario: Render de la vista secundaria
- **WHEN** la ruta secundaria del modulo esta activa
- **THEN** el sistema SHALL mostrar el contenido de `GestionRespuesta` dentro de la region secundaria del shell con titulo, descripcion y placeholders visibles

#### Scenario: Vista secundaria sin control de navegacion
- **WHEN** `GestionRespuesta` se renderiza como contenido del shell
- **THEN** la pagina SHALL depender de la capa routes para apertura, cierre y navegacion, y no SHALL gestionar directamente el flujo de rutas

### Requirement: El modulo SHALL incluir pruebas de comportamiento del flujo estructural
El sistema SHALL cubrir con Vitest y Testing Library el render del shell, la presencia de la pagina principal y la integracion entre rutas anidadas y la region secundaria persistente, enfocandose en comportamiento observable del modulo.

#### Scenario: Cobertura del layout y pagina principal
- **WHEN** se ejecutan las pruebas del modulo
- **THEN** el conjunto de tests SHALL verificar que el layout y la pagina principal renderizan sin errores con el contenido base esperado

#### Scenario: Cobertura del shell gobernado por rutas
- **WHEN** se ejecutan las pruebas sobre la ruta secundaria del modulo
- **THEN** el conjunto de tests SHALL verificar que la region secundaria se abre por routing y que `GestionRespuesta` se renderiza sin reemplazar la pagina principal

### Requirement: El modulo SHALL documentar su arquitectura inicial
El sistema SHALL incluir un `README.md` dentro del modulo que documente el proposito de Gestion Correspondencia, su estructura de carpetas, la responsabilidad de cada capa y el flujo de shell persistente gobernado por routing previsto para futuras ampliaciones.

#### Scenario: Documentacion disponible para futuras iteraciones
- **WHEN** un desarrollador consulte el modulo por primera vez
- **THEN** el `README.md` SHALL describir el flujo de navegacion, las capas del modulo y la forma recomendada de escalar la implementacion del shell
