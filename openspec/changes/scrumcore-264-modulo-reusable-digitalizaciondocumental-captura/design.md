## Context

SCRUMCORE-264: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL-CAPTURA

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

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. `CaptureOperation` queda en el contrato reusable del scanner (`ScanOptions.captureOperation`) para que la toolbar no duplique logica de adquisicion.
2. `DigitalizacionDocumentalWorkspace` mantiene una unica funcion de captura y las acciones `Nuevo`, `Reemplazar`, `Insertar` y `Agregar` solo definen la intencion operacional.
3. `DynamsoftTwainClient.scan()` conserva el flujo nativo de adquisicion y resuelve despues el orden visual/PDF comparando paginas previas con paginas recien adquiridas.
4. `generatePdf()` sigue usando los indices de `this.pages`; por eso no es necesario mover fisicamente imagenes dentro del buffer Dynamsoft para insertar/reemplazar.

## Risks / Trade-offs

- `removeBlankPages` sigue ejecutandose dentro del flujo existente de scanner. Debe validarse con scanner real cuando se combine con documentos previamente capturados.
- `REPLACE` sustituye la pagina activa por todas las paginas recien capturadas, permitiendo reemplazos 1:N si el driver entrega mas de una pagina.
- La toolbar queda con mas acciones primarias; se conserva iconografia compacta y dropdown para `Insertar` para mantener densidad operativa.

## Migration Plan

1. Extender contrato `ScanOptions` con `CaptureOperation`.
2. Resolver orden de paginas en el adaptador Dynamsoft despues de adquirir.
3. Agregar acciones al toolbar inmediatamente despues de `Escanear`.
4. Cubrir operaciones con pruebas de adaptador y workspace.
5. Documentar arquitectura en `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-292-capture-management.md`.

## Open Questions

- Validar con scanner real si el procesamiento de paginas en blanco debe limitarse solo a paginas recien adquiridas en flujos de append/insert/replace.
