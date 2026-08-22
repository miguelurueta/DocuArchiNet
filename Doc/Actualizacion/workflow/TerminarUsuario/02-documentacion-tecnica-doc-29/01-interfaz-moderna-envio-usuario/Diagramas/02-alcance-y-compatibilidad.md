# Alcance y compatibilidad

```mermaid
flowchart LR
    Usuario[Enviar a usuario DOC-29]
    Usuario --> Trigger[workflow-user-send-trigger]
    Trigger --> Modal[Modal propio y accesible]
    Modal --> Preview[PreviewEnviarUsuario]
    Modal --> Execute[EjecutarEnvioUsuario]

    Grupo[Enviar a grupo] -. sin cambio .-> Gate[Gate existente]
    Flujo[Continuar flujo] -. sin cambio .-> Conector[IdConector]
    Legacy[ImageButtonEnviarUsuario] -. retirado de esta página .-> Ninguno[Sin fallback Web Forms]
```

El comando de usuario se registra para todo contexto Workflow válido y no lee ni modifica el gate de las otras operaciones. Grupo y Continuar flujo conservan su contrato y su ciclo de vida.
