# Consumo frontend del ASMX

Esta guía describe el único contrato que puede consumir el frontend. No agrega una UI nueva ni reemplaza `workflow/Webworkflow.aspx`.

## Precondiciones

1. La página se sirve desde el mismo origen de la aplicación autenticada.
2. El usuario ya inició sesión en Workflow; la cookie de Forms Authentication y la sesión ASP.NET se envían con `credentials: 'same-origin'`.
3. El servidor debe tener el gate activo solo para el piloto. El navegador no envía usuario, grupo, actividad, ruta, permisos ni token como autorización.
4. El único dato permitido en la solicitud es un entero positivo `idTarea`.

## Llamada JavaScript

```javascript
async function previewEnviarTarea(idTarea) {
  if (!Number.isSafeInteger(idTarea) || idTarea <= 0) {
    throw new Error('Seleccione una tarea válida.');
  }

  const response = await fetch(
    '/webservice/WebServiceWorkflowModern.asmx/PreviewEnviarTarea',
    {
      method: 'POST',
      credentials: 'same-origin',
      headers: {
        'Content-Type': 'application/json; charset=utf-8',
        'Accept': 'application/json',
        'X-Requested-With': 'XMLHttpRequest'
      },
      body: JSON.stringify({ idTarea })
    }
  );

  if (!response.ok) {
    throw new Error('No fue posible consultar la previsualización.');
  }

  const envelope = await response.json();
  if (!envelope || !envelope.d) {
    throw new Error('La respuesta del preview no tiene el contrato esperado.');
  }

  return envelope.d;
}
```

El ASMX usa nombres .NET en PascalCase en la respuesta: `IdTarea`, `Destinos`, `Contexto` y `Error`. El valor de `TokenVersion` se muestra o conserva para una futura fase de envío; DOC-10 no ofrece ningún método de ejecución.

## Manejo de resultado

```javascript
async function cargarPreview(idTarea, contenedor, mensaje) {
  try {
    const preview = await previewEnviarTarea(idTarea);

    if (preview.Error) {
      mensaje.textContent = preview.Error.MensajeVisible;
      contenedor.replaceChildren();
      return preview;
    }

    const items = preview.Destinos.map((destino) => {
      const option = document.createElement('button');
      option.type = 'button';
      option.dataset.conectorId = String(destino.Id);
      option.textContent = `${destino.Nombre}${destino.Destinatario ? ` — ${destino.Destinatario}` : ''}`;
      return option;
    });

    mensaje.textContent = '';
    contenedor.replaceChildren(...items);
    return preview;
  } catch (error) {
    mensaje.textContent = 'No fue posible cargar los destinos.';
    contenedor.replaceChildren();
    throw error;
  }
}
```

Se usa `textContent` y `replaceChildren`; no se debe insertar con `innerHTML` ningún valor del ASMX. Un bloqueo funcional se muestra tal cual mediante `Error.MensajeVisible`, sin inferir permisos ni intentar el flujo legacy.

## Matriz de respuestas

| Resultado | Frontend permitido | Prohibido |
| --- | --- | --- |
| `Error = null` y destinos | Mostrar destinos en orden y contexto seguro. | Ejecutar transición, cambiar estado o confiar en IDs adicionales. |
| `WORKFLOW_MODERN_INACTIVE` | Informar que el preview no está habilitado. | Reintentar con otro usuario/grupo o hacer fallback automático. |
| `WORKFLOW_CONTEXT_INVALID` | Pedir renovar sesión o volver a iniciar sesión. | Fabricar valores de sesión desde JavaScript. |
| `WORKFLOW_TASK_*`, `WORKFLOW_ROUTE_CLOSED`, `*_INCONSISTENT`, `WORKFLOW_NO_DESTINATIONS` | Mostrar el mensaje funcional y limpiar destinos. | Exponer el detalle técnico o usar información anterior en caché. |
| Error HTTP/red | Mostrar mensaje genérico y registrar solo telemetría autorizada. | Mostrar cuerpo de excepción, cookies o configuración. |

La respuesta no se debe almacenar de forma persistente: tarea, destinos y token pueden cambiar en el flujo legacy. Para volver a consultar, se llama de nuevo al endpoint con el mismo `idTarea` y se vuelve a interpretar la respuesta.
