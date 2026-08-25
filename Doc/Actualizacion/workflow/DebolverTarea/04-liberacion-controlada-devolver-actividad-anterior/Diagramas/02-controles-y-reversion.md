# Controles y reversión

~~~mermaid
flowchart LR
  A[Solicitud autorizada] --> B[Controles documentales y SELECT saneados]
  B --> C{Versión, alcance y controles conformes?}
  C -- No --> D[Abortar sin mutar ambiente]
  C -- Sí --> E[Gestión de despliegue aprobada]
  E --> F{Se ordena reversión?}
  F -- Sí --> G[Restaurar paquete acordado]
  F -- No --> H[Registrar resultado saneado]
  G --> H
~~~

La reversión solo afecta intentos nuevos y no altera tareas, auditoría ni transiciones confirmadas.
