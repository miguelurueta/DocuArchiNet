# PROMPT ARQUITECTONICO  Ticket 05 FE
# Ajustar AppTableExport para soportar backend export en Pagina actual (Excel/PDF)

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Corregir el comportamiento de exportacion en `AppTableExport` para que las opciones **Excel** y **PDF** no queden desactivadas en el modo **Pagina actual** (`currentPage`), respetando estrictamente las capacidades reales del backend y manteniendo compatibilidad hacia atras con consumidores existentes.

Notas:
- `selectedRows` no tiene soporte backend en el backend de referencia; se mantiene como flujo client-side limitado a CSV hasta que exista soporte backend explicito.
- La evidencia explicita de capacidad backend en el shared component es la presencia de `dataSource.getBackendExportFile` (no existe hoy una declaracion mas granular de modos soportados por backend).

El caso reportado es en `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx` (opcion "Reportes"), donde Excel/PDF aparecen como "proximamente" o inactivos para:
- Exportar pagina actual
- Exportar seleccionadas


CONTEXTO EXISTENTE

- Frontend:
  - `src/app/Components/UI/AppTable/AppTableExport.tsx`
  - `src/app/Components/UI/AppTable/AppTableExport.types.ts`
  - `src/app/Components/UI/AppTable/AppTableExport.utils.ts`
  - `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
  - `src/modules/gestionCorrespondencia/hooks/useGestionCorrespondenciaTable.ts`
- Backend (referencia funcional):
  - `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Services\Service\Workflow\BandejaCorrespondencia\WorkflowInboxService.cs`
    - soporta `Format`: `csv`, `xlsx`, `pdf`
    - soporta `ExportMode`: `AllMatching`, `CurrentPage` (case-insensitive)
    - NO soporta `selectedRows` (cualquier otro modo retorna "Modo de exportacion no soportado")

Nota de ubicacion documental:
- Este ticket vive bajo `docs/Architecture/AppTabs/` por convencion del repositorio; aplica al shared component `AppTableExport`.


UBICACION (OBLIGATORIA)

```
src/app/Components/UI/AppTable/
```


RESTRICCIONES (OBLIGATORIAS)

- no consumir APIs dentro de `AppTableExport` (solo usar `dataSource`)
- no acoplarse a un modulo/pantalla especifica
- no usar `any`
- mantener compatibilidad hacia atras (si `dataSource.getBackendExportFile` no existe, comportamiento actual)
- no introducir estilos globales


PROBLEMA (ROOT CAUSE)

En `AppTableExport.types.ts`, `shouldUseBackendAppTableExport(...)` solo devuelve `true` cuando:
- existe `dataSource.getBackendExportFile`
- `mode === "allMatching"`
- `format` es `csv|xlsx|pdf`

Y `isAppTableExportExecutable(...)`:
- devuelve `true` para backend export solo en `allMatching`
- para el resto, solo permite `csv`

Esto provoca que Excel/PDF queden deshabilitados en `currentPage` (y siempre en `selectedRows`), incluso cuando el backend soporta exportar `currentPage`.


CAMBIO REQUERIDO (OBLIGATORIO)

1. HABILITAR BACKEND EXPORT PARA `currentPage`
   - Ajustar `shouldUseBackendAppTableExport(...)` para permitir `mode === "currentPage"` cuando exista `getBackendExportFile`.
   - Ajustar `isAppTableExportExecutable(...)` para que `xlsx/pdf` sean ejecutables en `currentPage` cuando aplique backend export.

2. DEFINIR REGLA PARA `selectedRows`
   - Mantener `selectedRows` como exportacion client-side solo para `csv` (comportamiento actual), a menos que exista soporte backend explicito futuro.
   - No se debe intentar llamar backend con `ExportMode = selectedRows` para no romper modulos cuyo backend no lo soporte.

3. UX EN MENUS
   - Si un formato (xlsx/pdf) no es ejecutable para un modo (p.ej. `selectedRows`), el submenu debe quedar deshabilitado como hoy, sin afectar otros modos.
   - Evitar que el label parent muestre "(proximamente)" cuando exista al menos un modo ejecutable (p.ej. Excel para `currentPage` y/o `allMatching`).


PRUEBAS UNITARIAS (OBLIGATORIAS)

Agregar/ajustar pruebas para cubrir:
- `isAppTableExportExecutable` permite `xlsx/pdf` en `currentPage` cuando `getBackendExportFile` existe.
- `isAppTableExportExecutable` mantiene `selectedRows` como `csv` only si no hay capability backend.
- `AppTableExport` no deshabilita Excel/PDF para `currentPage` cuando `getBackendExportFile` existe.
- `AppTableExport` no intenta ejecutar export si `disabled` o `exportLoading` activo.


CRITERIOS DE ACEPTACION

- En Gestion Correspondencia, Excel/PDF quedan habilitados al menos para:
  - `Pagina actual` (via backend export)
  - `Todos los resultados` (via backend export, como ya ocurre)
- `Seleccionados`:
  - se mantiene habilitado para CSV cuando hay seleccion
  - se mantiene deshabilitado para Excel/PDF si el backend no soporta `selectedRows`
- Sin regresiones en modulos que no proveen `getBackendExportFile`.
