## Context

El formulario de radicación renderiza campos dinámicos desde `camposPlantilla`, incluyendo `ANEXOS_COR`. Actualmente el campo se presenta como input simple y no aprovecha el endpoint de autocompletado. El cambio debe encajar en el flujo existente de Axios (manejo centralizado de errores) y en la arquitectura de componentes reutilizables, manteniendo atributos de accesibilidad y metadata (required, disabled, title, tooltip).

## Goals / Non-Goals

**Goals:**
- Renderizar `ANEXOS_COR` como autocompletado con consultas a `/api/PlantillaRadicado/solicitaAutoCompleteCampos`.
- Reutilizar un componente de autocompletado para que otros campos similares se integren con mínima duplicación.
- Mantener los atributos y metadatos actuales del campo (required, disabled, title, tooltipAyuda).
- Centralizar errores en Axios usando las mismas funciones de consumo (query o mutation) existentes.

**Non-Goals:**
- No se rediseña el formulario completo ni el sistema de render dinámico de campos.
- No se agregan nuevas dependencias UI ni se cambia el stack de MUI/Ant Design.
- No se implementa una política global de caché o paginación para autocompletados fuera de este caso.

## Decisions

- **Componente reutilizable de autocompletado**: crear un componente dedicado que reciba `campo` (de `camposPlantilla`) y que construya `data-ident`, `tbl_control` y props de accesibilidad. Se prefiere encapsular la lógica de consulta y de render para facilitar la extensión a otros campos.
  - Alternativas: lógica inline en `RadicacionForm`. Se descarta por duplicación y baja reutilización.

- **Consumo de API vía capa existente**: utilizar el servicio de Axios ya centralizado (query/mutation) para mantener manejo de errores y logging coherente.
  - Alternativas: llamada directa con `axios` en el componente. Se descarta por romper centralización de errores.

- **Obtención de `tbl_control` desde `tbl_control`**: el valor se lee del objeto de campo (`camposPlantilla`) y se pasa al servicio, garantizando que el request sea dinámico.
  - Alternativas: hardcode en el componente. Se descarta por falta de flexibilidad.

## Risks / Trade-offs

- [Riesgo] Respuesta de API vacía o inconsistente → Mitigación: mostrar estado vacío sin romper la UI, manejar errores en la capa de Axios.
- [Riesgo] Llamadas excesivas por cada tecla → Mitigación: usar debounce en el autocompletado si existe util compartido; si no, limitar con `minLength` antes de consultar.
- [Trade-off] Encapsular lógica en componente reutilizable implica agregar props y tipado adicional, pero reduce duplicación futura.
