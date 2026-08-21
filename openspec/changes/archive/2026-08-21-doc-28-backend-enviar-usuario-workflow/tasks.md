<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## 1. Refinamiento aprobado

- [x] 1.1 [M] Consolidar límites, decisiones y riesgos del corte backend. Área/archivos: `refinement.md`, exploración de `TerminarUsuario` y clases Workflow existentes. Origen: D-07, RQ-06. Verificación: matriz D/R sin marcadores y exclusiones de UI, gate, conector y reasignación explícitas.
- [x] 1.2 [M] Reemplazar el diseño y la especificación genéricos por contratos y escenarios observables. Área/archivos: `design.md`, `specs/backend-enviar-usuario-workflow/spec.md`. Origen: D-01, RQ-01. Verificación: cada D-01…D-07 aparece en diseño y especificación con criterio comprobable.

## 2. Contratos y lectura

- [x] 2.1 [M] Añadir modelos, DTOs, códigos públicos y puertos exclusivos de Enviar a usuario. Área/archivos: `Modelo/Workflow/Terminar`, `DTOs/Workflow/Terminar`. Origen: D-01, RQ-01. Verificación: búsqueda estática confirma ausencia de `IdConector` en los contratos nuevos.
- [x] 2.2 [S] Calcular `PuedeCambioUsuario` desde permisos legacy con denegación fail-closed. Área/archivos: `webservice/WorkflowPreviewSessionContextGate.vb`, `Modelo/Workflow/Terminar/WorkflowModernModels.vb`. Origen: D-02, RQ-02. Verificación: prueba de contexto acepta únicamente el índice 18 con valor `1`.
- [x] 2.3 [M] Implementar la consulta de destinos usuario–actividad autorizados con filtro parametrizado, cursor, orden y límite. Área/archivos: `Infrastructure/Repositories/Workflow/MySqlEnvioUsuarioRepository.vb`. Origen: D-03, RQ-03. Verificación: pruebas de repositorio inspeccionan parámetros, `SELECT`, límite y cursor.
- [x] 2.4 [M] Implementar validador y servicio de preview exclusivo sin feature gate ni escrituras. Área/archivos: `Services/Workflow/Terminar/ValidadorEnvioUsuarioTarea.vb`, `Services/Workflow/Terminar/ServicioEnvioUsuarioTarea.vb`. Origen: D-03, RQ-03. Verificación: pruebas de servicio comprueban resultado paginado y cero llamadas a auditoría o ejecutor.
- [x] 2.5 [S] Publicar `PreviewEnviarUsuario` y sus respuestas seguras en el ASMX moderno. Área/archivos: `webservice/WebServiceWorkflowModern.asmx.vb`. Origen: D-01, RQ-01. Verificación: prueba de contrato comprueba firma, JSON y ausencia de campos por conector.

## 3. Ejecución segura

- [x] 3.1 [M] Resolver y reautorizar el destino usuario–actividad dentro del lock. Área/archivos: `Infrastructure/Repositories/Workflow/MySqlEnvioUsuarioRepository.vb`, `Services/Workflow/Terminar/ServicioEnvioUsuarioTarea.vb`. Origen: D-04, RQ-04. Verificación: la prueba focal comprueba que token, permiso, destino y requisitos se revalidan bajo el lease y antes del ejecutor.
- [x] 3.2 [M] Encapsular la política de respuesta permitida sin reasignación ni handlers Web Forms. Área/archivos: `Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioUsuarioRequisitosAdapter.vb`. Origen: D-04, RQ-04. Verificación: la prueba focal exige `Verifica_respuesta_radicado_sin_respuesta`, resultado `YES` y ausencia de APIs excluidas.
- [x] 3.3 [M] Añadir el adaptador mutante directo a `Terminar_Tarea_Workflow`. Área/archivos: `Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioUsuarioExecutorAdapter.vb`. Origen: D-05, RQ-05. Verificación: la prueba focal confirma una llamada, `Page = Nothing`, destino directo, conector cero y APIs excluidas ausentes.
- [x] 3.4 [M] Integrar ejecución bajo `GET_LOCK`, resultados normalizados y endpoint `EjecutarEnvioUsuario`. Área/archivos: `Services/Workflow/Terminar/ServicioEnvioUsuarioTarea.vb`, `webservice/WebServiceWorkflowModern.asmx.vb`. Origen: D-04, RQ-04. Verificación: la prueba focal verifica orden de revalidación bajo el lease y la composición ASMX sin motor en transporte.
- [x] 3.5 [S] Registrar mecanismo `ASMX_ENVIO_USUARIO` en auditoría sanitizada y preservar el éxito ante falla de auditoría. Área/archivos: `Infrastructure/Workflow/Terminar/WorkflowLegacyAuditoriaAdapter.vb`, `Services/Workflow/Terminar/ServicioEnvioUsuarioTarea.vb`. Origen: D-06, RQ-05. Verificación: la prueba focal exige el mecanismo y la advertencia ante falla de auditoría.

## 4. Verificación y documentación

- [x] 4.1 [L] Añadir pruebas focales para contrato, permiso, lectura, filtro, cursor, límite, token, lock, requisitos, advertencias y auditoría. Área/archivos: `tests/workflow-user-send.test.cjs` y pruebas unitarias de las capas afectadas. Origen: D-07, RQ-06. Verificación: 9 pruebas focales y 38 en la corrida conjunta, sin E2E autenticado.
- [x] 4.2 [S] Verificar que Continuar flujo conserva contratos y recorrido por conector. Área/archivos: `tests/workflow-transition-*.test.cjs`, `webservice/WebServiceWorkflowModern.asmx.vb`. Origen: D-07, RQ-06. Verificación: las pruebas existentes de transición y feature gate pasaron sin cambios de payload.
- [x] 4.3 [M] Documentar arquitectura, contrato, seguridad y evidencia del corte backend. Área/archivos: `Doc/Actualizacion/workflow/TerminarUsuario/01-implementacion-envio-usuario/`. Origen: D-07, RQ-06. Verificación: cuatro documentos describen endpoints, límites, auditoría, no escritura y relevo UI.
- [x] 4.4 [S] Ejecutar validación OpenSpec, auditoría de refinement y compilación disponible; registrar limitaciones sin E2E. Área/archivos: `openspec/changes/doc-28-backend-enviar-usuario-workflow`, evidencia local de validación. Origen: D-07, RQ-06. Verificación: `openspec validate --strict`, `opsxj:refine --sync`, suite focal/regresión y compilación finalizaron sin fallos; no se ejecutó E2E.
