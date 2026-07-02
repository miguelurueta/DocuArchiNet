# Diagramas De Estado Frontend

## Estado Del Modulo

```mermaid
stateDiagram-v2
  [*] --> IniciandoModulo
  IniciandoModulo --> ConsultandoEstadoActivo

  ConsultandoEstadoActivo --> FormularioRadicacion: sin activo estado 0
  ConsultandoEstadoActivo --> ContextoDocumentalActivo: activo estado 0
  ConsultandoEstadoActivo --> FormularioRadicacion: error controlado

  FormularioRadicacion --> Radicando: usuario radica
  Radicando --> ContextoDocumentalActivo: requiere gestion documental
  Radicando --> FormularioRadicacion: sin gestion documental
  Radicando --> FormularioRadicacion: error

  ContextoDocumentalActivo --> PanelDocumentos: navegar a documentos
  PanelDocumentos --> EnviandoAPendiente: usuario envia a pendiente
  EnviandoAPendiente --> TramitePendiente: backend estado 1
  TramitePendiente --> FormularioRadicacion: contexto limpiado

  FormularioRadicacion --> ModalPendientes: abrir pendientes
  ModalPendientes --> TomandoPendiente: asignacion-tarea
  TomandoPendiente --> ContextoDocumentalActivo: backend estado 0
  TomandoPendiente --> ModalPendientes: error o bloqueo
```

## Estado Documental

```mermaid
stateDiagram-v2
  [*] --> SinContexto

  SinContexto --> ActivoEstado0: radicacion exitosa con gestion documental
  SinContexto --> ActivoEstado0: tomar pendiente
  SinContexto --> ActivoEstado0: inicio detecta estado activo

  ActivoEstado0 --> PendienteEstado1: enviar a pendiente
  PendienteEstado1 --> SinContexto: limpiar contexto frontend
  PendienteEstado1 --> ActivoEstado0: tomar pendiente nuevamente

  ActivoEstado0 --> SinContexto: cierre de sesion o cambio controlado de modulo
```

## Guard De Documentos

```mermaid
stateDiagram-v2
  [*] --> Evaluando
  Evaluando --> DocumentosDisabled: sin idEstadoRadicado
  Evaluando --> DocumentosDisabled: estado distinto de 0
  Evaluando --> DocumentosDisabled: requiereGestionDocumental false
  Evaluando --> DocumentosEnabled: estado 0 e idEstadoRadicado valido

  DocumentosEnabled --> RenderCapDocument
  DocumentosDisabled --> BloquearRender
```

## Matriz De Habilitacion UI

| Estado frontend | Tab Documentos | Boton Enviar a Pendiente | Modal Pendientes | Accion Tomar |
|---|---|---|---|---|
| Sin contexto | Inactivo | Oculto/Inactivo | Activo | Activa |
| Activo `estado = 0` | Activo | Activo | Visible con bloqueo de toma | Inactiva o bloqueada |
| Pendiente `estado = 1` | Inactivo | Inactivo | Activo | Activa |
| Error de inicio | Inactivo | Inactivo | Activo si el modulo cargo | Activa con validacion backend |

