# Exploración del tipo documental opcional

Revisión del 26 de noviembre de 2026. El usuario solicita poder importar documentos sin que el tipo documental sea obligatorio. El [prompt 15](../15-prompt-tipo-documental-opcional.md) propone una política por perfil que mantiene el comportamiento requerido para perfiles anteriores. Esta capacidad está pendiente de implementación y validación.

## Hallazgos y origen del bloqueo

| Punto revisado | Exigencia actual |
|---|---|
| `ValidadorPerfilImportacion.Validar` | La publicación requiere una regla `TIPOLOGIA`; quitarla permite guardar borrador, pero no publicar. |
| `ResolvedorReglasPerfilImportacion.Convertir` | Una tipología vacía sigue produciendo `VALOR_VACIO` aunque `Obligatoria=false`. |
| `PerfilImportacionCatalogAdapter.ValidarValoresNucleo` | Cada tipología resuelta exige una coincidencia de catálogo y se convierte a referencia interna. |
| `CoordinadorImportacionCarpetas.ValidarCorrespondenciaValores` | Exige correspondencia con reglas y referencia válida para la tipología resuelta. |
| `ComandoAlmacenamientoCarpetas.Componer` y `TipologiaId` | Exigen exactamente una tipología; usan `Single` y validan ID positivo. |
| `PrepararDatosAlmacenamiento` y `ValidarDatos` | Exigen una tipología de catálogo autorizada y un nombre no vacío. |
| `PerfilImportacionCatalogAdapter.LeerAlmacenamiento` | La comprobación posterior espera el identificador del comando; la ausencia debe incorporarse explícitamente al verificador y recuperación. |

Las rutas principales son [servicios](../../../../../Services/ProduccionDocumental/ImportacionCarpetas/), [comando de almacenamiento](../../../../../Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComandoAlmacenamientoCarpetas.vb), [adaptador de catálogo documental](../../../../../Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.Documento.vb) y [punto de llamada legacy](../../../../../Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.Legacy.vb).

Quitar únicamente el requisito de publicación desplazaría el fallo a una etapa posterior. Relajar únicamente la verificación posterior podría aceptar un documento con tipo incorrecto. Ambos controles deben evolucionar conjuntamente con la preparación y el comando.

## Soporte observado en el almacenamiento

En [ClassAlmacenamiento.vb](../../../../../workflow/ClassAlmacenamiento.vb), `AlmacenamientoDocumentoProduccionDocumental` transmite ID y descripción al motor `Almacenamiento`. Este contiene ramas para representar el ID cero como `NULL` en el gabinete y el inventario; la descripción vacía puede persistirse como `NULL`, y la descripción del índice se sustituye por «NA».

Este comportamiento hace viable estudiar la ausencia conservando el escritor intacto. No acredita que los gabinetes o bases desplegadas permitan todas esas representaciones. Antes de efectos deben comprobarse columnas, nulabilidad, restricciones y obligatoriedad de índices. La clase documental y el formato siguen siendo necesarios aunque falte la tipología TRD.

La carga individual pasa por `PreAlmacenaDocumentoProduccion`, que sí rechaza expresamente un ID de tipología no positivo. La importación utiliza su adaptador propio hacia `AlmacenamientoDocumentoProduccionDocumental`. Esta mejora no modifica la carga individual ni autoriza relajar su validación.

## Decisiones propuestas para implementar

- Política de perfil `REQUERIDO` / `OPCIONAL`; propiedad ausente significa `REQUERIDO` y se omite al serializar para conservar documentos y huellas anteriores.
- La regla conserva su obligatoriedad particular. El modo opcional permite no tener regla o una ausencia resuelta de una regla no obligatoria; no invalida una regla que expresamente exige valor.
- La ausencia genuina y un valor informado inválido tienen recorridos diferentes. Código desconocido, ID ajeno, ambigüedad, extracción fallida, dependencia pendiente o catálogo inaccesible siguen bloqueando conforme a las políticas existentes.
- El plan conserva ausencia y procedencia sin IDs ficticios; el adaptador traduce a `0` y descripción vacía únicamente al invocar el contrato legacy existente.
- El verificador debe comparar las representaciones exactas por campo. La ausencia no permite ignorar vínculos, identificadores, integridad, reclamaciones o reconciliación de resultados inciertos.
- Configuración incompatible del gabinete bloquea antes de confirmar y crear expedientes; no se modifica el esquema para eludir ese bloqueo.
- La política fijada en el lote no cambia al publicar otra versión del perfil. La mejora es independiente del prompt 14 de fecha inicial.

Los nombres de propiedades y representación final de ausencia pueden refinarse por compatibilidad demostrada, manteniendo esas reglas. La implementación debe registrar el contrato final de todos los consumidores.

## Evidencia y comprobaciones pendientes

Se ejecutaron cinco comprobaciones locales sobre los validadores compilados de producción, sin base de datos, HTTP o almacenamiento:

1. Sin regla de tipología al publicar: `TIPOLOGIA / FUENTE_REQUERIDA`.
2. Sin regla al guardar borrador: estructura válida, todavía no publicable.
3. Regla con cadena vacía y `Obligatoria=false`: `VALOR_VACIO`.
4. Regla con `Nothing` y `Obligatoria=false`: `VALOR_VACIO`.
5. Referencia `0`: inválida como identificador interno.

Los datos de ejecución permanecen en `.tmp`, excluidos de Git. El verificador D-RI-07 confirmó 10 archivos y 24 funciones protegidas sin diferencias durante la revisión. No se probaron persistencia real sin tipo, esquema de un gabinete desplegado o recuperación real del nuevo modo.

La implementación debe completar TD-01–TD-16 del prompt, incluidos compatibilidad histórica, preparación segmentada, verificación posterior, recuperación V2 y comportamiento mixto con/sin tipo. Una prueba con dobles no acredita compatibilidad del esquema real ni un almacenamiento real exitoso.

## Revisión del prompt

El 27 de noviembre de 2026 se aplicó el evaluador local `testFrontendPromptReview` de `tools/opsxj/scripts/lib/frontendPromptReviewService.js`, utilizado por `opsxj:prompt-review`, con perfil `legacy-webforms-vb`, sin correcciones automáticas ni acceso remoto. Resultado: cero hallazgos bloqueantes o mayores y un aviso informativo de revisión semántica. La revisión de arquitectura comprobó la coherencia entre política, regla particular, representación de ausencia, llamada al escritor y verificación posterior.

La comprobación de enlaces de los dos documentos nuevos, índice y guía verificó 47 destinos locales sin enlaces rotos. El prompt contiene 16 escenarios de aceptación. `git diff --check` y el verificador de congelación de almacenamiento pasaron. Esta entrega es documental y no ejecuta la implementación descrita.
