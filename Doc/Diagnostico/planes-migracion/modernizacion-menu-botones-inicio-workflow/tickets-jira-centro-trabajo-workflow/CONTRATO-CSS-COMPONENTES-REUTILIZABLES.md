# Contrato CSS reutilizable — Centro de Trabajo Workflow

Este contrato convierte el lenguaje visual de `../centro-trabajo-workflow-sin-bandeja.html` en componentes reutilizables para la aplicación WebForms. Es obligatorio para JIRA-02, JIRA-04, JIRA-05 y JIRA-06.

## Regla de encapsulamiento

Todas las reglas deben iniciar en `.workflow-centro-trabajo-moderno`; no se permiten selectores globales como `.btn`, `.dropdown-menu`, `table`, `button` o `.nav-link` sin alcance. Los estilos legacy continúan siendo la base funcional.

## Tokens obligatorios

```css
.workflow-centro-trabajo-moderno {
  --ctw-navy: #35477f;
  --ctw-blue: #0062cc;
  --ctw-ink: #202d47;
  --ctw-muted: #69758b;
  --ctw-line: #dce2f1;
  --ctw-pale: #eef1fb;
  --ctw-bg: #f5f7fb;
  --ctw-danger: #ad3741;
  --ctw-warning: #a56809;
  --ctw-radius: 5px;
  --ctw-control-height: 34px;
}
```

## Componentes que deben construirse

| Componente | Clase propuesta | Contrato visual tomado del HTML base |
|---|---|---|
| Botón secundario | `.ctw-btn` | Alto 34 px, borde `#bfcae0`, fondo blanco, texto `--ctw-navy`, radio 5 px, hover `--ctw-pale`. |
| Acción primaria | `.ctw-btn--primary` | Fondo y borde `--ctw-blue`, texto blanco. Solo para transición principal `Enviar`. |
| Acción peligrosa | `.ctw-btn--danger` | Borde de peligro, texto `--ctw-danger`, sin fondo rojo permanente. |
| Botón de icono | `.ctw-icon-btn` | Mínimo 34×34 px; en móvil 40×40 px; `aria-label` obligatorio. |
| Barra de acciones | `.ctw-action-bar` | Flex, separación 7 px, wrap, línea inferior `--ctw-line`; separador vertical reutilizable `.ctw-separator`. |
| Menú de tarea/documento | `.ctw-menu`, `.ctw-menu__trigger`, `.ctw-menu__panel`, `.ctw-menu__item` | Panel blanco, borde `--ctw-line`, sombra `0 11px 24px #23345a30`, mínimo 33 px por ítem, hover `--ctw-pale`. |
| Barra de documento | `.ctw-document-bar` | Fondo blanco, borde `--ctw-line`, controles de documento alineados al extremo derecho. |
| Etiqueta/estado | `.ctw-badge`, `.ctw-badge--warning`, `.ctw-badge--success` | Radio 99 px, texto 11 px semibold; color siempre acompañado por texto. |
| Panel/título | `.ctw-panel`, `.ctw-pane-head` | Fondo blanco, línea `--ctw-line`, título `--ctw-navy`. |
| Fila documento activa | `.ctw-document-row--selected` | Fondo `#edf3ff` y `box-shadow: inset 3px 0 0 var(--ctw-blue)`; sin bordes redondeados. |

## Reglas de reutilización

1. Los componentes reciben clases sobre controles legacy existentes; no se generan botones paralelos ni se clona markup.
2. La composición de menús usa los dropdowns existentes. El adaptador solo asigna clases, atributos ARIA y estado visual.
3. El botón `Enviar` es el único candidato a variante primaria; `Cerrar`, `Eliminar` y equivalentes usan variante peligrosa y conservan confirmación legacy.
4. Los mismos componentes se emplean en menú de tarea, barra de visor, índice y documentos relacionados; las diferencias se resuelven por modificadores, no por CSS duplicado.
5. Los componentes deben soportar contenedores re-renderizados por UpdatePanel y no dependen de IDs numéricos.

## Verificación visual obligatoria

- Comparación lado a lado con el HTML base para botones, menú abierto, barra de acciones y documento seleccionado.
- Capturas en 1366, 1024, 768 y 375 px.
- Comprobar foco visible, hover, deshabilitado, seleccionado y menú abierto.
