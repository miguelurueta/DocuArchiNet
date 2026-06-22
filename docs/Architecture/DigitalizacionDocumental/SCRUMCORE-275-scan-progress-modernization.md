# SCRUMCORE-259 / SCRUMCORE-275 - Modernizacion de progreso de escaneo

## Contexto

La experiencia nativa `PaperStream IP` que aparece durante `AcquireImage()` pertenece al driver del scanner. DocuArchi no puede modificar visualmente ese dialogo desde React ni reemplazar sus textos, barra o boton de cancelacion interno.

DocuArchi si controla el estado del scanner, el preview PDF, el toolbar, las miniaturas, el overlay de carga y el procesamiento posterior. Este cambio concentra esos estados en un unico overlay corporativo minimalista sobre el preview para evitar indicadores dispersos y mensajes tecnicos en la UI.

## Auditoria Dynamsoft

El contrato local `DynamsoftWebTwainObject` usado por el proyecto expone `AcquireImage(options, onSuccess, onFailure)`. No expone eventos tipados y estables para progreso por pagina durante la adquisicion nativa. Por esa razon:

- Durante `AcquireImage()` el overlay muestra estado indeterminado: `Escaneando documentos`.
- La pagina actual y total solo son confiables despues de que Dynamsoft actualiza `HowManyImagesInBuffer`.
- El avance posterior si es controlado por DocuArchi: construccion de paginas, eliminacion de paginas en blanco, Deskew, Auto Crop, Auto Rotate, generacion de PDF y preparacion final. La UI agrupa esos estados en mensajes corporativos simples.

## Estados Soportados

| Estado visible | Origen | Observacion |
| --- | --- | --- |
| Escaneando documentos | `useDigitalizacionScanner.scan()` + `DynamsoftTwainClient.scan()` | Indeterminado mientras corre el driver. |
| Procesando documentos | Despues de `AcquireImage()` y procesamiento posterior | Agrupa construccion de paginas, blank-page removal, Deskew, Auto Crop, Auto Rotate y preparacion final. |
| Generando PDF | `generatePdf()` | Se muestra durante la generacion del documento final. |

## Diseno Final

Mockup textual:

```text
+----------------------------------------------------+
| Preview PDF                                        |
|                                                    |
|                [Loader Contasoft]                  |
|                Escaneando documentos               |
|                                      [Cancelar]    |
|                                                    |
+----------------------------------------------------+
```

El overlay vive dentro de `DigitalizacionDocumentalWorkspace` y se alimenta con `scanner.progress`. No desmonta el preview, no cambia miniaturas y no reemplaza el dialogo nativo PaperStream cuando el usuario usa modo driver.

La experiencia visible queda reducida a loader corporativo Contasoft, un unico texto de estado y el boton `Cancelar operacion` cuando aplica. No se muestran barras de progreso, porcentajes, paginas actuales/totales, SDKs, runtime, drivers ni mensajes tecnicos internos.

## Eliminacion De Loaders Duplicados

Auditoria:

| Indicador | Archivo | Condicion | Decision |
| --- | --- | --- | --- |
| Overlay corporativo | `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx` | `scanner.progress` o `scanner.loading` | Se mantiene como fuente unica. |
| Spinner/loader historico del preview | Scope `Preview digitalizacion`, clase global Ant `ant-spin` | Puede aparecer si un loader heredado queda montado dentro del preview durante operaciones asincronicas | Se suprime cuando el preview tiene `data-progress-active="true"`. |
| Loader visual Dynamsoft Web TWAIN | `body > .dynamsoft-dialog-wrap .ds-dwt-loaderBar` | Dynamsoft lo inyecta durante operaciones del runtime como `AcquireImage()` | Se elimino el loader visual duplicado generado por Dynamsoft Web TWAIN mediante CSS global. |
| Skeleton/loader de `AppVisorEmbedPdf` | `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx` | Prop `loading` | No participa en el flujo actual de `AppDigitalizador`, pero queda identificado como loader historico del visor PDF. |

Regla de render:

- Si `scanner.progress` existe, el overlay corporativo usa ese snapshot.
- Si `scanner.progress` aun no existe pero `scanner.loading` es `true`, el workspace deriva un snapshot visual desde `scanner.status`.
- El texto visible se normaliza a `Escaneando documentos`, `Procesando documentos` o `Generando PDF`.
- Mientras el overlay corporativo esta activo, `Preview digitalizacion` expone `data-progress-active="true"` y oculta spinners Ant heredados dentro de ese scope.
- El loader visual `ds-dwt-loaderBar` generado por Dynamsoft Web TWAIN se oculta globalmente porque el SDK lo monta sobre `body`, fuera del scope React del preview.
- La supresion de `ds-dwt-loaderBar` no oculta el dialogo nativo PaperStream IP, el indicador propio del scanner, ni el overlay corporativo DocuArchi.
- El footer reutiliza el mismo estado simplificado; no crea un segundo estado visual.

## Riesgos

- El avance durante adquisicion real depende del driver. Mostrar un porcentaje exacto antes del retorno de `AcquireImage()` seria enganoso.
- `Cancelar operacion` cancela el flujo DocuArchi; la cancelacion interna del driver sigue dependiendo del dialogo PaperStream.
- Si Dynamsoft agrega eventos tipados estables en el futuro, deben conectarse al mismo contrato `ScanProgressSnapshot` sin cambiar la UI.
- La supresion del spinner historico esta acotada al preview y solo durante progreso activo para no afectar loaders de otros modulos.

## Evidencia Tecnica

- `src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.types.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`
- `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts`
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx`
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.module.css`
