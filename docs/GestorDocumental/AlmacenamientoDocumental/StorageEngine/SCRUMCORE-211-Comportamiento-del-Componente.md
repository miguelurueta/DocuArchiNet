# SCRUMCORE-211 — Comportamiento del Componente

## Modal de firmas (AppVisorEmbedPdf)

Se agrega una pestaña adicional en el modal de firmas:

- **Dibujar firma**: firma por canvas (plugin oficial).
- **Subir firma**: imagen (plugin oficial).
- **Firma personal**: consumo de API temporal (SCRUM-201) para descargar firma como imagen y usarla como stamp (plugin oficial).

## Firma personal (estados)

La pestaña “Firma personal” expone estados enterprise:

- `loading`: mientras solicita metadata y descarga el blob.
- `empty`: cuando `success=true` y `data=null` (o caso equivalente de “sin firma”).
- `error`: cuando `success=false` o ocurre un error controlado.
- `ready`: cuando existe firma descargada (ObjectURL + ArrayBuffer) lista para “Usar firma personal”.

## Acciones

- “Usar firma personal”: prepara una definición `SignatureStampFieldDefinition` (creationType upload) y permite al usuario continuar con el flujo estándar de placement (EmbedPDF).
- “Reintentar”: limpia estado previo y vuelve a ejecutar carga.

