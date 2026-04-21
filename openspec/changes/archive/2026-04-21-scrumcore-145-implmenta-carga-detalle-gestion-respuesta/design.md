## Context

La vista de detalle de `GestionRespuesta` depende de varias piezas que hoy se resuelven de forma parcial o dispersa:

- `idTareaWf` derivado de la ruta.
- Estructura de respuesta obtenida por API (`useEstructuraRespuestaIdTarea`).
- Superficie de edicion (`AppEditor`) y sus estados locales.
- Dependencias de UI relacionadas (metadata de cabecera, pestañas y paneles auxiliares).

El ticket `SCRUMCORE-145` busca consolidar la carga del detalle para que el flujo sea predecible y consistente: primero resolver estructura/contexto, luego habilitar contenido editable y dependencias asociadas.

## Goals / Non-Goals

**Goals:**
- Definir una orquestacion clara de carga para el detalle de `GestionRespuesta`.
- Separar estados de ciclo de vida (`loading`, `ready`, `blocked`) con reglas de render consistentes.
- Garantizar que editor y componentes dependientes solo se activen cuando exista contexto valido.
- Reducir regresiones de render y estados intermedios inconsistentes entre tabs y header.

**Non-Goals:**
- No rediseñar visualmente `AppEditor`.
- No modificar el contrato backend de estructura de respuesta.
- No introducir nuevas librerias de estado global.

## Decisions

### 1) Fuente unica de verdad para estado de carga del detalle
- **Decision:** centralizar en el nivel de ruta/contendor (`GestionCorrespondenciaRoute` + `GestionRespuesta`) la resolucion de estado del detalle.
- **Rationale:** evita que cada componente hijo repita validaciones y estados de carga propios.
- **Alternativas consideradas:**
  - Resolver estado en cada tab por separado: descartado por duplicidad y riesgo de divergencia.
  - Resolver estado solo en servicios: descartado por acoplar datos con decisiones de UI.

### 2) Habilitacion escalonada de dependencias
- **Decision:** activar dependencias por fases:
  1) resolver `idTareaWf` y estructura;
  2) poblar metadata/contexto de cabecera;
  3) habilitar editor y acciones de detalle.
- **Rationale:** evita montar editor en estados sin contexto valido y mejora estabilidad de render.
- **Alternativas consideradas:**
  - Montar editor siempre y “parchar” despues: descartado por parpadeos y estados transitorios inconsistentes.

### 3) Contrato de estados explicitos para componentes hijos
- **Decision:** pasar estados derivados y datos ya normalizados a componentes hijos (sin que los hijos consulten nuevamente estructura).
- **Rationale:** reduce acoplamiento y facilita pruebas de cada estado.
- **Alternativas consideradas:**
  - Que hijos llamen hooks directamente: descartado por dependencia circular de carga y dificultad de testeo.

### 4) Cobertura de pruebas por estado observable
- **Decision:** reforzar pruebas en rutas/tab principal para validar:
  - carga inicial del detalle,
  - ready con contenido operativo,
  - bloqueo por falta de contexto o error.
- **Rationale:** el ticket es de orquestacion de carga; la cobertura debe centrarse en estados visibles y transiciones.

## Risks / Trade-offs

- **[Riesgo] Mayor complejidad condicional en ruta/contendor** → **Mitigacion:** encapsular reglas en helpers/estado derivado tipado.
- **[Riesgo] Regresiones en tests existentes por cambios de timing/render** → **Mitigacion:** ajustar tests para esperar estados finales y evitar supuestos fragiles.
- **[Trade-off] Menos autonomia en componentes hijos** → **Mitigacion:** interfaces explicitas con props derivadas y tipadas.

## Migration Plan

1. Inventariar puntos actuales de carga del detalle (`idTareaWf`, estructura, metadata, editor).
2. Consolidar estado derivado de carga en contenedor principal.
3. Ajustar `GestionRespuestaMainTabContent` para consumir estado/contexto ya resuelto.
4. Validar render de tabs y dependencias auxiliares bajo estados `loading/ready/blocked`.
5. Actualizar pruebas del flujo de detalle y validar `spec:validate` + tests relevantes.

Rollback:
- Revertir cambios de orquestacion en contenedor y restaurar flujo previo de carga distribuida.

## Open Questions

- Se requiere persistir un estado “ultimo detalle valido” para evitar vacios visuales en recargas?
- El comportamiento esperado al cambiar rapidamente entre detalles debe cancelar cargas previas o solo ignorar respuestas tardias?
- Debe existir indicador de reintento explicito en casos de error de carga para este alcance?
