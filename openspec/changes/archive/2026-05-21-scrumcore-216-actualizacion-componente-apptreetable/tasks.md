## Tasks

## 1. Baseline & Safety

- [x] 1.1 Revisar API actual de `AppTreeTable` y listar props/behavior legacy (sin breaking changes)
- [x] 1.2 Identificar puntos de integracion en `AppTable` (rows, selection, actions, estados) que `AppTreeTable` debe reutilizar
- [x] 1.3 Identificar consumidores clave (minimo `DocumentosWorkbench`) para validar compatibilidad
- [x] 1.4 Verificar restricciones del ticket (no reemplazar `AppTable`, no hardcodear columnas/acciones, no acoplar a `GestionCorrespondencia`, no modificar `vite.config.ts`)

## 2. Types & Contracts

- [x] 2.1 Definir/ajustar tipos publicos `AppTreeTableRow` (mantener compatibilidad)
- [x] 2.2 Crear tipos internos `TreeTableRow` (level/parent/expanded/hasChildren/originalNode)
- [x] 2.3 Validar que no se introduce `any` ni relajacion de TS strict

## 3. Adapters (Tree -> Table)

- [x] 3.0 Normalizar notacion en docs/codigo a `Tree -> Table` (evitar caracteres especiales en markdown)
- [x] 3.1 Implementar `flattenTree()` (Tree -> lista lineal con level/parent/hasChildren)
- [x] 3.2 Implementar `resolveTreeIndentation()` (padding/indent por level)
- [x] 3.3 Implementar `mapTreeRowsToAppTableRows()` (lineal -> filas consumibles por `AppTable`)
- [x] 3.4 Agregar pruebas unitarias de adapters (flattening, indent, mapping)

## 4. Hooks (estado de expansion y filas visibles)

- [x] 4.1 Implementar `useTreeExpansionState()` (expandedIds estable + handlers expand/collapse)
- [x] 4.2 Implementar `useTreeVisibleRows()` (calculo de visibles + memoizacion)
- [x] 4.3 Agregar pruebas unitarias de hooks (expand/collapse, estabilidad entre renders)

## 5. Componente `AppTreeTable` como wrapper de `AppTable`

- [x] 5.1 Reemplazar render interno para usar `AppTable` como engine (sin reimplementar tabla)
- [x] 5.2 Implementar columna principal `label` con affordance expand/collapse + indentacion
- [x] 5.3 Mantener estados legacy: `loading`, `empty`, `error`, `retry` (mensajes en espanol)
- [x] 5.4 Mantener integracion `onSelectRow` (seleccion delegada desde `AppTable` hacia API legacy)
- [x] 5.5 Asegurar callbacks estables y evitar recomputaciones masivas al expandir/contraer
- [x] 5.6 Mantener compatibilidad de acciones/eventos existentes (delegadas por `AppTable`) sin duplicar logica

## 6. Integracion y regresion

- [x] 6.1 Ajustar pruebas de `AppTreeTable` existentes para el nuevo engine (wrapper sobre `AppTable`)
- [x] 6.2 Agregar pruebas de integracion `AppTreeTable -> AppTable` (render base + expand/collapse + seleccion)
- [x] 6.3 Ejecutar/ajustar prueba de integracion en `DocumentosWorkbench` para evitar regresiones
- [x] 6.4 Verificar que consumidores existentes no requieren cambios (API publica intacta)
- [x] 6.4a Garantizar no-regresion: el refactor no debe afectar otros componentes fuera de `AppTreeTable`/`AppTable` y su consumo directo
- [x] 6.5 Asegurar que los tests que cubren el spec incluyen tag `[SPEC:APPTREETABLE-216]` (o el SPEC ID definido) en `describe`/nombre

## 7. Documentacion enterprise

- [x] 7.1 Crear carpeta `docs/Components/AppTreeTable/`
- [x] 7.2 Documentar arquitectura: `docs/Components/AppTreeTable/SCRUMCORE-216-Arquitectura.md` (incluye Mermaid: classDiagram, sequenceDiagram, stateDiagram-v2, trazabilidad a codigo)
- [x] 7.3 Documentar implementacion: `docs/Components/AppTreeTable/SCRUMCORE-216-Implementacion-Detallada.md` (arbol de carpetas impactadas + por capas)
- [x] 7.4 Documentar integracion backend (no aplica): `docs/Components/AppTreeTable/SCRUMCORE-216-Integracion-BackEnd.md` (dejar explicitamente "no aplica" + compatibilidad futura Dynamic UI)
- [x] 7.5 Documentar pruebas: `docs/Components/AppTreeTable/SCRUMCORE-216-Pruebas.md` (matriz de cobertura + evidencia)
- [x] 7.6 Documentar metadata: `docs/Components/AppTreeTable/SCRUMCORE-216-Metadata.md` (ticket, branch, commits, archivos, comandos de test, riesgos residuales)
- [x] 7.2 Registrar evidencia de tests ejecutados (comandos + salida relevante)
