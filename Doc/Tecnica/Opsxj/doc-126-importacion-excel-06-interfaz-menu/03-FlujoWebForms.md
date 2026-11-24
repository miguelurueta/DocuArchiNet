# DOC-126 — Interfaz Excel y menú

- Ticket: DOC-126
- Cambio OpenSpec: doc-126-importacion-excel-06-interfaz-menu
- Clasificacion: webforms_ui (WebForms UI)
- Estado: refinamiento técnico; implementación y aceptación aún sin ejecutar.

## Flujo

GET renderiza host si la autoridad Excel vigente lo permite; POST AbrirCargaExcel emite CSRF y devuelve capacidad/conexiones. Listar/Consultar/GuardarPlantilla usa el CAS actual. IniciarRecepcion devuelve el ID antes de subir; FormData cumple el receptor existente. Preparar conserva datos de incidencias incluso con ok=false. Confirmar usa intención/hash congelados y consentimiento. ConsultarResultado recupera el resultado sin reimportar. Montaje repetido/postback parcial usa generación propia y conserva el menú común.

## Riesgos

El parser de perfiles no representa el envelope Excel. Emitir de la guarda Excel exige POST JSON y no se llama desde GET. La referencia contextual aleatoria no es autorización; storage guarda sólo ID/conexión y falla con aviso. Un timeout/close no demuestra rollback, y una consulta PREPARADA no habilita un replay automático. Una plantilla nueva con acuse perdido no se vuelve a crear automáticamente. No hay nuevas conexiones ni publicación administrativa de fuentes desde el diálogo.
