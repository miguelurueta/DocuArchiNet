## Context

`SCRUMCORE-114` crea el componente shared `AppInputSelect` en la capa UI del
proyecto. La propuesta inicial generada desde Jira nombra la capability como
`app-appinputselect-01-fe` y menciona un componente tecnico derivado del ticket,
pero la implementacion real debe aterrizarse como `AppInputSelect`, siguiendo el
patron de componentes shared ya existentes en `src/app/Components/UI/`.

El componente debe construirse sobre `Select` de Ant Design y ofrecer una API
reusable para opciones locales y remotas, con soporte de estados vacio, loading,
responsive y tamaños `sm`, `md` y `lg`.

## Goals / Non-Goals

**Goals:**
- Crear `AppInputSelect` como wrapper reusable sobre Ant Design `Select`.
- Alinear la API del componente con los patrones shared de `AppButton`,
  `AppInput` y `AppDropdown`.
- Permitir opciones locales y remotas mediante `options` y `fetchOptions`.
- Mantener una apariencia nativa de Ant Design con ajustes minimos via CSS Modules.
- Exponer tamaños `sm`, `md`, `lg` alineados al lenguaje visual del sistema.
- Incluir documentación de uso y pruebas basicas del contrato reusable.

**Non-Goals:**
- Resolver logica de negocio de un modulo especifico.
- Acoplar el componente a un endpoint backend concreto.
- Introducir una capa global de cache o data fetching.
- Reemplazar selects ya existentes en modulos consumidores dentro de esta FE.

## Decisions

- La implementacion vivira en `src/app/Components/UI/AppInputSelect/`.
- El componente usara `Select` y `Empty` de Ant Design como base de render.
- La API shared expondra `size="sm" | "md" | "lg"` y mapeara internamente a los
  tamaños y clases del wrapper.
- La carga remota entrara por `fetchOptions(query)`; el wrapper administrara
  estado local de `loading`, `options` y `empty` sin conocer el dominio.
- `notFoundContent` sera el punto de integracion para el estado `no data`.
- Los ajustes visuales seran minimos y mantendran radio discreto, responsive y
  comportamiento nativo de Ant Design.
- Se exportara desde `src/app/Components/UI/index.ts`.

## Risks / Trade-offs

- [Riesgo] La propuesta Jira usa un nombre de capability y componente poco
  natural para el código real.
  -> Mitigacion: normalizar el diseño e implementación alrededor de
  `AppInputSelect`, dejando la referencia del ticket solo en OpenSpec.

- [Riesgo] El wrapper puede crecer demasiado si absorbe lógica de red compleja.
  -> Mitigacion: limitar la integración remota a `fetchOptions` y adaptadores de
  opciones, sin cache ni orchestration global.

- [Riesgo] Ajustes visuales excesivos pueden romper el look nativo de Ant Design.
  -> Mitigacion: usar CSS Modules solo para sizing, radius leve y detalles de
  empty/loading, preservando hover/focus/status del control base.

- [Riesgo] La busqueda remota puede sufrir respuestas fuera de orden.
  -> Mitigacion: documentar el uso de debounce/cancelacion en el contenedor y
  mantener la implementación del wrapper defensiva frente a resultados tardíos.
