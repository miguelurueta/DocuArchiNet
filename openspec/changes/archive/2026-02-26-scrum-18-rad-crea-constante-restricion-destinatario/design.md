## Context

El ticket `SCRUM-18` solicita crear en `RadicacionForm.tsx` una constante llamada `CDeRelacionEstadoRetriccionDto` para soportar reglas de restriccion asociadas al campo destinatario. Actualmente la definicion no existe de forma explicita y centralizada, lo que dificulta aplicar validaciones y evolucionar reglas de negocio en la caracterizacion del destinatario sin acoplarlas a bloques ad-hoc del componente.

## Goals / Non-Goals

**Goals:**
- Definir la constante `CDeRelacionEstadoRetriccionDto` con estructura tipada y estable.
- Ubicar la constante en un punto reutilizable dentro del modulo de radicacion (o en archivo dedicado si aplica), evitando duplicacion.
- Integrar su lectura en `RadicacionForm.tsx` sin romper el flujo actual de remitente/destinatario.
- Permitir extension futura de reglas de restriccion para destinatario.
- Cubrir con pruebas el uso de la constante en el flujo del formulario.

**Non-Goals:**
- Redisenar todo el flujo de destinatarios o motor de restricciones.
- Cambiar contratos backend existentes en este ticket.
- Introducir dependencias nuevas fuera del stack actual.

## Decisions

### Decision 1: Modelo tipado para la constante
- **Decision:** definir un tipo/DTO explicito para `CDeRelacionEstadoRetriccionDto` y construir la constante con ese contrato.
- **Rationale:** reduce errores por estructura dinamica y facilita mantenimiento.
- **Alternatives considered:** usar objeto libre `Record<string, unknown>` (descartado por baja seguridad de tipos).

### Decision 2: Encapsular la constante en modulo reutilizable
- **Decision:** ubicar la constante en un archivo exportable del dominio radicacion y consumirla desde `RadicacionForm`.
- **Rationale:** desacopla la configuracion de restricciones del render UI.
- **Alternatives considered:** definir la constante inline dentro del componente (descartado por menor reutilizacion y mayor ruido).

### Decision 3: Integracion no disruptiva
- **Decision:** aplicar la constante solo donde impacta destinatario, conservando atributos declarativos existentes y comportamiento actual.
- **Rationale:** minimiza riesgo de regresiones en campos no relacionados.
- **Alternatives considered:** refactor completo del formulario de restricciones (descartado por alcance excesivo).

## Risks / Trade-offs

- **[Risk]** Ambiguedad en la estructura exacta del DTO puede generar interpretaciones distintas.  
  **Mitigation:** definir spec detallada con nombres de propiedades y escenarios de uso.
- **[Risk]** Integrar restricciones podria afectar validaciones actuales de destinatario.  
  **Mitigation:** agregar pruebas de regresion sobre flujo existente.
- **[Risk]** Si la constante se usa en varios componentes, puede haber divergencia de versiones.  
  **Mitigation:** centralizar en un solo modulo exportado.

## Migration Plan

1. Especificar estructura final de `CDeRelacionEstadoRetriccionDto` en la spec.
2. Crear constante y tipos asociados en modulo de radicacion.
3. Integrar la constante en `RadicacionForm` para reglas de destinatario.
4. Agregar/actualizar pruebas unitarias.
5. Validar que no hay regresiones del flujo actual.

Rollback:
- Revertir commit del cambio `SCRUM-18`; no requiere migracion de datos.

## Open Questions

- Confirmar si la constante debe incluir solo estado/restriccion o tambien metadatos de UI (mensajes, prioridad).
- Confirmar si la fuente de verdad sera local (constante fija) o si luego debe hidratarse desde API.
