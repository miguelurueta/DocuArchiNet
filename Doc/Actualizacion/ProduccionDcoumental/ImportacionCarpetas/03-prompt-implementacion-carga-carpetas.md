# Prompt de implementación: carga documental por carpetas

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior en ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1, respetando las fronteras existentes y el alcance de esta entrega.

## OBJETIVO

Implementar importación desde dispositivos en Producción Documental mediante las entregas 04–12 de [la guía 00](00-guia-ejecucion-prompts.md); 13 requiere activar su alcance opcional. Este prompt general aporta requisitos transversales: no obliga a ejecutar todas las entregas al trabajar una especializada ni autoriza duplicar repositorios, contratos o scripts. Reutilizar capacidades compatibles y preservar SII. El texto describe implementación futura, no componentes o tablas ya existentes.

### 1. Contexto obligatorio

Leer AGENTS y, desde este directorio, `02-prompt-implementacion-perfil-importacion.md`, `Exploracion/00-exploracion-importacion-masiva-por-carpetas.md`, `01-diagnostico-reutilizacion-tablas.md`, `02-evidencia-esquema-perfil.json`, `03-validacion-bd-y-definicion-perfil.md`, `04-diagrama-flujo-tecnico-perfil.md` y `05-contexto-tecnico-carga-carpetas.md` (estos cinco últimos también bajo `Exploracion/`). Leer `Prototipo/README.md` y `Prototipo/importacion-carpetas.html`: preservar interacción, sustituir ejemplos/validación local/simulación por contratos reales.

La exploración 03 precisa hipótesis de tablas; diagramas son propuestas. Verificar código/perfil actuales y símbolos/rutas de la matriz 05. Stack existente: WebForms/VB.NET 4.6.1, JavaScript, ASMX/ASHX; no asumir Core/MVC, React, API nueva desplegada o worker.

La carga depende de una versión publicada del perfil. Si falta, identificar contratos/orden y avanzar solo componentes independientes con dobles explícitos; no declarar carga completa ni crear configuración fija/paralela. Administración de perfiles pertenece a 02.

### 2. Resultado funcional

Desde un nivel elegible del árbol existente, el usuario abre «Importar carpetas», elige perfil publicado compatible, agrega raíces, revisa expedientes/documentos, confirma y consulta resultados persistidos por carpeta/archivo. Cada raíz seleccionada representa un expediente bajo ese nivel; los documentos se almacenan en el gabinete y se incorporan al expediente. Nombre visible según regla de carpeta; código según política del perfil, sin concatenación improvisada ni formato fijo de orden.

- Reutilizar árbol, administración de estructuras y permisos existentes. No crear otro explorador, selector de nivel en el asistente ni niveles a partir de carpetas/subcarpetas.
- Capturar ID/ámbito del nivel al abrir; validar en servidor existencia, elegibilidad, permisos y coherencia. Mostrar ruta canónica de solo lectura, obtenida del servidor; no usarla como identidad. Destino ausente/no elegible impide abrir.
- Gabinete, clasificación y nivel de Producción son conceptos distintos. El perfil debe ser compatible y nunca redirigir el nivel. Todas las raíces usan el mismo nivel confirmado; distribuir entre niveles requiere otra capacidad.
- Conservar ID/ámbito en preflight, plan, lote y comando de creación. Cambiar árbol, pestaña o sesión no mueve un lote. Para otro destino, cerrar y abrir una nueva preparación desde el árbol.
- Revalidar al preparar, confirmar y crear. Nivel movido, ámbito cambiado, indisponible o permiso revocado detiene lo afectado para revisión, sin redirección silenciosa.
- La creación recibe `id_nivel_padre`: verificar ID de expediente y relación devuelta con el nivel confirmado. Obtener solo un ID de expediente no acredita ubicación.

Ejemplo no fijo: nivel existente «Gestión de compras» → carpetas 1246566/1246567 → dos expedientes hijos; gabinete «Documentos de compras» almacena sus documentos de forma independiente.

El perfil define si las subcarpetas aportan archivos al mismo expediente o se rechazan; no crear expedientes por nivel físico automáticamente. Rechazar estrategias no implementadas. El inventario de archivos no representa carpetas vacías; informarlo sin fabricarlas. Archivos sueltos requieren política explícita.

Fuera del alcance inicial: rutas servidor/red, servicios externos nuevos, ejecución desatendida con navegador cerrado, acceso automático a archivos locales tras recarga, ZIP sustituto de carpeta y framework general de conectores. Separar origen/perfil/operaciones permite evolucionar sin infraestructura especulativa.

### 3. Reutilización y aislamiento

Mantén intactos la carga individual y SII: código, tablas, configuración, contratos y consumidores. No reutilices intenciones Workflow, tareas ficticias, autorregistro ni relaciones predeterminadas de gabinete para representar el lote nuevo.

Integra una entrada propia «Importar carpetas» en Producción. Reutiliza estilos, controles de catálogo, mensajes y presentación de progreso cuando sus contratos sean compatibles. No copies íntegramente `LoadFilePERSON` ni alteres su estado global para cambiar de expediente por archivo. Su búsqueda por `file.name` no sirve como identidad en este flujo.

Invoca operaciones documentales existentes mediante adaptadores nuevos y acotados, sin modificar SII ni los contratos compartidos. Evalúa `Registrar_Expediente_Conservacion` para creación y `AlmacenamientoDocumentoProduccionDocumental` para almacenamiento con campos índice explícitos. Verifica todos sus efectos y dependencias antes de elegirlos; no basta con que el método acepte el parámetro.

No uses el ASHX compartido como coordinador del lote. Si una operación legacy requiere sesión, documenta exactamente qué lee y modifica. La identidad autenticada puede provenir de la sesión, pero el destino/perfil/archivo proviene del registro persistido del lote. No manipules selecciones globales para simular navegación. Si es imprescindible establecer temporales de compatibilidad durante una llamada, aísla el adaptador, delimita y restaura el contexto en `Finally`, usa rutas por operación y prueba exclusión frente a otras peticiones. No presentes ese adaptador como apto para background. Si no puede aislarse sin afectar otro recorrido, busca una operación inferior compatible y registra la limitación en vez de alterar SII.

### 4. Cliente JavaScript

#### 4.1. Base gráfica y recorrido aprobados

Usar HTML/CSS/JS y README de `Prototipo/` como referencia visual: árbol existente → nivel elegible → «Importar carpetas» → validar contexto → ventana con destino fijo → Perfil → Carpetas → Revisión → confirmar/ejecutar → Resultados → actualizar rama de origen. Sin nivel o permiso no habilitar apertura. Esta decisión sustituye propuestas anteriores de seleccionar nivel dentro del perfil/asistente.

Integrar `Gestion/WebFormProducionDocumental.aspx` y `js/gestion/WebFormProducionDocumental.js` tras verificar contrato del nodo y acción de menú/barra. No copiar el árbol simulado ni convertir `ActivaCargaArchivos` en carga multiexpediente; preservar su contrato individual. La aprobación visual no convierte catálogos, límites, permisos, temporizadores o resultados ficticios en reglas de negocio.

| Parte | Requisito |
|---|---|
| Cabecera/destino | Operación y ruta completa de solo lectura; sin selector ni creación de niveles. |
| Perfil | Versiones publicadas autorizadas compatibles; mostrar clasificación, gabinete, responsable, restricciones y reglas. Cambiarlo conserva nivel e invalida plan. |
| Carpetas | Agregar raíces repetidamente, quitar pendientes, cantidades/tamaños y expedientes propuestos bajo origen. |
| Revisión | Valores resueltos/diferidos, filtros e incidencias por archivo; consumir `puedeConfirmar` según errores globales, unidades elegibles, modo/ámbito. En parcial, un error individual no bloquea todo. Revisar destino y contenido. |
| Resultados | Estados persistidos, IDs reales, avance, fallos parciales y acciones permitidas; distinguir incierto de fallo confirmado. |
| Contexto | Perfil/versión, clasificación, nivel y gabinete visibles; distinguir ubicación del expediente y almacenamiento. |

Reutilizar modal/controles compatibles y adaptar dimensiones sin cambiar recorrido. Encapsular CSS/estado, respetar ClientID y usar `type="button"` sin postback accidental. No duplicar formularios ASP.NET, bibliotecas, eventos ni controles tras UpdatePanel. Inicializar y liberar recursos correctamente al cerrar/reabrir. Eliminar ejemplos, IDs DEMO, selector de escenarios y simulaciones de producción.

#### 4.2. Apertura, cierre y árbol

- El cliente envía referencia del nivel; servidor resuelve usuario/ámbito efectivo desde autenticación y recursos autorizados, devuelve ruta canónica y capacidades. No confiar en identidad cliente ni releer selección mutable de sesión para crear.
- Otro destino requiere nueva preparación desde el árbol. Advertir descarte de preparación sin confirmar; no perder archivos por postback o cambio de perfil.
- Cerrar no cancela peticiones servidor ni borra lote: conservar ID para consulta/reanudación autorizada. Archivos no recibidos pueden requerir reselección; no copiar el descarte de estado de la maqueta para lotes confirmados.
- Actualizar la rama de origen mediante mecanismos existentes, IDs y relaciones confirmados, sin reconstruir árbol, duplicar nodos o alterar selección actual. Si el nivel no está visible, conservar resultados y permitir navegación existente al expediente; no cambiar su vínculo por un problema de presentación.

#### 4.3. Inventario y transferencia

- Implementa «Agregar carpeta» repetible con detección de soporte de `webkitdirectory`/rutas relativas. Verifica los navegadores objetivo; no dependas de multiselección de raíces en el selector nativo. El arrastre de carpetas es opcional y requiere enumeración completa probada, no solo `dataTransfer.files`.
- Mantén una colección propia, con `selectionId`, `rootId`, `clientFileId`, ruta relativa, nombre original, tamaño y `lastModified`, además de la referencia `File`. No reconstruyas esa colección desde filas HTML o `input.files` después de cada cambio.
- Conserva raíces homónimas como selecciones distintas. Una detección de repetición por nombre/tamaño/fecha es solo una advertencia, no una garantía de igualdad de contenido.
- Permite quitar raíces/archivos antes de confirmar. Invalida el preview cuando cambia el inventario, perfil, nivel u otra entrada que afecte el plan. Después de confirmar, no cambies silenciosamente el contenido del lote.
- Usa `textContent` para nombres externos, evita HTML interpolado y libera object URLs. Limita filas y previews simultáneos mediante paginación o renderizado acotado. No cargues todo el lote en memoria como bytes/base64.
- Muestra resumen de expedientes, archivos, bytes, errores por campo y conflictos. La validación cliente ayuda al usuario; el servidor es la autoridad.
- Envía manifiestos con límites de tamaño/cantidad y paginación o fragmentación si son necesarios. La confirmación requiere que el servidor haya recibido y validado el inventario completo.
- Transfiere archivos mediante `FormData` individual, con concurrencia acotada; comienza con una operación documental en vuelo si el adaptador usa sesión. No uses `Promise.all` sin límites.
- Diferencia progreso de transferencia, recepción validada y almacenamiento/incorporación confirmados. No muestres porcentajes de bytes inventados si solo tienes progreso por archivo.
- Gestiona errores HTTP, expiración de sesión, respuesta inválida, timeout y cancelación; toda promesa debe finalizar. Ante pérdida de respuesta consulta estado antes de repetir efectos.

Referencia de la API: https://developer.mozilla.org/en-US/docs/Web/API/HTMLInputElement/webkitdirectory y https://developer.mozilla.org/en-US/docs/Web/API/File/webkitRelativePath. La ruta recibida es relativa; el navegador no entrega al servidor acceso directo al disco del usuario.

### 5. Contratos y recorrido del servidor

Implementa entrada HTTP compatible → Service → Repository/adaptadores → respuesta estructurada. Los nombres siguientes son operaciones conceptuales, no rutas obligatorias:

1. **Abrir desde la estructura y consultar perfiles:** recibir el nivel seleccionado del árbol existente, validar usuario, ámbito, elegibilidad y permisos; consumir perfiles publicados compatibles. Conservar ese contexto explícito durante todo el recorrido.
2. **Preparar importación:** recibir inventario y versión; resolver en servidor agrupación, datos del expediente, tipologías e índices. Devolver plan, revisión/identidad y errores sin crear expedientes ni almacenar documentos definitivos.
3. **Confirmar plan:** comprobar integridad y vigencia, permisos, cuotas y conflictos; persistir de forma idempotente lote, raíces/expedientes propuestos, archivos y configuración efectiva. Asociar el plan al usuario/ámbito; no aceptar un plan editado por el cliente.
4. **Recibir archivo:** comprobar propiedad del lote, estado y archivo esperado; recibir el contenido en staging propio, validar bytes reales y registrar recepción. La ruta física se genera en servidor. No crear aún efectos documentales para un archivo inválido.
5. **Procesar siguiente elemento:** adquirir exclusión persistida, revalidar autoridad y destino, crear/resolver el expediente de su raíz una sola vez, almacenar el documento, verificar incorporación y guardar sus IDs/resultados. Cada operación debe ser acotada; no procesar todo el lote en una petición.
6. **Consultar estado:** devolver avance paginado y resultados persistidos, incluidos estados inciertos que requieren reconciliación.
7. **Reintentar/reconciliar:** repetir únicamente fases seguras y pendientes después de verificar efectos previos.
8. **Cancelar pendientes:** detener nuevas operaciones; consultar/reconciliar las que estaban en vuelo. No eliminar documentos o expedientes ya creados como supuesto rollback.

La primera versión puede ser coordinada por peticiones sucesivas del cliente, conservando estado durable. Cerrar el navegador no garantiza que una petición en curso se detenga. Al volver, consultar estado y reanudar; los archivos aún no transferidos requieren nueva selección y comprobación. No iniciar tareas en memoria dentro de ASP.NET para prometer continuidad desatendida.

### 6. Reglas y validaciones

**Integridad configurable:** por defecto, todos los archivos previstos deben pasar (`COMPLETO_OBLIGATORIO`). `PARCIAL_PERMITIDO` requiere configuración explícita en el perfil publicado y se conserva en el plan/lote. El usuario aprobó ambos ámbitos configurables en el perfil: EXPEDIENTE y LOTE. Modo completo exige selección explícita al publicar; modo parcial usa NO_APLICA. Conservar la elección en el plan/lote sin permitir cambiarla durante ejecución. En modo completo, no iniciar efectos documentales de la unidad exigida hasta recibir y validar todos sus archivos; no marcarla completa hasta confirmar todos los efectos. Un fallo posterior conserva avances y requiere recuperación, sin rollback destructivo. No eliminar rechazados, cancelados o inciertos del total original para simular éxito. Una exclusión anterior a confirmar cambia el inventario e invalida la revisión.

Reutiliza el resolvedor del perfil. Las constantes proceden de la tabla interna, el nombre visible de la carpeta y los índices variables de las fuentes configuradas. No aceptes desde JavaScript valores finales de clasificación, SQL, nombres libres de columnas ni destinos sin validación.

Para `1246566-125-OrdenCompra.pdf`, la extracción por guion es solo un caso de prueba configurable; demuestra también otro formato. Conserva ceros iniciales cuando el tipo de destino lo permita. Resuelve códigos externos de tipología mediante la configuración del perfil, sin modificar equivalencias SII.

Aplica límites funcionales y físicos, campos requeridos, coherencia de clasificación, permisos de creación/carga, pertenencia de campos al gabinete y políticas generales de tamaño/formato. Revalida antes de cada efecto aunque el preview haya sido válido. La disponibilidad de un perfil no concede por sí sola acceso al gabinete o nivel.

Compara conflictos usando la identidad documental definida y su ámbito. No fusionar expedientes automáticamente por nombre. Diferencia una continuación del mismo lote de una importación nueva coincidente.

### 7. Recepción segura y almacenamiento

Rechaza rutas absolutas, traversal, componentes inválidos y colisiones después de normalizar. Conserva nombre/ruta original como metadatos; almacena con identificadores opacos por lote/archivo, contención verificada y sin exposición pública directa. No reproduzcas `SaveAs(path & file.FileName)`.

Comprueba extensión permitida, tamaño real, contenido/formato según validadores disponibles, límites totales y espacio temporal. `accept` y MIME enviados por navegador no son prueba suficiente. Respeta límites IIS/ASP.NET existentes; no los aumentes globalmente sin justificar. Si un tamaño requiere transferencia fragmentada, delimita esa capacidad o recházalo claramente; no prometas soporte ilimitado.

Registra hash del contenido recibido cuando se utilice para integridad/reintento; nombre, fecha y tamaño no sustituyen esa comprobación. Autoriza acceso a temporales y aplica retención/limpieza controlada sin borrar archivos pendientes de reconciliación.

Compón un valor final por campo índice según el perfil antes del almacenamiento. Verifica si la operación reutilizada ya incorpora al expediente, registra inventario e índice electrónico; no dupliques esos efectos. Al finalizar comprueba gabinete, ID de documento y relación con el expediente esperado. Preserva el comportamiento de formatos soportados, incluido Multi-TIFF, o informa explícitamente qué formatos no admite esta primera entrega.

### 8. Persistencia, idempotencia y recuperación

Diseña tablas propias para lote, raíz/expediente del lote, archivo y eventos/intentos necesarios. No reutilices tablas operativas SII. Conserva versión del perfil y configuración efectiva, identidad de usuario/ámbito, revisión del plan, IDs documentales, estados, errores seguros y tiempos.

Define estados y transiciones explícitos, por ejemplo: pendiente, recibiendo, recibido, procesando, completado, fallido, resultado incierto y cancelado. Separa transferencia de resultado documental. No permitas que dos peticiones reclamen el mismo elemento; implementa exclusión y revisiones persistidas compatibles con MySQL 5.1. La expiración de una exclusión no autoriza a repetir ciegamente una operación cuyo efecto sea incierto.

Usa claves únicas de operación para doble confirmación y reenvío del mismo archivo/fase. Una fila de estado o un hash no garantizan por sí solos exactamente una creación: examina la ventana entre persistencia legacy y actualización del lote. Si no existe una correlación fiable para reconciliar, marca resultado incierto y exige resolución explícita, sin reintento automático destructivo o duplicador.

No prometas una transacción única entre MySQL, archivos e índices XML. Guarda avances verificables, reconcilia fallos parciales y conserva evidencia. Distingue fallo antes de persistir, rechazo conocido y resultado desconocido. Los errores de limpieza o de representación en pantalla no deben producir otro almacenamiento.

### 9. SQL MySQL 5.1: entrega manual

Si necesitas crear o actualizar tablas, entrega `Sql/02-carga-carpetas-mysql51.sql` en este directorio, con DDL completo y concreto correspondiente al código. El equipo del usuario lo ejecutará; no aplicar migraciones a la base configurada ni al arrancar la aplicación.

Referencia como precondición las tablas reales del perfil y su script; no recrearlas ni suponer que ya fueron desplegadas. Incluir orden de ejecución, índices, restricciones soportadas, consultas de comprobación de solo lectura y recuperación ante ejecución parcial. Usar InnoDB, tipos/collation compatibles y sintaxis MySQL 5.1; no JSON nativo, `utf8mb4`, columnas generadas ni construcciones modernas no soportadas. No confiar en CHECK como validación efectiva.

No incluir `USE` de una base operativa, credenciales, borrados ni modificaciones SII. Documentar si admite repetición y cómo detectar diferencias de estructura; `IF NOT EXISTS` no verifica compatibilidad. El DDL puede causar commits implícitos: no prometer rollback transaccional de esquema. Los scripts deben quedar listos para revisión manual, no como pseudocódigo.

### 10. Verificación y criterios de aceptación

Verificar comportamiento, no solo búsquedas de texto:

- Sin nivel válido/permiso no se inicia; servidor rechaza referencia manipulada. Apertura única, ruta de solo lectura, sin segundo selector ni niveles nuevos.
- Cambio de perfil conserva nivel e invalida plan; perfiles incompatibles impiden confirmar. Otra pestaña/sesión no altera destino. Cerrar/reabrir desde otro nivel crea preparación nueva, sin modificar lotes ni duplicar eventos/solicitudes.
- Resultados actualizan rama mediante IDs reales, sin nodos duplicados ni alterar navegación. Producción no contiene «Cargar ejemplo», escenarios ficticios, IDs DEMO o temporizadores simulados.
- Dos raíces generan dos expedientes relacionados con nivel confirmado y documentos en destino correcto. Nivel eliminado/movido/sin permiso bloquea lo afectado para revisión.
- Homónimos en raíces/subcarpetas conservan identidad/contenido; raíces homónimas no se fusionan. Subcarpetas siguen política y se informa limitación de vacías.
- Inventario/perfil cambiado invalida plan; rechazar planes ajenos, incompletos o manipulados. Doble confirmación, reenvío y clientes concurrentes no repiten efectos confirmados.
- Fallo tras crear/almacenar antes de responder conserva IDs o estado incierto, sin reintento ciego. Sesión perdida, cierre/recarga/cancelación permiten consultar sin prometer recuperar bytes locales no enviados.
- Rechazar ruta maliciosa, exceso real de tamaño, gabinete no autorizado y tipología inválida. Revalidar cambios de permiso, perfil y esquema de gabinete.
- Índices extraídos siguen perfil y no alteran relaciones compartidas. Éxito exige almacenamiento e incorporación verificados, no HTTP 200 ni transferencia finalizada.
- Carga individual operativa y SII sin cambios. Ejecutar DOC-88 pertinente distinguiendo evidencia estática; pruebas reales sujetas a autorización/runbook. Reportar no ejecutadas y cómo completarlas.

### 11. Orden de entrega

1. Diseño 04 y perfil 02: contratos, puntos reutilizables y decisiones pendientes.
2. Fundaciones 07-A: SQL/repositorios de plan y operación, sin esperar al servicio de preflight.
3. Preflight 06 y confirmación/recepción 07-B; la interfaz 05 avanza contra esos contratos.
4. Adaptadores 08/09 y coordinación 10, con caracterización de efectos y recuperación.
5. Integración final de interfaz 05 y seguimiento 11, incluyendo cierre/reanudación y árbol.
6. Verificación 12 y actualización de exploración. 13 sigue siendo opcional.

Trabaja hasta completar el alcance autorizado; no presentes la interfaz de selección como una importación terminada. Si falta una dependencia o una capacidad de reconciliación, documenta el bloqueo específico y completa las partes independientes sin inventar garantías. Entrega archivos afectados, contratos, SQL, resultados de pruebas, procedimiento de validación y evidencia de que SII no cambió.

## CONTEXTO Y CONTRATO DE INTEGRACIÓN

Leer guía 00 y diseño vigente. Documentar request/response, autoridad, errores, estados y efectos: Cliente → ASMX/ASHX (responsabilidad Controller) → Service → Repository/adaptadores → respuesta. No introducir MVC ni endpoints públicos de adaptadores 08/09.

## PRECONDICIONES DE RUTAS

Antes de implementar, inventariar archivos exactos, propietario, responsabilidad, consumidores y condición nuevo/existente en `Diseno/design.md`. Complementa modelo/contratos de `Diseno/`; OpenSpec lo referencia sin duplicarlo. Verificar estas rutas propuestas; no crear todas las capas por entrega ni afirmar archivos existentes:

| Responsabilidad | Ubicación común propuesta | Propiedad |
|---|---|---|
| Modelos y puertos | `Modelo/ProduccionDocumental/ImportacionCarpetas/` | 04 define; cada entrega añade solo su contrato. |
| DTOs de entrada/salida | `DTOs/ProduccionDocumental/ImportacionCarpetas/` | 02/06/07/10/11 según operación. |
| Servicios y reglas | `Services/ProduccionDocumental/ImportacionCarpetas/` | 02 perfil; 06 plan; 07 recepción; 10 coordinación; 11 consulta/mantenimiento. |
| Repositorios | `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/` | 02 perfil; 07-A operación. Los demás consumen sus interfaces. |
| Adaptadores documentales | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/` | 08 expedientes; 09 almacenamiento e incorporación. |
| Servicio web nuevo | `webservice/WebServiceImportacionCarpetas.asmx` y `webservice/WebServiceImportacionCarpetas.asmx.vb` | Una fachada de las operaciones anteriores; sin reglas documentales en transporte. |
| Recepción multipart nueva | `generic_control/ImportacionCarpetasUpload.ashx` y `generic_control/ImportacionCarpetasUpload.ashx.vb` | 07-B; delega en su servicio; no sustituye el handler compartido. |
| Integración visual existente | `Gestion/WebFormProducionDocumental.aspx`, su code-behind `.aspx.vb` y `js/gestion/WebFormProducionDocumental.js` | 05 integra; 11 extiende resultados sobre la misma ventana. |
| JavaScript específico nuevo | `js/gestion/ImportacionCarpetas/` | 05 estado/transporte/renderizado; 11 seguimiento; nombres exactos en el inventario. |

Registrar compilación/contenido en `GestionDocumental-Docuarchi.net.vbproj`, manteniendo namespaces y referencias .NET Framework 4.6.1. Inventariar host del 13 solo tras decidir alojamiento; no presumir un ejecutable desplegado.

## RESTRICCIONES CRÍTICAS

Aplicar restricciones de las secciones 2–9: SII y carga individual intactos; sin árboles, motores o repositorios paralelos, autorregistro, tareas ficticias ni valores DEMO productivos. No alterar pruebas/huellas para ocultar regresiones. COMPLETO_OBLIGATORIO requiere elegir EXPEDIENTE/LOTE al publicar y congelarlo en plan/lote; PARCIAL_PERMITIDO usa NO_APLICA. Preservar efectos y reconciliar; sin rollback global.

SQL manual MySQL 5.1: perfil pertenece a 02; operación a 07. Cambios posteriores usan incrementos coordinados; existir un script o conexión no acredita ni autoriza aplicación.

## CRITERIOS DE ACEPTACIÓN

Cumplir la matriz de la sección 10 y REC-01/02 en completo/EXPEDIENTE, completo/LOTE y parcial: dos raíces en destino autorizado, índices correctos y repetición/recuperación sin duplicación.

## PRUEBAS OBLIGATORIAS

Pruebas unitarias o QA manual reproducible según impacto e integración para persistencia/efectos; registrar comandos, entorno, resultados y limitaciones. Compilar cambios VB/proyecto con MSBuild y configuración real del vbproj, no build .NET moderno; documentar herramientas/referencias ausentes. Verificar JavaScript con `node --check <archivo-real>` y pruebas de comportamiento. DOC-88 acredita regresión estática, no persistencia real.

E2E, código, validación autorizada y evidencia saneada son parte integral del mismo cambio. 12 consolida regresión, no sustituye pruebas por componente. Adaptadores internos se prueban primero por contrato/integración, sin endpoints auxiliares; dependencias faltantes dejan integración pendiente, no aprobada.

Antes de autenticar, leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`. Requiere autorización explícita de ambiente, cuentas y datos descartables, no concedida por este prompt. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión.

Secretos efímeros: no imprimir ni guardar credenciales, cookies, tokens o cadenas de conexión. Controles solo SELECT y evidencia saneada. Respetar gates existentes, sin habilitar arbitrariamente permisos/funciones ni reintroducir flags o listas de usuarios/grupos para activar la experiencia moderna. No cerrar sin validación requerida: registrar bloqueo explícito, motivo y casos no ejecutados; mocks/simulaciones no sustituyen evidencia real.

## E2E POR CAPACIDAD Y CRITERIO DE CIERRE

### Reutilización obligatoria de infraestructura E2E

Inventariar y reutilizar `tools/e2e/package.json`, `tools/e2e/playwright.config.cjs`, `tools/e2e/tests/support/`, `tools/e2e/scripts/`, `tools/e2e/fixtures/` y runbook. Especificaciones nuevas en `tools/e2e/tests/`, comandos en el paquete existente y reporteros/directorios configurados; sanear capturas/trazas.

Verificar compatibilidad de `tools/e2e/tests/support/authenticated-workflow-session.cjs` con Producción; no fabricar tareas Workflow. Si falta soporte, extender autenticación/configuración/runner compartidos de forma compatible y probar consumidores afectados. Documentar validadores de autorización, perfiles y utilidades reutilizados y extensión mínima necesaria.

No copiar suites. No crear login, sesión, .env, dependencias, instalación de navegadores, runner o configuración Playwright paralelos. Scripts nuevos en `tools/e2e/scripts/` solo seleccionan escenarios y delegan en mecanismos compartidos, sin reimplementar autenticación/autorización. No cambiar globalmente workers, reintentos, timeouts, permisos o configuración para lograr pruebas verdes. Toda extensión preserva contratos y acredita regresión pertinente. DOC-88 de política complementa, no reemplaza E2E real. Inventario de reutilización y ausencia de duplicación son criterios de aceptación; 04 los documenta y 13 actúa solo al activar alcance.

Implementar estos escenarios como parte de la entrega, reutilizando `tools/e2e`. Son pruebas futuras: esta documentación no acredita suites creadas ni corridas. Leer [la estrategia de comprobación progresiva](Validacion/01-estrategia-e2e-capacidades.md).

| ID | Precondiciones | Pasos del recorrido | Aserciones obligatorias |
|---|---|---|---|
| REC-01 | 02/05–11 integrados; nivel y gabinete descartables autorizados. | Desde el árbol abrir importación; seleccionar perfil y dos carpetas; revisar, confirmar, recibir, avanzar y consultar resultados. | Dos expedientes bajo el nivel original; archivos en gabinete esperado e incorporados; índices coinciden con perfil; totales coinciden con inventario. |
| REC-02 | Lote anterior confirmado. | Repetir confirmación y consultas; cerrar y reabrir ventana; recuperar lote por ID. | Mismos IDs y efectos; no duplicar expedientes/documentos; destino original permanece. |

Para cada caso registrar estado EJECUTADO_OK, FALLIDO o NO_EJECUTADO, SHA probado, ambiente, datos sintéticos, pasos/comando real, evidencia saneada de UI/HTTP y comprobaciones persistidas de solo lectura. No declarar aprobada una capacidad únicamente con mocks o revisión estática. Un caso bloqueado debe identificar dependencia y responsable; no se omite del informe.

REC son escenarios de aceptación del conjunto: 12 los ejecuta sobre la integración. No crear otra suite que duplique los escenarios especializados; agrupar/reutilizar los mismos casos.

Entregar especificación automatizada y comando reproducible dentro de la infraestructura existente; el nombre final se inventaría en `Diseno/design.md`. Si la autorización no permite ejecutarla, entregar suite y evidencia local pertinente, manteniendo la validación real pendiente. No habilitar permisos ni ejecutar cargas por inferencia de este prompt.

## DOCUMENTACIÓN TÉCNICA

Actualizar documentación existente en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`: `Diseno/` para contratos y decisiones, y `Validacion/` para evidencia. Mantener guía 00 y consumidores sincronizados si cambia un contrato. No abrir una segunda fuente de verdad. Registrar archivos y funciones afectados, dependencia propietaria, casos ejecutados/no ejecutados y riesgos pendientes. Una revisión estructural PASS no certifica implementación ni ausencia de regresiones.

## ENTREGABLE FINAL

Integrar el recorrido de importación desde el nivel seleccionado de Producción mediante las entregas 04–12, sin una implementación paralela a los prompts especializados. Entregar los artefactos especificados en este prompt y una relación verificable de cambios, pruebas, resultados y pendientes. No declarar funcionalidades completas por entregar únicamente stubs, mocks o documentación.
