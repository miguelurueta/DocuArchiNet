## Context

El ticket SCRUMCORE-214 solicita un componente reusable **AppTreeTable** (frontend) con comportamiento **backend-driven**, compatible con el enfoque definido en SCRUM-205.

El repositorio ya cuenta con:

- React 19 + Vite + TypeScript estricto.
- UI stack mixto (Ant Design + MUI).
- Patrón de componentes UI reutilizables en `src/app/Components/UI/**`.

Este cambio crea una base técnica y de contrato para un nuevo componente UI, sin alterar arquitectura global ni rutas.

## Goals / Non-Goals

**Goals:**

- Definir una arquitectura de componente **reusable** `AppTreeTable` con una API estable.
- Asegurar compatibilidad con consumo **backend-driven** (datos + configuración/metadata desde API).
- Asegurar una integración limpia: sin afectar otros plugins/componentes del proyecto.
- Incluir tests de comportamiento (Vitest + RTL) alineados a OpenSpec.

**Non-Goals:**

- No implementar un árbol/tabla “hardcoded” por un módulo específico.
- No acoplar el componente a un endpoint único: el componente debe recibir `dataSource`/`loader` o `props` (según spec).
- No introducir nuevas dependencias grandes si no son estrictamente necesarias.

## Decisions

### 1) Ubicación y naming

**Decisión:** crear el componente en `src/app/Components/UI/AppTreeTable/`.

**Por qué:**

- Sigue el patrón existente de UI reusable en `src/app/Components/UI/**`.
- Permite versionar y testear de manera aislada.

**Alternativas consideradas:**

- `src/shared/components`: descartado porque la convención actual usa `src/app/Components/UI/**` para UI reusable.

### 2) Modelo “backend-driven”

**Decisión:** soportar un modo backend-driven con dos capas:

1) **DTO de respuesta** (data + metadata) tipado en `src/app/Components/UI/AppTreeTable/types.ts`.
2) **Adapter/mapper** opcional que transforma el DTO a un `ViewModel` consumido por la UI.

**Por qué:**

- Mantiene el componente UI desacoplado del shape exacto del backend.
- Permite evolución del contrato sin romper el render principal.

**Alternativas consideradas:**

- Render directo contra DTO del backend: descartado por acoplamiento y baja mantenibilidad.

### 3) Contrato del componente

**Decisión:** exponer un contrato basado en:

- `rows`: datos ya normalizados (si el consumidor no quiere backend-driven).
- `load()` o `dataSource`: loader async (si el consumidor quiere backend-driven).
- callbacks opcionales (`onSelect`, `onExpand`, etc.) definidos por spec.

**Por qué:**

- Permite dos modos de adopción: inmediato (props) o completo (loader).
- Facilita testear comportamiento sin red real (mock de loader).

### 4) UI library

**Decisión:** reutilizar Ant Design para los controles base (por consistencia con otros UI components ya presentes) y aislar estilos con CSS Modules.

**Por qué:**

- Ya existe Ant Design en el repo.
- Reduce riesgo de inconsistencias visuales y de bundle.

**Alternativas consideradas:**

- MUI TreeView/Datagrid: descartado por peso/compatibilidad y por requerir decisiones de licencia/feature-set.

## Risks / Trade-offs

- **[Riesgo] Contrato SCRUM-205 no está explícito en este change** → Mitigación: capturar requisitos mínimos en la spec y dejar “Open Questions” para validar con el ticket SCRUM-205.
- **[Riesgo] Backend-driven requiere metadata compleja (columnas/acciones/permisos)** → Mitigación: diseñar un DTO extensible y mapear a ViewModel.
- **[Trade-off] Soportar 2 modos (props vs loader) aumenta superficie** → Mitigación: definir prioridades claras en spec (ej. loader tiene precedencia).

## Migration Plan

- Crear componente `AppTreeTable` sin consumidores.
- Agregar un consumidor mínimo (si el ticket lo exige) en un módulo real, detrás de una integración no invasiva.
- Agregar tests unitarios (Vitest + RTL) para el contrato base.
- Rollback: revertir carpeta del componente y su export (sin tocar módulos existentes).

## Open Questions

- ¿Cuál es el contrato exacto “backend-driven” de SCRUM-205 (shape y reglas)?
- ¿Se requiere paginación/ordenamiento/filtros y quién los controla (cliente vs servidor)?
- ¿Hay requerimientos de permisos/claims para acciones por fila?
- ¿Se requiere soporte de “lazy children load” (expand/async) o se entrega el árbol completo?
