## Why

La devolución a usuario anterior ya tiene backend e interfaz moderna, pero requiere una verificación transversal antes de la liberación controlada. DOC-38 consolida evidencia de seguridad, aislamiento, compatibilidad y no regresión sin introducir otra ruta funcional.

## What Changes

- Define una matriz reproducible de verificación local y QA manual no autenticada para Devolver → Usuario anterior.
- Comprueba contratos de preview, historial, token, lock, auditoría y aislamiento frente a componentes de respuestas.
- Comprueba que la UI moderna es exclusiva y que conserva los flujos vecinos de Workflow.
- Actualiza el índice y la evidencia del paquete técnico con resultados, limitaciones, riesgos y recomendación para la etapa 05.
- No modifica código de producción, configuración, datos, endpoints ni contratos para obtener evidencia.

## Capabilities

### New Capabilities

- `verificacion-transversal`: Compone evidencia verificable y una decisión de aptitud para liberar la devolución a usuario anterior.

### Modified Capabilities

- Ninguna. DOC-38 verifica comportamiento existente; no cambia requisitos de otras capacidades.

## Impact

- Pruebas CJS/VB y análisis estático de `Services/Workflow/DevolverUsuarioAnterior`, `workflow/` y `js/workflow/`.
- Documentación bajo `Doc/Actualizacion/workflow/DevolverUsuarioAnterior/01-implementacion-devolver-usuario-anterior/`.
- Sin impacto en APIs públicas, configuración de ambiente ni tablas de negocio.
