## Context

`SCRUMCORE-51` ya definio los contratos base de `AppTableExport`, incluyendo modos, formatos, datasource y metadata institucional del reporte. Este cambio implementa la primera fase funcional sobre esa base: exportacion local de `currentPage` y `selectedRows`, sin backend y sin depender de un modulo concreto.

El objetivo de esta fase es validar la pieza reusable que consume `AppTableExportDataSource`, resuelve disponibilidad de modos segun capacidades reales del datasource y genera archivos locales a partir de filas y columnas ya disponibles en frontend. La integracion visual final mediante `AppDropdown` y la exportacion total server-side quedan para cambios posteriores.

Las restricciones relevantes son:

- no reintroducir contratos ya definidos en `SCRUMCORE-51`
- no acoplar la implementacion a `GestionCorrespondencia` ni a otro modulo
- no mezclar loading de exportacion con loading de datos de la tabla
- no exportar columnas de acciones puramente visuales como si fueran datos

## Goals / Non-Goals

**Goals:**

- Implementar `AppTableExport` como coordinador reusable de exportacion local.
- Soportar los modos `currentPage` y `selectedRows` usando solo el datasource ya tipado.
- Resolver la disponibilidad de acciones segun las capacidades reales del datasource.
- Mantener separadas la logica de resolucion de filas, la serializacion del archivo y el trigger visual.
- Reutilizar `AppTableExportReportMeta` y el asset corporativo ya estandarizado para formatos que lo soporten.

**Non-Goals:**

- No implementar `allLoaded` ni `allMatching` en esta fase.
- No conectar backend ni iterar paginas server-side.
- No cerrar aun la integracion visual final con `AppDropdown` dentro de `AppTableQueryWrapper`.
- No resolver un layout PDF complejo o una experiencia avanzada de exportacion asincrona larga.
- No migrar todavia modulos consumidores.

## Decisions

### 1. `AppTableExport` se implementa sobre el contrato de datasource ya existente

La implementacion debe consumir `AppTableExportDataSource<T>` y no leer estado interno de AG Grid ni de una pantalla concreta.

Razon:

- mantiene el componente reusable y desacoplado del renderer
- permite soportar `currentPage` y `selectedRows` con la misma API ya acordada en `SCRUMCORE-51`
- evita que la solucion quede atada a una sola implementacion de tabla

Alternativas consideradas:

- leer seleccion o filas visibles directamente desde internals de AG Grid
  - descartado por acoplamiento tecnico y baja portabilidad
- recibir filas y seleccion por props sueltas sin datasource
  - descartado porque rompe la semantica ya establecida para fases posteriores

### 2. La disponibilidad de modos se resuelve por capacidades, no por pantalla

`currentPage` y `selectedRows` solo deben aparecer cuando el datasource realmente los soporte.

Razon:

- evita logica condicional dispersa por modulo
- mantiene coherencia entre contratos y UI
- permite degradar limpiamente cuando una tabla no expone seleccion

Alternativas consideradas:

- mostrar siempre ambos modos y fallar al ejecutar
  - descartado por mala UX y errores evitables
- delegar esa decision a cada pantalla
  - descartado por duplicacion de reglas

### 3. La serializacion del archivo se separa del trigger visual

La implementacion debe distinguir:

- resolucion de filas a exportar
- filtrado/transformacion de columnas exportables
- serializacion por formato
- disparo de descarga

Razon:

- permite probar la logica sin depender del componente visual final
- deja el sistema preparado para integrar `AppDropdown` despues sin reescribir la logica
- facilita extender formatos o estrategias de exportacion

Alternativas consideradas:

- concentrar toda la logica dentro de un unico componente visual
  - descartado por baja testabilidad y alto acoplamiento

### 4. El loading de exportacion se mantiene separado del loading de la tabla

La exportacion debe manejar su propio estado, por ejemplo `exportLoading`, sin activar skeleton ni loading veil de `AppTable`.

Razon:

- la exportacion no significa recarga de datos
- evita señales visuales engañosas para el usuario
- respeta la semantica ya establecida en `SCRUMCORE-50`

Alternativas consideradas:

- reutilizar `loading` de tabla para exportacion
  - descartado porque activaria estados visuales incorrectos

### 5. Solo se exportan columnas con valor de dato

La implementacion debe excluir columnas de acciones o puramente visuales salvo que exista una forma explicita de resolver un valor exportable.

Razon:

- evita archivos contaminados con celdas sin semantica
- mantiene consistencia entre exportacion y contenido de negocio

Alternativas consideradas:

- exportar todas las columnas visibles sin filtrado
  - descartado porque incluiria acciones y columnas no representables

## Risks / Trade-offs

- [Riesgo] `selectedRows` puede no existir o no tener datos en algunas tablas
  → Mitigacion: resolver disponibilidad por capacidad del datasource y ocultar o deshabilitar la opcion con semantica clara.

- [Riesgo] Exportar columnas visuales como si fueran datos reales
  → Mitigacion: aplicar filtrado explicito de columnas exportables y no asumir que toda columna visible es exportable.

- [Riesgo] Mezclar exportacion con loading de tabla
  → Mitigacion: mantener `exportLoading` separado y no conectar exportacion con skeleton/veil de `AppTable`.

- [Riesgo] Diseñar una primera fase demasiado dependiente del formato final
  → Mitigacion: separar serializacion por formato y mantener esta fase enfocada en exportacion local basica.

- [Riesgo] Repetir responsabilidades de cambios futuros (`allLoaded`, `allMatching`, integracion visual completa)
  → Mitigacion: dejar estos modos y la integracion visual final fuera del alcance de este ticket.

## Migration Plan

1. Implementar `AppTableExport` sobre los contratos existentes de `SCRUMCORE-51`.
2. Resolver utilidades de seleccion de filas y columnas exportables.
3. Implementar serializacion local para los formatos habilitados en esta fase.
4. Agregar pruebas para `currentPage`, `selectedRows`, ausencia de seleccion y disponibilidad por capacidades.
5. Mantener el componente listo para integracion posterior con `AppDropdown` y `AppTableQueryWrapper`.

Rollback:

- revertir `AppTableExport` y sus helpers sin tocar los contratos base definidos en `SCRUMCORE-51`
- mantener intacta la arquitectura documental y contractual de fases posteriores

## Open Questions

- Que formatos deben quedar realmente habilitados en esta fase inicial: `csv` solamente o tambien `xlsx`.
- Si `selectedRows` debe ocultarse o mostrarse disabled cuando no haya seleccion.
- Si la primera version debe incluir ya nombre de archivo derivado de `reportMeta`.
- Si el encabezado ejecutivo debe limitarse a `xlsx` en esta fase y dejar `pdf` para un ticket posterior.
