# Contrato del ASMX

## Operación

`POST webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioTarea`

El ASMX conserva el contenedor estándar `{ "d": ... }` y sesión habilitada.

```json
{
  "idTarea": 123,
  "idConector": 45,
  "tokenVersion": "version-mostrada-por-preview"
}
```

Los tres valores son intención del cliente, no autorización. El servidor no acepta usuario, grupo, ruta, actividad, permisos, destino ni cadena de conexión del navegador.

## Respuesta

```json
{
  "d": {
    "Exito": true,
    "EstadoFinal": "completada",
    "MensajeFuncional": "La tarea fue enviada",
    "CodigoBloqueo": null,
    "Advertencias": [],
    "ActividadDestino": "Actividad autorizada",
    "Destino": {
      "Id": 45,
      "IdActividadDestino": 77,
      "Nombre": "Actividad autorizada",
      "Destinatario": null,
      "Grupo": "GRUPO",
      "Tipo": "RUTA"
    },
    "TokenVersion": "version-mostrada-por-preview",
    "ReferenciaAuditoria": "WF-MOD-xxxxxxxxxxxxxxxx",
    "EsReintentable": false,
    "Requisitos": [],
    "Error": null
  }
}
```

Los nombres y valores anteriores son ilustrativos. No se retornan SQL, excepciones, controles, `Session`, HTML, credenciales ni textos originales del motor legacy.

## Validación y códigos

| Situación | Código | Efecto |
| --- | --- | --- |
| Sesión, relación o permiso inválido | `WORKFLOW_CONTEXT_INVALID` | No se resuelve ni ejecuta destino. |
| Feature gate fuera del piloto | `WORKFLOW_MODERN_INACTIVE` | No hay fallback al envío legacy. |
| Tarea, conector o token inválido | `WORKFLOW_TASK_INVALID`, `WORKFLOW_VERSION_INVALID` | Bloqueo sin consulta de ejecución. |
| Token vencido/tarea ya cambiada | `WORKFLOW_VERSION_CONFLICT` | Reintentar requiere nuevo preview. |
| Conector ajeno al contexto actual | `WORKFLOW_CONNECTOR_UNAVAILABLE` o `WORKFLOW_TRANSITION_INCONSISTENT` | No se invoca el motor. |
| Respuesta o aprobación pendiente | `WORKFLOW_REQUIREMENT_NOT_MET` | Se conserva el estado de tarea. |
| Solicitud concurrente | `WORKFLOW_TRANSITION_IN_PROGRESS` | A lo sumo una llega al motor. |
| Fallo controlado del motor o infraestructura | `WORKFLOW_TRANSITION_REJECTED` o `WORKFLOW_TRANSITION_UNAVAILABLE` | Resultado normalizado y seguro. |

## RUTA y FLUJO

Para RUTA se valida grupo, ruta, actividad fuente, conector, actividad real destino y correo. Para FLUJO se validan flujo, nodo fuente, actividad fuente y usuario/grupo fuente; además se conserva la diferencia entre actividad real destino y actividad de flujo requerida por `Terminar_Tarea_Workflow`.

## Idempotencia

`GET_LOCK` usa la clave determinista de tarea y token. Dentro de ese lease se releen tarea y destino. Después de un envío exitoso, un reintento con el token anterior debe quedar en conflicto o tarea no disponible; no abre una segunda transacción ni actualiza estados directamente.

## Compatibilidad

`PreviewEnviarTarea` no cambia. La interfaz Web Forms actual mantiene su recorrido y no se oculta una tarea por una respuesta simulada: la interfaz consumidora debe hacerlo solo ante `Exito=true`.
