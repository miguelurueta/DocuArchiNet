## Context

SCRUMCORE-260: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- OCR-ZONA.

El ticket solicita OCR sobre una zona seleccionada, reutilizando la seleccion visual creada para recorte manual en SCRUMCORE-269. El mismo ticket establece una restriccion explicita: solo auditoria y diseno tecnico, sin implementar OCR hasta confirmar capacidades de licencia y runtime.

## Goals / Non-Goals

**Goals**
- Auditar el estado actual de Dynamsoft Web TWAIN y del contrato local de digitalizacion frente a OCR.
- Definir una arquitectura futura para OCR por zona que reutilice `PageCropSelection`.
- Documentar flujo UX, dependencias, riesgos y criterios para habilitar implementacion posterior.
- Evitar introducir botones, drawers o llamadas OCR funcionales sin confirmacion de licencia.

**Non-Goals**
- Ejecutar OCR real en SCRUMCORE-260.
- Agregar el boton `OCR Zona` al toolbar actual.
- Crear un segundo mecanismo de seleccion de area.
- Instalar librerias OCR, assets de idiomas, workers o servicios nuevos.
- Exponer `DWObject` directamente desde React.

## Auditoria Tecnica

1. El runtime configurado sigue siendo `dwt@19.3.2`, cargado desde CDN mediante `loadDynamsoftScripts()`.
2. La licencia actual se inyecta como `Dynamsoft.DWT.ProductKey`, pero el repositorio no contiene evidencia local de entitlement OCR.
3. El contrato TypeScript local `DynamsoftWebTwainObject` no expone metodos OCR tipados. Expone captura, URL de imagen, dimensiones, crop, deskew, rotacion, borrado y conversion a PDF.
4. No existen dependencias OCR en `package.json` ni assets de idioma OCR en el repositorio.
5. La seleccion de area existente vive en `DigitalizacionDocumentalWorkspace` como `cropSelection` y usa `PageCropSelection` con coordenadas reales de pagina.
6. La capa actual ya cumple la dependencia principal: no hace falta una seleccion nueva.

## Decisions

1. SCRUMCORE-260 queda como auditoria y diseno tecnico, no como implementacion funcional.
2. La futura accion `OCR Zona` debe depender de la seleccion existente (`cropSelection`) y del `pageId` activo.
3. El futuro OCR debe entrar por un contrato propio, por ejemplo `DigitalizacionOcrClient`, no ampliando la UI para tocar Dynamsoft directamente.
4. El adapter real debe tener una compuerta de capacidades antes de habilitar UI:
   - licencia con OCR disponible;
   - API OCR disponible en runtime;
   - idiomas instalados/configurados;
   - prueba de rendimiento sobre area recortada.
5. Si no hay capacidades OCR confirmadas, la UI no debe mostrar una accion funcional incompleta.
6. El resultado futuro debe mostrarse en drawer lateral no modal bloqueante, con acciones `Copiar`, `Insertar en metadato` y `Cerrar`.

## Proposed Future Architecture

```text
DigitalizacionDocumentalWorkspace
  -> seleccion existente: { pageId, selection }
  -> boton OCR Zona habilitado solo si capability.ocrZone === true
  -> useDigitalizacionOcrZone()
  -> DigitalizacionOcrClient.recognizeZone({ pageId, selection, imageUrl })
  -> adapter OCR validado
  -> drawer Resultado OCR
```

Contrato sugerido:

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

## UX Future Flow

1. Usuario activa `Seleccionar area`.
2. Usuario dibuja una region sobre la pagina activa.
3. Si `ocrZone` esta confirmado, el toolbar habilita `OCR Zona`.
4. Al ejecutar, el cliente OCR procesa solo la zona seleccionada.
5. El resultado aparece en drawer lateral con texto extraido.
6. Acciones:
   - `Copiar`: envia texto al portapapeles.
   - `Insertar en metadato`: delega en callback de integracion del modulo consumidor.
   - `Cerrar`: descarta el drawer sin modificar paginas.

## Risks / Trade-offs

- La licencia actual de Dynamic Web TWAIN 19.3.2 no demuestra por codigo que incluya OCR.
- OCR puede requerir modulos o recursos adicionales no cargados actualmente.
- Ejecutar OCR en frontend puede competir con `AcquireImage()` y procesamiento de imagenes si se habilita sin cola o cancelacion.
- Idiomas, DPI, contraste y calidad del area seleccionada afectan precision.
- `Insertar en metadato` necesita contrato con los modulos consumidores antes de implementarse.
- Si el runtime OCR modifica el buffer DWT, debe evitarse alterar `scanner.pages` y el PDF pendiente.

## Migration Plan

1. Cerrar SCRUMCORE-260 con auditoria y documento tecnico.
2. Validar licencia actual y runtime OCR en ambiente con scanner real.
3. Si OCR esta disponible, abrir un ticket de implementacion para:
   - crear `DigitalizacionOcrClient`;
   - agregar discovery de capacidades;
   - habilitar boton `OCR Zona`;
   - renderizar drawer de resultado;
   - cubrir pruebas unitarias e integracion.

## Open Questions

- La licencia instalada incluye OCR u OCR Pro?
- Que API exacta expone el runtime desplegado para OCR en DWT 19.3.2?
- Que idiomas deben soportarse inicialmente?
- `Insertar en metadato` inserta en un campo seleccionado, en un formulario externo o en un callback del host?
- Debe persistirse auditoria del texto extraido?
