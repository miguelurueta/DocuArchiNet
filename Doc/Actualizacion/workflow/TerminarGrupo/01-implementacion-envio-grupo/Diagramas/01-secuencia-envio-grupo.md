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

## Búsqueda paginada DOC-26

```text
UI moderna
  -> PreviewEnviarGrupo(idTarea) [SELECT, primera página]
  -> escribir término o cambiar página
  -> BuscarDestinosEnvioGrupo(idTarea, termino, pagina, tamanoPagina) [SELECT]
  -> validar sesión, gate, permiso, tarea y ruta
  -> actividad única + resumen de grupo + TieneMas
  -> seleccionar IdActividadDestino + TokenVersion
  -> EjecutarEnvioGrupo conserva GET_LOCK y revalidación
```
