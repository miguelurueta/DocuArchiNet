# Exploración de la fecha inicial por importación

Revisión del 26 de noviembre de 2026. Alcance: permitir elegir una fecha inicial documental al iniciar un lote de importación por carpetas, sin cambiar el almacenamiento. El [prompt 14](../14-prompt-fecha-inicial-por-importacion.md) reúne el encargo de implementación y sus criterios de aceptación. Esta exploración no acredita la implementación de la capacidad.

## Necesidad y decisiones de alcance

El nombre de carpeta `ABRIL 2025` no contiene un día y no permite obtener una fecha inicial completa sin una decisión documental adicional. El usuario solicitó una fecha constante elegida al crear la importación y manifestó preocupación por la regresión sobre el proceso existente.

| Aspecto | Decisión o condición |
|---|---|
| Valor elegido por el usuario | Una fecha fija por importación, aplicable a sus expedientes nuevos. |
| Configuración del perfil | Habilita la fuente de la fecha; no contiene el valor particular del lote. |
| Constantes existentes | Conservan su significado como literal del perfil. |
| Pausar o reanudar | El usuario no requiere edición de la fecha durante la pausa. Se recomienda conservar el valor confirmado para reducir estados y evitar mezclas. |
| Expedientes existentes | No modificar sus fechas por esta mejora. |
| Almacenamiento | Funciones actuales intocables; mantener D-RI-07 y su verificador. |
| Estado del trabajo | Exploración y prompt creados; capacidad pendiente de implementación y validación. |

## Hallazgos del código actual

Los enlaces señalan archivos existentes; los nombres de funciones delimitan los puntos revisados.

- [ResolvedorReglasPerfilImportacion.vb](../../../../../Services/ProduccionDocumental/ImportacionCarpetas/ResolvedorReglasPerfilImportacion.vb): las fuentes admitidas son `CONSTANTE`, `CARPETA`, `ARCHIVO`, `EXPEDIENTE` y `PROVEEDOR`. No existe una fuente de parámetros de la importación. Para campos del expediente se rechazan `ARCHIVO` y `EXPEDIENTE`; la configuración de proveedor tampoco permite este destino. `FECHA_INICIAL` es requerida aunque `Obligatoria` sea falso y se valida su relación con `FECHA_FINAL`.
- [ValidadorPerfilImportacion.vb](../../../../../Services/ProduccionDocumental/ImportacionCarpetas/ValidadorPerfilImportacion.vb): publicar requiere reglas de código, nombre, fecha inicial y tipología. La fecha particular del lote no debe convertirse en un requisito para publicar una regla estructuralmente válida del nuevo modo.
- [ServicioPreparacionImportacion.vb](../../../../../Services/ProduccionDocumental/ImportacionCarpetas/ServicioPreparacionImportacion.vb): `CalcularPlan` y `CalcularRaicesSegmentadas` proyectan las raíces recibidas; `ResolverRaiz` construye un ejemplo con nombre de carpeta y clasificación. Es necesario transportar la nueva fecha por estos puntos.
- [ServicioPreparacionSegmentadaImportacion.vb](../../../../../Services/ProduccionDocumental/ImportacionCarpetas/ServicioPreparacionSegmentadaImportacion.vb): `Registrar` persiste las partes de raíces mediante una proyección explícita. `Validar` reconstruye otra solicitud y puede trabajar con un subconjunto de raíces. Un parámetro añadido únicamente a la pantalla o al inicio de preparación se perdería en ese recorrido.
- [MySqlImportacionCarpetasRepository.Preparacion.vb](../../../../../Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Preparacion.vb): `InventarioDePartes` recupera las raíces desde su JSON. La cabecera conserva contexto y snapshot; no hay un parámetro de fecha del lote. Las partes existentes ofrecen un lugar persistible para el dato opcional sin modificar tablas. El SQL y el guard del repositorio de perfiles definen `fuente_tipo` como `VARCHAR(40)`, compatible con el nombre propuesto; esto es evidencia del código y no una inspección del esquema desplegado.
- [PerfilImportacionJson.vb](../../../../../Services/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionJson.vb) y [PlanSegmentadoImportacionJson.vb](../../../../../Services/ProduccionDocumental/ImportacionCarpetas/PlanSegmentadoImportacionJson.vb): el JSON tiene forma cerrada y participa en las huellas. Una nueva propiedad serializada como `null` puede alterar la representación de un documento histórico y su huella; debe omitirse cuando no aplica. El snapshot representa la publicación y no debe contener parámetros variables de ejecución.
- [ComandoCreacionExpedienteImportacion.vb](../../../../../Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComandoCreacionExpedienteImportacion.vb): la creación obtiene `FechaInicial` de `campos("FECHA_INICIAL").Valor`, procedente de `ValoresPlan`. Este recorrido permite mantener el escritor existente. Los índices que ya heredan la fecha pueden reflejar el valor elegido mediante sus reglas actuales.

## Diseño propuesto para la implementación

Añadir la fuente `IMPORTACION`, restringida a la fecha inicial y resuelta en preflight. El calendario único de la ventana llena una propiedad opcional de cada raíz del manifiesto. Las proyecciones conservan el dato en partes de preparación y planes; el resolvedor materializa el valor en `Resultado.Elementos` de ámbito `RAIZ` y el adaptador de creación consume los valores del plan como lo hace actualmente.

La repetición del parámetro por raíz permite utilizar los documentos JSON persistidos existentes, pero exige una comprobación global de uniformidad. En preparación segmentada se deben comparar todas las raíces, también las de otras partes, antes de validar y sellar. El valor no se recupera de la primera raíz ignorando discrepancias. Si aún no se han persistido raíces, recuperar el identificador de preparación no recupera una fecha que nunca se guardó.

Los nombres nuevos y la normalización de controles se fijan en el prompt. El implementador puede ajustar detalles internos por compatibilidad demostrada y debe documentar el resultado. Cambiar la fuente para otros destinos, modificar expedientes existentes o alterar el almacenamiento excede el alcance.

## Riesgos y comprobaciones

| Riesgo | Control exigido |
|---|---|
| Nuevo campo obligatorio bloquea perfiles anteriores. | Modo explícito por regla; propiedad ausente admitida para documentos anteriores; FI-01 y FI-02. |
| Fecha guardada en perfil altera ejecuciones simultáneas. | Parámetro en raíces/plan de cada ejecución; FI-04. |
| Fecha desaparece o difiere entre partes. | Proyecciones completas y validación global sobre todas las raíces; FI-06 a FI-08. |
| Nueva propiedad cambia huellas anteriores. | Omisión localizada de valores nulos y expectativas previas a los cambios; FI-02. |
| Respuesta tardía mezcla fecha nueva con una revisión anterior. | Invalidación y nueva clave de preparación; FI-09. |
| Reintento recalcula o sustituye la fecha. | Consumir el valor persistido y conservar barreras de idempotencia; FI-08 y FI-11. |
| Calendario cambia el día por zona horaria. | Fecha civil canónica sin conversión UTC; FI-05. |
| Retorno a código antiguo impide recuperar planes nuevos. | Publicación coordinada y conservación del lector compatible hasta cerrar operaciones nuevas. |
| Presentación «Opcional» induce una configuración inviable. | Representar obligatoriedad efectiva sin reescribir reglas históricas; FI-12. |

## Evidencia y límites de la revisión

El ensayo local del resolvedor de producción cubrió cinco casos: `ABRIL 2025` con formato `yyyy-MM-dd` produce `FECHA_INVALIDA`; una constante canónica explícita es válida; una fecha inicial vacía sigue siendo requerida aunque `Obligatoria=false`; `ARCHIVO` y `PROVEEDOR` se rechazan para ese destino con los controles actuales. El ensayo no utilizó base de datos, HTTP ni almacenamiento y no demuestra el comportamiento del nuevo modo, que todavía no existe.

La comprobación `node tools/diagnostico/verify-radicacion-storage-freeze.cjs` confirmó la congelación de 10 archivos y 24 funciones protegidas en la revisión previa a crear el prompt. Los datos de ejecución locales permanecen en `.tmp`, excluidos de Git. No se ejecutaron cargas ni escrituras documentales reales para esta exploración.

La implementación debe aportar las verificaciones FI-01–FI-14 del prompt. No hay una duda funcional pendiente que impida comenzar el trabajo; quedan por demostrar la compatibilidad de contratos, huellas, preparación persistida y recuperación sobre el código implementado.

## Revisión del prompt

Se aplicó el evaluador local `testFrontendPromptReview` de `tools/opsxj/scripts/lib/frontendPromptReviewService.js`, el mismo utilizado por `opsxj:prompt-review`, con perfil `legacy-webforms-vb`, sin correcciones automáticas ni acceso remoto. Tras ajustar encabezados, rutas y requisitos de verificación, el resultado fue cero hallazgos bloqueantes o mayores y un aviso informativo de revisión semántica. La lectura de arquitectura comprobó correspondencia entre fuente, transporte por raíces, validación global, huellas y recuperación; no certifica el código futuro.

La revisión de enlaces locales de los dos documentos nuevos, el índice y la guía comprobó 45 destinos sin enlaces rotos. `git diff --check` no encontró errores de espacios. Esta entrega modifica documentación y no ejecuta la implementación descrita en el prompt.
