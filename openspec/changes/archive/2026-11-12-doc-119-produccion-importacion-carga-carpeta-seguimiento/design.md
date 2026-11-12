<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
## Context

DOC-119 — PRODUCCION-IMPORTACION-CARGA-CARPETA-SEGUIMIENTO. Fuente completa preservada en `specs/produccion-importacion-carga-carpeta-seguimiento/jira-context.md`, incluido el acuerdo UI/UX de 2026-11-10.

Inspección de código sobre `d7dbf201338bbfef3f82e3521359f30b1824bf95`, rama `feature/DOC-119`, carpeta principal. Jira identifica main `9faa8a35106d8f35ca1164f6d580596b36cd4c06` como contraste actual; `a3c7263763a89901ba78d05ccebc284a43a4cd76` es antecedente histórico. Ninguna evidencia anterior se atribuye a una corrida de DOC-119.

La consulta actual reautoriza propietario/ámbito/nivel/recursos y usa SELECT, con snapshot por página. CSV exporta una página. La recuperación guarda IDs acotados y actualiza el árbol por relaciones confirmadas.

| Criterio | Hallazgo de inspección | Implicación |
| --- | --- | --- |
| SEG-01 | Servicio admite raíz/estado/recepción; cliente consulta/exporta sin esos filtros | Reproducir brecha antes de corregir con DTO vigente |
| UI-01 | El menú Importación se crea desde el host administrativo; los accesos operativos están en una franja aparte | Garantizar acceso al importador sin administrar perfiles |
| UI-02 | Contextual de Paneltreview llama abrir sin capturar el nodo; abrir usa data-nivel | Fijar nivel pulsado explícito, reautorizado por servidor |
| UI-04 | Plugin posiciona antes de mostrar, sin límite vertical ni teclado explícito | Reproducir recorte y teclado; inventariar consumidores compartidos |
| DOC-118 | Archivo administrativo 29/38 conservado y cierre técnico posterior 38/38 en JSON/reporte | Separar cortes; ninguno acredita SEG-01/02 actual |

## Goals / Non-Goals

**Goals**

- Acreditar SEG-01/02 integrados y explicitar resolución operativa de SEG-03.
- Reorganizar UI-01–05 después de aceptar el seguimiento.
- Preservar autoridad, destino, recuperación, identidad y efectos confirmados.

**Non-Goals**

- Historial nuevo, exportación global, nuevas capas/endpoints/tablas.
- Modificar SII, AlmacenamientoDocumentoProduccionDocumental, semántica operativa o soporte PDF.
- Habilitar limpieza, workers, permisos, cuotas o aislamiento adicional XML por inferencia.
- Reutilizar autorizaciones reales anteriores o borrar recursos de pruebas.

## PRECONDICIONES DE RUTAS

Inventario previo de rutas existentes. Antes de añadir cualquier archivo, justificarlo en este inventario y registrarlo en `GestionDocumental-Docuarchi.net.vbproj` cuando corresponda.

| Responsabilidad | Rutas y símbolos | Tratamiento |
| --- | --- | --- |
| Bootstrap | `gestion/WebFormProducionDocumental.aspx`, `gestion/WebFormProducionDocumental.aspx.vb`: RenderImportacionCarpetas/RenderPerfilImportacion y sus PreRender | Integrar accesos y conservar contexto tras postback |
| Menú administrativo | `js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js`: doc110-navigation; ProveedorAdminEditor.js y FuenteAdminEditor.js en el mismo directorio | Integrar operación sin depender de administración |
| Cliente | `js/gestion/ImportacionCarpetas/ImportacionCarpetas.js`: montar, abrir, consultar, manual, verify, crearReferencias | Una instancia; destino explícito; filtros existentes si se reproduce brecha |
| Vista | `js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js`, `ImportacionCarpetas.css` y `PerfilImportacionEditor.css` en el mismo directorio | Consulta/destino/avisos en interfaz existente |
| Árbol/contextual | `js/gestion/WebFormProducionDocumental.js`: contextual de Paneltreview y Doc111ActualizarRama | Nivel pulsado frente a seleccionado; identidad por relación/ID |
| Plugin compartido | `js/jquery.contextMenu.js`, `js/jquery.contextMenu.css` | Corrección tras reproducción e inventario de consumidores |
| Endpoint/contrato | `webservice/WebServiceImportacionCarpetas.asmx.vb`: ConsultarLote, ConsultarRecursosLote, ExportarPaginaLote, ConsultarConfirmacionLote | Reutilizar DTO y dispatcher; sin endpoints nuevos |
| Servicio/autoridad | `Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb`: ConsultarPagina/ExportarPagina/Autorizar; `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb`: ResolverDestino | Reutilizar; modificar solo ante defecto reproducido |
| Persistencia | `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Consulta.vb`: ValidarFiltroConsulta/ConsultarRaices/ConsultarArchivos | SELECT parametrizado; sin esquema/repositorio paralelo |
| Limpieza | `Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.Limpieza.vb`; `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionLimpiezaOriginalImportacion.vb`: Crear | Sin cambios ni invocación; deshabilitada por defecto |
| Pruebas locales | `tests/doc111-importacion-carpetas-client.test.cjs`, `tests/doc110-perfil-importacion-behavior.test.cjs`, `tests/ImportacionCarpetasBehaviorTests.cs` | Ampliar solo casos afectados |
| E2E | `tools/e2e/scripts/run-doc111-importacion-carpetas.cjs`, `run-doc113-ventana-importacion.cjs` en el mismo directorio; `tools/e2e/tests/doc111-importacion-carpetas.spec.cjs` | Revisar modos antes de ejecutar; REC no equivale a lectura |
| E2E de seguimiento | Nuevos `tools/e2e/scripts/run-doc119-seguimiento.cjs`, `tools/e2e/tests/support/doc119-seguimiento.cjs`, `tools/e2e/tests/doc119-seguimiento-policy.test.cjs` | Selector SEG-01/02 de lectura: reutiliza openSession, consola y queryRows existentes; sin login/puente/coordinador alternativo. Archivos Node de pruebas, sin Compile VB |
| Documentación | `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/11-prompt-seguimiento-operacion.md`, `Diseno/02-contratos-operacion.md` y `Validacion/` bajo ese directorio; `Doc/Tecnica/Opsxj/doc-119-produccion-importacion-carga-carpeta-seguimiento/` | Navegación definitiva, contratos y evidencia con límites |
| Regresión UI nueva | `tests/doc119-navigation.test.cjs`, `tests/doc119-context-menu.test.cjs` y `Validacion/doc119-contextuales-inventario-20261112.json` | Casos afectados de navegación y contrato compartido, Chromium instalado; archivos Node/datos de prueba sin Compile VB |

## Decisions

### D-01 — Procedencia y aceptación por escenario (RQ-01)

Separar inspección, prueba local con dobles y ejecución autenticada. Conservar archivo administrativo DOC-118; contrastar `doc118-integracion-completa-20261110.json` y el primer corte de `doc118-resultados-integracion.md` como cierre técnico posterior. Su 38/38 y sus lotes son antecedentes, no aceptación SEG-01/02. Registrar SHA/ambiente/resultado de cada prueba nueva.

### D-02 — Lectura y CSV de una página (RQ-02, SEG-01)

Reutilizar ConsultarLote/ConsultarRecursosLote/ExportarPaginaLote. Verificar filtros raíz/estado/recepción, límites, páginas y totales original/filtrado. Distinguir RECIBIDO de almacenamiento confirmado. CSV exporta exactamente página/filtros representados, neutraliza fórmulas y no reconcilia. Páginas sucesivas no prometen snapshot global. Si se reproduce la falta de filtros en cliente, transportar los campos vigentes y cubrir regresión antes de aceptar SEG-01.

### D-03 — Recuperación por IDs y rama original (RQ-03, SEG-02)

Conservar IDs de plan/lote bajo el espacio de recuperación como pistas, nunca autoridad. Consultar confirmación sin reenviar ConfirmarPlan; el lote recuperado fija su destino aunque cambie la selección visible. Reseleccionar por recursoId/raizId; metadatos no sustituyen validación de contenido en servidor. Actualizar rama original por relación/ID confirmado, sin duplicación, expansión ni cambio de selección.

Criterio de aceptación por capas aprobado expresamente el2026-11-12: SEG-02 real29623218 verifica recarga/reselección sin envío, relación existente única, selección/expansión y huellas conservadas; inserción ausente y raíces físicas homónimas se complementan con regresión local del cliente real/servicios sintéticos. Decisión ACEPTADO_CON_COBERTURA_MIXTA documentada en `doc119-aceptacion-cobertura-mixta-propuesta-20261112.md` (estado formal ACEPTADO_CON_COBERTURA_MIXTA). Conservar PASS_READONLY_WITH_LIMITS del runtime y ambos casos no observados en IIS; no reclasificar pruebas locales como integración real.

### D-04 — Limpieza condicionada (RQ-04, SEG-03)

Conservar composición deshabilitada sin endpoint/job/botón/configuración nueva. Habilitar o ejecutar requiere política de retención, mecanismo interno concreto y autorización específica. Operar sin limpieza requiere decisión expresa del responsable y documentar consumo de disco: no se adopta por silencio. SEG-03 permanece condicionado/no ejecutado hasta resolución. No borrar temporales, .part, historial ni recursos de otras pruebas.

### D-05 — Menú superior independiente de administración (RQ-05, UI-01)

Integrar Importar carpetas… y Consultar importación… en Importación, disponible al importador sin administrar perfiles. Administración conserva permisos separados y separación visual. Reutilizar menú existente o construir acceso operativo sin elevar privilegios; evitar duplicación tras UpdatePanel. Retirar franja solo después de verificar equivalencia de referencia manual, último lote, verificación y avisos.

### D-06 — Destino explícito según acceso (RQ-06, UI-02)

Superior usa nivel seleccionado elegible; contextual usa explícitamente el nivel pulsado aunque difiera. Distinguir niveles de expedientes/documentos/fondo por formato e identidad actuales, nunca por nombre. Pasar únicamente referencia de nivel a AbrirImportacion con autorización actual; mostrar nivel/ruta canónica servidor. No permitir cambio silencioso de destino abierto. Consulta contextual respeta autoridad/contexto existentes y no reasigna destino de un lote ni crea historial por nivel.

### D-07 — Asistente único y recuperación visible (RQ-07, UI-03)

Reutilizar montar/asistente/servicios. Alojar consulta por referencia en diálogo/vista sin segundo coordinador. Mantener último lote, verificación de confirmación sin acuse, sessionStorage acotado, reselección y avisos visibles. Postback/reapertura no duplica listeners/instancias. Cerrar libera recursos de navegador sin prometer cancelar efectos servidor ni conservar File tras recarga.

### D-08 — Viewport y teclado del contextual (RQ-08, UI-04)

Reproducir recorte; medir menú visible y ajustar posición/altura con scroll al viewport. Soportar apertura por teclado, recorrido, Escape y retorno de foco al invocador. Si cambia plugin/CSS compartido, inventariar y comprobar consumidores afectados, incluidos árbol/documentos; mantener acción sobre recurso original.

### D-09 — Secuencia y guía definitiva (RQ-09, UI-05)

Orden: diagnóstico/corrección reproducida del seguimiento → aceptación integrada SEG-01/02 → UI-01–04 → regresión UI-05/guía → consolidación posterior DOC-120. Defectos de destino/autoridad/acceso/recuperación se corrigen en DOC-119. Cubrir niveles seleccionado/pulsado, homónimos, sin selección, permisos denegados/revocados, reapertura/postback/recarga, consulta/CSV/recuperación, foco y carga individual pertinente. No marcar tareas por existencia de archivos o resultados históricos.

Compuerta operativa abierta por decisión responsable «aceptado» del2026-11-12: SEG-01 integrado completo ccb25ec9 y SEG-02 con cobertura mixta aprobada según D-03. Esto permite iniciar UI sin modificar los resultados ni ampliar efectos de prueba. La guía y la evidencia final mantienen separados los alcances real/local y los límites de inserción/homónimos.

### D-10 — Autorizaciones y límites (RQ-10)

Pruebas reales, carga, fallos inyectados, permisos y limpieza requieren autorización expresa de ambiente, cuentas, recursos y efectos. Controles RA/DA solo SELECT; secretos efímeros y evidencia saneada. Preservar SII/almacenamiento compartido, esquema, semántica operativa, PDF/rechazo TIFF, snapshot/inventario/efectos, EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA. Sin rollback global, workers, cuotas o aislamiento XML adicional.

## Risks / Trade-offs

- Mover accesos al menú administrativo sin resolver su ausencia para importadores perdería acceso operativo.
- Capturar selección en vez del nodo pulsado puede abrir otro destino; servidor mantiene autoridad final.
- Ocultar franja antes de equivalencia rompe recuperación/avisos.
- Plugin compartido exige regresión de otros menús.
- Timeout exige consulta/verificación y nunca reenvío automático.
- Sin autorización real no hay aceptación SEG-01/02; sin decisión expresa SEG-03 sigue condicionado.

## Migration Plan

Sin migración ni despliegue. El mapa de `plan-tareas-propuesto.md` se aplicó a `tasks.md` por indicación del usuario. El corte inicial de 4/24 sobre94ca9277 precede las autorizaciones y pruebas registradas posteriormente. El2026-11-12 se aceptó expresamente la cobertura mixta de SEG-02, después de SEG-01 real y resolución de SEG-03; entonces se implementó UI-01–05. Regresión final local99/99, carga individual/contratos33/33, navegación6/6 repetida tras ajustar el rótulo contextual y build0errores. Evidencia y revisión final en `Validacion/doc119-ui-verificacion-20261112.md`; no representan una nueva prueba UI autenticada en IIS.

Evidencia nueva bajo `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/`: `doc119-procedencia-20261111.md`, `doc119-filtros-local-20261111.md` y `doc119-plan-aceptacion-20261111.md`. Son documentos de evidencia/plan, sin Compile de proyecto. Casos nuevos se incorporan al archivo de pruebas existente; recursos JS/CSS modificados renuevan caché en el ASPX existente.

Una reversa de navegación restaura accesos previos conservando pistas/datos/efectos; no revierte lotes ni elimina staging. Justificar y registrar archivos nuevos antes de compilar.

## Resoluciones y límites de entrega

- SEG-03 resuelto expresamente como operación sin limpieza, datos conservados y sin cuota/plazo nuevo. La medición57archivos/24.371bytes pertenece al corte previo a la preparación posterior; no es inventario actual ni ejecución de borrado.
- SEG-01 real ccb25ec9 y SEG-02 real29623218 mantienen sus SHA, presupuestos y ambiente localhost. Cobertura complementaria local de inserción/homónimos aceptada expresamente; resultado runtime intacto.
- La reorganización final se verifica en Chromium local con servicios sintéticos y compilación Framework. No se atribuye autenticación/IIS/MySQL real a esas pruebas. Archivo, publicación, integración a main y validación autenticada posterior conservan sus acciones específicas.
