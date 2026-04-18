## 1. Cursor y seleccion en modo multi-hoja

- [ ] 1.1 Revisar el comportamiento de cursor tras recalculos del layout paginado
- [ ] 1.2 Validar que la seleccion no se rompa al cruzar limites visuales de pagina
- [ ] 1.3 Confirmar que la capa visual no interfiera con foco ni mapping de posiciones
- [ ] 1.4 Registrar cualquier ajuste necesario para preservar undo/redo y seleccion estable

## 2. Scroll, navegacion y page context

- [ ] 2.1 Revisar `usePaginationMetrics.ts` para eliminar retroalimentaciones o jitter de scroll
- [ ] 2.2 Revisar `usePageContext.ts` para estabilizar el calculo de `Pagina X de Y`
- [ ] 2.3 Validar `scrollIntoView` dentro del `canvas` paginado
- [ ] 2.4 Confirmar prioridad coherente entre cursor y scroll reciente al resolver pagina actual

## 3. Compatibilidad con `PageBreak`, imagenes y zoom

- [ ] 3.1 Confirmar que `PageBreak` no rompe scroll, seleccion ni page context
- [ ] 3.2 Validar compatibilidad completa con imagenes locales/remotas, resize y `data-align`
- [ ] 3.3 Verificar que `zoomLevel` no desalinee paginas, contador ni seleccion
- [ ] 3.4 Confirmar que toolbar y modo controlled/uncontrolled permanecen sin regresion

## 4. Performance y reduccion de flicker

- [ ] 4.1 Identificar recalculos y reflows innecesarios en documentos multipagina
- [ ] 4.2 Ajustar sincronizacion visual para reducir flicker y reposicionamientos erraticos
- [ ] 4.3 Validar comportamiento aceptable en documentos largos
- [ ] 4.4 Mantener la arquitectura actual sin reintroducir wrappers o editores por pagina

## 5. Pruebas y evidencia

- [ ] 5.1 Agregar o ajustar pruebas de scroll y page context en modo multi-hoja
- [ ] 5.2 Agregar pruebas de seleccion/cursor cruzando limites de pagina
- [ ] 5.3 Agregar pruebas de compatibilidad con `PageBreak`, zoom e imagenes
- [ ] 5.4 Ejecutar pruebas focalizadas del editor y registrar resultados
- [ ] 5.5 Ejecutar validacion TypeScript o equivalente y registrar residuos ajenos si aparecen
- [ ] 5.6 Registrar evidencia final en este archivo

## Evidencia

- Pendiente de implementacion.
