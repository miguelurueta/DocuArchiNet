# Prompts Jira — Modernización del listado de autorizaciones

Esta carpeta organiza la implementación por etapas Jira dependientes. Cada ticket debe enlazar un solo prompt numerado y ejecutar exclusivamente su alcance. `00-contexto-obligatorio.md` y `bloque-e2e-integrado.md` son instrucciones comunes versionadas; no son tickets independientes.

| Etapa | Prompt | Requiere | Resultado |
|---|---|---|---|
| Contexto | `00-contexto-obligatorio.md` | Repositorio y decisiones funcionales | Límites comunes; no implementa. |
| 01 | `01-backend-consulta-y-descarga-segura.md` | Decisiones de Exploración aprobadas | Contrato de lectura/descarga seguro y comprobable. |
| 02 | `02-ui-moderna-listado-autorizaciones.md` | 01 aprobado | Modal moderno oficial integrado a Historial. |
| 03 | `03-migracion-consumidores-y-retiro-legacy.md` | 02 aprobado | Consumidores migrados y ruta legacy retirada sin referencias activas. |
| 04 | `04-verificacion-transversal.md` | 03 aprobado | Evidencia consolidada y decisión técnica. |
| 05 | `05-liberacion-controlada.md` | 04 aprobado | Matriz y runbook; no despliega. |

## Precondiciones funcionales

No iniciar 01 hasta decidir y registrar en Exploración:

1. si la superficie muestra solo autorizaciones vigentes o el historial completo;
2. tratamiento de anulaciones, contador y consolidado;
3. roles con permiso de consulta y descarga;
4. formato orientado al usuario (por ejemplo PDF) y permanencia o no de XML;
5. significado funcional de `ACTIVIDAD` y `ACTIVIDAD_USUARIO`;
6. alcance del consumidor compartido `WebFormConsultaTareasWorkflow`.

El prototipo `../Exploracion/02-modelo-ui-listado-autorizaciones-moderno.html` es una referencia de interacción, no código productivo ni definición final del contrato.

Cuando Jira asigne el DOC, la documentación técnica debe residir en una carpeta propia `DOC-<número>-<RESUMEN>` bajo `Autorizacion/`; ningún prompt debe inventar el identificador.

