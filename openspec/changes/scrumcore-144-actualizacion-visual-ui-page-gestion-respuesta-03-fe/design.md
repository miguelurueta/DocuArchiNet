## Context

La pantalla `GestionRespuestaMainTabContent` requiere un ajuste visual puntual para mejorar densidad y jerarquía de información sin alterar contratos de datos ni lógica de negocio.

Estado actual identificado:

- El botón de retorno se renderiza con texto (`Volver a la bandeja`), ocupando ancho relevante en header.
- El bloque `metadata` no está optimizado para convivir junto al control de retorno como una barra compacta.
- `headerDescription` introduce ruido visual para este flujo.
- `AppUpload` se percibe grande para el contexto de respuesta y desplaza contenido relevante.

Restricciones:

- Mantener React + TypeScript estricto + CSS Modules.
- No modificar endpoint, modelos de dominio ni flujo de envío.
- Mantener accesibilidad (focus, labels, navegación por teclado).

Stakeholders principales:

- Usuarios operativos de gestión de respuesta.
- Equipo FE que mantiene Design System (`AppButton`, `AppUpload`).

## Goals / Non-Goals

**Goals:**

- Convertir el botón de retorno en control icon-only con `LeftOutlined`.
- Reubicar `metadata` para que quede visualmente a la derecha del botón de retorno.
- Eliminar la salida visual de `headerDescription`.
- Reducir el footprint visual de `AppUpload` (modo compacto) manteniendo funcionalidad.
- Asegurar comportamiento responsive en desktop/tablet/mobile.

**Non-Goals:**

- Cambiar fuentes de datos de `estrucTuraRespuesta`.
- Cambiar reglas de validación o envío de archivos.
- Rediseñar por completo `GestionRespuestaMainTabContent`.
- Introducir dependencias nuevas de UI.

## Decisions

### 1) Ajuste del header en el módulo consumidor (no en componentes globales)

**Decisión:** Implementar layout del header desde `GestionRespuestaMainTabContent` y su CSS Module local, reutilizando componentes existentes.

**Rationale:** El requerimiento es contextual de una vista específica; evita impacto transversal en otros consumidores.

**Alternativas consideradas:**

- Modificar un layout/header global compartido.
  - Rechazada: riesgo de regresión en otras pantallas.
- Crear un componente nuevo exclusivo de header.
  - Rechazada por ahora: sobreingeniería para un ajuste acotado.

### 2) Botón de retorno icon-only reutilizando `AppButton`

**Decisión:** Mantener `AppButton` para consistencia DS, pero en modo solo ícono con `LeftOutlined` y `aria-label`.

**Rationale:** Consistencia visual y de comportamiento con controles ya homologados; mínima superficie de cambio.

**Alternativas consideradas:**

- Reemplazar por `button` nativo custom.
  - Rechazada: rompe consistencia con DS y estados de interacción.

### 3) Metadata en línea con el control de retorno

**Decisión:** Estructurar un contenedor horizontal `back + metadata`, permitiendo wrap controlado en breakpoints pequeños.

**Rationale:** Mejora scanning visual; conserva semántica del bloque y evita reposicionamientos bruscos.

**Alternativas consideradas:**

- Mantener metadata debajo del botón para todos los tamaños.
  - Rechazada: desperdicia espacio en desktop y no cumple intención de compactar header.

### 4) Ocultar `headerDescription` en este flujo

**Decisión:** No renderizar contenido de `headerDescription` en la vista objetivo (pasando valor neutro si la API del componente lo exige).

**Rationale:** El copy no agrega valor al caso actual y aumenta altura del bloque superior.

**Alternativas consideradas:**

- Dejar descripción con texto más corto.
  - Rechazada: mantiene costo vertical y ruido visual.

### 5) Compactación de `AppUpload` por variante de estilo

**Decisión:** Aplicar estilo compacto preferiblemente desde el contexto local; escalar a variante reusable en `AppUpload` solo si el componente ya expone patrón de variantes/tamaños.

**Rationale:** Minimiza riesgos en consumidores existentes y permite iteración rápida.

**Alternativas consideradas:**

- Alterar estilos base globales de `AppUpload`.
  - Rechazada: alto riesgo de regresión cross-módulo.

## Risks / Trade-offs

- **[Riesgo]** El botón icon-only puede perder claridad para usuarios nuevos.  
  **Mitigación:** `aria-label`, tooltip opcional y posición estable al inicio del header.

- **[Riesgo]** Compactar `AppUpload` podría reducir área clickeable en mobile.  
  **Mitigación:** mantener tamaño mínimo interactivo y validar con pruebas visuales en breakpoints pequeños.

- **[Riesgo]** Cambios de layout pueden romper alineación con textos largos de metadata.  
  **Mitigación:** aplicar `wrap`, `min-width` y truncamiento controlado según DS.

- **[Trade-off]** Solución local prioriza velocidad sobre abstracción reusable inmediata.  
  **Mitigación:** documentar oportunidad de promover variante reusable de `AppUpload` si aparecen más consumidores.

## Migration Plan

1. Ajustar header en `GestionRespuestaMainTabContent.tsx` (icon-only + metadata inline + no `headerDescription`).
2. Aplicar estilos en `GestionRespuestaMainTabContent.module.css` para alineación y responsive.
3. Compactar `AppUpload` en el mismo contexto (o habilitar variante de tamaño en componente base si es necesario).
4. Verificar visualmente desktop/tablet/mobile.
5. Ejecutar pruebas focalizadas del módulo de gestión de correspondencia y componente afectado.
6. Si hay regresión visual, rollback parcial a layout anterior manteniendo cambios aislados por CSS Module.

## Open Questions

- ¿`AppUpload` actual ya soporta prop de tamaño/variant reutilizable o requiere clase contextual?
- ¿se desea tooltip visible en el botón de retorno icon-only además de `aria-label`?
- ¿se requiere truncamiento explícito para valores largos de `Remitente` y `Trámite` en desktop?
