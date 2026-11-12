# DOC-119 — SEG-01 aceptado; recuperación pendiente

Estado: **7/24 tareas verificadas**. Se completa2.4;2.5 y2.7 permanecen abiertas. [Evidencia saneada](doc119-seg01-aceptado-20261112.json), contrastada con los estados locales y con las referencias/manifiesto de la preparación. No contiene cuentas, claves, cookies, tokens, CSV ni documentos privados.

## Preparación existente, sin repetición de efectos

La corrida accc9ae9f3344d7a96123ed7cd986a79 confirmó plan45/lote29 en destino7, propietario141/empresa2, con27 archivos/dos raíces; recibió un PDF de677 bytes y creó un expediente. Se detuvo por SETUP_POSTCONDITION_FAILED con196HTTP/10SELECT/1sesión.

La corrida de lectura ccb25ec9d37a4da083caccfbe80001b1 comprobó esos datos existentes:26 archivos pendientes, uno recibido, un expediente, cero documentos almacenados y **dos intentos confirmados: RECEPCION y CREAR_EXPEDIENTE**. La expectativa anterior de un único intento era incorrecta. Se corrigió el verificador y se registran cada condición y fase por separado. No se repitieron confirmación, recepción o creación.

La proyección de control DOC-119 incluye ahora origen_doc y bytes_recibidos mediante un SELECT adicional acotado al lote y correlacionado por archivo_id. Así la reselección usa la copia local correcta por nombre+ruta, y la huella también detecta cambios de metadatos. Se conservaron los controles compartidos anteriores y sus evidencias históricas.

## Aceptación real SEG-01

Corrida ccb25ec9, ensamblado local SHA-256 67e3b823cad825c558c2d0ee06065a0b4c4143e92cd18c1861e1c03eaf98c90e; HEAD2a641b3f con hashes explícitos del ejecutor modificado sin commit durante esa ejecución. El registro de código no se reduce al HEAD ni atribuye a ese commit los archivos aún modificados.

**PASS_READONLY**: propietario consulta27 archivos, aplica filtros, pasa realmente a una segunda página, descarga CSV de esa página con una fila y comprueba un filtro vacío. Carolina136/empresa2, identificada por SELECT vigente y autenticada, no puede consultar ni exportar el lote ajeno. No hay sustitución de respuestas del servicio ni reducción artificial del tamaño de página.

Consumo de la ventana: **384 HTTP/29 SELECT/2 sesiones**, dentro de600/80/2. Incluye descubrimiento y verificación inicial del lote existente. Huellas de negocio antes/después idénticas: b6c564f13b168822728f35e09ec7f551dde0b1da9ad183c68d65223bf108b278. Recepción no se interpreta como documento almacenado: sigue en cero almacenamiento. Esta aceptación se conserva aunque la ventana posterior de recuperación haya fallado.

## Detención de SEG-02 y corrección del verificador

SEG-02 consumió274HTTP/8SELECT/1sesión y se detuvo con ORIGINAL_BRANCH_REQUIRED tras recargar y seleccionar el alterno169. La comprobación previa devolvió null si el padre estaba ausente, ambiguo u oculto o faltaba su contenedor; el registro anterior no distingue cuál de esas condiciones ocurrió. No se afirma una causa concreta de visibilidad ni un defecto del producto.

El verificador impedía continuar antes de reseleccionar, aunque RQ-03 contempla que la rama oculta/ausente no invalide el lote. Se sustituyó por una observación estructurada (AVAILABLE/PARENT_ABSENT/PARENT_AMBIGUOUS/PARENT_HIDDEN/BRANCH_ABSENT). La consulta y la reselección continúan sin envío; una rama no disponible debe conservarse intacta y el resultado registra ese límite, sin acreditar inserción ni deduplicación de una rama que no se pudo comprobar. Las ramas disponibles siguen exigiendo una relación por ID, preservando selección y expansión. No se retiran nodos del DOM real para forzar aceptación.

La selección SQL de alternos prioriza ahora hijos y después hermanos del destino original, con la misma autoridad vigente y parámetros, para favorecer una rama original visible. Esa preferencia no garantiza disponibilidad ni convierte una limitación en éxito; se observa el DOM real.

Regresión local: **48/48** pruebas de políticas/correlación/fuentes y **2/2** recorridos Chromium con cliente real/servicios sintéticos, rama disponible y rama oculta. No se presentan esos recorridos como aceptación integrada de SEG-02. La ventana fallida no completó reselección ni huella posterior: solo SEG-01 acredita invariancia de negocio en esta corrida.

## Próxima ventana concreta

Ejecutor preparado: `node tools/e2e/scripts/run-doc119-datos-aceptacion.cjs --run <32-hex-nuevo> --authorized-plan --recovery-only ccb25ec9d37a4da083caccfbe80001b1`.

Solo SEG-02, hasta600HTTP/60SELECT/2sesiones. Valida la corrida cerrada con SEG-01 aprobado, las huellas iguales, la preparación original y el mismo manifiesto. Revisa por SELECT que el lote29/plan45 conserva los datos antes de autenticarse y no duplica efectos. No vuelve a ejecutar SEG-01 ni solicita credenciales de Carolina; captura propietario/SELECT en consola enmascarada. La aceptación previa solo se referencia por runId, sin copiarla como resultado nuevo.

Conserva los límites conocidos: este inventario no tiene raíces físicas homónimas, y una relación ya representada no acredita inserción de una relación nueva. SEG-02 permanece parcial si esos casos no se observan; la compuerta UI sigue cerrada. Una nueva ventana tras el STOPPED requiere instrucción manual expresa según el runbook.
