# SCRUMCORE-297 AppHorizontalScroller

## Objetivo

Documentar de forma enterprise la creación de `AppHorizontalScroller`, un primitive UI reusable para rails horizontales responsive con scroll en X, sin consumo HTTP ni acoplamiento a dominio.

## Artefactos

| Documento | Contenido |
|---|---|
| [01-arquitectura.md](./01-arquitectura.md) | Objetivo, alcance, no objetivos, mapa de archivos, arquitectura y diagramas. |
| [02-api-contrato-visual.md](./02-api-contrato-visual.md) | API de props, defaults, normalización de dimensiones y contrato visual. |
| [03-responsive-accesibilidad-css.md](./03-responsive-accesibilidad-css.md) | Reglas responsive, accesibilidad, CSS Modules, scroll snap y edge fade. |
| [04-uso-e-integracion.md](./04-uso-e-integracion.md) | Ejemplos de uso, imports, composición con dominio y relación futura con SCRUM-162. |
| [05-pruebas-validacion.md](./05-pruebas-validacion.md) | Estrategia de pruebas, comandos, evidencias, build y auditorías. |
| [06-riesgos-checklist.md](./06-riesgos-checklist.md) | Riesgos, mitigaciones, restricciones y checklist de aceptación. |

## Resumen Ejecutivo

- Componente: `AppHorizontalScroller`.
- Ubicación: `src/app/Components/UI/AppHorizontalScroller/`.
- Stack: React 19, TypeScript, CSS Modules.
- Contrato: renderiza `children` en scroll horizontal nativo.
- Accesibilidad: `role="region"` + `aria-label` obligatorio.
- No consume: APIs, `axios`, `fetch`, servicios HTTP, hooks de dominio.
- No modifica: `AppTable`, `AppTreeTable`, `GestionCorrespondencia`.
- Futuro consumidor previsto: SCRUM-162 mediante componente de dominio, no desde el primitive.

## Estado

- Implementación completada.
- Tests focalizados: `14 passed`.
- Lint focalizado: OK.
- `spec:validate`: OK.
- Build general: bloqueado por error preexistente externo a SCRUMCORE-297, documentado en [05-pruebas-validacion.md](./05-pruebas-validacion.md).
