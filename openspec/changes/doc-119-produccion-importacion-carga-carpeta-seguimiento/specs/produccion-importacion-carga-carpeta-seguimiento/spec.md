<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
## ADDED Requirements

### Requirement: RQ-01 — Procedencia de aceptación
El expediente de DOC-119 SHALL distinguir evidencia histórica, inspección y pruebas nuevas con SHA, ambiente, escenario y límites según D-01.

#### Scenario: Cierre técnico posterior al administrativo
- **WHEN** se contrasta DOC-118 como antecedente
- **THEN** se preserva su archivo administrativo y se registra el cierre técnico posterior del JSON de integración y reporte acumulado
- **AND** ningún resultado histórico se presenta como aceptación SEG-01/02 de DOC-119

#### Scenario: Cliente con servicios sintéticos
- **WHEN** se ejecuta Chromium con dobles locales
- **THEN** se informa alcance local sin acreditar autenticación, IIS, MySQL o efectos reales

### Requirement: RQ-02 — Lectura y CSV por página
El seguimiento SHALL consultar filtros/páginas autorizados y exportar una página con los mismos filtros, conservando totales y autoridad sin efectos según D-02.

#### Scenario: Estados mixtos y filtros
- **WHEN** el propietario autorizado filtra por raíz, estado o recepción y recorre páginas
- **THEN** totalOriginal conserva inventario confirmado y totalFiltrado representa el filtro, manteniendo IDs y estados separados de recepción/efecto
- **AND** rechazados/inciertos no desaparecen del inventario ni RECIBIDO se presenta como DOCUMENTO_ALMACENADO

#### Scenario: Exportación representada
- **WHEN** se descarga CSV de la página consultada con filtros vigentes
- **THEN** se exportan solo sus filas, neutralizando fórmulas y preservando comillas, separadores y saltos de línea
- **AND** no se anuncia exportación total, snapshot global ni se exponen campos privados

#### Scenario: Lectura sin reconciliación
- **WHEN** se consulta lote, recursos, confirmación o CSV
- **THEN** no se avanza, reconcilia, confirma, recibe ni altera estado/auditoría de negocio
- **AND** los controles reales autorizados usan únicamente SELECT y conservan huellas

#### Scenario: Identidad ajena o permiso revocado
- **WHEN** otra identidad consulta/exporta un lote o el propietario pierde autoridad vigente
- **THEN** se rechaza la operación sin filas expuestas ni efectos

### Requirement: RQ-03 — Recuperación por identidad
El sistema SHALL recuperar referencias sin repetir efectos y actualizar la rama original por IDs confirmados, conservando destino/selección según D-03.

#### Scenario: Recarga con confirmación sin acuse
- **WHEN** se recarga tras perder respuesta de ConfirmarPlan
- **THEN** se consultan pistas de plan/lote aisladas por contexto sin volver a confirmar
- **AND** ausencia de resultado verificable mantiene bloqueo y un lote recuperado conserva destino original

#### Scenario: Recepción incompleta y homónimos
- **WHEN** se reselecciona archivo sin recibir entre raíces con nombres iguales
- **THEN** se elige por recursoId/raizId y metadatos permitidos; bytes/hash/autoridad se validan en servidor
- **AND** no se reemplazan recibidos, inciertos ni recursos en vuelo por coincidencia de nombre

#### Scenario: Rama original con otra selección
- **WHEN** la consulta retorna relaciones confirmadas y está seleccionado otro nodo
- **THEN** se agregan solo IDs ausentes a la rama original disponible sin expandirla ni mover selección
- **AND** identidades homónimas permanecen distintas y rama oculta/ausente no invalida el lote

### Requirement: RQ-04 — Limpieza condicionada
DOC-119 SHALL mantener limpieza deshabilitada y datos conservados sin resolución operativa expresa según D-04.

#### Scenario: Sin decisión de retención
- **WHEN** no hay política/mecanismo/autorización de limpieza acordados
- **THEN** no se habilita borrado, plazo, endpoint o job ni se eliminan staging, partes o historial
- **AND** SEG-03 continúa condicionado/no ejecutado

#### Scenario: Operación acordada sin limpieza
- **WHEN** el responsable decide expresamente operar sin limpieza
- **THEN** se documentan límite y consumo de disco sin presentar SEG-03 como ejecución real de borrado

### Requirement: RQ-05 — Menú superior y permisos independientes
La interfaz SHALL ofrecer accesos operativos en Importación sin depender de administrar perfiles y mantener separación de permisos según D-05.

#### Scenario: Importador sin administración
- **WHEN** un actor autorizado para importar no puede administrar perfiles
- **THEN** dispone de Importar carpetas… y Consultar importación… sin acceso administrativo concedido

#### Scenario: Menú existente y postback
- **WHEN** ya existe menú administrativo y se reinicializa por postback
- **THEN** accesos operativos/administrativos se integran una sola vez con separación visual y permisos actuales

### Requirement: RQ-06 — Destino explícito
La interfaz SHALL diferenciar nivel seleccionado de pulsado y mostrar destino reautorizado por servidor según D-06.

#### Scenario: Acceso superior
- **WHEN** se invoca importar desde menú superior con nivel elegible seleccionado
- **THEN** se solicita ese nivel y se muestran nivel/ruta canónica autorizados
- **AND** ausencia de selección elegible impide iniciar

#### Scenario: Contextual de otro nivel
- **WHEN** está seleccionado A y se invoca Importar carpetas aquí… sobre B
- **THEN** se solicita B incluso si sus nombres coinciden y se reautoriza en servidor
- **AND** cambiar selección después de abrir no modifica silenciosamente destino

#### Scenario: Nodo excluido o permiso revocado
- **WHEN** el menú se abre en expediente, documento, fondo vacío o nivel sin autoridad vigente
- **THEN** no se importa por fallback en otro nodo seleccionado
- **AND** revocación posterior a mostrar acceso se rechaza en servidor

#### Scenario: Consulta contextual
- **WHEN** se consulta referencia de lote desde contexto de nivel
- **THEN** se respeta autoridad/contexto existentes y destino propio del lote
- **AND** no se añade historial nuevo ni contrato de búsqueda por nivel

### Requirement: RQ-07 — Asistente y recuperación preservados
La reorganización SHALL reutilizar asistente único y conservar referencia manual, último lote, verificación y avisos según D-07.

#### Scenario: Retirada de franja
- **WHEN** se retiran controles permanentes
- **THEN** ya existe acceso equivalente verificado a consulta manual, último lote y verificación de confirmación sin acuse, manteniendo aviso visible

#### Scenario: Cierre, recarga y UpdatePanel
- **WHEN** se cierra, reabre, recarga o realiza postback
- **THEN** no se duplican instancias/listeners y se preservan pistas y recuperación
- **AND** cerrar no promete cancelar servidor ni conservar File tras recarga

### Requirement: RQ-08 — Contextual visible y accesible
El menú SHALL funcionar dentro del viewport y por teclado conservando consumidores compartidos según D-08.

#### Scenario: Bordes y altura reducida
- **WHEN** se abre cerca del borde o excede altura disponible
- **THEN** posición/altura se ajustan al viewport y scroll permite alcanzar todas las opciones

#### Scenario: Teclado y foco
- **WHEN** se abre por teclado y se navega o se pulsa Escape
- **THEN** opciones son accesibles y el cierre devuelve foco al invocador sin ejecutar otra acción

#### Scenario: Plugin compartido modificado
- **WHEN** cambia jquery.contextMenu.js o su CSS
- **THEN** se inventarían y verifican consumidores afectados conservando apertura/cierre/acción sobre su recurso original

### Requirement: RQ-09 — Secuencia y guía definitiva
La entrega SHALL aceptar seguimiento antes de reorganizar y verificar regresión/guía definitiva según D-09.

#### Scenario: Orden operativo y UI
- **WHEN** se inicia UI-01–04
- **THEN** SEG-01/02 tienen aceptación integrada documentada y brechas de seguimiento reproducidas están resueltas
- **AND** UI-05 y guía se verifican en DOC-119 antes de consolidar DOC-120

#### Scenario: Matriz de regresión
- **WHEN** se evalúan accesos reorganizados
- **THEN** se cubren seleccionado/pulsado, homónimos, sin selección, permisos denegados/revocados, reapertura/postback/recarga, consulta/CSV/recuperación y foco/teclado
- **AND** guía distingue lecturas de efectos, incertidumbre y detención de DOC-118; carga individual pertinente sigue operativa

### Requirement: RQ-10 — Autorización y límites
La ejecución SHALL respetar autorizaciones específicas y límites de producto/datos según D-10.

#### Scenario: Continuación de refinamiento
- **WHEN** solo se ha autorizado continuar el refinamiento
- **THEN** no se autentica ni ejecuta pruebas reales/carga/fallos/permisos/limpieza
- **AND** no se sustituye aceptación integrada por antecedentes

#### Scenario: Cambio acotado
- **WHEN** se corrige brecha reproducida o reorganizan accesos
- **THEN** se reutilizan contratos/servicios/coordinador sin cambiar SII/almacenamiento compartido/esquema/semántica operativa
- **AND** se preservan PDF/rechazo TIFF, snapshot/inventario/efectos e integridad EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, sin rollback global, workers o aislamiento XML adicional
