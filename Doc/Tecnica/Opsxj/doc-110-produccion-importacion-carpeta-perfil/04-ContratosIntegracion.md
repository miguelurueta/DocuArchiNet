# PRODUCCION-IMPORTACION-CARPETA-PERFIL

- Ticket: DOC-110
- Cambio OpenSpec: doc-110-produccion-importacion-carpeta-perfil
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

La fachada propia es `webservice/WebServiceImportacionCarpetas.asmx`, mediante POST JSON con sesión habilitada. El cliente envía `{solicitud: payload}` y cabecera `X-Importacion-Perfil-CSRF`; recibe el sobre ASMX `d` con `ok`, `codigo`, `mensaje`, `operacionId`, `datos` e `incidencias`. Las identidades persistidas viajan como String decimal canónico positivo; la referencia publicada opcional admite null. Revisiones de cabecera y versión se transportan separadamente donde corresponde.

Las familias de operaciones incluyen ListarPerfiles/ObtenerPerfil/GuardarBorrador/PublicarVersion/CrearVersion/ValidarSeleccionPerfil; ListarAdministracionProveedores/GuardarProveedor/ValidarProveedor/EnsayarProveedor/PublicarProveedor; y ListarFuentes/ObtenerFuente/GuardarFuente/ActivarFuente/RevocarFuente/HistorialFuente. El cliente contiene una lista cerrada de operaciones; la fachada vuelve a validar la forma del cuerpo y no acepta autoridad de identidad o conexión enviada por el navegador.

El contexto se resuelve desde la sesión de Gestión y la relación usuario/empresa en RA. Cada petición exige ADMINISTRAR_PERFILES_IMPORTACION; administrar proveedores o fuentes requiere además su facultad específica. El token queda vinculado a sesión, usuario y empresa. La revocación se refleja en la siguiente petición. IMPORTAR_CARPETAS y la selección operativa pertenecen a una entrega posterior: administrar no concede ejecutar importaciones.

RA contiene perfiles, proveedores y autorizaciones de fuentes; los catálogos documentales usan RA o DA según su contrato. Las conexiones lógicas se resuelven en servidor. Descubrimiento y uso se limitan al prefijo literal imp_02 o excepción exacta por conexión configurada en servidor; una excepción de nombre no concede autorización de fuente. No se aceptan credenciales, cadenas de conexión o SQL desde el cliente.

El esquema y permisos se entregan mediante SQL manual MySQL 5.1/InnoDB: instalación de perfiles 01, migración de identidades 03, proveedores 04/05 y fuentes 07, con controles y recuperación propios. No hay DDL automático en runtime. La falta de esquema informa indisponibilidad de la capacidad nueva; la carga individual preserva su contrato DOC-88. SII no se adapta al perfil ni se modifica para completar esta entrega.

Contratos detallados: `Diseno/07-contrato-http-seguridad-perfil.md`, `Diseno/09-proveedores-datos-indices.md`, `Diseno/10-catalogo-proveedores-tabla.md` y `Diseno/11-administracion-fuentes-da.md`, bajo `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`. [Evidencia y límites](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc110-verificacion-final-20261027.md). La aplicación externa de asignación de permisos queda fuera del proyecto; la prueba de revocación usó el cambio puntual/restauración expresamente autorizados, sin implementar integración externa.
