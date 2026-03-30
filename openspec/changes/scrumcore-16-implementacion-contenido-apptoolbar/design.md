## Context

El cambio `SCRUMCORE-16` no crea `AppToolbar` ni `AppContent` desde cero. Ambos componentes ya existen y `GestionCorrespondencia` ya usa una composicion basada en ellos. El ajuste nuevo del ticket se concentra en la zona de acciones del `AppToolbar` dentro de `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`, donde hoy existen acciones y contenido contextual que no coinciden con la estructura exacta pedida por Jira.

El requerimiento explicita una toolbar con solo dos acciones visibles: un `AppDropdown` de exportacion y un `AppButton` para abrir la respuesta contextual. Eso implica eliminar el contenido previo de acciones dentro de la toolbar consumidora, mantener la navegacion relativa `respuesta`, y reforzar el estilo enterprise del modulo sin romper el design system ni el routing anidado existente.

Adicionalmente, el dropdown pedido no es plano: requiere submenu jerarquico para Excel y PDF con opciones hijas. Como `SCRUMCORE-15` introdujo `AppDropdown` como wrapper reusable, este cambio debe extender ese componente sin degradar su API, manteniendo a Ant Design como detalle interno y `AppButton` como trigger principal.

## Goals / Non-Goals

**Goals:**
- Refactorizar la toolbar consumidora de `GestionCorrespondencia` para dejar exactamente dos acciones visibles: `Exportar` y `Abrir respuesta contextual`.
- Implementar el trigger `Exportar` con `AppDropdown` y `AppButton`, soportando submenu jerarquico para Excel y PDF.
- Reconstruir `Abrir respuesta contextual` con `AppButton`, iconografia enterprise y navegacion relativa a `respuesta`.
- Ajustar estilos del modulo para que la toolbar tenga apariencia enterprise y comportamiento responsive coherente con el ticket.
- Mantener cobertura de pruebas sobre el dropdown reusable, la toolbar compartida y la ruta del modulo consumidor.

**Non-Goals:**
- No introducir exportacion real a archivos, llamadas API ni logica de negocio de backend.
- No rediseñar el layout global del dashboard ni `GestionCorrespondenciaLayout`.
- No reemplazar el contrato general de `AppToolbar` en todos los modulos; el ajuste se limita a habilitar una composicion mas flexible para este consumidor.
- No introducir dependencias nuevas fuera de las ya presentes en Ant Design y el design system interno.

## Decisions

### 1. Resolver la accion `Exportar` con `AppDropdown` reusable, no con `Dropdown` directo en la pagina

El ticket exige que el dropdown se construya usando `AppButton` como trigger y Ant Design `Dropdown` como base tecnica. La decision es mantener esa complejidad encapsulada en `AppDropdown`, extendiendo su API para soportar `children` jerarquicos por item.

Rationale:
- Preserva el desacoplamiento entre consumidores y el vendor UI.
- Permite reutilizar el mismo patron en otras toolbars o menus contextuales del proyecto.
- Mantiene coherencia con el cambio `SCRUMCORE-15`.

Alternatives considered:
- Usar `Dropdown` de Ant Design directamente en `GestionCorrespondencia`: descartado por fuga de detalle tecnico y perdida de consistencia.
- Implementar un dropdown ad hoc solo para este modulo: descartado por duplicacion innecesaria.

### 2. Permitir que `AppToolbar` acepte contenido de acciones personalizado

La toolbar compartida ya resuelve titulo, breadcrumbs y grupos de acciones tipadas. Sin embargo, el ticket exige una estructura exacta de dos controles con layout propio, donde el primer control es un dropdown jerarquico. La decision es permitir `actionContent` como region explicita del lado de acciones para este tipo de composicion avanzada.

Rationale:
- Evita deformar `AppToolbarAction` para representar jerarquias de menu que no pertenecen a su contrato original.
- Mantiene la primitive compartida flexible para consumidores avanzados sin perder compatibilidad con el uso actual.
- Reduce la necesidad de wrappers duplicados en el modulo.

Alternatives considered:
- Forzar todo dentro de `actions`/`primaryAction`: descartado porque el contrato actual no modela menus hijos.
- Renderizar los botones fuera de `AppToolbar`: descartado porque rompe la intencion del ticket y la cohesion visual de la toolbar.

### 3. Mantener la navegacion relativa del boton de respuesta contextual

El boton `Abrir respuesta contextual` debe seguir navegando a `respuesta` mediante `useNavigate`, usando `AppButton` y `EyeFilled`. El cambio no debe alterar el flujo del `Drawer` ya probado en `GestionCorrespondenciaRoute`.

Rationale:
- Preserva el comportamiento funcional existente del modulo.
- Evita introducir regresiones en routing anidado.
- Alinea el cambio con una refactorizacion visual y estructural, no funcional.

Alternatives considered:
- Navegacion absoluta: descartada por innecesaria y menos robusta dentro del modulo anidado.
- Mantener la accion previa como `primaryAction` del toolbar: descartado porque el ticket exige reconstruccion explicita con `AppButton`.

### 4. Aplicar los estilos enterprise en el CSS Module del modulo consumidor

Las reglas de `border-radius`, `background`, `box-shadow`, padding, gap y responsive se resuelven en `GestionCorrespondencia.module.css`, no en `AppToolbar.module.css`, porque el ticket pide un ajuste de esta instancia consumidora del componente.

Rationale:
- Evita cambiar globalmente la apariencia base de `AppToolbar` para todos los modulos.
- Mantiene el cambio acotado a la vista especifica pedida por Jira.
- Permite cumplir la responsividad exacta del ticket sin forzar el contrato de todos los consumidores.

Alternatives considered:
- Sobrescribir permanentemente el estilo base de `AppToolbar`: descartado porque afectaria usos existentes.
- Usar estilos inline en la pagina: descartado por inconsistencia con CSS Modules del repo.

## Risks / Trade-offs

- [Risk] Extender `AppDropdown` con jerarquia puede volver mas compleja su API. -> Mitigation: modelar `children` de forma opcional y mantener el caso plano intacto.
- [Risk] La nueva region `actionContent` en `AppToolbar` puede solaparse con `actions` existentes. -> Mitigation: conservar compatibilidad y usar la region solo donde el consumidor necesita layout avanzado.
- [Risk] El responsive pedido por Jira puede diferir del comportamiento base del toolbar. -> Mitigation: encapsular los overrides solo en `GestionCorrespondencia.module.css`.
- [Risk] El ticket puede interpretarse como eliminacion total del contexto del toolbar. -> Mitigation: limitar la eliminacion al contenido interior de acciones del consumidor, preservando el rol estructural de la toolbar.

## Migration Plan

1. Refinar el proposal y la spec del cambio para describir la toolbar exacta requerida por el ticket.
2. Extender `AppDropdown` para soportar items jerarquicos con iconografia reusable.
3. Ajustar `AppToolbar` para aceptar composicion de acciones personalizada sin romper consumidores existentes.
4. Refactorizar `GestionCorrespondencia` y su CSS Module para reflejar la nueva estructura visual.
5. Ejecutar pruebas enfocadas de `AppDropdown`, `AppToolbar` y `GestionCorrespondenciaRoute`.

Rollback:
- Si el nuevo dropdown jerarquico genera regresiones visuales o de interaccion, revertir el uso en `GestionCorrespondencia` y mantener `AppDropdown` en su version plana anterior hasta estabilizar el contrato.

## Open Questions

- Si en una iteracion futura `Exportar Todo` y `Exportar Seleccionados` deben conectarse a handlers reales o seguir como placeholders visuales.
- Si otros modulos del proyecto necesitaran adoptar tambien el patron de `actionContent` en `AppToolbar`.
