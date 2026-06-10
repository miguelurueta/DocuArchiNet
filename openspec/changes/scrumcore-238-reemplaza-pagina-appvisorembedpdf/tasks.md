## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira + prompt SCRUM-249 refinado.
- [x] 1.2 Reemplazar spec generico por requirements verificables.
- [x] 1.3 Reemplazar design generico por decisiones, riesgos y plan tecnico.
- [x] 1.4 Reemplazar tasks generico por plan implementable por fases.

## 2. Servicio HTTP y contratos

- [ ] 2.1 Crear tipos del modulo de reemplazo de paginas PDF anotadas.
- [ ] 2.2 Crear service dedicado para `init`, `chunk`, `status`, `complete`, `cancel` y `paginas-anotadas`.
- [ ] 2.3 Usar `clienteApi`, `AbortSignal` y desempaquetado de `AppResponses<T>`.
- [ ] 2.4 Normalizar errores de dominio preservando `Field`, `Message` y `RequestId` cuando existan.
- [ ] 2.5 Implementar upload de chunks con body binario puro, `Content-Type: application/octet-stream` y `X-Total-Chunks`.
- [ ] 2.6 No setear manualmente `Content-Length` desde browser.
- [ ] 2.7 Implementar `statusUploadTemporal` para confirmar `COMPLETED` cuando `complete` no entregue evidencia suficiente o el flujo lo requiera.
- [ ] 2.8 Implementar cancelacion best-effort de temporales creados.
- [ ] 2.9 Agregar tests del service para endpoints, envelope, errores, chunks, status, complete, cancel y reemplazo multipagina.

## 3. Utilidades PDF

- [ ] 3.1 Crear utilidad para detectar paginas anotadas desde `annotation.state.pages`.
- [ ] 3.2 Garantizar conversion de page index base 0 a `PageNumber` base 1, dedupe y orden.
- [ ] 3.3 Crear utilidad SHA-256 para `Blob` usando `crypto.subtle` con fallback documentado.
- [ ] 3.4 Validar si se aprueba agregar `pdf-lib` u otra alternativa real para extraer PDFs de una pagina sin rasterizacion.
- [ ] 3.5 Si se aprueba dependencia, instalar/agregar la alternativa aprobada y documentar licencia, peso, impacto bundle y estrategia de import dinamico.
- [ ] 3.6 Si no se aprueba dependencia o no hay mecanismo real, marcar bloqueo explicito, documentar la razon y no continuar con la implementacion de exportacion de paginas.
- [ ] 3.7 Agregar tests de paginas anotadas, SHA-256 y extraccion single-page PDF cuando exista mecanismo aprobado.

## 4. AppVisorEmbedPdf

- [ ] 4.1 Extender tipos del ref del visor con metodo para exportar paginas PDF anotadas.
- [ ] 4.2 Implementar exportacion: detectar paginas, ejecutar `commit()`, ejecutar `saveAsCopy()` y extraer blobs PDF de una pagina.
- [ ] 4.3 Retornar resultado sin exponer `@embedpdf/*` ni detalles internos del engine.
- [ ] 4.4 Mantener `load`, `reset` y `cancelCurrentLoad` compatibles.
- [ ] 4.5 Limpiar password en memoria volatil cuando aplique: reset, cambio de documento, cancelacion, cierre o desmontaje.
- [ ] 4.6 Agregar tests del visor para sin anotaciones, paginas base 1, commit antes de export, blobs PDF y errores controlados.

## 5. AppPdfToolbar

- [ ] 5.1 Agregar accion presentacional para guardar paginas anotadas si el diseno actual lo permite.
- [ ] 5.2 Recibir flags de habilitado/deshabilitado, loading/progreso y callback desde consumidor.
- [ ] 5.3 No importar `clienteApi`, endpoints, workflow ni tipos EmbedPDF.
- [ ] 5.4 Agregar o ajustar tests presentacionales.

## 6. DocumentosWorkbench

- [ ] 6.1 Validar documento activo, `viewerKind = pdf`, permisos, visor listo y documento no firmado.
- [ ] 6.2 Obtener contexto real: `NombreGabinete`, `IdDocumento`, radicado, workflow, modulo y tipologia cuando existan.
- [ ] 6.3 Orquestar exportacion del visor, upload temporal por pagina, complete y reemplazo final.
- [ ] 6.4 Enviar `RutaTemporalId` por cada item de `Paginas`; usar raiz solo como fallback compatible.
- [ ] 6.5 Implementar `AbortController` y latest-wins para cambios de documento o nueva operacion.
- [ ] 6.6 Limpiar temporales best-effort si la operacion falla o se cancela antes del reemplazo final.
- [ ] 6.7 No llamar `DELETE` despues de success de `paginas-anotadas`.
- [ ] 6.8 Refrescar documento visible tras success sin romper seleccion ni carga gestionada.
- [ ] 6.9 Manejar errores funcionales: password, anti-desfase, validacion, upload y reemplazo.
- [ ] 6.10 Agregar tests de Workbench para validaciones, success, error, cancelacion, latest-wins, documento firmado y no-regresion de imagenes.

## 7. Seguridad, observabilidad y documentacion

- [ ] 7.1 Verificar que `OriginalPdfPassword` solo vive en memoria y solo viaja en request final cuando aplica.
- [ ] 7.2 Evitar logs de password, JWT, blobs completos y rutas fisicas sensibles.
- [ ] 7.3 Usar `window.__DV_DEBUG__` solo para logs no sensibles.
- [ ] 7.4 Documentar contrato de APIs, `Content-Length` restringido en browser, limpieza de temporales y decision PDF.
- [ ] 7.5 Documentar anti-desfase: campos enviados solo si existe fuente real; pendiente si no existe.

## 8. Validacion

- [ ] 8.1 Ejecutar `npm.cmd run build`.
- [ ] 8.2 Ejecutar tests del service de reemplazo.
- [ ] 8.3 Ejecutar tests de utilidades PDF.
- [ ] 8.4 Ejecutar tests de `AppVisorEmbedPdf`.
- [ ] 8.5 Ejecutar tests de `AppPdfToolbar`.
- [ ] 8.6 Ejecutar tests de `DocumentosWorkbench`.
- [ ] 8.7 Validar OpenSpec con `npx.cmd openspec validate scrumcore-238-reemplaza-pagina-appvisorembedpdf --strict`.

## 9. QA manual

- [ ] 9.1 Abrir PDF normal, anotar una pagina, guardar y verificar reemplazo.
- [ ] 9.2 Abrir PDF normal, anotar varias paginas y verificar reemplazo en una sola operacion logica.
- [ ] 9.3 Cancelar durante upload y verificar limpieza best-effort.
- [ ] 9.4 Cambiar de documento durante upload y verificar latest-wins.
- [ ] 9.5 Intentar guardar documento firmado electronicamente y verificar bloqueo.
- [ ] 9.6 Probar PDF protegido con password valida si el flujo existe.
- [ ] 9.7 Probar password invalida y error `originalPdfPassword` si el flujo existe.
- [ ] 9.8 Simular rechazo anti-desfase si hay metadata real disponible.
- [ ] 9.9 Confirmar que no se muestran rutas fisicas a usuarios finales cuando aplique.
- [ ] 9.10 Confirmar no-regresion: exportacion, impresion, firma, imagenes y carga gestionada.

## 10. Publish / cierre

- [ ] 10.1 Revisar diff final de design/spec/tasks e implementacion.
- [ ] 10.2 Crear commit y push de artefactos refinados o implementacion segun el estado del flujo.
- [ ] 10.3 Abrir/actualizar PR mediante el flujo `opsxj:archive` cuando corresponda.
- [ ] 10.4 Actualizar Jira con enlace/evidencia cuando corresponda.
