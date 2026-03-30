## Why

CREA-COMPONENTE-APPDROPDOWN. PROMPT PROFESIONAL  Crear componente Dropdown avanzado AppDropdown altamente reutilizable (React 19 + TS estricto + Ant Design)

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-15.
- Se formaliza la propuesta OpenSpec para implementar `AppDropdown` a partir del ticket Jira.
- Se define la capability `app-dropdown` como parte de la capa UI reutilizable.
- Se conserva el contexto funcional del ticket como base para los siguientes artefactos OpenSpec.

## Capabilities

### New Capabilities
- `app-dropdown`: Componente reusable `AppDropdown` para la capa UI compartida del proyecto.

### Modified Capabilities
- 

## Impact

- Nuevo componente compartido en `src/app/Components/UI/AppDropdown/`.
- Posible integracion inicial en un modulo consumidor real del proyecto.
- Nuevas pruebas de comportamiento para el contrato reusable del componente.
