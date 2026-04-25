## 1. Base de paginas reales

- [x] 1.1 Diseñar e introducir el nodo `page` en el schema de `AppEditor`
- [x] 1.2 Adaptar la configuracion de TipTap para que el documento paginado use paginas reales como estructura base
- [ ] 1.3 Implementar el render de hojas reales con area util y margenes reales en `paginationMode="visual"`
- [x] 1.4 Mantener `paginationMode="none"` funcionando como flujo continuo sin imponer la estructura paginada

## 2. Migracion y compatibilidad del contenido

- [x] 2.1 Definir la migracion desde el contenido actual basado en flujo continuo al modelo `doc -> page -> blocks`
- [x] 2.2 Ajustar parseo y serializacion para evitar dependencia de `data-page-break-spacer` y `spacerHeight`
- [ ] 2.3 Garantizar compatibilidad con toolbar, links, imagenes locales, zoom y modo controlado/no controlado

## 3. Reflow incremental

- [ ] 3.1 Implementar reflow incremental desde el bloque afectado hacia adelante
- [ ] 3.2 Implementar split de parrafos por posicion real de texto
- [ ] 3.3 Implementar merge inverso para traer contenido desde la pagina siguiente al borrar
- [ ] 3.4 Eliminar paginas vacias intermedias y mantener el documento normalizado despues del reflow

## 4. Seleccion, cursor y marks inline

- [ ] 4.1 Preservar cursor y seleccion por posicion logica del documento despues del reflow
- [ ] 4.2 Mantener links, bold, italic y underline al partir o recomponer un parrafo
- [ ] 4.3 Validar estabilidad basica de undo/redo en escenarios multipagina cubiertos

## 5. Bloques indivisibles, paste y listas basicas

- [ ] 5.1 Implementar regla para mover imagenes y bloques indivisibles completos a la siguiente hoja
- [ ] 5.2 Soportar paste largo con redistribucion multipagina sin overflow visible
- [ ] 5.3 Asegurar continuidad razonable de listas basicas entre paginas reales

## 6. Retiro de arquitectura vieja y hardening

- [ ] 6.1 Retirar `autoPagination.ts`, `autoPageBreak.ts` y la logica vieja del camino principal del modo paginado si quedan obsoletos
- [ ] 6.2 Evitar convivencia de dos motores paginados sobre el mismo flujo
- [ ] 6.3 Optimizar el reflow para evitar recomputacion completa del documento en cada tecla

## 7. Pruebas y validacion

- [ ] 7.1 Cubrir escritura al final de pagina y continuidad a la siguiente hoja
- [ ] 7.2 Cubrir edicion de un parrafo ya partido entre paginas
- [ ] 7.3 Cubrir borrado con reflow inverso hacia arriba
- [ ] 7.4 Cubrir paste largo con formato inline y links
- [ ] 7.5 Cubrir imagenes que no caben y deben pasar completas a la siguiente hoja
- [ ] 7.6 Cubrir listas basicas y documentos multipagina reales
