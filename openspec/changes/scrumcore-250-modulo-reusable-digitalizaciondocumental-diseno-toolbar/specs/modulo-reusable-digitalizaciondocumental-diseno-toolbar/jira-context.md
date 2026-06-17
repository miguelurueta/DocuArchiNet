# Jira Context - SCRUMCORE-250

## Summary

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- DISEÑO-TOOLBAR

## Description

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

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DIGITALIZACIONDOCUMENTAL, DISEÑO, MODULOS, REUSABLE, TOOLBAR
