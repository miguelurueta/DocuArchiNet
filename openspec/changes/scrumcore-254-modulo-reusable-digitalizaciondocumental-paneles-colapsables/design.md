## Context

SCRUMCORE-254: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- PANELES-COLAPSABLES

## Jira Details

> PANELES COLAPSABLES PARA DIGITALIZACIÓN DOCUMENTAL
> CONTEXTO
> Actualmente el módulo utiliza tres áreas:
> Miniaturas.
> 
> Preview PDF.
> 
> Configuración de escaneo.
> 
> Se requiere permitir que el usuario oculte paneles para maximizar el espacio útil del documento.
> OBJETIVO
> Permitir:
> ✓ Ocultar Miniaturas.✓ Mostrar Miniaturas.
> ✓ Ocultar Configuración.✓ Mostrar Configuración.
> Manteniendo el Preview PDF como área central dinámica.
> ==================================================FASE 1 - AUDITORÍA
> Documentar en:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-265-collapsible-panels.md
> Analizar:
> Layout actual.
> 
> Grid actual.
> 
> Dependencias.
> 
> Riesgos.
> 
> ==================================================FASE 2 - MINIATURAS
> Agregar botón:
> ☰ Miniaturas
> Estados:
> Visible.
> 
> Oculto.
> 
> El panel debe:
> Contraerse a 0.
> 
> Mantener Drag & Drop.
> 
> Mantener selección.
> 
> Mantener scroll.
> 
> ==================================================FASE 3 - CONFIGURACIÓN
> Agregar botón:
> ⚙ Configuración
> Estados:
> Visible.
> 
> Oculto.
> 
> ==================================================FASE 4 - PREVIEW RESPONSIVO
> Cuando un panel se oculta:
> Preview debe expandirse automáticamente.
> Cuando ambos se ocultan:
> Preview debe ocupar el ancho disponible.
> ==================================================FASE 5 - PERSISTENCIA
> Persistir:
> showThumbnailsshowConfiguration
> usando localStorage.
> ==================================================RENDIMIENTO
> No generar:
> Re-render completo.
> 
> Re-carga de scanner.
> 
> Re-carga de miniaturas.
> 
> ==================================================VALIDACIONES
> tsceslintvitest
> IMPLEMENTAR.

## Goals / Non-Goals

**Goals**
- Permitir ocultar/mostrar Miniaturas y Configuracion de escaneo desde el toolbar.
- Expandir el Preview PDF cuando uno o ambos paneles laterales estan ocultos.
- Persistir `showThumbnails` y `showConfiguration` en `localStorage`.
- Mantener montados los paneles laterales para preservar seleccion, drag and drop, scroll y valores de configuracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.
- Cambios en Dynamsoft, backend, generacion PDF, upload o metadata.
- Reemplazar el layout del workspace por otro componente.

## Decisions

1. El estado de paneles vive en `DigitalizacionDocumentalWorkspace` como `panelPreferences`.
2. La persistencia usa la clave `docuarchi:digitalizacion:panel-preferences` con los campos `showThumbnails` y `showConfiguration`.
3. El layout se resuelve por CSS Grid usando atributos `data-thumbnails-collapsed` y `data-configuration-collapsed` en `<main>`.
4. Los paneles laterales usan el componente reusable `AppCollapseRail` en modo `inline`; el workspace controla el estado y el rail provee header, toggle, restore rail, `aria-controls` y `aria-expanded`.
5. El grid del workspace solo controla la liberacion de ancho: las columnas laterales pasan a `0` cuando el `AppCollapseRail` correspondiente esta colapsado.

## Risks / Trade-offs

- Mantener el DOM montado conserva scroll/estado, pero exige usar el contrato accesible de `AppCollapseRail` para evitar `aria-hidden` manual sobre contenido con foco.
- `localStorage` puede estar bloqueado o corrupto; se encapsula lectura/escritura en `try/catch` y se vuelve a ambos paneles visibles.
- En mobile, los paneles colapsados deben ocupar `block-size: 0` porque el grid pasa a una columna.

## Migration Plan

1. Agregar auditoria en `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-265-collapsible-panels.md`.
2. Agregar estado persistido en `DigitalizacionDocumentalWorkspace`.
3. Envolver Miniaturas y Configuracion con `AppCollapseRail` y ajustar CSS Grid para columnas colapsadas y preview expandido.
4. Cubrir toggles y persistencia en `AppDigitalizador.test.tsx`.
5. Ejecutar suite focal, typecheck, lint y `spec:validate`.

## Open Questions

- La calibracion visual final debe validarse con scanner fisico y lotes grandes para confirmar scroll real de miniaturas.
