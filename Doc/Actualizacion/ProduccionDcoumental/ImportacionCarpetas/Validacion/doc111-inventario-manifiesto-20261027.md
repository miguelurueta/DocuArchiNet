# DOC-111 — Inventario y manifiesto

Fecha: 2026-10-27. Alcance: tareas 7.1–7.6, RQ-03/D-03, repositorio local principal. Progreso acumulado: 49/130. Evidencia local; no acredita UI integrada ni E2E.

`InventarioImportacion.js` conserva selección, raíz, archivo y referencia File sin depender de filas DOM. Cada selección agrega una raíz con identidades nuevas, incluso al repetir nombres o la misma carpeta. Cancelar no agrega entradas. Quitar una raíz libera sus referencias; quitar su último archivo elimina la raíz vacía. El selector exige capacidad de directorios y se limpia entre aperturas para permitir repetir la selección. No lee contenido ni infiere directorios vacíos.

`ImportacionCarpetas.js` mantiene la revisión local: agregar/quitar archivos o cambiar versión invalida la preparación; una respuesta tardía o de una solicitud anterior no habilita confirmación. Devuelve copias de la referencia de plan. Este módulo todavía no renderiza una ventana ni llama al servicio; esas responsabilidades siguen pendientes.

`CanonizadorManifiestoImportacion.vb` valida pertenencia, correlaciones, una raíz por selección, raíces pobladas, decimales canónicos y cuotas explícitas del servidor. Rechaza rutas absolutas/traversal, segmentos vacíos, barras inversas, controles y colisiones NFC dentro de cada raíz, incluida la coincidencia archivo/directorio. Conserva los nombres originales y ordena una copia independiente. El repositorio reutiliza la misma canonización de ruta, sin cambiar el formato de sus huellas. No construye rutas físicas. La política RECHAZAR produce incidencias por archivo de subcarpeta; INCLUIR conserva esos archivos en su raíz. Una estrategia no implementada se rechaza.

Completitud significa consistencia de todas las entradas recibidas: el servidor no puede demostrar que el navegador enumeró cada archivo del disco. No se fabrican carpetas vacías. La preparación deberá usar conjuntamente Canonizar y EvaluarAgrupacion y aplicar las incidencias al modo de integridad, tarea 7.13 todavía abierta.

Validación ejecutada:

- `node --test tests/doc111-importacion-carpetas-client.test.cjs tests/doc110-perfil-importacion-client.test.cjs`: PASS, 9 pruebas. Homónimos, referencias, cancelación, selección repetida, errores sin cambios y respuestas tardías.
- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: PASS. Compila/ejecuta Framework con regresión administrativa y nuevos casos de manifiesto, colisiones NFC, pertenencia, cuotas, overflow y políticas de subcarpetas.
- MSBuild del proyecto completo: PASS, salida `.tmp/doc111-build`, sin desplegar ni sustituir el bin activo.

No se realizaron llamadas HTTP, consultas/escrituras BD, cargas documentales ni cambios de permisos. No se alteraron límites de IIS/ASP.NET. Los límites de prueba son sintéticos; la política operativa real sigue pendiente de configuración. Los archivos JavaScript se incluyen en el proyecto pero aún no se cargan desde la página de Producción; la integración de ventana corresponde al bloque 12. Apertura y preparación de servidor continúan desde 7.7.
