## Context

El formulario de radicación ya resuelve `title_control` y `tooltipAyuda` para campos como `Descripcion_Documento` y `RE_flujo_trabajo`, pero `Fecha Límite Respuesta` aún usa un label estático sin integración con metadatos de plantilla.  
El cambio es local al módulo de radicación y no requiere cambios de API ni nuevos componentes base.

## Goals / Non-Goals

**Goals:**
- Incorporar metadatos de plantilla al campo `FECHALIMITERESPUESTA` en `RadicacionForm`.
- Mantener consistencia visual y de accesibilidad con el patrón existente de tooltips en labels.
- Cubrir el comportamiento con pruebas de UI observables.

**Non-Goals:**
- No rediseñar el layout del formulario.
- No cambiar el tipo de control (`DatePicker`) ni el flujo de datos del formulario.
- No modificar contratos backend ni estructura de `CampoPlantillaDTO`.

## Decisions

1. Resolver el campo por `name_campo = "FECHALIMITERESPUESTA"` desde `camposPlantilla`.
- Rationale: mantiene el patrón usado en otros campos, evita hardcodear textos.
- Alternativa descartada: usar solo label literal fijo; se descarta por inconsistencia funcional.

2. Construir un `labelNode` para el campo de fecha con `title_control` y `tooltipAyuda`.
- Rationale: reutiliza el mismo patrón de accesibilidad (`aria-describedby`, icono `tooltip-ayuda`) ya aplicado en el formulario.
- Alternativa descartada: crear componente genérico nuevo para labels con tooltip; se descarta por alcance reducido del cambio.

3. Conservar el `DatePicker` actual y extender únicamente metadatos visuales/accesibilidad.
- Rationale: minimiza riesgo de regresión en validación y captura de fecha.
- Alternativa descartada: migrar a renderer dinámico completo del campo; se descarta por sobrealcance.

4. Agregar/actualizar pruebas en `RadicacionForm.spec.test.tsx` enfocadas en comportamiento.
- Rationale: validar presencia de tooltip y atributos declarativos sin acoplarse a implementación interna.
- Alternativa descartada: pruebas snapshot; se descarta por fragilidad.

## Risks / Trade-offs

- [Riesgo] El backend podría enviar `name_campo` con variaciones de formato.
  -> Mitigación: usar comparación exacta con el valor esperado del dominio y fallback al label actual.

- [Riesgo] Dependencia de render del tooltip de AntD en entorno de test.
  -> Mitigación: verificar markup y atributos accesibles del trigger, no overlays complejos.

- [Trade-off] Se mantiene lógica de labels repetida en el componente.
  -> Mitigación: aceptar duplicación mínima en este cambio; evaluar refactor posterior si aparecen más casos.

## Migration Plan

1. Actualizar spec delta de `campos-dinamicos-plantilla` para incluir `FECHALIMITERESPUESTA`.
2. Implementar ajuste de label/tooltip en `RadicacionForm`.
3. Ejecutar tests de radicación y registrar evidencia en artifacts de cambio.
4. Si hay regresión, rollback revertiendo los cambios en spec delta y `RadicacionForm`.

## Open Questions

- ¿El campo puede venir como `FechaLimiteRespuesta` en alguna plantilla legacy? Si aplica, habría que documentar alias permitidos en spec.
