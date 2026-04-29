## Context

El ticket `SCRUMCORE-190` solicita crear el componente reusable **AppVisorPdf** (01-FE) como
parte de la capa UI compartida del proyecto.

Hoy el repo contiene documentaci\u00f3n de arquitectura para un visor PDF en `docs/Architecture/AppVisorPdf/`,
pero no existe una capability OpenSpec formalizada para el componente UI compartido.

## Goals / Non-Goals

**Goals:**
- Definir una estrategia de implementaci\u00f3n incremental para introducir `AppVisorPdf` en `src/app/Components/UI/AppVisorPdf/`.
- Mantener el componente desacoplado del dominio (integrable por props/callbacks).
- Preparar un contrato verificable con tests (unit/behavior) y/o e2e seg\u00fan aplique.

**Non-Goals:**
- Implementar anotaciones, firmas, o un sistema completo de edici\u00f3n PDF en esta fase.
- Acoplar el visor a `gestionCorrespondencia` u otros m\u00f3dulos.
- Definir o imponer un backend (el visor puede funcionar con URL/Blob/bytes).

## Decisions

1) **Ubicaci\u00f3n y ownership del componente**
- **Decision:** `AppVisorPdf` vivir\u00e1 en `src/app/Components/UI/AppVisorPdf/` como componente UI compartido.
- **Why:** asegura reutilizaci\u00f3n y evita duplicaci\u00f3n por m\u00f3dulos.
- **Alternatives:** implementarlo dentro de un m\u00f3dulo consumidor (`src/modules/...`) y extraerlo luego.

2) **Contrato de integraci\u00f3n**
- **Decision:** el componente expondr\u00e1 un contrato tipado (props) para:
  - fuente del documento (URL/Blob/ArrayBuffer),
  - callbacks de error/estado (loading/loaded/error),
  - controles configurables (p\u00e1gina inicial, zoom inicial, toolbar opcional).
- **Why:** permite integraci\u00f3n en m\u00f3dulos sin contaminar el core con reglas de negocio.
- **Alternatives:** API impl\u00edcita acoplada a servicios del dominio.

3) **Motor PDF (fase 01)**
- **Decision:** iniciar con una abstracci\u00f3n de motor (adapter) y seleccionar motor concreto en implementaci\u00f3n,
  priorizando dependencias ya presentes o de bajo impacto.
- **Why:** el ticket 01-FE prioriza crear el componente y su contrato; la selecci\u00f3n de motor puede requerir validaci\u00f3n
  de peso, licencias y compatibilidad.
- **Alternatives:** fijar de una vez `pdf.js`/`react-pdf` sin validaci\u00f3n de impacto.

## Risks / Trade-offs

- **[Bundle size + performance]** \u2192 validar motor PDF antes de consolidar; medir con `npm run build` y revisar chunks.
- **[Accesibilidad/UX]** \u2192 definir desde el inicio estados (loading/error) y controles de teclado b\u00e1sicos.
- **[Integraci\u00f3n inconsistente]** \u2192 forzar contrato tipado y tests de contrato (props/callbacks).

## Migration Plan

1) Crear skeleton `AppVisorPdf` (render container + estados).
2) Definir props y tipos (fuente, callbacks, controles).
3) Implementar motor PDF m\u00ednimo (render 1ra p\u00e1gina / navegaci\u00f3n b\u00e1sica) o dejar adapter mockeable si el motor se decide en siguiente ticket.
4) Agregar tests que cubran los escenarios del spec.
5) (Opcional) Integrar en un consumidor real bajo feature-flag o en una vista de demo.

## Open Questions

- \u00bfCu\u00e1l es el motor PDF objetivo (pdf.js directo, wrapper tipo `react-pdf`, o alternativa) y su impacto en bundle?
- \u00bfEl visor requiere soporte de anotaciones o solo visualizaci\u00f3n en esta etapa 01-FE?
- \u00bfLa fuente de documentos ser\u00e1 URL p\u00fablica, URL autenticada, o bytes provistos por API?

