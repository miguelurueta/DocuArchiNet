# Prompt P06 para interfaz, integración y aceptación

Estado: consolidado el 2026-11-26, preparado para refinamiento y ejecución; aplicación sin implementar por redactar este archivo.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: P01–P05 implementados y verificados en lo que se consume. Orden interno obligatorio: RI10 → RI11.

Este prompt reúne las subfases RI10, RI11 de la exploración. Los identificadores RI conservan trazabilidad de tareas/funciones; no son prompts adicionales a ejecutar. Reducir archivos no elimina verificaciones ni autoriza saltar precondiciones internas.

- **RI10:** Decisiones que afecten al recorrido habilitado; no heredar límites/datos de la maqueta. Trazabilidad: RQ-RI-01 a RQ-RI-11.
- **RI11:** Decisiones de cada capacidad incluida y autorización de ambiente/despliegue que corresponda. Trazabilidad: RQ-RI-01 a RQ-RI-12.

Refinar al abrir cada subfase solo las decisiones que consume; conservar acuerdos ya registrados y continuar trabajo independiente. La instrucción de ejecutar el prompt autoriza su alcance acordado, no decisiones funcionales todavía pendientes ni pruebas del ambiente sin autorización. Versionar contratos antes de cambiar consumidores.

## OBJETIVO

Integrar la interfaz compacta con ambos accesos y contratos reales; comprobar el recorrido completo de radicación, entrega del seguimiento, publicación y recuperación.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Límite D-RI-03 confirmado: este módulo radica y crea/entrega el seguimiento; elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente. No trasladar esas funciones a esta pantalla ni rediseñar sus políticas.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

### Subfase RI10

M01–M07/M27/M28/M30/M31 y N12/N13. Página interna, padre de Producción, js/radicacion/WebFormRadicacionEntranteInterna.js y estilos propios; acceso independiente real localizado en RI00.

### Subfase RI11

Todos los contratos implementados; GestionDocumental-Docuarchi.net.vbproj, scripts de esquema/configuración, página y handlers/ASMX nuevos, consumidores afectados y matriz de aceptación.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

Crear solo los componentes que correspondan a las subfases de este prompt; no implementar capas de paquetes posteriores.

## TRABAJO REQUERIDO

### Subfase RI10

1. **RI10.1.** Integrar entrada desde Producción con operación propia y entrada independiente autorizada desde el menú acordado. Resolver plantilla/configuración de sesión nueva; no obligar a seleccionar un documento en otra pantalla ni ofrecer un acceso sin autoridad de servidor.

2. **RI10.2.** Aplicar disposición compacta, identificación de remitente y principal/anexos explícitos, campos dinámicos y datos adicionales preservados. En producción real el origen lo determina la apertura; el selector de comparación del HTML no obliga a permitir mezcla o cambio arbitrario de expediente.

3. **RI10.3.** Conectar buscadores y selecciones a catálogos autorizados. Invalidar valores antiguos y respuestas tardías por operación/revisión; conservar foco y datos compatibles tras UpdatePanel. No crear nuevos contratos de validación basados solo en atributos HTML.

4. **RI10.4.** Integrar el cargador reutilizado y adaptado en P04 según D-RI-04 con elegir/arrastrar principal, múltiples anexos, progreso/estado confirmado, retiro/sustitución y consulta de recepción incierta. Conservar la presentación compacta aprobada sin introducir otro cargador completo. No llamar Subido a un archivo que solo existe como File local ni confirmar durante recepción incompleta.

5. **RI10.5.** Preparar revisión desde servidor y confirmar exactamente su manifiesto/versión. Bloquear doble envío y consultar resultado incierto; mostrar registro con pendientes y seguimiento sin dar por respondido el trámite. Usar descarga/impresión reales.

6. **RI10.6.** Comunicar resultado al padre correcto mediante protocolo acordado, actualizar lista sin cambiar otro contexto y cerrar/reabrir sin suponer reversión. Integrar lectura de seguimiento con permisos y los recorridos reales inventariados en M30/M31: auxiliar de internos, diagrama, copia de datos, relacionados/expediente y catálogos. La limpieza debe preservar el resultado confirmado y no reutilizar identidad/manifiesto anterior; no habilitar controles inactivos ni altas de terceros no acordadas.

7. **RI10.7.** Verificar teclado, foco, labels/errores, zoom, pantallas pequeñas y varias plantillas. No endurecer ni eliminar reglas/campos al copiar la maqueta; retirar los datos, plazos, File en memoria y temporizadores de demostración de la ruta real.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-01 a RQ-RI-11.

### Subfase RI11

1. **RI11.1.** Consolidar funciones/cambios reales frente al mapa M/N, aprobaciones aplicadas, migraciones, endpoints publicados, permisos, configuración de plantilla/trámite/gabinete y formatos de salida. Registrar cualquier exclusión solicitada sin llamar completo al alcance original pendiente.

2. **RI11.2.** Ejecutar compilación y publicación reproducibles con .NET Framework 4.6.1 y dependencias reales. Verificar que Compile/Content, handler/ASMX, scripts/CSS y migración/configuración estén en el artefacto; no depender de archivos que solo existen en desarrollo.

3. **RI11.3.** Comprobar ambos accesos con principal/anexos, identidad propia/representada/tercero, distintas plantillas y tres modalidades. Confirmar persistencia documental, único radicado, obligación, propietario, Workflow y descarga/impresión; el alcance de respuesta termina en disponibilidad/lectura del seguimiento por el módulo existente, sin elaborar, enviar ni cerrar respuestas.

4. **RI11.4.** Ejecutar escenarios de permiso retirado, expediente cerrado, sesión caducada, cambio de revisión, dos pestañas, carga truncada, respuesta perdida antes/después del commit, fallos de incorporación/Workflow/correo y recuperación tras reciclado. Registrar consultas de control por referencia de operación.

5. **RI11.5.** Validar regresión de Producción, adjuntos clásicos/Simplificados y políticas de Workflow. No sumar cambios ajenos para hacer pasar una prueba ni modificar autenticación/configuración/flags arbitrariamente.

6. **RI11.6.** Preparar orden de migración, configuración y publicación, ensayo reversible en entorno autorizado y recuperación de operaciones en vuelo. La reversión de aplicación no borra radicados/archivos confirmados; definir compatibilidad de esquema y acceso a pendientes.

7. **RI11.7.** Entregar matriz de evidencia y decisión de disponibilidad. Sin validación integrada autorizada, reportar implementación técnica comprobada y aceptación pendiente; no marcar operativo ni cerrar con pruebas estáticas o simulaciones.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-01 a RQ-RI-12.

## CRITERIOS DE ACEPTACIÓN

### Subfase RI10

- Una sesión autorizada nueva puede subir, revisar, radicar y consultar el resultado real.
- Desde Producción se conserva el recorrido y se actualiza el modal/listado correcto.
- La UI no mezcla orígenes, identidades ni respuestas tardías y refleja estados del servidor.
- Plantillas, postbacks, descarga, consulta y seguimiento funcionan con la composición real.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

### Subfase RI11

- Ambas entradas y capacidades acordadas tienen evidencia de dominio y de interfaz real.
- No hay duplicados del mismo intento ni pendientes ocultos presentados como éxito completo.
- Artefacto publicado y esquema/configuración coinciden con lo probado.
- Recuperación y reversión preservan documentos/radicados confirmados; limitaciones y pendientes son explícitos.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

No declarar completado este prompt si una subfase requerida conserva decisiones o verificaciones pendientes. Registrar qué se completó, qué falta y qué trabajo independiente puede continuar; no repetir permisos ni acuerdos ya registrados.

## PRUEBAS OBLIGATORIAS

### Subfase RI10

Pruebas de cliente con respuestas tardías/errores y pruebas integradas de WebForms. Reutilizar infraestructura tools/e2e para ambos accesos con archivos reales descartables, tres modalidades, terceros permitidos, dos pestañas y recuperación. La simulación HTML y un mock HTTP no acreditan la aceptación del recorrido real.

### Subfase RI11

Ejecutar la matriz completa en infraestructura existente y ambiente autorizado, además de las pruebas focales acumuladas. Registrar comandos, artefactos, resultados y consultas SELECT saneadas. Publicar/desplegar solo si la instrucción de ejecución y la autorización del ambiente lo incluyen; preparar todo lo verificable antes de solicitar esa aprobación final.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.

## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar el código de las subfases, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. Crear durante la ejecución `Doc/Diagnostico/radicacion-interna/implementacion/P06-resultado.md`, con estado y evidencia por subfase RI, y actualizar contratos/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. Publicar/desplegar solo si la autorización incluye el ambiente correspondiente. Este módulo no gestiona respuestas.
