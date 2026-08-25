# Decisión y autorización

~~~mermaid
flowchart TD
  A[DOC-34: evidencia técnica aprobada] --> B[PR #29 fusionado en main]
  B --> C{Ambiente, versión, ventana y roles autorizados?}
  C -- No --> D[Solicitar aprobación operativa]
  C -- Sí --> E[Aplicar matriz por ambiente]
  E --> F[Ejecutar runbook autorizado]
~~~

La decisión actual sigue la rama No: DOC-35 no recibió autorización operativa.
