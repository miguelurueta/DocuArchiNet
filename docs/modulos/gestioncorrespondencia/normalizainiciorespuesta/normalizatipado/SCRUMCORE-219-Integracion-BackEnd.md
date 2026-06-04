# SCRUMCORE-219 - Integracion BackEnd

## Contrato backend

El endpoint actual no cambia. El frontend acepta variantes existentes o futuras compatibles del campo de identificador de respuesta radicado.

Variantes soportadas:

- `idRespuestaRadicado`
- `IdRespuestaRadicado`
- `ID_RESPUESTA_RADICADO`
- `id_respuesta_radicado`

## Normalizacion frontend

Todas las variantes se traducen a:

- `idRespuestaRadicado`

Esta traduccion ocurre exclusivamente en `mapEstructuraRespuesta`.

## Compatibilidad legacy

Payloads sin `idRespuestaRadicado` siguen funcionando.

Los campos existentes no cambian:

- `Radicado`
- `Destinatario`
- `TramiteDocumento`

No se modifica el contrato backend ni se agrega endpoint.
