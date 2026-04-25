# Prompt: 27-FE AppEditor Fase 2A reflow incremental de parrafos

Actua como arquitecto y desarrollador senior especializado en TipTap, ProseMirror y motores de reflow multipagina.

Necesito implementar `AppEditor Fase 2A - reflow incremental de parrafos` sobre la base de paginas reales ya creada en Fase 1.

## Objetivo
- recalcular distribucion desde el bloque afectado hacia adelante
- partir parrafos por posicion real de texto
- empujar contenido posterior
- traer contenido hacia arriba al borrar

## Restricciones
- no recomputar todo el documento en cada tecla
- no usar hacks visuales
- no perder integridad del parrafo
- no dejar paginas vacias intermedias

## Casos obligatorios
- escribir al final de pagina
- editar un parrafo ya partido
- crecer ese parrafo y empujar el resto
- borrar texto y subir contenido desde la pagina siguiente

## Entregables
- implementacion real del motor incremental
- normalizacion del documento
- pruebas de reflow de parrafos

## Criterios de aceptacion
- no hay perdida ni duplicacion de texto
- el flujo multipagina de parrafos queda estable
- el rendimiento de escritura normal se mantiene razonable

