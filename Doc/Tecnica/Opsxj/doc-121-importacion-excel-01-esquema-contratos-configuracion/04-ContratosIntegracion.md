# IMPORTACION-EXCEL-01-ESQUEMA-CONTRATOS-CONFIGURACION

- Ticket: DOC-121
- Cambio OpenSpec: doc-121-importacion-excel-01-esquema-contratos-configuracion
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

Contrato normativo de fase: Doc/Actualizacion/Importacion/Diseno/01-contratos-configuracion-doc121.md. Especifica operaciones futuras AbrirCargaExcel, Listar/Consultar/GuardarPlantilla, RecibirArchivo, Preparar, Confirmar y ConsultarResultado. JSON por referencias y multipart para un archivo; envelope acorde a RespuestaPerfilImportacion, con identidad de carga propia y protección de solicitud separada del permiso de perfiles.

SQL manual: permiso RA por defecto 0; dos columnas al final de cabecera DA; una tabla InnoDB de ejecución con identidad, snapshot y hashes. Sin FK a bases distintas o catálogos legacy. Generador de propuestas de índice desde atributos actuales y plantilla de GRANT a completar por DBA en copia local. No hay ejecución automática, secretos, semillas de fuente o habilitación de usuarios.

Fuente legible no concede escritura. La cuenta dedicada se vincula en servidor al ámbito, DA e ID histórico autorizado. RA/DA lógicos se verifican en el ambiente, sin asumir un único esquema físico. El preflight actual debe preservar la ampliación de BENEFICIARIO; el inventario histórico no es autoridad para reducirlo.
