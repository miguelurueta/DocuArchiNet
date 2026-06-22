# SCRUMCORE-280 - Seleccion multiple de paginas

## Motivacion

El modulo de digitalizacion ya permite escanear, previsualizar, reordenar, rotar, eliminar, recortar y generar PDF. SCRUMCORE-261 extiende esa base para permitir seleccionar varias paginas y ejecutar acciones masivas sin duplicar estado entre el panel de miniaturas y el organizador.

## Flujo UX

1. El usuario escanea o carga paginas en el buffer de digitalizacion.
2. El panel de miniaturas muestra un checkbox por pagina.
3. Click normal sobre una miniatura cambia la pagina activa.
4. Ctrl+click o Cmd+click sobre una miniatura alterna su seleccion y tambien la deja activa.
5. El checkbox permite seleccionar o deseleccionar sin cambiar el flujo de drag and drop.
6. La barra de miniaturas muestra la cantidad seleccionada y permite seleccionar/deseleccionar todo.
7. `Seleccionar todo` marca todas las paginas visibles del documento.
8. `Deseleccionar todo` limpia la seleccion.
9. Cuando hay seleccion, la toolbar del preview conserva los mismos botones; no aparece una barra secundaria.
10. El badge contextual `N seleccionadas` aparece antes del campo `Pagina`.
11. Los botones existentes de rotar y eliminar aplican a la seleccion cuando `selectedPageIds` no esta vacio.

## Estados

| Estado | Descripcion |
| --- | --- |
| `selectedPageId` | Pagina activa para preview, zoom, crop individual y navegacion. |
| `selectedPageIds` | Fuente unica de paginas seleccionadas para acciones masivas. |
| Sin paginas | No hay seleccion posible; el panel muestra estado vacio. |
| Sin seleccion | Se conservan acciones individuales sobre la pagina activa. |
| Seleccion parcial | Badge contextual `N seleccionadas` y botones existentes aplicados a la seleccion. |
| Seleccion total | Accion de barra cambia a `Deseleccionar todo`. |
| Paginas eliminadas | La seleccion se depura para remover IDs que ya no existen. |
| Organizador abierto | Reutiliza `selectedPageIds`; no mantiene seleccion paralela. |

## Arquitectura

La seleccion multiple se centraliza en `DigitalizacionDocumentalWorkspace` mediante el modelo `SelectedPageIds`.

```text
DigitalizacionDocumentalWorkspace
  selectedPageId      -> pagina activa
  selectedPageIds     -> seleccion multiple
  Miniaturas          -> checkbox / Ctrl+click / seleccionar todo
  Toolbar preview     -> una sola toolbar, botones existentes con alcance contextual
  Organizador paginas -> reutiliza selectedPageIds
  Scanner client      -> rotatePage / removePage / reorderPages
```

Las acciones masivas recorren las paginas en el orden actual de `scanner.pages`, filtrando por `selectedPageIds`. Esto evita depender del orden de insercion del `Set` y mantiene coherencia despues de reordenamientos.

## Decisiones

- `selectedPageIds` reemplaza la seleccion interna previa del organizador.
- El click normal no altera la seleccion multiple; solo cambia la pagina activa.
- Ctrl+click/Cmd+click alterna seleccion y pagina activa.
- La eliminacion masiva solicita confirmacion antes de invocar `removePage`.
- No se crea toolbar secundaria para seleccion multiple.
- Crop masivo queda documentado como capacidad futura; no se agrega control duplicado ni accion visible hasta validar su implementacion.
- No se modifica el contrato `DigitalizacionScannerClient`; se reutilizan `rotatePage` y `removePage` por pagina.

## Consolidacion de Toolbar

La experiencia final mantiene una sola toolbar de preview. La seleccion multiple vive unicamente en el estado `selectedPageIds`; no introduce un contenedor independiente de acciones masivas.

Sin seleccion:

```text
Rotar izquierda | Rotar derecha | Seleccionar area | Eliminar pagina | ...
```

Con seleccion multiple:

```text
Rotar izquierda | Rotar derecha | Seleccionar area | Eliminar pagina | ... | [N seleccionadas] Pagina [ ]
```

Reglas:

- `Rotar izquierda` aplica `270` grados a todas las paginas seleccionadas cuando `selectedPageIds.size > 0`.
- `Rotar derecha` aplica `90` grados a todas las paginas seleccionadas cuando `selectedPageIds.size > 0`.
- `Eliminar pagina` elimina todas las paginas seleccionadas cuando `selectedPageIds.size > 0`.
- Si no hay seleccion multiple, los mismos botones conservan su comportamiento individual sobre `selectedPageId`.
- No se renderizan botones `Rotar derecha seleccionadas`, `Rotar izquierda seleccionadas`, `Eliminar paginas seleccionadas` ni `Aplicar crop seleccionadas` en el preview.
- El organizador conserva sus controles propios para no cambiar su flujo ni el drag and drop.

## Riesgos

- En lotes grandes, invocar muchas rotaciones o eliminaciones una por una puede requerir una API batch futura.
- Si el adapter del scanner procesa operaciones secuenciales lentamente, el usuario puede percibir demora en 100+ paginas.
- Crop masivo requiere validar que una misma seleccion sea segura en paginas con tamanos u orientaciones diferentes.
- La confirmacion usa `window.confirm`; si producto requiere experiencia corporativa, debe reemplazarse por modal reusable en otro ticket.

## Casos de Prueba

| Caso | Resultado esperado |
| --- | --- |
| Ctrl+click miniatura | La pagina queda activa y se agrega/remueve de `selectedPageIds`. |
| Checkbox miniatura | La pagina se agrega/remueve de la seleccion. |
| Seleccionar todo | Todas las paginas quedan seleccionadas. |
| Deseleccionar todo | `selectedPageIds` queda vacio. |
| Rotar derecha con seleccion | El boton existente `Rotar derecha` llama `rotatePage(pageId, 90)` para cada pagina seleccionada. |
| Rotar izquierda con seleccion | El boton existente `Rotar izquierda` llama `rotatePage(pageId, 270)` para cada pagina seleccionada. |
| Eliminar pagina con seleccion | El boton existente `Eliminar pagina` solicita confirmacion y llama `removePage(pageId)` para cada pagina. |
| Toolbar consolidada | No existe toolbar secundaria ni botones masivos duplicados en preview. |
| Cancelar eliminacion | No llama `removePage`. |
| Organizador 2x2 a 6x6 | Mantiene seleccion y drag and drop usando el mismo estado. |
| 10, 50, 100, 300 paginas | La UI conserva virtualizacion CSS y no duplica estado por vista. |

## Pendientes

- Implementar crop masivo real despues de validar reglas de coordenadas entre paginas.
- Evaluar API batch en el scanner client para operaciones de alto volumen.
- Reemplazar confirmacion nativa por modal corporativo si UX lo requiere.
