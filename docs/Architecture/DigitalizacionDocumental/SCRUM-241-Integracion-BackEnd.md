# SCRUM-241 Integracion BackEnd

## Estado Actual

SCRUMCORE-241 no integra persistencia backend. La fase deja listo el workbench y el PDF generado desde `useDigitalizacionScanner`.

## Contratos Futuros

Para completar persistencia se requiere definir:

- Endpoint para crear documento con archivo PDF.
- Endpoint para adjuntar PDF a `idDocumentoDestino`.
- Contrato de metadata TRD/tipologia/lista de chequeo.
- Manejo de errores funcionales de unicidad, permisos y validacion documental.

## Payload Esperado

Campos minimos proyectados:

- `modo`: `crear` o `adjuntar`.
- `nombreGabinete`.
- `radicado`.
- `idDocumentoDestino` cuando aplique.
- `pdf`.
- `metadata` documental resuelta.

## Scanner

El backend no debe recibir paginas sueltas como source-of-truth de preview. La UI debe enviar el PDF generado por el adapter y metadata validada.

## Upload

El boton primario del modal queda como punto de integracion para la fase de upload. Los estados futuros deberian cubrir `uploading`, `saving`, `success` y `error`.
