# Secuencia de grupo

```text
UI (gate activo)
  -> PreviewEnviarGrupo(idTarea) [SELECT]
  -> seleccionar IdActividadDestino + TokenVersion
  -> EjecutarEnvioGrupo
  -> gate + GET_LOCK + relectura + requisitos
  -> adaptador directo
  -> Terminar_Tarea_Workflow
  -> auditoría sanitizada + actualización visual
```
