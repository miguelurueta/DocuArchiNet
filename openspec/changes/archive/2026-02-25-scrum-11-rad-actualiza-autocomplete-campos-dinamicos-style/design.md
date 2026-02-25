## Context

En `RadicacionForm.tsx` y en los renderers de campos dinámicos se mezclan campos estáticos y campos de tipo `AUTOCOMPLETE`/`SELECCION` definidos por `camposPlantilla`.
El ticket `SCRUM-11` solicita actualizar el estilo visual de los `autocomplete` dinámicos para mantener consistencia de UI y evitar diferencias entre secciones del formulario.

El ajuste impacta principalmente:
- `CamposPlantillaAutoCompleteRenderer.tsx`
- estilos de `FormRadicacion.module.css`
- pruebas de rendering del módulo de radicación.

## Goals / Non-Goals

**Goals:**
- Unificar el estilo de los campos `AUTOCOMPLETE` dinámicos bajo una guía visual consistente.
- Mantener comportamiento funcional existente (consulta, selección, ingreso manual y accesibilidad).
- Dejar cobertura de pruebas para evitar regresiones de estilo/estructura esperada.

**Non-Goals:**
- Cambiar contratos de API o payloads de autocompletado.
- Reescribir la arquitectura del formulario de radicación.
- Modificar estilos de módulos no relacionados.

## Decisions

### Decisión 1: Centralizar estilos en el renderer de autocomplete dinámico
El estilo objetivo se aplicará en `CamposPlantillaAutoCompleteRenderer` para que todos los campos dinámicos compartan clases y estructura homogénea.

Alternativa descartada:
- Ajustar estilos campo por campo en `RadicacionForm.tsx`.
  - Se descarta por duplicar lógica y aumentar mantenimiento.

### Decisión 2: Mantener semántica y atributos de accesibilidad existentes
No se alterarán `aria-label`, `aria-describedby`, `required`, `disabled`, ni data attributes de trazabilidad.

Alternativa descartada:
- Simplificar markup removiendo atributos de accesibilidad.
  - Se descarta por impacto funcional y de QA automatizado.

### Decisión 3: Validar con tests de componente
Se actualizarán tests en `RadicacionForm.spec.test.tsx` y/o `CamposPlantillaAutoCompleteRenderer.spec.test.tsx` para validar presencia de clases/estructura esperada y no-regresión funcional.

Alternativa descartada:
- Validación manual sin tests.
  - Se descarta por riesgo de regresión recurrente.

## Risks / Trade-offs

- [Riesgo] Un cambio de clases puede romper selectores existentes de pruebas E2E.
  → Mitigación: preservar `data-ident` y actualizar pruebas afectadas.

- [Riesgo] Cambios visuales en AntD pueden tener diferencias por versión.
  → Mitigación: aplicar estilos sobre clases propias del módulo, no sobre internos frágiles de AntD.

- [Trade-off] Mayor especificidad CSS para forzar consistencia.
  → Mitigación: mantener el alcance en `FormRadicacion.module.css` y evitar `!important` salvo necesidad puntual.

## Migration Plan

1. Ajustar estilos/clases de `AUTOCOMPLETE` dinámicos en renderer y módulo CSS.
2. Verificar que `ASUNTO`, `ANEXOS_COR` y campos dinámicos equivalentes mantengan comportamiento.
3. Actualizar pruebas unitarias relacionadas.
4. Ejecutar suite del módulo radicación.
5. En caso de regresión visual crítica, rollback de clases nuevas manteniendo cambios funcionales.

## Open Questions

- ¿La actualización de style aplica solo a campos dinámicos de `Datos especializados` o también a `ASUNTO` y `ANEXOS_COR` en tarjetas base?
- ¿Existe un token de diseño corporativo que deba usarse para estados focus/hover en autocomplete?
