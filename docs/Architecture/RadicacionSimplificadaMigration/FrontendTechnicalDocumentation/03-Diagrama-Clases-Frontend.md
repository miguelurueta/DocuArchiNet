# Diagrama De Clases Frontend

## Vista General

```mermaid
classDiagram
  class RadicacionRoutePage {
    +useCamposPlantilla()
    +render()
  }

  class RadicacionStartupGuard {
    +useRadicacionEstadoActivo()
    +resolverEntrada()
  }

  class RadicacionDocumentalProvider {
    +contexto: RadicacionDocumentalContextValue
    +setActivoDesdeBackend()
    +limpiarContextoDocumental()
  }

  class RadicacionPage {
    +plantilla: PlantillaRadicadoDTO
    +render()
  }

  class RadicacionTabs {
    +activeTab: RadicacionTabKey
    +setActiveTab()
    +isDocumentosEnabled()
  }

  class RadicacionForm {
    +plantilla: PlantillaRadicadoDTO
    +radicar()
    +limpiarFormulario()
    +enviarActivoAPendiente()
  }

  class RadicacionPendientesModal {
    +open()
    +close()
    +onAsignacionTarea()
  }

  class AppTable {
    +queryState
    +actions
    +render()
  }

  class RadicacionDocumentosGuard {
    +canRenderDocumentos()
  }

  class CapDocument {
    +render()
  }

  class RadicacionPendientesService {
    +getEstadoActivo()
    +getContador()
    +tomarPendiente()
    +enviarPendiente()
  }

  class RadicacionRegistroService {
    +registrarEntrante()
  }

  class RadicacionDocumentalContextValue {
    +activo: boolean
    +estado: 0|1|null
    +idEstadoRadicado: number|null
    +consecutivoRadicado: string|null
    +idTareaWorkflow: number|null
    +requiereGestionDocumental: boolean
  }

  RadicacionRoutePage --> RadicacionStartupGuard
  RadicacionRoutePage --> RadicacionDocumentalProvider
  RadicacionRoutePage --> RadicacionPage
  RadicacionStartupGuard --> RadicacionPendientesService
  RadicacionStartupGuard --> RadicacionDocumentalProvider
  RadicacionDocumentalProvider --> RadicacionDocumentalContextValue
  RadicacionPage --> RadicacionTabs
  RadicacionTabs --> RadicacionForm
  RadicacionTabs --> RadicacionPendientesModal
  RadicacionTabs --> RadicacionDocumentosGuard
  RadicacionDocumentosGuard --> CapDocument
  RadicacionDocumentosGuard --> RadicacionDocumentalProvider
  RadicacionForm --> RadicacionRegistroService
  RadicacionForm --> RadicacionPendientesService
  RadicacionForm --> RadicacionDocumentalProvider
  RadicacionPendientesModal --> AppTable
  RadicacionPendientesModal --> RadicacionPendientesService
  RadicacionPendientesModal --> RadicacionDocumentalProvider
```

## Notas De Diseno

| Relacion | Regla |
|---|---|
| `RadicacionForm -> RadicacionDocumentalProvider` | Solo escribe contexto despues de radicacion exitosa o envio a pendiente. |
| `RadicacionPendientesModal -> AppTable` | La tabla dispara `asignacion-tarea`; el modal no debe interpretar columnas manualmente si el contrato viene como `DynamicUiTableDto`. |
| `RadicacionDocumentosGuard -> CapDocument` | `CapDocument` no debe montarse sin contexto activo `estado = 0`. |
| `RadicacionStartupGuard -> RadicacionPendientesService` | La deteccion de activo sucede al inicio del modulo. |

