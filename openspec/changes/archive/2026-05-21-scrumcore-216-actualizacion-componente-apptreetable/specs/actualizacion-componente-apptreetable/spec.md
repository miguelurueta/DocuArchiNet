# Capability: actualizacion-componente-apptreetable

## Overview
Refactoriza `AppTreeTable` para que sea un wrapper/adaptador reusable sobre `AppTable`, preservando compatibilidad con consumidores actuales y preparando la infraestructura para evolucion futura (Dynamic UI) sin introducir cambios rompientes.

## Requirements

### R1. Wrapper sobre AppTable
- `AppTreeTable` MUST renderizar usando `AppTable` internamente como engine base.
- `AppTreeTable` MUST NOT reimplementar un motor tabular alterno.

### R2. Compatibilidad API publica
- `AppTreeTable` MUST mantener el contrato publico actual sin requerir cambios en consumidores existentes.
- Cualquier prop existente MUST seguir funcionando con el mismo significado (o equivalente observable).

### R3. Expansion local y estable
- El estado `expanded` MUST ser controlado localmente por `AppTreeTable`.
- Expand/collapse MUST ser estable entre renders (sin perder estado al re-render normal).

### R4. Adaptacion Tree -> Table explicita
- La solucion MUST incluir adaptadores/helpers explicitos para:
  - flattening
  - calculo de filas visibles
  - indentacion
  - mapeo a filas consumibles por `AppTable`

### R5. Estados legacy
- `loading`, `empty`, `error`, `retry` MUST seguir presentes y mostrar mensajes en espanol como hoy.

### R6. Calidad y pruebas
- Se MUST agregar/ajustar pruebas unitarias para adapters/hooks (flattening, visibles, indent).
- Se MUST mantener pruebas de integracion para `AppTreeTable` y un consumer representativo (`DocumentosWorkbench`) sin regresiones.

### R7. No breaking changes
- La solucion MUST mantener compatibilidad binaria/semantica con consumidores actuales (sin cambios requeridos en llamadas existentes).
- Cualquier cambio interno MUST estar cubierto por pruebas de regresion relevantes.

## Non-Goals
- Implementar nuevos contratos backend-driven o lazy loading backend-driven (fuera de alcance).
- Reemplazar `AppTable`.

## Notes
- Este ticket NO introduce nuevos endpoints/contratos backend; solo refactoriza `AppTreeTable` como wrapper sobre `AppTable`.

