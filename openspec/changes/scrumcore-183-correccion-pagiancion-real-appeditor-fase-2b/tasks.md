# Tasks: SCRUMCORE-183 (Fase 2B)

- [ ] Identificar los puntos exactos donde split/merge puede perder marks inline (links/bold/italic/underline).
- [ ] Endurecer preservacion de seleccion en `useAppEditor` durante:
  - [ ] removeAutoPageBreaks (cleanup)
  - [ ] splitTextBlockAtPositionAndInsertPageBreak
  - [ ] merges al remover breaks (`mergeOnRemove`)
- [ ] Agregar pruebas:
  - [ ] link en parrafo partido (edicion y reflow)
  - [ ] marks inline en punto de corte (bold/italic/underline)
  - [ ] seleccion cruzando pagina
  - [ ] undo/redo post-reflow sin corrupcion

