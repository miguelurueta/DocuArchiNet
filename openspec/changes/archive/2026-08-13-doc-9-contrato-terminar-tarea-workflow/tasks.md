<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## 1. Refinamiento y arquitectura

- [x] 1.1 Consolidar alcance, exclusiones y frontera legacy de la fundacion. Origen: D-01, RQ-01
- [x] 1.2 Definir contratos de capas, DTOs y limites sin dependencias WebForms. Origen: D-02, RQ-02
- [x] 1.3 Definir la habilitacion fail-closed y la infraestructura reutilizable. Origen: D-04, RQ-04
- [x] 1.4 Definir `Domain/Shared` e `Infrastructure/Shared/Data` reutilizables y los puertos exclusivos de Workflow. Origen: D-05, RQ-05

## 2. Implementación de la fundación

- [x] 2.1 Organizar modelos e interfaces de Domain por `Modelo/Workflow/Terminar/`, DTOs por `DTOs/Workflow/Terminar/` y servicios, proveedores y validadores de Application por `Services/Workflow/Terminar/`. Origen: D-02, RQ-02
- [x] 2.2 Implementar `Infrastructure/Workflow/Terminar/WorkflowLegacyExecutorAdapter` inerte y sin llamadas al motor legacy. Origen: D-03, RQ-03
- [x] 2.3 Implementar `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate` con comportamiento fail-closed. Origen: D-04, RQ-04
- [x] 2.4 Incorporar `ContextoModulo` y contratos e implementaciones iniciales en `Infrastructure/Shared/Data` sin repositorio generico ni acoplamiento Workflow. Origen: D-05, RQ-05
- [x] 2.5 Registrar `Infrastructure/Repositories/Workflow/` como contrato futuro sin SQL de negocio. Origen: D-05, RQ-05

## 3. Evidencia

- [x] 3.1 Agregar `tools/validation/Verify-Doc9Foundation.ps1` para gate, adapter y limites de capas. Origen: D-06, RQ-06
- [x] 3.2 Ejecutar compilacion MSBuild y la verificacion focal; registrar evidencia unit OPSXJ. Origen: D-06, RQ-06
- [x] 3.3 Ejecutar QA manual del flujo WebForms vigente y registrar ambiente, pasos y resultado. Origen: D-01, RQ-06

## 4. Documentación y cierre

- [x] 4.1 Documentar arquitectura, contratos, seguridad, flujo y evidencia en el paquete tecnico DOC-9. Origen: D-02, RQ-02
- [x] 4.2 Validar OpenSpec estricto y mantener la trazabilidad decision → requisito → tarea. Origen: D-06, RQ-06
