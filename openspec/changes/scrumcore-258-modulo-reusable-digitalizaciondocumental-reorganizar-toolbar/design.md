## Context

SCRUMCORE-258 reorganiza el toolbar del Preview PDF del modulo reutilizable de Digitalizacion Documental.

## Goals / Non-Goals

**Goals**

- Agrupar acciones por contexto funcional.
- Reducir carga visual del toolbar.
- Mantener todos los comandos existentes.
- Usar botones con icono y tooltip, sin texto permanente dentro de botones.
- Documentar toolbar actual, propuesta y justificacion UX.

**Non-Goals**

- No cambiar scanner, preview, miniaturas ni organizador.
- No modificar la logica funcional de rotar, eliminar, limpiar, zoom, seleccionar area o buscar pagina.
- No crear un toolbar nuevo fuera del Preview PDF.

## Decisions

1. El toolbar conserva `role="toolbar"` y agrega cuatro grupos con `role="group"`: Organizacion, Navegacion, Edicion y Visualizacion.
2. El grupo Organizacion contiene solo `Organizar paginas`.
3. El grupo Navegacion conserva el control `Pagina` y cambia el boton a `Buscar pagina`.
4. El grupo Edicion contiene rotacion, seleccion de area, eliminar pagina y limpiar documento.
5. El grupo Visualizacion contiene zoom, fit width, fit page y pantalla completa.
6. Los separadores son CSS, no componentes nuevos, para mantener el cambio acotado.

## Risks / Trade-offs

- Algunos tests previos buscaban el boton `Ir a pagina`; se actualizan al nuevo nombre `Buscar pagina`.
- El label visible `Pagina` se mantiene porque pertenece al input existente, no a un boton.
- `npm run lint` completo puede seguir fallando por deuda preexistente fuera del alcance; se valida lint focalizado.

## Migration Plan

1. Reorganizar JSX del toolbar en grupos funcionales.
2. Ajustar tooltips y aria-labels.
3. Agregar CSS de separadores responsive.
4. Actualizar tests del digitalizador.
5. Crear documentacion en `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-274-toolbar-reorganization.md`.

## Validation

- `npx tsc --noEmit`
- `npx eslint src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx src/app/Components/UI/AppDigitalizador/tests/AppDigitalizador.test.tsx`
- `npm test -- --run src/app/Components/UI/AppDigitalizador/tests/AppDigitalizador.test.tsx`
