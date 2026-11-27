# Exploración de trámites con respuesta en radicación interna

**Límite de implementación confirmado, D-RI-03, 2026-11-26:** este módulo radica y crea/entrega el seguimiento con modalidad, plazo y responsable. La elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente de respuestas. El análisis del consumidor y su cierre que sigue aporta contexto de compatibilidad; no autoriza implementar esas operaciones en radicación interna ni cambiar sus políticas. Las propuestas anteriores se interpretan con este límite.

La radicación interna puede utilizar trámites con respuesta. El registro actual ya consulta la configuración del trámite y crea seguimiento en `ra_respuesta_radicado` cuando corresponde. La activación completa depende del catálogo de la plantilla interna, la modalidad de respuesta, el plazo, la asignación y el recorrido de respuesta/cierre. No equivale a mostrar nuevos nombres en un combo.

Hay dos puntos que requieren atención antes de ampliar su uso: una función de lectura de modalidad consulta la columna de activación del seguimiento, y los filtros de catálogo interno y clásico se excluyen entre sí. Además, la respuesta formal reutilizable genera una radicación saliente; debe confirmarse si ese es el resultado esperado para una solicitud interna.

| Referencia | Valor |
|---|---|
| Fecha | 2026-11-25 |
| Base de la revisión | `986f0f1c` |
| Estado | Exploración de capacidad; sin activación de configuración ni cambios de aplicación |
| Interfaz vigente | Aprobada en D-RI-02, commit `23464d6b` |
| Relación con otras exploraciones | [Diagnóstico](diagnostico.md), [carga directa](exploracion-carga-directa.md) y D-RI-01 para remitente tercero |

## Conceptos que utiliza el código

La configuración reside en `tipo_doc_entrante`. Estos campos cumplen funciones distintas:

| Campo | Uso verificado |
|---|---|
| `requiere_respuesta` | Activa la creación de seguimiento: el registro interno prepara la inserción cuando su valor es distinto de cero. |
| `obliga_respuesta_radicado_tramite` | El validador de respuesta diferencia `1`: respuesta con radicado; `0`: confirmación. No debe confundirse con la activación del seguimiento. |
| `resp_correo_fisico_electronico` | Define el canal de envío: el recorrido revisado diferencia físico/convencional y electrónico. Es independiente de exigir respuesta formal. |
| `numero_dias_vence` | Alimenta el cálculo de vencimiento por trámite. |
| `tipo_activo_rad_interno` | Participa en la elegibilidad del catálogo interno. |
| `activo_modulo_respuesta` | Se resuelve durante el registro y se entrega a la creación de la tarea como configuración de módulo de gestión. |

Las combinaciones coherentes con las ramas revisadas son:

| Seguimiento requerido | Modalidad configurada | Recorrido esperado |
|---|---|---|
| No: `requiere_respuesta = 0` | No aplica para crear seguimiento | La radicación interna no crea la solicitud de respuesta por esta condición. El cierre conserva los demás requisitos de Workflow. |
| Sí: valor distinto de cero | `obliga_respuesta_radicado_tramite = 0` | Crea seguimiento y exige confirmación de respuesta; el validador comprueba `FECHA_RESPUETA`. |
| Sí: valor distinto de cero | `obliga_respuesta_radicado_tramite = 1` | Crea seguimiento y exige respuesta formal con radicado, fecha y documento según la rama de envío. |

Esta matriz describe el código; no acredita qué combinaciones están configuradas en el ambiente. Otros valores de modalidad deben inventariarse antes de considerarlos soportados.

## Capacidades existentes y evidencia

| Capacidad | Evidencia | Conclusión |
|---|---|---|
| Catálogo interno | [ClassGaProducionDocumental.vb](../../../Gestion/ClassGaProducionDocumental.vb), `Lista_tipos_documentales_de_radicacion_interna`, línea 5473. Filtra por plantilla, `estado_tipo_documento = 1`, `tipo_activo_pqr <> 1` y `tipo_activo_rad_interno = 1`. | No excluye los trámites por requerir respuesta. Pueden aparecer si cumplen el resto del contexto. |
| Crear seguimiento | [ClassRadicador.vb](../../../radicador/ClassRadicador.vb), `Registra_Radicacion_entrante_interna`, líneas 2633–2654 y 2915–2940. | Consulta `requiere_respuesta`, inserta solicitud y auditoría de respuesta dentro de la transacción del registro. |
| Responsable | [ClassRadicador.vb](../../../radicador/ClassRadicador.vb), líneas 2097–2247, 2527 y 2917. | El usuario de gestión resuelto desde la asignación es el propietario inicial del seguimiento. El tercero remitente no sustituye esa identidad interna. |
| Fecha de vencimiento | [ClassRadicador.vb](../../../radicador/ClassRadicador.vb), `Seleccion_tipo_tramite`, línea 17557, y extracción del campo `FECHALIMITERESPUESTA`, línea 2804. | Existe cálculo con días laborales y persistencia del vencimiento; debe comprobarse coherencia entre plantilla, control y registro de respuesta. |
| Seguimiento y gestión | [Class_ra_respuesta_radicado.vb](../../../radicador/Class_ra_respuesta_radicado.vb), lectura de estructura y funciones de semáforo; [Classgestionrespuesta.vb](../../../Gestion/Classgestionrespuesta.vb), gestión de respuesta, confirmación y reasignación. | Se dispone de componentes reutilizables; hace falta verificar su acceso con la plantilla interna y el propietario del seguimiento. |
| Validar la obligación | [Classgestionrespuesta.vb](../../../Gestion/Classgestionrespuesta.vb), `Verifica_respuesta_radicado_sin_respuesta`, líneas 8431–8651. | Distingue confirmación y respuesta formal; no consiste únicamente en haber cargado un archivo. |
| Validar transiciones modernas | [WorkflowLegacyRequisitosAdapter.vb](../../../Infrastructure/Workflow/Terminar/WorkflowLegacyRequisitosAdapter.vb) y [WorkflowLegacyEnvioUsuarioRequisitosAdapter.vb](../../../Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioUsuarioRequisitosAdapter.vb). | Reutilizan la comprobación de respuesta al ejecutar sus respectivos recorridos. |
| Elaborar respuesta formal | [Classgestionrespuesta.vb](../../../Gestion/Classgestionrespuesta.vb), `Subir_respuesta_radicado` y `upload_subir_respuesta_radicado`, alrededor de líneas 11082–11225. | Resuelve plantilla predeterminada de respuesta, homologa un trámite y llama a `Registra_Radicado_plantilla_respuesta` con `RADICACIONSALIENTE`. |

## Hallazgos que condicionan la activación

### La modalidad inicial puede confundirse con la activación del seguimiento

En [Class_tipo_doc_entrante.vb](../../../radicador/Class_tipo_doc_entrante.vb), `Retorna_tipo_respuesta_tramite_radicado` consulta `requiere_respuesta` en la línea 1052. `Retorna_estado_respuesta_obligatoria`, pese a su nombre, consulta esa misma columna en la línea 1081. La radicación interna usa el resultado de la segunda función para inicializar `TIPO_RESPUESTA_ELAB_USUARIO`.

El lector `Solicita_estado_obligatoria_respuesta_tramite`, línea 574, sí consulta `obliga_respuesta_radicado_tramite`. La gestión y validación posteriores utilizan ese lector y algunas pantallas recalculan la modalidad mientras no hay respuesta elaborada.

Por ejemplo, si un trámite tiene seguimiento activado con valor `1` y modalidad `0`, la inserción inicial toma `1` de la primera columna mientras el validador posterior interpreta `0` como confirmación. Es una inconsistencia de lectura confirmada en código; no demuestra que el ambiente tenga esa combinación ni un fallo de usuario ya ocurrido. La función se comparte con otros recorridos de radicación: la corrección debe identificar consumidores y comprobar ambas modalidades antes de cambiarla.

### Marcar un trámite como interno puede retirarlo del catálogo clásico

La lista interna exige `tipo_activo_rad_interno = 1`. La lista clásica `Listar_Tipos_Documentales` en [ClassRadicador.vb](../../../radicador/ClassRadicador.vb), alrededor de línea 13971, exige `tipo_activo_rad_interno <> 1`. Cambiar ese atributo de un trámite compartido modifica su disponibilidad en el otro módulo.

Se requiere inventariar plantilla y consumidores antes de activar o crear una configuración específica para interna. Tampoco conviene duplicar descripciones sin control: varios lectores resuelven trámite y reglas por plantilla más `Descripcion_Doc`, aunque el combo tenga un ID.

### El plazo y la plantilla deben ser coherentes con la obligación

`Seleccion_tipo_tramite` sale con `If numero_dias = 0 Then Exit Function` antes de completar la resolución de flujo. El vencimiento del seguimiento se toma del metadato activo `FECHALIMITERESPUESTA`. Una fecha visible en la maqueta no prueba que la plantilla real cree, conserve y valide ese valor.

Para activar respuesta con plazo deben comprobarse días configurados, calendario, control generado, obligatoriedad y valor persistido. El significado de cero días requiere definición funcional; no se debe inventar un vencimiento ni considerar cero como ausencia de respuesta.

### El control de respuesta depende del propietario y del tipo de transición

`Verifica_respuesta_radicado_sin_respuesta` busca el seguimiento por radicado y usuario de gestión. Si no encuentra un registro del usuario, devuelve `YES`. Por eso debe probarse que el propietario inicial, las bandejas y las reasignaciones mantengan la relación correcta.

Los adaptadores modernos de transición por conector y envío a usuario consultan esa obligación. [WorkflowLegacyEnvioGrupoRequisitosAdapter.vb](../../../Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioGrupoRequisitosAdapter.vb) conserva una política distinta: consulta aprobaciones pendientes y no ejecuta esa misma validación de respuesta. Esto describe una diferencia de contrato, no demuestra por sí solo un cierre indebido. No se puede afirmar que todas las operaciones bloqueen igual ni agregarles indiscriminadamente una regla nueva. Se debe comprobar el recorrido concreto hasta el cierre y la conservación del propietario al transferir.

### La respuesta formal existente genera un radicado saliente

El motor obtiene la plantilla con `util_default_respuesta = 1` mediante [Class_system_plantilla_radicado.vb](../../../radicador/Class_system_plantilla_radicado.vb), línea 75. `Solicita_tipo_tramite_respuesta`, en [Class_tipo_doc_entrante.vb](../../../radicador/Class_tipo_doc_entrante.vb), línea 485, busca un trámite elegible de esa plantilla y excluye los marcados internos. Si no coincide la descripción, selecciona el primer trámite devuelto.

Debe definirse si las solicitudes internas conservarán esa respuesta saliente o requieren otro tipo de respuesta interna. Conservar el motor actual exige plantilla y trámite de respuesta válidos, documento, firma/aprobación cuando aplique y canal de entrega. La selección por defecto no sustituye una homologación funcional revisada.

## Condiciones para habilitar un trámite

1. Resolver la plantilla interna autorizada y comprobar que el trámite esté activo y disponible para ella, sin retirar involuntariamente un trámite de otros catálogos.
2. Definir seguimiento y modalidad por separado: sin seguimiento, confirmación o respuesta formal. Resolver la inconsistencia de lectura y contrastar los consumidores compartidos.
3. Comprobar vencimiento, calendario y campo de plantilla, incluidos los casos de cero días y cambio de trámite.
4. Resolver un responsable interno válido; mantener distintas las identidades de ejecutor, remitente y propietario de la respuesta.
5. Verificar gabinete, flujo, actividad y configuración de módulo de respuesta; comprobar la aparición de la obligación en la bandeja del responsable y la trazabilidad hasta el radicado.
6. Para respuesta formal, verificar plantilla de respuesta y homologación de trámite, y confirmar la decisión sobre generar radicado saliente.
7. Probar respuesta, confirmación, reasignaciones, devolución, envío a grupo/usuario y cierre según sus contratos. No ampliar la regla de bloqueo de todas las transiciones como efecto secundario de la activación.
8. Validar fallos posteriores al registro y reintentos: la solicitud de respuesta se inserta antes del commit y el Workflow se asigna después; un fallo de asignación no equivale a inexistencia del radicado o de la obligación.

## Presentación recomendada en la interfaz aprobada

Conservar el selector **Trámite** como decisión del usuario. Según la configuración autorizada, mostrar una indicación de lectura como **Requiere respuesta formal** o **Solo confirmación**, junto con el vencimiento y quién debe responder. No añadir un checkbox que permita omitir una obligación del trámite.

El resumen de confirmación debe incluir esa modalidad. Tras registrar, el acceso a seguimiento debe utilizar el identificador confirmado del radicado/solicitud y los permisos del usuario.

La [maqueta HTML](maqueta.html) incorpora, a solicitud del usuario, ejemplos de las tres modalidades sobre el formulario compacto: Solicitud administrativa para respuesta formal, Revisión documental para confirmación y Comunicación informativa sin solicitud de respuesta. Son asociaciones ficticias para revisar la presentación; no activan ni modifican el catálogo real. La vista de seguimiento es de lectura y no elabora ni confirma respuestas.

La carga directa en exploración puede entregar documentos al mismo registro común. No debe saltarse la creación de la solicitud de respuesta ni dar por contestado el trámite solo porque el usuario haya subido el documento principal.

## Inventario de configuración pendiente

El catálogo efectivo reside en base de datos; la inspección del repositorio no confirma nombres ni cantidades de trámites activos en el ambiente. Para un inventario autorizado se puede ejecutar el siguiente `SELECT` parametrizado, sin modificar configuración:

```sql
SELECT t.id_Tipo_Doc_Entrante, t.Descripcion_Doc,
       t.system_plantilla_radicado_id_plantilla,
       t.estado_tipo_documento, t.tipo_activo_rad_interno, t.tipo_activo_pqr,
       t.requiere_respuesta, t.obliga_respuesta_radicado_tramite,
       t.numero_dias_vence, t.activo_modulo_respuesta,
       t.resp_correo_fisico_electronico,
       t.ID_WF_FLUJOS_TRABAJO, t.nombre_gabinete_workflow
FROM tipo_doc_entrante AS t
WHERE t.system_plantilla_radicado_id_plantilla = @PlantillaInternaId
ORDER BY t.Descripcion_Doc, t.id_Tipo_Doc_Entrante;
```

La decisión funcional pendiente principal es si la respuesta formal de este módulo debe generar el radicado saliente que utiliza el motor existente. También debe acordarse qué trámites se ofrecerán y qué recorridos deben exigir una respuesta previa a transferir o cerrar.

## Verificación y aceptación

Pasaron **23 comprobaciones existentes** con `node --test tests/workflow-user-send.test.cjs tests/workflow-group-send.test.cjs`. Cubren contratos, revalidación y políticas distintas de envío a usuario/grupo; no ejecutan una radicación interna con respuesta contra el ambiente ni prueban la corrección de la lectura de modalidad.

La aceptación de una implementación debe cubrir:

- Sin seguimiento; seguimiento con confirmación; y respuesta formal, tanto desde Producción como desde la futura carga directa.
- Igual y distinta combinación de `requiere_respuesta` y `obliga_respuesta_radicado_tramite`, sin confundir activación con modalidad.
- Registro de solicitud, fecha límite, propietario, bandeja y auditoría coherentes con el trámite elegido.
- Intentos de avanzar sin confirmar, respuesta incompleta, respuesta formal terminada, reversión y cambios de responsable.
- Transferencias a usuario/grupo, devoluciones y cierre; comprobación de la política de cada operación.
- Plantilla de respuesta ausente, homologación inexistente o ambigua, plazo cero y campo de vencimiento ausente.
- Remitente tercero, usuario representado y ejecutor distinto, conservando el responsable interno de la obligación.
- Fallo de Workflow después de commit, correo pendiente y respuesta de red perdida sin duplicar radicado ni solicitud de respuesta.

No se cambió código de aplicación ni configuración, no se ejecutó el `SELECT`, no se activaron trámites y no se realizó E2E autenticada. La conclusión es de viabilidad del código y de condiciones de integración; la variación HTML muestra únicamente datos y estados ficticios.
