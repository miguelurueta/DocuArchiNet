# Prompt 09 — Almacenamiento e incorporación documental

## Encargo

Lee guía 00, prompt 03, diseño 04 y contratos 07/08. Implementa un adaptador propio que almacene archivos recibidos y verifique incorporación al expediente confirmado, reutilizando operaciones compatibles sin modificar SII.

## Investigación obligatoria

En `workflow/ClassAlmacenamiento.vb`, verifica `PreAlmacenaDocumentoProduccion`, `AlmacenamientoDocumentoProduccionDocumental` y sus llamadas a almacenamiento. La segunda recibe `List(Of CDcamposAsignaAlmacenamiento)` pero conserva dependencias de sesión/temporales. Examina qué pasos producen imagen, registro, relación e índice y cuáles pueden fallar después de persistir. No suponer que almacenar e incorporar son dos llamadas independientes ni ejecutar ambas a ciegas.

El adaptador SII `LegacyImportDocumentStorageAdapter` exige contexto Workflow: es referencia, no una operación de carpetas. No enviar tareas ficticias ni alterar contratos SII para reutilizarlo.

Hallazgo concreto a caracterizar: `AlmacenamientoDocumentoProduccionDocumental` obtiene el nombre de `FileInfo(RutaArchivoAlmacenar).Name` y elimina `MatrizDocumentosFinal` después de persistir; también puede eliminar el temporal de sesión. Esto contradice pasarle el único staging retenido y asumir que su nombre opaco no altera el nombre documental. Utilizar una copia de trabajo aislada y consumible, conservando staging y evidencia hasta verificar; preservar nombre/extensión mediante un contrato realmente soportado o una copia con nombre seguro dentro de un directorio único. Si no puede garantizarse sin cambiar el método compartido, ese punto de reutilización no es apto. La política de limpieza de 11 controla staging, no impide los borrados internos del método legacy.

## Trabajo

- Comando explícito: identidad de operación/archivo, temporal verificado, expediente/nivel, gabinete, tipología, campos índice resueltos y autoridad. Cargar datos desde el plan/lote, no aceptar destinos arbitrarios del cliente.
- Componer una asignación por campo, comprobar pertenencia al gabinete, tipo, longitud, obligatoriedad y tipología compatible. Aplicar constantes/herencia/nombre según perfil sin cambiar relaciones globales de gabinete.
- Verificar dependencia de sesión. Aislar cualquier contexto de compatibilidad con restauración en Finally y exclusión probada frente a otros consumidores; no presentar esa solución como apta para worker. Si no es aislable, documentar el límite y usar una operación inferior compatible sin modificar SII.
- Preparar temporales únicos y formatos admitidos; caracterizar Multi-TIFF, inventario e índice electrónico. No anunciar formatos no verificados.
- Retornar resultado estructurado: rechazado antes de efectos, persistencia confirmada con IDs, parcial con fase pendiente o incierto. No clasificar excepción de proyección/limpieza como garantía de que no se almacenó.
- Verificar gabinete, documento, expediente y efectos de incorporación. Si falta un efecto compatible, ejecutar solo ese paso con su identidad; no volver a almacenar. Las consultas de reconciliación deben justificar cómo atribuyen el documento a esta operación.
- Persistir IDs/evidencia mediante 07 y conservar temporales necesarios ante incertidumbre. Eliminar temporales solo con la política segura de 11, sin ocultar el éxito documental por un error de limpieza.

## Aceptación

Probar que nombres originales y extensiones se conservan conforme a la política incluso con temporales opacos, homónimos y Multi-TIFF; un fallo posterior a persistencia/borrado no destruye el staging retenido. Resolver/revalidar campos diferidos con los IDs reales del expediente usando el resolvedor 02. Probar concurrencia con escritores externos del expediente/índice y separación de sesión/temporales; una exclusión del lote no demuestra aislamiento con carga individual o SII.

Pruebas con índices extraídos de dos estructuras configurables, ceros iniciales, tipología inexistente, campo ajeno al gabinete, homónimos, almacenamiento ya confirmado, incorporación faltante, relación a otro expediente y respuesta perdida tras persistencia. Caracterizar fallos del índice/XML y limpieza. No prometer exactamente una vez cuando no existe correlación fiable: bloquear reintento automático y reportar revisión.

Entregar adaptador y verificador, mapa de efectos y ventanas de fallo, compatibilidad de formatos y pruebas. No escribir otro registro de lotes ni cambiar tablas SII; SQL incremental MySQL 5.1 solo si el diseño operativo exige un cambio justificado.
