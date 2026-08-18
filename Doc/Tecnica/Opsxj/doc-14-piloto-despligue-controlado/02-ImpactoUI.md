# PILOTO-DESPLIGUE-CONTROLADO

- Ticket: DOC-14
- Cambio OpenSpec: doc-14-piloto-despligue-controlado
- Clasificacion: cross_cutting (Transversal)

## Superficies UI

- `workflow/Webworkflow.aspx` consulta `WorkflowModernPresentationBootstrap` para emitir únicamente los assets de la experiencia permitida por el gate servidor.
- El gate activo habilita la interfaz moderna para contextos Workflow válidos no excluidos cuando el modo oficial está explícitamente configurado; un conflicto de alcance, exclusión o metadatos inválidos conserva la interfaz legacy.
- No se modifican los controles, permisos ni reglas de negocio legacy; la página solo decide la presentación permitida.

## Validacion visual

La matriz reproducible está en `Doc/Actualizacion/workflow/Terminar/06-piloto-pruebas-rollout/04-pruebas-y-evidencia.md` e incluye 1366x768, 1024x768, 768x1024 y 375x812. La QA manual autenticada no se ejecutó: requiere ambiente, cuentas y autorización explícita.
