<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
## Purpose

Consolidar la verificación trazable de importación de carpetas sobre la interfaz definitiva, diferenciando evidencia histórica, pruebas focales, límites operativos y autorización de cada ejecución.

## ADDED Requirements

### Requirement: RQ-01 — Base y vigencia por componente
DOC-120 SHALL cumplir la decisión D-01: Fijar el merge de DOC-119 como base funcional. Cada caso conserva SHA/huellas de sus componentes, prueba, ambiente y resultado originales; la identidad de huellas permite reutilización local, no una ejecución real nueva. Un cambio invalida únicamente los casos dependientes; el mapa explicita esa dependencia.

#### Scenario: Código vigente
- **WHEN** se contrasta una evidencia con la versión final
- **THEN** se comparan sus componentes y huellas, conservando SHA/runtime/ambiente de origen
- **AND** solo los casos dependientes de un cambio se revisan o reejecutan

#### Scenario: Cambio posterior
- **WHEN** cambia la interfaz o un componente después de una corrida
- **THEN** se registra el nuevo SHA y el impacto antes de aceptar la versión
- **AND** igualdad de huellas no se presenta como una sesión real nueva

### Requirement: RQ-02 — Matriz e índices únicos
DOC-120 SHALL cumplir la decisión D-02: Crear matriz-verificacion.md y resultados.md en Validacion, ausentes en la base inspeccionada. La matriz relaciona requisito/caso, componente/símbolo, prueba, SHA/huella, ambiente, evidencia, resultado, vigencia y límites. Resultados mantiene cronología y enlaza fuentes primarias sin sobrescribirlas.

#### Scenario: Índices con procedencia
- **WHEN** se incorpora un caso a la matriz
- **THEN** figuran requisito/caso/componente/prueba/SHA/ambiente/evidencia/resultado/vigencia/límite
- **AND** resultados enlaza el primario y preserva su cronología

#### Scenario: Clasificación y conteos
- **WHEN** se reutilizan resultados históricos o locales
- **THEN** se distinguen REAL_HISTORICO, LOCAL_HISTORICO_VIGENTE, LOCAL_DOC120, ESTATICO y NO_EJECUTADO
- **AND** no se suman suites superpuestas ni se etiqueta CI como E2E integral

### Requirement: RQ-03 — Consumo de EXP/ALM/COORD/SEG
DOC-120 SHALL cumplir la decisión D-03: Usar resultados efectivos y sus cortes finales. DOC-116 resolvió su ventana interna mediante variante de laboratorio; DOC-117 acredita ALM-01/02/03C con límites; DOC-118 distingue archivo administrativo de cierre técnico38/38; DOC-119 conserva SEG-01 real y SEG-02 con cobertura mixta aceptada. No sumar suites de cortes distintos ni reproducir efectos inciertos retenidos.

#### Scenario: Resoluciones posteriores
- **WHEN** un informe contiene bloqueos históricos y una resolución posterior
- **THEN** se identifica el corte efectivo con su fuente primaria y variante
- **AND** el archivo administrativo DOC-118 no sustituye su cierre técnico38/38

#### Scenario: Efecto incierto conservado
- **WHEN** un intento permanece EN_CURSO/público INCIERTO
- **THEN** se conserva con sus identidades y límites
- **AND** no se repite almacenamiento ni se borra o edita su checkpoint para obtener PASS

### Requirement: RQ-04 — Interfaz definitiva y regresión focal
DOC-120 SHALL cumplir la decisión D-04: Mapear UI01–05 al menú superior/contextual y asistente resultantes de DOC-119. Cubrir seleccionado/pulsado, homónimos, selección inválida, autoridad denegada/revocada, referencia/último lote/confirmación sin acuse, recarga/reapertura/UpdatePanel, árbol, foco y viewport. Reutilizar fuentes vigentes y ejecutar solo casos afectados o brechas documentadas.

#### Scenario: Destino y autoridad
- **WHEN** se valida acceso superior o contextual en la UI definitiva
- **THEN** se conserva seleccionado frente a pulsado, ruta autorizada y destino fijo
- **AND** nodos excluidos y autoridad denegada no provocan fallback

#### Scenario: Recuperación y avisos
- **WHEN** se consulta referencia/último lote o confirmación sin acuse tras recarga/postback
- **THEN** se conserva identidad, acceso equivalente, avisos, árbol/selección y foco
- **AND** no se reconfirma automáticamente ni se atribuye un caso sintético a IIS

### Requirement: RQ-05 — Integridad, incertidumbre y consumidores
DOC-120 SHALL cumplir la decisión D-05: Relacionar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, valores diferidos y fecha estable, revocación, stop e incertidumbre con casos del arnés existente. Comprobar contrato del plugin en sus26 registros/22archivos y carga individual pertinente. Un callback sintético no demuestra la acción de negocio real de cada módulo.

#### Scenario: Integridad y valores
- **WHEN** se evalúan escenarios completos/parciales, valores diferidos y fecha estable
- **THEN** cada regla tiene caso y evidencia de las barreras/revisiones existentes
- **AND** recepción y efecto permanecen separados y stop no promete rollback

#### Scenario: Consumidores y carga individual
- **WHEN** se consume la prueba del contextual compartido y la carga individual
- **THEN** se conserva recurso invocador, teclado/viewport y alcance probado por consumidor
- **AND** los26 contratos sintéticos no se anuncian como26 operaciones de negocio reales

### Requirement: RQ-06 — INT-02 degradación acotada
DOC-120 SHALL cumplir la decisión D-06: Conservar el caso real histórico de ausencia temporal de ra_integracion_evento con restauración y carga individual, sin extrapolar a cada incompatibilidad. Contrastar los verificadores/esquema y el guard de indisponibilidad actuales. Exigir variante nueva solo si un requisito concreto y una diferencia reproducida la hacen necesaria; registrar NO_NECESARIA con justificación en los demás casos.

#### Scenario: Degradación histórica delimitada
- **WHEN** se reutiliza la ausencia de ra_integracion_evento
- **THEN** se conserva alcance de indisponibilidad/restauración/carga individual
- **AND** no se extrapola a toda incompatibilidad ni se reinstala SQL

#### Scenario: Necesidad de variante
- **WHEN** se encuentra una incompatibilidad exigida sin evidencia suficiente
- **THEN** se documenta reproducción, requisito, restauración y autorización para la variante concreta
- **AND** si no es necesaria consta NO_NECESARIA justificada sin modificar el esquema

### Requirement: RQ-07 — INT-03 capacidad con presupuesto definido
DOC-120 SHALL cumplir la decisión D-07: Antes de medir acordar objetivo, cantidad/distribución/tamaño de archivos y raíces, navegadores/versiones, concurrencia, duración, ambiente, recursos, disco temporal/libre y umbrales de tiempo/memoria. Incluir topes HTTP/SELECT/sesiones/bytes/efectos y criterio de parada. Sin esos acuerdos, INT-03 queda NO_EJECUTADO por falta de definición/autorización.

#### Scenario: Contrato previo
- **WHEN** se propone medir INT-03
- **THEN** existen objetivo, corpus, navegadores/versiones, concurrencia, disco, umbrales y topes de ejecución revisados
- **AND** una medición solo afirma el presupuesto y ambiente observados

#### Scenario: Sin definición o autorización
- **WHEN** faltan objetivo, umbrales o autorización específica de capacidad
- **THEN** INT-03 consta NO_EJECUTADO con motivo, responsable e impacto operativo
- **AND** no se infiere capacidad máxima de páginas, lotes pequeños o pruebas locales

### Requirement: RQ-08 — Autorización separada y herramientas existentes
DOC-120 SHALL cumplir la decisión D-08: La continuación autoriza refinamiento/inspección y preparación local. Antes de ejecutar cada ventana real exigir ambiente, cuentas, recursos, operaciones, presupuestos y recuperación revisados. Reutilizar autenticación, runners y queryRows existentes; controles RA/DA exclusivamente SELECT y secretos efímeros. Autorizaciones DOC110–119 no se heredan.

#### Scenario: Ventana real concreta
- **WHEN** se pretende autenticar, consultar BD, cargar o inyectar fallos
- **THEN** se exige autorización de ambiente/cuentas/recursos/operaciones/presupuestos/recuperación
- **AND** los controles RA/DA usan solo SELECT y las credenciales permanecen efímeras

#### Scenario: Resultado detenido
- **WHEN** una ventana real falla o consume su presupuesto
- **THEN** se conserva STOPPED y su consumo sin retry automático
- **AND** no se reutiliza una autorización anterior para ampliar efectos

### Requirement: RQ-09 — Fuentes y decisiones protegidas
DOC-120 SHALL cumplir la decisión D-09: Conservar SII/Integracionccv, AlmacenamientoDocumentoProduccionDocumental, Web.config, esquema, semántica de recepción/efectos y PDF/rechazo TIFF. Mantener decisión expresa SEG-03 de operar sin limpieza y datos retenidos. No añadir jobs, cuotas, formatos, endpoints o aislamiento XML adicional.

#### Scenario: Fuentes y formatos
- **WHEN** se consolida o corrige una brecha de DOC-120
- **THEN** se preservan SII/almacenamiento compartido/configuración/esquema y PDF con rechazo TIFF
- **AND** no se habilitan jobs/cuotas/limpieza/aislamiento XML adicional

#### Scenario: Decisión sin limpieza
- **WHEN** se consume SEG-03 de DOC-119
- **THEN** se conserva operación sin limpieza y datos retenidos
- **AND** la medición histórica de disco no se anuncia como actual ni como borrado real

### Requirement: RQ-10 — Defectos reproducidos y criterio de cierre
DOC-120 SHALL cumplir la decisión D-10: Modificar producto solo ante una brecha reproducible vinculada a requisito, causa y prueba de regresión. Registrar cada no ejecutado con motivo, responsable e impacto operativo. Autoridad/destino/integridad/recuperación con fallo crítico bloquean salida. Límites necesitan aceptación expresa; archivo/CI/checklists no sustituyen aceptación integral.

#### Scenario: Brecha reproducible
- **WHEN** se detecta un defecto de autoridad, destino, integridad o recuperación
- **THEN** se registra causa/caso/corrección/regresión y se bloquea salida mientras sea crítico
- **AND** no se desarrolla producto por ausencia de un checklist genérico

#### Scenario: Aceptación y cierre
- **WHEN** se propone declarar lista la versión
- **THEN** la matriz identifica cada no ejecutado y su motivo, responsable e impacto; los límites tienen aceptación expresa
- **AND** archivo, CI y número de tareas no sustituyen aceptación operativa ni merge verificado
