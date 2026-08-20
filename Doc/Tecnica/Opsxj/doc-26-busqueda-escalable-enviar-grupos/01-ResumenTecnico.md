# BUSQUEDA-ESCALABLE-ENVIAR-GRUPOS

- Ticket: DOC-26
- Cambio OpenSpec: doc-26-busqueda-escalable-enviar-grupos
- Clasificacion: cross_cutting

## Objetivo

Limitar la carga de destinos de Enviar a grupo y permitir buscar una actividad por su nombre o por un grupo asociado. La selección se mantiene por IdActividadDestino; un grupo no es un destino independiente.

## Alcance y compatibilidad

Se incorporó una consulta paginada de solo lectura y el preview conserva su firma, pero devuelve únicamente la primera página. EjecutarEnvioGrupo conserva la terna de ejecución, su relectura y el lock. Continuar flujo no cambia y sigue usando IdConector. No se crearon gates, configuraciones, índices ni migraciones. Con el gate inactivo permanece el postback Web Forms.
