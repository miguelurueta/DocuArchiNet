## Context

SCRUMCORE-250: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- DISEÑO-TOOLBAR

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

## Goals / Non-Goals

**Goals**
- Auditar el toolbar actual del digitalizador documental.
- Implementar el rediseno autorizado del toolbar superior.
- Mantener intactos layout principal, panel de configuracion, scanner, preview y PDF.
- Eliminar el estado compacto de configuracion activa en la barra para evitar duplicidad con el panel lateral.

**Non-Goals**
- No mover el panel lateral de Configuracion de Escaneo.
- No cambiar seleccion de scanner, captura, preview, generacion PDF ni metadata.
- No agregar dependencias nuevas de iconos.
- No implementar drawer, modal ni persistencia de preferencias.

## Decisions

1. Aunque Jira solicitaba inicialmente solo auditoria/propuesta, el usuario autorizo implementacion directa.
2. Se usa `@ant-design/icons`, ya presente en el proyecto, en lugar de agregar `lucide-react`.
3. El toolbar se organiza en grupos:
   - captura;
   - transformacion de pagina;
   - limpieza/eliminacion;
   - salida PDF.
4. Las acciones se renderizan icon-only usando `AppButton` con `icon`, `aria-label` y `tooltip`.
5. `Eliminar` queda como accion destructiva visual (`danger`).
6. `Limpiar` se deshabilita si no hay paginas capturadas.
7. La configuracion activa permanece en el panel lateral; no se duplica en una fila inferior del toolbar.

## Risks / Trade-offs

- La claridad operacional depende de `aria-label` y `tooltip`, porque el toolbar queda icon-only.
- El resumen compacto duplicaba informacion del panel derecho y se elimina para recuperar espacio vertical.
- No se agregan dependencias de iconos; se usan equivalentes disponibles en `@ant-design/icons`.

## Migration Plan

1. Aplicar iconografia y agrupacion visual sobre `DigitalizacionDocumentalWorkspace`.
2. Ajustar CSS responsive del toolbar.
3. Mantener tests existentes y agregar cobertura focal si aplica.
4. Validar TypeScript, lint, vitest focal y OpenSpec.

## Open Questions

- Confirmar si en una fase futura el toolbar debe soportar atajos de teclado.
- Confirmar si en una fase futura se requieren indicadores compactos dentro del propio panel lateral.
