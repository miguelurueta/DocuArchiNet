<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## ADDED Requirements

### Requirement: RQ-01 Fundación paralela sin efectos

Para D-01, el sistema SHALL incorporar contratos paralelos por tipo técnico, módulo y caso de uso sin modificar `workflow/Webworkflow.aspx`, `workflow/Webworkflow.aspx.vb`, `ClassWorkflow.Terminar_Tarea_Workflow` ni `ClassWorkflow.Cambia_Estado`.

#### Scenario: Flujo legacy preservado

- **WHEN** se compila la solucion con los archivos de DOC-9
- **THEN** el flujo WebForms existente permanece como unico camino que puede terminar una tarea.

### Requirement: RQ-02 Límites tipados entre capas

Para D-02, Domain y Application SHALL estar libres de dependencias de Page, Session, GridView, UpdatePanel y ModalPopupExtender; las fronteras nuevas SHALL usar modelos, interfaces y DTOs explicitamente tipados. Los modelos y puertos de Domain del caso de uso terminar SHALL residir en `Modelo/Workflow/Terminar/`; los DTOs en `DTOs/Workflow/Terminar/` y los servicios, proveedores y validadores de Application en `Services/Workflow/Terminar/`, todas rutas desde la raíz del repositorio.

#### Scenario: Inspección de clases nuevas

- **WHEN** se revisan las clases VB de `Modelo/Workflow/Terminar/`, `DTOs/Workflow/Terminar/` y `Services/Workflow/Terminar/`
- **THEN** no se encuentran dependencias WebForms en Domain o Application y los datos visibles se representan con DTOs sin HTML, SQL, credenciales ni excepciones internas.

### Requirement: RQ-03 Frontera legacy sin ejecución en DOC-9

Para D-03, `WorkflowLegacyExecutorAdapter` SHALL ser la unica frontera nueva reservada para el motor legacy y SHALL devolver `WORKFLOW_MODERN_EXECUTION_PENDING` durante esta fase.

#### Scenario: Composición accidental

- **WHEN** una capa posterior crea `WorkflowLegacyExecutorAdapter` durante DOC-9
- **THEN** no invoca `Terminar_Tarea_Workflow` ni `Cambia_Estado` y no se modifica una tarea existente.

### Requirement: RQ-04 Habilitación moderna fail-closed

Para D-04, `IWorkflowModernFeatureGate` SHALL evaluar una configuracion de servidor y un `ContextoModuloWorkflow` validado; la ausencia, invalidez o falta de autorizacion SHALL devolver estado `inactivo`.

#### Scenario: Configuración ausente o perfil excluido

- **WHEN** `WorkflowCentroTrabajoModernActive` no existe, es invalido, el contexto no coincide o existe una exclusion
- **THEN** `ConfiguracionWorkflowModernFeatureGate` devuelve `WORKFLOW_MODERN_INACTIVE` o `WORKFLOW_MODERN_EXCLUDED` sin habilitar por defecto.

### Requirement: RQ-05 Datos reutilizables y persistencia acotada

Para D-05, `Infrastructure/Shared/Data` SHALL exponer contratos reutilizables de conexión, ejecución parametrizada, transacción, paginación y auditoría técnica; `ModuleConnectionFactory` SHALL recibir el contexto común `ContextoModulo` y no conocer símbolos, errores o modelos Workflow. Workflow SHALL especializar el contexto, mantener puertos de repositorio propios y reservar sus implementaciones para `Infrastructure/Repositories/Workflow/`, sin `GenericRepository`.

#### Scenario: Repositorio Workflow posterior

- **WHEN** una fase posterior implemente persistencia de Workflow
- **THEN** recibe `ContextoModuloWorkflow`, usa la infraestructura compartida mediante su contexto base `ContextoModulo`, emplea parámetros tipados y no lee Session, no devuelve DataSet ni HTML.

### Requirement: RQ-06 Evidencia diferenciada de QA manual

Para D-06, la entrega SHALL registrar los comandos automatizados ejecutados y SHALL mantener abierta la QA manual hasta contar con ambiente, pasos y resultado reproducibles.

#### Scenario: Cierre técnico

- **WHEN** se solicita validar DOC-9
- **THEN** la compilacion y la verificacion focal tienen evidencia verificable y la ausencia de QA manual bloquea el cierre sin declararla realizada.
