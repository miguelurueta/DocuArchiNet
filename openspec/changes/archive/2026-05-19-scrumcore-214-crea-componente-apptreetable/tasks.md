## 1. Setup

- [x] 1.1 Revisar spec SCRUM-205 relacionada (backend-driven) y alinear nomenclatura de DTO/metadata
- [x] 1.2 Definir API pública del componente (`props` + tipos) en `src/app/Components/UI/AppTreeTable/`

## 2. Core Implementation

- [x] 2.1 Crear estructura base del componente `AppTreeTable` (index + component + styles)
- [x] 2.2 Implementar render de árbol (expand/collapse) en modo `rows` (sin loader)
- [x] 2.3 Implementar modo backend-driven con `load()` (loading/empty/error)
- [x] 2.4 Implementar callbacks tipados (selección, expand/collapse) según spec
- [x] 2.5 Asegurar aislamiento: no efectos globales (no navegación, no side effects fuera del componente)
- [x] 2.6 Integrar `AppTreeTable` en `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx` dentro del “Listado” en `AppCollapseRail` sin afectar AppVisor ni otras regiones

## 3. Tests (Vitest + RTL)

- [x] 3.1 Test `[SPEC:APP-APPTREETABLE-001]` renderiza filas jerárquicas desde `rows`
- [x] 3.2 Test `[SPEC:APP-APPTREETABLE-002]` renderiza loading/empty/error con `load()` mock
- [x] 3.3 Test `[SPEC:APP-APPTREETABLE-003]` expand/collapse muestra/oculta hijos correctamente
- [x] 3.4 Test `[SPEC:APP-APPTREETABLE-006]` DocumentosWorkbench renderiza AppTreeTable en Listado/AppCollapseRail sin afectar el contenedor del AppVisor (smoke test)

## 4. Documentación enterprise (SCRUMCORE-214)

- [x] 4.1 Crear `SCRUMCORE-214-Metadata.md` (branch/commits/tests)
- [x] 4.2 Crear `SCRUMCORE-214-Comportamiento-del-Componente.md`
- [x] 4.3 Crear `SCRUMCORE-214-Arquitectura-Tecnica.md` (Mermaid)
- [x] 4.4 Crear `SCRUMCORE-214-Testing-Enterprise.md` (evidencias unit)
