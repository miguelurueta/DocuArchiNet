# Contratos y mapping

DOC-101 no introduce upload, endpoint, DTO ni esquema. El nombre del documento se mantiene por el formato canónico.

## Contrato del cargador

| Entrada/salida | Regla |
| --- | --- |
| `name` | Uno de `graph`, `wacom`, `print`, `fabric`, `prettify` |
| retorno | `Promise<string>` con el nombre listo |
| concurrencia | Misma promesa mientras la capacidad carga |
| éxito | Todos los recursos requeridos y símbolo final presentes |
| fallo | Rechazo saneado; nodo fallido removido y promesa liberada para reintento |

Touch-punch es el único recurso tolerado como opcional dentro de un manifiesto: su fallo no invalida jQuery UI/contextMenu. Los demás recursos son obligatorios para su capacidad.

## Evidencia de red

El runner solo persiste conteos enteros con claves cerradas: `core`, `graph`, `wacom`, `print`, `fabric`, `prettify` y `other`. La clasificación usa temporalmente el pathname, pero la evidencia no conserva URL, query, tarea, documento, usuario, cookie ni credencial.

No cambian `Handler_image_wf.ashx`, los WebMethods de iText, los parámetros de impresión ni la autenticación existente.
