# SCRUMCORE-258 / SCRUMCORE-274 - Reorganizacion Toolbar Digitalizacion

## Toolbar Actual

El toolbar del Preview PDF acumulaba acciones de organizacion, navegacion, edicion y visualizacion en una sola fila sin separadores funcionales. Con el organizador de paginas, seleccion de area y controles de zoom, la lectura visual quedaba poco clara.

## Toolbar Propuesto

El toolbar mantiene los mismos comandos, pero se divide en cuatro grupos:

1. Edicion: `Rotar izquierda`, `Rotar derecha`, `Seleccionar area`, `Eliminar pagina`, `Limpiar documento`
2. Visualizacion: `Reducir zoom`, `Aumentar zoom`, `Ajustar ancho`, `Ajustar pagina`, `Pantalla completa`
3. Organizacion: `Organizar paginas`
4. Navegacion: campo `Pagina` y `Buscar pagina`

Todos los comandos usan `AppButton` con icono y tooltip. No hay texto permanente dentro de botones; el unico texto visible es el label del control de pagina porque pertenece al campo de navegacion existente.

## Grupos Funcionales

Los grupos se renderizan con `role="group"` dentro del toolbar `Visualizacion preview`:

- `Edicion`
- `Visualizacion`
- `Organizacion`
- `Navegacion`

CSS agrega separadores verticales entre grupos en escritorio y separadores horizontales cuando el toolbar envuelve en mobile.

## Justificacion UX

- La edicion queda primero para concentrar las acciones de pagina de uso inmediato.
- La visualizacion queda al lado de edicion porque afecta la inspeccion del documento.
- La organizacion queda separada como entrada global al organizador.
- La navegacion cierra el toolbar con el flujo `Pagina [___]`.
- Los estados disabled se conservan cuando no hay paginas o no existe pagina seleccionada.

## Mockup Final

```text
[Rotar izq] [Rotar der] [Seleccion] [Eliminar] [Limpiar] | [Zoom -] [Zoom +] [Ancho] [Pagina] [Full] | [Organizar] | Pagina [___] [Buscar]
```

## Validacion

- `npx tsc --noEmit`
- `npx eslint` sobre archivos TS/TSX afectados
- Vitest del digitalizador
