# Ticket: 22-FE AppEditor Fase 1 paginas reales base

## Identificacion
- Cambio: `22-FE`
- Nombre: `AppEditor Fase 1 - paginas reales base`
- Ticket propuesto: `SCRUMCORE-AE-F1-01`
- Modulo: `src/app/Components/UI/AppEditor/`
- Dependencia previa: reemplaza la base de paginacion visual actual

## Problema
El editor actual representa hojas de forma visual, pero el contenido sigue viviendo en un flujo continuo. Eso permite:

- desbordes antes de corregir
- cortes inconsistentes
- `pageBreak` automaticos como parche estructural
- margen util no garantizada
- comportamiento fragil al editar contenido ya repartido

## Objetivo
Crear la base arquitectonica de hojas reales para `AppEditor`, de modo que el documento deje de depender de separadores visuales y pase a operar sobre paginas reales dentro del modelo del editor.

## Resultado esperado
- el documento se organiza en paginas reales
- cada pagina tiene alto util real y limites estructurales reales
- desaparece la dependencia del modelo actual basado en `pageBreak + spacer`
- se mantiene compatibilidad con TipTap/ProseMirror
- queda lista la base para reflow real en la Fase 2

## Alcance
- definir nuevo schema con `page` como nodo real
- adaptar `doc` para contener paginas
- definir reglas de contenido permitido dentro de `page`
- crear normalizador inicial de documento a paginas
- crear representacion real de hoja A4 con margenes reales
- mantener toolbar, zoom, modo controlado y serializacion en estado coherente
- dejar apagado o removido el motor viejo cuando el nuevo este activo

## No alcance
- reflow fino tipo Word para todos los casos
- soporte completo de paste masivo con redistribucion compleja
- manejo completo de listas partidas
- merge hacia arriba al borrar
- optimizacion final de rendimiento para documentos enormes

## Entregables tecnicos
1. Nuevo nodo `page` en el schema.
2. Adaptacion de render y parsing para paginas reales.
3. Estrategia de migracion desde el HTML actual al modelo paginado.
4. Base de layout por hoja real con area util y margenes reales.
5. Mecanismo inicial para crear pagina siguiente cuando ya no cabe mas contenido al final.
6. Eliminacion o aislamiento del sistema actual de `autoPageBreak` como base primaria.

## Archivos candidatos
- `src/app/Components/UI/AppEditor/infrastructure/tiptap.extensions.ts`
- `src/app/Components/UI/AppEditor/infrastructure/tiptap.config.ts`
- `src/app/Components/UI/AppEditor/presentation/AppEditor.tsx`
- `src/app/Components/UI/AppEditor/application/useAppEditor.ts`
- `src/app/Components/UI/AppEditor/application/normalizeEditorHtml.ts`
- nuevos archivos de nodo/extensiones de pagina si aplica

## Criterios de aceptacion
- el editor renderiza paginas reales, no simuladas por separadores visuales
- escribir al final de una pagina crea continuidad en la siguiente sin montar contenido
- el area util de la hoja se respeta estructuralmente
- el sistema ya no depende de `data-page-break-spacer`
- el documento sigue siendo editable y serializable
- no se rompe toolbar, links, imagenes locales ni modo controlado

## Riesgos
- migracion de contenido existente
- compatibilidad con HTML persistido
- impacto en seleccion y cursor
- deuda si conviven dos motores de paginacion

## Validacion minima
- abrir contenido existente y migrarlo
- escribir hasta el final de la primera pagina
- crear segunda pagina real
- confirmar que no hay contenido visible entre hojas
- confirmar que el HTML resultante no depende del spacer viejo

## Definicion de terminado
La Fase 1 termina cuando `AppEditor` ya trabaja con paginas reales y la continuidad basica entre hojas existe, aunque el reflow avanzado todavia quede para Fase 2.
