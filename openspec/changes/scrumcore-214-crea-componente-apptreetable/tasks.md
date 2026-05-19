## 1. Setup

- [ ] 1.1 Revisar spec SCRUM-205 relacionada (backend-driven) y alinear nomenclatura de DTO/metadata
- [ ] 1.2 Definir API pública del componente (`props` + tipos) en `src/app/Components/UI/AppTreeTable/`

## 2. Core Implementation

- [ ] 2.1 Crear estructura base del componente `AppTreeTable` (index + component + styles)
- [ ] 2.2 Implementar render de árbol (expand/collapse) en modo `rows` (sin loader)
- [ ] 2.3 Implementar modo backend-driven con `load()` (loading/empty/error)
- [ ] 2.4 Implementar callbacks tipados (selección, expand/collapse) según spec
- [ ] 2.5 Asegurar aislamiento: no efectos globales (no navegación, no side effects fuera del componente)

## 3. Tests (Vitest + RTL)

- [ ] 3.1 Test `[SPEC:APP-APPTREETABLE-001]` renderiza filas jerárquicas desde `rows`
- [ ] 3.2 Test `[SPEC:APP-APPTREETABLE-002]` renderiza loading/empty/error con `load()` mock
- [ ] 3.3 Test `[SPEC:APP-APPTREETABLE-003]` expand/collapse muestra/oculta hijos correctamente

## 4. Documentación enterprise (SCRUMCORE-214)

- [ ] 4.1 Crear `SCRUMCORE-214-Metadata.md` (branch/commits/tests)
- [ ] 4.2 Crear `SCRUMCORE-214-Comportamiento-del-Componente.md`
- [ ] 4.3 Crear `SCRUMCORE-214-Arquitectura-Tecnica.md` (Mermaid)
- [ ] 4.4 Crear `SCRUMCORE-214-Testing-Enterprise.md` (evidencias unit)
