# Alcance sin gate y relevo a etapa 02

```text
DOC-28
  -> contratos ASMX exclusivos de usuario
  -> preview de solo lectura + ejecución segura
  -> sin UI, sin Web Forms, sin gate y sin configuración

Etapa 02
  -> selección y confirmación accesibles
  -> consume PreviewEnviarUsuario / EjecutarEnvioUsuario
  -> conserva payload directo y bloqueos del servidor

Fuera de ambas etapas DOC-28
  -> Continuar flujo por IdConector
  -> Enviar a grupo
  -> Pendientes / batch
  -> reasignación de respuesta
```

No existe fallback de gate en DOC-28: `WorkflowCentroTrabajoModernActive` no se consulta ni se modifica. La reversa técnica del corte backend es retirar sus componentes; ninguna transición ya confirmada se revierte.
