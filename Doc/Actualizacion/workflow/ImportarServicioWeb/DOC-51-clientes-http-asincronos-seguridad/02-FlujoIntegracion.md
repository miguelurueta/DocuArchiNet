# Flujo de integración

- Ticket: DOC-51
- Cambio OpenSpec: doc-51-servicio-proveedor
- Clasificacion: cross_cutting

1. Un adaptador de proveedor prepara URI, verbo, bytes, media type, encabezados permitidos, timeout, máximo, MIME aceptados y `correlationId`.
2. El transporte copia los valores y construye un `HttpRequestMessage` exclusivo.
3. El token del llamador se enlaza con el timeout de la operación.
4. La respuesta se recibe primero como encabezados.
5. El validador comprueba estado, MIME y longitud; después lee por bloques hasta el máximo.
6. Se entrega contenido validado o una excepción tipada con mensaje saneado.

`GetPreview` sigue mediado por backend: DOC-51 no retorna una URL externa al navegador ni realiza efectos sobre tarea o documentos.
