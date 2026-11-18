# DOC-120 — Viabilidad y contrato de procesamiento por archivo

**Dictamen: viable. Implementacion local autorizada por "implementa",2026-11-18.** Contrato concreto: [campos, procedencia, indice manual y compatibilidad](contrato-focal-implementado-doc120.md). Sin aplicacion del SQL ni despliegue de esta version. Base inspeccionada: `b7dbe3c0`, incluida la medición real de tres archivos de lote50. Este documento sustituye, para el desarrollo pendiente, la dependencia física entre PDF de un mismo lote/expediente. El mapa TXT actualizado resume el guardado implementado; el anexo/manifiesto captura sus fuentes.

La regla de negocio fijada por el usuario es que A puede almacenarse sin volver a comprobar los originales de B. El lote agrupa y controla progreso; la identidad del expediente, autorización y clasificación siguen siendo prerrequisitos estructurales. Ante un archivo no disponible se pausa antes del siguiente efecto y el usuario elige **Continuar con los demás** o **Pausar para revisar**. No hay omisión, reintento ni consentimiento automáticos.

## Hallazgos que condicionan la viabilidad

| Frontera real | Acoplamiento actual | Ajuste requerido |
|---|---|---|
| `CoordinadorImportacionCarpetas.Documento.vb:AlmacenarArchivoMedido` y `.Staging.vb:VerificarStagingCreacion` | Dos recorridos de todos los originales del ámbito. Cada original reautoriza, carga metadatos y se verifica físicamente. | Vista focal de A y retención física de A; los originales de B no se cargan ni abren para guardar A. |
| `MySqlImportacionCarpetasRepository.vb:ObtenerArchivo/Operar/BuscarLote` | Cada carga individual consulta cinco catálogos de estructura, relee lote/configuración y reconstruye JSON. | Contexto y esquema comprobados en la frontera pertinente, fuera de bucles por original; lecturas de control acotadas al recurso. No introducir caché de permisos entre solicitudes. |
| `.Control.vb:ValidarReservaDocumental/HayReclamacionDocumental` | Incertidumbre de B o estado agregado de su raíz bloquea nuevas reservas de A. | Separar elegibilidad individual, detención global y exclusión de un escritor todavía activo. La reserva continúa siendo transaccional, con CAS y token. |
| `.Barreras.vb`, `.Valores.vb` y catálogo/comando documental | La recepción completa y algunos estados agregados son prerrequisitos del recurso. | Validar pertenencia, recepción e identidad del expediente del objetivo; conservar las reglas de índices/tipología/fechas, sin dependencia física de otros PDF. |
| Creación: `ReservarCreacionConStaging/EvaluarBarreraCreacion` | La raíz puede quedar sin crearse porque otro archivo no llegó o desapareció. | Para la raíz elegible, disponer de al menos un PDF recibido y verificado, con sus datos de expediente válidos. Un faltante de B no impide crear la raíz para A; no crear una raíz sin ningún archivo disponible. |
| `.DocumentoV2.vb`, `.Consolidacion.vb` | Checkpoints y consolidación cargan vistas extensas; `INCIERTO` se propaga a raíz/lote. | Checkpoints focales y resúmenes calculados por resultados; no deserializar el manifiesto completo ni reconstruir todos los miembros por cada fase de cada PDF. |
| `ServicioConsultaImportacion:ProyectarEstado` y `ImportacionCarpetas.js:accionesRecurso/blocked/choose/start` | Un faltante o incidencia detiene toda la coordinación; faltan decisiones de continuar. | Capacidades de avance por recurso y estado de decisión explícito, con lista de excluidos solo para la ejecución consentida. |
| `ConsultarRecursoLote` y `readFocal/huellaRecursos` | La lectura focal todavía verifica huella de todos los IDs/revisiones. | Contrato focal versionado: acuse y consulta de recurso/padre; auditoría completa al inicio, al cierre o por consulta expresa, sin huella de todos los miembros tras cada PDF. |

Las dos pasadas físicas y la reconstrucción completa se deben retirar de **todas** las fronteras del camino individual, no únicamente del primer bucle. La medición atribuyó72,321% a metadatos y0,082% al escritor; son proporciones observadas en tres documentos, no tiempos garantizados de la nueva versión.

## Recorrido previsto

```text
Consultar lote y enumerar pendientes por inicio o reanudación
  (una enumeración lógica, paginada si lo exige el contrato)
                       |
                       v
Elegir archivo A por ID / crear su expediente si aún no existe
                       |
                       v
Reautorizar y comprobar contexto focal / original de A
    | disponible                         | no disponible
    v                                    v
Reservar A con revisión/token        Registrar incidencia segura de A
    |                                Mostrar archivo, fase, causa y avance
    v                                    |
Retener y validar A                       v
Fijar índices si corresponde         Esperar decisión del usuario
    |                                /                         \
    v                    Continuar con los demás        Pausar para revisar
Almacenar A una vez       excluir A en esta ejecución     conservar avance
    |                    y elegir otro elegible         sin nuevo efecto
    v
Comprobar resultado y checkpoint de A
    |
    +-- confirmado -> contar éxito; lectura focal; siguiente archivo
    +-- incierto -> conservar evidencia; consultar, nunca reescribir A

Cierre: resultados persistidos -> completa / con pendientes / por verificar
```

## Reglas del servidor

1. **Alcance por ID.** Cada recurso se obtiene por `archivo_id`, relación raíz/lote y usuario/empresa/destino autorizados. La pertenencia al lote no sustituye autorización, ni un ID enviado por el cliente concede una reserva. Se conserva sesión/CSRF/origen, snapshot, identidad por `ID_EXPEDIENTE`, gabinete, tipología e índices.
2. **Datos vigentes.** La selección inicial es una lista de candidatos, no una autorización futura. Cada efecto compara revisiones y obtiene su contexto focal actual. No se cambia ni se vuelve a ejecutar el origen de valores ya fijados. Configuración y catálogos compartidos se leen fuera del recorrido de originales; cualquier reutilización queda limitada a la solicitud y su origen/contrato, con reautorización inmediatamente antes del efecto.
3. **Reserva.** Reutilizar transacción, token, hash y CAS del repositorio único. Una consulta previa de `intento_activo_id` no basta para reclamar; dos peticiones sobre A producen como máximo un escritor. Una fila afectada no autoriza omitir el registro de intento/evento. No se promete cero SELECT totales ni una sola escritura de negocio por PDF.
4. **Una operación en vuelo.** Se conserva la ejecución secuencial y la exclusión de escritores activos. Un `RESERVADO/EN_CURSO`, marcador huérfano o autorización V2 sin retorno no se ignora por ser otro ID, por timeout ni por consentimiento del usuario. La independencia de originales no permite dos escritores simultáneos. Una incertidumbre de B solo admite avance seguro de A si la evidencia durable demuestra que el escritor de B terminó y las guardas vigentes lo permiten; en otro caso se conserva la pausa y la consulta/verificación. El usuario no puede liberar una reclamación mediante el diálogo.
5. **Integridad de A.** Validar existencia, tamaño, hash, PDF e identidad/ruta privada del original de A. Retenerlo hasta terminar su operación mediante los puertos V2 actuales. No abrir/hash/verificar B para guardar A. La época y evidencias existentes de recuperación V2 siguen siendo obligatorias; no confiar en un sello permanente de “ya verificado”.
6. **Creación y completitud.** Antes de almacenar debe existir un expediente confirmado para su raíz. Su creación sigue siendo única y requiere datos de raíz válidos y un original disponible; no exige la recepción física de otros PDF. `COMPLETO_OBLIGATORIO` conserva el inventario y solo permite declarar cierre completo cuando todos los documentos previstos están confirmados. `EXPEDIENTE/LOTE` conservan su alcance de evaluación final, sin actuar como barrera física cruzada por documento. `PARCIAL_PERMITIDO` y las incidencias de preparación mantienen su trazabilidad. No convertir el perfil a parcial ni reescribir snapshots para conseguir rendimiento.
7. **Información de B.** Un faltante, rechazo o resultado incierto individual no elimina su fila ni reduce el total original. La identidad del expediente confirmado no se pierde porque su estado documental agregado tenga pendientes. Un problema de identidad/creación de la raíz sí impide almacenar sus hijos; no se inventa un expediente a partir del nombre/código.

## Archivo no disponible y decisión del usuario

| Situación | Certeza y registro | Acciones |
|---|---|---|
| Falta `File` local para un `PENDIENTE` | No hubo envío; el registro servidor sigue pendiente. Incidencia en la vista, sin inventar recepción o intento remoto. | Aviso y decisión; reselección existente cuando el usuario revise. |
| El original recibido no existe/no se puede leer/no coincide, antes de reclamar | Se puede acreditar cero invocaciones del escritor de esa solicitud. Registro focal y evento seguros. | Decisión para seguir con otros o pausar; no confirmar A ni marcarlo completado. |
| Falla el original después de reservar, con evidencia inequívoca de no autorización/invocación | Cierre controlado de la reserva propia bajo token/revisión y diario; evento durable, conservando recepción, valores e historial. | Después del cierre confirmado se permite decidir continuar con otros. |
| Existe autorización/invocación posible, respuesta perdida o fallo del checkpoint | `INCIERTO`, preservando IDs, evidencia y reclamación. No transformarlo en falta de archivo sin efectos. | Consultar/verificar; sin retry ni cierre inferido. El avance de otros requiere la guarda de exclusión de ejecución indicada arriba. |
| Sesión/permiso revocado, destino distinto o identidad de raíz inválida | Falla de autoridad/contexto, no una incidencia que pueda omitirse desde UI. | Detener el alcance afectado; no ofrecer continuar para eludir autorización. |

Para un faltante **con cero invocación acreditada**, reutilizar `estado_efecto=NO_INICIADO`, `resultado_doc` con resultado negativo de causa cerrada y sin IDs documentales, y `ra_integracion_evento`. Si hubo reserva propia, cerrar su intento como `RECHAZADO`, sin borrar evidencia. El lector/reservador deberá reconocer únicamente este resultado no documental específico: tras una revisión explícita y un preflight físico nuevo puede reclamarlo con CAS y conservar el evento anterior. Al reclamar, consumir el negativo reconocido del archivo de forma atómica para que el nuevo V2 parta de su resultado vacío, preservando el historial anterior; no borrar otro resultado ni reutilizar esta excepción para cualquier `RECHAZADO` o `INCIERTO`. La forma publica exacta y la compatibilidad de los codecs se fijan en el contrato focal implementado de3.171.

El diálogo muestra nombre/ruta relativa, fase, origen del faltante, causa comprobada, confirmados/pendientes y la certeza del efecto. Dos botones: **Continuar con los demás** y **Pausar para revisar**. Escape/cierre equivale a pausa; foco contenido y devuelto al recurso; sin `alert/confirm` nativos. Se abre una sola vez por recurso/fase/decisión en esa ejecución. Continuar excluye únicamente ese recurso, sin descartarlo del inventario o consentir reintentos; la decisión no se restaura como autorización automática después de recargar. Un cambio de cuenta/lote/inventario invalida la decisión pendiente.

**Límite real de recuperación:** `AlmacenStagingImportacion.VerificarReenvio` comprueba un original ya recibido; no reconstruye un `.bin` que desapareció. Este cambio no inventa recepción automática de reemplazo para `RECIBIDO`. Se permite vincular originales locales todavía pendientes y revalidar un original retenido que vuelva a estar disponible por un procedimiento autorizado. Si falta el temporal de un recibido, la UI explica la incidencia y conserva el pendiente; restaurarlo mediante una nueva capacidad de escritura necesitará su contrato específico. Esto no impide continuar con otros archivos disponibles.

## Repositorio, contrato focal y consolidación

- Misma clase parcial `MySqlImportacionCarpetasRepository`; no otro escritor ni otra plataforma de jobs. Tablas de control: archivo/intento/lote/evento, raíz cuando cambia su resumen. No retirar las escrituras V2 de preparación, autorización, retorno y resultado por reducir viajes SQL.
- Lectura operativa focal de archivo/raíz/lote y sellos del plan/snapshot confirmado. Evitar `CargarPlan` y JSON de todos los originales por cada reserva/checkpoint. La proyección parte de `origen_doc`, `datos_doc` de raíz y `plan_valores_doc`, emitidos por la confirmación y no modificables después; se contrasta con cabecera de plan confirmado, contexto, snapshot y sus reglas. Para plan1 y plan2 se debe demostrar esta procedencia/inmutabilidad en3.171, sin confiar en datos suministrados por el navegador ni crear una caché permanente de autoridad. Los fragmentos completos quedan para preparación, consulta exhaustiva, cierre o recuperación que realmente los requiera.
- Mantener `ConsultarRecursoLote`; ampliar su contrato de forma versionada. La solicitud/respuesta focal incorporará `versionLectura` (1 por defecto;2 para la ruta nueva). La versión1 conserva huella y comportamiento actuales. La versión2 identifica recurso/padre/estado/revisiones y certeza sin exigir huella de todos los miembros ni devolver progreso global como si hubiese sido recontado. El host anuncia la capacidad y el cliente la exige; servidor anterior no se interpreta como acuse nuevo ni provoca replay.
- La enumeración inicial y las consultas expresas siguen siendo paginadas y autorizadas, sin filtros ocultos de interfaz. Se conserva el inventario confirmado y la detección de duplicados por ID. El listado de candidatos no cambia su pertenencia; una modificación concurrente se controla al reclamar y en la lectura final, no recorriendo de nuevo todos los IDs después de cada efecto.
- Consolidar resultados con consultas agregadas/cambios focales; el estado final completo exige recuento coherente contra el inventario original y pruebas de resultados. No construir todos los objetos JSON ni releer físicamente originales completados para pintar el progreso de A. Contadores por acuses confirmados durante la ejecución; recuentos globales al inicio/cierre/consulta expresa, identificando su momento. Un agregado SQL puede leer varias filas: no presentarlo como cero lecturas físicas del motor.
- Reutilizar estados existentes. `PARCIAL` representa cierre con incidencias/pendientes según datos; `PENDIENTE_VERIFICACION` conserva incertidumbres, `DETENIDO` la detención persistente. El rótulo “Importación con pendientes” es presentación, no un estado SQL nuevo ni una eliminación del trabajo. La capacidad individual no se deduce únicamente de un estado agregado. El lote no es `COMPLETADO` si falta un documento original.
- **Punto tecnico resuelto en3.171/3.197:** proyeccion interna de procedencia y referencias en eventos existentes, con indice manual09. El [contrato implementado](contrato-focal-implementado-doc120.md) fija cabecera, prueba, unicidad logica bajo lock, codecs y compatibilidad. Ningun SQL se aplica desde la DLL.

## Presupuesto y verificación

El objetivo es una enumeración inicial por inicio/retoma, trabajo focal acotado por archivo/fase y recuento final; cero cargas por ID ni aperturas físicas de otros originales para almacenar A. Las consultas internas por archivo deben mantenerse acotadas independientemente de1/100/1000 originales. Las reservas/checkpoints siguen requiriendo lecturas y escrituras de control; no se acepta como presupuesto una promesa de “1 SELECT total”. El conteo real incluirá `information_schema`, autoridad, catálogos, cargas de plan/JSON, V2 y consolidación, separado de los SELECT de un ejecutor externo.

Verificar, al menos: A válido/B ausente/C válido; A se almacena después de consentimiento para saltar B; pausa no despacha C; no decisión no avanza; B modificado; ausencia antes/después de reserva; guardas V2 antes y después de AUTORIZADO; dos sesiones sobre A; escritor de B todavía activo; raíz existente y por crear; mismo expediente/raíces distintas; perfil completo EXPEDIENTE/LOTE y parcial; plan1/2; revocación, filtros/CSV/árbol; recarga y acuse perdido. Probar control de carreras/rollback con MySQL real en su ventana propia; un doble no lo acredita.

Primero medir localmente comandos/operaciones, llamadas por ID, reconstrucciones y accesos a staging con1/100/1000 archivos y transporte demorado, sin efectos reales. Después compilar y ejecutar regresión focal. La comparación real usará archivos pendientes equivalentes, mismo perfil y ambiente, autorización nueva y topes explícitos; no repetir los documentos ya completados ni reutilizar la ventana cerrada de tres archivos. Separar resultados locales, despliegue y prueba real. No garantizar una reducción porcentual antes de observarla.

## Entrega y reversibilidad

Actualizar el mapa TXT **resumido de guardado**, el anexo/manifiesto de fuentes y la guía al finalizar la implementación, diferenciando esta propuesta del código actual. El resumen mostrará archivo seleccionado, comprobaciones propias, reserva, writer, checkpoint, decisión de usuario y cuellos restantes. La evidencia incluye compatibilidad de lectura de datos históricos y de incidencias nuevas; ningún rollback rota tokens, libera intentos, borra documentos o repite efectos. Despliegue con respaldo y prueba real necesitan su autorización específica.

Tareas3.171–3.196 en el cambio activo. Las once tareas anteriores de aceptación/capacidad/DDL específico permanecen abiertas. Este dictamen no las completa y no ejecuta una nueva importación.
