<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-50-contratos-multi-proveedor

## Fuente y alcance

- Ticket: `DOC-50` — CONTRATOS-MULTI-PROVEEDOR
- Cambio OpenSpec: `doc-50-contratos-multi-proveedor`
- Fuente Jira: `specs/contratos-multi-proveedor/jira-context.md`
- Perfil tecnológico: ASP.NET Web Forms/ASMX y Visual Basic sobre .NET Framework 4.6.1.
- Alcance aprobado: núcleo contractual aditivo de `ImportarServicioWeb`; no incluye adaptadores HTTP, persistencia, ejecución de importaciones, cambios de endpoints ni activación de gates.

Este artefacto es la compuerta entre el ticket y la implementación. Las decisiones siguientes se basan en el ticket, el contrato compartido y la inspección del código y las exploraciones vigentes.

## Contexto inspeccionado

- `Domain/Shared/ContextoModulo.vb`: contexto común con módulo, usuario, grupo y login; no contiene tarea, ruta, trámite ni proveedor.
- `Modelo/Workflow/Terminar/WorkflowModernModels.vb`: patrón de contexto Workflow, modelos internos sin `Session` y resultados tipados con código funcional.
- `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb`: puertos de negocio que reciben contexto explícito y mantienen infraestructura fuera del modelo.
- `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb`: convención de DTO públicos serializables sin SQL, HTML, excepciones ni `Session`.
- `Services/Workflow/Terminar/ServicioTransicionTarea.vb`: referencia de fachada de aplicación con dependencias inyectadas y validación previa.
- `ServiciosIntegracion/Class_ra_ser_servicioIntegracion.vb`: el despacho productivo inspeccionado solo implementa explícitamente `INTEGRACIONSII`; no existe evidencia estática suficiente para declarar otro proveedor soportado.
- `ServiciosIntegracion/ClassAdjuntaDocumentoServicioIntegracion.vb` y `webservice/WebServiceAdjuntaDocumentoServicioIntegracion.asmx.vb`: obtienen tarea y estado mutable desde `HttpContext.Current.Session`.
- `workflow/ClassAlmacenamiento.vb`: contiene el recorrido vigente y llama a `AlmacenaDocumentoTareaWorkflow(...)`; se preserva como caja negra y no se modifica en DOC-50.
- `Doc/Actualizacion/workflow/ImportarServicioWeb/Exploracion/03-exploracion-backend-importar-servicio-web.md`: hallazgos B-01, B-02 y B-03 sobre tarea y tipología mutables y bloqueo de sesión; recomienda capturar el contexto antes del primer `Await`.
- `Doc/Actualizacion/workflow/ImportarServicioWeb/CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`: define las ocho operaciones lógicas y la frontera contractual compartida.
- `GestionDocumental-Docuarchi.net.vbproj`: los archivos VB nuevos deben agregarse como `Compile Include` sin mover entradas existentes.

El comportamiento actual que debe preservarse incluye las firmas ASMX, la consulta y el almacenamiento SII existentes, las claves de sesión consumidas por el legado y la implementación de `AlmacenaDocumentoTareaWorkflow(...)`. DOC-50 crea una frontera paralela que todavía no sustituye esos recorridos.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Publicar la versión contractual `v1` para las ocho operaciones lógicas en DTO serializables canónicos, con `schemaVersion`, correlación, nulabilidad y errores seguros; los fixtures JSON son parte verificable del contrato. | `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb`; contrato compartido | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Modelar `ContextoImportacionServicio` como instantánea inmutable compuesta desde el contexto Workflow autorizado antes de cualquier operación asíncrona; ninguna capa interna leerá `HttpContext` o `Session`. | `Domain/Shared/ContextoModulo.vb`; modelos Workflow; exploración B-01/B-03 | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Resolver proveedores exclusivamente por identidad canónica registrada mediante `IExternalImportProvider`; identidad nula, vacía, duplicada o desconocida produce resultado tipado no soportado y nunca fallback a SII. | `Class_ra_ser_servicioIntegracion.vb`, `Select Case "INTEGRACIONSII"`; exploración §15 | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Revalidar con puertos inyectables usuario autenticado, permiso vigente, tarea operable, ruta, trámite y proveedor; valores provenientes del navegador son referencias, no autoridad. | Interfaces Workflow; exploración §6 y §18 | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Mantener la entrega aditiva y sin efectos: la fachada solo expone capacidades y consulta contractual; no modifica ASMX, persistencia, sesión ni `AlmacenaDocumentoTareaWorkflow(...)`. | Fachada Workflow; `workflow/ClassAlmacenamiento.vb`; exploración §12 y §16 | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Separar DTOs, modelos, interfaces y servicios en las rutas canónicas del ticket, registrar cada VB en el proyecto y prohibir duplicados en capas legacy. | `GestionDocumental-Docuarchi.net.vbproj`; rutas obligatorias de Jira | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Validar contratos, registro y estabilidad del contexto con pruebas focales sin sesión ni red; el E2E real queda fuera de DOC-50 y requiere autorización independiente. | `Tests/`; política AGENTS.md; exploración §19 | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Crear un único paquete técnico en `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-50-contratos-contexto-registro-multiproveedor/`, con ocho documentos y diagramas coherentes con el código y la evidencia real. | Ubicación confirmada para la entrega DOC-50 | D-08 | RQ-08 | Origen: D-08, RQ-08 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Los ocho contratos v1 y sus fixtures expresan requests/responses, nulabilidad, códigos, autorización, idempotencia y concurrencia sin conceptos exclusivos de SII. | WHEN una prueba carga cada fixture THEN lo valida contra el DTO o forma contractual correspondiente y detecta campos o semántica divergentes. | Una evolución incompatible rompería consumidores; rollback: conservar v1 y agregar una versión posterior, nunca reinterpretar campos publicados. |
| RQ-02 | El contexto conserva usuario, tarea, ruta, trámite, proveedor y autorización capturados al inicio. | WHEN cambia la sesión después de construir el contexto THEN servicios y proveedores siguen usando la instantánea original y ninguna clase nueva consulta `HttpContext`. | Evita contaminación entre pestañas; rollback: retirar solo la integración paralela, sin tocar sesión legacy. |
| RQ-03 | Un proveedor conocido se obtiene por identidad registrada y uno desconocido falla de forma segura. | WHEN se solicita una identidad no registrada THEN se retorna `PROVIDER_NOT_SUPPORTED` o código contractual equivalente y no se instancia ni ejecuta SII. | El repositorio solo demuestra soporte explícito de INTEGRACIONSII; proveedores adicionales requieren configuración y adaptador comprobados. |
| RQ-04 | Las reglas de autorización se ejecutan mediante dependencias deterministas y rechazan contexto desactualizado o inconsistente. | WHEN permiso, tarea, ruta, trámite o proveedor dejan de ser válidos THEN la operación termina antes de consultar o mutar recursos externos. | No se confía en identificadores del navegador; el legado continúa intacto hasta integrar adaptadores posteriores. |
| RQ-05 | La entrega no produce efectos de importación ni cambia contratos públicos existentes. | WHEN se ejecutan las pruebas de DOC-50 THEN no se realizan llamadas de red, escrituras documentales, cambios de tarea ni invocaciones a `AlmacenaDocumentoTareaWorkflow(...)`. | Compatibilidad protegida por implementación paralela; rollback: eliminar archivos nuevos y entradas nuevas del proyecto. |
| RQ-06 | Cada responsabilidad reside una sola vez en DTOs, Modelo o Services y todos los archivos VB compilan desde el proyecto. | WHEN la prueba estructural inspecciona rutas y `.vbproj` THEN encuentra exactamente los archivos canónicos y ninguna definición duplicada en ASMX, App_Code o carpetas prohibidas. | Reduce deriva; las entradas existentes del proyecto no se reordenan ni sustituyen. |
| RQ-07 | Tres suites focales cubren forma contractual, registro y contexto sin red ni sesión, y se registra evidencia de build compatible con el entorno disponible. | WHEN se ejecutan las suites THEN cubren proveedor conocido y desconocido, duplicados, inmutabilidad, validación y fixtures compartidos. | No se ejecutará E2E autenticado ni carga sin autorización expresa; su ausencia se documentará, no se simulará como PASS. |
| RQ-08 | La documentación técnica usa la ruta DOC-50 y enlaza arquitectura, flujo, contrato, errores, pruebas, diagramas y metadata. | WHEN se verifican los enlaces relativos THEN todos resuelven dentro del paquete único y describen solo código y pruebas realmente entregados. | Evita duplicación con fuentes de exploración; no se crea el paquete erróneo `SCRUMCORE-000`. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` debe estar desarrollada en `design.md`, reflejada en al menos un requirement/scenario de `spec.md` y vinculada a una tarea mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea con checkbox debe conservar su origen. Las tareas de validación, rollout y documentación también deben indicar la decisión o requisito que verifican.
3. Las reglas de Web Forms, ASMX y .NET Framework se limitan a las rutas y compatibilidad demostradas por el código inspeccionado.
4. El soporte de proveedores adicionales a `INTEGRACIONSII` no se afirmará hasta contar con evidencia de configuración; el contrato y registro sí deben permitir agregarlos explícitamente.
5. El estado `approved` confirma que la matriz no tiene placeholders y que las decisiones son suficientemente específicas para sincronizar design, spec y tasks.

## Resultado del refinamiento

- Estado: aprobado para sincronizar decisiones y requisitos con `design.md`, `spec.md` y `tasks.md`.
- Próximo comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-50 --sync`.
