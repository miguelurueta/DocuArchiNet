# PROMPT ARQUITECTÓNICO  Ticket 03 BE
# Normalizar adaptadores backend para autocomplete de AppInputTags

Rol esperado:
Arquitecto de software senior backend (C#, APIs, contratos, documentación)


OBJETIVO

Documentar y estabilizar los payloads y respuestas de los endpoints de autocomplete (`solicitaAutoCompleteCampos`, `autoCompleteTercero`, `solicitaAutoCompleteDestinatarioRestriccion`) para que cualquier hook pueda mapearlos a los contratos `AppAutocompleteRequest` y `AppAutocompleteOption` sin alterar el componente.


CONTEXTO EXISTENTE

- El hook `useAutocompleteCamposPlantilla` ya mapea `AutoCompleteCampoItemDTO` a un array genérico.
- Se han definido reglas de front (debounce, minLength, loading) que requieren respuestas consistentes.
- El componente `AppInputTags` debe seguir recibiendo `{ value, label }`.


UBICACIÓN (OBLIGATORIA)

```
DocuArchi.Api/Controllers/PlantillaRadicado/
MiApp.Services/Service/PlantillaRadicado/
```


RESTRICCIONES (OBLIGATORIAS)

- Mantener compatibilidad semántica con los consumers actuales de los endpoints
- No introducir breaking changes para otros módulos que consumen las mismas APIs
- Documentar claramente qué campos se mapean a `value` y `label`


CONTRATO (OBLIGATORIO)

1. `AppAutocompleteRequest` (frontend)
   - `query` -> `TextoBuscado` o `valueCampo`/`ValueAuto`
   - `fieldName` -> `name_campo` o `nombreCampo`
   - `context` -> `idScript`, `CDeRelacionEstadoRetriccionDto`, etc.

2. `AppAutocompleteOption` (respuesta)
   - `label` proviene de `texValue`, `nombre`, `descripcion`, etc.
   - `value` proviene de `idValue`, `id`, `valueCampo`, `idTercero`
   - `meta` incluye el objeto crudo si se requiere más info


REGLAS DE IMPLEMENTACIÓN (OBLIGATORIAS)

- Crear mappers que traduzcan cada payload:
   * `/solicitaAutoCompleteCampos` recibe `TextoBuscado`, `tbl_control`, `name_campo` y opcionales
   * `/autoCompleteTercero` recibe `idScript`, `nombreCampo`, `valueCampo`
   * `/solicitaAutoCompleteDestinatarioRestriccion` recibe `ValueAuto` y `CDeRelacionEstadoRetriccionDto`
- Normalizar la respuesta a `{ label, value }` dejando `meta` para campos opcionales
- Incluir documentación en la API (Swagger/README) explicando cómo se mapean los campos
- Validar que la respuesta siempre contiene listas (`data`, `Data`, etc.) y manejar fallback vacío


PRUEBAS OBLIGATORIAS

- Tests unitarios de los mappers para cada endpoint
- Validar que `label` y `value` no sean nulos
- Simular errores de API y asegurar que el hook los maneja sin romper el componente


CRITERIOS DE ACEPTACIÓN

- Documentación actualizada con los payloads y mapeos
- Hook `useAutocompleteCamposPlantilla` sigue funcionando sin cambios en la UI
- Cualquier otro módulo puede reutilizar los mismos adaptadores
