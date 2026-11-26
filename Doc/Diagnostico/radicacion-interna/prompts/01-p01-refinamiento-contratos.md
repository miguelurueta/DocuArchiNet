# Prompt P01 para refinamiento y contratos

Estado: consolidado el 2026-11-26, preparado para refinamiento y ejecución; aplicación sin implementar por redactar este archivo.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: Exploraciones vigentes, D-RI-01/D-RI-02/D-RI-03 y mapa M/N.

Este prompt reúne las subfases RI00 de la exploración. Los identificadores RI conservan trazabilidad de tareas/funciones; no son prompts adicionales a ejecutar. Reducir archivos no elimina verificaciones ni autoriza saltar precondiciones internas.

- **RI00:** G01–G11, cerrando ahora las decisiones transversales y asignando el resto a su paquete. Trazabilidad: RQ-RI-01 a RQ-RI-12.

Refinar al abrir cada subfase solo las decisiones que consume; conservar acuerdos ya registrados y continuar trabajo independiente. La instrucción de ejecutar el prompt autoriza su alcance acordado, no decisiones funcionales todavía pendientes ni pruebas del ambiente sin autorización. Versionar contratos antes de cambiar consumidores.

## OBJETIVO

Concretar contratos y decisiones mínimas del alcance operativo. Entrega documental sin cambios de aplicación.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- D-RI-06: tratar la radicación y sus contratos compartidos como núcleo crítico. Caracterizar el comportamiento antes de modificarlo, justificar cada adaptación/extracción indispensable y conservar numeración, transacciones y consumidores. Un servicio o control nuevo no aísla las clases/tablas comunes ni acredita ausencia de regresiones; no reescribir el motor como parte del cambio visual.
- Límite D-RI-03 confirmado: este módulo radica y crea/entrega el seguimiento; elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente. No trasladar esas funciones a esta pantalla ni rediseñar sus políticas.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

### Subfase RI00

Leer los puntos M01–M31 del mapa, la página interna, ClassRadicador.Registra_Radicacion_entrante_interna, las sobrecargas de ClassAlmacenamiento y los adaptadores modernos de Workflow. Contrastar las firmas y consumidores con el código vigente.

El alcance es documental. Las rutas siguientes sirven para planificar futuras altas; no autorizan crearlas en este paquete.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

El inventario de rutas corresponde a futuras entregas; este prompt es solo documentación y no crea componentes de aplicación.

## TRABAJO REQUERIDO

### Subfase RI00

1. **RI00.1.** Registrar aprobaciones existentes y resolver G01: A con expediente previo y materialización de registros compatibles, o B con origen independiente y clasificación posterior. Para B definir dónde se almacena, clasifica y relaciona cada documento; prohibir el atajo de IDs cero. No exigir todas las demás respuestas para continuar análisis independiente.

2. **RI00.2.** Congelar responsabilidades N01–N13, nombres públicos mínimos, tipos/rangos de IDs, protección de petición, autoridad por operación y errores visibles. Concretar apertura, actualización, revisión, confirmación, consulta y recepción multipart sin actor ni rutas del cliente como autoridad.

3. **RI00.3.** Definir el esquema durable mínimo de operación, recepción/manifiesto, confirmación y fases. Identificar claves únicas, revisiones, caducidad, reclamaciones y correlación con radicado, documentos, tarea y respuesta. Inventariar esquema real y herramientas antes de proponer tablas o migración.

4. **RI00.4.** Caracterizar el núcleo crítico antes de planificar su modificación: firma y consumidores de `Registra_Radicacion_entrante_interna`, WebMethods afectados, datos/formatos visibles, permisos, SELECT FOR UPDATE, todos los formatos de consecutivo, relaciones documentales, seguimiento, Commit y efectos posteriores. Determinar qué comparten conexión/transacción y cómo consultar un efecto incierto sin repetirlo. Crear una matriz de invariantes, casos de caracterización y verificación previa/posterior; el paquete funcional ejecutará la evidencia antes de editar. Identificar por cambio la necesidad, alternativas, adaptación mínima y consumidores; un ASMX separado no justifica reescribir el motor ni demuestra que baste un wrapper.

5. **RI00.5.** Fijar matriz de permisos, plantilla/campos, catálogo de terceros, datos de impresión/correo y plazos; registrar lo pendiente por puerta/paquete. El módulo registra la obligación y entrega el seguimiento al módulo existente de respuestas: no diseña elaboración, envío ni cierre de respuestas ni cambia políticas de transición. Identificar el acceso de menú independiente real y quién está autorizado. Inventariar M30/M31 con eventos/conectividad/visibilidad: auxiliar, diagrama, copia, relacionados, expediente, catálogos y limpieza; no inferir un recorrido operativo solo por markup ni activar CRUD/ramas salientes como efecto incidental.

6. **RI00.6.** Concretar cada paquete en archivos existentes/nuevos y tareas revisables con una verificación primaria. Si una decisión cambia contrato de otro paquete, registrar su revisión y actualización antes de editar consumidores.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-01 a RQ-RI-12.

## CRITERIOS DE ACEPTACIÓN

### Subfase RI00

- Contrato v1 distingue procedencia, ejecutor, remitente, responsable y referencias de documentos.
- Cada resultado parcial tiene prueba prevista y una consulta de recuperación localizada.
- Existe una línea base del núcleo y una justificación verificable por cambio compartido; están definidos casos previos/posteriores para numeración, transacción, documentos, seguimiento y consumidores. No se presenta un plan de pruebas como evidencia ejecutada.
- La alternativa de carga elegida tiene recorrido completo; no se oculta la variante sin expediente como trabajo supuestamente terminado.
- Las decisiones pendientes tienen dueño funcional, paquete y consecuencia; ninguna recomendación se marca aprobada por redactar este archivo.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

No declarar completado este prompt si una subfase requerida conserva decisiones o verificaciones pendientes. Registrar qué se completó, qué falta y qué trabajo independiente puede continuar; no repetir permisos ni acuerdos ya registrados.

## PRUEBAS OBLIGATORIAS

### Subfase RI00

Revisión documental de contratos, relaciones y consumidores. Como línea base local, comprobar y ejecutar tests/bootstrap-table-global-contract.test.cjs, tests/doc90-radicacion-classic-attachment.test.cjs, tests/radicacion-simple-attachment-upload-signatures.test.cjs y tests/workflow-user-send.test.cjs. No realizar consultas autenticadas ni mutaciones para suplir datos ausentes.

Este paquete es solo documentación sin cambios de código. E2E no aplica porque no expone un recorrido de usuario ni un endpoint nuevo. No crea modelos, DTOs ni servicios de aplicación. No aplica build MSBuild mientras no cambien aplicación/proyecto; revisar vínculos, firmas, consumidores y contratos. La E2E necesaria de los paquetes funcionales sigue incluida en su aceptación, no se presenta como realizada aquí.



## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Crear implementacion/contratos-v1.md e implementacion/decisiones.md con los acuerdos y tareas verificables; no cambiar aplicación ni configuración. Crear durante la ejecución `Doc/Diagnostico/radicacion-interna/implementacion/P01-resultado.md`, con estado y evidencia por subfase RI, y actualizar contratos/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. Publicar/desplegar solo si la autorización incluye el ambiente correspondiente. Este módulo no gestiona respuestas.
