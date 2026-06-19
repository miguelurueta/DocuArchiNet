# Jira Context - SCRUMCORE-258

## Summary

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL-REORGANIZAR-TOOLBAR

## Description

> REORGANIZACIÓN DEL TOOLBAR DE DIGITALIZACIÓN
> CONTEXTO
> Actualmente el toolbar contiene múltiples acciones relacionadas con:
> Navegación
> 
> Edición
> 
> Visualización
> 
> Organización
> 
> Con la incorporación de nuevas funcionalidades (Selección de Área, Organizador de Páginas, Zoom, etc.) se requiere una reorganización para mejorar la experiencia de usuario.
> OBJETIVO
> Agrupar acciones por contexto funcional.
> Reducir carga visual.
> Mejorar descubrimiento de funcionalidades.
> Mantener compatibilidad con funcionalidades actuales.
> ==================================================ORDEN PROPUESTO
> GRUPO 1ORGANIZACIÓN
> ⊞ Organizar páginas
> Tooltip:Organizar páginas
> GRUPO 2NAVEGACIÓN
> 🔍 Página
> Control existente:
> [ Página ] [ 🔍 ]
> Permite:
> Ir a página específica.
> GRUPO 3EDICIÓN
> ↶ Rotar izquierda
> ↷ Rotar derecha
> ✂ Seleccionar área
> 🗑 Eliminar
> 🧹 Limpiar
> GRUPO 4VISUALIZACIÓN
> 🔍− Zoom negativo
> 🔍＋ Zoom positivo
> ↔ Ajustar ancho
> □ Ajustar página
> ⛶ Pantalla completa
> ==================================================LAYOUT VISUAL
> ┌───────────────────────────────────────────────┐│ ⊞ | Página [___] 🔍 | ↶ ↷ ✂ 🗑 🧹 | - + ↔ □ ⛶ │└───────────────────────────────────────────────┘
> ==================================================REGLAS UX
> Usar únicamente:
> AppButton
> con:
> icono
> 
> tooltip
> 
> No mostrar texto permanente.
> Mostrar texto únicamente en tooltip.
> ==================================================TOOLTIPS
> ⊞ Organizar páginas
> ↶ Rotar izquierda
> ↷ Rotar derecha
> ✂ Seleccionar área
> 🗑 Eliminar página
> 🧹 Limpiar documento
> 🔍− Reducir zoom
> 🔍＋ Aumentar zoom
> ↔ Ajustar ancho
> □ Ajustar página
> ⛶ Pantalla completa
> 🔍 Buscar página
> ==================================================SEPARADORES
> Agregar separadores visuales entre grupos.
> Organización
> |
> Navegación
> |
> Edición
> |
> Visualización
> ==================================================ESTADOS
> Deshabilitar acciones cuando:
> No exista página seleccionada.
> Ejemplo:
> RotarEliminarSeleccionar área
> ==================================================DOCUMENTACIÓN
> Crear:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-274-toolbar-reorganization.md
> Documentar:
> Toolbar actual
> 
> Toolbar propuesto
> 
> Grupos funcionales
> 
> Justificación UX
> 
> Mockup final
> 
> ==================================================VALIDACIONES
> npx tsc --noEmit
> eslint
> vitest
> IMPLEMENTAR

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DIGITALIZACIONDOCUMENTAL, MODULOS, REORGANIZAR, REUSABLE, TOOLBAR
