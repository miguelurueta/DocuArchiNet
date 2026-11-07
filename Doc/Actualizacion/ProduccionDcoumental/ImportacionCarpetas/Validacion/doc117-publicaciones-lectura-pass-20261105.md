# DOC-117 — Publicaciones verificadas por lectura

Resultado autenticado: **PASS**. Ambiente de pruebas local autorizado, nivel 7. Evidencia saneada: `tools/e2e/artifacts/doc117-publications-readonly-bc7e1e197446cf08.json`. No se almacenaron credenciales, cookies ni cadenas de conexión. No se ejecutaron preparaciones, confirmaciones, recepciones ni creación/almacenamiento documental.

| Perfil / versión | Reglas | Snapshot SHA256 leído |
| --- | --- | --- |
| 26/40 | Coinciden con el manifiesto | `dac2933f65ce8ad15dd3922eab5b1831d5495dcb9e21636350710fc014664b16` |
| 27/42 | Coinciden con el manifiesto | `b4474e42abb2d746c75fe12aa5e7eb359677b408e393e4cfad7313afbb99a494` |

Manifiesto lógico: `2006f16cc218074f4b2d930daceea480ad5d19dbb1ac888295f7d3d5451ff47f`. La comprobación conserva Orden y contenido de reglas al comparar las publicaciones por identidad. Estos hashes corresponden al snapshot observado en esta lectura; no convierten las recetas offline en una aceptación de almacenamiento final.

## Límites y siguiente paso

No se comprobó ausencia de raíces, vigencia de tipología/gabinete por SELECT ni almacenamiento/incorporación final. La autorización registrada es exclusivamente de lectura y no habilita carga, instalación de DLL o coexistencia. Sigue pendiente preparar y revisar el driver de raíz única, comprobar la ausencia de las dos raíces mediante controles SELECT y obtener autorización específica para los efectos ALM-01. Las tareas 15.1, 16.1, 17.1 y 18.1 permanecen abiertas.
