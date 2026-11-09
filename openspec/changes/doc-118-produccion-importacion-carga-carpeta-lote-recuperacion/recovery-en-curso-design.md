# Recuperación explícita de EN_CURSO — diseño candidato

Estado actual:implementación local hasta RV-06 y matriz de intercalaciones verificada en RV-07. Los apartados iniciales conservan el razonamiento de la evaluación original; los cortes posteriores delimitan lo implementado y su evidencia. No hay despliegue, presupuesto real nuevo ni recuperación de69.

## Conclusión

La evidencia disponible no permite liberar con seguridad el intento69. Sus conteos físicos actuales son cero, pero no tiene una prueba de no invocación ni un resultado de invocación persistido que permita distinguir todas las ventanas de fallo. La ausencia de filas, el tiempo, el cierre del navegador y la limpieza del depurador no sustituyen esas pruebas.

Para intentos futuros es viable evaluar un protocolo versionado dentro del coordinador, adaptador, staging y repositorio actuales. Necesita dos condiciones independientes: impedir que el ejecutor anterior continúe y conocer qué efecto pudo ejecutar. Si cualquiera falta, conserva el bloqueo. La exclusión del ejecutor por sí sola no prueba ausencia de un efecto anterior.

La factibilidad está condicionada a demostrar exclusión del mismo recurso físico entre todos los ejecutores admitidos. Un lock local en una copia distinta de otro nodo no sirve. No se habilita recuperación donde esa identidad o la semántica del almacenamiento no puedan demostrarse.

## Evidencia de código

Referencias sobre el repositorio principal en `34099b316208431b2f5e6a58bea63ffa7b34ce24`; el manifiesto de evaluación conserva las huellas.

| Punto | Fuente | Implicación |
| --- | --- | --- |
| `_iniciar` precede a retener el original y crear la copia; `_invocar` está después | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb:31` | EN_CURSO no acredita que entró el escritor. |
| Evidencia inicial `ALMACENAMIENTO_PREPARADO`, sin IDs documentales | `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Documento.vb:51` | El diario actual no distingue autorización de invocación y resultado observado. |
| Retenciones externas desde la comprobación previa hasta el finally | `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Documento.vb:51` | Hay una base para conservar el original durante la llamada, no un certificado de resultado. |
| La retención devuelve un lector `FileShare.Read`; `recepcion.lock` se libera al salir de `ConservarVerificado` | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb:81` | Tener `recepcion.lock` libre no prueba que terminó una llamada. Un segundo lector tampoco prueba exclusión. |
| `FileShare.None` ya se usa bajo el lock del lote | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.Limpieza.vb:8` | Es un precedente de exclusión física. Su API exige COMPLETADO y puede eliminar; no debe invocarse para recuperar EN_CURSO. |
| La llamada legacy devuelve IDs por referencia y el adaptador los copia en finally | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.Legacy.vb:30` | Puede capturarse una observación futura a la salida, sin cambiar el método compartido. Un proceso terminado abruptamente puede no dejar esa observación. |
| Resultado y consolidación usan una transacción exterior | `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Documento.vb:54` | La recuperación debe conservar esta atomicidad y sus predicados de identidad/revisión. |
| Reconciliación exige INCIERTO con evidencia e IDs conocidos | `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Reconciliacion.vb:95` | Añadir EN_CURSO a una lista de estados sería insuficiente y peligroso. |
| La verificación actual coteja IDs recibidos contra gabinete/inventario/índice y valores | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.Documento.vb:57` | Puede reutilizarse para evidencia registrada; buscar una fila parecida no demuestra que pertenezca al intento. |

## Alternativas examinadas

| Alternativa | Decisión | Motivo |
| --- | --- | --- |
| Expirar EN_CURSO por fecha o cierre de navegador | Descartada | No impide que una llamada continúe. |
| Liberar al observar cero filas | Descartada | Es una observación temporal y no cubre una invocación pendiente ni todos sus efectos. |
| Usar solo `recepcion.lock` o adquirir otro lector | Descartada | El lock del lote es breve y los lectores son compartidos. |
| Usar el endpoint de limpieza | Descartada | Sus precondiciones y finalidad son distintas; incluye eliminación. |
| Reconstruir DocumentoId/InventarioId por nombre y propietario | Descartada | La coincidencia no vincula el efecto con el intento. |
| Mover `_iniciar` junto a `_invocar` | Mejora preventiva opcional | Reduce la ventana EN_CURSO, pero una caída entre commit e invocación sigue siendo ambigua; no resuelve69. |
| Diario de autorización/retorno más exclusión física versionada | Candidata condicionada | Permite reconocer casos limitados sin reejecutar el escritor ni crear otro motor. |

## Protocolo candidato V2

V2 es una versión de evidencia del intento, no otra máquina de estados. Continúan RESERVADO, EN_CURSO, CONFIRMADO, RECHAZADO e INCIERTO. Se reutilizan `ra_integracion_intento.evidencia_doc` y `ra_integracion_evento`; no se proponen tablas, migraciones, workers o flags de activación.

1. El adaptador adquiere una retención del original antes del inicio durable y la conserva durante preparación, autorización de invocación, llamada, observación de retorno y persistencia. Debe garantizarlo dentro del propio adaptador, incluso si se llama fuera del coordinador.
2. El inicio durable registra V2, identidad del intento/recurso, huella de contexto/snapshot/recepción y capacidad de exclusión del proveedor. Todos se derivan en servidor. Un intento sin esta evidencia sigue siendo V1; no se reconstruye ni rellena retrospectivamente.
3. Antes de `_invocar`, una operación del repositorio con el orden de locks existente registra una única autorización de invocación, vinculada a token, solicitud y revisión. Si falla o su respuesta es incierta, el adaptador no invoca. El registro precede a la llamada; puede haber autorización sin efecto, y ese caso permanece ambiguo si no hay otra evidencia.
4. Al salir de la invocación, un finally del adaptador intenta registrar una observación cerrada del retorno y los IDs devueltos, sin reemplazarla por éxito inferido. No suprime abortos ni promete registro ante terminación abrupta. Si el registro falta, la recuperación no inventa su contenido.
5. El checkpoint normal conserva el flujo existente. El diario adicional no crea ni confirma un documento por sí mismo. Los errores públicos y logs excluyen cuerpos, conexiones, rutas, cookies y tokens.

El finally no es una garantía frente a terminación del proceso: .NET documenta, por ejemplo, que `Environment.FailFast` no ejecuta bloques finally. Esa API no se usará como mecanismo de recuperación ni de pruebas de esta evaluación. [Referencia de Microsoft](https://learn.microsoft.com/en-us/dotnet/api/system.environment.failfast?view=netframework-4.8.1).

Los nombres candidatos de evento son `AUTORIZAR_INVOCACION_DOCUMENTAL` y `OBSERVAR_RETORNO_DOCUMENTAL`. La implementación deberá fijar sus documentos tipados y comprobar compatibilidad con el serializador y la normalización actual antes de declararlos estables.

La autorización actualiza el documento V2 y añade su evento en la misma transacción. La rama negativa exige un inicio V2 original coherente que siga en fase de preparación, además de cero autorizaciones, retornos y resultados; no se basa solamente en que falta una fila de evento. Una combinación contradictoria queda bloqueada. El checkpoint V2 debe validar la fase autorizada y sus pruebas; no se relaja la comparación exacta de evidencia inicial que protege V1.

```mermaid
flowchart LR
    R[RESERVADO] --> H[Retención viva del original]
    H --> E[EN_CURSO con evidencia V2]
    E --> G[Autorización durable de invocación]
    G --> L[Una llamada legacy]
    L --> O[Observación durable del retorno]
    O --> C[Checkpoint y consolidación existentes]
```

No hay una transición automática basada en un reloj. El diagrama muestra el orden de las pruebas, no una garantía de que toda caída deje un resultado recuperable.

## Exclusión para recuperar

La recuperación obtiene una guardia no destructiva del staging. Bajo el lock breve del lote intenta abrir el original exacto con exclusión incompatible con los lectores del ejecutor. Comprueba referencia, bytes, contenido e identidad física del proveedor; libera el lock del lote tras obtener los handles necesarios y conserva la guardia exclusiva hasta cerrar la transacción operativa.

La adquisición debe ser sin espera prolongada: un conflicto devuelve bloqueo y libera todo lo adquirido. Un original ausente, cambiado, con enlaces, sin permisos o en un proveedor sin semántica acreditada devuelve no elegible. No se crea, copia, elimina o reemplaza el original para obtener una prueba.

Ningún puerto de recuperación tiene acceso a `_invocar` o al almacenador. La exclusión impide avanzar a un ejecutor V2 anterior. Un ejecutor pausado antes de adquirir la retención no puede tener inicio V2 durable. Uno pausado dentro de la invocación conserva el lector e impide adquirir la guardia exclusiva. Uno que terminó pierde la retención, pero su resultado todavía debe clasificarse mediante el diario.

La prueba solo es válida si todos los ejecutores potenciales usan el mismo recurso físico y la misma disciplina V2. Un entorno distribuido no acreditado y los intentos V1 quedan excluidos. La implementación debe fallar cerrada si no puede demostrar esa condición mediante datos del proveedor y del intento; un campo aportado por el cliente no la acredita.

La documentación distingue un lector compartido (`FileShare.Read`) de la exclusión de nuevas aperturas (`FileShare.None`). Esa semántica sobre un archivo no certifica por sí misma la identidad del recurso entre nodos; esa parte es una condición adicional del diseño y de RV-02. [Referencia de Microsoft](https://learn.microsoft.com/en-us/dotnet/api/system.io.fileshare?view=netframework-4.8.1).

## Decisión de recuperación

| Evidencia y estado | Resultado candidato |
| --- | --- |
| V1, sin versión o prueba incompleta | Conservar EN_CURSO y la reclamación. Incluye69. |
| Guardia exclusiva no obtenida | Conservar estado; ejecutor no descartado. |
| V2 y guardia válida, cero autorizaciones de invocación, cero resultados/IDs, ausencia física concordante | Cierre explícito sin invocación como RECHAZADO; conservar recepción/expediente/valores e inventario. |
| V2 y guardia válida, autorización única, retorno registrado con IDs completos, verificación física positiva y contexto vigente | Confirmar los mismos IDs y consolidar mediante una transacción; no almacenar. |
| Autorización de invocación sin retorno fiable, IDs incompletos, pruebas contradictorias, retorno sin autorización o resultado físico discrepante | Conservar bloqueo. Cero filas no cambia esta decisión. |
| Checkpoint ya confirmado o cierre ya persistido | Devolver su historial autorizado, sin escritura nueva ni afectar una reclamación posterior. |

El cierre negativo candidato usa un código distinto de `RESERVA_CERRADA_SIN_INICIO`: `EJECUCION_CERRADA_SIN_INVOCACION`. La respuesta confirma el cierre del control, nunca un documento. Sería una excepción nueva, estrecha y condicionada a V2 para devolver el archivo a NO_INICIADO y retirar únicamente su puntero. No añade EN_CURSO→NO_INICIADO a las transiciones generales. Una ejecución posterior exige otra petición autorizada, revisión actual y clave nueva; no se encadena a la recuperación.

La rama positiva solo admite IDs capturados por el servidor en la observación de ese intento. Reutiliza el verificador de catálogo y un comando interno de comprobación que no pueda ejecutar almacenamiento. No modifica ni sustituye el resultado de una invocación en vuelo, ni fuerza EN_CURSO a INCIERTO para reutilizar una API.

## Autoridad, concurrencia e idempotencia

- Reutilizar sesión Gestión, autoridad de importación, CSRF, contexto persistido y snapshot. No aceptar actor, prueba de ausencia, PID o `resuelta=true` enviados por el cliente.
- Reutilizar referencias del DTO de reconciliación; cualquier operación nueva del dispatcher debe pasar los mismos guards. El nombre público se fijará en la revisión de implementación.
- Leer la vista, obtener la guardia, verificar y reautorizar; bajo locks lote→raíz→archivo→intento volver a comparar vista, revisión, token/hash, marcadores y diario. Conservar la guardia hasta commit/rollback.
- En la misma transacción persistir resultado de recuperación, punteros/revisiones, evento y consolidación. No sostener transacciones de base durante la llamada legacy o la adquisición del lock físico.
- Repetir la recuperación devuelve el resultado durable del mismo intento. Nunca limpia el puntero de un intento posterior ni vuelve a invocar el escritor.
- Con detención vigente, la propuesta bloquea el cierre negativo hasta fijar expresamente esa semántica. Una confirmación positiva preserva señal e inventario y usa la consolidación existente.

## Aplicación al intento69 y despliegue

El intento69 sigue excluido. No se ha instalado V2 en su ejecutor, no tiene el diario nuevo y no existe una conversión retroactiva válida. Un despliegue de la funcionalidad no cambia su elegibilidad. Los intentos históricos41/53 también permanecen excluidos.

La compatibilidad debe probar que V1 conserva su conducta y que un despliegue mixto nunca inventa V2 ni habilita recuperación en nodos/proveedores sin exclusión acreditada. Ninguna entrega local concede autorización para desplegar, detener workers, cambiar permisos o actuar sobre datos reales.

## Revisión necesaria

### Corte posterior RV-07

La continuidad autorizada tras RV-06 incorpora6.7, verificación de D-12/RQ-12..D-16/RQ-16 sin ampliar contratos ni cambiar producto.12 casos nuevos con barreras explícitas combinan adaptador/repositorio/guardia, salida con respuestas perdidas, dos recuperadores, rollback y lector en otro proceso.133 casos V2 internos y223/223 Node pasan; dos lectores nuevos cerrados. La matriz distingue cada ventana y sus pruebas previas de diario, proveedor, V1, autoridad e historial. Progreso24/34; sigue RV-08. Es verificación local con SQL/escritor/transacción simulados; no califica MySQL/IIS ni una llamada legacy real. Fuente:`doc118-matriz-v2-local-20261109.md`.

### Corte posterior RV-06

La continuidad autorizada tras RV-05 incorpora6.6/D-16/RQ-16:RecuperarDocumentoV2 en ASMX con cuatro referencias cerradas, guard operativo y autoridad RECONCILIAR. La composición añade solo recuperación y fija HttpContext/RA/DA; no conecta un escritor. Su respuesta diferencia cierre RECHAZADO sin invocación de documento confirmado y conserva privacidad e historial autorizado.19 casos HTTP nuevos,223/223 Node; dispatcher/composición reales compilados, HTTP sintético/guardia real y SELECT/transacción simulados. Sin despliegue, calificación MySQL real o cambios del almacenamiento habitual V1. Progreso23/33; siguen RV-07/RV-08. Fuente:`doc118-recuperacion-http-v2-local-20261109.md`.

Antes de implementar: aprobar esta extensión de RQ-07, el documento tipado V2 y la prueba del proveedor; integrar tareas al mapa activo y verificar la excepción negativa y los predicados positivos. La factibilidad del proveedor es una compuerta: si no puede demostrarse con el almacenamiento actual, la rama correspondiente debe permanecer deshabilitada por falta de capacidad, sin sustituirla por una declaración del operador.

La evaluación no elimina el bloqueo operativo de DOC-118 ni acredita sus casos reales pendientes. Progreso vigente:17/27. No hubo nuevas sesiones, HTTP, SELECT o efectos durante esta evaluación.

## Validación local posterior RV-02-A

El usuario solicitó continuar con la compuerta de exclusión. La prueba local con archivos sintéticos sobre NTFS pasó13/13 aserciones y confirmó salida de sus6 procesos hijo. Usa un handle de lectura exclusivo, identidad nativa del archivo abierto, tamaño y hash; rechaza otro archivo con los mismos bytes, reemplazo del mismo path, hardlinks, contenido cambiado, ausencia y guardia cerrada. No se abrió el staging real de ningún lote.

El control posterior a liberar la guardia permitió un nuevo lector:conservar el handle hasta commit y aplicar el fencing de estado siguen siendo obligatorios. El resultado no implementa el puerto del proveedor, no acredita identidades entre nodos y no habilita recuperación de69. RV-02-B continúa pendiente.

La identidad nativa se limita al mismo equipo y la vida del archivo; los IDs pueden reutilizarse y ReFS requiere un identificador distinto. No se usa un ID persistido como prueba suficiente de continuidad. [Referencia de Microsoft](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/ns-fileapi-by_handle_file_information). Evidencia y límites:`Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-staging-lock-probe-20261108.md`.

## Contratos internos posteriores RV-01

La continuación posterior autoriza6.1/D-11/RQ-11 para fijar los documentos sin conectar el protocolo. `DocumentosAlmacenamientoV2Importacion.vb` contiene inicio, autorización, retorno y un sobre de fase. El inicio liga propietario/ámbito/nivel, recurso, solicitud, huellas de contexto/snapshot/recepción/valores, expediente, gabinete, revisiones e identidad opaca del original. Autorización y retorno ligan huellas canónicas de sus documentos previos. No se guardan tokens, rutas, nombres de equipo, mensajes ni excepciones.

Construir una autorización en memoria no es obtener permiso de invocar; leer JSON con origen SERVIDOR tampoco prueba su procedencia. Estos son contratos internos, sin llamadas desde HTTP, adaptador, coordinador o repositorio. La procedencia durable y la emisión bajo retención siguen pendientes de RV-02-B/RV-03/RV-04. El retorno INCIERTO admite conservar IDs parciales o ausentes; nunca los inventa ni vuelve elegible ese caso.

Pasaron19 casos V2 en el harness de comportamiento existente y18/18 pruebas Node de comportamiento/cliente; las herramientas mantienen205/205. No se suman casos internos C# al total Node. Progreso activo18/28 por la nueva6.1 completada; quedan las mismas10 tareas anteriores. Evidencia:`doc118-contratos-v2-local-20261108.md`. El intento69 y D-07/RQ-07 permanecen sin cambios.

## Puerto posterior RV-02-B

La continuación autoriza6.2/D-12/RQ-12. El proveedor existente incorpora un puerto interno separado de V1, en un parcial. Abre únicamente originales y locks existentes; el lock del lote es breve y los handles del original/ancestros se conservan hasta Dispose. La retención compartida excluye escritura/borrado; la guardia exclusiva excluye nuevos lectores. Valida NTFS fijo local, ruta final del handle, identidad nativa, un único enlace, bytes/hash y vínculos de contexto/recurso. Rechaza junctions, UNC, dispositivos, ADS y ausencia; no crea, copia, retira o reemplaza archivos.

Una época aleatoria por AppDomain y la identidad de la raíz fijan las referencias opacas. No acredita otro proceso/nodo o reinicio por MachineName, PID o hash:la recuperación tras reiniciar el ejecutor sigue bloqueada. La identidad nativa tampoco es permanente. Estas restricciones son deliberadas; no se convierten en capacidad del despliegue ni subsanan la falta de inicio/diario V2 de69.

ConservarDurante ejecuta una acción síncrona con exclusión y validación antes/después, rechaza Dispose reentrante y hace esperar Dispose concurrente. El futuro consumidor debe conservar commit/rollback dentro de esa sección; el puerto no incorpora SQL, gates ni decisión de recuperación. RV-03/RV-04/RV-05 permanecen pendientes.

21 casos del proveedor real pasaron en el harness existente, con2 lectores de procesos separados y1 helper de junction cerrados; regresión18/18 y herramientas205/205. Se reutilizó la fixture de junction existente. Microsoft documenta la compartición/handles de directorio y la identidad por handle:[CreateFileW](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilew), [GetFileInformationByHandle](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-getfileinformationbyhandle), [GetFinalPathNameByHandleW](https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-getfinalpathnamebyhandlew). Progreso19/29, mismas10 tareas anteriores. Evidencia:`doc118-puerto-staging-v2-local-20261108.md`.

## Recorrido interno posterior RV-03

La continuación autoriza6.3/D-13/RQ-13. El nuevo parcial del mismo adaptador compone autorización, datos, copia, escritor y verificador existentes con un puerto obligatorio de persistencia V2. Retiene el original antes del inicio y hasta salir. Inicio y autorización requieren acuses internos concordantes; una respuesta desconocida o alterada termina en incertidumbre sin escritor, retries o fallback V1. La instancia consume una ejecución con Interlocked; la exclusión durable entre instancias depende de RV-04.

La observación se intenta en finally al salir del escritor, conservando IDs parciales. Su estado INCIERTO no se confunde con la verificación física posterior. Solo un acuse de observación exacto permite seguir a verificación/checkpoint; un checkpoint sin respuesta conserva IDs y resultado público incierto. Cancelación/ThreadAbort se propagan y la retención se libera. El acuse no se serializa como autorización JSON y su tipo no demuestra commit; el futuro repositorio debe emitirlo solo tras persistencia confirmada.

20 casos nuevos del harness local usan el proveedor real sobre PDF sintético y dobles explícitos de persistencia/escritor. Incluyen pausa, concurrencia, respuestas perdidas, vínculos alterados, error/IDs parciales, cancelación y aborto de un hilo propio. No hay composición pública V2, implementación MySQL ni recuperación activada. Progreso20/30; mismas10 tareas anteriores pendientes. Evidencia:`doc118-adaptador-v2-local-20261108.md`.

## Persistencia posterior RV-04

6.4/D-14/RQ-14 implementa los callbacks de RV-03 en el mismo repositorio MySQL mediante Operar. Bloquea lote/raíz/archivo/intento y lee el diario bajo la misma transacción. Evidencia anterior, token, solicitud, contexto, recurso, revisiones y eventos se comparan antes del SQL parametrizado. Un gate solo puede transitar PREPARADO→AUTORIZADO una vez:su respuesta perdida no permite volver a obtener acuse. Inicio, autorización y observación cambian evidencia y añaden evento juntos; los acuses se construyen después del retorno de Operar, tras commit.

El checkpoint conserva las tres pruebas tipadas en el diario y guarda el resultado normal existente en intento/archivo, con evento y consolidación en la transacción exterior. Repetición confirmada solo devuelve historial idéntico; no consolida de nuevo. La proyección de una evidencia activa V2 usa un código distinto al preparado V1 y no expone las pruebas. El lector V1 conserva su conducta; su checkpoint no acepta una evidencia V2 activa.

20 casos SQL del mismo núcleo de producción se integran con el adaptador y un doble transaccional:concurrencia de gate, pérdidas de respuesta, rollback por excepción/cero filas/evento/consolidación, diario desordenado/contradictorio, contexto, stop, IDs parciales y replay. Regresión223/223 Node, más chequeo estático DDL ampliado; los casos C# no se suman al contador Node. No se abrió una conexión MySQL real ni se activó composición pública, recuperación o69. Progreso21/31. Evidencia:`doc118-persistencia-v2-local-20261108.md`.

## Recuperación interna posterior RV-05 —2026-11-09

6.5/D-15/RQ-15 implementa clasificación, comprobación y persistencia sin puerto de escritor en el coordinador/repositorio existentes. Lee protocolo/diario desde servidor, valida plan/inventario/contexto y adquiere la guardia real. PREPARADO coherente sin gate/retorno/resultado admite cierre RECHAZADO/EJECUCION_CERRADA_SIN_INVOCACION solo si no hay stop y tres SELECT no encuentran huellas documentales en el expediente. Otros documentos del mismo expediente bloquean esta rama deliberadamente conservadora.

RETORNO_OBSERVADO concordante con IDs completos reutiliza el verificador de catálogo por esos IDs. Un nuevo comando interno conserva EN_CURSO y EsVerificacion=true:los puertos de almacenamiento lo rechazan. No se fingen reservas, no se fuerza INCIERTO ni se repite almacenamiento.

Guardar vuelve a autorizar bajo locks, compara una huella exacta de vista/plan/diario incluidos IDs y fechas de evento, revisa la misma prueba física y mantiene la guardia mediante ConservarDurante hasta Operar/commit/rollback. Cambios concurrentes rechazan la escritura; evento y consolidación son parte de la transacción. Historial concordante se devuelve sin guardia/nuevas escrituras, incluso si hay otra reclamación posterior o se perdió la respuesta. La rama negativa conserva recepción/PDF/expediente/valores e inventario y solo retira su marcador.

22 casos locales pasan en el harness existente, con guardia real, verificador SELECT y transacción simulados, cero escritor. La regresión Node es223/223; no se suman los casos internos C#. El comando tiene un delta antes/después explícito sobre la revisión original; las otras15 fuentes de evaluación no cambian. Progreso22/32. HTTP, calificación real y69 siguen sin habilitar. Evidencia:`doc118-recuperacion-v2-local-20261109.md`.
