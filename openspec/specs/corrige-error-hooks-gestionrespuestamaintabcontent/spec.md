# corrige-error-hooks-gestionrespuestamaintabcontent Specification

## Purpose
TBD - created by archiving change scrumcore-139-corrige-error-hooks-gestionrespuestamaintabcontent. Update Purpose after archive.
## Requirements
### Requirement: GestionRespuestaMainTabContent MUST not redeclare React hooks
El archivo `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx` MUST NOT contener imports duplicados desde `"react"` que provoquen redeclaración de identificadores (por ejemplo `useEffect`, `useId`, `useState`).

#### Scenario: No redeclaration error after fixing imports
- **WHEN** el proyecto ejecuta el `build`/`lint` del pipeline sobre el archivo
- **THEN** no aparece el error `Identifier 'useEffect' has already been declared`

### Requirement: The GestionRespuestaMainTabContent screen MUST pass build/lint after the change
Después de aplicar la corrección de imports, la pantalla asociada a `GestionRespuestaMainTabContent` MUST quedar sin errores de `build`/`lint` en el pipeline del proyecto (incluyendo, si aplica, errores por imports duplicados, imports no usados, variables no usadas y reglas de orden/agrupación de imports).

#### Scenario: Pipeline build/lint succeeds for the screen
- **WHEN** el pipeline ejecuta sus validaciones (`lint`/`build` y/o checks equivalentes)
- **THEN** la pantalla/archivo no genera errores de compilación ni de lint relacionados a la corrección

