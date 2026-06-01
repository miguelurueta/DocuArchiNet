# SCRUMCORE-234 - Pruebas

## Estrategia

La cobertura se separa en tres niveles:

- Unit: calculo deterministico de escala y guards basicos.
- Integracion React: ciclo load -> ready -> auto-fit apply once -> no re-fit agresivo.
- Manual QA: comportamiento real con PDFs portrait, landscape, metadata rotation, rotate manual, thumbnails y zoom.

## Unit Tests

Archivo:

- `src/app/Components/UI/AppVisorEmbedPdf/autoFit/autoFit.math.test.ts`

Casos cubiertos:

- `computeFitScale` con `fitMode=width`.
- `computeFitScale` con `fitMode=page`.
- Fallback a `1` cuando hay tamanios invalidos.
- Contenido rotado simulado mediante swap de dimensiones.

Comando recomendado:

```powershell
npm run test -- src/app/Components/UI/AppVisorEmbedPdf/autoFit/autoFit.math.test.ts
```

## Integracion React

Casos esperados:

- Al cargar documento y recibir ready, se aplica auto-fit una vez.
- Si cambia `documentId` o `seq`, la aplicacion stale no hace commit.
- Si el usuario hace zoom manual, el sistema no debe reimponer fit en resize.
- Si producto define reactivacion de Smart Fit, debe volver a aplicar fit explicitamente.

Estado:

- Pendiente de completar segun `openspec/changes/scrumcore-234-actualizacion-componente-appvisorembedpdf/tasks.md`.

## Manual QA Checklist

- [ ] PDF portrait: abre ajustado al ancho disponible.
- [ ] PDF landscape: abre sin recorte inesperado.
- [ ] PDF con rotacion metadata 90/270: calcula fit usando dimensiones efectivas.
- [ ] Rotate manual: no genera loops ni saltos repetidos.
- [ ] Zoom manual: no se resetea inesperadamente.
- [ ] Scroll: mantiene navegacion estable.
- [ ] Thumbnails: no pierden sincronizacion visible.
- [ ] Firma/anotaciones: siguen operando despues del fit.
- [ ] Seleccion de texto: la capa `SelectionLayer` sigue dentro de `PagePointerProvider`.
- [ ] Seleccion de texto: arrastrar sobre texto muestra highlight.
- [ ] Seleccion de texto: boton `Copy` copia texto al portapapeles.
- [ ] Seleccion de texto: `Ctrl+C` / `Cmd+C` copia texto cuando hay seleccion.
- [ ] Seleccion de texto: click/drag no dispara drag del bitmap renderizado.
- [ ] PDF rotado por metadata: `SelectionLayer` y `AnnotationLayer` siguen alineados con `RenderLayer`.
- [ ] Rotacion manual 90/270: no hay clipping visible en el borde inferior/lateral.

## Evidencia de Comandos Ejecutados

### Test unitario Auto-Fit

Comando ejecutado:

```powershell
npm run test -- src/app/Components/UI/AppVisorEmbedPdf/autoFit/autoFit.math.test.ts
```

Resultado:

- PASS.
- `1` test file passed.
- `4` tests passed.
- Ultima duracion reportada por Vitest: `1.81s`.

### Build general

Comando ejecutado:

```powershell
npm run build -- --mode development
```

Resultado:

- Falla.
- La falla ya no incluye el error anterior de `scope.getSelection` en el flujo de seleccion/copiado.
- Persisten errores TypeScript no resueltos y algunos del visor no relacionados directamente con la documentacion Auto-Fit.

Errores relevantes observados:

- `AppEditor`: errores de tipos/variables no usadas.
- `AppVisorEmbedPdf`: errores previos en BlobPart, firmas de tareas EmbedPDF, variable `onResetRotation` no usada y contrato `exclusive` en annotation tools.
- `gestionCorrespondencia`: errores de tipos y variables no usadas.

Interpretacion:

- El build general no es evidencia de cierre completo.
- Para SCRUMCORE-234, la evidencia automatizada disponible y acotada es el test unitario de math; debe ejecutarse antes de archive.

## Comandos Recomendados Antes de Cierre

```powershell
npm run test -- src/app/Components/UI/AppVisorEmbedPdf/autoFit/autoFit.math.test.ts
npm run test -- src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx
npm run build -- --mode development
```

Si el build general sigue fallando por deuda no relacionada, registrar el listado de errores y adjuntar decision de alcance antes de cerrar Jira.
