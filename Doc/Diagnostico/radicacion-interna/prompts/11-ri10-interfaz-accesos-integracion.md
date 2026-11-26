# Prompt RI10 para integrar la interfaz real y ambos accesos

Estado: preparado para refinamiento y ejecución del paquete, no implementado por redactar esta serie.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: RI01–RI09 en los contratos que se consumen; servicios listos y verificados. Puertas de refinamiento: Decisiones que afecten al recorrido habilitado; no heredar límites/datos de la maqueta. Trazabilidad: RQ-RI-01 a RQ-RI-11.

Al abrir este paquete, contrastar resultados anteriores y cerrar solo sus decisiones necesarias antes del código afectado. No exigir que toda la exploración esté aprobada para empezar análisis independiente. La instrucción de ejecutar el paquete autoriza el trabajo acordado, no elecciones funcionales todavía pendientes ni E2E del ambiente. No volver a pedir acuerdos ya registrados. Registrar los detalles técnicos y adaptar contrato/consumidores si cambia su revisión.

## OBJETIVO

Convertir la maqueta en interfaz WebForms operativa conectada a los contratos reales, sin copiar su estado ficticio ni perder las capacidades del módulo.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

M01–M07/M27/M28/M30/M31 y N12/N13. Página interna, padre de Producción, js/radicacion/WebFormRadicacionEntranteInterna.js y estilos propios; acceso independiente real localizado en RI00.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

## TRABAJO REQUERIDO

1. **RI10.1.** Integrar entrada desde Producción con operación propia y entrada independiente autorizada desde el menú acordado. Resolver plantilla/configuración de sesión nueva; no obligar a seleccionar un documento en otra pantalla ni ofrecer un acceso sin autoridad de servidor.

2. **RI10.2.** Aplicar disposición compacta, identificación de remitente y principal/anexos explícitos, campos dinámicos y datos adicionales preservados. En producción real el origen lo determina la apertura; el selector de comparación del HTML no obliga a permitir mezcla o cambio arbitrario de expediente.

3. **RI10.3.** Conectar buscadores y selecciones a catálogos autorizados. Invalidar valores antiguos y respuestas tardías por operación/revisión; conservar foco y datos compatibles tras UpdatePanel. No crear nuevos contratos de validación basados solo en atributos HTML.

4. **RI10.4.** Integrar carga real con elegir/arrastrar principal, múltiples anexos, progreso/estado confirmado, retiro/sustitución y consulta de recepción incierta. No llamar Subido a un archivo que solo existe como File local ni confirmar durante recepción incompleta.

5. **RI10.5.** Preparar revisión desde servidor y confirmar exactamente su manifiesto/versión. Bloquear doble envío y consultar resultado incierto; mostrar registro con pendientes y seguimiento sin dar por respondido el trámite. Usar descarga/impresión reales.

6. **RI10.6.** Comunicar resultado al padre correcto mediante protocolo acordado, actualizar lista sin cambiar otro contexto y cerrar/reabrir sin suponer reversión. Integrar lectura de seguimiento con permisos y los recorridos reales inventariados en M30/M31: auxiliar de internos, diagrama, copia de datos, relacionados/expediente y catálogos. La limpieza debe preservar el resultado confirmado y no reutilizar identidad/manifiesto anterior; no habilitar controles inactivos ni altas de terceros no acordadas.

7. **RI10.7.** Verificar teclado, foco, labels/errores, zoom, pantallas pequeñas y varias plantillas. No endurecer ni eliminar reglas/campos al copiar la maqueta; retirar los datos, plazos, File en memoria y temporizadores de demostración de la ruta real.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-01 a RQ-RI-11.

## CRITERIOS DE ACEPTACIÓN

- Una sesión autorizada nueva puede subir, revisar, radicar y consultar el resultado real.
- Desde Producción se conserva el recorrido y se actualiza el modal/listado correcto.
- La UI no mezcla orígenes, identidades ni respuestas tardías y refleja estados del servidor.
- Plantillas, postbacks, descarga, consulta y seguimiento funcionan con la composición real.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

## PRUEBAS OBLIGATORIAS

Pruebas de cliente con respuestas tardías/errores y pruebas integradas de WebForms. Reutilizar infraestructura tools/e2e para ambos accesos con archivos reales descartables, tres modalidades, terceros permitidos, dos pestañas y recuperación. La simulación HTML y un mock HTTP no acreditan la aceptación del recorrido real.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.


## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar código del paquete, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. La parte que dependa de una decisión pendiente se identifica sin declararla concluida. Crear durante su ejecución `Doc/Diagnostico/radicacion-interna/implementacion/RI10-resultado.md` y actualizar contrato/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. No publicar ni modificar un ambiente que carezca de autorización.
