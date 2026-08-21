# Secuencia de Enviar a usuario

```text
Cliente de etapa 02 (no incluido en DOC-28)
  -> PreviewEnviarUsuario(idTarea, consulta, cursor, tamanoPagina) [SELECT]
  -> recibe destinos usuario–actividad + TokenVersion
  -> selecciona destino y confirma intención
  -> EjecutarEnvioUsuario(idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion)
  -> GET_LOCK + revalidación completa
  -> adaptador directo exclusivo
  -> Terminar_Tarea_Workflow
  -> auditoría sanitizada + resultado público
```

La selección del cliente no autoriza el envío. El servidor vuelve a resolver el destino y puede bloquear una intención antes del motor legacy.
