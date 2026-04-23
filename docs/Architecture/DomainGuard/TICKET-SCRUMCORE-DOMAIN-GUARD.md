# PROMPT ARQUITECTÓNICO — Crear patrón reusable `DomainGuard` / `ScreenGuard`

## Rol esperado

Arquitecto de software senior frontend (React 19, TypeScript estricto, Clean Architecture, componentes reutilizables, control de estado UI, accesibilidad, testing, documentación técnica).

## Objetivo

Crear un patrón reusable **DomainGuard** que permita bloquear pantallas o secciones UI cuando no se cumple una condición de dominio, evitando duplicación de lógica y asegurando consistencia UX en toda la aplicación.

## Dependencias

- React 19
- Infraestructura de componentes shared
- Sistema de navegación del proyecto

## Contexto existente

Actualmente la lógica de bloqueo de pantallas se implementa de forma puntual en cada módulo, generando duplicación de lógica y riesgo de inconsistencias en comportamiento y UX.

## Estado actual

No existe un componente o hook centralizado para manejar condiciones de bloqueo de dominio, lo que provoca:

- lógica duplicada
- inconsistencia en UX
- dificultad de mantenimiento

## Ubicación esperada

Nota: este repo usa convención `src/app/Components/...` (con C mayúscula). La ubicación final debe respetar la convención del repo.

- Componente: `src/app/Components/Shared/DomainGuard/`
- Hook (si aplica): `src/app/hooks/useDomainGuard.ts`
- Tests: `src/app/Components/Shared/DomainGuard/tests/*`

## Restricciones obligatorias

- NO usar `any`
- NO acoplar a módulos específicos
- NO incluir lógica de negocio en el componente
- NO ejecutar lógica de dominio dentro del guard
- NO romper render de children cuando esté habilitado

## Regla arquitectónica obligatoria

El patrón `DomainGuard` debe desacoplar completamente la condición de dominio de la representación UI.

Esto implica:

- la condición se define externamente
- el guard solo controla el render
- fallback configurable
- reusable en cualquier módulo
- no acoplado a lógica de negocio

## Contrato esperado

### Componente

`DomainGuard({ isBlocked: boolean, fallback: ReactNode, children: ReactNode })`

### Hook

`useDomainGuard({ isEmpty, error, condition }) -> { isBlocked, reason }`

### Semántica

- si `isBlocked` → render fallback
- si no → render children
- **children no deben ejecutarse si está bloqueado** (no se montan, no efectos)

## Ejemplo de uso

```tsx
<DomainGuard
  isBlocked={isEmpty || Boolean(error)}
  fallback={<ErrorState />}
>
  <GestionRespuesta />
</DomainGuard>
```

## Reglas de implementación obligatorias

- No renderizar children cuando esté bloqueado
- Renderizar fallback completamente
- No ejecutar efectos secundarios de children
- Permitir fallback custom
- Mantener tipado estricto

## Reglas de migración segura

- No romper pantallas existentes
- Permitir adopción progresiva
- No forzar refactor global inmediato
- Mantener comportamiento previo cuando no se use el guard

## Reglas de consistencia visual

- fallback reemplaza completamente el contenido
- no coexistencia de estados (no UI parcial)
- UX consistente entre pantallas

## Reglas de interacción

- bloquear completamente interacción cuando esté bloqueado
- permitir CTA en fallback
- no ejecutar eventos de children
- no permitir interacción residual

## Accesibilidad y teclado

- fallback accesible (recomendado `role="alert"` cuando sea error bloqueante)
- CTA accesible por teclado
- no focus trap
- navegación clara

## Riesgos a evitar

- render doble
- children ejecutándose en background
- fallback inconsistente
- acoplamiento a dominio
- pérdida de control del flujo UI

## Pruebas unitarias obligatorias

- renderiza children cuando no está bloqueado
- renderiza fallback cuando está bloqueado
- no ejecuta children en estado bloqueado (no montado)

## Pruebas de integración UI obligatorias

- fallback reemplaza completamente contenido
- children no se renderizan
- integración con pantallas reales funciona

## Pruebas de interacción en navegador obligatorias

- interacción bloqueada correctamente
- CTA funcional
- no hay eventos activos

## Pruebas E2E obligatorias

- pantalla bloqueada correctamente
- navegación desde fallback funciona
- integración con flujo real del módulo

## Pruebas QT / calidad

- sin errores de build
- sin warnings de lint
- sin errores en consola
- comportamiento consistente

## Criterios de aceptación

- patrón reusable funcional
- desacoplado de módulos
- sin duplicación de lógica
- consistente en UX
- sin regresiones

## Documentación obligatoria

El prompt original solicita documentación en `docs/Components/DomainGuard/`. En este repo, por solicitud, se conserva en `docs/Architecture/DomainGuard/` (esta carpeta).

Se deben crear estos archivos (además de este ticket) en `docs/Architecture/DomainGuard/`:

1. `SCRUMCORE-[XX]-Arquitectura.md`
   - Descripción del patrón DomainGuard
   - Problema que resuelve
   - Requerimientos funcionales
   - Diagrama de clases
   - Diagrama de secuencia
   - Diagrama de estados
   - Casos de uso
   - Flujo de ejecución
   - Justificación arquitectónica
   - Beneficios del patrón

2. `SCRUMCORE-[XX]-Implementacion-Detallada.md`
   - Funciones creadas (DomainGuard, hook)
   - Funciones modificadas
   - Descripción detallada de cada función
   - Ubicación de archivos
   - Explicación del flujo de ejecución
   - Decisiones técnicas

3. `SCRUM-[XX]-Integracion-BackEnd.md`
   - Si aplica integración con backend
   - Relación del patrón con endpoints
   - Uso en flujos que dependen de datos backend
   - Manejo de errores de backend
   - Consideraciones de integración

4. `SCRUM-[XX]-Pruebas.md`
   - Pruebas unitarias
   - Pruebas de integración UI
   - Pruebas de interacción navegador
   - Pruebas E2E
   - Casos de prueba
   - Resultados
   - Evidencia de no regresión

## Instrucción final

Antes de implementar:

- definir contrato del componente
- definir hook reusable
- validar puntos de integración

Luego:

- implementar DomainGuard
- integrar en pantalla piloto
- validar comportamiento

Finalmente reportar:

- contrato final
- estrategia implementada
- pruebas ejecutadas
- documentación generada
- validación de no regresión

## “Versión actual” del repo (no omitir detalle)

En el repo ya existen guards, pero no un DomainGuard genérico:

1. Guard de autorización (routing/auth):
   - `src/app/auth/ProteccionRuta/RutaProtegida.tsx`
   - `src/app/auth/ProteccionRuta/Autorizado.tsx`

2. Guard de acciones (tabla dinámica):
   - `src/app/Components/UI/AppTable/utils/dynamicUiActionGuard.ts`

Este ticket NO modifica esos componentes; el `DomainGuard` será un patrón adicional (opt‑in).
