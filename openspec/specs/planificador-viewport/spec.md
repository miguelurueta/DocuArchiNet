# planificador-viewport Specification

## Purpose

Define el comportamiento observable del planificador de render del visor PDF, su manejo seguro de errores y la conservación de la experiencia durante scroll, zoom y rotación.

## Requirements

### Requirement: Clasificación saneada de errores
El runner SHALL clasificar fallos de frame principal, visor, worker y recursos opcionales sin persistir información documental o de sesión. <!-- RQ-01; D-01 -->

#### Scenario: Error clasificado
- **WHEN** se recibe `error`, `unhandledrejection`, `pageerror`, respuesta HTTP fallida o fallo de recurso
- **THEN** la evidencia contiene únicamente código estable, origen, tipo, archivo base y línea saneada

#### Scenario: Dato no permitido
- **WHEN** el origen contiene mensaje libre, stack, URL, query, ruta, tarea, usuario, documento, cookie o contenido
- **THEN** el dato no se persiste ni se imprime y una categoría desconocida queda como fallo seguro

### Requirement: Resultado final honesto
El runner SHALL derivar su resultado de los hitos funcionales y errores clasificados. <!-- RQ-02; D-02 -->

#### Scenario: Corrida limpia
- **WHEN** selección, primer canvas, scroll, zoom y recargas terminan sin errores
- **THEN** el resultado es `PASSED`

#### Scenario: Observación opcional
- **WHEN** los hitos terminan y solo existe un fallo conocido de recurso opcional
- **THEN** el resultado es `PASSED_WITH_OBSERVATIONS`

#### Scenario: Fallo funcional
- **WHEN** falta un hito, ocurre un error requerido o existe un error no clasificado
- **THEN** el resultado es `FAILED` y el proceso devuelve código distinto de cero

### Requirement: Cola secuencial y deduplicada
El visor SHALL mantener máximo un trabajo efectivo por página y generación y esperar la promesa real del render. <!-- RQ-03; D-03 -->

#### Scenario: Solicitudes duplicadas
- **WHEN** una página/generación ya está en cola, renderizando o renderizada
- **THEN** no se inicia un segundo `RenderTask` efectivo

#### Scenario: Error o cancelación
- **WHEN** `getPage` o render falla o se cancela
- **THEN** la cola libera su estado en `finally` y continúa con el siguiente trabajo

### Requirement: Generaciones y cancelación
El visor SHALL invalidar trabajo obsoleto e impedir que sus resultados reemplacen contenido vigente. <!-- RQ-04; D-04 -->

#### Scenario: Zoom o rotación rápida
- **WHEN** cambia la generación durante un render
- **THEN** el trabajo anterior se cancela cuando sea posible y su finalización no instala el canvas

#### Scenario: Página deja de ser necesaria
- **WHEN** una página sale del conjunto visible/adyacente
- **THEN** su tarea efectiva se cancela o su resultado queda descartado

### Requirement: Planificación acotada por viewport
El visor SHALL reconciliar únicamente páginas visibles y adyacentes y agrupar ráfagas de eventos. <!-- RQ-05; D-05 -->

#### Scenario: Scroll rápido
- **WHEN** ocurren múltiples eventos de scroll en un frame
- **THEN** se ejecuta como máximo una reconciliación de viewport para ese frame

#### Scenario: Observer no disponible
- **WHEN** `IntersectionObserver` no existe
- **THEN** el visor usa un fallback geométrico coalescido sin cambiar el contrato público

### Requirement: Compatibilidad visual y aislamiento
El visor SHALL conservar API, DOM y estado visual, y aislar el fallo de una página. <!-- RQ-06; D-06 -->

#### Scenario: Interacción conservada
- **WHEN** el usuario navega, amplía o rota
- **THEN** se conservan página activa, posición proporcional, overlays, anotaciones y rotación

#### Scenario: Fallo de página
- **WHEN** una página no puede cargarse o renderizarse
- **THEN** se muestra su estado de error y las demás páginas continúan procesándose

### Requirement: Verificación reproducible
DOC-100 SHALL validarse con pruebas focales, política, sintaxis y evidencia autorizada. <!-- RQ-07; D-07 -->

#### Scenario: Validación local
- **WHEN** se ejecutan pruebas Node y validación sintáctica
- **THEN** clasificación, sanitización, cola, cancelación, generaciones, viewport y compatibilidad terminan sin fallos

#### Scenario: E2E no autorizada
- **WHEN** no existe autorización vigente de ambiente, cuenta y tarea
- **THEN** no se ejecuta la prueba autenticada y se registra el bloqueo sin afirmar que el error real desapareció
