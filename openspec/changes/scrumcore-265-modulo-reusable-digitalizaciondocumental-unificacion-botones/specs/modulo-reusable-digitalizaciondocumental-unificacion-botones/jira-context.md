# Jira Context - SCRUMCORE-265

## Summary

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL-UNIFICACION-BOTONES

## Description

> UNIFICACIÓN DE BOTONES ESCANEAR Y NUEVO DOCUMENTO
> CONTEXTO
> Actualmente existen dos acciones independientes:
> ✓ Escanear
> ✓ Nuevo
> Sin embargo ambas pertenecen al mismo flujo de captura documental y generan redundancia visual en la toolbar.
> Se requiere simplificar la experiencia de usuario utilizando un único botón inteligente cuyo comportamiento cambie según el estado actual del documento.
> ==================================================
> OBJETIVO
> Eliminar la coexistencia de:
> Escanear
> Nuevo
> y reemplazarlos por una única acción contextual.
> ==================================================
> COMPORTAMIENTO
> ESTADO 1
> DOCUMENTO VACÍO
> ==================================================
> CONDICIÓN
> pages.length === 0
> ==================================================
> MOSTRAR
> 🖨 Escanear
> ==================================================
> TOOLTIP
> "Iniciar captura documental"
> ==================================================
> COMPORTAMIENTO
> Click
> ↓
> Iniciar captura inmediatamente.
> ↓
> NO solicitar confirmación.
> ==================================================
> ESTADO 2
> DOCUMENTO CON CONTENIDO
> ==================================================
> CONDICIÓN
> pages.length > 0
> ==================================================
> REEMPLAZAR EL MISMO BOTÓN
> Mostrar:
> 📄 Nuevo Documento
> o
> ↻ Nuevo
> (según lineamientos visuales actuales)
> ==================================================
> TOOLTIP
> "Descartar documento actual e iniciar uno nuevo"
> ==================================================
> COMPORTAMIENTO
> Click
> ↓
> Mostrar confirmación obligatoria.
> ==================================================
> UTILIZAR LA CONFIRMACIÓN EXISTENTE
> NO crear un nuevo modal.
> NO crear una nueva implementación.
> Reutilizar exactamente la alerta ya desarrollada para la operación NEW.
> ==================================================
> MENSAJE
> Se encontraron páginas en el documento actual.
> ¿Desea descartarlas e iniciar una nueva captura?
> ==================================================
> BOTONES
> Cancelar
> Continuar
> ==================================================
> SI EL USUARIO CANCELA
> NO realizar cambios.
> Mantener:
> ✓ Páginas
> ✓ Miniaturas
> ✓ Selección
> ✓ Preview
> ✓ PDF generado
> ✓ Estado actual
> ==================================================
> SI EL USUARIO CONTINÚA
> Ejecutar exactamente la operación:
> NEW
> ya implementada en SCRUMCORE-292.
> ==================================================
> LIMPIAR
> ✓ Páginas
> ✓ Miniaturas
> ✓ Selecciones
> ✓ Crop temporal
> ✓ PDF temporal
> ✓ Estado de navegación
> ✓ Estado de captura
> ==================================================
> RESULTADO
> Documento vacío.
> ↓
> Iniciar nueva captura.
> ==================================================
> TOOLBAR
> ANTES
> [ Escanear ]
> [ Nuevo ]
> [ Reemplazar ]
> [ Insertar ]
> [ Agregar ]
> ==================================================
> DESPUÉS
> [ Escanear / Nuevo Documento ]
> [ Reemplazar ]
> [ Insertar ]
> [ Agregar ]
> ==================================================
> BENEFICIOS
> ✓ Menos ruido visual.
> ✓ Menos botones.
> ✓ Flujo más intuitivo.
> ✓ Mejor aprovechamiento del espacio.
> ✓ Experiencia similar a aplicaciones profesionales de digitalización.
> ==================================================
> COMPATIBILIDAD
> Validar con:
> ✓ Escáner
> ✓ Carga de imágenes
> ✓ Carga de PDF
> ✓ Reemplazar
> ✓ Insertar
> ✓ Agregar
> ✓ Navegación flotante
> ✓ Selección múltiple
> ✓ Pantalla completa
> ==================================================
> DOCUMENTACIÓN
> Actualizar:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-292-capture-management.md
> Agregar sección:
> "Botón Inteligente Escanear / Nuevo Documento"
> ==================================================
> VALIDAR
> npx tsc --noEmit
> eslint
> vitest
> IMPLEMENTAR

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DIGITALIZACIONDOCUMENTAL, MODULOS, REUSABLE, UNIFICACION
