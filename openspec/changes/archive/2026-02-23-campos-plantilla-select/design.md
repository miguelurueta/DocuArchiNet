## Context

La UI de radicacion ya renderiza campos dinamicos basados en `camposPlantilla`, incluyendo autocompletado, con una estructura y estilos consolidados. Falta cubrir el comportamiento `SELECCION` con la misma estructura visual, accesibilidad y reglas de atributos declarativas, sin desarmar lo ya construido. Se requiere mantener TypeScript estricto y el stack de UI actual (Ant Design/MUI).

## Goals / Non-Goals

**Goals:**
- Incorporar renderizado de campos `SELECCION` (cuando `campo_tip = 1`) reutilizando la estructura y estilos de los campos `AUTOCOMPLETE`.
- Aplicar reglas declarativas de atributos (required, disabled, maxLength, type/pattern, data-api-method, data-group) y accesibilidad.
- Mantener compatibilidad con internacionalizacion para labels y tooltips.
- Agregar o ajustar pruebas de UI con trazabilidad a specs.

**Non-Goals:**
- Cambiar arquitectura de modulos, rutas o layout.
- Introducir nuevas dependencias de UI.
- Reescribir componentes existentes de autocompletado.

## Decisions

- **Reutilizar estructura y estilos existentes**: se extendera el renderizado en el componente de plantilla (o un renderer especifico) para `SELECCION`, replicando la misma jerarquia y clases CSS usadas por `AUTOCOMPLETE`. Esto minimiza regresiones visuales.
- **Mantener atributos declarativos desde `camposPlantilla`**: todos los atributos y flags (obligatorio, disable, maxLength, data-api-method, data-group, title, tooltip) se aplicaran igual que en autocompletado para consistencia.
- **Validaciones mediante `type`/`pattern`**: cuando aplique, se usaran atributos HTML sin introducir nuevas librerias.

## Risks / Trade-offs

- [Riesgo] Duplicar estructura de UI o crear inconsistencias visuales. -> Mitigacion: reutilizar clases y estructura de los campos `AUTOCOMPLETE` y cubrir con tests.
- [Riesgo] Campos `SELECCION` sin opciones inmediatas (dependientes de API). -> Mitigacion: respetar `data-api-method` para integracion posterior sin bloquear UI.
- [Riesgo] Cambios colaterales en componentes existentes. -> Mitigacion: pruebas de regresion y cambios acotados.
