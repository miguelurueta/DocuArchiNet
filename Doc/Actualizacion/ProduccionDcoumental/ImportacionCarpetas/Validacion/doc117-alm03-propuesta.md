# DOC-117 — Propuesta técnica, sin ejecución real

Diseño aprobado editorialmente. Esta propuesta no autoriza cargas, credenciales ni instalación de DLL. Conservar todos los recursos; no tocar lote 18/intento 41 ni repetir efectos inciertos. Controles SELECT por los DSN registrados MembershipUsers, sin URL MySQL. Usuario de aplicación y cuenta MySQL de lectura son identidades distintas. Secretos efímeros exclusivamente en consola.

## Alcance observable

Importación y carga individual oficial de Producción en dos sesiones independientes del mismo actor autorizado; la sesión ASP.NET compartida puede serializar y no sirve para atribuir simultaneidad. Cada flujo tendrá PDF y destino propios; conservar originales, copia consumible y relaciones de cada documento. No probar consumidores SII ni exclusión del XML.

El solapamiento requiere tiempos fiables de entrada/salida de los escritores y un intervalo común, con correlación al recurso exacto. Dos peticiones despachadas juntas o con respuestas superpuestas no bastan. Reutilizar registro existente si ofrece esa observación sin secretos; si no la ofrece, proponer instrumentación de laboratorio separada con restauración, nunca añadir telemetría o puertas productivas por inferencia. Sin esa observación, resultado SERIALIZADO o NO_ACREDITADO, no PASS de concurrencia.

## Presupuesto máximo propuesto

Un plan/lote nuevos versión 38/nivel 7, dos raíces, dos recepciones y dos expedientes; dos documentos de importación. Una carga individual adicional en expediente 467/tipología 50 únicamente si el preflight acredita acceso/clasificación y ese destino se autoriza expresamente otra vez: tres documentos nuevos en total. No se crea o altera ese expediente por inferencia. El solapamiento se prueba entre una de las dos cargas de importación y la carga individual; la otra carga de importación completa el recorrido sin ampliar concurrencia.

Manifestar hashes y nombres exactos de tres PDF de 677 bytes y sufijo único antes de autorizar. Sin autorización válida del destino individual o sin observación segura del escritor, no ejecutar. No cambiar permisos, perfiles, esquema, configuración o función compartida para obtener un PASS.

## Controles y salida

Snapshots SELECT antes/después, IDs por flujo, exactamente un documento/inventario/incorporación por PDF, valores y expediente propios, retención/hash de originales. Consulta posterior ante respuesta incierta, sin replay de un escritor dudoso. Conservar todos los recursos. Reportar separadamente coincidencia temporal, integridad propia y límites XML/SII.

## Estado

NO_EJECUTADO. No se ha aprobado mecanismo de observación, recursos exactos o autorización del destino individual.

Revisión posterior 2026-11-06: `doc117-alm03-observabilidad-local-20261106.json` acredita la revisión de llamadores, 23 pruebas locales y una captura CLR únicamente sintética. Los intervalos exteriores pueden solaparse sin ejecución interna simultánea; se rechaza ese falso positivo. La captura sintética todavía no tiene extracción de eventos correlacionados por método/hilo/recurso y no es un mecanismo listo para IIS. `doc117-alm03-observacion-propuesta-20261106.md` plantea una excepción editorial exclusivamente de laboratorio para observar dentro del método compartido, dejando intacto el producto. Esta excepción no se ha aprobado ni aplicado. No hay candidata ALM-03 instalada o carga real y no se hereda autorización de ALM-02. Progreso 16/18; 17.1–18.1 pendientes.

Decisión posterior del usuario: excepción editorial aprobada mediante «aprobado» y desarrollada en copia local. `doc117-alm03-metodo-local-20261106.md/.json` registra candidata desactivada compilada, preservación de 912 entradas, 184/184 pruebas y restauración aislada PASS. Las anotaciones previas de espera son históricas. La entrada de la candidata rechaza todo almacenamiento; todavía faltan recursos reales, guardas de llamadores y ejecutor cerrado. No está instalada ni autorizada para ejecutarse. ALM-03 real sigue NO_EJECUTADO, 16/18.

Lectura autorizada posterior: doce SELECT en total, sin efectos. El preflight se detuvo al exigir nivel 7 también al destino individual; el diagnóstico identificó expediente 467/relación 1072 en nivel 8 y tipología 50 compatible. No se alteran esas relaciones. `doc117-alm03-destino-lectura-20261106.md/.json` distingue el preflight no aceptado del diagnóstico completado. Preparación revisable siguiente en `doc117-alm03-setup-listo-20261106.md`: importación en nivel 7, destino individual de nivel 8 solo verificado, máximo un plan/lote y dos recepciones/expedientes/materializaciones, cero almacenamiento/runtime. Hasta 160 SELECT nuevos de esa fase; requiere autorización específica. Soporte local y regresión 221/221 PASS, sin preparación real aún.

Preparación autorizada después mediante «autoriza»: PASS real, 80/160 SELECT, plan 38/lote 22, archivos 36/37, expedientes 666/667 y relaciones 1091/1092. Destino individual 467/nivel 8/1072 verificado sin carga. Originales, valores y recursos anteriores conservados; cero documentos, ALMACENAR o cambios de DLL. Ver `doc117-alm03-setup-pass-20261106.md/.json`. Los estados anteriores describen sus etapas históricas. Hay identidades/huellas reales cerradas, pero faltan las guardas activas, candidata y ejecutor de coexistencia revisados; no hay autorización de instalación o almacenamiento derivada de esta preparación. ALM-03 de coexistencia NO_EJECUTADO, 16/18.
