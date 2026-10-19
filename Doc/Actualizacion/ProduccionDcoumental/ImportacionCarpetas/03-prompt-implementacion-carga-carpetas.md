# Prompt de implementación: carga documental por carpetas

Fecha: 2026-10-19. Alcance: implementación posterior del recorrido de importación desde dispositivo. Este documento no indica que la funcionalidad ni sus tablas ya existan.

Este es el prompt general del recorrido. Su implementación se descompone en los prompts 04–12 según [00-guia-ejecucion-prompts.md](00-guia-ejecucion-prompts.md); el 13 es opcional y requiere activar ese alcance. Al ejecutar un prompt especializado, este documento aporta contexto y requisitos transversales, no obliga a implementar a la vez todas las entregas. No duplicar repositorios, contratos ni scripts entre prompts.

## Prompt

Actúa como arquitecto senior y desarrollador especializado en JavaScript, ASP.NET WebForms, VB.NET y MySQL. Implementa la importación documental por carpetas desde dispositivos en Producción Documental, reutilizando las capacidades compatibles del repositorio y preservando íntegramente SII.

### 1. Contexto obligatorio

Lee AGENTS.md y las instrucciones aplicables. Lee, desde este directorio:

- `02-prompt-implementacion-perfil-importacion.md`.
- `Exploracion/00-exploracion-importacion-masiva-por-carpetas.md`.
- `Exploracion/01-diagnostico-reutilizacion-tablas.md`.
- `Exploracion/02-evidencia-esquema-perfil.json`.
- `Exploracion/03-validacion-bd-y-definicion-perfil.md`.
- `Exploracion/04-diagrama-flujo-tecnico-perfil.md`.
- `Exploracion/05-contexto-tecnico-carga-carpetas.md`.
- `Prototipo/README.md` y `Prototipo/importacion-carpetas.html`: base gráfica interactiva. Preservar el recorrido visual y sustituir ejemplos, validaciones locales y ejecución simulada por contratos reales; no copiar sus datos fijos como reglas de negocio.

El documento 03 de la exploración precisa las hipótesis iniciales sobre tablas. Los diagramas incluyen propuestas, no contratos implementados. Verifica el estado actual del código y del perfil antes de diseñar.

Inspecciona los símbolos y rutas de la matriz del documento 05. No presupongas ASP.NET Core/MVC, React, una API nueva desplegada ni un worker disponible. El proyecto revisado utiliza WebForms/VB.NET, framework 4.6.1, JavaScript, servicios ASMX y ASHX.

Dependencia: esta carga consume una versión publicada del perfil. Si su implementación aún no existe, identifica los contratos necesarios y el orden de entrega; avanza en componentes independientes y pruebas mediante dobles explícitos. No declares la carga completa ni construyas configuración paralela o fija para eludir esa dependencia. La administración de perfiles pertenece al prompt anterior.

### 2. Resultado funcional

El usuario selecciona un nivel en la estructura existente de Producción Documental y abre desde allí la ventana «Importar carpetas». Dentro de ella selecciona un perfil autorizado compatible, agrega una o varias carpetas, revisa expedientes/documentos propuestos, confirma y obtiene resultados persistidos por carpeta y archivo. Los expedientes se crean bajo el nivel recibido; los documentos se almacenan en el gabinete y quedan incorporados al expediente correspondiente.

Implementa como modalidad inicial propuesta «cada raíz seleccionada representa un expediente». El nombre visible procede de la carpeta según la regla del perfil; el código único se resuelve por su política, no por una concatenación improvisada. No confundas nombre con identidad ni fijes formato de orden de trabajo.

**Requisito confirmado: los expedientes deben crearse dentro de una estructura específica de Producción Documental.** Resuelve explícitamente el nivel padre existente, su identificador y su ámbito. El gabinete identifica dónde se almacenan los documentos; no sustituye la ubicación del expediente dentro del árbol de Producción. La clasificación documental tampoco determina por sí sola esa ubicación.

- El usuario ya explora y administra la estructura mediante el árbol existente de Producción. Selecciona allí un nivel elegible y desde ese contexto pulsa «Importar carpetas» para abrir la ventana. Reutilizar esa selección y sus permisos; no desarrollar otro explorador, creador de estructuras o selector de nivel dentro del asistente.
- Al abrir, capturar explícitamente ID y ámbito del nivel seleccionado, verificarlos en servidor y mostrar la ruta completa como destino de solo lectura. El perfil debe ser compatible con ese contexto; no lo sustituye ni redirige hacia otro nivel. Mostrar una vista previa de los expedientes bajo él. No abrir la operación con destino ausente o no elegible.
- En esta primera modalidad, todas las raíces seleccionadas crean expedientes bajo el mismo nivel confirmado. No convertir automáticamente carpetas/subcarpetas físicas en niveles nuevos de Producción. Una distribución de raíces entre varios niveles requiere otra capacidad explícita.
- Incluir el ID del nivel padre y su ámbito en preflight, plan confirmado, registro del lote y comando de creación. La ruta visible se obtiene del servidor; no se usa como clave de identidad. Para otro destino, cerrar la ventana, seleccionar otro nivel en el árbol y abrir una nueva preparación. Nunca cambiar un lote confirmado al cambiar la selección del árbol o la sesión.
- Verificar permisos, existencia, elegibilidad y coherencia del nivel al preparar, confirmar y crear cada expediente. Si el nivel cambia de ubicación/ámbito o deja de estar disponible, detener las operaciones afectadas para revisar el destino; no redirigirlas silenciosamente.
- La operación existente de creación recibe `id_nivel_padre` y devuelve identificadores de relación. Verificar después de crear que el expediente quedó vinculado al nivel confirmado; no considerar suficiente obtener un ID de expediente.

Ejemplo ilustrativo de estructura resultante:

```text
Producción Documental
└── Archivo de gestión
    └── Área administrativa
        └── Gestión de compras             ← nivel padre existente, seleccionado
            ├── 1246566                    ← expediente creado por la primera carpeta
            └── 1246567                    ← expediente creado por la segunda carpeta

Gabinete Documentos de compras             ← almacenamiento documental independiente
└── Documentos incorporados a cada expediente anterior
```

El perfil debe expresar explícitamente si las subcarpetas aportan sus archivos al mismo expediente o si esa estructura no se admite. No crear un expediente por cada nivel automáticamente. Rechazar estrategias aún no implementadas. No importar carpetas vacías mediante un inventario que solo entrega archivos ni aceptar archivos sueltos sin una política definida.

Fuera de alcance inicial: lectura de rutas de servidor/red, servicios externos nuevos, ejecución desatendida con el navegador cerrado, selección automática de archivos locales después de recargar, ZIP como sustituto de carpeta y un framework general de conectores. La separación entre origen, perfil y operaciones documentales debe permitir una extensión posterior sin introducir ahora infraestructura especulativa.

### 3. Reutilización y aislamiento

Mantén intactos la carga individual y SII: código, tablas, configuración, contratos y consumidores. No reutilices intenciones Workflow, tareas ficticias, autorregistro ni relaciones predeterminadas de gabinete para representar el lote nuevo.

Integra una entrada propia «Importar carpetas» en Producción. Reutiliza estilos, controles de catálogo, mensajes y presentación de progreso cuando sus contratos sean compatibles. No copies íntegramente `LoadFilePERSON` ni alteres su estado global para cambiar de expediente por archivo. Su búsqueda por `file.name` no sirve como identidad en este flujo.

Invoca operaciones documentales existentes mediante adaptadores nuevos y acotados, sin modificar SII ni los contratos compartidos. Evalúa `Registrar_Expediente_Conservacion` para creación y `AlmacenamientoDocumentoProduccionDocumental` para almacenamiento con campos índice explícitos. Verifica todos sus efectos y dependencias antes de elegirlos; no basta con que el método acepte el parámetro.

No uses el ASHX compartido como coordinador del lote. Si una operación legacy requiere sesión, documenta exactamente qué lee y modifica. La identidad autenticada puede provenir de la sesión, pero el destino/perfil/archivo proviene del registro persistido del lote. No manipules selecciones globales para simular navegación. Si es imprescindible establecer temporales de compatibilidad durante una llamada, aísla el adaptador, delimita y restaura el contexto en `Finally`, usa rutas por operación y prueba exclusión frente a otras peticiones. No presentes ese adaptador como apto para background. Si no puede aislarse sin afectar otro recorrido, busca una operación inferior compatible y registra la limitación en vez de alterar SII.

### 4. Cliente JavaScript

#### 4.1. Base gráfica y recorrido aprobados por el usuario

Usa `Prototipo/importacion-carpetas.html`, su CSS, JavaScript y README como referencia de interacción. El usuario aprobó el flujo corregido que abre la importación desde la estructura existente. Esta decisión prevalece sobre menciones anteriores a seleccionar el nivel dentro del perfil o del asistente. La aprobación visual no convierte los catálogos, permisos, límites, temporizadores ni resultados ficticios de la maqueta en requisitos de negocio.

```text
Producción Documental: árbol y navegación existentes
    │
    ├─ Sin nivel elegible / sin permiso → no habilitar importación
    │
    └─ Nivel seleccionado → acción «Importar carpetas»
                              │
                              ▼
                   Validar contexto en servidor
                              │
                              ▼
                   Abrir ventana con destino fijo
                              │
                  Perfil → Carpetas → Revisión
                              │
                              ▼
                      Confirmar y ejecutar
                              │
                              ▼
                    Resultados por documento
                              │
                              ▼
                Actualizar el nivel de origen en el árbol
```

Concreta la integración en `Gestion/WebFormProducionDocumental.aspx` y `js/gestion/WebFormProducionDocumental.js`, después de identificar el contrato real del nodo seleccionado y la acción de menú/barra adecuada. No copies la pantalla de navegación simulada del HTML: ya existe en el módulo. No uses `ActivaCargaArchivos` como si fuera importación multiexpediente; su carga individual permanece disponible y sin cambio de contrato.

| Parte de la ventana | Comportamiento requerido |
|---|---|
| Cabecera y destino | Nombre de la operación, ruta completa del nivel recibido y destino de solo lectura. Sin selector de estructura ni botones para crear niveles. |
| Paso 1: Perfil | Listar versiones publicadas autorizadas y compatibles con el nivel recibido. Mostrar clasificación, gabinete, responsable, restricciones y reglas. Cambiar perfil conserva el nivel y obliga a recalcular el plan. |
| Paso 2: Carpetas | Agregar carpetas repetidamente, quitar pendientes, mostrar cantidades/tamaños y expedientes propuestos bajo el nivel de origen. |
| Paso 3: Revisión | Mostrar valores resueltos o pendientes de creación, detalle por archivo, filtros e incidencias. Consumir `puedeConfirmar` del servidor según errores globales, unidades elegibles, modo y ámbito de integridad; no bloquear todo por cualquier incidencia individual si el perfil permite parcial. Exigir revisión del destino y contenido. |
| Paso 4: Resultados | Mostrar estados persistidos, identificadores reales, avance, fallos parciales y acciones permitidas según el estado. Distinguir resultado incierto de fallo confirmado. |
| Panel de contexto | Mantener visibles perfil/versión, clasificación, nivel de Producción y gabinete; distinguir ubicación del expediente de almacenamiento de documentos. |

Reutiliza el modal y controles compatibles del proyecto. La implementación puede adaptar dimensiones y estilos del prototipo para encajar en el módulo, manteniendo su recorrido. Encapsula CSS y estado JavaScript; usa `type="button"` donde no corresponda postback y contempla ClientID generado por WebForms. No dupliques formularios ASP.NET, bibliotecas, handlers de eventos ni controles tras un UpdatePanel. Verifica la inicialización y liberación de recursos al abrir, cerrar y abrir nuevamente.

#### 4.2. Contexto de apertura, cierre y actualización del árbol

- La acción captura el identificador del nivel seleccionado y solicita validación de servidor. Usuario y ámbito efectivo se resuelven desde la autenticación y los recursos autorizados; no se confía en valores enviados por el navegador. El servidor devuelve la ruta canónica y los permisos/capacidades pertinentes.
- Conserva el contexto validado en la preparación y posteriormente en el lote. No releas una variable de sesión de «nivel seleccionado» para decidir cada creación. Navegar en otra pestaña no puede cambiar el destino de un lote abierto.
- Para otro nivel, cerrar la ventana y abrir una nueva preparación desde el árbol. Si hay una preparación local sin confirmar, comunicar su descarte cuando corresponda; no perder archivos seleccionados silenciosamente por un postback o por cambiar de perfil.
- Cerrar una ventana no equivale a cancelar operaciones del servidor ni a borrar el lote. Si hay trabajo confirmado, conservar su identidad para consultar/reanudar con autorización. Los archivos locales aún no recibidos pueden requerir nueva selección. La maqueta descarta estado al reabrir; no copiar esa limitación como comportamiento de producción para lotes confirmados.
- Al finalizar o al recibir resultados confirmados, actualizar mediante el mecanismo existente la rama del nivel de origen y sus expedientes creados. No reconstruir todo el árbol, cambiar la selección actual del usuario o agregar nodos basándose únicamente en el nombre de la carpeta. Usar IDs y relaciones confirmadas, y evitar nodos duplicados al consultar nuevamente el lote.
- Si el nivel no está visible al terminar, conservar los resultados y permitir ubicar el expediente mediante navegación existente. No redirigirlo ni vincularlo a otro nivel para resolver un problema de presentación.

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

Prueba reglas y coordinación con pruebas de comportamiento, no solo búsquedas de texto:

- Sin selección válida o sin permiso no se inicia la importación; una petición directa con un nivel manipulado es rechazada en servidor.
- Seleccionar un nivel y pulsar la acción existente abre una única ventana con su ruta de solo lectura. No aparece un segundo selector de estructura ni se crean niveles.
- Cambiar de perfil conserva el nivel de origen; perfiles incompatibles no permiten confirmar. Cambiar la selección en otra pestaña no altera el plan ni el lote.
- Cerrar y reabrir desde otro nivel inicia una preparación con el nuevo contexto sin modificar lotes anteriores. Abrir repetidamente no duplica eventos ni solicitudes.
- Los resultados confirmados actualizan la rama de origen con IDs reales sin duplicar nodos, sin recrear la estructura y sin alterar la navegación actual del usuario.
- La versión productiva elimina los botones «Cargar ejemplo», el selector de escenarios, IDs DEMO y temporizadores de simulación. Los errores y resultados provienen de operaciones reales y verificables.

- Dos raíces producen dos expedientes y cada documento queda en el destino correspondiente.
- Los dos expedientes quedan relacionados con el nivel padre confirmado de Producción. Cambiar la selección del árbol en otra pestaña no cambia el destino del lote; un nivel eliminado, movido o sin permiso bloquea la operación afectada y requiere revisión.
- Archivos homónimos en raíces/subcarpetas distintas conservan identidad y contenido.
- Dos raíces con igual nombre no se fusionan silenciosamente.
- Subcarpetas siguen la estrategia explícita; se informa la limitación de carpetas vacías.
- Cambiar inventario o perfil invalida el plan; no se acepta un plan ajeno, incompleto o manipulado.
- Doble confirmación, reenvío y dos clientes concurrentes no repiten operaciones confirmadas.
- Fallo después de crear expediente o almacenar documento y antes de responder conserva IDs o produce un estado incierto sin reintento ciego.
- Pérdida de sesión, cierre/recarga y cancelación permiten consultar resultados sin prometer recuperación de archivos locales no enviados.
- Ruta maliciosa, tamaño real excedido, gabinete no autorizado y tipología inválida se rechazan en servidor.
- El servidor vuelve a validar cuando cambian permisos, disponibilidad del perfil o esquema del gabinete.
- Los valores de nombre de archivo se aplican al índice correcto según perfil y no alteran relaciones compartidas.
- El éxito requiere almacenamiento e incorporación verificados; no basta con HTTP 200 o transferencia finalizada.
- La carga individual sigue funcionando y SII permanece sin cambios.

Ejecuta las verificaciones DOC-88 pertinentes como regresión y distingue su alcance estático. Compila con herramientas reales del proyecto. Pruebas autenticadas o cargas reales requieren autorización del ambiente/cuentas y el runbook aplicable; no realizarlas por tener conexión disponible. No guardar credenciales, cookies ni cadenas de conexión. Reporta con precisión las pruebas no ejecutadas y cómo completarlas.

### 11. Orden de entrega

1. Diseño 04 y perfil 02: contratos, puntos reutilizables y decisiones pendientes.
2. Fundaciones 07-A: SQL/repositorios de plan y operación, sin esperar al servicio de preflight.
3. Preflight 06 y confirmación/recepción 07-B; la interfaz 05 avanza contra esos contratos.
4. Adaptadores 08/09 y coordinación 10, con caracterización de efectos y recuperación.
5. Integración final de interfaz 05 y seguimiento 11, incluyendo cierre/reanudación y árbol.
6. Verificación 12 y actualización de exploración. 13 sigue siendo opcional.

Trabaja hasta completar el alcance autorizado; no presentes la interfaz de selección como una importación terminada. Si falta una dependencia o una capacidad de reconciliación, documenta el bloqueo específico y completa las partes independientes sin inventar garantías. Entrega archivos afectados, contratos, SQL, resultados de pruebas, procedimiento de validación y evidencia de que SII no cambió.
