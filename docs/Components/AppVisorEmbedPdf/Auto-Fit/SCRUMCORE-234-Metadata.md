# SCRUMCORE-234 - Metadata

- **Ticket**: SCRUMCORE-234
- **Nombre**: Actualizacion componente AppVisorEmbedPdf - Auto-Fit deterministico, zoom/rotacion y seleccion de texto
- **Fecha**: 2026-06-01 (America/Bogota)
- **Componente principal**: `AppVisorEmbedPdf`
- **Ruta documental**: `docs/Components/AppVisorEmbedPdf/Auto-Fit/`
- **Ruta solicitada originalmente**: `docs/Components/AppTable/Auto-Fit/`
- **Decision de ruta**: se documenta en `AppVisorEmbedPdf` porque el cambio pertenece al visor PDF y ya existe una estructura documental del componente.
- **Tipo**: mejora UX / hardening frontend
- **Commit de cierre documental**: `SCRUMCORE-234: document zoom and selection updates`
- **Backend**: NO modificado
- **Endpoints**: NO modificados
- **OCR / imagen / ML**: NO introducido
- **Persistencia de URLs temporales/tokens**: NO introducida

## Resumen Ejecutivo

SCRUMCORE-234 introduce Auto-Fit deterministico en `AppVisorEmbedPdf` para ajustar el PDF al viewport usando solo metricas disponibles del visor, el engine EmbedPDF y la rotacion metadata reportada. El objetivo es mejorar la lectura inicial del documento sin aplicar heuristicas de contenido y sin pelear con el zoom, scroll, thumbnails, rotacion manual, firma o anotaciones.

La politica implementada hasta este punto aplica el ajuste una vez despues del handshake `ready` del documento. El modo por defecto es `width`, con soporte matematico para `page`.

Adicionalmente, durante el cierre del ticket se documento y ajusto el hardening del visor en dos frentes relacionados:

- Zoom/rotacion/render por pagina: el slot de cada pagina usa `rotatedWidth/rotatedHeight`, se evita clipping con `overflow: visible`, y las ramas de render consideran rotacion metadata y rotacion manual para conservar escala, punteros y anotaciones.
- Seleccion de texto/copiado: se habilito `SelectionLayer`, se registro `SelectionPluginPackage` desde `@embedpdf/plugin-selection/react` para montar `CopyToClipboard`, y se agrego menu contextual `Copy` mas soporte `Ctrl/Cmd+C` usando la API `forDocument(documentId)`.

## Problema y Evidencia

Problema observado:

- Algunos PDFs llegan con orientacion o dimensiones incomodas para lectura inicial.
- El visor no siempre queda ajustado de forma consistente al contenedor disponible.
- Un ajuste continuo podria romper la intencion del usuario si ya hizo zoom o rotacion manual.

Evidencia tecnica del cambio:

- Existe modulo dedicado en `src/app/Components/UI/AppVisorEmbedPdf/autoFit/`.
- `computeFitScale()` calcula escala de forma deterministica para `width` y `page`.
- `applyAutoFitOnce()` aplica zoom con centro de viewport y guards de metricas invalidas.
- `AppVisorEmbedPdf` guarda la intencion de auto-fit por `documentId`/`seq` y evita commits stale.
- El plugin selection se registra desde la entrada React oficial: `@embedpdf/plugin-selection/react`.
- `SelectionLayer` se renderiza dentro de `PagePointerProvider`, junto a `RenderLayer` y `AnnotationLayer`, para que la capa reciba coordenadas, escala y rotacion correctas.
- El copiado ya no depende del native selection del DOM; delega en `scope.copyToClipboard()`, que emite el evento interno que `CopyToClipboard` escribe al portapapeles con `navigator.clipboard.writeText(text)`.
- El bitmap renderizado del PDF se deja sin drag/pointer events para que la interaccion vaya a `SelectionLayer`/`InteractionManager`.

## Alcance

- Auto-fit post-ready por documento.
- Calculo de escala deterministico usando viewport y tamanio de contenido.
- Modo default `width`; soporte de calculo para `page`.
- Compatibilidad con rotacion metadata mediante swap de dimensiones en pasos 90/270.
- Correccion de clipping visual en paginas rotadas mediante slot rotado y contenedor base.
- Seleccion de texto visible y copiables usando plugin oficial de EmbedPDF.
- Stale-ignore por identidad de documento y secuencia de carga.
- Documentacion enterprise del feature.

## Fuera de Alcance

- Auto-rotate por contenido.
- OCR, deteccion por imagen o ML.
- Cambios de backend o endpoints.
- Persistencia de URLs temporales, tokens o blobs.
- Reaplicar auto-fit agresivamente en cada resize.
- Redisenar el toolbar o agregar controles visibles nuevos si producto no lo confirma.

## Restricciones

- No romper `latest-wins` ni cancelacion de carga.
- No reabrir documentos ni alterar `maxDocuments`.
- No cambiar contratos publicos del visor salvo necesidad explicita.
- No registrar datos sensibles en logs.
- Los logs de diagnostico deben quedar detras de `window.__DV_DEBUG__`.

## Checklist de Cumplimiento

- [x] Backend no modificado.
- [x] Endpoints no modificados.
- [x] Sin OCR/imagen/ML.
- [x] Sin persistencia de URLs temporales/tokens.
- [x] Modulo aislado `autoFit/`.
- [x] Calculo deterministico `computeFitScale()`.
- [x] Auto-fit post-ready apply-once.
- [x] Guard stale por `documentId`/`seq`.
- [x] Tests unitarios de matematica de escala.
- [x] `SelectionPluginPackage` registrado desde `@embedpdf/plugin-selection/react`.
- [x] `InteractionManagerPluginPackage` registrado desde `@embedpdf/plugin-interaction-manager/react`.
- [x] `SelectionLayer` agregado en todas las ramas de render de pagina.
- [x] Menu contextual `Copy` conectado a `scope.copyToClipboard()` y `scope.clear()`.
- [x] `Ctrl/Cmd+C` conectado a `scope.copyToClipboard()` con guard `scope.getState().selection`.
- [x] CSS evita drag del bitmap renderizado para priorizar la capa de seleccion.
- [ ] Integracion React completa de resize/zoom manual.
- [ ] QA manual completo en PDFs portrait/landscape/metadata rotation.
- [ ] Archive OpenSpec y cierre Jira despues de merge.
