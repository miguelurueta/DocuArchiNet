# Controles de validación y ejecución

```mermaid
flowchart TD
    Inicio[EjecutarEnvioUsuario] --> Contexto{Contexto válido\ny CAMBIO_USUARIO?}
    Contexto -- No --> Bloqueado[Respuesta funcional bloqueada]
    Contexto -- Sí --> Forma{Solicitud y token válidos?}
    Forma -- No --> Bloqueado
    Forma -- Sí --> Lock[GET_LOCK por tarea y token]
    Lock --> Lease{Lock adquirido?}
    Lease -- No --> Bloqueado
    Lease -- Sí --> Repermiso[Reconsultar CAMBIO_USUARIO]
    Repermiso --> Vigencia{Tarea, token, ruta y flujo vigentes?}
    Vigencia -- No --> Bloqueado
    Vigencia -- Sí --> Destino{Usuario, actividad, ruta\ny UTIL_ASIGNA_TAREA vigentes?}
    Destino -- No --> Bloqueado
    Destino -- Sí --> Respuesta{Respuesta = YES?}
    Respuesta -- No --> Bloqueado
    Respuesta -- Sí --> Ejecutar[Adaptador legacy exclusivo]
    Ejecutar --> Auditoria[Auditoría sanitizada]
    Auditoria --> Resultado[Resultado público + advertencias]
    Bloqueado --> Liberar[Liberar lease]
    Resultado --> Liberar
```

Preview no recorre este flujo: usa solo las validaciones y lecturas necesarias para presentar una página de destinos autorizados. Ningún bloqueo invoca el adaptador mutante.
