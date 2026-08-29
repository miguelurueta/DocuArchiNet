# Fundación de backend y contratos de Notas de Workflow

## Why

Las notas de Workflow dependen hoy de endpoints y una clase legacy que mezclan sesión, controles WebForms, autorización y persistencia. La revisión estática identifica que la tarea seleccionada puede provenir de un valor de sesión mutable y que los contratos actuales no delimitan de forma uniforme tarea, nota, actor y resultado funcional.

DOC-40 define la base interna que permitirá sustituir ese acoplamiento de forma gradual. La fase autorizada crea únicamente contratos, gate, servicio y repositorio inactivo; no cambia todavía el recorrido de ningún usuario ni habilita operaciones sobre datos.

## What Changes

- Define contratos internos de Notas para listar, contar, crear, consultar, actualizar y eliminar, siempre con `idTarea` explícito y, cuando corresponda, `idNota`.
- Define un gate de contexto de Notas que obtiene identidad, grupo y permiso desde la sesión autenticada del servidor y falla cerrado.
- Define el puerto de acceso a tarea reutilizando el patrón `ITareaWorkflowRepository.ObtenerTarea(contexto, idTarea)` para comprobar acceso y estado antes de cualquier operación posterior.
- Incorpora la ruta de negocio de Workflow al snapshot autorizado: el servidor resuelve `IdRutaWorkflow` en el contexto y el `IdRuta` propio de la tarea; ningún cliente entrega nombre de ruta, tabla o metadato de ruta.
- Establece resultados funcionales estables: `Forbidden`, `TaskNotActive`, `NoteNotFound`, `NotOwner`, `VersionConflict`, `InvalidContent` y `Unavailable`.
- Establece la separación entre transporte, gate, servicio, modelos y repositorios, sin introducir `Page`, `GridView`, `UpdatePanel` ni `HttpContext` en dominio o infraestructura.
- Documenta pruebas unitarias focales y la evidencia requerida para la etapa que autorice crear el código.

## Non-Goals

- No se modifica `workflow/`, JavaScript, páginas WebForms, consumidores ni `WorkflowCentroTrabajoModernActive`.
- No se publica un ASMX de Notas, no se ejecutan escrituras, no se modifica esquema ni se consulta una base de datos real.
- No se copia, envuelve ni extiende `Class_anotacion_tarea` como implementación moderna.
- No se cambian módulos distintos de Workflow ni se migra Radicación, Gestión de Correspondencia o consulta histórica.
- No se aplica una migración ni se ejecuta una escritura. Las políticas de futura mutación se documentan con base en la inspección de código y metadatos MySQL, pero su implementación exige un cambio posterior autorizado.

## Impact

- Capacidad nueva: `backend-contratos-notas`.
- Áreas candidatas para una autorización posterior: `Modelo/Workflow/`, DTOs de Workflow, `Services/Workflow/`, `Infrastructure/Repositories/Workflow/` y `webservice/`.
- Compatibilidad: los contratos y clientes legacy permanecen intactos mientras no exista un consumidor de Workflow migrado y habilitado de forma explícita.
- Patrón de rutas: el acceso posterior reutilizará la resolución parametrizada de `rutas_workflow`; si requiere metadatos dinámicos, el nombre de ruta se valida en servidor antes de formar un identificador técnico.
- Evidencia futura: pruebas unitarias sin base real; una E2E no procede hasta que otra fase exponga un comportamiento de usuario y cuente con autorización de ambiente y cuentas.

## Decision Boundary

La autorización recibida habilita contratos internos, gate, pruebas locales y consultas MySQL de metadatos en solo lectura. Los esquemas autorizados operan en MySQL 5.1: la primera escritura exige una migración aprobada por esquema con `ANOTACION_TAREA` en InnoDB, `Dato_Anotacion TEXT utf8`, auditoría InnoDB disponible, índices de lectura y tabla de idempotencia verificadas. El contrato acepta solo Unicode BMP y rechaza pares sustitutos; una futura actualización de motor podrá habilitar `utf8mb4` en un cambio separado. La publicación de endpoints, activación de consumidores, configuración de gate, E2E y cambios de ambiente siguen requiriendo autorización explícita posterior.
