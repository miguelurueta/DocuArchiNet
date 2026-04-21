# SCRUMCORE-142 — Arquitectura: DomainGuard / ScreenGuard

## Descripción del patrón

`DomainGuard` es un patrón de UI para **bloquear pantallas o secciones** cuando
no se cumple una condición de dominio, sin acoplar el guard a reglas de negocio.

El guard funciona como un **mount gate**:

- si está bloqueado, **renderiza un fallback** y **NO monta** los `children`
- si no está bloqueado, renderiza `children` normalmente

## Problema que resuelve

En ausencia de un guard reusable, los módulos tienden a:

- duplicar lógica de bloqueo (`isEmpty/error/invalid input`)
- dejar UI parcialmente funcional (acciones disponibles sin precondiciones)
- ejecutar efectos secundarios en background (queries, `useEffect`, subscriptions)

El objetivo es estandarizar el “gating” de dominio y evitar bypasses.

## Requerimientos funcionales

- Entrada: `isBlocked: boolean`, `fallback: ReactNode`, `children: ReactNode`
- `isBlocked=true` → fallback visible, children no montados
- `isBlocked=false` → children montados
- Tipado estricto (sin `any`)
- Fallback configurable (el guard no impone UI)

## Diagrama de clases (conceptual)

```text
+-------------------+
|   DomainGuard     |
|-------------------|
| isBlocked: bool   |
| fallback: Node    |
| children: Node    |
|-------------------|
| render(): Node    |
+-------------------+

+-------------------+
|  useDomainGuard   |  (helper opcional)
|-------------------|
| input: {          |
|   isEmpty?        |
|   error?          |
|   condition?      |
| }                 |
|-------------------|
| returns: {        |
|   isBlocked       |
|   reason          |
| }                 |
+-------------------+
```

## Diagrama de secuencia

```text
Pantalla/Feature -> (evalúa dominio) -> isBlocked(boolean)
Pantalla/Feature -> DomainGuard(isBlocked, fallback, children)

alt isBlocked = true
  DomainGuard -> render fallback
  DomainGuard -X-> NO monta children (no effects)
else isBlocked = false
  DomainGuard -> monta children (flujo normal)
end
```

## Diagrama de estados

```text
            +------------------+
            |     Enabled      |
            | (children mount) |
            +---------+--------+
                      |
          isBlocked=true
                      |
                      v
            +------------------+
            |     Blocked      |
            | (fallback only)  |
            +------------------+
                      ^
          isBlocked=false
                      |
                      +----------------
```

## Casos de uso

- **Preflight de datos**: bloquear UI hasta que exista estructura mínima (ej. id válido + data requerida).
- **Bloqueo por error**: bloquear cuando `error != null` y evitar acciones no válidas.
- **Bloqueo por empty**: bloquear cuando `data=[]` representa ausencia de dominio (no continuar).

## Flujo de ejecución (alto nivel)

1. La pantalla calcula la condición de dominio (`isBlocked`) desde su estado/hook.
2. `DomainGuard` decide qué se monta:
   - fallback (bloqueado)
   - children (habilitado)

## Justificación arquitectónica

- Desacopla condición (dominio) de presentación (fallback).
- Evita efectos secundarios al no montar children (más fuerte que “disabled”).
- Facilita adopción progresiva (opt‑in) sin reescribir pantallas existentes.

## Beneficios del patrón

- Consistencia UX: no UI parcialmente funcional.
- Menos bugs: no hay “bypass” por handlers activos.
- Mejor mantenibilidad: lógica repetida se normaliza.
- Pruebas sencillas: contrato pequeño y determinista.

