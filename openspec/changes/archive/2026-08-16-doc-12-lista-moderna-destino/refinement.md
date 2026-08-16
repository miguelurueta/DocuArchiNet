<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - DOC-12: lista moderna de destinos

## Fuente y alcance

- Ticket: `DOC-12` — LISTA-MODERNA-DESTINO.
- Cambio OpenSpec: `doc-12-lista-moderna-destino`.
- Perfil técnico observado: ASP.NET Web Forms .NET Framework 4.6.1, VB.NET, JavaScript gradual, CSS y ASMX.
- Alcance propuesto: Presentation para consultar y representar destinos; no ejecuta una transición ni sustituye la UI legacy.
- Fuera de alcance inicial: Application, Domain, Infrastructure, `EjecutarEnvioTarea`, `Terminar_Tarea_Workflow`, `Cambia_Estado`, correo, auditoría y eventos dinámicos.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx` conserva el enlace **Continuar flujo**, el `GridView_envia_flujo`, el modal legacy y sus botones Web Forms invisibles. El camino actual publica un postback a `ImageButtonterminar`.
- `workflow/Webworkflow.aspx.vb` expone `WorkflowCentroTrabajoModernActive`, basado en `WorkflowCentroTrabajoModernEnabled` y perfiles piloto de página.
- `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` implementa `IWorkflowModernFeatureGate` con `WorkflowCentroTrabajoModernActive`, usuarios y grupos; es el gate que el ASMX vuelve a validar.
- `webservice/WebServiceWorkflowModern.asmx.vb` publica `PreviewEnviarTarea(idTarea)` como operación de lectura autenticada y no compone la ejecución durante esa llamada.
- `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb` publica `TipoDecision`, `Contexto.Radicado`, `Contexto.ActividadOrigen`, `Contexto.GrupoActual`, destinos, `RequiereNotificacion`, `TokenVersion` y `Error`.
- El DTO actual no contiene trámite, actividad actual legible, notificación o requisitos por destino. `ServicioTransicionTarea.Previsualizar` no llena requisitos de preview.

## Decisiones de refinamiento

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | La lista moderna será un consumidor paralelo y de solo lectura; no altera ni invoca los controles, el modal o la transición legacy. | `workflow/Webworkflow.aspx`; `workflow/Webworkflow.aspx.vb`; `webservice/WebServiceWorkflowModern.asmx.vb` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | La activación visual debe provenir en servidor de `IWorkflowModernFeatureGate`; la propiedad de página existente no es equivalente al gate del ASMX y no se reutiliza como autorización. | `workflow/Webworkflow.aspx.vb`; `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | La primera versión omite trámite y el nombre legible de la actividad actual. JavaScript representa únicamente los campos reales del DTO y no deriva valores de datos legacy. | `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb`; `Services/Workflow/Terminar/ServicioTransicionTarea.vb` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | La interacción moderna conserva los cinco estados de Presentation, accesibilidad y un callback de selección; nunca llama a `EjecutarEnvioTarea`. | `webservice/WebServiceWorkflowModern.asmx.vb`; `workflow/Webworkflow.aspx` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | La entrega se verifica con build, pruebas focales y QA reproducible. La E2E autenticada solo se ejecuta con ambiente y cuentas de prueba autorizados. | `GestionDocumental-Docuarchi.net.vbproj`; `tools/e2e`; `AGENTS.md` | D-05 | RQ-05 | Origen: D-05, RQ-05 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Con el camino moderno desactivado, el enlace, grid y modal legacy conservan su comportamiento. | Cuando el bootstrap no está activo, JavaScript no se enlaza ni consulta el ASMX. | Rollback inmediato por desactivación del gate; no se modifica el motor legacy. |
| RQ-02 | La interfaz moderna se inicializa únicamente desde un atributo emitido por servidor tras evaluar el mismo gate del ASMX. | Cuando el gate no permite el contexto, la interfaz no consume el endpoint ni reemplaza el enlace legacy. | Evita dos poblaciones piloto con reglas distintas. |
| RQ-03 | La UI usa radicado, tipo y grupo actual como contexto; omite trámite y actividad actual legible, que no están en el DTO real. | Cuando faltan esos datos, la UI no los infiere desde IDs, campos ocultos, sesión o HTML legacy. | No se amplía el backend por esos campos en DOC-12; la UI no fabrica datos en navegador. |
| RQ-04 | Carga, ausencia de destinos, error controlado, lista y selección son perceptibles y recuperables. | Al seleccionar un destino se emite `idTarea`, identificador de conector, `tokenVersion` y resumen visible; no se ejecuta envío. | Escape, foco atrapado y teclado no dejan foco perdido; no se añade una segunda confirmación. |
| RQ-05 | Código y evidencia permiten validar no regresión y revertir la interfaz. | Build, pruebas focales y QA verifican banderas, estados, teclado, responsive y ausencia de ejecución. | Las pruebas autenticadas o mutantes requieren autorización; el gate queda apagado al finalizar. |

## Decisión de alcance registrada

DOC-12 procede como Presentation usando el DTO actual. La cabecera del modal usa radicado, tipo y grupo actual; trámite y actividad actual legible quedan fuera de la primera versión. La prohibición de modificar el code-behind debe reconciliarse con la obligación de emitir desde servidor un bootstrap derivado de `IWorkflowModernFeatureGate`.

## Resultado del refinamiento

Estado: borrador controlado. Las inspecciones, trazabilidad y decisión del DTO están registradas; falta la confirmación formal del refinamiento antes de sincronizarlo.
