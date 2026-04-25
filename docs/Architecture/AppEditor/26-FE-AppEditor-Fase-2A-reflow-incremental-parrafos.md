# Ticket: 26-FE AppEditor Fase 2A reflow incremental de parrafos

## Identificacion
- Cambio: `26-FE`
- Nombre: `AppEditor Fase 2A - reflow incremental de parrafos`
- Ticket propuesto: `SCRUMCORE-AE-F2A-01`
- Modulo: `src/app/Components/UI/AppEditor/`
- Dependencia previa: `22-FE AppEditor Fase 1 - paginas reales base`

## Objetivo
Implementar el motor incremental de reflow para texto y parrafos entre paginas reales.

## Alcance
- detectar bloque afectado por edicion
- recalcular distribucion desde el bloque afectado hacia adelante
- partir parrafos por posicion de texto real
- mantener continuidad del mismo parrafo entre paginas
- empujar contenido posterior en cascada
- traer contenido hacia arriba cuando se borra
- eliminar paginas vacias intermedias

## No alcance
- seleccion/cursor avanzada
- paste masivo hardening
- listas complejas
- imagenes complejas

## Criterios de aceptacion
- escribir al final de una pagina continua en la siguiente
- editar un parrafo ya partido recompone bien el flujo
- borrar texto trae contenido de la pagina siguiente hacia arriba
- no se duplican ni se pierden caracteres
- el documento queda normalizado tras el reflow

## Validacion minima
- typing al final de hoja
- editar en medio de un parrafo partido
- crecer y reducir un parrafo multipagina
- documento con tres o mas paginas

