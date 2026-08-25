# Arquitectura de verificación

- Ticket: DOC-34
- Cambio OpenSpec: `doc-34-verificacion-transversal-devolver-tarea`
- Clasificación: `cross_cutting`

## Fuentes de evidencia

DOC-34 observa la cadena ya implementada sin alterarla. Correlaciona cuatro fuentes: inspección estática de ASMX/VB.NET/JavaScript/Web Forms, pruebas CJS focales, compilación MSBuild y QA manual no autenticada. La salida es una matriz saneada y un dictamen técnico único.

## Superficies observadas

| Capa | Componentes | Garantía verificada |
| --- | --- | --- |
| Transporte | `WebServiceWorkflowModern.asmx.vb` | Preview y ejecución reciben únicamente el contrato público y reconstruyen el contexto de sesión. |
| Aplicación | `ServicioDevolverActividad.vb` | Permiso, token, relectura bajo lock, resultado funcional y auditoría saneada. |
| Persistencia | `MySqlDevolverActividadRepository.vb` | Preview `SELECT` parametrizado sobre aristas entrantes Ruta/Flujo, con orden, cursor y límite. |
| Frontera legacy | Adaptador de devolución y auditoría | Una ejecución usa el adaptador aislado; no añade tratamiento de respuestas ni UI heredada. |
| Presentación | scripts `workflow-return-activity-*` y `Webworkflow.aspx` | Preview, selección, confirmación, bloqueo temporal y marcado accesible sin gate. |
| Compatibilidad | suites de usuario, grupo, transición y gate | Las transiciones vecinas conservan sus contratos propios. |

## Decisión arquitectónica

La verificación local puede aprobar las garantías estáticas y automatizadas, pero no reemplaza una QA visual. No se reutiliza el ambiente ni se consume una tarea para obtener evidencia: las E2E autorizadas de DOC-32 y DOC-33 se citan como antecedente saneado y no se ejecutan de nuevo en DOC-34.
