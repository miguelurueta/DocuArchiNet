# SCRUMCORE-143
# Ajustes visuales en `GestionRespuestaMainTabContent` (Header + AppUpload)

## Objetivo

Definir la arquitectura de ajustes visuales en la vista de gestión de respuesta para:

- reemplazar el botón `Volver a la bandeja` por un botón solo ícono con `LeftOutlined`
- reubicar `metadata` en el header, a la derecha inmediata del ícono de retorno
- eliminar mensajes de `headerDescription`
- reducir visualmente el tamaño del `AppUpload` para una presentación más compacta

Este documento describe únicamente lineamientos de implementación FE. No ejecuta cambios funcionales ni lógica de negocio.


## Alcance funcional

### Incluye

- ajuste de composición visual del bloque superior de `GestionRespuestaMainTabContent`
- ajuste de micro-layout del header para alinear botón de retorno + metadata en la misma fila
- desactivación de `headerDescription` en el header informativo
- ajuste de dimensiones visuales de `AppUpload` dentro del módulo de gestión de respuesta

### No incluye

- cambios de endpoints
- cambios de contratos de datos (`estrucTuraRespuesta`)
- cambios de comportamiento backend
- rediseño completo del formulario


## Contexto del cambio

Actualmente el header muestra:

- botón con texto: `Volver a la bandeja`
- bloque `metadata` en posición separada
- descripciones de header (`headerDescription`)

Se requiere un header más compacto, con prioridad en iconografía y datos de contexto.


## Especificación UI objetivo

## 1) Botón de retorno (solo ícono)

- Reemplazar el `AppButton` con texto por variante icon-only.
- Ícono requerido: `LeftOutlined`.
- Debe conservar accesibilidad:
  - `aria-label="Volver a la bandeja"`
  - foco visible
  - navegación por teclado

Regla visual:

- no mostrar texto visible junto al ícono
- mantener acción de navegación existente sin alterar flujo

## 2) Reubicación de metadata

Bloque objetivo:

```tsx
metadata={[
  { label: "Radicado", value: loading ? "..." : (estrucTuraRespuesta?.Radicado ?? "-") },
  { label: "Remitente", value: loading ? "..." : (estrucTuraRespuesta?.Destinatario ?? "-") },
  { label: "Trámite", value: estrucTuraRespuesta?.TramiteDocumento ?? "-" },
]}
```

Requisito de layout:

- `metadata` permanece en el header
- debe ubicarse inmediatamente a la derecha del botón ícono `LeftOutlined`
- en desktop: misma línea horizontal
- en móvil: puede envolver a segunda línea sin perder jerarquía

## 3) Eliminación de `headerDescription`

- Remover render de mensajes asociados a `headerDescription`.
- No dejar placeholders vacíos ni espacio reservado.
- Si existe prop obligatoria en componente base, enviar valor neutro (`undefined` / `null`) sin texto en UI.

## 4) Ajuste visual de `AppUpload` (más pequeño)

Aplicar modo compacto de UI:

- reducir alto del contenedor de carga
- reducir padding interno
- reducir tamaño de tipografía secundaria
- mantener botones y dropzone funcionales

Reglas:

- no degradar accesibilidad ni zona mínima clickeable
- no romper comportamiento responsive
- no alterar lógica de carga/validación


## Diseño responsive

### Desktop

- fila superior con ícono de retorno + metadata alineados
- spacing corto y consistente con design system

### Tablet

- mantener jerarquía visual
- metadata puede ajustar gap y wrap controlado

### Mobile

- botón ícono visible al inicio
- metadata debajo o al costado según ancho disponible
- `AppUpload` compacto sin colisiones ni overflow horizontal


## Accesibilidad

- botón de retorno icon-only con `aria-label` obligatorio
- orden de tabulación intacto
- contraste de texto/ícono conforme al DS
- mantener estados de foco en controles del `AppUpload`


## Archivos candidatos a modificación (cuando se implemente)

- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.module.css`
- `src/app/Components/UI/AppUpload/*` (solo si se requiere variante compacta reusable del componente base)


## Estrategia técnica recomendada

1. Ajustar composición del header en `GestionRespuestaMainTabContent`.
2. Sustituir botón textual por botón icon-only con `LeftOutlined`.
3. Reordenar bloque `metadata` en el contenedor derecho del botón.
4. Eliminar render de `headerDescription`.
5. Aplicar variante compacta de `AppUpload` con CSS Module local o prop de tamaño si existe.
6. Verificar responsive en anchos pequeños.


## Criterios de aceptación

- El botón `Volver a la bandeja` ya no muestra texto; solo ícono `LeftOutlined`.
- `metadata` se visualiza a la derecha del botón de retorno en el header.
- No se muestran mensajes de `headerDescription`.
- `AppUpload` se percibe visualmente más pequeño y compacto.
- No hay regresión funcional en navegación, carga de archivos ni accesibilidad básica.
