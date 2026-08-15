# Contrato ASMX

Endpoint: `POST /webservice/WebServiceWorkflowModern.asmx/PreviewEnviarTarea`.

Entrada JSON:

```json
{ "idTarea": 123 }
```

El usuario, grupo, ruta, login y credenciales no se aceptan en la solicitud. El code-behind usa `WorkflowPreviewSessionContextGate`: para cada sesión Gestión autenticada valida la relación servidor `remit_dest_interno.Relacion_Workflow`, establece solo las claves relacionadas desde el login Workflow y obtiene en servidor los snapshots de conexión Workflow y Docuarchi. No invoca la inicialización legacy completa, no registra auditoría ni carga permisos o scripts. El snapshot Docuarchi solo se usa para el estado documental de rutas y no se serializa.

La respuesta ASMX contiene el envoltorio estándar `d` y una `PrevisualizacionTransicionDto`: `IdTarea`, `Origen`, `TipoDecision`, `Contexto`, `Destinos`, `Requisitos`, `RequiereNotificacion`, `TokenVersion` y `Error`. No expone SQL, credenciales, sesión ni detalle de excepciones. La guía consumible por JavaScript está en [05-consumo-frontend-asmx.md](05-consumo-frontend-asmx.md).

Ejemplo de respuesta permitida:

```json
{
  "d": {
    "IdTarea": 123,
    "Origen": "10",
    "TipoDecision": "FLUJO",
    "Contexto": {
      "Radicado": "RAD-123",
      "ActividadOrigen": "10",
      "GrupoActual": "Grupo Workflow"
    },
    "Destinos": [
      {
        "Id": 7,
        "IdActividadDestino": 11,
        "Nombre": "Revisión",
        "Destinatario": "Usuario destino - Cargo",
        "Grupo": "Grupo destino",
        "Tipo": "FLUJO",
        "Orden": 1
      }
    ],
    "Requisitos": [],
    "RequiereNotificacion": false,
    "TokenVersion": "42",
    "Error": null
  }
}
```

## Códigos funcionales

| Código | Significado |
| --- | --- |
| `WORKFLOW_CONTEXT_INVALID` | Sesión incompleta, no autenticada o sin relación Gestión → Workflow válida. |
| `WORKFLOW_MODERN_INACTIVE` | Gate apagado o usuario/grupo fuera del piloto. |
| `WORKFLOW_TASK_INVALID` / `WORKFLOW_TASK_UNAVAILABLE` | Tarea inválida, cerrada o no autorizada. |
| `WORKFLOW_ROUTE_CLOSED` | Ruta no disponible. |
| `WORKFLOW_CONNECTOR_UNAVAILABLE` / `WORKFLOW_TRANSITION_INCONSISTENT` | Origen o destino inconsistente. |
| `WORKFLOW_NO_DESTINATIONS` | No hay destinos autorizados para mostrar. |

El gate queda apagado por defecto en `Web.config` mediante `WorkflowCentroTrabajoModernActive=false`. Para un piloto se habilita explícitamente y se restringe con `WorkflowCentroTrabajoModernUsers` o `WorkflowCentroTrabajoModernGroups`; exclusiones prevalecen mediante las claves `...ExcludedUsers` y `...ExcludedGroups`.

`TIPO_RUTA_ABIERTA_CERRADA` y `TIPO_ABIERTA_CERRADA_ACTIVIDAD` no generan un bloqueo en este contrato: expresan libertad de asignación y el preview devuelve los conectores autorizados aunque tengan un valor distinto de cero.
