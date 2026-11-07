# DOC-116 EXP02 interno — ejecución preparada

Estado: ejecutor y recuperación listos, activación específica pendiente. 108/108 pruebas locales PASS. Ningún fallo interno real ejecutado y DLL de IIS original intacto.

## Binarios revisables

DLL original: 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5. Candidata de laboratorio: e26242b901043394bd1dc38063becfe399cd093738cfd10b7ebca24785d8944a. Compilada desde la copia local con overlay externo y manifest de 912 entradas de proyecto/recurso; SHA 412a115d0a4e7d8b6b839aa5ba17082337fce14bb129332645db82509909d1d3. Se comprobaron antes/después de compilar y se fijan los hashes del ejecutor y del helper. No se atribuye evidencia del DLL original al binario instrumentado.

## Alcance exacto propuesto

Reutiliza lote 18/plan 34, nivel 215, contexto 141/2, publicación 38 y las dos raíces 29/30 con sus PDF recibidos. No prepara planes/lotes ni recibe PDF adicionales. Máximo dos expedientes nuevos, tres solicitudes, cero documentos definitivos y cero cambios de permisos. Se conservan recursos antiguos y nuevos.

Primero raíz 29: creación real y checkpoint confirmado; excepción interna al devolverlo, verificación del histórico por el coordinador y replay confirmado con los mismos IDs. Después raíz 30: excepción tras legacy y antes de checkpoint; marca privada con IDs/intent, evidencia física por SELECT y conservación del intento EN_CURSO/publicación INCIERTO. No se vuelve a crear ni se modifica el checkpoint manualmente.

## Activación y restauración

El ejecutor comprobará con el runtime original que ambas raíces sigan frescas, que la concesión siga efectiva y que no haya recursos posteriores inesperados. Copia original con hash comprobado y diario persistido antes de activar. Un guardián Node lanzado por proceso independiente oculto queda listo antes de sustituir el DLL; restaura al terminar el worker o a los 600 segundos. Se exige una ventana exclusiva del sitio sin otros despliegues, importaciones ni reinicios manuales.

Sustitución por archivo temporal verificado y rename; se traslada/comprueba ACL de Windows del archivo. Finally restaura el original, comprueba hash y abre sesión/consulta de lectura con el sitio original. Si un tercero cambia el binario no se sobrescribe a ciegas: se conserva estado y se detiene para revisión. Diario: tools/e2e/artifacts/doc116-internal-runtime-lease.json; respaldo: .tmp/doc116-internal-runtime/original.dll. Recuperación explícita: node tools/e2e/scripts/support/doc116-runtime-lease.cjs --restore <manifest-autorizado>.

## Verificación local

Compilación WebForms/VB real PASS. Pruebas locales de contratos: dos ventanas, máximo tres peticiones, marca incorrecta, identidad incompleta, transporte incierto sin retries y efectos documentales rechazados. Recuperación probada con archivos descartables: backup, guarda independiente obligatoria, sustitución/restauración, binario/backup alterado y autorización ausente. ACL probada nativamente solo en archivos descartables. El DLL real no se modificó en estos tests. Playwright descubre exactamente la prueba protegida, sin ejecutarla.

## Aprobación pendiente

La instrucción del usuario «continua con esa prueba» se registra como aprobación de preparar la vía propuesta; la respuesta anterior había limitado expresamente esa aprobación al diseño. Ahora los binarios y scripts están concretos: falta aprobación explícita de la sustitución temporal del DLL y de los efectos máximos anteriores. Credenciales solo en consola privada al autorizar. La aceptación de DOC116 sigue bloqueada hasta ejecutar y revisar los resultados reales.

## Autorización y condición de conservación de DLL

Usuario autorizó la activación con condición de conservar la DLL. Antes de abrir consola se crearon y comprobaron por SHA256 dos respaldos persistidos: .tmp/doc116-internal-runtime/original.dll y .tmp/doc116-internal-reserve/original.dll. Ambos coinciden con el DLL de IIS original; activación exige las dos copias intactas y restauración permite recuperar desde la reserva si la primaria se altera. No se borran al terminar. Se explicó que una avería física del equipo no admite garantía absoluta. Autorización saneada: `doc116-exp02-interno-autorizacion-20261105.json`. No se ha desplegado ni ejecutado todavía.
