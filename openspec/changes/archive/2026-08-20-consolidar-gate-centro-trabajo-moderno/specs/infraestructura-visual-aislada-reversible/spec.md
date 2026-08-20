## MODIFIED Requirements

### Requirement: Experiencia moderna oficial para contextos válidos
El sistema SHALL entregar la capa visual moderna DOC-2 y registrar los scripts y bootstraps ASMX modernos para todo contexto Workflow válido. La política de servidor MUST NOT leer ni requerir `WorkflowCentroTrabajoModernActive`, modo oficial, usuarios, grupos, exclusiones, metadatos de piloto ni rollback para decidir la disponibilidad moderna. El sistema MUST NOT leer ni requerir `WorkflowCentroTrabajoModernEnabled` ni `WorkflowCentroTrabajoModernPilotProfiles`.

#### Scenario: Configuración de gate inactiva con contexto válido
- **WHEN** las claves de gate están inactivas, vacías o contienen un alcance de piloto, pero el contexto Workflow es válido
- **THEN** `Webworkflow.aspx` entrega recursos, viewport, clase raíz, bootstraps y scripts ASMX modernos, y habilita los controles modernos de continuar y enviar a grupo.

#### Scenario: Contexto inválido
- **WHEN** no existe un contexto Workflow válido
- **THEN** el servidor no enlaza operaciones ASMX y conserva las validaciones seguras de sesión y contexto, sin exponer una ruta legacy alternativa.

### Requirement: Viewport responsive aislado de la autorización operativa
El sistema SHALL declarar un `HtmlMeta` estático y tipado de viewport tanto en el host superior `Defaul/WebFormInicioDocuarchiGestion.aspx` como en el acceso directo `workflow/Webworkflow.aspx`. Ambos SHALL emitir el viewport independientemente de los gates históricos y del gate operativo. La presencia del viewport o de recursos visuales no SHALL conceder autorización ni alterar controles Web Forms.

#### Scenario: Inicio en dispositivo angosto
- **WHEN** cualquier usuario abre la página inicial en un viewport de 375 px
- **THEN** el navegador usa 375 px como ancho CSS para el host, sin que esa declaración active recursos, clases ni acciones modernas de Workflow.

#### Scenario: Workflow en dispositivo angosto
- **WHEN** cualquier contexto abre Workflow en un viewport de 375 px
- **THEN** el navegador usa 375 px como ancho CSS y aplica el reflujo scoped de DOC-2; la lista documental y el visor no se presentan como una versión de escritorio reducida.

#### Scenario: Gate operativo inactivo
- **WHEN** el gate operativo está inactivo o no tiene alcance configurado
- **THEN** el host superior y `Webworkflow.aspx` conservan viewport, recursos, clase raíz y operaciones modernas para todo contexto Workflow válido.

#### Scenario: Navegación del host en ancho intermedio
- **WHEN** un contexto habilitado carga `workflow/Webworkflow.aspx` dentro del iframe del shell con un ancho de 1199 px o menor
- **THEN** el shell usa el toggle legacy para iniciar el sidebar cerrado y conservar ancho útil para la lista documental y sus acciones; en otro módulo, contexto no habilitado o por encima de ese ancho conserva el umbral baseline de 992 px.

### Requirement: Exclusión global de flujos legacy visibles
El sistema SHALL presentar `Continuar flujo` y `Enviar a grupo` exclusivamente como controles modernos habilitados para todo contexto Workflow válido, sin depender de un gate de despliegue. El sistema SHALL no entregar postbacks ni diálogos legacy para esas acciones. La retirada de las claves históricas SHALL conservar el viewport y la presentación moderna sin convertirlos en una señal de autorización.

#### Scenario: Claves de gate inactivas
- **WHEN** el gate operativo permanece inactivo en configuración local
- **THEN** Workflow registra JavaScript de preview, búsqueda, confirmación y ejecución modernos; `Continuar flujo` y `Enviar a grupo` permanecen habilitados y no pueden activar `ImageButtonterminar`, `ImageButtonEnviaActividad` ni un diálogo legacy.

#### Scenario: Reglas de negocio no cumplidas
- **WHEN** la sesión, permisos, requisitos, destino o concurrencia de una acción no son válidos
- **THEN** el contrato ASMX moderno devuelve su bloqueo funcional correspondiente, sin ejecutar el flujo legacy.

### Requirement: Evidencia de despliegue y reversión
Antes de aprobar el despliegue, el cambio SHALL contar con evidencia por SHA de la experiencia oficial para un contexto Workflow válido y de la página inicial en viewports 1366, 1024, 768 y 375 px. La reversión SHALL consistir en volver a la versión anterior del paquete; no dependerá de un gate de alcance.

#### Scenario: QA manual autorizada
- **WHEN** se dispone de ambiente WebForms, acceso TLS, cuentas piloto/no piloto y datos Workflow controlados
- **THEN** se ejecutan los recorridos modernos de preview, ejecución, reversión de versión, caché, navegación interna y los viewports definidos; un resultado pendiente no se declara aprobado.

### Requirement: Entrega versionada de presentación
El sistema SHALL incrementar la versión de URL de los activos `workflow-centro-trabajo-moderno.css` y `centro-trabajo-visual.js` cuando cambie la lógica que organiza o estiliza los controles modernos. La versión nueva SHALL estar presente en el markup de `Webworkflow.aspx` para invalidar recursos almacenados por el navegador.

#### Scenario: Cambio de ubicación de una acción moderna
- **WHEN** el adaptador visual cambia los selectores o clases que ubican una acción de Workflow
- **THEN** el navegador solicita el adaptador y el CSS con una versión distinta de la previamente desplegada, y no conserva una posición ni estilo de la implementación anterior.
