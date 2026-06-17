## Why

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- DISEÑO-TOOLBAR. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-250.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.
- Actualizacion de alcance: el usuario autorizo implementar el rediseno del toolbar.
- Se implementa toolbar superior compacto icon-only, agrupado y con iconografia corporativa usando `@ant-design/icons`.
- Se mantienen intactos scanner, preview, PDF, layout principal y panel lateral.
- Se elimina la fila inferior de resumen de configuracion porque duplicaba el panel lateral.

## Resultado de implementacion

- Captura queda en el primer grupo.
- Rotacion izquierda/derecha queda agrupada.
- Eliminar y Limpiar quedan agrupados como acciones de mantenimiento.
- Generar PDF queda como accion de salida.
- Todas las acciones usan `AppButton` con `icon`, `aria-label` y `tooltip`.
- La configuracion activa queda centralizada en el panel lateral; el toolbar conserva solo acciones.

## Jira Details

> AUDITORÍA Y REDISEÑO DEL TOOLBAR DE DIGITALIZACIÓN
> CONTEXTO
> Actualmente el módulo permite:
> Detectar scanner.
> 
> Seleccionar scanner.
> 
> Escanear documentos.
> 
> Visualizar miniaturas.
> 
> Visualizar preview.
> 
> Generar PDF.
> 
> Configurar modo DocuArchi.
> 
> Configurar modo Driver Scanner.
> 
> Los ajustes de Metadata, Preview PDF y Layout principal ya fueron resueltos y no hacen parte de este ticket.
> OBJETIVO
> Realizar una auditoría completa del toolbar actual y proponer un toolbar corporativo, moderno y orientado a productividad documental.
> IMPORTANTE
> NO IMPLEMENTAR.
> NO MODIFICAR CÓDIGO.
> NO CAMBIAR COMPORTAMIENTO.
> SOLO AUDITORÍA Y PROPUESTA DE DISEÑO.
> ==================================================
> AUDITORÍA DEL TOOLBAR ACTUAL==================================================
> 
> Analizar:
> Botones existentes.
> 
> Ubicación actual.
> 
> Estados de habilitación.
> 
> Estados disabled.
> 
> Estados loading.
> 
> Experiencia de uso.
> 
> Botones actuales:
> Escanear
> 
> Reintentar
> 
> Limpiar
> 
> Rotar
> 
> Eliminar
> 
> Generar PDF
> 
> Determinar:
> Qué acciones deben permanecer.
> 
> Qué acciones deben cambiar de ubicación.
> 
> Qué acciones deben agruparse.
> 
> Qué acciones son redundantes.
> 
> Qué acciones faltan.
> 
> Objetivo:
> Diseñar un toolbar superior corporativo.
> Propuesta base:
> Analizar:
> Jerarquía visual.
> 
> Agrupación lógica.
> 
> Acciones primarias.
> 
> Acciones secundarias.
> 
> Iconografía recomendada.
> 
> Sugerir iconos compatibles con Lucide.
> Ejemplos:
> ScanLine
> 
> RotateCcw
> 
> RotateCw
> 
> Trash2
> 
> Broom
> 
> Settings
> 
> FileText
> 
> Save
> 
> Analizar cómo exponer las capacidades detectadas en SCRUMCORE-249.
> Capacidades disponibles:
> ADF
> 
> Duplex
> 
> Color
> 
> Gray
> 
> B&W
> 
> Resolution
> 
> Driver Scanner (PaperStream)
> 
> DocuArchi
> 
> Evaluar:
> A)Panel lateral permanente.
> B)Drawer lateral.
> C)Modal de configuración.
> Determinar cuál ofrece mejor experiencia.
> Propuesta:
> Scanner
> [Fujitsu fi-7160 ▼]
> Modo
> ○ DocuArchi○ Driver Scanner
> Si modo = DocuArchi
> ADF☑ Activado
> Duplex☑ Activado
> Color[Color ▼]
> Resolución[300 dpi ▼]
> Evaluar:
> Distribución.
> 
> Legibilidad.
> 
> Escalabilidad futura.
> 
> Propuesta:
> Modo
> ○ Driver Scanner
> Configuración:
> [⚙ Configurar Scanner]
> Al escanear:
> IfShowUI = true
> y utilizar la ventana nativa PaperStream.
> Determinar:
> Compatibilidad.
> 
> Experiencia de usuario.
> 
> Ventajas.
> 
> Desventajas.
> 
> Evaluar mostrar indicadores compactos.
> Ejemplo:
> ADF ✓Duplex ✓Color ✓300 dpi
> Objetivo:
> Mostrar configuración activa sin abrir paneles.
> Antes de cualquier implementación generar:
> Mockup textual del toolbar.
> 
> Mockup textual del panel de configuración.
> 
> Distribución final de acciones.
> 
> Flujo DocuArchi.
> 
> Flujo Driver Scanner.
> 
> No implementar hasta presentar la propuesta.
> Auditoría del toolbar actual.
> 
> Toolbar recomendado.
> 
> Configuración recomendada.
> 
> Estrategia DocuArchi.
> 
> Estrategia Driver Scanner.
> 
> Iconografía recomendada.
> 
> Mockup textual final.
> 
> Roadmap de implementación.
> 
> Validaciones:
> npx tsc --noEmit
> 
> eslint
> 
> vitest
> 
> NO IMPLEMENTAR.
> SOLO AUDITORÍA Y PROPUESTA DE DISEÑO.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DIGITALIZACIONDOCUMENTAL, DISEÑO, MODULOS, REUSABLE, TOOLBAR

## Capabilities

### New Capabilities
- `modulo-reusable-digitalizaciondocumental-diseno-toolbar`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
