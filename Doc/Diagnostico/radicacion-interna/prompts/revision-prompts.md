# Revisión de los prompts de radicación interna

La serie de 12 prompts cubre el alcance explorado: Producción y carga directa, remitente interno/tercero, plantillas dinámicas, registro, seguimiento, recuperación e interfaz. Está preparada para refinar y ejecutar por paquetes. Las decisiones G01–G11 conservan su estado pendiente hasta registrarse el acuerdo correspondiente; la revisión no las aprueba ni acredita una implementación.

Fecha: 2026-11-25. Base de aplicación revisada: `226d2000`. Perfil: `legacy-webforms-vb`. Consultar [guía](00-guia-ejecucion.md), [contexto](contexto-comun.md) y [mapa M/N](../mapa-funciones-implementacion.md).

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
| La respuesta formal usa plantilla y homologación saliente; las transiciones de Workflow tienen contratos distintos. | RI09 refina G09–G11 y verifica cada modalidad/transición; no aplica bloqueo universal. |
| El markup contiene capacidades condicionadas y ramas que no prueban conectividad por sí mismas. | M30/M31 y RI00/RI10 exigen inventariar auxiliar, diagrama, copia, relaciones, expediente, catálogos y limpieza según su recorrido real. |
| Una pantalla moderna puede quedar aislada del motor o del artefacto publicado. | RI10 integra ambos accesos y estados reales; RI11 comprueba proyecto, esquema/configuración, publicación y aceptación completa. |

Se distinguen componentes nuevos propuestos de funciones existentes. El mapa organiza M01–M31 y N01–N13, y los paquetes siguen un orden sin dependencias circulares. RI07 crea la obligación; RI09 la gestiona. La separación evita asignar dos inserciones de seguimiento a paquetes distintos.

## Trazabilidad de requisitos

| Requisito | Paquetes principales | Verificación final |
|---|---|---|
| RQ-RI-01, procedencia Producción | RI02, RI06, RI10 | Selección y relaciones reales conservadas. |
| RQ-RI-02, carga directa | RI02, RI05, RI06, RI10 | Sesión nueva, recepción y documentos definitivos. |
| RQ-RI-03, identidades | RI04, RI07, RI09 | Remitente, ejecutor y propietario correctos. |
| RQ-RI-04, plantillas | RI01, RI03, RI10 | Campos/valores/configuraciones y postbacks reales. |
| RQ-RI-05, destino | RI01, RI03, RI10 | Ruta/actividad/grupo/usuario revalidados. |
| RQ-RI-06, respuesta | RI03, RI07, RI09 | Obligación, modalidad, plazo, propietario y gestión. |
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
| RI00 | `906704de77c84bb1d2c9f65a5c9c552d32a6776f63c975380cc0f22df93fae21` |
| RI01 | `755610726bf0cc3ec235a46f7fc5cc6f0cbbcf0335be4bc5300a7ed08371ebd3` |
| RI02 | `6e515b698177d4df167a98b69c202507543a452af78c4e08b1fedd65a31cc9eb` |
| RI03 | `7cf256fa8c124234a3a0d67744af73a0762f48f1663806cadb180589d73fdb1a` |
| RI04 | `a0ad213be3568c55dea774ec6d0a05c2991ea885021a34e0c56b2c2853c38c05` |
| RI05 | `3ea458386f46696ac1863374163cbf99279e3ce10ae4074b1405c51d6ec005ed` |
| RI06 | `7a3b54cdd1d4353ed1f6ff24c3fea25ec47dc05e5cac003549eaec5d56e84ca5` |
| RI07 | `180f68e2fdbbc32202144b320319d8741332d2010c95ddd98b85fa69e73b1c0f` |
| RI08 | `89015e9b2feacb652aea14db251c41128dfefdf21d59958a68f7600a34da567b` |
| RI09 | `17ecfda51c7a2d76e8d63d4901b2878f6f36ecd04dba5c16e388b792baf52fda` |
| RI10 | `62905dcead8cf640bd5c11c71de2220b22cb93f7991ca87faa771e629e2f24d8` |
| RI11 | `f9f586d3d736393f3243cea525ed61e6499aa1a07417d6b1e9037af4a15c9236` |
