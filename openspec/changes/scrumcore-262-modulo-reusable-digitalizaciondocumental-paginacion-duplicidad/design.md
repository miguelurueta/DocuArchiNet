## Context

SCRUMCORE-262: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- PAGINACION-DUPLICIDAD

## Jira Details

> DUPLICACIÓN DE PÁGINAS, NAVEGACIÓN FLOTANTE Y CONTROL AVANZADO DE PAGINACIÓN
> CONTEXTO
> Actualmente el módulo de digitalización dispone de:
> ✓ Escaneo documental
> ✓ Preview PDF
> ✓ Organizador de páginas
> ✓ Miniaturas
> ✓ Drag & Drop
> ✓ Rotación
> ✓ Eliminación
> ✓ Crop manual
> ✓ Selección múltiple
> ✓ Zoom
> ✓ Ajustar ancho
> ✓ Ajustar página
> ✓ Pantalla completa
> Se requiere incorporar una experiencia profesional de navegación documental similar a Adobe Acrobat, Foxit o PaperPort.
> ==================================================
> OBJETIVOS
> Implementar:
> Duplicación de páginas.
> 
> Navegación flotante avanzada.
> 
> Búsqueda directa de páginas.
> 
> Navegación anterior / siguiente.
> 
> Compatibilidad completa con pantalla completa.
> 
> Indicador permanente de paginación.
> 
> ==================================================
> FUNCIONALIDAD 1
> DUPLICAR PÁGINA
> Agregar nuevo botón:
> 📄 Duplicar Página
> Ubicación:
> Toolbar principal.
> ==================================================
> COMPORTAMIENTO
> Página seleccionada:
> Página 5
> ↓
> Duplicar
> ↓
> Resultado:
> Página 5
> Página 6 (copia exacta)
> ==================================================
> REGLAS
> Duplicar:
> ✓ Imagen
> ✓ Thumbnail
> ✓ Rotación
> ✓ Crop aplicado
> ✓ Estado visual
> ✓ Metadatos asociados
> ==================================================
> ACTUALIZAR
> Después de duplicar:
> ✓ Preview
> ✓ Miniaturas
> ✓ Organizador
> ✓ Contador de páginas
> ==================================================
> FUNCIONALIDAD 2
> CONTROL FLOTANTE DE PAGINACIÓN
> Eliminar el control de navegación integrado actualmente en toolbar.
> Mover toda la navegación a un componente flotante independiente.
> ==================================================
> NUEVO COMPONENTE
> PageNavigatorFloating
> ==================================================
> UBICACIÓN
> Modo Normal:
> Centro inferior del Preview PDF.
> Superpuesto sobre el documento.
> NO afectar layout.
> NO modificar tamaños existentes.
> ==================================================
> DISEÑO
> Fondo:
> Semitransparente.
> Backdrop Blur.
> Bordes redondeados.
> Estilo moderno tipo Adobe Acrobat.
> ==================================================
> EJEMPLO
> ┌──────────────────────────────┐│ ◀ Página 25 / 120 ▶ │└──────────────────────────────┘
> ==================================================
> FUNCIONALIDAD 3
> PÁGINA ANTERIOR
> Botón:
> ◀
> ==================================================
> COMPORTAMIENTO
> Página actual:
> 25
> ↓
> Página anterior
> ↓
> 24
> ==================================================
> FUNCIONALIDAD 4
> PÁGINA SIGUIENTE
> Botón:
> ▶
> ==================================================
> COMPORTAMIENTO
> Página actual:
> 25
> ↓
> Página siguiente
> ↓
> 26
> ==================================================
> FUNCIONALIDAD 5
> BUSCAR PÁGINA
> El indicador:
> Página 25 / 120
> Debe ser interactivo.
> ==================================================
> COMPORTAMIENTO
> Click sobre:
> 25
> ↓
> Transformar en input.
> Ejemplo:
> ◀ [25] / 120 ▶
> Usuario escribe:
> 87
> ↓
> Enter
> ↓
> Ir automáticamente a página 87.
> ==================================================
> VALIDACIONES
> Página menor a 1
> ↓
> Ir a página 1.
> Página mayor al total
> ↓
> Ir a última página.
> Valor inválido
> ↓
> No generar error.
> ==================================================
> FUNCIONALIDAD 6
> INDICADOR PERMANENTE
> Mostrar siempre:
> Página X de Y
> Ejemplo:
> Página 25 de 120
> ==================================================
> SINCRONIZACIÓN
> Debe sincronizar:
> ✓ Preview
> ✓ Miniaturas
> ✓ Organizador
> ✓ Selección activa
> ==================================================
> FUNCIONALIDAD 7
> PANTALLA COMPLETA
> El componente flotante debe permanecer visible.
> ==================================================
> COMPORTAMIENTO
> Modo normal:
> Preview↓Control visible
> Pantalla completa:
> Preview Full Screen↓Control visible
> ==================================================
> REGLAS
> NO desmontar componente.
> NO perder estado.
> NO reinicializar navegación.
> ==================================================
> FUNCIONALIDAD 8
> AUTOHIDE
> Cuando no exista interacción:
> 3 segundos
> ↓
> Reducir opacidad.
> ==================================================
> AL MOVER EL MOUSE
> Recuperar opacidad completa.
> ==================================================
> FUNCIONALIDAD 9
> ATAJOS DE TECLADO
> Soportar:
> ← Página anterior
> → Página siguiente
> Home Primera página
> End Última página
> ==================================================
> SINCRONIZACIÓN CON OTRAS VISTAS
> Debe funcionar correctamente con:
> ✓ Vista normal
> ✓ Pantalla completa
> ✓ Organizador de páginas
> ✓ Vista 2x2
> ✓ Vista 3x3
> ✓ Vista 4x4
> ✓ Vista 5x5
> ✓ Zoom
> ✓ Ajustar ancho
> ✓ Ajustar página
> ==================================================
> ARQUITECTURA
> Crear:
> PageNavigatorFloating
> Responsabilidades:
> Página actual.
> 
> Total páginas.
> 
> Navegación.
> 
> Búsqueda.
> 
> Atajos.
> 
> Fullscreen.
> 
> ==================================================
> RENDIMIENTO
> Validar:
> 10 páginas
> 50 páginas
> 100 páginas
> 300 páginas
> 500 páginas
> ==================================================
> DOCUMENTACIÓN
> Crear:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-285-page-navigation-and-duplicate.md
> Documentar:
> Arquitectura.
> 
> UX.
> 
> Flujo.
> 
> Eventos.
> 
> Casos de uso.
> 
> Compatibilidad con Full Screen.
> 
> Compatibilidad con Organizador.
> 
> ==================================================
> VALIDAR
> npx tsc --noEmit
> eslint
> vitest
> IMPLEMENTAR

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. TBD

## Risks / Trade-offs

- TBD

## Migration Plan

1. TBD

## Open Questions

- TBD
