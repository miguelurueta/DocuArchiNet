## Why

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL-CAPTURA. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-264.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> GESTIÓN AVANZADA DE CAPTURA DOCUMENTAL
> NUEVO · REEMPLAZAR · INSERTAR · AGREGAR
> CONTEXTO
> Actualmente el módulo de digitalización permite:
> ✓ Escanear documentos
> ✓ Visualizar Preview PDF
> ✓ Organizar páginas
> ✓ Reordenar páginas
> ✓ Rotar páginas
> ✓ Eliminar páginas
> ✓ Crop manual
> ✓ Selección múltiple
> ✓ Zoom
> ✓ Pantalla completa
> ✓ Navegación avanzada
> ✓ Duplicar páginas
> Se requiere incorporar operaciones avanzadas para construir y corregir documentos durante el proceso de digitalización sin necesidad de reiniciar el flujo completo.
> ==================================================
> OBJETIVO
> Implementar cuatro operaciones documentales:
> Nuevo
> 
> Reemplazar
> 
> Insertar
> 
> Agregar
> 
> Todas las operaciones deben funcionar tanto para:
> ✓ Escáner
> ✓ Imágenes
> ✓ PDF
> (según capacidades actualmente soportadas por el módulo)
> ==================================================
> UBICACIÓN EN TOOLBAR
> Estas operaciones pertenecen al flujo de captura documental.
> NO son operaciones de edición.
> ==================================================
> UBICACIÓN OBLIGATORIA
> Deben ubicarse inmediatamente después del botón:
> Escanear
> ==================================================
> ORDEN
> [ Escanear ]
> [ Nuevo ]
> [ Reemplazar ]
> [ Insertar ▼ ]
> [ Agregar ]
> Separador visual
> [ Rotar Izquierda ]
> [ Rotar Derecha ]
> [ Seleccionar Área ]
> [ Eliminar ]
> Separador visual
> [ Zoom - ]
> [ Zoom + ]
> [ Ajustar Ancho ]
> [ Ajustar Página ]
> [ Pantalla Completa ]
> ==================================================
> OPERACIÓN 1
> NUEVO
> ==================================================
> DESCRIPCIÓN
> Permite iniciar un nuevo documento desde cero.
> ==================================================
> COMPORTAMIENTO
> Si NO existen páginas:
> ↓
> Iniciar nueva captura inmediatamente.
> ==================================================
> SI EXISTEN PÁGINAS
> Mostrar confirmación.
> ==================================================
> MENSAJE
> Se encontraron páginas en el documento actual.
> ¿Desea descartarlas e iniciar una nueva captura?
> ==================================================
> BOTONES
> Cancelar
> Continuar
> ==================================================
> SI EL USUARIO CONFIRMA
> Eliminar:
> ✓ Páginas
> ✓ Miniaturas
> ✓ Selecciones
> ✓ Crop temporal
> ✓ Navegación activa
> ✓ PDF temporal
> ✓ Estado temporal asociado
> ==================================================
> RESULTADO
> Documento vacío.
> ↓
> Iniciar nueva captura.
> ==================================================
> TOOLTIP
> "Iniciar un nuevo documento"
> ==================================================
> OPERACIÓN 2
> REEMPLAZAR
> ==================================================
> DESCRIPCIÓN
> Permite volver a capturar una página y reemplazar la página actualmente seleccionada.
> ==================================================
> REQUISITO
> Debe existir una página activa en el visor.
> ==================================================
> COMPORTAMIENTO
> Página actual:
> Página 15
> ↓
> Reemplazar
> ↓
> Escanear o cargar imagen
> ↓
> Sustituir Página 15
> ==================================================
> REGLAS
> Mantener:
> ✓ Posición original
> ✓ Orden del documento
> ✓ Navegación
> ==================================================
> ACTUALIZAR
> ✓ Preview
> ✓ Miniatura
> ✓ Organizador
> ✓ Navegador flotante
> ==================================================
> TOOLTIP
> "Reemplazar la página actual"
> ==================================================
> OPERACIÓN 3
> INSERTAR
> ==================================================
> DESCRIPCIÓN
> Permite insertar nuevas páginas dentro del documento.
> ==================================================
> REQUISITO
> Debe existir una página activa.
> ==================================================
> BOTÓN
> Insertar ▼
> ==================================================
> DESPLEGABLE
> Insertar antes
> Insertar después
> ==================================================
> FLUJO
> Página actual:
> Página 10
> ↓
> Insertar después
> ↓
> Escanear o cargar imagen
> ↓
> Insertar nueva página
> ==================================================
> EJEMPLO
> ANTES
> 1
> 2
> 3
> 4
> ==================================================
> Insertar después de página 2
> ==================================================
> RESULTADO
> 1
> 2
> Nueva
> 3
> 4
> ==================================================
> ACTUALIZAR
> ✓ Preview
> ✓ Miniaturas
> ✓ Organizador
> ✓ Navegación
> ==================================================
> TOOLTIP
> "Insertar páginas antes o después de la actual"
> ==================================================
> OPERACIÓN 4
> AGREGAR
> ==================================================
> DESCRIPCIÓN
> Permite agregar nuevas páginas al final del documento.
> ==================================================
> COMPORTAMIENTO
> Documento actual
> 1
> 2
> 3
> 4
> ↓
> Agregar
> ↓
> Escanear o cargar imagen
> ↓
> Resultado
> 1
> 2
> 3
> 4
> Nueva
> ==================================================
> REGLAS
> Siempre agregar al final.
> ==================================================
> ACTUALIZAR
> ✓ Preview
> ✓ Miniaturas
> ✓ Organizador
> ✓ Navegación
> ==================================================
> TOOLTIP
> "Agregar páginas al final del documento"
> ==================================================
> ESTADOS DE BOTONES
> Nuevo
> Siempre habilitado.
> Agregar
> Siempre habilitado.
> Reemplazar
> Solo cuando exista página activa.
> Insertar
> Solo cuando exista página activa.
> ==================================================
> COMPATIBILIDAD
> Debe funcionar correctamente con:
> ✓ Organizador de páginas
> ✓ Drag & Drop
> ✓ Selección múltiple
> ✓ Crop manual
> ✓ Rotación
> ✓ Duplicación
> ✓ Navegación flotante
> ✓ Pantalla completa
> ✓ Zoom
> ✓ Ajustar ancho
> ✓ Ajustar página
> ==================================================
> RENDIMIENTO
> Validar:
> 10 páginas
> 50 páginas
> 100 páginas
> 300 páginas
> 500 páginas
> ==================================================
> ARQUITECTURA
> Crear modelo conceptual:
> CaptureOperation
> Tipos:
> NEW
> REPLACE
> INSERT_BEFORE
> INSERT_AFTER
> APPEND
> ==================================================
> DOCUMENTACIÓN
> Crear:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-292-capture-management.md
> Documentar:
> Arquitectura.
> 
> Flujos.
> 
> Estados.
> 
> UX.
> 
> Casos de uso.
> 
> Riesgos.
> 
> Compatibilidad.
> 
> ==================================================
> VALIDAR
> npx tsc --noEmit
> eslint
> vitest
> IMPLEMENTAR

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: CAPTURA, DIGITALIZACIONDOCUMENTAL, MODULOS, REUSABLE

## Capabilities

### New Capabilities
- `modulo-reusable-digitalizaciondocumental-captura`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
