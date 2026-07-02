# Diagramas De Secuencia Frontend

## Inicio Del Modulo Con Estado Activo

```mermaid
sequenceDiagram
  actor Usuario
  participant Route as RadicacionRoutePage
  participant Startup as RadicacionStartupGuard
  participant Api as RadicacionPendientesService
  participant Ctx as RadicacionDocumentalProvider
  participant Tabs as RadicacionTabs
  participant Docs as RadicacionDocumentosGuard

  Usuario->>Route: Entra a /dashboard/radicacion
  Route->>Route: Carga plantilla
  Route->>Startup: Inicializa modulo
  Startup->>Api: GET /api/radicacion/pendientes/estado-activo
  Api-->>Startup: TieneActivoEstado0 = true
  Startup->>Ctx: setActivoDesdeBackend(contexto)
  Startup->>Tabs: Navegar a documentos
  Tabs->>Docs: Renderizar tab documentos
  Docs-->>Tabs: Permitido por estado = 0
```

## Inicio Del Modulo Sin Estado Activo

```mermaid
sequenceDiagram
  actor Usuario
  participant Route as RadicacionRoutePage
  participant Startup as RadicacionStartupGuard
  participant Api as RadicacionPendientesService
  participant Tabs as RadicacionTabs

  Usuario->>Route: Entra a /dashboard/radicacion
  Route->>Startup: Inicializa modulo
  Startup->>Api: GET /api/radicacion/pendientes/estado-activo
  Api-->>Startup: TieneActivoEstado0 = false
  Startup->>Tabs: Abrir formulario
  Tabs-->>Usuario: Documentos inactivo
```

## Tomar Pendiente Desde AppTable

```mermaid
sequenceDiagram
  actor Usuario
  participant Modal as RadicacionPendientesModal
  participant Table as AppTable
  participant Api as RadicacionPendientesService
  participant Ctx as RadicacionDocumentalProvider
  participant Tabs as RadicacionTabs

  Usuario->>Modal: Abre pendientes
  Modal->>Table: Renderiza lista DynamicUiTable
  Usuario->>Table: Ejecuta accion asignacion-tarea
  Table->>Modal: Entrega payload de fila
  Modal->>Api: POST /api/radicacion/pendientes/{idEstadoRadicado}/tomar
  Api-->>Modal: estadoActual = 0 + contexto documental
  Modal->>Ctx: setActivoDesdeBackend(contexto)
  Modal->>Tabs: Navegar a documentos
  Tabs-->>Usuario: Documentos activo
```

## Enviar Tramite Activo A Pendiente

```mermaid
sequenceDiagram
  actor Usuario
  participant Form as RadicacionForm
  participant Api as RadicacionPendientesService
  participant Ctx as RadicacionDocumentalProvider
  participant Tabs as RadicacionTabs

  Usuario->>Form: Clic Enviar a Pendientes
  Form->>Form: Validar contexto estado = 0
  Form->>Api: POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente
  Api-->>Form: estadoActual = 1
  Form->>Ctx: limpiarContextoDocumental()
  Form->>Tabs: Desactivar documentos
  Tabs-->>Usuario: Formulario o estado base
```

## Limpiar Formulario Sin Borrar Contexto Documental

```mermaid
sequenceDiagram
  actor Usuario
  participant Form as RadicacionForm
  participant Antd as AntD Form
  participant Local as Estado Local Formulario
  participant Ctx as RadicacionDocumentalProvider

  Usuario->>Form: Clic Limpiar
  Form->>Antd: resetFields()
  Form->>Local: resetAutocomplete/flujo/tramite/asunto/remitente/destinatario
  Form->>Local: incrementar resetKey si aplica
  Form-->>Ctx: No modificar contexto documental
  Form-->>Usuario: Formulario limpio
```

