# Diseño técnico — DOC-51 clientes HTTP asíncronos y seguridad

## Contexto

DOC-50 publicó DTO v1 y puertos del núcleo. DOC-51 agrega una frontera HTTP reutilizable entre casos de uso backend y adaptadores concretos. `Class_ClassResfull.vb` solo caracteriza contratos: su I/O bloqueante, configuración mutable y callback global no pasan al recorrido moderno.

## Objetivos y no objetivos

Se implementan transporte asíncrono, cancelación/timeout, validación acotada, errores seguros, fixtures y pruebas locales. No se implementan reglas SII, endpoints ASMX, persistencia, almacenamiento, UI ni gate.

## Decisiones

### D-01 — Puerto asíncrono en el núcleo

`ImportarServicioWebInterfaces.vb` declara `IExternalImportProviderClient` con capacidades, consulta, preview y descarga basadas en `Task` y `CancellationToken`. Reutiliza contratos tipados de DOC-50, infraestructura no lo redefine y el puerto síncrono continúa intacto.

### D-02 — Solicitud preparada e inmutable

El transporte recibe un descriptor que fija URI, método, encabezados permitidos, contenido, media type, timeout, límite y `correlationId`. No decide JSON contra formulario ni reserializa cuerpos; el adaptador futuro aporta ese contrato. Se copian colecciones y validan invariantes al construirlo.

### D-03 — Fábrica y TLS seguros

`ExternalImportHttpClientFactory` suministra cliente/handler estable, inyectable y con validación TLS predeterminada. No modifica `DefaultRequestHeaders`, `BaseAddress`, `Timeout`, `ServicePointManager` ni callbacks globales durante una petición.

### D-04 — Cancelación distinta de timeout

El transporte combina el token del llamador con timeout por operación. `OperationCanceledException` es cancelación si el token externo fue solicitado y timeout si expiró solo el interno. Se prohíben `.Result`, `.Wait`, `GetAwaiter().GetResult` y `Task.Run`.

### D-05 — Validación y lectura acotada

El validador comprueba status, media type y `Content-Length`; luego lee el stream por bloques con cancelación y detiene al exceder el máximo, incluso con chunked o longitud falsa. Solo después deserializa JSON; descargas devuelven bytes y metadatos saneados.

### D-06 — Errores tipados y observabilidad

El mapper distingue `EXTERNAL_ACCESS_DENIED`, `EXTERNAL_TIMEOUT`, `EXTERNAL_CANCELLED`, `EXTERNAL_UNAVAILABLE` y `EXTERNAL_INVALID_RESPONSE`. Mensajes públicos/registrables incluyen `correlationId`, pero no credenciales, tokens, query, URL completa, rutas, encabezados secretos ni cuerpo.

### D-07 — Preview mediado

El puerto entrega un recurso validado para construir `GetPreviewResponseDto`; no expone URL externa o respuesta cruda. Autorización y entrega al navegador pertenecen a fronteras posteriores y DOC-51 no publica endpoints.

### D-08 — Prueba local y documentación

Pruebas CommonJS inspeccionan garantías estructurales y usan servidor HTTP loopback para escenarios deterministas. Fixtures `http-v1` son saneados. La documentación registra formatos, opciones ambientales y que SII/gate no participan.

## Flujo

1. El cliente futuro prepara solicitud y política.
2. La fábrica entrega cliente estable y el transporte crea mensaje local.
3. Se envía con token enlazado solicitando primero encabezados.
4. Se valida status, MIME y tamaño y se lee en forma acotada.
5. Se retorna contenido validado o error tipado seguro.

## Riesgos y mitigaciones

- `System.Net.Http` antiguo: usar APIs compatibles con .NET Framework 4.6.1 y compilar.
- Límites SII desconocidos: hacerlos obligatorios/configurables, sin valores productivos ficticios.
- Pruebas Node no sustituyen integración VB completa: combinarlas con build y dejar cobertura SII para Backend 06.
- ASMX puede no propagar `Task`: no crear adaptadores bloqueantes; decidir frontera web posteriormente.

## Migración y reversión

El cambio es aditivo y desconectado del legacy. Revertir retira nuevas entradas, clases, puerto y pruebas. No hay migración de datos ni configuración productiva.
