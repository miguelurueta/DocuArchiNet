# Prompt RI06 para incorporación definitiva y procedencia de documentos

Estado: preparado para refinamiento y ejecución del paquete, no implementado por redactar esta serie.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: RI02, RI03, RI05. Puertas de refinamiento: G01, G06, G07 y política de incorporación de Q-CD-06. Trazabilidad: RQ-RI-01, RQ-RI-02, RQ-RI-07, RQ-RI-09, RQ-RI-11.

Al abrir este paquete, contrastar resultados anteriores y cerrar solo sus decisiones necesarias antes del código afectado. No exigir que toda la exploración esté aprobada para empezar análisis independiente. La instrucción de ejecutar el paquete autoriza el trabajo acordado, no elecciones funcionales todavía pendientes ni E2E del ambiente. No volver a pedir acuerdos ya registrados. Registrar los detalles técnicos y adaptar contrato/consumidores si cambia su revisión.

## OBJETIVO

Entregar referencias documentales válidas al registro común y recuperar una incorporación incierta sin duplicar documentos ni alterar el almacenamiento de los consumidores existentes.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Límite D-RI-03 confirmado: este módulo radica y crea/entrega el seguimiento; elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente. No trasladar esas funciones a esta pantalla ni rediseñar sus políticas.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

M10/M19/M20/M21 y N08. Revisar Almacena_documento_radicado_externo, Retorna_parametros_almacenamiento_documento_radicado_interno, Actualiza_documento_produccio_radicado, PreAlmacenaDocumentoProduccion y AlmacenamientoDocumentoProduccionDocumental.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

## TRABAJO REQUERIDO

1. **RI06.1.** Implementar la alternativa G01 elegida: A revalida expediente/tipología y obtiene registros reales de Producción al incorporar; B prepara almacenamiento/relaciones independientes con clasificación posterior acordada. No pasar cero por una firma que exige un documento previo.

2. **RI06.2.** Normalizar origen Producción frente a recepción nueva mediante manifiesto autorizado. Para existentes verificar expediente, gabinete, vigencia y relación de radicación; para nuevos exigir recepción íntegra de la operación y política de clasificación.

3. **RI06.3.** Reutilizar almacenamiento físico, índices, gabinetes, firmas y conversiones reales. Si hace falta contexto explícito, agregar sobrecarga/adaptador acotado preservando la firma y pruebas de los consumidores actuales.

4. **RI06.4.** Persistir reclamación y resultado por archivo con referencia durable al documento/incorporación. Consultar evidencia antes de repetir un efecto cuya respuesta se perdió; si el proveedor no permite determinarlo, dejar incertidumbre y recuperación controlada.

5. **RI06.5.** Publicar ConsultarIncorporacion y ReconciliarIncorporacion para preparación incompleta. Mantener el principal explícito, los anexos y la caracterización real; los folios no se inventan a partir del tamaño o fecha de modificación del archivo.

6. **RI06.6.** Definir el estado cuando los documentos quedaron incorporados pero falla el núcleo posterior. No borrar automáticamente datos definitivos como si fueran temporales ni exigir al usuario volver a subir lo ya reconocido.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-01, RQ-RI-02, RQ-RI-07, RQ-RI-09, RQ-RI-11.

## CRITERIOS DE ACEPTACIÓN

- Ambos orígenes entregan referencias válidas sin depender de la selección mutable del árbol.
- Fallo/timeout después de guardar conserva evidencia y evita duplicar el mismo documento.
- Solo se incorporan archivos del manifiesto y roles confirmados.
- La variante sin expediente, si fue elegida, funciona sin simular registros de Producción.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

## PRUEBAS OBLIGATORIAS

Pruebas de adaptadores con fallos antes/después del efecto, varios gabinetes, firmas/conversión y referencias ajenas. Integración de almacenamiento real con archivos descartables y consultas solo SELECT de control; verificar relaciones/índices y ausencia de duplicados al reconciliar.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.


## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar código del paquete, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. La parte que dependa de una decisión pendiente se identifica sin declararla concluida. Crear durante su ejecución `Doc/Diagnostico/radicacion-interna/implementacion/RI06-resultado.md` y actualizar contrato/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. No publicar ni modificar un ambiente que carezca de autorización.
