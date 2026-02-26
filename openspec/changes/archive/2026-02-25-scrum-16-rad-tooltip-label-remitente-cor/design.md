## Context

En `RadicacionForm.tsx` existe el control de remitente con `data-ident="pl-radicacion-spe-REMITENTE_COR"`, pero su configuracion no siempre se deriva de la metadata declarativa en `camposPlantilla`. El cambio `SCRUM-16` requiere resolver el registro por `name_campo = "REMITENTE_COR"` y usar esa metadata para mantener atributos funcionales y de UX (`required`, `disabled`, `title`, `tooltipAyuda`) de forma consistente.

## Goals / Non-Goals

**Goals:**
- Resolver `REMITENTE_COR` desde `camposPlantilla` con comparacion normalizada (`trim` + `uppercase`).
- Conectar el control de remitente para que use la metadata del registro resuelto.
- Aplicar `obligatorio_campo` y `disable_campo` al selector de remitente.
- Mapear `title_control` a `title` del label y renderizar `tooltipAyuda` con `span.tooltip-ayuda` + icono de informacion.
- Preservar el comportamiento de selector tipo token ya implementado para remitente.

**Non-Goals:**
- Cambiar contratos backend de autocompletado.
- Redisenar componentes de destinatario u otros campos de radicacion no relacionados con `REMITENTE_COR`.
- Introducir nuevas dependencias UI.

## Decisions

### Decision 1: Resolver campo por `name_campo` normalizado
- **Decision:** ubicar el registro de remitente por `normalize(name_campo) === "REMITENTE_COR"`.
- **Rationale:** evita fragilidad por variaciones de mayusculas, espacios o origen de datos.
- **Alternatives considered:** hardcode por posicion de arreglo (descartado por fragil).

### Decision 2: Reutilizar el componente token existente
- **Decision:** mantener el componente de remitente tipo token y solo inyectar metadata declarativa.
- **Rationale:** minimiza riesgo de regresiones funcionales y mantiene experiencia actual.
- **Alternatives considered:** crear un componente nuevo solo para metadata (descartado por costo e impacto).

### Decision 3: UX del label basada en metadata
- **Decision:** usar `title_control` como `title` del label y `tooltipAyuda` en `span.tooltip-ayuda` con icono.
- **Rationale:** homogeneidad con otros campos dinamicos y cumplimiento explicito del ticket.
- **Alternatives considered:** usar solo tooltip nativo del navegador (descartado por no cumplir el patron visual requerido).

## Risks / Trade-offs

- **[Risk]** Si `REMITENTE_COR` llega duplicado en `camposPlantilla`, puede tomarse un registro no esperado.  
  **Mitigation:** usar primera coincidencia y cubrir con pruebas de resolucion.
- **[Risk]** Regresion en validaciones del campo remitente (required/disabled).  
  **Mitigation:** agregar pruebas de formulario con metadata controlada.
- **[Risk]** Diferencias de estilo del tooltip frente a otros labels.  
  **Mitigation:** reutilizar clase existente `tooltip-ayuda` y el mismo patron de icono.

## Migration Plan

1. Localizar metadata `REMITENTE_COR` en `RadicacionForm`.
2. Inyectar atributos declarativos al selector de remitente.
3. Ajustar render del label con `title` y `tooltipAyuda`.
4. Agregar pruebas en `RadicacionForm.spec.test.tsx`.
5. Ejecutar pruebas del modulo radicacion.

Rollback:
- Revertir commit del cambio en `feature/SCRUM-16` para regresar al comportamiento anterior.

## Open Questions

- Confirmar si el modo token de remitente debe permitir siempre maximo 1 valor o si en futuras plantillas puede habilitar seleccion multiple real.
