# Producción — Ventana integrada de importación por carpetas

## Purpose

Completar las brechas comprobadas de la ventana integrada de importación y acreditar UI-01/02/03 reutilizando sus componentes y contratos existentes, sin duplicar carga individual o backend.

## Requirements

### Requirement: RQ-01 — Una ventana con destino autorizado estable

El sistema SHALL reutilizar la ventana integrada existente y conservar el nivel capturado al abrir, validado por servidor, aunque se seleccione otro perfil o nodo; preservar la carga individual y el seguimiento ya integrado.

Trazabilidad: D-01, RQ-01.

#### Scenario: Cambio de perfil o nodo
- **WHEN** se abre desde A y luego se selecciona un perfil compatible y el nodo B
- **THEN** la ventana conserva A y su ruta de solo lectura, sin otro árbol, otro selector de destino o redirección del lote

#### Scenario: Apertura repetida
- **WHEN** se inicializa o solicita abrir repetidamente la misma instancia
- **THEN** se conserva una sola ventana y una sola solicitud por acción, sin reemplazar la carga individual

### Requirement: RQ-02 — Respuesta inválida con error recuperable

El sistema SHALL rechazar datos de respuesta incompatibles con la estructura que consume la interfaz antes de mostrarlos o habilitar confirmación; dejar un error accesible y conservar la posibilidad de revisar o cerrar.

Trazabilidad: D-02, RQ-02.

#### Scenario: Incidencia o valor anidado inválidos
- **WHEN** PrepararPlan responde con un sobre válido pero Incidencias contiene null u otro elemento anidado no consumible
- **THEN** se muestra RESPUESTA_INVALIDA o su mensaje seguro, no se confirma el plan y no ocurre una excepción sin manejar ni queda una revisión vacía

#### Scenario: Respuesta tardía o catálogo inválido
- **WHEN** una respuesta llega para una revisión/instancia anterior o contiene perfiles/valores incompatibles
- **THEN** no reemplaza el modelo vigente ni habilita confirmación; la pantalla sigue utilizable sin llamadas repetidas automáticamente

### Requirement: RQ-03 — Ciclo de vida sin eventos duplicados

El sistema SHALL conservar una instancia por host, liberar recursos al retirarlo o cerrar y rechazar respuestas tardías; teclado y foco deben permanecer utilizables durante reapertura y actualización parcial.

Trazabilidad: D-03, RQ-03.

#### Scenario: Mismo host tras actualización
- **WHEN** el ciclo de carga parcial inicializa otra vez un host existente
- **THEN** no duplica listeners, diálogo o solicitudes y conserva el destino capturado de la ventana

#### Scenario: Host retirado durante apertura
- **WHEN** se sustituye o elimina el host con una solicitud pendiente y después llega la respuesta
- **THEN** la instancia anterior se destruye, no reaparece ni modifica el nuevo host y no intenta enfocar un nodo retirado

### Requirement: RQ-04 — Escenarios visuales UI-01 a UI-03 verificables

La entrega SHALL proporcionar recorridos automatizados focales de UI-01, UI-02 y UI-03 sobre el árbol y los componentes reales, con aserciones de UI/HTTP e identificación de efectos autorizados.

Trazabilidad: D-04, RQ-04.

#### Scenario: UI-01
- **WHEN** se selecciona A, se abre desde Producción, se cambia perfil y se navega a B
- **THEN** destino y solicitudes de la instancia permanecen en A; no se duplica ventana ni selector

#### Scenario: UI-02
- **WHEN** se agregan dos raíces homónimas con subcarpetas, se prepara y se cambia el inventario antes de confirmar
- **THEN** identidades y rutas se conservan, se invalida la revisión anterior y solo una nueva preparación válida puede confirmarse cuando esa escritura esté autorizada

#### Scenario: UI-03
- **WHEN** se abre/cierra/reabre, se ejecuta actualización parcial real, se usa teclado y se obtiene un error de servicio
- **THEN** hay una petición por acción, foco recuperado cuando el control sigue vigente y error visible; un resultado simulado se identifica como tal y no acredita el backend real

### Requirement: RQ-05 — Autorización y herramientas compartidas

La entrega SHALL reutilizar la infraestructura E2E existente y exigir autorizaciones específicas del escenario antes de autenticar o persistir recursos; no solicitar secretos por chat ni crear infraestructura o configuración paralelas.

Trazabilidad: D-05, RQ-05.

#### Scenario: Autorización ausente
- **WHEN** faltan aprobaciones de ambiente/cuenta o de preparación/confirmación cuando el caso las requiere
- **THEN** no se inicia ese recorrido real y queda NO_EJECUTADO con motivo, dependencia y responsable

#### Scenario: Efectos de prueba acotados
- **WHEN** se autoriza UI-01/02/03 con recursos concretos
- **THEN** solo se realizan efectos expresamente incluidos, los controles RA/DA son SELECT y no se reciben/almacenan documentos ni se reutilizan aprobaciones DOC-111 por inferencia

### Requirement: RQ-06 — Evidencia suficiente para cierre

La entrega SHALL identificar por escenario la comprobación local o real, SHA y huella probados, comandos y limitaciones, conservando procedencia de DOC-111 y contratos comunes; no cerrar integración real con pruebas sintéticas.

Trazabilidad: D-06, RQ-06.

#### Scenario: Regresión local
- **WHEN** se ejecutan pruebas cliente y validaciones de compilación requeridas
- **THEN** se registran comandos/resultados reales y salidas aisladas; una limitación de herramientas se declara sin atribuir un build moderno o despliegue

#### Scenario: Caso real sin ejecutar
- **WHEN** se revisan resultados para cierre y algún UI-01/02/03 carece de evidencia real requerida
- **THEN** se conserva NO_EJECUTADO/FALLIDO y el bloqueo explícito; el PASS local y las aceptaciones REC históricas no lo sustituyen
