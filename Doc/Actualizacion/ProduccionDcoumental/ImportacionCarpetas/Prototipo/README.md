# Base gráfica de importación por carpetas

Abre [importacion-carpetas.html](importacion-carpetas.html) directamente en un navegador de escritorio. Conserva los archivos HTML, CSS y JavaScript en este mismo directorio. No requiere instalación, servidor, conexión a internet ni autenticación.

## Qué representa

Propuesta visual basada en Producción Documental: navegación lateral por estructura, tonos azules/grises, tablas de documentos, acciones de carga y contexto del expediente/gabinete. Es una vista nueva de importación, no una copia exacta ni una modificación de la pantalla desplegada. La navegación lateral es contextual e ilustrativa.

El recorrido implementado visualmente es Árbol existente de Producción → Seleccionar nivel → Abrir ventana de importación → Perfil → Carpetas → Revisión → Resultados. Se puede seleccionar un directorio real para inventariar nombres, rutas relativas y tamaños; no se lee el contenido, no se sube y no se guarda información persistente. Los perfiles, permisos, catálogos, conflictos, IDs y resultados documentales son ficticios. La recarga descarta el estado.

## Recorrido recomendado

1. Seleccionar «Gestión de compras» o «Correspondencia» en el árbol de Producción. Pulsar «Importar carpetas»: se abre la ventana con ese destino de solo lectura. Elegir «Órdenes de trabajo» y consultar sus reglas. El nivel padre es independiente del gabinete de documentos.
2. Continuar a Carpetas y pulsar «Cargar ejemplo» para agregar dos raíces con cuatro documentos. También se puede usar «Agregar carpeta» repetidamente.
3. Revisar los documentos y abrir «Ver» para comprobar expediente, tipología e índices resueltos.
4. Cambiar el escenario a «Conflicto de expediente» para ver cómo se bloquea la confirmación.
5. Cambiar a «Respuesta perdida al almacenar», marcar la revisión y confirmar la simulación.
6. Consultar el resultado incierto y pulsar «Simular verificación del resultado». La demostración encuentra el documento; no lo vuelve a almacenar. Este resultado está programado para enseñar el flujo, no implementa reconciliación real.
7. Exportar el CSV de resultados ficticios o iniciar una nueva importación.

El perfil «Expedientes administrativos» muestra una segunda interpretación del nombre: descripción completa y tipología constante. Los separadores, campos y límites de la maqueta son datos de ejemplo, no una especificación rígida para producción.

La estructura resultante se muestra en Carpetas, Revisión y Resultados: cada raíz propone un expediente bajo el nivel recibido al abrir. No existe selector de estructura dentro de la ventana. Para otro destino se cierra, se selecciona otro nodo del árbol y se abre una nueva preparación. En la maqueta, esa nueva apertura descarta el inventario local anterior; no se eliminan efectos documentales porque todos son simulados. «Nueva importación» dentro de la ventana conserva el nivel de origen. El lote simulado conserva una copia de su destino; el cambio de perfil no modifica el nivel. En implementación se validarán perfiles compatibles y los permisos del contexto en servidor.

## Archivos y capturas

| Archivo | Función |
|---|---|
| [importacion-carpetas.html](importacion-carpetas.html) | Estructura, navegación de etapas, panel de destino y diálogo accesible. |
| [importacion-carpetas.css](importacion-carpetas.css) | Presentación autónoma adaptable a escritorio y móvil, sin CDN. |
| [importacion-carpetas.js](importacion-carpetas.js) | Estado local, selección de archivos, ejemplos, validación ilustrativa y ejecución simulada. |
| [vista-estructura.png](vista-estructura.png) | Selección en el árbol de Producción y acción para abrir la importación. |
| [vista-perfil.png](vista-perfil.png) | Primera etapa en escritorio. |
| [vista-revision.png](vista-revision.png) | Inventario, validación y confirmación. |
| [vista-resultados.png](vista-resultados.png) | Resultado parcial con operación por verificar. |
| [vista-movil.png](vista-movil.png) | Adaptación móvil y ejemplo de archivos homónimos. |

## Mapeo para implementar en ASP.NET

La política completa/parcial se detalló después de este prototipo: su simulación secuencial y bloqueo de todos los errores no implementan esa política. En la implementación prevalece la guía 00: recibir/validar todos los archivos de la unidad exigida antes de sus efectos, o procesar elegibles si el perfil permite parcial. Ambos ámbitos, por expediente y por lote, están aprobados como configuración del perfil; esta maqueta no implementa sus barreras reales. El HTML es base gráfica aprobada, no referencia ejecutable de atomicidad, idempotencia o reconciliación real.

| Elemento visual o función | Integración real requerida |
|---|---|
| Selector `profile` y panel de destino | Consultar perfiles publicados autorizados y su versión desde el servidor. Reemplazar el objeto `profiles` de ejemplo. |
| Acción `open-import` desde el árbol existente | Recibir ID/ámbito del nivel seleccionado, verificar elegibilidad y permisos, abrir la ventana con destino de solo lectura y conservarlo en el plan. Verificar la relación expediente–nivel después de crear. No implementar otro selector ni creador de estructuras. |
| Colección de raíces/archivos y `selectedFolders` | Conservar IDs de selección y referencia `File`; enviar manifiesto validado y obtener IDs persistidos. No identificar únicamente por nombre. |
| `validate`, reglas y detalle | Sustituir validación ilustrativa por preflight del servicio. El cliente puede anticipar errores, pero no autorizar ni decidir el destino final. |
| Confirmación | Enviar identidad/revisión de un plan completo asociado al usuario. Confirmar de manera idempotente en servidor. |
| `startSimulation`, `tick`, `markDone` | Sustituir completamente por recepción segura, ejecución acotada y consulta de estado persistido. Los temporizadores actuales solo animan la maqueta. |
| Pausa, cancelación y verificación | Implementar contratos de pendientes/operaciones en vuelo y reconciliación real; no suponer cancelación de efectos por detener JavaScript. |
| IDs y progreso | Mostrar identificadores reales y distinguir bytes recibidos de efectos documentales confirmados. |
| Exportación | Generar a partir de resultados autorizados y persistidos; mantener protección frente a fórmulas en CSV. |

Integrar en la arquitectura WebForms/ASMX/ASHX existente según el prompt, evitando copiar el documento completo dentro de una página con otro formulario. Adaptar selectores si WebForms genera ClientID, usar botones que no provoquen postbacks accidentales y asegurar inicialización única o limpieza de eventos tras actualizaciones parciales. Encapsular estilos bajo el contenedor del módulo antes de incorporarlos a la aplicación: esta maqueta usa selectores globales porque es autónoma. Evaluar compatibilidad del diálogo nativo con los navegadores objetivo o adaptarlo al modal existente.

La interfaz no decide permisos, clasificación, coincidencias, tablas ni rutas físicas. Reutilizar operaciones documentales mediante adaptadores compatibles; mantener SII intacto. La implementación de tablas se entrega en SQL MySQL 5.1 para ejecución manual del equipo, conforme al [prompt de carga](../03-prompt-implementacion-carga-carpetas.md).

## Verificación realizada

Se ejecutó una prueba local con Chromium headless/Playwright sobre el archivo HTML, sin autenticación ni conexión al sistema. Comprobó navegación, ejemplo, consulta de detalle, conflicto que bloquea confirmación, resultado incierto y verificación simulada, pausa/reanudación, selección real de una carpeta de prueba, archivos homónimos en subcarpetas, rechazo de extensión, cancelación y descarga CSV. Se comprobó ausencia de errores JavaScript y tráfico HTTP; a 390 px no hubo desbordamiento horizontal de página. Se revisaron visualmente las capturas de escritorio y móvil.

Esto no certifica integración ASP.NET, permisos, carga real, acceso a base de datos, rendimiento masivo ni compatibilidad de selección de carpetas en otros navegadores/dispositivos. El selector no representa carpetas vacías y la maqueta limita el inventario a 500 archivos. No hay lectura por arrastre, recuperación persistente ni carga desatendida.

Tras la precisión del flujo de entrada se repitió la prueba: la acción de importar permanece deshabilitada sin nivel seleccionado; abre un diálogo desde el árbol; no hay segundo selector de nivel; cambiar de perfil conserva el destino; cerrar y abrir desde otro nodo recibe el nuevo contexto. Se regeneraron las capturas.
