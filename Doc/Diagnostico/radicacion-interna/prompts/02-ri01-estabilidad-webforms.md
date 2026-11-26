# Prompt RI01 para estabilizar dependencias, eventos y controles dinámicos

Estado: preparado para refinamiento y ejecución del paquete, no implementado por redactar esta serie.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: RI00: contrato y alcance de estabilización. Puertas de refinamiento: Solo las decisiones que alteren comportamiento de controles; no requiere resolver todos los catálogos. Trazabilidad: RQ-RI-04, RQ-RI-05, RQ-RI-10, RQ-RI-11.

Al abrir este paquete, contrastar resultados anteriores y cerrar solo sus decisiones necesarias antes del código afectado. No exigir que toda la exploración esté aprobada para empezar análisis independiente. La instrucción de ejecutar el paquete autoriza el trabajo acordado, no elecciones funcionales todavía pendientes ni E2E del ambiente. No volver a pedir acuerdos ya registrados. Registrar los detalles técnicos y adaptar contrato/consumidores si cambia su revisión.

## OBJETIVO

Corregir las inconsistencias verificadas de la página actual manteniendo su comportamiento mientras se construye la integración nueva.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Límite D-RI-03 confirmado: este módulo radica y crea/entrega el seguimiento; elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente. No trasladar esas funciones a esta pantalla ni rediseñar sus políticas.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

M02/M04/M07/M12/M27/M28: radicador/WebFormRadicacionEntranteInterna.aspx y .aspx.vb, Gestion/ClassGaProducionDocumental.vb y js/radicacion/WebFormRadicacionEntranteInterna.js.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

## TRABAJO REQUERIDO

1. **RI01.1.** Caracterizar el orden efectivo de scripts y consolidar jQuery/UI/plugins compatibles por documento. Retirar la segunda carga que elimina extensiones, sin actualizar versiones globales ni confundir instancias del padre y del iframe.

2. **RI01.2.** Registrar CheckStatus/endRequest y los eventos de campos una sola vez. Retirar manejadores propios al reconstruir, conservar los de otros componentes y comprobar varios postbacks consecutivos sin efectos multiplicados.

3. **RI01.3.** Corregir las búsquedas, comprobaciones de referencia y asignaciones independientes de Hidden_Estado_opcion_fecha, Hidden_Estado_opcion_cita_respuesta y Hidden_Estado_opcion_radicado_general. Verificar las ocho combinaciones de las tres opciones y sus consumidores reales.

4. **RI01.4.** Asegurar creación de controles dinámicos en fase compatible con postdata/ViewState, ID y tipos estables. No activar EnableEventValidation sin reconstrucción y registro apropiados; no dar por segura la selección de un combo.

5. **RI01.5.** Sincronizar el usuario efectivo visible/oculto al cargar o cambiar grupo/actividad/flujo, invalidando selecciones anteriores. Al terminar, revalidar pertenencia en servidor; no arreglar solamente el onchange.

6. **RI01.6.** Enlazar el cierre con ButtonSalir_radica_interno y la instancia correcta. Caracterizar descarga/notificación para no cerrar como éxito ante un resultado parcial; el protocolo tipado completo llega en RI08/RI10.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-04, RQ-RI-05, RQ-RI-10, RQ-RI-11.

## CRITERIOS DE ACEPTACIÓN

- Plugins disponibles antes y después de postbacks; un evento produce un solo efecto.
- Valores y foco se conservan al reconstruir distintas plantillas.
- La identidad consumida coincide con la selección válida o se rechaza.
- El callback no pulsa el cierre del visor externo ni convierte correo pendiente en radicación inexistente.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

## PRUEBAS OBLIGATORIAS

Pruebas locales de dependencias y eventos, matrices de opciones y postdata con tipos de campo reales. Mantener tests/bootstrap-table-global-contract.test.cjs. En IIS autorizado comprobar postbacks de trámite/grupo, valores, validación y modal padre/iframe; un DOM aislado no acredita el ciclo WebForms.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.


## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar código del paquete, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. La parte que dependa de una decisión pendiente se identifica sin declararla concluida. Crear durante su ejecución `Doc/Diagnostico/radicacion-interna/implementacion/RI01-resultado.md` y actualizar contrato/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. No publicar ni modificar un ambiente que carezca de autorización.
