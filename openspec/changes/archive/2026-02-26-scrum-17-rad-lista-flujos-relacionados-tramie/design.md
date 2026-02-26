## Context

En `RadicacionForm.tsx`, el campo `data-ident="pl-radicacion-spe-Descripcion_Documento"` (tramite) y el campo `data-ident="pl-radicacion-spe-RE_flujo_trabajo"` (flujo) existen, pero actualmente no estan enlazados por consumo dinamico de API. El ticket `SCRUM-17` requiere que al cambiar tramite se consuma `/api/tramite/tramites/empsolicitaListaflujosRelacionadosTramite` con `idTipoDocEntrante = idValue` del tramite seleccionado, llenando opciones de flujo y limpiandolas cuando no haya valor o respuesta.

## Goals / Non-Goals

**Goals:**
- Capturar el `onChange` del selector de tramite (`Descripcion_Documento`).
- Resolver `idValue` del tramite seleccionado y consumir la API de flujos relacionados.
- Poblar opciones de `RE_flujo_trabajo` con `{ idValue, Value }` retornados.
- Limpiar opciones de flujo cuando `idValue` sea `null` o la API retorne vacio.
- Conservar atributos declarativos existentes (`required`, `disabled`, `title`, `tooltipAyuda`) del campo flujo.
- Centralizar manejo de errores con la misma capa axios/hook reutilizable.
- Dejar componente/logic reutilizable para otros pares campo-origen/campo-destino.

**Non-Goals:**
- Cambiar contratos backend o endpoints existentes.
- Redisenar completo del formulario de radicacion.
- Modificar reglas funcionales de campos no involucrados.

## Decisions

### Decision 1: Hook reutilizable para carga de flujos por tramite
- **Decision:** crear un hook de consulta (React Query + axios centralizado) para obtener flujos por `idTipoDocEntrante`.
- **Rationale:** mantiene consistencia con arquitectura del modulo y facilita reuso para futuros campos dependientes.
- **Alternatives considered:** consumo directo en componente con `fetch`/estado manual (descartado por acoplamiento y duplicacion de errores).

### Decision 2: Estado local de opciones dependientes en `RadicacionForm`
- **Decision:** mantener opciones de flujo en estado local derivado de respuesta del hook y resetearlas en cambios invalidos.
- **Rationale:** control explicito del ciclo limpiar/cargar, evitando opciones stale.
- **Alternatives considered:** recalcular opciones solo con `useMemo` sobre data bruta (descartado por complejidad en resets cuando `idValue` es null).

### Decision 3: Mantener metadata declarativa del campo flujo
- **Decision:** conservar `disabled`, `title_control`, `tooltipAyuda` y labels de `RE_flujo_trabajo`, aplicando un disabled efectivo que considere metadata + estado sin tramite.
- **Rationale:** cumple requerimiento sin romper comportamiento actual de UI.
- **Alternatives considered:** sobreescribir completamente el comportamiento del selector flujo (descartado por riesgo de regresion).

## Risks / Trade-offs

- **[Risk]** Diferencias de tipo entre `idValue` (number/string/null) pueden disparar consultas incorrectas.  
  **Mitigation:** normalizar y validar `idTipoDocEntrante` antes de consultar.
- **[Risk]** Respuesta vacia o error puede dejar opciones obsoletas.  
  **Mitigation:** limpiar opciones al iniciar consulta invalida y en errores.
- **[Risk]** Posible sobreconsulta al cambiar rapido de tramite.  
  **Mitigation:** usar cache/query-key por id y control de habilitacion `enabled`.

## Migration Plan

1. Crear servicio/hook para consumir flujos relacionados por `idTipoDocEntrante`.
2. Integrar `onChange` en selector de tramite para actualizar el id seleccionado.
3. Poblar/limpiar opciones del selector flujo segun respuesta/valor.
4. Mantener metadatos declarativos de flujo (required, disabled, title, tooltip).
5. Agregar pruebas unitarias de comportamiento y manejo de error.

Rollback:
- Revertir commit del cambio `SCRUM-17`; no requiere migracion de datos.

## Open Questions

- Confirmar si flujo debe quedar siempre deshabilitado mientras no exista tramite valido o solo cuando metadata lo indique.
- Confirmar si al cambiar tramite debe limpiar tambien el valor ya seleccionado en flujo (ademas de las opciones).
