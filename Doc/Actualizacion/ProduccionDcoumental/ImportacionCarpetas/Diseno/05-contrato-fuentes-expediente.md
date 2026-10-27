# Contrato de fuentes de expediente — DOC-110 / tarea 1.3

Estado: definición técnica para revisión, sin implementación ni aprobación automática del refinamiento. Desarrolla D-03/RQ-03. Los valores particulares los configura cada perfil; este contrato define qué significan y cómo se validan. Coincidencias se rige por el contrato aprobado 06 y la ejecución legacy por el prompt 08.

## Evidencia y límites del consumidor

| Fuente | Hecho observado | Consecuencia |
|---|---|---|
| `Gestion/ClassGaExpediente.vb:7279`, `Registrar_Expediente_Conservacion` | Recibe código, fechas extremas, nombre/identificación de responsable y empresa; devuelve ID de expediente. | No equiparar nombre visible, código e ID; resolver entradas antes de crear. |
| Mismo método, comprobación inicial | Rechaza fecha extrema inicial vacía. | Fecha inicial requerida para este recorrido, aunque una regla se marque opcional. |
| Mismo archivo:1698, 6018 y 7363 | Dos recorridos comprueban 45 caracteres; Conservación comprueba 70 con mensaje de 45. | No deducir límite de un mensaje ni de la columna aislada. |
| Mismo método, responsable | Admite cadenas vacías convirtiéndolas a NULL; limita identificación a 60 caracteres. | No declarar responsable obligatorio universal por inferencia. La configuración o clasificación puede exigirlo. |
| Mismo método, fechas | Fecha inicial participa en cálculos de retención; fecha de registro se obtiene aparte. | Fecha documental y auditoría no son intercambiables. |

Para el contrato inicial del código de expediente se adopta límite de compatibilidad de 45 caracteres. Es una restricción técnica conservadora propuesta del importador, no una afirmación de que Conservación solo admita 45. Ampliarlo exige caracterizar y probar el recorrido completo del adaptador 08 y actualizar el contrato; no modificar legacy ni truncar valores.

## Matriz de destinos

Los nombres de esta tabla son conceptos contractuales, no nuevas columnas ni claves SQL libres. `destino_ref` se resuelve contra un registro cerrado de campos. Las fuentes permanecen CONSTANTE, CARPETA, ARCHIVO y EXPEDIENTE según el modelo común.

| Destino | Fuentes admitidas en esta entrega | Tipo y obligatoriedad | Etapa / validación |
|---|---|---|---|
| Código administrativo del perfil | Entrada administrativa de cabecera, fuera de reglas de expediente | Texto ASCII no vacío, hasta 64 según modelo común; único por ámbito | Guardado: validar contrato de cabecera. No usarlo como código de cada expediente. |
| Código aportado del expediente | CONSTANTE o CARPETA con extracción explícita | Texto no vacío; hasta 45; conservar ceros iniciales | PREFLIGHT, antes de creación. Constante repetida no autoriza crear duplicados: el plan aplica la política versionada del contrato 06. |
| Fecha documental inicial | CONSTANTE o CARPETA con extracción y formato explícitos | Fecha de calendario válida, obligatoria para Conservación | PREFLIGHT. No convertir fecha de archivo o fecha del equipo en valor implícito. |
| Fecha documental final | CONSTANTE o CARPETA con extracción y formato explícitos | Fecha válida o ausencia explícita si la clasificación admite omisión | PREFLIGHT. Si existe, inicial ≤ final. Una fuente configurada que falla no equivale a omisión permitida. |
| Responsable del expediente | CONSTANTE como referencia de catálogo; CARPETA como clave de búsqueda de catálogo | Una referencia válida; obligatoria cuando perfil/clasificación lo requiera | PREFLIGHT. Exigir coincidencia única y autorización en ámbito; nombre visible no es identidad. |
| Código/ID real de expediente para índice documental | EXPEDIENTE, seleccionando propiedad permitida | Tipo de la propiedad real compatible con el índice destino | TRAS_EXPEDIENTE para expediente nuevo. Validar origen/tipo al publicar; resolver desde resultado confirmado del adaptador. Nunca inventar el ID. |
| Fecha/responsable heredados por un índice | EXPEDIENTE, seleccionando propiedad permitida | Tipo de fecha/referencia o representación declarada compatible con el índice | TRAS_EXPEDIENTE en el recorrido de expediente nuevo. No sustituye la entrada necesaria para crearlo. |

No se incorpora generación automática de código a través de una regla inventada ni se supone el significado de `estado_codigo_unico`. Si se requiere una estrategia de generación legacy, el adaptador 08 debe caracterizarla y registrarse como capacidad explícita antes de habilitar esa opción. Esta entrega sí permite publicar un perfil que aporte código por constante/carpeta, sujeto a validaciones.

ARCHIVO sigue permitido para destinos de documento, índices y tipología según sus reglas. No se admite como fuente de estos campos comunes del expediente en el contrato inicial: una carpeta puede contener varios archivos y no existe una reducción acordada. Rechazar esa combinación; no tomar el primero, mínimo, máximo ni último silenciosamente. Incorporar una reducción futura exige ampliar contrato y pruebas.

## Representación y validación determinista

- Código: tratar como texto. No convertir a número ni añadir sufijos. Aplicar solo normalizaciones declaradas, sin truncar. La política se define en contrato 06; el adaptador 08 caracteriza identidad/comparación real, sin depender del orden o cultura del navegador.
- Fecha constante: representación canónica `yyyy-MM-dd`, sin hora ni conversión UTC. Fecha extraída: formato exacto declarado, validación de calendario y conversión a esa representación. Admitir en este contrato `yyyy-MM-dd`, `yyyyMMdd`, `dd-MM-yyyy` y `dd_MM_yyyy`; rechazar formatos no registrados y años de dos dígitos. No inferir formato por cultura. Fechas de auditoría siguen siendo instantes UTC independientes.
- Responsable constante: guardar referencia de catálogo autorizado, no nombre libre. Responsable extraído: guardar clave de búsqueda y catálogo/campo de búsqueda permitidos, no SQL. Cero o varias coincidencias producen incidencia. Revalidar vigencia al preparar/ejecutar; no reemplazarlo por el usuario que carga.
- El adaptador 08 transforma referencia de responsable en nombre/identificación requeridos por legacy. La identificación debe respetar 60 caracteres observados y cualquier límite físico más restrictivo del consumidor; no truncar nombres para hacerlos pasar.
- Una fuente CONSTANTE tiene valor literal tipado o referencia. CARPETA declara atributo/extracción y conversión dentro del conjunto de operaciones del resolvedor; ninguna carpeta física se lee para publicar. EXPEDIENTE declara propiedad permitida y etapa.
- Campo opcional omitido: no existe asignación activa y el contrato del destino permite ausencia. Campo requerido no admite ese estado. Nunca reinterpretar NULL o cadena vacía como «hoy», «usuario actual» o un identificador predeterminado.
- Rechazar fuentes circulares: un campo de creación no puede depender del expediente que se está creando. No crear un expediente de prueba para publicar.

## Publicación, ensayo y ejecución

| Momento | Obligación |
|---|---|
| Guardar borrador | Permitir configuración incompleta identificada; rechazar estructura malformada, fuente/destino incompatible, duplicidad de asignación o referencias manipuladas. |
| Publicar | Comprobar fuentes de cada requerido, combinación/etapa/tipo, constantes, referencias actuales y formato de fecha. No exigir que una carpeta real o el ID futuro existan. |
| Ensayar | Usar metadatos sintéticos y catálogos autorizados. Devolver valor, incidencia o dependencia tipada, sin efectos documentales. Un ejemplo correcto no demuestra que todas las carpetas futuras sean válidas. |
| Preparar importación, entrega posterior | Resolver código/fechas/responsable por carpeta y verificar requeridos y catálogos vigentes; resolver coincidencias conforme al contrato 06 y caracterización del adaptador 08. |
| Crear e incorporar, entrega posterior | Consumir valores validados, revalidar permisos y obtener propiedades reales tras creación confirmada. Sin reescribir configuración publicada. |

## Criterios observables de prueba

1. Código `000123` conserva ceros; 46 caracteres genera incidencia sin truncamiento bajo el límite inicial.
2. Fecha `2024-02-29` válida y `2023-02-29` inválida; `03-04-2026` solo significa 3 de abril si está declarado `dd-MM-yyyy`.
3. Fecha final anterior a inicial rechaza; ausencia final permitida no se convierte en fecha actual.
4. Responsable extraído sin coincidencia o con dos coincidencias rechaza; referencia fuera del ámbito rechaza.
5. Fuente ARCHIVO para fecha inicial del expediente rechaza aunque el ejemplo tenga un solo archivo; la misma categoría puede ser válida para un índice documental.
6. Fuente EXPEDIENTE/ID para entrada de creación rechaza; para índice TRAS_EXPEDIENTE devuelve dependencia tipada en ensayo.
7. Publicar una regla válida de carpeta no exige una carpeta real. Una constante inválida impide publicación.

Estos casos son especificaciones de pruebas futuras, no corridas aprobadas. No se cambiaron SII, funciones compartidas, datos o SQL.
