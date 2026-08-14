# Límite de repositorios Workflow

Esta carpeta contendrá implementaciones MySQL específicas de Workflow cuando se habilite el preview de solo lectura. DOC-9 no agrega consultas ni replica SQL legacy: los contratos se definieron en `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb` y la infraestructura transversal está en `Infrastructure/Shared/Data/`.

Las implementaciones futuras deben usar consultas parametrizadas, recibir `ContextoModuloWorkflow` validado y no acceder a `HttpContext.Current.Session`, `DataSet` ni controles Web Forms.
