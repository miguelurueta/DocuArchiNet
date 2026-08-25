# LIBERACION-DEVOLVER-TAREA-ACTIVIDAD

- Ticket: DOC-35
- Cambio OpenSpec: doc-35-liberacion-devolver-tarea-actividad
- Clasificacion: cross_cutting

## Servicios y reglas

No se cambia ningún servicio. La liberación futura conserva ServicioDevolverActividad, el adaptador de devolución y el repositorio de Ruta/Flujo ya verificados en DOC-34.

Las reglas que se deben preservar son: preview de solo lectura con SELECT parametrizados, conector entrante reconstruido en servidor, token y permiso revalidados dentro del lock por tarea, auditoría saneada y ausencia de tratamiento de respuestas.

DOC-35 no habilita E2E, carga, cambios de gate, ajustes de configuración ni mutaciones de Workflow. Una consulta de control solo puede hacerse con autorización explícita para el ambiente y debe ser SELECT saneado.
