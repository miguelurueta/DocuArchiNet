# DOC-102 — componentes y reglas

## Flujo técnico

```text
[Cliente / PDF.js]
        | GET o HEAD + doc_ref + Range/condicionales
        v
[Handler_image_wf.ashx  (endpoint; equivale al Controller)]
        | valida método y resuelve referencia
        v
[DocumentoStreamingReferencia + RangoHttpDocumento + CondicionesHttpDocumento
 (servicios puros de autorización y protocolo)]
        | ruta canónica autorizada + intervalo
        v
[FileStream  (repositorio de solo lectura del archivo)]
        | bloques de hasta 64 KiB
        v
[Respuesta HTTP 200/206/304/416]
```

Esta operación legacy no atraviesa un Controller MVC, Service de negocio ni Repository de base de datos. El handler es el endpoint, los componentes de `Infrastructure/Documentos/Streaming` encapsulan reglas y `FileStream` es la única fuente de bytes. No hay escritura de sesión, tarea, documento o auditoría.

## Servicios y reglas

- El token contiene versión, vencimiento, sesión, ruta canónica, nombre y MIME bajo protección `MachineKey`; la ruta no aparece en la URL.
- La resolución falla cerrada ante alteración, expiración, otra sesión, traversal o archivo inexistente.
- Solo se acepta un rango `bytes=start-end`, `bytes=start-` o `bytes=-suffix`; los rangos múltiples o inválidos responden `416`.
- `If-None-Match` precede a `If-Modified-Since`; `If-Range` débil, inválido o desactualizado degrada a `200` completo.
- Los errores públicos son genéricos y no incluyen excepciones ni rutas.
