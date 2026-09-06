# Arquitectura

`IExternalImportProviderClient` permanece en Modelo y define capacidades, consulta, preview y descarga asíncronos con `CancellationToken`. Los adaptadores concretos consumen esta interfaz; el cliente SII corresponde a Backend 06.

La infraestructura se divide en:

- `ExternalImportHttpClientFactory`: cliente estable e inyectable, timeout global infinito y TLS del sistema.
- `ExternalImportHttpTransport`: mensaje por solicitud, token enlazado y envío con `ResponseHeadersRead`.
- `ExternalImportHttpResponseValidator`: estado, MIME y lectura acotada.
- `ExternalImportHttpErrorMapper`: categorías seguras y correlación.

No se mutan `BaseAddress`, defaults de encabezado, timeout o callbacks globales durante una solicitud. El transporte legacy se conserva intacto.
