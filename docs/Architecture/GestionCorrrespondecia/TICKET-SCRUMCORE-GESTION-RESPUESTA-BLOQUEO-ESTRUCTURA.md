# PROMPT ARQUITECTÓNICO — Bloquear Gestión Respuesta cuando falta estructura para `idTareaWf`

## Rol esperado

Arquitecto de software senior frontend (React 19, TypeScript estricto, Clean Architecture, control de estado UI, domain guards, accesibilidad, testing, documentación técnica).

## Objetivo

Implementar un bloqueo total de la pantalla de **Gestión Respuesta** cuando no exista estructura válida para el `idTareaWf`, evitando la ejecución de flujos inválidos y garantizando consistencia funcional; además, dejar trazabilidad completa mediante **documentación técnica obligatoria**.

## Dependencias

- Hook: `useEstructuraRespuestaIdTarea`
- Endpoint: `GET /api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea`
- Navegación: `/dashboard/gestion-correspondencia`

## Contexto existente

El componente `GestionRespuestaMainTabContent` consume `useEstructuraRespuestaIdTarea(idTareaWf)` para obtener la estructura base del proceso.

## Estado actual

Cuando la API retorna `success=true` y `data=[]`, el sistema muestra un mensaje informativo pero permite continuar con:

- editor
- envío
- adjuntos

Esto genera flujos inválidos que fallan en integraciones posteriores.

## Ubicación esperada

Prompt original:

- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
- `src/modules/gestionCorrespondencia/hooks/useEstructuraRespuestaIdTarea.ts`

Recomendación (para que el guard sea realmente “a nivel de pantalla” y no haya bypass):

- Incluir también `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx` (resuelve el `idTareaWf` desde `useParams`).

## Restricciones obligatorias

- NO usar `any`
- NO permitir interacción sin estructura
- NO ejecutar hooks dependientes
- NO renderizar UI parcial
- NO permitir bypass del bloqueo

Nota de compatibilidad con la versión actual del repo:

- `useEstructuraRespuestaIdTarea.ts` actualmente contiene un `as any` al mapear el payload. Para cumplir “NO usar any”, este ticket debe incluir la eliminación de ese `any` (tipado correcto del payload).

## Regla arquitectónica obligatoria

Se debe implementar un **Domain Guard a nivel de pantalla** que determine si la estructura es válida antes de habilitar cualquier interacción.

Esto implica:

- bloqueo total del flujo si no hay estructura
- no render de componentes dependientes
- no ejecución de efectos secundarios

Recomendación para cumplir “no focus / no interacción residual”:

- En estado bloqueado, **NO renderizar** toolbar/editor/adjuntos (preferible a “disabled”).

## Contrato esperado

`useEstructuraRespuestaIdTarea` retorna:

- data
- loading
- error
- isEmpty

Nota de compatibilidad con la versión actual del repo:

- El hook hoy expone `estrucTuraRespuesta` (no `data`). Este ticket debe:
  - o bien normalizar/renombrar para exponer `data` (sin romper consumidores), o
  - ajustar el contrato interno manteniendo tipado estricto y actualizando los puntos de uso.

## Reglas de implementación obligatorias

- Si `isEmpty` o `error` → bloquear pantalla
- No renderizar editor
- Deshabilitar AppToolbar
- Bloquear modales
- No ejecutar hooks dependientes
- Mostrar alerta con `idTareaWf`
- Mostrar botón **Volver a la bandeja**

Recomendación para evitar bypass y cumplir todas las reglas:

- “Deshabilitar AppToolbar” debe implementarse como “no renderizar AppToolbar” en modo bloqueado (así no existe foco ni handlers activos).

## Reglas de migración segura

- No romper flujo válido
- No afectar otras pantallas
- No alterar APIs
- No introducir regresiones

## Reglas de consistencia visual

- Estado bloqueado reemplaza completamente el contenido
- No coexistencia de editor y alerta
- CTA único visible

CTA único requerido:

- “Volver a la bandeja” debe navegar a `'/dashboard/gestion-correspondencia'` (ruta fija del módulo).

## Reglas de interacción

- Solo permitir acción “Volver a la bandeja”
- Bloquear todos los eventos
- No permitir focus en elementos ocultos

## Accesibilidad y teclado

- Alerta con `role="alert"`
- Botón accesible por teclado
- Sin focus trap

## Riesgos a evitar

- ejecución parcial del flujo
- hooks activos en background
- UI inconsistente
- apertura de modales
- navegación incorrecta

## Pruebas unitarias obligatorias

- detecta correctamente `isEmpty`
- activa estado bloqueado
- no renderiza editor

## Pruebas de integración UI obligatorias

- toolbar deshabilitada
- editor no visible
- alerta visible
- CTA funcional

## Pruebas de interacción en navegador obligatorias

- no interacción posible
- navegación funciona correctamente

## Pruebas E2E obligatorias

- `data=[]` bloquea pantalla
- error bloquea pantalla
- data válida permite flujo
- navegación a bandeja correcta

## Pruebas QT / calidad

- sin errores de build
- sin warnings de lint
- sin errores en consola
- comportamiento consistente

## Criterios de aceptación

- pantalla se bloquea cuando `data=[]`
- pantalla se bloquea cuando hay error
- no es posible interactuar con editor ni toolbar
- solo existe CTA “Volver a la bandeja”
- no hay regresiones

## Documentación obligatoria

Prompt original solicita crear archivos en: `docs/modulos/gestioncorrespondencia/`.

Nota de compatibilidad con la estructura actual de este repo:

- La carpeta no existe hoy. Este ticket debe incluir **crear** esa ruta (o, si el equipo decide unificar, acordar alternativa). Para no perder detalle del prompt, se mantiene el requerimiento literal de la ruta.

Se deben crear los siguientes archivos en `docs/modulos/gestioncorrespondencia/`:

1. `SCRUMCORE-[XX]-Arquitectura.md`
   - Descripción del problema
   - Requerimiento funcional
   - Diagrama de clases
   - Diagrama de secuencia
   - Diagrama de estados
   - Casos de uso
   - Flujo de ejecución
   - Justificación del Domain Guard
   - Impacto en arquitectura

2. `SCRUMCORE-[XX]-Implementacion-Detallada.md`
   - Lista de funciones creadas
   - Lista de funciones modificadas
   - Descripción detallada de cada función
   - Ubicación de cada cambio (ruta + archivo)
   - Explicación del flujo implementado
   - Decisiones técnicas

3. `SCRUM-[XX]-Integracion-BackEnd.md`
   - Endpoint consumido
   - Parámetros enviados
   - Respuesta esperada
   - Manejo de errores (400, 200 sin datos)
   - Relación con lógica frontend
   - Validaciones aplicadas

4. `SCRUM-[XX]-Pruebas.md`
   - Pruebas unitarias ejecutadas
   - Pruebas de integración UI
   - Pruebas de interacción navegador
   - Pruebas E2E
   - Casos de prueba detallados
   - Resultados obtenidos
   - Evidencia de no regresión

## Instrucción final

Antes de implementar:

- revisar flujo de hooks
- identificar dependencias de estructura

Luego:

- implementar Domain Guard
- bloquear render de componentes
- agregar UI de error

Finalmente reportar:

- estrategia de bloqueo
- cambios realizados
- documentación generada
- pruebas ejecutadas
- validación de no regresión

## Referencias de la versión actual (contrato BE)

Backend: `GET /api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea?idTareaWf=...`

- `idTareaWf <= 0` → `400 BadRequest` con `success=false`
- sin filas para `ID_TAREA_WF` → `200 OK` con `success=true`, `message="Sin resultados"`, `data=[]`
- claim inválido / mapping inconsistente / exception → `400 BadRequest` con `success=false`

Archivo BE:

- `D:\\imagenesda\\GestorDocumental\\DocuArchiCore\\DocuArchi.Api\\Controllers\\GestionCorrespondencia\\SolicitaEstructuraRespuestaIdTareaController.cs`

## Referencias de navegación del módulo (versión actual)

- Ruta base: `/dashboard/gestion-correspondencia`
- Ruta detalle: `/dashboard/gestion-correspondencia/respuesta/:id`
- Acción dominante de retorno vive en el shell: volver a `/dashboard/gestion-correspondencia`

Archivo FE:

- `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx`
