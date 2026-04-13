## Context

El ticket `SCRUMCORE-89` solicita la implementacion de una estructura para
`GestionCorrespondencia`, apoyandose en el prompt arquitectonico existente.
Se requiere mantener el layout principal y modularizar componentes sin romper
la navegacion actual ni el contrato de tabs.

## Goals / Non-Goals

**Goals:**
- Definir una estructura de componentes consistente para `GestionCorrespondencia`.
- Mantener `GestionRespuesta.tsx` como orquestador de tabs.
- Reutilizar componentes existentes sin duplicar shared.

**Non-Goals:**
- Cambios de backend o integraciones API.
- Reemplazar el sistema de tabs existente.
- Reestructurar rutas o layout global del dashboard.

## Decisions

- Desacoplar el contenido principal en componentes de modulo dedicados.
  - Alternativa: mantener todo en la pagina. Rechazado por mantenibilidad.
- Usar CSS Modules locales para ajustes visuales.
  - Alternativa: estilos globales. Rechazado por riesgo de regresion.

## Risks / Trade-offs

- [Riesgo] Cambios estructurales afecten layout master-detail → Mitigacion: conservar estructura del shell y validar rutas.
- [Riesgo] Sobre-ingenieria de componentes → Mitigacion: limitar extracciones a unidades necesarias.
