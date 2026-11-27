# Decisiones de implementación de radicación interna

Fecha: 2026-11-26. P01 en refinamiento. Este registro conserva acuerdos y separa las decisiones funcionales pendientes de las definiciones técnicas de trabajo. La continuación solicitada autoriza avanzar con P01; no aprueba opciones pendientes ni pruebas de ambiente.

## Acuerdos vigentes

| Acuerdo | Estado y alcance |
|---|---|
| D-RI-01 | Aprobado: interno/tercero como remitente; autenticado como ejecutor; responsable Workflow independiente. |
| D-RI-02 | Interfaz compacta aprobada; preservar campos dinámicos y capacidades reales. Datos/límites de ejemplo no son configuración de ambiente. |
| D-RI-03 | Confirmado: radicar y crear/entregar seguimiento. Elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente. |
| D-RI-04 | Sustituido por D-RI-05. No ejecutar la adaptación antes propuesta del cargador compartido. |
| D-RI-05 | Aprobado: control nuevo independiente, estado por operación/instancia y recepción temporal propia. |
| D-RI-06 | Instruido: núcleo crítico, caracterización previa, cambios indispensables justificados y comparación posterior. |
| D-RI-07 | Exigido: almacenamiento intocable. Prevalece sobre permisos genéricos de extracción; archivo central y funciones de baseline sin cambios, sobrecargas, clones o bypass. |

## Decisiones pendientes por etapa

El dueño funcional de las decisiones es el usuario/responsable del producto; el implementador aporta evidencia y registra el acuerdo sin atribuir nombres o aprobaciones que no se hayan recibido. Las verificaciones de configuración y compatibilidad son responsabilidad técnica del paquete indicado.

| Puerta | Estado al abrir P01 | Cierre necesario y consecuencia | Paquete que la consume |
|---|---|---|---|
| G01 | Pendiente; consulta enviada al usuario en esta continuación. | Elegir expediente autorizado antes de confirmar, o solicitar clasificación posterior condicionada a una ruta existente compatible. No asumir A por recomendación ni habilitar B modificando almacenamiento. | P01 para cerrar alternativa; P02/P04/P05/P06 para consumirla. |
| G02 | Pendiente. | Catálogo, ámbito y tipos de tercero. Sin ello se puede diseñar el contrato tipado, pero no publicar una búsqueda global ni elegir tablas reales. | P03. |
| G03 | Pendiente en la capacidad nueva. | Acceso directo, permisos de representación/carga/radicación y plantillas elegibles. Los permisos existentes se preservan, no conceden automáticamente todas las facultades nuevas. | P02/P03/P04. |
| G04 | Pendiente. | Selección de terceros existentes o alta/edición. Mientras se refina, alta/edición no forma parte del contrato público; la recomendación de seleccionar existentes no es aprobación. | P03. |
| G05 | Pendiente. | Campos del remitente en salidas y destinatarios de notificación/entrega. No redirigir correo por haber elegido un tercero. | P03/P05/P06. |
| G06 | Pendiente. | Extensiones, contenido, cantidad/tamaños y folios. Los 20/50 MB y 10 archivos del HTML no se trasladan al servidor. | P04. |
| G07 | Pendiente. | Separación o mezcla de orígenes. El contrato v1 admite ambos accesos; no habilita un manifiesto mixto ni cambio arbitrario de origen. | P02/P04/P06. |
| G08 | Pendiente. | Reanudación entre sesiones, caducidad, limpieza y atención de resultados inciertos. El diario durable no aprueba conservación indefinida ni permisos de soporte universales. | P02/P04/P05. |
| G09 | Consumidor existente identificado en código; validación de entrega/configuración pendiente. | Verificar que la obligación se lee desde el módulo de respuestas vigente. No elegir un nuevo motor de respuesta ni modificar su homologación. | P05. |
| G10 | Filtros de catálogo identificados; configuración del ambiente no consultada. | Determinar trámites elegibles por plantilla sin retirar los clásicos y comprobar su entrega al consumidor. | P02/P05. |
| G11 | Políticas existentes se conservan; cero días/edición del plazo pendientes. | Refinar esos valores al implementar configuración, sin equiparar plazo cero a ausencia de seguimiento ni cambiar cierre/transferencia. | P02/P05/P06. |

G01 es la única pregunta funcional que se adelanta en esta continuación. Las demás se refinan en el paquete que las necesita, conservando el trabajo independiente.

## Definiciones técnicas de trabajo v1

Son decisiones de diseño documentadas para P01, no cambios aplicados ni aprobaciones funcionales. Se versionan si la implementación revela una incompatibilidad.

| Ref. | Definición |
|---|---|
| T01 | Referencias de operación/archivo/revisión/intento separadas; contexto autenticado solo del servidor. |
| T02 | ASMX interno tipado y handler multipart propios; contratos del ASMX general y cargador compartido conservados. |
| T03 | IDs legacy en transporte como cadenas decimales canónicas, con validación de rango antes de cada firma Integer/Long. |
| T04 | Revisión estructural de operación y versión de archivo distintas: terminar un upload no invalida artificialmente otros archivos del mismo manifiesto. |
| T05 | Una revisión preparada identifica datos, configuración, destino, remitente y recepciones exactos. Editarlos invalida la preparación. |
| T06 | Clave estable por confirmación e intención; ante incertidumbre se consulta, sin clave nueva para forzar otro radicado. |
| T07 | Diario y correlación en tablas nuevas propuestas de operación, sin agregar columnas/flags al almacenamiento protegido. |
| T08 | Mapeo del núcleo y asociación de operación en una misma transacción solo si se demuestra recurso/conexión compatibles; almacenamiento permanece fuera de esa atomicidad global. |
| T09 | Almacenamiento con firmas vigentes mediante adaptador externo, copia temporal propia y referencias separadas de fuente/copia/rótulo. |
| T10 | Un efecto incierto no se reejecuta al vencer un plazo o lease; la recuperación requiere evidencia inequívoca o atención controlada. |
| T11 | Bootstrap autorizado de la página y token efímero del módulo; consultas de dominio sin registro/almacenamiento incidental. |
| T12 | El guard D-RI-07 se ejecuta antes/después; su baseline no se actualiza para permitir cambios. |

Ver [contratos v1](contratos-v1.md), [inventario](inventario-archivos.md) y [caracterización](matriz-caracterizacion.md).
