## Context

El ticket `SCRUMCORE-212` apunta a homologar frontend con un contrato Auth actualizado que introduce nuevos claims JWT.

Hoy el frontend usa un modelo mixto:
- `usuario.permisos` (string[]) como fuente principal para autorización.
- `Claim[]` interno con forma `{ nombre, valor }` generado en `ManejadorJWT`.
- Reglas de autorización distribuidas en `useAuth`, `RutaProtegida` y utilidades como `dynamicUiActionGuard`.

El acoplamiento actual a `permisos` puede romperse si backend prioriza claims JWT con naming/estructura distinta. Se requiere una capa de normalización para absorber variantes sin afectar componentes consumidores.

## Goals / Non-Goals

**Goals:**
- Homologar el contrato frontend de autenticación para aceptar claims nuevos sin romper flujos existentes.
- Definir una fuente de verdad única de autorización en frontend (claims normalizados).
- Mantener compatibilidad temporal con payload legacy (`usuario.permisos`).
- Reducir duplicidad de lógica de mapeo de claims entre login, OTP y estado de sesión.

**Non-Goals:**
- Rediseñar navegación o arquitectura SPA.
- Cambiar estrategia de expiración/renovación de token (`TokenWatcher`).
- Introducir control de autorización completamente nuevo en UI.
- Eliminar de inmediato soporte legacy; la migración será gradual.

## Decisions

### Decision 1: Introducir un normalizador de claims en capa Auth

Se define un adaptador único que construye `Claim[]` normalizados desde:
- claims del JWT (nuevo contrato), y/o
- `usuario.permisos` (fallback legacy).

Rationale:
- Evita que cada módulo interprete claims de manera distinta.
- Permite transición backend/frontend sin cambios masivos en UI.

Alternatives considered:
- Leer claims directamente en cada hook/componente.
  - Rechazada: genera drift y reglas inconsistentes.
- Seguir usando solo `permisos`.
  - Rechazada: no soporta el nuevo contrato.

### Decision 2: Endurecer validación de contrato Auth con modo compatible

Se ajusta la validación de respuesta de autenticación para aceptar:
- formato nuevo (claims JWT / estructura nueva), y
- formato legacy mientras dure migración.

Rationale:
- Evita falsos negativos de contrato durante despliegues parciales.
- Mantiene mensajes de error accionables cuando faltan campos críticos.

Alternatives considered:
- Romper compatibilidad y exigir contrato nuevo desde el primer release.
  - Rechazada: alto riesgo operativo si backend y frontend no se despliegan sincronizados.

### Decision 3: Mantener API pública de autorización estable en frontend

`useAuth`, `AutenticacionContext`, `RutaProtegida` y guards de acciones deben seguir consumiendo una representación estable (`Claim[]`/set de permisos efectivos), sin depender de la forma raw del token.

Rationale:
- Minimiza superficie de cambio en módulos de negocio.
- Permite validar la migración con pruebas acotadas.

Alternatives considered:
- Exponer directamente claims raw a consumidores.
  - Rechazada: acopla UI al backend y dificulta mantenimiento.

### Decision 4: Persistencia con prioridad a token y proyección derivada

El token se mantiene como artefacto canónico de sesión. La proyección de claims para autorización se rehidrata de forma determinística, con fallback legacy si corresponde.

Rationale:
- Alinea sesión con backend.
- Facilita auditoría y evita divergencia entre token y almacenamiento auxiliar.

Alternatives considered:
- Persistir múltiples snapshots de permisos sin recomputar.
  - Rechazada: riesgo de desincronización.

## Risks / Trade-offs

- [Risk] Divergencia semántica entre claims nuevos y permisos legacy.
  -> Mitigation: matriz explícita de mapeo claim->permiso efectivo y pruebas de compatibilidad.

- [Risk] Reglas de guard que dependen de strings exactos pueden perder cobertura.
  -> Mitigation: centralizar normalización/canonicalización (case, prefijos, aliases).

- [Risk] Fallback legacy puede ocultar errores del contrato nuevo.
  -> Mitigation: warnings controlados y criterio de retiro del fallback por flag/versionado.

- [Risk] Regresión en login/OTP por cambios de validación de contrato.
  -> Mitigation: pruebas unitarias de validadores + pruebas de integración en hooks `useLogin` y OTP.

## Migration Plan

1. Definir contrato de claims canónico en frontend (tipos + mapeador).
2. Implementar adaptador con doble lectura (nuevo contrato + fallback legacy).
3. Actualizar validadores de respuesta Auth para aceptar ambos formatos.
4. Conectar normalizador en puntos de entrada de sesión (login/OTP/rehidratación).
5. Ajustar y ejecutar pruebas de autorización (`useAuth`, rutas protegidas, guards de acciones).
6. Activar monitoreo funcional; cuando backend esté estable, retirar fallback legacy.

Rollback:
- Mantener pathway legacy (`usuario.permisos`) detrás de la misma interfaz de autorización.
- Si hay incidente, forzar consumo legacy temporal sin cambiar API pública de frontend.

## Open Questions

- ¿Cuál es la estructura exacta de claims nuevos (nombre del claim, tipo, cardinalidad)?
- ¿El backend seguirá enviando `usuario.permisos` durante toda la transición o solo en una ventana corta?
- ¿Existe mapeo oficial entre claims nuevos y permisos de UI existentes (`claimKey`, `requiredAny`, `requiredAll`)?
- ¿Se requiere compatibilidad con tokens emitidos antes de la fecha de despliegue?
