# SCRUM-241 Pruebas

## Unitarias E Integracion

Validado con:

```txt
npx eslint src/modules/digitalizacion --ext .ts,.tsx
npx vitest run src/modules/digitalizacion
```

Resultado de Vitest:

- 6 test files passed.
- 32 tests passed.

## Cobertura Funcional

La suite cubre:

- Render modo `crear`.
- Render modo `adjuntar`.
- Contexto invalido.
- Documento destino requerido.
- Cancelacion.
- Reset por cambio de contexto.
- Dispositivos scanner.
- Captura de paginas.
- Rotar pagina.
- Eliminar pagina.
- Generar PDF.

## Interaccion Browser

Los tests ejercitan selector de scanner, boton de escaneo, botones de pagina y generacion de PDF usando un `DigitalizacionScannerClient` fake.

## Regresion

Se mantiene compatibilidad con pruebas previas de FE-01/FE-02:

- Contratos del modulo.
- Hook documental.
- Hook scanner.
- Dynamsoft adapter.
- Loader de scripts.

## Pendientes

- E2E real con scanner/runtime.
- Persistencia backend.
- Metadata avanzada cuando exista contrato final.
