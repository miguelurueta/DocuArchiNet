<!-- opsxj:refinement version=1 state=draft -->

# Refinamiento — DOC-119

## Fuente y alcance

DOC-119: PRODUCCION-IMPORTACION-CARGA-CARPETA-SEGUIMIENTO. Contexto completo conservado en `specs/produccion-importacion-carga-carpeta-seguimiento/jira-context.md`. Tecnología observada: WebForms VB y JavaScript legacy, sin reglas React ni arquitectura nueva. Impacto transversal.

Este artefacto es la compuerta entre el ticket y la implementacion. No se aprueba por generacion automatica: una persona responsable debe confirmar alcance, decisiones, compatibilidad y evidencia de codigo.

Propuesta sobre `d7dbf201338bbfef3f82e3521359f30b1824bf95`. No se ejecutó autenticación, HTTP de aplicación, SELECT real, carga, fallos inyectados, permisos o limpieza en este refinamiento.

## Contexto inspeccionado

- `Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb`: Autorizar/ConsultarPagina/ExportarPagina/ConsultarConfirmacion/CeldaCsv; autoridad actual, totales separados y CSV de una página.
- `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Consulta.vb`: ValidarFiltroConsulta y SELECT parametrizados por usuario/ámbito/nivel/lote, snapshot por página.
- `js/gestion/ImportacionCarpetas/ImportacionCarpetas.js`: crearReferencias/validarReseleccion/consultar/abrir/manual/verify; cliente no envía filtros que admite servicio. Reproducir antes de corregir.
- `gestion/WebFormProducionDocumental.aspx.vb`: host operativo y franja con apertura/último lote/verificación/referencia.
- `js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js`: menú Importación creado desde host administrativo; resolver disponibilidad para importadores.
- `js/gestion/WebFormProducionDocumental.js`: contextual de Paneltreview no toma nodo pulsado; Doc111ActualizarRama conserva selección y deduplica por relación/ID.
- `js/jquery.contextMenu.js`: cálculo de posición previo a mostrar y sin límite vertical/teclado explícitos; reproducir defecto y revisar consumidores.
- `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionLimpiezaOriginalImportacion.vb`: política predeterminada deshabilitada.
- Evidencia DOC-111 local: consulta/CSV/recuperación/rama. DOC-118: archivo administrativo histórico separado de `doc118-integracion-completa-20261110.json` y primer corte de `doc118-resultados-integracion.md` (cierre técnico 38/38).
- Corrida nueva: `node --test tests/doc111-importacion-carpetas-client.test.cjs`, 16/16 PASS sobre SHA base indicado. Chromium con servicios sintéticos, sin aceptación integrada ni autenticación real.

Inventario completo de rutas/símbolos en PRECONDICIONES DE RUTAS de design.md. Las referencias siguientes se contrastaron contra la base indicada; son evidencia de implementación existente, no aceptación de los cambios propuestos.

## Evidencia que fundamenta las decisiones

| Ref. | Archivo y símbolo inspeccionados | Hecho comprobado |
| --- | --- | --- |
| E-01 | `Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb:67` — ConsultarPagina; `:103` — ExportarPagina; `:174` — Autorizar; `:215` — CeldaCsv | El servicio valida filtros, reautoriza lote, separa totales y exporta una página escapada; la autoridad no se toma del navegador |
| E-02 | `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Consulta.vb:35` — ConsultarRaices; `:40` — ConsultarArchivos; `:61` — ValidarFiltroConsulta; `:95` — LeerPaginaConsulta | Tipos RAICES/ARCHIVOS, página desde 1, techo 200 reducido por política de servicio, filtros cerrados y SELECT con cuenta/orden/límite |
| E-03 | `js/gestion/ImportacionCarpetas/ImportacionCarpetas.js:159` — consultar; bloque de acción exportar; `:297` — abrir | Consulta de archivos/raíces y CSV no envían filtros raíz/estado/recepción; abrir toma data-nivel del host |
| E-04 | Mismo archivo `:45` — crearReferencias; `:65` — validarReseleccion; `:126` — sync; `:340` — manual; `:344` — verify; `:366` — inicializar | Pistas acotadas por contexto; reselección ligada al recurso; confirmación consultada sin reenviarla; avisos y sustitución de host tras postback |
| E-05 | `gestion/WebFormProducionDocumental.aspx.vb:8` — RenderImportacionCarpetas; `:26` — ImportacionCarpetas_PreRender; `:33` — RenderPerfilImportacion | Host operativo y administrativo separados; controles de consulta/verificación están en la franja; PreRender actualiza nivel seleccionado |
| E-06 | `js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js:12` — host administrativo; `:34` — construcción de doc110-navigation | Menú Importación se crea durante la inicialización administrativa e incluye accesos a perfiles/proveedores/fuentes |
| E-07 | `js/gestion/WebFormProducionDocumental.js:44` — contextual Paneltreview; `:132` — contextual de documentos; `:1050` — Doc111ActualizarRama | Importar carpetas no captura el nodo pulsado; actualización de rama usa contexto e IDs confirmados, no nombres |
| E-08 | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb:32` — Resolver; `:52` — ResolverDestino; `:96` — AutorizarRecursos | Permiso de importar actual en RA, empresa/usuario/nivel vigentes, ruta/sello reconstruidos y gabinete reautorizado; no exige administración |
| E-09 | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionLimpiezaOriginalImportacion.vb` — Crear | Ausencia de política explícita compone política deshabilitada; no publica endpoint ni programa trabajo |
| E-10 | `js/jquery.contextMenu.js:12` — contextMenu; `:65` — mouseEventFunc; hideMenu | Invocador actual es el elemento enlazado; posición calcula ancho/alto antes de mostrar, sin ajuste de altura ni teclado explícito |
| E-11 | `tests/doc111-importacion-carpetas-client.test.cjs`; `tests/ImportacionCarpetasBehaviorTests.cs` — consultas, CSV, identidad y recuperación | Casos locales existentes reutilizables; sus dobles no acreditan IIS/MySQL/usuarios reales |
| E-12 | `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-cierre-administrativo-20261110.md`, `doc118-integracion-completa-20261110.json` y `doc118-resultados-integracion.md` | Archivo administrativo y aceptación técnica posterior son cortes diferentes y deben conservarse con su procedencia |

## Decisiones propuestas para aprobación

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks propuestas |
| --- | --- | --- | --- | --- | --- |
| D-01 | Identificar cada resultado por escenario/SHA/ambiente y separar los dos cierres DOC-118 | E-11/E-12 | D-01 | RQ-01 | Origen: D-01, RQ-01; mapa 1.1 y 5.4 |
| D-02 | Mantener mismo lote/tipo/filtros/página/tamaño entre lectura y CSV, con reautorización y sin efectos | E-01/E-02/E-03/E-08 | D-02 | RQ-02 | Origen: D-02, RQ-02; mapa 2.1–2.4 |
| D-03 | Recuperar plan/lote sin repetir confirmación y deduplicar relaciones en nivel original sin mover selección | E-04/E-07/E-11 | D-03 | RQ-03 | Origen: D-03, RQ-03; mapa 2.5 |
| D-04 | Conservar limpieza deshabilitada y resolver SEG-03 expresamente antes del cierre | E-09 y fuente Jira SEG-03 | D-04 | RQ-04 | Origen: D-04, RQ-04; mapa 2.6 |
| D-05 | Construir/reutilizar un único menú operativo aun sin host administrativo y separar sus permisos | E-05/E-06/E-08 | D-05 | RQ-05 | Origen: D-05, RQ-05; mapa 3.1–3.2 |
| D-06 | Superior fija nivel seleccionado; contextual fija nodo pulsado; servidor resuelve autoridad/ruta y lote conserva destino | E-03/E-05/E-07/E-08 | D-06 | RQ-06 | Origen: D-06, RQ-06; mapa 3.3–3.4 |
| D-07 | Preservar referencia manual, último lote, verificación, avisos y ciclo de vida antes de retirar franja | E-04/E-05 y vista existente | D-07 | RQ-07 | Origen: D-07, RQ-07; mapa 3.5–3.7 |
| D-08 | Ajustar posición/altura al viewport y manejar apertura/recorrido/Escape/foco sin romper consumidores | E-07/E-10 e inventario de consumidores | D-08 | RQ-08 | Origen: D-08, RQ-08; mapa 4.1–4.4 |
| D-09 | Condicionar UI a aceptación SEG-01/02 y consolidar guía/UI-05 antes de DOC-120 | Fuente Jira UI/UX y E-11 | D-09 | RQ-09 | Origen: D-09, RQ-09; mapa 2.7, 5.1 y 5.3 |
| D-10 | Separar permiso de desarrollar de autorización de pruebas reales, preservando fuentes/datos protegidos | AGENTS.md/runbook/E-08/E-09 | D-10 | RQ-10 | Origen: D-10, RQ-10; mapa 1.2 y 5.2 |

## Reglas concretas de cada decisión

### D-01 — Procedencia de resultados y base verificable

**Situación actual y evidencia:** E-12 conserva cierre administrativo DOC-118 con 29/38 y aceptación técnica posterior con 38/38. El primer corte del reporte acumulado corresponde al JSON técnico posterior; los cortes anteriores del mismo reporte siguen siendo historia. E-11 contiene pruebas locales, no certificación autenticada de seguimiento.

**Decisión:** la matriz de DOC-119 identifica para cada caso SEG/UI: fuente, SHA, ambiente, fecha, clase de comprobación, resultado y límites. Una prueba nueva obtiene su propia identificación. La coincidencia de funcionalidad o el estado Listo de Jira no reemplazan evidencia de ejecución.

**Aceptación:** al revisar una fila SEG-01/02 se puede distinguir sin inferencias si es inspección, doble local o ejecución integrada y qué commit se verificó. No se suman pruebas internas incluidas a las del contenedor ni se presentan lotes DOC-118 como lotes nuevos DOC-119. El archivo administrativo permanece intacto.

**Compatibilidad y entregable:** matriz de procedencia y expediente de cierre, mapa 1.1/5.4. No reescribir reportes históricos, huellas ni recursos de prueba para uniformar sus resultados.

### D-02 — SEG-01: filtros, páginas, autoridad y CSV

**Situación actual y evidencia:** E-01/E-02 ya soportan filtros; E-03 envía lote/tipo/página/tamaño sin ellos. Es una diferencia comprobada por inspección; el caso de interfaz debe reproducirse antes de cambiar producto. Las lecturas y CSV pasan por Autorizar, reconstruyen destino desde el lote y revalidan recursos con E-08.

**Contrato que se conserva:** ConsultarRecursosLote y ExportarPaginaLote usan loteId, tipo, pagina, tamanoPagina y filtros raizId/estado/estadoRecepcion actuales. RAICES admite estado de raíz y ARCHIVOS estado de efecto/recepción. No se envía estadoRecepcion para RAICES; no se agregan campos, endpoint o catálogo de estados paralelo. Página desde 1, tamaño dentro de política vigente y techo 200; filtro/tipo/referencia inválidos se rechazan en servidor.

**Regla de funcionamiento:** filtros activos forman parte de la consulta representada. Al paginar se mantienen; CSV usa el mismo lote, tipo, filtros, número y tamaño de esa página. totalOriginal conserva inventario confirmado y totalFiltrado corresponde a la selección. RECIBIDO es recepción, no prueba de almacenamiento. Cada página conserva su revisión de lectura; exportar después no promete identidad de snapshot si el lote cambió entre solicitudes.

**Aceptación observable:** con recursos en estados mixtos se comprueban primera/siguiente página y filtro con coincidencias y sin ellas. CSV contiene las filas de su consulta de página; no recorre silenciosamente el lote completo ni reduce totalOriginal. Casos de fórmula con prefijos directos/espacios/controles y comillas/separadores/saltos conservan neutralización de CeldaCsv. Una cuenta ajena o autoridad revocada no recibe filas/CSV. Controles SELECT antes/después verifican que leer/exportar no avance, reciba, confirme, reconcilie o modifique estado/auditoría.

**Compatibilidad y entregable:** mapa 2.1–2.4, con corrección de transporte/vista únicamente si la reproducción acredita brecha. Se reutilizan orden, proyección pública y límites del repositorio; no se eligen valores nuevos de configuración de ambiente.

### D-03 — SEG-02: recarga, reselección y rama original

**Situación actual y evidencia:** crearReferencias (E-04) limita almacenamiento a versión del formato e IDs de plan/lote bajo DOC111_REF_ más el espacio opaco de contexto. No guarda File, ruta, manifiesto, token, clave de mutación o hash. verify consulta ConsultarConfirmacionLote con planId; Doc111ActualizarRama (E-07) toma nivelRef/referenciaContexto y relaciones confirmadas.

**Decisión de recuperación:** tras recarga se reautoriza el plan/lote. Si no hay confirmación verificable se conserva bloqueo/aviso sin reenviar ConfirmarPlan ni generar otro lote. Si existe lote, su nivel/ruta provienen de ConsultarLote, aunque el árbol tenga seleccionado otro nivel. Si sessionStorage está bloqueado, se avisa de la falta de persistencia y continúa disponible consulta manual por referencia; no se promete recuperación del File tras recargar.

**Decisión de reselección:** se elige explícitamente recursoId/raizId autorizado con recepción por completar, efecto NO_INICIADO y sin intento activo. Nombre/tamaño/ultimaModificacionLocal coincidentes solo habilitan vincular el File a ese recurso; el servidor conserva bytes/hash/autoridad. No se reemplazan ya recibidos, inciertos o en vuelo ni se identifica por nombre entre raíces homónimas.

**Decisión del árbol:** agregar únicamente raíces EXPEDIENTE_CONFIRMADO/COMPLETADA/PARCIAL sin intento activo y con expedienteId/relacionNivelId/nombreExpediente confirmado. Clave existente: relacionNivelId|nivelRef|expedienteId|. El padre debe ser único, visible y tener rama cargada; referenciaContexto debe coincidir. Si no, se omite actualización visual sin alterar resultados del lote. No expandir, cambiar selección, inventar nodos documentales ni duplicar por consultas repetidas.

**Aceptación y entregable:** mapa 2.5 acredita recarga autenticada con recepción por completar y selección en otro nivel, dos raíces homónimas, consulta repetida y rama oculta/ausente. El contador de ConfirmarPlan no aumenta por recuperar; el envío se liga al ID elegido; nuevas relaciones aparecen una vez en la rama original y selección/expansión se conservan. Las pruebas locales existentes son antecedente, no aceptación integrada.

### D-04 — SEG-03: decisión operativa de retención

**Situación actual y evidencia:** E-09 compone limpieza deshabilitada por defecto. Existe limpieza interna con verificación/diario; consultar/exportar no la invoca. La sesión actual no ha acordado retención, operación de limpieza ni borrado de pruebas.

**Decisión de este cambio:** conservar esa composición, sin Web.config, endpoint, botón, job o plazo nuevos. Ningún resultado SEG-01/02 habilita limpieza. Las protecciones de recursos activos, inciertos y documentos permanentes no se debilitan.

**Resoluciones admisibles para cierre:** registrar decisión expresa del responsable de operar sin limpieza, con límite y consumo de disco documentados, dejando SEG-03 como no ejecutado real; o definir retención, mecanismo interno y alcance, obtener autorización específica y registrar el resultado real correspondiente. Una política propuesta sin autorización/ejecución no se etiqueta como prueba de borrado.

**Aceptación y entregable:** mapa 2.6 conserva decisión, responsable, alcance y estado real. No hay eliminación de .part, historial, staging o datos protegidos por este refinamiento. Si se propone ejecutar limpieza, requiere otro plan concreto revisable antes del efecto; no está autorizado por aprobar el diseño.

### D-05 — UI-01: operación disponible sin administrar

**Situación actual y evidencia:** E-05 resuelve por separado host operativo y administrativo. E-06 crea Importación durante montaje de perfiles; trasladar allí los botones sin resolver esa dependencia podría ocultar la operación al importador sin permiso administrativo.

**Decisión:** reutilizar un único menú Importación cuando exista y asegurar su creación para el host operativo autorizado aunque falte host administrativo. Sus accesos operativos son Importar carpetas… y Consultar importación…. Perfiles/proveedores/fuentes quedan en un grupo visual separado y solo si sus autorizaciones actuales lo permiten. No condicionar importar a PuedeAdministrar ni conceder importar por administrar.

**Aceptación:** verificar por separado actor importador sin administración, administrador sin permiso de importar y actor con ambos. El primero tiene operación sin administración; el segundo no obtiene operación por sus privilegios administrativos; el tercero ve ambos grupos. Reinicialización y UpdatePanel conservan un solo menú y un solo envío por activación.

**Compatibilidad y entregable:** mapa 3.1–3.2 afecta composición de navegación y bootstrap existentes. Ocultar/deshabilitar un acceso no sustituye reautorización servidor. La retirada de franja queda sujeta a D-07, no se ejecuta como parte de mover el primer botón.

### D-06 — UI-02: nivel elegido, nivel pulsado y autoridad

**Situación actual y evidencia:** contextual de Paneltreview (E-07) se enlaza al panel y su callback ignora el elemento del evento; abrir (E-03) lee data-nivel actualizado por PreRender (E-05). Esa combinación no identifica el nivel pulsado. ResolverDestino (E-08) sí valida referencia/empresa/nivel activo/propietario o facultades vigentes y construye ruta/sello sin necesitar autoridad del navegador.

**Decisión por acceso:** superior captura una vez el nivel seleccionado elegible. Contextual identifica el nodo de nivel que originó la invocación, incluso al pulsar su icono/texto o abrir por teclado, y captura su referencia antes de abrir. El panel/fondo no constituye nivel. Distinguir nodos nivel de las identidades compuestas de expediente; documento/expediente/fondo no usan fallback a la selección. Dos nombres iguales no son identidad equivalente.

**Frontera servidor:** abrir solicita AbrirImportacion con nivelRef capturado. Usuario/empresa/permisos/ruta/sello se resuelven por servidor; no se aceptan desde la acción del menú. ABRIR conserva facultades actuales de nivel y operación, y AutorizarRecursos conserva gabinete/destino. El asistente muestra nivel y ruta canónica de la respuesta; cambiar selección después no cambia ese contexto. Una revocación entre mostrar menú e invocar se rechaza sin preparación/confirmación.

**Consulta contextual:** conserva consulta por referencia y autoridad del lote. Invocar desde B no reasigna a B un lote cuyo destino servidor es A, ni presenta una referencia manual como historial de B. No se agrega filtro/búsqueda de lotes por nivel o endpoint para construir un historial nuevo.

**Aceptación y entregable:** mapa 3.3–3.4 compara seleccionado A/pulsado B, incluidos homónimos: petición de apertura B, ruta servidor B y ninguna petición para A. Sin selección, nodo excluido o referencia no válida no comienza importación. El lote consultado mantiene destino propio. Permiso revocado, destino movido o gabinete denegado se verifican por las reglas existentes, sin cambiar permisos reales para probar sin autorización específica.

### D-07 — UI-03: equivalencia antes de retirar controles

**Situación actual y evidencia:** E-04/E-05 contienen apertura, último lote, consulta manual, verificación y estado visible en la franja. montar/inicializar gestionan una instancia por host; la vista conserva foco y libera recursos. Trasladar solo apertura y ocultar la franja perdería funciones de recuperación.

**Equivalencia exigida:** Importar carpetas… reutiliza abrir/asistente; Consultar importación… abre vista/diálogo de referencia que reutiliza manual/consulta. Mantener acceso al último lote conocido, verificación del plan sin acuse y aviso visible mientras exista incertidumbre. Una referencia inválida se rechaza sin consultar; una consulta denegada no borra ni convierte en autorizado el lote. No se construye un historial ni otra máquina de coordinación.

**Ciclo de vida:** usar el mismo almacenamiento de pistas y mantener su aislamiento por contexto. Una reinicialización del mismo host no remonta listeners; reemplazar host por postback destruye la instancia anterior y evita que respuestas tardías la revivan. Cerrar libera URL de Blob, temporizadores y File locales sin prometer cancelar el servidor; al cerrar el diálogo se devuelve foco al invocador que siga disponible.

**Aceptación:** antes de retirar franja, comprobar en la navegación nueva las cuatro funciones: importar, referencia manual, último lote y verificación del plan. Repetir con recarga/postback/reapertura y sessionStorage bloqueado: aviso visible y consulta manual permanecen. Un clic produce una apertura, una verificación o una consulta; recuperar nunca dispara confirmación nueva. La selección del árbol no se convierte en destino del lote consultado.

**Compatibilidad y entregable:** mapa 3.5–3.7 reutiliza servicios/contratos/guard actual. Franja se retira solo tras esa equivalencia y conservación de avisos; no se elimina el host/contexto operativo necesario para la instancia o actualización del árbol.

### D-08 — UI-04: posición, recorrido y consumidores compartidos

**Situación actual y evidencia:** E-10 mide mientras el menú está oculto, decide arriba/abajo sin acotar altura y toma como activeElement el elemento enlazado. El plugin ofrece delegateEventTo/showMenu/hideMenu, pero no un recorrido de teclado explícito. Primero registrar caso reproducible de recorte y del foco; no dar por corregido por añadir CSS.

**Posición/altura:** medir dimensiones utilizables al abrir; ajustar ambos ejes para permanecer en viewport considerando scroll de página. Cuando exceda altura disponible, limitar altura y permitir desplazarse hasta la última opción. Cerrar/restablecer no deja un menú anterior visible ni posición/scroll heredados que oculten acciones de la siguiente apertura.

**Teclado/foco:** apertura contextual de teclado debe conservar el nodo invocador, igual que ratón. Recorrido permite alcanzar acciones disponibles; activación ejecuta una sola vez sobre ese nodo. Escape cierra sin activar y devuelve foco al invocador; cuando una acción abre diálogo, el diálogo recibe foco y su cierre restaura el origen disponible. No usar la corrección de foco para cambiar selección/destino silenciosamente.

**Alcance compartido comprobado:** búsqueda de .contextMenu( en js encuentra consumidores en Producción, clasificación, instrumentos, toponimia, colaboración/compartidos, historial, Docuarchi, radicación, RadicadorSimplificado, gestión de trámites y Workflow. Son consumidores por inspección, no certificación de cada página; inventario exacto de archivos se conserva abajo. Si plugin/CSS compartido cambia, verificar usos afectados y su acción original, no solo importación.

**Aceptación y entregable:** mapa 4.1–4.4 conserva caso antes/después en bordes y ventana reducida; todas las opciones son alcanzables con scroll/teclado y Escape restaura foco. Menús de árbol/documentos mantienen su recurso original y callbacks; no hay activaciones duplicadas ni interferencia al alternar menús. Si una página real requiere autenticación, registrar su verificación solo tras autorización; un doble local no la certifica.

### D-09 — UI-05: compuertas, regresión y guía operativa

**Secuencia vinculante del diseño:** primero reproducir/corregir brechas de seguimiento y acreditar SEG-01/02; después UI-01–04; finalmente matriz UI-05 y guía de navegación definitiva. SEG-03 debe tener resolución expresa antes del cierre, sin convertirlo en permiso implícito de limpieza. DOC-120 consolida la interfaz resultante, no absorbe defectos de destino/autoridad/acceso/recuperación de DOC-119.

**Compuerta de entrada a UI:** mapa 2.7 exige evidencia integrada identificada de SEG-01/02 y ausencia de brechas reproducidas abiertas que afecten su aceptación. Estar preparado el runner, pasar 16 pruebas locales o estar cerrado DOC-118 no satisface esta compuerta. Corregir seguimiento antes de esa aceptación sí pertenece a la fase operativa.

**Matriz UI-05:** registrar para cada acceso superior/contextual: nivel seleccionado frente a pulsado y homónimos; sin selección/nodo excluido; permisos denegados y revocados; ruta visible; reapertura/postback/recarga; referencia manual/último lote/verificación; menú dentro de viewport, teclado/Escape/foco. Adjuntar regresión de consumidores compartidos afectados y carga individual pertinente. Cada caso indica SHA, ambiente, tipo de prueba, resultado y límite.

**Guía final y entregable:** mapa 5.1/5.3 documenta dónde iniciar/consultar/exportar una página, qué conservar antes de recargar, cómo reseleccionar por ID y cómo tratar incertidumbre/detención/verificación de DOC-118. Separar lecturas de acciones con efectos; no presentar cerrar diálogo como cancelar servidor ni sugerir replay ciego. No registrar navegación propuesta como guía definitiva antes de verificarla.

### D-10 — Autorización de ejecución y límites protegidos

**Decisión:** autorización de desarrollo/refinamiento no extiende autorizaciones históricas para ambiente/cuentas/recursos. Plan de prueba real identifica antes de ejecutar URL/ambiente, cuentas autorizadas, recursos existentes o descartables, lecturas/efectos previstos, presupuesto y criterio de detención. E2E autenticada, carga, cambios de permisos, inyección de fallos y limpieza requieren alcance específico; no se lanzan por estar documentados en Jira.

**Controles y secretos:** RA/DA usan solo SELECT para comprobar estado/auditoría; no reparar datos con UPDATE/DELETE. Credenciales/cookies/conexiones son efímeras y no se imprimen ni versionan. Reutilizar sesión/runner existentes y revisar su modo: el runner REC puede confirmar/recibir/crear/almacenar, por lo que su nombre o un modo de verificación no autoriza el recorrido mutante completo.

**Compatibilidad obligatoria:** no modificar AlmacenamientoDocumentoProduccionDocumental ni SII, esquema, semántica de recepción/confirmación/recuperación o inventario/snapshot/efectos confirmados. Conservar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA; PDF soportado y TIFF/Multi-TIFF rechazados. No introducir workers, cuotas globales, jobs, permisos, rollback global ni aislamiento XML adicional. Conservar datos/temporales y recursos protegidos previos.

**Aceptación y entregable:** mapa 1.2/5.2 conserva plan autorizado y contraste de diff/huellas contra fuentes/configuración/esquema protegidos. Sin autorización se informa escenario no ejecutado real y se avanza solo en trabajo independiente autorizado; no se emite PASS integrado. Ninguna corrección de menú elimina controles de autoridad del endpoint.

## Inventario de consumidores del menú contextual

Búsqueda de código realizada con `rg -l '\.contextMenu\(' js -g '*.js' -g '!**/*.min.js'`. Lista de 22 archivos con invocaciones; varios contienen más de un menú. Antes de modificar plugin compartido, contrastar su inclusión en páginas y el caso afectado, sin asumir que cada invocación es una página desplegada:

- `js/gestion/WebFormProducionDocumental.js`
- `js/gestion/WebFormGaadmonclasificacion.js`
- `js/gestion/WebFormGaGestionInstrumentos.js`
- `js/gestion/WebFormGatoponimica.js`
- `js/gestion/WebFormGaListaDocumentosColaboracion.js`
- `js/gestion/WebFormDocumentoCompartidoOtrosUsuarios.js`
- `js/gestion/WebFormDocumentosCompartidosRevision.js`
- `js/gestion/FormGaHistorialRespuesta.js`
- `js/gestion_correspondencia/WebForm_interface_gestion_tramite.js`
- `js/Docuarchi/WebFormDaConsultaDocumento.js`
- `js/Docuarchi/WebFormDaIndiceDocuarhi.js`
- `js/radicacion/WebFormListaSolicitudesPorMiAprobacion.js`
- `js/radicacion/WebFormListaSolicitudesAprobacion.js`
- `js/radicacion/WebFormConsultaRadicacion.js`
- `js/RadicadorSimplificado/Publico/WebFormConsultaRadicadoPublico.js`
- `js/workflow/WebFormFreeImage.js`
- `js/workflow/WebFormFreeImageVisorEmerge.js`
- `js/workflow/WebFormPendientes.js`
- `js/workflow/WebFormindice.js`
- `js/workflow/WebFormConsultaTareasWorkflow.js`
- `js/workflow/WebFormRecuperarTarea.js`
- `js/workflow/Webworkflow.js`

## Requisitos verificables

| ID | Resultado observable | Criterio | Compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Procedencia atribuible por caso | WHEN se revisa SEG/UI, THEN fuente/SHA/ambiente/clase/resultado distinguen antecedente y corrida actual | Archivo administrativo DOC-118 intacto; mapa 1.1/5.4 |
| RQ-02 | Consulta filtrada/paginada y CSV autorizados | WHEN cambia página o se exporta, THEN se conserva lote/tipo/filtros/tamaño; originales no se reducen, fórmulas se neutralizan y controles SELECT no observan efectos | Sin snapshot global ni exportación total; identidad ajena/revocada sin datos; mapa 2.1–2.4 |
| RQ-03 | Recuperación sin nuevos efectos | WHEN se recarga sin acuse y reselecciona por ID, THEN no aumenta ConfirmarPlan, lote mantiene destino y árbol deduplica relaciones sin mover selección | Metadatos no prueban bytes; rama ausente/oculta no invalida resultados; mapa 2.5 |
| RQ-04 | Retención resuelta sin activación implícita | WHEN se propone cierre, THEN hay decisión expresa sin limpieza con consumo documentado o política/mecanismo/autorización/resultado reales identificados | Borrado no ejecutado no se marca PASS; sin eliminar pruebas; mapa 2.6 |
| RQ-05 | Operación independiente de administración | WHEN se comparan importador, administrador y actor con ambos, THEN cada uno conserva únicamente accesos autorizados y una instancia de menú | Sin elevar permisos o duplicar envío tras postback; mapa 3.1–3.2 |
| RQ-06 | Destino capturado por identidad | WHEN seleccionado A/contextual B, THEN AbrirImportacion solicita B y muestra ruta servidor B; nodos excluidos no caen a A | Revocación/destino/gabinete se reautorizan; lote conserva destino propio; mapa 3.3–3.4 |
| RQ-07 | Cuatro accesos y avisos equivalentes | WHEN se retira franja, THEN importar/referencia/último lote/verificación funcionan tras reapertura/postback/recarga con una instancia y aviso visible | Host operativo/pistas preservados; storage bloqueado mantiene consulta manual; mapa 3.5–3.7 |
| RQ-08 | Menú alcanzable y recurso original | WHEN abre en borde/altura reducida o teclado, THEN opciones alcanzables, Escape restaura foco y acción usa invocador correcto una vez | Verificar consumidores afectados del inventario compartido; mapa 4.1–4.4 |
| RQ-09 | Compuerta operativa y guía definitiva | WHEN comienza UI, THEN SEG-01/02 tienen aceptación integrada; cierre conserva matriz UI-05 y guía separando lectura/efectos | No sustituir aceptación por pruebas locales ni diferir defectos a DOC-120; mapa 2.7/5.1/5.3 |
| RQ-10 | Ejecución dentro de autorización y fuentes protegidas | WHEN se ejecuta prueba real, THEN existe autorización concreta y controles SELECT/secretos efímeros; contraste final no altera fuentes/esquema/configuración protegidos | Sin limpieza/jobs/cuotas/efectos ni permisos por inferencia; mapa 1.2/5.2 |

## Trazabilidad y resultado

Design/spec y este refinamiento desarrollan las mismas diez decisiones y requisitos. Este documento explicita situación comprobada, contrato/regla, aceptación, compatibilidad y entregable por decisión; no usa la tabla de IDs como sustituto del contenido. `tasks.md` contiene las 24 tareas con área/complejidad/origen/verificación y las referencias numéricas utilizadas aquí, aplicadas por indicación del usuario. `plan-tareas-propuesto.md` conserva la procedencia de la propuesta; el estado de ejecución se mantiene únicamente en `tasks.md`, con 0/24 completadas.

Decisiones abiertas identificadas, sin completar por inferencia: resolución operativa SEG-03 y autorización concreta de ambiente/cuentas/recursos para SEG-01/02 y casos UI reales. No impiden revisar este diseño, pero sí ejecutar efectos o declarar el cierre correspondiente. La corrección documental actual no aprueba automáticamente el refinamiento ni ejecuta su plan.

Estado: borrador para revisión humana; mapa aplicado, sin tareas ejecutadas. La auditoría `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-119 --sync` confirma decisiones y orígenes en tasks; solo falla `refinement:approved`. Tras aprobación del contenido, volver a sincronizar. No iniciar implementación por existencia de artefactos. Autorizaciones reales y resolución SEG-03 se registran aparte antes de acciones o cierre.
