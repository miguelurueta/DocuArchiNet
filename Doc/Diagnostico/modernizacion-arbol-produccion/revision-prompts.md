# Revisión de los prompts de modernización de niveles y expedientes

Fecha de revisión: 2026-11-25. Base de entrada: commit `00897144`. Serie revisada: los ocho prompts PB00 a PB05 y PF01/PF02 enlazados en la [guía de ejecución](prompts/00-guia-ejecucion.md).

**Resultado final: ocho prompts con PASS, cero hallazgos bloqueantes, cero mayores y cero menores.** Cada archivo conserva el aviso informativo `MANUAL_REVIEW_RECOMMENDED`. Este resultado corresponde a revisión estructural del texto; no aprueba decisiones funcionales ni acredita implementación de la aplicación.

## Comandos utilizados y alcance

### Suite solicitada

El comando `opsxj:test-prompt-review` está disponible en el tooling del repositorio Core. Es una suite de casos del motor PowerShell; no recibe los ocho archivos como entradas de revisión. Se ejecutó desde la carpeta principal actual:

```powershell
npm.cmd --prefix 'D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Tools\jira-open' run opsxj:test-prompt-review --
```

Resultado: exit code 0 y `PASS: opsxj:prompt-review validates prompts and exit codes.` También se observaron los resultados de sus comprobaciones internas: legibilidad, 40 casos locales AST; errores de API, 27 casos locales AST; evidencia de implementación de errores de API, 21 casos con stubs locales.

Los temporales de esta ejecución se dirigieron a `.tmp/opsxj-review-arbol-20261125/runtime/` del workspace. La suite utilizó recursos temporales y no evaluó la aplicación ni consultó servicios remotos. Sus reglas de backend Core no se trasladaron indiscriminadamente a los prompts WebForms.

### Revisión de los ocho archivos

Se utilizó el revisor local de este repositorio y el perfil explícito `legacy-webforms-vb`, mediante este comando para cada archivo:

```text
node tools/opsxj/scripts/opsxj.js opsxj:technical-review Doc/Diagnostico/modernizacion-arbol-produccion/prompts/ARCHIVO.md --tech-profile legacy-webforms-vb --no-fix
```

`ARCHIVO.md` representa cada nombre concreto de la tabla de resultados. Se ejecutaron los ocho comandos de forma secuencial, conservando por archivo el reporte JSON, salida, código de retorno y SHA-256 del texto evaluado. `--no-fix` mantuvo la evaluación sin correcciones automáticas; los ajustes se realizaron y revisaron expresamente.

La suite PowerShell solicitada y este revisor Node son implementaciones distintas. Sus resultados se registran por separado: pasar la suite no sustituye la revisión de nuestros archivos ni demuestra por sí solo el comportamiento del revisor Node.

### Pruebas del revisor local aplicado

```text
npm.cmd --prefix tools/opsxj test -- scripts/lib/frontendPromptReviewService.test.js
```

Resultado: 1 archivo de pruebas aprobado, 27 pruebas aprobadas, exit code 0.

## Comparación antes y después

La evaluación inicial de los textos originales produjo 34 bloqueantes y 51 mayores. La evaluación final utiliza los mismos perfiles y reglas, sin cambiar el motor ni reducir su severidad.

| Prompt | Bloqueantes iniciales | Mayores iniciales | Estado final | Bloqueantes finales | Mayores finales |
|---|---:|---:|---|---:|---:|
| [PB00 Inventario](prompts/01-pb00-inventario-contratos.md) | 4 | 4 | PASS | 0 | 0 |
| [PF01 Presentación](prompts/02-pf01-presentacion-arbol.md) | 4 | 8 | PASS | 0 | 0 |
| [PB01 Identidad](prompts/03-pb01-identidad-contexto.md) | 4 | 6 | PASS | 0 | 0 |
| [PB02 Acciones](prompts/04-pb02-acciones-traslado.md) | 5 | 6 | PASS | 0 | 0 |
| [PB03 Lecturas](prompts/05-pb03-lecturas-capacidades.md) | 6 | 8 | PASS | 0 | 0 |
| [PF02 Experiencia](prompts/06-pf02-experiencia-contextual.md) | 4 | 7 | PASS | 0 | 0 |
| [PB04 Compatibilidad](prompts/07-pb04-compatibilidad-integral.md) | 5 | 7 | PASS | 0 | 0 |
| [PB05 Cierre](prompts/08-pb05-verificacion-cierre.md) | 2 | 5 | PASS | 0 | 0 |
| **Total** | **34** | **51** | **8 PASS** | **0** | **0** |

En ambas rondas hubo cero hallazgos menores. Al finalizar hay ocho avisos informativos de revisión manual, uno por prompt. El código de salida del revisor depende de bloqueantes, por lo que se comprobaron también los mayores y menores; no se utilizó solamente exit code 0 para declarar el resultado.

## Correcciones incorporadas

- Se hicieron explícitas las secciones de rol, restricciones críticas, criterios de aceptación, pruebas obligatorias, documentación técnica y entregable final. Cada prompt conserva su alcance específico y sus condiciones verificables.
- Se añadió el flujo de presentación, contexto, lecturas y cierre donde la formulación anterior lo dejaba disperso.
- Los paquetes con posibles cambios de varias responsabilidades incorporan precondiciones de rutas e inventario de archivos exactos en `implementacion/design.md`, además del registro de archivos nuevos en el `.vbproj`. Las rutas propuestas no obligan a crear capas vacías ni constituyen componentes ya implementados.
- La verificación exige evidencia de pruebas unitarias/focales o QA manual y compilación MSBuild cuando corresponda. La exclusión de build en entregas solo visuales o documentales debe justificarse por su impacto real.
- El inventario PB00 mantiene alcance exclusivamente documental, con justificación de por qué no tiene un recorrido E2E propio. Los paquetes funcionales integran su recorrido E2E o validación manual reproducible en la aceptación del mismo cambio.
- Se exige reutilizar `tools/e2e`, leer el runbook, disponer de autorización de ambiente/cuentas/datos descartables, sanear evidencia, proteger secretos y no sustituir comprobaciones reales con simulaciones. No se introduce un sistema de activación por usuario/grupo.
- Se concretó el refinamiento por paquete: puede comenzar con decisiones pendientes; la parte que modifica aplicación necesita resolver sus decisiones y contar con autorización. La exploración sigue siendo el registro central y ninguna de las 18 decisiones se marcó aprobada durante esta revisión.

## Comprobación semántica complementaria

La corrección documental conserva estos límites del diseño:

1. Los rótulos visuales no se incorporan al nombre canónico ni reemplazan la identidad del nodo.
2. Validar selección no autoriza operaciones futuras por sesión ni resuelve automáticamente el caso de varias pestañas.
3. Cortar/pegar conserva las reglas de propiedad y el efecto transaccional; eliminar no se redefine como quitar un nodo visual.
4. PB03 sigue siendo opcional y no exige nuevas tablas o endpoints si los datos existentes bastan.
5. La importación y Excel conservan sus contratos, ámbitos de autoridad y separación de formularios.
6. Pasar revisión estructural no sustituye decisiones de producto, compilación del cambio futuro ni validación autorizada del ambiente.

No se introdujeron palabras o marcadores de exención del tooling para hacer que el revisor omitiera las reglas de aplicación. Se mantuvo el perfil WebForms y no se añadieron exigencias de React/TypeScript o de APIs .NET Core ajenas a este módulo.

## Trazabilidad local de la revisión

Los reportes, logs y copias originales de ejecución permanecen excluidos de Git:

```text
.tmp/opsxj-review-arbol-20261125/baseline/
.tmp/opsxj-review-arbol-20261125/final/
```

Cada reporte final contiene `sourceSha256`, `technologyProfile`, hallazgos y `cliExitCode`. Los hashes abreviados siguientes identifican el contenido exacto evaluado, no el estado de aprobación:

| Archivo | SHA-256 del prompt final, primeros 12 caracteres |
|---|---|
| `01-pb00-inventario-contratos.md` | `f9ecccd08d0c` |
| `02-pf01-presentacion-arbol.md` | `4446590652e8` |
| `03-pb01-identidad-contexto.md` | `82e019691392` |
| `04-pb02-acciones-traslado.md` | `60b4f7349810` |
| `05-pb03-lecturas-capacidades.md` | `fd319d33105e` |
| `06-pf02-experiencia-contextual.md` | `1cf4b8eb9c4e` |
| `07-pb04-compatibilidad-integral.md` | `b94d46ae28e6` |
| `08-pb05-verificacion-cierre.md` | `bf2e1fcac846` |

Motor local revisado: `tools/opsxj/scripts/lib/frontendPromptReviewService.js`, SHA-256 `8f7c1d1b34844f18e33ad155e4b78323a86523abe31946f275c4dd2c67fc277a`. Catálogo de perfiles: `tools/opsxj/scripts/lib/opsxjProfileCatalog.js`, SHA-256 `c58b55874f6004e56e0552762f759578056f9fb8ac129fef01d34b507d91fe91`.

Modificar un prompt o una condición sustancial de su contexto requiere revisar la vigencia de este resultado y volver a evaluar los archivos afectados. La entrega se limita a los prompts, la guía, la exploración y este informe; no implementa funcionalidades del gestor ni ejecuta E2E autenticado o carga real.
