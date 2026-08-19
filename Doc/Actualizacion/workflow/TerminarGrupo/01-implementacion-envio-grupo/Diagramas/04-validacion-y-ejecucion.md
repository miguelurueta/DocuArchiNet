# Controles de validación y ejecución

```mermaid
flowchart TD
    Inicio[EjecutarEnvioGrupo] --> Gate{Gate activo?}
    Gate -- No --> Bloqueado[Respuesta funcional bloqueada]
    Gate -- Sí --> Permiso{Cambio_Ruta efectivo?}
    Permiso -- No --> Bloqueado
    Permiso -- Sí --> Lock[GET_LOCK por tarea]
    Lock --> Relectura[Releer tarea y token]
    Relectura --> Vigente{Tarea, ruta, flujo y actividad vigentes?}
    Vigente -- No --> Bloqueado
    Vigente -- Sí --> Destino{Destino pertenece a la ruta?}
    Destino -- No --> Bloqueado
    Destino -- Sí --> Aprobacion{Aprobación pendiente?}
    Aprobacion -- Sí --> Bloqueado
    Aprobacion -- No --> Ejecutar[Adaptador legacy directo]
    Ejecutar --> Auditoria[Auditoría sanitizada]
    Auditoria --> Resultado[Resultado público]
    Lock --> Liberar[LIBERAR_LOCK]
    Resultado --> Liberar
```

Solo el adaptador legacy ejecuta la transición. Las consultas de destinos y validación son de solo lectura; los bloqueos funcionales no cambian tarea, estado ni auditoría.
