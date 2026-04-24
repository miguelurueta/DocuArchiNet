# PROMPT PROFESIONAL — Exploración arquitectónica completa AppEditor (Modo Análisis)

Repositorio: DocuArchiCore.react  
Rol: Arquitecto de Software Senior / Analista Técnico

=====================================================================

## OBJETIVO

Realizar una exploración técnica profunda del módulo `AppEditor` basada EXCLUSIVAMENTE en el código real del repositorio, y generar documentación arquitectónica exhaustiva, trazable y auditable.

⚠️ ESTE PROMPT ES SOLO DE ANÁLISIS  
❌ NO implementar cambios  
❌ NO proponer refactors aún  
❌ NO inventar arquitectura  

=====================================================================

## ALCANCE

Ruta base:
src/app/Components/UI/AppEditor/

Debe analizar:

### Entrada pública
- index.ts
- README.md
- domain/editor.types.ts
- domain/editor.model.ts
- domain/save-state.types.ts

### Presentation
- AppEditor.tsx
- AppEditorToolbar.tsx
- AppEditorSaveAction.tsx

### Application
- useAppEditor.ts
- usePaginationMetrics.ts
- usePageContext.ts
- useAppEditorSaveState.ts
- normalizeEditorHtml.ts
- localImageIds.ts
- autoPagination.ts
- autoPageBreak.ts

### Infrastructure
- tiptap.config.ts
- tiptap.extensions.ts
- TiptapEditorContent.tsx
- page-break.extension.ts
- resizable-image.extension.ts
- indexeddb/appEditorImageStore.ts
- indexeddb/localImage.types.ts

### Tests (obligatorio cruzar comportamiento)
- AppEditor.test.tsx
- AppEditorToolbar.test.tsx
- useAppEditor.test.tsx
- usePageContext.test.tsx
- usePaginationMetrics.test.tsx
- autoPageBreak.test.ts
- autoPagination.test.ts
- pageBreak.extension.test.ts
- resizableImage.extension.test.ts
- appEditorImageStore.test.ts
- localImageIds.test.ts

=====================================================================

## REGLAS CRÍTICAS

- No asumir comportamiento sin evidencia en código
- Diferenciar claramente:
  - comportamiento propio vs Tiptap/ProseMirror
- Usar rutas reales siempre
- No omitir funciones internas críticas
- Si algo no está claro → declararlo explícitamente
- Priorizar trazabilidad sobre narrativa

=====================================================================

## ARCHIVOS OBLIGATORIOS (SIEMPRE)

### 1. SCRUMCORE-19-FE-Arquitectura.md

Debe incluir:

- Requerimiento
- Diagrama de clases
- Diagrama de secuencia
- Diagrama de estados
- Casos de uso
- Flujo de ejecución
- Justificación arquitectónica

Contenido adicional obligatorio:

1. Resumen ejecutivo
2. Arquitectura por capas (presentation, application, domain, infrastructure)
3. Diagrama ASCII de alto nivel
4. Casos de uso completos:
   - edición
   - toolbar
   - listas
   - imágenes
   - paginación
   - zoom
   - save state
5. Estados del sistema:
   - UI
   - edición
   - selección
   - paginación
   - scroll
   - imágenes
   - guardado
6. Secuencias (ASCII):
   - render inicial
   - escritura
   - final de página
   - paste
   - toolbar
   - imágenes
   - scroll
7. Unidades arquitectónicas:
   - componentes
   - hooks
   - extensiones
   - stores
8. Mapa de archivos completo
9. Mapa de modificación por necesidad
10. Riesgos y zonas sensibles

=====================================================================

### 2. SCRUMCORE-19-FE-Implementacion-Detallada.md

Debe incluir:

- Funciones creadas → No aplica
- Funciones modificadas → No aplica
- Descripción de funciones existentes críticas
- Ubicación exacta (ruta/archivo)
- Decisiones técnicas

Contenido adicional:

- Inventario de funciones clave:
  - nombre
  - archivo
  - propósito
  - parámetros
  - retorno
  - efectos secundarios
- Relación entre hooks y componentes
- Dependencias internas del módulo
- Acoplamientos detectados

=====================================================================

### 3. SCRUM-19-FE-Integracion-BackEnd.md

Debe incluir:

- Endpoint(s)
- Parámetros
- Respuestas
- Manejo de errores
- Relación con frontend

Para este caso:

✔ Marcar explícitamente:
"NO APLICA — El módulo AppEditor no tiene integración directa con backend en este análisis"

=====================================================================

### 4. SCRUM-19-FE-Pruebas.md

Debe incluir:

- Unitarias
- Integración UI
- Browser interaction
- E2E
- Casos de prueba
- Resultados
- Evidencia de no regresión

Contenido adicional:

- Relación entre tests y funcionalidades
- Cobertura real del módulo
- Zonas sin cobertura
- Riesgos no cubiertos por tests
- Qué validan exactamente:
  - paginación
  - listas
  - imágenes
  - scroll
  - selección
  - save state

=====================================================================

## CRITERIOS DE SALIDA

La respuesta debe:

- Estar separada por archivo
- Usar nombres EXACTOS definidos
- Basarse únicamente en código real
- Incluir diagramas ASCII
- Incluir tablas cuando aporte claridad
- Ser lo suficientemente precisa para:
  - auditar
  - mantener
  - refactorizar posteriormente

=====================================================================

## ORDEN DE ENTREGA

1. SCRUMCORE-19-FE-Arquitectura.md
2. SCRUMCORE-19-FE-Implementacion-Detallada.md
3. SCRUM-19-FE-Integracion-BackEnd.md
4. SCRUM-19-FE-Pruebas.md

=====================================================================

## SECCIÓN FINAL OBLIGATORIA

Agregar en cada archivo:

### Supuestos y pendientes

- dudas técnicas reales
- partes del código ambiguas
- validaciones necesarias
- posibles inconsistencias

=====================================================================
