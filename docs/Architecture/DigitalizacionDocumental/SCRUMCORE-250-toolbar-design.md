# SCRUMCORE-250 - Auditoria toolbar de digitalizacion

## Alcance implementado

Se implemento un unico ajuste visual sin dependencia funcional: eliminar la barra inferior de resumen del toolbar.

Fila removida:

```txt
Ajustes | DocuArchi | ADF si | Duplex no | Color 200 dpi
```

La informacion removida ya existe en el panel lateral de Configuracion de Escaneo. No se modificaron:

- integracion Dynamsoft;
- seleccion de scanner;
- configuracion de captura;
- flujo de escaneo;
- preview;
- miniaturas;
- generacion PDF;
- upload o metadata.

## Version actual

Toolbar actual despues del ajuste icon-only:

```txt
[Escanear]
| [Rotar izquierda] [Rotar derecha] [Eliminar pagina] [Limpiar lote]
| [Generar PDF]
```

Panel lateral actual:

```txt
Configuracion de Escaneo
Scanner
Modo de captura
ADF
Duplex
Color
Resolucion
Driver del Scanner
```

## Auditoria AppButton

Componentes del toolbar que usan `AppButton`:

| Accion | Componente |
| --- | --- |
| Escanear | `AppButton` |
| Rotar izquierda | `AppButton` |
| Rotar derecha | `AppButton` |
| Eliminar pagina | `AppButton` |
| Limpiar lote | `AppButton` |
| Generar PDF | `AppButton` |

Todas las acciones del toolbar se renderizan en modo icon-only usando `icon`, `aria-label` y `tooltip`. No se agregaron componentes nuevos ni CSS personalizado para botones.

Componentes que no usan `AppButton`:

| Elemento | Motivo |
| --- | --- |
| Miniatura de pagina | Es selector de pagina dentro de lista, no accion de toolbar |

No se identificaron botones personalizados exclusivos del toolbar fuera del estandar corporativo.

## Version propuesta

El toolbar recomendado debe mantenerse compacto y reservar la configuracion para el panel lateral:

```txt
┌──────────────────────────────────────────────────────┐
│ [Scan] │ [↺] [↻] [Trash] [Broom] │ [FileText] │
└──────────────────────────────────────────────────────┘
```

Acciones primarias recomendadas:

| Accion | Ubicacion |
| --- | --- |
| Escanear | Toolbar |
| Rotar izq | Toolbar |
| Rotar der | Toolbar |
| Eliminar | Toolbar |
| Generar PDF | Toolbar |

Acciones secundarias recomendadas:

| Accion | Ubicacion |
| --- | --- |
| Limpiar | Toolbar o menu secundario |
| Reordenar | Menu contextual de miniatura o modo de ordenamiento |
| Duplicar | Menu contextual de miniatura |
| Mover arriba | Menu contextual de miniatura |
| Mover abajo | Menu contextual de miniatura |

Acciones de visualizacion recomendadas:

| Accion | Ubicacion |
| --- | --- |
| Zoom + | Toolbar |
| Zoom - | Toolbar |
| Zoom 100% | Toolbar o menu de zoom |
| Fit Width | Toolbar |
| Fit Page | Toolbar |
| Pantalla completa | Toolbar |
| Mostrar cuadricula | Panel o menu de visualizacion |
| Mostrar/ocultar miniaturas | Toolbar si el layout lo soporta |

Configuracion recomendada para panel lateral:

| Configuracion | Ubicacion |
| --- | --- |
| Scanner | Panel lateral |
| Driver Scanner | Panel lateral |
| ADF | Panel lateral |
| Duplex | Panel lateral |
| Resolucion | Panel lateral |
| Color | Panel lateral |

Calidad documental recomendada para panel lateral o seccion avanzada:

| Accion | Ubicacion |
| --- | --- |
| Auto Rotate | Panel lateral avanzado |
| Deskew | Panel lateral avanzado |
| Auto Crop | Panel lateral avanzado |
| Blank Page Removal | Panel lateral avanzado |

## Estrategia de migracion

1. Mantener `AppButton` como unico boton de toolbar.
2. Agregar tooltips descriptivos sobre `AppButton` si el componente corporativo ya lo soporta.
3. Introducir acciones de visualizacion sin tocar Dynamsoft ni PDF.
4. Mover acciones por pagina al menu contextual de miniaturas.
5. Mantener configuracion de scanner exclusivamente en el panel lateral.

## Validacion manual recomendada

1. Abrir `/__sandbox/app-digitalizador`.
2. Confirmar que ya no aparece la fila `Ajustes | DocuArchi | ADF | Duplex | Color`.
3. Confirmar que los botones del toolbar son icon-only y muestran tooltip.
4. Confirmar que el panel lateral conserva scanner, ADF, Duplex, Color y Resolucion.
5. Seleccionar scanner.
6. Escanear una pagina.
7. Rotar, eliminar, limpiar y generar PDF.
