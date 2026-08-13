# Modernización del envío y terminación de tareas Workflow

Este directorio contiene prompts secuenciales para implementar una experiencia moderna de **Continuar / Enviar tarea**, conservando la lógica de negocio vigente.

## Principio arquitectónico

JavaScript administra la experiencia asíncrona; el servidor conserva la decisión, las validaciones, los eventos dinámicos y la transacción. No se debe duplicar `Terminar_Tarea_Workflow` ni `Cambia_Estado` durante la primera fase.

## Arquitectura obligatoria

La implementación debe seguir una **arquitectura limpia pragmática dentro de un monolito modular**, aplicada gradualmente con patrón *Strangler Fig*. La solución moderna coexistirá con el flujo legado y solo reemplazará componentes cuando estén cubiertos por pruebas.

```text
Presentation (ASMX / JavaScript)
        ↓
Application (servicios, casos de uso, validadores y DTO)
        ↓
Domain (modelos e interfaces)
        ↓
Infrastructure (repositorios MySQL y adaptadores)
        ↓
Motor legado encapsulado: Terminar_Tarea_Workflow / Cambia_Estado
```

Reglas de dependencia:

- `Presentation` solo invoca `Application` y mapea JSON.
- `Application` no depende de `Page`, `Session`, `GridView`, `UpdatePanel` ni `ModalPopupExtender`.
- `Domain` no depende de ASP.NET, base de datos ni interfaz.
- `Infrastructure` implementa las interfaces declaradas en `Domain`.
- El adaptador legado es el único componente nuevo autorizado a invocar `Terminar_Tarea_Workflow` y `Cambia_Estado`.
- No crear una segunda implementación de la terminación, cambio transaccional, firma, expediente, balanceo o eventos dinámicos.

## Acceso a datos reutilizable obligatorio

La infraestructura de datos debe ser reutilizable por otros módulos, pero los repositorios de negocio deben conservar límites específicos por dominio.

```text
Infrastructure/Shared/Data
  ├── IModuleConnectionFactory
  ├── IDataExecutor
  ├── ITransactionFactory
  ├── IAuditoriaRepository
  └── modelos comunes de paginación, resultado y error

Infrastructure/Repositories/Workflow
  ├── MySqlTareaWorkflowRepository
  ├── MySqlTransicionFlujoRepository
  ├── MySqlTransicionRutaRepository
  └── MySqlConfiguracionConectorRepository
```

- `IModuleConnectionFactory` debe resolver la conexión autorizada del módulo sin que repositorios consuman `HttpContext.Current.Session` directamente.
- Toda consulta nueva debe ser parametrizada y devolver modelos tipados; no `DataSet` ni HTML.
- No implementar un `GenericRepository` que mezcle entidades y reglas de módulos diferentes.
- Los repositorios específicos de Workflow no se reutilizan directamente en Radicación, Expedientes o Gestión Documental; esos módulos reutilizan la infraestructura común y definen sus propios repositorios.

## Orden de ejecución

1. `01-fundacion-contratos.md`
2. `02-preview-ruta-flujo.md`
3. `03-ejecucion-segura.md`
4. `04-lista-moderna.md`
5. `05-confirmacion-especializada.md`
6. `06-piloto-pruebas-rollout.md`

## Acoplamiento obligatorio entre prompts

La secuencia no permite crear implementaciones paralelas de los mismos límites:

- El Prompt 01 crea la base, incluido `IWorkflowModernFeatureGate` como única fuente nueva de `WorkflowCentroTrabajoModernActive`, con comportamiento fail-closed.
- El Prompt 02 crea `webservice/WebServiceWorkflowModern.asmx` y expone solo `PreviewEnviarTarea`; valida la habilitación en servidor antes de resolver destinos.
- El Prompt 03 agrega `EjecutarEnvioTarea` al mismo ASMX y vuelve a validar la habilitación antes de invocar el adapter legacy.
- El Prompt 04 consume solo el bootstrap visual emitido por servidor; no calcula la bandera y no llama preview si está inactiva.
- El Prompt 05 usa `ConfirmationDialog` genérico mediante el adaptador Workflow; JavaScript llama al ASMX y el ASMX delega a Application.
- El Prompt 06 configura el piloto, mide y desactiva la bandera base; no crea una segunda configuración ni permite llamadas ASMX modernas fuera del alcance habilitado.

## Convención documental obligatoria

Cada prompt es un paquete documental independiente. No se agregan archivos técnicos junto al prompt ni se usan nombres libres.

```text
Doc/Actualizacion/workflow/Terminar/
  <NN>-<slug-del-prompt>/
    00-indice.md
    01-arquitectura.md
    02-contrato.md
    03-flujo-y-seguridad.md
    04-pruebas-y-evidencia.md
    Diagramas/
```

Reglas:

- `<NN>` coincide con el prefijo del prompt: por ejemplo, `02-preview-ruta-flujo.md` usa la carpeta `02-preview-ruta-flujo/`.
- El prompt fuente permanece en `Doc/Actualizacion/workflow/Terminar/<NN>-<slug>.md`; su documentación de implementación se crea solo dentro de su subcarpeta homónima.
- `00-indice.md` identifica ticket, fecha, estado, alcance y archivos relacionados.
- `01-arquitectura.md` documenta capas, responsabilidades, dependencias, decisiones y alternativas descartadas.
- `02-contrato.md` documenta DTO, parámetros, JSON, validaciones, errores funcionales y compatibilidad.
- `03-flujo-y-seguridad.md` documenta secuencia, autorización, límites legacy, estados, concurrencia, riesgos y rollback.
- `04-pruebas-y-evidencia.md` registra comandos, compilación, pruebas focales, QA manual, resultados, limitaciones y evidencia.
- `Diagramas/` contiene archivos Mermaid o fuentes estructuradas de componentes, secuencia y estados cuando correspondan.
- No se crean documentos técnicos en la raíz del repositorio, en la raíz de `Terminar/` ni fuera del paquete del prompt, salvo que el prompt lo justifique expresamente.

## Alcance legado que debe preservarse

- Envío por ruta y por flujo documental.
- Retorno por flujo y envíos manuales a usuario o grupo.
- Respuesta/confirmación, aprobaciones, firma digital, expediente, copia documental y balanceo.
- Eventos dinámicos `PRETERMINARACTIVIAD` y `TERMINARACTIVIDAD`.
- Cambio transaccional de estado y trazabilidad existente.

## Referencias principales

- `workflow/Webworkflow.aspx`
- `workflow/Webworkflow.aspx.vb`
- `workflow/ClassWorkflow.vb`
- `workflow/Class_flujo_trabajo_workflow.vb`
- `workflow/Class_grupos_workflow.vb`
- `workflow/Class_actividades_disponibles_envio.vb`
- `webservice/WebServiceWorkflow.asmx.vb`
