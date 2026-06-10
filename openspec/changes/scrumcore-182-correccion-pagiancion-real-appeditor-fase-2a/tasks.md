# Tasks: SCRUMCORE-182

- [ ] Revisar `application/autoPagination.ts` y confirmar splits por `coordsAtPos` + preferencia por whitespace + fallback por caracter.
- [ ] Asegurar invalidacion incremental en `application/useAppEditor.ts`:
  - [ ] incluir bloque anterior al afectado
  - [ ] incluir `pageBreak` auto previo cuando aplique
  - [ ] reaccionar a cambios de altura reales (imagenes / reflow CSS)
- [ ] Asegurar normalizacion post-reflow:
  - [ ] sin breaks auto redundantes
  - [ ] sin paginas vacias intermedias
- [ ] Agregar/ajustar pruebas:
  - [ ] escribir al final de pagina
  - [ ] editar parrafo partido
  - [ ] crecer parrafo y empujar
  - [ ] borrar y subir contenido

