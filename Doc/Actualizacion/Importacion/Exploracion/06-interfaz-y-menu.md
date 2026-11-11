# Prompt 06 — Interfaz de carga e integración del menú

## ROL ESPERADO

Desarrollador Web Forms y especialista de interacción accesible para herramientas administrativas.

## OBJETIVO

Integrar el recorrido del cargador en la navegación habitual con acceso por su propio permiso.

## DEPENDENCIAS

Requiere los servicios funcionales de las fases 02 a 05 y sus contratos de estados y errores.

## CONTEXTO

Trabaja en D:/imagenesda/DocuachiNet/DocuArchiNet, ASP.NET Web Forms con VB.NET, .NET Framework 4.6.1 y MySQL 5.1.37. Lee AGENTS.md y Doc/Actualizacion/Importacion/Exploracion/carga-excel-csv-tablas-imp-02.md, especialmente su diseño técnico resuelto. Estos siete prompts son fases consecutivas del mismo cambio funcional. Reutiliza el cambio OpenSpec y los resultados de las fases anteriores.

El alcance aprobado comprende XLS y XLSX, los modos crear/agregar/reemplazar, permiso ADMINISTRAR_CARGAS_IMPORTACION en remit_dest_interno_perfil_produccion, cuenta dedicada sobre la DA autorizada, validación por carga e índice UNIQUE permanente. Agregar rechaza toda la carga ante claves repetidas internas o existentes; reemplazar valida la unicidad del archivo y sustituye el contenido autorizado. Los límites iniciales son 10.000 filas y 10 MB, configurables en servidor; 1 MB equivale a 1.000.000 bytes.

Reutiliza imp01_xmlconexion, imp01_plantillaimp, imp01_campos_plantilla y las tablas imp_02_* elegibles. El diseño añade CAMPO_UNICO y REVISION_CARGA a la cabecera y una tabla imp01_carga_ejecucion. Preserva la compatibilidad de las plantillas históricas y sus consumidores de SII e importación de carpetas.

## PRECONDICIONES DE RUTAS

Inventariar los archivos exactos en design.md antes de editar. Contrastar cada ruta propuesta con el código existente y reutilizar las responsabilidades ya implementadas. Aplicar únicamente las modificaciones de la fase actual. Registrar los archivos nuevos de compilación y contenido en GestionDocumental-Docuarchi.net.vbproj según el patrón del proyecto.

| Responsabilidad | Rutas previstas |
|---|---|
| Modelos | Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb |
| DTOs y contrato request/response | DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb |
| Servicios | Services/ProduccionDocumental/ImportacionExcel/ServicioPlantillasExcel.vb, ServicioLecturaExcel.vb, ServicioPreparacionExcel.vb y ServicioConfirmacionExcel.vb en esa misma carpeta |
| Repositorios | Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPlantillaExcelRepository.vb y MySqlCargaExcelRepository.vb en esa misma carpeta |
| Adaptadores e infraestructura | Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelContextAdapter.vb, ImportacionExcelRequestGuard.vb, ImportacionExcelConexionAdapter.vb y LectorArchivoExcel.vb en esa misma carpeta |
| Servicio web | webservice/WebServiceImportacionExcel.asmx y webservice/WebServiceImportacionExcel.asmx.vb |
| Receptor de archivo | generic_control/ImportacionExcelUpload.ashx y generic_control/ImportacionExcelUpload.ashx.vb |
| Interfaz | js/gestion/ImportacionExcel/ImportacionExcelApi.js, js/gestion/ImportacionExcel/ImportacionExcelVista.js y Styles/ImportacionExcel.css |
| Integración existente | Gestion/WebFormProducionDocumental.aspx, Gestion/WebFormProducionDocumental.aspx.vb, Gestion/Class_remit_dest_interno_perfil_produccion.vb y js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js |
| Pruebas | tests/ImportacionExcelBehaviorTests.cs y tools/e2e/tests/importacion-excel.spec.cjs, ampliando los harness y utilidades existentes cuando corresponda |

Las rutas propuestas delimitan responsabilidades. Ajustar la división de archivos a la implementación real y registrar la justificación en design.md.

## RESTRICCIONES CRÍTICAS

- Mantener la sesión y empresa como autoridad. El navegador envía referencias, nunca un servidor, base o cadena de conexión elegidos libremente.
- No imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión en código, documentación o evidencias. Usar secretos efímeros durante pruebas y configuración protegida para la cuenta del servidor.
- No modificar AlmacenamientoDocumentoProduccionDocumental ni SII para resolver la carga tabular. Preservar los contratos de los consumidores existentes.
- CSV, migración entre gabinetes, omisión de duplicados y actualización automática por coincidencia de clave están fuera de alcance.
- El agente genera los cambios SQL en archivos e indica su ejecución manual por el usuario. No debe ejecutar SQL modificador de esquema, datos o permisos ni aplicar archivos SQL directa o indirectamente. La comprobación posterior utiliza exclusivamente SELECT después de la confirmación de ejecución manual.
- Conservar los cambios ajenos presentes en el repositorio local y agrupar las entregas conforme al flujo del cambio.

## FLUJO OBLIGATORIO DE SQL MANUAL

1. Generar los cambios SQL como archivos nuevos y versionables en Doc/Actualizacion/Importacion/Sql/. Cubrir instalación, tablas, columnas, índices, permisos y cualquier corrección o preparación de datos que se necesite. Conservar los scripts históricos ya aplicados y entregar las correcciones en otro archivo.
2. Entregar una guía con archivo, orden, destino RA/DA, precondiciones, efectos previstos y consultas SELECT de comprobación. Mantener las credenciales fuera del SQL entregado.
3. Indicar al usuario qué archivo debe ejecutar manualmente. El agente no debe ejecutar SQL modificador ni aplicar esos archivos, tampoco mediante scripts de arranque, tests o una ruta indirecta de la aplicación.
4. Esperar la confirmación de ejecución manual del usuario antes de verificar ese cambio en la base. Registrar PENDIENTE_EJECUCION_MANUAL mientras esa confirmación falte y continuar el trabajo local independiente.
5. Después de la confirmación, revisar el resultado mediante consultas exclusivamente SELECT y evidencia saneada. Registrar diferencias y, si hace falta otra modificación, generar un nuevo archivo para ejecución manual.
6. La ausencia de errores en un archivo o su entrega no acredita que esté aplicado. La revisión posterior debe comprobar las estructuras, índices, relaciones y permisos observables correspondientes y registrar cualquier aspecto que requiera evidencia del usuario.

El código del cargador implementa los modos funcionales acordados para uso del usuario. La instalación o adecuación de su base sigue esta secuencia manual y debe estar verificada antes de probar las operaciones que dependan de ella.

## IMPLEMENTACIÓN DE ESTA FASE

1. Añadir Cargar datos desde Excel al menú Importación de Producción Documental. Construir o reutilizar el menú desde la navegación común, de modo que funcione con exclusivamente ADMINISTRAR_CARGAS_IMPORTACION.
2. Implementar el diálogo de plantilla, archivo XLS/XLSX, hoja y modo. Mostrar destino y clave única configurada, límites efectivos y estado de la operación.
3. Conectar la administración de plantillas con los servicios de la fase 02. Presentar los estados vacío, cargando, con incidencias, preparado, confirmando, confirmado y resultado incierto.
4. Presentar la vista previa acotada y las incidencias numeradas. Permitir confirmar después de validar y exigir una confirmación explícita del contenido afectado al reemplazar.
5. Conservar el ID de carga y consultar el resultado cuando se pierda una respuesta. Prevenir confirmaciones accidentales repetidas y respuestas de red antiguas que sustituyan una selección reciente.
6. Incorporar foco inicial, etiquetas accesibles, navegación por teclado, Escape y retorno del foco. Comprobar que el diálogo y sus controles sean visibles en el área disponible.
7. Coordinar el acceso con el menú resultante de los trabajos vigentes de navegación, reutilizando su implementación. Preservar perfiles, fuentes, proveedores e importación de carpetas según sus permisos.
8. Exponer solo los datos necesarios para decidir y actuar, con errores saneados y mensajes operativos.

## CRITERIOS DE ACEPTACIÓN

- Un usuario con el permiso exclusivo del cargador encuentra y usa su acceso.
- Crear, agregar y reemplazar pueden recorrerse desde la interfaz hasta consultar el resultado.
- El usuario distingue las incidencias, el rechazo de duplicados y un resultado incierto recuperable.
- Se acreditan teclado/foco y la visibilidad de los controles.
- La regresión conserva los accesos existentes y los permisos de cada capacidad.
- El E2E integrado utiliza los servicios reales del cambio en la ejecución autorizada. Los recorridos locales con transporte simulado se registran por separado como evidencia local.

## PRUEBAS OBLIGATORIAS

Realiza la compilación MSBuild o el build pertinente al alcance de esta fase, y pruebas unitarias o QA manual reproducible que acrediten sus criterios. Registra comandos, resultado, SHA, ambiente y límites de evidencia. Las pruebas locales o con transporte simulado se identifican como tales.

El E2E del recorrido ejecutable es parte integral del mismo cambio funcional. Reutilizar tools/e2e y su autenticación, configuración, validadores y utilidades existentes, sin infraestructura paralela. Antes de autenticar, leer AGENTS.md y tools/e2e/AGENT-RUNBOOK.md. La ejecución real requiere autorización explícita de ambiente, cuentas y datos descartables. Cubrir control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión del alcance modificado. Los tests no deben instalar, migrar, sembrar, limpiar o reparar la base mediante SQL automático. La instalación se ejecuta manualmente por el usuario y se revisa mediante SELECT antes de los recorridos dependientes. Las verificaciones locales usan fixtures sintéticos sin escrituras en bases reales. Las escrituras funcionales de una prueba real requieren el alcance específico autorizado de esa prueba y no sustituyen la ejecución manual de los archivos SQL.

Mantener secretos efímeros y evidencia saneada, con consultas de control solo SELECT. Respetar los gates de autorización vigentes. No habilitar permisos arbitrariamente. El criterio de cierre del cambio exige validación autorizada y registrar cualquier bloqueo explícito. Los mocks, simulaciones y evidencia ficticia no acreditan una ejecución real. Las fases de base técnica documentarán la justificación cuando E2E no aplique todavía a su entrega.

## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar la documentación existente en Doc/Actualizacion/Importacion/. Conservar la exploración como antecedente y registrar el diseño, las decisiones y los contratos vigentes del cambio OpenSpec. Guardar el SQL revisable en Doc/Actualizacion/Importacion/Sql/, y la evidencia y matriz de verificación en Doc/Actualizacion/Importacion/Validacion/. Añadir únicamente los documentos necesarios para explicar la entrega de esta fase.

## ENTREGABLE FINAL

Interfaz, cliente API y navegación integrados, contratos de interacción documentados y evidencia de accesibilidad y recorridos.
