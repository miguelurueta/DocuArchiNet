# DOC-116–120 — revisión arquitectónica del alcance pendiente

Fecha: 2026-11-04. Actualización de las cinco descripciones de Jira verificada por lectura posterior. Se conservaron títulos y estados «Por hacer». Sin comentarios enviados, nuevas cargas, pruebas autenticadas, cambios de producto, permisos ni configuración. El texto original de cada descripción permanece en el historial de Jira; se conservaron además copias locales antes de escribir.

## Conclusión y orden recomendado

Los cinco tickets no representan cinco componentes por construir. DOC-111 entregó adaptadores, coordinación y seguimiento; DOC-113–115 ampliaron aceptación específica. Ejecutar revisión de cobertura DOC-116/117, concentrar integración de fallos y concurrencia en DOC-118, completar operación DOC-119 y consolidar DOC-120. Cada cierre requiere su evidencia; no hay cierre automático por existencia de código.

## Base y límites de la revisión

Base entregada: merge PR125 `a3c7263763a89901ba78d05ccebc284a43a4cd76`. Se verificaron 27 referencias de rutas de implementación y 23 referencias de informes específicos; los informes transversales son adicionales. Las pruebas históricas mantienen su SHA y alcance: esta revisión no vuelve a ejecutarlas ni declara probado el commit actual mediante una corrida nueva. Código y evidencia se contrastaron en la copia aislada de entrega para evitar atribuir cambios ajenos del directorio principal.

Se conservan PDF, snapshot y barreras por EXPEDIENTE/LOTE o PARCIAL/NO_APLICA. No tocar almacenamiento legacy/SII, implementar TIFF/Multi-TIFF, reabrir aislamiento XML adicional descartado ni habilitar limpieza/worker por inferencia. La convivencia externa no se presenta como exclusión global. Limpieza real queda condicionada a política y autorización; la composición actual la deshabilita por defecto, sin asumir instaladas las claves mencionadas en un informe histórico.

## Resultado por ticket

### [DOC-116](https://contasoftcompany.atlassian.net/browse/DOC-116) — Revisión de cobertura del adaptador de expedientes existente; cerrar solo brechas de contrato y aceptación. No implementar otro creador.

**Estado Jira:** Por hacer. Descripción leída y verificada tras actualización.

**Ya implementado:** Crear recibe contexto/raíz/lote/intento; usa autoridad y nivel fijados, inicio durable, operación legacy de conservación, captura de IDs y checkpoint en el repositorio único. El coordinador exige barrera y original verificado. Verificación por expediente/relación/ciclo Archivo Gestión, sin atribución por nombre. Fuentes diferidas se materializan después de confirmar el expediente. DOC111 ejecutó REC EXPEDIENTE/LOTE/PARCIAL con dos raíces y comprobación de IDs/relaciones; no equivale a los negativos EXP02/03.

**Escenarios y brechas:**

- EXP-01 — COBERTURA PARCIAL REAL/LOCAL: dos raíces bajo nivel7 e IDs/relaciones tienen evidencia REC; cambio de selección A→B y destino fijo tienen pruebas locales/UI. Verificar la combinación exacta en el recorrido autenticado antes de marcar EXP01 completo; reutilizar la parte ya probada.

- EXP-02 — LOCAL, aceptación específica pendiente: fallo tras creación y antes de checkpoint, respuesta perdida/IDs incompletos y reejecución de la misma raíz. La prueba debe reconciliar solo evidencia fiable o conservar INCIERTO sin otra creación; nunca buscar por nombre.

- EXP-03 — LOCAL, aceptación específica pendiente: revocar permiso del nivel antes de crear, comprobar rechazo y cero expedientes nuevos con SELECT. La revocación probada en DOC114 para PrepararPlan no sustituye este efecto.

- Contrato adicional — mapear homónimos en ámbitos distintos, códigos inválidos/límites funcionales, nivel movido y tipos/clasificación a las pruebas Framework existentes. Ejecutar solo lo no acreditado o afectado por un cambio. Caracterizar límites de coexistencia con creación manual sin prometer exclusión global.

**Cierre exigido:** Entregar matriz criterio→código→prueba→SHA→estado; cubrir EXP01–03 o registrar explícitamente dependencia/no ejecución. No cerrar por existir el archivo ni exigir nuevas tablas/endpoint. Solo corregir defectos demostrados dentro de este adaptador.

**Rutas existentes:**

- `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb`

- `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.Legacy.vb`

- `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Creacion.vb`

- `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.ResultadoCreacion.vb`

**Evidencia específica:**

- [doc111-adaptador-creacion-local-20261029.md](doc111-adaptador-creacion-local-20261029.md)

- [doc111-fase-creacion-local-20261029.md](doc111-fase-creacion-local-20261029.md)

- [doc111-verificacion-expediente-local-20261028.md](doc111-verificacion-expediente-local-20261028.md)

- [doc111-rec-expediente-pass-20261101.md](doc111-rec-expediente-pass-20261101.md)

### [DOC-117](https://contasoftcompany.atlassian.net/browse/DOC-117) — Revisión focal del adaptador de almacenamiento/incorporación existente y su verificador; conservar la función compartida intacta.

**Estado Jira:** Por hacer. Descripción leída y verificada tras actualización.

**Ya implementado:** Comando explícito y valores efectivos verificados; copia consumible privada con nombre original, retención del staging, llamada legacy única, IDs documento/inventario capturados y comprobación de gabinete/expediente/índices/incorporación. Errores posteriores conservan INCIERTO sin replay. REC reales verificaron PDFs, nombres, índices, IDs y repetición/reapertura; lote2 tuvo reconciliación del documento16204 sin volver a almacenarlo. Esta evidencia no acredita todos los negativos ni concurrencia externa.

**Escenarios y brechas:**

- ALM-01 — COBERTURA PARCIAL REAL/LOCAL: índices, tipología, nombre/extensión, vínculo e inventario tienen REC real; ceros/homónimos y guards están caracterizados localmente. DOC114 probó dos formatos en preparación; eso no acredita por sí solo su almacenamiento final. Completar únicamente combinaciones sin evidencia.

- ALM-02 — COBERTURA PARCIAL REAL/LOCAL: lote2 acredita conservación del mismo documento y reconciliación; el fallo inyectado exacto después de almacenar/antes de checkpoint y sus ventanas requieren vincular pruebas locales e integración faltante. Con IDs insuficientes o evidencia discordante, conservar INCIERTO. Si falta incorporación no existe licencia para volver a almacenar ni inventar un reparador: comprobar la vía soportada y reportar bloqueo cuando no sea segura.

- ALM-03 — NO ACREDITADO COMO CONCURRENCIA REAL: la carga individual durante degradación de DOC111 no equivale a importación y carga individual simultáneas. Si se requiere probar coexistencia, acotar a sesión, copia/original y relaciones con recursos autorizados. No reabrir el aislamiento XML descartado ni declarar SII concurrente validado; cualquier escenario SII requiere alcance y autorización separados.

- Negativos — reutilizar pruebas de campo ajeno al gabinete, tipología inexistente, relación incorrecta, nombre inseguro, staging alterado, pérdida de respuesta y checkpoint fallido. Mantener rechazo explícito TIFF/Multi-TIFF; no ampliar formatos.

**Cierre exigido:** Matriz ALM01–03 con procedencia real/local y límites, llamada legacy protegida, original conservado y ausencia de doble almacenamiento. Ninguna corrección preventiva de la función legacy o del XML. Integraciones no autorizadas permanecen NO_EJECUTADO y no se convierten en PASS.

**Rutas existentes:**

- `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb`

- `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.Legacy.vb`

- `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb`

- `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Documento.vb`

- `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Documento.vb`

**Evidencia específica:**

- [doc111-adaptador-documento-local-20261030.md](doc111-adaptador-documento-local-20261030.md)

- [doc111-fase-documento-local-20261030.md](doc111-fase-documento-local-20261030.md)

- [doc111-valores-diferidos-local-20261030.md](doc111-valores-diferidos-local-20261030.md)

- [doc111-rec-lote-pass-20261102.md](doc111-rec-lote-pass-20261102.md)

- [doc111-lote2-verificacion-pass-20261102.md](doc111-lote2-verificacion-pass-20261102.md)

### [DOC-118](https://contasoftcompany.atlassian.net/browse/DOC-118) — Validar y reforzar solo brechas reproducidas de coordinación/recuperación existente. Prioridad: efectos inciertos, dos clientes y detención segura.

**Estado Jira:** Por hacer. Descripción leída y verificada tras actualización.

**Ya implementado:** Una fase por recurso/petición; barreras por EXPEDIENTE/LOTE y parcial; reclamaciones, revisiones, consolidación desde inventario original, detención y reconciliación por evidencia conocida. REC reales cubrieron los tres modos y replays; lote2 acredita cierre de reserva sin inicio y reconciliación documental. DOC115 añade concurrencia real de confirmación/recepción con conexiones físicas independientes; NO acredita concurrencia de escritores documentales ni una llamada legacy en vuelo.

**Escenarios y brechas:**

- COORD-01 — COBERTURA REAL: reutilizar los REC EXPEDIENTE/LOTE/PARCIAL y sus pruebas de barrera/inventario. Contrastar el caso exacto A lista/B pendiente con la evidencia antes de cerrarlo; no repetir los recorridos completos ya aprobados por defecto.

- COORD-02 — BRECHA PRIORITARIA: dos clientes/sesiones independientes solicitan avance documental, respuesta perdida y reclamación vencida mientras la primera llamada sigue activa. Debe haber un solo efecto; vencimiento jamás prueba ausencia; INCIERTO bloquea otra ejecución hasta verificación fiable. Definir puntos de fallo autorizados sin modificar el almacenamiento compartido. Los dobles locales y la recepción 07A no sustituyen esta integración.

- COORD-03 — BRECHA PRIORITARIA: detención durante fase activa, cierre de navegador y consulta desde otra sesión autorizada. Conservar efecto confirmado/IDs/snapshot; impedir nuevas fases afectadas y verificar qué reanudación soporta realmente el contrato. No inventar un endpoint de reanudar ni continuidad desatendida.

- Seguridad y disponibilidad — mapear pruebas existentes de permiso revocado, destino cambiado, revisión obsoleta y publicación nueva. La reanudación conserva snapshot; consulta/verificación siguen exigiendo autoridad. Completar solo negativos no acreditados en la fase correspondiente.

**Cierre exigido:** Acreditar exclusión o bloqueo seguro de efectos documentales, detención en vuelo y consolidación exacta; documentar estados no resueltos y recuperación permitida. No duplicar máquina de estados, repositorio, motor o worker. No declarar completitud si persisten efectos inciertos.

**Rutas existentes:**

- `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.vb`

- `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Barreras.vb`

- `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Consolidacion.vb`

- `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Reconciliacion.vb`

- `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Operaciones.vb`

- `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Reconciliacion.vb`

**Evidencia específica:**

- [doc111-barreras-creacion-local-20261029.md](doc111-barreras-creacion-local-20261029.md)

- [doc111-consolidacion-local-20261030.md](doc111-consolidacion-local-20261030.md)

- [doc111-reconciliacion-local-20261030.md](doc111-reconciliacion-local-20261030.md)

- [doc111-reserva-detencion-local-20261028.md](doc111-reserva-detencion-local-20261028.md)

- [doc115-07a-resultados.md](doc115-07a-resultados.md)

### [DOC-119](https://contasoftcompany.atlassian.net/browse/DOC-119) — Validación operativa del seguimiento existente: lectura/CSV, recuperación del cliente y árbol. Limpieza permanece condicionada a una decisión explícita de retención y habilitación.

**Estado Jira:** Por hacer. Descripción leída y verificada tras actualización.

**Ya implementado:** Consulta autorizada, filtros/páginas, totales originales, CSV seguro por página, recuperación por IDs/sessionStorage, reselección y actualización por relación/ID sin cambiar selección. Consultar/exportar no ejecuta reconciliación. DOC113 acredita apertura, preparación, foco/teclado/postback/reapertura; no acredita todos los estados del seguimiento. Limpieza interna existe con diario y verificación; la composición actual usa por defecto política deshabilitada y permite inyección explícita. No hay endpoint, botón ni job de borrado. El informe local histórico menciona claves de configuración: no asumirlas instaladas ni añadirlas a Web.config.

**Escenarios y brechas:**

- SEG-01 — LOCAL + consultas reales REC: falta acreditar el recorrido integrado de filtros/paginación/descarga CSV y usuario ajeno con estados mixtos. Verificar lectura sin mutación con SELECT, totalOriginal separado de totalFiltrado, recibido frente a almacenado y fórmulas CSV neutralizadas. La exportación vigente es de una página; no presentarla como exportación total.

- SEG-02 — COBERTURA PARCIAL: REC acredita reapertura por IDs; navegador local acredita reselección, homónimos y árbol con helper real/servicio sintético. Completar recarga autenticada con recepción pendiente y actualización deduplicada del nivel original, conservando selección. Metadatos no prueban igualdad: bytes/hash/autoridad se verifican en servidor.

- SEG-03 — CONDICIONADO, NO_EJECUTADO REAL: limpieza local protege activos/inciertos/permanentes y solo retira el original .bin elegible, sin poda de .part/historial. Antes de ejecución real se requiere política de retención, mecanismo operativo interno concreto y autorización específica. Si se acuerda operación sin limpieza habilitada, documentar ese límite y consumo de disco; no marcar SEG03 ejecutado ni inventar plazo/borrado automático.

- Guía operativa — actualizar pasos de consulta, CSV paginado, selección local pendiente, incertidumbre, detención y recuperación de DOC118. Separar comandos de lectura de efectos. No crear un segundo coordinador ni un planificador web.

**Cierre exigido:** Cerrar SEG01/02 con evidencia integrada y resolver explícitamente el alcance/estado de SEG03. Registrar discrepancias documentales contra código actual. Mantener resultados y temporales de pruebas conservados; este ticket no autoriza su eliminación.

**Rutas existentes:**

- `Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb`

- `Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.Limpieza.vb`

- `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionLimpiezaOriginalImportacion.vb`

- `js/gestion/ImportacionCarpetas/ImportacionCarpetas.js`

- `js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js`

- `js/gestion/WebFormProducionDocumental.js`

**Evidencia específica:**

- [doc111-consulta-exportacion-local-20261028.md](doc111-consulta-exportacion-local-20261028.md)

- [doc111-recuperacion-local-20261028.md](doc111-recuperacion-local-20261028.md)

- [doc111-cliente-rama-local-20261030.md](doc111-cliente-rama-local-20261030.md)

- [doc111-limpieza-local-20261030.md](doc111-limpieza-local-20261030.md)

### [DOC-120](https://contasoftcompany.atlassian.net/browse/DOC-120) — Consolidación de verificación integral y brechas transversales; no repetir desarrollo ni todas las cargas de DOC110–119.

**Estado Jira:** Por hacer. Descripción leída y verificada tras actualización.

**Ya implementado:** DOC111: REC EXPEDIENTE/LOTE/PARCIAL, IDs/índices y replays, recuperación real del lote2, degradación de una tabla y carga individual. DOC113: UI01/02/03. DOC114: PLAN01/02/03 con permisos restaurados. DOC115: RECIB01/02/03 y 07A con conexiones independientes. Estas evidencias conservan su alcance: no acreditan capacidad máxima, limpieza real, SII real ni todos los escenarios de efectos en vuelo.

**Escenarios y brechas:**

- INT-01 — consolidar matriz única requisito→caso→componente→prueba→SHA/ambiente→resultado real/local/estático/NO_EJECUTADO. Reutilizar evidencia verificada y consumir resultados de EXP/ALM/COORD/SEG; resolver faltantes críticos y contradicciones. No heredar PASS solo por pertenecer al mismo módulo ni ejecutar ciegamente todos los prompts.

- INT-02 — REUTILIZABLE CON LÍMITE: degradación DOC111 probó ausencia de ra_integracion_evento, recuperación y carga individual. Cubrir solo variantes de incompatibilidad realmente exigidas sin extrapolar una tabla a todos los fallos de esquema. SQL manual y estado aplicado verificado; no reinstalar migraciones existentes.

- INT-03 — PENDIENTE DE DEFINICIÓN/AUTORIZACIÓN: acordar cantidad/tamaño de archivos, navegadores, concurrencia, disco temporal y umbrales de tiempo/memoria antes de medir. Ejecutar únicamente presupuesto aprobado. Sin objetivo o ambiente, registrar pendiente y no prometer capacidad máxima.

- Regresión focal — confirmar matriz completa/parcial, valores diferidos/fecha estable, revocación, detención e incertidumbre con las brechas de DOC116–119; revisar diff/SII y carga individual pertinente. SII real no ejecutado debe permanecer declarado. Mantener fuera el aislamiento XML adicional descartado y los formatos no soportados.

- Actualidad de evidencia — comparar huellas de componentes; un cambio posterior invalida solo los casos afectados. CI opsxj/OpenSpec es complementario y no ejecuta todos los E2E/documentos. No atribuir las 93 pruebas Node ni 2 Framework de DOC115 a una corrida integral nueva.

**Cierre exigido:** Mantener Validacion/matriz-verificacion.md y Validacion/resultados.md como índices de evidencia existente; evitar otra máquina o plataforma de pruebas. Cada no ejecutado debe tener motivo, responsable y efecto sobre la salida a operación. No declarar listo si quedan brechas críticas de autoridad/integridad/recuperación; los límites aceptados deben quedar explícitos.

**Rutas existentes:**

- `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-matriz-final-20261102.md`

- `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc115-verificacion-final-20261104.md`

- `tools/e2e/tests/doc111-importacion-carpetas.spec.cjs`

- `tools/e2e/tests/doc113-ventana-importacion.spec.cjs`

- `tools/e2e/tests/doc114-plan-importacion.spec.cjs`

- `tools/e2e/tests/doc115-recepcion-importacion.spec.cjs`

**Evidencia específica:**

- [doc111-matriz-final-20261102.md](doc111-matriz-final-20261102.md)

- [doc111-cierre-tecnico-20261102.md](doc111-cierre-tecnico-20261102.md)

- [doc113-verificacion-final-20261103.md](doc113-verificacion-final-20261103.md)

- [doc114-final-report-20261103.md](doc114-final-report-20261103.md)

- [doc115-verificacion-final-20261104.md](doc115-verificacion-final-20261104.md)

## Trazabilidad de la actualización

Se cambiaron únicamente las descripciones. Se leyó cada ticket antes de escribir y se comparó con la revisión inicial; las descripciones originales se respaldaron antes de cualquier mutación. Cada PUT quedó registrado y su resultado se volvió a leer/comparar. Resultado: cinco descripciones verificadas, cero cambios de estado y cero comentarios. Diario y copias operativas locales: `.tmp/doc116-120-jira-update-journal.json`, `.tmp/doc116-120-jira-before.json` y `.tmp/doc116-120-jira-update-result.json`. Estos archivos operativos no contienen credenciales y no se publicaron.
