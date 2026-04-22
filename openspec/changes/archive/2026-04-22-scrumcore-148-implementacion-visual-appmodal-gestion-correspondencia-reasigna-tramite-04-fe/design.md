## Context

`SCRUMCORE-148` solicita una confirmacion visual posterior al envio valido de `ReasignarRespuestaModal` en el modulo `gestionCorrespondencia`.

Hoy el flujo de reasignacion cierra el modal de entrada, pero no muestra un estado de exito desacoplado con `AppModal`. El ticket exige un modal limpio, centrado y consistente con el Design System, con foco en accesibilidad y responsive.

Restricciones relevantes:
- Mantener React 19 + TypeScript estricto.
- Reusar componentes shared existentes (`AppModal`, `AppButton`).
- No modificar `AppModal`.
- No introducir logica de negocio en el nuevo modal.

Stakeholders:
- Usuarios operativos de Gestion de Correspondencia.
- Equipo frontend responsable de estandar visual y pruebas.

## Goals / Non-Goals

**Goals:**
- Crear `TramiteReasignadoModal` reusable en `src/modules/gestionCorrespondencia/components/modalTramiteReasignado/`.
- Integrar la apertura del modal de confirmacion al completar el submit valido en `ReasignarRespuestaModal`.
- Renderizar contenido de confirmacion (`Usuario Asignado`, `Radicado`) con estilo consistente y accesible.
- Garantizar comportamiento responsive desktop/tablet/mobile.
- Cubrir pruebas unitarias e integracion minima del flujo.

**Non-Goals:**
- Cambiar endpoints, contratos API o reglas de negocio de reasignacion.
- Alterar componentes shared (`AppModal`, `AppButton`).
- Rediseñar de forma global el modulo de gestion de correspondencia.

## Decisions

### 1) Nuevo componente de confirmacion desacoplado
- **Decision:** crear `TramiteReasignadoModal.tsx` + `TramiteReasignadoModal.module.css` en carpeta dedicada.
- **Rationale:** evita acoplar UI de confirmacion al modal de captura; facilita reutilizacion y testeo unitario.
- **Alternatives considered:**
  - Renderizar confirmacion dentro de `ReasignarRespuestaModal` como estado interno: descartado por mezclar dos intenciones de UI.
  - Usar `message/success` temporal en lugar de modal: descartado por incumplir requerimiento visual.

### 2) Orquestacion con dos estados de modal
- **Decision:** manejar aperturas con banderas separadas:
  - `isReasignarOpen`
  - `isTramiteReasignadoOpen`
  y un payload de confirmacion.
- **Rationale:** reduce condiciones ambiguas y evita colision de render cuando un modal cierra y otro abre.
- **Alternatives considered:**
  - Un solo estado con enum complejo: posible, pero menos legible para este alcance.

### 3) Contrato tipado estricto para datos de confirmacion
- **Decision:** props obligatorias en `TramiteReasignadoModal`:
  - `open: boolean`
  - `usuarioAsignado: string`
  - `radicado: string`
  - `onClose: () => void`
- **Rationale:** mantiene TypeScript estricto y evita `any` o dependencias implícitas del estado padre.
- **Alternatives considered:**
  - Pasar un objeto opcional completo y resolver nulos dentro del modal: descartado por mayor complejidad y ramas.

### 4) Foco inicial y accesibilidad centrada en accion primaria
- **Decision:** enfocar el boton `Aceptar` al abrir el modal de confirmacion.
- **Rationale:** mejora cierre rapido por teclado y cumple requerimiento de accesibilidad.
- **Alternatives considered:**
  - Dejar foco en el contenedor modal: descartado por menor eficiencia de teclado.

### 5) Responsive controlado por CSS Module local
- **Decision:** resolver ancho, padding y boton full width en mobile desde `TramiteReasignadoModal.module.css`.
- **Rationale:** evita estilos globales y mantiene encapsulamiento.
- **Alternatives considered:**
  - Ajustes globales de modal: descartado por riesgo de regresion en otros consumidores de `AppModal`.

## Risks / Trade-offs

- [Riesgo] Doble modal visible por sincronizacion incorrecta de estados  
  -> Mitigacion: cerrar `ReasignarRespuestaModal` antes de abrir confirmacion y cubrir con test de integracion.

- [Riesgo] Pérdida de datos de confirmacion al cerrar modal origen  
  -> Mitigacion: persistir payload de confirmacion en estado padre antes de alternar visibilidad.

- [Riesgo] Regresion de accesibilidad en foco y teclado  
  -> Mitigacion: pruebas enfocadas a foco inicial y accion de cierre por boton primario.

- [Trade-off] Componente adicional para una sola pantalla  
  -> Beneficio: claridad de responsabilidades y reutilizacion futura en flujos similares.

## Migration Plan

1. Crear carpeta `modalTramiteReasignado` y componente base.
2. Implementar estilos responsive en CSS Module.
3. Integrar modal de confirmacion en flujo de `ReasignarRespuestaModal`.
4. Ajustar pruebas unitarias e integracion.
5. Verificar comportamiento visual en desktop/tablet/mobile.

Rollback:
- Revertir commit del cambio restablece el comportamiento previo sin afectar contratos backend ni shared components.

## Open Questions

- ¿El nombre mostrado en `Usuario Asignado` se toma del primer usuario seleccionado o de un formato agregado cuando hay multiples?
- ¿Al cerrar confirmacion se mantiene foco/scroll en la misma bandeja o se requiere reset visual adicional?
- ¿Se requiere telemetry de evento `tramite_reasignado_confirmado` en esta fase o queda fuera de alcance?
