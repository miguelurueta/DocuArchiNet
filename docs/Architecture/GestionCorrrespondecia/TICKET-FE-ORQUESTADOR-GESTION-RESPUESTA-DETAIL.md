# PROMPT ARQUITECTÓNICO — Orquestar carga de detalle en `GestionRespuesta` (estructura + editor + dependencias)

## Rol esperado

Arquitecto de software senior frontend (React 19, TypeScript estricto, React Query, Clean Architecture, data orchestration, UX states, accesibilidad, testing).

## Validación de compatibilidad (sin regresión)

Este prompt es **compatible** con este repositorio y puede implementarse sin regresiones si:

- se introduce como capa nueva **opt‑in** (hook orquestador + adopción inicialmente en `GestionRespuesta`);
- se respetan las restricciones (“no queries sin preflight”, “no UI parcial interactiva”);
- se entiende “interacción progresiva” como progresiva por **secciones** (secundarias), manteniendo bloqueo para lo **crítico**.

Puntos a precisar (sin cambiar el alcance del prompt):

1. **APIs secundarias** (editor/adjuntos/metadata)
   - En la versión actual del módulo, varias piezas son UI placeholder o estado local (no hay integración real completa con backend para todas).
   - El orquestador debe soportar estas APIs como dependencias futuras sin romper el contrato, usando `enabled=false` hasta que existan services reales.
2. **Domain Guard implementado previamente**
   - Existe un `DomainGuard` reusable que funciona como *mount gate* (cuando bloquea, no monta children), lo cual alinea con “NO ejecutar queries sin preflight”.
3. **Regla “NO UI parcialmente interactiva” vs “Interacción progresiva”**
   - Compatible si se define que:
     - lo **crítico** (estructura + contenido inicial del editor) bloquea interacción principal;
     - lo **secundario** (adjuntos/metadata) puede cargar progresivamente sin bloquear toda la pantalla.
4. **Documentación obligatoria**
   - La ruta `docs/modulos/gestioncorrespondencia/` puede no existir hoy. El ticket debe crearla o acordar una ruta estándar equivalente, sin omitir contenido.

## Objetivo

Centralizar la carga de datos del detalle de `GestionRespuesta` mediante un **orquestador** que controle preflight, dependencias, estados de carga y errores, evitando ejecución de flujos inválidos y garantizando UX consistente con **skeletons progresivos**.

## Dependencias

- `useEstructuraRespuestaIdTarea`
- APIs secundarias (editor, adjuntos, metadata)
- React Query
- `DomainGuard` implementado previamente

## Contexto existente

Actualmente cada componente puede ejecutar sus propias llamadas, generando:

- lógica dispersa
- ejecuciones innecesarias
- UX inconsistente
- riesgo de flujos inválidos

## Estado actual

No existe un orquestador central que controle:

- preflight
- ejecución condicionada
- estados agregados
- skeletons coordinados

## Ubicación esperada

- `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDetail.ts`
- `src/modules/gestionCorrespondencia/components/*`

## Restricciones obligatorias

- NO usar `any`
- NO ejecutar queries sin preflight válido
- NO duplicar lógica en componentes
- NO acoplar componentes a APIs
- NO permitir UI parcialmente interactiva

## Regla arquitectónica obligatoria

Se debe implementar un **Data Orchestration Layer** que centralice la carga de datos y controle el flujo completo de la pantalla.

Esto implica:

- un hook orquestador único
- control de dependencias entre queries
- separación de estados críticos vs secundarios
- UI desacoplada de lógica de carga

## Contrato esperado

`useGestionRespuestaDetail(idTareaWf)` retorna:

- `isBlocked`
- `loadingGlobal`
- `errorGlobal`
- `estructura`
- `editorData`
- `adjuntos`
- `metadata`
- `sectionsLoading`
- `sectionsError`

## Reglas de implementación obligatorias

- Ejecutar preflight con estructura
- Bloquear ejecución si no hay estructura
- Usar `enabled` de React Query para dependencias
- Separar datos críticos vs secundarios
- No habilitar editor hasta tener contenido inicial
- Manejar errores por sección

Reglas adicionales recomendadas (para evitar fricción en la implementación sin cambiar el objetivo):

- Definir explícitamente qué compone `loadingGlobal` (recomendado: solo carga crítica) para evitar que se quede activo por cargas secundarias.
- Si una API secundaria aún no existe, el hook debe exponer `editorData/adjuntos/metadata` como `null/undefined` y mantener su query deshabilitada, sin romper el contrato del orquestador.

## Reglas de migración segura

- No romper componentes existentes
- Permitir adopción progresiva
- No alterar APIs
- Mantener comportamiento actual en flujo válido

## Reglas de consistencia visual

- Skeleton por sección
- No flash de contenido
- Progreso visible
- No UI inconsistente

## Reglas de interacción

- Editor bloqueado hasta carga crítica
- Secciones secundarias no bloquean todo
- Interacción progresiva

## Accesibilidad

- Skeletons perceptibles
- Estados de error accesibles
- No pérdida de foco

## Riesgos a evitar

- ejecución sin preflight
- race conditions
- UI parcialmente activa
- duplicación de lógica
- inconsistencia de estados

## Pruebas unitarias

- hook orquestador agrupa estados correctamente
- gating funciona
- no ejecución sin estructura

## Pruebas integración UI

- skeletons correctos
- secciones se cargan progresivamente
- errores no rompen pantalla

## Pruebas navegador

- interacción progresiva
- editor no usable antes de tiempo

## Pruebas E2E

- flujo completo de carga
- bloqueo sin estructura
- carga progresiva correcta

## Pruebas QT

- sin errores build
- sin warnings
- UX consistente

## Criterios de aceptación

- no se ejecutan APIs sin estructura
- skeletons visibles y coherentes
- carga progresiva funcional
- errores controlados
- no regresiones

## Documentación obligatoria

Ruta requerida por el prompt:

- `docs/modulos/gestioncorrespondencia/`

**Nota (compatibilidad repo):** en este repo esa ruta puede no existir aún. Este ticket debe crearla o acordar una ruta estándar equivalente, sin omitir el contenido.

Archivos:

- `SCRUMCORE-[XX]-Arquitectura.md`
  - diagramas de clase
  - secuencia
  - estados
  - casos de uso
  - flujo orquestador
- `SCRUMCORE-[XX]-Implementacion-Detallada.md`
  - hook creado
  - funciones modificadas
  - descripción
  - ubicación
- `SCRUM-[XX]-Integracion-BackEnd.md`
  - APIs consumidas
  - dependencias
  - manejo de errores
- `SCRUM-[XX]-Pruebas.md`
  - unitarias
  - integración
  - browser
  - E2E
  - resultados

## Instrucción final

Antes de implementar:

- definir contrato del hook
- identificar APIs dependientes

Luego:

- implementar orquestador
- integrar en página
- validar estados

Finalmente reportar:

- contrato
- flujo
- pruebas
- documentación
- validación de no regresión
