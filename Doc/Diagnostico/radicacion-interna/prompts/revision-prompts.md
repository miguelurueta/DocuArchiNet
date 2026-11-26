# Revisión de los seis prompts de radicación interna

La serie vigente tiene **6 prompts P01–P06**, consolidados el 2026-11-26. Conserva las 12 subfases RI00–RI11 y sus tareas, criterios y pruebas específicas. Los 12 archivos anteriores se sustituyen por esta serie; no se ejecutan como otra lista de prompts. Consultar [guía](00-guia-ejecucion.md), [contexto común](contexto-comun.md) y [mapa M/N](../mapa-funciones-implementacion.md).

Se conserva D-RI-03: este módulo radica y crea/entrega el seguimiento; elaborar, enviar, confirmar y cerrar respuestas corresponde al módulo existente. Los acuerdos pendientes se refinan al abrir la subfase que los consume. Redactar/consolidar prompts no implementa aplicación ni acredita pruebas del ambiente.

## Revisión estructural

Motor: `testFrontendPromptReview` de `tools/opsxj/scripts/lib/frontendPromptReviewService.js`. Perfil: `legacy-webforms-vb`. Se revisaron los seis textos completos; no se modificó el motor ni el reporte compartido `.opsxj/reports/prompt-review-report.json`. Esta llamada directa al motor no equivale a ejecutar la suite de otro repositorio ni a una prueba funcional del módulo.

| Prompt | BLOCKER | MAJOR | MINOR | INFO |
|---|---:|---:|---:|---:|
| [P01](01-p01-refinamiento-contratos.md) | 0 | 0 | 0 | 1 |
| [P02](02-p02-base-webforms-contexto-configuracion.md) | 0 | 0 | 0 | 1 |
| [P03](03-p03-remitentes-internos-terceros.md) | 0 | 0 | 0 | 1 |
| [P04](04-p04-carga-incorporacion-documentos.md) | 0 | 0 | 0 | 1 |
| [P05](05-p05-registro-recuperacion-seguimiento.md) | 0 | 0 | 0 | 1 |
| [P06](06-p06-interfaz-integracion-aceptacion.md) | 0 | 0 | 0 | 1 |

Cada INFO es `MANUAL_REVIEW_RECOMMENDED`: el motor solicita revisión semántica adicional. Se revisaron la agrupación, las dependencias y los límites; siguen siendo necesarias las decisiones y pruebas de cada ejecución futura.

## Cobertura conservada al consolidar

| Prompt vigente | Subfases de origen | Orden interno |
|---|---|---|
| P01 | RI00 | RI00 |
| P02 | RI01, RI02, RI03 | RI01 → RI02 → RI03 |
| P03 | RI04 | RI04 |
| P04 | RI05, RI06 | RI05 → RI06 |
| P05 | RI07, RI08, RI09 | RI07 → RI08 → RI09 |
| P06 | RI10, RI11 | RI10 → RI11 |

La comparación realizada al consolidar conservó 73 tareas identificadas, 60 apariciones de criterios de aceptación (incluyen requisitos comunes repetidos) y 12 bloques de pruebas específicas. Se conservaron las referencias a funciones de cada subfase. Las restricciones, rutas y pruebas generales se comparten una vez por prompt para evitar repetición.

Actualización por D-RI-05, confirmada el 2026-11-26 y que sustituye D-RI-04: P04 define un control nuevo e independiente (`RadicacionInternaUpload`, nombre propuesto) y recepción temporal propia; P06 lo integra en la UI aprobada. Se precisan RI05.1, RI05.4, RI10.4 y sus pruebas de estado, ciclo de vida y ausencia de dependencia del cargador compartido. Se mantienen los 73 identificadores de tarea, 61 apariciones de criterios y 12 bloques de pruebas específicas. No se aprueban límites, expediente o permisos por elegir el control. La revisión estructural se repite sobre los seis textos actuales y se actualizan sus huellas. Se trata de una actualización de los prompts; el control aún no está implementado.

Actualización por D-RI-06, instruida el 2026-11-26: los seis prompts incorporan la protección del núcleo crítico. P01 prepara la caracterización y la justificación por cambio; P02 conserva contratos del ASMX compartido; P05 exige caracterización ejecutada antes de modificar, adaptación/extracción mínima y comparación posterior; P06 condiciona la aceptación a evidencia de esos invariantes y consumidores. Se precisan las tareas existentes y se agregan tres criterios: la serie actual conserva 73 tareas identificadas, 64 apariciones de criterios y 12 bloques de pruebas específicas. No se modificó la aplicación ni se ejecutaron pruebas del núcleo al redactar esta actualización; la revisión estructural se repite sobre los textos actuales.

P02 mantiene RI01 → RI02 → RI03 para no adelantar configuración que depende de contexto. P03/P04 dependen de P02 y pueden avanzar independientemente. P05 mantiene RI07 → RI08 → RI09; P06 mantiene RI10 → RI11. El grafo de seis paquetes y el de doce subfases no tienen ciclos. Las decisiones se refinan por subfase; agruparlas no exige resolver todas al iniciar un prompt.

## Revisión arquitectónica y funcional documental

| Riesgo o dependencia verificada | Resolución exigida por la serie |
|---|---|
| Apertura y generador dependen de documento/expediente de Producción y sesión mutable. | RI00 define alternativa documental; RI02 crea operación propia; RI03 desacopla metadatos; RI06 entrega referencias válidas. |
| El cargador existente comparte estado/eventos y sus adjuntos clásicos/Simplificados exigen radicado existente. | D-RI-05 crea un control independiente, con estado/endpoint propios, sin adaptar el cargador compartido; RI05 implementa la recepción temporal y RI06 conserva la compatibilidad del almacenamiento. |
| Se mezclan remitente, ejecutor y responsable si solo se cambia el combo. | RI04 resuelve identidad tipada y autoridad; RI07 registra actor/propietario; RI09 mantiene propietario de respuesta. |
| El lector de modalidad consulta `requiere_respuesta`. | RI03 separa activación, modalidad y canal, caracteriza consumidores y verifica corrección compartida. |
| Controles dinámicos, tres opciones de plantilla y dependencias se reconstruyen durante postback. | RI01 estabiliza; RI03/RI10 conservan metadatos/IDs/tipos y exigen prueba WebForms real. |
| Consecutivos y registro tienen una transacción; almacenamiento/Workflow/correo tienen efectos posteriores. | D-RI-06 exige caracterización previa y comparación posterior; RI07 justifica la extracción mínima y conserva `FOR UPDATE`/asociación durable; RI08 reconcilia fases sin repetir el núcleo. |
| Un timeout puede ocultar un efecto confirmado. | RI05/RI06/RI07/RI08 exigen evidencia durable, consulta y conflictos por revisión, sin repetir escrituras inciertas. |
| El módulo existente de respuestas tiene plantilla/homologación y políticas propias. | D-RI-03 limita RI09 a entrega/lectura del seguimiento; se comprueba compatibilidad, sin implementar respuesta ni cambiar esas políticas. |
| El markup contiene capacidades condicionadas y ramas que no prueban conectividad por sí mismas. | M30/M31 y RI00/RI10 exigen inventariar auxiliar, diagrama, copia, relaciones, expediente, catálogos y limpieza según su recorrido real. |
| Una pantalla moderna puede quedar aislada del motor o del artefacto publicado. | RI10 integra ambos accesos y estados reales; RI11 comprueba proyecto, esquema/configuración, publicación y aceptación completa. |

Se distinguen componentes nuevos propuestos de funciones existentes. El mapa organiza M01–M31 y N01–N13, y los paquetes siguen un orden sin dependencias circulares. RI07 crea la obligación; RI09 comprueba su entrega al módulo existente de respuestas. La separación evita duplicar el seguimiento o incorporar elaboración, envío y cierre de respuestas a este módulo. M24/M26 quedan como consumidores de compatibilidad, no funciones a modificar.

## Trazabilidad de requisitos por subfase

| Requisito | Subfases principales | Verificación final |
|---|---|---|
| RQ-RI-01, procedencia Producción | RI02, RI06, RI10 | Selección y relaciones reales conservadas. |
| RQ-RI-02, carga directa | RI02, RI05, RI06, RI10 | Sesión nueva, recepción y documentos definitivos. |
| RQ-RI-03, identidades | RI04, RI07, RI09 | Remitente, ejecutor y propietario correctos. |
| RQ-RI-04, plantillas | RI01, RI03, RI10 | Campos/valores/configuraciones y postbacks reales. |
| RQ-RI-05, destino | RI01, RI03, RI10 | Ruta/actividad/grupo/usuario revalidados. |
| RQ-RI-06, seguimiento | RI03, RI07, RI09 | Obligación, modalidad, plazo, propietario y lectura por el consumidor existente. |
| RQ-RI-07, repetición segura | RI02, RI05–RI08 | Sin duplicar ante concurrencia o respuesta perdida. |
| RQ-RI-08, resultado parcial | RI07–RI10 | Registro confirmado distinguido de fases pendientes. |
| RQ-RI-09, autoridad | RI02 y transportes de RI03–RI10 | Contexto/recurso en cada petición, consulta y recuperación. |
| RQ-RI-10, experiencia | RI01, RI05, RI10 | Recepción real, accesibilidad, validaciones y cierre. |
| RQ-RI-11, consumidores | RI01, RI03, RI04, RI06, RI08–RI10 | Producción, clásica/Simplificada y Workflow conservados. |
| RQ-RI-12, operación | RI00, RI11 y evidencia de cada paquete | Esquema, build, publicación, configuración y recorrido autorizados. |

RI11 consolida todos los requisitos. Esta tabla describe las comprobaciones exigidas a la implementación, no resultados de pruebas de aplicación ejecutadas durante la redacción.

## Reproducción de la revisión estructural

Desde la raíz del repositorio con Node y el motor existente:

```powershell
@'
import fs from 'node:fs';
import path from 'node:path';
import { testFrontendPromptReview } from './tools/opsxj/scripts/lib/frontendPromptReviewService.js';
const dir = 'Doc/Diagnostico/radicacion-interna/prompts';
const files = fs.readdirSync(dir).filter(f => /^\d{2}-p\d{2}.*\.md$/.test(f)).sort();
let errors = 0;
for (const file of files) {
  const findings = testFrontendPromptReview({
    promptText: fs.readFileSync(path.join(dir, file), 'utf8'),
    technologyProfile: 'legacy-webforms-vb'
  });
  errors += findings.filter(f => ['BLOCKER','MAJOR','MINOR'].includes(f.severity)).length;
  console.log(JSON.stringify({ file, findings }));
}
if (files.length !== 6 || errors > 0) process.exitCode = 1;
'@ | node --input-type=module
```

La evidencia local de revisión, comparación de cobertura y validación de vínculos se conserva bajo `.tmp/diagnostico-radicacion-interna-20261125/`, excluido de Git. La validación comprueba enlaces, inventario M/N, 53 declaraciones existentes y suites candidatas; localizar una declaración no prueba su comportamiento. No se cambiaron aplicación, esquema ni configuración; no se ejecutó MSBuild, carga real, radicación, correo ni E2E del ambiente al consolidar estos documentos.

## Huellas de los prompts revisados

SHA-256 del texto UTF-8 con saltos LF. Repetir revisión y actualizar huella si cambia un prompt.

| Prompt | SHA-256 |
|---|---|
| P01 | `6027b208c3543a24f8aab3ec0dafc51fd3662f8aa81f77270a927bfba006f46e` |
| P02 | `c55200280c4a3b08f283e31e7ab61332c5108809f2b4986b8eb0b00f3f03750f` |
| P03 | `4151150e7751f70cef66224865090c19ba0cc81cb9cdc4b12b302bfe3dae1b11` |
| P04 | `6bbb71ab6966954f41af0c040c590eebafdeac3e741066a0ebac73bc648a19f9` |
| P05 | `fc52ae7c3ebecd270d2b3db741f891704388f8ced6ecc2d3aa58b0d7bacee978` |
| P06 | `7bee3c13074a55a5b79b5e5bed0d82779ea94b1e2525987a771d5b630f3d50f1` |
