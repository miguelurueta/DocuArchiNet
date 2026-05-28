# AppProgressBatch - Preview check

Este archivo contiene un diagrama Mermaid minimo para validar si el preview de VS Code esta funcionando.

```mermaid
flowchart LR
  A[Inicio] --> B[Procesar item]
  B --> C[Actualizar progreso]
  C --> D[Finalizar]
```

Si este diagrama tambien aparece y desaparece, el problema esta en la extension o en el preview de VS Code, no en los diagramas de arquitectura.

