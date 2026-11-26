# Prompt RI00 para refinamiento y contratos de la implementación

Estado: preparado para refinamiento y ejecución del paquete, no implementado por redactar esta serie.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: Exploraciones vigentes y mapa M01–M31/N01–N13. Puertas de refinamiento: G01–G11, cerrando ahora las decisiones transversales y asignando el resto a su paquete. Trazabilidad: RQ-RI-01 a RQ-RI-12.

Al abrir este paquete, contrastar resultados anteriores y cerrar solo sus decisiones necesarias antes del código afectado. No exigir que toda la exploración esté aprobada para empezar análisis independiente. La instrucción de ejecutar el paquete autoriza el trabajo acordado, no elecciones funcionales todavía pendientes ni E2E del ambiente. No volver a pedir acuerdos ya registrados. Registrar los detalles técnicos y adaptar contrato/consumidores si cambia su revisión.

## OBJETIVO

Concretar un contrato v1 verificable y las decisiones mínimas para implementar la capacidad completa sin rehacer la exploración ni tratar la maqueta como configuración real.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

Leer los puntos M01–M31 del mapa, la página interna, ClassRadicador.Registra_Radicacion_entrante_interna, las sobrecargas de ClassAlmacenamiento y los adaptadores modernos de Workflow. Contrastar las firmas y consumidores con el código vigente.

El alcance es documental. Las rutas siguientes sirven para planificar futuras altas; no autorizan crearlas en este paquete.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

## TRABAJO REQUERIDO

1. **RI00.1.** Registrar aprobaciones existentes y resolver G01: A con expediente previo y materialización de registros compatibles, o B con origen independiente y clasificación posterior. Para B definir dónde se almacena, clasifica y relaciona cada documento; prohibir el atajo de IDs cero. No exigir todas las demás respuestas para continuar análisis independiente.

2. **RI00.2.** Congelar responsabilidades N01–N13, nombres públicos mínimos, tipos/rangos de IDs, protección de petición, autoridad por operación y errores visibles. Concretar apertura, actualización, revisión, confirmación, consulta y recepción multipart sin actor ni rutas del cliente como autoridad.

3. **RI00.3.** Definir el esquema durable mínimo de operación, recepción/manifiesto, confirmación y fases. Identificar claves únicas, revisiones, caducidad, reclamaciones y correlación con radicado, documentos, tarea y respuesta. Inventariar esquema real y herramientas antes de proponer tablas o migración.

4. **RI00.4.** Trazar el núcleo actual: select FOR UPDATE, todos los formatos de consecutivo, inserciones/actualizaciones, seguimiento, Commit y efectos posteriores. Determinar qué comparten conexión/transacción y cómo se consulta un efecto incierto sin repetirlo.

5. **RI00.5.** Fijar matriz de permisos, plantilla/campos, catálogo de terceros, datos de impresión/correo, plazos, respuesta formal y políticas de Workflow; registrar lo pendiente por puerta/paquete. Identificar el acceso de menú independiente real y quién está autorizado. Inventariar M30/M31 con eventos/conectividad/visibilidad: auxiliar, diagrama, copia, relacionados, expediente, catálogos y limpieza; no inferir un recorrido operativo solo por markup ni activar CRUD/ramas salientes como efecto incidental.

6. **RI00.6.** Concretar cada paquete en archivos existentes/nuevos y tareas revisables con una verificación primaria. Si una decisión cambia contrato de otro paquete, registrar su revisión y actualización antes de editar consumidores.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-01 a RQ-RI-12.

## CRITERIOS DE ACEPTACIÓN

- Contrato v1 distingue procedencia, ejecutor, remitente, responsable y referencias de documentos.
- Cada resultado parcial tiene prueba prevista y una consulta de recuperación localizada.
- La alternativa de carga elegida tiene recorrido completo; no se oculta la variante sin expediente como trabajo supuestamente terminado.
- Las decisiones pendientes tienen dueño funcional, paquete y consecuencia; ninguna recomendación se marca aprobada por redactar este archivo.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

## PRUEBAS OBLIGATORIAS

Revisión documental de contratos, relaciones y consumidores. Como línea base local, comprobar y ejecutar tests/bootstrap-table-global-contract.test.cjs, tests/doc90-radicacion-classic-attachment.test.cjs, tests/radicacion-simple-attachment-upload-signatures.test.cjs y tests/workflow-user-send.test.cjs. No realizar consultas autenticadas ni mutaciones para suplir datos ausentes.

Este paquete es solo documentación sin cambios de código. E2E no aplica porque no expone un recorrido de usuario ni un endpoint nuevo. No crea modelos, DTOs ni servicios de aplicación. No aplica build MSBuild mientras no cambien aplicación/proyecto; revisar vínculos, firmas, consumidores y contratos. La E2E necesaria de los paquetes funcionales sigue incluida en su aceptación, no se presenta como realizada aquí.

## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar implementacion/contratos-v1.md, implementacion/decisiones.md y matriz de funciones/tareas listas para ejecución según el contexto común. No cambiar aplicación ni configuración. Crear durante su ejecución `Doc/Diagnostico/radicacion-interna/implementacion/RI00-resultado.md` y actualizar contrato/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. No publicar ni modificar un ambiente que carezca de autorización.
