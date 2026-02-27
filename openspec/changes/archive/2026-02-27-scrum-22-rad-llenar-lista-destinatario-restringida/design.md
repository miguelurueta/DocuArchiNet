## Context

El formulario de Radicación combina campos estáticos y dinámicos. Para `Destinatario_Cor` existe autocompletado condicionado por reglas de restricción (`CDeRelacionEstadoRetriccionDto`) que dependen del trámite (`pl-radicacion-spe-Descripcion_Documento`).

Actualmente el problema reportado es que la lista de destinatarios restringidos no se llena de forma autmatica cuando hay una restrición cuando cambia el trámite.

## Goals / Non-Goals

**Goals:**
- Garantizar que `Destinatario_Cor` consulte el endpoint de autocompletado restringido con payload válido y actualizado.
- Asegurar que el cambio de `Descripcion_Documento` refresque el estado de restricción usado por `Destinatario_Cor`.
- Mantener la UX del control estándar de autocompletado sin auto-selección forzada.
- Evitar disparos de API en primer render sin interacción del usuario.

**Non-Goals:**
- Rediseñar la UI del formulario o reemplazar Ant Design.
- Cambiar contratos backend fuera del payload requerido por la API.
- Introducir nuevos módulos de estado global o nuevas dependencias.

## Decisions

1. Estado de restricción centralizado en `RadicacionForm`
- Se mantiene `CDeRelacionEstadoRetriccionDto` como fuente de verdad en el formulario padre y se pasa por props al control `Destinatario_Cor`.
- Rationale: evita desalineación entre trámite seleccionado y payload de autocompletado.
- Alternative considered: que cada campo calcule su propia restricción. Se descarta por duplicación y riesgo de drift.

2. Gating explícito por interacción de usuario
- Las consultas de restricción/autocompletado se habilitan solo cuando hay interacción válida (cambio de trámite o texto con longitud > 0).
- Rationale: reduce ruido de red y evita payload incompleto en render inicial.
- Alternative considered: mantener enabled siempre en hooks y filtrar en backend. Se descarta por costo y comportamiento no determinista en frontend.

3. Payload estricto para endpoint restringido
- El request de autocompletado de `Destinatario_Cor` debe enviar:
  - `ValueAuto` con texto digitado por usuario.
  - `CDeRelacionEstadoRetriccionDto` actualizado al trámite actual.
- Rationale: alinea frontend con contrato backend y resuelve respuestas vacías por parámetros incompletos.
- Alternative considered: enviar estructura legacy (`TextoBuscado`, `idScript:0`). Se descarta por incompatibilidad con el contrato actual.

4. Sin auto-selección del primer resultado
- Se conserva comportamiento estándar de select/autocomplete: mostrar opciones y dejar selección al usuario.
- Rationale: evita cambios de valor implícitos que desajusten validaciones y expectativas de negocio.
- Alternative considered: auto-seleccionar primer ítem. Se descarta por reportes previos de comportamiento inesperado.

## Risks / Trade-offs

- [Riesgo] Diferencias entre entornos sobre campos obligatorios del payload de restricción.  
  → Mitigación: normalizar valores nulos a `0`/`""` y agregar pruebas de contrato en hooks.

- [Riesgo] Condiciones de carrera entre cambio de trámite y digitación rápida en destinatario.  
  → Mitigación: usar estado derivado actualizado y debounce existente antes de consultar.

- [Riesgo] Regresión en campos dinámicos no relacionados por cambios compartidos de utilidades de autocompletado.  
  → Mitigación: pruebas de no-regresión en `RadicacionForm.spec.test.tsx` y hook de autocompletado.
