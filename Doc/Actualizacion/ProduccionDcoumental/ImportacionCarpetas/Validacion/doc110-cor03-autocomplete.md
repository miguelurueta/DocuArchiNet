# COR-03 — Autocompletado de perfiles

Implementado por autorización expresa del usuario. Tareas oficiales 11.1 y 11.2 completas; 11.3 mantiene pendiente la aceptación HTTP autenticada. Sin cambios de tablas, SII o carga individual. Publicación principal a cargo del usuario.

## Funcionamiento

El campo busca por código o nombre. Desde dos caracteres solicita hasta diez sugerencias después de 300 ms; muestra código — nombre como texto seguro. Permite selección con teclado o ratón y filtra por el código único del ámbito. Buscar aplica el texto libre; Limpiar elimina filtros y vuelve a página 1. Escape cierra primero el menú y después el diálogo. Cada reconstrucción destruye el widget y sus temporizadores; respuestas antiguas no sustituyen búsquedas nuevas.

Se reutiliza jQuery UI 1.8.22 ya cargado y PerfilImportacionApi. ListarPerfiles incorpora termino opcional, máximo 160 caracteres. Repository usa LOCATE parametrizado sobre codigo OR nombre y mantiene ambito_ref, paginación y filtros anteriores. La comparación conserva las reglas de collation actuales. No se descargan todos los perfiles ni se limita la búsqueda al contenido visible de una página.

## Verificación

- Compilación Debug del proyecto WebForms real PASS, salida aislada .tmp/doc110-cor03-build. DLL anterior conservado en .tmp/doc110-before-cor03.dll y nuevo DLL copiado al bin local para revisión del usuario.
- Ocho pruebas Node PASS: behavior (VB/C# Framework), client y editor. Incluyen rechazo de término excesivo, contexto no autorizado, búsqueda limitada, respuestas fuera de orden, render sin HTML, selección, limpieza y destrucción del menú usando la biblioteca real.
- Playwright en navegador real: doc110-perfil-autocomplete.spec.cjs PASS; teclado, Enter, Escape, limpieza, reapertura y ausencia de coincidencias. Usa transporte simulado y no acredita autenticación ni ASMX.
- Consulta real del Repository local mediante SELECT: 36 comprobaciones PASS, evidencia doc110-cor03-consulta.json. Búsqueda de fragmentos de código/nombre, tratamiento literal de comillas/%/_, ámbito de los resultados y filtro exacto anterior. No acredita aislamiento con dos sesiones autenticadas.
- Suite autenticada existente ampliada con AUTOCOMPLETE_CODE_NAME_SELECTION_CLEAR, consultas por fragmento de código/nombre, selección y limpieza. Pendiente corrida autorizada; si no hay perfil de referencia registra NOT_RUN en vez de PASS.
- OpenSpec estricto PASS. Ningún resultado implica despliegue al servidor principal.

Comandos: node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc110-perfil-importacion-editor.test.cjs tests/doc110-perfil-importacion-client.test.cjs; desde tools/e2e: node_modules/.bin/playwright.cmd test tests/doc110-perfil-autocomplete.spec.cjs tests/doc110-perfil-importacion-anonymous.spec.cjs --workers=1 --reporter=list.

Resultado conjunto posterior al DLL local: 14/14 pruebas Playwright PASS en 14.9 s (1 interfaz con transporte simulado y 13 login/rechazo anónimo real).
