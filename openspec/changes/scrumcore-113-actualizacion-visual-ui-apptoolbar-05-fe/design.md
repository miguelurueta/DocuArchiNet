## Context

`SCRUMCORE-113` ajusta visualmente el `AppToolbar` que vive dentro del tab
**Gestion** en `GestionRespuesta`. El estado actual del componente usa una
combinacion de `actions` y `primaryAction`, lo que crea una jerarquia visual
distinta entre botones que para este caso ya no representa la intencion del
workspace.

El requerimiento del ticket es dejar tres acciones homogéneas y alineadas a la
izquierda dentro del toolbar:

- `Solicitud de Aprobacion`
- `Guardar`
- `Enviar`

Todas deben reutilizar el mismo contrato visual con `size="sm"` y
`variant="ghost"`, sin modificar la estructura del tab ni afectar el workbench
del editor y adjuntos.

Adicionalmente, el ajuste visual requiere que el texto y el icono de estas
acciones usen una tinta gris en estado normal, conservando sin cambios el
estado hover, focus y el comportamiento general del `AppButton`.

## Goals / Non-Goals

**Goals:**
- Unificar visualmente las tres acciones del `AppToolbar` del tab **Gestion**.
- Eliminar la distincion visual entre accion primaria y acciones secundarias.
- Mantener las acciones agrupadas a la izquierda dentro del toolbar.
- Ajustar solo la tinta base de texto e iconos para las acciones `ghost` de
  este toolbar, sin alterar hover ni jerarquia interactiva.
- Preservar el layout actual del workbench y la integracion con `AppCollapseRail`
  y `AppUpload`.

**Non-Goals:**
- Cambiar logica de negocio o handlers reales de las acciones.
- Modificar el tab **Documentos**.
- Introducir un nuevo contenedor para reemplazar `AppToolbar`.
- Alterar estilos globales o shared fuera del alcance del toolbar consumido en
  `GestionRespuestaMainTabContent`.

## Decisions

- Reutilizar `AppToolbar` como contenedor principal del bloque de acciones.
- Representar las tres acciones dentro de `actions` en lugar de combinar
  `actions` + `primaryAction`.
- Mantener `size="sm"` y `variant="ghost"` en las tres acciones para un lenguaje
  visual uniforme.
- Mantener el comportamiento interactivo actual de `AppButton`, limitando el
  ajuste cromatico al estado normal del texto e iconos.
- Usar iconografia de Ant Design coherente con el contexto actual del modulo.
- Mantener el ajuste encapsulado en
  `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`.

## Risks / Trade-offs

- [Riesgo] El `AppToolbar` puede aplicar un orden visual distinto entre acciones
  y primary action.
  -> Mitigacion: mover las tres acciones al mismo bloque `actions`.

- [Riesgo] Cambiar la jerarquia visual de `Enviar` puede alterar expectativas de
  UX previas.
  -> Mitigacion: dejar la decision explicitada en el ticket como ajuste visual
  intencional y no como regresion.

- [Riesgo] El texto `Solicitud de Aprobacion` puede exigir wrap en resoluciones
  medias.
  -> Mitigacion: validar que el toolbar conserve alineacion izquierda y
  comportamiento responsive sin romper el layout del workbench.

- [Riesgo] Un cambio global en `AppButton` puede alterar otros consumidores de
  la variante `ghost`.
  -> Mitigacion: documentar que solo se ajusta la tinta base y preservar hover,
  focus y comportamiento general para evitar regresiones visuales no deseadas.
