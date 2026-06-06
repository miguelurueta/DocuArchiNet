## Context

El flujo de gestión de respuesta navega a una ruta con parámetro `:id` y monta `GestionRespuesta` dentro de `GestionCorrespondenciaRoute`.

Hoy, al cambiar `/respuesta/:idA` a `/respuesta/:idB`, React puede reutilizar parte del subárbol existente si la identidad de los nodos no cambia de forma explícita. Eso deja estado local o contextual persistente (por ejemplo archivos, visor, row activa, valores de editor) aunque la tarea haya cambiado.

Después de los cambios previos (219–222) la mayor parte del estado transversal ya está centralizado en contexto y hooks; por eso el riesgo de contaminación entre tareas se incrementó si no se fuerza un reinicio de árbol de detalle por tarea.

El cambio debe acotar solo lifecycle y aislamiento de estado sin tocar backend, contratos externos ni comportamiento funcional.

### Stakeholders
- Usuarios que consumen `GestionRespuesta` desde bandeja/visor/documentos
- Equipo de QA en regresión de `AppTable` / `AppTreeTable`
- Equipo de producto por estabilidad de navegación entre tareas

## Goals / Non-Goals

**Goals:**
- Garantizar remount completo del árbol de detalle al cambiar `parsedId` en ruta (`/dashboard/gestion-correspondencia/respuesta/:id`).
- Aislar estado local, contextos y providers asociados al detalle entre tareas distintas.
- Evitar que respuestas asíncronas de solicitudes previas sobrescriban estado del detalle recién montado.
- Preservar estabilidad de UX y layout (master-detail, tabs, carga/bloqueo, deep-link).

**Non-Goals:**
- Cambiar endpoints, contratos API o lógica de backend.
- Alterar AppTable/AppTreeTable ni su contrato público.
- Introducir nuevas features funcionales o cambios de visuales.
- Hacer cambios globales de estado fuera del árbol de detalle de `GestionRespuesta`.

## Decisions

### Decisión 1: key determinística en el ancla de detalle
- **Elección:** aplicar `key={`gestion-respuesta-${parsedId}`}` al componente que representa todo el árbol de detalle dentro de `GestionCorrespondenciaRoute` (el envoltorio que renderiza `detailContent`).
- **Por qué:** fuerza desmontaje/montaje garantizado al cambiar `:id`; evita que React reutilice instancias de componentes/proveedores para tareas distintas.
- **Alternativas consideradas:**
  - Usar estado manual de limpieza (`useState` resets): descartada porque rompe la idea de ciclo de vida declarativo y tiende a omitir caminos de estado interno.
  - Key solo en `AppTabs`: insuficiente para asegurar el remount de providers y contenedores internos.

### Decisión 2: alcance del remount
- **Elección:** incluir en el remount a `GestionRespuesta` y su subárbol completo con providers locales (`GestionRespuestaDocumentosProvider`), tabs y componentes de documentos/visor.
- **Por qué:** la contaminación detectada ocurre en partes internas del detalle; un remount parcial deja rutas de fuga.
- **Alternativas consideradas:**
  - Key solo en `GestionRespuestaMainTabContent` o `DocumentosWorkbench`: aislado parcial, no cubre estado transversal del detalle completo.

### Decisión 3: manejo anti-stale por ciclo de vida al remount
- **Elección:** preservar/fortalecer cancelación o guardas de stale en efectos asíncronos existentes y validar que los hooks usados por el detalle no apliquen respuestas de solicitudes anteriores al nuevo mount.
- **Por qué:** el cambio de identidad desmonta componentes, pero requests en vuelo pueden finalizar tarde y reintroducir data antigua si no se controla.
- **Alternativas consideradas:**
  - Ignorar y confiar en remount: insuficiente para requests que resuelven fuera de componente o en hooks externos.

### Decisión 4: clave base en `parsedId` validado
- **Elección:** usar el `parsedId` ya calculado en ruta como fuente de key; en caso de `:id` inválido mantener comportamiento actual y navegación de bloqueo.
- **Por qué:** estabilidad y consistencia con el estado actual (hook de estructura ya opera con `parsedId`).
- **Alternativas consideradas:**
  - Generar key con objeto compuesto extra (`idTareaWf/radicado/idRespuesta`): aporta complejidad sin valor adicional y puede generar keys inestables durante loading.

## Risks / Trade-offs

- **[Riesgo] Remount insuficiente por ubicación de la key** → Mitigación: ubicar la key en el contenedor de detalle de `GestionCorrespondenciaRoute` (no en nodos secundarios aislados).
- **[Riesgo] Stale responses por request en vuelo** → Mitigación: revisar hooks asíncronos involucrados en mount del detalle y aplicar/validar guardado de “activo” o cancelación.
- **[Riesgo] Flicker al cambiar de tarea** → Mitigación: limitar remount solo al árbol de detalle; conservar shell y estado de navegación.
- **[Riesgo] Cambios de focus/foco brusco en remount rápido** → Mitigación: no introducir side effects extra en efecto de key; validar pruebas de estabilidad de teclado.
- **[Riesgo] Re-render global por key mal aplicada** → Mitigación: no key en toda la ruta, solo en el subárbol de detalle.

## Migration Plan

### Despliegue
1. Implementar diseño en `GestionCorrespondenciaRoute.tsx` con key por `parsedId`.
2. Ajustar/confirmar limpieza en hooks y providers asociados al detalle para estado stale-safe.
3. Ajustar tests de navegación y detalle para cubrir cambio entre ids.
4. Ejecutar suite afectada y pruebas de regresión específicas de `AppTable`/`AppTreeTable`.

### Rollback
- Revertir localmente la key del detalle y deshacer cambios de cleanup asociados.
- Si fuera necesario, restaurar última versión estable del route/página del detalle; validar que no hay efectos secundarios persistentes en navegación.

## Open Questions

- ¿El remount debe cubrir también un posible estado de shell compartido adicional en componentes de padre superior fuera de `GestionCorrespondenciaRoute`?  
  **Propuesta:** empezar con subárbol de detalle y ampliar solo si hay evidencia de persistencia residual.
- ¿Qué hooks del detalle ya emiten abort/signal actualizable por mount y deben fortalecerse explícitamente como parte de este ticket o dejan para hardening futuro?  
  **Propuesta:** validar durante implementación y registrar en riesgos residuales si queda algo fuera de alcance.
