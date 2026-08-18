# Arquitectura

## Fronteras

```text
Presentation
  WorkflowTransitionUi -> evento de destino seleccionado
  WorkflowTransitionConfirmationIntegration -> adaptador de caso de uso
  ConfirmationDialog -> componente genérico y accesible
  WorkflowTransitionPagePresentation -> actualización visual por data-attributes

Application e integración existente
  WebServiceWorkflowModern.EjecutarEnvioTarea
  ServicioTransicionTarea
  feature gate, concurrencia, requisitos, autorización y adaptadores legacy
```

El navegador transporta únicamente la terna validada por el preview. La decisión de transición, permisos, concurrencia y cambios de estado siguen siendo responsabilidad de servidor.

## Responsabilidades

| Componente | Responsabilidad | Dependencias permitidas | Dependencias prohibidas |
| --- | --- | --- | --- |
| `ConfirmationDialog` | Renderizado, foco, teclado, estados y prevención de doble envío | Configuración y callbacks genéricos | Workflow, Web Forms, Session, selectores de página, SQL y controles ocultos |
| `WorkflowTransitionConfirmationIntegration` | Traducción de selección/ASMX al contrato visual | `ConfirmationDialog`, `WebServiceWorkflowModern.asmx` | Motor legacy, controles ocultos y reglas de negocio |
| `WorkflowTransitionPagePresentation` | Retirar la fila si está presente, restaurar la lista, limpiar contexto/visor, ocultar acciones, actualizar contador y anunciar éxito durante seis segundos | Atributos `data-workflow-*` emitidos por página | IDs ocultos, funciones legacy y postbacks |
| `Webworkflow.aspx(.vb)` | Registrar assets solo durante el piloto y emitir hosts visuales estables | `WorkflowModernPresentationBootstrap` | Autorización cliente o ejecución directa |
| `ServicioTransicionTarea` | Revalidar y ejecutar la transición | Repositorios y adaptadores autorizados existentes | Lógica de renderizado |

## Activación y reversa

`WorkflowModernPresentationBootstrap` evalúa el mismo `IWorkflowModernFeatureGate` que protege el ASMX. Cuando no está activo, la página no registra CSS, scripts ni listeners de confirmación. El rollback consiste en apagar la bandera; no requiere migración ni reescritura del flujo legacy.

## Alternativas descartadas

1. Acoplar el componente genérico a `GridView2`, `Hidden_id_tarea_sel` o helpers legacy: rompe reutilización y mezcla Presentation con el flujo antiguo.
2. Actualizar la página antes de conocer `Exito`: puede ocultar una tarea que el servidor bloqueó o no ejecutó.
3. Agregar un endpoint o DTO para contexto solo visual: no es necesario para la primera versión; se muestran únicamente campos existentes.
