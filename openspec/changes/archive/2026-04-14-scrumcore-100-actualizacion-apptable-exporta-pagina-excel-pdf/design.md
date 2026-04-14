## Context

Actualmente `AppTableExport` decide si una combinacion (format, mode) es ejecutable usando:
- `shouldUseBackendAppTableExport(...)` y `isAppTableExportExecutable(...)` en `src/app/Components/UI/AppTable/AppTableExport.types.ts`.
- Un `dataSource` que opcionalmente expone `getBackendExportFile(...)`.

Estado actual relevante:
- `shouldUseBackendAppTableExport(...)` solo considera backend export para `mode === "allMatching"`.
- Para modos client-side, `downloadAppTableExportFile(...)` solo soporta `csv` (ver `src/app/Components/UI/AppTable/AppTableExport.utils.ts`), por lo que `xlsx/pdf` quedan deshabilitados para `currentPage` y `selectedRows`.

Caso observado:
- En `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx` se muestran opciones de exportacion para `currentPage`, `selectedRows` y `allMatching`.
- El backend de referencia (`WorkflowInboxService.cs`) soporta `Format: csv/xlsx/pdf` y `ExportMode: AllMatching/CurrentPage` (no soporta `selectedRows`).

Restriccion clave:
- `AppTableExport` es shared component; no puede acoplarse al modulo ni conocer particularidades del backend. La unica evidencia explicita de capacidad backend hoy es `dataSource.getBackendExportFile`.


## Goals / Non-Goals

**Goals:**
- Permitir `xlsx/pdf` en `currentPage` cuando exista `dataSource.getBackendExportFile` (backend export disponible).
- Mantener compatibilidad hacia atras: si `getBackendExportFile` no existe, el comportamiento debe permanecer igual (solo `csv` fuera de `allMatching`).
- Mantener `selectedRows` como exportacion client-side limitada a `csv`, evitando invocar backend con `ExportMode=selectedRows`.
- Corregir estados de UI (disabled y labels) para no mostrar "(proximamente)" cuando exista al menos una combinacion ejecutable para el formato.

**Non-Goals:**
- Agregar soporte client-side para `xlsx/pdf` (no se implementara generacion local de Excel/PDF).
- Cambiar el contrato publico de `AppTableExportDataSource` en esta iteracion (no se agregan nuevos flags/capabilities).
- Implementar soporte backend para `selectedRows` (queda para un ticket/backlog aparte).


## Decisions

1. **Expandir backend export a `currentPage` (ademas de `allMatching`)**
   - Decision: `shouldUseBackendAppTableExport` debe devolver `true` cuando exista `getBackendExportFile` y `mode` sea `allMatching` o `currentPage`, para `format` en `csv/xlsx/pdf`.
   - Racional: el backend de referencia soporta `CurrentPage`; el shared component no tiene otra forma de conocerlo mas que por la existencia de `getBackendExportFile`.
   - Alternativas consideradas:
     - (A) Mantener backend solo en `allMatching` y forzar CSV en `currentPage`: mantiene status quo pero deja UX incorrecta.
     - (B) Agregar nuevo capability `getBackendExportCapabilities()` o `backendModes`: mas correcto pero rompe contrato y requiere migracion. Se descarta en este ticket por alcance/compatibilidad.

2. **Mantener `selectedRows` como CSV-only (client-side)**
   - Decision: `selectedRows` no debe activar backend export automaticamente, aun si existe `getBackendExportFile`.
   - Racional: el backend de referencia falla para `selectedRows`; habilitarlo implicaria errores funcionales en tiempo de ejecucion.
   - Alternativas consideradas:
     - (A) Permitir backend export en `selectedRows` cuando existe `getBackendExportFile`: riesgo alto de incompatibilidad y errores.
     - (B) Extender contrato para declarar modos backend soportados: se deja como mejora futura.

3. **UI: evaluar ejecutabilidad por combinacion (format, mode) y derivar label parent**
   - Decision: el label parent (p. ej. "Exportar en Excel") debe basarse en si existe al menos un `mode` habilitado para ese formato.
   - Racional: evitar mensajes engañosos "(proximamente)" cuando al menos una opcion (p. ej. `currentPage`) sea ejecutable.


## Risks / Trade-offs

- [Riesgo] Habilitar backend export para `currentPage` podria fallar en algunos consumidores si su backend solo soporta `allMatching`.
  → Mitigacion: mantener `selectedRows` bloqueado para backend; y respaldar con pruebas unitarias enfocadas en reglas actuales. Si aparece un consumidor con backend parcial, se debera ajustar su `getBackendExportFile` o evolucionar el contrato con capacidades declaradas.

- [Riesgo] La existencia de `getBackendExportFile` no garantiza soporte de todos los modos.
  → Mitigacion: limitar el cambio a `currentPage` (caso confirmado) y documentar el trade-off; proponer futura evolucion del contrato si aparecen backends que no soporten `currentPage`.

- [Riesgo] Cambios en labels/disabled pueden introducir regresiones visuales.
  → Mitigacion: agregar pruebas de integracion UI (Testing Library) sobre el dropdown para verificar estados de items.


## Migration Plan

1. Ajustar `shouldUseBackendAppTableExport` e `isAppTableExportExecutable` para habilitar backend en `currentPage`.
2. Ajustar derivacion de items/labels en `AppTableExport.tsx` si es necesario para reflejar correctamente combinaciones ejecutables.
3. Agregar/ajustar pruebas unitarias:
   - reglas de ejecutabilidad (`*.types.ts`)
   - render/disabled states del menu (`AppTableExport`)
4. Validar que consumidores sin `getBackendExportFile` mantienen el mismo comportamiento.
5. (Opcional) Documentar en README/guia de `AppTableExport` que `getBackendExportFile` habilita `currentPage` backend export.

Rollback:
- Revertir cambios en `AppTableExport.types.ts` y `AppTableExport.tsx` si se detecta un consumidor cuyo backend no soporta `currentPage`.


## Open Questions

- ¿Existen otros consumidores con `getBackendExportFile` cuyo backend NO soporte `currentPage` (solo `allMatching`)?
- ¿Se quiere evolucionar el contrato con una declaracion explicita de capacidades (modes soportados por backend) para evitar inferencias por presencia de `getBackendExportFile`?

