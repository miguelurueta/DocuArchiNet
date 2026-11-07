# PRODUCCION-IMPORTACION-CARPETA-PERFIL

- Ticket: DOC-110
- Cambio OpenSpec: doc-110-produccion-importacion-carpeta-perfil
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

`Gestion/WebFormProducionDocumental.aspx` incorpora el acceso a perfiles, proveedores y fuentes dentro de la navegación de Producción. Su code-behind emite el contenedor con capacidades y token del contexto autorizado, y actualiza el nivel después de UpdatePanel. Los administradores adicionales se muestran según sus facultades; ocultar un botón no sustituye la autorización ASMX.

`PerfilImportacionEditor.js` presenta búsqueda por código/nombre, lista paginada, configuración guiada, reglas, ensayo e historial. `ProveedorAdminEditor.js` permite configurar desde metadatos, validar, ensayar y publicar versiones; `FuenteAdminEditor.js` administra columnas autorizadas, disponibilidad e historial. `PerfilImportacionEditor.css` limita los estilos a estas superficies.

La búsqueda reutiliza el autocomplete existente, admite teclado y texto seguro, descarta respuestas anteriores y restaura foco al limpiar/cerrar. La apertura repetida y UpdatePanel evitan eventos duplicados. Formularios publicados son de lectura; cambios pendientes requieren una decisión explícita antes de navegar y bloquean publicación. Los estados de espera finalizan ante errores, sin anunciar éxito ni reintentar una escritura incierta. Las referencias revocadas se conservan con diagnóstico, sin sustituirlas automáticamente.

## Validacion visual

El recorrido reproducible abre el menú de Importación, busca por nombre/código, selecciona por teclado, limpia y reabre; luego revisa el detalle guiado, los campos dependientes y la presentación de versiones publicadas. Las corridas autenticadas --acceptance y --provider-scope verificaron los comportamientos en IIS; sus resultados están en la documentación de evidencia.

La revisión local final ejecutó 15 pruebas Chromium PASS: navegación, permisos adicionales, foco, autocomplete, formulario adaptable, detalle guiado, fuentes y proveedores con transporte simulado. Los cuatro casos autenticados omitidos en esa corrida local conservan evidencia previa propia; no se presentan como nuevas corridas reales. La prueba de disposición usa 1366×900 y ajustes de ventana declarados en la suite. No se atribuye una auditoría completa de accesibilidad a estas comprobaciones.

[Mapa de pruebas, comandos y resultados](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc110-verificacion-final-20261027.md). No se guardan capturas de login, tokens, cookies o documentos reales como evidencia del cambio.
