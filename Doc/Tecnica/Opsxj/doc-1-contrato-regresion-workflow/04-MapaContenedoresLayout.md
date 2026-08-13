# Mapa de contenedores y restricciones de layout

## Jerarquía relevante

```text
#div_content_general_wf
├── UpdatePanel_menu_cab / #menucab
├── zona de tareas / Panelactividad
├── UpdatePanelseleccion
│   └── GridView_list_documento_relacion_wf
├── #contenido_indice
│   └── UpdatePanelindice / Panel_indice
└── #contenido_imagen
    ├── UpdatePanel_panel_toll
    └── UpdatePanelVisor / #ifrm_visor_
```

## Contrato para JIRA-04 y cambios visuales posteriores

| Zona | Cambios permitidos | Restricciones |
| --- | --- | --- |
| `#div_content_general_wf` | Grid/Flex, `gap`, tamaños mínimos y máximos, color/espaciado | Conservar el nodo raíz y la lógica de alto heredada hasta que exista una migración explícita. |
| Cabecera / `#menucab` | Estilos de presentación y envoltorios no estructurales | No mover `UpdatePanel_menu_cab`, controles ni disparadores ASP.NET. |
| `Panelactividad` | Overflow, alto flexible y estilos de filas | No romper `GridView2` ni la selección por JavaScript. |
| `UpdatePanelseleccion` | Ancho, borde, overflow del área externa | No convertir el panel en `display: contents` ni reubicar filas/checkbox. |
| `#contenido_indice` | Distribución lateral, ancho y visibilidad controlada por reglas existentes | No eliminar la alternancia de ancho con `#contenido_imagen` ni colapsar `UpdatePanelindice`. |
| `#contenido_imagen` | Zona flexible de visor, límites de tamaño y bordes | No modificar `#ifrm_visor_`, rutas ni cálculo de alto sin una prueba específica. |
| `UpdatePanelVisor` / toolbar | Presentación interna | No moverlos fuera del contenedor ni sustituir su ciclo parcial. |

## Prohibiciones explícitas

- No aplicar `display: contents` a un `UpdatePanel` o a un contenedor que contiene controles WebForms con ID generado.
- No reordenar nodos entre los paneles de documentos, índice y visor.
- No usar `position: absolute` para ocultar una zona que el script mide por `clientHeight`.
- No depender de hijos añadidos por JavaScript para definir el contexto de tarea o documento.

