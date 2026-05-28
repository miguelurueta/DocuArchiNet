# Prompts de construccion - AppUploadDocumental

## Objetivo

Definir el paquete de prompts necesario para construir la migracion moderna del componente legacy `FileUploadHandler.js`, cubriendo la vista reusable, la orquestacion por lotes, el cliente tecnico de almacenamiento y la especializacion documental.

## Prompts obligatorios

| Orden | Prompt | Resultado esperado |
| --- | --- | --- |
| 1 | `docs/Architecture/AppProgressBatch/PROMPT-IMPLEMENTACION-AppProgressBatch.md` | Componente shared reusable para progreso secuencial por lotes. Ya existe. |
| 2 | `docs/Architecture/AppUploadDocumental/PROMPT-IMPLEMENTACION-AppUploadBatchView.md` | Vista base reusable para cualquier carga de archivos: lista, preview, acciones, estados y slots de metadata. |
| 3 | `docs/Architecture/AppUploadDocumental/PROMPT-IMPLEMENTACION-upload-storage-client.md` | Cliente tecnico sin UI para `init -> chunks -> complete -> cancel -> almacenar`. |
| 4 | `docs/Architecture/AppUploadDocumental/PROMPT-IMPLEMENTACION-AppUploadDocumental.md` | Componente documental final que compone `AppUpload`, `AppUploadBatchView`, `AppProgressBatch` y el cliente de almacenamiento. |

## Regla de construccion

No implementar `AppUploadDocumental` como una copia del HTML legacy. La migracion debe dividir responsabilidades:

```txt
AppUpload
  seleccion, drag/drop y control base de archivos

AppUploadBatchView
  experiencia visual reusable de carga: lista, preview, acciones, contador, estados

AppProgressBatch
  ejecucion secuencial, progreso, cancelacion y resumen

upload-storage-client
  integracion tecnica con API nueva de almacenamiento

AppUploadDocumental
  reglas documentales: tipologia, fecha, configuracion API, sugerencia, payload final
```

## Cobertura quirurgica legacy

Los prompts cubren explicitamente:

- seleccion multiple;
- drag/drop;
- eliminar individual;
- eliminar todos;
- guardar individual;
- guardar todos;
- preview del archivo seleccionado;
- contador de archivos;
- estados por archivo;
- validacion por extension y tamano desde API;
- modo `reject`;
- modo `queue-with-error`;
- tipologia por archivo;
- tipologia obligatoria;
- sugerencia de tipologia por nombre de archivo;
- fecha documental por archivo;
- modos documentales con extension efectiva;
- upload temporal por chunks;
- registro final individual por archivo;
- retorno tipado para registro en interfaz;
- cancelacion con `DELETE upload-temporal`;
- progreso global y por item;
- callbacks React tipados;
- descarte de jQuery, Bootstrap manual, WebForms, XHR legacy y callbacks por string.

## Orden recomendado de implementacion

1. Construir y probar `AppProgressBatch`.
2. Construir `AppUploadBatchView` como vista reusable sin dominio.
3. Construir `upload-storage-client` sin UI.
4. Construir `AppUploadDocumental` integrando las tres piezas.
5. Ejecutar pruebas unitarias, integracion y navegador.

## Criterio de cierre

La construccion se considera lista cuando:

- `AppUploadDocumental` permite subir multiples documentos;
- cada documento puede tener tipologia independiente;
- la fecha por documento se valida cuando el flujo la requiere;
- tipos, tamano y extensiones provienen de API o loaders obligatorios;
- guardar individual y guardar todos funcionan;
- la API nueva se usa por chunks y registro final individual;
- los datos devueltos para actualizar la interfaz se emiten como eventos tipados, no como callbacks string;
- la UI puede especializarse para otros tipos de carga mediante `AppUploadBatchView`;
- no queda dependencia runtime de archivos legacy.
