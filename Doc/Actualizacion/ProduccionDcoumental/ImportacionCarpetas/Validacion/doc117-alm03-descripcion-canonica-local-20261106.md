# DOC-117 ALM-03: corrección local de la descripción canónica

La corrección de soporte está implementada y validada localmente: **310/310 pruebas PASS**, compilación completa sin errores y OpenSpec estricto PASS. No hubo autenticación, SELECT reales, cargas o instalación durante este avance. La evidencia real sigue en **250 SELECT acumulados**, A confirmado y B sin iniciar; DOC-117 permanece **16/18**.

## Qué cambia

El rechazo reproducido en `doc117-alm03-guard-diagnostic-verificado-20261106.md/.json` se debía a una descripción equivalente pero distinta del literal `Comprobante de Egreso` de la guarda de laboratorio. La corrección no convierte todas las comparaciones en insensibles a mayúsculas ni modifica el argumento enviado al escritor.

El nuevo módulo `doc117-alm03-canonical-description.cjs` prepara un descriptor inmutable vinculado a empresa 2, expediente 467 y tipología 50. Su lectura futura usa una única sentencia SELECT parametrizada, con el mismo filtro de instrumento activo, empresa y compatibilidad serie/subserie/transversal, y exige una fila inequívoca. La normalización Unicode/espacios/mayúsculas sirve exclusivamente para comprobar que la descripción del catálogo corresponde al nombre humano aprobado. El descriptor conserva el texto exacto y su SHA-256 UTF-8.

La variante de la guarda compara el hash del argumento con ese valor exacto del catálogo. Así acepta, por ejemplo, un catálogo que efectivamente dice `COMPROBANTE DE EGRESO`, sin sustituirlo por el literal mixto escrito a mano. Una representación distinta de la que se fijó se rechaza, aunque sea equivalente tras normalizarla. El argumento original se conserva en el llamador; la entrada al escritor sigue exigiendo `nombre <> value.Name` bajo comparación binaria para detectar cualquier cambio posterior. Las comprobaciones de actor, destino, tipología, clase, temporal, contenido y sesiones se mantienen.

El helper UI admite `requiredDocumentTypeName`: si la opción seleccionada no coincide exactamente con la descripción fijada, se detiene antes de Guardar y antes del callback de despacho. La representación enviada no se reescribe.

`createCanonicalControls` exige ese descriptor y revalida el catálogo antes y después. Los valores esperados de NOMBRE/TIPODOCUMENTO y la comprobación física usan la descripción exacta fijada; un cambio de catálogo o un valor almacenado solo equivalente se rechaza. Los controles de fechas, ceros de texto, clasificación, IDs, incorporación y documentos previos conservan su comportamiento.

Se mantiene el camino histórico de los ejecutores y la plantilla de guarda usados en la evidencia anterior. La corrección se publica como una variante separada y un punto de entrada de controles canónicos, para que una futura corrida use ambos contratos. No se alteran diarios, comandos, documentos, temporales o artefactos anteriores ni se habilita su repetición.

## Validación

- Guarda .NET compilada con modelos reales: acepta el valor exacto del catálogo en mayúsculas, espacios/Unicode y ancho completo; rechaza texto distinto, representación equivalente pero diferente, tipología o selección incorrecta, archivo alterado y mutación del argumento entre preparar/entrar. La entrada global del observador sigue desactivada.
- Controles con catálogo simulado: preservan la representación canónica y rechazan cambios de catálogo, NOMBRE o TIPODOCUMENTO. El resto de negativos de clasificación, índices, conteos y preservación sigue pasando.
- UI oficial con dobles: rechaza el vínculo exacto ausente/inválido/distinto antes de despachar. Sin modificar el contrato opcional, los consumidores históricos mantienen su comportamiento.
- Regresión final conjunta: **310/310 PASS**. Se corrigió una prueba que suponía ausencia de evidencia anterior: ahora verifica que `--check` conserve sus archivos byte a byte. No se borra evidencia para hacer pasar una prueba.
- Build completo: `COMPILED_CANONICAL_DORMANT_NO_BINDING`, cero errores, 912 entradas originales intactas. DLL fuera de `bin`, sin binding de catálogo, Site/Run/recursos reales ni autorización; escritor e importaciones desactivados.
- DLL instalada, cuatro copias de respaldo históricas y candidatas históricas comprobadas. Informes de la detención y del diagnóstico real preservados por SHA.

Build local: `.tmp/doc117-alm03-canonical-build-20261106/build-result.json`. Candidata desactivada: `8f3c30d5d54d97345e1f01146649141dba1b431acd8c297efb0735f2cad8dcae`. Regresión: `.tmp/doc117-alm03-canonical-final-regression-20261106.tap`. Certificado independiente: `doc117-alm03-descripcion-canonica-local-20261106.json`.

## Límite real pendiente

La descripción real del catálogo todavía no se ha vuelto a leer ni fijado con este protocolo; las variantes de las pruebas son sintéticas. La DLL compilada es una variante desactivada, no una candidata activa para repetir el diagnóstico o la pareja anteriores. Los permisos anteriores no son reutilizables: el PDF diagnóstico ya fue enviado una vez, sus diarios existen y los ejecutores rechazan las huellas de código nuevas.

La corrección local no acredita almacenamiento, valores físicos reales o coexistencia después de corregir la guarda. Para completar 17.1 se requiere otro caso acotado que use el nuevo contrato, recursos y presupuesto propios, y una autorización concreta de sus efectos. El acumulado inicial de cualquier continuación será **250**: quedan 70 hasta el techo histórico 320; no se reinició ni amplió el presupuesto. No se enviaron A/B ni se reconciliaron o repararon intentos. **17.1 y 18.1 siguen abiertas; 16/18, sin cierre o archivo.**
