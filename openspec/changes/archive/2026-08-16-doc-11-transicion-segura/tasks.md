<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->

## 1. Refinamiento trazable

- [x] 1.1 Inspeccionar el ASMX moderno, gate de sesión, servicio, repositorios y llamadas legacy de RUTA/FLUJO; registrar los límites comprobados. Origen: D-01, RQ-01. Cobertura: D-02, D-03, D-04, D-05, D-06, RQ-02, RQ-03, RQ-04.
- [x] 1.2 Reemplazar el diseño, especificación y tareas genéricos por decisiones, requisitos, escenarios y riesgos verificables; aprobar refinement. Origen: D-01, RQ-01. Cobertura: D-02, D-03, D-04, D-05, D-06, D-07, D-08, D-09, D-10, RQ-02, RQ-03, RQ-04, RQ-05, RQ-06, RQ-07.

## 2. Contratos y validación de dominio

- [x] 2.1 Incorporar el modelo tipado de destino autorizado y los puertos específicos para resolver destino, requisitos, auditoría y guard de concurrencia; no reutilizar el DTO de preview como orden de ejecución. Origen: D-03, RQ-02. Cobertura: D-04, D-07, D-08, RQ-05, RQ-06.
- [x] 2.2 Endurecer ValidadorTransicionTarea: idTarea e idConector positivos, token obligatorio y códigos funcionales estables; mapear errores sin detalle interno. Origen: D-03, RQ-02. Cobertura: D-08, RQ-06.
- [x] 2.3 Añadir el guard MySQL por tarea y token, con adquisición, releída dentro del guard y liberación garantizada; no escribir estados ni abrir transacción de negocio. Origen: D-07, RQ-05.

## 3. Infraestructura de revalidación

- [x] 3.1 Implementar resolución de ejecución RUTA contra grupo, ruta, actividad origen, conector, actividad real destino y estado de correo usando IModuleConnectionFactory. Origen: D-03, RQ-02. Cobertura: D-04, RQ-03.
- [x] 3.2 Implementar resolución de ejecución FLUJO contra flujo, nodo/actividad fuente, usuario/grupo fuente, actividad real destino y todos los identificadores que Terminar_Tarea_Workflow requiere. Origen: D-03, RQ-02. Cobertura: D-04, RQ-03.
- [x] 3.3 Integrar las validaciones previas existentes de respuesta, aprobación y autorización en un límite de infraestructura sin exponer Session ni texto legacy al servicio; conservar firma, expediente, copia y balanceo en el motor legacy. Origen: D-05, RQ-03. Cobertura: D-06, RQ-04.
- [x] 3.4 Implementar auditoría segura y normalización de resultado para éxito, rechazo, advertencia posterior e indisponibilidad. Origen: D-08, RQ-06.

## 4. Application, adaptador y ASMX

- [x] 4.1 Cambiar ServicioTransicionTarea y EjecutorTransicionTarea para gate, validación, tarea/token, guard, relectura, destino autorizado, requisitos, ejecución y auditoría en ese orden. Origen: D-01, RQ-01. Cobertura: D-03, D-06, D-07, D-08, RQ-02, RQ-04, RQ-05, RQ-06.
- [x] 4.2 Implementar WorkflowLegacyExecutorAdapter como único llamador de Terminar_Tarea_Workflow, usando Page=Nothing y actualización de interfaz desactivada, pero conservando PRETERMINARACTIVIAD, TERMINARACTIVIDAD, correo y Cambia_Estado interno. Origen: D-05, RQ-03. Cobertura: D-06, RQ-04.
- [x] 4.3 Extender WorkflowPreviewSessionContextGate con inicialización exclusiva para ejecución, validación de permisos y limpieza segura del contexto Workflow incompleto. Origen: D-02, RQ-01.
- [x] 4.4 Agregar EjecutarEnvioTarea al mismo WebServiceWorkflowModern, componer dependencias de ejecución y convertir fallas no controladas a ResultadoTransicionDto seguro. Origen: D-01, RQ-01. Cobertura: D-08, RQ-06.
- [x] 4.5 Registrar los archivos nuevos en GestionDocumental-Docuarchi.net.vbproj y comprobar que no cambia Webworkflow.aspx, Webworkflow.aspx.vb ni ClassWorkflow.vb. Origen: D-05, RQ-03. Cobertura: D-10, RQ-07.

## 5. Pruebas y evidencia técnica

- [x] 5.1 Agregar pruebas focales para validador, mapeo de ResultadoTransicionDto, token/conector alterado, gate/sesión, bloqueo de requisitos y normalización de errores. Origen: D-02, RQ-01. Cobertura: D-03, D-06, D-08, RQ-02, RQ-04, RQ-06.
- [x] 5.2 Agregar pruebas de repositorio para el mapeo de destino RUTA y FLUJO, incluyendo la diferencia entre actividad real y actividad de flujo. Origen: D-04, RQ-02, RQ-03.
- [x] 5.3 Agregar pruebas del guard para doble clic, dos solicitudes y liberación ante error; verificar que a lo sumo una invocación llegue al adaptador. Origen: D-07, RQ-05.
- [x] 5.4 Actualizar las utilidades E2E y el runbook para autenticar, ejecutar la llamada, comprobar el resultado y restaurar el gate; dejar la ejecución mutante condicionada a autorización y registros descartables. Origen: D-09, RQ-07. Cobertura: D-10.
- [x] 5.5 Ejecutar build de .NET Framework, pruebas focales, QA manual de RUTA/FLUJO/bloqueos y concurrencia; registrar comando, resultado, entorno y limitaciones reales. Origen: D-09, RQ-07.

## 6. Documentación, validación y piloto

- [x] 6.1 Crear el paquete Doc/Actualizacion/workflow/Terminar/03-ejecucion-segura con índice, arquitectura, contrato, seguridad, pruebas, inventario y diagramas Mermaid de componentes, secuencia, concurrencia y estados. Origen: D-01, RQ-03. Cobertura: D-04, D-05, D-07, D-08, D-09, RQ-05, RQ-06, RQ-07.
- [x] 6.2 Documentar criterio de piloto y rollback: gate desactivado por defecto, lista explícita de usuarios/grupos, métricas/evidencias y desactivación sin migración. Origen: D-10, RQ-01, RQ-07.
- [x] 6.3 Ejecutar opsxj:refine, OpenSpec validate y revisión estática que confirme que solo el adaptador nuevo llama Terminar_Tarea_Workflow. Origen: D-05, RQ-03. Cobertura: D-09, RQ-07.
