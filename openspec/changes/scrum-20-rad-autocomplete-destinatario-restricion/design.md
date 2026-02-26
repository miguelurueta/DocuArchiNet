## Context

El cambio SCRUM-20 implementa autocompletado para `Destinatario_Cor` en `RadicacionForm.tsx` usando la API `POST /api/PlantillaRadicado/solicitaAutoCompleteDestinatarioRestriccion`.

Estado actual:
- Ya existe infraestructura de campos dinamicos (`camposPlantilla`) y flujos de autocomplete en otros campos.
- Existe una constante `CDeRelacionEstadoRetriccionDto` usada para restriccion de destinatario.
- El control objetivo es un selector con tokens (modo multiple) y debe conservar `required`, `disabled`, `title` y `tooltipAyuda`.

Restricciones:
- Mantener TypeScript estricto y arquitectura por modulos.
- Consumir API con cliente centralizado (axios + hooks existentes).
- No romper flujo actual de Radicacion ni la estructura visual/funcional del control tipo token.

## Goals / Non-Goals

**Goals:**
- Integrar autocomplete remoto para `Destinatario_Cor` con payload `{ idScript, nombreCampo, valueCampo }` y `idScript` desde `camposPlantilla.id_escript`.
- Llenar sugerencias y permitir seleccion de tokens en el control objetivo.
- Limpiar opciones cuando el valor de busqueda sea vacio y mantener valor seleccionado de forma estable.
- Centralizar manejo de errores con las utilidades de consumo API existentes.
- Dejar base reutilizable para otros campos token de plantilla.

**Non-Goals:**
- Cambiar el contrato backend de la API.
- Rediseñar estilos de Ant Design o estructura general del formulario.
- Migrar a otra libreria de estado/consulta.

## Decisions

1. Crear hook dedicado para autocomplete de destinatario restringido.
- Decision: encapsular solicitud, normalizacion de respuesta, loading y errores en un hook (`useAutocompleteDestinatarioRestriccion`).
- Rationale: reduce acoplamiento en `RadicacionForm` y permite reutilizacion en campos similares.
- Alternativa considerada: resolver todo dentro del componente.
  - Descartada por duplicacion de logica y menor testabilidad.

2. Usar contrato de request tipado y builder de payload.
- Decision: construir payload con helper tipado que tome `id_escript` desde `camposPlantilla` y `valueCampo` del input.
- Rationale: evita enviar parametros legacy incorrectos y garantiza compatibilidad con API actual.
- Alternativa considerada: payload inline en cada llamada.
  - Descartada por riesgo de divergencia entre campos.

3. Integrar con `Select` multiple token de AntD preservando props del campo.
- Decision: mantener el control token existente (`mode="multiple"`) y solo conectar `options`, `onSearch`, `onSelect`/`onChange`.
- Rationale: conserva comportamiento visual y accesibilidad ya validada.
- Alternativa considerada: componente custom desde cero.
  - Descartada por mayor costo y riesgo de regresion UI.

4. Manejo de errores centralizado y no intrusivo.
- Decision: mapear errores API a capa comun de cliente/hook, exponer estado controlado y fallback silencioso para no romper escritura manual.
- Rationale: el usuario mantiene continuidad aunque falle backend.
- Alternativa considerada: lanzar error al arbol UI.
  - Descartada por impacto negativo en flujo de radicacion.

## Risks / Trade-offs

- [Riesgo] Latencia en tipeo por llamadas frecuentes → Mitigacion: debounce y minimo de caracteres antes de consultar.
- [Riesgo] Respuesta backend inconsistente (labels/ids nulos) → Mitigacion: normalizacion defensiva y filtrado de items invalidos.
- [Riesgo] Estado desincronizado entre texto buscado y tokens seleccionados → Mitigacion: separar `searchValue`, `options` y `selectedValues`.
- [Riesgo] Reutilizacion incompleta para otros campos → Mitigacion: abstraer builder/request parser en utilidades compartidas.

## Migration Plan

1. Implementar hook y tipos del contrato de autocomplete restringido.
2. Integrar el hook en `RadicacionForm` para `Destinatario_Cor` conservando props actuales.
3. Agregar pruebas unitarias del hook (success/error) y del componente (carga de opciones, seleccion token, limpieza).
4. Ejecutar tests del modulo Radicacion y ajustar regresiones.
5. Archivar cambio OpenSpec cuando pruebas y comportamiento manual queden validados.

Rollback:
- Revertir commit de integracion del campo y restaurar comportamiento previo de entrada sin autocomplete remoto.

## Open Questions

- Confirmar si backend requiere longitud minima especifica para `valueCampo` (2 vs 3 caracteres).
- Confirmar nombre exacto del campo (`Destinatario_Cor` vs variante en metadata) para evitar mismatch en `nombreCampo`.
- Confirmar si el endpoint retorna identificador unico para cada opcion o solo texto (impacta deduplicacion de tokens).
