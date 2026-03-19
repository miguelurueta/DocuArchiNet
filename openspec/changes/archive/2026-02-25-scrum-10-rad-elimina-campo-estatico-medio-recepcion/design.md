## Context

El formulario de radicación mezcla campos fijos en `RadicacionForm.tsx` con campos dinámicos construidos desde `camposPlantilla`.
Actualmente existe un campo fijo con `data-ident="pl-radicacion-spe-Medio-recep"` en la tarjeta "Medio de Recepción del Trámite", mientras que la plantilla también puede definir campos relacionados como `MEDIORECEPCION`.

Esta duplicidad produce dos problemas:
- Inconsistencia funcional y visual cuando metadata de plantilla y campo fijo no coinciden.
- Mayor costo de mantenimiento porque reglas (label, tooltip, obligatoriedad, opciones) quedan repartidas.

Stakeholders: equipo funcional de radicación, QA y usuarios operativos que diligencian el formulario.

## Goals / Non-Goals

**Goals:**
- Eliminar el campo fijo `pl-radicacion-spe-Medio-recep` del render principal.
- Mantener una sola fuente de verdad para "medio de recepción" basada en metadata de plantilla.
- Evitar regresiones en el flujo general del formulario y en selectores usados por automatización.

**Non-Goals:**
- Rediseñar la estructura completa del formulario.
- Cambiar contratos de backend o el formato de `camposPlantilla`.
- Reemplazar Ant Design o la arquitectura del módulo.

## Decisions

### Decisión 1: Remover el bloque estático de "Tipo de Recepción" en `RadicacionForm.tsx`
Se elimina el `Form.Item` estático que usa `data-ident="pl-radicacion-spe-Medio-recep"`.

Rationale:
- Evita duplicidad con campos dinámicos de plantilla.
- Reduce riesgo de divergencia entre UX codificada y configuración funcional.

Alternativas consideradas:
- Ocultar el campo fijo condicionalmente solo cuando exista `MEDIORECEPCION` dinámico.
  - Descartada por complejidad adicional y coexistencia de dos caminos de render.

### Decisión 2: Consolidar el comportamiento en el renderer dinámico existente
La captura de "medio de recepción" queda soportada por `CamposPlantillaAutoCompleteRenderer`/`CamposPlantillaRenderer` según `ComportamientoCampo` y metadata.

Rationale:
- Reutiliza rutas ya probadas de label, tooltip, required, disabled y opciones.
- Mantiene consistencia con otros campos parametrizados del formulario.

Alternativas consideradas:
- Crear un renderer nuevo exclusivo para MEDIORECEPCION.
  - Descartada por duplicar responsabilidades ya cubiertas.

### Decisión 3: Ajustar pruebas para validar ausencia del campo fijo y presencia del dinámico
Se agregarán/actualizarán tests de `RadicacionForm` para comprobar:
- El campo fijo eliminado no se renderiza.
- El campo dinámico configurado por plantilla sí se renderiza y conserva atributos.

Rationale:
- Bloquea regresiones futuras en cambios de layout.

## Risks / Trade-offs

- [Riesgo] Plantillas sin definición de `MEDIORECEPCION` podrían dejar el formulario sin ese dato.
  → Mitigación: validar cobertura de plantillas activas y definir fallback funcional si negocio lo exige.

- [Riesgo] Automatizaciones QA que apuntan al `data-ident` antiguo fallarán.
  → Mitigación: actualizar selectores de pruebas e informar cambio en notas técnicas.

- [Trade-off] Menos control hardcodeado en frontend.
  → Se gana coherencia de configuración centralizada en metadata.

## Migration Plan

1. Eliminar el bloque estático `pl-radicacion-spe-Medio-recep` de `RadicacionForm.tsx`.
2. Verificar que el campo dinámico `MEDIORECEPCION` se visualiza correctamente desde plantilla.
3. Actualizar tests unitarios del formulario para cubrir el cambio.
4. Ejecutar suite de pruebas del módulo radicación.
5. Si en QA aparece ausencia de campo por plantilla incompleta, rollback rápido reintroduciendo temporalmente el bloque fijo.

## Open Questions

- ¿Todas las plantillas productivas incluyen `MEDIORECEPCION` cuando el proceso lo requiere?
- ¿El negocio requiere que "medio de recepción" permanezca en la misma tarjeta visual o puede quedar solo en "Datos especializados"?
