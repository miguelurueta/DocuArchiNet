# Spec: SCRUMCORE-182 AppEditor Fase 2A - reflow incremental de parrafos

## Objetivo

Implementar un motor incremental de reflow multipagina que:
- recalcula desde el bloque afectado hacia adelante
- parte parrafos por posicion real de texto
- empuja contenido posterior y trae contenido hacia arriba al borrar
- no deja paginas vacias intermedias
- no pierde ni duplica texto

## Reglas

1. Invalidez incremental:
   - En cambios de doc: determinar `dirtyStartChildIndex` a partir de la posicion afectada.
   - El reflow debe incluir, como minimo, el bloque anterior al afectado.
   - Si existe `pageBreak` auto previo inmediato, incluirlo en la limpieza/reflow.
2. Split de textblocks:
   - Si un bloque textblock desborda el boundary de pagina, se inserta `pageBreak` auto:
     - preferir split en whitespace cercano hacia atras.
     - si no existe corte limpio, permitir split por caracter.
3. Cascada:
   - Cada accion (split/before/list-item) puede afectar bloques posteriores; el motor itera hasta estabilidad o limite seguro.
4. Normalizacion:
   - No deben quedar `pageBreak` auto redundantes ni paginas vacias intermedias.
   - El documento final debe ser serializable/rehidratable sin cambios espurios.

## Casos obligatorios

- typing al final de pagina: el texto continua en la siguiente hoja.
- editar un parrafo ya partido: recompone bien el flujo.
- crecer el parrafo: empuja el resto sin corrupcion.
- borrar texto: sube contenido desde la pagina siguiente, sin paginas vacias.

## Criterios de aceptacion

- 0 perdida / 0 duplicacion de texto.
- flujo estable (misma entrada => mismo resultado).
- rendimiento razonable: no recalcular todo el documento por tecla.

