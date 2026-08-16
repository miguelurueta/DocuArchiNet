# Contrato de datos y callback

## Entrada

La única llamada de red de DOC-12 es un `POST` de mismo origen a `../webservice/WebServiceWorkflowModern.asmx/PreviewEnviarTarea`:

```json
{ "idTarea": 41 }
```

`idTarea` proviene del control visual de tarea seleccionada. No aporta usuario, grupo, ruta, actividad, permisos ni datos de autorización; el ASMX los vuelve a validar.

## Resultado usado

La UI desempaqueta `d` y solo conserva estos campos publicados:

```json
{
  "IdTarea": 41,
  "TipoDecision": "Flujo documental",
  "Contexto": {
    "Radicado": "RAD-41",
    "GrupoActual": "Gestión documental"
  },
  "Destinos": [
    {
      "Id": 8,
      "Nombre": "Revisión",
      "Destinatario": "Ana",
      "Grupo": "",
      "Tipo": "Flujo",
      "Orden": 1
    }
  ],
  "TokenVersion": "v-41",
  "Error": null
}
```

Por decisión de alcance, no se presenta trámite ni actividad actual legible. Tampoco se deriva información desde IDs, HTML legacy, campos ocultos, sesión o reglas de negocio.

## Estados de la interfaz

| Estado | Mensaje y recuperación |
| --- | --- |
| `cargando` | Anuncia la carga en región viva. |
| `sin-destinos` | Indica que no hay destinos disponibles. |
| `error-controlado` | Muestra un mensaje seguro y ofrece Reintentar. |
| `lista-disponible` | Expone tabla de escritorio y tarjetas móviles. |
| `destino-seleccionado` | Conserva el modal y confirma visualmente la selección. |

## Callback de selección

La selección ejecuta, como máximo, `WorkflowTransitionUi.onDestinationSelected(detail)` si fue registrado y publica `workflow:destination-selected` en `window`.

```js
window.addEventListener("workflow:destination-selected", function (event) {
  // El adaptador de confirmación posterior consume solo este detalle.
  const detail = event.detail;
});
```

El detalle es:

```json
{
  "idTarea": 41,
  "idConector": 8,
  "tokenVersion": "v-41",
  "destino": {
    "nombre": "Revisión",
    "destinatario": "Ana",
    "grupo": "",
    "tipo": "Flujo"
  }
}
```

DOC-12 no llama una operación de envío, ni botones Web Forms invisibles, ni ejecuta correo, auditoría o cambios de estado.
