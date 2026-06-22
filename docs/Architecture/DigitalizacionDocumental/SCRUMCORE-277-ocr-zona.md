# SCRUMCORE-277 - OCR por zona

## Motivacion

SCRUMCORE-260 solicita extraer texto desde una region seleccionada de una pagina digitalizada. La seleccion visual ya existe por SCRUMCORE-269 y se usa para recorte manual, por lo que OCR por zona debe reutilizar ese mismo modelo y no crear otra herramienta de seleccion.

El alcance de este cambio queda limitado a auditoria y diseno tecnico. El ticket indica explicitamente no implementar OCR funcional hasta confirmar licencia, APIs disponibles, idiomas y rendimiento del runtime actual.

## Auditoria Actual

| Punto auditado | Hallazgo | Decision |
| --- | --- | --- |
| SDK frontend | `dwt@19.3.2` cargado desde CDN por `loadDynamsoftScripts()` | Mantener sin cambios. |
| Licencia | La licencia se inyecta como `Dynamsoft.DWT.ProductKey` | No hay evidencia local de entitlement OCR. |
| Contrato DWT local | `DynamsoftWebTwainObject` tipa captura, imagen, crop, deskew, rotacion, borrado y PDF | No hay API OCR tipada. |
| Dependencias frontend | `package.json` no contiene librerias OCR | No instalar OCR en esta fase. |
| Recursos OCR | No hay assets de idioma ni workers OCR en el repo | Requiere validacion futura. |
| Seleccion de area | `DigitalizacionDocumentalWorkspace` conserva `cropSelection` con `PageCropSelection` | Reutilizar como fuente unica de coordenadas. |

Conclusion: el repositorio esta listo para disenar OCR por zona sobre la seleccion existente, pero no hay base suficiente para activar OCR funcional sin una validacion de licencia/runtime.

## Dependencias

- SCRUMCORE-269: seleccion de area y recorte manual.
- `PageCropSelection`: coordenadas reales de pagina `{ x, y, width, height }`.
- `ScanPage`: pagina activa con `pageId`, dimensiones e imagen fuente.
- Dynamsoft Web TWAIN 19.3.2 y licencia instalada.
- Validacion futura de API OCR e idiomas.

## Arquitectura Propuesta

```text
DigitalizacionDocumentalWorkspace
  seleccion actual -> { pageId, selection }
  capability.ocrZone
  boton OCR Zona
  useDigitalizacionOcrZone
  DigitalizacionOcrClient
  adapter OCR validado
  drawer Resultado OCR
```

La UI no debe tocar `DWObject` ni APIs OCR directamente. Debe depender de un cliente inyectable y testeable, similar al patron actual del scanner.

Contrato propuesto para un ticket posterior:

```ts
export type OcrZoneRequest = {
  pageId: string;
  selection: PageCropSelection;
  language?: string;
};

export type OcrZoneResult = {
  text: string;
  confidence?: number;
  durationMs: number;
};

export interface DigitalizacionOcrClient {
  getCapabilities(): Promise<{ ocrZone: boolean; languages: string[] }>;
  recognizeZone(request: OcrZoneRequest): Promise<OcrZoneResult>;
}
```

## Flujo Futuro

1. Usuario activa `Seleccionar area`.
2. Usuario dibuja una region sobre la pagina activa.
3. Si no existe seleccion, `OCR Zona` permanece deshabilitado con tooltip `Seleccione un area primero`.
4. Si existe seleccion y `capability.ocrZone` esta disponible, `OCR Zona` se habilita con tooltip `Extraer texto de la seleccion`.
5. La accion envia solo `{ pageId, selection }` al cliente OCR.
6. El resultado se muestra en drawer lateral con titulo `Texto extraido`.
7. Acciones del drawer:
   - `Copiar`: copia el texto al portapapeles.
   - `Insertar en metadato`: delega en un callback del modulo host.
   - `Cerrar`: cierra el resultado sin modificar paginas.

## Casos De Uso

- Cedulas: extraer numero de documento.
- Facturas: extraer numero de factura, valor o fecha.
- Contratos: extraer radicado o numero de contrato.
- Formularios: extraer campos especificos desde zonas repetibles.

## Riesgos

- La licencia actual puede cubrir solo captura TWAIN y no OCR.
- OCR puede requerir modulos adicionales de Dynamsoft o recursos de idioma no cargados.
- OCR en frontend puede afectar rendimiento si compite con escaneo, Deskew, Auto Crop, Auto Rotate o PDF.
- Una zona con baja resolucion o contraste puede devolver texto pobre.
- `Insertar en metadato` requiere contrato de integracion con el host.
- Si la API OCR opera sobre el buffer DWT, debe garantizarse que no modifique `scanner.pages`, miniaturas ni PDF pendiente.

## Criterios Para Implementacion Posterior

Antes de agregar UI funcional:

1. Confirmar por licencia que OCR u OCR Pro esta disponible.
2. Identificar API exacta del runtime instalado.
3. Confirmar idiomas requeridos y assets necesarios.
4. Probar OCR sobre una region real seleccionada en scanner fisico.
5. Medir tiempo de respuesta sobre zonas pequenas y medianas.
6. Definir contrato de `Insertar en metadato`.
7. Cubrir tests de capacidad no disponible, OCR exitoso, error OCR, copiar e insercion.

## Decision SCRUMCORE-260

No se implementa boton, drawer ni OCR real en esta fase. El entregable es la auditoria tecnica y el diseno de arquitectura para habilitar una implementacion posterior con una compuerta de capacidades validada.
