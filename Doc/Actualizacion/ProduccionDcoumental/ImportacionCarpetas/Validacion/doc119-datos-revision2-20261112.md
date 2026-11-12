# DOC-119 — preparación corregida, revisión2

Estado: **preparada y verificada localmente; nueva ejecución manual pendiente**. Corrige los datos de prueba de la corrida4ffe5447 sin cambiar producto, perfil, permisos o límite de efectos. Conserva la corrida y los originales anteriores.

## Diagnóstico de la preparación anterior

Cuatro SELECT posteriores, cero HTTP y cero sesiones de aplicación: consulta del contexto conocido, publicaciones40/42, sus diez reglas y planes recientes del propietario141/empresa2/destino7. Diagnóstico cerrado y credenciales eliminadas. **No existen planes de ese alcance desde2026-11-12 19:40UTC hasta el corte consultado.** Total de controles de este intento:3SELECT de preparación+4SELECT de diagnóstico=7; no se suman como HTTP o sesiones de aplicación.

El perfil26/version40 extrae la tipología como referencia del segundo segmento del nombre del PDF sin extensión. `DOC119-001.pdf` produce `001`: el validador de referencias del ensamblado actual lo rechaza y el adaptador de catálogo lanza SOLICITUD_INVALIDA para referencias inválidas. Esto explica el rechazo observado de PrepararPlan; la corrida original no conservó el mensaje del servidor, por lo que no se presenta esa etapa como observación directa del cuerpo HTTP. No es un defecto reproducido del producto.

El mismo perfil obtiene CODIGO del primer segmento de carpeta, NOMBRE de la carpeta completa y FECHA_INICIAL del segundo segmento con formato yyyyMMdd. Las carpetas idénticas de la propuesta anterior también habrían producido el mismo código DOC119; el repositorio bloquea códigos repetidos en el inventario. No se retira esa protección para fabricar datos de aceptación.

## Inventario corregido

| Recurso | Revisión2 |
| --- | --- |
| Carpeta r1/s1 | `11926111201-20261112` |
| Carpeta r2/s2 | `11926111202-20261112` |
| PDF | Todos `DOC119-79.pdf`, con tipología resuelta79 |
| r1 | a1, un PDF directamente en la carpeta |
| r2 | a2–a27, un PDF por subcarpeta pendiente-001 a pendiente-026 |
| Volumen | 27 PDF ×677 bytes=18.279 bytes locales |
| Efectos propuestos | Un plan/un lote/dos raíces; recibir solamente a1 y crear solamente el expediente de r1; dejar26 pendientes |

Ubicación local excluida: `.tmp/doc119-next-preflight/eb09fc5981be453b936b2bf1714c43a3/fixture-v2`; manifiesto `fixture-manifest-v2.json`. Las copias usan fechas reales y conservan contenido SHA-256 a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c. No se alteraron los originales de la revisión1.

Se comprobó con el resolvedor del ensamblado local y las reglas40 obtenidas por SELECT: ambos códigos son distintos, ambas fechas2026-11-12 y los27 PDF resuelven tipología79, sin incidencias puras. `--check` comprueba los27 originales, sus nombres/rutas/tamaños/modificaciones/hash; **11/11 pruebas locales** pasan, incluida correlación de copias homónimas por ruta y rechazo de ambigüedad. Son verificaciones locales; falta el catálogo vigente, las coincidencias documentales, los guards y la persistencia reales. El runner detiene antes de confirmar si snapshot/elegibilidad/valores no coinciden.

La correlación de la reselección local ahora compara **nombre y ruta relativa** para elegir la copia correcta entre PDF de igual nombre; el cliente sigue revalidando tamaño/fecha por el recursoId seleccionado. No se elige simplemente la primera coincidencia de nombre.

## Alcance y límites de la próxima ventana

Mismo ambiente localhost y propietario141/empresa2/destino7, perfil26/version40/gabinete10; SETUP400HTTP/40SELECT/1sesión. Después SEG-01:600HTTP/80SELECT/2sesiones con Carolina136; SEG-02:600HTTP/60SELECT/2sesiones. No almacenar documentos, limpiar, reconciliar ni cambiar permisos/perfiles. No se reutilizan los secretos ni se resetea el consumo del intento anterior.

Esta revisión conserva archivos homónimos, **no raíces físicas homónimas**. Por tanto permite avanzar en paginación y recuperación, pero no acredita el caso adicional de raíces homónimas de D-03. El runner lo informa como límite; no se altera la condición de aceptación para abrir2.7. Tampoco se fuerza una relación ausente retirándola del DOM. La preparación de un caso de homónimos compatible o una decisión expresa de alcance sigue pendiente antes de aceptar SEG-02 completo.

Tras el STOPPED no se lanza automáticamente otra corrida. El próximo comando manual, con un runId nuevo, utiliza `run-doc119-datos-aceptacion.cjs --run <32-hex-nuevo> --authorized-plan` y esta revisión; pide nuevos secretos en consola. Se requiere instrucción expresa para esa ventana después del fallo, según el runbook, no una repetición ciega del manifiesto original.
