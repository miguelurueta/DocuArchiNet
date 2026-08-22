# DOC-29 — Secuencia de interfaz moderna de Enviar a usuario

```mermaid
sequenceDiagram
    participant U as Usuario
    participant P as Webworkflow.aspx
    participant UI as WorkflowUserSendUi
    participant C as ConfirmationDialog
    participant S as WebServiceWorkflowModern
    participant V as Presentación parcial

    P->>UI: bootstrap propio, sin consultar gate
    U->>UI: activar Enviar a usuario
    UI->>S: PreviewEnviarUsuario(idTarea, consulta, cursor, tamanoPagina)
    S-->>UI: JSON paginado + token + destinos autorizados
    UI->>C: workflow:user-destination-selected
    U->>C: confirmar destino
    C->>S: EjecutarEnvioUsuario(usuario, actividad, token)
    alt éxito correlacionado
        S-->>C: resultado exitoso
        C->>V: retirar solo tarea, visor y contador
        V-->>U: mensaje de éxito propio
    else bloqueo, cancelación o respuesta obsoleta
        S-->>C: bloqueo o error seguro
        C-->>U: contexto restaurado sin postback legacy
    end
```

La secuencia no contiene `IdConector`, controles ocultos, `Cambia_Estado`, `After_envio_usuario_workflow` ni reasignación de respuesta. `WorkflowTransitionUi` permanece fuera de este recorrido.
