# DOC-117 ALM-01 — Manifiesto exacto, sin ejecutar

## format-a — publicación 26/40

Raíz: `00011701-20261105`. PDF: `DOC117-79.pdf`, 677 bytes. Código esperado: `00011701`; nombre: raíz completa; fecha inicial: `2026-11-05`; tipología por REFERENCIA: `79`. Índice 14/TEXTO: ID del expediente confirmado por el mismo flujo, sin un ID inventado.

## format-b — publicación 27/42

Raíz: `00011702#05-11-2026`. PDF: `79.pdf`, 677 bytes. Código esperado: `00011702`; nombre: raíz completa; fecha inicial: `2026-11-05`; tipología por REFERENCIA: `79`. Índice 14/TEXTO: ID del expediente confirmado por el mismo flujo, sin un ID inventado.

PDF reutilizado: doc111-carga-individual.pdf; SHA256 `a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c`. SHA256 del manifiesto lógico: `2006f16cc218074f4b2d930daceea480ad5d19dbb1ac888295f7d3d5451ff47f`. JSON ejecutable offline: tools/e2e/fixtures/doc117-alm01-manifest.json. Presupuesto: dos planes, dos lotes, dos recepciones, dos expedientes, dos documentos y hasta cuatro solicitudes de almacenamiento contando dos replay confirmados. Recursos conservados.

## Validación y compuertas

Ocho pruebas locales PASS: recetas resueltas por ResolvedorReglasPerfilImportacion del código actual sin conexión, valores diferidos sin identidad ficticia de ambiente, rechazos ante cambios/inyectados, publicación por identidad/reglas, paginación de lectura y rechazo de PrepararPlan por el cliente reutilizado en readOnly. Los IDs sintéticos usados en el test Framework son dobles locales explícitos. No sustituyen una lectura de publicaciones vigente ni certifican tipología, gabinete, ausencia de raíces o efectos reales.

La consola de lectura compara las publicaciones 40/42 y registra su snapshot SHA256, sin pedir MySQL o preparar planes. Solo acepta ambiente/cuenta/certificado autorizados. Su PASS no autoriza la carga ni comprueba ausencia de raíces. Si difieren reglas/publicaciones, detener y revisar sin cambiar perfiles. El driver de raíz única está preparado y validado localmente reutilizando opciones acotadas del transporte/verificador compartidos; no se invoca runRec de dos raíces para sortear esa diferencia. La ejecución real sigue sin autorizarse.
