## Context

SCRUM-19 requiere que el formulario capture el `idValue` seleccionado en el campo `Descripcion_Documento` (`data-ident="pl-radicacion-spe-Descripcion_Documento"`) y consulte un endpoint de restriccion por tipo de tramite.

Estado actual del codigo:
- `RadicacionForm` ya maneja `onChange` del tramite y guarda un id en estado para otros flujos.
- Existe infraestructura de hooks con React Query para llamadas GET/POST en Radicacion.
- El campo destinatario ya consume reglas de restriccion para habilitar/deshabilitar comportamiento, por lo que esta nueva consulta debe integrarse sin romper el flujo existente.

Restricciones:
- Mantener TypeScript estricto y capa de consumo API centralizada.
- Reusar patrones de hooks y normalizacion existentes.
- Evitar regresiones en selects dependientes (por ejemplo `RE_flujo_trabajo`).

## Goals / Non-Goals

**Goals:**
- Capturar el `idValue` del tramite al seleccionar en `Descripcion_Documento`.
- Consumir `GET /api/tramite/tramites/solicitaEstructuraRelacionTipoRestriccion` enviando el id seleccionado como parametro.
- Normalizar la respuesta para uso consistente en la UI.
- Mantener el comportamiento actual del formulario y permitir extension futura.

**Non-Goals:**
- Cambiar contrato del backend o endpoint.
- Rediseñar el formulario o componentes de seleccion.
- Reemplazar arquitectura de hooks/React Query existente.

## Decisions

1. Crear hook dedicado para estructura de restriccion por tramite.
- Decision: implementar `useEstructuraRelacionTipoRestriccion` con React Query y query param tipado.
- Rationale: encapsula llamada GET, normalizacion y control de errores sin ensuciar el componente.
- Alternativa descartada: llamada directa en `onChange` del componente.

2. Reusar `selectedTramiteId` como fuente de verdad.
- Decision: usar el estado ya existente del tramite seleccionado para disparar la consulta.
- Rationale: evita duplicar estado y mantiene coherencia con otras dependencias (flujos relacionados).
- Alternativa descartada: estado adicional solo para restriccion.

3. Mantener manejo de errores no intrusivo.
- Decision: error controlado en hook con fallback seguro (valor vacio) para no bloquear formulario.
- Rationale: el usuario debe poder continuar si la consulta falla temporalmente.
- Alternativa descartada: lanzar error bloqueante en UI principal.

## Risks / Trade-offs

- [Riesgo] `idValue` vacio o invalido al seleccionar opcion sin id → Mitigacion: normalizacion defensiva y no consulta cuando no hay id valido.
- [Riesgo] Dependencias entre consultas pueden generar estados intermedios inconsistentes → Mitigacion: desacoplar query keys y limpiar estado derivado al cambiar tramite.
- [Riesgo] Cambio de contrato backend en nombres de campos → Mitigacion: parser tolerante y pruebas de normalizacion.

## Migration Plan

1. Crear hook `useEstructuraRelacionTipoRestriccion` con request GET y normalizacion.
2. Integrar hook en `RadicacionForm` atado a `selectedTramiteId` del campo `Descripcion_Documento`.
3. Actualizar pruebas unitarias del formulario y hook para validar llamada, params y fallback en error.
4. Ejecutar suite de Radicacion y validar manualmente seleccion de tramite.

Rollback:
- Revertir integración del hook y restaurar flujo actual de selección sin consulta adicional.

## Open Questions

- Confirmar nombre exacto del query param esperado por backend para el id del tramite.
- Confirmar si la respuesta debe afectar solo restriccion de destinatario o tambien otros campos.
- Definir comportamiento esperado cuando la API retorna estructura vacia.
