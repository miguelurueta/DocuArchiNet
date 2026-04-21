## Why

CREA-COMPONENTE-DOMAIN-GUARD. PROMPT ARQUITECTÓNICO — Crear patrón reusable DomainGuard / ScreenGuard

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-142.
- Se formaliza la propuesta OpenSpec para implementar AppDomainGuard a partir del ticket Jira.
- Se define la capability `app-domain-guard` como parte de la capa UI reutilizable.
- Se conserva el contexto funcional del ticket como base para los siguientes artefactos OpenSpec.

## Capabilities

### New Capabilities
- `app-domain-guard`: Componente reusable AppDomainGuard para la capa UI compartida del proyecto.

### Modified Capabilities
- 

## Impact

- Nuevo componente compartido en `src/app/Components/UI/AppDomainGuard/`.
- Posible integracion inicial en un modulo consumidor real del proyecto.
- Nuevas pruebas de comportamiento para el contrato reusable del componente.
