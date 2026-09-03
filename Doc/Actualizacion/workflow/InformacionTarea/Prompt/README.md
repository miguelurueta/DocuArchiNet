# Prompts Jira — Modernización de Información de la tarea

Jira controla orden, aprobaciones y cierre. Cada ticket enlaza un único prompt numerado. `00-contexto-obligatorio.md` y `bloque-e2e-integrado.md` son instrucciones comunes versionadas y no representan tickets de implementación.

| Etapa | Prompt | Requiere | Resultado |
|---|---|---|---|
| Contexto | `00-contexto-obligatorio.md` | Repositorio | Límites comunes; no implementa. |
| 01 | `01-backend-informacion-tarea-segura.md` | Decisiones funcionales aprobadas | Contrato de lectura, catálogo publicable y autorización. |
| 02 | `02-ui-moderna-informacion-tarea.md` | 01 aprobado | Modal moderno oficial integrado al menú Detalle. |
| 03 | `03-retiro-recorrido-legacy.md` | 02 aprobado | Ruta `S-DTS` y componentes exclusivos retirados con referencias cero. |
| 04 | `04-verificacion-transversal.md` | 03 aprobado | Evidencia consolidada y decisión técnica. |
| 05 | `05-liberacion-controlada.md` | 04 aprobado | Matriz y runbook; no despliega. |

## Decisiones obligatorias antes de 01

1. perfiles autorizados para consultar información;
2. acceso sobre tareas activas, asignadas, terminadas o históricas;
3. campos funcionales obligatorios del resumen;
4. fuente autorizada de etiquetas, tipos, orden y visibilidad de campos variables;
5. tratamiento de identificación y demás datos personales;
6. visibilidad excepcional de identificadores internos para soporte;
7. semántica de estado de tarea, trámite y módulo de radicación;
8. formato de importes y campos extensos;
9. patrón oficial: modal centrado o panel lateral.

Fuentes de decisión:

- `../Exploracion/01-exploracion-modernizacion-informacion-tarea.md`.
- `../Exploracion/02-modelo-ui-informacion-tarea-moderno.html`.

El prototipo es una referencia de interacción, no código productivo ni autorización. Cuando Jira asigne el DOC, crear la documentación técnica en una carpeta `DOC-<número>-<RESUMEN>` bajo `InformacionTarea/`; no inventar el identificador.

