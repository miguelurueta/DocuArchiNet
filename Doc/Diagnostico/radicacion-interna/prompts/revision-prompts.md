# Revisión de los prompts de radicación interna

La serie de 12 prompts cubre el alcance explorado: Producción y carga directa, remitente interno/tercero, plantillas dinámicas, registro, seguimiento, recuperación e interfaz. Está preparada para refinar y ejecutar por paquetes. Las decisiones G01–G11 conservan su estado pendiente hasta registrarse el acuerdo correspondiente; la revisión no las aprueba ni acredita una implementación.

Fecha inicial: 2026-11-25. Alcance y revisión actualizados el 2026-11-26 por D-RI-03. Base de aplicación revisada: `226d2000`. Perfil: `legacy-webforms-vb`. Consultar [guía](00-guia-ejecucion.md), [contexto](contexto-comun.md) y [mapa M/N](../mapa-funciones-implementacion.md).

## Revisión estructural

Se evaluó el contenido de cada prompt con `testFrontendPromptReview` de `tools/opsxj/scripts/lib/frontendPromptReviewService.js`, utilizando el perfil legacy WebForms. Es una llamada directa al motor local; no equivale a ejecutar la suite de pruebas del comando de otro repositorio ni a una prueba funcional del módulo. No se modificó el motor ni se sobrescribió el reporte compartido `.opsxj/reports/prompt-review-report.json`.

| Prompt | BLOCKER | MAJOR | MINOR | INFO |
|---|---:|---:|---:|---:|
| [RI00](01-ri00-refinamiento-contratos.md) | 0 | 0 | 0 | 1 |
| [RI01](02-ri01-estabilidad-webforms.md) | 0 | 0 | 0 | 1 |
| [RI02](03-ri02-operacion-contexto-autoridad.md) | 0 | 0 | 0 | 1 |
| [RI03](04-ri03-plantillas-tramites-respuesta.md) | 0 | 0 | 0 | 1 |
| [RI04](05-ri04-remitentes-internos-terceros.md) | 0 | 0 | 0 | 1 |
| [RI05](06-ri05-recepcion-temporal-archivos.md) | 0 | 0 | 0 | 1 |
| [RI06](07-ri06-incorporacion-documental.md) | 0 | 0 | 0 | 1 |
| [RI07](08-ri07-registro-transaccional-idempotente.md) | 0 | 0 | 0 | 1 |
| [RI08](09-ri08-finalizacion-recuperacion-salidas.md) | 0 | 0 | 0 | 1 |
| [RI09](10-ri09-seguimiento-respuesta-workflow.md) | 0 | 0 | 0 | 1 |
| [RI10](11-ri10-interfaz-accesos-integracion.md) | 0 | 0 | 0 | 1 |
| [RI11](12-ri11-aceptacion-operativa-publicacion.md) | 0 | 0 | 0 | 1 |

El hallazgo informativo de todos los prompts es `MANUAL_REVIEW_RECOMMENDED`: el motor pide revisión semántica adicional. Se revisaron responsabilidades y secuencia frente al código según la tabla siguiente. Al implementar, corresponde contrastarlas de nuevo con el estado vigente y las decisiones del paquete.

## Revisión arquitectónica y funcional documental

| Riesgo o dependencia verificada | Resolución exigida por la serie |
|---|---|
| Apertura y generador dependen de documento/expediente de Producción y sesión mutable. | RI00 define alternativa documental; RI02 crea operación propia; RI03 desacopla metadatos; RI06 entrega referencias válidas. |
| Los handlers clásicos/Simplificados adjuntan a un radicado existente. | RI05 incorpora transporte previo independiente y conserva sus contratos; no relaja IDs obligatorios. |
| Se mezclan remitente, ejecutor y responsable si solo se cambia el combo. | RI04 resuelve identidad tipada y autoridad; RI07 registra actor/propietario; RI09 mantiene propietario de respuesta. |
| El lector de modalidad consulta `requiere_respuesta`. | RI03 separa activación, modalidad y canal, caracteriza consumidores y verifica corrección compartida. |
| Controles dinámicos, tres opciones de plantilla y dependencias se reconstruyen durante postback. | RI01 estabiliza; RI03/RI10 conservan metadatos/IDs/tipos y exigen prueba WebForms real. |
| Consecutivos y registro tienen una transacción; almacenamiento/Workflow/correo tienen efectos posteriores. | RI07 conserva `FOR UPDATE` y asocia operación/radicado; RI08 registra/reconcilia fases sin repetir el núcleo. |
| Un timeout puede ocultar un efecto confirmado. | RI05/RI06/RI07/RI08 exigen evidencia durable, consulta y conflictos por revisión, sin repetir escrituras inciertas. |
| El módulo existente de respuestas tiene plantilla/homologación y políticas propias. | D-RI-03 limita RI09 a entrega/lectura del seguimiento; se comprueba compatibilidad, sin implementar respuesta ni cambiar esas políticas. |
| El markup contiene capacidades condicionadas y ramas que no prueban conectividad por sí mismas. | M30/M31 y RI00/RI10 exigen inventariar auxiliar, diagrama, copia, relaciones, expediente, catálogos y limpieza según su recorrido real. |
| Una pantalla moderna puede quedar aislada del motor o del artefacto publicado. | RI10 integra ambos accesos y estados reales; RI11 comprueba proyecto, esquema/configuración, publicación y aceptación completa. |

Se distinguen componentes nuevos propuestos de funciones existentes. El mapa organiza M01–M31 y N01–N13, y los paquetes siguen un orden sin dependencias circulares. RI07 crea la obligación; RI09 comprueba su entrega al módulo existente de respuestas. La separación evita duplicar el seguimiento o incorporar elaboración, envío y cierre de respuestas a este módulo. M24/M26 quedan como consumidores de compatibilidad, no funciones a modificar.

## Trazabilidad de requisitos

| Requisito | Paquetes principales | Verificación final |
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

Desde la raíz del repositorio, usando Node y el motor existente:

```powershell
@'
import fs from 'node:fs';
import path from 'node:path';
import { testFrontendPromptReview } from './tools/opsxj/scripts/lib/frontendPromptReviewService.js';
const dir = 'Doc/Diagnostico/radicacion-interna/prompts';
const files = fs.readdirSync(dir).filter(f => /^\d{2}-ri\d{2}.*\.md$/.test(f)).sort();
let errors = 0;
for (const file of files) {
  const findings = testFrontendPromptReview({
    promptText: fs.readFileSync(path.join(dir, file), 'utf8'),
    technologyProfile: 'legacy-webforms-vb'
  });
  const relevant = findings.filter(f => ['BLOCKER','MAJOR','MINOR'].includes(f.severity));
  errors += relevant.length;
  console.log(JSON.stringify({ file, findings }));
}
if (files.length !== 12 || errors > 0) process.exitCode = 1;
'@ | node --input-type=module
```

La ejecución usada para esta entrega conservó JSON de revisión y validación bajo `.tmp/diagnostico-radicacion-interna-20261125/`, excluido de Git. La validación documental pasó sobre 17 documentos, 96 enlaces locales, 53 declaraciones de funciones existentes, el inventario M/N, las suites candidatas y el orden de dependencias. Comprobar declaraciones localiza el código; no prueba su comportamiento. No se cambiaron archivos de aplicación, esquema ni configuración; no se ejecutó MSBuild, E2E/carga real, radicación ni pruebas transaccionales del ambiente al redactar estos documentos.

## Huellas de los prompts revisados

SHA-256 del texto UTF-8 con saltos LF. Si cambia un prompt, repetir la revisión y actualizar su huella y resultado.

| Prompt | SHA-256 |
|---|---|
| RI00 | `9ab8ff42884bda7603ab7a07c7d71d006ebc6aadb9f26d57311581b1e06ab299` |
| RI01 | `84461fc411d98178006ee729e4e0120c76a915e3734c1bd2bd56fd4651fa5c96` |
| RI02 | `e9b6f2b5fe916cc5439c9e5606d96a632eb99197bde90a814a89501137e02106` |
| RI03 | `9ab8018da97a79c5c4d2f53bb3f554b5a79f6d0752a5e51d90dec00ee6efeb4e` |
| RI04 | `8898cc733293ce7472c674e8d24e36ba4d1bfaf3335513ac7218d8c43c5ec156` |
| RI05 | `48f34ca6cde20975e6ef62959679a9927f587d76a55349f320e6e281dae7e643` |
| RI06 | `b2f7fdc6398612931380f0f17a2cb5cf44f9a6d96502bd921e77d15d11c37dce` |
| RI07 | `fc4e6d93151763b0d6cce2b39e5bc9916f3e3d5655b4a5a6c8cf9f7740d3da3d` |
| RI08 | `c84787a74418a655a1b2bd7cffd2250c839c2ef86242a9b6e52eabe39867fbcd` |
| RI09 | `c341a757b38db6c1af9eda49e80a66a6cba52bc542abc4d769690773c6872732` |
| RI10 | `2ae9e59a439dd99936b85be948139a74a6a6f6b457e6e75948be7a4f2593c659` |
| RI11 | `951b38e5ce8c62f589889f99a884af2851a599514bb481e36dd11d6b325145c8` |
